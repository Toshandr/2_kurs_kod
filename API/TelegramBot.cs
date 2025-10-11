using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using API.Models;

public class Bot
{
    private readonly TelegramBotClient _botClient;
    private readonly Dictionary<long, UserData> _userStates;

    public Bot()
    {
        _botClient = new TelegramBotClient("8335329999:AAFCHSE7KHsAWXQ8rJhgcNE6sarCMqo8ix8");
        _userStates = new Dictionary<long, UserData>();
    }

    public async Task StartBotAsync(CancellationToken cancellationToken = default)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken
        );

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"Бот @{me.Username} запущен!");

        await Task.Delay(-1, cancellationToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        var chatId = message.Chat.Id;
        var user = message.From;

        if (message.Text == "/start")
        {
            string firstName = user?.FirstName ?? "Не указано";
            string lastName = user?.LastName ?? "";
            string username = user?.Username != null ? $"@{user.Username}" : "Не указан";

            _userStates[chatId] = new UserData
            {
                FirstName = firstName,
                LastName = lastName,
                Username = username,
                IsWaitingForAge = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"✅ Ваши данные:\n👤 Имя: {firstName}\n📛 Никнейм: {username}\n\n📅 Введите ваш возраст:",
                cancellationToken: cancellationToken);
            return;
        }

        if (_userStates.ContainsKey(chatId))
        {
            var userData = _userStates[chatId];

            if (userData.IsWaitingForAge)
            {
                if (int.TryParse(message.Text, out int age) && age > 0 && age < 120)
                {
                    userData.Age = age;
                    userData.IsWaitingForAge = false;
                    userData.IsWaitingForCity = true;

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "🏙️ Теперь введите ваш город:",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Пожалуйста, введите корректный возраст (число от 1 до 120):",
                        cancellationToken: cancellationToken);
                }
                return;
            }

            if (userData.IsWaitingForCity)
            {
                string city = message.Text;
                userData.City = city;
                userData.IsWaitingForCity = false;

                // Сохраняем пользователя напрямую в базу данных
                bool registrationSuccess = await RegisterUserInDatabase(userData);

                if (registrationSuccess)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"✅ Регистрация завершена!\n\n📋 Ваши данные:\n👤 Имя: {userData.FirstName}\n📛 Никнейм: {userData.Username}\n📅 Возраст: {userData.Age}\n🏙️ Город: {userData.City}",
                        cancellationToken: cancellationToken);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Ошибка при регистрации. Попробуйте позже.",
                        cancellationToken: cancellationToken);
                }

                _userStates.Remove(chatId);
            }
        }
    }

    private async Task<bool> RegisterUserInDatabase(UserData userData)
    {
        try
        {
            using (BaseContext db = new BaseContext())
            {
                // Формируем полное имя
                string fullName = userData.FirstName;
                if (!string.IsNullOrEmpty(userData.LastName))
                {
                    fullName += " " + userData.LastName;
                }

                var newUser = new API.Models.User
                {
                    Name = fullName,
                    Age = userData.Age,
                    TelegramTeg = userData.Username,
                    CityNow = userData.City,
                    CityLater = userData.City
                };

                db.Users.Add(newUser);
                await db.SaveChangesAsync();

                Console.WriteLine($"Пользователь {fullName} успешно сохранен в БД с ID: {newUser.Id}");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении в БД: {ex.Message}");
            return false;
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка бота: {exception.Message}");
        return Task.CompletedTask;
    }
}
public class UserData
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Age { get; set; } = 0;
    public bool IsWaitingForAge { get; set; } = false;
    public bool IsWaitingForCity { get; set; } = false;
}
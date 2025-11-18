using System.Linq;
using API.Models;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

public class Bot
{
    private static readonly Lazy<TelegramBotClient> _lazyBotClient = new Lazy<TelegramBotClient>(() =>
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        
        // Fallback to appsettings.json if environment variable is not set
        if (string.IsNullOrEmpty(token))
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            
            token = configuration["Token"] ?? configuration["TelegramBot:Token"];
        }
        
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("TELEGRAM_BOT_TOKEN environment variable or Token in appsettings.json is not set");
        }
        
        return new TelegramBotClient(token);
    });
    
    public static TelegramBotClient _botClient => _lazyBotClient.Value;
    private readonly Dictionary<long, UserData> _userStates;

    public Bot()
    {
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

            _userStates[chatId] = new UserData
            {
                FirstName = firstName,
                LastName = lastName,
                ChatId = chatId, // Сохраняем chatId вместо username
                IsWaitingForAge = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"✅ Ваши данные:\n👤 Имя: {firstName}\n📛 ID: {chatId}\n\n📅 Введите ваш возраст:",
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
                        text: $"✅ Регистрация завершена!\n\n📋 Ваши данные:\n👤 Имя: {userData.FirstName}\n📛 ID: {userData.ChatId}\n📅 Возраст: {userData.Age}\n🏙️ Город: {userData.City}\n👤 Роль: guest",
                        cancellationToken: cancellationToken);

                    // Здесь можно вызвать отправку сообщения "ЛОХ" всем новым пользователям
                    //List<string> test = ["1331310743"];
                   // NotificationService notify = new NotificationService(_botClient);
                    //await notify.SendLoxMessageAsync(test);
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "❌ Ошибка при регистрации. Пользователь с таким ID уже существует.",
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
            Console.WriteLine($"Попытка сохранения пользователя: {userData.FirstName}, ChatId: {userData.ChatId}");
            using (BaseContext db = new BaseContext())
            {
                // Проверяем подключение
                if (!db.Database.CanConnect())
                {
                    Console.WriteLine("ОШИБКА: Не удалось подключиться к БД при регистрации пользователя!");
                    return false;
                }

                // Формируем полное имя
                string fullName = userData.FirstName;
                if (!string.IsNullOrEmpty(userData.LastName))
                {
                    fullName += " " + userData.LastName;
                }

                // Проверяем, есть ли уже пользователь с таким ChatId
                var existingUser = db.Users.FirstOrDefault(u => u.TelegramTeg == userData.ChatId.ToString());
                if (existingUser != null)
                {
                    Console.WriteLine($"Пользователь с ChatId {userData.ChatId} уже существует в БД (ID: {existingUser.Id})");
                    return false;
                }

                var newUser = new API.Models.User
                {
                    Name = fullName,
                    Age = userData.Age,
                    TelegramTeg = userData.ChatId.ToString(), // Сохраняем chatId как строку
                    CityNow = userData.City,
                    CityLater = userData.City,
                    Role = "guest" // Устанавливаем роль guest по умолчанию
                };

                db.Users.Add(newUser);
                int savedCount = await db.SaveChangesAsync();

                Console.WriteLine($"Пользователь {fullName} успешно сохранен в БД с ID: {newUser.Id} (сохранено записей: {savedCount})");
                
                // Проверяем, что пользователь действительно сохранился
                var verifyUser = db.Users.FirstOrDefault(u => u.Id == newUser.Id);
                if (verifyUser != null)
                {
                    Console.WriteLine($"Проверка: пользователь найден в БД - ID: {verifyUser.Id}, Name: {verifyUser.Name}");
                }
                else
                {
                    Console.WriteLine("ПРЕДУПРЕЖДЕНИЕ: Пользователь не найден после сохранения!");
                }
                
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении в БД: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
            }
            return false;
        }
    }

 
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка бота: {exception.Message}");
        return Task.CompletedTask;
    }

    public class UserData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public long ChatId { get; set; } // Изменено с Username на ChatId
        public string City { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
        public bool IsWaitingForAge { get; set; } = false;
        public bool IsWaitingForCity { get; set; } = false;
    }
}
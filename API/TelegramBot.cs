using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
            string username = user?.Username != null ? $"@{user.Username}" : "Не указан";

            _userStates[chatId] = new UserData
            {
                FirstName = firstName,
                Username = username,
                IsWaitingForCity = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"✅ Ваши данные:\n👤 Имя: {firstName}\n📛 Никнейм: {username}\n\n🏙️ Введите ваш город:",
                cancellationToken: cancellationToken);
            return;
        }

        if (_userStates.ContainsKey(chatId) && _userStates[chatId].IsWaitingForCity)
        {
            string city = message.Text;
            var userData = _userStates[chatId];
            userData.City = city;
            userData.IsWaitingForCity = false;

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"✅ Все данные:\n👤 Имя: {userData.FirstName}\n📛 Никнейм: {userData.Username}\n🏙️ Город: {userData.City}",
                cancellationToken: cancellationToken);

            _userStates.Remove(chatId);
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
    public bool IsWaitingForCity { get; set; } = false;
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

public class NotificationService
{
    private readonly ITelegramBotClient _botClient;
    
    // Конструктор принимает уже инициализированного бота
    public NotificationService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendMessageToCityAsync(string city, string text)
    {
        var tasks = BD.SearchByCity(city);
        
        foreach (var user in tasks)
        {
            // Асинхронная отправка сообщения каждому пользователю
            _ = SendMessageToUserAsync(user.TelegramTeg, text);
        }
    }

    public async Task SendMessageToUserAsync(string chatId, string message)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);
        }
        catch (Exception ex)
        {
            // Логирование ошибок (пользователь заблокировал бота и т.д.)
            Console.WriteLine($"Не удалось отправить сообщение {chatId}: {ex.Message}");
        }
    }
}

// Модель пользователя Telegram (предполагается, что уже существует)

// Пример использования:
// var notificationService = new NotificationService(botClient);
// await notificationService.SendLoxMessageAsync(usersList);
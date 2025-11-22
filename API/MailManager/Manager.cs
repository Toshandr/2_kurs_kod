using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

using API.Models;

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
        catch (Exception)
        {
            // Логирование ошибок (пользователь заблокировал бота и т.д.)
            using(BaseContext db = new BaseContext())
            {
                var i = db.Users.FirstOrDefault(i => i.TelegramTeg == chatId);
                db.Users.Remove(i);
                await db.SaveChangesAsync();
                Console.WriteLine($"Пользователь: {i.Name} с таким чатом ID: {chatId} был удален так как пользователь заблокировал бота или удалил чат.");
            }
        }
    }
}

// Модель пользователя Telegram (предполагается, что уже существует)

// Пример использования:
// var notificationService = new NotificationService(botClient);
// await notificationService.SendLoxMessageAsync(usersList);
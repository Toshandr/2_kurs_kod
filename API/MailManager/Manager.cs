using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

using API.Models;
using API.FileSys;

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
        var users = BD.SearchByCity(city);
        
        if (users == null || users.Count == 0)
        {
            Console.WriteLine($"[NotificationService] Пользователи в городе {city} не найдены");
            return;
        }
        
        foreach (var user in users)
        {
            // Асинхронная отправка сообщения каждому пользователю
            _ = SendMessageToUserAsync(user.TelegramTeg, text, user.Name);
        }
    }

    public async Task SendMessageToUserAsync(string chatId, string message, string? userName = null)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);
            
            // Логируем отправленное сообщение
            if (long.TryParse(chatId, out long chatIdLong))
            {
                // Если имя не передано, пытаемся получить из БД
                if (string.IsNullOrEmpty(userName))
                {
                    using (BaseContext db = new BaseContext())
                    {
                        var user = db.Users.FirstOrDefault(u => u.TelegramTeg == chatId);
                        userName = user?.Name ?? "Unknown";
                    }
                }
                
                FileUser.LogUserMessage(chatIdLong, userName, $"Исходящее (от админа): {message}");
                Console.WriteLine($"[NotificationService] Сообщение отправлено пользователю {userName} (ChatId: {chatId})");
            }
        }
        catch (Exception ex)
        {
            // Логирование ошибок (пользователь заблокировал бота и т.д.)
            FileSystem.LogError($"Ошибка отправки сообщения пользователю с ChatId: {chatId}", ex);
            
            using(BaseContext db = new BaseContext())
            {
                var i = db.Users.FirstOrDefault(i => i.TelegramTeg == chatId);
                if (i != null)
                {
                    db.Users.Remove(i);
                    await db.SaveChangesAsync();
                    Console.WriteLine($"Пользователь: {i.Name} с таким чатом ID: {chatId} был удален так как пользователь заблокировал бота или удалил чат.");
                    FileSystem.LogInfo($"Пользователь {i.Name} (ChatId: {chatId}) удален из БД - бот заблокирован");
                }
            }
        }
    }
}

// Модель пользователя Telegram (предполагается, что уже существует)

// Пример использования:
// var notificationService = new NotificationService(botClient);
// await notificationService.SendLoxMessageAsync(usersList);
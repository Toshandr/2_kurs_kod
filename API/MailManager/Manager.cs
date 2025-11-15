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

    public async Task SendLoxMessageAsync(List<string> users)
    {
        var tasks = new List<Task>();
        
        foreach (var user in users)
        {
            // Асинхронная отправка сообщения каждому пользователю
            var task = SendMessageToUserAsync(Convert.ToString(user), "Мишка Фредди знает, что ты не готовился к коллоквиуму");
            tasks.Add(task);
        }

        // Ожидаем завершения всех отправок
        await Task.WhenAll(tasks);
    }

    private async Task SendMessageToUserAsync(string chatId, string message)
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
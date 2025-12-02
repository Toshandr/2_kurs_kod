namespace API.Handlers;

public static class NotificationsHandler
{
    public static async Task SendMessageToUser(int userId, string message)
    {
        var telegramTag = BD.SearchUser(userId);
        
        if (telegramTag == null)
        {
            throw new Exception($"Пользователь с ID {userId} не найден");
        }

        var notificationService = new NotificationService(Bot._botClient);
        await notificationService.SendMessageToUserAsync(telegramTag, message);
    }

    public static async Task SendMessageToCity(string city, string message)
    {
        var notificationService = new NotificationService(Bot._botClient);
        await notificationService.SendMessageToCityAsync(city, message);
    }
}

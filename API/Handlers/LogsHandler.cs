using System.Text;

namespace API.Handlers;

public static class LogsHandler
{
    private static readonly string BaseDirectory = GetBaseDirectory();
    private static readonly string SystemLogPath = Path.Combine(BaseDirectory, "logs", "System", "System.txt");
    private static readonly string UsersLogDirectory = Path.Combine(BaseDirectory, "logs", "Users");

    private static string GetBaseDirectory()
    {
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        
        if (currentDir.StartsWith("/app"))
        {
            return "/app";
        }
        else
        {
            var apiDir = currentDir;
            while (!string.IsNullOrEmpty(apiDir) && !Directory.Exists(Path.Combine(apiDir, "API")))
            {
                apiDir = Path.GetDirectoryName(apiDir);
            }
            
            if (!string.IsNullOrEmpty(apiDir) && Directory.Exists(Path.Combine(apiDir, "API")))
            {
                return Path.Combine(apiDir, "API");
            }
            
            return currentDir;
        }
    }

    public static string GetSystemLog()
    {
        try
        {
            if (File.Exists(SystemLogPath))
            {
                return File.ReadAllText(SystemLogPath, Encoding.UTF8);
            }
            return "Системный лог пуст";
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка чтения системного лога", ex);
            throw;
        }
    }

    public static List<UserLogInfo> GetUserLogsList()
    {
        try
        {
            var userLogs = new List<UserLogInfo>();

            if (!Directory.Exists(UsersLogDirectory))
            {
                return userLogs;
            }

            var files = Directory.GetFiles(UsersLogDirectory, "*.txt");

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var parts = fileName.Split('_', 2);
                
                if (parts.Length == 2)
                {
                    userLogs.Add(new UserLogInfo
                    {
                        ChatId = parts[0],
                        UserName = parts[1],
                        FileName = Path.GetFileName(file)
                    });
                }
            }

            return userLogs;
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка получения списка логов пользователей", ex);
            throw;
        }
    }

    public static string GetUserLog(string fileName)
    {
        try
        {
            var filePath = Path.Combine(UsersLogDirectory, fileName);
            
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            
            return "Лог пользователя не найден";
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError($"Ошибка чтения лога пользователя {fileName}", ex);
            throw;
        }
    }
}

public class UserLogInfo
{
    public string ChatId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}

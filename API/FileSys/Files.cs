using System.Text;

namespace API.FileSys;

/// <summary>
/// Класс для логирования системных ошибок в файл System.txt
/// </summary>
public sealed class FileSystem
{
    // Определяем базовую директорию в зависимости от окружения
    private static readonly string BaseDirectory = GetBaseDirectory();
    private static readonly string LogDirectory = Path.Combine(BaseDirectory, "logs", "System");
    private static readonly string LogFilePath = Path.Combine(LogDirectory, "System.txt");
    private static readonly object _lock = new object();

    private static string GetBaseDirectory()
    {
        // Проверяем, запущено ли приложение в Docker (путь начинается с /app)
        var currentDir = AppDomain.CurrentDomain.BaseDirectory;
        
        if (currentDir.StartsWith("/app"))
        {
            // В Docker используем /app/logs
            return "/app";
        }
        else
        {
            // Локально - ищем папку API в структуре проекта
            var apiDir = currentDir;
            while (!string.IsNullOrEmpty(apiDir) && !Directory.Exists(Path.Combine(apiDir, "API")))
            {
                apiDir = Path.GetDirectoryName(apiDir);
            }
            
            if (!string.IsNullOrEmpty(apiDir) && Directory.Exists(Path.Combine(apiDir, "API")))
            {
                return Path.Combine(apiDir, "API");
            }
            
            // Если не нашли, используем текущую директорию
            return currentDir;
        }
    }

    // Публичный метод для использования в FileUser
    public static string GetBaseDirectoryPublic() => GetBaseDirectory();

    /// <summary>
    /// Записывает ошибку в системный лог
    /// </summary>
    public static void LogError(string errorMessage, Exception? exception = null)
    {
        try
        {
            // Создаем директорию, если её нет
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
                Console.WriteLine($"[FileSystem] Создана директория: {LogDirectory}");
            }

            var logEntry = new StringBuilder();
            logEntry.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR:");
            logEntry.AppendLine(errorMessage);
            
            if (exception != null)
            {
                logEntry.AppendLine($"Exception: {exception.Message}");
                logEntry.AppendLine($"StackTrace: {exception.StackTrace}");
                
                if (exception.InnerException != null)
                {
                    logEntry.AppendLine($"InnerException: {exception.InnerException.Message}");
                }
            }
            
            logEntry.AppendLine(new string('-', 80));

            // Потокобезопасная запись в файл
            lock (_lock)
            {
                File.AppendAllText(LogFilePath, logEntry.ToString());
            }
        }
        catch (Exception ex)
        {
            // Если не можем записать в файл, выводим в консоль
            Console.WriteLine($"[FileSystem] Ошибка записи в лог-файл: {ex.Message}");
        }
    }

    /// <summary>
    /// Записывает информационное сообщение в системный лог
    /// </summary>
    public static void LogInfo(string message)
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}\n";

            lock (_lock)
            {
                File.AppendAllText(LogFilePath, logEntry);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка записи в лог-файл: {ex.Message}");
        }
    }
}

/// <summary>
/// Класс для сохранения истории чата пользователей
/// </summary>
public sealed class FileUser
{
    // Используем ту же базовую директорию, что и FileSystem
    private static readonly string BaseDirectory = FileSystem.GetBaseDirectoryPublic();
    private static readonly string LogDirectory = Path.Combine(BaseDirectory, "logs", "Users");
    private static readonly object _lock = new object();

    /// <summary>
    /// Создает или обновляет файл истории чата пользователя
    /// </summary>
    /// <param name="chatId">ID чата пользователя</param>
    /// <param name="userName">ФИО пользователя</param>
    /// <param name="message">Сообщение</param>
    public static void LogUserMessage(long chatId, string userName, string message)
    {
        try
        {
            // Создаем директорию, если её нет
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
                Console.WriteLine($"[FileUser] Создана директория: {LogDirectory}");
            }

            // Формируем имя файла: chatId_ФИО.txt
            string sanitizedUserName = SanitizeFileName(userName);
            string fileName = $"{chatId}_{sanitizedUserName}.txt";
            string filePath = Path.Combine(LogDirectory, fileName);

            // Если файл не существует, создаем его с базовой информацией
            lock (_lock)
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[FileUser] Файл не существует, создаю новый: {filePath}");
                    
                    var initialContent = new StringBuilder();
                    initialContent.AppendLine($"=== История чата пользователя ===");
                    initialContent.AppendLine($"Имя: {userName}");
                    initialContent.AppendLine($"Chat ID: {chatId}");
                    initialContent.AppendLine($"Дата создания файла: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    initialContent.AppendLine($"Примечание: Файл создан автоматически при первом сообщении");
                    initialContent.AppendLine(new string('=', 50));
                    initialContent.AppendLine();
                    
                    File.WriteAllText(filePath, initialContent.ToString());
                    Console.WriteLine($"[FileUser] ✓ Файл успешно создан для {userName}");
                    FileSystem.LogInfo($"Автоматически создан файл истории для пользователя {userName} (ChatId: {chatId})");
                }

                // Формируем запись с временем
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
                File.AppendAllText(filePath, logEntry);
            }

            FileSystem.LogInfo($"Сообщение пользователя {userName} (ChatId: {chatId}) записано в файл");
        }
        catch (Exception ex)
        {
            FileSystem.LogError($"Ошибка записи сообщения пользователя {userName} (ChatId: {chatId})", ex);
            Console.WriteLine($"[FileUser] ✗ Ошибка записи в файл пользователя: {ex.Message}");
        }
    }

    /// <summary>
    /// Создает файл для нового пользователя с начальной информацией
    /// </summary>
    public static void CreateUserFile(long chatId, string userName, int age, string city)
    {
        try
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
                Console.WriteLine($"[FileUser] Создана директория: {LogDirectory}");
            }

            string sanitizedUserName = SanitizeFileName(userName);
            string fileName = $"{chatId}_{sanitizedUserName}.txt";
            string filePath = Path.Combine(LogDirectory, fileName);

            Console.WriteLine($"[FileUser] Создание файла: {filePath}");

            var initialContent = new StringBuilder();
            initialContent.AppendLine($"=== История чата пользователя ===");
            initialContent.AppendLine($"Имя: {userName}");
            initialContent.AppendLine($"Chat ID: {chatId}");
            initialContent.AppendLine($"Возраст: {age}");
            initialContent.AppendLine($"Город: {city}");
            initialContent.AppendLine($"Дата регистрации: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            initialContent.AppendLine(new string('=', 50));
            initialContent.AppendLine();

            lock (_lock)
            {
                File.WriteAllText(filePath, initialContent.ToString());
            }

            Console.WriteLine($"[FileUser] ✓ Файл успешно создан для {userName}");
            FileSystem.LogInfo($"Создан файл истории для пользователя {userName} (ChatId: {chatId})");
        }
        catch (Exception ex)
        {
            FileSystem.LogError($"Ошибка создания файла для пользователя {userName} (ChatId: {chatId})", ex);
            Console.WriteLine($"[FileUser] ✗ Ошибка создания файла пользователя: {ex.Message}");
        }
    }

    /// <summary>
    /// Очищает имя файла от недопустимых символов
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        return sanitized.Trim();
    }
}
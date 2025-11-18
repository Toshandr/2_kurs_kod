using System.Linq;
using System.Data;
using System.Data.Common;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using API.Models;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Применяем миграции при запуске и проверяем подключение
// Ждем немного, чтобы БД точно была готова
await Task.Delay(2000);

int maxRetries = 5;
int retryDelay = 3000; // 3 секунды

for (int attempt = 1; attempt <= maxRetries; attempt++)
{
    try
    {
        Console.WriteLine($"Попытка подключения к базе данных (попытка {attempt}/{maxRetries})...");
        using (var dbContext = new BaseContext())
        {
            // Проверяем, можем ли мы подключиться к БД
            if (dbContext.Database.CanConnect())
            {
                Console.WriteLine("Подключение к БД успешно!");
                
                // Проверяем, какие миграции уже применены
                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                var appliedMigrations = dbContext.Database.GetAppliedMigrations().ToList();
                
                Console.WriteLine($"Примененных миграций: {appliedMigrations.Count}");
                if (appliedMigrations.Any())
                {
                    Console.WriteLine("Примененные миграции:");
                    foreach (var migration in appliedMigrations)
                    {
                        Console.WriteLine($"  - {migration}");
                    }
                }
                
                Console.WriteLine($"Ожидающих миграций: {pendingMigrations.Count}");
                if (pendingMigrations.Any())
                {
                    Console.WriteLine("Ожидающие миграции:");
                    foreach (var migration in pendingMigrations)
                    {
                        Console.WriteLine($"  - {migration}");
                    }
                    
                    // Применяем миграции
                    Console.WriteLine("Применение миграций...");
                    try
                    {
                        dbContext.Database.Migrate();
                        Console.WriteLine("Миграции базы данных применены успешно");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                        }
                        // Если ошибка "relation already exists", значит таблица была создана ранее
                        // Помечаем миграцию как примененную вручную
                        if (ex.Message.Contains("already exists") || ex.InnerException?.Message.Contains("already exists") == true)
                        {
                            Console.WriteLine("Обнаружена существующая таблица. Помечаем миграции как примененные...");
                            try
                            {
                                var connection = dbContext.Database.GetDbConnection();
                                var wasOpen = connection.State == System.Data.ConnectionState.Open;
                                if (!wasOpen)
                                {
                                    await connection.OpenAsync();
                                }
                                
                                using var command = connection.CreateCommand();
                                
                                // Создаем таблицу истории миграций, если её нет
                                command.CommandText = @"
                                    CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                                        ""MigrationId"" VARCHAR(150) NOT NULL PRIMARY KEY,
                                        ""ProductVersion"" VARCHAR(32) NOT NULL
                                    )";
                                await command.ExecuteNonQueryAsync();
                                
                                // Помечаем все ожидающие миграции как примененные
                                foreach (var migration in pendingMigrations)
                                {
                                    command.CommandText = $@"
                                        INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                                        VALUES ('{migration}', '9.0.9')
                                        ON CONFLICT (""MigrationId"") DO NOTHING";
                                    await command.ExecuteNonQueryAsync();
                                    Console.WriteLine($"Миграция {migration} помечена как примененная");
                                }
                                
                                if (!wasOpen && connection.State == System.Data.ConnectionState.Open)
                                {
                                    await connection.CloseAsync();
                                }
                                
                                Console.WriteLine("Миграции успешно помечены как примененные");
                            }
                            catch (Exception markEx)
                            {
                                Console.WriteLine($"Ошибка при пометке миграций: {markEx.Message}");
                                throw;
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Все миграции уже применены");
                }
                
                // Проверяем, что таблица users существует и сколько в ней записей
                var userCount = dbContext.Users.Count();
                Console.WriteLine($"В базе данных сейчас {userCount} пользователей");
                
                if (userCount > 0)
                {
                    Console.WriteLine("Пользователи в БД:");
                    var users = dbContext.Users.Take(10).ToList();
                    foreach (var user in users)
                    {
                        Console.WriteLine($"  - ID: {user.Id}, Name: {user.Name}, TelegramTeg: {user.TelegramTeg}");
                    }
                }
                
                break; // Успешно, выходим из цикла
            }
            else
            {
                Console.WriteLine($"Не удалось подключиться к БД (попытка {attempt}/{maxRetries})");
                if (attempt < maxRetries)
                {
                    Console.WriteLine($"Ожидание {retryDelay / 1000} секунд перед следующей попыткой...");
                    await Task.Delay(retryDelay);
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при попытке {attempt}/{maxRetries}: {ex.Message}");
        if (attempt < maxRetries)
        {
            Console.WriteLine($"Ожидание {retryDelay / 1000} секунд перед следующей попыткой...");
            await Task.Delay(retryDelay);
        }
        else
        {
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"InnerException: {ex.InnerException.Message}");
            }
        }
    }
}


app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

var botik = new Bot();
var cancellationTokenSource = new CancellationTokenSource();


var botTask = Task.Run(() => botik.StartBotAsync(cancellationTokenSource.Token));

Console.WriteLine("Запускаем бота и Web API...");
var menu = new Menu();
Menu.MainMenu();
await app.RunAsync();

cancellationTokenSource.Cancel();

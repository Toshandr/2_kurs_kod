using API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


class Menu
{
    public static void MainMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Регионы");
        Console.WriteLine("2. Пользователи");
        Console.WriteLine("3. Отправить предупреждение");
        Console.WriteLine("4. Завершить работу");

        if (!int.TryParse(Console.ReadLine(), out int tab))
        {
            Console.WriteLine("Некорректный ввод!"); 
            MainMenu();
            return;
        }

        switch (tab)
        {
            case 1:
                RegionsMenu();
                break;

            case 2:
                UsersMenu();
                break;

            case 3:
                SendWarningMenu();
                break;

            case 4:
                Console.WriteLine("Завершение...");
                Environment.Exit(0);
                break;

            default:
                Console.WriteLine("Нет такого пункта.");
                MainMenu();
                break;
        }
    }

    // ----------------------------------------------------------
    //       МЕНЮ РЕГИОНОВ
    // ----------------------------------------------------------
    private static void RegionsMenu()
    {
        Console.Clear();

        Dictionary<string, int> cities_users = [];

        using (BaseContext db = new BaseContext())
        {
            var cityUserCount = db.Users
                .GroupBy(u => u.CityNow)
                .Select(g => new
                {
                    City = g.Key,
                    UserCount = g.Count()
                })
                .OrderByDescending(u => u.UserCount)
                .ToDictionary(x => x.City, x => x.UserCount);

            cities_users = cityUserCount;

            Console.WriteLine("Пользователи по городам:");
            foreach (var stat in cityUserCount)
            {
                Console.WriteLine($"{stat.Key}: {stat.Value}");
            }
        }

        Console.WriteLine("\n1. Просмотреть пользователей города");
        Console.WriteLine("2. Отправить предупреждение в город");
        Console.WriteLine("3. Назад");

        if (!int.TryParse(Console.ReadLine(), out int regions_tab))
        {
            RegionsMenu();
            return;
        }

        switch (regions_tab)
        {
            case 1:
                ShowUsersInCity();
                break;

            case 2:
                SendWarningToCity();
                break;

            case 3:
                MainMenu();
                break;

            default:
                RegionsMenu();
                break;
        }
    }

    // ----------------------------------------------------------
    //       ПОКАЗ ПОЛЬЗОВАТЕЛЕЙ ГОРОДА
    // ----------------------------------------------------------
    private static void ShowUsersInCity()
    {
        Console.Write("Введите название города: ");
        string? city = Console.ReadLine()?.Trim();

        var users_in_city = BD.SearchByCity(city);

        Console.WriteLine("ID   Имя   Возраст   Роль");
        Console.WriteLine("-------------------------------------------");

        if (users_in_city != null && users_in_city.Count() > 0)
        {
            foreach (var stat in users_in_city)
            {
                Console.WriteLine(
                    $"ID: {stat.Id}\n" +
                    $"USERNAME: {stat.Name}\n" +
                    $"AGE: {stat.Age}\n" +
                    $"ROLE: {stat.Role}\n" +
                    "-------------------------------------------");
            }
        }
        else
        {
            Console.WriteLine("Такого региона нет или там нет пользователей.");
        }

        Console.WriteLine("\nНажмите Enter чтобы вернуться...");
        Console.ReadLine();
        RegionsMenu();
    }

    // ----------------------------------------------------------
    //       ОТПРАВКА ПРЕДУПРЕЖДЕНИЯ В ГОРОД
    // ----------------------------------------------------------
    private static void SendWarningToCity()
    {
        Console.Write("Введите название города: ");
        string? city = Console.ReadLine()?.Trim();

        Console.Write("Введите текст предупреждения: ");
        string message = Console.ReadLine();

        NotificationService notify = new NotificationService(Bot._botClient);
        _ = notify.SendMessageToCityAsync(city, message);

        Console.WriteLine($"\nСообщение отправлено");
        Console.WriteLine("Нажмите Enter чтобы вернуться...");
        Console.ReadLine();

        RegionsMenu();
    }

    // ----------------------------------------------------------
    //       МЕНЮ ПОЛЬЗОВАТЕЛЕЙ
    // ----------------------------------------------------------
    private static void UsersMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Показать всех");
        Console.WriteLine("2. Отправить сообщение пользователю");
        Console.WriteLine("3. Назад");

        if (!int.TryParse(Console.ReadLine(), out int t)) 
        { 
            UsersMenu(); 
            return; 
        }

        switch (t)
        {
            case 1:
                ShowAllUsers();
                break;

            case 2:
                SendWarningToUser();
                break;

            case 3:
                MainMenu();
                break;
        }
    }

    private static void ShowAllUsers()
    {
        using (BaseContext db = new BaseContext())
        {
            var all = db.Users.ToList();

            foreach (var u in all)
            {
                Console.WriteLine($"[{u.Id}] {u.Name} ({u.CityNow})");
            }
        }

        Console.WriteLine("\nНажмите Enter...");
        Console.ReadLine();
        UsersMenu();
    }

    // ----------------------------------------------------------
    //       ОТПРАВКА ПОЛЬЗОВАТЕЛЮ
    // ----------------------------------------------------------
    private static void SendWarningToUser()
    {
        Console.Write("Введите ID пользователя: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            UsersMenu();
            return;
        }

        Console.Write("Введите сообщение: ");
        string msg = Console.ReadLine();

        NotificationService notify = new NotificationService(Bot._botClient);
        string? us_id = BD.SearchUser(id);
        if(us_id != null){
            _ = notify.SendMessageToUserAsync(us_id, msg);

            Console.WriteLine("Сообщение отправлено.");
            Console.WriteLine("Нажмите Enter...");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("Пользователь не найден.");
            Console.WriteLine("Нажмите Enter...");
            Console.ReadLine();
        }
        UsersMenu();
    }

    // ----------------------------------------------------------
    //       ОБЩЕЕ МЕНЮ ОТПРАВКИ (3 пункт)
    // ----------------------------------------------------------
    private static void SendWarningMenu()
    {
        Console.Clear();
        Console.WriteLine("1. Отправить пользователю");
        Console.WriteLine("2. Отправить в город");
        Console.WriteLine("3. Назад");

        if (!int.TryParse(Console.ReadLine(), out int t)) 
        { 
            SendWarningMenu(); 
            return; 
        }

        switch (t)
        {
            case 1:
                SendWarningToUser();
                break;

            case 2:
                SendWarningToCity();
                break;

            case 3:
                MainMenu();
                break;
        }
    }
}

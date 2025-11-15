using API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

class Menu
{
    public static void MainMenu()
    {
        Console.WriteLine("1. Регионы");
        Console.WriteLine("2. Пользователи");
        Console.WriteLine("3. Отправить предупреждение");
        Console.WriteLine("4. Завершить работу");

        var tab = int.Parse(Console.ReadLine());

        switch (tab)
        {
            case 1:
                Dictionary<string, int> cities_users = [];
                using (BaseContext db = new BaseContext())
                {

                    var cityUserCount = db.Users
                    .GroupBy(u => u.CityNow)
                    .Select(g => new
                    {
                        City = g.Key,
                        UserCount = g.Count()
                    }).OrderByDescending(u => u.UserCount)
                    .ToDictionary(x => x.City, x => x.UserCount);
                    cities_users = cityUserCount;
                    foreach (var stat in cityUserCount)
                    {
                        Console.WriteLine($"{stat.Key}: {stat.Value}");
                    }
                }
                Console.WriteLine("\n1. Просмотреть пользователей города");
                Console.WriteLine("2. Отправить предупреждение в город");
                Console.WriteLine("3. Выйти");
                int regions_tab = int.Parse(Console.ReadLine());

                switch (regions_tab)
                {
                    case 1:
                        Console.Write("Введите название города: ");
                        string? city = Console.ReadLine();
                        
                    ////////////////////////
                        Console.WriteLine("ID   Имя   Возраст   Роль");
                        Console.WriteLine("-------------------------------------------");
                        using (BaseContext db = new BaseContext())
                        {
                            //какая-то проверка плюс цикл
                            var users_in_city = BD.SearchByCity();
                            if(users_in_city != null){
                                foreach (var stat in users_in_city)
                                {
                                    Console.WriteLine($"ID: {stat.Id} \nUSERNAME: {stat.Name} AGE: {stat.Age} ROLE: {stat.Role}\n-------------------------------------------");
                                }
                            }
                            else
                            {
                                Console.WriteLine("такого региона нет");
                            }
                            break;
                        }
                }
                break;
        }
    }
}
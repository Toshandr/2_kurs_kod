using System.Linq;
using API.Models;

public sealed class BD
{
    public static List<User>? SearchByCity(string city)
    {
        List<User> response = [];
        using (BaseContext db = new BaseContext())
        {
            try{
                var users_in_city = db.Users.Where(p => p.CityNow == city).ToList();
                return users_in_city;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске пользователей в городе {city}: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }

    public static string? SearchUser(int id)
    {
        try
        {
            using (BaseContext db = new BaseContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    return user.TelegramTeg;
                }
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при поиске пользователя с ID {id}: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return null;
        }
    }
}

using API.Models;

namespace API.Handlers;

public static class UsersHandler
{
    public static List<User> GetAllUsers()
    {
        using (BaseContext db = new BaseContext())
        {
            return db.Users.ToList();
        }
    }

    public static User? GetUserById(int id)
    {
        using (BaseContext db = new BaseContext())
        {
            return db.Users.FirstOrDefault(u => u.Id == id);
        }
    }

    public static List<User>? GetUsersByCity(string city)
    {
        return BD.SearchByCity(city);
    }

    public static string? GetUserTelegramTag(int id)
    {
        return BD.SearchUser(id);
    }

    public static User CreateUser(string name, int age, string telegramTag, string cityNow, string cityLater, string role = "guest")
    {
        using (BaseContext db = new BaseContext())
        {
            var newUser = new User
            {
                Name = name,
                Age = age,
                TelegramTeg = telegramTag,
                CityNow = cityNow,
                CityLater = cityLater,
                Role = role
            };

            db.Users.Add(newUser);
            db.SaveChanges();
            return newUser;
        }
    }
}

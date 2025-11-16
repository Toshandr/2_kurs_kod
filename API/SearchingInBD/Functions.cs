using API.Models;

public sealed class BD
{
    public static List<User>? SearchByCity(string city)
    {
        List<User> response = [];
        using (BaseContext db = new BaseContext())
        {
            try{
                var users_in_city = db.Users.Where(p => p.CityNow == city);
                foreach(var i in users_in_city)
                {
                    response.Add(i);
                }
                return response;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }

    public static string? SearchUser(int id)
    {
        using (BaseContext db = new BaseContext())
        {
                foreach(var i in db.Users)
                {
                    if(i.Id == id)
                    {
                        return i.TelegramTeg;
                    }
                }
            return null;
        }
    }
}

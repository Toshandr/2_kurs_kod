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
                return (List<User>)users_in_city;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
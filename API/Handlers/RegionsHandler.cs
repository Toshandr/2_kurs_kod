using API.Models;

namespace API.Handlers;

public static class RegionsHandler
{
    public static Dictionary<string, int> GetCityStatistics()
    {
        using (BaseContext db = new BaseContext())
        {
            var cityStats = db.Users
                .GroupBy(u => u.CityNow)
                .Select(g => new
                {
                    City = g.Key,
                    UserCount = g.Count()
                })
                .OrderByDescending(u => u.UserCount)
                .ToDictionary(x => x.City, x => x.UserCount);

            return cityStats;
        }
    }

    public static List<User>? GetUsersInCity(string city)
    {
        return BD.SearchByCity(city);
    }
}

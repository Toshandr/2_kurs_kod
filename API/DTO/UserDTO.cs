namespace API.DTO;

public class TelegramUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int Age { get; set; } = 0;
}

public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TelegramTag { get; set; } = string.Empty;
    public string? Role { get; set; }
    public int Age { get; set; }
    public string CityNow { get; set; } = string.Empty;
    public string CityLater { get; set; } = string.Empty;
}


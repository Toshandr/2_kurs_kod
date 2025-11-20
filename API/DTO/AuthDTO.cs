namespace API.DTO;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
}

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? Role { get; set; }
    public int Age { get; set; }
    public string? CityNow { get; set; }
    public string? CityLater { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserResponse User { get; set; } = null!;
}

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public UserResponse User { get; set; } = null!;
}


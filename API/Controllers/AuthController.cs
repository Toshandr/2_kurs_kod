using System.Security.Cryptography;
using System.Text;
using API.Models;
using API.Services;
using API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            using (var db = new BaseContext())
            {
                var user = db.Users.FirstOrDefault(u => 
                    (u.TelegramTeg == request.Username || u.Name == request.Username));

                if (user == null)
                {
                    return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
                }

                // Проверяем пароль, если он установлен
                if (!string.IsNullOrEmpty(user.Password))
                {
                    if (string.IsNullOrEmpty(request.Password))
                    {
                        return Unauthorized(new { message = "Требуется пароль" });
                    }

                    // Простая проверка пароля (в продакшене лучше использовать хеширование)
                    // Для совместимости проверяем как хеш, так и простой пароль
                    var passwordHash = HashPassword(request.Password);
                    if (user.Password != passwordHash && user.Password != request.Password)
                    {
                        return Unauthorized(new { message = "Неверное имя пользователя или пароль" });
                    }
                }

                // Генерируем JWT токен
                var token = _jwtService.GenerateToken(user.Id, user.Name, user.Role);

                var response = new LoginResponse
                {
                    Token = token,
                    User = new UserResponse
                    {
                        Id = user.Id,
                        Name = user.Name,
                        TelegramTag = user.TelegramTeg,
                        Role = user.Role,
                        Age = user.Age,
                        CityNow = user.CityNow,
                        CityLater = user.CityLater
                    }
                };

                return Ok(response);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Ошибка входа: {ex.Message}" });
        }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        try
        {
            using (var db = new BaseContext())
            {
                // Проверяем, существует ли пользователь
                var existingUser = db.Users.FirstOrDefault(u => 
                    u.TelegramTeg == request.Username || u.Name == request.Name);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Пользователь с таким именем уже существует" });
                }

                // Создаем нового пользователя
                var newUser = new User
                {
                    Name = request.Name,
                    TelegramTeg = request.Username,
                    Password = !string.IsNullOrEmpty(request.Password) 
                        ? HashPassword(request.Password) 
                        : null,
                    Role = request.Role ?? "User",
                    Age = request.Age,
                    CityNow = request.CityNow ?? "",
                    CityLater = request.CityLater ?? ""
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                // Генерируем JWT токен для нового пользователя
                var token = _jwtService.GenerateToken(newUser.Id, newUser.Name, newUser.Role);

                var response = new RegisterResponse
                {
                    Message = "Пользователь успешно зарегистрирован",
                    Token = token,
                    User = new UserResponse
                    {
                        Id = newUser.Id,
                        Name = newUser.Name,
                        TelegramTag = newUser.TelegramTeg,
                        Role = newUser.Role,
                        Age = newUser.Age,
                        CityNow = newUser.CityNow,
                        CityLater = newUser.CityLater
                    }
                };

                return Ok(response);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Ошибка регистрации: {ex.Message}" });
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}


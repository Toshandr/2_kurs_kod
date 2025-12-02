using API.Models;
using API.DTO;
using API.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        try
        {
            var users = UsersHandler.GetAllUsers();
            var response = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                TelegramTag = u.TelegramTeg,
                Role = u.Role,
                Age = u.Age,
                CityNow = u.CityNow,
                CityLater = u.CityLater
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка получения списка пользователей", ex);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var user = UsersHandler.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = "Пользователь не найден" });
            }

            var response = new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                TelegramTag = user.TelegramTeg,
                Role = user.Role,
                Age = user.Age,
                CityNow = user.CityNow,
                CityLater = user.CityLater
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError($"Ошибка получения пользователя {id}", ex);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("city/{city}")]
    public IActionResult GetUsersByCity(string city)
    {
        try
        {
            var users = UsersHandler.GetUsersByCity(city);
            
            if (users == null || users.Count == 0)
            {
                return Ok(new List<UserResponse>());
            }

            var response = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                TelegramTag = u.TelegramTeg,
                Role = u.Role,
                Age = u.Age,
                CityNow = u.CityNow,
                CityLater = u.CityLater
            }).ToList();

            return Ok(response);
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError($"Ошибка получения пользователей города {city}", ex);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] TelegramUserRequest request)
    {
        try
        {
            var newUser = UsersHandler.CreateUser(
                request.FirstName,
                request.Age,
                request.Username,
                request.City,
                request.City
            );

            return Ok(new { message = "Пользователь успешно создан", userId = newUser.Id });
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка создания пользователя", ex);
            return BadRequest(new { message = ex.Message });
        }
    }
}

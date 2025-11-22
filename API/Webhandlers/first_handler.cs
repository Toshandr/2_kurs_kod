using API.Models;
using API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Models;


[ApiController]
[Route("api/users")]
public class usersController : ControllerBase
{
    [HttpGet("list")]
    public IActionResult GetAllStudents()
    {
        try
        {
            using (BaseContext db = new BaseContext())
            {
                var users = db.Users.ToList();
                var connectionString = db.Users.Count();
                if (users == null || users.Count == 0)
                {
                    return BadRequest(connectionString);
                }

                List<string> answer = new List<string>();

                foreach (var u in users)
                {
                    answer.Add($"{u.Id}.{u.Name} - {u.TelegramTeg}");
                }
                return Ok(answer);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

[HttpPost]
public async Task<IActionResult> UserRegistration([FromBody] TelegramUserRequest request)
{
    try
    {
        using (BaseContext db = new BaseContext())
        {
            var new_user = new User
            {
                Name = request.FirstName,
                Age = request.Age, 
                TelegramTeg = request.Username,
                CityNow = request.City,
                CityLater = request.City 
            };

            db.Users.Add(new_user);
            db.SaveChanges();
            return Ok("Пользователь успешно зарегистрирован");
        }
    }
    catch (Exception ex)
    {
        return BadRequest($"Ошибка регистрации: {ex.Message}");
    }
}

    [HttpGet("stat")]
    public IActionResult Statistic()
    {
        using (BaseContext db = new BaseContext()) {
            var users = db.Users.ToList();
            var ans = users.CountBy(x => x.CityNow);
            return Ok(ans);    
        }
    }
    
}



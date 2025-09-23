using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var users = db.Users.ToList();
                if (users == null || users.Count == 0)
                {
                    return BadRequest("Пользователей нет");
                }
                List<string> answer = new List<string>();
                foreach (User u in users)
                {
                    answer.Add($"{u.ID}.{u.Name} - {u.Telegram_Teg}");
                }
                return Ok(answer);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}



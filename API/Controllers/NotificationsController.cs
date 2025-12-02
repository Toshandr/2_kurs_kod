using API.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    [HttpPost("user")]
    public async Task<IActionResult> SendMessageToUser([FromBody] SendMessageToUserRequest request)
    {
        try
        {
            await NotificationsHandler.SendMessageToUser(request.UserId, request.Message);
            return Ok(new { message = "Сообщение отправлено" });
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError($"Ошибка отправки сообщения пользователю {request.UserId}", ex);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("city")]
    public async Task<IActionResult> SendMessageToCity([FromBody] SendMessageToCityRequest request)
    {
        try
        {
            await NotificationsHandler.SendMessageToCity(request.City, request.Message);
            return Ok(new { message = "Сообщение отправлено" });
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError($"Ошибка отправки сообщения в город {request.City}", ex);
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class SendMessageToUserRequest
{
    public int UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class SendMessageToCityRequest
{
    public string City { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

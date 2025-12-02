using API.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    [HttpGet("system")]
    public IActionResult GetSystemLog()
    {
        try
        {
            var log = LogsHandler.GetSystemLog();
            return Ok(new { content = log });
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка получения системного лога", ex);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("users")]
    public IActionResult GetUserLogsList()
    {
        try
        {
            var userLogs = LogsHandler.GetUserLogsList();
            return Ok(userLogs);
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка получения списка логов пользователей", ex);
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("users/{fileName}")]
    public IActionResult GetUserLog(string fileName)
    {
        try
        {
            var log = LogsHandler.GetUserLog(fileName);
            return Ok(new { content = log });
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError($"Ошибка получения лога пользователя {fileName}", ex);
            return BadRequest(new { message = ex.Message });
        }
    }
}

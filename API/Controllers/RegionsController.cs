using API.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/regions")]
public class RegionsController : ControllerBase
{
    [HttpGet("statistics")]
    public IActionResult GetCityStatistics()
    {
        try
        {
            var cityStats = RegionsHandler.GetCityStatistics();
            return Ok(cityStats);
        }
        catch (Exception ex)
        {
            API.FileSys.FileSystem.LogError("Ошибка получения статистики по городам", ex);
            return BadRequest(new { message = ex.Message });
        }
    }
}

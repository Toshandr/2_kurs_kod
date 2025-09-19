using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Hello, ASP.NET!";
    }
}

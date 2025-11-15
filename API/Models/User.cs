using System;
using System.Collections.Generic;

namespace API.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public string TelegramTeg { get; set; } = null!;

    public string CityNow { get; set; } = null!;

    public string CityLater { get; set; } = null!;

    public string? Password { get; set; }

    public string? Role { get; set; }
}


public class DTOUser()
{
    
}

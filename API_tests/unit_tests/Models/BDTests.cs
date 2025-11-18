// Класс для мока статического метода
public static class BD
{
    public static Func<string, List<User>> SearchByCity { get; set; } = 
        city => new List<User>();
}

public class User
{
    public string TelegramTeg { get; set; }
}

// Test output helper для захвата Console.WriteLine
public class ConsoleOutput : IDisposable
{
    private readonly StringWriter _stringWriter;
    private readonly TextWriter _originalOutput;

    public ConsoleOutput()
    {
        _stringWriter = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_stringWriter);
    }

    public string GetOutput()
    {
        return _stringWriter.ToString();
    }

    public void Dispose()
    {
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
    }
}
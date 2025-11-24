public sealed class Files
{
    static async void WriteAndCreateUser(string name, string msg, TimeSpan time)
    {
        string path = $"API//logs//Users//{name}";
        string content = msg + Convert.ToString(time);
       // StreamWriter.WriteLineAsync(path, content);
    } 
}
namespace Executor;

public class HumanFacingConsoleLogger : ILogger
{
    public HumanFacingConsoleLogger()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }
    
    public void Info(string message)
    {
        Console.WriteLine($"🔵 {message}");
    }

    public void Error(string message)
    {
        Console.WriteLine($"🟥 {message}");
    }

    public void Warning(string message)
    {
        Console.WriteLine($"🔶 {message}");
    }
}

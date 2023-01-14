namespace ExplogineMonoGame;

public class VirtualRuntime : IRuntime
{
    public IWindow Window { get; } = new VirtualWindow();
    public ClientFileSystem FileSystem { get; } = new();
}

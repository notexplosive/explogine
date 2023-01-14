namespace ExplogineMonoGame;

public class Runtime : IRuntime
{
    public Runtime(IWindow window, ClientFileSystem fileSystem)
    {
        Window = window;
        FileSystem = fileSystem;
    }

    public IWindow Window { get; }
    public ClientFileSystem FileSystem { get; }
}

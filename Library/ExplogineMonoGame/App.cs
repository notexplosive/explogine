using ExplogineCore;

namespace ExplogineMonoGame;

public class App
{
    public IWindow Window { get; }
    public ClientFileSystem FileSystem { get; }

    public App(IWindow window, ClientFileSystem fileSystem)
    {
        Window = window;
        FileSystem = fileSystem;
    }
}

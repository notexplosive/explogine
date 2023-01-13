using ExplogineCore;

namespace ExplogineMonoGame;

public class App
{
    public IWindow Window { get; internal set; }
    public ClientFileSystem FileSystem { get; internal set; }

    public App(IWindow window, ClientFileSystem fileSystem)
    {
        Window = window;
        FileSystem = fileSystem;
    }
}

namespace ExplogineMonoGame;

internal class ClientApp : App
{

    public ClientApp() : base(new PlatformAgnosticWindow(), new ClientFileSystem())
    {
    }

    public void Setup(PlatformAgnosticWindow window, ClientFileSystem fileSystem)
    {
        Window = window;
        FileSystem = fileSystem;
    }
}

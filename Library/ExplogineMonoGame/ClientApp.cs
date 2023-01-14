namespace ExplogineMonoGame;

internal class ClientApp : IApp
{
    private ClientFileSystem _fileSystem;
    private bool _isInitialized;
    private PlatformAgnosticWindow _window;

    public ClientApp()
    {
        _window = new PlatformAgnosticWindow();
        _fileSystem = new ClientFileSystem();
    }

    public IWindow Window
    {
        get
        {
            if (!_isInitialized)
            {
                Client.Debug.LogError("Attempted to access Window before ClientApp was initialized");
            }

            return _window;
        }
    }

    public ClientFileSystem FileSystem
    {
        get
        {
            if (!_isInitialized)
            {
                Client.Debug.LogError("Attempted to access FileSystem before ClientApp was initialized");
            }

            return _fileSystem;
        }
    }

    public void Setup(PlatformAgnosticWindow window, ClientFileSystem fileSystem)
    {
        _isInitialized = true;
        _window = window;
        _fileSystem = fileSystem;
    }
}

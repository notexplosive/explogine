namespace ExplogineMonoGame;

internal class ClientRuntime : IRuntime
{
    private ClientFileSystem _fileSystem;
    private bool _isInitialized;
    private RealWindow _window;

    public ClientRuntime()
    {
        _window = new RealWindow();
        _fileSystem = new ClientFileSystem();
    }

    public IWindow Window
    {
        get
        {
            if (!_isInitialized)
            {
                Client.Debug.LogError("Attempted to access Window before ClientRuntime was initialized");
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
                Client.Debug.LogError("Attempted to access FileSystem before ClientRuntime was initialized");
            }

            return _fileSystem;
        }
    }

    public void Setup(RealWindow window, ClientFileSystem fileSystem)
    {
        _isInitialized = true;
        _window = window;
        _fileSystem = fileSystem;
    }
}

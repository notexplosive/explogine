using System;

namespace ExplogineMonoGame;

internal class ClientApp : IApp
{
    private bool _isInitialized;
    private ClientFileSystem _fileSystem;
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
            if (_isInitialized)
            {
                return _window;
            }

            throw new Exception("Attempted to access Window before ClientApp was initialized");
        }
    }

    public ClientFileSystem FileSystem
    {
        get
        {
            if (_isInitialized)
            {
                return _fileSystem;
            }

            throw new Exception("Attempted to access FileSystem before ClientApp was initialized");
        }
    }

    public void Setup(PlatformAgnosticWindow window, ClientFileSystem fileSystem)
    {
        _isInitialized = true;
        _window = window;
        _fileSystem = fileSystem;
    }
}

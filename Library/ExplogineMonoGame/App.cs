using System;

namespace ExplogineMonoGame;

public class App
{
    public App(IWindow window, ClientFileSystem fileSystem)
    {
        Window = window;
        FileSystem = fileSystem;
    }

    /// <summary>
    ///     Wrapper for accessing the Window of your platform.
    /// </summary>
    public IWindow Window { get; protected set; }
    
    /// <summary>
    ///     Wrapper for accessing the Filesystem of your platform.
    /// </summary>
    public ClientFileSystem FileSystem { get; protected set; }
}
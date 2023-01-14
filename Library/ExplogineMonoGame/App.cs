using System;

namespace ExplogineMonoGame;

public class App
{
    public App(IWindow window, ClientFileSystem fileSystem)
    {
        Window = window;
        FileSystem = fileSystem;
    }

    public IWindow Window { get; protected set; }
    public ClientFileSystem FileSystem { get; protected set; }
}
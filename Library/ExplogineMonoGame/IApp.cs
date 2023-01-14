using System;

namespace ExplogineMonoGame;

public interface IApp
{
    /// <summary>
    ///     Wrapper for accessing the Window of your platform.
    /// </summary>
    public IWindow Window { get; }
        
    /// <summary>
    ///     Wrapper for accessing the Filesystem of your platform.
    /// </summary>
    public ClientFileSystem FileSystem { get; }
}
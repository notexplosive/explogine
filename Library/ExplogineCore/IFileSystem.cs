namespace ExplogineCore;

public interface IFileSystem
{
    /// <summary>
    ///     Checks if a file exists
    /// </summary>
    /// <param name="relativePathToFile"></param>
    /// <returns>True if the file exists, false otherwise.</returns>
    public bool HasFile(string relativePathToFile);

    /// <summary>
    ///     Creates a file and its directory structure if it doesn't exist. If the file exists this is a no-op.
    /// </summary>
    /// <param name="relativePathToFile"></param>
    public void CreateFile(string relativePathToFile);

    /// <summary>
    ///     Removes a file
    /// </summary>
    /// <param name="relativePathToFile"></param>
    public void DeleteFile(string relativePathToFile);

    /// <summary>
    ///     Creates a file and its directory structure, if the file already exist it is wiped.
    /// </summary>
    /// <param name="relativePathToFile"></param>
    public void CreateOrOverwriteFile(string relativePathToFile);

    /// <summary>
    ///     Appends lines to a file that exists.
    /// </summary>
    /// <param name="relativePathToFile"></param>
    /// <param name="lines"></param>
    public void AppendToFile(string relativePathToFile, params string[] lines);

    /// <summary>
    ///     Reads the contents of a file if it exists. If the file does not exist returns empty string.
    /// </summary>
    /// <param name="relativePathToFile"></param>
    /// <returns>Contents of the file or empty string</returns>
    public string ReadFile(string relativePathToFile);

    /// <summary>
    ///     Returns a list of files in directory
    /// </summary>
    /// <param name="targetRelativePath"></param>
    /// <param name="extension">Only returns files with designated extension</param>
    /// <param name="recursive">Searches within directories</param>
    /// <returns>List of paths relative to the FileSystem object's root</returns>
    public List<string> GetFilesAt(string targetRelativePath, string extension = "*", bool recursive = true);

    /// <summary>
    ///     Creates (or overwrites) a file and writes lines to it.
    /// </summary>
    /// <param name="relativeFileName"></param>
    /// <param name="lines"></param>
    void WriteToFile(string relativeFileName, params string[] lines);

    /// <summary>
    /// Working Directory of this FileSystem object
    /// </summary>
    /// <returns></returns>
    string GetCurrentDirectory();

    /// <summary>
    /// Creates directory if it does not exist.
    /// Creates a new filesystem object at the subdirectory path.
    /// </summary>
    /// <param name="subDirectory"></param>
    /// <returns></returns>
    IFileSystem GetDirectory(string subDirectory);

    /// <summary>
    /// Gets length of a file in bytes.
    /// </summary>
    /// <param name="relativePathToFile"></param>
    /// <returns></returns>
    public long GetFileSize(string relativePathToFile);

    Task<string> ReadFileAsync(string fileName);
}

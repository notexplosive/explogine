namespace ExplogineCore;

public interface IFileSystem
{
    public string ContentPath { get; }
    public List<string> GetFilesAtDirectory(string targetDirectory, string extension);
    public Task<string> ReadFileInContent(string relativePath);
    public Task<string> ReadFile(string path);
    public void WriteFile(string path, string contents);
    public void CreateFileIfNotExist(string path);
    public void AppendFile(string path, params string[] lines);
    public string GetAppDataPath(string path);
    public void ClearFile(string path);

    public List<string> GetFilesAtContentDirectory(string targetDirectory, string extension = "*")
    {
        var contentPath = ContentPath;
        var targetFullPath = Path.Join(contentPath, targetDirectory);

        return GetFilesAtDirectory(targetFullPath, extension);
    }
}

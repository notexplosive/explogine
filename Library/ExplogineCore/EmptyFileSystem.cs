namespace ExplogineCore;

public class EmptyFileSystem : IFileSystem
{
    public List<string> GetFilesAtDirectory(string targetDirectory, string extension)
    {
        return new List<string>();
    }

    public Task<string> ReadFileInContent(string relativePath)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> ReadFile(string path)
    {
        return Task.FromResult(string.Empty);
    }

    public void WriteFile(string path, string contents)
    {
    }

    public void CreateFileIfNotExist(string path)
    {
    }

    public void AppendFile(string path, params string[] lines)
    {
    }

    public void ClearFile(string path)
    {
    }

    public string GetAppDataPath(string path)
    {
        return "";
    }

    public string ContentPath => string.Empty;
}

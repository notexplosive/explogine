namespace ExplogineCore;

public interface IFileSystem
{
    public string ContentPath { get; }
    public List<string> GetFilesAtContentDirectory(string targetDirectory, string extension);
    public Task<string> ReadFileInContentDirectory(string relativePath);
    public Task<string> ReadTextFileInWorkingDirectory(string path);
    public void WriteFileToWorkingDirectory(string path, string contents);
    public void CreateFileInWorkingDirectory(string path);
    public void AppendToFileInWorkingDirectory(string path, string contents);
}

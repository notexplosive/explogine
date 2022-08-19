namespace ExplogineCore;

public interface IFileSystem
{
    public string ContentPath { get; }
    public List<string> GetFilesAtContentDirectory(string targetDirectory, string extension);
    public Task<string> ReadFileInContentDirectory(string relativePath);
    public Task<string> ReadTextFileInWorkingDirectory(string path);
    public void WriteFileToWorkingDirectory(string path, string contents);
}

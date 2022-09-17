namespace ExplogineCore;

public interface IFileSystem
{
    public bool HasFile(string relativePathToFile);
    public void CreateFile(string relativePathToFile);
    public void CreateOrOverwriteFile(string relativePathToFile);
    public void AppendToFile(string relativePathToFile, params string[] lines);
    public string ReadFile(string relativePathToFile);
    public List<string> GetFilesAt(string targetRelativePath, string extension = "*", bool recursive = true);
    void WriteToFile(string relativeFileName, params string[] lines);
}

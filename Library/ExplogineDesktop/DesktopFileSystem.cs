using ExplogineCore;
using ExplogineMonoGame;

namespace ExplogineDesktop;

public class DesktopFileSystem : IFileSystem
{
    public string ContentPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Client.ContentBaseDirectory);

    public List<string> GetFilesAtDirectory(string targetFullPath, string extension = "*")
    {
        var result = new List<string>();

        
        var infoAtTargetPath = new DirectoryInfo(targetFullPath);
        if (!infoAtTargetPath.Exists)
        {
            throw new DirectoryNotFoundException($"Missing content directory {targetFullPath}");
        }
        
        var files = infoAtTargetPath.GetFiles("*." + extension);
        foreach (var file in files)
        {
            result.Add(file.FullName);
        }

        var directories = infoAtTargetPath.GetDirectories();
        foreach (var directory in directories)
        {
            var subDirectoryResults = GetFilesAtDirectory(Path.Join(targetFullPath, directory.Name), extension);

            foreach (var fileName in subDirectoryResults)
            {
                result.Add(fileName);
            }
        }

        return result;
    }

    public async Task<string> ReadFileInContentDirectory(string relativePath)
    {
        var local = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Client.ContentBaseDirectory),
            relativePath);
        if (File.Exists(local))
        {
            var result = await File.ReadAllTextAsync(local);
            return result;
        }

        throw new FileNotFoundException();
    }

    public async Task<string> ReadTextFileInWorkingDirectory(string path)
    {
        if (File.Exists(path))
        {
            var result = await File.ReadAllTextAsync(path);
            return result;
        }

        throw new FileNotFoundException();
    }

    public async void WriteFileToWorkingDirectory(string path, string contents)
    {
        await File.WriteAllTextAsync(path, contents);
    }

    public void CreateFileInWorkingDirectory(string path)
    {
        File.Create(path);
    }

    public async void AppendToFileInWorkingDirectory(string path, string contents)
    {
        await File.AppendAllTextAsync(path, contents);
    }
}

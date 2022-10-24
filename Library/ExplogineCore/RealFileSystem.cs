namespace ExplogineCore;

public class RealFileSystem : IFileSystem
{
    public RealFileSystem(string rootPath)
    {
        RootPath = rootPath;
        Directory.CreateDirectory(RootPath);
    }
    
    public string RootPath { get; }

    public bool HasFile(string relativePathToFile)
    {
        return FileInfoAt(relativePathToFile).Exists;
    }

    public FileInfo FileInfoAt(string relativePathToFile)
    {
        return new FileInfo(ToWorkingPath(relativePathToFile));
    }

    public void CreateFile(string relativePathToFile)
    {
        var info = FileInfoAt(relativePathToFile);
        Directory.CreateDirectory(info.Directory!.FullName);
        if (!info.Exists)
        {
            info.Create().Close();
        }
    }

    public void DeleteFile(string relativePathToFile)
    {
        File.Delete(ToWorkingPath(relativePathToFile));
    }

    public void CreateOrOverwriteFile(string relativePathToFile)
    {
        var info = FileInfoAt(relativePathToFile);
        if (info.Exists)
        {
            info.Delete();
        }
        CreateFile(relativePathToFile);
    }

    public void AppendToFile(string relativePathToFile, params string[] lines)
    {
        File.AppendAllLines(ToWorkingPath(relativePathToFile), lines);
    }

    public string ReadFile(string relativePathToFile)
    {
        if (!FileInfoAt(relativePathToFile).Exists)
        {
            return string.Empty;
        }
        
        return File.ReadAllText(ToWorkingPath(relativePathToFile));
    }

    public List<string> GetFilesAt(string targetRelativePath, string extension = "*", bool recursive = true)
    {
        var fullPaths = GetFilesAtFullPath(ToWorkingPath(targetRelativePath), extension, recursive);

        var result = new List<string>();
        var root = RootPath;
        foreach (var path in fullPaths)
        {
            var revisedPath = path.Replace(root, string.Empty).Replace(Path.DirectorySeparatorChar.ToString(), "/");
            if (revisedPath.StartsWith('/'))
            {
                revisedPath = revisedPath.Substring(1);
            }
            result.Add(revisedPath);
        }

        return result;
    }
    
    private List<string> GetFilesAtFullPath(string targetFullPath, string extension = "*", bool recursive = true)
    {
        var result = new List<string>();

        var infoAtTargetPath = new DirectoryInfo(targetFullPath);
        if (!infoAtTargetPath.Exists)
        {
            throw new DirectoryNotFoundException($"{targetFullPath}");
        }

        var files = infoAtTargetPath.GetFiles("*." + extension);
        foreach (var file in files)
        {
            result.Add(file.FullName);
        }

        if (recursive)
        {
            var directories = infoAtTargetPath.GetDirectories();
            foreach (var directory in directories)
            {
                var subDirectoryResults = GetFilesAtFullPath(Path.Join(targetFullPath, directory.Name), extension);

                foreach (var fileName in subDirectoryResults)
                {
                    result.Add(fileName);
                }
            }
        }

        return result;
    }

    public void WriteToFile(string relativePathToFile, params string[] lines)
    {
        CreateOrOverwriteFile(relativePathToFile);
        AppendToFile(relativePathToFile, lines);
    }

    private string ToWorkingPath(string relativePath)
    {
        if (relativePath == ".")
        {
            return RootPath;
        }
        return Path.Join(RootPath, relativePath);
    }

    public string GetCurrentDirectory()
    {
        return RootPath;
    }
}

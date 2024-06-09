using System.Diagnostics.Contracts;

namespace ExplogineCore;

public class RealFileSystem : IFileSystem
{
    public RealFileSystem(string rootPath)
    {
        RootPath = rootPath;
        Directory.CreateDirectory(RootPath);
    }

    public string RootPath { get; }

    public string FullNormalizedRootPath
    {
        get
        {
            var fullPath = new FileInfo(RootPath).FullName;
            if (fullPath.StartsWith('/'))
            {
                fullPath = fullPath.Substring(1);
            }

            return fullPath.Replace(Path.DirectorySeparatorChar, '/');
        }
    }

    public bool HasFile(string relativePathToFile)
    {
        return FileInfoAt(relativePathToFile).Exists;
    }

    public void CreateFile(string relativePathToFile)
    {
        if (RealFileSystem.IsInvalidPathName(relativePathToFile))
        {
            // Silently fail (unfortunately)
            return;
        }

        var info = FileInfoAt(relativePathToFile);
        Directory.CreateDirectory(info.Directory!.FullName);
        
        if (!info.Exists)
        {
            info.Create().Close();
        }
    }

    private static bool IsInvalidPathName(string path)
    {
        foreach (var illegalChar in Path.GetInvalidPathChars())
        {
            if (path.Contains(illegalChar))
            {
                return true;
            }
        }

        return false;
    }

    public void DeleteFile(string relativePathToFile)
    {
        File.Delete(ToAbsolutePath(relativePathToFile));
    }

    public void CreateOrOverwriteFile(string relativePathToFile)
    {
        if (RealFileSystem.IsInvalidPathName(relativePathToFile))
        {
            // Silently fail (unfortunately)
            return;
        }
        
        var info = FileInfoAt(relativePathToFile);
        if (info.Exists)
        {
            info.Delete();
        }

        CreateFile(relativePathToFile);
    }

    public void AppendToFile(string relativePathToFile, params string[] lines)
    {
        File.AppendAllLines(ToAbsolutePath(relativePathToFile), lines);
    }

    public string ReadFile(string relativePathToFile)
    {
        if (!FileInfoAt(relativePathToFile).Exists)
        {
            return string.Empty;
        }

        return File.ReadAllText(ToAbsolutePath(relativePathToFile));
    }

    public byte[] ReadBytes(string relativePathToFile)
    {
        if (!FileInfoAt(relativePathToFile).Exists)
        {
            return Array.Empty<byte>();
        }
        
        return File.ReadAllBytes(ToAbsolutePath(relativePathToFile));
    }

    [Pure]
    public List<string> GetFilesAt(string targetRelativePath, string extension = "*", bool recursive = true)
    {
        // Create the directory
        GetDirectory(targetRelativePath);
        
        var fullPaths = GetFilesAtFullPath(ToAbsolutePath(targetRelativePath), extension, recursive);

        var result = new List<string>();
        foreach (var path in fullPaths)
        {
            var revisedPath = GetRelativePath(path);
            if (revisedPath.StartsWith('/'))
            {
                revisedPath = revisedPath.Substring(1);
            }

            result.Add(revisedPath);
        }

        return result;
    }

    private string GetRelativePath(string targetPath)
    {
        return Path.GetRelativePath(FullNormalizedRootPath, ToAbsolutePath(targetPath)).Replace(Path.DirectorySeparatorChar, '/');
    }

    public void WriteToFile(string relativePathToFile, params string[] lines)
    {
        CreateOrOverwriteFile(relativePathToFile);
        AppendToFile(relativePathToFile, lines);
    }
    
    public StreamDescriptor OpenFileStream(string relativePathToFile)
    {
        var info = FileInfoAt(relativePathToFile);
        CreateOrOverwriteFile(relativePathToFile);
        return new StreamDescriptor(info);
    }

    public class StreamDescriptor
    {
        private readonly FileStream _fileStream;
        private readonly StreamWriter _streamWriter;

        public StreamDescriptor(FileInfo info)
        {
            _fileStream = new FileStream(info.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            _streamWriter = new StreamWriter(_fileStream);
        }

        public void Close()
        {
            _streamWriter.Dispose();
            _fileStream.Dispose();
        }

        public void Write(string content)
        {
            _streamWriter.WriteLine(content);
        }

        public void Flush()
        {
            _streamWriter.Flush();
        }
    }

    public string GetCurrentDirectory()
    {
        return RootPath;
    }

    public FileInfo FileInfoAt(string relativePathToFile)
    {
        return new FileInfo(ToAbsolutePath(relativePathToFile));
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

    private string ToAbsolutePath(string givenPath)
    {
        if (Path.IsPathRooted(givenPath))
        {
            return new FileInfo(givenPath).FullName;
        }
        
        if (givenPath == ".")
        {
            return RootPath;
        }

        return Path.Join(RootPath, givenPath);
    }

    public IFileSystem GetDirectory(string subDirectory)
    {
        return new RealFileSystem(ToAbsolutePath(subDirectory));
    }

    public long GetFileSize(string relativePathToFile)
    {
        return FileInfoAt(relativePathToFile).Length;
    }

    public async Task<string> ReadFileAsync(string relativePathToFile)
    {
        if (!FileInfoAt(relativePathToFile).Exists)
        {
            return string.Empty;
        }

        return await File.ReadAllTextAsync(ToAbsolutePath(relativePathToFile));
    }

    public IFileSystem CreateDirectory(string destinationPath = ".")
    {
        var finalPath = FullNormalizedRootPath;
        if (destinationPath != ".")
        {
            finalPath = ToAbsolutePath(destinationPath);
        }
        
        Directory.CreateDirectory(finalPath);
        return GetDirectory(destinationPath);
    }

    public void DeleteDirectory(string path, bool recursive)
    {
        Directory.Delete(ToAbsolutePath(path), recursive);
    }
}

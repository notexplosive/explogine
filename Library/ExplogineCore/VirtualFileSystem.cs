﻿namespace ExplogineCore;

public class VirtualFileSystem : IFileSystem
{
    private readonly VirtualRoot _root = new();

    public bool HasFile(string relativePathToFile)
    {
        var directory = _root.CreateDirectoriesUpToFile(relativePathToFile, false);

        if (directory != null)
        {
            var file = directory.GetLocalFile(_root.GetFileName(relativePathToFile));
            return file != null;
        }

        return false;
    }

    public void CreateFile(string relativePathToFile)
    {
        WriteToFile(relativePathToFile, "");
    }

    public void DeleteFile(string relativePathToFile)
    {
        _root.CreateDirectoriesUpToFile(relativePathToFile, false)?.RemoveFile(_root.GetFileName(relativePathToFile));
    }

    public void CreateOrOverwriteFile(string relativePathToFile)
    {
        DeleteFile(relativePathToFile);
        CreateFile(relativePathToFile);
    }

    public void AppendToFile(string relativePathToFile, params string[] lines)
    {
        var directory = _root.CreateDirectoriesUpToFile(relativePathToFile, false);

        if (directory != null)
        {
            var file = directory.GetLocalFile(_root.GetFileName(relativePathToFile));

            if (file != null)
            {
                file.Content += string.Join("\n", lines);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }

    public string ReadFile(string relativePathToFile)
    {
        var directory = _root.CreateDirectoriesUpToFile(relativePathToFile, false);

        if (directory != null)
        {
            var file = directory.GetLocalFile(_root.GetFileName(relativePathToFile));

            if (file != null)
            {
                return file.Content;
            }
        }

        return string.Empty;
    }

    public List<string> GetFilesAt(string targetRelativePath, string extension = "*", bool recursive = true)
    {
        var result = new List<string>();
        var directory = _root.GetDirectory(targetRelativePath, false);

        if (directory != null)
        {
            foreach (var file in directory.AllFiles(true))
            {
                result.Add((file as IVirtualItem).FullPath());
            }
        }

        return result;
    }

    public void WriteToFile(string relativeFileName, params string[] lines)
    {
        _root.CreateDirectoriesUpToFile(relativeFileName, true)
            ?.CreateFile(_root.GetFileName(relativeFileName), string.Join('\n', lines));
    }

    private interface IVirtualItem
    {
        VirtualDirectory Parent { get; }
        string Name { get; }

        public string FullPath()
        {
            var path = Name;

            var parent = Parent;
            while (!parent.IsRoot)
            {
                path = $"{parent.Name}/{path}";
                parent = parent.Parent;
            }

            return path;
        }
    }

    private record VirtualRoot() : VirtualDirectory(null!, "/")
    {
        public VirtualDirectory? GetDirectory(string path, bool forceCreate)
        {
            var nodes = path.SplitDirectorySeparators();
            var currentDirectory = this as VirtualDirectory;

            if (path == "" || path == ".")
            {
                return this;
            }

            foreach (var node in nodes)
            {
                var newLocalDirectory = GetLocalDirectory(node);

                if (newLocalDirectory == null)
                {
                    if (forceCreate)
                    {
                        newLocalDirectory = currentDirectory.CreateDirectory(node);
                    }
                    else
                    {
                        return null;
                    }
                }

                currentDirectory = newLocalDirectory;
            }

            return currentDirectory;
        }

        public VirtualDirectory? CreateDirectoriesUpToFile(string path, bool forceCreate)
        {
            var nodes = path.SplitDirectorySeparators().ToList();
            nodes.RemoveAt(nodes.Count - 1);
            return GetDirectory(string.Join("/", nodes), forceCreate);
        }

        public string GetFileName(string path)
        {
            var nodes = path.SplitDirectorySeparators();
            return nodes[^1];
        }
    }

    private record VirtualDirectory(VirtualDirectory Parent, string Name) : IVirtualItem
    {
        private readonly List<IVirtualItem> _items = new();
        public bool IsRoot => this is VirtualRoot;

        public VirtualDirectory CreateDirectory(string name)
        {
            return (VirtualDirectory) AddItem(new VirtualDirectory(this, name));
        }

        public VirtualFile CreateFile(string name, string content)
        {
            return (VirtualFile) AddItem(new VirtualFile(this, name, content));
        }

        private IVirtualItem AddItem(IVirtualItem item)
        {
            if (item.Name.Contains('/') || item.Name.Contains('\\'))
            {
                throw new Exception("Invalid character");
            }

            _items.Add(item);
            return item;
        }

        public IEnumerable<IVirtualItem> AllItems()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        public IEnumerable<VirtualFile> AllFiles(bool recursive)
        {
            foreach (var item in _items)
            {
                if (item is VirtualFile file)
                {
                    yield return file;
                }

                if (recursive)
                {
                    if (item is VirtualDirectory directory)
                    {
                        foreach (var f in directory.AllFiles(recursive))
                        {
                            yield return f;
                        }
                    }
                }
            }
        }

        public VirtualFile? GetLocalFile(string name)
        {
            foreach (var item in AllFiles(false))
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }

        public VirtualDirectory? GetLocalDirectory(string name)
        {
            foreach (var item in AllItems())
            {
                if (item is VirtualDirectory directory)
                {
                    if (directory.Name == name)
                    {
                        return directory;
                    }
                }
            }

            return null;
        }

        public void RemoveFile(string relativePathToFile)
        {
            var file = GetLocalFile(relativePathToFile);
            if (file != null)
            {
                _items.Remove(file);
            }
        }

        public override string ToString()
        {
            return (this as IVirtualItem).FullPath();
        }
    }

    private record VirtualFile(VirtualDirectory Parent, string Name, string Content) : IVirtualItem
    {
        public string Content { get; set; } = Content;

        public override string ToString()
        {
            return (this as IVirtualItem).FullPath();
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotCore;

public interface IFileSystem
{
    public string ContentPath { get; }
    public List<string> GetFilesAtContentDirectory(string targetDirectory, string extension);
    public Task<string> ReadFileInContentDirectory(string relativePath);
    public Task<string> ReadTextFile(string path);
}

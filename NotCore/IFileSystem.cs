using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotCore;

public interface IFileSystem
{
    public List<string> GetFilesAtContentDirectory(string targetDirectory, string extension);
    public Task<string> ReadFileInContentDirectory(string relativePath);
    public Task<string> ReadTextFile(string path);
    public string ContentPath { get; }
}

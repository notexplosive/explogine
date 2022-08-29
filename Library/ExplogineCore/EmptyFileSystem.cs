﻿namespace ExplogineCore;

public class EmptyFileSystem : IFileSystem
{
    public List<string> GetFilesAtContentDirectory(string targetDirectory, string extension)
    {
        return new List<string>();
    }

    public Task<string> ReadFileInContentDirectory(string relativePath)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> ReadTextFileInWorkingDirectory(string path)
    {
        return Task.FromResult(string.Empty);
    }

    public void WriteFileToWorkingDirectory(string path, string contents)
    {
    }

    public void CreateFileInWorkingDirectory(string path)
    {
    }

    public void AppendToFileInWorkingDirectory(string path, string contents)
    {
    }

    public string ContentPath => string.Empty;
}
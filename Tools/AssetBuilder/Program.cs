using AssetBuilder;

const string contentSectionHeader = "#---------------------------------- Content ---------------------------------#";
var excludedDirectories = new HashSet<string> {"bin", "obj"};
var pathToContentFile = Path.Join(args[0]);

Console.WriteLine($"Path to content file: {pathToContentFile}");
if (!File.Exists(pathToContentFile))
{
    Console.WriteLine("File does not exist, aborting.");
    return 1;
}

var contentFileInfo = new FileInfo(pathToContentFile);
var contentDirectoryInfo = contentFileInfo.Directory;
var lines = File.ReadAllLines(pathToContentFile);
var resultLines = new List<string>();

IContentWriter GetContentWriter(string extension)
{
    return extension switch
    {
        ".png" => new TextureContentWriter(),
        ".ogg" => new OggContentWriter(),
        ".spritefont" => new SpriteFontContentWriter(),
        ".fx" => new EffectContentWriter(),
        _ => new CopyContentWriter()
    };
}

void RegisterFile(FileInfo fileInfo)
{
    var relativePath = Path.GetRelativePath(contentDirectoryInfo!.FullName, fileInfo.FullName);
    Console.WriteLine($"Registering {relativePath}");
    resultLines.AddRange(GetContentWriter(fileInfo.Extension.ToLower())
        .GetContentLines(relativePath.Replace(Path.DirectorySeparatorChar, '/')));
    resultLines.Add(string.Empty);
}

void RegisterContentsOfDirectoryRecursive(DirectoryInfo currentDirectory)
{
    foreach (var file in currentDirectory.GetFiles())
    {
        if (file.Name != contentFileInfo.Name)
        {
            try
            {
                RegisterFile(file);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\tFailed {e.Message}");
            }
        }
    }

    foreach (var directory in currentDirectory.GetDirectories())
    {
        if (!excludedDirectories.Contains(directory.Name))
        {
            RegisterContentsOfDirectoryRecursive(directory);
        }
    }
}

foreach (var line in lines)
{
    resultLines.Add(line);

    if (line == contentSectionHeader)
    {
        break;
    }
}

resultLines.Add(string.Empty);

RegisterContentsOfDirectoryRecursive(contentDirectoryInfo!);

Console.WriteLine($"Writing result to {contentFileInfo.Name}...");
File.WriteAllLines(contentFileInfo.FullName, resultLines);
Console.WriteLine("Done");

return 0;

using System.Diagnostics.Contracts;

namespace ExplogineCore;

public static class StringExtensions
{
    [Pure]
    public static string[] SplitLines(this string str)
    {
        return str.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
    }

    [Pure]
    public static string[] SplitDirectorySeparators(this string str)
    {
        return str.Split(new[] {"\\", "/"}, StringSplitOptions.None);
    }

    [Pure]
    public static string RemoveFileExtension(this string str)
    {
        var fileInfo = new FileInfo(str);
        return fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
    }
}

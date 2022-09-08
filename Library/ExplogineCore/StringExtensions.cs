﻿namespace ExplogineCore;

public static class StringExtensions
{
    public static string[] SplitLines(this string str)
    {
        return str.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
    }
}

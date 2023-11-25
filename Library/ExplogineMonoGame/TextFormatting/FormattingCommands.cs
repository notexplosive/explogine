using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.TextFormatting;

public static class FormattingCommands
{
    internal static readonly Dictionary<string, ICommand> DefaultCommands =
        Reflection.GetStaticFieldsThatDeriveFromType<BuiltInCommands, ICommand>();
}

using System.Collections.Generic;
using ExplogineCore;

namespace ExplogineMonoGame.TextFormatting;

internal class Commands
{
    public static ScopedCommand Color = new(
        args => new PushColor(args),
        () => new PopColor());

    public static ScopedCommand Font = new(
        args => new PushFont(args),
        () => new PopFont());

    public static Command Image = new(
        args => new ImageLiteralInstruction(args)
    );
    
    public static Command Texture = new(
        args => new TextureLiteralInstruction(args)
    );

    public static readonly Dictionary<string, ICommand> LookupTable =
        Reflection.GetStaticFieldsThatDeriveFromType<Commands, ICommand>();
}

using JetBrains.Annotations;

namespace ExplogineMonoGame.TextFormatting;

public class BuiltInCommands
{
    [UsedImplicitly]
    public static ScopedCommand Color = new(
        args => new ColorCommand(args),
        () => new ColorCommand.Pop());

    [UsedImplicitly]
    public static ScopedCommand Font = new(
        args => new FontCommand(args),
        () => new FontCommand.Pop());

    [UsedImplicitly]
    public static ScopedCommand Scale = new(
        args => new FontScaleCommand(args),
        () => new FontScaleCommand.Pop());

    [UsedImplicitly]
    public static Command Image = new(
        args => new ImageLiteralInstruction(args)
    );

    [UsedImplicitly]
    public static Command Texture = new(
        args => new TextureLiteralInstruction(args)
    );
}

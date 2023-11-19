using JetBrains.Annotations;

namespace ExplogineMonoGame.TextFormatting;

internal class BuiltInCommands
{
    [UsedImplicitly]
    public static ScopedCommand Color = new(
        args => new PushColor(args),
        () => new PopColor());

    [UsedImplicitly]
    public static ScopedCommand Font = new(
        args => new PushFont(args),
        () => new PopFont());

    [UsedImplicitly]
    public static Command Image = new(
        args => new ImageLiteralInstruction(args)
    );

    [UsedImplicitly]
    public static Command Texture = new(
        args => new TextureLiteralInstruction(args)
    );
}
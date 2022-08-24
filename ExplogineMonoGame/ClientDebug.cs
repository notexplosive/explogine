using ExplogineMonoGame.Cartridges;

namespace ExplogineMonoGame;

public class ClientDebug
{
    internal DebugCartridge Cartridge { get; } = new();
    public DebugLevel DebugLevel { get; set; }

    public bool IsActive => DebugLevel == DebugLevel.Active || IsPassive;
    public bool IsPassive => DebugLevel == DebugLevel.Passive;
}

using ExplogineMonoGame.Cartridges;

namespace ExplogineMonoGame;

public class ClientDebug
{
    public DebugLevel DebugLevel { get; set; }
    public bool IsActive => DebugLevel == DebugLevel.Active || IsPassive;
    public bool IsPassive => DebugLevel == DebugLevel.Passive;
}

using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Cartridges;

public interface ICartridge : IUpdateInputHook, IDrawHook, IUpdateHook
{
    CartridgeConfig CartridgeConfig { get; }
    void OnCartridgeStarted();
    bool ShouldLoadNextCartridge();
    void Unload();
}

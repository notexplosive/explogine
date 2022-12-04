using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

public interface ICartridge
{
    CartridgeConfig CartridgeConfig { get; }
    void OnCartridgeStarted();
    void Update(float dt);
    void Draw(Painter painter);
    void UpdateInput(InputFrameState input);
    bool ShouldLoadNextCartridge();
    void Unload();
}
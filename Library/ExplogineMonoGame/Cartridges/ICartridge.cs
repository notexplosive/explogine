using ExplogineMonoGame.HitTesting;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

public interface ICartridge
{
    CartridgeConfig CartridgeConfig { get; }
    void OnCartridgeStarted();
    void Update(float dt);
    void Draw(Painter painter);
    void UpdateInput(InputFrameState input, HitTestStack hitTestStack);
    bool ShouldLoadNextCartridge();
    void Unload();
}
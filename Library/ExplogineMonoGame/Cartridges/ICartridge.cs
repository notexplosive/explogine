namespace ExplogineMonoGame.Cartridges;

public interface ICartridge : IUpdateInput
{
    CartridgeConfig CartridgeConfig { get; }
    void OnCartridgeStarted();
    void Update(float dt);
    void Draw(Painter painter);
    bool ShouldLoadNextCartridge();
    void Unload();
}

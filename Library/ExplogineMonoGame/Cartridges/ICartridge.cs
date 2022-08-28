namespace ExplogineMonoGame.Cartridges;

public interface ICartridge
{
    public CartridgeConfig CartridgeConfig { get; }
    public void OnCartridgeStarted();
    public void Update(float dt);
    public void Draw(Painter painter);
    public bool ShouldLoadNextCartridge();
}
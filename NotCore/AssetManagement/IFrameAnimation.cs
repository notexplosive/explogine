namespace NotCore.AssetManagement;

public interface IFrameAnimation
{
    public int Length { get; }
    public bool Loop { get; }
    public int GetFrame(float elapsedTime);
}

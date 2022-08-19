namespace ExplogineMonoGame.AssetManagement;

public interface IAsset
{
    public string Key { get; }
}

public abstract class Asset : IAsset
{
    public string Key { get; }

    protected Asset(string key)
    {
        Key = key;
    }
}
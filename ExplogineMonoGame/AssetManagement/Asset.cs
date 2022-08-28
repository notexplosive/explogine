using System;

namespace ExplogineMonoGame.AssetManagement;

public abstract class Asset : IDisposable
{
    private readonly IDisposable? _ownedContent;

    protected Asset(IDisposable? ownedContent)
    {
        _ownedContent = ownedContent;
    }

    public void Dispose()
    {
        _ownedContent?.Dispose();
    }
}

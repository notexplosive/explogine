using System;

namespace ExplogineMonoGame.Cartridges;

public class IntroCartridge : ICartridge
{
    public void Update(float dt)
    {
        Client.Debug.Log("Made with NotFramework by NotExplosive");
        Client.Debug.Log("-- NotExplosive.net --");
    }

    public void Draw(Painter painter)
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return true;
    }

    public void OnCartridgeStarted()
    {
    }
}

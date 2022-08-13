using System;

namespace NotCore;

public class IntroCartridge : ICartridge
{
    public void Update(float dt)
    {
        Console.WriteLine("Made with NotFramework by NotExplosive");
        Console.WriteLine("-- NotExplosive.net --");
    }

    public void Draw(Painter painter)
    {
    }

    public bool ShouldLoadNextCartridge()
    {
        return true;
    }
}

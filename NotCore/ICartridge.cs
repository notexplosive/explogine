using System;
using System.Collections;
using System.Collections.Generic;

namespace NotCore;

public interface ICartridge
{
    public void Update(float dt);
    public void Draw(Painter painter);
    public bool ShouldLoadNextCartridge();
}

public interface ICartridgeWithPreload : ICartridge
{
    public IEnumerable<Loader.LoadEvent> Preload(Painter painter);
}
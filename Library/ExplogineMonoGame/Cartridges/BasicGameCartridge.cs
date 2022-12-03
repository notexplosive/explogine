﻿using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame.Cartridges;

public abstract class BasicGameCartridge : ICartridge, ILoadEventProvider, ICommandLineParameterProvider
{
    public abstract void OnCartridgeStarted();
    public abstract void Update(float dt);
    public abstract void Draw(Painter painter);
    public abstract void UpdateInput(AllDeviceFrameState input);

    public bool ShouldLoadNextCartridge()
    {
        return false;
    }

    public abstract CartridgeConfig CartridgeConfig { get; }
    public abstract void AddCommandLineParameters(CommandLineParametersWriter parameters);
    public abstract IEnumerable<ILoadEvent?> LoadEvents(Painter painter);
}

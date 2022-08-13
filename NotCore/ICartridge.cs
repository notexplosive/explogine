﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace NotCore;

public interface ICartridge
{
    public void Update(float dt);
    public void Draw(Painter painter);
    public bool ShouldLoadNextCartridge();
}

public interface ILoadEventProvider
{
    public IEnumerable<Loader.LoadEvent> LoadEvents(Painter painter);
}

public interface ICommandLineParameterProvider
{
    public void SetupFormalParameters(CommandLineArguments args);
}
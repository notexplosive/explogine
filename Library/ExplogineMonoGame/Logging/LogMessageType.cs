using System;

namespace ExplogineMonoGame.Logging;

[Flags]
public enum LogMessageType
{
    Verbose = 1,
    Info = 2,
    Warning = 4,
    Error = 8
}

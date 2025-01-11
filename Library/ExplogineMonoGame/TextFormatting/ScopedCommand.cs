using System;

namespace ExplogineMonoGame.TextFormatting;

public record ScopedCommand(Func<string[], Instruction> CreatePush, Func<Instruction> CreatePop) : ICommand;

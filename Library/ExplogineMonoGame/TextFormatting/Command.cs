using System;

namespace ExplogineMonoGame.TextFormatting;

public record Command(Func<string[], Instruction> Create) : ICommand;

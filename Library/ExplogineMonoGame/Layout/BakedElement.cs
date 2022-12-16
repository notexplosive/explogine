using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Layout;

public readonly record struct BakedElement(RectangleF Rectangle, string Name, int NestingLevel);

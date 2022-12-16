using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Layout;

public readonly record struct BakedLayoutElement(RectangleF Rectangle, string Name, int NestingLevel);

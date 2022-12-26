namespace ExplogineCore.Data;

public record struct XyBool(bool X, bool Y)
{
    public static XyBool True => new(true, true);
    public static XyBool False => new(false, false);
}

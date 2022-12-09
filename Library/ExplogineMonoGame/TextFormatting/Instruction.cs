using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.TextFormatting;

public abstract class Instruction
{
    public static implicit operator Instruction(string str)
    {
        return new StringLiteralInstruction(str);
    }
    
    public static implicit operator Instruction(StaticImageAsset asset)
    {
        return new ImageLiteralInstruction(asset);
    }
    
    public static implicit operator Instruction(Texture2D texture)
    {
        return new TextureLiteralInstruction(texture);
    }
}
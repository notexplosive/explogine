using ExplogineCore.Lua;
using ExplogineMonoGame.Luigi.Description;
using JetBrains.Annotations;

namespace ExplogineMonoGame.Luigi;

public class LuaLayoutElement
{
    private readonly LuaLayoutBuilderBlock _owningGroup;
    private readonly string _currentElementName;

    public LuaLayoutElement(LuaLayoutBuilderBlock owningGroup, string currentElementName)
    {
        _owningGroup = owningGroup;
        _currentElementName = currentElementName;
    }
    
    [UsedImplicitly]
    [LuaMember("decorate")]
    public LuaLayoutElement Decorate(IGuiDescription? guiDescription)
    {
        if (guiDescription == null)
        {
            Client.Debug.LogError("Attempted to Decorate with a null");
            return this;
        }
        
        _owningGroup.AddInstruction(new GuiLayoutInstruction(_currentElementName, guiDescription));
        return this;
    }
}

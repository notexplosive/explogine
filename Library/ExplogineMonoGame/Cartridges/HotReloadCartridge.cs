using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Cartridges;

public class HotReloadCartridge : MultiCartridge
{
    public HotReloadCartridge(IRuntime runtime, params Cartridge[] startingCartridges) : base(runtime, startingCartridges)
    {
    }

    protected override void BeforeUpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (Client.Debug.IsPassiveOrActive && input.Keyboard.Modifiers.Control && input.Keyboard.GetButton(Keys.R, true).WasPressed)
        {
            RegenerateCurrentCartridge();
            
            if (CurrentCartridge is IHotReloadable hotReloadable)
            {
                hotReloadable.OnHotReload();
            }
        }
    }
}
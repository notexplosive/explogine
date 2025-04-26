using System;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Cartridges;

public class HotReloadCartridge : MultiCartridge
{
    public HotReloadCartridge(IRuntime runtime, params Cartridge[] startingCartridges) : base(runtime,
        startingCartridges)
    {
    }

    protected override void BeforeUpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var ctrl = input.Keyboard.Modifiers.Control;
        var ctrlShift = input.Keyboard.Modifiers.ControlShift;
        if (Client.Debug.IsPassiveOrActive && (ctrl || ctrlShift) && input.Keyboard.GetButton(Keys.R, true).WasPressed)
        {
            GC.Collect();

            if (ctrl)
            {
                var cartridge = CurrentCartridge;
                RegenerateCurrentCartridge();
                HotReload(cartridge);
            }
            else if (ctrlShift)
            {
                // Mega-reload, restart from scratch
                for (var i = 0; i < TotalCartridgeCount; i++)
                {
                    var cartridge = GetCartridgeAt(i);
                    RegenerateCartridge(i);
                    HotReload(cartridge);
                }

                // hot reload self (does not actually regen own cartridge)
                OnHotReload();
                
                Client.FinishedLoading.Add(() =>
                {
                    SwapTo(0);
                });
            }
        }
    }

    private static void HotReload(Cartridge? cartridge)
    {
        if (cartridge is IHotReloadable hotReloadable)
        {
            hotReloadable.OnHotReload();
        }
    }
}

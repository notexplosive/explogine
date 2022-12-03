using System;
using System.Linq;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

public class FrameStep
{
    public void Update(InputFrameState InputFrameState)
    {
        if (Client.Debug.IsPassiveOrActive)
        {
            if (Client.CartridgeChain.IsFrozen)
            {
                if (InputFrameState.Mouse.ScrollDelta() < 0 || InputFrameState.Keyboard.GetButton(Keys.OemPipe).WasPressed)
                {
                    Client.CartridgeChain.UpdateCurrentCartridge(1 / 60f);
                }
            }

            if (InputFrameState.Keyboard.GetButton(Keys.Space).WasPressed && InputFrameState.Keyboard.Modifiers.Control)
            {
                Client.CartridgeChain.IsFrozen = !Client.CartridgeChain.IsFrozen;
                var enabledString = Client.CartridgeChain.IsFrozen ? "Frozen" : "Unfrozen";
                Client.Debug.Log($"FrameStep {enabledString}");
            }
        }
    }
}

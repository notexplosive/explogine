using System;
using System.Linq;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

public class FrameStep
{
    public void Update(AllDeviceFrameState allDeviceFrameState)
    {
        if (Client.Debug.IsPassiveOrActive)
        {
            if (Client.CartridgeChain.IsFrozen)
            {
                if (allDeviceFrameState.Mouse.ScrollDelta() < 0 || allDeviceFrameState.Keyboard.GetButton(Keys.OemPipe).WasPressed)
                {
                    Client.CartridgeChain.UpdateCurrentCartridge(1 / 60f);
                }
            }

            if (allDeviceFrameState.Keyboard.GetButton(Keys.Space).WasPressed && allDeviceFrameState.Keyboard.Modifiers.Control)
            {
                Client.CartridgeChain.IsFrozen = !Client.CartridgeChain.IsFrozen;
                var enabledString = Client.CartridgeChain.IsFrozen ? "Frozen" : "Unfrozen";
                Client.Debug.Log($"FrameStep {enabledString}");
            }
        }
    }
}

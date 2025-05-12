using System.Collections.Generic;
using System.Diagnostics.Contracts;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

public class ConsumableInput
{
    private ConsumableInput(ConsumableKeyboard keyboard, ConsumableMouse mouse, GamePadFrameState gamePad)
    {
        Keyboard = keyboard;
        Mouse = mouse;
        GamePad = gamePad;
    }

    public ConsumableInput(InputFrameState raw) : this(new ConsumableKeyboard(raw.Keyboard),
        new ConsumableMouse(raw.Mouse), raw.GamePad)
    {
    }

    public ConsumableKeyboard Keyboard { get; }
    public ConsumableMouse Mouse { get; }

    /// <summary>
    ///     GamePad is not consumable, we just forward the raw frame state
    /// </summary>
    public GamePadFrameState GamePad { get; }

    public ConsumableInput MouseOnly()
    {
        var emptyKeyboard = new KeyboardFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        return new ConsumableInput(new ConsumableKeyboard(emptyKeyboard), Mouse,
            new GamePadFrameState(InputSnapshot.Empty, InputSnapshot.Empty));
    }

    public class ConsumableMouse
    {
        private readonly HashSet<MouseButton> _consumedButtons = new();
        private readonly MouseFrameState _raw;
        private bool _scrollDeltaIsConsumed;

        public ConsumableMouse(MouseFrameState raw)
        {
            _raw = raw;
        }

        public ButtonFrameState GetButton(MouseButton button, bool shouldConsume = false)
        {
            var result = _consumedButtons.Contains(button)
                ? new ButtonFrameState(false, false)
                : _raw.GetButton(button);

            if (shouldConsume)
            {
                Consume(button);
            }

            return result;
        }

        [Pure]
        public Vector2 Position(Matrix? toCanvas = null)
        {
            return _raw.Position(toCanvas);
        }

        public int ScrollDelta(bool shouldConsume = false)
        {
            if (_scrollDeltaIsConsumed)
            {
                return 0;
            }

            if (shouldConsume)
            {
                _scrollDeltaIsConsumed = true;
            }

            return _raw.ScrollDelta();
        }

        public int NormalizedScrollDelta(bool shouldConsume = false)
        {
            var scrollVector = new Vector2(0, ScrollDelta(shouldConsume));
            if (scrollVector.Y != 0)
            {
                return (int) scrollVector.Normalized().Y;
            }

            return 0;
        }

        public void ConsumeScrollDelta()
        {
            _scrollDeltaIsConsumed = true;
        }

        public Vector2 Delta(Matrix? toCanvas = null)
        {
            return _raw.Delta(toCanvas);
        }

        public IEnumerable<(ButtonFrameState, MouseButton)> EachButton()
        {
            foreach (var key in _raw.EachButton())
            {
                if (_consumedButtons.Contains(key.Item2))
                {
                    yield return (ButtonFrameState.Empty, key.Item2);
                }
                else
                {
                    yield return key;
                }
            }
        }

        public bool WasAnyButtonPressedOrReleased()
        {
            return _raw.WasAnyButtonPressedOrReleased();
        }

        public void Consume(MouseButton button)
        {
            _consumedButtons.Add(button);
        }
    }

    public class ConsumableKeyboard
    {
        private readonly HashSet<Keys> _consumedKeys = new();
        private readonly HashSet<char> _consumedTextInput = new();
        private readonly KeyboardFrameState _raw;

        public ConsumableKeyboard(KeyboardFrameState raw)
        {
            _raw = raw;
        }

        public ModifierKeys Modifiers => _raw.Modifiers;

        public ButtonFrameState GetButton(Keys keys, bool shouldConsume = false)
        {
            var result = _consumedKeys.Contains(keys) ? ButtonFrameState.Empty : _raw.GetButton(keys);
            if (shouldConsume)
            {
                Consume(keys);
            }

            return result;
        }

        public char[] GetEnteredCharacters(bool shouldConsume = false)
        {
            var result = new List<char>();

            foreach (var character in _raw.GetEnteredCharacters())
            {
                if (!_consumedTextInput.Contains(character))
                {
                    result.Add(character);
                }

                if (shouldConsume)
                {
                    ConsumeTextInput(character);
                    var key = InputUtil.CharToKeys(character);
                    if (key.HasValue)
                    {
                        Consume(key.Value);
                    }
                }
            }

            return result.ToArray();
        }

        public IEnumerable<(ButtonFrameState, Keys)> EachKey()
        {
            foreach (var key in _raw.EachKey())
            {
                if (_consumedKeys.Contains(key.Item2))
                {
                    yield return (ButtonFrameState.Empty, key.Item2);
                }
                else
                {
                    yield return key;
                }
            }
        }

        public bool IsAnyKeyDown()
        {
            return _raw.IsAnyKeyDown();
        }

        public void Consume(Keys keys)
        {
            _consumedKeys.Add(keys);
        }

        public void ConsumeTextInput(char c)
        {
            _consumedTextInput.Add(c);

            var keys = InputUtil.CharToKeys(c);
            if (keys.HasValue && !_consumedKeys.Contains(keys.Value))
            {
                Consume(keys.Value);
            }
        }
    }
}

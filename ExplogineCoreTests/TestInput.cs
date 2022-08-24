using System;
using ExplogineMonoGame.Input;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace ExplogineCoreTests;

public class TestInput
{
    [Fact]
    public void nothing_pressed_by_default()
    {
        var input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        input.Keyboard.IsAnyKeyDown().Should().BeFalse();
        input.Mouse.IsAnyButtonDown().Should().BeFalse();
        input.GamePad.IsAnyButtonDownOnAnyGamePad().Should().BeFalse();
    }

    [Fact]
    public void button_pressed()
    {
        var input = new InputFrameState(
            new InputSnapshot(
                new KeyboardState(Keys.Space),
                new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                    ButtonState.Released, ButtonState.Released),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.A),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.B),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.X),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.Y),
                    new GamePadDPad())
            ),
            InputSnapshot.Empty
        );

        input.Keyboard.IsAnyKeyDown().Should().BeTrue();
        input.Mouse.IsAnyButtonDown().Should().BeTrue();
        input.GamePad.IsAnyButtonDown(PlayerIndex.One).Should().BeTrue();
        input.GamePad.IsAnyButtonDownOnAnyGamePad().Should().BeTrue();

        input.Keyboard.GetButton(Keys.Space).IsDown.Should().BeTrue();
        input.Mouse.GetButton(MouseButton.Left).IsDown.Should().BeTrue();
        input.GamePad.GetButton(GamePadButton.A,PlayerIndex.One).IsDown.Should().BeTrue();

        input.Keyboard.GetButton(Keys.Space).WasPressed.Should().BeTrue();
        input.Mouse.GetButton(MouseButton.Left).WasPressed.Should().BeTrue();
        input.GamePad.GetButton(GamePadButton.A,PlayerIndex.One).WasPressed.Should().BeTrue();
    }

    [Fact]
    public void button_released()
    {
        var input = new InputFrameState(
            InputSnapshot.Empty,
            new InputSnapshot(
                new KeyboardState(Keys.Space),
                new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                    ButtonState.Released, ButtonState.Released),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.A),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.B),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.X),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.Y),
                    new GamePadDPad())
            )
        );

        input.Keyboard.IsAnyKeyDown().Should().BeFalse();
        input.Mouse.IsAnyButtonDown().Should().BeFalse();
        input.GamePad.IsAnyButtonDownOnAnyGamePad().Should().BeFalse();

        input.Keyboard.GetButton(Keys.Space).IsDown.Should().BeFalse();
        input.Mouse.GetButton(MouseButton.Left).IsDown.Should().BeFalse();
        input.GamePad.GetButton(GamePadButton.A,PlayerIndex.One).IsDown.Should().BeFalse();
        input.GamePad.GetButton(GamePadButton.B,PlayerIndex.Two).IsDown.Should().BeFalse();
        input.GamePad.GetButton(GamePadButton.X,PlayerIndex.Three).IsDown.Should().BeFalse();
        input.GamePad.GetButton(GamePadButton.Y,PlayerIndex.Four).IsDown.Should().BeFalse();

        input.Keyboard.GetButton(Keys.Space).WasReleased.Should().BeTrue();
        input.Mouse.GetButton(MouseButton.Left).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(GamePadButton.A,PlayerIndex.One).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(GamePadButton.B,PlayerIndex.Two).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(GamePadButton.X,PlayerIndex.Three).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(GamePadButton.Y,PlayerIndex.Four).WasReleased.Should().BeTrue();
    }

    [Fact]
    public void serialize_snapshot()
    {
        var before = new InputSnapshot(
            new KeyboardState(Keys.Space, Keys.A),
            new MouseState(
                0, 0, 0,
                ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                ButtonState.Released, ButtonState.Released),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.A),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.B),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.X),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.Y),
                new GamePadDPad())
        );

        var bytes = InputSerialization.AsString(before);
        var after = new InputSnapshot(bytes);
        
        after.MouseButtonStates.Should().BeEquivalentTo(before.MouseButtonStates);
        after.PressedKeys.Should().BeEquivalentTo(before.PressedKeys);
        after.MousePosition.Should().Be(before.MousePosition);

        foreach (var playerIndex in Enum.GetValues<PlayerIndex>())
        {
            after.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates.Should().BeEquivalentTo(before.GamePadSnapshotOfPlayer(playerIndex).GamePadButtonStates);
            after.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick.Should().Be(before.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick);
            after.GamePadSnapshotOfPlayer(playerIndex).GamePadLeftTrigger.Should().Be(before.GamePadSnapshotOfPlayer(playerIndex).GamePadLeftTrigger);
            after.GamePadSnapshotOfPlayer(playerIndex).GamePadRightTrigger.Should().Be(before.GamePadSnapshotOfPlayer(playerIndex).GamePadRightTrigger);
        }
    }

    [Fact]
    public void serialization_output()
    {
        var snapshot = new InputSnapshot(
            new KeyboardState(Keys.Space, Keys.A),
            new MouseState(
                0, 0, 0,
                ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                ButtonState.Released, ButtonState.Released),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.A),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.B),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.X),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.Y),
                new GamePadDPad())
        );

        snapshot.Serialize().Should().Be("M:0,0,1|K:32,65|G:1,0.5,1,0,0,1,1|G:1,0.5,1,0,0,1,2|G:1,0.5,1,0,0,1,4|G:1,0.5,1,0,0,1,8");
    }

    [Fact]
    public void serialization_sanity_check()
    {
        var buttonStates = InputSerialization.IntToStates(1, 5);

        buttonStates.Length.Should().Be(5);
        buttonStates[0].Should().Be(ButtonState.Pressed);
        buttonStates[1].Should().Be(ButtonState.Released);
    }
}

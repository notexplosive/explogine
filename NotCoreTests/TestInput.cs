using System;
using ExplogineMonoGame.Input;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace NotCoreTests;

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

        input.Keyboard.IsKeyDown(Keys.Space).Should().BeTrue();
        input.Mouse.IsButtonDown(MouseButton.Left).Should().BeTrue();
        input.GamePad.IsButtonDown(GamePadButton.A, PlayerIndex.One).Should().BeTrue();

        input.Keyboard.WasKeyPressed(Keys.Space).Should().BeTrue();
        input.Mouse.WasButtonPressed(MouseButton.Left).Should().BeTrue();
        input.GamePad.WasButtonPressed(GamePadButton.A, PlayerIndex.One).Should().BeTrue();
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

        input.Keyboard.IsKeyDown(Keys.Space).Should().BeFalse();
        input.Mouse.IsButtonDown(MouseButton.Left).Should().BeFalse();
        input.GamePad.IsButtonDown(GamePadButton.A, PlayerIndex.One).Should().BeFalse();
        input.GamePad.IsButtonDown(GamePadButton.B, PlayerIndex.Two).Should().BeFalse();
        input.GamePad.IsButtonDown(GamePadButton.X, PlayerIndex.Three).Should().BeFalse();
        input.GamePad.IsButtonDown(GamePadButton.Y, PlayerIndex.Four).Should().BeFalse();

        input.Keyboard.WasKeyReleased(Keys.Space).Should().BeTrue();
        input.Mouse.WasButtonReleased(MouseButton.Left).Should().BeTrue();
        input.GamePad.WasButtonReleased(GamePadButton.A, PlayerIndex.One).Should().BeTrue();
        input.GamePad.WasButtonReleased(GamePadButton.B, PlayerIndex.Two).Should().BeTrue();
        input.GamePad.WasButtonReleased(GamePadButton.X, PlayerIndex.Three).Should().BeTrue();
        input.GamePad.WasButtonReleased(GamePadButton.Y, PlayerIndex.Four).Should().BeTrue();
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

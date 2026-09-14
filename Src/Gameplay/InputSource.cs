using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

public class InputSource
{
    public bool IsGamepad { get; }

    public Keys LeftKey { get; }
    public Keys RightKey { get; }
    public Keys DownKey { get; }
    public Keys UpKey { get; }
    public PlayerIndex GamepadIndex { get; }

    private InputSource(bool isGamepad, Keys leftKey, Keys rightKey, Keys downKey, Keys upKey, PlayerIndex gamepadIndex)
    {
        IsGamepad = isGamepad;
        LeftKey = leftKey;
        RightKey = rightKey;
        DownKey = downKey;
        UpKey = upKey;
        GamepadIndex = gamepadIndex;
    }

    public static InputSource FromKeyboard(Keys leftKey, Keys rightKey)
    {
        bool isWasd = leftKey == Keys.A;
        Keys downKey = isWasd ? Keys.S : Keys.Down;
        Keys upKey = isWasd ? Keys.W : Keys.Up;

        return new InputSource(false, leftKey, rightKey, downKey, upKey, PlayerIndex.One);
    }

    public static InputSource FromGamepad(PlayerIndex gamepadIndex)
    {
        return new InputSource(true, Keys.None, Keys.None, Keys.None, Keys.None, gamepadIndex);
    }


    public bool IsLeftPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadLeft) || pad.ThumbSticks.Left.X < -0.5f;
        }

        return Keyboard.GetState().IsKeyDown(LeftKey);
    }

    public bool IsRightPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadRight) || pad.ThumbSticks.Left.X > 0.5f;
        }

        return Keyboard.GetState().IsKeyDown(RightKey);
    }

    public bool IsDownPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadDown) || pad.ThumbSticks.Left.Y < -0.5f;
        }

        return Keyboard.GetState().IsKeyDown(DownKey);
    }


    public bool IsUpPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadUp) || pad.ThumbSticks.Left.Y > 0.5f;
        }

        return Keyboard.GetState().IsKeyDown(UpKey);
    }

    public float GetAnalogAxis()
    {
        if (IsGamepad)
        {
            return GamePad.GetState(GamepadIndex).ThumbSticks.Left.X;
        }

        return 0f;
    }
}
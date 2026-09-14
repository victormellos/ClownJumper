using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

public class MenuNavigator
{
    public bool LeftPressed { get; private set; }
    public bool RightPressed { get; private set; }
    public bool ConfirmPressed { get; private set; }
    public bool CancelPressed { get; private set; }

    private bool _wasLeftDown;
    private bool _wasRightDown;
    private bool _wasConfirmDown;
    private bool _wasCancelDown;

    public void Update()
    {
        bool leftDown = IsLeftDown();
        bool rightDown = IsRightDown();
        bool confirmDown = IsConfirmDown();
        bool cancelDown = IsCancelDown();

        LeftPressed = leftDown && !_wasLeftDown;
        RightPressed = rightDown && !_wasRightDown;
        ConfirmPressed = confirmDown && !_wasConfirmDown;
        CancelPressed = cancelDown && !_wasCancelDown;

        _wasLeftDown = leftDown;
        _wasRightDown = rightDown;
        _wasConfirmDown = confirmDown;
        _wasCancelDown = cancelDown;
    }

    private static bool IsLeftDown()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            return true;

        return AnyGamepad(pad => pad.IsButtonDown(Buttons.DPadLeft) || pad.ThumbSticks.Left.X < -0.5f);
    }

    private static bool IsRightDown()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            return true;

        return AnyGamepad(pad => pad.IsButtonDown(Buttons.DPadRight) || pad.ThumbSticks.Left.X > 0.5f);
    }

    private static bool IsConfirmDown()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
            return true;

        return AnyGamepad(pad => pad.IsButtonDown(Buttons.DPadDown) || pad.ThumbSticks.Left.Y < -0.5f);
    }

    private static bool IsCancelDown()
    {
        var keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
            return true;

        return AnyGamepad(pad => pad.IsButtonDown(Buttons.DPadUp) || pad.ThumbSticks.Left.Y > 0.5f);
    }


    private static bool AnyGamepad(System.Func<GamePadState, bool> predicate)
    {
        for (int i = 0; i < GamePad.MaximumGamePadCount; i++)
        {
            var state = GamePad.GetState((PlayerIndex)i);

            if (state.IsConnected && predicate(state))
                return true;
        }

        return false;
    }
}
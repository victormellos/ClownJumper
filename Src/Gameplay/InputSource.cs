using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

/// <summary>
/// Representa de onde vem o input de um jogador: teclado (com um par de
/// teclas específico) ou um gamepad (com seu índice). Criado na tela de
/// seleção de jogadores (PlayerJoinScreen) e depois repassado ao Player.
/// </summary>
public class InputSource
{
    public bool IsGamepad { get; }

    public Keys LeftKey { get; }
    public Keys RightKey { get; }
    public Keys DownKey { get; }
    public PlayerIndex GamepadIndex { get; }

    private InputSource(bool isGamepad, Keys leftKey, Keys rightKey, Keys downKey, PlayerIndex gamepadIndex)
    {
        IsGamepad = isGamepad;
        LeftKey = leftKey;
        RightKey = rightKey;
        DownKey = downKey;
        GamepadIndex = gamepadIndex;
    }

    public static InputSource FromKeyboard(Keys leftKey, Keys rightKey)
    {
        // WASD usa S como "baixo"; Setas usa a seta para baixo. Inferimos
        // pelo par de teclas recebido, para não ter que mudar todo lugar
        // que já chama FromKeyboard(Keys.A, Keys.D) ou (Keys.Left, Keys.Right).
        Keys downKey = leftKey == Keys.A ? Keys.S : Keys.Down;

        return new InputSource(false, leftKey, rightKey, downKey, PlayerIndex.One);
    }

    public static InputSource FromGamepad(PlayerIndex gamepadIndex)
    {
        return new InputSource(true, Keys.None, Keys.None, Keys.None, gamepadIndex);
    }

    /// <summary>
    /// Verdadeiro se essa fonte de input está "apertando esquerda" agora
    /// (usado tanto para o join quanto para o movimento no jogo).
    /// </summary>
    public bool IsLeftPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadLeft) || pad.ThumbSticks.Left.X < -0.5f;
        }

        return Keyboard.GetState().IsKeyDown(LeftKey);
    }

    /// <summary>
    /// Verdadeiro se essa fonte de input está "apertando direita" agora.
    /// </summary>
    public bool IsRightPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadRight) || pad.ThumbSticks.Left.X > 0.5f;
        }

        return Keyboard.GetState().IsKeyDown(RightKey);
    }

    /// <summary>
    /// Verdadeiro se essa fonte de input está "apertando baixo" agora
    /// (usado como confirmação universal, ex.: na seleção de cor).
    /// </summary>
    public bool IsDownPressed()
    {
        if (IsGamepad)
        {
            var pad = GamePad.GetState(GamepadIndex);
            return pad.IsButtonDown(Buttons.DPadDown) || pad.ThumbSticks.Left.Y < -0.5f;
        }

        return Keyboard.GetState().IsKeyDown(DownKey);
    }

    /// <summary>
    /// Eixo analógico horizontal (-1 a 1) desta fonte, usado para o
    /// movimento suave do trampolim quando vem de um gamepad.
    /// Teclado não tem analógico, então sempre retorna 0 aqui.
    /// </summary>
    public float GetAnalogAxis()
    {
        if (IsGamepad)
        {
            return GamePad.GetState(GamepadIndex).ThumbSticks.Left.X;
        }

        return 0f;
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

public class Player
{
    public Character Trampoline { get; private set; }
    public Character Clown { get; private set; }

    public Score Score;

    public string Name;
    public int Lives;

    /// <summary>
    /// Cor usada para tingir o sprite do palhaço (que tem partes brancas
    /// justamente para isso). Jogador 1 = azul, Jogador 2 = vermelho.
    /// </summary>
    public Color TintColor = Color.White;

    /// <summary>
    /// Temporizador de respawn deste jogador. Cada jogador tem o seu,
    /// para respawns não interferirem entre jogadores diferentes.
    /// </summary>
    public Delay RespawnTimer { get; } = new Delay(500.0);

    private KeyboardState _keyboard;
    private GamePadState _gamepad;

    private bool _isAlive = true;
    public bool IsAlive
    {
        get => _isAlive;
        set
        {
            _isAlive = value;

            if (!_isAlive)
            {
                OnDeath();
            }
        }
    }

    private readonly Keys _leftKey;
    private readonly Keys _rightKey;
    private readonly PlayerIndex _playerIndex;

    public Player(Character trampoline, Character clown, Keys left, Keys right, PlayerIndex playerIndex, string name = "Jogador", int lives = 5)
    {
        Trampoline = trampoline;
        Clown = clown;

        Score = new Score();

        Name = name;
        Lives = lives;

        _leftKey = left;
        _rightKey = right;

        _playerIndex = playerIndex;
    }

    public void UpdateInput()
    {
        _keyboard = Keyboard.GetState();
        _gamepad = GamePad.GetState(_playerIndex);
    }

    public void HandleInput()
    {
        if (!IsAlive)
            return;

        if (_keyboard.IsKeyDown(_leftKey))
            Trampoline.Position.X -= 10f;

        if (_keyboard.IsKeyDown(_rightKey))
            Trampoline.Position.X += 10f;

        Trampoline.Position.X += _gamepad.ThumbSticks.Left.X * 15f;
    }

    private void OnDeath()
    {
        Clown = null;
        Score.ResetCombo();
    }
}

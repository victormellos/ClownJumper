using Microsoft.Xna.Framework;

namespace ClownJumper;

public class Player
{
    public Character Trampoline { get; private set; }
    public Character Clown { get; private set; }

    public Score Score;

    public string Name;

    /// <summary>
    /// Time compartilhado deste jogador (usado no modo Coop). Quando presente,
    /// as vidas passam a ser lidas/escritas do time em vez de um valor próprio.
    /// </summary>
    public TeamState Team;

    private int _lives;
    public int Lives
    {
        get => Team != null ? Team.Lives : _lives;
        set
        {
            if (Team != null)
                Team.Lives = value;
            else
                _lives = value;
        }
    }

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

    private readonly InputSource _inputSource;

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

    public Player(Character trampoline, Character clown, InputSource inputSource, string name = "Jogador", int lives = 5)
    {
        Trampoline = trampoline;
        Clown = clown;

        Score = new Score();

        Name = name;
        Lives = lives;

        _inputSource = inputSource;
    }

    public void UpdateInput()
    {
        // A leitura em si é feita sob demanda pelo InputSource (Keyboard.GetState()
        // e GamePad.GetState() já são baratos de chamar a cada frame), então não
        // há estado para cachear aqui além do que o InputSource já resolve.
    }

    public void HandleInput()
    {
        if (!IsAlive)
            return;

        if (_inputSource.IsLeftPressed())
            Trampoline.Position.X -= 10f;

        if (_inputSource.IsRightPressed())
            Trampoline.Position.X += 10f;

        Trampoline.Position.X += _inputSource.GetAnalogAxis() * 15f;
    }

    private void OnDeath()
    {
        Clown = null;
        Score.ResetCombo();
    }
}
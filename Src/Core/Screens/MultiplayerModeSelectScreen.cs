using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

/// <summary>
/// Tela exibida depois que os 2 jogadores confirmaram entrada no
/// multiplayer (PlayerJoinScreen). Aqui escolhem entre Coop e VS, e o
/// GameScreen é criado já com as fontes de input de cada jogador.
/// </summary>
public class MultiplayerModeSelectScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private readonly InputSource _player1Source;
    private readonly InputSource _player2Source;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private Texture2D _background;

    private KeyboardState _previousKeyboard;

    public MultiplayerModeSelectScreen(
        GraphicsDevice graphicsDevice,
        ContentManager content,
        ScreenManager screenManager,
        InputSource player1Source,
        InputSource player2Source)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
        _player1Source = player1Source;
        _player2Source = player2Source;
    }

    public void Initialize()
    {
    }

    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);
        _textFont = _content.Load<SpriteFont>("fonts/ScoreFont");
        _background = _content.Load<Texture2D>("images/background");
    }

    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        bool coopPressed = IsKeyPressedThisFrame(keyboard, Keys.D1);
        bool versusPressed = IsKeyPressedThisFrame(keyboard, Keys.D2);

        if (coopPressed)
        {
            _screenManager.RequestScreenChange(
                new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Coop, _player1Source, _player2Source));
        }
        else if (versusPressed)
        {
            _screenManager.RequestScreenChange(
                new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Versus, _player1Source, _player2Source));
        }

        _previousKeyboard = keyboard;
    }

    private bool IsKeyPressedThisFrame(KeyboardState current, Keys key)
    {
        return current.IsKeyDown(key) && !_previousKeyboard.IsKeyDown(key);
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.YellowGreen);

        _spriteBatch.Begin();

        _spriteBatch.Draw(
            _background,
            new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height),
            Color.White);

        _spriteBatch.DrawString(
            _textFont,
            "ESCOLHA O MODO MULTIPLAYER",
            new Vector2(0, 0),
            Color.Black);

        string texto = "1 - Coop        2 - Versus";
        Vector2 tamanho = _textFont.MeasureString(texto);

        float posX = (_graphicsDevice.Viewport.Width - tamanho.X) / 2;
        float posY = _graphicsDevice.Viewport.Height - tamanho.Y - 10;

        _spriteBatch.DrawString(
            _textFont,
            texto,
            new Vector2(posX, posY),
            Color.Black);

        _spriteBatch.End();
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

/// <summary>
/// Tela exibida depois do "OK" no menu principal, onde o jogador escolhe
/// entre jogar Solo ou ir para o fluxo de Multiplayer (seleção de
/// jogadores e depois Coop/VS).
/// </summary>
public class ModeSelectScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private Texture2D _background;

    private KeyboardState _previousKeyboard;

    public ModeSelectScreen(GraphicsDevice graphicsDevice, ContentManager content, ScreenManager screenManager)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
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

        bool soloPressed = IsKeyPressedThisFrame(keyboard, Keys.D1);
        bool multiplayerPressed = IsKeyPressedThisFrame(keyboard, Keys.D2);
        bool trainingPressed = IsKeyPressedThisFrame(keyboard, Keys.D3);

        if (soloPressed)
        {
            _screenManager.RequestScreenChange(
                new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Solo));
        }
        else if (multiplayerPressed)
        {
            _screenManager.RequestScreenChange(
                new PlayerJoinScreen(_graphicsDevice, _content, _screenManager));
        }
        else if (trainingPressed)
        {
            _screenManager.RequestScreenChange(
                new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Training));
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
            "ESCOLHA O MODO",
            new Vector2(0, 0),
            Color.Black);

        string texto = "1 - Solo        2 - Multiplayer        3 - Treinamento";
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
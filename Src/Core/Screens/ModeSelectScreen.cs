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
    private RotatingBackground _background;

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
        _background = new RotatingBackground(_content);
    }

    public void Update(GameTime gameTime)
    {
        _background.Update(gameTime);

        var keyboard = Keyboard.GetState();

        bool soloPressed = IsKeyPressedThisFrame(keyboard, Keys.D1);
        bool multiplayerPressed = IsKeyPressedThisFrame(keyboard, Keys.D2);

        if (soloPressed)
        {
            _screenManager.RequestScreenChange(
                new SoloModeSelectScreen(_graphicsDevice, _content, _screenManager));
        }
        else if (multiplayerPressed)
        {
            _screenManager.RequestScreenChange(
                new PlayerJoinScreen(_graphicsDevice, _content, _screenManager));
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

        _background.Draw(_spriteBatch, _graphicsDevice);

        _spriteBatch.DrawString(
            _textFont,
            "ESCOLHA O MODO",
            new Vector2(0, 0),
            Color.Black);

        string texto = "1 - Solo        2 - Multiplayer";
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
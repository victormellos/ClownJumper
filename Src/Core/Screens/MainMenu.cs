using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VictorMellos;

public class MainMenu : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private Texture2D _background;

    private KeyboardState _previousKeyboard;

    public MainMenu(GraphicsDevice graphicsDevice, ContentManager content, ScreenManager screenManager)
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

        bool startPressed =
            IsKeyPressedThisFrame(keyboard, Keys.Enter) ||
            IsKeyPressedThisFrame(keyboard, Keys.Space);

        if (startPressed)
        {
            _screenManager.RequestScreenChange(
                new GameScreen(_graphicsDevice, _content, _screenManager));
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
            "JOGO LEGAL DO PALHACO FELIZ",
            new Vector2(0, 0),
            Color.Black);

        string texto = "Pressione ENTER ou ESPACO para jogar";
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

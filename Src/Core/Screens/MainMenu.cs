using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;

public class MainMenu : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private RotatingBackground _background;

    private readonly MenuNavigator _navigator = new();

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
        _background = new RotatingBackground(_content);
    }

    public void Update(GameTime gameTime)
    {
        _background.Update(gameTime);
        _navigator.Update();

        if (_navigator.ConfirmPressed)
        {
            _screenManager.RequestScreenChange(BuildModeSelectScreen());
        }
    }

    private IScreen BuildModeSelectScreen()
    {
        return new ChoiceScreen(
            _graphicsDevice,
            _content,
            _screenManager,
            "ESCOLHA O MODO",
            new[]
            {
                new ChoiceOption("Solo", BuildSoloModeSelectScreen),
                new ChoiceOption("Multiplayer", () =>
                    new PlayerJoinScreen(_graphicsDevice, _content, _screenManager)),
            });
    }


    private IScreen BuildSoloModeSelectScreen()
    {
        return new ChoiceScreen(
            _graphicsDevice,
            _content,
            _screenManager,
            "ESCOLHA O MODO SOLO",
            new[]
            {
                new ChoiceOption("Normal", () =>
                    new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Solo)),
                new ChoiceOption("Treinamento", () =>
                    new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Training)),
            });
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.YellowGreen);

        _spriteBatch.Begin();

        _background.Draw(_spriteBatch, _graphicsDevice);

        _spriteBatch.DrawString(
            _textFont,
            "JOGO LEGAL DO PALHACO FELIZ",
            new Vector2(0, 0),
            Color.Black);

        string texto = "Pressione BAIXO para jogar";
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
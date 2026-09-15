using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;


public class SoloGameOverScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private RotatingBackground _background;

    private readonly MenuNavigator _navigator = new();

    public SoloGameOverScreen(GraphicsDevice graphicsDevice, ContentManager content, ScreenManager screenManager)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
    }

    public void Initialize()
    {
        _navigator.Prime();
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

        if (_navigator.ConfirmPressed || _navigator.CancelPressed)
        {
            _screenManager.RequestScreenChange(
                new MainMenu(_graphicsDevice, _content, _screenManager));
        }
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.YellowGreen);

        _spriteBatch.Begin();

        _background.Draw(_spriteBatch, _graphicsDevice);

        const string title = "FIM DE JOGO";
        Vector2 titleSize = _textFont.MeasureString(title);
        float titleX = (_graphicsDevice.Viewport.Width - titleSize.X) / 2;

        _spriteBatch.DrawString(_textFont, title, new Vector2(titleX, 80), Color.Black);

        const string subtitle = "Dinheiro insuficiente para continuar";
        Vector2 subtitleSize = _textFont.MeasureString(subtitle);
        float subtitleX = (_graphicsDevice.Viewport.Width - subtitleSize.X) / 2;

        _spriteBatch.DrawString(_textFont, subtitle, new Vector2(subtitleX, 130), Color.Black);

        const string footer = "Pressione qualquer botao para voltar ao menu";
        Vector2 footerSize = _textFont.MeasureString(footer);
        float footerX = (_graphicsDevice.Viewport.Width - footerSize.X) / 2;
        float footerY = _graphicsDevice.Viewport.Height - footerSize.Y - 10;

        _spriteBatch.DrawString(_textFont, footer, new Vector2(footerX, footerY), Color.Black);

        _spriteBatch.End();
    }
}

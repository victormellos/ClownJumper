using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;


public class ChoiceScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private readonly string _title;
    private readonly ChoiceOption[] _options;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private RotatingBackground _background;

    private readonly MenuNavigator _navigator = new();
    private int _selectedIndex;

    public ChoiceScreen(
        GraphicsDevice graphicsDevice,
        ContentManager content,
        ScreenManager screenManager,
        string title,
        ChoiceOption[] options)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
        _title = title;
        _options = options;
    }

    public void Initialize()
    {
        _selectedIndex = 0;

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

        if (_navigator.LeftPressed)
        {
            _selectedIndex = (_selectedIndex - 1 + _options.Length) % _options.Length;
        }
        else if (_navigator.RightPressed)
        {
            _selectedIndex = (_selectedIndex + 1) % _options.Length;
        }

        if (_navigator.ConfirmPressed)
        {
            _screenManager.RequestScreenChange(_options[_selectedIndex].CreateScreen());
            return;
        }

        if (_navigator.CancelPressed)
        {
            _screenManager.RequestGoBack();
        }
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.YellowGreen);

        _spriteBatch.Begin();

        _background.Draw(_spriteBatch, _graphicsDevice);

        _spriteBatch.DrawString(
            _textFont,
            _title,
            new Vector2(0, 0),
            Color.Black);

        DrawOptions();

        _spriteBatch.End();
    }

    private void DrawOptions()
    {
        const string spacing = "     ";

        float totalWidth = 0f;
        var optionTexts = new string[_options.Length];

        for (int i = 0; i < _options.Length; i++)
        {
            optionTexts[i] = i == _selectedIndex
                ? $"[{_options[i].Label}]"
                : _options[i].Label;

            totalWidth += _textFont.MeasureString(optionTexts[i]).X;

            if (i < _options.Length - 1)
                totalWidth += _textFont.MeasureString(spacing).X;
        }

        Vector2 lineSize = _textFont.MeasureString("X");
        float posX = (_graphicsDevice.Viewport.Width - totalWidth) / 2;
        float posY = _graphicsDevice.Viewport.Height - lineSize.Y - 10;

        for (int i = 0; i < _options.Length; i++)
        {
            Color color = i == _selectedIndex ? Color.DarkGreen : Color.Black;

            _spriteBatch.DrawString(_textFont, optionTexts[i], new Vector2(posX, posY), color);

            posX += _textFont.MeasureString(optionTexts[i]).X + _textFont.MeasureString(spacing).X;
        }
    }
}
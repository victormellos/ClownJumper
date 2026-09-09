using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

/// <summary>
/// Tela genérica de "escolha entre opções por texto": mostra um título no
/// topo e uma linha de opções embaixo (ex.: "1 - Normal        2 -
/// Treinamento"), cada uma associada a uma tecla. Ao pressionar a tecla de
/// uma opção, troca para a tela retornada por ChoiceOption.CreateScreen.
///
/// Substitui o que antes eram telas separadas e praticamente idênticas
/// (ModeSelectScreen, SoloModeSelectScreen, MultiplayerModeSelectScreen):
/// a única diferença real entre elas era o título e as opções, então agora
/// isso é passado como dado no construtor em vez de duplicar a classe.
/// </summary>
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

    private KeyboardState _previousKeyboard;

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
    }

    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);
        _textFont = _content.Load<SpriteFont>("fonts/ScoreFont");
        _background = new RotatingBackground(_content);

        _previousKeyboard = Keyboard.GetState();
    }

    public void Update(GameTime gameTime)
    {
        _background.Update(gameTime);

        var keyboard = Keyboard.GetState();

        foreach (var option in _options)
        {
            if (IsKeyPressedThisFrame(keyboard, option.Key))
            {
                _screenManager.RequestScreenChange(option.CreateScreen());
                break;
            }
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
            _title,
            new Vector2(0, 0),
            Color.Black);

        string texto = BuildOptionsText();
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

    private string BuildOptionsText()
    {
        return string.Join("        ", System.Array.ConvertAll(_options, o => o.Label));
    }
}
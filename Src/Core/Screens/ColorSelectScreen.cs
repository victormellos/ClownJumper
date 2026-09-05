using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;

/// <summary>
/// Tela exibida depois do PlayerJoinScreen, onde cada um dos 2 jogadores
/// escolhe a cor do seu palhaço. A tela é dividida ao meio (jogador 1 à
/// esquerda, jogador 2 à direita); cada um navega a lista de cores com
/// esquerda/direita e confirma com "baixo". Uma cor já confirmada por um
/// jogador fica indisponível para o outro.
/// </summary>
public class ColorSelectScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private readonly InputSource _player1Source;
    private readonly InputSource _player2Source;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;

    // Lista de cores disponíveis para escolha, na ordem em que aparecem.
    private static readonly (string Name, Color Value)[] AvailableColors =
    {
        ("Azul", Color.Blue),
        ("Vermelho", Color.Red),
        ("Verde", Color.Green),
        ("Amarelo", Color.Yellow),
        ("Branco", Color.White),
        ("Preto", Color.Black),
        ("Roxo", Color.Purple),
        ("Laranja", Color.Orange),
        ("Ciano", Color.Cyan),
        ("Banana", Color.PaleGoldenrod),
        ("Marrom", Color.Brown),
        ("Rosa", Color.Pink),
    };

    private int _player1ColorIndex;
    private int _player2ColorIndex;

    private bool _player1Confirmed;
    private bool _player2Confirmed;

    // Evita que segurar a tecla repita o movimento a cada frame; só reage
    // no frame em que o botão passou de solto para pressionado.
    private bool _player1WasLeftDown;
    private bool _player1WasRightDown;
    private bool _player2WasLeftDown;
    private bool _player2WasRightDown;

    public ColorSelectScreen(
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
        _player1ColorIndex = 0;
        _player2ColorIndex = FindNextAvailable(0, _player1ColorIndex, 1);

        _player1Confirmed = false;
        _player2Confirmed = false;
    }

    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);
        _textFont = _content.Load<SpriteFont>("fonts/ScoreFont");
    }

    public void Update(GameTime gameTime)
    {
        if (!_player1Confirmed)
        {
            UpdatePlayerSelection(
                _player1Source,
                ref _player1ColorIndex,
                _player2ColorIndex,
                ref _player1WasLeftDown,
                ref _player1WasRightDown,
                () => _player1Confirmed = true);
        }

        if (!_player2Confirmed)
        {
            UpdatePlayerSelection(
                _player2Source,
                ref _player2ColorIndex,
                _player1ColorIndex,
                ref _player2WasLeftDown,
                ref _player2WasRightDown,
                () => _player2Confirmed = true);
        }

        if (_player1Confirmed && _player2Confirmed)
        {
            _screenManager.RequestScreenChange(
                new MultiplayerModeSelectScreen(
                    _graphicsDevice,
                    _content,
                    _screenManager,
                    _player1Source,
                    _player2Source,
                    AvailableColors[_player1ColorIndex].Value,
                    AvailableColors[_player2ColorIndex].Value));
        }
    }

    private void UpdatePlayerSelection(
        InputSource source,
        ref int colorIndex,
        int otherPlayerColorIndex,
        ref bool wasLeftDown,
        ref bool wasRightDown,
        System.Action onConfirm)
    {
        bool leftDown = source.IsLeftPressed();
        bool rightDown = source.IsRightPressed();

        if (leftDown && !wasLeftDown)
        {
            colorIndex = FindNextAvailable(colorIndex, otherPlayerColorIndex, -1);
        }
        else if (rightDown && !wasRightDown)
        {
            colorIndex = FindNextAvailable(colorIndex, otherPlayerColorIndex, 1);
        }

        wasLeftDown = leftDown;
        wasRightDown = rightDown;

        if (source.IsDownPressed())
        {
            onConfirm();
        }
    }

    /// <summary>
    /// Anda na lista de cores a partir de currentIndex na direção informada
    /// (1 ou -1), pulando a cor que o outro jogador já tem escolhida.
    /// </summary>
    private int FindNextAvailable(int currentIndex, int otherPlayerColorIndex, int direction)
    {
        int index = currentIndex;

        for (int attempts = 0; attempts < AvailableColors.Length; attempts++)
        {
            index = (index + direction + AvailableColors.Length) % AvailableColors.Length;

            if (index != otherPlayerColorIndex)
                return index;
        }

        return currentIndex;
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.YellowGreen);

        _spriteBatch.Begin();

        int halfWidth = _graphicsDevice.Viewport.Width / 2;

        DrawPlayerColumn(
            "Jogador 1",
            new Rectangle(0, 0, halfWidth, _graphicsDevice.Viewport.Height),
            _player1ColorIndex,
            _player1Confirmed);

        DrawPlayerColumn(
            "Jogador 2",
            new Rectangle(halfWidth, 0, halfWidth, _graphicsDevice.Viewport.Height),
            _player2ColorIndex,
            _player2Confirmed);

        _spriteBatch.End();
    }

    private void DrawPlayerColumn(string title, Rectangle area, int colorIndex, bool confirmed)
    {
        var (colorName, colorValue) = AvailableColors[colorIndex];

        _spriteBatch.DrawString(
            _textFont,
            title,
            new Vector2(area.X + 10, area.Y + 10),
            Color.Black);

        // Bloco de cor grande no centro da coluna, para o jogador ver o
        // resultado visual da escolha, não só o nome.
        const int swatchSize = 64;
        var swatchRect = new Rectangle(
            area.X + (area.Width - swatchSize) / 2,
            area.Y + (area.Height - swatchSize) / 2,
            swatchSize,
            swatchSize);

        _spriteBatch.Draw(_pixel ??= CreatePixel(), swatchRect, colorValue);

        string statusText = confirmed ? $"{colorName} (confirmado)" : colorName;
        Vector2 statusSize = _textFont.MeasureString(statusText);

        _spriteBatch.DrawString(
            _textFont,
            statusText,
            new Vector2(area.X + (area.Width - statusSize.X) / 2, swatchRect.Bottom + 10),
            Color.Black);

        if (!confirmed)
        {
            string helpText = "<- ->  escolher     baixo  confirmar";
            Vector2 helpSize = _textFont.MeasureString(helpText);

            _spriteBatch.DrawString(
                _textFont,
                helpText,
                new Vector2(area.X + (area.Width - helpSize.X) / 2, area.Bottom - helpSize.Y - 10),
                Color.Black);
        }
    }

    private Texture2D _pixel;

    private Texture2D CreatePixel()
    {
        var texture = new Texture2D(_graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}
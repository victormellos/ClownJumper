using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

/// <summary>
/// Tela onde os jogadores "entram" no multiplayer apertando esquerda na
/// sua fonte de input (WASD, Setas, Controle 1 ou Controle 2). Assim que
/// 2 jogadores confirmarem, avança automaticamente para a escolha de
/// Coop/VS, levando consigo as fontes de input escolhidas.
/// </summary>
public class PlayerJoinScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private Texture2D _background;

    // As 4 fontes possíveis de input, na ordem em que aparecem na tela.
    private InputSource[] _candidateSources;
    private string[] _candidateLabels;

    private readonly List<InputSource> _joinedSources = new();

    public PlayerJoinScreen(GraphicsDevice graphicsDevice, ContentManager content, ScreenManager screenManager)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
    }

    public void Initialize()
    {
        _joinedSources.Clear();

        _candidateSources = new[]
        {
            InputSource.FromKeyboard(Keys.A, Keys.D),
            InputSource.FromKeyboard(Keys.Left, Keys.Right),
            InputSource.FromGamepad(PlayerIndex.One),
            InputSource.FromGamepad(PlayerIndex.Two),
        };

        _candidateLabels = new[]
        {
            "WASD",
            "Setas",
            "Controle 1",
            "Controle 2",
        };
    }

    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);
        _textFont = _content.Load<SpriteFont>("fonts/ScoreFont");
        _background = _content.Load<Texture2D>("images/background");
    }

    public void Update(GameTime gameTime)
    {
        for (int i = 0; i < _candidateSources.Length; i++)
        {
            var source = _candidateSources[i];

            if (_joinedSources.Contains(source))
                continue;

            if (source.IsLeftPressed())
            {
                _joinedSources.Add(source);
            }
        }

        if (_joinedSources.Count >= 2)
        {
            _screenManager.RequestScreenChange(
                new MultiplayerModeSelectScreen(_graphicsDevice, _content, _screenManager, _joinedSources[0], _joinedSources[1]));
        }
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
            "Aperte ESQUERDA para entrar (2 jogadores)",
            new Vector2(0, 0),
            Color.Black);

        float posY = 60f;

        for (int i = 0; i < _candidateSources.Length; i++)
        {
            bool joined = _joinedSources.Contains(_candidateSources[i]);
            string status = joined ? "PRONTO" : "Aguardando...";

            _spriteBatch.DrawString(
                _textFont,
                $"{_candidateLabels[i]}: {status}",
                new Vector2(10, posY),
                joined ? Color.DarkGreen : Color.Black);

            posY += 30f;
        }

        _spriteBatch.End();
    }
}
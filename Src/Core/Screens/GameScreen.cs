using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

public class GameScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;
    private readonly GameMode _mode;
    private readonly InputSource _player1Source;
    private readonly InputSource _player2Source;
    private readonly Color _player1Color;
    private readonly Color _player2Color;

    private SoundEffect _bounceSound;
    private SoundEffect _popSound;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;

    private List<Player> _players;

    private Level _level = new();
    private int _levelNumber;

    private readonly Random _random = new();

    private Sprite _balloonSprite;
    private readonly List<Balloon> _balloons = new();

    // Tamanho visual/hitbox que os personagens sempre tiveram (sprites antigos
    // eram 58x64 e 154x58). Os sprites novos são desenhados em 32x32, então
    // usamos esses valores como referência de escala, sem mudar a jogabilidade.
    private const int ClownWidth = 58;
    private const int ClownHeight = 64;
    private const int TrampolineWidth = 154;
    private const int TrampolineHeight = 58;

    // Tamanho de exibição do coração de vida (o arquivo é 32x32, mas
    // desenhamos menor para caber bem no HUD embaixo da tela).
    private const int HeartSize = 20;
    private const int HeartSpacing = 4;

    private Sprite _heartSprite;
    private Sprite _infinitySprite;

    public GameScreen(
        GraphicsDevice graphicsDevice,
        ContentManager content,
        ScreenManager screenManager,
        GameMode mode = GameMode.Solo,
        InputSource player1Source = null,
        InputSource player2Source = null,
        Color? player1Color = null,
        Color? player2Color = null)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
        _mode = mode;
        _player1Source = player1Source ?? InputSource.FromKeyboard(Keys.Left, Keys.Right);
        _player2Source = player2Source ?? InputSource.FromKeyboard(Keys.A, Keys.D);
        _player1Color = player1Color ?? Color.Blue;
        _player2Color = player2Color ?? Color.Red;
    }

    public void Initialize()
    {
        _players = new List<Player>();

        var clown1 = new Character(new Vector2(100f, 120f), Vector2.Zero);
        var trampoline1 = new Character(new Vector2(100f, 400f), Vector2.Zero);

        var player1 = new Player(trampoline1, clown1, _player1Source)
        {
            TintColor = _player1Color
        };

        _players.Add(player1);

        if (_mode == GameMode.Coop || _mode == GameMode.Versus || _mode == GameMode.TrainingCoop)
        {
            var clown2 = new Character(new Vector2(300f, 120f), Vector2.Zero);
            var trampoline2 = new Character(new Vector2(300f, 400f), Vector2.Zero);

            var player2 = new Player(trampoline2, clown2, _player2Source, "Jogador 2")
            {
                TintColor = _player2Color
            };

            _players.Add(player2);

            if (_mode == GameMode.Coop || _mode == GameMode.TrainingCoop)
            {
                // No Coop, vidas, pontos e combo são do time: os dois jogadores
                // passam a compartilhar a mesma instância de Score e TeamState.
                var sharedTeam = new TeamState(player1.Lives);
                var sharedScore = player1.Score;

                player1.Team = sharedTeam;
                player2.Team = sharedTeam;
                player2.Score = sharedScore;
            }
        }
    }

    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);

        _balloonSprite = new Sprite
        {
            Texture = _content.Load<Texture2D>("images/balloon_base"),
            TintTexture = _content.Load<Texture2D>("images/balloon_tint")
        };

        _bounceSound = _content.Load<SoundEffect>("sounds/bounce");
        _popSound = _content.Load<SoundEffect>("sounds/pop");

        _textFont = _content.Load<SpriteFont>("fonts/ScoreFont");

        _heartSprite = new Sprite
        {
            Texture = _content.Load<Texture2D>("images/heart_base"),
            TintTexture = _content.Load<Texture2D>("images/heart_tint")
        };
        _heartSprite.Scale = new Vector2(
            (float)HeartSize / _heartSprite.Texture.Width,
            (float)HeartSize / _heartSprite.Texture.Height
        );
        if (IsTrainingMode)
            {
                _infinitySprite = new Sprite
                {
                    Texture = _content.Load<Texture2D>("images/inf_base"),
                    TintTexture = _content.Load<Texture2D>("images/inf_tint")
                };
                _infinitySprite.Scale = new Vector2(
                    (float)HeartSize / _infinitySprite.Texture.Width,
                    (float)HeartSize / _infinitySprite.Texture.Height
                );
            }

        foreach (var player in _players)
        {
            player.Clown.Sprite = new Sprite
            {
                Texture = _content.Load<Texture2D>("images/clown_base"),
                TintTexture = _content.Load<Texture2D>("images/clown_tint")
            };
            player.Clown.Width = ClownWidth;
            player.Clown.Height = ClownHeight;
            player.Clown.Sprite.Scale = new Vector2(
                (float)ClownWidth / player.Clown.Sprite.Texture.Width,
                (float)ClownHeight / player.Clown.Sprite.Texture.Height
            );

            player.Trampoline.Sprite = new Sprite
            {
                Texture = _content.Load<Texture2D>("images/trampoline_base"),
                TintTexture = _content.Load<Texture2D>("images/trampoline_tint")
            };
            player.Trampoline.Width = TrampolineWidth;
            player.Trampoline.Height = TrampolineHeight;
            player.Trampoline.Sprite.Scale = new Vector2(
                (float)TrampolineWidth / player.Trampoline.Sprite.Texture.Width,
                (float)TrampolineHeight / player.Trampoline.Sprite.Texture.Height
            );

            player.Trampoline.Position = new Vector2(
                player.Trampoline.Position.X,
                _graphicsDevice.Viewport.Height - player.Trampoline.Height
            );
        }
    }

    public void CreateBalloons(int quantity)
    {
        _balloons.Clear();

        const int spacingX = 4;
        const int spacingY = 4;

        int rows = quantity;

        int balloonSize = (_graphicsDevice.Viewport.Height - ((rows - 1) * spacingY)) / rows;
        balloonSize = Math.Clamp(balloonSize, 16, 48);

        int columns = (_graphicsDevice.Viewport.Width + spacingX) / (balloonSize + spacingX);

        _balloonSprite.Scale = new Vector2(
            (float)balloonSize / _balloonSprite.Texture.Width,
            (float)balloonSize / _balloonSprite.Texture.Height
        );

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var type = Balloon.RollType(_random);

                var balloon = new Balloon(
                    new Vector2(
                        x * (balloonSize + spacingX),
                        y * (balloonSize + spacingY)
                    ),
                    type.Value
                )
                {
                    Sprite = _balloonSprite,
                    Width = balloonSize,
                    Height = balloonSize,
                    TintColor = type.TintColor,
                    TimeToLive = type.TimeToLive
                };

                _balloons.Add(balloon);
            }
        }
    }

    /// <summary>
    /// Envelhece cada balão na tela e remove (sem pontuar nem penalizar) os
    /// que ultrapassaram seu tempo de vida. Vermelho tem TimeToLive nulo e
    /// nunca expira por aqui.
    /// </summary>
    private void UpdateBalloonLifetimes(GameTime gameTime)
    {
        for (int i = _balloons.Count - 1; i >= 0; i--)
        {
            var balloon = _balloons[i];
            balloon.UpdateLifetime(gameTime);

            if (balloon.IsExpired)
            {
                _balloons.RemoveAt(i);
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        // Ordem de atualização: Input -> Movimento -> Física -> Colisão -> Resposta da colisão.

        foreach (var player in _players)
        {
            player.UpdateInput();
            player.HandleInput();
        }

        if (_balloons.Count == 0)
        {
            _levelNumber++;
            _level = new Level();
            // TODO(Etapa 3/4): CreateBalloons(quantity) em grid será substituído
            // pelo spawn contínuo em posição aleatória guiado por _level.SpawnInterval.
            // Usamos _levelNumber como ponte temporária só pra manter a build íntegra
            // entre etapas.
            CreateBalloons(_levelNumber);
        }

        UpdateBalloonLifetimes(gameTime);

        foreach (var player in _players)
        {
            UpdatePlayer(player, gameTime);
        }

        if (_mode == GameMode.Coop || _mode == GameMode.TrainingCoop)
        {
            CheckCoopGameOver();
        }
    }

    /// <summary>
    /// Treinamento (solo ou coop) tem vidas infinitas: o jogador/time nunca
    /// recebe "game over", só perde pontos e combo ao cair.
    /// </summary>
    private bool IsTrainingMode => _mode == GameMode.Training || _mode == GameMode.TrainingCoop;

    /// <summary>
    /// No Coop, as vidas são do time: assim que elas acabam, todos os
    /// jogadores do time são derrubados juntos, mesmo que algum ainda
    /// estivesse com o clown vivo no momento.
    /// </summary>
    private void CheckCoopGameOver()
    {
        bool teamOutOfLives = _players.Count > 0 && _players[0].Lives <= 0;

        if (!teamOutOfLives)
            return;

        foreach (var player in _players)
        {
            if (player.IsAlive)
            {
                player.IsAlive = false;
            }
        }
    }

    private void UpdatePlayer(Player player, GameTime gameTime)
    {
        var trampoline = player.Trampoline;

        trampoline.Position.X = MathHelper.Clamp(
            trampoline.Position.X,
            0,
            _graphicsDevice.Viewport.Width - trampoline.Width
        );

        var clown = player.Clown;
        if (clown == null)
            return;

        clown.Position += clown.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        float baseSpeed = 980f;
        float comboMultiplier = 1f + (player.Score.Combo * 0.0008f);
        float speed = baseSpeed * comboMultiplier;

        // Gravidade
        clown.Velocity.Y += 16f * comboMultiplier;

        int maxX = _graphicsDevice.Viewport.Width - clown.Width;
        int maxY = _graphicsDevice.Viewport.Height - clown.Height;

        if (clown.Position.Y > (maxY + clown.Height))
        {
            player.Score.AddPoints(-1200);
            player.Score.ResetCombo();

            if (!IsTrainingMode)
            {
                player.Lives--;
            }

            player.RespawnTimer.Wait(gameTime, () =>
            {
                clown.Position = trampoline.Position + new Vector2(trampoline.Width / 2, -clown.Height);
                clown.Velocity = Vector2.Zero;
            });

            if (!IsTrainingMode && player.Lives <= 0)
            {
                player.IsAlive = false;
            }

            return;
        }

        if (clown.Position.X > maxX)
        {
            clown.Velocity.X *= -1;
            clown.Position.X = maxX;
        }
        else if (clown.Position.X < 0)
        {
            clown.Velocity.X *= -1;
            clown.Position.X = 0;
        }
        else if (clown.Position.Y < -10)
        {
            clown.Velocity.Y *= -1;
            clown.Position.Y = -10;
        }

        if (Collision.Intersects(trampoline, clown))
        {
            int angle = trampoline.Bounds.Center.X - clown.Bounds.Center.X + 90;

            clown.Velocity.X = speed * (float)Math.Cos(MathHelper.ToRadians(angle));
            clown.Velocity.Y = speed * (float)Math.Sin(MathHelper.ToRadians(angle)) * -1;

            _bounceSound.Play();
        }

        for (int i = _balloons.Count - 1; i >= 0; i--)
        {
            Balloon balloon = _balloons[i];

            if (Collision.Intersects(clown, balloon))
            {
                player.Score.AddPoints(balloon.Value);
                _balloons.RemoveAt(i);
                _popSound.Play();
            }
        }
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        Vector2 scorePosition = new Vector2(10, 10);
        float offsetY = 0f;

        foreach (var player in _players)
        {
            if (player.Clown != null)
            {
                player.Clown.Sprite.DrawTinted(_spriteBatch, player.Clown.Position, player.TintColor);
            }

            foreach (var balloon in _balloons)
            {
                balloon.Sprite.DrawTinted(_spriteBatch, balloon.Position, balloon.TintColor);
            }

            player.Trampoline.Sprite.DrawTinted(_spriteBatch, player.Trampoline.Position, player.TintColor);

            string comboText = player.Score.Combo <= 1 ? "" : $"\nCombo : {player.Score.Combo}";
            string scoreText = $"{player.Name} : {player.Score.Points}{comboText}";

            _spriteBatch.DrawString(
                _textFont,
                scoreText,
                new Vector2(scorePosition.X, scorePosition.Y + offsetY),
                Color.White
            );

            offsetY += 30f;
        }

        DrawLives();

        _spriteBatch.End();
    }

    /// <summary>
    /// Desenha os corações de vida de cada jogador na parte de baixo da
    /// tela (embaixo, para não ficar atrás dos balões como estava antes).
    /// Jogador 1 fica alinhado à esquerda, jogador 2 à direita.
    /// </summary>
    private void DrawLives()
    {
        float heartsY = _graphicsDevice.Viewport.Height - HeartSize - 8;

        for (int playerIndex = 0; playerIndex < _players.Count; playerIndex++)
        {
            var player = _players[playerIndex];
            bool alignRight = playerIndex == 1;

            if (IsTrainingMode)
            {
                float infinityX = alignRight
                    ? _graphicsDevice.Viewport.Width - HeartSize - 8
                    : 8;

                _infinitySprite.DrawTinted(
                    _spriteBatch,
                    new Vector2(infinityX, heartsY),
                    player.TintColor
                );

                continue;
            }

            for (int i = 0; i < player.Lives; i++)
            {
                float heartX = alignRight
                    ? _graphicsDevice.Viewport.Width - HeartSize - 8 - (i * (HeartSize + HeartSpacing))
                    : 8 + (i * (HeartSize + HeartSpacing));

                _heartSprite.DrawTinted(
                    _spriteBatch,
                    new Vector2(heartX, heartsY),
                    player.TintColor
                );
            }
        }
    }
}
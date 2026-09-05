using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VictorMellos;

public class GameScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;

    private SoundEffect _bounceSound;
    private SoundEffect _popSound;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;

    private List<Player> _players;

    private Level _level = new(1);
    private int _levelNumber;

    private readonly Random _random = new();

    private Sprite _balloonSprite;
    private readonly List<Balloon> _balloons = new();

    public GameScreen(GraphicsDevice graphicsDevice, ContentManager content, ScreenManager screenManager)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
    }

    public void Initialize()
    {
        _players = new List<Player>();

        var clown1 = new Character(new Vector2(100f, 120f), Vector2.Zero);
        var trampoline1 = new Character(new Vector2(100f, 400f), Vector2.Zero);

        _players.Add(new Player(trampoline1, clown1, Keys.Left, Keys.Right, PlayerIndex.One));
    }

    public void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_graphicsDevice);

        _balloonSprite = new Sprite
        {
            Texture = _content.Load<Texture2D>("images/balloon")
        };

        _bounceSound = _content.Load<SoundEffect>("sounds/bounce");
        _popSound = _content.Load<SoundEffect>("sounds/pop");

        _textFont = _content.Load<SpriteFont>("fonts/ScoreFont");

        foreach (var player in _players)
        {
            player.Clown.Sprite = new Sprite
            {
                Texture = _content.Load<Texture2D>("images/clown")
            };
            player.Clown.Width = player.Clown.Sprite.Texture.Width;
            player.Clown.Height = player.Clown.Sprite.Texture.Height;

            player.Trampoline.Sprite = new Sprite
            {
                Texture = _content.Load<Texture2D>("images/Trampoline")
            };
            player.Trampoline.Width = player.Trampoline.Sprite.Texture.Width;
            player.Trampoline.Height = player.Trampoline.Sprite.Texture.Height;

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

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var balloon = new Balloon(
                    new Vector2(
                        x * (balloonSize + spacingX),
                        y * (balloonSize + spacingY)
                    ),
                    _random.Next(200, 1001)
                )
                {
                    Sprite = _balloonSprite,
                    Width = balloonSize,
                    Height = balloonSize
                };

                _balloons.Add(balloon);
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
            _level = new Level(_levelNumber);
            CreateBalloons(_level.Diff);
        }

        foreach (var player in _players)
        {
            UpdatePlayer(player, gameTime);
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
            player.Lives--;
            player.Score.ResetCombo();

            player.RespawnTimer.Wait(gameTime, () =>
            {
                clown.Position = trampoline.Position + new Vector2(trampoline.Width / 2, -clown.Height);
                clown.Velocity = Vector2.Zero;
            });

            if (player.Lives <= 0)
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
                _spriteBatch.Draw(player.Clown.Sprite.Texture, player.Clown.Position, Color.White);
            }

            foreach (var balloon in _balloons)
            {
                _spriteBatch.Draw(
                    balloon.Sprite.Texture,
                    new Rectangle((int)balloon.Position.X, (int)balloon.Position.Y, balloon.Width, balloon.Height),
                    Color.White
                );
            }

            _spriteBatch.Draw(player.Trampoline.Sprite.Texture, player.Trampoline.Position, Color.White);

            string livesText = player.Lives <= 0 ? "Morto!" : $"Vidas: {player.Lives}";
            string comboText = player.Score.Combo <= 1 ? "" : $"\nCombo : {player.Score.Combo}";
            string scoreText = $"{player.Name} : {player.Score.Points}\n{livesText}{comboText}";

            _spriteBatch.DrawString(
                _textFont,
                scoreText,
                new Vector2(scorePosition.X, scorePosition.Y + offsetY),
                Color.White
            );

            offsetY += 30f;
        }

        _spriteBatch.End();
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;


public class ShopScreen : IScreen
{
    private readonly GraphicsDevice _graphicsDevice;
    private readonly ContentManager _content;
    private readonly ScreenManager _screenManager;
    private readonly SoloSaveData _saveData;
    private readonly SoloUpgrades _upgrades;

    private SpriteFont _textFont;
    private SpriteBatch _spriteBatch;
    private RotatingBackground _background;

    private readonly MenuNavigator _navigator = new();

    private enum UpgradeChoice
    {
        Lives,
        Luck,
        Quantity,
        Duration
    }

    private static readonly UpgradeChoice[] Choices =
    {
        UpgradeChoice.Lives,
        UpgradeChoice.Luck,
        UpgradeChoice.Quantity,
        UpgradeChoice.Duration
    };

    private int _selectedIndex;

    public ShopScreen(
        GraphicsDevice graphicsDevice,
        ContentManager content,
        ScreenManager screenManager,
        SoloSaveData saveData)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _screenManager = screenManager;
        _saveData = saveData;
        _upgrades = new SoloUpgrades(saveData);
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
            _selectedIndex = (_selectedIndex - 1 + Choices.Length) % Choices.Length;
        }
        else if (_navigator.RightPressed)
        {
            _selectedIndex = (_selectedIndex + 1) % Choices.Length;
        }

        if (_navigator.ConfirmPressed)
        {
            TryBuySelected();
        }

        if (_navigator.CancelPressed)
        {
            LeaveShop();
        }
    }

    private void TryBuySelected()
    {
        bool bought = Choices[_selectedIndex] switch
        {
            UpgradeChoice.Lives => _upgrades.TryBuyLives(),
            UpgradeChoice.Luck => _upgrades.TryBuyLuck(),
            UpgradeChoice.Quantity => _upgrades.TryBuyQuantity(),
            UpgradeChoice.Duration => _upgrades.TryBuyDuration(),
            _ => false
        };

        if (bought)
        {
            SoloSaveManager.Save(_saveData);
        }
    }

    private void LeaveShop()
    {
        bool canAffordNextLife = _saveData.Money >= _upgrades.GetNextLivesPrice();

        if (!canAffordNextLife)
        {
            _screenManager.RequestScreenChange(
                new SoloGameOverScreen(_graphicsDevice, _content, _screenManager));
            return;
        }

        _screenManager.RequestScreenChange(
            new GameScreen(_graphicsDevice, _content, _screenManager, GameMode.Solo));
    }

    public void Draw()
    {
        _graphicsDevice.Clear(Color.YellowGreen);

        _spriteBatch.Begin();

        _background.Draw(_spriteBatch, _graphicsDevice);

        _spriteBatch.DrawString(
            _textFont,
            $"LOJA - Dinheiro: {_saveData.Money}",
            new Vector2(0, 0),
            Color.Black);

        DrawUpgradeOptions();

        string footer = "ESQUERDA/DIREITA escolhe | BAIXO compra | CIMA sai";
        Vector2 footerSize = _textFont.MeasureString(footer);
        float footerX = (_graphicsDevice.Viewport.Width - footerSize.X) / 2;
        float footerY = _graphicsDevice.Viewport.Height - footerSize.Y - 10;

        _spriteBatch.DrawString(_textFont, footer, new Vector2(footerX, footerY), Color.Black);

        _spriteBatch.End();
    }

    private void DrawUpgradeOptions()
    {
        float posY = 60f;

        for (int i = 0; i < Choices.Length; i++)
        {
            string label = BuildLabel(Choices[i]);
            Color color = i == _selectedIndex ? Color.DarkGreen : Color.Black;

            string prefix = i == _selectedIndex ? "> " : "  ";

            _spriteBatch.DrawString(_textFont, prefix + label, new Vector2(10, posY), color);

            posY += 30f;
        }
    }

    private string BuildLabel(UpgradeChoice choice)
    {
        return choice switch
        {
            UpgradeChoice.Lives =>
                $"Vidas (nivel {_upgrades.LivesLevel}) - Proximo: {_upgrades.GetNextLivesPrice()}",
            UpgradeChoice.Luck =>
                FormatMaxable("Sorte", _upgrades.LuckLevel, _upgrades.GetNextLuckPrice()),
            UpgradeChoice.Quantity =>
                FormatMaxable("Quantidade", _upgrades.QuantityLevel, _upgrades.GetNextQuantityPrice()),
            UpgradeChoice.Duration =>
                FormatMaxable("Duracao", _upgrades.DurationLevel, _upgrades.GetNextDurationPrice()),
            _ => string.Empty
        };
    }

    private static string FormatMaxable(string name, int level, int? nextPrice)
    {
        if (nextPrice == null)
            return $"{name} (nivel {level}) - MAXIMO";

        return $"{name} (nivel {level}) - Proximo: {nextPrice.Value}";
    }
}

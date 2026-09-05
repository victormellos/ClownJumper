using Microsoft.Xna.Framework;

namespace VictorMellos;

public class ClownGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly ScreenManager _screenManager = new();

    public ClownGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = 800,
            PreferredBackBufferHeight = 500
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _screenManager.RequestScreenChange(
            new MainMenu(GraphicsDevice, Content, _screenManager));

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screenManager.Draw();
        base.Draw(gameTime);
    }
}

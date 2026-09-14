using Microsoft.Xna.Framework;

namespace ClownJumper;

public class ClownGame : Game
{
    private const int WindowedWidth = 800;
    private const int WindowedHeight = 500;

    private readonly GraphicsDeviceManager _graphics;
    private readonly ScreenManager _screenManager = new();
    private FullscreenController _fullscreenController;

    public ClownGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WindowedWidth,
            PreferredBackBufferHeight = WindowedHeight
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _fullscreenController = new FullscreenController(_graphics, WindowedWidth, WindowedHeight);

        _screenManager.RequestScreenChange(
            new MainMenu(GraphicsDevice, Content, _screenManager));
    }

    protected override void Update(GameTime gameTime)
    {
        _fullscreenController.Update();
        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _screenManager.Draw();
        base.Draw(gameTime);
    }
}
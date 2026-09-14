using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;


public class FullscreenController
{
    private readonly GraphicsDeviceManager _graphics;


    private readonly int _windowedWidth;
    private readonly int _windowedHeight;

    private bool _wasKeyDown;

    public FullscreenController(GraphicsDeviceManager graphics, int windowedWidth, int windowedHeight)
    {
        _graphics = graphics;
        _windowedWidth = windowedWidth;
        _windowedHeight = windowedHeight;
    }

    public void Update()
    {
        var keyboard = Keyboard.GetState();
        bool keyDown = keyboard.IsKeyDown(Keys.F);

        if (keyDown && !_wasKeyDown)
        {
            Toggle();
        }

        _wasKeyDown = keyDown;
    }

    private void Toggle()
    {
        if (_graphics.IsFullScreen)
        {
            _graphics.PreferredBackBufferWidth = _windowedWidth;
            _graphics.PreferredBackBufferHeight = _windowedHeight;
            _graphics.IsFullScreen = false;
        }
        else
        {
            var displayMode = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode;

            _graphics.PreferredBackBufferWidth = displayMode.Width;
            _graphics.PreferredBackBufferHeight = displayMode.Height;
            _graphics.IsFullScreen = true;
        }

        _graphics.ApplyChanges();
    }
}
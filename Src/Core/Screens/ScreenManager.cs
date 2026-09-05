using Microsoft.Xna.Framework;

namespace ClownJumper;

/// <summary>
/// Controla qual tela está ativa e faz a transição entre elas.
/// Cada tela recebe uma referência a este manager e chama RequestScreenChange
/// quando quiser trocar de tela (ex.: "START" no menu -> gameplay).
/// </summary>
public class ScreenManager
{
    private IScreen _currentScreen;

    public void RequestScreenChange(IScreen newScreen)
    {
        _currentScreen = newScreen;

        _currentScreen.Initialize();
        _currentScreen.LoadContent();
    }

    public void Update(GameTime gameTime)
    {
        _currentScreen?.Update(gameTime);
    }

    public void Draw()
    {
        _currentScreen?.Draw();
    }
}

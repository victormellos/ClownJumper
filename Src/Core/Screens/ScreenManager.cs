using System;
using Microsoft.Xna.Framework;

namespace VictorMellos;

/// <summary>
/// Controla qual tela está ativa e faz a transição entre elas.
/// Cada tela recebe uma referência a este manager e chama RequestScreenChange
/// quando quiser trocar de tela (ex.: "START" no menu -> gameplay).
/// </summary>
public class ScreenManager
{
    private IScreen _currentScreen;
    private bool _currentScreenLoaded;

    public void RequestScreenChange(IScreen newScreen)
    {
        _currentScreen = newScreen;
        _currentScreenLoaded = false;

        _currentScreen.Initialize();
    }

    public void Update(GameTime gameTime)
    {
        if (_currentScreen == null)
            return;

        // LoadContent só precisa rodar uma vez, logo após Initialize,
        // mas fazemos aqui (e não em RequestScreenChange) para manter
        // o carregamento de conteúdo fora do fluxo de troca de tela em si.
        if (!_currentScreenLoaded)
        {
            _currentScreen.LoadContent();
            _currentScreenLoaded = true;
        }

        _currentScreen.Update(gameTime);
    }

    public void Draw()
    {
        _currentScreen?.Draw();
    }
}

using Microsoft.Xna.Framework;

namespace ClownJumper;

/// <summary>
/// Contrato comum para qualquer tela do jogo (menu, gameplay, etc.).
/// O ScreenManager controla o ciclo de vida de quem implementa essa interface.
/// </summary>
public interface IScreen
{
    void Initialize();
    void LoadContent();
    void Update(GameTime gameTime);
    void Draw();
}

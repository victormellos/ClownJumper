using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;

/// <summary>
/// Background giratório usado nas telas de menu. A imagem gira em torno do
/// próprio centro (fica de ponta-cabeça, depois de lado, depois em pé de
/// novo). Como um retângulo girado "estoura" pra fora dos cantos da tela,
/// a textura é desenhada maior que o viewport (ver <see cref="ScaleFactor"/>)
/// pra nunca sobrar espaço vazio nas bordas.
/// </summary>
public class RotatingBackground
{
    /// <summary>
    /// O pior caso é a imagem girada 45°: a diagonal do retângulo da tela
    /// precisa caber dentro da imagem. Pra uma tela 16:9 isso dá uma
    /// diagonal ~1.15x maior que a largura, então 1.2 já cobre com folga
    /// (e cobre proporções mais quadradas também, que precisam de menos).
    /// </summary>
    private const float ScaleFactor = 1.2f;

    /// <summary>Velocidade de rotação, em radianos por segundo.</summary>
    private const float RotationSpeed = 0.3f;

    private readonly Texture2D _texture;
    private float _rotation;

    public RotatingBackground(ContentManager content)
    {
        _texture = content.Load<Texture2D>("images/background");
    }

    public void Update(GameTime gameTime)
    {
        _rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        var viewport = graphicsDevice.Viewport;
        var center = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
        var origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);

        // Escala pra cobrir a tela inteira (maior lado) e ainda sobrar a
        // margem de segurança do ScaleFactor pra rotação não deixar canto vazio.
        float baseScale = System.Math.Max(
            (float)viewport.Width / _texture.Width,
            (float)viewport.Height / _texture.Height
        );
        float scale = baseScale * ScaleFactor;

        spriteBatch.Draw(
            _texture,
            center,
            null,
            Color.White,
            _rotation,
            origin,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}
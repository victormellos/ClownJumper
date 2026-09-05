using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper;

public class Sprite
{
    public Texture2D Texture;

    /// <summary>
    /// Camada opcional só com a parte branca do sprite (o resto transparente).
    /// Usada para tingir apenas essa parte por cima da textura base.
    /// </summary>
    public Texture2D TintTexture;

    public Vector2 Scale = Vector2.One;

    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(
            Texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            Scale,
            SpriteEffects.None,
            0f
        );
    }

    /// <summary>
    /// Desenha a textura base normalmente e, por cima, a camada de tint
    /// (parte branca) já colorida com a cor do jogador.
    /// </summary>
    public void DrawTinted(SpriteBatch spriteBatch, Vector2 position, Color tintColor)
    {
        spriteBatch.Draw(
            Texture,
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            Scale,
            SpriteEffects.None,
            0f
        );

        if (TintTexture != null)
        {
            spriteBatch.Draw(
                TintTexture,
                position,
                null,
                tintColor,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
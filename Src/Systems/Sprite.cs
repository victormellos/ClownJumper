using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VictorMellos;

public class Sprite
{
    public Texture2D Texture;
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
}

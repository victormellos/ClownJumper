using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClownJumper
{
    public class Character
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Sprite Sprite;

        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        // Bounds é virtual para que classes derivadas (ex.: Balloon) possam
        // sobrescrevê-lo caso precisem de uma lógica diferente de hitbox.
        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    Width,
                    Height
                );
            }
        }

        public Character(Vector2 initialPosition, Vector2 initialVelocity)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            
        }
        public void Update(GameTime gameTime)
        {
            Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite != null)
            {
                spriteBatch.Draw(Sprite.Texture, Position, Color.White);
            }
        }
    }
}
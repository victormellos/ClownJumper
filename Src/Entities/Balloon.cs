using Microsoft.Xna.Framework;

namespace ClownJumper
{
    public class Balloon : Character
    {
        public int Value;

        public override int Width { get; set; } = 64;
        public override int Height { get; set; } = 64;

        public Balloon(Vector2 initialPosition, int value)
            : base(initialPosition, Vector2.Zero)
        {
            Value = value;
        }
    }
}

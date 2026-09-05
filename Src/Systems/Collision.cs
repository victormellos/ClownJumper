using Microsoft.Xna.Framework;

namespace VictorMellos;

public static class Collision
{
    public static bool Intersects(Character a, Character b)
    {
        Rectangle rectA = new Rectangle(
            (int)a.Position.X,
            (int)a.Position.Y,
            a.Width,
            a.Height
        );

        Rectangle rectB = new Rectangle(
            (int)b.Position.X,
            (int)b.Position.Y,
            b.Width,
            b.Height
        );

        return rectA.Intersects(rectB);
    }
}
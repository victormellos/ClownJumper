using Microsoft.Xna.Framework;

namespace ClownJumper;

public class AirBurstEffect
{

    private const double LifetimeSeconds = 0.35;

    public Vector2 Center;
    public Color TintColor;

    public float MaxRadius;

    private double _age;

    public bool IsFinished => _age >= LifetimeSeconds;

    public float Progress => (float)MathHelper.Clamp((float)(_age / LifetimeSeconds), 0f, 1f);

    public float CurrentRadius => MaxRadius * Progress;


    public float CurrentAlpha => 1f - Progress;

    public AirBurstEffect(Vector2 center, Color tintColor, float maxRadius)
    {
        Center = center;
        TintColor = tintColor;
        MaxRadius = maxRadius;
    }

    public void Update(GameTime gameTime)
    {
        _age += gameTime.ElapsedGameTime.TotalSeconds;
    }
}
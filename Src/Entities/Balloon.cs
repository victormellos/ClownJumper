using Microsoft.Xna.Framework;

namespace ClownJumper
{
    /// <summary>
    /// Um tipo de balão: a cor de tingimento, os pontos que ele vale, o peso
    /// relativo de chance de aparecer (não precisa somar 100, o sorteio
    /// normaliza pela soma de todos os pesos) e o tempo de vida em segundos
    /// antes de expirar sozinho (null = nunca expira).
    /// </summary>
    public readonly struct BalloonType
    {
        public readonly Color TintColor;
        public readonly int Value;
        public readonly double Weight;
        public readonly double? TimeToLive;

        public BalloonType(Color tintColor, int value, double weight, double? timeToLive)
        {
            TintColor = tintColor;
            Value = value;
            Weight = weight;
            TimeToLive = timeToLive;
        }
    }

    public class Balloon : Character
    {
        public int Value;

        public Color TintColor = Color.White;


        public double? TimeToLive;
        public double Age;
        public bool IsExpired => TimeToLive.HasValue && Age >= TimeToLive.Value;
        public static readonly BalloonType[] Types =
        {
            new BalloonType(Color.Red, 1, 35.0, null),       
            new BalloonType(Color.Orange, 2, 25.0, 22.0),    
            new BalloonType(Color.Yellow, 3, 15.0, 17.0),     
            new BalloonType(Color.Green, 5, 10.0, 13.0),      
            new BalloonType(Color.Blue, 8, 6.0, 10.0),        
            new BalloonType(Color.Purple, 12, 4.0, 8.0),   
            new BalloonType(Color.HotPink, 16, 2.0, 6.0),    
            new BalloonType(Color.Black, 25, 1.5, 5.0),      
            new BalloonType(Color.White, 40, 0.3, 4.0),     
            new BalloonType(Color.Gold, 75, 0.2, 3.0),       
        };

        public override int Width { get; set; } = 64;
        public override int Height { get; set; } = 64;

        public Balloon(Vector2 initialPosition, int value)
            : base(initialPosition, Vector2.Zero)
        {
            Value = value;
        }
        public void UpdateLifetime(GameTime gameTime)
        {
            Age += gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Sorteia um tipo de balão com base nos pesos de raridade em Types,
        /// usando o Random compartilhado passado por quem chama (ex.: GameScreen).
        /// </summary>
        public static BalloonType RollType(System.Random random)
        {
            double totalWeight = 0.0;
            foreach (var type in Types)
            {
                totalWeight += type.Weight;
            }

            double roll = random.NextDouble() * totalWeight;
            double cumulative = 0.0;

            foreach (var type in Types)
            {
                cumulative += type.Weight;
                if (roll < cumulative)
                {
                    return type;
                }
            }

            return Types[Types.Length - 1];
        }
    }
}
using Microsoft.Xna.Framework;

namespace ClownJumper
{
    public readonly struct BalloonType
    {
        public readonly Color TintColor;
        public readonly int Value;
        public readonly double Weight;

        public BalloonType(Color tintColor, int value, double weight)
        {
            TintColor = tintColor;
            Value = value;
            Weight = weight;
        }
    }

    public class Balloon : Character
    {
        public int Value;

        public Color TintColor = Color.White;


        public static readonly BalloonType[] Types =
        {
            new BalloonType(Color.Red, 1, 35.0),        // Vermelho
            new BalloonType(Color.Orange, 2, 25.0),     // Laranja
            new BalloonType(Color.Yellow, 3, 15.0),     // Amarelo
            new BalloonType(Color.Green, 5, 10.0),      // Verde
            new BalloonType(Color.Blue, 8, 6.0),        // Azul
            new BalloonType(Color.Purple, 12, 4.0),     // Roxo
            new BalloonType(Color.HotPink, 16, 2.0),    // Rosa
            new BalloonType(Color.Black, 25, 1.5),      // Preto
            new BalloonType(Color.White, 40, 0.3),      // Branco
            new BalloonType(Color.Gold, 75, 0.2),       // Dourado
        };

        public override int Width { get; set; } = 64;
        public override int Height { get; set; } = 64;

        public Balloon(Vector2 initialPosition, int value)
            : base(initialPosition, Vector2.Zero)
        {
            Value = value;
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
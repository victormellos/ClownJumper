using System.IO.Compression;

namespace VictorMellos;
public class Score
{
    public int Points { get; private set; }
    public int Combo { get; private set; } = 1;

    public void AddPoints(int basePoints)
    {
        Points += basePoints * Combo;
        Combo++;
    }

    public void ResetCombo()
    {
        Combo = 1;
    }
}
using System;
using Microsoft.Xna.Framework;

namespace ClownJumper;

/// <summary>
/// Controla a dificuldade da partida: o jogo sobe de nível a cada
/// LevelDurationSeconds de tempo decorrido OU a cada PopsPerLevel balões
/// estourados (o que vier primeiro). Cada nível reduz o intervalo entre
/// spawns de balão em IntervalReductionPerLevel, com um piso mínimo.
/// </summary>
public class Level
{
    private const double LevelDurationSeconds = 15.0;
    private const int PopsPerLevel = 10;
    private const double IntervalReductionPerLevel = 0.10; // 10% por nível
    private const double InitialSpawnInterval = 1.2; // segundos
    private const double MinSpawnInterval = 0.2; // segundos

    public int Number { get; private set; } = 1;
    public double SpawnInterval { get; private set; } = InitialSpawnInterval;

    private double _timeInCurrentLevel;
    private int _popsInCurrentLevel;

    /// <summary>
    /// Deve ser chamado a cada frame com o tempo decorrido. Avança o nível
    /// automaticamente quando o tempo do nível atual se esgota.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        _timeInCurrentLevel += gameTime.ElapsedGameTime.TotalSeconds;

        if (_timeInCurrentLevel >= LevelDurationSeconds)
        {
            AdvanceLevel();
        }
    }

    public void RegisterPop()
    {
        _popsInCurrentLevel++;

        if (_popsInCurrentLevel >= PopsPerLevel)
        {
            AdvanceLevel();
        }
    }

    private void AdvanceLevel()
    {
        Number++;
        _timeInCurrentLevel = 0.0;
        _popsInCurrentLevel = 0;

        double reducedInterval = SpawnInterval * (1.0 - IntervalReductionPerLevel);
        SpawnInterval = Math.Max(MinSpawnInterval, reducedInterval);
    }
}
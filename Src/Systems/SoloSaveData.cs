namespace ClownJumper;

/// <summary>
/// Estado persistente do modo história (Solo): dinheiro acumulado, o nível
/// atual de cada um dos 4 upgrades e a fase em que o jogador parou (ainda
/// não usada, reservada para quando o modo história tiver fases).
/// </summary>
public class SoloSaveData
{
    public int Money;

    public int LivesUpgradeLevel;
    public int LuckUpgradeLevel;
    public int QuantityUpgradeLevel;
    public int DurationUpgradeLevel;

    public int CurrentPhase;

    /// <summary>
    /// Cria um save novo com os valores iniciais: 0 de dinheiro, todos os
    /// upgrades no nível 0, fase 1.
    /// </summary>
    public static SoloSaveData CreateNew()
    {
        return new SoloSaveData
        {
            Money = 0,
            LivesUpgradeLevel = 0,
            LuckUpgradeLevel = 0,
            QuantityUpgradeLevel = 0,
            DurationUpgradeLevel = 0,
            CurrentPhase = 1
        };
    }
}

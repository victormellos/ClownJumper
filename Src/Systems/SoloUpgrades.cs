using System;

namespace ClownJumper;

/// <summary>
/// Os 4 upgrades compráveis no modo história (Solo), lidos a partir de um
/// SoloSaveData. Cada método de preço calcula o custo do PRÓXIMO nível
/// (o nível que o jogador ainda não tem).
///
/// - Vidas: sem teto de nível/preço, mas o efeito (vidas iniciais) tem um
///   teto de LivesEffectCap, então comprar além disso não muda mais nada
///   no jogo (mas ShopScreen barra a compra ao chegar lá).
/// - Sorte: 10 níveis (0 a 9). Cada nível libera 1 cor de balão a mais,
///   na ordem em que elas aparecem em Balloon.Types (nível 0 = só a
///   primeira cor, Vermelho; nível 9 = todas as 10 cores liberadas).
/// - Quantidade: 5 níveis. Cada nível reduz o intervalo de spawn da Level
///   em 10% (multiplicativo).
/// - Duração: 5 níveis. Cada nível aumenta o TimeToLive dos balões em 10%
///   (multiplicativo). Não afeta balões com TimeToLive nulo (nunca expiram).
/// </summary>
public class SoloUpgrades
{
    private readonly SoloSaveData _data;

    // Vidas: preço 5×2ⁿ, sem teto de nível, efeito com teto.
    private const int LivesBasePrice = 5;
    public const int LivesEffectCap = 30;

    // Sorte: preço 3×2ⁿ, 10 níveis (0 a 9), 1 cor por nível.
    private const int LuckBasePrice = 3;
    public const int LuckMaxLevel = 9;

    // Quantidade: preço 3×2ⁿ, 5 níveis, -10% de intervalo de spawn por nível.
    private const int QuantityBasePrice = 3;
    public const int QuantityMaxLevel = 5;
    private const double SpawnIntervalReductionPerLevel = 0.10;

    // Duração: preço 3×2ⁿ, 5 níveis, +10% de TimeToLive por nível.
    private const int DurationBasePrice = 3;
    public const int DurationMaxLevel = 5;
    private const double TimeToLiveIncreasePerLevel = 0.10;

    public SoloUpgrades(SoloSaveData data)
    {
        _data = data;
    }

    public int LivesLevel => _data.LivesUpgradeLevel;
    public int LuckLevel => _data.LuckUpgradeLevel;
    public int QuantityLevel => _data.QuantityUpgradeLevel;
    public int DurationLevel => _data.DurationUpgradeLevel;

    /// <summary>
    /// Preço para comprar o próximo nível de Vidas (nível atual + 1).
    /// Sem teto: sempre é possível calcular o próximo preço.
    /// </summary>
    public int GetNextLivesPrice()
    {
        return LivesBasePrice * (int)Math.Pow(2, _data.LivesUpgradeLevel);
    }

    /// <summary>
    /// Preço para comprar o próximo nível de Sorte, ou null se já estiver
    /// no nível máximo (todas as cores já desbloqueadas).
    /// </summary>
    public int? GetNextLuckPrice()
    {
        if (_data.LuckUpgradeLevel >= LuckMaxLevel)
            return null;

        return LuckBasePrice * (int)Math.Pow(2, _data.LuckUpgradeLevel);
    }

    /// <summary>
    /// Preço para comprar o próximo nível de Quantidade, ou null se já
    /// estiver no nível máximo.
    /// </summary>
    public int? GetNextQuantityPrice()
    {
        if (_data.QuantityUpgradeLevel >= QuantityMaxLevel)
            return null;

        return QuantityBasePrice * (int)Math.Pow(2, _data.QuantityUpgradeLevel);
    }

    /// <summary>
    /// Preço para comprar o próximo nível de Duração, ou null se já
    /// estiver no nível máximo.
    /// </summary>
    public int? GetNextDurationPrice()
    {
        if (_data.DurationUpgradeLevel >= DurationMaxLevel)
            return null;

        return DurationBasePrice * (int)Math.Pow(2, _data.DurationUpgradeLevel);
    }

    /// <summary>
    /// Tenta comprar o próximo nível de Vidas. Retorna false se não tiver
    /// dinheiro suficiente. Sem teto de nível (o teto é só no efeito).
    /// </summary>
    public bool TryBuyLives()
    {
        int price = GetNextLivesPrice();

        if (_data.Money < price)
            return false;

        _data.Money -= price;
        _data.LivesUpgradeLevel++;
        return true;
    }

    /// <summary>
    /// Tenta comprar o próximo nível de Sorte. Retorna false se não tiver
    /// dinheiro suficiente ou se já estiver no nível máximo.
    /// </summary>
    public bool TryBuyLuck()
    {
        int? price = GetNextLuckPrice();

        if (price == null || _data.Money < price.Value)
            return false;

        _data.Money -= price.Value;
        _data.LuckUpgradeLevel++;
        return true;
    }

    /// <summary>
    /// Tenta comprar o próximo nível de Quantidade. Retorna false se não
    /// tiver dinheiro suficiente ou se já estiver no nível máximo.
    /// </summary>
    public bool TryBuyQuantity()
    {
        int? price = GetNextQuantityPrice();

        if (price == null || _data.Money < price.Value)
            return false;

        _data.Money -= price.Value;
        _data.QuantityUpgradeLevel++;
        return true;
    }

    /// <summary>
    /// Tenta comprar o próximo nível de Duração. Retorna false se não
    /// tiver dinheiro suficiente ou se já estiver no nível máximo.
    /// </summary>
    public bool TryBuyDuration()
    {
        int? price = GetNextDurationPrice();

        if (price == null || _data.Money < price.Value)
            return false;

        _data.Money -= price.Value;
        _data.DurationUpgradeLevel++;
        return true;
    }

    /// <summary>
    /// Vidas iniciais do modo história, considerando o upgrade de Vidas
    /// (nível 0 = as 5 vidas padrão do jogo), limitado ao teto de efeito.
    /// </summary>
    public int GetStartingLives(int baseLives = 5)
    {
        int lives = baseLives + _data.LivesUpgradeLevel;
        return Math.Min(lives, LivesEffectCap);
    }

    /// <summary>
    /// Quantas cores de balão (contando a partir do índice 0 em
    /// Balloon.Types) estão desbloqueadas com o nível atual de Sorte.
    /// Nível 0 = 1 cor (só a primeira, Vermelho); cada nível seguinte
    /// libera mais uma, até todas em LuckMaxLevel.
    /// </summary>
    public int GetUnlockedColorCount(int totalColors)
    {
        int unlocked = 1 + _data.LuckUpgradeLevel;
        return Math.Clamp(unlocked, 1, totalColors);
    }

    /// <summary>
    /// Aplica o upgrade de Duração a um TimeToLive base. Balões com
    /// TimeToLive nulo (nunca expiram) continuam nulos.
    /// </summary>
    public double? ApplyDurationUpgrade(double? baseTimeToLive)
    {
        if (baseTimeToLive == null)
            return null;

        double multiplier = 1.0 + (_data.DurationUpgradeLevel * TimeToLiveIncreasePerLevel);
        return baseTimeToLive.Value * multiplier;
    }

    /// <summary>
    /// Aplica o upgrade de Quantidade a um intervalo de spawn base,
    /// reduzindo-o (balões nascem mais frequentemente). Nunca deixa o
    /// intervalo menor que o piso mínimo já usado pela Level.
    /// </summary>
    public double ApplySpawnIntervalUpgrade(double baseInterval, double minInterval)
    {
        double multiplier = Math.Pow(1.0 - SpawnIntervalReductionPerLevel, _data.QuantityUpgradeLevel);
        double reduced = baseInterval * multiplier;

        return Math.Max(minInterval, reduced);
    }
}

namespace ClownJumper;

public enum GameMode
{
    Solo,
    Coop,
    Versus,

    /// <summary>
    /// Igual ao Solo, mas com vidas infinitas: o jogador nunca recebe
    /// "game over", só perde pontos/combo ao cair.
    /// </summary>
    Training,

    /// <summary>
    /// Igual ao Coop (vidas/pontuação compartilhadas), mas com vidas
    /// infinitas: o time nunca recebe "game over".
    /// </summary>
    TrainingCoop
}
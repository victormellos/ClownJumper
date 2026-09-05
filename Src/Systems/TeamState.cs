namespace ClownJumper;

/// <summary>
/// Estado compartilhado de um time no modo Coop. As vidas são do time,
/// não de cada jogador individualmente — quando um clown cai, desconta
/// da vida do time inteiro.
/// </summary>
public class TeamState
{
    public int Lives;

    public TeamState(int lives)
    {
        Lives = lives;
    }
}
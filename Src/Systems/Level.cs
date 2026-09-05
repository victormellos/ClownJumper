using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClownJumper;
public class Level
{
    // "Diff" = difficulty: quantidade de linhas de balões geradas para o nível.
    public int Diff = 1;

    public Level(int level_Number)
    {
        Diff =
        Math.Max
        (
            1,
            level_Number + level_Number * level_Number / 5
        );


    }
}

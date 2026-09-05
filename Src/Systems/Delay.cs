using System;
using Microsoft.Xna.Framework;

namespace VictorMellos;

/// <summary>
/// Temporizador simples de "não repetir uma ação antes de X ms".
/// A primeira chamada a Wait() sempre dispara a ação imediatamente;
/// chamadas seguintes só disparam depois que DelayTime (ms) tiver passado
/// desde o último disparo.
/// </summary>
public class Delay
{
    private double _nextAllowedTrigger;
    private readonly double _delayTimeMs;

    public Delay(double delayTimeMs)
    {
        _delayTimeMs = delayTimeMs;
    }

    public void Wait(GameTime gameTime, Action action)
    {
        if (_nextAllowedTrigger > gameTime.TotalGameTime.TotalMilliseconds)
            return;

        _nextAllowedTrigger = gameTime.TotalGameTime.TotalMilliseconds + _delayTimeMs;
        action.Invoke();
    }
}

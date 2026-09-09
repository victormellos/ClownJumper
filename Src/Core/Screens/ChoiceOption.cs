using System;
using Microsoft.Xna.Framework.Input;

namespace ClownJumper;

/// <summary>
/// Representa uma opção dentro de um ChoiceScreen: qual tecla aciona ela,
/// o texto exibido (ex.: "1 - Normal") e qual tela deve ser aberta quando
/// a tecla é pressionada.
///
/// CreateScreen é um Func em vez de uma instância pronta porque a tela de
/// destino só deve ser criada no momento da troca (ela pode depender de
/// estado capturado por closure, como as InputSource/Color de cada
/// jogador no fluxo multiplayer).
/// </summary>
public readonly record struct ChoiceOption(Keys Key, string Label, Func<IScreen> CreateScreen);
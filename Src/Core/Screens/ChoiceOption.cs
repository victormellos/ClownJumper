using System;

namespace ClownJumper;

public readonly record struct ChoiceOption(string Label, Func<IScreen> CreateScreen);
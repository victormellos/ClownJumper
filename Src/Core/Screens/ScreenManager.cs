using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ClownJumper;

public class ScreenManager
{
    private readonly Stack<IScreen> _screenStack = new();

    private IScreen CurrentScreen => _screenStack.Count > 0 ? _screenStack.Peek() : null;

    public void RequestScreenChange(IScreen newScreen)
    {
        _screenStack.Push(newScreen);

        newScreen.Initialize();
        newScreen.LoadContent();
    }


    public void RequestGoBack()
    {
        if (_screenStack.Count <= 1)
            return;

        _screenStack.Pop();
    }

    public void Update(GameTime gameTime)
    {
        CurrentScreen?.Update(gameTime);
    }

    public void Draw()
    {
        CurrentScreen?.Draw();
    }
}
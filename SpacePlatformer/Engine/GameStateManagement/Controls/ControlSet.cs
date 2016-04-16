using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace SpacePlatformer.Engine.GameStateManagement.Controls
{
    public abstract class ControlSet
    {
        protected KeyboardState previousKeyboardState;
        protected GameScreen parent;

        public ControlSet(GameScreen parent)
        {
            previousKeyboardState = Keyboard.GetState();
            this.parent = parent;
        }

        public abstract void HandleInput(GameTime gameTime, InputState input);
    }
}

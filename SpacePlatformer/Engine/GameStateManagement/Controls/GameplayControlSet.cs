using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameStateManagement.Screens;
using SpacePlatformer.Engine.Rendering;
using SpacePlatformer.Engine.GUI;
using Microsoft.Xna.Framework.Content;
using SpacePlatformer.Engine.Entities;

namespace SpacePlatformer.Engine.GameStateManagement.Controls
{
    class GameplayControlSet : ControlSet
    {
        private InputAction pauseAction;
        private Map map;
        private Cursor cursor;
        private ContentManager content;

        public GameplayControlSet (GameScreen parent, Map map, Cursor cursor, ContentManager content) : base(parent) {
            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            this.map = map;
            this.cursor = cursor;
            this.content = content;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)parent.ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;

            if (pauseAction.Evaluate(input, parent.ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                parent.ScreenManager.AddScreen(new PauseMenuScreen(), parent.ControllingPlayer);
#endif
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                    map.player.Move(Direction.Left);

                if (keyboardState.IsKeyDown(Keys.Right))
                    map.player.Move(Direction.Right);

                if (keyboardState.IsKeyDown(Keys.Up))
                    map.player.JumpOn();
                else
                    map.player.JumpOff();

                if (keyboardState.IsKeyDown(Keys.A))
                    ViewHandling.CameraZoom += 0.02f;

                if (keyboardState.IsKeyDown(Keys.Z))
                    ViewHandling.CameraZoom -= 0.02f;

                if (keyboardState.IsKeyDown(Keys.W))
                    Renderer.DrawWireframes = true;
                else
                    Renderer.DrawWireframes = false;

                if (keyboardState.IsKeyDown(Keys.LeftControl))
                    map.timescale = 0.25f;
                else
                    map.timescale = 1.0f;

                if (keyboardState.IsKeyDown(Keys.F2))
                {
                    map = Map.LoadMap("testmap.xml");
                    EntityHandler.Initialize(map);
                }

                cursor.Update(Mouse.GetState());

                previousKeyboardState = keyboardState;
            }
        }
    }
}

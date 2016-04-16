#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using SpacePlatformer.Engine.Entities;
using SpacePlatformer.Engine;
using SpacePlatformer.Engine.Rendering;
using SpacePlatformer.Engine.GUI;
using SpacePlatformer;
#endregion

namespace GameStateManagement.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        public static ContentManager content;
        SpriteFont gameFont;

        private Map map;
        static public Cursor cursor;

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            map = new Map();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("Fonts\\GameFont");

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif

            Renderer.Initialize(ScreenManager.GraphicsDevice);
            GuiHandler.Initialize();

            map = Map.LoadMap("testmap.xml");
            EntityHandler.Initialize(map);
            cursor = new Cursor(new Vector2(), new Material("Images\\Cursor"));
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                map.Update(gameTime);
            }
        }

        KeyboardState lastKeyboardState;
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
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

                if (keyboardState.IsKeyDown(Keys.OemComma) && lastKeyboardState.IsKeyUp(Keys.OemComma))
                    EntityHandler.ThrustersOn();
                else if (lastKeyboardState.IsKeyDown(Keys.OemComma) && keyboardState.IsKeyUp(Keys.OemComma))
                    EntityHandler.ThrustersOff();

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

                if (lastKeyboardState.IsKeyDown(Keys.Space) && keyboardState.IsKeyUp(Keys.Space))
                    map.Pause = !map.Pause;

                if (keyboardState.IsKeyDown(Keys.F2))
                {
                    map = Map.LoadMap("testmap.xml");
                    EntityHandler.Initialize(map);
                }

                if (lastKeyboardState.IsKeyDown(Keys.F5) && keyboardState.IsKeyUp(Keys.F5))
                    map.SaveMap("testmap.xml");
                
                cursor.Update(Mouse.GetState());

                lastKeyboardState = keyboardState;
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            Renderer.RenderMap(map);
            Renderer.RenderGui();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}

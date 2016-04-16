using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePlatformer.Engine.GUI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Editor;
using GameStateManagement.Screens;

namespace SpacePlatformer.Engine.Rendering
{
    static class GuiRendering
    {
        private static BasicEffect defaultGuiShader;
        private static SpriteFont defaultFont;

        internal static void Initialize()
        {
            defaultGuiShader = new BasicEffect(Renderer.Device);
            defaultGuiShader.VertexColorEnabled = true;
            defaultGuiShader.Projection = Matrix.CreateOrthographicOffCenter(
                0, Renderer.Device.Viewport.Width,
                Renderer.Device.Viewport.Height, 0,
                0, 1);
            defaultFont = GameplayScreen.content.Load<SpriteFont>("Fonts\\GameFont");
        }

        public static void RenderWindow(Window window)
        {
            if (!window.Hidden)
            {
                string titleString = window.Title;
                Vector2 titleSize = defaultFont.MeasureString(titleString);
                WindowContents activePage = window.ActivePage;

                defaultGuiShader.Alpha = (float)(window.color.A) / 255.0f;
                defaultGuiShader.CurrentTechnique.Passes[0].Apply();

                //Draw the main background rectangle
                PrimitiveRendering.DrawRectangle(window.screenPosition, window.size);
                //Draw a rectangle on top of the menu rectangle to highlight the currently active menu item
                PrimitiveRendering.DrawRectangle(
                    window.screenPosition + new Vector2(activePage.GetMenuItemWidth * activePage.ActiveMenuItemX, activePage.GetMenuItemHeight(activePage.ActiveMenuItemX) * activePage.ActiveMenuItemY),
                    new Vector2(activePage.GetMenuItemWidth, activePage.GetMenuItemHeight(activePage.ActiveMenuItemX))
                    );
                //Put a background behind the title text
                PrimitiveRendering.DrawRectangle(window.screenPosition + new Vector2(0.0f, -titleSize.Y), new Vector2(window.size.X, titleSize.Y));

                Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                for (int i = 0; i < activePage.frames.Count; i++)
                {
                    for (int j = 0; j < activePage.frames[i].elements.Count; j++)
                    {
                        Vector2 gridPosition = new Vector2(
                            activePage.GetMenuItemWidth  / 2.0f + activePage.GetMenuItemWidth  * i,
                            activePage.GetMenuItemHeight(i) / 2.0f + activePage.GetMenuItemHeight(i) * j
                            );
                        if (activePage.frames[i].elements[j].material != null)
                        {
                            Texture2D texture = activePage.frames[i].elements[j].material.GetTexture(1);
                            Renderer.SpriteBatch.Draw(
                                texture,
                                activePage.GetScreenPosition + gridPosition,
                                null, Color.White, 0.0f,
                                new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),
                                activePage.frames[i].elements[j].material.scale, SpriteEffects.None, 1.0f
                                );
                        }
                        else
                        {
                            Vector2 textSize = defaultFont.MeasureString(activePage.frames[i].elements[j].text);
                            Renderer.SpriteBatch.DrawString(
                                defaultFont,
                                activePage.frames[i].elements[j].text,
                                activePage.GetScreenPosition + gridPosition - textSize * 0.5f,
                                Color.White);
                        }
                    }
                }
                Renderer.SpriteBatch.End();

                float centeredTextOffset = (window.size.X / 2.0f) - titleSize.X / 2.0f;
                RenderToScreen(defaultFont, titleString, new Vector2(window.screenPosition.X + centeredTextOffset, window.screenPosition.Y - titleSize.Y));
            }
        }

        public static void RenderElement(GuiElement gui)
        {
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Texture2D texture = gui.material.GetTexture(1);
            Renderer.SpriteBatch.Draw(texture, gui.screenPosition, Color.White);
            Renderer.SpriteBatch.End();
        }

        public static void RenderToScreen(SpriteFont font, string text, Vector2 position)
        {
            Renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, null);
            Renderer.SpriteBatch.DrawString(font, text, position, Color.White);
            Renderer.SpriteBatch.End();
        }
    }
}

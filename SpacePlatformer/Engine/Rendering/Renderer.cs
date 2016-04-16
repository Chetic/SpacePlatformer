using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Entities;
using SpacePlatformer.Engine.GUI;
using GameStateManagement;
using SpacePlatformer.Engine.Procedural;
using GameStateManagement.Screens;
using FarseerPhysics.Common;

namespace SpacePlatformer.Engine.Rendering
{
    static class Renderer
    {
        public static GraphicsDevice Device { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static bool DrawWireframes { get; set; }
        public static RenderTarget2D MainTarget;
        
        public static void Initialize(GraphicsDevice device)
        {
            Device = device;
            DrawWireframes = false;

            MainTarget = new RenderTarget2D(
                    Renderer.Device,
                    Renderer.Device.Viewport.Width,
                    Renderer.Device.Viewport.Height);

            GuiRendering.Initialize();
            ViewHandling.Initialize();
            DeferredLighting.Initialize();
            DynamicShadows.Initialize();
            PrimitiveRendering.Initialize();
            EntityRendering.Initialize();
            PostEffects.Initialize();

            // Create a new SpriteBatch, which can be used to draw textures
            Renderer.SpriteBatch = new SpriteBatch(device);
        }

        public static void RenderMap(Map map)
        {
            DynamicShadows.Render();
            DeferredLighting.Render(map);
            //PostEffects.Apply();

            Renderer.Device.SetRenderTarget(null);
            Renderer.SpriteBatch.Begin();
            Renderer.SpriteBatch.Draw(MainTarget, new Vector2(), Color.White);
            Renderer.SpriteBatch.End();
        }

        public static void RenderGui()
        {
            GuiRendering.RenderWindow(GuiHandler.EditWindow);
            GuiRendering.RenderElement(GameplayScreen.cursor);

            if (DrawWireframes)
            {
                foreach (Entity entity in EntityHandler.ActiveMap.entities)
                {
                    PrimitiveRendering.DrawWireFrameOf(entity);

                    //Draw center of mass (body center) as blue X
                    //Vector2 center = ViewHandling.WorldToScreenCoords(entity.body.Position);
                    //PrimitiveRendering.DrawLine(center + new Vector2(-4.0f, -4.0f), center + new Vector2(5.0f, 5.0f), Color.Blue);
                    //PrimitiveRendering.DrawLine(center + new Vector2(-4.0f, 4.0f), center + new Vector2(5.0f, -5.0f), Color.Blue);
                }
                foreach (LightSource ls in EntityHandler.ActiveMap.lightSources)
                {
                    PrimitiveRendering.DrawWireFrameOf(ls);
                }
            }
        }
    }
}

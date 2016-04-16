using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SpacePlatformer.Engine.Entities;
using Microsoft.Xna.Framework;
using GameStateManagement.Screens;

namespace SpacePlatformer.Engine.Rendering
{
    static class DynamicShadows
    {
        static QuadRenderComponent quadRender;
        static ShadowmapResolver shadowmapResolver;
        public static RenderTarget2D screenShadows;
        static Texture2D tileTexture;

        internal static void Initialize()
        {
            quadRender = new QuadRenderComponent();
            shadowmapResolver = new ShadowmapResolver(Renderer.Device, quadRender, ShadowmapSize.Size256, ShadowmapSize.Size1024);
            shadowmapResolver.LoadContent();
            screenShadows = new RenderTarget2D(
                Renderer.Device,
                Renderer.Device.Viewport.Width,
                Renderer.Device.Viewport.Height,
                true,
                Renderer.Device.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24Stencil8);
            tileTexture = GameplayScreen.content.Load<Texture2D>("Images\\tile");
        }
        
        internal static void Render()
        {
            Renderer.Device.Clear(Color.CornflowerBlue);

            foreach (LightSource ls in EntityHandler.ActiveMap.lightSources)
            {
                Renderer.Device.SetRenderTarget(ls.RenderTarget);
                Renderer.Device.Clear(Color.Transparent);
                DrawCasters(ls);
                Renderer.Device.SetRenderTarget(null);
                shadowmapResolver.ResolveShadows(ls.RenderTarget, ls.RenderTarget, ls.body.Position);
            }

            Renderer.Device.SetRenderTarget(screenShadows);
            Renderer.Device.Clear(Color.Black); //Ends up being the final background color
            Renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, ViewHandling.ViewMatrix);
            foreach (LightSource ls in EntityHandler.ActiveMap.lightSources)
            {
                Renderer.SpriteBatch.Draw(ls.RenderTarget, (ls.body.Position - ls.size*2.0f)*64.0f, ls.lightColor);
            }
            Renderer.SpriteBatch.End();
        }

        static void DrawCasters(LightSource lightSource)
        {
            Renderer.SpriteBatch.Begin();
            Entity entity;
            EntityRendering.ResetRenderQueue();
            while ((entity = EntityRendering.GetNextRenderEntity()) != null)
            {
                if (entity.material == null)
                    continue;
                Texture2D texture = entity.material.GetTexture(1);
                Renderer.SpriteBatch.Draw(
                    texture,
                    (entity.body.Position - lightSource.body.Position + lightSource.size*2.0f) * 64.0f,
                    null, Color.White, entity.body.Rotation,
                    new Vector2(texture.Width / 2.0f, texture.Height / 2.0f),
                    entity.material.scale, SpriteEffects.None, 0.0f
                    );
            }
            Renderer.SpriteBatch.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SpacePlatformer.Engine.Entities;
using GameStateManagement.Screens;
using Microsoft.Xna.Framework;

namespace SpacePlatformer.Engine.Rendering
{
    static class DeferredLighting
    {
        private static RenderTarget2D _colorMapRenderTarget;
        private static RenderTarget2D _depthMapRenderTarget;
        private static RenderTarget2D _normalMapRenderTarget;
        private static RenderTarget2D _shadowMapRenderTarget;
        private static Effect _lightEffect1;
        private static Effect _lightEffect2;
        private static VertexPositionTexture[] _vertices;

        internal static void Initialize()
        {
            PresentationParameters pp = Renderer.Device.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;
            _colorMapRenderTarget = new RenderTarget2D(Renderer.Device, width, height, true, format, DepthFormat.Depth24Stencil8);
            _depthMapRenderTarget = new RenderTarget2D(Renderer.Device, width, height, true, format, DepthFormat.Depth24Stencil8);
            _normalMapRenderTarget = new RenderTarget2D(Renderer.Device, width, height, true, format, DepthFormat.Depth24Stencil8);
            _shadowMapRenderTarget = new RenderTarget2D(Renderer.Device, width, height, true, format, DepthFormat.Depth24Stencil8);
            _lightEffect1 = GameplayScreen.content.Load<Effect>("Shaders\\Lighting\\DeferredShadow");
            _lightEffect2 = GameplayScreen.content.Load<Effect>("Shaders\\Lighting\\DeferredCombined");
            _vertices = new VertexPositionTexture[4];
            _vertices[0] = new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0));
            _vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0));
            _vertices[2] = new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1));
            _vertices[3] = new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1));
        }

        internal static void Render(Map map)
        {
            // Set the render targets
            Renderer.Device.SetRenderTarget(_colorMapRenderTarget);
            // Clear all render targets
            Renderer.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1, 0);

            //---------- Color map drawing ----------
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, ViewHandling.ViewMatrix);
            foreach (Entity entity in map.entities)
            {
                EntityRendering.RenderToWorld(entity, RenderMode.ColorMap);
            }
            Renderer.SpriteBatch.End();
            //----------------------------------------------

            // Reset the render target
            Renderer.Device.SetRenderTarget(_normalMapRenderTarget);
            // Clear all render targets 
            Renderer.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1, 0);

            //---------- Normal map drawing ----------
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, ViewHandling.ViewMatrix);
            foreach (Entity entity in map.entities)
            {
                EntityRendering.RenderToWorld(entity, RenderMode.NormalMap);
            }
            Renderer.SpriteBatch.End();
            //----------------------------------------------

            Renderer.Device.SetRenderTarget(_depthMapRenderTarget);
            // Clear all render targets 
            Renderer.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1, 0);

            //---------- Depth map drawing ----------
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, ViewHandling.ViewMatrix);
            foreach (Entity entity in map.entities)
            {
                EntityRendering.RenderToWorld(entity, RenderMode.DepthMap);
            }
            Renderer.SpriteBatch.End();
            //----------------------------------------------

            // Gather all the textures from the Rendertargets 
            GenerateShadowMap();

            // Deactive the render targets to resolve them
            Renderer.Device.SetRenderTarget(Renderer.MainTarget);
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Renderer.SpriteBatch.Draw(DynamicShadows.screenShadows, Vector2.Zero, Color.White);
            Renderer.SpriteBatch.End();

            DrawCombinedMaps();
        }

        private static void DrawCombinedMaps()
        {
            _lightEffect2.CurrentTechnique = _lightEffect2.Techniques["DeferredCombined"];
            _lightEffect2.Parameters["ambient"].SetValue(Lighting._ambientPower);
            _lightEffect2.Parameters["ambientColor"].SetValue(Lighting._ambientColor.ToVector4());
            // This variable is used to boost to output of the light sources when they are combined
            // I found 4 a good value for my lights but you can also make this dynamic if you want
            _lightEffect2.Parameters["lightAmbient"].SetValue(4);
            _lightEffect2.Parameters["ColorMap"].SetValue(_colorMapRenderTarget);
            _lightEffect2.Parameters["ShadingMap"].SetValue(_shadowMapRenderTarget);
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            foreach (var pass in _lightEffect2.CurrentTechnique.Passes)
            {
                pass.Apply();
                Renderer.SpriteBatch.Draw(_colorMapRenderTarget, Vector2.Zero, Color.White);
            }
            Renderer.SpriteBatch.End();
        }

        private static Texture2D GenerateShadowMap()
        {
            Renderer.Device.SetRenderTarget(_shadowMapRenderTarget);
            Renderer.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);
            Renderer.Device.DepthStencilState = DepthStencilState.DepthRead;
            Renderer.Device.BlendState = BlendState.AlphaBlend;
            // For every light inside the current scene, you can optimize this
            // list to only draw the lights that are visible a.t.m.
            foreach (LightSource light in EntityHandler.ActiveMap.lightSources)
            {
                //TODO: "continue;" on lights outside screen
                _lightEffect1.CurrentTechnique = _lightEffect1.Techniques["DeferredPointLight"];
                _lightEffect1.Parameters["lightStrength"].SetValue(light.power * ViewHandling.CameraZoom);
                _lightEffect1.Parameters["lightPosition"].SetValue(new Vector3(Vector2.Transform(light.body.Position * 64.0f, ViewHandling.ViewMatrix), 0.0f));
                _lightEffect1.Parameters["lightColor"].SetValue(light.lightColor.ToVector3());
                _lightEffect1.Parameters["lightRadius"].SetValue(light.radius * ViewHandling.CameraZoom);
                _lightEffect1.Parameters["screenWidth"].SetValue(Renderer.Device.Viewport.Width);
                _lightEffect1.Parameters["screenHeight"].SetValue(Renderer.Device.Viewport.Height);
                _lightEffect1.Parameters["NormalMap"].SetValue(_normalMapRenderTarget);
                _lightEffect1.Parameters["DepthMap"].SetValue(_depthMapRenderTarget);
                foreach (var pass in _lightEffect1.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    // Draw the full screen Quad
                    Renderer.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, 2);
                }
            }
            // Deactivate alpha blending...
            Renderer.Device.BlendState = BlendState.AlphaBlend;

            return _shadowMapRenderTarget;
        }
    }
}

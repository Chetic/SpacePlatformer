using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement.Screens;
using Microsoft.Xna.Framework;

namespace SpacePlatformer.Engine.Rendering
{
    static class PostEffects
    {
        private static GaussianBlur gaussianBlur;

        internal static void Initialize()
        {
            gaussianBlur = new GaussianBlur(GameplayScreen.content.Load<Effect>("Shaders\\GaussianBlur"));
            gaussianBlur.ComputeKernel(7, 2.0f);
            InitGaussianBlurRenderTargets();
        }

        private static RenderTarget2D renderTarget1, renderTarget2;
        private static void InitGaussianBlurRenderTargets()
        {
            // Since we're performing a Gaussian blur on a texture image the
            // render targets are half the size of the source texture image.
            // This will help improve the blurring effect.
            int renderTargetWidth;
            int renderTargetHeight;

            renderTargetWidth = Renderer.MainTarget.Width / 2;
            renderTargetHeight = Renderer.MainTarget.Height / 2;

            renderTarget1 = new RenderTarget2D(Renderer.Device,
                renderTargetWidth, renderTargetHeight, false,
                Renderer.Device.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            renderTarget2 = new RenderTarget2D(Renderer.Device,
                renderTargetWidth, renderTargetHeight, false,
                Renderer.Device.PresentationParameters.BackBufferFormat,
                DepthFormat.None);

            // The texture offsets used by the Gaussian blur shader depends
            // on the dimensions of the render targets. The offsets need to be
            // recalculated whenever the render targets are recreated.

            gaussianBlur.ComputeOffsets(renderTargetWidth, renderTargetHeight);
        }

        internal static void Apply()
        {
            Texture2D output = gaussianBlur.PerformGaussianBlur(Renderer.MainTarget, renderTarget1, renderTarget2, Renderer.SpriteBatch);
            BasicEffect transparencyEffect = new BasicEffect(Renderer.Device);
            transparencyEffect.Alpha = 0.25f;

            Renderer.Device.SetRenderTarget(Renderer.MainTarget);
            Renderer.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Renderer.Device.SetRenderTarget(Renderer.MainTarget);
            //transparencyEffect.CurrentTechnique.Passes[0].Apply();
            Renderer.SpriteBatch.Draw(output, new Vector2(), null, Color.White);
            Renderer.SpriteBatch.End();
        }
    }
}

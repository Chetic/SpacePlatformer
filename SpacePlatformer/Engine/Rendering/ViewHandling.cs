using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePlatformer.Engine.Rendering
{
    static class ViewHandling
    {
        public static float CameraZoom = 1.0f;
        public static Vector2 CameraPosition = new Vector2();
        public static Matrix ViewMatrix { get; private set; }

        static Matrix bgViewMatrix;
        static Vector2 screenCenter;
        static Texture2D backgroundTexture;

        internal static void Initialize()
        {
            ViewMatrix = Matrix.Identity;
            bgViewMatrix = Matrix.Identity;

            screenCenter = new Vector2(Renderer.Device.Viewport.Width / 2f,
                                       Renderer.Device.Viewport.Height / 2f);

            backgroundTexture = Background.GenerateStarfield(Renderer.Device, Renderer.Device.Viewport.Width, Renderer.Device.Viewport.Height);
        }

        public static void UpdateCamera(Vector2 playerPosition)
        {
            ViewMatrix =
                Matrix.CreateTranslation(new Vector3(CameraPosition, 0.0f)) *
                Matrix.CreateScale(CameraZoom) *
                Matrix.CreateTranslation(new Vector3(screenCenter, 0.0f)); //Center screen on 0,0 (ignoring camera position)

            bgViewMatrix =
                Matrix.CreateTranslation(new Vector3(-new Vector2(backgroundTexture.Width * 64.0f / 14.0f, backgroundTexture.Height * 64.0f / 14.0f), 0.0f)) *
                Matrix.CreateTranslation(new Vector3(CameraPosition, 0.0f)) *
                Matrix.CreateScale(CameraZoom / 4.0f) *
                Matrix.CreateTranslation(new Vector3(screenCenter, 0.0f));
        }

        public static Vector2 WorldToScreenCoords(Vector2 worldPos)
        {
            return 64.0f * (worldPos * CameraZoom) + screenCenter + CameraZoom * CameraPosition;
        }

        public static Vector2 ScreenToWorldCoords(Vector2 screenPos)
        {
            return (((screenPos - screenCenter) / CameraZoom) - (CameraPosition)) / (64.0f);
        }
    }
}

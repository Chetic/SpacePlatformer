using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePlatformer.Engine
{
    static class Background
    {
        /// <summary>
        /// Generates a starfield texture for use as a background
        /// </summary>
        /// <param name="width">Desired texture width</param>
        /// <param name="height">Desired texture height</param>
        /// <returns>A black texture with every pixel having a 1 in 1000 chance of being white</returns>
        public static Texture2D GenerateStarfield(GraphicsDevice graphicsDevice, int width, int height)
        {
            Random rnd = new Random();
            Texture2D texture = new Texture2D(graphicsDevice, width, height);
            uint[] rawData = new uint[width*height];

            for (int i = 0; i < rawData.Length; i++)
            {
                if (rnd.Next(1000) == 0)
                    rawData[i] = 0xFFFFFFFF;
                else
                    rawData[i] = 0xFF000000;
            }

            texture.SetData(rawData);

            return texture;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Rendering;

namespace SpacePlatformer.Engine.Procedural
{
    static class TextureGenerator
    {
        /// <summary>
        /// Modifies a texture to have a border of white pixels
        /// </summary>
        /// <param name="texture">Texture to modify by reference</param>
        static public void AddBorder(Texture2D texture)
        {
            int width  = texture.Width;
            int height = texture.Height;
            UInt32[] pixels = new UInt32[width * height];
            texture.GetData<UInt32>(pixels);
            for (int i = 0; i < width; i++)
            {
                pixels[i] = 0xFFFFFFFF;
                pixels[i + (width) * (height - 1)] = 0xFFFFFFFF;
            }
            for (int i = 0; i < height; i++)
            {
                pixels[width * i] = 0xFFFFFFFF;
                pixels[width * i + (width - 1)] = 0xFFFFFFFF;
            }
            texture.SetData<UInt32>(pixels);
        }

        /// <summary>
        /// Fills a texture with a specified color
        /// </summary>
        /// <param name="texture">Texture to modify by reference</param>
        /// <param name="color">Color to fill texture with</param>
        static public void FillTexture(Texture2D texture, Color color)
        {
            Color[] pixels = new Color[texture.Width * texture.Height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetData<Color>(pixels);
        }

        /// <summary>
        /// Creates a single-color mask based on an input texture
        /// </summary>
        /// <param name="texture">Texture to use as mask</param>
        /// <param name="color">Color to set pixels with alpha > 0 to</param>
        /// <returns>A new copy of texture masked to color</returns>
        static public Texture2D GenerateMaskedColorTexture(Texture2D texture, Color color)
        {
            Texture2D output = new Texture2D(Renderer.Device, texture.Width, texture.Height);
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].A > 0)
                    pixels[i] = color;
            }
            output.SetData<Color>(pixels);
            return output;
        }
    }
}

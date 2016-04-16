using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePlatformer.Engine.Rendering;
using SpacePlatformer.Engine.Procedural;
using System.Xml;

namespace SpacePlatformer.Engine.Entities
{
    class Block : Entity
    {
        public Block(Vector2 position, Vector2 size, bool isStatic)
            : base(position, size, null, isStatic)
        {
            GenerateNoiseTexture();
        }

        private void GenerateNoiseTexture()
        {
            int width = (int)(size.X * 64);
            int height = (int)(size.Y * 64);
            Texture2D tex = new Texture2D(Renderer.Device, width, height);
            Noise2d.GenerateNoiseMap(width, height, ref tex, 5);
            TextureGenerator.AddBorder(tex);
            this.material = new Material();
            this.material.ApplyTexture(1, tex);
        }
    }
}

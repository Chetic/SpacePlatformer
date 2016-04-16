using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePlatformer.Engine.Rendering;

namespace SpacePlatformer.Engine.GUI
{
    public class GuiElement
    {
        public Vector2 screenPosition;
        public Material material;
        public string text;

        public GuiElement(Vector2 position, string text)
        {
            this.screenPosition = position;
            this.text = text;
        }

        public GuiElement(Vector2 position, Material material)
        {
            this.screenPosition = position;
            this.material = material;
        }
    }
}

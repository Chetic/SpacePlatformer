using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Rendering;
using SpacePlatformer.Engine.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePlatformer.Engine.GUI
{
    class GuiButton : GuiElement
    {
        Action action;

        public GuiButton(Vector2 position, Material material)
            : base(position, material)
        {
            this.action = () =>
            {
                Material entMaterial = GuiHandler.EditWindow.ActivePage.GetActiveElement().material;
                Texture2D entTexture = entMaterial.GetTexture(1);
                Entity newEntity = new ConcaveMesh(Cursor.storedClickPosition, new Vector2(entTexture.Width, entTexture.Height) / 64.0f, entMaterial, false, entTexture.Name);
                EntityHandler.Add(newEntity);
            };
        }

        public GuiButton(Vector2 position, Material material, Action action)
            : base(position, material)
        {
            this.action = action;
        }

        public GuiButton(Vector2 position, string text, Action action)
            : base(position, text)
        {
            this.action = action;
        }

        public void OnClick()
        {
            action();
        }
    }
}

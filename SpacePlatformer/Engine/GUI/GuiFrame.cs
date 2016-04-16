using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Rendering;
using GameStateManagement.Screens;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace SpacePlatformer.Engine.GUI
{
    /// <summary>
    /// A drawable container for GUI elements.
    /// </summary>
    class GuiFrame
    {
        public List<GuiElement> elements;
        public int ActiveElement { get; private set; }
        public float MenuItemHeight { get; private set; }
        public float sizeY { get; set; }

        public GuiFrame(float height)
        {
            elements = new List<GuiElement>();
            this.sizeY = height;
        }

        /// <summary>
        /// Adds a GuiElement which will be rendered with coordinates relative to the GuiFrame
        /// </summary>
        /// <param name="frame">The element to be added on top of the frame</param>
        internal virtual void AddElement(GuiElement element)
        {
            elements.Add(element);
            MenuItemHeight = sizeY / elements.Count;
        }

        internal void Update(float mousePositionY)
        {
            ActiveElement = (int)((int)(mousePositionY) / MenuItemHeight);
            if (ActiveElement < 0)
                ActiveElement = 0;
            if (ActiveElement >= elements.Count)
                ActiveElement = elements.Count - 1;

        }
    }
}

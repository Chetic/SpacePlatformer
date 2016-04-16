using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePlatformer.Engine.GUI;
using Microsoft.Xna.Framework;

namespace SpacePlatformer.Engine.Editor
{
    class WindowContents
    {
        public List<GuiFrame> frames;

        public float GetMenuItemWidth { get; private set; }
        public float GetMenuItemHeight(int menuItem) { return frames[menuItem].MenuItemHeight; }

        public int ActiveMenuItemX = 0;
        public int ActiveMenuItemY { get { return frames[ActiveMenuItemX].ActiveElement; } }

        public Vector2 GetScreenPosition { get { return parentWindow.screenPosition; } } //TODO: +offset
        private Window parentWindow;
        private Vector2 size { get { return parentWindow.size; } }

        public WindowContents(Window parentWindow)
        {
            this.parentWindow = parentWindow;
            this.frames = new List<GuiFrame>();
        }

        /// <summary>
        /// Adds a GuiFrame which will be rendered with coordinates relative to the EditWindow
        /// </summary>
        /// <param name="frame">The element to be added on top of the frame</param>
        internal virtual void AddFrame(GuiFrame frame)
        {
            frames.Add(frame);
            GetMenuItemWidth = this.size.X / (frames.Count);
        }

        /// <summary>
        /// Updates the state of the Window contents based on where the mouse is pointing
        /// </summary>
        /// <param name="mousePosition">The position of the Cursor in screen coordinates</param>
        public void Update(Vector2 mousePosition)
        {
            ActiveMenuItemX = (int)((int)(mousePosition.X - this.GetScreenPosition.X) / GetMenuItemWidth);

            //Clamp item selection
            if (ActiveMenuItemX < 0)
                ActiveMenuItemX = 0;
            if (ActiveMenuItemX >= frames.Count)
                ActiveMenuItemX = frames.Count - 1;

            frames[ActiveMenuItemX].Update(mousePosition.Y - this.GetScreenPosition.Y);
        }

        internal GuiElement GetActiveElement()
        {
            return this.frames[ActiveMenuItemX].elements[ActiveMenuItemY];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpacePlatformer.Engine.GUI;
using Microsoft.Xna.Framework;

namespace SpacePlatformer.Engine.Editor
{
    /// <summary>
    /// Controls the position, title and size of a Window
    /// Contains a scrollable list of WindowContents (controlled by left/right buttons)
    /// </summary>
    class Window
    {
        //TODO: Window definitions in files

        public bool Hidden { get; set; }
        public string Title { get; private set; }

        public Vector2 screenPosition;
        public Vector2 size;
        public List<WindowContents> pages;
        public Color color;

        public WindowContents ActivePage { get { return pages[activePageIndex]; } }
        private int activePageIndex;

        private GuiFrame scrollerButtonsFrame;

        public Window(string title, Vector2 screenPosition, Vector2 size)
        {
            this.Title = title;
            this.screenPosition = screenPosition;
            this.size = size;
            this.color = Color.White;
            this.color.A = (byte)127;
            this.Hidden = true;
            this.activePageIndex = 0;
            this.pages = new List<WindowContents>();
            scrollerButtonsFrame = new GuiFrame(this.size.Y);
            scrollerButtonsFrame.AddElement(new GuiButton(new Vector2(), "<", () => { GuiHandler.EditWindow.PrevPage(); }));
            scrollerButtonsFrame.AddElement(new GuiButton(new Vector2(), ">", () => { GuiHandler.EditWindow.NextPage(); }));
            scrollerButtonsFrame.AddElement(new GuiButton(new Vector2(), "0/0", () => {}));
        }

        internal void ShowAt(Vector2 position)
        {
            this.screenPosition = position - this.size / 2.0f;
            this.Hidden = false;
        }

        internal void Hide()
        {
            this.Hidden = true;
        }

        internal void AddPage(WindowContents page)
        {
            pages.Add(page);
            pages[pages.Count - 1].AddFrame(scrollerButtonsFrame);
        }

        /// <summary>
        /// Updates the state of the Window based on where the mouse is pointing
        /// </summary>
        /// <param name="mousePosition">The position of the Cursor in screen coordinates</param>
        public void Update(Vector2 mousePosition)
        {
            ActivePage.Update(mousePosition);

            //Update page counter - e.g. "2/3"
            scrollerButtonsFrame.elements[scrollerButtonsFrame.elements.Count - 1].text = (activePageIndex + 1).ToString() + "/" + pages.Count;
        }

        internal void NextPage()
        {
            activePageIndex++;
            if (activePageIndex >= pages.Count)
                activePageIndex = 0;
        }

        internal void PrevPage()
        {
            activePageIndex--;
            if (activePageIndex < 0)
                activePageIndex = pages.Count - 1;
        }
    }
}

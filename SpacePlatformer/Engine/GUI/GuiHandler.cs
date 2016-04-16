using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Rendering;
using SpacePlatformer.Engine.Editor;
using SpacePlatformer.Engine.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePlatformer.Engine.GUI
{
    static class GuiHandler
    {
        public static Window EditWindow { get; set; }

        internal static void Initialize()
        {
            GuiHandler.EditWindow = new Window("Editor", new Vector2(), new Vector2(640, 480));
            /*-------------- Page 1 ---------------*/
            WindowContents TestPage1 = new WindowContents(GuiHandler.EditWindow);
            TestPage1.frames.Clear();
            GuiFrame innerFrame = new GuiFrame(480.0f);
            innerFrame.AddElement(new GuiButton(new Vector2(), new Material("Images\\freddy_box")));
            innerFrame.AddElement(new GuiButton(new Vector2(), new Material("Images\\DefaultParticle")));
            innerFrame.AddElement(new GuiButton(new Vector2(), new Material("Images\\Animated\\Alien")));
            innerFrame.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\sampleShip1")));
            innerFrame.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\meteorBig")));
            TestPage1.AddFrame(innerFrame);
            GuiFrame innerFrame2 = new GuiFrame(480.0f);
            innerFrame2.AddElement(new GuiButton(new Vector2(), new Material("Images\\SmokeParticle")));
            innerFrame2.AddElement(new GuiButton(new Vector2(), new Material("Images\\ThrusterIcon"), () =>
            {
                Thruster newThruster = new Thruster(Cursor.storedClickPosition, TestPage1.GetActiveElement().material);
                EntityHandler.Add(newThruster);
            }));
            innerFrame2.AddElement(new GuiButton(new Vector2(), new Material("Images\\Cursor")));
            innerFrame2.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\sampleShip2")));
            innerFrame2.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\meteorSmall")));
            TestPage1.AddFrame(innerFrame2);
            GuiFrame innerFrame3 = new GuiFrame(480.0f);
            innerFrame3.AddElement(new GuiButton(new Vector2(), new Material("Images\\Player")));
            innerFrame3.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\sampleShip3")));
            innerFrame3.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\enemyShip")));
            innerFrame3.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\weapon1")));
            innerFrame3.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\weapon2")));
            innerFrame3.AddElement(new GuiButton(new Vector2(), new Material("Images\\External\\weapon3")));
            TestPage1.AddFrame(innerFrame3);
            GuiHandler.EditWindow.AddPage(TestPage1);

            /*-------------- Page 2 ---------------*/
            WindowContents TestPage2 = new WindowContents(GuiHandler.EditWindow);
            TestPage2.frames.Clear();
            GuiFrame page2_column1 = new GuiFrame(180.0f);
            page2_column1.AddElement(new GuiButton(new Vector2(), "Set Parent", () => { Cursor.StartSetParent(); }));
            page2_column1.AddElement(new GuiButton(new Vector2(), "Add child", () => { Cursor.StartAddChild(); }));
            page2_column1.AddElement(new GuiButton(new Vector2(), "Clear parent", () => { EntityHandler.ClearParent(Cursor.tempEntity); }));
            page2_column1.AddElement(new GuiButton(new Vector2(), "Clear children", () => { EntityHandler.ClearChildren(Cursor.tempEntity); }));
            GuiFrame page2_column2 = new GuiFrame(180.0f);
            page2_column2.AddElement(new GuiButton(new Vector2(), "Remove", () => { EntityHandler.Remove(Cursor.tempEntity); }));
            page2_column2.AddElement(new GuiButton(new Vector2(), "Toggle Static", () => { Cursor.tempEntity.body.IsStatic = !Cursor.tempEntity.body.IsStatic; }));
            TestPage2.AddFrame(page2_column1);
            TestPage2.AddFrame(page2_column2);
            GuiHandler.EditWindow.AddPage(TestPage2);

            /*-------------- Page 3 ---------------*/
            WindowContents TestPage3 = new WindowContents(GuiHandler.EditWindow);
            TestPage3.frames.Clear();
            GuiFrame page3_column1 = new GuiFrame(180.0f);
            page3_column1.AddElement(new GuiButton(new Vector2(), "Add Light", () => { EntityHandler.Add(new LightSource(Cursor.storedClickPosition, new Material("Images\\lightmask"))); }));
            TestPage3.AddFrame(page3_column1);
            GuiHandler.EditWindow.AddPage(TestPage3);
        }

        internal static void ClickActiveButton()
        {
            GuiElement button = GuiHandler.EditWindow.ActivePage.GetActiveElement();
            if (button is GuiButton)
                ((GuiButton)button).OnClick();
        }
    }
}

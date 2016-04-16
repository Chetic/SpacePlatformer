using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpacePlatformer.Engine.Entities;
using SpacePlatformer.Engine.Rendering;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SpacePlatformer.Engine.Editor;
using GameStateManagement.Screens;
using GameStateManagement;
using FarseerPhysics.Collision;

namespace SpacePlatformer.Engine.GUI
{
    public class Cursor : GuiElement
    {
        static public Entity tempEntity, secondaryTempEntity;
        static public Vector2 storedClickPosition;
        static private Vector2 worldPosition;
        static private bool leftMouseDown = false, middleMouseDown = false;
        static private bool rightMouseDown = false;

        public Cursor(Vector2 position, Material material)
            : base(position, material)
        {
            worldPosition = ViewHandling.ScreenToWorldCoords(position);
        }
        
        MouseState lastMouseState = new MouseState();
        Vector2 lastPosition = new Vector2();
        public void Update(MouseState mouseState)
        {
            if (lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
                OnLeftButtonDown();
            if (lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                OnLeftButtonUp();
            if (lastMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed)
                OnRightButtonDown();
            if (lastMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
                OnRightButtonUp();
            if (lastMouseState.MiddleButton == ButtonState.Released && mouseState.MiddleButton == ButtonState.Pressed)
                OnMiddleButtonDown();
            if (lastMouseState.MiddleButton == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Released)
                OnMiddleButtonUp();
            if (lastMouseState.ScrollWheelValue > mouseState.ScrollWheelValue)
                OnScrollDown();
            else if (lastMouseState.ScrollWheelValue < mouseState.ScrollWheelValue)
                OnScrollUp();

            screenPosition = new Vector2((float)(Mouse.GetState().X), (float)(Mouse.GetState().Y));
            worldPosition = ViewHandling.ScreenToWorldCoords(screenPosition);

            //If conditions are right to move an entity
            if (tempEntity != null && leftMouseDown && !rightMouseDown)
            {
                tempEntity.body.ResetDynamics();
                //Apply force towards cursor if unpaused, otherwise move entity
                if (EntityHandler.ActiveMap.Pause || tempEntity.body.IsStatic)
                {
                    tempEntity.body.Awake = true; //Avoids objects hanging in the air
                    tempEntity.body.Position = Cursor.worldPosition - storedClickPosition;
                }
                else
                {
                    Vector2 bodyToCursor = (Cursor.worldPosition - tempEntity.body.Position);
                    tempEntity.body.ApplyLinearImpulse(bodyToCursor * tempEntity.body.Mass * bodyToCursor.LengthSquared());
                }
            }

            if (middleMouseDown)
                ViewHandling.CameraPosition -= (lastPosition - screenPosition) * (1.0f / ViewHandling.CameraZoom);

            //If an Entity is not currently being manipulated, figure out if Cursor is touching anything
            if (!leftMouseDown && !rightMouseDown)
            {
                tempEntity = null;
                foreach (Entity ent in EntityHandler.ActiveMap.entities)
                {
                    foreach (Fixture fixture in ent.body.FixtureList)
                    {
                        if (fixture.TestPoint(ref worldPosition))
                        {
                            tempEntity = ent;
                        }
                    }
                }
                foreach (LightSource ent in EntityHandler.ActiveMap.lightSources)
                {
                    foreach (Fixture fixture in ent.body.FixtureList)
                    {
                        if (fixture.TestPoint(ref worldPosition))
                        {
                            tempEntity = ent;
                        }
                    }
                }
            }

            GuiHandler.EditWindow.Update(this.screenPosition);

            lastMouseState = mouseState;
            lastPosition = screenPosition;
        }

        public void OnLeftButtonDown()
        {
            leftMouseDown = true;

            if (rightMouseDown)
            {
                GuiHandler.ClickActiveButton();
            }
            else if (tempEntity != null)
            {
                storedClickPosition = worldPosition - tempEntity.body.Position;
            }
        }

        public void OnLeftButtonUp()
        {
            leftMouseDown = false;

            if (!rightMouseDown && settingParent)
            {
                settingParent = false;
                EntityHandler.SetParent(tempEntity, secondaryTempEntity);
            }
            else if (!rightMouseDown && addingChild)
            {
                addingChild = false;
                EntityHandler.AddChild(secondaryTempEntity, tempEntity);
            }
        }

        public void OnMiddleButtonDown()
        {
            middleMouseDown = true;
        }

        public void OnMiddleButtonUp()
        {
            middleMouseDown = false;
        }

        public void OnRightButtonDown()
        {
            rightMouseDown = true;
            storedClickPosition = worldPosition;
            GuiHandler.EditWindow.ShowAt(screenPosition);
        }

        public void OnRightButtonUp()
        {
            rightMouseDown = false;
            GuiHandler.EditWindow.Hide();
        }

        private const float rotationGrid = (float)Math.PI / 8.0f;
        private void OnScrollDown()
        {
            if (MovingEntity())
                GeneralHelper.RotateByGrid(tempEntity, rotationGrid);
            else
                ViewHandling.CameraZoom -= 0.05f;
        }

        private void OnScrollUp()
        {
            if (MovingEntity())
                GeneralHelper.RotateByGrid(tempEntity, -rotationGrid);
            else
                ViewHandling.CameraZoom += 0.05f;
        }

        private bool MovingEntity()
        {
            return tempEntity != null && leftMouseDown && !rightMouseDown;
        }

        static bool settingParent = false;
        internal static void StartSetParent()
        {
            secondaryTempEntity = Cursor.tempEntity;
            settingParent = true;
        }

        static bool addingChild = false;
        internal static void StartAddChild()
        {
            secondaryTempEntity = Cursor.tempEntity;
            addingChild = true;
        }
    }
}

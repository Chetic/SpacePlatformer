using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using SpacePlatformer.Engine.GUI;
using SpacePlatformer.Engine.Rendering;
using GameStateManagement.Screens;
using Microsoft.Xna.Framework.Graphics;

namespace SpacePlatformer.Engine.Entities
{
    static class EntityHandler
    {
        static public Map ActiveMap { get; private set; }

        static public void Initialize(Map map)
        {
            ActiveMap = map;

            map.world.Clear();

            //TODO: Force a "Deserialization finished" callback instead of the code below ------------
            ActiveMap.player.body = BodyFactory.CreateRectangle(EntityHandler.ActiveMap.world, ActiveMap.player.size.X, ActiveMap.player.size.Y, 1.0f, ActiveMap.player.startPosition);
            ActiveMap.player.body.FixedRotation = true;
            foreach (Entity entity in ActiveMap.entities)
            {
                if (entity.body == null)
                    entity.body = BodyFactory.CreateRectangle(EntityHandler.ActiveMap.world, entity.size.X, entity.size.Y, 1.0f, entity.startPosition);
                entity.body.IsStatic = entity.startStatic;
                entity.body.Rotation = entity.startRotation;

                for (int i = 0; i < Material.MaxLayers; i++)
                {
                    if (entity.material.textureLayers[i] != null)
                    {
                        if (entity.material.textureLayers[i].Name != null && entity.material.textureLayers[i].Name != "")
                            entity.material.textureLayers[i] = GameplayScreen.content.Load<Texture2D>(entity.material.textureLayers[i].Name);
                    }
                }

                //Avoid re-casting with keyword 'is'
                ConcaveMesh concaveMesh = entity as ConcaveMesh;
                if (concaveMesh != null)
                {
                    concaveMesh.meshTexture = GameplayScreen.content.Load<Texture2D>(concaveMesh.meshTexture.Name);
                    concaveMesh.PerformTextureToMesh();
                }
            }
            foreach (LightSource ls in ActiveMap.lightSources)
            {
                ls.Init();
            }
            //-----------------------------------------------------------------------------------------
        }

        static public void Add(Entity entity)
        {
            entity.id = entity.body.BodyId;

            if (entity is LightSource)
            {
                ActiveMap.lightSources.Add((LightSource)entity);
            }
            else
            {
                ActiveMap.entities.Add(entity);
            }
        }

        static public void Remove(Entity entity)
        {
            ActiveMap.world.RemoveBody(entity.body);
            ActiveMap.entities.Remove(entity);
        }

        static public void Update(float dt)
        {
            ActiveMap.world.Step(dt);

            foreach (Entity entity in ActiveMap.entities)
            {
                //This is a workaround for when entities happen to be thrown around as null.
                if (entity == null)
                    continue;
                entity.Update(dt);
            }
        }

        public static void SetParent(Entity parent, Entity child)
        {
            if (parent == child || parent == null || child == null)
                return;

            child.parent = parent;
            parent.children.Add(child);
            child.parentWeldJoint = JointFactory.CreateWeldJoint(EntityHandler.ActiveMap.world, parent.body, child.body, parent.body.Position);
        }

        internal static void AddChild(Entity parent, Entity child)
        {
            SetParent(parent, child);
        }

        internal static void ClearParent(Entity child)
        {
            if (child == null) return;
            if (child.parent != null)
            {
                child.parent.children.Remove(child);
                child.parent = null;
                ActiveMap.world.RemoveJoint(child.parentWeldJoint);
            }
        }

        internal static void ClearChildren(Entity entity)
        {
            if (entity == null) return;
            while (entity.children.Count > 0)
                ClearParent(entity.children[0]);
        }

        public static void ThrustersOn()
        {
            foreach (Entity entity in ActiveMap.entities)
            {
                if (entity is Thruster)
                {
                    ((Thruster)entity).active = true;
                }
            }
        }

        public static void ThrustersOff()
        {
            foreach (Entity entity in ActiveMap.entities)
            {
                if (entity is Thruster)
                {
                    ((Thruster)entity).active = false;
                }
            }
        }

        /// <summary>
        /// Returns the top-most entity at a specified world position
        /// </summary>
        /// <param name="position">The location at which to look for an entity</param>
        /// <returns>The entity at the specified position. null if no Entity is found</returns>
        internal static Entity GetEntityAt(Vector2 position)
        {
            foreach (Entity entity in ActiveMap.entities)
            {
                //Grabs the first entity with its center within 0.25f of cursor pointer
                if ((position - entity.body.Position).Length() < 0.25f)
                    return entity;
            }
            return null;
        }

        internal static Entity GetEntityById(int id)
        {
            foreach (Entity entity in ActiveMap.entities)
            {
                if (entity.id == id)
                    return entity;
            }

            return null;
        }
    }
}

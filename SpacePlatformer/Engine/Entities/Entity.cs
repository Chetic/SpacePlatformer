#define FORCED_PARENTING

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System;
using SpacePlatformer.Engine.Rendering;
using System.Collections.Generic;
using System.Xml;
using GameStateManagement.Screens;
using FarseerPhysics.Dynamics.Joints;
using System.Xml.Serialization;

namespace SpacePlatformer.Engine.Entities
{
    [XmlInclude(typeof(ConcaveMesh))]
    [XmlInclude(typeof(Player))]
    public class Entity
    {
        [XmlIgnore]
        public Body body;
        [XmlIgnore]
        public WeldJoint parentWeldJoint;

        public Vector2 startPosition;
        public Vector2 position { get { return body.Position; } set { startPosition = value; } }
        public bool startStatic;
        public bool IsStatic { get { return body.IsStatic; } set { startStatic = value; } }
        public float startRotation;
        public float rotation { get { return body.Rotation; } set { startRotation = value; } }

        public bool hidden = false;
        public Vector2 size;
        public Entity parent;
        public List<Entity> children;
        public Material material;
        public int id;

        public Entity()
        {
        }

        public Entity(Vector2 position, Vector2 size, Material material, bool isStatic)
        {
            this.material = material;
            this.size = size;
            this.body = BodyFactory.CreateRectangle(EntityHandler.ActiveMap.world, size.X, size.Y, 1.0f, position);

            body.IsStatic = isStatic;

            children = new List<Entity>();
        }
        
        virtual public void Update(float dt)
        {
        }
    }
}

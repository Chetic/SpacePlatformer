using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using FarseerPhysics.Common.Decomposition;
using SpacePlatformer.Engine.Rendering;
using System.Xml;
using GameStateManagement.Screens;
using System;

namespace SpacePlatformer.Engine.Entities
{
    public class ConcaveMesh : Entity
    {
        public Texture2D meshTexture;
        private List<Vertices> verticesList;
        public List<Vertices> GetVerticesList() { return verticesList; }

        public ConcaveMesh()
            : base()
        {
        }

        public ConcaveMesh(Vector2 position, Vector2 size, Material material, bool isStatic, string meshTextureName)
            : base(position, size, material, isStatic)
        {
            this.meshTexture = GameplayScreen.content.Load<Texture2D>(meshTextureName);
            this.meshTexture.Name = meshTextureName;
            PerformTextureToMesh();
        }

        internal void PerformTextureToMesh()
        {
            World world = EntityHandler.ActiveMap.world;
            bool isStatic = body.IsStatic;
            Vector2 position = body.Position;
            float rotation = body.Rotation;

            //Remove the bounding box collision detection hull
            body.Dispose();

            //Convert texture to raw 32-bit pixel data
            uint[] polyData = new uint[meshTexture.Width * meshTexture.Height];
            meshTexture.GetData(polyData);

            //Find vertices by alpha channel
            Vertices verts = PolygonTools.CreatePolygon(polyData, meshTexture.Width, true);
            //Scale to 64 pixels per meter and put entity origin in the center
            Vector2 scale = new Vector2(1.0f / 64.0f);
            Vector2 center = new Vector2(-(meshTexture.Width / 2.0f), -(meshTexture.Height / 2.0f))*scale;
            verts.Scale(ref scale);
            verts.Translate(ref center);

            //Generate a list of convex polygons to make up the concave mesh and use as the physical entity's body
            verticesList = BayazitDecomposer.ConvexPartition(verts);
            body = BodyFactory.CreateCompoundPolygon(world, verticesList, 1.0f);

            body.BodyType = isStatic ? BodyType.Static : BodyType.Dynamic;
            body.Position = position;
            body.Rotation = rotation;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common.Decomposition;

namespace SpacePlatformer.Engine
{
    public static class MeshCreation
    {
        public static List<Vertices> GenerateFromTexture(Texture2D t)
        {
            //Convert texture to raw 32-bit pixel data
            uint[] polyData = new uint[t.Width * t.Height];
            t.GetData(polyData);

            //Find vertices by alpha channel
            Vertices verts = PolygonTools.CreatePolygon(polyData, t.Width, true);

            //Scale to 64 pixels per meter and put entity origin in the center
            Vector2 scale = new Vector2(1.0f / 64.0f);
            Vector2 center = new Vector2(-(t.Width / (64.0f * 2.0f)), -(t.Height / (64.0f * 2.0f)));
            verts.Scale(ref scale);
            verts.Translate(ref center);

            //Generate a list of convex polygons to make up the concave mesh and use as the physical entity's body
            List<Vertices> vertList = BayazitDecomposer.ConvexPartition(verts);

            return vertList;
        }
    }
}

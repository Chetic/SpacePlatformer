using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace SpacePlatformer.Engine
{
    public static class WorldHandler
    {
        public static World world;

        public static void Initialize()
        {
            world = new World(Vector2.Zero);
        }
    }
}

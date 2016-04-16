using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePlatformer.Engine.Rendering;

namespace SpacePlatformer.Engine.Entities
{
    class NPC : Entity
    {
        public NPC(Vector2 position, Vector2 size, Material material, bool isStatic)
            : base(position, size, material, false)
        {
        }
    }
}

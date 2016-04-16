using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpacePlatformer.Engine.Rendering;
using System.Xml;

namespace SpacePlatformer.Engine.Entities
{
    class Thruster : Entity
    {
        public bool active = false;
        private float thrustage = 0.0f;

        public Thruster()
            : base()
        {
        }

        public Thruster(Vector2 position, Material thrusterMaterial)
            : base(position, new Vector2(thrusterMaterial.GetTexture(1).Width / 64.0f, thrusterMaterial.GetTexture(1).Height / 64.0f), thrusterMaterial, false)
        {
        }

        override public void Update(float dt)
        {
            base.Update(dt);

            thrustage /= 10.0f;

            if (active)
            {
                thrustage += 20.0f;

                foreach (Entity entity in children)
                {
                    if (entity is ParticleEmitter)
                        ((ParticleEmitter)entity).active = true;
                }
            }
            else
            {
                foreach (Entity entity in children)
                {
                    if (entity is ParticleEmitter)
                        ((ParticleEmitter)entity).active = false;
                }
            }

            this.body.ApplyForce(thrustage * new Vector2((float)Math.Cos(this.body.Rotation), (float)Math.Sin(this.body.Rotation)));
        }
    }
}

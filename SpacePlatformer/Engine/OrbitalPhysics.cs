using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SpacePlatformer.Engine.Entities;

namespace SpacePlatformer.Engine
{
    static class OrbitalPhysics
    {
        public const float G = 1.0f; //Gravitational constant

        /// <summary>
        /// Returns the gravitational force vector from 'of' towards 'on'.
        /// </summary>
        public static Vector2 GetNewtonForceVector(Entity of, Entity on)
        {
            float m1 = of.body.Mass;
            float m2 = on.body.Mass;
            Vector2 r = (of.body.Position - on.body.Position);

            return (r *
                G * ((m1 * m2) / (r.LengthSquared())) //Newton's Law of Universal Gravity
                );
        }

        /// <summary>
        /// Returns the necessary impulse to put 'orbiter' in a circular orbit around 'around'.
        /// </summary>
        public static Vector2 GetCircularOrbitImpulse(Entity orbiter, Entity around)
        {
            float majorAxisMultiplier = 1.0f;
            Vector2 v = around.body.Position - orbiter.body.Position;
            Vector2 r = v;
            v = Vector2.Transform(v, Matrix.CreateRotationZ((float)Math.PI / 2.0f));
            v = (v / v.Length()) * (float)(Math.Sqrt(G * around.body.Mass / (r.Length() * majorAxisMultiplier)));
            return v * 100.0f;
        }
    }
}

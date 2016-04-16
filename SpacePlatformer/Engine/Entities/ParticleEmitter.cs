using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SpacePlatformer.Engine.Rendering;

namespace SpacePlatformer.Engine.Entities
{
    public class Particle : Entity
    {
        public float life;

        public Particle(Vector2 position, Vector2 initialVelocity, Material material, float life)
            : base(position, new Vector2(0.1f, 0.1f), material, false)
        {
            this.hidden = true;
            this.life = life;
            this.body.CollidesWith = Category.None;
        }

        override public void Update(float dt)
        {
            base.Update(dt);

            if (life > 0.0f)
            {
                hidden = false;
                life -= dt;
            }
            else
            {
                hidden = true;
            }
        }
    }

    public class ParticleEmitter : Entity
    {
        public Vector2 position;
        public Particle[] particles;
        public Material particleMaterial;
        public bool active = false;
        private int currentParticleIndex = 0;
        private int numParticles;
        private float emitSpread, emitRate, emitCounter = 0.0f, initialVelocity, particleLifetime = 0.5f;
        private Random rnd = new Random();

        /// <summary>
        /// Creates a general entity for particle emission. Can be attached to other entities through SetParent.
        /// </summary>
        /// <param name="world">The Farseer Physics world into which the entity and it's sub-entities are to be placed</param>
        /// <param name="position">The starting position of the entity</param>
        /// <param name="numParticles">The maximum number of particles the ParticleEmitter may spawn</param>
        /// <param name="emitRate">The time between each particle being spawned, in seconds, when the emitter is active</param>
        /// <param name="emitSpread">A random value between -0.5 to 0.5 is multiplied with this and added to the angle at which each particle is spawned</param>
        /// <param name="emitDirection">The angle from which to spawn each particle (0 is right, pi is down)</param>
        /// <param name="particleTexture">The texture to use for this emitter's particles</param>
        public ParticleEmitter(Vector2 position, int numParticles, float emitRate, float emitSpread, float emitDirection, Material particleMaterial)
            : base(position, new Vector2(0.5f, 0.5f), particleMaterial, false)
        {
            this.position = position;
            this.emitRate = emitRate;
            this.emitSpread = emitSpread;
            this.body.Rotation = emitDirection;
            this.initialVelocity = 5.0f;
            this.particleMaterial = particleMaterial;
            this.numParticles = numParticles;
            this.body.CollidesWith = Category.None;
            this.hidden = true;

            particles = new Particle[numParticles];
            for (int i = 0; i < numParticles; i++)
            {
                particles[i] = new Particle(position, new Vector2(), particleMaterial, 1.0f);
            }
        }

        /*override public void SetParent(Entity parent)
        {
            base.SetParent(parent);

            this.body.IgnoreCollisionWith(parent.body); //Should perhaps always be done?
            for (int i = 0; i < numParticles; i++)
            {
                particles[i].body.IgnoreCollisionWith(parent.body);
            }
        }*/

        override public void Update(float dt)
        {
            base.Update(dt);

            emitCounter += dt;
            if (emitCounter >= emitRate && active)
            {
                emitCounter = 0.0f;

                SpawnParticle();
            }

            for (int i = 0; i < numParticles; i++)
            {
                particles[i].Update(dt);
            }
        }

        private void SpawnParticle()
        {
            double spreadValue = (rnd.NextDouble() - 0.5) * emitSpread;

            particles[currentParticleIndex].body.Position = this.body.Position;
            particles[currentParticleIndex].body.LinearVelocity =
                    new Vector2(
                        (float)Math.Cos(this.body.Rotation + spreadValue),
                        (float)Math.Sin(this.body.Rotation + spreadValue)) * initialVelocity
                        + this.body.LinearVelocity;
            particles[currentParticleIndex].life = particleLifetime;
            currentParticleIndex = (currentParticleIndex + 1) % numParticles;
        }
    }
}

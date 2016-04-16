using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SpacePlatformer.Engine.Entities;
using GameStateManagement;
using SpacePlatformer.Engine.Rendering;

namespace SpacePlatformer.Engine
{
    class Mapp : DrawableGameComponent
    {
        private World world;
        private List<Entityy> entities;
        public Player player;
        private SpriteFont hudFont;

        public Mapp(Game game)
            : base(game)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            Renderer.Initialize(graphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            player.body.ApplyForce(OrbitalPhysics.GetNewtonForceVector(entities[0], player));
            entities[1].body.ApplyForce(OrbitalPhysics.GetNewtonForceVector(entities[0], entities[1]));

            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            Renderer.HandleCamera(player.body.Position, entities[0].body.Position);
        }

        protected override void Draw(GameTime gameTime)
        {
        }
    }
}

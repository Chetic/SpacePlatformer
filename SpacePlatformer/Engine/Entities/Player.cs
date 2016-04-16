
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using System;
using SpacePlatformer.Engine.Rendering;
using System.Xml;

namespace SpacePlatformer.Engine.Entities
{
    public class Player : Entity
    {
        EntityStateController stateController;

        public Player()
            : base()
        {
            //body.FixedRotation = true;
            stateController = new EntityStateController(EntityState.Idle);
        }

        public Player(Vector2 position, Material material)
            : base(position, new Vector2(32.0f / 64.0f), material, false)
        {
            body.FixedRotation = true;
            stateController = new EntityStateController(EntityState.Idle);
        }

        public void Move(Direction direction)
        {
            if (direction == Direction.Right)
                body.ApplyLinearImpulse(new Vector2(0.025f, 0.0f));
            if (direction == Direction.Left)
                body.ApplyLinearImpulse(new Vector2(-0.025f, 0.0f));

            //Avoids air-walking
            if (!(stateController.activeState == EntityState.Jumping))
            {
                stateController.ChangeState(EntityState.Walking);
            }
        }

        public void JumpOn()
        {
            if (!(stateController.activeState == EntityState.Jumping))
            {
                //Allows jumping as long as in contact with something
                if (body.ContactList != null)
                {
                    body.ApplyLinearImpulse(new Vector2(0.0f, -1.0f));
                    stateController.ChangeState(EntityState.Jumping);
                }
            }
        }

        public void JumpOff()
        {
            if (stateController.activeState == EntityState.Jumping)
            {
                stateController.ChangeState(EntityState.Idle);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}

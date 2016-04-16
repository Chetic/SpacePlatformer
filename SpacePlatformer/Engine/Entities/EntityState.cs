using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpacePlatformer.Engine.Entities
{
    public enum EntityState
    {
        Idle,
        Walking,
        Jumping
    }

    class EntityStateController
    {
        public EntityState activeState { get; private set; }

        public EntityStateController(EntityState initialState)
        {
            this.activeState = initialState;
        }

        public void ChangeState(EntityState nextState)
        {
            //TODO: State transitions
            this.activeState = nextState;
        }
    }
}

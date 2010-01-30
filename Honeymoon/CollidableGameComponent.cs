using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class CollidableGameComponent : DrawableGameComponent
    {
        public HoneymoonGame GameHM;

        public CollidableGameComponent(PlayerIndex PlayerNumber)
            : base(HoneymoonGame.Instance)
        {
            this.GameHM = HoneymoonGame.Instance;
            this.PlayerNumber = PlayerNumber;
        }

        public PlayerIndex PlayerNumber;
        private bool collisionEnabled;
        public Vector2 Position;
        public float CollisionRadius;
        public float CollisionRadiusSq { get { return CollisionRadius * CollisionRadius; } }

        public bool CollisionEnabled
        {
            get { return collisionEnabled; }
            set
            {
                if (collisionEnabled != value)
                {
                    collisionEnabled = value;
                    if (collisionEnabled)
                        GameHM.collidableObjects.Add(this);
                    else
                        GameHM.collidableObjects.Remove(this);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (collisionEnabled)
                GameHM.collidableObjects.Remove(this);
            GameHM.Components.Remove(this);
            base.Dispose(disposing);
        }

        public virtual void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
        }
    }
}

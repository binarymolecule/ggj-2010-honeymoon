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

        public CollidableGameComponent()
            : base(HoneymoonGame.Instance)
        {
            GameHM = HoneymoonGame.Instance;
        }

        public bool CollisionEnabled;
        public Vector2 Position;
        public float CollisionRadius;
        public float CollisionRadiusSq { get { return CollisionRadius * CollisionRadius; } }

        public virtual void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class CollidableGameComponent : DrawableGameComponent
    {
        public CollidableGameComponent(Game game)
            : base(game)
        {
        }

        public bool CollisionEnabled;
        public Vector2 Position;
        public float Radius;
        public float RadiusSq { get { return Radius * Radius; } }

        public virtual void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
        }
    }
}

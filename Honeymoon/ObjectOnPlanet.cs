using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class ObjectOnPlanet : CollidableGameComponent
    {
        public Planet planet;
        public Vector2 PositionOnPlanet = new Vector2(-(float)Math.PI / 2.0f, 0);

        public ObjectOnPlanet(Planet planet)
            : base(planet.PlayerNumber)
        {
            this.planet = planet;
        }
    }
}

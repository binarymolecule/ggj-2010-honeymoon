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
        public Vector2 PositionOnPlanet;

        public ObjectOnPlanet(Planet planet)
            : base(planet.PlayerNumber)
        {
            this.planet = planet;
        }
    }
}

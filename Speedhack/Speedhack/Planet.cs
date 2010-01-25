using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;

namespace Speedhack
{
    public class Planet
    {
        public Geom geometry;
        public Body body;
        public Texture2D texture;

        public Planet(PhysicsSimulator sim, Texture2D tex, Vector2 pos, float size, float mass)
        {
            body = BodyFactory.Instance.CreateCircleBody(sim, size, mass);
            geometry = GeomFactory.Instance.CreateCircleGeom(body, size, 64);
            body.Position = pos;
            geometry.RestitutionCoefficient = 0.2f;
            geometry.FrictionCoefficient = 0.9f;
            texture = tex;
        }

        public void Update(float ms)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, body.Position, null, Color.White, body.Rotation,
                             new Vector2(texture.Width / 2, texture.Height / 2), 1,
                             SpriteEffects.None, 0);
        }
    }
}
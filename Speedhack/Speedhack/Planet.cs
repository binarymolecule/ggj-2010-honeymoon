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
        public PhysicsSimulator physicsSimulator;
        public Geom geometry;
        public Body body;
        public Texture2D texture;
        public float torque;
        public bool hasGravity;

        public Planet(PhysicsSimulator sim, Texture2D tex, Vector2 pos, float size, float mass, float tor, bool grav)
        {
            physicsSimulator = sim;
            body = BodyFactory.Instance.CreateCircleBody(physicsSimulator, size, mass);
            geometry = GeomFactory.Instance.CreateCircleGeom(body, size, 64);
            body.Position = pos;
            geometry.RestitutionCoefficient = 0.0f;
            geometry.FrictionCoefficient = 0.5f;
            texture = tex;
            torque = tor;
            hasGravity = grav;
        }

        public void Update(float ms)
        {
            body.ApplyTorque(torque * ms);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, body.Position, null, Color.White, body.Rotation,
                             new Vector2(texture.Width / 2, texture.Height / 2), 1,
                             SpriteEffects.None, 0);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class CoconutOrbit : CollidableGameComponent
    {
        public Planet planet;
        public Tree tree;
        public float height;
        public static float CoconutOrbitHeight = 128.0f;
        public static float CoconutOrbitVelocity = 1.0f;

        public Vector2 PositionOnPlanet;

        public CoconutOrbit(Planet planet, Tree tree, float angle, float height)
            : base(planet.PlayerNumber)
        {
            this.planet = planet;
            this.tree = tree;
            this.height = height; //CoconutOrbitHeight;
            this.DrawOrder = 3;
            this.CollisionEnabled = true;
            this.PositionOnPlanet = new Vector2(angle, height);
            this.Position = planet.GetPositionInPlanetOrbit(angle, height);
            this.CollisionRadius = 16.0f;
            this.tree.CoconutCount++;
        }

        protected override void Dispose(bool disposing)
        {
            this.tree.CoconutCount--;
            base.Dispose(disposing);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PositionOnPlanet.X += CoconutOrbitVelocity * seconds;
            Position = planet.GetPositionInPlanetOrbit(PositionOnPlanet.X, height);
        }

        public override void Draw(GameTime gameTime)
        {
/*
            // Draw trail
            Vector2 tmpPos = planet.GetPositionInPlanetOrbit(PositionOnPlanet.X - 0.08f, height);
            GameHM.CurrentTheme.Coconut.Draw(this, gameTime, "coconut", tmpPos, new Color(Color.White, 0.3f), planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 0.8f);
            tmpPos = planet.GetPositionInPlanetOrbit(PositionOnPlanet.X - 0.04f, height);
            GameHM.CurrentTheme.Coconut.Draw(this, gameTime, "coconut", tmpPos, new Color(Color.White, 0.5f), planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 0.9f);
*/
            GameHM.CurrentTheme.Coconut.Draw(this, gameTime, "coconut", Position, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 1.0f);
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is Monkey)
            {
                GameHM.CurrentTheme.SoundMissile.Play();

                // Coconut is converted into a missile when hit
                Vector2 dir = -1.0f * offsetMeToOther;
                dir.Normalize();
                CoconutMissile coconut = new CoconutMissile(Position, dir, ((Monkey)otherObject).PlayerNumber);
                GameHM.Components.Add(coconut);
                this.Dispose();
            }
        }
    }
}

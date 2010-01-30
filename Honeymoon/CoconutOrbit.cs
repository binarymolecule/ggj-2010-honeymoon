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
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Planet planet;
        public float height;
        public static float CoconutOrbitHeight = 128.0f;
        public static float CoconutOrbitVelocity = 1.0f;

        public Vector2 PositionOnPlanet;

        public CoconutOrbit(Planet planet, float height)
        {
            this.planet = planet;
            this.height = height; //CoconutOrbitHeight;
            this.DrawOrder = 3;
            this.CollisionEnabled = true;
            this.CollisionRadius = 14;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("coconut");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PositionOnPlanet.X += CoconutOrbitVelocity * seconds;
            Position = planet.GetPositionOnPlanet(PositionOnPlanet.X, height);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            System.Console.Out.WriteLine("Coconut collides with other object!");
            if (otherObject is Monkey)
            {
                Vector2 dir = offsetMeToOther;
                dir.Normalize();
                CoconutMissile coconut = new CoconutMissile(Position, dir);
                GameHM.Components.Add(coconut);
                this.Dispose();
            }
        }
    }
}

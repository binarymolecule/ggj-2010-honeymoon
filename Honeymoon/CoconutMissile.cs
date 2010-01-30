using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class CoconutMissile : CollidableGameComponent
    {
        public Vector2 Velocity;
        public float Angle;
        public float fadeOutDuration;
        public bool fadingOut;
        public static float CoconutMissileVelocity = 250.0f;
        public static float CoconutMissileTorque = 10.0f;
        public static float CoconutMissileFadeoutDuration = 1.0f;
        public static float CoconutMissileBounceVelocity = 75.0f;

        public CoconutMissile(Vector2 pos, Vector2 dir, PlayerIndex PlayerNumber)
            : base(PlayerNumber)
        {
            this.Position = pos;
            this.Velocity = CoconutMissileVelocity * dir;
            this.Angle = (float)Math.Atan2(dir.Y, dir.X);
            this.CollisionEnabled = true;
            this.fadingOut = false;
            this.CollisionRadius = 16.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += seconds * Velocity;
            Angle += seconds * CoconutMissileTorque;

            if (fadingOut)
            {
                fadeOutDuration -= seconds;
                if (fadeOutDuration <= 0.0f)
                {
                    this.Dispose();
                }
            }
            else
            {
                // Check if coconut is outside display
                Vector2 windowSize = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
                float offset = CollisionRadius * 2.0f;
                if ((Position.X < -offset) || (Position.X > windowSize.X + offset) ||
                    (Position.Y < -offset) || (Position.Y > windowSize.Y + offset))
                {
                    this.Dispose();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Color color = (fadingOut ? new Color(Color.White, (float)Math.Max(0.0f, fadeOutDuration / CoconutMissileFadeoutDuration)) : Color.White);
            float scale = (fadingOut ? (float)Math.Max(0.0f, fadeOutDuration / CoconutMissileFadeoutDuration) : 1.0f);
            GameHM.CurrentTheme.Coconut.Draw(this, gameTime, "coconut", Position, color, Angle, scale);
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            // Test if missile collides with an object to explode
            bool collides = false;
            if (otherObject is Planet)
                collides = true;
            else if (otherObject is Monkey)
                collides = !((Monkey)otherObject).PlayerNumber.Equals(this.PlayerNumber);
            else if (otherObject is CoconutMissile)
                collides = !((CoconutMissile)otherObject).PlayerNumber.Equals(this.PlayerNumber);
            else if (otherObject is CoconutOrbit)
                collides = !((CoconutOrbit)otherObject).PlayerNumber.Equals(this.PlayerNumber);

            if (collides)
            {
                CollisionEnabled = false; // remove from game's collidable object list

                // Create explosion 
                Vector2 exploPos = (otherObject is Monkey) ? otherObject.Position : Position;
                new CoconutExplosion(exploPos, PlayerNumber, true);

                // Bounce from planet surface
                if (otherObject is Planet)
                {
                    Vector2 dir = -1.0f * offsetMeToOther;

                    // rotate by small random amount
                    float dx = dir.X, dy = dir.Y;
                    float randomAngle = (float)(GameHM.Randomizer.NextDouble() * 2.0 - 1.0);
                    float sin = (float)Math.Sin(randomAngle), cos = (float)Math.Cos(randomAngle);
                    dir.X = cos * dx + sin * dy;
                    dir.Y = cos * dy - sin * dx;

                    dir.Normalize();
                    Velocity = CoconutMissileBounceVelocity * dir;
                    this.fadeOutDuration = CoconutMissileFadeoutDuration;
                    this.fadingOut = true;
                }
                else
                {
                    this.Dispose();
                }

                // Destroy other object when colliding with a coconut
                if (otherObject is CoconutMissile || otherObject is CoconutOrbit)
                {
                    otherObject.Dispose();
                }
            }
        }
    }
}

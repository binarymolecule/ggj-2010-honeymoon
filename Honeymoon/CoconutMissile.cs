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
        public PlayerIndex PlayerNumber;
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Vector2 Velocity;
        public float Angle;
        public float fadeOutDuration;
        public bool fadingOut;
        public static float CoconutMissileVelocity = 250.0f;
        public static float CoconutMissileTorque = 10.0f;
        public static float CoconutMissileFadeoutDuration = 1.0f;
        public static float CoconutMissileBounceVelocity = 75.0f;

        public CoconutMissile(Vector2 pos, Vector2 dir, PlayerIndex PlayerNumber)
        {
            this.PlayerNumber = PlayerNumber;
            this.Position = pos;
            this.Velocity = CoconutMissileVelocity * dir;
            this.Angle = (float)Math.Atan2(dir.Y, dir.X);
            this.CollisionEnabled = true;
            this.CollisionRadius = 14;
            this.fadingOut = false;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("coconut");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
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
                float offset = Math.Max(Sprite.Width, Sprite.Height);
                if ((Position.X < -offset) || (Position.X > windowSize.X + offset) ||
                    (Position.Y < -offset) || (Position.Y > windowSize.Y + offset))
                {
                    this.Dispose();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, Angle, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is Planet)
            {
                // Bounce from planet surface
                Vector2 dir = -1.0f * offsetMeToOther;
                dir.Normalize();
                Velocity = CoconutMissileBounceVelocity * dir;
                this.fadeOutDuration = CoconutMissileFadeoutDuration;
                this.fadingOut = true;
                CollisionEnabled = false; // remove from game's collidable object list

                // Create explosion
                CoconutExplosion explosion = new CoconutExplosion(Position);
                GameHM.Components.Add(explosion);
            }
        }
    }
}

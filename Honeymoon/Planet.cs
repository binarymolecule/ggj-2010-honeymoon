using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class Planet : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public float Rotation;
        public Vector2 Velocity;
        public static float BounceFactor = 0.9f;
        public static float Friction = 0.1f;

        public Planet()
        {
            GameHM.collidableObjects.Add(this);
            Radius = 64.0f;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("planet");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float  seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * seconds;
            Rotation += 1.0f * seconds;
            Velocity *= (float)Math.Pow(1.0f - Friction, seconds);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            offsetMeToOther.Normalize();
            float dot = Vector2.Dot(Velocity, offsetMeToOther);
            if (dot < 0) return;
            Velocity -= offsetMeToOther* dot * (1.0f + BounceFactor);
        }
    }
}

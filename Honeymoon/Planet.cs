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
        public float RotationSpeed;
        public Vector2 Velocity;
        public static float BounceFactor = 0.9f;
        public static float Friction = 0.5f;
        public static float RotationFriction = 0.1f;
        public static float PlanetRadius = 64.0f;
        public static float RotationSpeedScaleOnCollide = 0.02f;

        public Planet()
        {
            CollisionEnabled = true;
            GameHM.collidableObjects.Add(this);
            CollisionRadius = 64.0f;
            this.DrawOrder = 2;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("planet");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += Velocity * seconds;
            Rotation += RotationSpeed  * seconds;
            Velocity *= (float)Math.Pow(1.0f - Friction, seconds);
            RotationSpeed *= (float)Math.Pow(1.0f - RotationFriction, seconds);

            Vector2 windowSize = new Vector2(800,600);
            if ((Position.X < CollisionRadius && Velocity.X < 0) || (Position.X > windowSize.X - CollisionRadius && Velocity.X > 0)) Velocity.X *= -BounceFactor;
            if ((Position.Y < CollisionRadius && Velocity.Y < 0) || (Position.Y > windowSize.Y - CollisionRadius && Velocity.Y > 0)) Velocity.Y *= -BounceFactor;
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
            Vector2 forceTowardsOther = offsetMeToOther * dot;
            Vector2 oldVelocity = Velocity;

            if (otherObject is Planet)
            {
                Planet otherPlanet = otherObject as Planet;
                Velocity -= forceTowardsOther * 1.5f;
                otherPlanet.Velocity += forceTowardsOther * 0.5f;
            }
            else
            {
                Velocity -= forceTowardsOther * (1.0f + BounceFactor);
            }

            oldVelocity.Normalize();
            float speedAfter = Vector2.Dot(Velocity, oldVelocity);
            if (speedAfter > 0)
            {
                Vector2 rightSide = new Vector2(-Velocity.Y, Velocity.X);
                float sign = Math.Sign(Vector2.Dot(offsetMeToOther, rightSide));
                RotationSpeed += speedAfter * sign * RotationSpeedScaleOnCollide;
                if (otherObject is Planet)
                    (otherObject as Planet).RotationSpeed -= speedAfter * sign * RotationSpeedScaleOnCollide;
          }
        }

        public Vector2 GetPositionOnPlanet(float RotationRelativeToPlanet, float HeightAbovePlanetGround)
        {
            float rot = Rotation + RotationRelativeToPlanet;
            float abs = PlanetRadius + HeightAbovePlanetGround;
            return Position + new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * abs;
        }

    }
}

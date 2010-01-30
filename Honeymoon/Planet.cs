﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public static float Friction = 0.9f;
        public static float RotationFriction = 0.5f;
        public static float PlanetRadius = 64.0f;
        public static float RotationSpeedScaleOnCollide = 0.01f;

        public Planet()
        {
            CollisionEnabled = true;
            CollisionRadius = 64.0f;
            this.DrawOrder = 2;
            Game.Components.Add(this);
            new Tree(this);
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
            Rotation += RotationSpeed * seconds;
            Velocity *= (float)Math.Pow(1.0f - Friction, seconds);
            RotationSpeed *= (float)Math.Pow(1.0f - RotationFriction, seconds);
      
            Vector2 windowSize = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            ScreenWall wall = new ScreenWall();
            if (Position.X < CollisionRadius && Velocity.X < 0) OnCollide(wall, -Vector2.UnitX);
            if (Position.X > windowSize.X - CollisionRadius && Velocity.X > 0) OnCollide(wall, Vector2.UnitX);
            if (Position.Y < CollisionRadius && Velocity.Y < 0) OnCollide(wall, -Vector2.UnitY);
            if (Position.Y > windowSize.Y - CollisionRadius && Velocity.Y > 0) OnCollide(wall, Vector2.UnitY);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, Rotation, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (!(otherObject is Planet) && !(otherObject is ScreenWall)) return;

            offsetMeToOther.Normalize();
            float dot = Vector2.Dot(Velocity, offsetMeToOther);
            if (dot < 0) return;
            Vector2 forceTowardsOther = offsetMeToOther * dot;

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

            Vector2 wallPerpendicular = new Vector2(offsetMeToOther.Y, -offsetMeToOther.X);
            float speedAfter = Vector2.Dot(Velocity, wallPerpendicular);
            RotationSpeed += speedAfter * RotationSpeedScaleOnCollide;
            if (otherObject is Planet)
                (otherObject as Planet).RotationSpeed += speedAfter * RotationSpeedScaleOnCollide;

        }

        public Vector2 GetPositionOnPlanetGround(float RotationRelativeToPlanet, float HeightAbovePlanetGround)
        {
            float rot = Rotation + RotationRelativeToPlanet;
            float abs = PlanetRadius + HeightAbovePlanetGround;
            return Position + new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot)) * abs;
        }

        public Vector2 GetPositionInPlanetOrbit(float Rotation, float HeightAbovePlanetGround)
        {
            float abs = PlanetRadius + HeightAbovePlanetGround;
            return Position + new Vector2((float)Math.Cos(Rotation), (float)Math.Sin(Rotation)) * abs;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Honeymoon.Screens;

namespace Honeymoon
{
    public class Planet : CollidableGameComponent
    {
        public float Rotation;
        public float RotationSpeed;
        public Vector2 Velocity;

        public static float BounceFactor = 0.9f;
        public static float Friction = 0.9f;
        public static float RotationFriction = 0.5f;
        public static float PlanetRadius = 64.0f;
        public static float RotationSpeedScaleOnCollide = 0.01f;

        public TimeSpan AttackMoveUntil = TimeSpan.Zero;
        public bool IsAttackMove;

        public Planet(PlayerIndex PlayerNumber)
            : base(PlayerNumber)
        {
            this.CollisionEnabled = true;
            CollisionRadius = 64.0f;
            this.DrawOrder = 2;
            VersusScreen.Instance.Components.Add(this);
            GameHM.Themes[0].Planet.JumpTo(this, "planet", (float)random.NextDouble());
            GameHM.Themes[1].Planet.JumpTo(this, "planet", (float)random.NextDouble());

            new Tree(this);
        }

        private Random random = new Random();
            
        

        public override void Update(GameTime gameTime)
        {
            IsAttackMove = gameTime.TotalGameTime < AttackMoveUntil;

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
            GameHM.CurrentTheme.Planet.Draw(this, gameTime, "planet", Position, Color.White, Rotation, 1.0f);
            
            GameHM.spriteBatch.End();
            GameHM.spriteBatchAdditiveStart();
            GameHM.CurrentTheme.Planet.Draw(Velocity, gameTime, "highlightandshadow", Position, Color.White, 0, 1);
            GameHM.spriteBatch.End();
            GameHM.spriteBatchStart(); 
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (!(otherObject is Planet) && !(otherObject is ScreenWall) && !(otherObject is Monkey)) return;
            if (otherObject is Monkey && (otherObject as Monkey).PlayerNumber == PlayerNumber) return;

            offsetMeToOther.Normalize();
            float dot = Vector2.Dot(Velocity, offsetMeToOther);
            if (dot < 0) return;
            Vector2 forceTowardsOther = offsetMeToOther * dot;

            if (otherObject is Planet)
            {
                Planet otherPlanet = otherObject as Planet;
                Velocity -= forceTowardsOther * 1.5f;
                otherPlanet.Velocity += forceTowardsOther * 0.5f;
                GameHM.CurrentTheme.SoundCollide.Play();
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

        public Vector2 rot2vec(float rot)
        {
            return new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
        }

        public Vector2 GetPositionOnPlanetGround(float RotationRelativeToPlanet, float HeightAbovePlanetGround)
        {
            float rot = Rotation + RotationRelativeToPlanet;
            float abs = PlanetRadius + HeightAbovePlanetGround;
            return Position + rot2vec(rot) * abs;
        }

        public Vector2 GetPositionInPlanetOrbit(float Rotation, float HeightAbovePlanetGround)
        {
            float abs = PlanetRadius + HeightAbovePlanetGround;
            return Position + rot2vec(Rotation) * abs;
        }

        public Color GetShadingForPlanetGround(float RotationRelativeToPlanet)
        {
            float b = Vector2.Dot(GameHM.SunlightDir, rot2vec(Rotation + RotationRelativeToPlanet));
            b = Math.Max(0.5f, b);
            return new Color(b, b, b, 1);
        }
        public Color GetShadingForPlanetOrbit(float Rotation)
        {
            float b = Vector2.Dot(GameHM.SunlightDir, rot2vec(Rotation));
            b = Math.Max(0.5f, b);
            return new Color(b, b, b, 1);
        }
    }
}

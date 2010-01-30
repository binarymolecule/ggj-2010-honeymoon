using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class Monkey : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Planet planet;

        public static float MonkeyWalkingHeight = 28.0f;
        public static float GravityStrength = 1000.0f;
        public static float BounceFactor = 0.2f;
        public static float Friction = 0.95f;
        public static float MaxHeightForJump = 5.0f;
        public static float MinHeightForCrashJump = 20.0f;
        public static float RunStrength = 0.2f;
        public static float JumpStrength = 300.0f;
        public static float CrashJumpDownspeed = 300.0f;
        public static float CrashJumpPlanetSpeed = 2.0f;
        public static TimeSpan CrashJumpPenalty = TimeSpan.FromSeconds(0.5);

        public Vector2 PositionOnPlanet;
        public Vector2 VelocityOnPlanet;
        public bool DoingCrashJump;
        public TimeSpan CrashJumpPenaltyUntil = TimeSpan.Zero;


        public Monkey(Planet planet)
        {
            this.planet = planet;
            this.DrawOrder = 1;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("monkey");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameTime.TotalGameTime > CrashJumpPenaltyUntil)
            {
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
                VelocityOnPlanet.X += gamePadState.ThumbSticks.Left.X * RunStrength;
                if (gamePadState.IsButtonDown(Buttons.A) && PositionOnPlanet.Y < MaxHeightForJump) VelocityOnPlanet.Y = JumpStrength;
                if (gamePadState.IsButtonDown(Buttons.RightTrigger) && (PositionOnPlanet.Y > MinHeightForCrashJump || VelocityOnPlanet.Y < 0)) DoingCrashJump = true;
            }

            VelocityOnPlanet.Y -= GravityStrength * seconds;
            VelocityOnPlanet *= (float)Math.Pow(1.0f - Friction, seconds);
            if (DoingCrashJump) VelocityOnPlanet.Y = -CrashJumpDownspeed;

            PositionOnPlanet += VelocityOnPlanet * seconds;
            if (PositionOnPlanet.Y < 0)
            {
                PositionOnPlanet.Y = 0;
                VelocityOnPlanet.Y *= -BounceFactor;
                if (DoingCrashJump)
                {
                    planet.Velocity -= (planet.GetPositionOnPlanet(PositionOnPlanet.X, 0.0f) - planet.Position) * CrashJumpPlanetSpeed;
                    DoingCrashJump = false;
                    CrashJumpPenaltyUntil = gameTime.TotalGameTime.Add(CrashJumpPenalty);
                }
            }

            Position = planet.GetPositionOnPlanet(PositionOnPlanet.X, MonkeyWalkingHeight + PositionOnPlanet.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }



    }
}

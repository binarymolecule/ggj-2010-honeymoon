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
        public Planet planet;

        public static float MonkeyWalkingHeight = 28.0f;
        public static float GravityStrength = 1000.0f;
        public static float BounceFactor = 0.2f;
        public static float Friction = 0.9f;
        public static float MaxHeightForJump = 5.0f;
        public static float MinHeightForCrashJump = 30.0f;
        public static float RunStrength = 0.2f;
        public static float RunStrengthPlanet = 0.1f;
        public static float JumpStrength = 300.0f;
        public static float CrashJumpDownspeed = 300.0f;
        public static float CrashJumpPlanetSpeed = 5.0f;
        public static TimeSpan CrashJumpPenalty = TimeSpan.FromSeconds(0.5);

        public Vector2 PositionOnPlanet;
        public Vector2 VelocityOnPlanet;
        public bool DoingCrashJump;
        public TimeSpan CrashJumpPenaltyUntil = TimeSpan.Zero;

        public HelpSystem HelpMovement;

        public int HitPoints;
        public static int MaxHitPoints = 5;

        public Monkey(Planet planet, PlayerIndex PlayerNumber)
            : base(PlayerNumber)
        {
            this.HitPoints = MaxHitPoints;
            this.planet = planet;
            this.DrawOrder = 1;
            HelpMovement = new HelpSystem(this, "move");
            HelpMovement.DisplayHelp = true;
            this.CollisionEnabled = true;
            this.CollisionRadius = 30;
            Game.Components.Add(this);
        }

        String CurrentAnimation;

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameTime.TotalGameTime > CrashJumpPenaltyUntil)
            {
                GamePadState gamePadState = GamePad.GetState(PlayerNumber);

#if(DEBUG)
                if (gamePadState.IsButtonDown(Buttons.LeftTrigger))
                {
                    if (gamePadState.IsButtonDown(Buttons.DPadLeft)) planet.Rotation -= 0.1f;
                    else if (gamePadState.IsButtonDown(Buttons.DPadRight)) planet.Rotation += 0.1f;
                }
                else
                {
                    if (gamePadState.IsButtonDown(Buttons.DPadLeft)) planet.Position.X -= 10.0f;
                    else if (gamePadState.IsButtonDown(Buttons.DPadRight)) planet.Position.X += 10.0f;
                    if (gamePadState.IsButtonDown(Buttons.DPadUp)) planet.Position.Y -= 10.0f;
                    else if (gamePadState.IsButtonDown(Buttons.DPadDown)) planet.Position.Y += 10.0f;
                }
#endif


                bool standingOnTheGround = PositionOnPlanet.Y < MaxHeightForJump;
                if (gamePadState.IsButtonDown(Buttons.A) && standingOnTheGround) VelocityOnPlanet.Y = JumpStrength;
                if (gamePadState.IsButtonDown(Buttons.RightTrigger) && (PositionOnPlanet.Y > MinHeightForCrashJump || VelocityOnPlanet.Y < 0)) DoingCrashJump = true;
                if (!gamePadState.IsButtonDown(Buttons.LeftTrigger) && standingOnTheGround)
                {
                    planet.RotationSpeed += gamePadState.ThumbSticks.Left.X * RunStrengthPlanet;
                    VelocityOnPlanet.X += gamePadState.ThumbSticks.Left.X * RunStrengthPlanet;
                }
                else VelocityOnPlanet.X += gamePadState.ThumbSticks.Left.X * RunStrength;

                if (DoingCrashJump) CurrentAnimation = "crash";
                else CurrentAnimation = VelocityOnPlanet.X > 0 ? "right" : "left";
            }
            else CurrentAnimation = "penalty";


            if (VelocityOnPlanet.LengthSquared() > 11.0f)
                HelpMovement.DisplayHelp = false;

            VelocityOnPlanet.Y -= GravityStrength * seconds;
            VelocityOnPlanet *= (float)Math.Pow(1.0f - Friction, seconds);
            if (DoingCrashJump)
            {
                VelocityOnPlanet.X = 0;
                VelocityOnPlanet.Y = -CrashJumpDownspeed;
            }

            PositionOnPlanet += VelocityOnPlanet * seconds;
            if (PositionOnPlanet.Y < 0)
            {
                PositionOnPlanet.Y = 0;
                VelocityOnPlanet.Y *= -BounceFactor;
                if (DoingCrashJump)
                {
                    planet.Velocity -= (planet.GetPositionOnPlanetGround(PositionOnPlanet.X, 0.0f) - planet.Position) * CrashJumpPlanetSpeed;
                    DoingCrashJump = false;
                    CrashJumpPenaltyUntil = gameTime.TotalGameTime.Add(CrashJumpPenalty);
                }
            }

            Position = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, MonkeyWalkingHeight + PositionOnPlanet.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.CurrentTheme.Monkey.Draw(this, gameTime, CurrentAnimation, Position, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 1.0f);
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is CoconutOrbit)
            {
                // Do some animation stuff?
            }
            else if (otherObject is CoconutExplosion)
            {
                // Player is hurt
                HitPoints--;
                if (HitPoints <= 0)
                {
                    // End game
                }
            }
        }
    }
}

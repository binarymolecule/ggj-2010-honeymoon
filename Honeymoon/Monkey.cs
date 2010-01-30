using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class Monkey : ObjectOnPlanet
    {
        public static float MonkeyWalkingHeight = 28.0f;
        public static float GravityStrength = 1000.0f;
        public static float BounceFactor = 0.2f;
        public static float Friction = 0.9f;
        public static float FrictionAir = 0.99f;
        public static float MaxHeightForJump = 5.0f;
        public static float MinHeightForCrashJump = 30.0f;
        public static float RunStrength = 0.2f;
        public static float RunStrengthPlanet = 0.01f;
        public static float JumpStrength = 300.0f;
        public static float CrashJumpDownspeed = 300.0f;
        public static float CrashJumpPlanetSpeed = 5.0f;
        public static TimeSpan CrashJumpPenalty = TimeSpan.FromSeconds(0.5);
        public static float SineStrength = 0.2f;
        public static float SineResolution = 10.2f;

        public Vector2 VelocityOnPlanet;
        public bool DoingCrashJump;
        public TimeSpan CrashJumpPenaltyUntil = TimeSpan.Zero;

        public HelpSystem HelpMovement;

        public int HitPoints;
        public static int MaxHitPoints = 10;

        public Monkey(Planet planet)
            : base(planet)
        {
            this.HitPoints = MaxHitPoints;
            this.planet = planet;
            this.DrawOrder = 1;
            HelpMovement = new HelpSystem(this, "help_move");
            HelpMovement.DisplayHelp = true;
            this.CollisionEnabled = true;
            this.CollisionRadius = 30;
            Game.Components.Add(this);
        }

        String CurrentAnimation;

        int nextTheme;
        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool standingOnTheGround = PositionOnPlanet.Y < MaxHeightForJump;
            if (gameTime.TotalGameTime > CrashJumpPenaltyUntil)
            {
                GamePadState gamePadState = GamePad.GetState(PlayerNumber);

#if(DEBUG)
                // Some debugging controls...
                if (gamePadState.IsButtonDown(Buttons.Y) && !GameHM.Camera.IsShaking)
                {
                    GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime,
                                              DriftingCamera.CameraShakingFrequency,
                                              DriftingCamera.CameraShakingAmplitude);
                }
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


                if (gamePadState.IsButtonDown(Buttons.Start)) GameHM.CurrentTheme = GameHM.Themes[(nextTheme++) % 2];
#endif


                if (gamePadState.IsButtonDown(Buttons.A) && standingOnTheGround)
                    VelocityOnPlanet.Y = JumpStrength;
                if (!DoingCrashJump && gamePadState.IsButtonDown(Buttons.RightTrigger) && (PositionOnPlanet.Y > MinHeightForCrashJump || VelocityOnPlanet.Y < 0))
                {
                    DoingCrashJump = true;
                    if (!GameHM.Camera.IsShaking)
                    {
                        // rotate by small random amount
                        float angle = PositionOnPlanet.X + planet.Rotation;
                        GameHM.Camera.MoveCamera(planet.rot2vec(angle), DriftingCamera.CameraMotionVelocity);
                    }
                }
                if (!gamePadState.IsButtonDown(Buttons.LeftTrigger) && standingOnTheGround)
                {
                    planet.RotationSpeed -= gamePadState.ThumbSticks.Left.X * RunStrengthPlanet;
                    VelocityOnPlanet.X += gamePadState.ThumbSticks.Left.X * RunStrength;
                }
                else VelocityOnPlanet.X += gamePadState.ThumbSticks.Left.X * RunStrength;

                if (DoingCrashJump) CurrentAnimation = "crash";
                else CurrentAnimation = VelocityOnPlanet.X > 0 ? "right" : "left";
            }
            else CurrentAnimation = "penalty";


            if (VelocityOnPlanet.LengthSquared() > 11.0f)
                HelpMovement.DisplayHelp = false;

            VelocityOnPlanet.Y -= GravityStrength * seconds;
            VelocityOnPlanet *= (float)Math.Pow(1.0f - (standingOnTheGround ? Friction : FrictionAir), seconds);
            if (DoingCrashJump)
            {
                VelocityOnPlanet.X = 0;
                VelocityOnPlanet.Y = -CrashJumpDownspeed;
            }

            float sineMod = (float)Math.Sin(PositionOnPlanet.X * SineResolution) * SineStrength + 1.0f;
            PositionOnPlanet += VelocityOnPlanet * seconds * sineMod;
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
                IGotHit(offsetMeToOther);
            }
            else if (otherObject is Planet && (otherObject as Planet).PlayerNumber != PlayerNumber)
            {
                IGotHit(offsetMeToOther);
            }
        }

        private void IGotHit(Vector2 offsetMeToOther)
        {
            HelpSystem.GloballyEnabled = false;

            // Player is hurt
            HitPoints--;

            // Shake screen
            GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime,
                                      DriftingCamera.CameraShakingFrequency,
                                      DriftingCamera.CameraShakingAmplitude);

            if (offsetMeToOther.LengthSquared() > 5)
                new CoconutExplosion(Position, PlayerNumber, this);

            if (HitPoints == 0)
            {
                // End game
            }
        }
    }
}

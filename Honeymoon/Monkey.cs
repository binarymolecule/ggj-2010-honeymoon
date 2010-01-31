using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class Monkey : ObjectOnPlanet
    {
        public static float MonkeyWalkingHeight = 22.0f;
        public static float GravityStrength = 1000.0f;
        public static float BounceFactor = 0.2f;
        public static float Friction = 0.9f;
        public static float FrictionAir = 0.99f;
        public static float MaxHeightForJump = 5.0f;
        public static float MinHeightForCrashJump = 30.0f;
        public static float RunStrength = 0.2f;
        public static float RunStrengthPlanet = 0.01f;
        public static float JumpStrength = 350.0f;
        public static float CrashJumpDownspeed = 300.0f;
        public static float CrashJumpPlanetSpeed = 5.0f;
        public static TimeSpan CrashJumpPlanetAttack = TimeSpan.FromSeconds(0.5);
        public static TimeSpan CrashJumpPenalty = TimeSpan.FromSeconds(0.5);
        public static float SineStrength = 0.2f;
        public static float SineResolution = 10.2f;
        public static float AnimationFpsScale = 10.0f;

        public Vector2 VelocityOnPlanet;
        public bool DoingCrashJump;
        public TimeSpan CrashJumpPenaltyUntil = TimeSpan.Zero;
        public SoundEffectInstance WalkingSound;

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
            this.CurrentAnimation = "left";
            Game.Components.Add(this);
        }

        String CurrentAnimation;
        GamePadState currentGamePadState;
        GamePadState oldGamePadState;

        public override void Update(GameTime gameTime)
        {
            currentGamePadState = GamePad.GetState(PlayerNumber);
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool isWalking = false;
            bool standingOnTheGround = PositionOnPlanet.Y < MaxHeightForJump;
            if (gameTime.TotalGameTime > CrashJumpPenaltyUntil)
            {

#if(DEBUG)
                // Some debugging controls...
                if (currentGamePadState.IsButtonDown(Buttons.Y) && !GameHM.Camera.IsShaking)
                {
                    GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime,
                                              DriftingCamera.CameraShakingFrequency,
                                              DriftingCamera.CameraShakingAmplitude);
                }
                if (currentGamePadState.IsButtonDown(Buttons.LeftTrigger))
                {
                    if (currentGamePadState.IsButtonDown(Buttons.DPadLeft)) planet.Rotation -= 0.1f;
                    else if (currentGamePadState.IsButtonDown(Buttons.DPadRight)) planet.Rotation += 0.1f;
                }
                else
                {
                    if (currentGamePadState.IsButtonDown(Buttons.DPadLeft)) planet.Position.X -= 10.0f;
                    else if (currentGamePadState.IsButtonDown(Buttons.DPadRight)) planet.Position.X += 10.0f;
                    if (currentGamePadState.IsButtonDown(Buttons.DPadUp)) planet.Position.Y -= 10.0f;
                    else if (currentGamePadState.IsButtonDown(Buttons.DPadDown)) planet.Position.Y += 10.0f;
                }


                if (KeyJustPressed(Buttons.Start)) GameHM.CurrentTheme = GameHM.Themes[(GameHM.CurrentThemeID+1) % 2];
#endif


                if (currentGamePadState.IsButtonDown(Buttons.A) && standingOnTheGround)
                {
                    VelocityOnPlanet.Y = JumpStrength;
                    GameHM.CurrentTheme.SoundJump.Play();
                }
                if (!DoingCrashJump && currentGamePadState.IsButtonDown(Buttons.RightTrigger) && (PositionOnPlanet.Y > MinHeightForCrashJump || VelocityOnPlanet.Y < 0))
                {
                    DoingCrashJump = true;
                    GameHM.CurrentTheme.SoundStomp.Play();
                    if (!GameHM.Camera.IsShaking)
                    {
                        // rotate by small random amount
                        float angle = PositionOnPlanet.X + planet.Rotation;
                        GameHM.Camera.MoveCamera(planet.rot2vec(angle), DriftingCamera.CameraMotionVelocity);
                    }
                }
                if (!currentGamePadState.IsButtonDown(Buttons.LeftTrigger) && standingOnTheGround)
                {
                    planet.RotationSpeed -= currentGamePadState.ThumbSticks.Left.X * RunStrengthPlanet;
                    VelocityOnPlanet.X += currentGamePadState.ThumbSticks.Left.X * RunStrength;
                    isWalking = (currentGamePadState.ThumbSticks.Left.X != 0.0f);
                }
                else
                {
                    VelocityOnPlanet.X += currentGamePadState.ThumbSticks.Left.X * RunStrength;
                    isWalking = (currentGamePadState.ThumbSticks.Left.X != 0.0f);
                }

                if (DoingCrashJump) CurrentAnimation = "crash";
                else CurrentAnimation = VelocityOnPlanet.X > 0 ? "right" : "left";
            }
            else
            {
                CurrentAnimation = "penalty";
            }

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
                    planet.AttackMoveUntil = gameTime.TotalGameTime.Add(CrashJumpPlanetAttack);
                    DoingCrashJump = false;
                    CrashJumpPenaltyUntil = gameTime.TotalGameTime.Add(CrashJumpPenalty);
                }
            }

            if (isWalking)
            {
                if (WalkingSound == null)
                {
                    WalkingSound = GameHM.WalkingSound.CreateInstance();
                    WalkingSound.IsLooped = true;
                }
                else if (WalkingSound.State == SoundState.Stopped)
                {
                    WalkingSound.Play();
                }
            }
            else
            {
                if (WalkingSound != null && WalkingSound.State != SoundState.Stopped)
                {
                    WalkingSound.Stop();
                }
            }

            Position = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, MonkeyWalkingHeight + PositionOnPlanet.Y);

            oldGamePadState = currentGamePadState;
        }

        private bool KeyJustPressed(Buttons button)
        {
            if (oldGamePadState == null || currentGamePadState == null)
            {
                return false;
            }

            bool wasDown = oldGamePadState.IsButtonDown(button);
            bool isDown = currentGamePadState.IsButtonDown(button);

            return !wasDown && isDown;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteAnimationSwitcher sprite = PlayerNumber != PlayerIndex.One ? GameHM.CurrentTheme.MonkeyM : GameHM.CurrentTheme.MonkeyF;
            if (CurrentAnimation == "left" || CurrentAnimation == "right")
                sprite.Animations[CurrentAnimation].AnimationFPS = AnimationFpsScale * Math.Abs(VelocityOnPlanet.X);
            sprite.Draw(this, gameTime, CurrentAnimation, Position, planet.GetShadingForPlanetGround(PositionOnPlanet.X), planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 1.0f);
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is CoconutOrbit)
            {
                // Do some animation stuff?
                VelocityOnPlanet.Y = 0;
            }
            else if (otherObject is CoconutExplosion)
            {
                IGotHit(offsetMeToOther);
            }
            else if (otherObject is Planet)
            {
                Planet otherPlanet = otherObject as Planet;
                if (otherPlanet.PlayerNumber != PlayerNumber && otherPlanet.IsAttackMove)
                {
                    otherPlanet.AttackMoveUntil = TimeSpan.Zero;
                    otherPlanet.IsAttackMove = false;
                    IGotHit(Vector2.Zero);
                    new CoconutExplosion(Position, PlayerNumber, null, false);
                }
            }
        }

        private void IGotHit(Vector2 offsetMeToOther)
        {
            HelpSystem.GloballyEnabled = false;

            // Player is hurt
            HitPoints--;

            // Shake screen (only in evil mode)
            if (GameHM.CurrentThemeID == 1)
            {
                GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime,
                                          DriftingCamera.CameraShakingFrequency,
                                          DriftingCamera.CameraShakingAmplitude);
            }

            if (offsetMeToOther.LengthSquared() > 5)
                new CoconutExplosion(Position, PlayerNumber, this, false);

            if (HitPoints == 0)
            {
                GameHM.GameOver();
            }
        }
    }
}

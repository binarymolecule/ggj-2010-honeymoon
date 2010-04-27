using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Honeymoon.Screens;
using KiloWatt.Runtime.Support;

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
        public static float MinHeightForCrashJump = 10.0f;
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

        String CurrentAnimation;

        GamePadState currentGamePadState, oldGamePadState;
        KeyboardState currentKeyboardState, oldKeyboardState;

        // Definition of input keys for both players in keyboard mode
        public static Keys LeftKey(PlayerIndex i) { return (i == PlayerIndex.One) ? Keys.A : Keys.Left; }
        public static Keys RightKey(PlayerIndex i) { return (i == PlayerIndex.One) ? Keys.D : Keys.Right; }
        public static Keys JumpKey(PlayerIndex i) { return (i == PlayerIndex.One) ? Keys.W : Keys.Up; }
        public static Keys StompKey(PlayerIndex i) { return (i == PlayerIndex.One) ? Keys.S : Keys.Down; }
        public static Keys StealthKey(PlayerIndex i) { return (i == PlayerIndex.One) ? Keys.LeftControl : Keys.RightControl; }

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
            VersusScreen.Instance.Components.Add(this);
        }

        static Profile timeUpdate = Profile.Get("Monkey.Update");

        public override void Update(GameTime gameTime)
        {
            timeUpdate.Enter();

            currentGamePadState = GamePad.GetState(PlayerNumber);
            currentKeyboardState = Keyboard.GetState();
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            bool isWalking = false;
            bool standingOnTheGround = PositionOnPlanet.Y < MaxHeightForJump;
            if (gameTime.TotalGameTime > CrashJumpPenaltyUntil)
            {
#if(DEBUG)
                // Some debugging controls...
                if ((KeyJustPressed(Buttons.Y) || KeyJustPressed(Keys.F2)) && !GameHM.Camera.IsShaking)
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

                if (KeyJustPressed(Buttons.Start) || KeyJustPressed(Keys.F1))
                    GameHM.CurrentTheme = GameHM.Themes[(GameHM.CurrentThemeID + 1) % 2];
#endif

                if ((KeyJustPressed(Buttons.A) || KeyJustPressed(JumpKey(PlayerNumber))) && standingOnTheGround)
                {
                    VelocityOnPlanet.Y = JumpStrength;
                    //GameHM.CurrentTheme.SoundJump.Play();
                }

                bool CouldCrashJump = VelocityOnPlanet.Y < 0 || (VelocityOnPlanet.Y >= 0 && (PositionOnPlanet.Y > MinHeightForCrashJump));
                if (!DoingCrashJump && !standingOnTheGround && CouldCrashJump &&
                    (KeyJustPressed(Buttons.RightTrigger) || KeyJustPressed(StompKey(PlayerNumber))))
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
                
                // Walk left/right
                float dx = currentGamePadState.ThumbSticks.Left.X;
                if (currentKeyboardState.IsKeyDown(LeftKey(PlayerNumber)))
                    dx = -1.0f;
                else if (currentKeyboardState.IsKeyDown(RightKey(PlayerNumber)))
                    dx = 1.0f;
                VelocityOnPlanet.X += dx * RunStrength;
                isWalking = (dx != 0.0f);
                if (!(currentGamePadState.IsButtonDown(Buttons.LeftTrigger) ||
                      currentKeyboardState.IsKeyDown(StealthKey(PlayerNumber))) && standingOnTheGround)
                {
                    planet.RotationSpeed -= dx * RunStrengthPlanet;
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
            oldKeyboardState = currentKeyboardState;

            timeUpdate.Exit();
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

        private bool KeyJustPressed(Keys key)
        {
            if (oldKeyboardState == null || currentKeyboardState == null)
            {
                return false;
            }

            bool wasDown = oldKeyboardState.IsKeyDown(key);
            bool isDown = currentKeyboardState.IsKeyDown(key);

            return !wasDown && isDown;
        }

        static Profile timeDraw = Profile.Get("Monkey.Draw");

        public override void Draw(GameTime gameTime)
        {
            timeDraw.Enter();

            SpriteAnimationSwitcher sprite = PlayerNumber != PlayerIndex.One ? GameHM.CurrentTheme.MonkeyM : GameHM.CurrentTheme.MonkeyF;
            if (CurrentAnimation == "left" || CurrentAnimation == "right")
                sprite.Animations[CurrentAnimation].AnimationFPS = AnimationFpsScale * Math.Abs(VelocityOnPlanet.X);
            sprite.Draw(this, gameTime, CurrentAnimation, Position, planet.GetShadingForPlanetGround(PositionOnPlanet.X), planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 1.0f);

            if (CrashJumpPenaltyUntil > gameTime.TotalGameTime)
            {
                Vector2 pos = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, 0);
                float smokePerc = 1.0f - (float)(CrashJumpPenaltyUntil.Subtract(gameTime.TotalGameTime).TotalSeconds / CrashJumpPenalty.TotalSeconds);
                sprite.DrawPercentage(this, "stomp", smokePerc, pos, planet.GetShadingForPlanetGround(PositionOnPlanet.X), planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, 0.5f);
            }

            timeDraw.Exit();
        }

        static Profile timeCollide = Profile.Get("Monkey.OnCollide");

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            timeCollide.Enter();

            if (otherObject is CoconutOrbit)
            {
                // Do some animation stuff?
                VelocityOnPlanet.Y = 0;
            }
            else if (otherObject is CoconutExplosion)
            {
                if (CoconutMissile.SPLASH_DAMAGE)
                {
                    IGotHit(offsetMeToOther);
                }
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

            timeCollide.Exit();
        }

        private void IGotHit(Vector2 offsetMeToOther)
        {
            HelpSystem.GloballyEnabled = false;

            // Player is hurt
            HitPoints--;
            GameHM.CurrentTheme.SoundMonkeyHit.Play();

            // Hide helpers when hit
            if (HelpMovement.DisplayHelp)
                HelpMovement.DisplayHelp = false;

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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Honeymoon
{
    public class Intro : DrawableGameComponent
    {
        public HoneymoonGame GameHM;
        public Texture2D Screen;
        Vector2 Position;
        float fadingTimer, maxFadingTime;
        GamePadState currentGamePadState;
        GamePadState oldGamePadState;
        bool leavingIntro;

        public Intro()
            : base(HoneymoonGame.Instance)
        {
            GameHM = HoneymoonGame.Instance;
            this.Position = Vector2.Zero;
            this.DrawOrder = 8;
            this.leavingIntro = false;
        }

        public override void Update(GameTime gameTime)
        {
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (leavingIntro)
            {
                fadingTimer -= seconds;
                if (fadingTimer <= 0.0f) fadingTimer = 0.0f;
                if (!GameHM.Camera.IsShaking)
                {
                    GameHM.GameState = HoneymoonGame.GameStates.Game;
                    leavingIntro = false;
                }
            }
            else
            {
                if (KeyJustPressed(Buttons.A) || KeyJustPressed(Buttons.X) ||
                    KeyJustPressed(Buttons.Start))
                {
                    GameHM.SelectionSound.Play();
                    GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime, DriftingCamera.CameraShakingFrequency, DriftingCamera.CameraShakingAmplitude);
                    maxFadingTime = DriftingCamera.CameraShakingTime * 0.5f;
                    fadingTimer = maxFadingTime;
                    leavingIntro = true;
                }
            }

            oldGamePadState = currentGamePadState;
        }

        public override void Draw(GameTime gameTime)
        {
            Color color = new Color(Color.White, leavingIntro ? fadingTimer / maxFadingTime : 1.0f);
            GameHM.spriteBatch.Draw(Screen, Position, color);
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
    }
}
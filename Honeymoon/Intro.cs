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
        GamePadState currentGamePadState;
        GamePadState oldGamePadState;
        bool leavingIntro;

        public Intro()
            : base(HoneymoonGame.Instance)
        {
            GameHM = HoneymoonGame.Instance;
            this.DrawOrder = 8;
            this.leavingIntro = false;
        }

        public override void Update(GameTime gameTime)
        {
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (leavingIntro)
            {
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
                    GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime, DriftingCamera.CameraShakingFrequency, DriftingCamera.CameraShakingAmplitude);
                    leavingIntro = true;
                }
            }

            oldGamePadState = currentGamePadState;
        }

        public override void Draw(GameTime gameTime)
        {
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
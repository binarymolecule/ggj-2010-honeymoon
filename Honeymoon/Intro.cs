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
        GamePadState[] currentGamePadState = new GamePadState[2];
        GamePadState[] oldGamePadState = new GamePadState[2];
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
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentGamePadState[0] = GamePad.GetState(PlayerIndex.One);
            currentGamePadState[1] = GamePad.GetState(PlayerIndex.Two);
            if (leavingIntro)
            {
                fadingTimer -= seconds;
                if (fadingTimer <= 0.0f) fadingTimer = 0.0f;
                if (!GameHM.Camera.IsShaking)
                {
                    GameHM.GameState = HoneymoonGame.GameStates.Game;
                    GameHM.OnGameStarted();
                    leavingIntro = false;
                }
            }
            else
            {
                if (KeyJustPressed(0, Buttons.A) || KeyJustPressed(1, Buttons.A) ||
                    KeyJustPressed(0, Buttons.X) || KeyJustPressed(1, Buttons.X) ||
                    KeyJustPressed(0, Buttons.Start) || KeyJustPressed(1, Buttons.Start))
                {
                    GameHM.SelectionSound.Play();
                    GameHM.Camera.ShakeCamera(DriftingCamera.CameraShakingTime, DriftingCamera.CameraShakingFrequency, DriftingCamera.CameraShakingAmplitude);
                    maxFadingTime = DriftingCamera.CameraShakingTime * 0.5f;
                    fadingTimer = maxFadingTime;
                    leavingIntro = true;
                }
            }
            oldGamePadState[0] = currentGamePadState[0];
            oldGamePadState[1] = currentGamePadState[1];
        }

        public override void Draw(GameTime gameTime)
        {
            Color color = new Color(Color.White, leavingIntro ? fadingTimer / maxFadingTime : 1.0f);
            GameHM.spriteBatch.Draw(Screen, Position, color);
        }

        private bool KeyJustPressed(int index, Buttons button)
        {
            if (index < 0 || index > 1 ||
                oldGamePadState[index] == null || currentGamePadState[index] == null)
            {
                return false;
            }

            bool wasDown = oldGamePadState[index].IsButtonDown(button);
            bool isDown = currentGamePadState[index].IsButtonDown(button);

            return !wasDown && isDown;
        }
    }
}
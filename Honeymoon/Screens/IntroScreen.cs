using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeplersLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Honeymoon.Screens
{
    class IntroScreen : GameScreen
    {
        public Texture2D Background, Title;
        public DriftingCamera Camera = new DriftingCamera();
        public List<Texture2D> Parallax;
        public SpriteBatch spriteBatch;
        public SoundEffect SelectionSound;

        float fadingTimer, maxFadingTime;
        GamePadState[] currentGamePadState = new GamePadState[2];
        GamePadState[] oldGamePadState = new GamePadState[2];
        KeyboardState oldKeyboardState, currentKeyboardState;
        bool leavingIntro = false;

        public override void LoadContent()
        {
            base.LoadContent();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Background = Content.Load<Texture2D>("Textures/Backgrounds/good");
            Parallax = new List<Texture2D> {
                Content.Load<Texture2D>("Textures/Backgrounds/stars1"),
                Content.Load<Texture2D>("Textures/Backgrounds/stars2"),
                Content.Load<Texture2D>("Textures/Backgrounds/stars3")
            };

            SelectionSound = Content.Load<SoundEffect>("Sounds/select");
            Title = Content.Load<Texture2D>("Textures/Backgrounds/title");
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (otherScreenHasFocus)
                return;

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Camera.Update(seconds);
            Camera.Translation.X = (float)(Math.Sin(gameTime.TotalGameTime.TotalSeconds * 0.3) * 50.0);
            Camera.Translation.Y = (float)(Math.Cos(gameTime.TotalGameTime.TotalSeconds * 0.3) * 50.0);

            currentGamePadState[0] = GamePad.GetState(PlayerIndex.One);
            currentGamePadState[1] = GamePad.GetState(PlayerIndex.Two);
            currentKeyboardState = Keyboard.GetState();

            if (leavingIntro)
            {
                fadingTimer -= seconds;
                if (fadingTimer <= 0.0f) fadingTimer = 0.0f;

                if (!Camera.IsShaking)
                {
                    VersusScreen versus = new VersusScreen();
                    //ScreenManager.AddScreen(versus, null);
                    LoadingScreen.Load(ScreenManager, true, null, versus);
                    versus.OnGameStarted();
                    leavingIntro = false;
                    // ExitScreen();
                }
            }
            else
            {
                if (KeyJustPressed(0, Buttons.A) || KeyJustPressed(1, Buttons.A) ||
                    KeyJustPressed(0, Buttons.X) || KeyJustPressed(1, Buttons.X) ||
                    KeyJustPressed(0, Buttons.Start) || KeyJustPressed(1, Buttons.Start) ||
                    KeyJustPressed(Keys.Enter) || KeyJustPressed(Keys.Space))
                {
                    SelectionSound.Play();
                    Camera.ShakeCamera(DriftingCamera.CameraShakingTime, DriftingCamera.CameraShakingFrequency, DriftingCamera.CameraShakingAmplitude);
                    maxFadingTime = DriftingCamera.CameraShakingTime * 0.5f;
                    fadingTimer = maxFadingTime;
                    leavingIntro = true;
                }
                else if(KeyJustPressed(0, Buttons.Back) || KeyJustPressed(Keys.Escape))
                {
                    ExitScreen();
                }
            }
            oldGamePadState[0] = currentGamePadState[0];
            oldGamePadState[1] = currentGamePadState[1];
            oldKeyboardState = currentKeyboardState;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            Color color = new Color(Color.White, leavingIntro ? fadingTimer / maxFadingTime : 1.0f);

            spriteBatch.Begin();
            DrawBackgrounds();
            spriteBatch.Draw(Title, Vector2.Zero, color);
            spriteBatch.End();
        }

        private void DrawBackgrounds()
        {
            Vector2 camTranslation = Camera.Inverse2DTranslation;
            spriteBatch.Draw(Background, camTranslation * 0.9f, Color.White);

            float distance = 0.8f;
            foreach (Texture2D t2d in Parallax)
            {
                spriteBatch.Draw(t2d, camTranslation * distance, Color.White);
                distance -= 0.1f;
            }
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

        private bool KeyJustPressed(Keys key)
        {
            if (oldGamePadState == null || currentKeyboardState == null)
                return false;
            bool wasDown = oldKeyboardState.IsKeyDown(key);
            bool isDown = currentKeyboardState.IsKeyDown(key);
            return !wasDown && isDown;
        }
    }
}

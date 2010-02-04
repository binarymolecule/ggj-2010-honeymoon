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
                    //ExitScreen();
                }
            }
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex dummy;
            if (input.IsMenuSelect(null, out dummy))
            {
                SelectionSound.Play();
                Camera.ShakeCamera(DriftingCamera.CameraShakingTime, DriftingCamera.CameraShakingFrequency, DriftingCamera.CameraShakingAmplitude);
                maxFadingTime = DriftingCamera.CameraShakingTime * 0.5f;
                fadingTimer = maxFadingTime;
                leavingIntro = true;
            }
            else if (input.IsMenuCancel(null, out dummy))
            {
                ExitScreen();
            }
#if(!XBOX360)
            // Toggle fullscreen mode
            if (input.IsNewKeyPress(Keys.F5))
                ((HoneymoonGame)Game).ToggleFullScreen();
#endif
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
    }
}

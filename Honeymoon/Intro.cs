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
        int selection;
        GamePadState currentGamePadState;
        GamePadState oldGamePadState;

        public Intro()
            : base(HoneymoonGame.Instance)
        {
            GameHM = HoneymoonGame.Instance;
            Game.Components.Add(this);
            this.DrawOrder = 10;
            this.selection = 0;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!GameHM.Camera.IsMoving)
            {
                if (selection > 0 &&
                    (KeyJustPressed(Buttons.DPadUp) || KeyJustPressed(Buttons.LeftThumbstickUp)))
                {
                    GameHM.Camera.MoveCamera(Vector2.UnitY, 100.0f);
                }
                else if (selection < 1 &&
                         (KeyJustPressed(Buttons.DPadDown) || KeyJustPressed(Buttons.LeftThumbstickDown)))
                {
                    GameHM.Camera.MoveCamera(-Vector2.UnitY, 100.0f);
                }
            }
            if (KeyJustPressed(Buttons.A) || KeyJustPressed(Buttons.X) ||
                KeyJustPressed(Buttons.Start))
            {
            }

            oldGamePadState = currentGamePadState;
            base.Update(gameTime);
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
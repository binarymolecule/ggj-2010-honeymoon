using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class HelpSystem : DrawableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Monkey monkey;

        public bool DisplayHelp = true;
        public float FadePercent = 1.0f;
        public string SpriteName = "help_XXXXX";

        public HelpSystem(Monkey monkey, String screen)
            : base(HoneymoonGame.Instance)
        {
            this.monkey = monkey;
            this.DrawOrder = 3;
            this.SpriteName = "help_" + screen;
            Game.Components.Add(this);
        }

        protected override void LoadContent()
        {
            Sprite = Game.Content.Load<Texture2D>(SpriteName);
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FadePercent += seconds * (DisplayHelp ? 1.0f : -1.0f);
            if (FadePercent > 1) FadePercent = 1;
            else if (FadePercent < 0) FadePercent = 0;
        }


        public override void Draw(GameTime gameTime)
        {
            if (FadePercent < 0.0001) return;

            HoneymoonGame GameHM = Game as HoneymoonGame;
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, monkey.planet.Position, null, new Color(Color.White, FadePercent), monkey.planet.Rotation + monkey.PositionOnPlanet.X + (float)Math.PI / 2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }



    }
}

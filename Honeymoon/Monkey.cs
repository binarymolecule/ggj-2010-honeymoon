using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class Monkey : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public float PositionOnPlanet;
        public Planet planet;
        public static float MonkeyHeight = 64.0f;

        public Monkey(Planet planet)
        {
            this.planet = planet;
        }
         
        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("monkey");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float  seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Position = planet.GetPositionOnPlanet(PositionOnPlanet, MonkeyHeight/2.0f);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, planet.Rotation + PositionOnPlanet + (float)Math.PI/2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }



    }
}

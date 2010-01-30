using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class Tree : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Planet planet;
        public float growthFactor;
        public bool isMature;
        public static float GrowthPerSecond = 0.5f;

        public Vector2 PositionOnPlanet;

        public Tree(Planet planet)
        {
            this.planet = planet;
            this.growthFactor = 0.0f;
            this.isMature = false;
            this.PositionOnPlanet = Vector2.Zero;
            this.DrawOrder = 0;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("palm");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
            PositionOnPlanet.X = 0.0f;
            PositionOnPlanet.Y = -SpriteCenter.Y;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            growthFactor += seconds * GrowthPerSecond;
            if (growthFactor >= 1.0f) {
                growthFactor = 0.0f;
                if (!isMature) {
                    isMature = true;
                    // Tree has grown to maturity
                }
                else
                {
                    // Coconut has been produced
                }
            }
            if (!isMature)
            {
                // Tree is still growing
                float offsetY = growthFactor * Sprite.Height;
                Position = planet.GetPositionOnPlanet(PositionOnPlanet.X, PositionOnPlanet.Y + offsetY);
            }
            else
            {
                Position = planet.GetPositionOnPlanet(PositionOnPlanet.X, PositionOnPlanet.Y + Sprite.Height);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 1);
            GameHM.spriteBatch.End();
        }

    }
}

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
        public Vector2 PositionOnPlanet;
        public static float GrowthPerSecond = 0.2f;
                
        public Texture2D CoconutSprite;
        public Vector2 CoconutCenter;
        public Vector2 CoconutPosition;
        public static float CoconutOffsetFromTop = 24.0f;
        
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

            CoconutSprite = GameHM.Content.Load<Texture2D>("coconut");
            CoconutCenter = new Vector2(CoconutSprite.Width, CoconutSprite.Height) / 2.0f;
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
                CoconutPosition = planet.GetPositionOnPlanet(PositionOnPlanet.X, PositionOnPlanet.Y + 1.5f * Sprite.Height - CoconutOffsetFromTop);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float angle = planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f;

            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, angle, SpriteCenter, 1.0f, SpriteEffects.None, 1);
            if (isMature)
            {
                GameHM.spriteBatch.Draw(CoconutSprite, CoconutPosition, null, Color.White, angle, CoconutCenter, growthFactor, SpriteEffects.None, 1);
            }
            GameHM.spriteBatch.End();
        }

    }
}

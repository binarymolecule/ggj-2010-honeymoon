using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class Monkey : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Planet planet;
        public static float MonkeyWalkingHeight = 28.0f;
        public static float GravityStrength = 1000.0f;
        public static float BounceFactor = 0.2f;
        public static float Friction = 0.9f;
        public static float MaxHeightForJump = 5.0f;
        public static float JumpStrength = 100.0f;

        public Vector2 PositionOnPlanet;
        public Vector2 VelocityOnPlanet;


        public Monkey(Planet planet)
        {
            this.planet = planet;
            this.DrawOrder = 1;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("monkey");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            VelocityOnPlanet.X += gamePadState.ThumbSticks.Left.X;
            if (gamePadState.Buttons.A == ButtonState.Pressed && PositionOnPlanet.Y < MaxHeightForJump) VelocityOnPlanet.Y += JumpStrength;

            VelocityOnPlanet.Y -= GravityStrength * seconds;
            VelocityOnPlanet *= (float)Math.Pow(1.0f - Friction, seconds);

            PositionOnPlanet += VelocityOnPlanet * seconds;
            if (PositionOnPlanet.Y < 0)
            {
                PositionOnPlanet.Y = 0;
                VelocityOnPlanet.Y *= -BounceFactor;
            }

            Position = planet.GetPositionOnPlanet(PositionOnPlanet.X, MonkeyWalkingHeight + PositionOnPlanet.Y);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }



    }
}

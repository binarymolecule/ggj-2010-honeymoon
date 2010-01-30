using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class CoconutOrbit : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public Planet planet;
        public float height;
        public static float CoconutOrbitHeight = 128.0f;
        public static float CoconutOrbitVelocity = 1.0f;

        public Vector2 PositionOnPlanet;

        public CoconutOrbit(Planet planet, float angle, float height)
            : base(planet.PlayerNumber)
        {
            this.planet = planet;
            this.height = height; //CoconutOrbitHeight;
            this.DrawOrder = 3;
            this.CollisionEnabled = true;
            this.PositionOnPlanet = new Vector2(angle, height);
            this.Position = planet.GetPositionInPlanetOrbit(angle, height);
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("coconut");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;
            this.CollisionRadius = 0.5f * Sprite.Width;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PositionOnPlanet.X += CoconutOrbitVelocity * seconds;
            Position = planet.GetPositionInPlanetOrbit(PositionOnPlanet.X, height);
        }

        public override void Draw(GameTime gameTime)
        {
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, Color.White, planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f, SpriteCenter, 1.0f, SpriteEffects.None, 0);
            GameHM.spriteBatch.End();
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is Monkey)
            {
                // Coconut is converted into a missile when hit
                Vector2 dir = -1.0f * offsetMeToOther;
                dir.Normalize();
                CoconutMissile coconut = new CoconutMissile(Position, dir, ((Monkey)otherObject).PlayerNumber);
                GameHM.Components.Add(coconut);
                this.Dispose();
            }
        }
    }
}

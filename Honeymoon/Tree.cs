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
        public Planet planet;
        public Vector2 PositionOnPlanet;
        public float growth;  // within range 0..1
        public bool isMature; // mature tree is fully grown and produces coconuts
        public float sunlightFactor;
        public static float GrowthPerSecond = 1.2f;
                
        public Vector2 CoconutPosition;
        public int CoconutCount;
        public static float CoconutOffsetFromTop = 16.0f;
        public static int MaxNumberOfCoconuts = 5;

        public Tree(Planet planet)
            : base(planet.PlayerNumber)
        {
            this.planet = planet;
            this.growth = 0.0f;
            this.isMature = false;
            this.PositionOnPlanet = Vector2.Zero;
            this.DrawOrder = 0;
            this.CoconutCount = 0;
            Game.Components.Add(this);
            PositionOnPlanet.Y = -96.0f / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 direction = new Vector2(-(float)Math.Cos(planet.Rotation + PositionOnPlanet.X),
                                            -(float)Math.Sin(planet.Rotation + PositionOnPlanet.X));
            sunlightFactor = Math.Max(0.0f, Vector2.Dot(GameHM.SunlightDir, direction));
            if (CoconutCount < MaxNumberOfCoconuts)
            {
                growth += seconds * sunlightFactor * GrowthPerSecond;
            float spriteHeight = 96.0f;
            if (growth >= 1.0f)
            {
                    growth = 0.0f;
                if (!isMature)
                {
                        isMature = true;
                        // Tree has grown to maturity
                    }
                    else //if (CoconutCount < MaxNumberOfCoconuts)
                    {
                        // Coconut has been produced
                    CoconutOrbit coconut = new CoconutOrbit(planet, PositionOnPlanet.X + planet.Rotation, PositionOnPlanet.Y + 1.5f * spriteHeight - CoconutOffsetFromTop);
                        GameHM.Components.Add(coconut);
                    }
                }
            }
            if (!isMature)
            {
                // Tree is still growing
                float offsetY = growth * spriteHeight;
                Position = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, PositionOnPlanet.Y + offsetY);
            }
            else
            {
                Position = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, PositionOnPlanet.Y + spriteHeight);
                CoconutPosition = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, PositionOnPlanet.Y + 1.5f * spriteHeight - CoconutOffsetFromTop);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float angle = planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f;
            float c = Math.Min(1.0f, 0.5f + 0.5f * sunlightFactor);
            Color color = new Color(c, c, c);
            GameHM.CurrentTheme.Tree.Draw(this, gameTime, "palm", Position, color, angle, 1.0f);
            if (isMature)
                GameHM.CurrentTheme.Coconut.Draw(this, gameTime, "coconut", CoconutPosition, Color.White, angle, growth);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Honeymoon.Screens;

namespace Honeymoon
{
    public class Tree : ObjectOnPlanet
    {
        public float growth;  // within range 0..1
        public bool isMature; // mature tree is fully grown and produces coconuts
        public float sunlightFactor;
        public static float GrowthPerSecond = 25.0f / 80.0f;

        public Vector2 CoconutPosition;
        public int CoconutCount;
        public static float CoconutOffsetFromTop = 16.0f;
        public static int MaxNumberOfCoconuts = 5;
        public static float spriteHeight = 96.0f;

        public HelpSystem HelpCoconut;

        public Tree(Planet planet)
            : base(planet)
        {
            this.planet = planet;
            this.growth = 0.0f;
            this.isMature = false;
            this.PositionOnPlanet = Vector2.Zero;
            this.DrawOrder = 0;
            this.CoconutCount = 0;
            VersusScreen.Instance.Components.Add(this);
            PositionOnPlanet.X = (float) Math.PI / 2;
            PositionOnPlanet.Y = spriteHeight / 2.0f;
            HelpCoconut = new HelpSystem(this, "help_coconut");
            HelpCoconut.FadePercent = 0.0f;
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            sunlightFactor = Math.Max(0.0f, Vector2.Dot(GameHM.SunlightDir, planet.rot2vec(planet.Rotation+PositionOnPlanet.X)));
            if (CoconutCount < MaxNumberOfCoconuts)
            {
                growth += seconds * sunlightFactor * GrowthPerSecond;
                if (growth >= 1.0f)
                {
                    growth = 0.0f;
                    if (!isMature)
                    {
                        HelpCoconut.Angle = -0.3f;
                        HelpCoconut.DisplayHelp = true;
                        isMature = true;
                        // Tree has grown to maturity
                    }
                    else //if (CoconutCount < MaxNumberOfCoconuts)
                    {
                        HelpCoconut.DisplayHelp = false;

                        // Coconut has been produced
                        CoconutOrbit coconut = new CoconutOrbit(planet, this, PositionOnPlanet.X + planet.Rotation, PositionOnPlanet.Y + 0.5f * spriteHeight - CoconutOffsetFromTop);
                        GameHM.Components.Add(coconut);
                        GameHM.CurrentTheme.SoundCreateCoconut.Play();
                    }
                }
            }

            Position = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, PositionOnPlanet.Y + 4f);
            CoconutPosition = planet.GetPositionOnPlanetGround(PositionOnPlanet.X, PositionOnPlanet.Y + 0.5f * spriteHeight - CoconutOffsetFromTop);
        }

        public override void Draw(GameTime gameTime)
        {
            float angle = planet.Rotation + PositionOnPlanet.X + (float)Math.PI / 2.0f;
            GameHM.CurrentTheme.Tree.DrawPercentage(this, "palme", isMature ? 1 : growth, Position, planet.GetShadingForPlanetGround(PositionOnPlanet.X), angle, 1.0f);
            if (isMature)
                GameHM.CurrentTheme.Coconut.DrawPercentage(this, "coconut", 0, CoconutPosition, Color.White, angle, growth);
        }
    }
}

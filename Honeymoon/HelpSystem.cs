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
        public static bool GloballyEnabled = true;

        public ObjectOnPlanet monkey;
        public SpriteAnimationSwitcher animations;
        public String screen;
        public bool DisplayHelp = false;
        public float FadePercent = 1.0f;

        public HelpSystem(ObjectOnPlanet monkey, String screen)
            : base(HoneymoonGame.Instance)
        {
            this.monkey = monkey;
            this.DrawOrder = 3;
            this.screen = screen;
            animations = new SpriteAnimationSwitcher("help", new String[] { screen });
            animations.Animations[screen].AnimationFPS = 2.0f;
            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FadePercent += seconds * ((DisplayHelp && GloballyEnabled) ? 1.0f : -1.0f);
            if (FadePercent > 1) FadePercent = 1;
            else if (FadePercent < 0) FadePercent = 0;
        }


        public override void Draw(GameTime gameTime)
        {
            if (FadePercent < 0.0001) return;

            float rot = monkey.planet.Rotation + monkey.PositionOnPlanet.X + (float)Math.PI / 2.0f;
            rot = 0;
            animations.Draw(this, gameTime, screen, monkey.planet.Position, new Color(Color.White, FadePercent), rot, 1.0f);
        }
    }
}

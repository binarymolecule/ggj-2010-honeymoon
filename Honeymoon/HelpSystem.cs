﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Honeymoon.Screens;

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
        public float Angle = -0.3f;

        public HelpSystem(ObjectOnPlanet monkey, String screen)
            : base(VersusScreen.Instance.Game)
        {
            this.monkey = monkey;
            this.DrawOrder = 3;
            this.screen = screen;
            animations = new SpriteAnimationSwitcher("help", new String[] { screen });
            animations.Animations[screen].AnimationFPS = 2.0f;
            VersusScreen.Instance.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            FadePercent += seconds * ((DisplayHelp && GloballyEnabled) ? 1.0f : -1.0f);
            Angle += seconds * 0.03f;
            if (FadePercent > 1) FadePercent = 1;
            else if (FadePercent < 0) FadePercent = 0;
        }


        public override void Draw(GameTime gameTime)
        {
            if (FadePercent < 0.0001) return;

            float rot = monkey.planet.Rotation + monkey.PositionOnPlanet.X + (float)Math.PI / 2.0f;
            animations.Draw(this, gameTime, screen, monkey.planet.Position, new Color(Color.White, FadePercent), Angle, 1.0f);
        }
    }
}

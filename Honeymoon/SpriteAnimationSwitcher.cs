using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Honeymoon
{
    public class SpriteAnimationSwitcher
    {
        HoneymoonGame game;

        public String CurrentAnimation;
        public Dictionary<String, SpriteAnimation> Animations = new Dictionary<string, SpriteAnimation>();
        SpriteAnimation drawMe;

        public SpriteAnimationSwitcher(String theme, String[] animations)
        {
            game = HoneymoonGame.Instance;
            foreach (String anim in animations)
            {
                Animations.Add(anim, new SpriteAnimation("Textures/" + theme + "/" + anim));
            }
        }

        public float CurrentFrame;
        public int CurrentIntegerFrame { get { return (int)Math.Floor(CurrentFrame); } }

        TimeSpan lastUpdateTime = TimeSpan.Zero;

        public void Draw(GameTime gameTime, String Animation, Vector2 Position, Color Color, float Rotation, float Scale)
        {
            if (CurrentAnimation != Animation)
            {
                CurrentAnimation = Animation;
                drawMe = Animations[CurrentAnimation];
                CurrentFrame = 0;
            }

            float diff = (float)gameTime.TotalGameTime.Subtract(lastUpdateTime).TotalSeconds;
            if (diff > 2.0f) diff = 0.0f;

            CurrentFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * drawMe.AnimationFPS;
            int frame = CurrentIntegerFrame;
            if (frame >= drawMe.NumberOfFrames)
            {
                CurrentFrame = 0;
            }

            drawMe.Draw(CurrentIntegerFrame, Position, Color, Rotation, Scale);
        }
    }
}

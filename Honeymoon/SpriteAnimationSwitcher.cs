using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Honeymoon
{
    public class SpriteAnimationSwitcher
    {
        public String CurrentAnimation;
        public Dictionary<String, SpriteAnimation> Animations = new Dictionary<string, SpriteAnimation>();
        SpriteAnimation drawMe;

        public SpriteAnimationSwitcher(String[] animations)
        {
            foreach (String anim in animations)
            {
                Animations.Add(anim, new SpriteAnimation(anim));
            }
            SwitchTo(animations[0]);
        }

        public void SwitchTo(String animation)
        {
            if (CurrentAnimation == animation) return;
            CurrentAnimation = animation;
            drawMe = Animations[CurrentAnimation];
            drawMe.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            drawMe.Update(gameTime);
        }
        public void Draw(Vector2 Position, Color color, float Rotation, float Scale)
        {
            drawMe.Draw(Position, color, Rotation, Scale);
        }
    }
}

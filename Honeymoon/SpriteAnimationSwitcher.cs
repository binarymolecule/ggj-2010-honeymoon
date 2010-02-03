using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KeplersLibrary;
using KeplersDataTypes;

namespace Honeymoon
{
    public class SpriteAnimationSwitcher
    {
        HoneymoonGame game;

        public Dictionary<object, String> CurrentAnimation = new Dictionary<object, String>();
        public Dictionary<String, SpriteAnimation> Animations = new Dictionary<String, SpriteAnimation>();
        public Dictionary<object, float> CurrentFrame = new Dictionary<object,float>();

        public SpriteAnimationSwitcher(String theme, String[] animations)
        {
            game = HoneymoonGame.Instance;
            foreach (String anim in animations)
            {
                Animations.Add(anim, SpriteAnimation.Load(game.Content, "Textures/" + theme + "/" + anim));
            }
        }


        Dictionary<object,TimeSpan> lastUpdateTime = new Dictionary<object,TimeSpan>();

        public void Draw(object GameObject, GameTime gameTime, String Animation, Vector2 Position, Color Color, float Rotation, float Scale)
        {
            if (! CurrentAnimation.ContainsKey(GameObject))
            {
                CurrentAnimation[GameObject] = null;
                lastUpdateTime[GameObject] = TimeSpan.Zero;
            }

            if (CurrentAnimation[GameObject] != Animation)
            {
                CurrentAnimation[GameObject] = Animation;
                CurrentFrame[GameObject] = 0;
            }
            SpriteAnimation drawMe = Animations[Animation];

            float diff = (float)gameTime.TotalGameTime.Subtract(lastUpdateTime[GameObject]).TotalSeconds;
            lastUpdateTime[GameObject] = gameTime.TotalGameTime;
            if (diff > 2.0f) diff = 0.0f;

            CurrentFrame[GameObject] += diff * drawMe.AnimationFPS;
            int frame = (int)Math.Floor(CurrentFrame[GameObject]);
            if (frame >= drawMe.NumberOfFrames)
            {
                CurrentFrame[GameObject] = 0;
                frame = 0;
            }

            game.spriteBatch.Draw(drawMe, frame, Position, Color, Rotation, Scale);
        }
        public void DrawPercentage(object GameObject, String Animation, float ZeroToOne, Vector2 Position, Color Color, float Rotation, float Scale)
        {
            SpriteAnimation drawMe = Animations[Animation];
            int frame = (int)Math.Floor(drawMe.NumberOfFrames * ZeroToOne);
            if (frame >= drawMe.NumberOfFrames) frame = drawMe.NumberOfFrames - 1;
            game.spriteBatch.Draw(drawMe, frame, Position, Color, Rotation, Scale);
        }

        public void JumpTo(object GameObject, String Animation, float Position)
        {
            CurrentAnimation[GameObject] = Animation;
            CurrentFrame[GameObject] = Position * Animations[Animation].NumberOfFrames;
            lastUpdateTime[GameObject] = TimeSpan.Zero;
        }

    }
}

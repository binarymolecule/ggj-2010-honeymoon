using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Honeymoon
{
    public class SpriteAnimation
    {
        HoneymoonGame game;

        public Texture2D[] Sprites;
        public Vector2 SpriteCenter;

        public SpriteAnimation(String name)
        {
            game = HoneymoonGame.Instance;
            Texture2D asset = game.Content.Load<Texture2D>(name);
            if (asset != null)
            {
                Sprites = new Texture2D[] { asset };
            }
            else
            {
                List<Texture2D> loaded = new List<Texture2D>();
                while (true)
                {
                    String number = String.Format("###", loaded.Count);
                    asset = game.Content.Load<Texture2D>(name + number);
                    if (asset == null) break;
                    loaded.Add(asset);
                }
                Sprites = loaded.ToArray();
            }
        }

        public float CurrentFrame;
        public int CurrentIntegerFrame { get { return (int)Math.Floor(CurrentFrame); } }
        public float AnimationFPS = 1.0f;
        public int NumberOfFrames { get { return Sprites.Length; } }


        public delegate void AnimationFinishedHandler(SpriteAnimation animation);
        public event AnimationFinishedHandler AnimationFinished;

        public void Update(GameTime gameTime)
        {
            CurrentFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * AnimationFPS;
            int frame = CurrentIntegerFrame;
            if (frame >= NumberOfFrames)
            {
                AnimationFinished.Invoke(this);
                CurrentFrame = 0;
            }
        }

        public void Draw(Vector2 Position, Color color, float Rotation, float Scale)
        {
            game.spriteBatch.Begin();
            game.spriteBatch.Draw(Sprites[CurrentIntegerFrame], Position, null, color, Rotation, SpriteCenter, Scale, SpriteEffects.None, 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Honeymoon
{
    public class SpriteAnimation
    {
        HoneymoonGame game;

        public Texture2D[] Sprites;
        public Vector2 SpriteCenter;
        public float AnimationFPS = 1.0f;
        public int NumberOfFrames { get { return Sprites.Length; } }

        public SpriteAnimation(String name)
        {
            game = HoneymoonGame.Instance;

            try
            {
                Texture2D asset = game.Content.Load<Texture2D>(name);
                Sprites = new Texture2D[] { asset };
                return;
            }
            catch (ContentLoadException) { }

            List<Texture2D> loaded = new List<Texture2D>();
            while (true)
            {
                String number = String.Format("{0:000}", loaded.Count);
                try
                {
                    Texture2D asset = game.Content.Load<Texture2D>(name + number);
                    loaded.Add(asset);
                }
                catch (ContentLoadException) { break; }
            }
            Sprites = loaded.ToArray();

        }

        public void Draw(int Frame, Vector2 Position, Color Color, float Rotation, float Scale)
        {
            game.spriteBatch.Begin();
            game.spriteBatch.Draw(Sprites[Frame], Position, null, Color, Rotation, SpriteCenter, Scale, SpriteEffects.None, 0);
            game.spriteBatch.End();
        }
    }
}

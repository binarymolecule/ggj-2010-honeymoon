using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KeplersDataTypes;

namespace KeplersLibrary
{
    public static class SpriteBatchAnimationExtension
    {
        public static void Draw(this SpriteBatch spriteBatch, SpriteAnimation animation, int Frame,
            Vector2 Position, Color Color, float Rotation, float Scale)
        {
            spriteBatch.Draw(animation.Sprites[Frame], Position,
                null, Color,
                Rotation,
                animation.SpriteCenter, Scale,
                SpriteEffects.None, 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ContentProcessors
{
    [ContentSerializerRuntimeType("KeplersDataTypes.SpriteAnimation, KeplersDataTypes")]
    public class SpriteAnimationContent
    {
        public Texture2DContent[] Sprites;

        public float AnimationFPS;
    }
}

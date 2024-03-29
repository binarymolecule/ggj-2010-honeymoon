﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace KeplersDataTypes
{
    // See also KeplerLibrary.SpriteAnimationContent
    // http://blogs.msdn.com/shawnhar/archive/2009/03/25/automatic-xnb-serialization-in-xna-game-studio-3-1.aspx
    public class SpriteAnimation
    {
        public Texture2D[] Sprites
        {
            get
            {
                return this.sprites;
            }

            set
            {
                this.sprites = value;
                SpriteCenter = new Vector2(value[0].Width, value[0].Height) / 2.0f;
            }
        }

        public float AnimationFPS
        {
            get { return animationFps; }
            set { animationFps = value; }
        }


        private Texture2D[] sprites;

        private float animationFps = 25.0f;


        [ContentSerializerIgnore]
        public Vector2 SpriteCenter;

        [ContentSerializerIgnore]
        public int NumberOfFrames { get { return Sprites.Length; } }

        // Required for XNB deserialization
        protected SpriteAnimation()
        {

        }

        public SpriteAnimation(params Texture2D[] sprites)
        {
            this.Sprites = sprites;
        }

        public static SpriteAnimation Load(ContentManager content, String name)
        {
            try
            {
                SpriteAnimation ret = content.Load<SpriteAnimation>(name);
                return ret;
            }
            catch (ContentLoadException)
            {
                Texture2D asset = content.Load<Texture2D>(name);
                return new SpriteAnimation(asset);
            }
        }
    }
}

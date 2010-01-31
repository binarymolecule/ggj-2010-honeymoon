using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Honeymoon
{
    public class Theme
    {
        public Song BackgroundMusic;
        public Texture2D Parallax;
        public Texture2D Background;
        public SpriteAnimationSwitcher Monkey;
        public SpriteAnimationSwitcher Panel;
        public SpriteAnimationSwitcher Coconut;
        public SpriteAnimationSwitcher Planet;
        public SpriteAnimationSwitcher Tree;
        public SoundEffect SoundCreateCoconut;
    }
}

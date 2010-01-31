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
        public Color TutorialColor;
        public SpriteAnimationSwitcher SunTutorial;
        public Song BackgroundMusic;
        public List<Texture2D> Parallax = new List<Texture2D>();
        public Texture2D Background;
        public SpriteAnimationSwitcher Monkey;
        public SpriteAnimationSwitcher Panel;
        public SpriteAnimationSwitcher Coconut;
        public SpriteAnimationSwitcher Planet;
        public SpriteAnimationSwitcher Tree;
        public SoundEffect SoundCreateCoconut;
        public SoundEffect SoundJump;
        public SoundEffect SoundStomp;
        public SoundEffect SoundExplode;
        public SoundEffect SoundMissile;
        public SoundEffect SoundCollide;
        public SpriteAnimationSwitcher Beleuchtung;
    }
}

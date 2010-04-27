using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KeplersDataTypes;
using KeplersLibrary;
using Honeymoon.Screens;
using Microsoft.Xna.Framework.Media;

namespace Honeymoon
{
    public class MusicSwitcher
    {
        Song currentlyPlaying;
        TimeSpan timeOfNextChange = TimeSpan.Zero;

        public void Update(GameTime gameTime, Song playMe)
        {
            if (currentlyPlaying != playMe)
            {
                MediaPlayer.Stop();
                currentlyPlaying = playMe;
            }

            if (MediaPlayer.State != MediaState.Playing && gameTime.TotalGameTime > timeOfNextChange)
            {
                timeOfNextChange = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(0.5));
                MediaPlayer.Play(currentlyPlaying);
            }
        }
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Honeymoon
{
    public class PlayerPanel : DrawableGameComponent
    {
        public Monkey Player;
        public HoneymoonGame GameHM;
        public Vector2 Position;
        public static Vector2 Offset = new Vector2(130.0f, 0.0f);

        public PlayerPanel(Monkey player)
            : base(HoneymoonGame.Instance)
        {
            this.GameHM = HoneymoonGame.Instance;
            this.Player = player;
            this.Position = Vector2.Zero;
            Game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public void DrawPanelFixed(GameTime gameTime)
        {
            GameHM.CurrentTheme.Panel.Draw(this, gameTime, "score_" + String.Format("{0:000}", Math.Max(0, Math.Min(5, 10 - Player.HitPoints))), 
                                           Position + GameHM.Camera.Inverse2DTranslation * 0.5f, Color.White, 0.0f, 1.0f);
            GameHM.CurrentTheme.Panel.Draw(this, gameTime, "score_" + String.Format("{0:000}", Math.Max(0, Math.Min(5, 5 - Player.HitPoints))),
                                           Position + GameHM.Camera.Inverse2DTranslation * 0.5f + Offset, Color.White, 0.0f, 1.0f);
        }
    }
}
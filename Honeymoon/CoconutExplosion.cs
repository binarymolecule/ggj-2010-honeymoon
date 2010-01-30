﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Honeymoon
{
    public class CoconutExplosion : CollidableGameComponent
    {
        public Texture2D Sprite;
        public Vector2 SpriteCenter;
        public float timer;
        public static float CoconutExplosionTime = 1.0f;

        public CoconutExplosion(Vector2 pos)
        {
            this.DrawOrder = 4;
            this.CollisionEnabled = true;
            this.CollisionRadius = 32;            
            this.Position = pos;
            this.timer = CoconutExplosionTime;
        }

        protected override void LoadContent()
        {
            Sprite = GameHM.Content.Load<Texture2D>("explosion");
            SpriteCenter = new Vector2(Sprite.Width, Sprite.Height) / 2.0f;            
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= seconds;
            if (timer <= 0.0f)
            {
                this.Dispose();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float alpha = Math.Max(0.0f, timer / CoconutExplosionTime);
            Color color = new Color(1.0f, 1.0f, 1.0f, alpha);
            GameHM.spriteBatch.Begin();
            GameHM.spriteBatch.Draw(Sprite, Position, null, color, 0.0f, SpriteCenter, 1.0f, SpriteEffects.None, 1);
            GameHM.spriteBatch.End();
        }
    }
}
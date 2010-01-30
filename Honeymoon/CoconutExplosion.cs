using System;
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
        public float timer;
        public float angle;
        public float scale;
        public static float CoconutExplosionTime = 1.0f;

        public CoconutExplosion(Vector2 pos, float angle, float scale, PlayerIndex PlayerNumber)
            : base(PlayerNumber)
        {
            this.DrawOrder = 4;
            this.CollisionEnabled = true;
            this.Position = pos;
            this.timer = CoconutExplosionTime;
            this.angle = angle;
            this.scale = scale;
            this.CollisionRadius = 32.0f * scale;
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
            GameHM.CurrentTheme.Coconut.Draw(this, gameTime, "explosion", Position, color, angle, scale);
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is Monkey)
            {
                CollisionEnabled = false; // remove from game's collidable object list    
            }
        }
    }
}
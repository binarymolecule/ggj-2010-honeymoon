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
        Monkey onMonkey;
        public static Random random = new Random();
        public float timer;
        public float scale;
        public static float CoconutExplosionTime = 1.0f;
        private bool disableCollisionInUpdate;

        public CoconutExplosion(Vector2 pos, PlayerIndex PlayerNumber, Monkey onMonkey, bool CauseDamage)
            : base(PlayerNumber)
        {
            this.onMonkey = onMonkey;
            this.DrawOrder = 4;
            this.CollisionEnabled = CauseDamage;
            this.Position = pos;
            this.timer = onMonkey == null ? 0 : -CoconutExplosionTime;
            this.scale = (float)random.NextDouble() * 0.1f + 1.0f;
            this.CollisionRadius = 100.0f * scale;
            GameHM.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer += seconds;
            if (timer >= CoconutExplosionTime)
            {
                this.Dispose();
            }
            if (disableCollisionInUpdate && CollisionEnabled)
            {
                CollisionEnabled = false; // remove from game's collidable object list    
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (timer > 0)
            {
                if (onMonkey != null)
                    Position = onMonkey.Position;
                GameHM.CurrentTheme.Coconut.DrawPercentage(this, "explosion", timer / CoconutExplosionTime, Position, Color.White, 0, scale);
            }
        }

        public override void OnCollide(CollidableGameComponent otherObject, Vector2 offsetMeToOther)
        {
            if (otherObject is Monkey)
            {
                disableCollisionInUpdate = true;
            }
        }
    }
}
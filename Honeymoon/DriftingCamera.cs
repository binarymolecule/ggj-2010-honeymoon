using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class DriftingCamera
    {
        public Planet Planet1, Planet2;
        public Vector2 WorldCenter;
        public Vector3 Translation;
        public static Vector2 MaxOffset = new Vector2(100, 50);

        public Matrix TransformMatrix { get { return Matrix.CreateTranslation(Translation); } }

        public DriftingCamera(Planet planet1, Planet planet2)
        {
            this.Translation = Vector3.Zero;
            this.Planet1 = planet1;
            this.Planet2 = planet2;
            this.WorldCenter = new Vector2(HoneymoonGame.Instance.GraphicsDevice.Viewport.Width/2,
                                           HoneymoonGame.Instance.GraphicsDevice.Viewport.Height/2);
        }

        public void Update(float seconds)
        {
            Vector2 planetCenter = 0.5f * (Planet1.Position + Planet2.Position);
            Vector2 difference = WorldCenter - planetCenter;
            difference *= (float)0.001f;
            Translation.X = Math.Min(MaxOffset.X, Math.Max(-MaxOffset.X, Translation.X + difference.X));
            Translation.Y = Math.Min(MaxOffset.Y, Math.Max(-MaxOffset.Y, Translation.Y + difference.Y));
        }
    }
}

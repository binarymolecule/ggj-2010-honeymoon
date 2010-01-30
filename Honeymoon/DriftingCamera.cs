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

        private Vector2 motionDirection;
        private float motionVelocity;
        private bool isMoving;
        public static float CameraMotionVelocity = 100.0f;
        public static float CameraMotionFriction = 0.96f;
        public bool IsMoving { get { return isMoving; } }

        private Vector2 backupTranslation;
        private float shakeTimer;
        private float shakeFactor;
        private float shakeAmplitude;
        private bool isShaking;
        public bool IsShaking { get { return isShaking; } }
        public static float CameraShakingTime = 1.0f;
        public static float CameraShakingFrequency = 5.0f;
        public static float CameraShakingAmplitude = 10.0f;
        public static float CameraVelocityFactor = 0.01f;

        public Matrix TransformMatrix { get { return Matrix.CreateTranslation(Translation); } }

        public DriftingCamera(Planet planet1, Planet planet2)
        {
            this.Translation = Vector3.Zero;
            this.Planet1 = planet1;
            this.Planet2 = planet2;
            this.WorldCenter = new Vector2(HoneymoonGame.Instance.GraphicsDevice.Viewport.Width/2,
                                           HoneymoonGame.Instance.GraphicsDevice.Viewport.Height/2);
        }

        public void ShakeCamera(float seconds, float frequency, float amplitude)
        {
            shakeFactor = 2.0f * (float)Math.PI * frequency;
            shakeAmplitude = amplitude;
            shakeTimer = seconds;
            isShaking = true;
            backupTranslation.X = Translation.X;
            backupTranslation.Y = Translation.Y;
        }

        public void MoveCamera(Vector2 dir, float velocity)
        {
            motionDirection = dir;
            motionDirection.Normalize();
            motionVelocity = velocity;
            isMoving = true;
        }

        public void Update(float seconds)
        {
            if (isShaking)
            {
                shakeTimer -= seconds;
                if (shakeTimer <= 0.0f)
                {
                    isShaking = false;
                    Translation.X = backupTranslation.X;
                    Translation.Y = backupTranslation.Y;
                }
                else
                {
                    float offsetX = (float)Math.Sin(shakeTimer * shakeFactor) * shakeAmplitude;
                    float offsetY = (float)Math.Sin(shakeTimer * shakeFactor + 0.25) * shakeAmplitude;
                    Translation.X = backupTranslation.X + offsetX;
                    Translation.Y = backupTranslation.Y + offsetY;
                }
            }
            else if (isMoving)
            {
                motionVelocity *= CameraMotionFriction;
                if (motionVelocity < 1.0f)
                {
                    motionVelocity = 0.0f;
                    isMoving = false;
                }
                else
                {
                    Vector2 mot = motionVelocity * seconds * motionDirection;
                    Translation.X = Math.Min(MaxOffset.X, Math.Max(-MaxOffset.X, Translation.X + mot.X));
                    Translation.Y = Math.Min(MaxOffset.Y, Math.Max(-MaxOffset.Y, Translation.Y + mot.Y));
                }
                /*
                Vector2 planetCenter = 0.5f * (Planet1.Position + Planet2.Position);
                Vector2 difference = WorldCenter - planetCenter;
                difference *= CameraVelocityFactor;
                Translation.X = Math.Min(MaxOffset.X, Math.Max(-MaxOffset.X, Translation.X + difference.X));
                Translation.Y = Math.Min(MaxOffset.Y, Math.Max(-MaxOffset.Y, Translation.Y + difference.Y));
                */
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Honeymoon
{
    public class DriftingCamera
    {
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
        public static float CameraShakingDampening = 0.95f;
        public static float CameraVelocityFactor = 0.01f;

        public Matrix TransformMatrix { get { return Matrix.CreateTranslation(Translation); } }

        public DriftingCamera()
        {
            this.Translation = Vector3.Zero;
        }

        public void ShakeCamera(float seconds, float frequency, float amplitude)
        {
            if (!isShaking)
            {
                shakeFactor = 2.0f * (float)Math.PI * frequency;
                shakeAmplitude = amplitude;
                shakeTimer = seconds;
                isShaking = true;
                backupTranslation.X = Translation.X;
                backupTranslation.Y = Translation.Y;
            }
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
                    shakeAmplitude *= CameraShakingDampening;
                    if (shakeAmplitude < 0.5f)
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
            }
        }
    }
}

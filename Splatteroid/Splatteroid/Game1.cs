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

namespace Splatteroid
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D myRenderTarget;
        Texture2D gradient, backdrop, noiseTexture;
        Random random = new Random();
        Effect blobEffect;
        ResolveTexture2D resolveBackbuffer;

        class ParticleData
        {
            public Vector2 Pos;
            public Vector2 Speed;
            public int Size;
        }

        const int MAX_PARTICLES = 200;
        ParticleData[] Particles = new ParticleData[MAX_PARTICLES];

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                ParticleData pd = new ParticleData();
                pd.Pos = new Vector2(0, (float)(40 + random.NextDouble() * 50));
                pd.Speed = new Vector2((float)(20 * random.NextDouble() + 10),
                      (float)(4 * random.NextDouble() - 2)
                    );
                pd.Size = (int)(30 + 80 * random.NextDouble());
                Particles[i] = pd;
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            myRenderTarget = new RenderTarget2D(this.GraphicsDevice,
                 this.GraphicsDevice.Viewport.Width,
                 this.GraphicsDevice.Viewport.Height, 1,
                 SurfaceFormat.Color, RenderTargetUsage.PreserveContents);

            gradient = Content.Load<Texture2D>("textures/gradient");
            backdrop = Content.Load<Texture2D>("textures/universe");
            noiseTexture = Content.Load<Texture2D>("textures/noise");

            resolveBackbuffer = new ResolveTexture2D(this.GraphicsDevice,
                 this.GraphicsDevice.Viewport.Width,
                 this.GraphicsDevice.Viewport.Height, 1,
                 SurfaceFormat.Color);

            blobEffect = Content.Load<Effect>("effects/blob");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            foreach (ParticleData p in Particles)
            {
                if (p == null) continue;

                p.Pos += (float)gameTime.ElapsedRealTime.TotalSeconds * p.Speed;

                if (p.Pos.X > 500)
                {
                    p.Pos.X = 0;
                }

                if (p.Pos.Y < 0 || p.Pos.Y > 150)
                {
                    p.Pos.Y = 40;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Shortcut
            GraphicsDevice device = GraphicsDevice;
            device.Clear(Color.CornflowerBlue);

            // Draw Scene
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            spriteBatch.Draw(backdrop, Vector2.Zero, Color.White);
            spriteBatch.End();

            // Get Backbuffer
            device.ResolveBackBuffer(resolveBackbuffer);

            // Draw Particles
            device.SetRenderTarget(0, myRenderTarget);
            device.Clear(Color.Black);
            DrawParticles();
            device.SetRenderTarget(0, null);

            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            blobEffect.CurrentTechnique = blobEffect.Techniques["HajoBlob"];
            blobEffect.Parameters["fTime0_X"].SetValue((float)gameTime.TotalRealTime.TotalSeconds * 0.1f);
            blobEffect.Parameters["fInverseViewportDimensions"].SetValue(PixelDimensions());
            blobEffect.Parameters["noise_Tex"].SetValue(noiseTexture);
            blobEffect.Parameters["blobs_Tex"].SetValue(myRenderTarget.GetTexture());
            blobEffect.Parameters["backdrop_Tex"].SetValue(resolveBackbuffer);

            //graphics.GraphicsDevice.Textures[1] = myRenderTarget.GetTexture();
            //graphics.GraphicsDevice.Textures[2] = null; // noise
            blobEffect.Begin(SaveStateMode.SaveState);
            blobEffect.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(resolveBackbuffer, FullscreenRectangle(), Color.White);
            
            // first end the spriteBatch to make the custom effect happen
            spriteBatch.End();
            blobEffect.CurrentTechnique.Passes[0].End();
            blobEffect.End();
            

            base.Draw(gameTime);
        }

        protected void DrawParticles()
        {
            spriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            foreach (ParticleData p in Particles)
            {
                if (p == null) continue;
                float lerp = Math.Min(1, Math.Max(p.Pos.X / 500, 0));
                spriteBatch.Draw(gradient,
                    new Rectangle((int)p.Pos.X,
                    (int)p.Pos.Y, p.Size, p.Size),
                    Color.Lerp(Color.White, Color.Black, lerp));
            }
            spriteBatch.End();
        }

        Rectangle FullscreenRectangle()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;

            return new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        Vector2 PixelDimensions()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            return new Vector2(1.0f / viewport.Width, 1.0f / viewport.Height);
        }
    }
}

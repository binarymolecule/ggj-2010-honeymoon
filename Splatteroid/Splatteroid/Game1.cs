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
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;

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
        PhysicsSimulator physicsSimulator = new PhysicsSimulator();

        class ParticleData
        {
            public Body body;
            public Geom geometry;
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
                 pd.Size = (int)(30 + 80 * random.NextDouble());
                 pd.body = BodyFactory.Instance.CreateCircleBody(physicsSimulator, pd.Size/2, 1);
                 pd.geometry = GeomFactory.Instance.CreateCircleGeom(pd.body, pd.Size / 3, 16);
                 pd.body.Position = new Vector2((float)random.NextDouble() * 500, (float)random.NextDouble() * 500);
                 Particles[i] = pd;
                 physicsSimulator.Add(pd.geometry);
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

            Particles[0].body.LinearVelocity = (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left*300.1f);
            Particles[0].body.LinearVelocity.Y = -Particles[0].body.LinearVelocity.Y;
            physicsSimulator.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

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
            blobEffect.Parameters["fTime0_X"].SetValue((float)gameTime.TotalRealTime.TotalSeconds);
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
                spriteBatch.Draw(gradient,
                    new Rectangle((int)p.body.Position.X - p.Size,
                    (int)p.body.Position.Y - p.Size, p.Size, p.Size),
                    Color.White);
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

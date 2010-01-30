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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class HoneymoonGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public List<CollidableGameComponent> collidableObjects = new List<CollidableGameComponent>(); 

        public static HoneymoonGame Instance;

        public HoneymoonGame()
        {
            Instance = this;
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
            Planet prop = new Planet();
            Components.Add(prop);
            Planet prop2 = new Planet();
            Components.Add(prop2);
            prop.Position = new Vector2(100, 100);
            prop2.Position = new Vector2(500, 120);
            prop.Velocity = new Vector2(100, 0);
            prop2.Velocity = new Vector2(-100, 0);

            Monkey monkey1 = new Monkey(prop);
            Components.Add(monkey1);

            Tree tree1 = new Tree(prop);
            Components.Add(tree1);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            { 
                this.Exit();
            }

            CollidableGameComponent[] collide = collidableObjects.ToArray();
            for (int i = 0; i < collide.Length; i++)
            {
                CollidableGameComponent A = collide[i];
                if (!A.CollisionEnabled) continue;
                for (int j = i+1; j < collide.Length; j++)
                {
                    CollidableGameComponent B = collide[j];
                    if (!B.CollisionEnabled) continue;

                    Vector2 aToB = B.Position - A.Position;
                    if(A.CollisionRadiusSq+B.CollisionRadiusSq < aToB.LengthSquared()) continue;

                    A.OnCollide(B, aToB);
                    B.OnCollide(A, -aToB);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

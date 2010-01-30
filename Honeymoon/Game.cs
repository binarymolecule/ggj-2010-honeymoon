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
        public Vector2 SunlightDir; // direction of sunlight
        public static HoneymoonGame Instance;
        public Random Randomizer;
        public Theme[] Themes = new Theme[2];
        public Theme CurrentTheme = null;

        public HoneymoonGame()
        {
            Instance = this;
#if(DEBUG)
            Randomizer = new Random();
#else
            Randomizer = new Random(); // seed?
#endif
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;

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
            SunlightDir = new Vector2(0.0f, -1.0f);

            Planet prop = new Planet(PlayerIndex.One);
            prop.Position = new Vector2(200, 400);
            Monkey monkey1 = new Monkey(prop, PlayerIndex.One);

            Planet prop2 = new Planet(PlayerIndex.Two);
            prop2.Position = new Vector2(1000, 400);
            Monkey monkey2 = new Monkey(prop2, PlayerIndex.Two);

            PlayerPanel panel1 = new PlayerPanel(monkey1);
            PlayerPanel panel2 = new PlayerPanel(monkey2);
            panel1.Position = new Vector2(125, 80);
            panel2.Position = new Vector2(GraphicsDevice.Viewport.Width - 375, 80);
            
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


            for (int i = 0; i < Themes.Length; i++)
            {
                String type = (i == 0 ? "good" : "evil");

                Themes[i] = new Theme
                {
                    Background = Content.Load<Texture2D>("Textures/Backgrounds/" + type),
                    Monkey = new SpriteAnimationSwitcher("monkey_" + type, new String[] { "left", "right", "crash", "penalty" }),
                    Panel = new SpriteAnimationSwitcher("score_" + type, new String[] { "score_000", "score_001", "score_002", "score_003", "score_004", "score_005" }),
                    Coconut = new SpriteAnimationSwitcher(type, new String[] { "coconut", "explosion" }),
                    Planet = new SpriteAnimationSwitcher(type, new String[] { "planet" }),
                    Tree = new SpriteAnimationSwitcher("palme_" + type, new String[] { "palme" })
                };
            }

            CurrentTheme = Themes[0];
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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
                for (int j = i + 1; j < collide.Length; j++)
                {
                    CollidableGameComponent B = collide[j];
                    if (!B.CollisionEnabled) continue;

                    Vector2 aToB = B.Position - A.Position;
                    float r = A.CollisionRadius + B.CollisionRadius;
                    if (r * r < aToB.LengthSquared()) continue;

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
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(CurrentTheme.Background, Vector2.Zero, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

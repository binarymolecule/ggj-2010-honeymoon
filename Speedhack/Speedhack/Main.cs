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
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Controllers;

namespace Speedhack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PhysicsSimulator physicsSimulator;
        GravityController gravityPlanets;
        GravityController gravitySun;
        List<Planet> planets;
        List<Body> planetBodies;
        List<Body> sunBody;
        Planet sun;
        Body player;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            physicsSimulator = new PhysicsSimulator();            
            planets = new List<Planet>();
            planetBodies = new List<Body>();
            sunBody = new List<Body>();
            gravityPlanets = new GravityController(physicsSimulator, planetBodies, 5000.0f, 500.0f);
            gravityPlanets.GravityType = GravityType.Linear;
            gravitySun = new GravityController(physicsSimulator, sunBody, 80000.0f, 10000.0f);
            gravitySun.GravityType = GravityType.Linear;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            planets.Add(new Planet(physicsSimulator, Content.Load<Texture2D>("graphics/planet"),
                                   new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                                               graphics.GraphicsDevice.Viewport.Height / 2 - 200),
                                               45.0f, 1.0f, 50.0f, true));
            planets.Add(new Planet(physicsSimulator, Content.Load<Texture2D>("graphics/planet"),
                                   new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 100,
                                               graphics.GraphicsDevice.Viewport.Height / 2),
                                               45.0f, 1.0f, 30.0f, true));
            planets.Add(new Planet(physicsSimulator, Content.Load<Texture2D>("graphics/asteroid"),
                                   new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - 100,
                                               50.0f), 25.0f, 0.1f, 0.1f, false));
            sun = new Planet(physicsSimulator, Content.Load<Texture2D>("graphics/sun"),
                             new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                                         graphics.GraphicsDevice.Viewport.Height / 2),
                                         50.0f, 1000.0f, 0.0f, true);
            sunBody.Add(sun.body);
            sun.body.IgnoreGravity = true;

            int numPlanets = planets.Count;
            for (int i = 0; i < numPlanets; i++)
            {
                physicsSimulator.Add(planets[i].geometry);
                if (planets[i].hasGravity)
                    planetBodies.Add(planets[i].body);
            }
            physicsSimulator.Add(sun.geometry);

            player = sun.body;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            float ms = gameTime.ElapsedGameTime.Milliseconds;
            float sec = 0.001f * ms;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            player.Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            /*
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                player.ApplyForce(new Vector2(-500000.0f, 0.0f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                player.ApplyForce(new Vector2(500000.0f, 0.0f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                player.ApplyForce(new Vector2(0.0f, -500000.0f));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                player.ApplyForce(new Vector2(0.0f, 500000.0f));
            }
            */

            sun.Update(ms);
            int numPlanets = planets.Count;
            for (int i = 0; i < numPlanets; i++)
            {
                Vector2 diff = planets[i].body.Position - sun.body.Position;
                diff.Normalize();
                planets[i].body.ApplyForce(new Vector2(-diff.Y, diff.X) * 10.0f);
                planets[i].Update(ms);
            }
            
            physicsSimulator.Update(sec);
            gravityPlanets.Update(sec, sec);
            gravitySun.Update(sec, sec);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            sun.Draw(spriteBatch);
            int numPlanets = planets.Count;
            for (int i = 0; i < numPlanets; i++)
            {
                planets[i].Draw(spriteBatch);
            }            

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

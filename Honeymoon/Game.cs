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
        public enum GameStates { Intro, Game, GameOver };
        public GameStates GameState;

        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public List<CollidableGameComponent> collidableObjects = new List<CollidableGameComponent>();
        public Vector2 SunlightDir; // direction of sunlight
        public static HoneymoonGame Instance;
        public Random Randomizer;
        public Theme[] Themes = new Theme[2];
        public PlayerPanel PlayerPanel1, PlayerPanel2;
        public DriftingCamera Camera;
        float gameOverCounter = 0.0f;
        float twitchValue = 0.5f;
        int changeTwitchValueGameOver = 0; // -1 to decrease, +1 to increase, 0 for no effect
        float themeTransition = 0.0f;
        int targetTheme = 0;
        public int CurrentThemeID { get { return themeTransition > 0.5f ? 1 : 0; } }
        public Intro IntroController;

        public SoundEffect SelectionSound;
        public SoundEffect WalkingSound;
        public SoundEffectInstance NoiseSound;
        public Song GameOverMusic;

        public Theme CurrentTheme
        {
            get {
                return Themes[themeTransition > 0.5f ? 1 : 0];
            }
            set {
                targetTheme = (Themes[0] == value) ? 0 : 1;
            }
        }

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
            Planet prop1 = new Planet(PlayerIndex.One);
            prop1.Position = new Vector2(200, 400);
            Monkey monkey1 = new Monkey(prop1);

            Planet prop2 = new Planet(PlayerIndex.Two);
            prop2.Position = new Vector2(1000, 400);
            Monkey monkey2 = new Monkey(prop2);

            PlayerPanel1 = new PlayerPanel(monkey1);
            PlayerPanel2 = new PlayerPanel(monkey2);
            PlayerPanel1.Position = new Vector2(125, 80);
            PlayerPanel2.Position = new Vector2(GraphicsDevice.Viewport.Width - 375, 80);

            SunlightDir = new Vector2(0.0f, -1.0f);
            Camera = new DriftingCamera();            

            IntroController = new Intro();
            GameState = GameStates.Intro;

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

            resolvedBackbuffer = new ResolveTexture2D(GraphicsDevice,
                1280, 720,
                1, GraphicsDevice.DisplayMode.Format);

            twitchNoise = Content.Load<Texture2D>("Textures/Helpers/twitch_noise");
            twitchEffect = Content.Load<Effect>("Effects/twitch");
            twitchRenderTarget = new RenderTarget2D(GraphicsDevice, 128, 128, 1, GraphicsDevice.DisplayMode.Format, RenderTargetUsage.PreserveContents);

            SelectionSound = Content.Load<SoundEffect>("Sounds/select");
            WalkingSound = Content.Load<SoundEffect>("Sounds/footsteps");
            NoiseSound = Content.Load<SoundEffect>("Sounds/noise0").CreateInstance();
            NoiseSound.IsLooped = true;
            GameOverMusic = Content.Load<Song>("Music/gameover");

            IntroController.Screen = Content.Load<Texture2D>("Textures/Backgrounds/title");

            for (int i = 0; i < Themes.Length; i++)
            {
                String type = (i == 0 ? "good" : "evil");

                Themes[i] = new Theme
                {
                    Background = Content.Load<Texture2D>("Textures/Backgrounds/" + type),
                    Parallax = { 
                        Content.Load<Texture2D>("Textures/Backgrounds/stars1"),
                        Content.Load<Texture2D>("Textures/Backgrounds/stars2"),
                        Content.Load<Texture2D>("Textures/Backgrounds/stars3")
                    },
                    Monkey = new SpriteAnimationSwitcher("monkey_" + type, new String[] { "left", "right", "crash", "penalty" }),
                    Panel = new SpriteAnimationSwitcher("score_" + type, new String[] { "score_000", "score_001", "score_002", "score_003", "score_004", "score_005" }),
                    Coconut = new SpriteAnimationSwitcher(type, new String[] { "coconut", "explosion" }),
                    Planet = new SpriteAnimationSwitcher(type, new String[] { "planet", "highlightandshadow" }),
                    Tree = new SpriteAnimationSwitcher("palme_" + type, new String[] { "palme" }),
                    SunTutorial = new SpriteAnimationSwitcher("SunTutorial", new String[] { "sun" }),
                    TutorialColor = (i == 0) ? Color.White : Color.Red,
                    BackgroundMusic = Content.Load<Song>("Music/space"),
                    SoundCreateCoconut = Content.Load<SoundEffect>(i == 0 ? "Sounds/plop" : "Sounds/missile"),
                    SoundJump = Content.Load<SoundEffect>("Sounds/jump"),
                    SoundStomp = Content.Load<SoundEffect>("Sounds/stomp"),
                    SoundExplode = Content.Load<SoundEffect>(i == 0 ? "Sounds/heart" : "Sounds/explosion"),
                    SoundMissile = Content.Load<SoundEffect>(i == 0 ? "Sounds/plop" : "Sounds/missile"),
                    SoundCollide = Content.Load<SoundEffect>("Sounds/collide_" + type),
                    Beleuchtung = new SpriteAnimationSwitcher("beleuchtung_" + type, new String[] { "beleuchtung" }),
                };

                Themes[i].Planet.Animations["planet"].AnimationFPS = 10.0f;
                Themes[i].Beleuchtung.Animations["beleuchtung"].AnimationFPS = 10.0f;
                Themes[i].Monkey.Animations["left"].AnimationFPS = 30.0f;
                Themes[i].Monkey.Animations["right"].AnimationFPS = 30.0f;
                Themes[i].SunTutorial.Animations["sun"].AnimationFPS = 6.0f;

                if (i == 1)
                {
                    Themes[i].Parallax.Insert(2, Content.Load<Texture2D>("Textures/Backgrounds/skull"));
                }
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

            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Change to game over state some time after one player died
            if (gameOverCounter > 0.0f)
            {
                MediaPlayer.Volume = gameOverCounter;
                gameOverCounter -= seconds;
                if (gameOverCounter <= 0.0f)
                {
                    GameState = HoneymoonGame.GameStates.GameOver;
                    MediaPlayer.Stop();
                    MediaPlayer.Volume = 1.0f;
                }
            }            

            float worldTransitionDiff = seconds * 3f;

            if (targetTheme < themeTransition)
            {
                themeTransition = (float)Math.Max(themeTransition - worldTransitionDiff, targetTheme);
                twitchValue = themeTransition + 0.5f;
                if (GameState == GameStates.Game)
                {
                    MediaPlayer.Volume = 1.0f - 0.75f * themeTransition;
                    NoiseSound.Volume = 0.67f * themeTransition;
                    if (NoiseSound.State == SoundState.Paused)
                        NoiseSound.Resume();
                    else if (NoiseSound.State == SoundState.Stopped)
                        NoiseSound.Play();
                }
            }
            else if(targetTheme > themeTransition)
            {
                themeTransition = (float)Math.Min(themeTransition + worldTransitionDiff, targetTheme);
                twitchValue = themeTransition + 0.5f;
                if (GameState == GameStates.Game)
                {
                    MediaPlayer.Volume = 1.0f - 0.75f * themeTransition;
                    NoiseSound.Volume = 0.67f * themeTransition;
                    if (NoiseSound.State == SoundState.Paused)
                        NoiseSound.Resume();
                    else if (NoiseSound.State == SoundState.Stopped)
                        NoiseSound.Play();
                }
            }
            else if (GameState == GameStates.GameOver)
            {
                if (changeTwitchValueGameOver == 0 && Randomizer.NextDouble() < 0.01f)
                {
                    changeTwitchValueGameOver = (twitchValue > 1.0f ? -1 : 1);
                }
                if (changeTwitchValueGameOver != 0)
                {
                    twitchValue += changeTwitchValueGameOver * worldTransitionDiff;
                    if (twitchValue <= 0.5f)
                    {
                        twitchValue = 0.5f;
                        changeTwitchValueGameOver = 0;
                    }
                    else if (twitchValue >= 1.5f)
                    {
                        twitchValue = 1.5f;
                        changeTwitchValueGameOver = 0;
                    }
                }
            }
            else if (themeTransition == 0.0f && NoiseSound.State == SoundState.Playing)
            {
                NoiseSound.Pause();
            }

            if (GameState == GameStates.Game)
            {
                // Check for collisions
                CollidableGameComponent[] collide = collidableObjects.ToArray();
                for (int i = 0; i < collide.Length; i++)
                {
                    CollidableGameComponent A = collide[i];
                    for (int j = i + 1; j < collide.Length; j++)
                    {
                        CollidableGameComponent B = collide[j];
                        Vector2 aToB = B.Position - A.Position;
                        float r = A.CollisionRadius + B.CollisionRadius;
                        if (r * r < aToB.LengthSquared()) continue;

                        A.OnCollide(B, aToB);
                        B.OnCollide(A, -aToB);
                    }
                }
            }

            // Update camera matrix
            Camera.Update(seconds);

            if (GameState == GameStates.Game)
                base.Update(gameTime);
            else if (GameState == GameStates.Intro)
                IntroController.Update(gameTime);
            if (MediaPlayer.State != MediaState.Playing)
            {
                if (GameState == GameStates.GameOver)
                    MediaPlayer.Play(GameOverMusic);
                else
                    MediaPlayer.Play(CurrentTheme.BackgroundMusic);
            }
        }

        public void spriteBatchStart()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, Camera.TransformMatrix);
        }

        public void spriteBatchAdditiveStart()
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState, Camera.TransformMatrix);
            GraphicsDevice.RenderState.SourceBlend = Blend.DestinationColor;
            GraphicsDevice.RenderState.DestinationBlend = Blend.SourceColor;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatchStart();
            Vector2 camTranslation = new Vector2(-Camera.Translation.X, -Camera.Translation.Y);
            spriteBatch.Draw(CurrentTheme.Background, camTranslation * 0.9f, Color.White);

            float distance = 0.8f;
            foreach (Texture2D t2d in CurrentTheme.Parallax)
            {
                spriteBatch.Draw(t2d, camTranslation * distance, Color.White);
                distance -= 0.1f;
            }

            if (GameState == GameStates.Intro)
                IntroController.Draw(gameTime);
            else
                base.Draw(gameTime);
            spriteBatch.End();

            if (GameState != GameStates.Intro)
            {
                spriteBatch.Begin();
                CurrentTheme.Beleuchtung.Draw(this, gameTime, "beleuchtung", ScreenCenter, Color.White, 0, 2);
                PlayerPanel1.DrawPanelFixed(gameTime);
                PlayerPanel2.DrawPanelFixed(gameTime);
                CurrentTheme.SunTutorial.Draw(this, gameTime, "sun", ScreenCenter, CurrentTheme.TutorialColor, 0, 1);
                spriteBatch.End();
            }

            PerformTwitchEffect(gameTime);
        }

        ResolveTexture2D resolvedBackbuffer;
        Effect twitchEffect;
        RenderTarget2D twitchRenderTarget;
        Texture2D twitchNoise;

        Vector2 ScreenCenter = new Vector2(1280 / 2, 720 / 2);

        private void PerformTwitchEffect(GameTime gameTime)
        {
            if (GameState != GameStates.GameOver &&
                (themeTransition <= 0 || themeTransition >= 1))
            {
                return;
            }

            float fTime0_X = (float)gameTime.TotalRealTime.TotalSeconds;
            twitchEffect.Parameters["fTime0_X"].SetValue(twitchValue);
            twitchEffect.Parameters["Screen_AlignedQuad_AfterTwitch_Pixel_Shader_fTime0_X"].SetValue(fTime0_X);
            twitchEffect.Parameters["scene_Tex"].SetValue(resolvedBackbuffer);
            twitchEffect.Parameters["noise_tex_Tex"].SetValue(twitchNoise);

            EffectTechnique t = twitchEffect.Techniques["Screen_AlignedQuad"];
            GraphicsDevice.ResolveBackBuffer(resolvedBackbuffer);            

            // Pass 1
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            twitchEffect.Begin(SaveStateMode.SaveState);
            t.Passes["Twitch"].Begin();
            spriteBatch.Draw(resolvedBackbuffer, FullscreenRectangle(), Color.White);
            spriteBatch.End();
            t.Passes["Twitch"].End();
            twitchEffect.End();

            // Pass 2
            GraphicsDevice.ResolveBackBuffer(resolvedBackbuffer);
            RenderTarget2D tempTarget = twitchRenderTarget;
            GraphicsDevice.SetRenderTarget(0, tempTarget);

            twitchEffect.Parameters["p0_result_Tex"].SetValue(resolvedBackbuffer);

            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            twitchEffect.Begin(SaveStateMode.SaveState);
            t.Passes["ScaleDown"].Begin();
            spriteBatch.Draw(resolvedBackbuffer, new Rectangle(0, 0, 128, 128), Color.White);
            spriteBatch.End();            
            t.Passes["ScaleDown"].End();
            twitchEffect.End();

            GraphicsDevice.SetRenderTarget(0, null);

            // Pass 3

            twitchEffect.Parameters["p0_blurry_Tex"].SetValue(tempTarget.GetTexture());

            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            twitchEffect.Begin(SaveStateMode.SaveState);
            t.Passes["AfterTwitch"].Begin();
            spriteBatch.Draw(resolvedBackbuffer, FullscreenRectangle(), Color.White);
            spriteBatch.End();
            t.Passes["AfterTwitch"].End();
            twitchEffect.End();
        }

        Rectangle FullscreenRectangle()
        {
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            return new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        public void GameOver()
        {
            // Switch to evil mode if not already
            if (CurrentThemeID != 1)
            {
                CurrentTheme = Themes[1];
            }

            // Start game over counter (game will not end immediately)
            gameOverCounter = 1.0f;            
        }
    }
}

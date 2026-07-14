using Final_Project;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project
{
    public enum GameState
    {
        MainMenu,
        Playing
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameState currentGameState = GameState.MainMenu;
        private MainMenu mainMenu;

        private Matrix gameCameraPosition;
        private Texture2D backgroundTexture;
        private int scaledBackgroundWidth;
        private Texture2D groundTexture;
        private Rectangle ground;
        private Player player;

        private Texture2D spikeTexture;
        private System.Collections.Generic.List<Rectangle> spikes = new System.Collections.Generic.List<Rectangle>();

        private Texture2D oreganoTexture;
        private System.Collections.Generic.List<Rectangle> oreganoItems = new System.Collections.Generic.List<Rectangle>();
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            gameCameraPosition = Matrix.Identity;
            mainMenu = new MainMenu();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainMenu.LoadContent(Content);

            backgroundTexture = Content.Load<Texture2D>("background");
            groundTexture = Content.Load<Texture2D>("ground");

            scaledBackgroundWidth = backgroundTexture.Width * 2;

            ground = new Rectangle(
                0,
                600,
                scaledBackgroundWidth,
                groundTexture.Height);

            player = new Player(new Vector2(150, 600 - 50));
            player.LoadContent(Content);

            spikeTexture = Content.Load<Texture2D>("spike");
            //SPIKE PLACEMENTS
            spikes.Add(new Rectangle(400, 600 - spikeTexture.Height, spikeTexture.Width, spikeTexture.Height));
            spikes.Add(new Rectangle(650, 600 - spikeTexture.Height, spikeTexture.Width, spikeTexture.Height));

            oreganoTexture = Content.Load<Texture2D>("oregano"); 
            // OREGANO PLACEMENTS
            oreganoItems.Add(new Rectangle(500, 550, oreganoTexture.Width, oreganoTexture.Height));
            oreganoItems.Add(new Rectangle(800, 550, oreganoTexture.Width, oreganoTexture.Height));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (currentGameState)
            {
                case GameState.MainMenu:
                    bool shouldExit = false;
                    if (mainMenu.Update(gameTime, ref shouldExit))
                    {
                        currentGameState = GameState.Playing;
                    }
                    if (shouldExit)
                        Exit();
                    break;

                case GameState.Playing:
                    player.Update(gameTime, ground, spikes, oreganoItems);

                    float targetCameraX = (Window.ClientBounds.Width / 2f) - player.Position.X;
                    float minCameraX = -(scaledBackgroundWidth - Window.ClientBounds.Width);
                    float maxCameraX = 0f;
                    float clampedCameraX = MathHelper.Clamp(targetCameraX, minCameraX, maxCameraX);

                    gameCameraPosition = Matrix.CreateTranslation(clampedCameraX, 0f, 0f);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (currentGameState == GameState.MainMenu)
            {
                _spriteBatch.Begin();
                mainMenu.Draw(_spriteBatch, backgroundTexture);
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.Playing)
            {
                // moving cam view
                _spriteBatch.Begin(SpriteSortMode.Deferred, 
                    null, 
                    null, 
                    null, 
                    null, 
                    null, 
                    gameCameraPosition);

                _spriteBatch.Draw(
                    backgroundTexture,
                    new Rectangle(0, 0, scaledBackgroundWidth, Window.ClientBounds.Height),
                    Color.White);

                // TILE GROUND DRAW
                for (int x = 0; x < scaledBackgroundWidth; x += groundTexture.Width)
                {
                    _spriteBatch.Draw(
                        groundTexture,
                        new Vector2(x, ground.Y),
                        Color.White);
                }

                // draw spikes
                foreach (Rectangle spikeRect in spikes)
                {
                    _spriteBatch.Draw(spikeTexture, spikeRect, Color.White);
                }

                foreach (Rectangle oreganoRect in oreganoItems)
                {
                    _spriteBatch.Draw(oreganoTexture, oreganoRect, Color.White);
                }

                //draw player elements inside the moving world space
                player.Draw(_spriteBatch);
                _spriteBatch.End();

                _spriteBatch.Begin();
                player.DrawUI(_spriteBatch);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}

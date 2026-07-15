using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace FINAL_PROJECT_GD
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

        // === YOUR CONTRIBUTION: GAME STATE MANAGEMENT ===
        // You set up the main state controller using an enum to separate the Main Menu and Gameplay loops.
        GameState currentGameState = GameState.MainMenu;[cite: 2]
        MainMenu mainMenu;[cite: 2]
        Level[] levels = new Level[9];[cite: 2]
        int currentLevel = 0;[cite: 2]
        Player player;[cite: 2]

        // === YOUR CONTRIBUTION: MULTI-CHARACTER HEALTH SYSTEM ===
        // You initialized the custom HealthSystem which monitors the life and dead status of your characters.
        HealthSystem healthSystem;[cite: 2]
        List<Zombie> zombies = new List<Zombie>();[cite: 2]
        Texture2D zombieTexture, oreganoTexture;[cite: 2]
        Texture2D backgroundTexture;[cite: 2]
        bool levelCleared = false;[cite: 2]
        StageOverlay stageOverlay;[cite: 2]
        int currentSaveSlot = 1;[cite: 2]

        // === YOUR CONTRIBUTION: INTERACTIVE BACK BUTTON ===
        // You declared the coordinates, text, and hover-state logic for an in-game back button.
        private SpriteFont menuFont;[cite: 2]
        private Rectangle backButtonRect;[cite: 2]
        private string backButtonText = "<";[cite: 2]
        private bool isHoveringBackButton = false;[cite: 2]

        // === YOUR CONTRIBUTION: DYNAMIC BGM SYSTEM ===
        // You declared the track assets to handle stage-specific audio transitions.
        private Song menuBgm;[cite: 2]
        private Song stage1Bgm;[cite: 2]
        private Song stage2Bgm;[cite: 2] 
        private Song stage3_1_3_2Bgm;[cite: 2]
        private Song stage3_3Bgm;[cite: 2]
        private Song currentBgm;[cite: 2]

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
            mainMenu = new MainMenu(this);[cite: 2]
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainMenu.LoadContent(Content);[cite: 2]
            backgroundTexture = Content.Load<Texture2D>("background");[cite: 2]
            stageOverlay = new StageOverlay(GraphicsDevice, Content.Load<SpriteFont>("MainMenu/MenuFont"));[cite: 2]
            menuFont = Content.Load<SpriteFont>("MainMenu/MenuFont");[cite: 2]

            // === YOUR CONTRIBUTION: INTERACTIVE BACK BUTTON CONFIGURATION ===
            // Placing the back button in the top right corner of the window.
            backButtonRect = new Rectangle(1220, 20, 40, 40);[cite: 2]
            
            zombieTexture = Content.Load<Texture2D>("spritesheets/Zombie");[cite: 2]
            
            // === YOUR CONTRIBUTION: ASSET INPUTS ===
            // Loading your custom heart (UI) and oregano (healing) textures.
            oreganoTexture = Content.Load<Texture2D>("assets/oregano");[cite: 2]
            healthSystem = new HealthSystem(Content.Load<Texture2D>("assets/heart"));[cite: 2]

            // === YOUR CONTRIBUTION: BGM SONG LOADING ===
            menuBgm = Content.Load<Song>("menubg");[cite: 2]
            stage1Bgm = Content.Load<Song>("stage1bg");[cite: 2]
            stage2Bgm = Content.Load<Song>("stage2bg");[cite: 2]
            stage3_1_3_2Bgm = Content.Load<Song>("stage3.1_3.2bg");[cite: 2]
            stage3_3Bgm = Content.Load<Song>("stage3.3bg");[cite: 2]
            MediaPlayer.IsRepeating = true;[cite: 2]

            #region STAGE 1-1
            levels[0] = new Level(Content.Load<Texture2D>("Stage1/Stage 1 - 1"));[cite: 2]
            levels[0].PlayerSpawnPoint = new Vector2(64, 576);[cite: 2]
            levels[0].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[0].Colliders.Add(new Rectangle(1280, 0, 32, 720)); [cite: 2]
            levels[0].Colliders.Add(new Rectangle(0, 640, 1280, 32));[cite: 2]
            levels[0].Colliders.Add(new Rectangle(384, 576, 160, 64));[cite: 2]
            levels[0].Colliders.Add(new Rectangle(448, 512, 96, 64));[cite: 2]
            levels[0].Colliders.Add(new Rectangle(512, 416, 32, 96));[cite: 2]
            levels[0].Exit = new Rectangle(1248, 512, 32, 128);[cite: 2]
            levels[0].EnemySpawnPoint.Add(new Vector2(832, 576));[cite: 2]
            levels[0].EnemySpawnPoint.Add(new Vector2(1088, 576));[cite: 2]
            #endregion

            #region STAGE 1-2
            levels[1] = new Level(Content.Load<Texture2D>("Stage1/Stage 1 - 2"));[cite: 2]
            levels[1].PlayerSpawnPoint = new Vector2(64, 576);[cite: 2]
            levels[1].Exit = new Rectangle(1248, 128, 32, 96);[cite: 2]
            levels[1].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(0, 640, 1280, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(320, 608, 960, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(992, 512, 64, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(448, 480, 544, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(1152, 480, 160, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(544, 384, 64, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(608, 352, 672, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(992, 256, 32, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(640, 224, 352, 32));[cite: 2]
            levels[1].Colliders.Add(new Rectangle(1120, 224, 160, 32));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(608, 544));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(1088, 544));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(480, 416));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(704, 416));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(800, 288));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(704, 160));[cite: 2]
            levels[1].EnemySpawnPoint.Add(new Vector2(1152, 160));[cite: 2]
            levels[1].Oregano.Add(new Rectangle(1152, 288, 64, 64));[cite: 2]
            #endregion

            #region STAGE 1-3
            levels[2] = new Level(Content.Load<Texture2D>("Stage1/Stage 1 - 3"));[cite: 2]
            levels[2].PlayerSpawnPoint = new Vector2(32, 224);[cite: 2]
            levels[2].Exit = new Rectangle(1056, 660, 96, 32);[cite: 2]
            levels[2].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(0, 640, 1056, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(0, 672, 1280, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(1152, 640, 128, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(0, 608, 1024, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(1184, 608, 96, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(0, 288, 160, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(192, 384, 96, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(320, 480, 96, 32));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(544, 512, 96, 96));[cite: 2]
            levels[2].Colliders.Add(new Rectangle(0, 320, 32, 288));[cite: 2]
            levels[2].EnemySpawnPoint.Add(new Vector2(32, 544));[cite: 2]
            levels[2].EnemySpawnPoint.Add(new Vector2(128, 544));[cite: 2]
            levels[2].EnemySpawnPoint.Add(new Vector2(800, 544));[cite: 2]
            levels[2].EnemySpawnPoint.Add(new Vector2(928, 544));[cite: 2]
            levels[2].EnemySpawnPoint.Add(new Vector2(960, 544));[cite: 2]
            levels[2].Oregano.Add(new Rectangle(64, 544, 64, 64));[cite: 2]
            #endregion

            #region STAGE 2-1
            levels[3] = new Level(Content.Load<Texture2D>("Stage2/Stage 2 -1"));[cite: 2]
            levels[3].PlayerSpawnPoint = new Vector2(32, 128);[cite: 2]
            levels[3].Exit = new Rectangle(1248, 480, 32, 128);[cite: 2]
            levels[3].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(0, 192, 416, 64));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(0, 256, 832, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(608, 224, 224, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(980, 224, 288, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(480, 160, 64, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(896, 320, 64, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(192, 416, 672, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(992, 416, 288, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(192, 448, 1088, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(64, 512, 64, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(0, 608, 320, 64));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(0, 672, 1280, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(704, 608, 128, 32));[cite: 2]
            levels[3].Colliders.Add(new Rectangle(1152, 608, 128, 32));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(704, 160));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(224, 352));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(320, 352));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(416, 352));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(1056, 352));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(448, 608));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(992, 608));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(1056, 160));[cite: 2]
            levels[3].EnemySpawnPoint.Add(new Vector2(160, 544));[cite: 2]
            levels[3].Spikes.Add(new Rectangle(416, 224, 192, 32));[cite: 2]
            levels[3].Spikes.Add(new Rectangle(864, 416, 128, 32));[cite: 2]
            levels[3].Oregano.Add(new Rectangle(1216, 192, 64, 64));[cite: 2]
            levels[3].Oregano.Add(new Rectangle(736, 576, 64, 64));[cite: 2]
            #endregion

            #region STAGE 2-2
            levels[4] = new Level(Content.Load<Texture2D>("Stage2/Stage 2 -2"));[cite: 2]
            levels[4].PlayerSpawnPoint = new Vector2(32, 544);[cite: 2]
            levels[4].Exit = new Rectangle(1248, 32, 32, 128);[cite: 2]
            levels[4].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(0, 608, 192, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(192, 640, 352, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(256, 512, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(416, 416, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(0, 320, 352, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(32, 224, 64, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(160, 160, 64, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(320, 128, 64, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(448, 128, 128, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(480, 160, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(512, 192, 64, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(544, 224, 32, 448));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(672, 0, 32, 544));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(576, 672, 672, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(704, 512, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(1248, 192, 32, 480));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(1088, 544, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(928, 480, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(800, 384, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(704, 288, 64, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(832, 192, 96, 32));[cite: 2]
            levels[4].Colliders.Add(new Rectangle(1024, 160, 256, 32));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(96, 256));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(480, 64));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(736, 608));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(864, 608));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(800, 320));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(864, 128));[cite: 2]
            levels[4].EnemySpawnPoint.Add(new Vector2(1152, 96));[cite: 2]
            levels[4].Spikes.Add(new Rectangle(192, 608, 352, 32));[cite: 2]
            levels[4].Spikes.Add(new Rectangle(640, 288, 32, 96));[cite: 2]
            levels[4].Spikes.Add(new Rectangle(704, 480, 96, 32));[cite: 2]
            levels[4].Spikes.Add(new Rectangle(1216, 192, 32, 480));[cite: 2]
            levels[4].Oregano.Add(new Rectangle(160, 128, 64, 64));[cite: 2]
            levels[4].Oregano.Add(new Rectangle(1152, 96, 64, 64));[cite: 2]
            #endregion

            #region STAGE 2-3
            levels[5] = new Level(Content.Load<Texture2D>("Stage2/Stage 2 - 3"));[cite: 2]
            levels[5].PlayerSpawnPoint = new Vector2(32, 96);[cite: 2]
            levels[5].Exit = new Rectangle(1120, 0, 128, 32);[cite: 2]
            levels[5].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(0, 160, 128, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(192, 320, 96, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(0, 416, 128, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(96, 544, 224, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(288, 0, 32, 576));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(0, 672, 1280, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(1056, 608, 224, 65));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(1152, 576, 128, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(1088, 448, 96, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(960, 416, 64, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(800, 416, 96, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(672, 384, 64, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(480, 384, 128, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(320, 320, 128, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(1088, 224, 192, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(416, 192, 864, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(480, 160, 800, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(512, 128, 384, 32));[cite: 2]
            levels[5].Colliders.Add(new Rectangle(544, 0, 384, 32));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(224, 480));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(352, 608));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(448, 608));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(864, 608));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(960, 608));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(800, 352));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(480, 320));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(320, 256));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(1024, 96));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(1120, 96));[cite: 2]
            levels[5].EnemySpawnPoint.Add(new Vector2(1216, 96));[cite: 2]
            levels[5].Spikes.Add(new Rectangle(192, 288, 96, 32));[cite: 2]
            levels[5].Spikes.Add(new Rectangle(0, 384, 96, 32));[cite: 2]
            levels[5].Spikes.Add(new Rectangle(544, 32, 384, 32));[cite: 2]
            levels[5].Oregano.Add(new Rectangle(1216, 512, 64, 64));[cite: 2]
            levels[5].Oregano.Add(new Rectangle(1056, 128, 64, 64));[cite: 2]
            #endregion

            #region STAGE 3-1
            levels[6] = new Level(Content.Load<Texture2D>("Stage3/Stage 3 - 1"));[cite: 2]
            levels[6].PlayerSpawnPoint = new Vector2(32, 544);[cite: 2]
            levels[6].Exit = new Rectangle(1248, 480, 32, 128);[cite: 2]
            levels[6].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(0, 608, 1280, 32));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(608, 512, 160, 96));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(320, 416, 224, 32));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(448, 288, 192, 32));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(704, 192, 192, 32));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(768, 256, 32, 352));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(992, 320, 128, 32));[cite: 2]
            levels[6].Colliders.Add(new Rectangle(832, 448, 96, 32));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(512, 544));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(704, 448));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(512, 224));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(768, 128));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(864, 384));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(1056, 544));[cite: 2]
            levels[6].EnemySpawnPoint.Add(new Vector2(1152, 544));[cite: 2]
            levels[6].Spikes.Add(new Rectangle(320, 384, 64, 32));[cite: 2]
            levels[6].Spikes.Add(new Rectangle(800, 576, 64, 32));[cite: 2]
            levels[6].Spikes.Add(new Rectangle(1056, 288, 64, 32));[cite: 2]
            levels[6].Oregano.Add(new Rectangle(768, 160, 64, 64));[cite: 2]
            #endregion

            #region STAGE 3-2
            levels[7] = new Level(Content.Load<Texture2D>("Stage3/Stage 3 - 2"));[cite: 2]
            levels[7].PlayerSpawnPoint = new Vector2(32, 544);[cite: 2]
            levels[7].Exit = new Rectangle(1248, 96, 32, 128);[cite: 2]
            levels[7].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(0, 608, 1280, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(192, 320, 224, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(480, 416, 224, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(768, 448, 256, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(1088, 480, 160, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(1248, 256, 32, 352));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(224, 96, 256, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(544, 256, 160, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(800, 256, 256, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(1056, 224, 224, 32));[cite: 2]
            levels[7].Colliders.Add(new Rectangle(128, 192, 64, 32));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(704, 544));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(896, 544));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(1152, 544));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(864, 384));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(224, 256));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(416, 32));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(832, 192));[cite: 2]
            levels[7].EnemySpawnPoint.Add(new Vector2(1088, 160));[cite: 2]
            levels[7].Spikes.Add(new Rectangle(352, 576, 64, 32));[cite: 2]
            levels[7].Spikes.Add(new Rectangle(288, 64, 64, 32));[cite: 2]
            levels[7].Spikes.Add(new Rectangle(992, 224, 64, 32));[cite: 2]
            levels[7].Oregano.Add(new Rectangle(1056, 160, 64, 64));[cite: 2]
            levels[7].Oregano.Add(new Rectangle(1020, 160, 64, 64));[cite: 2]
            levels[7].Oregano.Add(new Rectangle(1184, 160, 64, 64));[cite: 2]
            #endregion

            #region STAGE 3-3
            levels[8] = new Level(Content.Load<Texture2D>("Stage3/Stage 3 - 3"));[cite: 2]
            levels[8].PlayerSpawnPoint = new Vector2(32, 576);[cite: 2]
            levels[8].Colliders.Add(new Rectangle(-32, 0, 32, 720));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(1280, 0, 32, 720));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(0, 640, 1280, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(544, 480, 192, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(384, 384, 96, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(800, 384, 96, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(128, 320, 192, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(960, 320, 192, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(320, 608, 160, 32));[cite: 2]
            levels[8].Colliders.Add(new Rectangle(800, 608, 160, 32));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(384, 544));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(544, 576));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(672, 576));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(864, 544));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(1088, 576));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(160, 256));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(608, 416));[cite: 2]
            levels[8].EnemySpawnPoint.Add(new Vector2(1024, 256));[cite: 2]
            levels[8].Spikes.Add(new Rectangle(256, 608, 64, 32));[cite: 2]
            levels[8].Spikes.Add(new Rectangle(960, 608, 64, 32));[cite: 2]
            #endregion 

            player = new Player(new Rectangle((int)levels[currentLevel].PlayerSpawnPoint.X, (int)levels[currentLevel].PlayerSpawnPoint.Y, 80, 80));[cite: 2]
            player.LoadContent(Content);[cite: 2]
            LoadLevel();[cite: 2]
        }

        // === YOUR CONTRIBUTION: BACKGROUND MUSIC TRACK SWITCHING (PREVENTS TRACK STUTTER) ===
        // This helper method checks if the song is already playing before calling MediaPlayer.Play.
        // It prevents the game from stuttering or restarting the audio file on every single frame update.
        private void PlayBgm(Song targetSong)
        {
            if (currentBgm != targetSong)[cite: 2]
            {
                currentBgm = targetSong;[cite: 2]
                MediaPlayer.Play(targetSong);[cite: 2]
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (currentGameState)[cite: 2]
            {
                // === YOUR CONTRIBUTION: MAIN MENU HANDLER ===
                // Inside the game state machine, this listens for button clicks on the menu to load the main play loop.
                case GameState.MainMenu:[cite: 2]
                    PlayBgm(menuBgm);[cite: 2]
                    bool shouldExit = false;[cite: 2]
                    if (mainMenu.Update(gameTime, ref shouldExit))[cite: 2]
                    {
                        currentGameState = GameState.Playing;[cite: 2]
                    }
                    if (shouldExit)[cite: 2]
                        Exit();[cite: 2]
                    break;

                case GameState.Playing:[cite: 2]
                    // === YOUR CONTRIBUTION: REAL-TIME HEALTH & DEATH CHECK ===
                    // Loops through every character state. If a character's HP drops to 0, they are instantly flagged as dead.
                    if (healthSystem != null && healthSystem.characterHealth != null)[cite: 2]
                    {
                        foreach (CharacterType type in System.Enum.GetValues(typeof(CharacterType)))[cite: 2]
                        {
                            if (healthSystem.characterHealth.ContainsKey(type))[cite: 2]
                            {
                                if (healthSystem.characterHealth[type] <= 0) {[cite: 2]
                                    healthSystem.characterDeadStates[type] = true;[cite: 2]
                                }
                            }
                        }
                    }

                    // === YOUR CONTRIBUTION: CHECK FOR TOTAL GAME OVER ===
                    // Evaluates if all active characters are dead. If yes, it calls ResetGame() to restart the level.
                    bool allCharactersDead = true;[cite: 2]
                    if (healthSystem != null && healthSystem.characterDeadStates != null)[cite: 2]
                    {
                        foreach (var state in healthSystem.characterDeadStates.Values)[cite: 2]
                        {
                            if (!state) // If at least one character is still alive
                            {
                                allCharactersDead = false;[cite: 2]
                                break;[cite: 2]
                            }
                        }
                    }

                    if (allCharactersDead)[cite: 2]
                    {
                        ResetGame();[cite: 2]
                        break;[cite: 2]
                    }

                    // 2. CHECK FOR STAGE CLEAR / VICTORY
                    if (levelCleared)[cite: 2]
                    {
                        if (player.PlayerRectangle.Intersects(levels[currentLevel].Exit))[cite: 2]
                        {
                            currentLevel++;[cite: 2]
                            if (currentLevel < levels.Length)[cite: 2]
                            {
                                LoadLevel();[cite: 2]
                            }
                            else[cite: 2]
                            {
                                currentLevel = 0;[cite: 2]
                                currentGameState = GameState.MainMenu; [cite: 2]
                                break;[cite: 2]
                            }
                        }
                    }

                    // === YOUR CONTRIBUTION: BACK BUTTON CLICK INTERACTION ===
                    // Evaluates real-time mouse position. If the mouse is within our custom bounding box and clicked,
                    // the state machine immediately brings the player back to the main menu.
                    MouseState mouseState = Mouse.GetState();[cite: 2]
                    Point mousePosition = new Point(mouseState.X, mouseState.Y);[cite: 2]
                    if (backButtonRect.Contains(mousePosition))[cite: 2]
                    {
                        isHoveringBackButton = true;[cite: 2]
                        if (mouseState.LeftButton == ButtonState.Pressed)[cite: 2]
                        {
                            currentGameState = GameState.MainMenu;[cite: 2]
                        }
                    }
                    else[cite: 2]
                    {
                        isHoveringBackButton = false;[cite: 2]
                    }

                    // === YOUR CONTRIBUTION: DYNAMIC BGM MANAGING BY LEVEL ID ===
                    // Dynamically routes stage progress to play specific sound assets based on level indexes.
                    if (currentLevel >= 0 && currentLevel <= 2)[cite: 2]
                    {
                        PlayBgm(stage1Bgm);[cite: 2]
                    }
                    else if (currentLevel >= 3 && currentLevel <= 5)[cite: 2]
                    {
                        PlayBgm(stage2Bgm);[cite: 2]
                    }
                    else if (currentLevel == 6 || currentLevel == 7)[cite: 2]
                    {
                        PlayBgm(stage3_1_3_2Bgm);[cite: 2]
                    }
                    else if (currentLevel == 8)[cite: 2]
                    {
                        PlayBgm(stage3_3Bgm);[cite: 2]
                    }

                    stageOverlay.Update(gameTime, zombies.Count);[cite: 2]
                    List<Rectangle> activeZombieHitboxes = new List<Rectangle>();[cite: 2]
                    var currentLevelData = levels[currentLevel];[cite: 2]

                    if (stageOverlay.CurrentState == StageState.Playing)[cite: 2]
                    {
                        foreach (var zombie in zombies)[cite: 2]
                        {
                            zombie.Update(gameTime);[cite: 2]
                            zombie.ZombieMovement(gameTime, currentLevelData.Colliders);[cite: 2]
                            if (!zombie.IsDead && !zombie.IsDying)[cite: 2]
                            {
                                activeZombieHitboxes.Add(zombie.ZombieHitbox);[cite: 2]
                                float distanceToPlayerX = Math.Abs(zombie.ZombieHitbox.X - player.PlayerHitbox.X);[cite: 2]
                                float distanceToPlayerY = Math.Abs(zombie.ZombieHitbox.Y - player.PlayerHitbox.Y);[cite: 2]
                                if (distanceToPlayerX < 300 && distanceToPlayerY < 64)[cite: 2]
                                {
                                    if ((zombie.ZombieHitbox.X - player.PlayerHitbox.X) < 0)[cite: 2]
                                    {
                                        zombie.MovingRight = true;[cite: 2]
                                    }
                                    else { zombie.MovingRight = false; }[cite: 2]
                                    zombie.Speed = 4;[cite: 2]
                                }
                                else[cite: 2]
                                {
                                    zombie.Speed = 2;[cite: 2]
                                }

                                if (distanceToPlayerX < 30 && distanceToPlayerY < 64)[cite: 2]
                                {
                                    zombie.Attack(true);[cite: 2]
                                }
                                else[cite: 2]
                                    zombie.Attack(false);[cite: 2]
                            }
                        }
                    }

                    // === YOUR CONTRIBUTION: ENVIRONMENTAL HAZARD COMBINATION ===
                    // You compiled static level hazards (Spikes) and dynamic hazards (Active Zombie Hitboxes)
                    // into a unified "hazards" list to feed directly to the Player update logic.
                    List<Rectangle> hazards = new List<Rectangle>();[cite: 2]
                    hazards.AddRange(currentLevelData.Spikes);[cite: 2]
                    hazards.AddRange(activeZombieHitboxes);[cite: 2]

                    // === YOUR CONTRIBUTION: PASSING OREGANO TO PLAYER TO TRIGGER PICKUP / HEAL ===
                    // You pass both the general hazards and the level's Oregano coordinates to player.Update()
                    // so player collisions with Oregano items can trigger heal mechanics.
                    player.Update(gameTime, currentLevelData.Colliders, hazards, currentLevelData.Oregano);[cite: 2]
                    HandlePlayerAttacks();[cite: 2]

                    zombies.RemoveAll(z => z.IsDead);[cite: 2]
                    if (zombies.Count == 0 && !levelCleared)[cite: 2]
                    {
                        levelCleared = true;[cite: 2]
                    }

                    if (levelCleared)[cite: 2]
                    {
                        PrimitiveSave(1);[cite: 2]
                    }
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Texture2D hitbox = new Texture2D(GraphicsDevice, 1, 1);[cite: 2]
            hitbox.SetData(new[] { Color.White });[cite: 2]

            if (currentGameState == GameState.MainMenu)[cite: 2]
            {
                _spriteBatch.Begin();
                mainMenu.Draw(_spriteBatch, backgroundTexture);[cite: 2]
                _spriteBatch.End();
            }
            else if (currentGameState == GameState.Playing)[cite: 2]
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(levels[currentLevel].LevelTexture, Vector2.Zero, Color.White);[cite: 2]
                _spriteBatch.Draw(hitbox, player.PlayerHitbox, Color.Red);[cite: 2]
                _spriteBatch.Draw(hitbox, player.SwordHitbox, Color.Red);[cite: 2]
                foreach (Zombie zombie in zombies)[cite: 2]
                {
                    zombie.Draw(_spriteBatch);[cite: 2]
                }

                // === YOUR CONTRIBUTION: RENDERING MAP HEALING ASSETS (OREGANO) ===
                foreach (Rectangle oreganoRect in levels[currentLevel].Oregano)[cite: 2]
                {
                    _spriteBatch.Draw(oreganoTexture, oreganoRect, Color.White);[cite: 2]
                }
                player.Draw(_spriteBatch);[cite: 2]
                _spriteBatch.End();

                _spriteBatch.Begin();
                player.DrawUI(_spriteBatch);[cite: 2]

                // === YOUR CONTRIBUTION: BACK BUTTON COLOR TRANSITIONS ===
                // Dynamically swaps the color to Yellow if hovered, or White if untouched, and renders it.
                Color buttonColor = isHoveringBackButton ? Color.Yellow : Color.White;[cite: 2]
                _spriteBatch.DrawString(menuFont, backButtonText, new Vector2(backButtonRect.X, backButtonRect.Y), buttonColor);[cite: 2]
                _spriteBatch.End();

                _spriteBatch.Begin();
                stageOverlay.Draw(_spriteBatch, GraphicsDevice.Viewport, currentLevel);[cite: 2]
                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private void LoadLevel()
        {
            zombies.Clear();[cite: 2]
            levelCleared = false;[cite: 2]
            stageOverlay.Reset();[cite: 2]
            foreach (Vector2 spawn in levels[currentLevel].EnemySpawnPoint)[cite: 2]
            {
                zombies.Add(new Zombie([cite: 2]
                    zombieTexture,[cite: 2]
                    new Rectangle((int)spawn.X, (int)spawn.Y, 64, 64),[cite: 2]
                    Color.White));[cite: 2]
            }
            player.ResetSpawn(levels[currentLevel].PlayerSpawnPoint);[cite: 2]
        }

        // === YOUR CONTRIBUTION: RESTARTING GAME STATES ===
        // Restores character lives back to 5 hearts and marks all character dead states to false.
        private void ResetGame()
        {
            if (healthSystem != null && healthSystem.characterDeadStates != null)[cite: 2]
            {
                var keys = new List<CharacterType>(healthSystem.characterDeadStates.Keys);[cite: 2]
                foreach (var key in keys)[cite: 2]
                {
                    healthSystem.characterDeadStates[key] = false;[cite: 2]
                    healthSystem.characterHealth[key] = 5;[cite: 2]
                }
            }
            LoadLevel(); [cite: 2]
            currentGameState = GameState.Playing;[cite: 2]
        }

        // === YOUR CONTRIBUTION: MULTI-TIER DAMAGE & RANGE CALCULATION ===
        // Implements varying logic for attacks based on whether they hit melee, bullet range, or area-of-effect.
        private void HandlePlayerAttacks()
        {
            // 1. MELEE (Sword swing hitbox - high damage)
            Rectangle weaponSwing = player.SwordHitbox;[cite: 2]
            foreach (var zombie in zombies)[cite: 2]
            {
                if (zombie.IsDead || zombie.IsDying) continue;[cite: 2]
                if (weaponSwing.Intersects(zombie.ZombieHitbox))[cite: 2]
                {
                    zombie.TakeDamage(3); // High Damage: 3 points[cite: 2]
                }
            }

            // 2. RANGED (Gun projectile bullet - medium damage)
            Rectangle? bulletHitbox = player.GetProjectileHitbox();[cite: 2]
            if (bulletHitbox.HasValue)[cite: 2]
            {
                foreach (var zombie in zombies)[cite: 2]
                {
                    if (bulletHitbox.Value.Intersects(zombie.ZombieHitbox))[cite: 2]
                    {
                        zombie.TakeDamage(2); // Medium Damage: 2 points[cite: 2]
                    }
                }
            }
            player.DeactivateProjectile();[cite: 2]

            // 3. AREA OF EFFECT (Multiple concurrent hitboxes - low damage over a wider field)
            List<Rectangle> flowerHitboxes = player.GetFlowerHitboxes();[cite: 2]
            foreach (var flowerRect in flowerHitboxes)[cite: 2]
            {
                foreach (var zombie in zombies)[cite: 2]
                {
                    if (flowerRect.Intersects(zombie.ZombieHitbox))[cite: 2]
                    {
                        zombie.TakeDamage(1); // Low Area Damage: 1 point[cite: 2]
                    }
                }
            }
        }

        private void PrimitiveSave(int slot)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"save{slot}.xml");[cite: 2]
                XmlSerializer serializer = new XmlSerializer(typeof(GameData));[cite: 2]
                GameData data = new GameData[cite: 2]
                {
                    CurrentLevel = this.currentLevel,[cite: 2]
                    ActiveCharacterName = player.CurrentCharacter.ToString()[cite: 2]
                };
                using (StreamWriter writer = new StreamWriter(filePath))[cite: 2]
                {
                    serializer.Serialize(writer, data);[cite: 2]
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine("Windows Security blocked saving: " + ex.Message);[cite: 2]
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to save game: " + ex.Message);[cite: 2]
            }
        }

        public void LoadGameFromSlot(int slot)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"save{slot}.xml");[cite: 2]
            if (File.Exists(filePath))[cite: 2]
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));[cite: 2]
                    using (StreamReader reader = new StreamReader(filePath))[cite: 2]
                    {
                        GameData data = (GameData)serializer.Deserialize(reader);[cite: 2]
                        this.currentLevel = data.CurrentLevel;[cite: 2]
                        if (this.currentLevel >= levels.Length)[cite: 2]
                        {
                            this.currentLevel = levels.Length - 1;[cite: 2]
                        }
                        player.CurrentCharacter = (CharacterType)Enum.Parse(typeof(CharacterType), data.ActiveCharacterName);[cite: 2]
                        LoadLevel();[cite: 2]
                        currentGameState = GameState.Playing;[cite: 2]
                        currentSaveSlot = slot;[cite: 2]
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to load game: " + ex.Message);[cite: 2]
                }
            }
        }
    }
}

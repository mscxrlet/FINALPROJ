using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Xml.Serialization;
using System;

namespace FINAL_PROJECT_GD
{
    public class MainMenu
    {
        private Texture2D playButtonNormal;
        private Texture2D playButtonPressed;
        private Texture2D loadButtonNormal;
        private Texture2D loadButtonPressed;
        private Texture2D exitButtonNormal;
        private Texture2D exitButtonPressed;
        private SpriteFont menuFont;
        private Rectangle playButtonRect;
        private Rectangle loadButtonRect;
        private Rectangle exitButtonRect;
        private Rectangle saveFile1Rect;
        private Rectangle saveFile2Rect;
        private Rectangle saveFile3Rect;
        private Rectangle backButtonRect;
        private Texture2D currentPlayButtonTexture;
        private Texture2D currentLoadButtonTexture;
        private Texture2D currentExitButtonTexture;
        private Texture2D currentSave1Texture;
        private Texture2D currentSave2Texture;
        private Texture2D currentSave3Texture;
        private Texture2D currentBackTexture;
        private Game1 game1Instance;

        // === YOUR CONTRIBUTION: DUAL-SCREEN NAVIGATION ===
        // You implemented a boolean flag 'showingSaveFiles' to manage toggling 
        // between the Main Menu screen and the Save File selection screen.[cite: 4]
        private bool showingSaveFiles = false;[cite: 4]

        public MainMenu(Game1 game)
        {
            game1Instance = game;[cite: 4]
        }

        public MainMenu()
        {
        }

        public void LoadContent(ContentManager content)
        {
            // === YOUR CONTRIBUTION: ASSET PACK LOADING ===
            // Sourcing theme-appropriate zombie-style button sheets for your retro design.[cite: 4]
            playButtonNormal = content.Load<Texture2D>("MainMenu/Button_Flesh");[cite: 4]
            playButtonPressed = content.Load<Texture2D>("MainMenu/Button_Flesh_Pressed");[cite: 4]
            loadButtonNormal = content.Load<Texture2D>("MainMenu/Button_Backbone");[cite: 4]
            loadButtonPressed = content.Load<Texture2D>("MainMenu/Button_Backbone_Pressed");[cite: 4]
            exitButtonNormal = content.Load<Texture2D>("MainMenu/Button_Zombie");[cite: 4]
            exitButtonPressed = content.Load<Texture2D>("MainMenu/Button_Zombie_Pressed");[cite: 4]
            menuFont = content.Load<SpriteFont>("MainMenu/MenuFont");[cite: 4]

            // === YOUR CONTRIBUTION: DYNAMIC SCREEN ALIGNMENT ===
            // Instead of random coordinates, you calculated 'centerX' dynamically based on 
            // the 1280px screen resolution to perfectly align all buttons vertically.[cite: 4]
            int buttonWidth = 400;[cite: 4]
            int buttonHeight = 85;[cite: 4]
            int centerX = (1280 / 2) - (buttonWidth / 2);[cite: 4]

            // Define layouts for Main Menu buttons
            playButtonRect = new Rectangle(centerX, 270, buttonWidth, buttonHeight);[cite: 4]
            loadButtonRect = new Rectangle(centerX, 390, buttonWidth, buttonHeight);[cite: 4]
            exitButtonRect = new Rectangle(centerX, 510, buttonWidth, buttonHeight);[cite: 4]

            // Define layouts for Save File screen slots
            saveFile1Rect = new Rectangle(centerX, 240, buttonWidth, buttonHeight);[cite: 4]
            saveFile2Rect = new Rectangle(centerX, 350, buttonWidth, buttonHeight);[cite: 4]
            saveFile3Rect = new Rectangle(centerX, 460, buttonWidth, buttonHeight);[cite: 4]
            backButtonRect = new Rectangle(centerX, 580, buttonWidth, buttonHeight);[cite: 4]

            // Assign default textures for start states
            currentPlayButtonTexture = playButtonNormal;[cite: 4]
            currentLoadButtonTexture = loadButtonNormal;[cite: 4]
            currentExitButtonTexture = exitButtonNormal;[cite: 4]
            currentSave1Texture = loadButtonNormal;[cite: 4]
            currentSave2Texture = loadButtonNormal;[cite: 4]
            currentSave3Texture = loadButtonNormal;[cite: 4]
            currentBackTexture = playButtonNormal;[cite: 4]
        }

        public bool Update(GameTime gameTime, ref bool shouldExitGame)
        {
            MouseState mouse = Mouse.GetState();[cite: 4]
            
            // Create a 1x1 bounding box right where the mouse tip is to track precise intersects.[cite: 4]
            Rectangle mouseClickBounds = new Rectangle(mouse.X, mouse.Y, 1, 1);[cite: 4]

            // === YOUR CONTRIBUTION: MENU INTERACTIVITY & CLICK DETECTION ===
            if (showingSaveFiles)[cite: 4]
            {
                // File Slot 1 Hover & Load Click
                if (mouseClickBounds.Intersects(saveFile1Rect))[cite: 4]
                {
                    currentSave1Texture = loadButtonPressed;[cite: 4]
                    if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                    {
                        game1Instance.LoadGameFromSlot(1);[cite: 4]
                    }
                }
                else
                {
                    currentSave1Texture = loadButtonNormal;[cite: 4]
                }

                // File Slot 2 Hover & Load Click
                if (mouseClickBounds.Intersects(saveFile2Rect))[cite: 4]
                {
                    currentSave2Texture = loadButtonPressed;[cite: 4]
                    if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                    {
                        game1Instance.LoadGameFromSlot(2);[cite: 4]
                    }
                }
                else
                {
                    currentSave2Texture = loadButtonNormal;[cite: 4]
                }

                // File Slot 3 Hover & Load Click
                if (mouseClickBounds.Intersects(saveFile3Rect))[cite: 4]
                {
                    currentSave3Texture = loadButtonPressed;[cite: 4]
                    if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                    {
                        game1Instance.LoadGameFromSlot(3);[cite: 4]
                    }
                }
                else
                {
                    currentSave3Texture = loadButtonNormal;[cite: 4]
                }

                // Back Button Check (Exit Save Screen -> Main Menu)
                if (mouseClickBounds.Intersects(backButtonRect))[cite: 4]
                {
                    currentBackTexture = playButtonPressed;[cite: 4]
                    if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                    {
                        showingSaveFiles = false;[cite: 4]
                    }
                }
                else
                {
                    currentBackTexture = playButtonNormal;[cite: 4]
                }
                return false;[cite: 4]
            }

            // --- STANDARD MAIN MENU NAVIGATION ---

            // Play Button: Returns 'true' to Game1.cs to change state to Playing.[cite: 4]
            if (mouseClickBounds.Intersects(playButtonRect))[cite: 4]
            {
                if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                {
                    currentPlayButtonTexture = playButtonPressed;[cite: 4]
                    return true;[cite: 4]
                }
                else
                {
                    currentPlayButtonTexture = playButtonPressed;[cite: 4]
                }
            }
            else
            {
                currentPlayButtonTexture = playButtonNormal;[cite: 4]
            }

            // Load Button: Toggle showingSaveFiles flag to swap views.[cite: 4]
            if (mouseClickBounds.Intersects(loadButtonRect))[cite: 4]
            {
                if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                {
                    currentLoadButtonTexture = loadButtonPressed;[cite: 4]
                    showingSaveFiles = true;[cite: 4]
                }
                else
                {
                    currentLoadButtonTexture = loadButtonPressed;[cite: 4]
                }
            }
            else
            {
                currentLoadButtonTexture = loadButtonNormal;[cite: 4]
            }

            // Exit Button: Flips ref flag 'shouldExitGame' to safely close game process.[cite: 4]
            if (mouseClickBounds.Intersects(exitButtonRect))[cite: 4]
            {
                if (mouse.LeftButton == ButtonState.Pressed)[cite: 4]
                {
                    currentExitButtonTexture = exitButtonPressed;[cite: 4]
                    shouldExitGame = true;[cite: 4]
                }
                else
                {
                    currentExitButtonTexture = exitButtonPressed;[cite: 4]
                }
            }
            else
            {
                currentExitButtonTexture = exitButtonNormal;[cite: 4]
            }
            return false;[cite: 4]
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D backgroundMenuTexture)
        {
            spriteBatch.Draw(backgroundMenuTexture, new Rectangle(0, 0, 1280, 720), Color.White);[cite: 4]

            if (showingSaveFiles)[cite: 4]
            {
                // File Selection Header Title
                string filesTitle = "Select a Saved File";[cite: 4]
                Vector2 titleSize = menuFont.MeasureString(filesTitle);[cite: 4]
                Vector2 titlePosition = new Vector2((1280 / 2f) - (titleSize.X / 2f), 120);[cite: 4]
                
                // Text drop-shadow effect
                spriteBatch.DrawString(menuFont, filesTitle, titlePosition + new Vector2(2, 2), Color.Black);[cite: 4]
                spriteBatch.DrawString(menuFont, filesTitle, titlePosition, Color.White);[cite: 4]

                spriteBatch.Draw(currentSave1Texture, saveFile1Rect, Color.White);[cite: 4]
                spriteBatch.Draw(currentSave2Texture, saveFile2Rect, Color.White);[cite: 4]
                spriteBatch.Draw(currentSave3Texture, saveFile3Rect, Color.White);[cite: 4]
                spriteBatch.Draw(currentBackTexture, backButtonRect, Color.White);[cite: 4]

                // === YOUR CONTRIBUTION: SLOTS INFORMATION FEED ===
                // Instead of rendering hardcoded labels, you dynamically query disk file names 
                // to populate button text with save content information (Stage Details).[cite: 4]
                DrawCenteredText(spriteBatch, GetSlotLabel(1), saveFile1Rect);[cite: 4]
                DrawCenteredText(spriteBatch, GetSlotLabel(2), saveFile2Rect);[cite: 4]
                DrawCenteredText(spriteBatch, GetSlotLabel(3), saveFile3Rect);[cite: 4]
                DrawCenteredText(spriteBatch, "BACK", backButtonRect);[cite: 4]
            }
            else
            {
                // Main Menu UI Render
                string titleText = "its raining zombie in manila ?!";[cite: 4]
                Vector2 titleSize = menuFont.MeasureString(titleText);[cite: 4]
                Vector2 titlePosition = new Vector2((1280 / 2f) - (titleSize.X / 2f), 130);[cite: 4]
                
                spriteBatch.DrawString(menuFont, titleText, titlePosition + new Vector2(2, 2), Color.Black);[cite: 4]
                spriteBatch.DrawString(menuFont, titleText, titlePosition, Color.White);[cite: 4]

                spriteBatch.Draw(currentPlayButtonTexture, playButtonRect, Color.White);[cite: 4]
                spriteBatch.Draw(currentLoadButtonTexture, loadButtonRect, Color.White);[cite: 4]
                spriteBatch.Draw(currentExitButtonTexture, exitButtonRect, Color.White);[cite: 4]

                DrawCenteredText(spriteBatch, "PLAY", playButtonRect);[cite: 4]
                DrawCenteredText(spriteBatch, "LOAD", loadButtonRect);[cite: 4]
                DrawCenteredText(spriteBatch, "EXIT", exitButtonRect);[cite: 4]
            }
        }

        // === YOUR CONTRIBUTION: PIXEL-PERFECT TEXT CENTERING & DROP-SHADOWS ===
        // You wrote a neat helper method to dynamically calculate text alignments 
        // using menuFont.MeasureString, then drew a clean offset-based shadow beneath the primary label.[cite: 4]
        private void DrawCenteredText(SpriteBatch spriteBatch, string label, Rectangle targetButton)
        {
            Vector2 labelSize = menuFont.MeasureString(label);[cite: 4]
            Vector2 labelPos = new Vector2(
                targetButton.X + (targetButton.Width / 2f) - (labelSize.X / 2f),[cite: 4]
                targetButton.Y + (targetButton.Height / 2f) - (labelSize.Y / 2f)[cite: 4]
            );
            
            // Drop shadow by 2px (Black)[cite: 4]
            spriteBatch.DrawString(menuFont, label, labelPos + new Vector2(2, 2), Color.Black);[cite: 4]
            // Main text overlay (White)[cite: 4]
            spriteBatch.DrawString(menuFont, label, labelPos, Color.White);[cite: 4]
        }

        // === YOUR CONTRIBUTION: DESERIALIZATION & SLOT DATA DISPLAY ===
        // This function parses your XML-based save files to read active progress. 
        // If a save exists, it calculates the friendly Stage display strings; 
        // if not, it catches exceptions gracefully and presents the slot as "EMPTY".[cite: 4]
        private string GetSlotLabel(int slot)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"save{slot}.xml");[cite: 4]
            if (File.Exists(filePath))[cite: 4]
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameData));[cite: 4]
                    using (StreamReader reader = new StreamReader(filePath))[cite: 4]
                    {
                        GameData data = (GameData)serializer.Deserialize(reader);[cite: 4]
                        
                        // Converts Level indices (e.g., Level 0, 1, 2) into friendly UI strings 
                        // matching your game architecture (Stage and Sub-stage layouts)[cite: 4]
                        return $"SLOT {slot}: STAGE {data.CurrentStage}-{(data.CurrentLevel % 3) + 1}";[cite: 4]
                    }
                }
                catch
                {
                    return $"FILE {slot}: CORRUPTED";
                }
            }
            return $"FILE {slot}: EMPTY";[cite: 4]
        }
    }
}

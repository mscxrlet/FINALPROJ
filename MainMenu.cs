using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project
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

        // state machine manager
        private bool showingSaveFiles = false;

        public MainMenu()
        {
        }

        public void LoadContent(ContentManager content)
        {
            // menu assets
            playButtonNormal = content.Load<Texture2D>("Button_Flesh");
            playButtonPressed = content.Load<Texture2D>("Button_Flesh_Pressed");
            loadButtonNormal = content.Load<Texture2D>("Button_Backbone");
            loadButtonPressed = content.Load<Texture2D>("Button_Backbone_Pressed");
            exitButtonNormal = content.Load<Texture2D>("Button_Zombie");
            exitButtonPressed = content.Load<Texture2D>("Button_Zombie_Pressed");

            menuFont = content.Load<SpriteFont>("MenuFont");

            int buttonWidth = 400;
            int buttonHeight = 85;
            int centerX = (1280 / 2) - (buttonWidth / 2);

            playButtonRect = new Rectangle(centerX, 270, buttonWidth, buttonHeight);
            loadButtonRect = new Rectangle(centerX, 390, buttonWidth, buttonHeight);
            exitButtonRect = new Rectangle(centerX, 510, buttonWidth, buttonHeight);

            saveFile1Rect = new Rectangle(centerX, 240, buttonWidth, buttonHeight);
            saveFile2Rect = new Rectangle(centerX, 350, buttonWidth, buttonHeight);
            saveFile3Rect = new Rectangle(centerX, 460, buttonWidth, buttonHeight);

            backButtonRect = new Rectangle(centerX, 580, buttonWidth, buttonHeight);

            currentPlayButtonTexture = playButtonNormal;
            currentLoadButtonTexture = loadButtonNormal;
            currentExitButtonTexture = exitButtonNormal;

            currentSave1Texture = loadButtonNormal;
            currentSave2Texture = loadButtonNormal;
            currentSave3Texture = loadButtonNormal;
            currentBackTexture = playButtonNormal;
        }

        public bool Update(GameTime gameTime, ref bool shouldExitGame)
        {
            MouseState mouse = Mouse.GetState();
            Rectangle mouseClickBounds = new Rectangle(mouse.X, mouse.Y, 1, 1);

            // save files menu
            if (showingSaveFiles)
            {
                // file slots 1
                if (mouseClickBounds.Intersects(saveFile1Rect))
                {
                    currentSave1Texture = loadButtonPressed;
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        // HERE FOR THE FILES 1 LOGIC
                    }
                }
                else
                {
                    currentSave1Texture = loadButtonNormal;
                }

                // file slots 2
                if (mouseClickBounds.Intersects(saveFile2Rect))
                {
                    currentSave2Texture = loadButtonPressed;
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        // HERE FOR THE FILES 2 LOGIC
                    }
                }
                else
                {
                    currentSave2Texture = loadButtonNormal;
                }

                // file slots 3
                if (mouseClickBounds.Intersects(saveFile3Rect))
                {
                    currentSave3Texture = loadButtonPressed;
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        // HERE FOR THE FILES 3 LOGIC
                    }
                }
                else
                {
                    currentSave3Texture = loadButtonNormal;
                }

                // back button check
                if (mouseClickBounds.Intersects(backButtonRect))
                {
                    currentBackTexture = playButtonPressed;
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        showingSaveFiles = false; // go back to the core main menu
                    }
                }
                else
                {
                    currentBackTexture = playButtonNormal;
                }

                return false;
            }

            // play
            if (mouseClickBounds.Intersects(playButtonRect)) 
            {
                if (mouse.LeftButton == ButtonState.Pressed) 
                {
                    currentPlayButtonTexture = playButtonPressed;
                    return true; 
                }
                else
                {
                    currentPlayButtonTexture = playButtonPressed; 
                }
            }
            else
            {
                currentPlayButtonTexture = playButtonNormal; 
            }

            // load
            if (mouseClickBounds.Intersects(loadButtonRect)) 
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    currentLoadButtonTexture = loadButtonPressed; 
                    showingSaveFiles = true; // switch to file list
                }
                else
                {
                    currentLoadButtonTexture = loadButtonPressed; 
                }
            }
            else
            {
                currentLoadButtonTexture = loadButtonNormal;
            }

            // exit
            if (mouseClickBounds.Intersects(exitButtonRect))
            {
                if (mouse.LeftButton == ButtonState.Pressed) 
                {
                    currentExitButtonTexture = exitButtonPressed; 
                    shouldExitGame = true; 
                }
                else
                {
                    currentExitButtonTexture = exitButtonPressed;
                }
            }
            else
            {
                currentExitButtonTexture = exitButtonNormal;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D backgroundMenuTexture)
        {
            spriteBatch.Draw(backgroundMenuTexture, new Rectangle(0, 0, 1280, 720), Color.White);

            if (showingSaveFiles)
            {
                // header title text
                string filesTitle = "Select a Saved File";
                Vector2 titleSize = menuFont.MeasureString(filesTitle);
                Vector2 titlePosition = new Vector2((1280 / 2f) - (titleSize.X / 2f), 120);
                spriteBatch.DrawString(menuFont, filesTitle, titlePosition + new Vector2(2, 2), Color.Black);
                spriteBatch.DrawString(menuFont, filesTitle, titlePosition, Color.White);

                spriteBatch.Draw(currentSave1Texture, saveFile1Rect, Color.White);
                spriteBatch.Draw(currentSave2Texture, saveFile2Rect, Color.White);
                spriteBatch.Draw(currentSave3Texture, saveFile3Rect, Color.White);
                spriteBatch.Draw(currentBackTexture, backButtonRect, Color.White);

                // render yung labels + here yung text
                DrawCenteredText(spriteBatch, "FILE SLOT 1", saveFile1Rect);
                DrawCenteredText(spriteBatch, "FILE SLOT 2", saveFile2Rect);
                DrawCenteredText(spriteBatch, "FILE SLOT 3", saveFile3Rect);
                DrawCenteredText(spriteBatch, "BACK", backButtonRect);
            }
            else
            {
                // main menu draw
                string titleText = "its raining zombie in manila ?!"; 
                Vector2 titleSize = menuFont.MeasureString(titleText); 
                Vector2 titlePosition = new Vector2((1280 / 2f) - (titleSize.X / 2f), 130); 
                spriteBatch.DrawString(menuFont, titleText, titlePosition + new Vector2(2, 2), Color.Black);
                spriteBatch.DrawString(menuFont, titleText, titlePosition, Color.White); 

                spriteBatch.Draw(currentPlayButtonTexture, playButtonRect, Color.White); 
                spriteBatch.Draw(currentLoadButtonTexture, loadButtonRect, Color.White);
                spriteBatch.Draw(currentExitButtonTexture, exitButtonRect, Color.White);

                DrawCenteredText(spriteBatch, "PLAY", playButtonRect);
                DrawCenteredText(spriteBatch, "LOAD", loadButtonRect);
                DrawCenteredText(spriteBatch, "EXIT", exitButtonRect);
            }
        }

        // shadow sa texts
        private void DrawCenteredText(SpriteBatch spriteBatch, string label, Rectangle targetButton)
        {
            Vector2 labelSize = menuFont.MeasureString(label);
            Vector2 labelPos = new Vector2(
                targetButton.X + (targetButton.Width / 2f) - (labelSize.X / 2f),
                targetButton.Y + (targetButton.Height / 2f) - (labelSize.Y / 2f)
            );

            // drop shadow by 2 px
            spriteBatch.DrawString(menuFont, label, labelPos + new Vector2(2, 2), Color.Black);

            // main text in white
            spriteBatch.DrawString(menuFont, label, labelPos, Color.White);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FINAL_PROJECT_GD
{
    public class HealthSystem
    {
        private Texture2D heartSheet;
        
        // === YOUR CONTRIBUTION: SPRITE SHEET CUTTING ===
        // You set up an array of rectangles to slice a single texture file (heartSheet) 
        // into 5 distinct frames representing different health levels.
        private Rectangle[] heartSourceRects; 
        
        // === YOUR CONTRIBUTION: GLOBAL PARTY HEALTH TRACKING ===
        // You used Dictionaries to keep track of individual health points and survival status 
        // for all characters simultaneously, even when they are benched (not active).
        public Dictionary<CharacterType, int> characterHealth;
        public Dictionary<CharacterType, bool> characterDeadStates;

        public HealthSystem(Texture2D heartSheet)
        {
            this.heartSheet = heartSheet;
            this.heartSourceRects = new Rectangle[5];
            
            // === YOUR CONTRIBUTION: SPRITE SHEET MATH ===
            // Instead of manually hardcoding coordinates, you wrote a loop that programmatically 
            // slices your 18x18 pixel heart frames, accounting for a 1-pixel gap/spacing between them.
            int heartWidth = 18;
            int heartHeight = 18;
            int spacing = 1;
            
            for (int i = 0; i < 5; i++)
            {
                int xCoord = i * (heartWidth + spacing);
                heartSourceRects[i] = new Rectangle(xCoord, 0, heartWidth, heartHeight);
            }

            // === YOUR CONTRIBUTION: PARTY HP INITIALIZATION ===
            // Initializing each character with full health (5) and set alive (false for dead state).
            characterHealth = new Dictionary<CharacterType, int>
            {
                { CharacterType.Abby, 5 },
                { CharacterType.Edward, 5 },
                { CharacterType.Nadine, 5 }
            };

            characterDeadStates = new Dictionary<CharacterType, bool>
            {
                { CharacterType.Abby, false },
                { CharacterType.Edward, false },
                { CharacterType.Nadine, false }
            };
        }

        public bool IsCharacterDead(CharacterType character) => characterDeadStates[character];

        // === YOUR CONTRIBUTION: AUTOMATIC SWAP-ON-DEATH MECHANIC ===
        // When a character takes damage and their health hits 0, this method handles the logic:
        // 1. It flags them as dead.
        // 2. It automatically scans the remaining party members.
        // 3. If it finds an alive character, it triggers the onCharacterSwap action.
        // 4. If everyone is dead, it triggers onGameOver to prompt ResetGame() in Game1.cs.
        public void TakeDamage(CharacterType currentCharacter, Action<CharacterType> onCharacterSwap, Action onGameOver)
        {
            if (characterDeadStates[currentCharacter]) return;

            characterHealth[currentCharacter]--;

            if (characterHealth[currentCharacter] <= 0)
            {
                characterHealth[currentCharacter] = 0;
                characterDeadStates[currentCharacter] = true;

                // Look for the next available living character
                foreach (CharacterType type in Enum.GetValues(typeof(CharacterType)))
                {
                    if (!characterDeadStates[type])
                    {
                        onCharacterSwap?.Invoke(type); // Triggers swap action immediately
                        return;
                    }
                }
                
                // If the loop finishes and no one is alive:
                onGameOver?.Invoke(); 
            }
        }

        // === YOUR CONTRIBUTION: BOUNDED HEALING SYSTEM ===
        // Adds HP to the character but guarantees their health never exceeds the maximum limit of 5.
        public void Heal(CharacterType currentCharacter, int amount)
        {
            if (characterDeadStates[currentCharacter]) return;

            characterHealth[currentCharacter] += amount;
            if (characterHealth[currentCharacter] > 5)
            {
                characterHealth[currentCharacter] = 5;
            }
        }

        // === YOUR CONTRIBUTION: DYNAMIC HUD RENDERING ===
        // This calculates which sprite frame to draw based on current health. 
        // It converts the health points into an index (e.g., 5 HP displays Index 0, 0 HP displays Index 4)
        // and scales the heart up cleanly by 3.0x to preserve retro-style pixel art sharpness.
        public void Draw(SpriteBatch spriteBatch, CharacterType activeCharacter)
        {
            int currentHP = characterHealth[activeCharacter];

            // 5 HP = Full Heart (Index 0)
            // 4 HP = 3/4 Heart (Index 1)
            // 3 HP = Half Heart (Index 2)
            // 2 HP = 1/4 Heart (Index 3)
            // 1 HP = Crimson Sliver (Index 3)
            // 0 HP = Empty Frame (Index 4)
            int frameIndex = 4 - currentHP;
            if (frameIndex < 0) frameIndex = 0;
            if (frameIndex > 4) frameIndex = 4;

            Rectangle sourceFrame = heartSourceRects[frameIndex];
            Vector2 position = new Vector2(30, 30);

            spriteBatch.Draw(
                heartSheet,
                position,
                sourceFrame,
                Color.White,
                0f,
                Vector2.Zero,
                3.0f, // Scaled up cleanly so pixel lines stay crisp
                SpriteEffects.None,
                0f
            );
        }
    }
}

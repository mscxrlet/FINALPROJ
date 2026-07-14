using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class HealthSystem
    {
        private Texture2D heartSheet;
        private Rectangle[] heartSourceRects; // array na naghohold sa lahat ng 5 distinct health stages

        private Dictionary<CharacterType, int> characterHealth;
        private Dictionary<CharacterType, bool> characterDeadStates;

        public HealthSystem(Texture2D heartSheet)
        {
            this.heartSheet = heartSheet;
            this.heartSourceRects = new Rectangle[5];

            int heartWidth = 18;
            int heartHeight = 18;
            int spacing = 1;

            // genereates source recsss for all 5 depletion frames 
            for (int i = 0; i < 5; i++)
            {
                int xCoord = i * (heartWidth + spacing);
                heartSourceRects[i] = new Rectangle(xCoord, 0, heartWidth, heartHeight);
            }

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

        public void TakeDamage(CharacterType currentCharacter, Action<CharacterType> onCharacterSwap, Action onGameOver)
        {
            if (characterDeadStates[currentCharacter]) return;

            characterHealth[currentCharacter]--;

            if (characterHealth[currentCharacter] <= 0)
            {
                characterHealth[currentCharacter] = 0;
                characterDeadStates[currentCharacter] = true;

                foreach (CharacterType type in Enum.GetValues(typeof(CharacterType)))
                {
                    if (!characterDeadStates[type])
                    {
                        onCharacterSwap?.Invoke(type);
                        return;
                    }
                }
                onGameOver?.Invoke();
            }
        }

        public void Heal(CharacterType currentCharacter, int amount)
        {
            if (characterDeadStates[currentCharacter]) return;

            characterHealth[currentCharacter] += amount;
            if (characterHealth[currentCharacter] > 5)
            {
                characterHealth[currentCharacter] = 5;
            }
        }

        public void Draw(SpriteBatch spriteBatch, CharacterType activeCharacter)
        {
            int currentHP = characterHealth[activeCharacter];

            // Calculate which specific frame index to display based on your 0-5 HP status
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

            // Render a single HUD container in the static top-left screen layout area
            Vector2 position = new Vector2(30, 30);

            spriteBatch.Draw(
                heartSheet,
                position,
                sourceFrame,
                Color.White,
                0f,
                Vector2.Zero,
                3.0f, // Scaled up cleanly so pixel lines are crisp
                SpriteEffects.None,
                0f
            );
        }
    }
}

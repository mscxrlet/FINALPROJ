using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class Animation
    {
        private Texture2D spriteSheet;
        private int frameWidth;
        private int frameHeight;
        private int frameCount;
        private int currentFrame;
        private float frameSpeed;
        private float timer;
        private int row;
        private bool looping = true;

        public Animation(
            Texture2D spriteSheet,
            int frameWidth,
            int frameHeight,
            int row,
            int frameCount,
            float frameSpeed,
            bool looping = true)
        {
            this.spriteSheet = spriteSheet;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.row = row;
            this.frameCount = frameCount;
            this.frameSpeed = frameSpeed;
            this.looping = looping;
            currentFrame = 0;
            timer = 0f;
        }

        public void Reset()
        {
            currentFrame = 0;
            timer = 0f;
        }

        // Fixed type spacing: GameTime instead of Game Time
        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= frameSpeed)
            {
                timer = 0f;
                currentFrame++;
                if (currentFrame >= frameCount)
                {
                    if (looping)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        currentFrame = frameCount - 1;
                    }
                }
            }
        }

        public Rectangle SourceRectangle
        {
            get
            {
                return new Rectangle(
                    currentFrame * frameWidth,
                    row * frameHeight,
                    frameWidth,
                    frameHeight);
            }
        }

        public bool Finished
        {
            get
            {
                return !looping && currentFrame == frameCount - 1;
            }
        }
    }
}

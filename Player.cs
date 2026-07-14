using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_Project
{
    public enum CharacterType
    {
        Abby, Edward, Nadine
    }

    public class Player
    {
        private Texture2D[] characterSheets;
        private Texture2D spriteSheet;

        private int idleFrames;
        private int walkFrames;
        private int runFrames;
        private int jumpFrames;
        private int hurtFrames;
        private int attackFrames;
        private int healFrames;
        private int deathFrames;

        private CharacterType currentCharacter = CharacterType.Abby;
        private Texture2D abbySheet;
        private Texture2D edwardSheet;
        private Texture2D nadineSheet;
        private KeyboardState previousKeyboard;

        // skill 
        private Texture2D bulletTexture;
        private Texture2D flowerAoESheet;
        private Animation flowerAoEAnimation;

        // pos n move
        private Vector2 position;
        private Vector2 velocity;
        private float moveSpeed = 4f;
        private float gravity = 0.40f;
        private float jumpPower = -10f;

        // charac states
        private bool facingRight = true;
        private bool moving = false;
        private bool jumping = false;
        private bool attacking = false;
        private bool dead = false;

        // for frame n sprite rendering
        const int FrameWidth = 64;
        const int FrameHeight = 64;
        private SpriteEffects flipEffect = SpriteEffects.None;

        // anim
        private Animation currentAnimation;
        private Animation idleRight;
        private Animation walkRight;
        private Animation runRight;
        private Animation jumpRight;
        private Animation hurtRight;
        private Animation attackRight;
        private Animation healRight;
        private Animation deathRight;

        // abby range skill
        private Vector2 projectilePosition;
        private Vector2 projectileVelocity;
        private bool projectileActive = false;
        private float projectileSpeed = 8f;
        private SpriteEffects projectileFlip = SpriteEffects.None;

        // edward melee speed
        private float originalAttackSpeed = 0.08f;

        // nadine aoe skill
        private float aoeCooldownTimer = 0f;
        private float aoeCooldownDuration = 5f; // 5 seconds cooldown
        private bool aoeActive = false;

        private System.Collections.Generic.List<MovingFlower> activeFlowers = new System.Collections.Generic.List<MovingFlower>();

        private HealthSystem healthSystem;
        private float damageCooldownTimer = 0f;
        private float deathTimer = 0f;
        private const float DamageCooldownDuration = 1.0f;


        public class MovingFlower
        {
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            public Animation Anim { get; set; }
            public MovingFlower(Vector2 pos, Vector2 vel, Animation animation)
            {
                Position = pos;
                Velocity = vel;
                Anim = animation;
            }
        }
        public Player(Vector2 startPosition)
        {
            position = startPosition;
            velocity = Vector2.Zero;
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public void LoadContent(ContentManager content)
        {
            abbySheet = content.Load<Texture2D>("abby_spritesheet");
            edwardSheet = content.Load<Texture2D>("edward_spritesheet");
            nadineSheet = content.Load<Texture2D>("nadine_spritesheet");
            bulletTexture = content.Load<Texture2D>("bullet");
            flowerAoESheet = content.Load<Texture2D>("flower_aoe");
            characterSheets = new Texture2D[] { abbySheet, edwardSheet, nadineSheet };

            ChangeCharacter(CharacterType.Abby);
            healthSystem = new HealthSystem(content.Load<Texture2D>("heart"));
        }

        private void ChangeCharacter(CharacterType newCharacter)
        {
            currentCharacter = newCharacter;
            dead = false;
            projectileActive = false; // reset skills on switch
            aoeActive = false;

            switch (currentCharacter)
            {
                case CharacterType.Abby:
                    spriteSheet = abbySheet;
                    idleFrames = 4;
                    walkFrames = 6;
                    runFrames = 4;
                    jumpFrames = 5;
                    hurtFrames = 4;
                    attackFrames = 5;
                    healFrames = 12;
                    deathFrames = 9;
                    originalAttackSpeed = 0.15f;
                    break;
                case CharacterType.Edward:
                    spriteSheet = edwardSheet;
                    idleFrames = 4;
                    walkFrames = 5;
                    runFrames = 4;
                    jumpFrames = 6;
                    hurtFrames = 4;
                    attackFrames = 7;
                    healFrames = 11;
                    deathFrames = 8;
                    originalAttackSpeed = 0.05f;
                    break;
                case CharacterType.Nadine:
                    spriteSheet = nadineSheet;
                    idleFrames = 4;
                    walkFrames = 6;
                    runFrames = 4;
                    jumpFrames = 8;
                    hurtFrames = 6;
                    attackFrames = 14;
                    healFrames = 11;
                    deathFrames = 9;
                    originalAttackSpeed = 0.08f;
                    break;
            }
            LoadAnimations();
        }

        private void LoadAnimations()
        {
            idleRight = new Animation(spriteSheet, 64, 64, 0, idleFrames, 0.20f);
            walkRight = new Animation(spriteSheet, 64, 64, 1, walkFrames, 0.18f);
            runRight = new Animation(spriteSheet, 64, 64, 2, runFrames, 0.10f);
            jumpRight = new Animation(spriteSheet, 64, 64, 3, jumpFrames, 0.12f, false);
            hurtRight = new Animation(spriteSheet, 64, 64, 4, hurtFrames, 0.15f, false);
            attackRight = new Animation(spriteSheet, 64, 64, 5, attackFrames, originalAttackSpeed, false);
            healRight = new Animation(spriteSheet, 64, 64, 6, healFrames, 0.10f, false);
            deathRight = new Animation(spriteSheet, 64, 64, 7, deathFrames, 0.15f, false);

            currentAnimation = idleRight;
        }

        public void Update(GameTime gameTime, Rectangle ground, System.Collections.Generic.List<Rectangle> spikes, System.Collections.Generic.List<Rectangle> oreganoItems)
        {
            KeyboardState keyboard = Keyboard.GetState();

            // reduce damage cooldown duration tracking over elapsed ticks
            if (damageCooldownTimer > 0)
            {
                damageCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // swap
            if (keyboard.IsKeyDown(Keys.D1) && previousKeyboard.IsKeyUp(Keys.D1) && !healthSystem.IsCharacterDead(CharacterType.Abby))
            {
                ChangeCharacter(CharacterType.Abby);
            }
            if (keyboard.IsKeyDown(Keys.D2) && previousKeyboard.IsKeyUp(Keys.D2) && !healthSystem.IsCharacterDead(CharacterType.Edward))
            {
                ChangeCharacter(CharacterType.Edward);
            }
            if (keyboard.IsKeyDown(Keys.D3) && previousKeyboard.IsKeyUp(Keys.D3) && !healthSystem.IsCharacterDead(CharacterType.Nadine))
            {
                ChangeCharacter(CharacterType.Nadine);
            }

            // spike damage colission
            Rectangle playerBounds = new Rectangle((int)position.X + 20, (int)position.Y + 18, 40, 62);
            if (damageCooldownTimer <= 0 && !dead)
            {
                foreach (Rectangle spike in spikes)
                {
                    if (playerBounds.Intersects(spike))
                    {
                        damageCooldownTimer = DamageCooldownDuration;
                        currentAnimation = hurtRight;
                        hurtRight.Reset();
                        healthSystem.TakeDamage(
                            currentCharacter,
                            nextCharacter => 
                            {
                                dead = true;
                                currentAnimation = deathRight;
                                deathRight.Reset();
                                deathTimer = 1.5f;
                            },
                            () =>
                            {
                                //run if all characters r ded
                                dead = true;
                                currentAnimation = deathRight;
                                deathRight.Reset();
                                deathTimer = -1f; // -1 = true permanent Game Over
                            }
                        );
                        break;
                    }
                }
            }

            if (!dead && currentAnimation != healRight)
            {
                for (int i = oreganoItems.Count - 1; i >= 0; i--)
                {
                    if (playerBounds.Intersects(oreganoItems[i]))
                    {
                        healthSystem.Heal(currentCharacter, 1);
                        currentAnimation = healRight;
                        healRight.Reset();
                        oreganoItems.RemoveAt(i);
                        break;
                    }
                }
            }

            // nadine flower aoe 
            for (int i = activeFlowers.Count - 1; i >= 0; i--)
            {
                activeFlowers[i].Position += activeFlowers[i].Velocity;
                activeFlowers[i].Anim.Update(gameTime);
                if (Vector2.Distance(position, activeFlowers[i].Position) > 600f)
                {
                    activeFlowers.RemoveAt(i);
                }
            }

            if (activeFlowers.Count == 0)
            {
                aoeActive = false;
            }

            if (aoeCooldownTimer > 0)
            {
                aoeCooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

           // move n input physics
            moving = false;

            if (!dead)
            {
                // hurtt anim
                if (currentAnimation == hurtRight)
                {
                    currentAnimation.Update(gameTime);
                    if (currentAnimation.Finished)
                    {
                        currentAnimation = idleRight;
                    }
                }
                else
                {
                    // Left 
                    if (keyboard.IsKeyDown(Keys.A) && !attacking)
                    {
                        position.X -= moveSpeed;
                        facingRight = false;
                        moving = true;
                    }
                    // Right 
                    else if (keyboard.IsKeyDown(Keys.D) && !attacking)
                    {
                        position.X += moveSpeed;
                        facingRight = true;
                        moving = true;
                    }

                    // Jump 
                    if (keyboard.IsKeyDown(Keys.Space) && !jumping)
                    {
                        velocity.Y = jumpPower;
                        jumping = true;
                    }

                    // ATTAXk
                    if (keyboard.IsKeyDown(Keys.E) && !attacking)
                    {
                        if (currentCharacter == CharacterType.Nadine && aoeCooldownTimer > 0)
                        {
                            // nothing coz may cooldown si nadine
                        }
                        else
                        {
                            attacking = true;
                            attackRight.Reset();
                            currentAnimation = attackRight;

                            // trigger skills
                            if (currentCharacter == CharacterType.Abby)
                            {
                                projectileActive = true;
                                projectilePosition = new Vector2(position.X + (facingRight ? FrameWidth - 10 : -20), position.Y + 14);
                                projectileVelocity = new Vector2(facingRight ? projectileSpeed : -projectileSpeed, 0);
                                projectileFlip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                            }
                            else if (currentCharacter == CharacterType.Nadine)
                            {
                                aoeActive = true;
                                aoeCooldownTimer = aoeCooldownDuration;
                                int singleFlowerWidth = flowerAoESheet.Width / 7;
                                float directionFactor = facingRight ? 1f : -1f;

                                Vector2[] offsets = new Vector2[]
                                {
                                    new Vector2(0, 10),
                                    new Vector2(30, -15),
                                    new Vector2(60, 25),
                                    new Vector2(90, -5),
                                    new Vector2(120, 15),
                                    new Vector2(150, 0)
                                };

                                for (int i = 0; i < offsets.Length; i++)
                                {
                                    Vector2 spawnPos = new Vector2(
                                        position.X + (facingRight ? FrameWidth : -40) + (offsets[i].X * directionFactor),
                                        position.Y + 20 + offsets[i].Y
                                    );

                                    Vector2 spawnVel = new Vector2(6f * directionFactor, 0f);
                                    Animation newAnim = new Animation(flowerAoESheet, singleFlowerWidth, flowerAoESheet.Height, 0, 7, 0.08f, true);
                                    activeFlowers.Add(new MovingFlower(spawnPos, spawnVel, newAnim));
                                }
                            }
                        }
                    }
                }
            }

            // --- UPDATE ABBY PROJECTILES ---
            if (projectileActive)
            {
                projectilePosition += projectileVelocity;
                if (Vector2.Distance(position, projectilePosition) > 800f)
                {
                    projectileActive = false;
                }
            }

            if (aoeActive && currentAnimation.Finished)
            {
                aoeActive = false;
            }

            // --- GLOBAL WORLD ENVIRONMENTAL PHYSICS ---
            velocity.Y += gravity;
            position += velocity;

            // Ground Floor Boundary Intersections
            playerBounds = new Rectangle((int)position.X + 20, (int)position.Y + 18, 40, 62);
            if (playerBounds.Intersects(ground))
            {
                position.Y = ground.Y - 76;
                velocity.Y = 0;
                jumping = false;
            }

            // Sprite Mirror Flipping Rules
            if (facingRight)
            {
                flipEffect = SpriteEffects.None;
            }
            else
            {
                flipEffect = SpriteEffects.FlipHorizontally;
            }

            // --- ANIMATION STATE SELECTOR LOGIC TREE ---
            if (dead)
            {
                currentAnimation = deathRight;
                if (!currentAnimation.Finished)
                {
                    currentAnimation.Update(gameTime);
                }
                else if (deathTimer > 0)
                {
                    deathTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (deathTimer <= 0)
                    {
                        foreach (CharacterType type in System.Enum.GetValues(typeof(CharacterType)))
                        {
                            if (!healthSystem.IsCharacterDead(type))
                            {
                                ChangeCharacter(type);
                                break;
                            }
                        }
                    }
                }
            }
            else if (currentAnimation == hurtRight || currentAnimation == healRight)
            {
            }
            else if (attacking)
            {
                currentAnimation = attackRight;
                currentAnimation.Update(gameTime);
                if (currentAnimation.Finished)
                {
                    attacking = false;
                }
            }
            else
            {
                if (jumping)
                {
                    currentAnimation = jumpRight;
                }
                else if (moving)
                {
                    currentAnimation = runRight;
                }
                else
                {
                    currentAnimation = idleRight;
                }
                currentAnimation.Update(gameTime);
            }

            previousKeyboard = keyboard;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            // spritesheet
            spriteBatch.Draw(
                spriteSheet,
                position,
                currentAnimation.SourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                1.25f,
                flipEffect,
                0f
            );

            // abby's projectile
            if (projectileActive)
            {
                spriteBatch.Draw(
                    bulletTexture,
                    projectilePosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    projectileFlip,
                    0f
                );
            }

            // aoe sliding out
            if (aoeActive)
            {
                foreach (var flower in activeFlowers)
                {
                    spriteBatch.Draw(
                        flowerAoESheet,
                        flower.Position,
                        flower.Anim.SourceRectangle,
                        Color.White,
                        0f,
                        Vector2.Zero,
                        1.0f, // size of petals 
                        flipEffect,
                        0f
                    );
                }
            }
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            if (healthSystem != null)
            {
                healthSystem.Draw(spriteBatch, currentCharacter);
            }
        }
    }
}

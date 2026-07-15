using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FINAL_PROJECT_GD
{
    // === YOUR CONTRIBUTION: MULTI-CHARACTER CLASS SYSTEM ===
    // You defined an enum to represent three distinct playstyles/characters[cite: 4].
    public enum CharacterType
    {
        Abby,
        Edward,
        Nadine
    }

    internal class Player
    {
        private const int FrameWidth = 64;
        private const int FrameHeight = 64;
        private const int WorldWidth = 1280;
        private const int PlayerSpeed = 3;
        private const int JumpSpeed = -18;
        private const int Gravity = 1;
        
        // Offset values separate the player's drawing rectangle from their physical physics hitbox[cite: 4].
        private const int HitboxOffsetX = 28;
        private const int HitboxOffsetY = 16;
        private const int HitboxWidth = 24;
        private const int HitboxHeight = 64;
        
        private const float ProjectileSpeed = 8f;
        private const float DamageCooldownDuration = 1f;
        private const float AoeCooldownDuration = 5f;

        // Pattern offset coordinates used for Nadine's unique Area of Effect (AoE) flower attack[cite: 4].
        private static readonly Vector2[] FlowerOffsets =
        {
            new Vector2(0, 10),
            new Vector2(30, -15),
            new Vector2(60, 25),
            new Vector2(90, -5),
            new Vector2(120, 15),
            new Vector2(150, 0)
        };

        private Rectangle playerRectangle;
        private Rectangle playerHitbox;
        private Texture2D spriteSheet;
        private Texture2D abbySheet;
        private Texture2D edwardSheet;
        private Texture2D nadineSheet;
        private Texture2D bulletTexture;
        private Texture2D flowerAoESheet; 
        
        private Animation currentAnimation;
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation hurtAnimation;
        private Animation attackAnimation;
        private Animation healAnimation;
        private Animation deathAnimation;
        
        private CharacterType currentCharacter = CharacterType.Abby;
        private KeyboardState previousKeyboard;
        private int velocityY;
        private bool grounded;
        private bool spaceReleased = true;
        private bool attackReleased = true;
        private bool facingRight = true;
        private bool moving;
        private bool jumping;
        private bool attacking;
        private bool dead;
        
        // Abby's Ranged Combat Properties
        private Vector2 projectilePosition;
        private Vector2 projectileVelocity;
        private bool projectileActive;
        private SpriteEffects projectileFlip;
        
        // Nadine's Flower Skill Properties
        private readonly List<MovingFlower> activeFlowers = new List<MovingFlower>();
        private float aoeCooldownTimer;
        
        private HealthSystem healthSystem;
        private float damageCooldownTimer;
        private float deathTimer;

        public Player(Rectangle playerRectangle)
        {
            this.playerRectangle = playerRectangle;
            // Initializes the strict physics bounding box relative to the visual draw frame[cite: 4].
            playerHitbox = new Rectangle(
                playerRectangle.X + HitboxOffsetX,
                playerRectangle.Y + HitboxOffsetY,
                HitboxWidth,
                HitboxHeight
            );
        }

        public Rectangle PlayerRectangle => playerRectangle;
        
        public Rectangle PlayerHitbox
        {
            get => playerHitbox;
            set => playerHitbox = value;
        }

        public CharacterType CurrentCharacter
        {
            get { return currentCharacter; }
            set { currentCharacter = value; }
        }

        public void LoadContent(ContentManager content)
        {
            abbySheet = content.Load<Texture2D>("spritesheets/abby_spritesheet");
            edwardSheet = content.Load<Texture2D>("spritesheets/edward_spritesheet");
            nadineSheet = content.Load<Texture2D>("spritesheets/nadine_spritesheet");
            bulletTexture = content.Load<Texture2D>("assets/bullet");
            flowerAoESheet = content.Load<Texture2D>("assets/flower_aoe");
            
            healthSystem = new HealthSystem(content.Load<Texture2D>("assets/heart"));
            
            // Set Abby as the default starting character
            ChangeCharacter(CharacterType.Abby);
        }

        // === YOUR CONTRIBUTION: CORE GAMEPLAY UPDATE LOOP ===
        // Your central update pipeline coordinates movement, collisions, actions, and animations[cite: 4].
        public void Update(
            GameTime gameTime,
            List<Rectangle> colliders,
            List<Rectangle> spikes,
            List<Rectangle> oreganoItems)
        {
            KeyboardState keyboard = Keyboard.GetState();
            moving = false;

            int moveX = PlayerInput(keyboard);
            HorizontalMovement(moveX, colliders);
            ApplyGravity(colliders);
            SyncDisplayToHitbox();
            UpdateTimers(gameTime);
            SwitchCharacter(keyboard);
            SpikeOnHit(spikes);
            Heal(oreganoItems);
            UpdateProjectile();
            UpdateFlowers(gameTime);
            UpdateAnimation(gameTime);

            previousKeyboard = keyboard;
        }

        private int PlayerInput(KeyboardState keyboard)
        {
            if (dead || currentAnimation == hurtAnimation)
            {
                return 0;
            }

            int moveX = 0;
            if (keyboard.IsKeyDown(Keys.A) && playerRectangle.Left >= 0)
            {
                moveX -= PlayerSpeed;
                facingRight = false;
            }
            if (keyboard.IsKeyDown(Keys.D) && playerRectangle.Right <= WorldWidth)
            {
                moveX += PlayerSpeed;
                facingRight = true;
            }

            moving = moveX != 0;
            Jump(keyboard);
            Attack(keyboard);

            if (keyboard.IsKeyUp(Keys.E))
            {
                attackReleased = true;
            }

            return moveX;
        }

        private void Jump(KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Space) && grounded && spaceReleased)
            {
                velocityY = JumpSpeed;
                grounded = false;
                jumping = true;
                spaceReleased = false;
            }
            if (keyboard.IsKeyUp(Keys.Space))
            {
                spaceReleased = true;
            }
        }

        // === YOUR CONTRIBUTION: CHARACTER-SPECIFIC COMBAT TRIGGERS ===
        // This switch activates Abby's shooting or Nadine's floral AoE skills when attacking[cite: 4].
        private void Attack(KeyboardState keyboard)
        {
            if (!keyboard.IsKeyDown(Keys.E) || attacking)
            {
                return;
            }
            if (currentCharacter == CharacterType.Nadine && aoeCooldownTimer > 0)
            {
                return; 
            }

            if (keyboard.IsKeyDown(Keys.E) && attackReleased)
            {
                attacking = true;
                attackAnimation.Reset();
                currentAnimation = attackAnimation;
                attackReleased = false;

                switch (currentCharacter)
                {
                    case CharacterType.Abby:
                        SpawnProjectile();
                        break;
                    case CharacterType.Nadine:
                        SpawnFlowers();
                        break;
                }
            }
        }

        // === YOUR CONTRIBUTION: AABB PLATFORMING COLLISIONS ===
        // Resolves left/right horizontal wall bounds against custom game tile structures[cite: 4].
        private void HorizontalMovement(int moveX, List<Rectangle> colliders)
        {
            playerHitbox.X += moveX;
            foreach (Rectangle collider in colliders)
            {
                if (!playerHitbox.Intersects(collider))
                {
                    continue;
                }
                if (moveX > 0)
                {
                    playerHitbox.X = collider.Left - playerHitbox.Width;
                }
                else if (moveX < 0)
                {
                    playerHitbox.X = collider.Right;
                }
            }
        }

        // === YOUR CONTRIBUTION: GRAVITY & GROUNDING PHYSICS ===
        // Adds acceleration down each frame. Resolves vertical tile collisions and flags floor grounding[cite: 4].
        private void ApplyGravity(List<Rectangle> colliders)
        {
            velocityY += Gravity;
            playerHitbox.Y += velocityY;
            grounded = false;

            foreach (Rectangle collider in colliders)
            {
                if (!playerHitbox.Intersects(collider))
                {
                    continue;
                }
                if (velocityY > 0) // Falling down onto a platform
                {
                    playerHitbox.Y = collider.Top - playerHitbox.Height;
                    velocityY = 0;
                    grounded = true;
                    jumping = false;
                }
                else if (velocityY < 0) // Hitting a ceiling
                {
                    playerHitbox.Y = collider.Bottom;
                    velocityY = 0;
                }
            }
        }

        // Anchors the actual visible drawing frame position relative to our physical hitbox[cite: 4].
        private void SyncDisplayToHitbox()
        {
            playerRectangle.X = playerHitbox.X - HitboxOffsetX;
            playerRectangle.Y = playerHitbox.Y - HitboxOffsetY;
        }

        // === YOUR CONTRIBUTION: HOTKEY CHARACTER SWAPPING ===
        // Let's players swap character models on the fly (1, 2, 3) if they are still alive[cite: 4].
        private void SwitchCharacter(KeyboardState keyboard)
        {
            TryChangeCharacter(keyboard, Keys.D1, CharacterType.Abby);
            TryChangeCharacter(keyboard, Keys.D2, CharacterType.Edward);
            TryChangeCharacter(keyboard, Keys.D3, CharacterType.Nadine);
        }

        private void TryChangeCharacter(KeyboardState keyboard, Keys key, CharacterType character)
        {
            bool keyPressed = keyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
            if (keyPressed && !healthSystem.IsCharacterDead(character))
            {
                ChangeCharacter(character);
            }
        }

        // === YOUR CONTRIBUTION: SPIKE HAZARD & DAMAGE REACTION ===
        // Triggers a custom hurt reaction and damage penalty when intersecting spike objects[cite: 4].
        // If dead, swap sequence or death timer logic triggers[cite: 4].
        private void SpikeOnHit(List<Rectangle> spikes)
        {
            if (dead || damageCooldownTimer > 0)
            {
                return;
            }  
            foreach (Rectangle spike in spikes)
            {
                if (!playerHitbox.Intersects(spike))
                {
                    continue;
                }
                damageCooldownTimer = DamageCooldownDuration;
                hurtAnimation.Reset();
                currentAnimation = hurtAnimation;
                
                // Triggers death timer sequence if final damage kills, or forces character swap
                healthSystem.TakeDamage(
                    currentCharacter,
                    _ => StartDeath(1.5f),
                    () => StartDeath(-1f)
                );
                break;
            }
        }

        // === YOUR CONTRIBUTION: COLLECTIBLE HEALING ("OREGANO") ===
        // Intersects with oregano items in the game world to heal the current character back to full health[cite: 4].
        private void Heal(List<Rectangle> oreganoItems)
        {
            if (dead || currentAnimation == healAnimation)
            {
                return;
            }
            for (int i = oreganoItems.Count - 1; i >= 0; i--)
            {
                if (!playerHitbox.Intersects(oreganoItems[i]))
                {
                    continue;
                }
                healthSystem.Heal(currentCharacter, 5);
                healAnimation.Reset();
                currentAnimation = healAnimation;
                oreganoItems.RemoveAt(i);
                break;
            }
        }

        private void StartDeath(float timer)
        {
            dead = true;
            attacking = false;
            deathTimer = timer;
            deathAnimation.Reset();  
            currentAnimation = deathAnimation;
        }

        // === YOUR CONTRIBUTION: ABBY'S RANGE SYSTEM (PROJECTILE SPAWNING) ===
        // Spawns bullet items relative to the side she's currently looking at[cite: 4].
        private void SpawnProjectile()
        {
            float startX;
            float horizontalSpeed;
            if (facingRight)
            {
                startX = playerRectangle.X + FrameWidth - 10;
                horizontalSpeed = ProjectileSpeed;
                projectileFlip = SpriteEffects.None;
            }
            else
            {
                startX = playerRectangle.X - 20;
                horizontalSpeed = -ProjectileSpeed;
                projectileFlip = SpriteEffects.FlipHorizontally;
            }
            
            projectilePosition = new Vector2(startX, playerRectangle.Y + 14);
            projectileVelocity = new Vector2(horizontalSpeed, 0);
            projectileActive = true;
        }

        private void UpdateProjectile()
        {
            if (!projectileActive)
            {
                return;
            }
            projectilePosition += projectileVelocity;
            Vector2 playerPosition = new Vector2(playerRectangle.X, playerRectangle.Y);
            
            // Auto deactivates bullet after moving 800 pixels to optimize performance[cite: 4].
            if (Vector2.Distance(playerPosition, projectilePosition) > 800f)
            {
                projectileActive = false;
            }
        }  

        public Rectangle? GetProjectileHitbox()
        {
            if (!projectileActive) return null;
            return new Rectangle((int)projectilePosition.X, (int)projectilePosition.Y, 16, 16);
        }

        public void DeactivateProjectile()
        {
            projectileActive = false;
        }

        // === YOUR CONTRIBUTION: NADINE'S FLOWER SKILL (AOE PATTERNS) ===
        // Instantiates and moves an array of custom flowers to cover a broad surface pattern in her facing direction[cite: 4].
        private void SpawnFlowers()
        {
            aoeCooldownTimer = AoeCooldownDuration;
            int flowerFrameWidth = flowerAoESheet.Width / 7;
            float direction = facingRight ? 1f : -1f;
            int startingOffsetX = facingRight ? FrameWidth : -40;

            foreach (Vector2 offset in FlowerOffsets)
            {
                Vector2 position = new Vector2(
                    playerRectangle.X + startingOffsetX + (offset.X * direction),
                    playerRectangle.Y + 20 + offset.Y
                );
                Vector2 velocity = new Vector2(6f * direction, 0f);
                
                Animation animation = new Animation(
                    flowerAoESheet,
                    flowerFrameWidth,
                    flowerAoESheet.Height,
                    0,
                    7,
                    0.08f,
                    true
                );
                
                activeFlowers.Add(new MovingFlower(position, velocity, animation));
            }
        }

        public List<Rectangle> GetFlowerHitboxes()
        {
            List<Rectangle> hitboxes = new List<Rectangle>();
            foreach (var flower in activeFlowers)
            {
                int width = flowerAoESheet.Width / 7;
                hitboxes.Add(new Rectangle((int)flower.Position.X, (int)flower.Position.Y, width, flowerAoESheet.Height));
            }
            return hitboxes;
        }

        private void UpdateFlowers(GameTime gameTime)
        {
            Vector2 playerPosition = new Vector2(playerRectangle.X, playerRectangle.Y);
            for (int i = activeFlowers.Count - 1; i >= 0; i--)
            {
                MovingFlower flower = activeFlowers[i];
                flower.Position += flower.Velocity;
                flower.Anim.Update(gameTime);
                
                // Cleanup out-of-range elements
                if (Vector2.Distance(playerPosition, flower.Position) > 600f)
                {
                    activeFlowers.RemoveAt(i);
                }
            }
        }

        // === YOUR CONTRIBUTION: EDWARD'S MELEE SWORD SYSTEM ===
        // Generates short-range melee damage blocks in front of Edward depending on his direction[cite: 4].
        public Rectangle SwordHitbox
        {
            get
            {
                if (!attacking) return Rectangle.Empty;
                
                int hitboxX = facingRight ? playerRectangle.Right : playerRectangle.Left + 30;
                int hitboxY = playerRectangle.Y + (playerRectangle.Height / 2);
                
                return new Rectangle(hitboxX - 30, hitboxY, 30, 20);
            }
        }

        private void UpdateTimers(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (damageCooldownTimer > 0)
            {
                damageCooldownTimer -= elapsedSeconds;
            }
            if (aoeCooldownTimer > 0)
            {
                aoeCooldownTimer -= elapsedSeconds;
            }
        }

        // === YOUR CONTRIBUTION: STATE MACHINE / ANIMATION TRANSLATOR ===
        // Evaluates current system indicators (moving, jumping, hurt, attacking) 
        // to assign and run appropriate frame sequences smoothly[cite: 4].
        private void UpdateAnimation(GameTime gameTime)
        {
            if (dead)
            {
                UpdateDeathAnimation(gameTime);
                return;
            }
            if (currentAnimation == hurtAnimation || currentAnimation == healAnimation)
            {
                UpdateTemporaryAnimation(gameTime);
                return;
            }
            if (attacking)
            {
                UpdateAttackAnimation(gameTime);
                return;
            }
            if (jumping)
            {
                currentAnimation = jumpAnimation;
            }
            else if (moving)
            {
                currentAnimation = runAnimation;
            }
            else
            {
                currentAnimation = idleAnimation;  
            }
            currentAnimation.Update(gameTime);
        }

        private void UpdateTemporaryAnimation(GameTime gameTime)
        {
            currentAnimation.Update(gameTime);
            if (!currentAnimation.Finished)
            {
                return;
            }
            idleAnimation.Reset();
            currentAnimation = idleAnimation;
        }

        private void UpdateAttackAnimation(GameTime gameTime)
        {
            currentAnimation = attackAnimation;
            currentAnimation.Update(gameTime);
            if (currentAnimation.Finished)
            {
                attacking = false;
            }
        }

        private void UpdateDeathAnimation(GameTime gameTime)
        {
            currentAnimation = deathAnimation;
            if (!deathAnimation.Finished)
            {
                deathAnimation.Update(gameTime);
                return;
            }
            if (deathTimer <= 0)
            {
                return;
            }
            deathTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (deathTimer <= 0)
            {
                SwitchToFirstLivingCharacter();
            }
        }

        private void SwitchToFirstLivingCharacter()  
        {
            foreach (CharacterType character in System.Enum.GetValues(typeof(CharacterType)))
            {
                if (!healthSystem.IsCharacterDead(character))
                {
                    ChangeCharacter(character);
                    return;
                }
            }
        }

        // === YOUR CONTRIBUTION: CHARACTER ATTRIBUTES CONFIGURATION ===
        // Sets dynamic frame tallies and custom attack speeds depending on who you swapped to[cite: 4].
        // All spritesheet graphics share the same row index rules to keep the file structured[cite: 4].
        private void ChangeCharacter(CharacterType character)
        {
            currentCharacter = character;
            dead = false;
            attacking = false;
            projectileActive = false;
            activeFlowers.Clear();
            
            switch (character)
            {
                case CharacterType.Abby:
                    spriteSheet = abbySheet;
                    CreateAnimations(
                        idleFrames: 4, runFrames: 4, jumpFrames: 5, hurtFrames: 4,
                        attackFrames: 5, healFrames: 12, deathFrames: 9, attackSpeed: 0.09f
                    );
                    break;
                case CharacterType.Edward:
                    spriteSheet = edwardSheet;
                    CreateAnimations(
                        idleFrames: 4, runFrames: 4, jumpFrames: 6, hurtFrames: 4,
                        attackFrames: 7, healFrames: 11, deathFrames: 8, attackSpeed: 0.05f
                    );
                    break;
                case CharacterType.Nadine:
                    spriteSheet = nadineSheet;
                    CreateAnimations(
                        idleFrames: 4, runFrames: 4, jumpFrames: 8, hurtFrames: 6,
                        attackFrames: 14, healFrames: 11, deathFrames: 9, attackSpeed: 0.08f
                    );
                    break;
            }
        }

        private void CreateAnimations(
            int idleFrames, int runFrames, int jumpFrames, int hurtFrames,
            int attackFrames, int healFrames, int deathFrames, float attackSpeed)
        {
            idleAnimation = CreateAnimation(0, idleFrames, 0.20f);
            runAnimation = CreateAnimation(2, runFrames, 0.10f);
            jumpAnimation = CreateAnimation(3, jumpFrames, 0.12f, false);
            hurtAnimation = CreateAnimation(4, hurtFrames, 0.15f, false);
            attackAnimation = CreateAnimation(5, attackFrames, attackSpeed, false);
            healAnimation = CreateAnimation(6, healFrames, 0.10f, false);
            deathAnimation = CreateAnimation(7, deathFrames, 0.15f, false);
            
            idleAnimation.Reset();
            currentAnimation = idleAnimation;
        }

        private Animation CreateAnimation(int row, int frameCount, float speed, bool looping = true)
        {
            return new Animation(spriteSheet, FrameWidth, FrameHeight, row, frameCount, speed, looping);
        }

        public void ResetSpawn(Vector2 spawn)
        {
            playerHitbox.X = (int)spawn.X;  
            playerHitbox.Y = (int)spawn.Y;
            velocityY = 0;
            grounded = false;
            jumping = false;
            SyncDisplayToHitbox();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            SpriteEffects playerFlip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            // Draw player character frame
            spriteBatch.Draw(
                spriteSheet,
                new Vector2(playerRectangle.X, playerRectangle.Y),
                currentAnimation.SourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                1.25f,
                playerFlip,
                0f
            );

            // Draw active bullets if present
            if (projectileActive)
            {
                spriteBatch.Draw(
                    bulletTexture,
                    projectilePosition,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1.1f,
                    projectileFlip,
                    0f
                );
            }

            // Draw active Nadine skill flowers
            foreach (MovingFlower flower in activeFlowers)
            {
                spriteBatch.Draw(
                    flowerAoESheet,
                    flower.Position,  
                    flower.Anim.SourceRectangle,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    playerFlip,
                    0f
                );
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using System.Globalization;

namespace RogueCastle
{
    public abstract class EnemyObj : CharacterObj, IDealsDamageObj
    {
        #region Variables
        protected const int STATE_WANDER = 0;
        protected const int STATE_ENGAGE = 1;
        protected const int STATE_PROJECTILE_ENGAGE = 2;
        protected const int STATE_MELEE_ENGAGE = 3;

        protected float DistanceToPlayer;

        protected GameObj TintablePart = null;
        protected int ProjectileRadius = 0;
        protected int MeleeRadius = 0;
        protected int EngageRadius = 0;
        protected float CooldownTime = 0; // The amount of time it takes before an enemy runs logic again.
        protected int m_damage = 0; // Used for property.
        protected bool CanFallOffLedges = true;
        protected bool AlwaysFaceTarget = false;
        public Vector2 ProjectileScale { get; internal set; }
        protected int ProjectileDamage = 5;
        protected float ProjectileSpeed = 100;
        protected int m_xpValue = 0; // Used for property.
        public bool Procedural { get; set; }
        public bool NonKillable { get; set; }
        public bool GivesLichHealth { get; set; }
        public bool IsDemented { get; set; }

        private int m_level = 0;
        public int Level
        {
            get { return m_level; }
            set
            {
                m_level = value;
                if (m_level < 0)
                    m_level = 0;
            }
        }

        protected int DamageGainPerLevel = 0;
        protected int XPBonusPerLevel = 0;
        protected int HealthGainPerLevel = 0;

        protected float MinMoneyGainPerLevel = 0;
        protected float MaxMoneyGainPerLevel = 0;

        protected float ItemDropChance = 0;
        protected int MinMoneyDropAmount = 0;
        protected int MaxMoneyDropAmount = 0;
        protected float MoneyDropChance = 0;

        protected float m_invincibilityTime = 0.4f; // The base delay speed for invincibility. Four frames.
        protected float InvincibilityTime
        {
            get
            {
                //Console.WriteLine(m_invincibilityTime * m_target.AttackAnimationDelay);
                //return m_invincibilityTime * m_target.AttackAnimationDelay; 
                return m_invincibilityTime;
            }
        }
        protected float m_invincibleCounter = 0;
        protected float m_invincibleCounterProjectile = 0;

        protected float StatLevelHPMod = 0;
        protected float StatLevelDMGMod = 0;
        protected float StatLevelXPMod = 0;

        public float InitialLogicDelay = 0;
        protected float m_initialDelayCounter = 0;

        protected PlayerObj m_target;
        public GameTypes.EnemyDifficulty Difficulty { get; internal set; }
        protected string m_resetSpriteName; // The name of the sprite when its Reset() method is called.

        private int m_numTouchingGrounds; // An int used to allow enemies to walk off ledges if he's touching more than one ground tile.

        private LogicBlock m_walkingLB;
        protected LogicBlock m_currentActiveLB;

        private LogicBlock m_cooldownLB; // The logicblock that is run while the enemy is in a cooldown state.
        private bool m_runCooldown = false;
        private float m_cooldownTimer = 0;
        private int[] m_cooldownParams;

        private Texture2D m_engageRadiusTexture;
        private Texture2D m_projectileRadiusTexture;
        private Texture2D m_meleeRadiusTexture;

        public bool IsProcedural { get; set; }
        public bool SaveToFile = true;
        public byte Type;

        private bool m_isPaused = false;
        protected bool m_bossVersionKilled = false;

        protected List<LogicBlock> logicBlocksToDispose; // A list primarily used for disposing LogicBlocks from classes that inherit from this base class.
        protected TweenObject m_flipTween;
        public bool PlayAnimationOnRestart { get; set; }
        public bool DropsItem { get; set; }
        protected bool m_saveToEnemiesKilledList = true;
        #endregion

        private void InitializeBaseEV()
        {
            Speed = 1;
            MaxHealth = 10;
            EngageRadius = 400;
            ProjectileRadius = 200;
            MeleeRadius = 50;
            KnockBack = Vector2.Zero;
            Damage = 5;
            ProjectileScale = new Vector2(1, 1);
            XPValue = 0;
            ProjectileDamage = 5;
            ItemDropChance = 0.0f; //0.5f;
            MinMoneyDropAmount = 1;
            MaxMoneyDropAmount = 1;
            //InvincibilityTime = 0.4f;
            MoneyDropChance = 0.50f;
            StatLevelHPMod = 0.160f;//0.165f;//0.16f;//0.175f;//0.15f;
            StatLevelDMGMod = 0.0910f;//0.0925f; //0.095f;//0.09f; // 0.1f;
            StatLevelXPMod = 0.025f;
            MinMoneyGainPerLevel = 0.23f;//0.20f;//0.215f;//0.17f;//0.25f; //The rate that the min money will raise.  This is tied into Enemy level. ADDITIVE
            MaxMoneyGainPerLevel = 0.29f;//0.26f;//0.325f;//0.32f;//0.4f; //The rate that max money will raise. This is tied into Enemy level. ADDITIVE
            ForceDraw = true;
        }

        protected override void InitializeEV() { }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new PlayAnimationLogicAction(true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(1));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new PlayAnimationLogicAction(true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(1));

            LogicSet stopWalkingLS = new LogicSet(this);
            stopWalkingLS.AddAction(new StopAnimationLogicAction());
            stopWalkingLS.AddAction(new MoveLogicAction(m_target, true, 0));
            stopWalkingLS.AddAction(new DelayLogicAction(1));

            m_walkingLB.AddLogicSet(walkTowardsLS, walkAwayLS, stopWalkingLS);
        }

        public EnemyObj(string spriteName, PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base(spriteName, physicsManager, levelToAttachTo)
        {
            this.DisableCollisionBoxRotations = true;
            this.Type = EnemyType.None;
            CollisionTypeTag = GameTypes.CollisionType_ENEMY;
            m_target = target;
            m_walkingLB = new LogicBlock();
            m_currentActiveLB = new LogicBlock();
            m_cooldownLB = new LogicBlock();
            logicBlocksToDispose = new List<LogicBlock>();
            m_resetSpriteName = spriteName;
            Difficulty = difficulty;
            ProjectileScale = new Vector2(1, 1);
            this.PlayAnimation(true);
            PlayAnimationOnRestart = true;
            this.OutlineWidth = 2;
            this.GivesLichHealth = true;
            this.DropsItem = true;
        }

        public void SetDifficulty(GameTypes.EnemyDifficulty difficulty, bool reinitialize)
        {
            Difficulty = difficulty;
            if (reinitialize == true)
                Initialize();
        }

        public void Initialize()
        {
            if (TintablePart == null)
                TintablePart = this.GetChildAt(0);

            InitializeBaseEV();
            InitializeEV();


            HealthGainPerLevel = (int)(base.MaxHealth * StatLevelHPMod);
            DamageGainPerLevel = (int)(m_damage * StatLevelDMGMod);
            XPBonusPerLevel = (int)(m_xpValue * StatLevelXPMod);

            // Saving the state of these properties so that they can be reset when ResetState() is called.
            m_internalLockFlip = this.LockFlip;
            m_internalIsWeighted = this.IsWeighted;
            m_internalRotation = this.Rotation;
            m_internalAnimationDelay = this.AnimationDelay;
            m_internalScale = this.Scale;
            InternalFlip = this.Flip;

            // This code is necessary in case InitializeLogic is called more than once on an enemy.
            // The caveat to this is we need to make sure all logic blocks are properly added to the logicBlocksToDispose (although they should be, otherwise we will leak memory).
            foreach (LogicBlock block in logicBlocksToDispose)
                block.ClearAllLogicSets();

            if (m_levelScreen != null)
                InitializeLogic();

            m_initialDelayCounter = InitialLogicDelay;
            //Console.WriteLine(InitialLogicDelay);
            CurrentHealth = MaxHealth;
        }

        public void InitializeDebugRadii()
        {
            if (m_engageRadiusTexture == null)
            {
                // Makes sure you don't create a texture greater than 2048. Textures are Radius * 2 + 2.
                int engageRadius = EngageRadius;
                int projectileRadius = ProjectileRadius;
                int meleeRadius = MeleeRadius;

                if (engageRadius > 1000) engageRadius = 1000;
                if (projectileRadius > 1000) projectileRadius = 1000;
                if (meleeRadius > 1000) meleeRadius = 1000;

                m_engageRadiusTexture = DebugHelper.CreateCircleTexture(engageRadius, m_levelScreen.Camera.GraphicsDevice);
                m_projectileRadiusTexture = DebugHelper.CreateCircleTexture(projectileRadius, m_levelScreen.Camera.GraphicsDevice);
                m_meleeRadiusTexture = DebugHelper.CreateCircleTexture(meleeRadius, m_levelScreen.Camera.GraphicsDevice);
            }
        }

        public void SetPlayerTarget(PlayerObj target)
        {
            m_target = target;
        }

        public void SetLevelScreen(ProceduralLevelScreen levelScreen)
        {
            m_levelScreen = levelScreen;
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_initialDelayCounter > 0)
                m_initialDelayCounter -= elapsedSeconds;
            else
            {
                if (m_invincibleCounter > 0)
                    m_invincibleCounter -= elapsedSeconds;

                if (m_invincibleCounterProjectile > 0)
                    m_invincibleCounterProjectile -= elapsedSeconds;

                if ((m_invincibleCounter <= 0 && m_invincibleCounterProjectile <= 0) && IsWeighted == false)
                {
                    // Resets acceleration for enemies that have no weight (i.e. are flying). Acceleration reduction is eased in to give it a nice smooth look.
                    if (AccelerationY < 0) AccelerationY += 15f;
                    else if (AccelerationY > 0) AccelerationY -= 15f;
                    if (AccelerationX < 0) AccelerationX += 15f;
                    else if (AccelerationX > 0) AccelerationX -= 15f;

                    if (AccelerationY < 3.6f && AccelerationY > -3.6f) AccelerationY = 0;
                    if (AccelerationX < 3.6f && AccelerationX > -3.6f) AccelerationX = 0;
                }

                if (IsKilled == false && IsPaused == false)
                {
                    DistanceToPlayer = CDGMath.DistanceBetweenPts(this.Position, m_target.Position);
                    if (DistanceToPlayer > EngageRadius)
                        State = STATE_WANDER;
                    else if (DistanceToPlayer < EngageRadius && DistanceToPlayer >= ProjectileRadius)
                        State = STATE_ENGAGE;
                    else if (DistanceToPlayer < ProjectileRadius && DistanceToPlayer >= MeleeRadius)
                        State = STATE_PROJECTILE_ENGAGE;
                    else
                        State = STATE_MELEE_ENGAGE;

                    // Count down the cooldown timer.
                    if (m_cooldownTimer > 0 && m_currentActiveLB == m_cooldownLB)
                        m_cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (m_cooldownTimer <= 0 && m_runCooldown == true)
                        m_runCooldown = false;

                    // This needs to go before the next attack is executed, otherwise if the developer ever wants the enemy to face the player then lock direction, it will never happen since he will lock
                    // before turning.
                    if (LockFlip == false)
                    {
                        if (AlwaysFaceTarget == false)
                        {
                            if (this.Heading.X < 0)
                                this.Flip = SpriteEffects.FlipHorizontally;
                            else
                                this.Flip = SpriteEffects.None;
                        }
                        else
                        {
                            if (this.X > m_target.X)
                                this.Flip = SpriteEffects.FlipHorizontally;
                            else
                                this.Flip = SpriteEffects.None;
                        }
                    }

                    //Only run an action check if another one is not currently being run.
                    if (m_currentActiveLB.IsActive == false && m_runCooldown == false)
                    {
                        switch (Difficulty)
                        {
                            case (GameTypes.EnemyDifficulty.BASIC):
                                RunBasicLogic();
                                break;
                            case (GameTypes.EnemyDifficulty.ADVANCED):
                                RunAdvancedLogic();
                                break;
                            case (GameTypes.EnemyDifficulty.EXPERT):
                                RunExpertLogic();
                                break;
                            case (GameTypes.EnemyDifficulty.MINIBOSS):
                                RunMinibossLogic();
                                break;
                        }

                        // Starts the timer.
                        if (m_runCooldown == true && m_currentActiveLB.ActiveLS.Tag == GameTypes.LogicSetType_ATTACK) // The tag makes sure that the current Logic set executed was an attack.
                            m_cooldownTimer = CooldownTime; // Convert to milliseconds.
                    }

                    if (m_currentActiveLB.IsActive == false && m_runCooldown == true && m_cooldownTimer > 0 && m_cooldownLB.IsActive == false)
                    {
                        m_currentActiveLB = m_cooldownLB;
                        m_currentActiveLB.RunLogicBlock(m_cooldownParams);
                    }

                    // Only make the enemy move if they are not invincible. This prevents enemies from moving in the air after getting hit.
                    // Only for Weighted enemies. Non-weighted logic is above.
                    if (IsWeighted == true && (m_invincibleCounter <= 0 && m_invincibleCounterProjectile <= 0))
                    {
                        if (HeadingX > 0) this.HeadingX = 1; // A hack to maintain a horizontal speed.
                        else if (HeadingX < 0) this.HeadingX = -1;
                        this.X += this.HeadingX * (this.CurrentSpeed * elapsedSeconds);
                    }
                    else if (m_isTouchingGround == true || IsWeighted == false) // Only move if you're touching the ground.
                        this.Position += this.Heading * (this.CurrentSpeed * elapsedSeconds);

                    // Code to make sure enemies cannot exit the room.
                    //bool stopMovement = false;
                    //if (IsWeighted == true)
                    {
                        if (this.X < m_levelScreen.CurrentRoom.Bounds.Left)
                            this.X = m_levelScreen.CurrentRoom.Bounds.Left;
                        else if (this.X > m_levelScreen.CurrentRoom.Bounds.Right)
                            this.X = m_levelScreen.CurrentRoom.Bounds.Right;

                        if (this.Y < m_levelScreen.CurrentRoom.Bounds.Top)
                            this.Y = m_levelScreen.CurrentRoom.Bounds.Top;
                        else if (this.Y > m_levelScreen.CurrentRoom.Bounds.Bottom)
                            this.Y = m_levelScreen.CurrentRoom.Bounds.Bottom;
                    }

                    if (m_currentActiveLB == m_cooldownLB) // If Teddy made CoolDownLB == currentActiveLB (which is a no-no), only update one of them.
                        m_currentActiveLB.Update(gameTime);
                    else
                    {
                        m_currentActiveLB.Update(gameTime);
                        m_cooldownLB.Update(gameTime);
                    }
                }
            }

            if (IsWeighted == true)
                CheckGroundCollision();

            // Maintains the enemy's speed in the air so that he can jump onto platforms.
            //if (m_isTouchingGround == false && IsWeighted == true && CurrentSpeed == 0)
            //    this.CurrentSpeed = this.Speed;


            if (this.CurrentHealth <= 0 && IsKilled == false && m_bossVersionKilled == false)
                Kill();

            base.Update(gameTime);
        }

        public void CheckGroundCollisionOld()
        {
            m_numTouchingGrounds = 0;
            float closestGround = int.MaxValue;
            IPhysicsObj collidedObj = null;
            int collisionMarginOfError = 10;
            bool goingDown = true; // A bool that keeps track of whether the player is going down a slope or not.
            //Might not need GroundCollisionRect. Could just use this.Bounds.bottom + 10 or something.
            // Finds the distance between the player and the closest collidable object below him.
            foreach (IPhysicsObj obj in m_levelScreen.PhysicsManager.ObjectList)
            {
                if (obj != this && obj.CollidesTop == true &&
                    (obj.CollisionTypeTag == GameTypes.CollisionType_WALL || obj.CollisionTypeTag == GameTypes.CollisionType_WALL_FOR_PLAYER || obj.CollisionTypeTag == GameTypes.CollisionType_ENEMYWALL
                    || obj.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL)) // only check walls below the player.
                {
                    //if ((obj.Rotation != 0) || //&& obj.Rotation >= -SlopeClimbRotation && obj.Rotation <= SlopeClimbRotation) || // Disabled because turrets go beyond slope rotation.
                    //  (obj.Bounds.Top >= this.Bounds.Bottom - 5 && // -5 margin of error
                    //  ((obj.Bounds.Left <= this.Bounds.Right && obj.Bounds.Right >= this.Bounds.Right && (this.Bounds.Right - obj.Bounds.Left > collisionMarginOfError)) ||
                    //  (this.Bounds.Left <= obj.Bounds.Right && this.Bounds.Right >= obj.Bounds.Right && (obj.Bounds.Right - this.Bounds.Left > collisionMarginOfError)))))
                    if (Math.Abs(obj.Bounds.Top - this.Bounds.Bottom) < collisionMarginOfError)
                    {
                        foreach (CollisionBox box in obj.CollisionBoxes)
                        {
                            if (box.Type == Consts.TERRAIN_HITBOX)
                            {
                                Rectangle rectToCheck = GroundCollisionRect;
                                if (box.AbsRotation != 0)
                                    rectToCheck = RotatedGroundCollisionRect;

                                if (CollisionMath.RotatedRectIntersects(rectToCheck, 0, Vector2.Zero, box.AbsRect, box.AbsRotation, Vector2.Zero))
                                {
                                    m_numTouchingGrounds++;

                                    if (box.AbsParent.Rotation == 0)
                                        goingDown = false;

                                    Vector2 mtd = CollisionMath.RotatedRectIntersectsMTD(GroundCollisionRect, 0, Vector2.Zero, box.AbsRect, box.AbsRotation, Vector2.Zero);
                                    //float distanceBetween = box.AbsRect.Top - (this.Bounds.Bottom);
                                    if (goingDown == true)
                                        goingDown = !CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, box.AbsRect, box.AbsRotation, Vector2.Zero);
                                    float distanceBetween = mtd.Y;
                                    if (closestGround > distanceBetween)
                                    {
                                        closestGround = distanceBetween;
                                        collidedObj = obj;
                                    }
                                }
                            }
                        }
                    }
                }
            }

             // Check to see if the player is touching the ground or not. Margin of error +-2.
                if (closestGround <= 2 && AccelerationY >= 0) // Only check ground if the player is falling.
                    m_isTouchingGround = true;
        }


        private void CheckGroundCollision()
        {
            m_isTouchingGround = false;
            m_numTouchingGrounds = 0;

            if (this.AccelerationY >= 0)// Only check ground collision if falling. Do not check if he's going up, or jumping.
            {
                IPhysicsObj closestTerrain = null;
                float closestFloor = float.MaxValue;

                IPhysicsObj closestRotatedTerrain = null;
                float closestRotatedFloor = float.MaxValue;

                Rectangle elongatedBounds = this.TerrainBounds;
                elongatedBounds.Height += 10; // A rectangle slightly larger than the player's height is needed to check for collisions with.

                //foreach (IPhysicsObj collisionObj in PhysicsMngr.ObjectList)
                foreach (TerrainObj collisionObj in m_levelScreen.CurrentRoom.TerrainObjList)
                {
                    if (collisionObj.Visible == true && collisionObj.IsCollidable == true && collisionObj.CollidesTop == true && collisionObj.HasTerrainHitBox == true &&
                        (collisionObj.CollisionTypeTag == GameTypes.CollisionType_WALL ||
                        collisionObj.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL ||
                        collisionObj.CollisionTypeTag == GameTypes.CollisionType_WALL_FOR_ENEMY ||
                        collisionObj.CollisionTypeTag == GameTypes.CollisionType_ENEMYWALL))
                    {
                        // Do separate checks for sloped and non-sloped terrain.
                        if (collisionObj.Rotation == 0)
                        {
                            Rectangle elongatedNumGroundsBounds = elongatedBounds; // A special bounds purely for checking the number of ground objects you're touching.
                            elongatedNumGroundsBounds.X -= 30;
                            elongatedNumGroundsBounds.Width += 60;
                            Vector2 numGroundMTD = CollisionMath.CalculateMTD(elongatedNumGroundsBounds, collisionObj.Bounds);
                            if (numGroundMTD != Vector2.Zero)
                                m_numTouchingGrounds++;

                            Vector2 mtd = CollisionMath.CalculateMTD(elongatedBounds, collisionObj.Bounds);
                            if (mtd.Y < 0) // Object is below player.
                            {
                                int distance = collisionObj.Bounds.Top - this.Bounds.Bottom;
                                if (distance < closestFloor)
                                {
                                    closestTerrain = collisionObj;
                                    closestFloor = distance;
                                }
                            }
                        }
                        else
                        {
                            Vector2 rotatedMTD = CollisionMath.RotatedRectIntersectsMTD(elongatedBounds, this.Rotation, Vector2.Zero, collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                            if (rotatedMTD != Vector2.Zero)
                                m_numTouchingGrounds++;


                            if (rotatedMTD.Y < 0)
                            {
                                float distance = rotatedMTD.Y;
                                if (distance < closestRotatedFloor)
                                    if (rotatedMTD.Y < 0)
                                    {
                                        closestRotatedTerrain = collisionObj;
                                        closestRotatedFloor = distance;
                                    }
                            }

                            //if (State != STATE_DASHING)
                            {
                                // Code for hooking the player to a slope
                                Rectangle elongatedRect = this.TerrainBounds;
                                elongatedRect.Height += 50;
                                int checkAmount = 15;
                                Vector2 elongatedMTD = CollisionMath.RotatedRectIntersectsMTD(elongatedRect, this.Rotation, Vector2.Zero, collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                                if (elongatedMTD.Y < 0) // Player is standing on a slope because the mtd is telling you to push him up.
                                {
                                    float dist = CDGMath.DistanceBetweenPts(elongatedMTD, Vector2.Zero);
                                    float slopeDistance = (float)(50 - (Math.Sqrt((dist * dist) * 2)));
                                    if (slopeDistance > 0 && slopeDistance < checkAmount) // This checks to see if the player is close enough to the ground to latch to it.
                                        this.Y += slopeDistance;
                                    float distance = rotatedMTD.Y;
                                    if (distance < closestRotatedFloor)
                                    {
                                        closestRotatedTerrain = collisionObj;
                                        closestRotatedFloor = distance;
                                    }
                                }
                            }
                        }
                    }

                    if (closestTerrain != null)
                        m_isTouchingGround = true;

                    if (closestRotatedTerrain != null)
                    {
                        //HookToSlope(closestRotatedTerrain);
                        m_isTouchingGround = true;
                    }

                    //if (this is EnemyObj_Zombie)
                    //Console.WriteLine(m_numTouchingGrounds);
                }
            }
        }

        private void HookToSlope(IPhysicsObj collisionObj)
        {
            this.UpdateCollisionBoxes();

            // Code for hooking the player to a slope
            Rectangle elongatedRect = this.TerrainBounds;
            elongatedRect.Height += 100;
            //int checkAmount = 15;
            float x1 = this.X;

            Vector2 elongatedMTD = CollisionMath.RotatedRectIntersectsMTD(elongatedRect, this.Rotation, Vector2.Zero, collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
            if (elongatedMTD.Y < 0) // Player is standing on a slope because the mtd is telling you to push him up.
            {
                bool checkCollision = false;
                float y1 = float.MaxValue;
                Vector2 pt1, pt2;
                if (collisionObj.Width > collisionObj.Height) // If rotated objects are done correctly.
                {
                    pt1 = CollisionMath.UpperLeftCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                    pt2 = CollisionMath.UpperRightCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);

                    //if (collisionObj.Rotation == 45)
                    if (collisionObj.Rotation > 0)// ROTCHECK
                        x1 = this.TerrainBounds.Left;
                    else
                        x1 = this.TerrainBounds.Right;

                    if (x1 > pt1.X && x1 < pt2.X)
                        checkCollision = true;
                }
                else // If rotated objects are done Teddy's incorrect way.
                {
                    //if (collisionObj.Rotation == 45)
                    if (collisionObj.Rotation > 0) // ROTCHECK
                    {
                        pt1 = CollisionMath.LowerLeftCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                        pt2 = CollisionMath.UpperLeftCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                        x1 = this.TerrainBounds.Right;
                        if (x1 > pt1.X && x1 < pt2.X)
                            checkCollision = true;
                    }
                    else
                    {
                        pt1 = CollisionMath.UpperRightCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                        pt2 = CollisionMath.LowerRightCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                        x1 = this.TerrainBounds.Left;
                        if (x1 > pt1.X && x1 < pt2.X)
                            checkCollision = true;
                    }
                }

                if (checkCollision == true)
                {
                    float u = pt2.X - pt1.X;
                    float v = pt2.Y - pt1.Y;
                    float x = pt1.X;
                    float y = pt1.Y;

                    y1 = y + (x1 - x) * (v / u);
                    y1 -= this.TerrainBounds.Bottom - this.Y - 2; // Up by 2 to ensure collision response doesn't kick in.
                    this.Y = (int)y1;
                }
            }
        }

        protected void SetCooldownLogicBlock(LogicBlock cooldownLB, params int[] percentage)
        {
            m_cooldownLB = cooldownLB;
            m_cooldownParams = percentage;
        }

        protected void RunLogicBlock(bool runCDLogicAfterward, LogicBlock block, params int[] percentage)
        {
            m_runCooldown = runCDLogicAfterward;
            m_currentActiveLB = block;
            m_currentActiveLB.RunLogicBlock(percentage);
        }

        protected virtual void RunBasicLogic()
        {
            //m_currentActiveLB = m_walkingLB;
            //m_currentActiveLB.RunLogicBlock(50, 30, 20);
        }

        protected virtual void RunAdvancedLogic() { RunBasicLogic(); }

        protected virtual void RunExpertLogic() { RunBasicLogic(); }

        protected virtual void RunMinibossLogic() { RunBasicLogic(); }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            IPhysicsObj otherBoxParent = otherBox.AbsParent as IPhysicsObj;
            Vector2 mtd = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);

            if (collisionResponseType == Consts.COLLISIONRESPONSE_FIRSTBOXHIT && (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_PLAYER || otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL || (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL && this.IsWeighted == true))
                && ((otherBox.AbsParent is ProjectileObj == false && m_invincibleCounter <= 0) || (otherBox.AbsParent is ProjectileObj && (m_invincibleCounterProjectile <= 0 || (otherBox.AbsParent as ProjectileObj).IgnoreInvincibleCounter == true))))
            {
                // Don't do anything if the enemy is demented
                if (IsDemented == true)
                {
                    m_invincibleCounter = InvincibilityTime;
                    m_invincibleCounterProjectile = InvincibilityTime;
                    m_levelScreen.ImpactEffectPool.DisplayQuestionMark(new Vector2(this.X, this.Bounds.Top));
                   return;
                }

                // Player has hit enemy.
                int damage = (otherBoxParent as IDealsDamageObj).Damage;
                bool isPlayer = false; // Keeps track of whether or not it is the player hitting the enemy.

                //if (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_PLAYER)
                if (otherBoxParent == m_target)
                {
                    if (CDGMath.RandomFloat(0, 1) <= m_target.TotalCritChance && this.NonKillable == false && otherBoxParent == m_target)
                    {
                        //m_levelScreen.TextManager.DisplayStringText("Critical!", Color.Red, new Vector2(this.X, this.Bounds.Top - 65));
                        m_levelScreen.ImpactEffectPool.DisplayCriticalText(new Vector2(this.X, this.Bounds.Top));
                        damage = (int)(damage * m_target.TotalCriticalDamage); // Critical hit!
                    }
                    isPlayer = true;
                }

                ProjectileObj projectile = otherBox.AbsParent as ProjectileObj;
                if (projectile != null)
                {
                    // m_lastDamageSource is for projectiles only.
                    m_invincibleCounterProjectile = InvincibilityTime;

                    if (projectile.DestroysWithEnemy == true && this.NonKillable == false)
                        projectile.RunDestroyAnimation(false);
                }

                Point intersectPt = Rectangle.Intersect(thisBox.AbsRect, otherBox.AbsRect).Center;
                if (thisBox.AbsRotation != 0 || otherBox.AbsRotation != 0)
                    intersectPt = Rectangle.Intersect(thisBox.AbsParent.Bounds, otherBox.AbsParent.Bounds).Center;
                Vector2 impactPosition = new Vector2(intersectPt.X, intersectPt.Y);

                if (projectile == null || (projectile != null && projectile.Spell != SpellType.Shout))
                {
                    if (projectile != null || (otherBoxParent.CollisionTypeTag != GameTypes.CollisionType_GLOBAL_DAMAGE_WALL ||
                        (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL && this.IsWeighted == true))) // Make sure flying enemies don't hit global damage walls.
                    HitEnemy(damage, impactPosition, isPlayer);
                }
                else if (projectile != null && projectile.Spell == SpellType.Shout) // Fus roh dah logic.
                {
                    if (CanBeKnockedBack == true && IsPaused == false)
                    {
                        //Knock the enemy to the left if he's to the left of the player.
                        this.CurrentSpeed = 0;

                        float knockbackMod = GameEV.BARBARIAN_SHOUT_KNOCKBACK_MOD;
                        if (this.KnockBack == Vector2.Zero)
                        {
                            if (this.X < m_target.X)
                                AccelerationX = -m_target.EnemyKnockBack.X * knockbackMod;
                            else
                                AccelerationX = m_target.EnemyKnockBack.X * knockbackMod;
                            AccelerationY = -m_target.EnemyKnockBack.Y * knockbackMod;
                        }
                        else
                        {
                            if (this.X < m_target.X)
                                AccelerationX = -KnockBack.X * knockbackMod;
                            else
                                AccelerationX = KnockBack.X * knockbackMod;
                            AccelerationY = -KnockBack.Y * knockbackMod;
                        }
                    }
                }

                // The invincibility counter is for the player only.
                if (otherBoxParent == m_target)
                    m_invincibleCounter = InvincibilityTime; // Convert to milliseconds.

                //m_levelScreen.ProjectileManager.DestroyProjectile(projectile);
            }

            // Only do terrain collision with actual terrain, since the player also has a terrain collision box.
            if (collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN && 
                (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_WALL || 
                otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_WALL_FOR_ENEMY || 
                otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL) 
                && this.CollisionTypeTag != GameTypes.CollisionType_ENEMYWALL)
            {
                // Enemy is colliding with a wall, so stop animating it.
                if (this.CurrentSpeed != 0 && mtd.X != 0)
                {
                    if (Math.Abs(mtd.X) > 10 && ((mtd.X > 0 && otherBoxParent.CollidesRight == true) ||( mtd.X < 0 && otherBoxParent.CollidesLeft == true)))
                        this.CurrentSpeed = 0;
                    //this.StopAnimation(); // OR don't stop it.
                    //this.PlayAnimation(0, 0, false); // Rather than stopping the animation, set it to the first frame.
                }

                //// Enemy is near an edge and is not allowed to fall off.
                if (m_numTouchingGrounds <= 1 && this.CurrentSpeed != 0 && mtd.Y < 0 && CanFallOffLedges == false)
                {
                    if (this.Bounds.Left < otherBoxParent.Bounds.Left && this.HeadingX < 0)
                    {
                        this.X = otherBoxParent.Bounds.Left + (this.AbsX - this.Bounds.Left);
                        this.CurrentSpeed = 0;
                        //this.StopAnimation();
                    }
                    else if (this.Bounds.Right > otherBoxParent.Bounds.Right && this.HeadingX > 0)
                    {
                        this.X = otherBoxParent.Bounds.Right - (this.Bounds.Right - this.AbsX);
                        this.CurrentSpeed = 0;
                        //this.StopAnimation();
                    }

                    m_isTouchingGround = true;
                }

                // If enemy is knocked back and touches the ground, stop moving him backwards.
                //if (this.AccelerationX != 0 && this.AccelerationY > 0 && CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect).Y != 0)
                if (this.AccelerationX != 0 && m_isTouchingGround == true)
                    this.AccelerationX = 0;
                //else
                //{
                //    Console.WriteLine(AccelerationX + " " + (AccelerationY > 0) + " " + (CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect).Y != 0));
                //}

                bool disableCollision = false;
                if (Math.Abs(mtd.X) < 10 && mtd.X != 0 && Math.Abs(mtd.Y) < 10 && mtd.Y != 0)
                    disableCollision = true;

                // Used to make sure enemy doesn't hook to 1-way ground above him if he's already standing on flooring.
                if (m_isTouchingGround == true && otherBoxParent.CollidesBottom == false && otherBoxParent.CollidesTop == true && otherBoxParent.TerrainBounds.Top < this.TerrainBounds.Bottom - 30)
                    disableCollision = true;

                // Special handling to prevent enemy from hooking to the top of tunnels.
                if (otherBoxParent.CollidesRight == false && otherBoxParent.CollidesLeft == false && otherBoxParent.CollidesTop == true && otherBoxParent.CollidesBottom == true)
                    disableCollision = true;

                // Disables collision on bookcases and stuff for enemies.
                //if (otherBoxParent is TerrainObj == false)
                //    disableCollision = true;

                Vector2 rotatedMTDPos = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
                if (disableCollision == false)
                    base.CollisionResponse(thisBox, otherBox, collisionResponseType);

                if (rotatedMTDPos.Y < 0 && otherBox.AbsRotation != 0 && IsWeighted == true) // Code to prevent player from sliding down rotated objects.
                    this.X -= rotatedMTDPos.X;
            }
        }

        public virtual void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            if (Game.PlayerStats.GodMode == true)
                damage = 9999999;

            if (m_target != null && m_target.CurrentHealth > 0)
            {
                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
                Blink(Color.Red, 0.1f);
                this.m_levelScreen.ImpactEffectPool.DisplayEnemyImpactEffect(collisionPt);

                if (isPlayer == true && (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2))
                {
                    this.CurrentHealth -= damage;
                    m_target.CurrentMana += (int)(damage * GameEV.SPELLSWORD_ATTACK_MANA_CONVERSION);
                    m_levelScreen.TextManager.DisplayNumberText(damage, Color.Red, new Vector2(this.X, this.Bounds.Top));
                    m_levelScreen.TextManager.DisplayNumberStringText((int)(damage * GameEV.SPELLSWORD_ATTACK_MANA_CONVERSION), "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.RoyalBlue, new Vector2(m_target.X, m_target.Bounds.Top - 30));
                }
                else
                {
                    this.CurrentHealth -= damage;
                    m_levelScreen.TextManager.DisplayNumberText(damage, Color.Red, new Vector2(this.X, this.Bounds.Top));
                }

                if (isPlayer == true) // Only perform these checks if the object hitting the enemy is the player.
                {
                    m_target.NumSequentialAttacks++;

                    if (m_target.IsAirAttacking == true)
                    {
                        m_target.IsAirAttacking = false; // Only allow one object to perform upwards air knockback on the player.
                        m_target.AccelerationY = -m_target.AirAttackKnockBack;
                        m_target.NumAirBounces++;
                    }
                }

                if (CanBeKnockedBack == true && IsPaused == false && (Game.PlayerStats.Traits.X != TraitType.Hypogonadism && Game.PlayerStats.Traits.Y != TraitType.Hypogonadism))
                {
                    //Knock the enemy to the left if he's to the left of the player.
                    this.CurrentSpeed = 0;

                    float knockbackMod = 1;
                    if (Game.PlayerStats.Traits.X == TraitType.Hypergonadism || Game.PlayerStats.Traits.Y == TraitType.Hypergonadism)
                        knockbackMod = GameEV.TRAIT_HYPERGONADISM;

                    if (this.KnockBack == Vector2.Zero)
                    {
                        if (this.X < m_target.X)
                            AccelerationX = -m_target.EnemyKnockBack.X * knockbackMod;
                        else
                            AccelerationX = m_target.EnemyKnockBack.X * knockbackMod;
                        AccelerationY = -m_target.EnemyKnockBack.Y * knockbackMod;
                    }
                    else
                    {
                        if (this.X < m_target.X)
                            AccelerationX = -KnockBack.X * knockbackMod;
                        else
                            AccelerationX = KnockBack.X * knockbackMod;
                        AccelerationY = -KnockBack.Y * knockbackMod;
                    }
                }

                m_levelScreen.SetLastEnemyHit(this);
            }
        }

        // Sets the enemy to a killed state, without all the jazz of SFX, animations, and giving you XP.
        public void KillSilently()
        {
            base.Kill(false);
        }

        public override void Kill(bool giveXP = true)
        {
            // Get health back for killing enemy.
            int vampirism = m_target.TotalVampBonus;
            if (vampirism > 0)
            {
                m_target.CurrentHealth += vampirism;
                this.m_levelScreen.TextManager.DisplayNumberStringText(vampirism, "LOC_ID_SKILL_SCREEN_14" /*"hp"*/, Color.LightGreen, new Vector2(m_target.X, m_target.Bounds.Top - 60));
            }

            // Get mana back for killing enemy.
            if (m_target.ManaGain > 0)
            {
                m_target.CurrentMana += m_target.ManaGain;
                m_levelScreen.TextManager.DisplayNumberStringText((int)m_target.ManaGain, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.RoyalBlue, new Vector2(m_target.X, m_target.Bounds.Top - 90));
            }

            if (Game.PlayerStats.SpecialItem == SpecialItemType.GoldPerKill)
            {
                m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Coin, ItemDropType.CoinAmount);
                m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Coin, ItemDropType.CoinAmount);
                //m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Coin, ItemDropType.CoinAmount);
            }

            this.m_levelScreen.KillEnemy(this);
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Enemy_Death");
            //SoundManager.Play3DSound(this, Game.ScreenManager.Player, "EnemyDeath2", "EnemyDeath3", "EnemyDeath4", "EnemyDeath5", "EnemyDeath6"
            //    , "EnemyDeath7", "EnemyDeath8", "EnemyDeath9");

            if (DropsItem == true)
            {
                // Chance of dropping health or mana.  Or in the case of a chicken, health.
                if (this.Type == EnemyType.Chicken)
                    m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                else if (CDGMath.RandomInt(1, 100) <= GameEV.ENEMY_ITEMDROP_CHANCE)
                {
                    if (CDGMath.RandomPlusMinus() < 0)
                        m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                    else
                        m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                }

                if (CDGMath.RandomFloat(0, 1) <= MoneyDropChance) // 100% chance of dropping money.
                {
                    int goldAmount = CDGMath.RandomInt(MinMoneyDropAmount, MaxMoneyDropAmount) * 10 + (int)((CDGMath.RandomFloat(MinMoneyGainPerLevel, MaxMoneyGainPerLevel) * this.Level) * 10);

                    int numBigDiamonds = (int)(goldAmount / ItemDropType.BigDiamondAmount);
                    goldAmount -= numBigDiamonds * ItemDropType.BigDiamondAmount;

                    int numDiamonds = (int)(goldAmount / ItemDropType.DiamondAmount);
                    goldAmount -= numDiamonds * ItemDropType.DiamondAmount;

                    int numMoneyBags = (int)(goldAmount / ItemDropType.MoneyBagAmount);
                    goldAmount -= numMoneyBags * ItemDropType.MoneyBagAmount;

                    int numCoins = goldAmount / ItemDropType.CoinAmount;

                    for (int i = 0; i < numBigDiamonds; i++)
                        m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.BigDiamond, ItemDropType.BigDiamondAmount);

                    for (int i = 0; i < numDiamonds; i++)
                        m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Diamond, ItemDropType.DiamondAmount);

                    for (int i = 0; i < numMoneyBags; i++)
                        m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.MoneyBag, ItemDropType.MoneyBagAmount);

                    for (int i = 0; i < numCoins; i++)
                        m_levelScreen.ItemDropManager.DropItem(this.Position, ItemDropType.Coin, ItemDropType.CoinAmount);
                }
            }

            if (m_currentActiveLB.IsActive)
                m_currentActiveLB.StopLogicBlock();
            m_levelScreen.ImpactEffectPool.DisplayDeathEffect(this.Position);

            if ((Game.PlayerStats.Class == ClassType.Lich || Game.PlayerStats.Class == ClassType.Lich2) && this.GivesLichHealth == true)
            {
                int lichHealthGain = 0;
                int playerLevel = Game.PlayerStats.CurrentLevel;
                int enemyLevel = (int)(this.Level * LevelEV.ENEMY_LEVEL_FAKE_MULTIPLIER);

                if (playerLevel < enemyLevel)
                    lichHealthGain = GameEV.LICH_HEALTH_GAIN_PER_KILL;
                else if (playerLevel >= enemyLevel)
                    lichHealthGain = GameEV.LICH_HEALTH_GAIN_PER_KILL_LESSER;

                int maxPlayerHealth = (int)Math.Round(((m_target.BaseHealth + m_target.GetEquipmentHealth() +
                        (Game.PlayerStats.BonusHealth * GameEV.ITEM_STAT_MAXHP_AMOUNT) +
                        SkillSystem.GetSkill(SkillType.Health_Up).ModifierAmount +
                        SkillSystem.GetSkill(SkillType.Health_Up_Final).ModifierAmount)) * GameEV.LICH_MAX_HP_OFF_BASE, MidpointRounding.AwayFromZero);

                // Only give health if the lich hasn't reach max health allowed.
                if (m_target.MaxHealth + lichHealthGain < maxPlayerHealth)
                {
                    Game.PlayerStats.LichHealth += lichHealthGain;
                    m_target.CurrentHealth += lichHealthGain;
                    m_levelScreen.TextManager.DisplayNumberStringText(lichHealthGain, "LOC_ID_PLAYER_OBJ_3" /*"max hp"*/, Color.LightGreen, new Vector2(m_target.X, m_target.Bounds.Top - 30));
                }
                
                //Game.PlayerStats.LichHealth += GameEV.LICH_HEALTH_GAIN_PER_KILL;
                //m_target.CurrentHealth += GameEV.LICH_HEALTH_GAIN_PER_KILL;
                //m_levelScreen.TextManager.DisplayNumberStringText(GameEV.LICH_HEALTH_GAIN_PER_KILL, "LOC_ID_PLAYER_OBJ_3" /*"max hp"*/, Color.LightGreen, new Vector2(m_target.X, m_target.Bounds.Top - 30));
            }

            Game.PlayerStats.NumEnemiesBeaten++;

            if (m_saveToEnemiesKilledList == true)
            {
                // Incrementing the number of times you've killed a specific type of enemy.
                Vector4 enemyData = Game.PlayerStats.EnemiesKilledList[(int)this.Type];
                switch (this.Difficulty)
                {
                    case (GameTypes.EnemyDifficulty.BASIC):
                        enemyData.X += 1;
                        break;
                    case (GameTypes.EnemyDifficulty.ADVANCED):
                        enemyData.Y += 1;
                        break;
                    case (GameTypes.EnemyDifficulty.EXPERT):
                        enemyData.Z += 1;
                        break;
                    case (GameTypes.EnemyDifficulty.MINIBOSS):
                        enemyData.W += 1;
                        break;
                }
                Game.PlayerStats.EnemiesKilledList[(int)this.Type] = enemyData;
            }

            if (giveXP == true && this.Type == EnemyType.Chicken)
                GameUtil.UnlockAchievement("FEAR_OF_CHICKENS");

            base.Kill();
        }

        public void PauseEnemy(bool forcePause = false)
        {
            if ((IsPaused == false && IsKilled == false && m_bossVersionKilled == false) || forcePause == true)
            {
                m_isPaused = true;
                this.DisableAllWeight = true;
                this.PauseAnimation();
            }
        }

        public void UnpauseEnemy(bool forceUnpause = false)
        {
            if ((IsPaused == true && IsKilled == false && m_bossVersionKilled == false) || forceUnpause == true)
            {
                m_isPaused = false;
                this.DisableAllWeight = false;
                this.ResumeAnimation();
            }
        }

        public void DrawDetectionRadii(Camera2D camera)
        {
            camera.Draw(m_engageRadiusTexture, new Vector2(this.Position.X - EngageRadius, this.Position.Y - EngageRadius), Color.Red * 0.5f);
            camera.Draw(m_projectileRadiusTexture, new Vector2(this.Position.X - ProjectileRadius, this.Position.Y - ProjectileRadius), Color.Blue * 0.5f);
            camera.Draw(m_meleeRadiusTexture, new Vector2(this.Position.X - MeleeRadius, this.Position.Y - MeleeRadius), Color.Green * 0.5f);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                if (m_currentActiveLB.IsActive)
                    m_currentActiveLB.StopLogicBlock();
                m_currentActiveLB = null;

                // Disposing all logicblocks creating by classes that inherit from EnemyObj.
                foreach (LogicBlock disposeLB in logicBlocksToDispose)
                    disposeLB.Dispose();

                for (int i = 0; i < logicBlocksToDispose.Count; i++)
                    logicBlocksToDispose[i] = null;
                logicBlocksToDispose.Clear();
                logicBlocksToDispose = null;

                m_target = null; // Do not dispose player. He will dispose himself.
                m_walkingLB.Dispose();
                m_walkingLB = null;

                if (m_cooldownLB.IsActive)
                    m_cooldownLB.StopLogicBlock();
                m_cooldownLB.Dispose();
                m_cooldownLB = null;

                if (m_engageRadiusTexture != null)
                    m_engageRadiusTexture.Dispose();
                m_engageRadiusTexture = null;

                if (m_engageRadiusTexture != null)
                    m_projectileRadiusTexture.Dispose();
                m_projectileRadiusTexture = null;

                if (m_engageRadiusTexture != null)
                    m_meleeRadiusTexture.Dispose();
                m_meleeRadiusTexture = null;

                if (m_cooldownParams != null)
                    Array.Clear(m_cooldownParams, 0, m_cooldownParams.Length);
                m_cooldownParams = null;

                TintablePart = null;
                m_flipTween = null;

                base.Dispose();
            }
        }

        public override void Reset()
        {
            if (m_currentActiveLB.IsActive == true)
                m_currentActiveLB.StopLogicBlock();

            if (m_cooldownLB.IsActive == true)
                m_cooldownLB.StopLogicBlock();

            this.m_invincibleCounter = 0;
            this.m_invincibleCounterProjectile = 0;
            this.State = STATE_WANDER;
            this.ChangeSprite(m_resetSpriteName);
            if (this.PlayAnimationOnRestart == true)
                this.PlayAnimation(true);
            m_initialDelayCounter = InitialLogicDelay;
            UnpauseEnemy(true);
            m_bossVersionKilled = false;
            m_blinkTimer = 0;
            base.Reset();
        }

        public virtual void ResetState() // The state to reset the enemy to when transitioning rooms.
        {
            if (m_currentActiveLB.IsActive == true)
                m_currentActiveLB.StopLogicBlock();

            if (m_cooldownLB.IsActive == true)
                m_cooldownLB.StopLogicBlock();

            this.m_invincibleCounter = 0;
            m_invincibleCounterProjectile = 0;
            this.State = STATE_WANDER;
            if (this.Type != EnemyType.Portrait) // Hack to make sure portraits don't change their picture.
                this.ChangeSprite(m_resetSpriteName);
            if (this.PlayAnimationOnRestart == true)
                this.PlayAnimation(true);
            m_initialDelayCounter = InitialLogicDelay;

            LockFlip = m_internalLockFlip;
            Flip = InternalFlip;
            this.AnimationDelay = m_internalAnimationDelay;
            UnpauseEnemy(true);

            CurrentHealth = MaxHealth; // Refresh the enemy's health if you leave the room.

            m_blinkTimer = 0;
        }

        protected float ParseTagToFloat(string key)
        {
            if (this.Tag != "")
            {
                int firstIndex = Tag.IndexOf(key + ":") + key.Length + 1; // Find the end of the word.
                int length = Tag.IndexOf(",", firstIndex); // Find the first comma.
                if (length == -1)
                    length = Tag.Length; // If you still can't find anything, go to the end of the string.
                try
                {
                    // flibit didn't like this
                    // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    // ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    CultureInfo ci = CultureInfo.InvariantCulture;
                    return float.Parse(Tag.Substring(firstIndex, length - firstIndex), NumberStyles.Any, ci);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not parse key:" + key + " with string:" + Tag + ".  Original Error: " + ex.Message);
                    return 0;
                }
            }
            return 0;
        }

        protected string ParseTagToString(string key)
        {
            int firstIndex = Tag.IndexOf(key + ":") + key.Length + 1; // Find the end of the word.
            int length = Tag.IndexOf(",", firstIndex); // Find the first comma.
            if (length == -1)
                length = Tag.Length; // If you still can't find anything, go to the end of the string.
            return Tag.Substring(firstIndex, length - firstIndex);
        }

        //public override void Draw(Camera2D camera)
        //{
        //    camera.Draw(Game.GenericTexture, GroundCollisionRect, Color.White);
        //    base.Draw(camera);
        //}

        protected override GameObj CreateCloneInstance()
        {
            return EnemyBuilder.BuildEnemy(this.Type, m_target, null, m_levelScreen, this.Difficulty);
            //return EnemyBuilder.BuildEnemy(this.Type, m_target, PhysicsMngr, m_levelScreen, this.Difficulty);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            EnemyObj clone = obj as EnemyObj;
            clone.IsProcedural = this.IsProcedural;
            clone.InitialLogicDelay = this.InitialLogicDelay;
            clone.NonKillable = this.NonKillable;
            clone.GivesLichHealth = this.GivesLichHealth;
            clone.DropsItem = this.DropsItem;
            clone.IsDemented = this.IsDemented;

            //clone.InternalFlip = this.InternalFlip; Defined in CharacterObj
        }

        private Rectangle GroundCollisionRect
        {
            get { return new Rectangle((int)this.Bounds.X - 10, (int)this.Bounds.Y, this.Width + 20, this.Height + 10); }
        }

        private Rectangle RotatedGroundCollisionRect
        {
            get { return new Rectangle((int)this.Bounds.X, (int)this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + 40); }
        }

        public override Rectangle Bounds
        {
            get 
            {
                if (this.IsWeighted == true)
                    return this.TerrainBounds;
                else
                    return base.Bounds;
            }
        }

        public override int MaxHealth
        {
            get { return base.MaxHealth + (HealthGainPerLevel * (Level - 1)); }
            internal set { base.MaxHealth = value; }
        }

        public int Damage 
        { 
            get { return m_damage + (DamageGainPerLevel * (Level - 1)); }
            internal set { m_damage = value; }
        }

        public int XPValue 
        {
            get { return m_xpValue + (XPBonusPerLevel * (Level - 1)); }
            internal set { m_xpValue = value; }
        }

        public string ResetSpriteName
        {
            get { return m_resetSpriteName; }
        }

        public new bool IsPaused
        {
            get { return m_isPaused; }
        }

        public override SpriteEffects Flip
        {
            get { return base.Flip; }
            set
            {
                if (Game.PlayerStats.Traits.X == TraitType.StereoBlind || Game.PlayerStats.Traits.Y == TraitType.StereoBlind)
                {
                    if (Flip != value && m_levelScreen != null)
                    {
                        if (m_flipTween != null && m_flipTween.TweenedObject == this && m_flipTween.Active == true)
                            m_flipTween.StopTween(false);

                        //float storedX = m_internalScale.X;
                        float storedX = this.ScaleY; // Uses ScaleY to support flip for shrunken blobs.
                        this.ScaleX = 0;
                        m_flipTween = Tween.To(this, 0.15f, Tweener.Tween.EaseNone, "ScaleX", storedX.ToString());
                    }
                }
                base.Flip = value;
            }
        }
    }
}

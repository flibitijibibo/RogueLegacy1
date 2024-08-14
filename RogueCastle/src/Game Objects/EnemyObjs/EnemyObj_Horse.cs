using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class EnemyObj_Horse : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private LogicBlock m_turnLB = new LogicBlock();

        private int m_wallDistanceCheck = 430;

        private float m_collisionCheckTimer = 0.5f;
        private float m_fireDropTimer = 0.5f;
        private float m_fireDropInterval = 0.075f;
        private float m_fireDropLifespan = 0.75f;//2;

        private int m_numFireShieldObjs = 2;//4;//2;
        private float m_fireDistance = 110;
        private float m_fireRotationSpeed = 1.5f;//0;//1.5f;
        private float m_fireShieldScale = 2.5f;
        private List<ProjectileObj> m_fireShieldList;

        private FrameSoundObj m_gallopSound;

        private bool m_turning = false; // Ensures the horse doesn't turn multiple times in a single update.

        protected override void InitializeEV()
        {
            LockFlip = true;
            #region Basic Variables - General
            Name = EnemyEV.Horse_Basic_Name;
            LocStringID = EnemyEV.Horse_Basic_Name_locID;

            MaxHealth = EnemyEV.Horse_Basic_MaxHealth;
            Damage = EnemyEV.Horse_Basic_Damage;
            XPValue = EnemyEV.Horse_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Horse_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Horse_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Horse_Basic_DropChance;

            Speed = EnemyEV.Horse_Basic_Speed;
            TurnSpeed = EnemyEV.Horse_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Horse_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Horse_Basic_Jump;
            CooldownTime = EnemyEV.Horse_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Horse_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Horse_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Horse_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Horse_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Horse_Basic_IsWeighted;

            Scale = EnemyEV.Horse_Basic_Scale;
            ProjectileScale = EnemyEV.Horse_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Horse_Basic_Tint;

            MeleeRadius = EnemyEV.Horse_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Horse_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Horse_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Horse_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Horse_Miniboss_Name;
                    LocStringID = EnemyEV.Horse_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Horse_Miniboss_MaxHealth;
                    Damage = EnemyEV.Horse_Miniboss_Damage;
                    XPValue = EnemyEV.Horse_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Horse_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Horse_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Horse_Miniboss_DropChance;

                    Speed = EnemyEV.Horse_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Horse_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Horse_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Horse_Miniboss_Jump;
                    CooldownTime = EnemyEV.Horse_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Horse_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Horse_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Horse_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Horse_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Horse_Miniboss_IsWeighted;

                    Scale = EnemyEV.Horse_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Horse_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Horse_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Horse_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Horse_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Horse_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Horse_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Horse_Expert_Name;
                    LocStringID = EnemyEV.Horse_Expert_Name_locID;

                    MaxHealth = EnemyEV.Horse_Expert_MaxHealth;
                    Damage = EnemyEV.Horse_Expert_Damage;
                    XPValue = EnemyEV.Horse_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Horse_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Horse_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Horse_Expert_DropChance;

                    Speed = EnemyEV.Horse_Expert_Speed;
                    TurnSpeed = EnemyEV.Horse_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Horse_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Horse_Expert_Jump;
                    CooldownTime = EnemyEV.Horse_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Horse_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Horse_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Horse_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Horse_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Horse_Expert_IsWeighted;

                    Scale = EnemyEV.Horse_Expert_Scale;
                    ProjectileScale = EnemyEV.Horse_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Horse_Expert_Tint;

                    MeleeRadius = EnemyEV.Horse_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Horse_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Horse_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Horse_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Horse_Advanced_Name;
                    LocStringID = EnemyEV.Horse_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Horse_Advanced_MaxHealth;
                    Damage = EnemyEV.Horse_Advanced_Damage;
                    XPValue = EnemyEV.Horse_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Horse_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Horse_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Horse_Advanced_DropChance;

                    Speed = EnemyEV.Horse_Advanced_Speed;
                    TurnSpeed = EnemyEV.Horse_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Horse_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Horse_Advanced_Jump;
                    CooldownTime = EnemyEV.Horse_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Horse_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Horse_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Horse_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Horse_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Horse_Advanced_IsWeighted;

                    Scale = EnemyEV.Horse_Advanced_Scale;
                    ProjectileScale = EnemyEV.Horse_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Horse_Advanced_Tint;

                    MeleeRadius = EnemyEV.Horse_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Horse_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Horse_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Horse_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }				
        }

        protected override void InitializeLogic()
        {

            LogicSet runLeftLS = new LogicSet(this);
            runLeftLS.AddAction(new ChangeSpriteLogicAction("EnemyHorseRun_Character", true, true));
            runLeftLS.AddAction(new MoveDirectionLogicAction(new Vector2(-1,0)));
            runLeftLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet runRightLS = new LogicSet(this);
            runRightLS.AddAction(new ChangeSpriteLogicAction("EnemyHorseRun_Character", true, true));
            runRightLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
            runRightLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet turnRightLS = new LogicSet(this);
            turnRightLS.AddAction(new ChangeSpriteLogicAction("EnemyHorseTurn_Character", true, true));
            turnRightLS.AddAction(new MoveDirectionLogicAction(new Vector2(-1, 0)));
            turnRightLS.AddAction(new DelayLogicAction(0.25f));
            turnRightLS.AddAction(new ChangePropertyLogicAction(this, "Flip", SpriteEffects.None));
            turnRightLS.AddAction(new RunFunctionLogicAction(this, "ResetTurn"));

            LogicSet turnLeftLS = new LogicSet(this);
            turnLeftLS.AddAction(new ChangeSpriteLogicAction("EnemyHorseTurn_Character", true, true));
            turnLeftLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
            turnLeftLS.AddAction(new DelayLogicAction(0.25f));
            turnLeftLS.AddAction(new ChangePropertyLogicAction(this, "Flip", SpriteEffects.FlipHorizontally));
            turnLeftLS.AddAction(new RunFunctionLogicAction(this, "ResetTurn"));

            LogicSet runLeftExpertLS = new LogicSet(this);
            runLeftExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyHorseRun_Character", true, true));
            runLeftExpertLS.AddAction(new MoveDirectionLogicAction(new Vector2(-1, 0)));
            ThrowStandingProjectiles(runLeftExpertLS, true);
            //runLeftExpertLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet runRightExpertLS = new LogicSet(this);
            runRightExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyHorseRun_Character", true, true));
            runRightExpertLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
            ThrowStandingProjectiles(runRightExpertLS, true);
            //runRightExpertLS.AddAction(new DelayLogicAction(0.0f));


            m_generalBasicLB.AddLogicSet(runLeftLS, runRightLS);
            //m_generalExpertLB.AddLogicSet(runLeftExpertLS, runRightExpertLS);

            m_turnLB.AddLogicSet(turnLeftLS, turnRightLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_turnLB);

            m_gallopSound = new FrameSoundObj(this, m_target, 2, "Enemy_Horse_Gallop_01", "Enemy_Horse_Gallop_02", "Enemy_Horse_Gallop_03");

            base.InitializeLogic();
        }

        private void ThrowStandingProjectiles(LogicSet ls, bool useBossProjectile = false)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "SpellDamageShield_Sprite",
                SourceAnchor = new Vector2(0, 60),//Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
                Lifespan = 0.75f,//0.65f,//0.75f
            };

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            ls.AddAction(new DelayLogicAction(0.075f));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            ls.AddAction(new DelayLogicAction(0.075f));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Dispose();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    if (this.Flip == SpriteEffects.FlipHorizontally)
                        RunLogicBlock(true, m_generalBasicLB, 100, 0); // Run to the left.
                    else
                        RunLogicBlock(true, m_generalBasicLB, 0, 100); // Run to the right
                    break;
                default:
                    break;
            }
        }

        protected override void RunAdvancedLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    if (this.Flip == SpriteEffects.FlipHorizontally)
                        RunLogicBlock(true, m_generalBasicLB, 100, 0); // Run to the left.
                    else
                        RunLogicBlock(true, m_generalBasicLB, 0, 100); // Run to the right
                    break;
                default:
                    break;
            }
        }

        protected override void RunExpertLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    if (this.Flip == SpriteEffects.FlipHorizontally)
                        RunLogicBlock(true, m_generalBasicLB, 100, 0); // Run to the left.
                    else
                        RunLogicBlock(true, m_generalBasicLB, 0, 100); // Run to the right
                    break;
                default:
                    break;
            }
        }

        protected override void RunMinibossLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    if (this.Flip == SpriteEffects.FlipHorizontally)
                        RunLogicBlock(true, m_generalBasicLB, 100, 0); // Run to the left.
                    else
                        RunLogicBlock(true, m_generalBasicLB, 0, 100); // Run to the right
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            //m_collidingWithGround = false;
            //m_collidingWithWall = false;

            //foreach (TerrainObj obj in m_levelScreen.CurrentRoom.TerrainObjList)
            //{
            //    if (obj.Rotation == 0)
            //    {
            //        if (CollisionMath.Intersects(obj.Bounds, WallCollisionPoint) == true)
            //            m_collidingWithWall = true;
            //        if (CollisionMath.Intersects(obj.Bounds, GroundCollisionPoint) == true)
            //            m_collidingWithGround = true;
            //    }
            //}

            //if (m_currentActiveLB != m_turnLB)
            //{
            //    if (m_collidingWithWall == true || m_collidingWithGround == false)
            //    {
            //        if (this.HeadingX < 0)
            //        {
            //            m_currentActiveLB.StopLogicBlock();
            //            RunLogicBlock(false, m_turnLB, 0, 100);
            //        }
            //        else
            //        {
            //            m_currentActiveLB.StopLogicBlock();
            //            RunLogicBlock(false, m_turnLB, 100, 0);
            //        }
            //    }
            //}

            if (m_target.AttachedLevel.CurrentRoom.Name != "Ending")
                m_gallopSound.Update();

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.Difficulty >= GameTypes.EnemyDifficulty.ADVANCED)
            {
                if (m_fireDropTimer > 0)
                {
                    m_fireDropTimer -= elapsedTime;
                    if (m_fireDropTimer <= 0)
                    {
                        DropFireProjectile();
                        m_fireDropTimer = m_fireDropInterval;
                    }
                }
            }
            
            if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT && this.IsPaused == false)
            {
                if (m_fireShieldList.Count < 1)
                    CastFireShield(m_numFireShieldObjs);
            }

            if (((this.Bounds.Left < m_levelScreen.CurrentRoom.Bounds.Left) || (this.Bounds.Right > m_levelScreen.CurrentRoom.Bounds.Right)) && m_collisionCheckTimer <= 0)
               TurnHorse();

            Rectangle collPt = new Rectangle();
            Rectangle collPt2 = new Rectangle(); // Pt2 is to check for sloped collisions.
            if (this.Flip == SpriteEffects.FlipHorizontally)
            {
                collPt = new Rectangle(this.Bounds.Left - 10, this.Bounds.Bottom + 20, 5, 5);
                collPt2 = new Rectangle(this.Bounds.Right + 50, this.Bounds.Bottom - 20, 5, 5);
            }
            else
            {
                collPt = new Rectangle(this.Bounds.Right + 10, this.Bounds.Bottom + 20, 5, 5);
                collPt2 = new Rectangle(this.Bounds.Left - 50, this.Bounds.Bottom - 20, 5, 5);
            }


            bool turn = true;
            foreach (TerrainObj terrain in m_levelScreen.CurrentRoom.TerrainObjList)
            {
                if (CollisionMath.Intersects(terrain.Bounds, collPt) || CollisionMath.Intersects(terrain.Bounds, collPt2))
                {
                    turn = false;
                    break;
                }
            }

            if (turn == true)
                TurnHorse();

            if (m_collisionCheckTimer > 0)
                m_collisionCheckTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        public void ResetTurn()
        {
            m_turning = false;
        }

        private void DropFireProjectile()
        {
            this.UpdateCollisionBoxes();
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "SpellDamageShield_Sprite",
                SourceAnchor = new Vector2(0, (this.Bounds.Bottom - this.Y) - 10),
                //Target = m_target,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                Angle = new Vector2(0, 0),
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
                Lifespan = m_fireDropLifespan,
                LockPosition = true,
            };

            m_levelScreen.ProjectileManager.FireProjectile(projData);
            projData.Dispose();
        }

        private void CastFireShield(int numFires)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "SpellDamageShield_Sprite",
                SourceAnchor = new Vector2(0, (this.Bounds.Bottom - this.Y) - 10),
                //Target = m_target,
                Speed = new Vector2(m_fireRotationSpeed, m_fireRotationSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Target = this,
                Damage = Damage,
                Angle = new Vector2(0, 0),
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = new Vector2(m_fireShieldScale, m_fireShieldScale),
                Lifespan = 999999,
                DestroysWithEnemy = false,
                LockPosition = true,
            };

            SoundManager.PlaySound("Cast_FireShield");
            float projectileDistance = m_fireDistance;
            for (int i = 0; i < (int)numFires; i++)
            {
                float angle = (360f / numFires) * i;

                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                proj.AltX = angle; // AltX and AltY are used as holders to hold the projectiles angle and distance from player respectively.
                proj.AltY = projectileDistance;
                proj.Spell = SpellType.DamageShield;
                proj.CanBeFusRohDahed = false;
                proj.AccelerationXEnabled = false;
                proj.AccelerationYEnabled = false;
                proj.IgnoreBoundsCheck = true;

                m_fireShieldList.Add(proj);
            }
        }

        private void TurnHorse()
        {
            if (m_turning == false)
            {
                m_turning = true;
                if (this.HeadingX < 0)
                {
                    m_currentActiveLB.StopLogicBlock();
                    RunLogicBlock(false, m_turnLB, 0, 100);
                }
                else
                {
                    m_currentActiveLB.StopLogicBlock();
                    RunLogicBlock(false, m_turnLB, 100, 0);
                }
                m_collisionCheckTimer = 0.5f;
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            TerrainObj terrain = otherBox.AbsParent as TerrainObj;
            if (otherBox.AbsParent.Bounds.Top < this.TerrainBounds.Bottom - 20 && terrain != null && terrain.CollidesLeft == true && terrain.CollidesRight == true && terrain.CollidesBottom == true)
            {
                if (collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN && otherBox.AbsRotation == 0 && m_collisionCheckTimer <= 0)
                {
                    Vector2 mtd = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);
                    if (mtd.X != 0)
                        TurnHorse();
                }
            }
            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            SoundManager.Play3DSound(this, m_target, "Enemy_Horse_Hit_01", "Enemy_Horse_Hit_02", "Enemy_Horse_Hit_03");
            base.HitEnemy(damage, collisionPt, isPlayer);
        }

        public override void Kill(bool giveXP = true)
        {
            foreach (ProjectileObj projectile in m_fireShieldList)
                projectile.RunDestroyAnimation(false);

            m_fireShieldList.Clear();

            SoundManager.Play3DSound(this, m_target, "Enemy_Horse_Dead");
            base.Kill(giveXP);
        }

        public override void ResetState()
        {
            m_fireShieldList.Clear();
            base.ResetState();
        }

        public EnemyObj_Horse(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyHorseRun_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Horse;
            m_fireShieldList = new List<ProjectileObj>();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                if (m_gallopSound != null)
                    m_gallopSound.Dispose();
                m_gallopSound = null;
                base.Dispose();
            }
        }

        private Rectangle WallCollisionPoint
        {
            get
            {
                if (this.HeadingX < 0)
                    return new Rectangle((int)this.X - m_wallDistanceCheck, (int)this.Y, 2, 2);
                else
                    return new Rectangle((int)this.X + m_wallDistanceCheck, (int)this.Y, 2, 2);
            }
        }

        private Rectangle GroundCollisionPoint
        {
            get
            {
                if (this.HeadingX < 0)
                    return new Rectangle((int)(this.X - (m_wallDistanceCheck * this.ScaleX)), (int)(this.Y + (60 * this.ScaleY)), 2, 2);
                else
                    return new Rectangle((int)(this.X + (m_wallDistanceCheck * this.ScaleX)), (int)(this.Y + (60 * this.ScaleY)), 2, 2);
            }
        }
    }
}

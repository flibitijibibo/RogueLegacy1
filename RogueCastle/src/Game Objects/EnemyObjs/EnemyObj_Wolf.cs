using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Wolf: EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();
        private LogicBlock m_wolfHitLB = new LogicBlock();

        public bool Chasing { get; set; }
        private float PounceDelay = 0.3f;
        private float PounceLandDelay = 0.5f; // How long after a wolf lands does he start chasing again.
        private Color FurColour = Color.White;

        private float m_startDelay = 1f;//  A special delay for wolves since they move too quick.
        private float m_startDelayCounter = 0;

        private FrameSoundObj m_runFrameSound;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Wolf_Basic_Name;
            LocStringID = EnemyEV.Wolf_Basic_Name_locID;

            MaxHealth = EnemyEV.Wolf_Basic_MaxHealth;
            Damage = EnemyEV.Wolf_Basic_Damage;
            XPValue = EnemyEV.Wolf_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Wolf_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Wolf_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Wolf_Basic_DropChance;

            Speed = EnemyEV.Wolf_Basic_Speed;
            TurnSpeed = EnemyEV.Wolf_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Wolf_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Wolf_Basic_Jump;
            CooldownTime = EnemyEV.Wolf_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Wolf_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Wolf_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Wolf_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Wolf_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Wolf_Basic_IsWeighted;

            Scale = EnemyEV.Wolf_Basic_Scale;
            ProjectileScale = EnemyEV.Wolf_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Wolf_Basic_Tint;

            MeleeRadius = EnemyEV.Wolf_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Wolf_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Wolf_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Wolf_Basic_KnockBack;
            #endregion

            InitialLogicDelay = 1;

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Wolf_Miniboss_Name;
                    LocStringID = EnemyEV.Wolf_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Wolf_Miniboss_MaxHealth;
                    Damage = EnemyEV.Wolf_Miniboss_Damage;
                    XPValue = EnemyEV.Wolf_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Wolf_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Wolf_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Wolf_Miniboss_DropChance;

                    Speed = EnemyEV.Wolf_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Wolf_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Wolf_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Wolf_Miniboss_Jump;
                    CooldownTime = EnemyEV.Wolf_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Wolf_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Wolf_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Wolf_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Wolf_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Wolf_Miniboss_IsWeighted;

                    Scale = EnemyEV.Wolf_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Wolf_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Wolf_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Wolf_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Wolf_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Wolf_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Wolf_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Wolf_Expert_Name;
                    LocStringID = EnemyEV.Wolf_Expert_Name_locID;

                    MaxHealth = EnemyEV.Wolf_Expert_MaxHealth;
                    Damage = EnemyEV.Wolf_Expert_Damage;
                    XPValue = EnemyEV.Wolf_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Wolf_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Wolf_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Wolf_Expert_DropChance;

                    Speed = EnemyEV.Wolf_Expert_Speed;
                    TurnSpeed = EnemyEV.Wolf_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Wolf_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Wolf_Expert_Jump;
                    CooldownTime = EnemyEV.Wolf_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Wolf_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Wolf_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Wolf_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Wolf_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Wolf_Expert_IsWeighted;

                    Scale = EnemyEV.Wolf_Expert_Scale;
                    ProjectileScale = EnemyEV.Wolf_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Wolf_Expert_Tint;

                    MeleeRadius = EnemyEV.Wolf_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Wolf_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Wolf_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Wolf_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Wolf_Advanced_Name;
                    LocStringID = EnemyEV.Wolf_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Wolf_Advanced_MaxHealth;
                    Damage = EnemyEV.Wolf_Advanced_Damage;
                    XPValue = EnemyEV.Wolf_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Wolf_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Wolf_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Wolf_Advanced_DropChance;

                    Speed = EnemyEV.Wolf_Advanced_Speed;
                    TurnSpeed = EnemyEV.Wolf_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Wolf_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Wolf_Advanced_Jump;
                    CooldownTime = EnemyEV.Wolf_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Wolf_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Wolf_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Wolf_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Wolf_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Wolf_Advanced_IsWeighted;

                    Scale = EnemyEV.Wolf_Advanced_Scale;
                    ProjectileScale = EnemyEV.Wolf_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Wolf_Advanced_Tint;

                    MeleeRadius = EnemyEV.Wolf_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Wolf_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Wolf_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Wolf_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    //this.MaxHealth = 999;
                    break;
            }		
        }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(false));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyWargRun_Character", true, true));
            walkTowardsLS.AddAction(new ChangePropertyLogicAction(this, "Chasing", true));
            walkTowardsLS.AddAction(new DelayLogicAction(1.0f));

            LogicSet stopWalkLS = new LogicSet(this);
            stopWalkLS.AddAction(new LockFaceDirectionLogicAction(false));
            stopWalkLS.AddAction(new MoveLogicAction(m_target, true, 0));
            stopWalkLS.AddAction(new ChangeSpriteLogicAction("EnemyWargIdle_Character", true, true));
            stopWalkLS.AddAction(new ChangePropertyLogicAction(this, "Chasing", false));
            stopWalkLS.AddAction(new DelayLogicAction(1.0f));

            LogicSet jumpLS = new LogicSet(this);
            jumpLS.AddAction(new GroundCheckLogicAction()); // Make sure it can only jump while touching ground.
            jumpLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Wolf_Attack"));
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyWargPounce_Character", true, true));
            jumpLS.AddAction(new DelayLogicAction(PounceDelay));
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyWargJump_Character", false, false));
            jumpLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            jumpLS.AddAction(new MoveDirectionLogicAction());
            jumpLS.AddAction(new JumpLogicAction());
            jumpLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            jumpLS.AddAction(new GroundCheckLogicAction());
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyWargIdle_Character", true, true));
            jumpLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpLS.AddAction(new DelayLogicAction(PounceLandDelay));



            LogicSet wolfHit = new LogicSet(this);
            wolfHit.AddAction(new ChangeSpriteLogicAction("EnemyWargHit_Character", false, false));
            wolfHit.AddAction(new DelayLogicAction(0.2f));
            wolfHit.AddAction(new GroundCheckLogicAction());

            m_generalBasicLB.AddLogicSet(walkTowardsLS, stopWalkLS, jumpLS);
            m_wolfHitLB.AddLogicSet(wolfHit);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);
            logicBlocksToDispose.Add(m_wolfHitLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 40, 40, 20); //walkTowardsLS, walkAwayLS, walkStopLS

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            if (m_startDelayCounter <= 0)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                        if (m_target.Y < this.Y - m_target.Height)
                            RunLogicBlock(false, m_generalBasicLB, 0, 0, 100); // walkTowardsLS, stopLS, jumpLS
                        else
                            RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                        break;
                    case (STATE_ENGAGE):
                        if (Chasing == false)
                            RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                        break;
                    case (STATE_WANDER):
                        if (Chasing == true)
                            RunLogicBlock(false, m_generalBasicLB, 0, 100, 0);
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void RunAdvancedLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                    if (m_target.Y < this.Y - m_target.Height)
                        RunLogicBlock(false, m_generalBasicLB, 0, 0, 100); // walkTowardsLS, stopLS, jumpLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                    break;
                case (STATE_ENGAGE):
                    if (Chasing == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                    break;
                case (STATE_WANDER):
                    if (Chasing == true)
                        RunLogicBlock(false, m_generalBasicLB, 0, 100, 0);
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
                    if (m_target.Y < this.Y - m_target.Height)
                        RunLogicBlock(false, m_generalBasicLB, 0, 0, 100); // walkTowardsLS, stopLS, jumpLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                    break;
                case (STATE_ENGAGE):
                    if (Chasing == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                    break;
                case (STATE_WANDER):
                    if (Chasing == true)
                        RunLogicBlock(false, m_generalBasicLB, 0, 100, 0);
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
                    if (m_target.Y < this.Y - m_target.Height)
                        RunLogicBlock(false, m_generalBasicLB, 0, 0, 100); // walkTowardsLS, stopLS, jumpLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                    break;
                case (STATE_ENGAGE):
                    if (Chasing == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0);
                    break;
                case (STATE_WANDER):
                    if (Chasing == true)
                        RunLogicBlock(false, m_generalBasicLB, 0, 100, 0);
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_startDelayCounter > 0)
                m_startDelayCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Maintains the enemy's speed in the air so that he can jump onto platforms.
            if (m_isTouchingGround == false && IsWeighted == true && CurrentSpeed == 0 && this.SpriteName == "EnemyWargJump_Character")
                this.CurrentSpeed = this.Speed;

            base.Update(gameTime);

            if (m_isTouchingGround == true && CurrentSpeed == 0 && IsAnimating == false)
            {
                this.ChangeSprite("EnemyWargIdle_Character");
                this.PlayAnimation(true);
            }

            if (this.SpriteName == "EnemyWargRun_Character")
                m_runFrameSound.Update();
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Wolf_Hit_01", "Wolf_Hit_02", "Wolf_Hit_03");
            if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
                m_currentActiveLB.StopLogicBlock();

            m_currentActiveLB = m_wolfHitLB;
            m_currentActiveLB.RunLogicBlock(100);
            base.HitEnemy(damage, position, isPlayer);
        }

        public override void ResetState()
        {
            m_startDelayCounter = m_startDelay;
            base.ResetState();
        }

        public override void Reset()
        {
            m_startDelayCounter = m_startDelay;
            base.Reset();
        }

        public EnemyObj_Wolf(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyWargIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Wolf;
            m_startDelayCounter = m_startDelay;
            m_runFrameSound = new FrameSoundObj(this, 1, "Wolf_Move01", "Wolf_Move02", "Wolf_Move03");
        }
    }
}

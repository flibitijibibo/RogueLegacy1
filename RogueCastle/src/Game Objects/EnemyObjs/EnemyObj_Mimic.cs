using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Mimic : EnemyObj
    {
        private bool m_isAttacking = false;
        private LogicBlock m_generalBasicLB = new LogicBlock();

        private FrameSoundObj m_closeSound;

        protected override void InitializeEV()
        {
            //this.AnimationDelay = 1 / 20f;
            //this.Scale = new Vector2(2, 2);
            //this.MaxHealth = 150;
            //this.IsWeighted = true;
            //this.JumpHeight = 400;
            //this.Speed = 400;

            #region Basic Variables - General
            Name = EnemyEV.Mimic_Basic_Name;
            LocStringID = EnemyEV.Mimic_Basic_Name_locID;

            MaxHealth = EnemyEV.Mimic_Basic_MaxHealth;
            Damage = EnemyEV.Mimic_Basic_Damage;
            XPValue = EnemyEV.Mimic_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Mimic_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Mimic_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Mimic_Basic_DropChance;

            Speed = EnemyEV.Mimic_Basic_Speed;
            TurnSpeed = EnemyEV.Mimic_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Mimic_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Mimic_Basic_Jump;
            CooldownTime = EnemyEV.Mimic_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Mimic_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Mimic_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Mimic_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Mimic_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Mimic_Basic_IsWeighted;

            Scale = EnemyEV.Mimic_Basic_Scale;
            ProjectileScale = EnemyEV.Mimic_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Mimic_Basic_Tint;

            MeleeRadius = EnemyEV.Mimic_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Mimic_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Mimic_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Mimic_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Mimic_Miniboss_Name;
                    LocStringID = EnemyEV.Mimic_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Mimic_Miniboss_MaxHealth;
                    Damage = EnemyEV.Mimic_Miniboss_Damage;
                    XPValue = EnemyEV.Mimic_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Mimic_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Mimic_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Mimic_Miniboss_DropChance;

                    Speed = EnemyEV.Mimic_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Mimic_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Mimic_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Mimic_Miniboss_Jump;
                    CooldownTime = EnemyEV.Mimic_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Mimic_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Mimic_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Mimic_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Mimic_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Mimic_Miniboss_IsWeighted;

                    Scale = EnemyEV.Mimic_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Mimic_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Mimic_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Mimic_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Mimic_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Mimic_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Mimic_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Mimic_Expert_Name;
                    LocStringID = EnemyEV.Mimic_Expert_Name_locID;

                    MaxHealth = EnemyEV.Mimic_Expert_MaxHealth;
                    Damage = EnemyEV.Mimic_Expert_Damage;
                    XPValue = EnemyEV.Mimic_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Mimic_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Mimic_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Mimic_Expert_DropChance;

                    Speed = EnemyEV.Mimic_Expert_Speed;
                    TurnSpeed = EnemyEV.Mimic_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Mimic_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Mimic_Expert_Jump;
                    CooldownTime = EnemyEV.Mimic_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Mimic_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Mimic_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Mimic_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Mimic_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Mimic_Expert_IsWeighted;

                    Scale = EnemyEV.Mimic_Expert_Scale;
                    ProjectileScale = EnemyEV.Mimic_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Mimic_Expert_Tint;

                    MeleeRadius = EnemyEV.Mimic_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Mimic_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Mimic_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Mimic_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Mimic_Advanced_Name;
                    LocStringID = EnemyEV.Mimic_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Mimic_Advanced_MaxHealth;
                    Damage = EnemyEV.Mimic_Advanced_Damage;
                    XPValue = EnemyEV.Mimic_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Mimic_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Mimic_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Mimic_Advanced_DropChance;

                    Speed = EnemyEV.Mimic_Advanced_Speed;
                    TurnSpeed = EnemyEV.Mimic_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Mimic_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Mimic_Advanced_Jump;
                    CooldownTime = EnemyEV.Mimic_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Mimic_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Mimic_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Mimic_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Mimic_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Mimic_Advanced_IsWeighted;

                    Scale = EnemyEV.Mimic_Advanced_Scale;
                    ProjectileScale = EnemyEV.Mimic_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Mimic_Advanced_Tint;

                    MeleeRadius = EnemyEV.Mimic_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Mimic_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Mimic_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Mimic_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            this.LockFlip = true;
        }

        protected override void InitializeLogic()
        {
            LogicSet basicWarningLS = new LogicSet(this);
            basicWarningLS.AddAction(new ChangeSpriteLogicAction("EnemyMimicShake_Character",false, false));
            basicWarningLS.AddAction(new PlayAnimationLogicAction(false));
            basicWarningLS.AddAction(new ChangeSpriteLogicAction("EnemyMimicIdle_Character", true, false));
            basicWarningLS.AddAction(new DelayLogicAction(3));

            LogicSet jumpTowardsLS = new LogicSet(this);
            jumpTowardsLS.AddAction(new GroundCheckLogicAction()); // Make sure it can only jump while touching ground.
            jumpTowardsLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpTowardsLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpTowardsLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyMimicAttack_Character", true, true));
            jumpTowardsLS.AddAction(new MoveDirectionLogicAction());
            jumpTowardsLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Chest_Open_Large"));
            jumpTowardsLS.AddAction(new JumpLogicAction());
            jumpTowardsLS.AddAction(new DelayLogicAction(0.3f));
            jumpTowardsLS.AddAction(new GroundCheckLogicAction());

            LogicSet jumpUpLS = new LogicSet(this);

            m_generalBasicLB.AddLogicSet(basicWarningLS, jumpTowardsLS);

            logicBlocksToDispose.Add(m_generalBasicLB);

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                default:
                    if (m_isAttacking == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0); // basicWarningLS, jumpTowardsLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 0, 100); // basicWarningLS, jumpTowardsLS                    
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
                default:
                    if (m_isAttacking == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0); // basicWarningLS, jumpTowardsLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 0, 100); // basicWarningLS, jumpTowardsLS                    
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
                default:
                    if (m_isAttacking == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0); // basicWarningLS, jumpTowardsLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 0, 100); // basicWarningLS, jumpTowardsLS                    
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
                default:
                    if (m_isAttacking == false)
                        RunLogicBlock(false, m_generalBasicLB, 100, 0); // basicWarningLS, jumpTowardsLS
                    else
                        RunLogicBlock(false, m_generalBasicLB, 0, 100); // basicWarningLS, jumpTowardsLS                    
                    break;
            }
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            if (m_isAttacking == false)
            {
                m_currentActiveLB.StopLogicBlock();
                m_isAttacking = true;
                LockFlip = false;
            }
            base.HitEnemy(damage, collisionPt, isPlayer);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (otherBox.AbsParent is PlayerObj)
            {
                if (m_isAttacking == false)
                {
                    m_currentActiveLB.StopLogicBlock();
                    m_isAttacking = true;
                    LockFlip = false;
                }
            }

            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }


        public EnemyObj_Mimic(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyMimicIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Mimic;
            this.OutlineWidth = 0;

            m_closeSound = new FrameSoundObj(this, m_target, 1, "Chest_Snap");
        }

        public override void Update(GameTime gameTime)
        {
            if (this.SpriteName == "EnemyMimicAttack_Character")
                m_closeSound.Update();
            base.Update(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_closeSound.Dispose();
                m_closeSound = null;
                base.Dispose();
            }
        }
    }
}

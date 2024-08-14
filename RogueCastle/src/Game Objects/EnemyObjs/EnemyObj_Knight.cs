using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Knight : EnemyObj
    {

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private float AttackThrustDelay = 0.65f;
        private float AttackThrustSpeed = 1850;
        private float AttackThrustDuration = 0.4f;

        private float AttackProjectileDelay = 0.35f;

        private float AttackThrustDelayExpert = 0.65f;
        private float AttackThrustSpeedExpert = 1750;
        private float AttackThrustDurationExpert = 0.25f;

        private float AttackProjectileExpertDelay = 0.425f;

        private float AttackThrustDelayMiniBoss = 0.65f;
        private float AttackThrustSpeedMiniBoss = 2300;
        private float AttackThrustDurationMiniBoss = 0.25f;

        private float AttackProjectileMinibossDelay = 0.5f;

        private FrameSoundObj m_walkSound, m_walkSound2;
            
        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Knight_Basic_Name;
            LocStringID = EnemyEV.Knight_Basic_Name_locID;

            MaxHealth = EnemyEV.Knight_Basic_MaxHealth;
            Damage = EnemyEV.Knight_Basic_Damage;
            XPValue = EnemyEV.Knight_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Knight_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Knight_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Knight_Basic_DropChance;

            Speed = EnemyEV.Knight_Basic_Speed;
            TurnSpeed = EnemyEV.Knight_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Knight_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Knight_Basic_Jump;
            CooldownTime = EnemyEV.Knight_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Knight_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Knight_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Knight_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Knight_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Knight_Basic_IsWeighted;

            Scale = EnemyEV.Knight_Basic_Scale;
            ProjectileScale = EnemyEV.Knight_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Knight_Basic_Tint;

            MeleeRadius = EnemyEV.Knight_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Knight_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Knight_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Knight_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Knight_Miniboss_Name;
                    LocStringID = EnemyEV.Knight_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Knight_Miniboss_MaxHealth;
                    Damage = EnemyEV.Knight_Miniboss_Damage;
                    XPValue = EnemyEV.Knight_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Knight_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Knight_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Knight_Miniboss_DropChance;

                    Speed = EnemyEV.Knight_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Knight_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Knight_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Knight_Miniboss_Jump;
                    CooldownTime = EnemyEV.Knight_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Knight_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Knight_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Knight_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Knight_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Knight_Miniboss_IsWeighted;

                    Scale = EnemyEV.Knight_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Knight_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Knight_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Knight_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Knight_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Knight_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Knight_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Knight_Expert_Name;
                    LocStringID = EnemyEV.Knight_Expert_Name_locID;

                    MaxHealth = EnemyEV.Knight_Expert_MaxHealth;
                    Damage = EnemyEV.Knight_Expert_Damage;
                    XPValue = EnemyEV.Knight_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Knight_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Knight_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Knight_Expert_DropChance;

                    Speed = EnemyEV.Knight_Expert_Speed;
                    TurnSpeed = EnemyEV.Knight_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Knight_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Knight_Expert_Jump;
                    CooldownTime = EnemyEV.Knight_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Knight_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Knight_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Knight_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Knight_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Knight_Expert_IsWeighted;

                    Scale = EnemyEV.Knight_Expert_Scale;
                    ProjectileScale = EnemyEV.Knight_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Knight_Expert_Tint;

                    MeleeRadius = EnemyEV.Knight_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Knight_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Knight_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Knight_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Knight_Advanced_Name;
                    LocStringID = EnemyEV.Knight_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Knight_Advanced_MaxHealth;
                    Damage = EnemyEV.Knight_Advanced_Damage;
                    XPValue = EnemyEV.Knight_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Knight_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Knight_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Knight_Advanced_DropChance;

                    Speed = EnemyEV.Knight_Advanced_Speed;
                    TurnSpeed = EnemyEV.Knight_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Knight_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Knight_Advanced_Jump;
                    CooldownTime = EnemyEV.Knight_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Knight_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Knight_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Knight_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Knight_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Knight_Advanced_IsWeighted;

                    Scale = EnemyEV.Knight_Advanced_Scale;
                    ProjectileScale = EnemyEV.Knight_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Knight_Advanced_Tint;

                    MeleeRadius = EnemyEV.Knight_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Knight_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Knight_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Knight_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }		
        }

        protected override void InitializeLogic()
        {
            //////////////// Movement Logic ////////////////////////
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightWalk_Character", true, true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(0.2f, 1.0f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightWalk_Character", true, true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(0.2f, 1.0f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.2f, 1.0f));
            /////////////////////////////////////////////////////////

            //////////////// Attack Logic ////////////////////
            LogicSet attackThrustLS = new LogicSet(this);
            attackThrustLS.AddAction(new MoveLogicAction(m_target, true, 0)); // Sets the enemy to face the player.
            attackThrustLS.AddAction(new LockFaceDirectionLogicAction(true)); // Lock his facing direction.
            attackThrustLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack_Character"));
            attackThrustLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackThrustLS.AddAction(new DelayLogicAction(AttackThrustDelay)); //The tell
            attackThrustLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            //attackThrustLS.AddAction(new MoveLogicAction(null, true, AttackThrustSpeed)); //The speed of his thrust.
            attackThrustLS.AddAction(new MoveDirectionLogicAction(AttackThrustSpeed));
            attackThrustLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "DisplayThrustDustEffect", this, 20, 0.3f));
            attackThrustLS.AddAction(new PlayAnimationLogicAction("AttackStart","End", false), Types.Sequence.Parallel);
            attackThrustLS.AddAction(new DelayLogicAction(AttackThrustDuration)); //The length of his thrust.
            attackThrustLS.AddAction(new MoveLogicAction(null, true, 0));
            attackThrustLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            attackThrustLS.AddAction(new DelayLogicAction(0.3f));
            attackThrustLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackThrustLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet attackThrustExpertLS = new LogicSet(this);
            attackThrustExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackThrustExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackThrustExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack_Character"));
            attackThrustExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackThrustExpertLS.AddAction(new DelayLogicAction(AttackThrustDelayExpert)); //The tell
            attackThrustExpertLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            attackThrustExpertLS.AddAction(new MoveDirectionLogicAction(AttackThrustSpeedExpert));
            attackThrustExpertLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "DisplayThrustDustEffect", this, 20, 0.3f));
            attackThrustExpertLS.AddAction(new PlayAnimationLogicAction("AttackStart","End", false), Types.Sequence.Parallel);
            attackThrustExpertLS.AddAction(new DelayLogicAction(AttackThrustDurationExpert)); //The length of his thrust.
            attackThrustExpertLS.AddAction(new MoveLogicAction(null, true, 0));
            attackThrustExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            attackThrustExpertLS.AddAction(new DelayLogicAction(0.3f));
            attackThrustExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackThrustExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet attackThrustMinibossLS = new LogicSet(this);
            attackThrustMinibossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackThrustMinibossLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackThrustMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack_Character"));
            attackThrustMinibossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackThrustMinibossLS.AddAction(new DelayLogicAction(AttackThrustDelayMiniBoss)); //The tell
            attackThrustMinibossLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            attackThrustMinibossLS.AddAction(new MoveDirectionLogicAction(AttackThrustSpeedMiniBoss));
            attackThrustMinibossLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "DisplayThrustDustEffect", this, 20, 0.3f));
            attackThrustMinibossLS.AddAction(new PlayAnimationLogicAction("AttackStart", "End", false), Types.Sequence.Parallel);
            attackThrustMinibossLS.AddAction(new DelayLogicAction(AttackThrustDurationMiniBoss)); //The length of his thrust.
            attackThrustMinibossLS.AddAction(new MoveLogicAction(null, true, 0));
            attackThrustMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            attackThrustMinibossLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackThrustMinibossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackThrustMinibossLS.AddAction(new LockFaceDirectionLogicAction(true));
            //
            attackThrustMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack_Character"));
            attackThrustMinibossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackThrustMinibossLS.AddAction(new DelayLogicAction(0.25f)); //The tell
            attackThrustMinibossLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            attackThrustMinibossLS.AddAction(new MoveDirectionLogicAction(AttackThrustSpeedMiniBoss));
            attackThrustMinibossLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "DisplayThrustDustEffect", this, 20, 0.3f));
            attackThrustMinibossLS.AddAction(new PlayAnimationLogicAction("AttackStart", "End", false), Types.Sequence.Parallel);
            attackThrustMinibossLS.AddAction(new DelayLogicAction(AttackThrustDurationMiniBoss)); //The length of his thrust.
            attackThrustMinibossLS.AddAction(new MoveLogicAction(null, true, 0));
            attackThrustMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            attackThrustMinibossLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackThrustMinibossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackThrustMinibossLS.AddAction(new LockFaceDirectionLogicAction(true));
            //
            attackThrustMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack_Character"));
            attackThrustMinibossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackThrustMinibossLS.AddAction(new DelayLogicAction(0.25f)); //The tell
            attackThrustMinibossLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            attackThrustMinibossLS.AddAction(new MoveDirectionLogicAction(AttackThrustSpeedMiniBoss));
            attackThrustMinibossLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "DisplayThrustDustEffect", this, 20, 0.3f));
            attackThrustMinibossLS.AddAction(new PlayAnimationLogicAction("AttackStart", "End", false), Types.Sequence.Parallel);
            attackThrustMinibossLS.AddAction(new DelayLogicAction(AttackThrustDurationMiniBoss)); //The length of his thrust.
            attackThrustMinibossLS.AddAction(new MoveLogicAction(null, true, 0));
            attackThrustMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            attackThrustMinibossLS.AddAction(new DelayLogicAction(0.3f));
            attackThrustMinibossLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackThrustMinibossLS.Tag = GameTypes.LogicSetType_ATTACK;

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "EnemySpearKnightWave_Sprite",
                SourceAnchor = new Vector2(30, 0),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0,0),
                Scale = ProjectileScale,
            };

            LogicSet throwSpearLS = new LogicSet(this);
            throwSpearLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwSpearLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwSpearLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearLS.AddAction(new DelayLogicAction(AttackProjectileDelay));
            throwSpearLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            throwSpearLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnight_Projectile"));
            throwSpearLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //throwSpearLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 0, ProjectileSpeed, false, 0, Damage));
            throwSpearLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            throwSpearLS.AddAction(new DelayLogicAction(0.3f));
            throwSpearLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwSpearLS.Tag = GameTypes.LogicSetType_ATTACK;



            LogicSet throwSpearExpertLS = new LogicSet(this);
            throwSpearExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwSpearExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwSpearExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearExpertLS.AddAction(new DelayLogicAction(AttackProjectileExpertDelay));
            ThrowThreeProjectiles(throwSpearExpertLS);
            throwSpearExpertLS.AddAction(new DelayLogicAction(0.3f));
            throwSpearExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwSpearExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwSpearMiniBossLS = new LogicSet(this);
            throwSpearMiniBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwSpearMiniBossLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwSpearMiniBossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearMiniBossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearMiniBossLS.AddAction(new DelayLogicAction(AttackProjectileMinibossDelay));
            throwSpearMiniBossLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnight_Projectile"));
            throwSpearMiniBossLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 30f));
            ThrowTwoProjectiles(throwSpearMiniBossLS);
            //
            throwSpearMiniBossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearMiniBossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearMiniBossLS.AddAction(new DelayLogicAction(0.05f));
            ThrowThreeProjectiles(throwSpearMiniBossLS);
            throwSpearMiniBossLS.AddAction(new DelayLogicAction(0.05f));
            //
            throwSpearMiniBossLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearMiniBossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearMiniBossLS.AddAction(new DelayLogicAction(0.05f));
            ThrowTwoProjectiles(throwSpearMiniBossLS);
            throwSpearMiniBossLS.AddAction(new DelayLogicAction(0.5f));
            //
            throwSpearMiniBossLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwSpearMiniBossLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / EnemyEV.Knight_Miniboss_AnimationDelay));
            throwSpearMiniBossLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwSpearMiniBossAltLS = new LogicSet(this);
            throwSpearMiniBossAltLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwSpearMiniBossAltLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwSpearMiniBossAltLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearMiniBossAltLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearMiniBossAltLS.AddAction(new DelayLogicAction(AttackProjectileMinibossDelay));
            throwSpearMiniBossAltLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnight_Projectile"));
            throwSpearMiniBossAltLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 30f));
            ThrowThreeProjectiles(throwSpearMiniBossAltLS);
            //            
            throwSpearMiniBossAltLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearMiniBossAltLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearMiniBossAltLS.AddAction(new DelayLogicAction(0.05f));
            ThrowTwoProjectiles(throwSpearMiniBossAltLS);
            throwSpearMiniBossAltLS.AddAction(new DelayLogicAction(0.05f));
            //
            throwSpearMiniBossAltLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearMiniBossAltLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearMiniBossAltLS.AddAction(new DelayLogicAction(0.05f));
            ThrowThreeProjectiles(throwSpearMiniBossAltLS);
            throwSpearMiniBossAltLS.AddAction(new DelayLogicAction(0.5f));
            //
            throwSpearMiniBossAltLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwSpearMiniBossAltLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / EnemyEV.Knight_Miniboss_AnimationDelay));
            throwSpearMiniBossAltLS.Tag = GameTypes.LogicSetType_ATTACK;
            #region - OLD THROW EXPERT SPEAR CODE

            /*
            LogicSet throwSpearExpertLS = new LogicSet(this);
            throwSpearExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwSpearExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwSpearExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightAttack2_Character"));
            throwSpearExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            throwSpearExpertLS.AddAction(new DelayLogicAction(AttackProjectileDelay));
            throwSpearExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            throwSpearExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 0, ProjectileSpeed, false, 0, Damage));
            throwSpearExpertLS.AddAction(new DelayLogicAction(0.3f));
            throwSpearExpertLS.AddAction(new JumpLogicAction());
            throwSpearExpertLS.AddAction(new DelayLogicAction(0.1f));
            throwSpearExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            throwSpearExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 0, ProjectileSpeed, false, 0, Damage));
            throwSpearExpertLS.AddAction(new GroundCheckLogicAction());
            throwSpearExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            throwSpearExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 0, ProjectileSpeed, false, 0, Damage));
            */
            # endregion
            /////////////////////////////////////////////////////////

            #region Old Jump spear logic
            LogicSet jumpSpearMany = new LogicSet(this);
            attackThrustLS.AddAction(new ChangeSpriteLogicAction("EnemySpearKnightIdle_Character", true, true));
            jumpSpearMany.AddAction(new MoveLogicAction(m_target, false, 300));
            jumpSpearMany.AddAction(new JumpLogicAction());
            jumpSpearMany.AddAction(new DelayLogicAction(0.3f));
            jumpSpearMany.AddAction(new GroundCheckLogicAction());
            jumpSpearMany.AddAction(new JumpLogicAction());
            ThrowRapidProjectiles(jumpSpearMany);
            ThrowRapidProjectiles(jumpSpearMany);
            ThrowRapidProjectiles(jumpSpearMany);
            jumpSpearMany.AddAction(new GroundCheckLogicAction());
            jumpSpearMany.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpSpearMany.AddAction(new DelayLogicAction(0.1f));
            jumpSpearMany.AddAction(new JumpLogicAction());
            ThrowRapidProjectiles(jumpSpearMany);
            ThrowRapidProjectiles(jumpSpearMany);
            ThrowRapidProjectiles(jumpSpearMany);
            jumpSpearMany.AddAction(new GroundCheckLogicAction());
            jumpSpearMany.AddAction(new MoveLogicAction(m_target, true, 0));
            //jumpSpearMany.AddAction(new DelayLogicAction(1.0f));
            jumpSpearMany.Tag = GameTypes.LogicSetType_ATTACK;
            #endregion


            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackThrustExpertLS, throwSpearExpertLS);
            m_generalMiniBossLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackThrustMinibossLS, throwSpearMiniBossLS, throwSpearMiniBossAltLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 55, 25, 20); //walkTowardsLS, walkAwayLS, walkStopLS

            projData.Dispose();

            base.InitializeLogic();

        }

        private void ThrowThreeProjectiles(LogicSet ls)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "EnemySpearKnightWave_Sprite",
                SourceAnchor = new Vector2(30, 0),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                Scale = ProjectileScale,
            };

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnight_Projectile"));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(45, 45);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-45, -45);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 0, ProjectileSpeed, false, 0, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 45, ProjectileSpeed, false, 0, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), -45, ProjectileSpeed, false, 0, Damage));
            ls.AddAction(new PlayAnimationLogicAction("Attack", "End", false));

            projData.Dispose();
        }

        private void ThrowTwoProjectiles(LogicSet ls)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "EnemySpearKnightWave_Sprite",
                SourceAnchor = new Vector2(30, 0),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(22, 22),
            };

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-22, -22);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), 22, ProjectileSpeed, false, 0, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(30, 0), -22, ProjectileSpeed, false, 0, Damage));
            ls.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            projData.Dispose();
        }

        private void ThrowRapidProjectiles(LogicSet ls)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "EnemySpearKnightWave_Sprite",
                SourceAnchor = new Vector2(130, -28),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = Vector2.Zero,
            };


            ls.AddAction(new DelayLogicAction(0.2f, 0.35f));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearKnightWave_Sprite", new Vector2(130, -28), 0, ProjectileSpeed, false, 0, Damage));            
            ls.AddAction(new DelayLogicAction(0.2f, 0.35f));
            projData.SourceAnchor = new Vector2(130, 28);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "EnemySpearnightWave_Sprite", new Vector2(130, 28), 0, ProjectileSpeed, false, 0, Damage));

            projData.Dispose();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE): 
                    RunLogicBlock(true, m_generalBasicLB, 20, 15, 0, 0, 65); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS,throwSpearLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 20, 20, 10, 0, 50); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 55, 30, 0, 0, 15); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
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
                    RunLogicBlock(true, m_generalAdvancedLB, 15, 10, 0, 60, 15); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS,throwSpearLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 15, 15, 10, 15, 45); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 55, 30, 0, 0, 15); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
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
                    RunLogicBlock(true, m_generalExpertLB, 15, 10, 0, 60, 15); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS,throwSpearLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 15, 15, 10, 15, 45); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 55, 30, 0, 0, 15); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustLS, throwSpearLS
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
                    RunLogicBlock(true, m_generalMiniBossLB, 14, 13, 11, 26, 18, 18); //walkTowardsLS, walkAwayLS, walkStopLS, attackThrustMinibossLS, throwSpearMiniBossLS, throwSpearMiniBossAltLS
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.SpriteName == "EnemySpearKnightWalk_Character")
            {
                m_walkSound.Update();
                m_walkSound2.Update();
            }
            base.Update(gameTime);
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Knight_Hit01", "Knight_Hit02", "Knight_Hit03");
            base.HitEnemy(damage, position, isPlayer);
        }


        public EnemyObj_Knight(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemySpearKnightIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            TintablePart = _objectList[1];
            this.Type = EnemyType.Knight;
            m_walkSound = new FrameSoundObj(this, m_target, 1, "KnightWalk1", "KnightWalk2");
            m_walkSound2 = new FrameSoundObj(this, m_target, 6, "KnightWalk1", "KnightWalk2");
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_walkSound.Dispose();
                m_walkSound = null;
                m_walkSound2.Dispose();
                m_walkSound2 = null;
                base.Dispose();
            }
        }
    }
}

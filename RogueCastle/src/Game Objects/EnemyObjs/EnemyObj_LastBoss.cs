using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Microsoft.Xna.Framework.Graphics;
using Tweener.Ease;

namespace RogueCastle
{
    public class EnemyObj_LastBoss : EnemyObj
    {
        private FrameSoundObj m_walkUpSoundFinalBoss;
        private FrameSoundObj m_walkDownSoundFinalBoss;

        private Vector2 AxeSpellScale = new Vector2(3.0f, 3.0f);
        private float AxeProjectileSpeed = 1100;

        private Vector2 DaggerSpellScale = new Vector2(3.5f, 3.5f);
        private float DaggerProjectileSpeed = 900;//875;//750;

        private float m_Spell_Close_Lifespan = 6;//8;
        private float m_Spell_Close_Scale = 3.5f;

        private int MegaFlyingDaggerProjectileSpeed = 2350;//2250;//2500; //3000
        private int MegaFlyingSwordAmount = 29;//31;//35;//35;//50;

        private int MegaUpwardSwordProjectileSpeed = 2450;//2250;
        private int MegaUpwardSwordProjectileAmount = 8;//7;//8;//15;


        private int m_Mega_Shield_Distance = 525;//550;//650;//500;//400;
        private float m_Mega_Shield_Scale = 4.0f;//3.0f;//400;
        private float m_Mega_Shield_Speed = 1.0f;//1.15f;//1.25f;

        private int m_numSpears = 26;
        private float m_spearDuration = 1.75f;//2.25f;//3;//5;

        private bool m_isHurt;
        private bool m_isDashing;
        private bool m_inSecondForm;
        private float m_smokeCounter = 0.05f;
        private float m_castDelay = 0.25f;//0.5f;
        private int m_orbsEasy = 1;//1;
        private int m_orbsNormal = 2;//3;
        private int m_orbsHard = 3;//3;//5;
        private float m_lastBossAttackDelay = 0.35f;//0.25f;

        private bool m_shake = false;
        private bool m_shookLeft = false;

        private float m_shakeTimer = 0;
        private float m_shakeDuration = 0.03f;

        private List<ProjectileObj> m_damageShieldProjectiles;

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_damageShieldLB = new LogicBlock();
        private LogicBlock m_cooldownLB = new LogicBlock();
        private LogicBlock m_secondFormCooldownLB = new LogicBlock();
        private LogicBlock m_firstFormDashAwayLB = new LogicBlock();

        private LogicBlock m_generalBasicNeoLB = new LogicBlock();


        private bool m_firstFormDying = false;
        private float m_teleportDuration = 0;
        private BlankObj m_delayObj;

        private bool m_isNeo = false;
        private bool m_neoDying = false;

        private ProjectileData m_daggerProjData;
        private ProjectileData m_axeProjData;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.LastBoss_Basic_Name;
            LocStringID = EnemyEV.LastBoss_Basic_Name_locID;

            MaxHealth = EnemyEV.LastBoss_Basic_MaxHealth;
            Damage = EnemyEV.LastBoss_Basic_Damage;
            XPValue = EnemyEV.LastBoss_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.LastBoss_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.LastBoss_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.LastBoss_Basic_DropChance;

            Speed = EnemyEV.LastBoss_Basic_Speed;
            TurnSpeed = EnemyEV.LastBoss_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.LastBoss_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.LastBoss_Basic_Jump;
            CooldownTime = EnemyEV.LastBoss_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.LastBoss_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.LastBoss_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.LastBoss_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.LastBoss_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.LastBoss_Basic_IsWeighted;

            Scale = EnemyEV.LastBoss_Basic_Scale;
            ProjectileScale = EnemyEV.LastBoss_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.LastBoss_Basic_Tint;

            MeleeRadius = EnemyEV.LastBoss_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.LastBoss_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.LastBoss_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.LastBoss_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.LastBoss_Miniboss_Name;
                    LocStringID = EnemyEV.LastBoss_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.LastBoss_Miniboss_MaxHealth;
                    Damage = EnemyEV.LastBoss_Miniboss_Damage;
                    XPValue = EnemyEV.LastBoss_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.LastBoss_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.LastBoss_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.LastBoss_Miniboss_DropChance;

                    Speed = EnemyEV.LastBoss_Miniboss_Speed;
                    TurnSpeed = EnemyEV.LastBoss_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.LastBoss_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.LastBoss_Miniboss_Jump;
                    CooldownTime = EnemyEV.LastBoss_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.LastBoss_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.LastBoss_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.LastBoss_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.LastBoss_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.LastBoss_Miniboss_IsWeighted;

                    Scale = EnemyEV.LastBoss_Miniboss_Scale;
                    ProjectileScale = EnemyEV.LastBoss_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.LastBoss_Miniboss_Tint;

                    MeleeRadius = EnemyEV.LastBoss_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.LastBoss_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.LastBoss_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.LastBoss_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.LastBoss_Expert_Name;
                    LocStringID = EnemyEV.LastBoss_Expert_Name_locID;

                    MaxHealth = EnemyEV.LastBoss_Expert_MaxHealth;
                    Damage = EnemyEV.LastBoss_Expert_Damage;
                    XPValue = EnemyEV.LastBoss_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.LastBoss_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.LastBoss_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.LastBoss_Expert_DropChance;

                    Speed = EnemyEV.LastBoss_Expert_Speed;
                    TurnSpeed = EnemyEV.LastBoss_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.LastBoss_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.LastBoss_Expert_Jump;
                    CooldownTime = EnemyEV.LastBoss_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.LastBoss_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.LastBoss_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.LastBoss_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.LastBoss_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.LastBoss_Expert_IsWeighted;

                    Scale = EnemyEV.LastBoss_Expert_Scale;
                    ProjectileScale = EnemyEV.LastBoss_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.LastBoss_Expert_Tint;

                    MeleeRadius = EnemyEV.LastBoss_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.LastBoss_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.LastBoss_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.LastBoss_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.LastBoss_Advanced_Name;
                    LocStringID = EnemyEV.LastBoss_Advanced_Name_locID;

                    MaxHealth = EnemyEV.LastBoss_Advanced_MaxHealth;
                    Damage = EnemyEV.LastBoss_Advanced_Damage;
                    XPValue = EnemyEV.LastBoss_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.LastBoss_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.LastBoss_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.LastBoss_Advanced_DropChance;

                    Speed = EnemyEV.LastBoss_Advanced_Speed;
                    TurnSpeed = EnemyEV.LastBoss_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.LastBoss_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.LastBoss_Advanced_Jump;
                    CooldownTime = EnemyEV.LastBoss_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.LastBoss_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.LastBoss_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.LastBoss_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.LastBoss_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.LastBoss_Advanced_IsWeighted;

                    Scale = EnemyEV.LastBoss_Advanced_Scale;
                    ProjectileScale = EnemyEV.LastBoss_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.LastBoss_Advanced_Tint;

                    MeleeRadius = EnemyEV.LastBoss_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.LastBoss_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.LastBoss_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.LastBoss_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    this.AnimationDelay = 1 / 10f;
                    if (LevelEV.WEAKEN_BOSSES == true)
                        this.MaxHealth = 1;
                    break;
            }		


        }

        protected override void InitializeLogic()
        {
            #region First Form Logic
            //////////////////////////////// FIRST FORM LOGIC
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new DebugTraceLogicAction("WalkTowardSLS"));
            walkTowardsLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            walkTowardsLS.AddAction(new GroundCheckLogicAction());
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character", true, true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkTowardsLS.AddAction(new DelayLogicAction(0.3f, 0.75f));
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new DebugTraceLogicAction("WalkAway"));
            walkAwayLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            walkAwayLS.AddAction(new GroundCheckLogicAction());
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character", true, true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            //walkAwayLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkAwayLS.AddAction(new DelayLogicAction(0.2f, 0.75f));
            walkAwayLS.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new DebugTraceLogicAction("walkStop"));
            walkStopLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            walkStopLS.AddAction(new GroundCheckLogicAction());
            walkStopLS.AddAction(new ChangeSpriteLogicAction("PlayerIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.25f, 0.5f));

            LogicSet attackLS = new LogicSet(this);
            attackLS.AddAction(new DebugTraceLogicAction("attack"));
            attackLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            attackLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 20f));
            attackLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackLS.AddAction(new ChangeSpriteLogicAction("PlayerAttacking3_Character", false, false));
            attackLS.AddAction(new PlayAnimationLogicAction(2, 4));
            attackLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Player_Attack01", "Player_Attack02"));
            attackLS.AddAction(new PlayAnimationLogicAction("AttackStart", "End"));
            attackLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            attackLS.AddAction(new ChangeSpriteLogicAction("PlayerIdle_Character", true, true));
            attackLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet moveAttackLS = new LogicSet(this);
            moveAttackLS.AddAction(new DebugTraceLogicAction("moveattack"));
            moveAttackLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            moveAttackLS.AddAction(new MoveLogicAction(m_target, true));
            moveAttackLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 20f));
            moveAttackLS.AddAction(new LockFaceDirectionLogicAction(true));
            moveAttackLS.AddAction(new ChangeSpriteLogicAction("PlayerAttacking3_Character", false, false));
            moveAttackLS.AddAction(new PlayAnimationLogicAction(2, 4));
            moveAttackLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Player_Attack01", "Player_Attack02"));
            moveAttackLS.AddAction(new PlayAnimationLogicAction("AttackStart", "End"));
            moveAttackLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            moveAttackLS.AddAction(new ChangeSpriteLogicAction("PlayerIdle_Character", true, true));
            moveAttackLS.AddAction(new LockFaceDirectionLogicAction(false));
            moveAttackLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwShieldLS = new LogicSet(this);
            throwShieldLS.AddAction(new DebugTraceLogicAction("Throwing Daggers"));
            throwShieldLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwShieldLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwShieldLS.AddAction(new ChangeSpriteLogicAction("PlayerLevelUp_Character"));
            throwShieldLS.AddAction(new PlayAnimationLogicAction(false));
            //CastCloseShield(throwShieldLS);
            throwShieldLS.AddAction(new RunFunctionLogicAction(this, "CastCloseShield"));
            throwShieldLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwShieldLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwDaggerLS = new LogicSet(this);
            throwDaggerLS.AddAction(new DebugTraceLogicAction("Throwing Daggers"));
            throwDaggerLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            throwDaggerLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwDaggerLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwDaggerLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 30f));
            throwDaggerLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", false));
            throwDaggerLS.AddAction(new ChangeSpriteLogicAction("PlayerLevelUp_Character"));
            throwDaggerLS.AddAction(new PlayAnimationLogicAction(false));
            throwDaggerLS.AddAction(new RunFunctionLogicAction(this, "ThrowDaggerProjectiles"));
            throwDaggerLS.AddAction(new DelayLogicAction(0.25f));
            throwDaggerLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            throwDaggerLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwDaggerLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            throwDaggerLS.Tag = GameTypes.LogicSetType_ATTACK;

            #region NEODaggers
            LogicSet throwDaggerNeoLS = new LogicSet(this);
            throwDaggerNeoLS.AddAction(new DebugTraceLogicAction("Throwing Daggers"));
            throwDaggerNeoLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            throwDaggerNeoLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwDaggerNeoLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwDaggerNeoLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 30f));
            throwDaggerNeoLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", false));
            throwDaggerNeoLS.AddAction(new ChangeSpriteLogicAction("PlayerLevelUp_Character"));
            throwDaggerNeoLS.AddAction(new PlayAnimationLogicAction(false));
            throwDaggerNeoLS.AddAction(new RunFunctionLogicAction(this, "ThrowDaggerProjectilesNeo"));
            throwDaggerNeoLS.AddAction(new DelayLogicAction(0.25f));
            throwDaggerNeoLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            throwDaggerNeoLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwDaggerNeoLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            throwDaggerNeoLS.Tag = GameTypes.LogicSetType_ATTACK;
            #endregion

            LogicSet jumpLS = new LogicSet(this);
            jumpLS.AddAction(new DebugTraceLogicAction("jumpLS"));
            jumpLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            jumpLS.AddAction(new GroundCheckLogicAction());
            jumpLS.AddAction(new MoveLogicAction(m_target, true));
            jumpLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Player_Jump"));
            jumpLS.AddAction(new JumpLogicAction());
            jumpLS.AddAction(new DelayLogicAction(0.2f));
            //ThrowAxeProjectiles(jumpLS);
            jumpLS.AddAction(new RunFunctionLogicAction(this, "ThrowAxeProjectiles"));
            jumpLS.AddAction(new DelayLogicAction(0.75f));
            jumpLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpLS.AddAction(new GroundCheckLogicAction());

            #region NEOJump
            LogicSet jumpNeoLS = new LogicSet(this);
            jumpNeoLS.AddAction(new DebugTraceLogicAction("jumpLS"));
            jumpNeoLS.AddAction(new ChangePropertyLogicAction(this, "CanBeKnockedBack", true));
            jumpNeoLS.AddAction(new GroundCheckLogicAction());
            jumpNeoLS.AddAction(new MoveLogicAction(m_target, true));
            jumpNeoLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpNeoLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Player_Jump"));
            jumpNeoLS.AddAction(new JumpLogicAction());
            jumpNeoLS.AddAction(new DelayLogicAction(0.2f));
            //ThrowAxeProjectiles(jumpLS);
            jumpNeoLS.AddAction(new RunFunctionLogicAction(this, "ThrowAxeProjectilesNeo"));
            jumpNeoLS.AddAction(new DelayLogicAction(0.75f));
            jumpNeoLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpNeoLS.AddAction(new GroundCheckLogicAction());
            #endregion

            LogicSet dashLS = new LogicSet(this);
            dashLS.AddAction(new DebugTraceLogicAction("dashLS"));
            //CastCloseShield(dashLS);
            dashLS.AddAction(new RunFunctionLogicAction(this, "CastCloseShield"));
            dashLS.AddAction(new RunFunctionLogicAction(this, "Dash", 0));
            dashLS.AddAction(new DelayLogicAction(0.25f));
            dashLS.AddAction(new RunFunctionLogicAction(this, "DashComplete"));

            LogicSet dashRightLS = new LogicSet(this);
            dashRightLS.AddAction(new DebugTraceLogicAction("dashAwayRightLS"));
            dashRightLS.AddAction(new RunFunctionLogicAction(this, "Dash", 1));
            dashRightLS.AddAction(new DelayLogicAction(0.25f));
            dashRightLS.AddAction(new RunFunctionLogicAction(this, "DashComplete"));

            LogicSet dashLeftLS = new LogicSet(this);
            dashLeftLS.AddAction(new DebugTraceLogicAction("dashAwayLeftLS"));
            dashLeftLS.AddAction(new RunFunctionLogicAction(this, "Dash", -1));
            dashLeftLS.AddAction(new DelayLogicAction(0.25f));
            dashLeftLS.AddAction(new RunFunctionLogicAction(this, "DashComplete"));

            #endregion


            #region SecondBoss
            ////////////////////////////// SECOND FORM LOGIC
            LogicSet walkTowardsSF = new LogicSet(this);
            walkTowardsSF.AddAction(new GroundCheckLogicAction());
            walkTowardsSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossRun_Character", true, true));
            walkTowardsSF.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsSF.AddAction(new LockFaceDirectionLogicAction(true));
            walkTowardsSF.AddAction(new DelayLogicAction(0.35f, 1.15f));
            walkTowardsSF.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet walkAwaySF = new LogicSet(this);
            walkAwaySF.AddAction(new GroundCheckLogicAction());
            walkAwaySF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossRun_Character", true, true));
            walkAwaySF.AddAction(new MoveLogicAction(m_target, false));
            //walkAwayLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkAwaySF.AddAction(new DelayLogicAction(0.2f, 1.0f));
            walkAwaySF.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet walkStopSF = new LogicSet(this);
            walkStopSF.AddAction(new GroundCheckLogicAction());
            walkStopSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossIdle_Character", true, true));
            walkStopSF.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopSF.AddAction(new DelayLogicAction(0.2f, 0.5f));

            LogicSet attackSF = new LogicSet(this);
            attackSF.AddAction(new MoveLogicAction(m_target, true, 0));
            attackSF.AddAction(new LockFaceDirectionLogicAction(true));
            attackSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossAttack_Character", false, false));
            attackSF.AddAction(new PlayAnimationLogicAction("Start", "BeforeAttack", false));
            attackSF.AddAction(new DelayLogicAction(m_lastBossAttackDelay));
            attackSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSwing"));
            attackSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_Effort_01", "FinalBoss_St2_Effort_02", "FinalBoss_St2_Effort_03", "FinalBoss_St2_Effort_04", "FinalBoss_St2_Effort_05"));
            attackSF.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            attackSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossIdle_Character", true, true));
            attackSF.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet castSpearsSF = new LogicSet(this);
            RunTeleportLS(castSpearsSF, "Centre");
            castSpearsSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell_Character", false, false));
            castSpearsSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSlam_Prime"));
            castSpearsSF.AddAction(new PlayAnimationLogicAction("Start", "BeforeCast"));
            castSpearsSF.AddAction(new DelayLogicAction(m_castDelay));
            castSpearsSF.AddAction(new RunFunctionLogicAction(this, "CastSpears", m_numSpears, m_spearDuration));
            castSpearsSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSlam"));
            castSpearsSF.AddAction(new PlayAnimationLogicAction("BeforeCast", "End"));
            castSpearsSF.AddAction(new DelayLogicAction(m_spearDuration + 1));

            LogicSet castRandomSwordsSF = new LogicSet(this);
            castRandomSwordsSF.AddAction(new ChangePropertyLogicAction(this, "CurrentSpeed", 0));
            castRandomSwordsSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell2_Character", false, false));
            castRandomSwordsSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSummon_a"));
            castRandomSwordsSF.AddAction(new PlayAnimationLogicAction("Start", "Cast"));
            castRandomSwordsSF.AddAction(new DelayLogicAction(m_castDelay));
            castRandomSwordsSF.AddAction(new RunFunctionLogicAction(this, "CastSwordsRandom"));
            castRandomSwordsSF.AddAction(new PlayAnimationLogicAction("Cast", "End"));
            castRandomSwordsSF.AddAction(new DelayLogicAction(1));

            LogicSet castSwordsLeftSF = new LogicSet(this);
            castSwordsLeftSF.AddAction(new LockFaceDirectionLogicAction(true, 1));
            RunTeleportLS(castSwordsLeftSF, "Left");
            castSwordsLeftSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell_Character", false, false));
            castSwordsLeftSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSlam_Prime"));
            castSwordsLeftSF.AddAction(new PlayAnimationLogicAction("Start", "BeforeCast"));
            castSwordsLeftSF.AddAction(new DelayLogicAction(m_castDelay));
            castSwordsLeftSF.AddAction(new RunFunctionLogicAction(this, "CastSwords", true));
            castSwordsLeftSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSlam"));
            castSwordsLeftSF.AddAction(new PlayAnimationLogicAction("BeforeCast", "End"));
            castSwordsLeftSF.AddAction(new DelayLogicAction(1));
            castSwordsLeftSF.AddAction(new LockFaceDirectionLogicAction(false, 0));

            LogicSet castSwordRightSF = new LogicSet(this);
            castSwordRightSF.AddAction(new LockFaceDirectionLogicAction(true, -1));
            RunTeleportLS(castSwordRightSF, "Right");
            castSwordRightSF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell_Character", false, false));
            castSwordRightSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSlam_Prime"));
            castSwordRightSF.AddAction(new PlayAnimationLogicAction("Start", "BeforeCast"));
            castSwordRightSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_BlockLaugh"));            
            castSwordRightSF.AddAction(new DelayLogicAction(m_castDelay));
            castSwordRightSF.AddAction(new RunFunctionLogicAction(this, "CastSwords", false));
            castSwordRightSF.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_SwordSlam"));
            castSwordRightSF.AddAction(new PlayAnimationLogicAction("BeforeCast", "End"));
            castSwordRightSF.AddAction(new DelayLogicAction(1));
            castSwordRightSF.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet castShield1SF = new LogicSet(this);
            //castShield1SF.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 20f));
            //castShield1SF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell_Character", false, false));
            //castShield1SF.AddAction(new PlayAnimationLogicAction("Start", "BeforeCast"));
            //castShield1SF.AddAction(new DelayLogicAction(m_castDelay));
            castShield1SF.AddAction(new RunFunctionLogicAction(this, "CastDamageShield", m_orbsEasy));
            //castShield1SF.AddAction(new PlayAnimationLogicAction("BeforeCast", "End"));
            //castShield1SF.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            //castShield1SF.AddAction(new DelayLogicAction(0));
            castShield1SF.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet castShield2SF = new LogicSet(this);
            //castShield2SF.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 20f));
            //castShield2SF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell_Character", false, false));
            //castShield2SF.AddAction(new PlayAnimationLogicAction("Start", "BeforeCast"));
            //castShield2SF.AddAction(new DelayLogicAction(m_castDelay));
            castShield2SF.AddAction(new RunFunctionLogicAction(this, "CastDamageShield", m_orbsNormal));
            //castShield2SF.AddAction(new PlayAnimationLogicAction("BeforeCast", "End"));
            //castShield2SF.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            //castShield2SF.AddAction(new DelayLogicAction(0));
            castShield2SF.AddAction(new LockFaceDirectionLogicAction(false));

            LogicSet castShield3SF = new LogicSet(this);
            //castShield3SF.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 20f));
            //castShield3SF.AddAction(new ChangeSpriteLogicAction("EnemyLastBossSpell_Character", false, false));
            //castShield3SF.AddAction(new PlayAnimationLogicAction("Start", "BeforeCast"));
            //castShield3SF.AddAction(new DelayLogicAction(m_castDelay));
            castShield3SF.AddAction(new RunFunctionLogicAction(this, "CastDamageShield", m_orbsHard));
            //castShield3SF.AddAction(new PlayAnimationLogicAction("BeforeCast", "End"));
            //castShield3SF.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            castShield3SF.AddAction(new DelayLogicAction(0));
            castShield3SF.AddAction(new LockFaceDirectionLogicAction(false));

            #endregion


            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF);
            m_damageShieldLB.AddLogicSet(castShield1SF, castShield2SF, castShield3SF);
            m_cooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS);
            m_secondFormCooldownLB.AddLogicSet(walkTowardsSF, walkAwaySF, walkStopSF);

            m_generalBasicNeoLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, jumpNeoLS, moveAttackLS, throwShieldLS, throwDaggerNeoLS, dashLS);


            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_damageShieldLB);
            logicBlocksToDispose.Add(m_cooldownLB);
            logicBlocksToDispose.Add(m_secondFormCooldownLB);

            logicBlocksToDispose.Add(m_generalBasicNeoLB);
            
            // Special logic block to get out of corners.
            m_firstFormDashAwayLB.AddLogicSet(dashLeftLS, dashRightLS);
            logicBlocksToDispose.Add(m_firstFormDashAwayLB);

            SetCooldownLogicBlock(m_cooldownLB, 70, 0, 30, 0, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
            //TEDDY MEGA BOSS COOLDOWN <- Look for that to find boss variant
            base.InitializeLogic();
        }

        private void RunTeleportLS(LogicSet logicSet, string roomPosition)
        {
            //logicSet.AddAction(new GroundCheckLogicAction());
            logicSet.AddAction(new ChangePropertyLogicAction(this, "IsCollidable", false));
            logicSet.AddAction(new ChangePropertyLogicAction(this, "IsWeighted", false));
            logicSet.AddAction(new ChangePropertyLogicAction(this, "Opacity", 0.5f));
            logicSet.AddAction(new ChangePropertyLogicAction(this, "CurrentSpeed", 0));
            logicSet.AddAction(new ChangeSpriteLogicAction("EnemyLastBossTeleport_Character", false, false));
            logicSet.AddAction(new Play3DSoundLogicAction(this, m_target, "FinalBoss_St2_BlockAction"));
            logicSet.AddAction(new DelayLogicAction(0.25f));
            logicSet.AddAction(new RunFunctionLogicAction(this, "TeleportTo", roomPosition));
            //logicSet.AddAction(new DelayLogicAction(0.25f)); //1.1f
            logicSet.AddAction(new DelayObjLogicAction(m_delayObj));
            logicSet.AddAction(new ChangePropertyLogicAction(this, "IsCollidable", true));
            logicSet.AddAction(new ChangePropertyLogicAction(this, "IsWeighted", true));
            logicSet.AddAction(new ChangePropertyLogicAction(this, "Opacity", 1));
        }

        public void ThrowAxeProjectiles()
        {
            if (m_axeProjData != null)
            {
                m_axeProjData.Dispose();
                m_axeProjData = null;
            }

            m_axeProjData = new ProjectileData(this)
            {
                SpriteName = "SpellAxe_Sprite",
                SourceAnchor = new Vector2(20, -20),
                Target = null,
                Speed = new Vector2(AxeProjectileSpeed, AxeProjectileSpeed),
                IsWeighted = true,
                RotationSpeed = 10,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(-90, -90),//(-72, -72),
                CollidesWithTerrain = false,
                Scale = AxeSpellScale,
            };

            Tween.RunFunction(0, this, "CastAxe", false);
            Tween.RunFunction(0.15f, this, "CastAxe", true);
            Tween.RunFunction(0.3f, this, "CastAxe", true);
            Tween.RunFunction(0.45f, this, "CastAxe", true);
            Tween.RunFunction(0.6f, this, "CastAxe", true);
        }


        public void ThrowAxeProjectilesNeo()
        {
            if (m_axeProjData != null)
            {
                m_axeProjData.Dispose();
                m_axeProjData = null;
            }

            m_axeProjData = new ProjectileData(this)
            {
                SpriteName = "SpellAxe_Sprite",
                SourceAnchor = new Vector2(20, -20),
                Target = null,
                Speed = new Vector2(AxeProjectileSpeed, AxeProjectileSpeed),
                IsWeighted = true,
                RotationSpeed = 10,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(-90, -90),//(-72, -72),
                CollidesWithTerrain = false,
                Scale = AxeSpellScale,
            };

            //Tween.RunFunction(0, this, "CastAxe", false);
            //Tween.RunFunction(0.15f, this, "CastAxe", true);
            Tween.RunFunction(0.3f, this, "CastAxe", true);
            Tween.RunFunction(0.3f, this, "CastAxe", true);
            Tween.RunFunction(0.3f, this, "CastAxe", true);
            //Tween.RunFunction(0.45f, this, "CastAxe", true);
            //Tween.RunFunction(0.6f, this, "CastAxe", true);
        }

        public void CastAxe(bool randomize)
        {
            if (randomize == true)
                m_axeProjData.AngleOffset = CDGMath.RandomInt(-70,70);
            m_levelScreen.ProjectileManager.FireProjectile(m_axeProjData);
            SoundManager.Play3DSound(this, m_target, "Cast_Axe");
            m_levelScreen.ImpactEffectPool.LastBossSpellCastEffect(this, 45, true);
        }

        //private void ThrowAxeProjectiles(LogicSet ls)
        //{
        //    ProjectileData projData = new ProjectileData(this)
        //    {
        //        SpriteName = "SpellAxe_Sprite",
        //        SourceAnchor = new Vector2(20, -20),
        //        Target = null,
        //        Speed = new Vector2(AxeProjectileSpeed, AxeProjectileSpeed),
        //        IsWeighted = true,
        //        RotationSpeed = 10,
        //        Damage = Damage,
        //        AngleOffset = 0,
        //        Angle = new Vector2(-90, -90),//(-72, -72),
        //        CollidesWithTerrain = false,
        //        Scale = AxeSpellScale,
        //    };

        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Axe"));            
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, 45, true)); 
        //    ls.AddAction(new DelayLogicAction(0.15f));
        //    projData.AngleOffset = CDGMath.RandomInt(-70, 70);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Axe"));
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, 45, true)); 
        //    ls.AddAction(new DelayLogicAction(0.15f));
        //    projData.AngleOffset = CDGMath.RandomInt(-70, 70);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Axe"));
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, 45, true));
        //    projData.AngleOffset = CDGMath.RandomInt(-70, 70);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Axe"));
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, 45, true)); 
        //    ls.AddAction(new DelayLogicAction(0.15f));
        //    projData.AngleOffset = CDGMath.RandomInt(-70, 70);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Axe"));
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, 45, true));
        //    ls.AddAction(new DelayLogicAction(0.15f));


        //    projData.Dispose();
        //}

        //private void ThrowDaggerProjectiles(LogicSet ls)
        //{
        //    ProjectileData projData = new ProjectileData(this)
        //    {
        //        SpriteName = "SpellDagger_Sprite",
        //        SourceAnchor = Vector2.Zero,
        //        Target = m_target,
        //        Speed = new Vector2(DaggerProjectileSpeed, DaggerProjectileSpeed),
        //        IsWeighted = false,
        //        RotationSpeed = 0,
        //        Damage = Damage,
        //        AngleOffset = 0,
        //        CollidesWithTerrain = false,
        //        Scale = DaggerSpellScale,
        //    };

        //    float rotation = 0;
        //    float x = m_target.X - this.X;
        //    float y = m_target.Y - this.Y;
        //    rotation = MathHelper.ToDegrees((float)Math.Atan2(y, x));

        //    //m_levelScreen.ImpactEffectPool.SpellCastEffect(this.Position, rotation, true);

        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Dagger"));
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, rotation, true)); 
        //    ls.AddAction(new DelayLogicAction(0.05f));
        //    projData.AngleOffset = CDGMath.RandomInt(-8, 8);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Dagger"));
        //    ls.AddAction(new DelayLogicAction(0.05f));
        //    projData.AngleOffset = CDGMath.RandomInt(-8, 8);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Dagger"));
        //    ls.AddAction(new DelayLogicAction(0.05f));
        //    projData.AngleOffset = CDGMath.RandomInt(-8, 8);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Dagger"));
        //    ls.AddAction(new DelayLogicAction(0.05f));
        //    projData.AngleOffset = CDGMath.RandomInt(-8, 8);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_Dagger"));

        //    projData.Dispose();
        //}

        public void ThrowDaggerProjectiles()
        {
            if (m_daggerProjData != null)
            {
                m_daggerProjData.Dispose();
                m_daggerProjData = null;
            }

            m_daggerProjData = new ProjectileData(this)
            {
                SpriteName = "SpellDagger_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = m_target,
                Speed = new Vector2(DaggerProjectileSpeed, DaggerProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = DaggerSpellScale,
            };

            //m_levelScreen.ImpactEffectPool.SpellCastEffect(this.Position, rotation, true);
            Tween.RunFunction(0, this, "CastDaggers", false);
            Tween.RunFunction(0.05f, this, "CastDaggers", true);
            Tween.RunFunction(0.1f, this, "CastDaggers", true);
            Tween.RunFunction(0.15f, this, "CastDaggers", true);
            Tween.RunFunction(0.2f, this, "CastDaggers", true);
        }

        public void ThrowDaggerProjectilesNeo()
        {
            if (m_daggerProjData != null)
            {
                m_daggerProjData.Dispose();
                m_daggerProjData = null;
            }

            m_daggerProjData = new ProjectileData(this)
            {
                SpriteName = "SpellDagger_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = m_target,
                Speed = new Vector2(DaggerProjectileSpeed - 160, DaggerProjectileSpeed - 160),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = DaggerSpellScale,
            };

            //m_levelScreen.ImpactEffectPool.SpellCastEffect(this.Position, rotation, true);
            Tween.RunFunction(0, this, "CastDaggers", false);
            Tween.RunFunction(0.05f, this, "CastDaggers", true);
            Tween.RunFunction(0.1f, this, "CastDaggers", true);
        }


        public void CastDaggers(bool randomize)
        {
            if (randomize == true)
                m_daggerProjData.AngleOffset = CDGMath.RandomInt(-8, 8);
            m_levelScreen.ProjectileManager.FireProjectile(m_daggerProjData);
            SoundManager.Play3DSound(this, m_target, "Cast_Dagger");
            m_levelScreen.ImpactEffectPool.LastBossSpellCastEffect(this, 0, true);
        }

        public void CastCloseShield()
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "SpellClose_Sprite",
                //Angle = new Vector2(90, 90),
                //SourceAnchor = new Vector2(120, -60),//(75,-200),//(50, 0),
                Speed = new Vector2(0, 0),//(450,450),//(1000, 1000),
                IsWeighted = false,
                RotationSpeed = 0f,
                DestroysWithEnemy = false,
                DestroysWithTerrain = false,
                CollidesWithTerrain = false,
                Scale = new Vector2(m_Spell_Close_Scale, m_Spell_Close_Scale),
                Damage = Damage,
                Lifespan = m_Spell_Close_Lifespan,
                LockPosition = true,
            };

            m_levelScreen.ProjectileManager.FireProjectile(projData);
            SoundManager.Play3DSound(this, m_target, "Cast_GiantSword");
            m_levelScreen.ImpactEffectPool.LastBossSpellCastEffect(this, 90, true);
            projData.Dispose();
        }

        //private void CastCloseShield(LogicSet ls)
        //{
        //    ProjectileData projData = new ProjectileData(this)
        //    {
        //        SpriteName = "SpellClose_Sprite",
        //        //Angle = new Vector2(90, 90),
        //        //SourceAnchor = new Vector2(120, -60),//(75,-200),//(50, 0),
        //        Speed = new Vector2(0, 0),//(450,450),//(1000, 1000),
        //        IsWeighted = false,
        //        RotationSpeed = 0f,
        //        DestroysWithEnemy = false,
        //        DestroysWithTerrain = false,
        //        CollidesWithTerrain = false,
        //        Scale = new Vector2(m_Spell_Close_Scale, m_Spell_Close_Scale),
        //        Damage = Damage,
        //        Lifespan = m_Spell_Close_Lifespan,
        //        LockPosition = true,
        //    };

        //    ls.AddAction(new Play3DSoundLogicAction(this, m_target, "Cast_GiantSword"));
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    ls.AddAction(new RunFunctionLogicAction(m_levelScreen.ImpactEffectPool, "LastBossSpellCastEffect", this, 90, true));

        //    projData.Dispose();
        //}

        #region Basic and Advanced Logic

        protected override void RunBasicLogic()
        {
            if (CurrentHealth > 0)
            {
                if (m_inSecondForm == false)
                {
                    if (m_isHurt == false)
                    {
                        switch (State)
                        {
                            case (STATE_MELEE_ENGAGE):
                                if (IsNeo == false)
                                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 0, 35, 35, 00, 0, 30); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicNeoLB, 0, 0, 0, 50, 20, 00, 0, 30); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                break;
                            case (STATE_PROJECTILE_ENGAGE):
                                if (IsNeo == false)
                                    RunLogicBlock(true, m_generalBasicLB, 35, 0, 0, 25, 0, 00, 20, 20); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicNeoLB, 25, 0, 20, 15, 0, 00, 15, 25); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                break;
                            case (STATE_ENGAGE):
                                if (IsNeo == false)
                                    RunLogicBlock(true, m_generalBasicLB, 40, 0, 0, 20, 0, 00, 40, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicNeoLB, 40, 0, 20, 20, 0, 00, 20, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                break;
                            case (STATE_WANDER):
                                if (IsNeo == false)
                                    RunLogicBlock(true, m_generalBasicLB, 50, 0, 0, 0, 0, 00, 50, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicNeoLB, 50, 0, 10, 10, 0, 00, 30, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwShieldLS, throwDaggerLS, dashLS
                                break;
                            default:
                                break;
                        }
                    }

                }
                else
                    RunAdvancedLogic();
            }
        }

        protected override void RunAdvancedLogic()
        {
            //if ((CurrentHealth < (MaxHealth / 3) * 2) && m_damageShieldProjectiles.Count < m_orbsNormal)
            //    RunLogicBlock(false, m_damageShieldLB, 0, 100, 0);  // castShield1SF, castShield2SF, castShield3SF
            //else if ((CurrentHealth < (MaxHealth / 3) * 1) && m_damageShieldProjectiles.Count < m_orbsHard)
            //    RunLogicBlock(false, m_damageShieldLB, 0, 0, 100);  // castShield1SF, castShield2SF, castShield3SF
            //else if (m_damageShieldProjectiles.Count < 1)
            //    RunLogicBlock(false, m_damageShieldLB, 100, 0, 0);  // castShield1SF, castShield2SF, castShield3SF
            //else
            {
                //RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 0, 100, 0, 0, 0, 0);
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                        RunLogicBlock(true, m_generalAdvancedLB, 31, 15, 0, 26, 3, 13, 6, 6); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
                        break;
                    case (STATE_PROJECTILE_ENGAGE):
                        RunLogicBlock(true, m_generalAdvancedLB, 52, 12, 0, 0, 11, 15, 5, 5); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalAdvancedLB, 68, 0, 0, 0, 10, 12, 5, 5); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalAdvancedLB, 63, 0, 0, 0, 15, 12, 5, 5); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Expert and Miniboss (Not Used)

        protected override void RunExpertLogic()
        {
            if (CurrentHealth > 0)
            {
                if (m_inSecondForm == false)
                {
                    if (m_isHurt == false)
                    {
                        switch (State)
                        {
                            case (STATE_MELEE_ENGAGE):
                                if (m_isTouchingGround == true)
                                    RunLogicBlock(true, m_generalBasicLB, 0, 10, 0, 20, 35, 10, 0, 25); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicLB, 0, 10, 0, 0, 55, 10, 0, 25); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                break;
                            case (STATE_PROJECTILE_ENGAGE):
                                if (m_target.IsJumping == false)
                                    RunLogicBlock(true, m_generalBasicLB, 20, 0, 10, 10, 0, 15, 20, 10); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicLB, 40, 0, 15, 0, 0, 15, 20, 10); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                break;
                            case (STATE_ENGAGE):
                                if (m_target.IsJumping == false)
                                    RunLogicBlock(true, m_generalBasicLB, 30, 0, 15, 20, 0, 25, 0, 10); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicLB, 50, 0, 15, 0, 0, 25, 0, 10); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                break;
                            case (STATE_WANDER):
                                if (m_target.IsJumping == false)
                                    RunLogicBlock(true, m_generalBasicLB, 50, 0, 10, 20, 0, 0, 20, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                else
                                    RunLogicBlock(true, m_generalBasicLB, 50, 0, 10, 20, 0, 0, 20, 0); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS, moveAttackLS, throwAxeLS, throwDaggerLS, dashLS
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                    RunAdvancedLogic();
            }
        }


        protected override void RunMinibossLogic()
        {
            RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 0, 0, 100, 0, 0, 0); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
            //if ((CurrentHealth < (MaxHealth / 3) * 2) && m_damageShieldProjectiles.Count < m_orbsNormal)
            //    RunLogicBlock(false, m_damageShieldLB, 0, 100, 0);  // castShield1SF, castShield2SF, castShield3SF
            //else if ((CurrentHealth < (MaxHealth / 3) * 1) && m_damageShieldProjectiles.Count < m_orbsHard)
            //    RunLogicBlock(false, m_damageShieldLB, 0, 0, 100);  // castShield1SF, castShield2SF, castShield3SF
            //else if (m_damageShieldProjectiles.Count < 1)
            //    RunLogicBlock(false, m_damageShieldLB, 100, 0, 0);  // castShield1SF, castShield2SF, castShield3SF
            //else
            //{
            //    switch (State)
            //    {
            //        case (STATE_MELEE_ENGAGE):
            //            RunLogicBlock(true, m_generalAdvancedLB, 30, 20, 0, 40, 0, 0, 5, 5); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
            //            break;
            //        case (STATE_PROJECTILE_ENGAGE):
            //            RunLogicBlock(true, m_generalAdvancedLB, 50, 20, 10, 0, 5, 5, 5, 5); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
            //            break;
            //        case (STATE_ENGAGE):
            //            RunLogicBlock(true, m_generalAdvancedLB, 80, 0, 0, 0, 10, 10, 0, 0); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
            //            break;
            //        case (STATE_WANDER):
            //            RunLogicBlock(true, m_generalAdvancedLB, 80, 0, 0, 0, 10, 10, 0, 0); //walkTowardsSF, walkAwaySF, walkStopSF, attackSF, castSpearsSF, castRandomSwordsSF, castSwordsLeftSF, castSwordRightSF
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }


        #endregion

        public void TeleportTo(string roomPosition)
        {
            Vector2 position = Vector2.Zero;
            float xDistance = 0;

            switch (roomPosition)
            {
                case ("Left"):
                    xDistance = m_levelScreen.CurrentRoom.Bounds.Left + 200;
                    break;
                case ("Right"):
                    xDistance = m_levelScreen.CurrentRoom.Bounds.Right - 200;
                    break;
                case ("Centre"):
                    xDistance = m_levelScreen.CurrentRoom.Bounds.Center.X;
                    break;
            }
            position = new Vector2(xDistance, this.Y);

            float totalMovement = Math.Abs(CDGMath.DistanceBetweenPts(this.Position, position));
            m_teleportDuration = totalMovement * 0.001f;
            m_delayObj.X = m_teleportDuration; // Delay hack.
            Tween.To(this, m_teleportDuration, Quad.EaseInOut, "X", position.X.ToString());
            SoundManager.Play3DSound(this, m_target, "FinalBoss_St2_BlockMove");
        }

        public void CastSwords(bool castLeft)
        {
            ProjectileData swordData = new ProjectileData(this)
            {
                SpriteName = "LastBossSwordProjectile_Sprite",
                Target = null,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                StartingRotation = 0,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                DestroysWithEnemy = false,
            };

            float delay = 1;
            int projSpeed = MegaFlyingDaggerProjectileSpeed;
            if (castLeft == false)
                projSpeed = (MegaFlyingDaggerProjectileSpeed * -1);
            SoundManager.Play3DSound(this, m_target, "FinalBoss_St2_SwordSummon_b");
            for (int i = 0; i < MegaFlyingSwordAmount; i++)
            {
                Vector2 spellPos = new Vector2(this.X, this.Y + CDGMath.RandomInt(-1320, 100));
                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(swordData);
                proj.Position = spellPos;
                Tween.By(proj, 2.5f, Tween.EaseNone, "delay", delay.ToString(), "X", projSpeed.ToString());
                Tween.AddEndHandlerToLastTween(proj, "KillProjectile");
                Tween.RunFunction(delay, typeof(SoundManager), "Play3DSound", this, m_target, new string[]{"FinalBoss_St2_SwordSummon_c_01", "FinalBoss_St2_SwordSummon_c_02",
                    "FinalBoss_St2_SwordSummon_c_03", "FinalBoss_St2_SwordSummon_c_04", "FinalBoss_St2_SwordSummon_c_05", "FinalBoss_St2_SwordSummon_c_06",
                    "FinalBoss_St2_SwordSummon_c_07", "FinalBoss_St2_SwordSummon_c_08"});
                m_levelScreen.ImpactEffectPool.SpellCastEffect(spellPos, 0, false);
                delay += 0.075f;//0.05f;
            }
        }

        public void CastSpears(int numSpears, float duration)
        {
            ProjectileData spearData = new ProjectileData(this)
            {
                SpriteName = "LastBossSpearProjectile_Sprite",
                Target = null,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                StartingRotation = 0,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                DestroysWithEnemy = false,
                ShowIcon = false,
                LockPosition = true,
                CanBeFusRohDahed = false,
            };

            int xOffsetRight = 0;
            int xOffsetLeft = 0;
            float delay = 0.5f;
            this.UpdateCollisionBoxes();
            Vector2 roomCentre = new Vector2(m_levelScreen.CurrentRoom.Bounds.Center.X, this.Y);

            for (int i = 0; i < numSpears; i++)
            {
                // Spears spreading right.
                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(spearData);
                proj.Scale = new Vector2(2, 2);
                proj.X = roomCentre.X + 50 + xOffsetRight;
                proj.Y = this.Y + (this.Bounds.Bottom - this.Y);// this.Bounds.Bottom + proj.Width / 2f - 10;
                proj.StopAnimation();
                //Tween.By(proj, 0.2f, Tween.EaseNone, "delay", delay.ToString(), "Y", (-(proj.Width + 10)).ToString());
                //proj.Y -= proj.Width;
                //Tween.By(proj, 0.2f, Tween.EaseNone, "delay", (delay + 1).ToString(), "Y", proj.Width.ToString());
                //proj.Y += proj.Width;
                xOffsetRight += proj.Width;
                Tween.RunFunction(delay, typeof(SoundManager), "Play3DSound", this, m_target, new string[]{"FinalBoss_St2_Lance_01", "FinalBoss_St2_Lance_02",
                    "FinalBoss_St2_Lance_03", "FinalBoss_St2_Lance_04", "FinalBoss_St2_Lance_05", "FinalBoss_St2_Lance_06", "FinalBoss_St2_Lance_07", "FinalBoss_St2_Lance_08"});
                Tween.RunFunction(delay, proj, "PlayAnimation", "Before", "End", false);
                Tween.RunFunction(delay + duration, proj, "PlayAnimation", "Retract", "RetractComplete", false);
                Tween.RunFunction(delay + duration, typeof(SoundManager), "Play3DSound", this, m_target, new string[]{"FinalBoss_St2_Lance_Retract_01", "FinalBoss_St2_Lance_Retract_02",
                                    "FinalBoss_St2_Lance_Retract_03", "FinalBoss_St2_Lance_Retract_04", "FinalBoss_St2_Lance_Retract_05", "FinalBoss_St2_Lance_Retract_06"});
                Tween.RunFunction(delay + duration + 1, proj, "KillProjectile");

                // Spears spreading left.
                ProjectileObj projLeft = m_levelScreen.ProjectileManager.FireProjectile(spearData);
                projLeft.Scale = new Vector2(2, 2);
                projLeft.X = roomCentre.X - 50 + xOffsetLeft;
                projLeft.Y = this.Y + (this.Bounds.Bottom - this.Y);
                projLeft.StopAnimation();
                //Tween.By(projLeft, 0.2f, Tween.EaseNone, "delay", delay.ToString(), "Y", (-(projLeft.Width + 10)).ToString());
                //projLeft.Y -= projLeft.Width;
                //Tween.By(projLeft, 0.2f, Tween.EaseNone, "delay", (delay + 1).ToString(), "Y", projLeft.Width.ToString());
                //projLeft.Y += projLeft.Width;
                xOffsetLeft -= projLeft.Width;
                Tween.RunFunction(delay, projLeft, "PlayAnimation", "Before", "End", false);
                Tween.RunFunction(delay + duration, projLeft, "PlayAnimation", "Retract", "RetractComplete", false);
                Tween.RunFunction(delay + duration + 1, projLeft, "KillProjectile");

                delay += 0.05f;
            }

            spearData.Dispose();
        }

        public void CastSwordsRandom()
        {
            Vector2 roomCentre = new Vector2(m_levelScreen.CurrentRoom.Bounds.Center.X, this.Y);
            this.UpdateCollisionBoxes();
            ProjectileData swordData = new ProjectileData(this)
            {
                SpriteName = "LastBossSwordVerticalProjectile_Sprite",
                Target = null,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                StartingRotation = 0,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                DestroysWithEnemy = false,
                LockPosition = true,
            };

            int projSpeed = MegaUpwardSwordProjectileSpeed;

            int xOffsetRight = 0;
            int xOffsetLeft = 0;
            float delay = 1;
            for (int i = 0; i < MegaUpwardSwordProjectileAmount; i++)
            {
                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(swordData);
                proj.Scale = new Vector2(1.5f, 1.5f);
                proj.X = roomCentre.X + 50 + xOffsetRight;
                proj.Y = roomCentre.Y + (this.Bounds.Bottom - this.Y) + 120;
                proj.Opacity = 0;
                Tween.To(proj, 0.25f, Tween.EaseNone, "Opacity", "1");
                Tween.By(proj, 2.5f, Quad.EaseIn, "delay", delay.ToString(), "Y", (-projSpeed).ToString());
                Tween.AddEndHandlerToLastTween(proj, "KillProjectile");

                xOffsetRight = CDGMath.RandomInt(50, 1000);

                ProjectileObj projLeft = m_levelScreen.ProjectileManager.FireProjectile(swordData);
                projLeft.Scale = new Vector2(2, 2);
                projLeft.X = roomCentre.X - 50 + xOffsetLeft;
                projLeft.Y = roomCentre.Y + (this.Bounds.Bottom - this.Y) + 120;
                projLeft.Opacity = 0;
                Tween.To(projLeft, 0.25f, Tween.EaseNone, "Opacity", "1");
                Tween.By(projLeft, 2.5f, Quad.EaseIn, "delay", delay.ToString(), "Y", (-projSpeed).ToString());
                Tween.AddEndHandlerToLastTween(proj, "KillProjectile");

                xOffsetLeft = -CDGMath.RandomInt(50, 1000);

                delay += 0.25f;
            }

            swordData.Dispose();
        }

        public void ChangeProjectileSpeed(ProjectileObj proj, float speed, Vector2 heading)
        {
            proj.AccelerationX = heading.X * speed;
            proj.AccelerationY = -heading.Y * speed;
        }


        public void CastDamageShield(int numOrbs)
        {
            foreach (ProjectileObj projectile in m_damageShieldProjectiles)
                projectile.KillProjectile();
            m_damageShieldProjectiles.Clear();

            ProjectileData orbData = new ProjectileData(this)
            {
                SpriteName = "LastBossOrbProjectile_Sprite",
                Angle = new Vector2(-65, -65),
                Speed = new Vector2(m_Mega_Shield_Speed, m_Mega_Shield_Speed),//(1.5f, 1.5f),//(2, 2),
                Target = this,
                IsWeighted = false,
                RotationSpeed = 0,
                CollidesWithTerrain = false,
                DestroysWithTerrain = false,
                DestroysWithEnemy = false,
                CanBeFusRohDahed = false,
                ShowIcon = false,
                Lifespan = 9999,
                Damage = Damage / 2,
            };

            SoundManager.Play3DSound(this, m_target, "FinalBoss_St2_SwordSummon_b");
            int projectileDistance = m_Mega_Shield_Distance;
            for (int i = 0; i < numOrbs; i++)
            {
                float angle = (360f / numOrbs) * i;

                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(orbData);
                proj.AltX = angle; // AltX and AltY are used as holders to hold the projectiles angle and distance from player respectively.
                proj.AltY = projectileDistance;
                proj.Spell = SpellType.DamageShield;
                proj.AccelerationXEnabled = false;
                proj.AccelerationYEnabled = false;
                proj.IgnoreBoundsCheck = true;
                proj.Scale = new Vector2(m_Mega_Shield_Scale, m_Mega_Shield_Scale);
                proj.Position = CDGMath.GetCirclePosition(angle, projectileDistance, this.Position);
                m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, false);

                m_damageShieldProjectiles.Add(proj);
            }
        }

        public void Dash(int heading)
        {
            this.HeadingY = 0;
            if (m_target.Position.X < this.X) // to the right of the player.
            {
                if (heading == 0)
                    this.HeadingX = 1;
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    this.ChangeSprite("PlayerFrontDash_Character");
                else
                    this.ChangeSprite("PlayerDash_Character");

                m_levelScreen.ImpactEffectPool.DisplayDashEffect(new Vector2(this.X, TerrainBounds.Bottom), false);
            }
            else // to the left of the player.
            {
                if (heading == 0)
                    this.HeadingX = -1;
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    this.ChangeSprite("PlayerDash_Character");
                else
                    this.ChangeSprite("PlayerFrontDash_Character");

                m_levelScreen.ImpactEffectPool.DisplayDashEffect(new Vector2(this.X, TerrainBounds.Bottom), true);
            }

            if (heading != 0)
                this.HeadingX = heading;

            SoundManager.Play3DSound(this, m_target, "Player_Dash");

            this.LockFlip = true;
            this.AccelerationX = 0;
            this.AccelerationY = 0;
            this.PlayAnimation(false);
            this.CurrentSpeed = 900;
            this.AccelerationYEnabled = false;
            m_isDashing = true;
        }

        public void DashComplete()
        {
            this.LockFlip = false;
            this.CurrentSpeed = 500;
            this.AccelerationYEnabled = true;
            m_isDashing = false;
            this.AnimationDelay = 1 / 10f;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_shake == true)
            {
                if (m_shakeTimer > 0)
                {
                    m_shakeTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_shakeTimer <= 0)
                    {
                        m_shakeTimer = m_shakeDuration;
                        if (m_shookLeft == true)
                        {
                            m_shookLeft = false;
                            this.X += 5;
                        }
                        else
                        {
                            this.X -= 5;
                            m_shookLeft = true;
                        }
                    }
                }
            }

            if (m_smokeCounter > 0 && m_inSecondForm == false)
            {
                m_smokeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_smokeCounter <= 0)
                {
                    m_smokeCounter = 0.25f;
                    if (CurrentSpeed > 0)
                        m_smokeCounter = 0.05f;
                    m_levelScreen.ImpactEffectPool.BlackSmokeEffect(this);
                }
            }

            if (m_inSecondForm == false)
            {
                if (m_isTouchingGround == false && m_currentActiveLB != null && this.SpriteName != "PlayerAttacking3_Character" && m_isDashing == false && this.SpriteName != "PlayerLevelUp_Character")
                {
                    if (this.AccelerationY < 0 && this.SpriteName != "PlayerJumping_Character")
                    {
                        this.ChangeSprite("PlayerJumping_Character");
                        this.PlayAnimation(true);
                    }
                    else if (this.AccelerationY > 0 && this.SpriteName != "PlayerFalling_Character")
                    {
                        this.ChangeSprite("PlayerFalling_Character");
                        this.PlayAnimation(true);
                    }
                }
                else if (m_isTouchingGround == true && m_currentActiveLB != null && this.SpriteName == "PlayerAttacking3_Character" && this.CurrentSpeed != 0)
                {
                    SpriteObj bossLegs = this.GetChildAt(PlayerPart.Legs) as SpriteObj;
                    if (bossLegs.SpriteName != "PlayerWalkingLegs_Sprite")
                    {
                        bossLegs.ChangeSprite("PlayerWalkingLegs_Sprite");
                        bossLegs.PlayAnimation(this.CurrentFrame, this.TotalFrames);
                        bossLegs.Y += 4;
                        bossLegs.OverrideParentAnimationDelay = true;
                        bossLegs.AnimationDelay = 1 / 10f;
                    }
                }
            }
            else
            {
                if (this.SpriteName == "EnemyLastBossRun_Character")
                {
                    m_walkUpSoundFinalBoss.Update();
                    m_walkDownSoundFinalBoss.Update();
                }
            }

            if (m_inSecondForm == false && CurrentHealth <= 0 && m_target.CurrentHealth > 0 && IsNeo == false)
            {
                // This is the first form death animation.
                if (IsTouchingGround == true && m_firstFormDying == false)
                {
                    m_firstFormDying = true;
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                    //m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                    m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                    this.IsWeighted = false;
                    this.IsCollidable = false;
                    this.AnimationDelay = 1 / 10f;
                    this.CurrentSpeed = 0;
                    this.AccelerationX = 0;
                    this.AccelerationY = 0;
                    this.TextureColor = Color.White;
                    this.ChangeSprite("PlayerDeath_Character");
                    SoundManager.PlaySound("Boss_Flash");
                    SoundManager.StopMusic(1);
                    m_target.StopAllSpells();
                    m_target.ForceInvincible = true;

                    if (m_target.X < this.X)
                        this.Flip = SpriteEffects.FlipHorizontally;
                    else
                        this.Flip = SpriteEffects.None;

                    if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
                        m_currentActiveLB.StopLogicBlock();
                }
                
                // This is the logic to move the player next to the boss.
                if (m_target.IsTouchingGround == true && m_inSecondForm == false && this.SpriteName == "PlayerDeath_Character")
                    MovePlayerTo();
            }

            if ((m_firstFormDying == false && m_inSecondForm == false) || (m_firstFormDying == true && m_inSecondForm == true) || (IsNeo == true && CurrentHealth > 0))
                base.Update(gameTime);

            // Code for the neo version
            if (m_inSecondForm == false && CurrentHealth <= 0 && m_target.CurrentHealth > 0 && IsNeo == true && IsTouchingGround == true && m_firstFormDying == false)
            {
                KillPlayerNeo();
                m_firstFormDying = true;
            }
        }

        public void MovePlayerTo()
        {
            m_target.StopAllSpells();
            m_levelScreen.ProjectileManager.DestroyAllProjectiles(true);
            m_inSecondForm = true;
            m_isKilled = true;
            m_levelScreen.RunCinematicBorders(16);
            m_currentActiveLB.StopLogicBlock();

            int xOffset = 250;

            Vector2 targetPos = Vector2.Zero;

            if (m_target.X < this.X && this.X > (m_levelScreen.CurrentRoom.X + 500) || this.X > (m_levelScreen.CurrentRoom.Bounds.Right - 500))//&& this.X < m_levelScreen.CurrentRoom.Bounds.Right - 300)
            {
                targetPos = new Vector2(this.X - xOffset, this.Y); // Move to the left of the boss.
                if (targetPos.X > m_levelScreen.CurrentRoom.Bounds.Right - 500)
                    targetPos.X = m_levelScreen.CurrentRoom.Bounds.Right - 500;
            }
            else
                targetPos = new Vector2(this.X + xOffset, this.Y); // Move to the right of the boss.

            m_target.Flip = SpriteEffects.None;
            if (targetPos.X < m_target.X)
                m_target.Flip = SpriteEffects.FlipHorizontally;

            float duration = CDGMath.DistanceBetweenPts(m_target.Position, targetPos) / (float)(m_target.Speed);

            m_target.UpdateCollisionBoxes(); // Necessary check since the OnEnter() is called before m_target can update its collision boxes.
            m_target.State = 1; // Force the m_target into a walking state. This state will not update until the logic set is complete.
            m_target.IsWeighted = false;
            m_target.AccelerationY = 0;
            m_target.AccelerationX = 0;
            m_target.IsCollidable = false;
            m_target.Y = this.m_levelScreen.CurrentRoom.Bounds.Bottom - 60 * 3 - (m_target.Bounds.Bottom - m_target.Y);
            m_target.CurrentSpeed = 0;
            m_target.LockControls();
            m_target.ChangeSprite("PlayerWalking_Character");
            LogicSet playerMoveLS = new LogicSet(m_target);
            playerMoveLS.AddAction(new DelayLogicAction(duration));
            m_target.RunExternalLogicSet(playerMoveLS);
            m_target.PlayAnimation(true);
            Tween.To(m_target, duration, Tween.EaseNone, "X", targetPos.X.ToString());
            Tween.AddEndHandlerToLastTween(this, "SecondFormDeath");
        }

        // This is where the death animation code needs to go.
        public void SecondFormDeath()
        {
            if (m_target.X < this.X)
                m_target.Flip = SpriteEffects.None;
            else
                m_target.Flip = SpriteEffects.FlipHorizontally;
            this.PlayAnimation(false);
            SoundManager.PlaySound("FinalBoss_St1_DeathGrunt");
            Tween.RunFunction(0.1f, typeof(SoundManager), "PlaySound", "Player_Death_SwordTwirl");
            Tween.RunFunction(0.7f, typeof(SoundManager), "PlaySound", "Player_Death_SwordLand");
            Tween.RunFunction(1.2f, typeof(SoundManager), "PlaySound", "Player_Death_BodyFall");

            float delay = 2;
            Tween.RunFunction(2, this, "PlayBlackSmokeSounds");
            for (int i = 0; i < 30; i++)
            {
                Tween.RunFunction(delay, m_levelScreen.ImpactEffectPool, "BlackSmokeEffect", this.Position, new Vector2(1 + (delay * 1), 1 + (delay * 1)));
                delay += 0.05f;
            }
            Tween.RunFunction(3, this, "HideEnemy");
            Tween.RunFunction(6, this, "SecondFormDialogue");
        }

        public void PlayBlackSmokeSounds()
        {
            SoundManager.PlaySound("Cutsc_Smoke");
        }

        public void HideEnemy()
        {
            this.Visible = false;
        }

        public void SecondFormDialogue()
        {
            RCScreenManager manager = m_levelScreen.ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("FinalBossTalk02");
            manager.DialogueScreen.SetConfirmEndHandler(m_levelScreen.CurrentRoom, "RunFountainCutscene");
            manager.DisplayScreen(ScreenType.Dialogue, true);
        }

        public void SecondFormComplete()
        {
            m_target.ForceInvincible = false;
            this.Level += LevelEV.LAST_BOSS_MODE2_LEVEL_MOD;

            this.Flip = SpriteEffects.FlipHorizontally;
            this.Visible = true;
            this.MaxHealth = EnemyEV.LastBoss_Advanced_MaxHealth;
            this.Damage = EnemyEV.LastBoss_Advanced_Damage;
            this.CurrentHealth = MaxHealth;
            Name = EnemyEV.LastBoss_Advanced_Name;
            if (LevelEV.WEAKEN_BOSSES == true)
                this.CurrentHealth = 1;
            this.MinMoneyDropAmount = EnemyEV.LastBoss_Advanced_MinDropAmount;
            this.MaxMoneyDropAmount = EnemyEV.LastBoss_Advanced_MaxDropAmount;
            this.MoneyDropChance = EnemyEV.LastBoss_Advanced_DropChance;

            this.Speed = EnemyEV.LastBoss_Advanced_Speed;
            this.TurnSpeed = EnemyEV.LastBoss_Advanced_TurnSpeed;
            this.ProjectileSpeed = EnemyEV.LastBoss_Advanced_ProjectileSpeed;
            this.JumpHeight = EnemyEV.LastBoss_Advanced_Jump;
            this.CooldownTime = EnemyEV.LastBoss_Advanced_Cooldown;
            this.AnimationDelay = 1 / EnemyEV.LastBoss_Advanced_AnimationDelay;

            this.AlwaysFaceTarget = EnemyEV.LastBoss_Advanced_AlwaysFaceTarget;
            this.CanFallOffLedges = EnemyEV.LastBoss_Advanced_CanFallOffLedges;
            this.CanBeKnockedBack = EnemyEV.LastBoss_Advanced_CanBeKnockedBack;

            this.ProjectileScale = EnemyEV.LastBoss_Advanced_ProjectileScale;
            this.TintablePart.TextureColor = EnemyEV.LastBoss_Advanced_Tint;

            this.MeleeRadius = EnemyEV.LastBoss_Advanced_MeleeRadius;
            this.EngageRadius = EnemyEV.LastBoss_Advanced_EngageRadius;
            this.ProjectileRadius = EnemyEV.LastBoss_Advanced_ProjectileRadius;

            this.ProjectileDamage = Damage;
            this.KnockBack = EnemyEV.LastBoss_Advanced_KnockBack;
            this.ChangeSprite("EnemyLastBossIdle_Character");
            //TEDDY MEGA BOSS COOLDOWN
            SetCooldownLogicBlock(m_secondFormCooldownLB, 40, 20, 40); //walkTowardsSF, walkAwaySF, walkStopSF
            this.PlayAnimation(true);
            this.Name = "The Fountain";

            this.IsWeighted = true;
            this.IsCollidable = true;

            //this.Y -= 50;
            //Tween.RunFunction(2, this, "SecondFormActive");
        }

        public void SecondFormActive()
        {
            if (this.IsPaused == true)
                this.UnpauseEnemy(true);
            m_levelScreen.CameraLockedToPlayer = true;
            m_target.UnlockControls();
            m_target.IsWeighted = true;
            m_target.IsCollidable = true;
            m_isKilled = false;
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            if (m_inSecondForm == false)
            {
                if (m_isHurt == false && m_isDashing == false)
                {
                    SoundManager.Play3DSound(this, m_target, "FinalBoss_St1_Dmg_01", "FinalBoss_St1_Dmg_02", "FinalBoss_St1_Dmg_03", "FinalBoss_St1_Dmg_04");
                    /*
                    this.LockFlip = false;
                    this.AnimationDelay = 1 / 10f;

                    if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
                        m_currentActiveLB.StopLogicBlock();

                    this.ChangeSprite("PlayerHurt_Character");
                    m_isHurt = true;
                    */    // TEDDY I REMOVED THIS SO THAT ENEMY DOESNT GO INTO HURT STATE.
                    base.HitEnemy(damage, collisionPt, isPlayer);
                }
            }
            else
            {
                SoundManager.Play3DSound(this, m_target, "FinalBoss_St2_Hit_01", "FinalBoss_St2_Hit_03", "FinalBoss_St2_Hit_04");
                SoundManager.Play3DSound(this, m_target, "FinalBoss_St2_DmgVox_01", "FinalBoss_St2_DmgVox_02", "FinalBoss_St2_DmgVox_03", "FinalBoss_St2_DmgVox_04",
                    "FinalBoss_St2_DmgVox_05", "FinalBoss_St2_DmgVox_06", "FinalBoss_St2_DmgVox_07", "FinalBoss_St2_DmgVox_08", "FinalBoss_St2_DmgVox_09");
                base.HitEnemy(damage, collisionPt, isPlayer);
            }
        }

        public override void Kill(bool giveXP = true)
        {
            if (m_target.CurrentHealth > 0)
            {
                if (m_inSecondForm == true && m_bossVersionKilled == false)
                {
                    m_bossVersionKilled = true;
                    SetPlayerData();

                    m_levelScreen.PauseScreen();
                    m_levelScreen.ProjectileManager.DestroyAllProjectiles(false);
                    m_target.StopAllSpells();
                    m_levelScreen.RunWhiteSlashEffect();
                    this.ChangeSprite("EnemyLastBossDeath_Character");
                    if (m_target.X < this.X)
                        this.Flip = SpriteEffects.FlipHorizontally;
                    else
                        this.Flip = SpriteEffects.None;
                    Tween.RunFunction(1f, this, "Part2");
                    SoundManager.PlaySound("Boss_Flash");
                    SoundManager.PlaySound("Boss_Eyeball_Freeze");
                    SoundManager.StopMusic(0);
                    m_target.LockControls();

                    GameUtil.UnlockAchievement("FEAR_OF_FATHERS");

                    if (Game.PlayerStats.TimesCastleBeaten > 1)
                        GameUtil.UnlockAchievement("FEAR_OF_TWINS");
                }

                if (IsNeo == true && m_neoDying == false)
                {
                    m_neoDying = true;
                    m_levelScreen.PauseScreen();
                    SoundManager.PauseMusic();

                    m_levelScreen.RunWhiteSlashEffect();

                    SoundManager.PlaySound("Boss_Flash");
                    SoundManager.PlaySound("Boss_Eyeball_Freeze");
                    Tween.RunFunction(1, m_levelScreen, "UnpauseScreen");
                    Tween.RunFunction(1, typeof(SoundManager), "ResumeMusic");
                }
            }
            //base.Kill(giveXP);
        }

        public void KillPlayerNeo()
        {
            m_isKilled = true;

            if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
                m_currentActiveLB.StopLogicBlock();

            this.IsWeighted = false;
            this.IsCollidable = false;
            this.AnimationDelay = 1 / 10f;
            this.CurrentSpeed = 0;
            this.AccelerationX = 0;
            this.AccelerationY = 0;

            this.ChangeSprite("PlayerDeath_Character");
            this.PlayAnimation(false);
            SoundManager.PlaySound("FinalBoss_St1_DeathGrunt");
            Tween.RunFunction(0.1f, typeof(SoundManager), "PlaySound", "Player_Death_SwordTwirl");
            Tween.RunFunction(0.7f, typeof(SoundManager), "PlaySound", "Player_Death_SwordLand");
            Tween.RunFunction(1.2f, typeof(SoundManager), "PlaySound", "Player_Death_BodyFall");
        }

        // This sets all the player data once the last boss has been beaten.
        public void SetPlayerData()
        {
            // Creating a new family tree node and saving.
            FamilyTreeNode newNode = new FamilyTreeNode()
            {
                Name = Game.PlayerStats.PlayerName,
                Age = Game.PlayerStats.Age,
                ChildAge = Game.PlayerStats.ChildAge,
                Class = Game.PlayerStats.Class,
                HeadPiece = Game.PlayerStats.HeadPiece,
                ChestPiece = Game.PlayerStats.ChestPiece,
                ShoulderPiece = Game.PlayerStats.ShoulderPiece,
                NumEnemiesBeaten = Game.PlayerStats.NumEnemiesBeaten,
                BeatenABoss = true,
                Traits = Game.PlayerStats.Traits,
                IsFemale = Game.PlayerStats.IsFemale,
                RomanNumeral = Game.PlayerStats.RomanNumeral,
            };
            Game.PlayerStats.FamilyTreeArray.Add(newNode);

            // Setting necessary after-death flags and saving.
            //Game.PlayerStats.IsDead = true;
            //Game.PlayerStats.Traits = Vector2.Zero;
            Game.PlayerStats.NewBossBeaten = false;
            Game.PlayerStats.RerolledChildren = false;
            Game.PlayerStats.NumEnemiesBeaten = 0;
            Game.PlayerStats.LichHealth = 0;
            Game.PlayerStats.LichMana = 0;
            Game.PlayerStats.LichHealthMod = 1;
            Game.PlayerStats.LoadStartingRoom = true;

            // The important one. These two flags will trigger new game plus at title screen.
            Game.PlayerStats.LastbossBeaten = true;
            Game.PlayerStats.CharacterFound = false; // This should be the ONLY place this is ever set to false.
            Game.PlayerStats.TimesCastleBeaten++;

            // Thanatophobia trait.
            if (Game.PlayerStats.ArchitectUsed == false && Game.PlayerStats.TimesDead <= 15)
                GameUtil.UnlockAchievement("FEAR_OF_DYING");

            (m_target.AttachedLevel.ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            Vector2 mtd = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);

            PlayerObj player = otherBox.AbsParent as PlayerObj;

            // Hits the player in Tanooki mode.
            if (player != null && otherBox.Type == Consts.TERRAIN_HITBOX && player.IsInvincible == false && player.State != PlayerObj.STATE_HURT)
                player.HitPlayer(this);

            if (m_isTouchingGround == true && m_isHurt == true)
            {
                m_isHurt = false;
                if (m_inSecondForm == false)
                    this.ChangeSprite("PlayerIdle_Character");
            }

            // This has to go before the wall check.
            if (otherBox.AbsParent is EnemyObj_Platform == false)
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);

            // This code gets him out of corners.
            TerrainObj terrain = otherBox.AbsParent as TerrainObj;
            if (terrain != null && m_isTouchingGround == false && terrain is DoorObj == false && m_inSecondForm == false)
            {
                if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
                    m_currentActiveLB.StopLogicBlock();

                if (mtd.X > 0) // Dash right
                    RunLogicBlock(true, m_firstFormDashAwayLB, 0, 100); // dashLeftLS, dashRightLS
                else if (mtd.X < 0) //Dash left
                    RunLogicBlock(true, m_firstFormDashAwayLB, 100, 0); // dashLeftLS, dashRightLS
            }
        }

        public void Part2()
        {
            SoundManager.PlaySound("FinalBoss_St2_WeatherChange_a");
            m_levelScreen.UnpauseScreen();

            if (m_currentActiveLB != null)
                m_currentActiveLB.StopLogicBlock();

            this.PauseEnemy(true);
            m_target.CurrentSpeed = 0;
            m_target.ForceInvincible = true;

            Tween.RunFunction(1, m_levelScreen, "RevealMorning");
            Tween.RunFunction(1, m_levelScreen.CurrentRoom, "ChangeWindowOpacity");
            Tween.RunFunction(5, this, "Part3");
        }

        public void Part3()
        {
            RCScreenManager manager = m_levelScreen.ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("FinalBossTalk03");
            manager.DialogueScreen.SetConfirmEndHandler(this, "Part4");
            manager.DisplayScreen(ScreenType.Dialogue, true);
        }

        public void Part4()
        {
            List<object> dataList = new List<object>();
            dataList.Add(this);
            (m_levelScreen.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GameOverBoss, true, dataList);
        }

        public EnemyObj_LastBoss(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("PlayerIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            foreach (GameObj obj in _objectList)
                obj.TextureColor = new Color(100, 100, 100);

            this.Type = EnemyType.LastBoss;

            m_damageShieldProjectiles = new List<ProjectileObj>();

            _objectList[PlayerPart.Boobs].Visible = false;
            _objectList[PlayerPart.Extra].Visible = false;
            _objectList[PlayerPart.Light].Visible = false;
            _objectList[PlayerPart.Glasses].Visible = false;
            _objectList[PlayerPart.Bowtie].Visible = false;
            _objectList[PlayerPart.Wings].Visible = false;

            string headPart = (_objectList[PlayerPart.Head] as IAnimateableObj).SpriteName;
            int numberIndex = headPart.IndexOf("_") - 1;
            headPart = headPart.Remove(numberIndex, 1);
            headPart = headPart.Replace("_", PlayerPart.IntroHelm + "_");
            _objectList[PlayerPart.Head].ChangeSprite(headPart);
            this.PlayAnimation(true);

            m_delayObj = new BlankObj(0, 0);

            m_walkDownSoundFinalBoss = new FrameSoundObj(this, 3, "FinalBoss_St2_Foot_01", "FinalBoss_St2_Foot_02", "FinalBoss_St2_Foot_03");
            m_walkUpSoundFinalBoss = new FrameSoundObj(this, 6, "FinalBoss_St2_Foot_04", "FinalBoss_St2_Foot_05");
        }

        public override void ChangeSprite(string spriteName)
        {
            base.ChangeSprite(spriteName);

            if (m_inSecondForm == false)
            {
                string headPart = (_objectList[PlayerPart.Head] as IAnimateableObj).SpriteName;
                int numberIndex = headPart.IndexOf("_") - 1;
                headPart = headPart.Remove(numberIndex, 1);
                headPart = headPart.Replace("_", PlayerPart.IntroHelm + "_");
                _objectList[PlayerPart.Head].ChangeSprite(headPart);

                _objectList[PlayerPart.Boobs].Visible = false;
                _objectList[PlayerPart.Extra].Visible = false;
                _objectList[PlayerPart.Light].Visible = false;
                _objectList[PlayerPart.Glasses].Visible = false;
                _objectList[PlayerPart.Bowtie].Visible = false;
                _objectList[PlayerPart.Wings].Visible = false;
            }
        }

        public override void Draw(Camera2D camera)
        {
             // This here is just awful. But I can't put it in the update because when he's killed he stops updating.
            if (IsKilled == true && this.TextureColor != Color.White)
            {
                m_blinkTimer = 0;
                this.TextureColor = Color.White;
            }

            base.Draw(camera);
        }

        public override void Reset()
        {
            m_neoDying = false;
            m_inSecondForm = false;
            m_firstFormDying = false;
            CanBeKnockedBack = true;
            base.Reset();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_damageShieldProjectiles.Clear();
                m_damageShieldProjectiles = null;
                m_delayObj.Dispose();
                m_delayObj = null;

                if (m_daggerProjData != null)
                {
                    m_daggerProjData.Dispose();
                    m_daggerProjData = null;
                }

                if (m_axeProjData != null)
                {
                    m_axeProjData.Dispose();
                    m_axeProjData = null;
                }
                base.Dispose();
            }
        }

        public bool IsSecondForm
        {
            get { return m_inSecondForm; }
        }

        public void ForceSecondForm(bool value)
        {
            m_inSecondForm = value;
        }

        public bool IsNeo
        {
            get { return m_isNeo; }
            set
            {
                m_isNeo = value;
                if (value == true)
                {
                    HealthGainPerLevel = 0;
                    DamageGainPerLevel = 0;
                    ItemDropChance = 0;
                    MoneyDropChance = 0;
                    m_saveToEnemiesKilledList = false;
                    CanFallOffLedges = true; //TEDDY - Making Neo last boss fall off ledges.
                }
            }
        }
    }
}

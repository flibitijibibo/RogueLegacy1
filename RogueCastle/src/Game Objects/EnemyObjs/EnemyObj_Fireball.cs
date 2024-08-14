using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;

namespace RogueCastle
{
    public class EnemyObj_Fireball: EnemyObj
    {
        //private float SeparationDistance = 150f; // The distance that bats stay away from other enemies.

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalNeoLB = new LogicBlock();

        private Color FlameColour = Color.OrangeRed;
        private float DashDelay = 0;
        private float DashSpeed = 0;
        private float DashDuration = 0;

        private float m_minibossFireTimeCounter = 0.6f;//0.5f;//0.35f;
        private float m_minibossFireTime = 0.6f;//0.5f;//0.35f;
        private float m_MinibossProjectileLifspan = 11f;//12f;//12f;

        private float m_MinibossProjectileLifspanNeo = 20f;//19f;//18f;

        private bool m_shake = false;
        private bool m_shookLeft = false;
        private float m_shakeTimer = 0;
        private float m_shakeDuration = 0.03f;

        //5000
        private int m_bossCoins = 30;
        private int m_bossMoneyBags = 17;
        private int m_bossDiamonds = 4;

        private bool m_isNeo = false;

        protected override void InitializeEV()
        {
            DashDelay = 0.75f;
            DashDuration = 0.5f;
            DashSpeed = 900;

            #region Basic Variables - General
            Name = EnemyEV.Fireball_Basic_Name;
            LocStringID = EnemyEV.Fireball_Basic_Name_locID;

            MaxHealth = EnemyEV.Fireball_Basic_MaxHealth;
            Damage = EnemyEV.Fireball_Basic_Damage;
            XPValue = EnemyEV.Fireball_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Fireball_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Fireball_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Fireball_Basic_DropChance;

            Speed = EnemyEV.Fireball_Basic_Speed;
            TurnSpeed = EnemyEV.Fireball_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Fireball_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Fireball_Basic_Jump;
            CooldownTime = EnemyEV.Fireball_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Fireball_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Fireball_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Fireball_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Fireball_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Fireball_Basic_IsWeighted;

            Scale = EnemyEV.Fireball_Basic_Scale;
            ProjectileScale = EnemyEV.Fireball_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Fireball_Basic_Tint;

            MeleeRadius = EnemyEV.Fireball_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Fireball_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Fireball_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Fireball_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Fireball_Miniboss_Name;
                    LocStringID = EnemyEV.Fireball_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Fireball_Miniboss_MaxHealth;
                    Damage = EnemyEV.Fireball_Miniboss_Damage;
                    XPValue = EnemyEV.Fireball_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Fireball_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Fireball_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Fireball_Miniboss_DropChance;

                    Speed = EnemyEV.Fireball_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Fireball_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Fireball_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Fireball_Miniboss_Jump;
                    CooldownTime = EnemyEV.Fireball_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Fireball_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Fireball_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Fireball_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Fireball_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Fireball_Miniboss_IsWeighted;

                    Scale = EnemyEV.Fireball_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Fireball_Miniboss_ProjectileScale;
                    //TintablePart.TextureColor = EnemyEV.Fireball_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Fireball_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Fireball_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Fireball_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Fireball_Miniboss_KnockBack;
                    #endregion
                    if (LevelEV.WEAKEN_BOSSES == true)
                        this.MaxHealth = 1;
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Fireball_Expert_Name;
                    LocStringID = EnemyEV.Fireball_Expert_Name_locID;

                    MaxHealth = EnemyEV.Fireball_Expert_MaxHealth;
                    Damage = EnemyEV.Fireball_Expert_Damage;
                    XPValue = EnemyEV.Fireball_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Fireball_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Fireball_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Fireball_Expert_DropChance;

                    Speed = EnemyEV.Fireball_Expert_Speed;
                    TurnSpeed = EnemyEV.Fireball_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Fireball_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Fireball_Expert_Jump;
                    CooldownTime = EnemyEV.Fireball_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Fireball_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Fireball_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Fireball_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Fireball_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Fireball_Expert_IsWeighted;

                    Scale = EnemyEV.Fireball_Expert_Scale;
                    ProjectileScale = EnemyEV.Fireball_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Fireball_Expert_Tint;

                    MeleeRadius = EnemyEV.Fireball_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Fireball_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Fireball_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Fireball_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Fireball_Advanced_Name;
                    LocStringID = EnemyEV.Fireball_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Fireball_Advanced_MaxHealth;
                    Damage = EnemyEV.Fireball_Advanced_Damage;
                    XPValue = EnemyEV.Fireball_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Fireball_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Fireball_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Fireball_Advanced_DropChance;

                    Speed = EnemyEV.Fireball_Advanced_Speed;
                    TurnSpeed = EnemyEV.Fireball_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Fireball_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Fireball_Advanced_Jump;
                    CooldownTime = EnemyEV.Fireball_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Fireball_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Fireball_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Fireball_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Fireball_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Fireball_Advanced_IsWeighted;

                    Scale = EnemyEV.Fireball_Advanced_Scale;
                    ProjectileScale = EnemyEV.Fireball_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Fireball_Advanced_Tint;

                    MeleeRadius = EnemyEV.Fireball_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Fireball_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Fireball_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Fireball_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                m_resetSpriteName = "EnemyGhostBossIdle_Character";
        }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostChase_Character", true, true));
            walkTowardsLS.AddAction(new ChaseLogicAction(m_target, true, 2.0f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostChase_Character", true, true));
            walkAwayLS.AddAction(new ChaseLogicAction(m_target, false, 1.0f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(1.0f, 2.0f));
            //walkStopLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet autoCoolDownLS = new LogicSet(this);
            autoCoolDownLS.AddAction(new MoveLogicAction(m_target, true));
            autoCoolDownLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet dashTowardsLS = new LogicSet(this);
            dashTowardsLS.AddAction(new MoveLogicAction(m_target, true, 0));
            dashTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostDashPrep_Character", true, true));
            //dashTowardsLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Flameball_Attack_Pause"));
            dashTowardsLS.AddAction(new DelayLogicAction(DashDelay));
            dashTowardsLS.AddAction(new RunFunctionLogicAction(this, "TurnToPlayer", null));
            dashTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostDash_Character", true, true));
            dashTowardsLS.AddAction(new LockFaceDirectionLogicAction(true));
            //dashTowardsLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Flameball_Attack"));
            dashTowardsLS.AddAction(new MoveLogicAction(m_target, true, DashSpeed));
            dashTowardsLS.AddAction(new DelayLogicAction(DashDuration));
            dashTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostIdle_Character", true, true));
            dashTowardsLS.AddAction(new MoveLogicAction(m_target, true, 0));
            dashTowardsLS.AddAction(new LockFaceDirectionLogicAction(false));
            dashTowardsLS.AddAction(new DelayLogicAction(0.75f));
            dashTowardsLS.Tag = GameTypes.LogicSetType_ATTACK;


            LogicSet dashTowardsFourProjectilesLS = new LogicSet(this);
            dashTowardsFourProjectilesLS.AddAction(new MoveLogicAction(m_target, true, 0));
            dashTowardsFourProjectilesLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostDashPrep_Character", true, true));
            dashTowardsFourProjectilesLS.AddAction(new DelayLogicAction(DashDelay));
            dashTowardsFourProjectilesLS.AddAction(new RunFunctionLogicAction(this, "TurnToPlayer", null));
            dashTowardsFourProjectilesLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostDash_Character", true, true));
            ThrowProjectiles(dashTowardsFourProjectilesLS, true);
            dashTowardsFourProjectilesLS.AddAction(new LockFaceDirectionLogicAction(true));
            dashTowardsFourProjectilesLS.AddAction(new MoveLogicAction(m_target, true, DashSpeed));
            dashTowardsFourProjectilesLS.AddAction(new DelayLogicAction(DashDuration));
            dashTowardsFourProjectilesLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostIdle_Character", true, true));
            dashTowardsFourProjectilesLS.AddAction(new MoveLogicAction(m_target, true, 0));
            dashTowardsFourProjectilesLS.AddAction(new LockFaceDirectionLogicAction(false));
            dashTowardsFourProjectilesLS.AddAction(new DelayLogicAction(0.75f));
            dashTowardsFourProjectilesLS.Tag = GameTypes.LogicSetType_ATTACK;


            ////////////// MINIBOSS LOGIC //////////////////

            LogicSet walkTowardsBossLS = new LogicSet(this);
            walkTowardsBossLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostBossIdle_Character", true, true));
            walkTowardsBossLS.AddAction(new ChaseLogicAction(m_target, true, 2.0f));

            LogicSet walkStopBossLS = new LogicSet(this);
            walkStopBossLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostBossIdle_Character", true, true));
            walkStopBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopBossLS.AddAction(new DelayLogicAction(1.0f, 2.0f));
            //walkStopLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet dashTowardsBossLS = new LogicSet(this);
            dashTowardsBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            dashTowardsBossLS.AddAction(new LockFaceDirectionLogicAction(true));
            dashTowardsBossLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostBossDashPrep_Character", true, true));
            dashTowardsBossLS.AddAction(new DelayLogicAction(DashDelay));
            dashTowardsBossLS.AddAction(new RunFunctionLogicAction(this, "TurnToPlayer", null));
            dashTowardsBossLS.AddAction(new ChangeSpriteLogicAction("EnemyGhostBossIdle_Character", true, true));
            dashTowardsBossLS.AddAction(new RunFunctionLogicAction(this, "ChangeFlameDirection"));
            dashTowardsBossLS.AddAction(new MoveLogicAction(m_target, true, DashSpeed));
            dashTowardsBossLS.AddAction(new DelayLogicAction(DashDuration));
            dashTowardsBossLS.AddAction(new ChangePropertyLogicAction(_objectList[0], "Rotation", 0));
            dashTowardsBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            dashTowardsBossLS.AddAction(new LockFaceDirectionLogicAction(false));
            dashTowardsBossLS.AddAction(new DelayLogicAction(0.75f));
            dashTowardsLS.Tag = GameTypes.LogicSetType_ATTACK;



            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsFourProjectilesLS);
            m_generalMiniBossLB.AddLogicSet(walkTowardsBossLS, walkAwayLS, walkStopBossLS, autoCoolDownLS, dashTowardsBossLS);

            m_generalNeoLB.AddLogicSet(walkTowardsBossLS, walkAwayLS, walkStopBossLS, autoCoolDownLS, dashTowardsBossLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalNeoLB);

            SetCooldownLogicBlock(m_generalBasicLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS


            base.InitializeLogic();
        }

        public void ChangeFlameDirection()
        {
            if (m_target.X < this.X)
                _objectList[0].Rotation = 90;
            else
                _objectList[0].Rotation = -90;
        }


        private void ThrowProjectiles(LogicSet ls, bool useBossProjectile = false)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "GhostProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
            };

            if (useBossProjectile == true)
                projData.SpriteName = "GhostProjectile_Sprite";

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            projData.Angle = new Vector2(60, 60);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(30, 30);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(120, 120);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(150, 150);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-60, -60);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-30, -30);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-120, -120);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-150, -150);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            projData.Dispose();
        }


        private void ThrowStandingProjectile(bool useBossProjectile = false)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "GhostProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
                Lifespan = m_MinibossProjectileLifspan,
            };

            if (IsNeo == true)
                projData.Lifespan = m_MinibossProjectileLifspanNeo;

            if (useBossProjectile == true)
                projData.SpriteName = "GhostBossProjectile_Sprite";
            ProjectileObj ball = m_levelScreen.ProjectileManager.FireProjectile(projData);
            ball.Rotation = 0; // Why is this needed? Because lockposition does some funky stuff.
            if (IsNeo == true)
                ball.TextureColor = Color.MediumSpringGreen;
            projData.Dispose();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
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
                    RunLogicBlock(true, m_generalAdvancedLB, 40, 0, 0, 0, 60); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
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
                    RunLogicBlock(true, m_generalExpertLB, 40, 0, 0, 0, 60); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsFourProjectilesLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsFourProjectilesLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsFourProjectilesLS
                    break;
                default:
                    break;
            }
        }

        protected override void RunMinibossLogic()
        {
            if (IsNeo == false)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                        RunLogicBlock(true, m_generalMiniBossLB, 52, 0, 0, 0, 48); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                        break;
                    case (STATE_PROJECTILE_ENGAGE):
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalMiniBossLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalMiniBossLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                        RunLogicBlock(true, m_generalNeoLB, 45, 0, 0, 0, 55); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                        break;
                    case (STATE_PROJECTILE_ENGAGE):
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalNeoLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalNeoLB, 100, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, autoCoolDownLS, dashTowardsLS
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS && this.IsPaused == false)
            {
                if (m_minibossFireTimeCounter > 0 && m_bossVersionKilled == false)
                {
                    m_minibossFireTimeCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_minibossFireTimeCounter <= 0)
                    {
                        ThrowStandingProjectile(true);
                        m_minibossFireTimeCounter = m_minibossFireTime;
                    }
                }
            }

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

            // Flocking code for flying bats.
            //foreach (EnemyObj enemyToAvoid in m_levelScreen.EnemyList) // Can speed up efficiency if the level had a property that returned the number of enemies in a single room.
            //{
            //    if (enemyToAvoid != this)
            //    {
            //        float distanceFromOtherEnemy = Vector2.Distance(this.Position, enemyToAvoid.Position);
            //        if (distanceFromOtherEnemy < this.SeparationDistance)
            //        {
            //            this.CurrentSpeed = .50f * this.Speed; // Move away at half speed.
            //            Vector2 seekPosition = 2 * this.Position - enemyToAvoid.Position;
            //            CDGMath.TurnToFace(this, seekPosition);
            //        }
            //    }
            //}
            //if (m_playingAnim == false)
            //{
            //    m_playingAnim = true;
            //    Tweener.Tween.To(this, CDGMath.RandomFloat(0,3), Tweener.Ease.Linear.EaseNone, "Opacity", "1");
            //    Tweener.Tween.AddEndHandlerToLastTween(this, "PlayAnimation", true);
            //}

            //this.Heading = new Vector2(
            //    (float)Math.Cos(Orientation), (float)Math.Sin(Orientation));
            base.Update(gameTime);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            // Hits the player in Tanooki mode.
            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS && m_bossVersionKilled == false)
            {
                PlayerObj player = otherBox.AbsParent as PlayerObj;
                if (player != null && otherBox.Type == Consts.WEAPON_HITBOX && player.IsInvincible == false && player.State == PlayerObj.STATE_TANOOKI)
                    player.HitPlayer(this);
            }

            if (collisionResponseType != Consts.COLLISIONRESPONSE_TERRAIN)
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public void TurnToPlayer()
        {
            float previousTurnspeed = TurnSpeed;
            this.TurnSpeed = 2;
            CDGMath.TurnToFace(this, m_target.Position);
            this.TurnSpeed = previousTurnspeed;
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            if (m_bossVersionKilled == false)
            {
                //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Flameball_Hit_03", "Flameball_Hit_02");
                base.HitEnemy(damage, collisionPt, isPlayer);
            }
        }

        public override void Kill(bool giveXP = true)
        {
            if (Difficulty != GameTypes.EnemyDifficulty.MINIBOSS)
            {
                base.Kill(giveXP);
            }
            else
            {
                if (m_target.CurrentHealth > 0)
                {
                    Game.PlayerStats.FireballBossBeaten = true;

                    SoundManager.StopMusic(0);
                    SoundManager.PlaySound("PressStart");
                    m_bossVersionKilled = true;
                    m_target.LockControls();
                    m_levelScreen.PauseScreen();
                    m_levelScreen.ProjectileManager.DestroyAllProjectiles(false);
                    m_levelScreen.RunWhiteSlashEffect();
                    Tween.RunFunction(1f, this, "Part2");
                    SoundManager.PlaySound("Boss_Flash");
                    SoundManager.PlaySound("Boss_Fireball_Freeze");

                    GameUtil.UnlockAchievement("FEAR_OF_FIRE");
                }
            }
        }

        public void Part2()
        {
            m_levelScreen.UnpauseScreen();
            m_target.UnlockControls();

            if (m_currentActiveLB != null)
                m_currentActiveLB.StopLogicBlock();

            this.PauseEnemy(true);
            this.ChangeSprite("EnemyGhostBossIdle_Character");
            this.PlayAnimation(true);
            m_target.CurrentSpeed = 0;
            m_target.ForceInvincible = true;

            Tween.To(m_levelScreen.Camera, 0.5f, Tweener.Ease.Quad.EaseInOut, "X", this.X.ToString(), "Y", this.Y.ToString());
            m_shake = true;
            m_shakeTimer = m_shakeDuration;

            for (int i = 0; i < 40; i++)
            {
                Vector2 explosionPos = new Vector2(CDGMath.RandomInt(this.Bounds.Left, this.Bounds.Right), CDGMath.RandomInt(this.Bounds.Top, this.Bounds.Bottom));
                Tween.RunFunction(i * 0.1f, typeof(SoundManager), "Play3DSound", this, m_target, new string[] { "Boss_Explo_01", "Boss_Explo_02", "Boss_Explo_03" });
                Tween.RunFunction(i * 0.1f, m_levelScreen.ImpactEffectPool, "DisplayExplosionEffect", explosionPos);
            }

            Tween.AddEndHandlerToLastTween(this, "Part3");

            if (IsNeo == false)
            {
                int totalGold = m_bossCoins + m_bossMoneyBags + m_bossDiamonds;
                List<int> goldArray = new List<int>();

                for (int i = 0; i < m_bossCoins; i++)
                    goldArray.Add(0);
                for (int i = 0; i < m_bossMoneyBags; i++)
                    goldArray.Add(1);
                for (int i = 0; i < m_bossDiamonds; i++)
                    goldArray.Add(2);

                CDGMath.Shuffle<int>(goldArray);
                float coinDelay = 2.5f / goldArray.Count; // The enemy dies for 2.5 seconds.

                for (int i = 0; i < goldArray.Count; i++)
                {
                    Vector2 goldPos = this.Position;
                    if (goldArray[i] == 0)
                        Tween.RunFunction(i * coinDelay, m_levelScreen.ItemDropManager, "DropItem", goldPos, ItemDropType.Coin, ItemDropType.CoinAmount);
                    else if (goldArray[i] == 1)
                        Tween.RunFunction(i * coinDelay, m_levelScreen.ItemDropManager, "DropItem", goldPos, ItemDropType.MoneyBag, ItemDropType.MoneyBagAmount);
                    else
                        Tween.RunFunction(i * coinDelay, m_levelScreen.ItemDropManager, "DropItem", goldPos, ItemDropType.Diamond, ItemDropType.DiamondAmount);
                }
            }
        }

        public void Part3()
        {
            SoundManager.PlaySound("Boss_Fireball_Death");
            m_levelScreen.ImpactEffectPool.DestroyFireballBoss(this.Position);
            base.Kill();
        }

        public bool BossVersionKilled
        {
            get { return m_bossVersionKilled; }
        }

        public EnemyObj_Fireball(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyGhostIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            //this.PlayAnimation();
            this.Type = EnemyType.Fireball;
            TintablePart = _objectList[0];
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
                    MoneyDropChance = 0;
                    ItemDropChance = 0;
                    m_saveToEnemiesKilledList = false;
                }
            }
        }
    }
}

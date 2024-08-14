using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class EnemyObj_Fairy : EnemyObj
    {
        //private float SeparationDistance = 650f; // The distance that bats stay away from other enemies.

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();
        private LogicBlock m_generalNeoLB = new LogicBlock();

        private bool m_isNeo = false;

        private float AttackDelay = 0.5f;

        public bool MainFairy { get; set; }
        public RoomObj SpawnRoom;
        public Vector2 SavedStartingPos { get; set; }
        public int NumHits = 1;

        private bool m_shake = false;
        private bool m_shookLeft = false;
        private float m_shakeTimer = 0;
        private float m_shakeDuration = 0.03f;

        //3500
        private int m_bossCoins = 30;
        private int m_bossMoneyBags = 12;
        private int m_bossDiamonds = 2;

        private Cue m_deathLoop;
        private bool m_playDeathLoop;

        private int m_numSummons = 22;//25;//30;
        private float m_summonTimer = 6;
        private float m_summonCounter = 6;
        private float m_summonTimerNeo = 3;//3;//10000;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Fairy_Basic_Name;
            LocStringID = EnemyEV.Fairy_Basic_Name_locID;

            MaxHealth = EnemyEV.Fairy_Basic_MaxHealth;
            Damage = EnemyEV.Fairy_Basic_Damage;
            XPValue = EnemyEV.Fairy_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Fairy_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Fairy_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Fairy_Basic_DropChance;

            Speed = EnemyEV.Fairy_Basic_Speed;
            TurnSpeed = EnemyEV.Fairy_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Fairy_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Fairy_Basic_Jump;
            CooldownTime = EnemyEV.Fairy_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Fairy_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Fairy_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Fairy_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Fairy_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Fairy_Basic_IsWeighted;

            Scale = EnemyEV.Fairy_Basic_Scale;
            ProjectileScale = EnemyEV.Fairy_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Fairy_Basic_Tint;

            MeleeRadius = EnemyEV.Fairy_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Fairy_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Fairy_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Fairy_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Fairy_Miniboss_Name;
                    LocStringID = EnemyEV.Fairy_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Fairy_Miniboss_MaxHealth;
                    Damage = EnemyEV.Fairy_Miniboss_Damage;
                    XPValue = EnemyEV.Fairy_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Fairy_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Fairy_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Fairy_Miniboss_DropChance;

                    Speed = EnemyEV.Fairy_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Fairy_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Fairy_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Fairy_Miniboss_Jump;
                    CooldownTime = EnemyEV.Fairy_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Fairy_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Fairy_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Fairy_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Fairy_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Fairy_Miniboss_IsWeighted;

                    Scale = new Vector2(2.5f, 2.5f);//EnemyEV.Fairy_Miniboss_Scale;
                    ProjectileScale = new Vector2(2f,2f);//EnemyEV.Fairy_Miniboss_ProjectileScale;
                    //TintablePart.TextureColor = EnemyEV.Fairy_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Fairy_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Fairy_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Fairy_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Fairy_Miniboss_KnockBack;

                    NumHits = 1;
                    #endregion
                    if (LevelEV.WEAKEN_BOSSES == true)
                        this.MaxHealth = 1;
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Fairy_Expert_Name;
                    LocStringID = EnemyEV.Fairy_Expert_Name_locID;

                    MaxHealth = EnemyEV.Fairy_Expert_MaxHealth;
                    Damage = EnemyEV.Fairy_Expert_Damage;
                    XPValue = EnemyEV.Fairy_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Fairy_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Fairy_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Fairy_Expert_DropChance;

                    Speed = EnemyEV.Fairy_Expert_Speed;
                    TurnSpeed = EnemyEV.Fairy_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Fairy_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Fairy_Expert_Jump;
                    CooldownTime = EnemyEV.Fairy_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Fairy_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Fairy_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Fairy_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Fairy_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Fairy_Expert_IsWeighted;

                    Scale = EnemyEV.Fairy_Expert_Scale;
                    ProjectileScale = EnemyEV.Fairy_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Fairy_Expert_Tint;

                    MeleeRadius = EnemyEV.Fairy_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Fairy_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Fairy_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Fairy_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Fairy_Advanced_Name;
                    LocStringID = EnemyEV.Fairy_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Fairy_Advanced_MaxHealth;
                    Damage = EnemyEV.Fairy_Advanced_Damage;
                    XPValue = EnemyEV.Fairy_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Fairy_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Fairy_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Fairy_Advanced_DropChance;

                    Speed = EnemyEV.Fairy_Advanced_Speed;
                    TurnSpeed = EnemyEV.Fairy_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Fairy_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Fairy_Advanced_Jump;
                    CooldownTime = EnemyEV.Fairy_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Fairy_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Fairy_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Fairy_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Fairy_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Fairy_Advanced_IsWeighted;

                    Scale = EnemyEV.Fairy_Advanced_Scale;
                    ProjectileScale = EnemyEV.Fairy_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Fairy_Advanced_Tint;

                    MeleeRadius = EnemyEV.Fairy_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Fairy_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Fairy_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Fairy_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                m_resetSpriteName = "EnemyFairyGhostBossIdle_Character";
        }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostMove_Character", true, true));
            walkTowardsLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyMove1", "FairyMove2", "FairyMove3"));
            walkTowardsLS.AddAction(new ChaseLogicAction(m_target, true, 1.0f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            walkAwayLS.AddAction(new ChaseLogicAction(m_target, false, 0.5f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f, 0.75f));

            LogicSet fireProjectilesLS = new LogicSet(this);
            fireProjectilesLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            fireProjectilesLS.AddAction(new MoveLogicAction(m_target, true, 0));
            fireProjectilesLS.AddAction(new LockFaceDirectionLogicAction(true));
            fireProjectilesLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostShoot_Character", false, false));
            fireProjectilesLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            fireProjectilesLS.AddAction(new DelayLogicAction(AttackDelay));
            fireProjectilesLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", false));
            fireProjectilesLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            fireProjectilesLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            fireProjectilesLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesLS.AddAction(new LockFaceDirectionLogicAction(false));
            fireProjectilesLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet fireProjectilesAdvancedLS = new LogicSet(this);
            fireProjectilesAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            fireProjectilesAdvancedLS.AddAction(new MoveLogicAction(m_target, true, 0));
            fireProjectilesAdvancedLS.AddAction(new LockFaceDirectionLogicAction(true));
            fireProjectilesAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostShoot_Character", false, false));
            fireProjectilesAdvancedLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            fireProjectilesAdvancedLS.AddAction(new DelayLogicAction(AttackDelay));
            fireProjectilesAdvancedLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", false));            
            fireProjectilesAdvancedLS.AddAction(new DelayLogicAction(0.15f));
            fireProjectilesAdvancedLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", false));
            fireProjectilesAdvancedLS.AddAction(new DelayLogicAction(0.15f));
            fireProjectilesAdvancedLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", false));
            fireProjectilesAdvancedLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            fireProjectilesAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            fireProjectilesAdvancedLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesAdvancedLS.AddAction(new LockFaceDirectionLogicAction(false));
            fireProjectilesAdvancedLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet fireProjectilesExpertLS = new LogicSet(this);
            fireProjectilesExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            fireProjectilesExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            fireProjectilesExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            fireProjectilesExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostShoot_Character", false, false));
            fireProjectilesExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            fireProjectilesExpertLS.AddAction(new DelayLogicAction(AttackDelay));
            ThrowEightProjectiles(fireProjectilesExpertLS);
            fireProjectilesExpertLS.AddAction(new DelayLogicAction(0.25f));
            ThrowEightProjectiles(fireProjectilesExpertLS);
            fireProjectilesExpertLS.AddAction(new DelayLogicAction(0.25f));
            ThrowEightProjectiles(fireProjectilesExpertLS);
            fireProjectilesExpertLS.AddAction(new DelayLogicAction(0.25f));
            ThrowEightProjectiles(fireProjectilesExpertLS);
            fireProjectilesExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));

            fireProjectilesExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostIdle_Character", true, true));
            fireProjectilesExpertLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            fireProjectilesExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            // MINIBOSS CODE

            LogicSet walkTowardsMinibossLS = new LogicSet(this);
            walkTowardsMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossMove_Character", true, true));
            walkTowardsMinibossLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyMove1", "FairyMove2", "FairyMove3"));
            walkTowardsMinibossLS.AddAction(new ChaseLogicAction(m_target, true, 1.0f));

            LogicSet walkAwayMinibossLS = new LogicSet(this);
            walkAwayMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossIdle_Character", true, true));
            walkAwayMinibossLS.AddAction(new ChaseLogicAction(m_target, false, 0.5f));

            LogicSet walkStopMinibossLS = new LogicSet(this);
            walkStopMinibossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopMinibossLS.AddAction(new DelayLogicAction(0.5f, 0.75f));

            LogicSet fireProjectilesMinibossLS = new LogicSet(this);
            fireProjectilesMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossIdle_Character", true, true));
            fireProjectilesMinibossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            fireProjectilesMinibossLS.AddAction(new LockFaceDirectionLogicAction(true));
            fireProjectilesMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossShoot_Character", false, false));
            fireProjectilesMinibossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            fireProjectilesMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossIdle_Character", true, true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(AttackDelay));
            fireProjectilesMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossShoot_Character", false, false));
            fireProjectilesMinibossLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            fireProjectilesMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossMove_Character", true, true));
            fireProjectilesMinibossLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesMinibossLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesMinibossLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesMinibossLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesMinibossLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesMinibossLS.AddAction(new RunFunctionLogicAction(this, "ThrowFourProjectiles", true));
            fireProjectilesMinibossLS.AddAction(new ChangeSpriteLogicAction("EnemyFairyGhostBossIdle_Character", true, true));
            fireProjectilesMinibossLS.AddAction(new DelayLogicAction(0.25f));
            fireProjectilesMinibossLS.AddAction(new LockFaceDirectionLogicAction(false));
            fireProjectilesMinibossLS.Tag = GameTypes.LogicSetType_ATTACK;

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesExpertLS);
            m_generalMiniBossLB.AddLogicSet(walkTowardsMinibossLS, walkAwayMinibossLS, walkStopMinibossLS, fireProjectilesMinibossLS);

            m_generalNeoLB.AddLogicSet(walkTowardsMinibossLS, walkAwayMinibossLS, walkStopMinibossLS, fireProjectilesMinibossLS);

            if (Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                m_generalCooldownLB.AddLogicSet(walkTowardsMinibossLS, walkAwayMinibossLS, walkStopMinibossLS);
            else
                m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);
            logicBlocksToDispose.Add(m_generalNeoLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 70, 30, 0); //walkTowardsLS, walkAwayLS, walkStopLS

            base.InitializeLogic();
        }

        public void ThrowFourProjectiles(bool useBossProjectile)
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
                projData.SpriteName = "GhostProjectileBoss_Sprite";

            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Boss_Flameskull_Roar_01", "Boss_Flameskull_Roar_02", "Boss_Flameskull_Roar_03");
            else
                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "FairyAttack1");

            m_levelScreen.ProjectileManager.FireProjectile(projData);
            projData.Angle = new Vector2(90, 90);
            m_levelScreen.ProjectileManager.FireProjectile(projData);
            projData.Angle = new Vector2(180, 180);
            m_levelScreen.ProjectileManager.FireProjectile(projData);
            projData.Angle = new Vector2(-90, -90);
            m_levelScreen.ProjectileManager.FireProjectile(projData);

            projData.Dispose();
        }

        //private void ThrowFourProjectiles(LogicSet ls, bool useBossProjectile = false)
        //{
        //    ProjectileData projData = new ProjectileData(this)
        //    {
        //        SpriteName = "GhostProjectile_Sprite",
        //        SourceAnchor = Vector2.Zero,
        //        Target = null,
        //        Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
        //        IsWeighted = false,
        //        RotationSpeed = 0,
        //        Damage = Damage,
        //        AngleOffset = 0,
        //        Angle = new Vector2(0, 0),
        //        CollidesWithTerrain = false,
        //        Scale = ProjectileScale,
           
        //    };

        //    if (useBossProjectile == true)
        //        projData.SpriteName = "GhostProjectileBoss_Sprite";

        //    if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
        //        ls.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "Boss_Flameskull_Roar_01", "Boss_Flameskull_Roar_02", "Boss_Flameskull_Roar_03"));
        //    else
        //        ls.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "FairyAttack1"));

        //    ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    projData.Angle = new Vector2(90, 90);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    projData.Angle = new Vector2(180, 180);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    projData.Angle = new Vector2(-90, -90);
        //    ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
        //    //ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
        //    projData.Dispose();
        //}


        private void ThrowEightProjectiles(LogicSet ls)
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

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(90, 90);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(180, 180);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));

            ls.AddAction(new DelayLogicAction(0.125f));
            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            projData.Angle = new Vector2(135, 135);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(45, 45);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-45, -45);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-135, -135);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Dispose();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 50, 10, 0, 40); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
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
                    RunLogicBlock(true, m_generalAdvancedLB, 50, 10, 0, 40); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 100, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 100, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS
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
                    RunLogicBlock(true, m_generalExpertLB, 50, 10, 0, 40); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 100, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 0, 0, 100, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesAdvancedLS
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
                    case (STATE_PROJECTILE_ENGAGE):
                        RunLogicBlock(true, m_generalMiniBossLB, 50, 10, 0, 40); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalMiniBossLB, 100, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalMiniBossLB, 80, 20, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
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
                    case (STATE_PROJECTILE_ENGAGE):
                        RunLogicBlock(true, m_generalNeoLB, 50, 10, 0, 40); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalNeoLB, 100, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalNeoLB, 80, 20, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectilesLS
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Difficulty == GameTypes.EnemyDifficulty.MINIBOSS && IsPaused == false)
            {
                if (m_summonCounter > 0)
                {
                    m_summonCounter -= elapsedTime;
                    if (m_summonCounter <= 0)
                    {
                        if (IsNeo == true)
                            m_summonTimer = m_summonTimerNeo;
                        m_summonCounter = m_summonTimer;
                        NumHits--;
                        if (IsKilled == false && NumHits <= 0 && m_levelScreen.CurrentRoom.ActiveEnemies <= m_numSummons + 1)
                        {
                            if (Game.PlayerStats.TimesCastleBeaten <= 0 || this.IsNeo == true)
                            {
                                CreateFairy(GameTypes.EnemyDifficulty.BASIC);
                                CreateFairy(GameTypes.EnemyDifficulty.BASIC);
                            }
                            else
                            {
                                CreateFairy(GameTypes.EnemyDifficulty.ADVANCED);
                                CreateFairy(GameTypes.EnemyDifficulty.ADVANCED);
                            }
                            NumHits = 1;
                        }
                    }
                }

                // Make sure Alexander doesn't move too much around the borders of the room.
                RoomObj currentRoom = m_levelScreen.CurrentRoom;
                Rectangle enemyBounds = this.Bounds;
                Rectangle roomBounds = currentRoom.Bounds;

                int rightBounds = enemyBounds.Right - roomBounds.Right;
                int leftBounds = enemyBounds.Left - roomBounds.Left;
                //int topBounds = enemyBounds.Top - roomBounds.Top - 20;
                int bottomBounds = enemyBounds.Bottom - roomBounds.Bottom;

                if (rightBounds > 0)
                    this.X -= rightBounds;
                if (leftBounds < 0)
                    this.X -= leftBounds;
                //if (topBounds < 0)
                //    this.Y -= topBounds;
                if (bottomBounds > 0)
                    this.Y -= bottomBounds;
            }

            //// Flocking code for flying bats.
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

            //this.Heading = new Vector2(
            //    (float)Math.Cos(Orientation), (float)Math.Sin(Orientation));

            if (m_shake == true)
            {
                if (m_shakeTimer > 0)
                {
                    m_shakeTimer -= elapsedTime;
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

            if (m_playDeathLoop == true)
            {
                if (m_deathLoop != null && m_deathLoop.IsPlaying == false)
                    m_deathLoop = SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Boss_Flameskull_Death_Loop");
            }

            base.Update(gameTime);
        }

        public EnemyObj_Fairy(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyFairyGhostIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.PlayAnimation(true);
            MainFairy = true;
            TintablePart = _objectList[0];
            this.Type = EnemyType.Fairy;
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


            if (collisionResponseType != Consts.COLLISIONRESPONSE_TERRAIN) // Disable terrain collision
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        private void CreateFairy(GameTypes.EnemyDifficulty difficulty)
        {
            EnemyObj_Fairy fairy = new EnemyObj_Fairy(null, null, null, difficulty);
            fairy.Position = this.Position;
            fairy.DropsItem = false;
            if (m_target.X < fairy.X)
                fairy.Orientation = MathHelper.ToRadians(0);
            else
                fairy.Orientation = MathHelper.ToRadians(180);

            fairy.Level = this.Level - LevelEV.ENEMY_MINIBOSS_LEVEL_MOD - 1; // Must be called before AddEnemyToCurrentRoom() to initialize levels properly.
            m_levelScreen.AddEnemyToCurrentRoom(fairy);
            fairy.PlayAnimation(true);

            fairy.MainFairy = false;
            fairy.SavedStartingPos = fairy.Position;
            fairy.SaveToFile = false;
            if (LevelEV.SHOW_ENEMY_RADII == true)
                fairy.InitializeDebugRadii();
            fairy.SpawnRoom = m_levelScreen.CurrentRoom;
            fairy.GivesLichHealth = false;

            //if (this.IsPaused)
            //    fairy.PauseEnemy();
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            if (m_bossVersionKilled == false)
            {
                base.HitEnemy(damage, position, isPlayer);

                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "SkeletonAttack1");
                //if (Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                //{
                //    NumHits--;
                //    if (IsKilled == false && NumHits <= 0 && m_levelScreen.CurrentRoom.ActiveEnemies <= m_numSummons + 1)
                //    {
                //        CreateFairy(GameTypes.EnemyDifficulty.BASIC);
                //        CreateFairy(GameTypes.EnemyDifficulty.BASIC);
                //        NumHits = 1;
                //    }
                //}
                //else if (Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                //{
                //    //m_levelScreen.ProjectileManager.FireProjectile("GhostProjectile_Sprite", this, Vector2.Zero, 0, ProjectileSpeed, false, 0, ProjectileDamage);
                //    //m_levelScreen.ProjectileManager.FireProjectile("GhostProjectile_Sprite", this, Vector2.Zero, 90, ProjectileSpeed, false, 0, ProjectileDamage);
                //    //m_levelScreen.ProjectileManager.FireProjectile("GhostProjectile_Sprite", this, Vector2.Zero, 180, ProjectileSpeed, false, 0, ProjectileDamage);
                //    //m_levelScreen.ProjectileManager.FireProjectile("GhostProjectile_Sprite", this, Vector2.Zero, -90, ProjectileSpeed, false, 0, ProjectileDamage);

                //}
            }
        }

        public override void Kill(bool giveXP = true)
        {
            if (Difficulty != GameTypes.EnemyDifficulty.MINIBOSS)
            {
                //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"EyeballDeath1", "EyeballDeath2");
                base.Kill(giveXP);
            }
            else
            {
                if (m_target.CurrentHealth > 0)
                {
                    Game.PlayerStats.FairyBossBeaten = true;

                    SoundManager.StopMusic(0);
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "PressStart");
                    m_bossVersionKilled = true;
                    m_target.LockControls();
                    m_levelScreen.PauseScreen();
                    m_levelScreen.ProjectileManager.DestroyAllProjectiles(false);
                    m_levelScreen.RunWhiteSlashEffect();
                    Tween.RunFunction(1f, this, "Part2");
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Boss_Flash");
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Boss_Flameskull_Freeze");

                    GameUtil.UnlockAchievement("FEAR_OF_GHOSTS");
                }
            }
        }

        public void Part2()
        {
            m_playDeathLoop = true;

            foreach (EnemyObj enemy in m_levelScreen.CurrentRoom.TempEnemyList)
            {
                if (enemy.IsKilled == false)
                    enemy.Kill(true);
            }

            m_levelScreen.UnpauseScreen();
            m_target.UnlockControls();

            if (m_currentActiveLB != null)
                m_currentActiveLB.StopLogicBlock();

            this.PauseEnemy(true);
            this.ChangeSprite("EnemyFairyGhostBossShoot_Character");
            this.PlayAnimation(true);
            m_target.CurrentSpeed = 0;
            m_target.ForceInvincible = true;

            if (IsNeo == true)
                m_target.InvincibleToSpikes = true;

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
            m_playDeathLoop = false;
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Boss_Flameskull_Death");
            if (m_deathLoop != null && m_deathLoop.IsPlaying == true)
                m_deathLoop.Stop(AudioStopOptions.Immediate);

            m_levelScreen.RunWhiteSlash2();
            base.Kill();
        }


        public override void Reset()
        {
            if (MainFairy == false)
            {
                m_levelScreen.RemoveEnemyFromRoom(this, SpawnRoom, this.SavedStartingPos);
                this.Dispose();
            }
            else
            {
                NumHits = 1;
                base.Reset();
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                SpawnRoom = null;
                if (m_deathLoop != null && m_deathLoop.IsDisposed == false)
                    m_deathLoop.Dispose();
                m_deathLoop = null;
                base.Dispose();
            }
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

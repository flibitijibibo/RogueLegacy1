using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Skeleton : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private float AttackDelay = 0.1f;
        private float JumpDelay = 0.25f;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Skeleton_Basic_Name;
            LocStringID = EnemyEV.Skeleton_Basic_Name_locID;

            MaxHealth = EnemyEV.Skeleton_Basic_MaxHealth;
            Damage = EnemyEV.Skeleton_Basic_Damage;
            XPValue = EnemyEV.Skeleton_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Skeleton_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Skeleton_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Skeleton_Basic_DropChance;

            Speed = EnemyEV.Skeleton_Basic_Speed;
            TurnSpeed = EnemyEV.Skeleton_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Skeleton_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Skeleton_Basic_Jump;
            CooldownTime = EnemyEV.Skeleton_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Skeleton_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Skeleton_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Skeleton_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Skeleton_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Skeleton_Basic_IsWeighted;

            Scale = EnemyEV.Skeleton_Basic_Scale;
            ProjectileScale = EnemyEV.Skeleton_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Skeleton_Basic_Tint;

            MeleeRadius = EnemyEV.Skeleton_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Skeleton_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Skeleton_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Skeleton_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Skeleton_Miniboss_Name;
                    LocStringID = EnemyEV.Skeleton_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Skeleton_Miniboss_MaxHealth;
                    Damage = EnemyEV.Skeleton_Miniboss_Damage;
                    XPValue = EnemyEV.Skeleton_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Skeleton_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Skeleton_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Skeleton_Miniboss_DropChance;

                    Speed = EnemyEV.Skeleton_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Skeleton_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Skeleton_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Skeleton_Miniboss_Jump;
                    CooldownTime = EnemyEV.Skeleton_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Skeleton_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Skeleton_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Skeleton_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Skeleton_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Skeleton_Miniboss_IsWeighted;

                    Scale = EnemyEV.Skeleton_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Skeleton_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Skeleton_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Skeleton_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Skeleton_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Skeleton_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Skeleton_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Skeleton_Expert_Name;
                    LocStringID = EnemyEV.Skeleton_Expert_Name_locID;

                    MaxHealth = EnemyEV.Skeleton_Expert_MaxHealth;
                    Damage = EnemyEV.Skeleton_Expert_Damage;
                    XPValue = EnemyEV.Skeleton_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Skeleton_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Skeleton_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Skeleton_Expert_DropChance;

                    Speed = EnemyEV.Skeleton_Expert_Speed;
                    TurnSpeed = EnemyEV.Skeleton_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Skeleton_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Skeleton_Expert_Jump;
                    CooldownTime = EnemyEV.Skeleton_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Skeleton_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Skeleton_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Skeleton_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Skeleton_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Skeleton_Expert_IsWeighted;

                    Scale = EnemyEV.Skeleton_Expert_Scale;
                    ProjectileScale = EnemyEV.Skeleton_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Skeleton_Expert_Tint;

                    MeleeRadius = EnemyEV.Skeleton_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Skeleton_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Skeleton_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Skeleton_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Skeleton_Advanced_Name;
                    LocStringID = EnemyEV.Skeleton_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Skeleton_Advanced_MaxHealth;
                    Damage = EnemyEV.Skeleton_Advanced_Damage;
                    XPValue = EnemyEV.Skeleton_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Skeleton_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Skeleton_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Skeleton_Advanced_DropChance;

                    Speed = EnemyEV.Skeleton_Advanced_Speed;
                    TurnSpeed = EnemyEV.Skeleton_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Skeleton_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Skeleton_Advanced_Jump;
                    CooldownTime = EnemyEV.Skeleton_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Skeleton_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Skeleton_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Skeleton_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Skeleton_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Skeleton_Advanced_IsWeighted;

                    Scale = EnemyEV.Skeleton_Advanced_Scale;
                    ProjectileScale = EnemyEV.Skeleton_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Skeleton_Advanced_Tint;

                    MeleeRadius = EnemyEV.Skeleton_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Skeleton_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Skeleton_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Skeleton_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }		
	

        }

        protected override void InitializeLogic()
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "BoneProjectile_Sprite",
                SourceAnchor = new Vector2(20, -20),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = true,
                RotationSpeed = 10,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(-72, -72),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
            };

            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonWalk_Character", true, true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(0.2f, 0.75f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonWalk_Character", true, true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(0.2f, 0.75f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.2f, 0.75f));


            LogicSet walkStopMiniBossLS = new LogicSet(this);
            walkStopMiniBossLS.AddAction(new StopAnimationLogicAction());
            walkStopMiniBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopMiniBossLS.AddAction(new DelayLogicAction(0.5f, 1.0f));   

            LogicSet throwBoneFarLS = new LogicSet(this);
            throwBoneFarLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwBoneFarLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwBoneFarLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            throwBoneFarLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            throwBoneFarLS.AddAction(new DelayLogicAction(AttackDelay));
            throwBoneFarLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            throwBoneFarLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneFarLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            throwBoneFarLS.AddAction(new DelayLogicAction(0.2f, 0.4f));
            throwBoneFarLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwBoneFarLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwBoneHighLS = new LogicSet(this);
            throwBoneHighLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwBoneHighLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwBoneHighLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            throwBoneHighLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            throwBoneHighLS.AddAction(new DelayLogicAction(AttackDelay));
            throwBoneHighLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            projData.Angle = new Vector2(-85, -85);
            throwBoneHighLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneHighLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            throwBoneHighLS.AddAction(new DelayLogicAction(0.2f, 0.4f));
            throwBoneHighLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwBoneHighLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet jumpBoneFarLS = new LogicSet(this);
            jumpBoneFarLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpBoneFarLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonJump_Character", false, false));
            jumpBoneFarLS.AddAction(new PlayAnimationLogicAction(1, 3, false));
            jumpBoneFarLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpBoneFarLS.AddAction(new JumpLogicAction());
            jumpBoneFarLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpBoneFarLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            jumpBoneFarLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            jumpBoneFarLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            projData.Angle = new Vector2(-72, -72);
            jumpBoneFarLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            jumpBoneFarLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            jumpBoneFarLS.AddAction(new DelayLogicAction(0.2f, 0.4f));
            jumpBoneFarLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpBoneFarLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet jumpBoneHighLS = new LogicSet(this);
            jumpBoneHighLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpBoneHighLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonJump_Character", false, false));
            jumpBoneHighLS.AddAction(new PlayAnimationLogicAction(1, 3, false));
            jumpBoneHighLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpBoneHighLS.AddAction(new JumpLogicAction());
            jumpBoneHighLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpBoneHighLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            jumpBoneHighLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            jumpBoneHighLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            projData.Angle = new Vector2(-85, -85);
            jumpBoneHighLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            jumpBoneHighLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            jumpBoneHighLS.AddAction(new DelayLogicAction(0.2f, 0.4f));
            jumpBoneHighLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpBoneHighLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwBoneExpertLS = new LogicSet(this);
            throwBoneExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwBoneExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwBoneExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            throwBoneExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            throwBoneExpertLS.AddAction(new DelayLogicAction(AttackDelay));
            throwBoneExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            ThrowThreeProjectiles(throwBoneExpertLS);
            throwBoneExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End"), Types.Sequence.Parallel);
            throwBoneExpertLS.AddAction(new DelayLogicAction(0.15f));
            ThrowThreeProjectiles(throwBoneExpertLS);
            throwBoneExpertLS.AddAction(new DelayLogicAction(0.15f));
            ThrowThreeProjectiles(throwBoneExpertLS);
            throwBoneExpertLS.AddAction(new DelayLogicAction(0.2f, 0.4f));
            throwBoneExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwBoneExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet jumpBoneExpertLS = new LogicSet(this);
            jumpBoneExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpBoneExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonJump_Character", false, false));
            jumpBoneExpertLS.AddAction(new PlayAnimationLogicAction(1, 3, false));
            jumpBoneExpertLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpBoneExpertLS.AddAction(new JumpLogicAction());
            jumpBoneExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpBoneExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            jumpBoneExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            jumpBoneExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            ThrowThreeProjectiles(jumpBoneExpertLS);
            jumpBoneExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End"), Types.Sequence.Parallel);
            jumpBoneExpertLS.AddAction(new DelayLogicAction(0.15f));
            ThrowThreeProjectiles(jumpBoneExpertLS);
            jumpBoneExpertLS.AddAction(new DelayLogicAction(0.15f));
            ThrowThreeProjectiles(jumpBoneExpertLS);
            jumpBoneExpertLS.AddAction(new DelayLogicAction(0.2f, 0.4f));
            jumpBoneExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpBoneExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet throwBoneMiniBossLS = new LogicSet(this);
            throwBoneMiniBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwBoneMiniBossLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwBoneMiniBossLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            throwBoneMiniBossLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            throwBoneMiniBossLS.AddAction(new DelayLogicAction(AttackDelay));
            projData.Angle = new Vector2(-89, -35);
            projData.RotationSpeed = 8;
            projData.SourceAnchor = new Vector2(5, -20);
            throwBoneMiniBossLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            throwBoneMiniBossLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            throwBoneMiniBossLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwBoneMiniBossLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonIdle_Character", true, true));
            throwBoneMiniBossLS.AddAction(new DelayLogicAction(0.40f, 0.90f));
            

            LogicSet throwBoneMiniBossRageLS = new LogicSet(this);
            throwBoneMiniBossRageLS.AddAction(new MoveLogicAction(m_target, true, 0));
            throwBoneMiniBossRageLS.AddAction(new LockFaceDirectionLogicAction(true));
            throwBoneMiniBossRageLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonAttack_Character", false, false));
            throwBoneMiniBossRageLS.AddAction(new PlayAnimationLogicAction("Start", "Windup"));
            throwBoneMiniBossRageLS.AddAction(new DelayLogicAction(AttackDelay));
            throwBoneMiniBossRageLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SkeletonHit1"));
            throwBoneMiniBossRageLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossRageLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossRageLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossRageLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossRageLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            throwBoneMiniBossRageLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            throwBoneMiniBossRageLS.AddAction(new LockFaceDirectionLogicAction(false));
            throwBoneMiniBossRageLS.AddAction(new DelayLogicAction(0.15f, 0.35f));
            throwBoneMiniBossRageLS.AddAction(new ChangeSpriteLogicAction("EnemySkeletonIdle_Character", true, true));

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS, jumpBoneFarLS, jumpBoneHighLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, throwBoneExpertLS, jumpBoneExpertLS);
            m_generalMiniBossLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopMiniBossLS, throwBoneMiniBossLS, throwBoneMiniBossRageLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 30, 30, 40); //walkTowardsLS, walkAwayLS, walkStopLS

            projData.Dispose();

            base.InitializeLogic();
        }

        private void ThrowThreeProjectiles(LogicSet ls)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "BoneProjectile_Sprite",
                SourceAnchor = new Vector2(20, -20),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = true,
                RotationSpeed = 10,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(-72, -72),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
            };

            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Speed = new Vector2(ProjectileSpeed - 350, ProjectileSpeed - 350);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Speed = new Vector2(ProjectileSpeed + 350, ProjectileSpeed + 350);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "BoneProjectile_Sprite", new Vector2(20, -20), -72, ProjectileSpeed, true, 10, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "BoneProjectile_Sprite", new Vector2(20, -20), -72, ProjectileSpeed - 350, true, 10, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "BoneProjectile_Sprite", new Vector2(20, -20), -72, ProjectileSpeed + 350, true, 10, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "BoneProjectile_Sprite", new Vector2(20, -18), -60, 10, true, 10, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "BoneProjectile_Sprite", new Vector2(20, -18), -60, 16, true, 10, Damage));
            //ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, "BoneProjectile_Sprite", new Vector2(20, -18), -60, 22, true, 10, Damage));

            projData.Dispose();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 10, 10, 0, 30, 50); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 10, 10, 0, 40, 40); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS
                    break;
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 40, 40, 20, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS
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
                    RunLogicBlock(true, m_generalAdvancedLB, 10, 10, 0, 15, 15, 25, 25); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS, jumpBoneFarLS, jumpBoneHighLS
                    break;
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 40, 40, 20, 0, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneFarLS, throwBoneHighLS, jumpBoneFarLS, jumpBoneHighLS
                    break;
                default:
                    RunBasicLogic();
                    break;
            }
        }

        protected override void RunExpertLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 15, 15, 0, 35, 35); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneExpertLS, jumpBoneExpertLS
                    break;
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 35, 35, 0, 0, 15); //walkTowardsLS, walkAwayLS, walkStopLS, throwBoneExpertLS, jumpBoneExpertLS
                    break;
                default:
                    RunBasicLogic();
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
                    if (m_levelScreen.CurrentRoom.ActiveEnemies > 1)
                        RunLogicBlock(true, m_generalMiniBossLB, 0, 0, 10, 90, 0); //walkTowardsLS, walkAwayLS, walkStopMiniBossLS, throwBoneMiniBossLS, throwBoneMiniBossRageLS
                    else
                    {
                        Console.WriteLine("RAGING");
                        RunLogicBlock(true, m_generalMiniBossLB, 0, 0, 10, 0, 90); //walkTowardsLS, walkAwayLS, walkStopMiniBossLS, throwBoneMiniBossLS, throwBoneMiniBossRageLS
                    }
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS && m_levelScreen.CurrentRoom.ActiveEnemies == 1)
                this.TintablePart.TextureColor = new Color(185, 0, 15);

            base.Update(gameTime);
        }

        public EnemyObj_Skeleton(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemySkeletonIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Skeleton;
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"SkeletonAttack1");
            base.HitEnemy(damage, position, isPlayer);
        }
    }
}

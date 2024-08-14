using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;

namespace RogueCastle
{
    public class EnemyObj_Starburst : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();

        private float FireballDelay = 0.5f;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Starburst_Basic_Name;
            LocStringID = EnemyEV.Starburst_Basic_Name_locID;

            MaxHealth = EnemyEV.Starburst_Basic_MaxHealth;
            Damage = EnemyEV.Starburst_Basic_Damage;
            XPValue = EnemyEV.Starburst_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Starburst_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Starburst_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Starburst_Basic_DropChance;

            Speed = EnemyEV.Starburst_Basic_Speed;
            TurnSpeed = EnemyEV.Starburst_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Starburst_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Starburst_Basic_Jump;
            CooldownTime = EnemyEV.Starburst_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Starburst_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Starburst_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Starburst_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Starburst_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Starburst_Basic_IsWeighted;

            Scale = EnemyEV.Starburst_Basic_Scale;
            ProjectileScale = EnemyEV.Starburst_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Starburst_Basic_Tint;

            MeleeRadius = EnemyEV.Starburst_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Starburst_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Starburst_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Starburst_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Starburst_Miniboss_Name;
                    LocStringID = EnemyEV.Starburst_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Starburst_Miniboss_MaxHealth;
                    Damage = EnemyEV.Starburst_Miniboss_Damage;
                    XPValue = EnemyEV.Starburst_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Starburst_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Starburst_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Starburst_Miniboss_DropChance;

                    Speed = EnemyEV.Starburst_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Starburst_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Starburst_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Starburst_Miniboss_Jump;
                    CooldownTime = EnemyEV.Starburst_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Starburst_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Starburst_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Starburst_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Starburst_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Starburst_Miniboss_IsWeighted;

                    Scale = EnemyEV.Starburst_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Starburst_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Starburst_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Starburst_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Starburst_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Starburst_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Starburst_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Starburst_Expert_Name;
                    LocStringID = EnemyEV.Starburst_Expert_Name_locID;

                    MaxHealth = EnemyEV.Starburst_Expert_MaxHealth;
                    Damage = EnemyEV.Starburst_Expert_Damage;
                    XPValue = EnemyEV.Starburst_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Starburst_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Starburst_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Starburst_Expert_DropChance;

                    Speed = EnemyEV.Starburst_Expert_Speed;
                    TurnSpeed = EnemyEV.Starburst_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Starburst_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Starburst_Expert_Jump;
                    CooldownTime = EnemyEV.Starburst_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Starburst_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Starburst_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Starburst_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Starburst_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Starburst_Expert_IsWeighted;

                    Scale = EnemyEV.Starburst_Expert_Scale;
                    ProjectileScale = EnemyEV.Starburst_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Starburst_Expert_Tint;

                    MeleeRadius = EnemyEV.Starburst_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Starburst_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Starburst_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Starburst_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Starburst_Advanced_Name;
                    LocStringID = EnemyEV.Starburst_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Starburst_Advanced_MaxHealth;
                    Damage = EnemyEV.Starburst_Advanced_Damage;
                    XPValue = EnemyEV.Starburst_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Starburst_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Starburst_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Starburst_Advanced_DropChance;

                    Speed = EnemyEV.Starburst_Advanced_Speed;
                    TurnSpeed = EnemyEV.Starburst_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Starburst_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Starburst_Advanced_Jump;
                    CooldownTime = EnemyEV.Starburst_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Starburst_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Starburst_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Starburst_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Starburst_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Starburst_Advanced_IsWeighted;

                    Scale = EnemyEV.Starburst_Advanced_Scale;
                    ProjectileScale = EnemyEV.Starburst_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Starburst_Advanced_Tint;

                    MeleeRadius = EnemyEV.Starburst_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Starburst_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Starburst_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Starburst_Advanced_KnockBack;
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
                SpriteName = "TurretProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Speed = new Vector2(this.ProjectileSpeed, this.ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = true, //false
                Scale = ProjectileScale,
            };

            LogicSet fireProjectileBasicLS = new LogicSet(this);
            projData.Angle = new Vector2(0, 0);
            fireProjectileBasicLS.AddAction(new RunFunctionLogicAction(this, "FireAnimation"));
            fireProjectileBasicLS.AddAction(new DelayLogicAction(FireballDelay));
            fireProjectileBasicLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Eyeball_ProjectileAttack"));
            fireProjectileBasicLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            fireProjectileBasicLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(90, 90);
            fireProjectileBasicLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(180, 180);
            fireProjectileBasicLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileBasicLS.AddAction(new ChangeSpriteLogicAction("EnemyStarburstIdle_Character", true, true));
            fireProjectileBasicLS.AddAction(new DelayLogicAction(1.0f, 1.0f));
            fireProjectileBasicLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet fireProjectileAdvancedLS = new LogicSet(this);
            projData.Angle = new Vector2(45, 45);
            fireProjectileAdvancedLS.AddAction(new ChangePropertyLogicAction(_objectList[1], "Rotation", 45));
            fireProjectileAdvancedLS.AddAction(new RunFunctionLogicAction(this, "FireAnimation"));
            fireProjectileAdvancedLS.AddAction(new ChangePropertyLogicAction(_objectList[1], "Rotation", 45));
            fireProjectileAdvancedLS.AddAction(new DelayLogicAction(FireballDelay));
            fireProjectileAdvancedLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Eyeball_ProjectileAttack"));
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-45, -45);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(135, 135);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-135, -135);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));       
            projData.Angle = new Vector2(-90, -90);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(90, 90);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(180, 180);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(0, 0);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyStarburstIdle_Character", true, true));
            fireProjectileAdvancedLS.AddAction(new ChangePropertyLogicAction(_objectList[1], "Rotation", 45));
            fireProjectileAdvancedLS.AddAction(new DelayLogicAction(1.0f, 1.0f));
            fireProjectileAdvancedLS.Tag = GameTypes.LogicSetType_ATTACK;

            //TEDDY - Just like Advanced projetiles but made so the bullets go through terrain.
            #region EXPERT
            LogicSet fireProjectileExpertLS = new LogicSet(this);
            projData.Angle = new Vector2(45, 45);
            projData.CollidesWithTerrain = false;
            projData.SpriteName = "GhostProjectile_Sprite";
            fireProjectileExpertLS.AddAction(new ChangePropertyLogicAction(_objectList[1], "Rotation", 45));
            fireProjectileExpertLS.AddAction(new RunFunctionLogicAction(this, "FireAnimation"));
            fireProjectileExpertLS.AddAction(new ChangePropertyLogicAction(_objectList[1], "Rotation", 45));
            fireProjectileExpertLS.AddAction(new DelayLogicAction(FireballDelay));
            fireProjectileExpertLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Eyeball_ProjectileAttack"));
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-45, -45);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(135, 135);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-135, -135);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(90, 90);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(180, 180);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(0, 0);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyStarburstIdle_Character", true, true));
            fireProjectileExpertLS.AddAction(new DelayLogicAction(1.0f, 1.0f));

            fireProjectileExpertLS.AddAction(new RunFunctionLogicAction(this, "FireAnimation"));
            fireProjectileExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "Eyeball_ProjectileAttack"));
            projData.Angle = new Vector2(25, 25);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-25, -25);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(115, 115);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-115, -115);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-70, -70);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(70, 70);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(160, 160);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-160, -160);
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyStarburstIdle_Character", true, true));
            fireProjectileExpertLS.AddAction(new ChangePropertyLogicAction(_objectList[1], "Rotation", 45));
            fireProjectileExpertLS.AddAction(new DelayLogicAction(1.25f, 1.25f));
            fireProjectileExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            #endregion


            LogicSet doNothing = new LogicSet(this);
            doNothing.AddAction(new DelayLogicAction(0.5f, 0.5f));

            m_generalBasicLB.AddLogicSet(fireProjectileBasicLS, doNothing);
            m_generalAdvancedLB.AddLogicSet(fireProjectileAdvancedLS, doNothing);
            m_generalExpertLB.AddLogicSet(fireProjectileExpertLS, doNothing);
            m_generalMiniBossLB.AddLogicSet(fireProjectileAdvancedLS, doNothing);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);

            projData.Dispose();

            base.InitializeLogic();
        }

        public void FireAnimation()
        {
            this.ChangeSprite("EnemyStarburstAttack_Character");
            (_objectList[0] as IAnimateableObj).PlayAnimation(true);
            (_objectList[1] as IAnimateableObj).PlayAnimation(false);
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100, 0);
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 100);
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
                    RunLogicBlock(true, m_generalAdvancedLB, 100, 0);
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 100);
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
                    RunLogicBlock(true, m_generalExpertLB, 100, 0);
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 0, 100);
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
                    RunLogicBlock(true, m_generalMiniBossLB, 60, 40, 0);
                    break;
                default:
                    break;
            }
        }

        public EnemyObj_Starburst(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyStarburstIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Starburst;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_HomingTurret : EnemyObj
    {
        private float FireDelay = 5;

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();

        protected override void InitializeEV()
        {
            LockFlip = false;
            FireDelay = 2.0f;//5;

            #region Basic Variables - General
            Name = EnemyEV.HomingTurret_Basic_Name;
            LocStringID = EnemyEV.HomingTurret_Basic_Name_locID;

            MaxHealth = EnemyEV.HomingTurret_Basic_MaxHealth;
            Damage = EnemyEV.HomingTurret_Basic_Damage;
            XPValue = EnemyEV.HomingTurret_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.HomingTurret_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.HomingTurret_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.HomingTurret_Basic_DropChance;

            Speed = EnemyEV.HomingTurret_Basic_Speed;
            TurnSpeed = EnemyEV.HomingTurret_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.HomingTurret_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.HomingTurret_Basic_Jump;
            CooldownTime = EnemyEV.HomingTurret_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.HomingTurret_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.HomingTurret_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.HomingTurret_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.HomingTurret_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.HomingTurret_Basic_IsWeighted;

            Scale = EnemyEV.HomingTurret_Basic_Scale;
            ProjectileScale = EnemyEV.HomingTurret_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.HomingTurret_Basic_Tint;

            MeleeRadius = EnemyEV.HomingTurret_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.HomingTurret_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.HomingTurret_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.HomingTurret_Basic_KnockBack;
            #endregion

            InitialLogicDelay = 1;

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.HomingTurret_Miniboss_Name;
                    LocStringID = EnemyEV.HomingTurret_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.HomingTurret_Miniboss_MaxHealth;
                    Damage = EnemyEV.HomingTurret_Miniboss_Damage;
                    XPValue = EnemyEV.HomingTurret_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.HomingTurret_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.HomingTurret_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.HomingTurret_Miniboss_DropChance;

                    Speed = EnemyEV.HomingTurret_Miniboss_Speed;
                    TurnSpeed = EnemyEV.HomingTurret_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.HomingTurret_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.HomingTurret_Miniboss_Jump;
                    CooldownTime = EnemyEV.HomingTurret_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.HomingTurret_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.HomingTurret_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.HomingTurret_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.HomingTurret_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.HomingTurret_Miniboss_IsWeighted;

                    Scale = EnemyEV.HomingTurret_Miniboss_Scale;
                    ProjectileScale = EnemyEV.HomingTurret_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.HomingTurret_Miniboss_Tint;

                    MeleeRadius = EnemyEV.HomingTurret_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.HomingTurret_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.HomingTurret_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.HomingTurret_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    FireDelay = 2.25f;//5;
                    #region Expert Variables - General
                    Name = EnemyEV.HomingTurret_Expert_Name;
                    LocStringID = EnemyEV.HomingTurret_Expert_Name_locID;

                    MaxHealth = EnemyEV.HomingTurret_Expert_MaxHealth;
                    Damage = EnemyEV.HomingTurret_Expert_Damage;
                    XPValue = EnemyEV.HomingTurret_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.HomingTurret_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.HomingTurret_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.HomingTurret_Expert_DropChance;

                    Speed = EnemyEV.HomingTurret_Expert_Speed;
                    TurnSpeed = EnemyEV.HomingTurret_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.HomingTurret_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.HomingTurret_Expert_Jump;
                    CooldownTime = EnemyEV.HomingTurret_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.HomingTurret_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.HomingTurret_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.HomingTurret_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.HomingTurret_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.HomingTurret_Expert_IsWeighted;

                    Scale = EnemyEV.HomingTurret_Expert_Scale;
                    ProjectileScale = EnemyEV.HomingTurret_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.HomingTurret_Expert_Tint;

                    MeleeRadius = EnemyEV.HomingTurret_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.HomingTurret_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.HomingTurret_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.HomingTurret_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    FireDelay = 1.5f;//5;
                    #region Advanced Variables - General
                    Name = EnemyEV.HomingTurret_Advanced_Name;
                    LocStringID = EnemyEV.HomingTurret_Advanced_Name_locID;

                    MaxHealth = EnemyEV.HomingTurret_Advanced_MaxHealth;
                    Damage = EnemyEV.HomingTurret_Advanced_Damage;
                    XPValue = EnemyEV.HomingTurret_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.HomingTurret_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.HomingTurret_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.HomingTurret_Advanced_DropChance;

                    Speed = EnemyEV.HomingTurret_Advanced_Speed;
                    TurnSpeed = EnemyEV.HomingTurret_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.HomingTurret_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.HomingTurret_Advanced_Jump;
                    CooldownTime = EnemyEV.HomingTurret_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.HomingTurret_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.HomingTurret_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.HomingTurret_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.HomingTurret_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.HomingTurret_Advanced_IsWeighted;

                    Scale = EnemyEV.HomingTurret_Advanced_Scale;
                    ProjectileScale = EnemyEV.HomingTurret_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.HomingTurret_Advanced_Tint;

                    MeleeRadius = EnemyEV.HomingTurret_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.HomingTurret_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.HomingTurret_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.HomingTurret_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }							

        }

        protected override void InitializeLogic()
        {
            float angle = this.Rotation;
            float delay = this.ParseTagToFloat("delay");
            float speed = this.ParseTagToFloat("speed");
            if (delay == 0)
            {
                Console.WriteLine("ERROR: Turret set with delay of 0. Shoots too fast.");
                delay = FireDelay;
            }

            if (speed == 0)
                speed = ProjectileSpeed;

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "HomingProjectile_Sprite",
                SourceAnchor = new Vector2(35,0),
                //Target = m_target,
                Speed = new Vector2(speed, speed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = true,
                Scale = ProjectileScale,
                FollowArc = false,
                ChaseTarget = false,
                TurnSpeed = 0f,//0.02f,
                StartingRotation = 0,
                Lifespan = 10f,            
            };

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet fireProjectileLS = new LogicSet(this);
            fireProjectileLS.AddAction(new PlayAnimationLogicAction(false), Types.Sequence.Parallel);
            fireProjectileLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileLS.AddAction(new RunFunctionLogicAction(this, "FireProjectileEffect"));
            fireProjectileLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            fireProjectileLS.AddAction(new DelayLogicAction(delay));

            LogicSet fireProjectileAdvancedLS = new LogicSet(this);
            fireProjectileAdvancedLS.AddAction(new PlayAnimationLogicAction(false), Types.Sequence.Parallel);
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileAdvancedLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            fireProjectileAdvancedLS.AddAction(new DelayLogicAction(0.1f));
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileAdvancedLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            fireProjectileAdvancedLS.AddAction(new DelayLogicAction(0.1f));
            fireProjectileAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileAdvancedLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            fireProjectileAdvancedLS.AddAction(new RunFunctionLogicAction(this, "FireProjectileEffect"));
            fireProjectileAdvancedLS.AddAction(new DelayLogicAction(delay));

            LogicSet fireProjectileExpertLS = new LogicSet(this);
            fireProjectileExpertLS.AddAction(new PlayAnimationLogicAction(false), Types.Sequence.Parallel);
            projData.ChaseTarget = true;
            projData.Target = m_target;
            projData.TurnSpeed = 0.02f;//0.065f;
            fireProjectileExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            fireProjectileExpertLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Turret_Attack01", "Turret_Attack02", "Turret_Attack03"));
            fireProjectileExpertLS.AddAction(new RunFunctionLogicAction(this, "FireProjectileEffect"));
            fireProjectileExpertLS.AddAction(new DelayLogicAction(delay));

            m_generalBasicLB.AddLogicSet(fireProjectileLS, walkStopLS);
            m_generalAdvancedLB.AddLogicSet(fireProjectileAdvancedLS, walkStopLS);
            m_generalExpertLB.AddLogicSet(fireProjectileExpertLS, walkStopLS);
            m_generalMiniBossLB.AddLogicSet(fireProjectileLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);

            projData.Dispose();

            base.InitializeLogic();
        }

        public void FireProjectileEffect()
        {
            Vector2 pos = this.Position;
            if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                pos.X += 30;
            else
                pos.X -= 30;
            m_levelScreen.ImpactEffectPool.TurretFireEffect(pos, new Vector2(0.5f, 0.5f));
            m_levelScreen.ImpactEffectPool.TurretFireEffect(pos, new Vector2(0.5f, 0.5f));
            m_levelScreen.ImpactEffectPool.TurretFireEffect(pos, new Vector2(0.5f, 0.5f));
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(false, m_generalBasicLB, 100, 0);
                    break;
                case (STATE_WANDER):
                default:
                    RunLogicBlock(false, m_generalBasicLB, 100, 0);
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
                    RunLogicBlock(false, m_generalAdvancedLB, 100, 0);
                    break;
                case (STATE_WANDER):
                default:
                    RunLogicBlock(false, m_generalAdvancedLB, 100, 0);
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
                    RunLogicBlock(false, m_generalExpertLB, 100, 0);
                    break;
                case (STATE_WANDER):
                default:
                    RunLogicBlock(false, m_generalExpertLB, 0, 100);
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
                    RunLogicBlock(false, m_generalBasicLB, 100, 0);
                    break;
                case (STATE_WANDER):
                default:
                    RunLogicBlock(false, m_generalBasicLB, 100, 0);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public EnemyObj_HomingTurret(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyHomingTurret_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.StopAnimation();
            ForceDraw = true;
            this.Type = EnemyType.HomingTurret;
            this.PlayAnimationOnRestart = false;
        }
    }
}

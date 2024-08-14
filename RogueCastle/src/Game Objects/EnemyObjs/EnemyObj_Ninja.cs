using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Ninja: EnemyObj
    {
        private LogicBlock m_basicTeleportAttackLB = new LogicBlock();
        private LogicBlock m_expertTeleportAttackLB = new LogicBlock();
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private RoomObj m_storedRoom; // Used to make sure ninjas don't teleport to the room you're in.

        private SpriteObj m_smoke;
        private SpriteObj m_log;

        private float TeleportDelay = 0.35f;
        private float ChanceToTeleport = 0.35f;
        private float PauseBeforeProjectile = 0.45f;
        private float PauseAfterProjectile = 0.45f;

        private float m_teleportDamageReduc = 0.60f;
        private TerrainObj m_closestCeiling;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Ninja_Basic_Name;
            LocStringID = EnemyEV.Ninja_Basic_Name_locID;

            MaxHealth = EnemyEV.Ninja_Basic_MaxHealth;
            Damage = EnemyEV.Ninja_Basic_Damage;
            XPValue = EnemyEV.Ninja_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Ninja_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Ninja_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Ninja_Basic_DropChance;

            Speed = EnemyEV.Ninja_Basic_Speed;
            TurnSpeed = EnemyEV.Ninja_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Ninja_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Ninja_Basic_Jump;
            CooldownTime = EnemyEV.Ninja_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Ninja_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Ninja_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Ninja_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Ninja_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Ninja_Basic_IsWeighted;

            Scale = EnemyEV.Ninja_Basic_Scale;
            ProjectileScale = EnemyEV.Ninja_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Ninja_Basic_Tint;

            MeleeRadius = EnemyEV.Ninja_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Ninja_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Ninja_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Ninja_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Ninja_Miniboss_Name;
                    LocStringID = EnemyEV.Ninja_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Ninja_Miniboss_MaxHealth;
                    Damage = EnemyEV.Ninja_Miniboss_Damage;
                    XPValue = EnemyEV.Ninja_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Ninja_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Ninja_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Ninja_Miniboss_DropChance;

                    Speed = EnemyEV.Ninja_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Ninja_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Ninja_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Ninja_Miniboss_Jump;
                    CooldownTime = EnemyEV.Ninja_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Ninja_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Ninja_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Ninja_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Ninja_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Ninja_Miniboss_IsWeighted;

                    Scale = EnemyEV.Ninja_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Ninja_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Ninja_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Ninja_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Ninja_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Ninja_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Ninja_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):

                    ChanceToTeleport = 0.65f;
                    #region Expert Variables - General
                    Name = EnemyEV.Ninja_Expert_Name;
                    LocStringID = EnemyEV.Ninja_Expert_Name_locID;

                    MaxHealth = EnemyEV.Ninja_Expert_MaxHealth;
                    Damage = EnemyEV.Ninja_Expert_Damage;
                    XPValue = EnemyEV.Ninja_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Ninja_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Ninja_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Ninja_Expert_DropChance;

                    Speed = EnemyEV.Ninja_Expert_Speed;
                    TurnSpeed = EnemyEV.Ninja_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Ninja_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Ninja_Expert_Jump;
                    CooldownTime = EnemyEV.Ninja_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Ninja_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Ninja_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Ninja_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Ninja_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Ninja_Expert_IsWeighted;

                    Scale = EnemyEV.Ninja_Expert_Scale;
                    ProjectileScale = EnemyEV.Ninja_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Ninja_Expert_Tint;

                    MeleeRadius = EnemyEV.Ninja_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Ninja_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Ninja_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Ninja_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    ChanceToTeleport = 0.50f;

                    #region Advanced Variables - General
                    Name = EnemyEV.Ninja_Advanced_Name;
                    LocStringID = EnemyEV.Ninja_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Ninja_Advanced_MaxHealth;
                    Damage = EnemyEV.Ninja_Advanced_Damage;
                    XPValue = EnemyEV.Ninja_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Ninja_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Ninja_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Ninja_Advanced_DropChance;

                    Speed = EnemyEV.Ninja_Advanced_Speed;
                    TurnSpeed = EnemyEV.Ninja_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Ninja_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Ninja_Advanced_Jump;
                    CooldownTime = EnemyEV.Ninja_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Ninja_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Ninja_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Ninja_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Ninja_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Ninja_Advanced_IsWeighted;

                    Scale = EnemyEV.Ninja_Advanced_Scale;
                    ProjectileScale = EnemyEV.Ninja_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Ninja_Advanced_Tint;

                    MeleeRadius = EnemyEV.Ninja_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Ninja_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Ninja_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Ninja_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }
        }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaRun_Character", true, true));
            walkTowardsLS.AddAction(new PlayAnimationLogicAction(true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(0.25f, 0.85f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaRun_Character", true, true));
            walkAwayLS.AddAction(new PlayAnimationLogicAction(true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(0.25f, 0.85f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaIdle_Character", true, true));
            walkStopLS.AddAction(new StopAnimationLogicAction());
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.25f, 0.85f));

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "ShurikenProjectile1_Sprite",
                SourceAnchor = new Vector2(15,0),
                Target = m_target,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 20,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = true,
                Scale = ProjectileScale,
            };

            LogicSet basicThrowShurikenLS = new LogicSet(this);
            basicThrowShurikenLS.AddAction(new MoveLogicAction(m_target, true, 0));
            basicThrowShurikenLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaIdle_Character", true, true));
            basicThrowShurikenLS.AddAction(new DelayLogicAction(PauseBeforeProjectile));
            basicThrowShurikenLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaThrow_Character", false, false));
            basicThrowShurikenLS.AddAction(new PlayAnimationLogicAction(1, 3, false));
            basicThrowShurikenLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Ninja_ThrowStar_01", "Ninja_ThrowStar_02", "Ninja_ThrowStar_03"));
            basicThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            basicThrowShurikenLS.AddAction(new PlayAnimationLogicAction(4, 5, false));
            basicThrowShurikenLS.AddAction(new DelayLogicAction(PauseAfterProjectile));                     
            basicThrowShurikenLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet advancedThrowShurikenLS = new LogicSet(this);
            advancedThrowShurikenLS.AddAction(new MoveLogicAction(m_target, true, 0));
            advancedThrowShurikenLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaIdle_Character", true, true));
            advancedThrowShurikenLS.AddAction(new DelayLogicAction(PauseBeforeProjectile));
            advancedThrowShurikenLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaThrow_Character", false, false));
            advancedThrowShurikenLS.AddAction(new PlayAnimationLogicAction(1, 3, false));
            advancedThrowShurikenLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "Ninja_ThrowStar_01", "Ninja_ThrowStar_02", "Ninja_ThrowStar_03"));
            advancedThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = -10;
            advancedThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = 10;
            advancedThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            advancedThrowShurikenLS.AddAction(new PlayAnimationLogicAction(4, 5, false));
            advancedThrowShurikenLS.AddAction(new DelayLogicAction(PauseAfterProjectile));
            advancedThrowShurikenLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet expertThrowShurikenLS = new LogicSet(this);
            expertThrowShurikenLS.AddAction(new MoveLogicAction(m_target, true, 0));
            expertThrowShurikenLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaIdle_Character", true, true));
            expertThrowShurikenLS.AddAction(new DelayLogicAction(PauseBeforeProjectile));
            expertThrowShurikenLS.AddAction(new ChangeSpriteLogicAction("EnemyNinjaThrow_Character", false, false));
            expertThrowShurikenLS.AddAction(new PlayAnimationLogicAction(1, 3, false));
            expertThrowShurikenLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "Ninja_ThrowStar_01", "Ninja_ThrowStar_02", "Ninja_ThrowStar_03"));
            projData.AngleOffset = 0;
            expertThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = -5;
            expertThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = 5;
            expertThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = -25;
            expertThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = 25;
            expertThrowShurikenLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            expertThrowShurikenLS.AddAction(new PlayAnimationLogicAction(4, 5, false));
            expertThrowShurikenLS.AddAction(new DelayLogicAction(PauseAfterProjectile));
            expertThrowShurikenLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet teleportAttackLS = new LogicSet(this);
            teleportAttackLS.AddAction(new LockFaceDirectionLogicAction(true));
            teleportAttackLS.AddAction(new RunFunctionLogicAction(this, "CreateLog", null));
            teleportAttackLS.AddAction(new DelayLogicAction(TeleportDelay));
            teleportAttackLS.AddAction(new RunFunctionLogicAction(this, "CreateSmoke", null));
            teleportAttackLS.AddAction(new DelayLogicAction(0.15f));
            teleportAttackLS.AddAction(new ChangePropertyLogicAction(this, "IsWeighted", true));
            teleportAttackLS.AddAction(new DelayLogicAction(0.15f));
            teleportAttackLS.AddAction(new GroundCheckLogicAction());
            teleportAttackLS.AddAction(new DelayLogicAction(0.15f));
            teleportAttackLS.AddAction(new LockFaceDirectionLogicAction(false));
            teleportAttackLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet teleportExpertAttackLS = new LogicSet(this);
            teleportExpertAttackLS.AddAction(new LockFaceDirectionLogicAction(true));
            teleportExpertAttackLS.AddAction(new RunFunctionLogicAction(this, "CreateLog", null));
            teleportExpertAttackLS.AddAction(new DelayLogicAction(TeleportDelay));
            teleportExpertAttackLS.AddAction(new RunFunctionLogicAction(this, "CreateSmoke", null));
            projData.Target = null;
            teleportExpertAttackLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "Ninja_ThrowStar_01", "Ninja_ThrowStar_02", "Ninja_ThrowStar_03"));
            projData.AngleOffset = 45;
            teleportExpertAttackLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = 135;
            teleportExpertAttackLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = -45;
            teleportExpertAttackLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.AngleOffset = -135;
            teleportExpertAttackLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            teleportExpertAttackLS.AddAction(new DelayLogicAction(0.15f));
            teleportExpertAttackLS.AddAction(new ChangePropertyLogicAction(this, "IsWeighted", true));
            teleportExpertAttackLS.AddAction(new DelayLogicAction(0.15f));
            teleportExpertAttackLS.AddAction(new GroundCheckLogicAction());
            teleportExpertAttackLS.AddAction(new DelayLogicAction(0.15f));
            teleportExpertAttackLS.AddAction(new LockFaceDirectionLogicAction(false));
            teleportExpertAttackLS.Tag = GameTypes.LogicSetType_ATTACK;

            m_basicTeleportAttackLB.AddLogicSet(teleportAttackLS);
            m_expertTeleportAttackLB.AddLogicSet(teleportExpertAttackLS);
            logicBlocksToDispose.Add(m_basicTeleportAttackLB);
            logicBlocksToDispose.Add(m_expertTeleportAttackLB);

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, advancedThrowShurikenLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, expertThrowShurikenLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 50, 50, 0); //walkTowardsLS, walkAwayLS, walkStopLS

            projData.Dispose();

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 40, 30, 0, 30); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 65, 35, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 50, 50, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
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
                    RunLogicBlock(true, m_generalAdvancedLB, 40, 30, 0, 30); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 65, 35, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 50, 50, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
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
                    RunLogicBlock(true, m_generalExpertLB, 40, 30, 0, 30); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 65, 35, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 50, 50, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
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
                    RunLogicBlock(true, m_generalBasicLB, 40, 30, 0, 30); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 65, 35, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 50, 50, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, basicThrowShurikenLS
                    break;
                default:
                    break;
            }
        }

        public EnemyObj_Ninja(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyNinjaIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Ninja;
            m_smoke = new SpriteObj("NinjaSmoke_Sprite");
            m_smoke.AnimationDelay = 1 / 20f;
            m_log = new SpriteObj("Log_Sprite");
            m_smoke.Visible = false;
            m_smoke.Scale = new Vector2(5, 5);
            m_log.Visible = false;
            m_log.OutlineWidth = 2;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Y < m_levelScreen.CurrentRoom.Y)
                this.Y = m_levelScreen.CurrentRoom.Y;
            base.Update(gameTime);
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            if (m_target != null && m_target.CurrentHealth > 0)
            {
                if (m_currentActiveLB != m_basicTeleportAttackLB && m_currentActiveLB != m_expertTeleportAttackLB)
                {
                    if (CDGMath.RandomFloat(0, 1) <= ChanceToTeleport && m_closestCeiling != null)
                    {
                        m_closestCeiling = FindClosestCeiling();
                        int distance = this.TerrainBounds.Top - m_closestCeiling.Bounds.Bottom;
                        if (m_closestCeiling != null && distance > 150 && distance < 700)
                        {
                            m_currentActiveLB.StopLogicBlock();
                            if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                                RunLogicBlock(false, m_expertTeleportAttackLB, 100);
                            else
                                RunLogicBlock(false, m_basicTeleportAttackLB, 100);

                            // Modify damage so that the ninja takes less.
                            //float damageMod = ;
                            damage = (int)(Math.Round(damage * (1 - m_teleportDamageReduc), MidpointRounding.AwayFromZero));
                        }
                    }
                }
            }
                
            base.HitEnemy(damage, position, isPlayer);
        }

        private TerrainObj FindClosestCeiling()
        {
            int closestDistance = int.MaxValue;
            TerrainObj closestObj = null;

            RoomObj room = m_levelScreen.CurrentRoom;

            foreach (TerrainObj terrain in room.TerrainObjList)
            {
                Rectangle collisionTestRect = new Rectangle(this.Bounds.Left, this.Bounds.Top - 2000, this.Bounds.Width, this.Bounds.Height + 2000);

                if (terrain.CollidesBottom == true && CollisionMath.Intersects(terrain.Bounds, collisionTestRect))
                {
                    // Only collide with terrain that the displacer would collide with.
                    float distance = float.MaxValue;
                    if (terrain.Bounds.Bottom < this.TerrainBounds.Top)
                        distance = this.TerrainBounds.Top - terrain.Bounds.Bottom;

                    if (distance < closestDistance)
                    {
                        closestDistance = (int)distance;
                        closestObj = terrain;
                    }
                }
            }

            return closestObj;
        }
        
        public void CreateLog()
        {
            m_log.Position = this.Position;
            m_smoke.Position = this.Position;
            m_smoke.Visible = true;
            m_log.Visible = true;
            m_log.Opacity = 1;
            m_smoke.PlayAnimation(false);

            Tweener.Tween.By(m_log, 0.1f, Tweener.Ease.Linear.EaseNone, "delay", "0.2", "Y", "10");
            Tweener.Tween.To(m_log, 0.2f, Tweener.Ease.Linear.EaseNone, "delay", "0.3", "Opacity", "0");

            SoundManager.Play3DSound(this, m_target, "Ninja_Teleport");
            this.Visible = false;
            this.IsCollidable = false;
            this.IsWeighted = false;

            m_storedRoom = m_levelScreen.CurrentRoom;
        }

        public void CreateSmoke()
        {
            if (m_levelScreen.CurrentRoom == m_storedRoom && m_closestCeiling != null)
            {
                //if (this.Y < m_levelScreen.CurrentRoom.Y)
                //    this.Y = m_levelScreen.CurrentRoom.Y + 60;

                //foreach (TerrainObj obj in m_levelScreen.CurrentRoom.TerrainObjList)
                //{
                //    if (CollisionMath.Intersects(this.Bounds, obj.Bounds))
                //        this.Y = obj.Bounds.Bottom + this.Height / 2;
                //}

                this.UpdateCollisionBoxes();
                this.Y = m_closestCeiling.Bounds.Bottom + (this.Y - this.TerrainBounds.Top);
                this.X = m_target.X;
                this.ChangeSprite("EnemyNinjaAttack_Character");
                this.Visible = true;
                this.AccelerationX = 0;
                this.AccelerationY = 0;
                this.CurrentSpeed = 0;
                this.IsCollidable = true;
                m_smoke.Position = this.Position;
                m_smoke.Visible = true;
                m_smoke.PlayAnimation(false);
                m_closestCeiling = null;
            }
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);
            m_log.Draw(camera);
            m_smoke.Draw(camera);
        }

        public override void Kill(bool giveXP = true)
        {
            m_smoke.Visible = false;
            m_log.Visible = false;
            base.Kill(giveXP);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_storedRoom = null;
                m_smoke.Dispose();
                m_smoke = null;
                m_log.Dispose();
                m_log = null;
                m_closestCeiling = null;
                base.Dispose();
            }
        }
    }
}

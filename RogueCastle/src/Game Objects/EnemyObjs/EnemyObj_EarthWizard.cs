using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_EarthWizard : EnemyObj
    {

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private float SpellDelay = 0.3f;
        private float SpellDuration = 0.75f;
        private int SpellIceProjectileCount = 24;//21;

        private float SpellFireDelay = 1.5f;
        private float SpellFireInterval = 0.2f;

        private Vector2 MiniBossFireballSize = new Vector2(2.0f, 2.0f);
        private Vector2 MiniBossIceSize = new Vector2(1.5f, 1.5f);

        private ProjectileObj m_fireballSummon;
        private ProjectileObj m_iceballSummon;

        private SpriteObj m_earthSummonInSprite;
        private SpriteObj m_earthSummonOutSprite;
        private ProjectileObj m_earthProjectileObj;

        public Vector2 SavedStartingPos { get; set; }
        public RoomObj SpawnRoom;

        private Vector2 m_spellOffset = new Vector2(40, -80);

        private float TeleportDelay = 0.5f;
        private float TeleportDuration = 1.0f;

        private float MoveDuration = 1.0f;
        private float m_earthParticleEffectCounter = 0.5f;
        private int m_effectCycle = 0;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.EarthWizard_Basic_Name;
            LocStringID = EnemyEV.EarthWizard_Basic_Name_locID;

            MaxHealth = EnemyEV.EarthWizard_Basic_MaxHealth;
            Damage = EnemyEV.EarthWizard_Basic_Damage;
            XPValue = EnemyEV.EarthWizard_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.EarthWizard_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.EarthWizard_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.EarthWizard_Basic_DropChance;

            Speed = EnemyEV.EarthWizard_Basic_Speed;
            TurnSpeed = EnemyEV.EarthWizard_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.EarthWizard_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.EarthWizard_Basic_Jump;
            CooldownTime = EnemyEV.EarthWizard_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.EarthWizard_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.EarthWizard_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.EarthWizard_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.EarthWizard_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.EarthWizard_Basic_IsWeighted;

            Scale = EnemyEV.EarthWizard_Basic_Scale;
            ProjectileScale = EnemyEV.EarthWizard_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.EarthWizard_Basic_Tint;

            MeleeRadius = EnemyEV.EarthWizard_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.EarthWizard_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.EarthWizard_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.EarthWizard_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    SpellDelay = 0.85f;
                    SpellDuration = 2.0f;
                    m_spellOffset = new Vector2(40, -140);
                    #region Miniboss Variables - General
                    Name = EnemyEV.EarthWizard_Miniboss_Name;
                    LocStringID = EnemyEV.EarthWizard_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.EarthWizard_Miniboss_MaxHealth;
                    Damage = EnemyEV.EarthWizard_Miniboss_Damage;
                    XPValue = EnemyEV.EarthWizard_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.EarthWizard_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.EarthWizard_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.EarthWizard_Miniboss_DropChance;

                    Speed = EnemyEV.EarthWizard_Miniboss_Speed;
                    TurnSpeed = EnemyEV.EarthWizard_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.EarthWizard_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.EarthWizard_Miniboss_Jump;
                    CooldownTime = EnemyEV.EarthWizard_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.EarthWizard_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.EarthWizard_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.EarthWizard_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.EarthWizard_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.EarthWizard_Miniboss_IsWeighted;

                    Scale = EnemyEV.EarthWizard_Miniboss_Scale;
                    ProjectileScale = EnemyEV.EarthWizard_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.EarthWizard_Miniboss_Tint;

                    MeleeRadius = EnemyEV.EarthWizard_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.EarthWizard_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.EarthWizard_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.EarthWizard_Miniboss_KnockBack;
                    #endregion
                    break;


                case (GameTypes.EnemyDifficulty.EXPERT):
                    SpellDelay = 0.7f;
                    SpellDuration = 3.5f;
					#region Expert Variables - General
					Name = EnemyEV.EarthWizard_Expert_Name;
                    LocStringID = EnemyEV.EarthWizard_Expert_Name_locID;
					
					MaxHealth = EnemyEV.EarthWizard_Expert_MaxHealth;
					Damage = EnemyEV.EarthWizard_Expert_Damage;
					XPValue = EnemyEV.EarthWizard_Expert_XPValue;
					
					MinMoneyDropAmount = EnemyEV.EarthWizard_Expert_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.EarthWizard_Expert_MaxDropAmount;
					MoneyDropChance = EnemyEV.EarthWizard_Expert_DropChance;
					
					Speed = EnemyEV.EarthWizard_Expert_Speed;
					TurnSpeed = EnemyEV.EarthWizard_Expert_TurnSpeed;
					ProjectileSpeed = EnemyEV.EarthWizard_Expert_ProjectileSpeed;
					JumpHeight = EnemyEV.EarthWizard_Expert_Jump;
					CooldownTime = EnemyEV.EarthWizard_Expert_Cooldown;
					AnimationDelay = 1 / EnemyEV.EarthWizard_Expert_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.EarthWizard_Expert_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.EarthWizard_Expert_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.EarthWizard_Expert_CanBeKnockedBack;
					IsWeighted = EnemyEV.EarthWizard_Expert_IsWeighted;
					
					Scale = EnemyEV.EarthWizard_Expert_Scale;
					ProjectileScale = EnemyEV.EarthWizard_Expert_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.EarthWizard_Expert_Tint;
					
					MeleeRadius = EnemyEV.EarthWizard_Expert_MeleeRadius;
					ProjectileRadius = EnemyEV.EarthWizard_Expert_ProjectileRadius;
					EngageRadius = EnemyEV.EarthWizard_Expert_EngageRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.EarthWizard_Expert_KnockBack;
					#endregion

                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    SpellDelay = 0.5f;
                    SpellDuration = 1.0f;

					#region Advanced Variables - General
					Name = EnemyEV.EarthWizard_Advanced_Name;
                    LocStringID = EnemyEV.EarthWizard_Advanced_Name_locID;
					
					MaxHealth = EnemyEV.EarthWizard_Advanced_MaxHealth;
					Damage = EnemyEV.EarthWizard_Advanced_Damage;
					XPValue = EnemyEV.EarthWizard_Advanced_XPValue;
					
					MinMoneyDropAmount = EnemyEV.EarthWizard_Advanced_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.EarthWizard_Advanced_MaxDropAmount;
					MoneyDropChance = EnemyEV.EarthWizard_Advanced_DropChance;
					
					Speed = EnemyEV.EarthWizard_Advanced_Speed;
					TurnSpeed = EnemyEV.EarthWizard_Advanced_TurnSpeed;
					ProjectileSpeed = EnemyEV.EarthWizard_Advanced_ProjectileSpeed;
					JumpHeight = EnemyEV.EarthWizard_Advanced_Jump;
					CooldownTime = EnemyEV.EarthWizard_Advanced_Cooldown;
					AnimationDelay = 1 / EnemyEV.EarthWizard_Advanced_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.EarthWizard_Advanced_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.EarthWizard_Advanced_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.EarthWizard_Advanced_CanBeKnockedBack;
					IsWeighted = EnemyEV.EarthWizard_Advanced_IsWeighted;
					
					Scale = EnemyEV.EarthWizard_Advanced_Scale;
					ProjectileScale = EnemyEV.EarthWizard_Advanced_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.EarthWizard_Advanced_Tint;
					
					MeleeRadius = EnemyEV.EarthWizard_Advanced_MeleeRadius;
					EngageRadius = EnemyEV.EarthWizard_Advanced_EngageRadius;
					ProjectileRadius = EnemyEV.EarthWizard_Advanced_ProjectileRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.EarthWizard_Advanced_KnockBack;
					#endregion

                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }		
        }

        // Special method for the blob miniboss to initialize the enemy's EV.
        public void PublicInitializeEV()
        {
            InitializeEV();
        }

        protected override void InitializeLogic()
        {
            InitializeProjectiles();

            LogicSet moveTowardsLS = new LogicSet(this);
            moveTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardIdle_Character", true, true));
            moveTowardsLS.AddAction(new ChaseLogicAction(m_target, new Vector2(-255, -175), new Vector2(255, -75), true, MoveDuration));

            LogicSet moveAwayLS = new LogicSet(this);
            moveAwayLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardIdle_Character", true, true));
            moveAwayLS.AddAction(new ChaseLogicAction(m_target, false, 1f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet castSpellLS = new LogicSet(this);
            castSpellLS.AddAction(new MoveLogicAction(m_target, true, 0));
            castSpellLS.AddAction(new LockFaceDirectionLogicAction(true));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CancelEarthSpell"));
            castSpellLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardSpell_Character"));
            castSpellLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeSpell"));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CastEarthSpellIn"));
            castSpellLS.AddAction(new DelayLogicAction(0.5f));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CastEarthSpellOut"));
            castSpellLS.AddAction(new DelayLogicAction(SpellDelay));
            castSpellLS.AddAction(new PlayAnimationLogicAction("CastSpell", "End"), Types.Sequence.Parallel);
            castSpellLS.AddAction(new DelayLogicAction(0.2f));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CastEarthSpell", SpellDuration));
            castSpellLS.AddAction(new DelayLogicAction(0.2f));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CancelEarthSpellIn"));
            castSpellLS.AddAction(new DelayLogicAction(0.5f));
            castSpellLS.AddAction(new LockFaceDirectionLogicAction(false));
            castSpellLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet castMiniBossFireSpellLS = new LogicSet(this);
            castMiniBossFireSpellLS.AddAction(new MoveLogicAction(m_target, true, 0));
            castMiniBossFireSpellLS.AddAction(new LockFaceDirectionLogicAction(true));
            castMiniBossFireSpellLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardSpell_Character"));
            castMiniBossFireSpellLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeSpell"));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "SummonFireball", null));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireDelay));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(SpellFireInterval));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "ResetFireball", null));
            castMiniBossFireSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castMiniBossFireSpellLS.AddAction(new DelayLogicAction(0.5f));
            castMiniBossFireSpellLS.AddAction(new LockFaceDirectionLogicAction(false));
            castMiniBossFireSpellLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet CastMiniBossIceSpellLS = new LogicSet(this);
            CastMiniBossIceSpellLS.AddAction(new MoveLogicAction(m_target, true, 0));
            CastMiniBossIceSpellLS.AddAction(new LockFaceDirectionLogicAction(true));
            CastMiniBossIceSpellLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardSpell_Character"));
            CastMiniBossIceSpellLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeSpell"));
            CastMiniBossIceSpellLS.AddAction(new RunFunctionLogicAction(this, "SummonIceball", null));
            CastMiniBossIceSpellLS.AddAction(new DelayLogicAction(SpellDelay));
            CastMiniBossIceSpellLS.AddAction(new RunFunctionLogicAction(this, "ShatterIceball", SpellIceProjectileCount));
            CastMiniBossIceSpellLS.AddAction(new PlayAnimationLogicAction("CastSpell", "End"), Types.Sequence.Parallel);
            CastMiniBossIceSpellLS.AddAction(new DelayLogicAction(0.5f));
            CastMiniBossIceSpellLS.AddAction(new RunFunctionLogicAction(this, "ResetIceball", null));
            CastMiniBossIceSpellLS.AddAction(new DelayLogicAction(0.5f));
            CastMiniBossIceSpellLS.AddAction(new LockFaceDirectionLogicAction(false));
            CastMiniBossIceSpellLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet teleportLS = new LogicSet(this);
            teleportLS.AddAction(new MoveLogicAction(m_target, true, 0));
            teleportLS.AddAction(new LockFaceDirectionLogicAction(true));
            teleportLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 30f));
            teleportLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardTeleportOut_Character", false, false));
            teleportLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeTeleport"));
            teleportLS.AddAction(new DelayLogicAction(TeleportDelay));
            teleportLS.AddAction(new PlayAnimationLogicAction("TeleportStart", "End"));
            teleportLS.AddAction(new ChangePropertyLogicAction(this, "IsCollidable", false));
            teleportLS.AddAction(new DelayLogicAction(TeleportDuration));
            teleportLS.AddAction(new TeleportLogicAction(m_target, new Vector2(-400, -400), new Vector2(400, 400)));
            teleportLS.AddAction(new ChangePropertyLogicAction(this, "IsCollidable", true));
            teleportLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardTeleportIn_Character", true, false));
            teleportLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 10f));
            teleportLS.AddAction(new LockFaceDirectionLogicAction(false));
            teleportLS.AddAction(new DelayLogicAction(0.5f));

            m_generalBasicLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, teleportLS);
            m_generalAdvancedLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, teleportLS);
            m_generalExpertLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, teleportLS);
            m_generalMiniBossLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, castMiniBossFireSpellLS, CastMiniBossIceSpellLS, teleportLS);
            m_generalCooldownLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 100, 0, 0); //moveTowardsLS, moveAwayLS, walkStopLS

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 40, 0, 0, 60, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
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
                    RunLogicBlock(true, m_generalBasicLB, 40, 0, 0, 60, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
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
                    RunLogicBlock(true, m_generalBasicLB, 40, 0, 0, 60, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
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
                    RunLogicBlock(true, m_generalMiniBossLB, 34, 0, 0, 22, 22, 22, 0); // (moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, castMiniBossFireSpellLS, CastMiniBossIceSpellLS, teleportLS);
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalMiniBossLB, 100, 0, 0, 0, 0, 0, 0); //(moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, castMiniBossFireSpellLS, CastMiniBossIceSpellLS, teleportLS);
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalMiniBossLB, 60, 10, 30, 0, 0, 0, 0); // (moveTowardsLS, moveAwayLS, walkStopLS, castSpellLS, castMiniBossFireSpellLS, CastMiniBossIceSpellLS, teleportLS);
                    break;
                default:
                    break;
            }
        }

        public EnemyObj_EarthWizard(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyWizardIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.PlayAnimation(true);
            TintablePart = _objectList[0];
            this.Type = EnemyType.EarthWizard;
        }

        private void InitializeProjectiles()
        {
            m_earthSummonInSprite = new ProjectileObj("WizardEarthSpellCast_Sprite");
            m_earthSummonInSprite.AnimationDelay = 1 / 10f;
            m_earthSummonInSprite.PlayAnimation(true);
            m_earthSummonInSprite.Scale = Vector2.Zero;

            m_earthSummonOutSprite = m_earthSummonInSprite.Clone() as SpriteObj;
            m_earthSummonOutSprite.PlayAnimation(true);

            m_earthProjectileObj = new ProjectileObj("WizardEarthSpell_Sprite");
            m_earthProjectileObj.IsWeighted = false;
            m_earthProjectileObj.CollidesWithTerrain = false;
            m_earthProjectileObj.DestroysWithEnemy = false;
            m_earthProjectileObj.Damage = Damage;
            m_earthProjectileObj.Scale = ProjectileScale;
            m_earthProjectileObj.AnimationDelay = 1 / 20f;
            m_earthProjectileObj.Rotation = 0;
            m_earthProjectileObj.CanBeFusRohDahed = false;
        }

        public void CancelEarthSpell()
        {
            Tweener.Tween.StopAllContaining(m_earthSummonOutSprite, false);
            Tweener.Tween.StopAllContaining(this, false);
            Tweener.Tween.To(m_earthSummonOutSprite, 0.5f, Tweener.Ease.Linear.EaseNone, "ScaleX", "0", "ScaleY", "0");
            if (m_earthProjectileObj.CurrentFrame != 1 && m_earthProjectileObj.CurrentFrame != m_earthProjectileObj.TotalFrames)
            {
                SoundManager.Play3DSound(this, m_target, "Earth_Wizard_Fall");
                m_earthProjectileObj.PlayAnimation("Grown", "End");
            }
            m_levelScreen.PhysicsManager.RemoveObject(m_earthProjectileObj);
        }

        public void CancelEarthSpellIn()
        {
            Tweener.Tween.StopAllContaining(m_earthSummonInSprite, false);
            Tweener.Tween.To(m_earthSummonInSprite, 0.5f, Tweener.Ease.Linear.EaseNone, "ScaleX", "0", "ScaleY", "0");
        }

        public void CastEarthSpellIn()
        {
            SoundManager.Play3DSound(this, m_target, "Earth_Wizard_Form");
            m_earthSummonInSprite.Scale = Vector2.Zero;
            Tweener.Tween.To(m_earthSummonInSprite, 0.5f, Tweener.Ease.Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
        }

        public void CastEarthSpellOut()
        {
            //SoundManager.PlaySound("Earth_Wizard_Form");
            m_earthSummonOutSprite.Scale = Vector2.Zero;
            m_earthSummonOutSprite.X = m_target.X;

            int closestDistance = int.MaxValue;
            TerrainObj closestObj = null;
            foreach (TerrainObj obj in m_levelScreen.CurrentRoom.TerrainObjList)
            {
                if (CollisionMath.Intersects(new Rectangle((int)m_target.X, (int)m_target.Y, 2, 720), obj.Bounds))
                {
                    int distanceBetween = obj.Bounds.Top - m_target.TerrainBounds.Bottom;
                    if (distanceBetween < closestDistance)
                    {
                        closestDistance = distanceBetween;
                        closestObj = obj;
                    }
                }
            }

            if (closestObj != null)
            {
                if (closestObj.Rotation == 0)
                    m_earthSummonOutSprite.Y = closestObj.Bounds.Top;
                else
                {
                    float y1 = float.MaxValue;
                    Vector2 pt1, pt2;
                    if (closestObj.Width > closestObj.Height) // If rotated objects are done correctly.
                    {
                        pt1 = CollisionMath.UpperLeftCorner(closestObj.TerrainBounds, closestObj.Rotation, Vector2.Zero);
                        pt2 = CollisionMath.UpperRightCorner(closestObj.TerrainBounds, closestObj.Rotation, Vector2.Zero);
                    }
                    else // If rotated objects are done Teddy's incorrect way.
                    {
                        //if (closestObj.Rotation == 45)
                        if (closestObj.Rotation > 0) // ROTCHECK
                        {
                            pt1 = CollisionMath.LowerLeftCorner(closestObj.TerrainBounds, closestObj.Rotation, Vector2.Zero);
                            pt2 = CollisionMath.UpperLeftCorner(closestObj.TerrainBounds, closestObj.Rotation, Vector2.Zero);
                        }
                        else
                        {
                            pt1 = CollisionMath.UpperRightCorner(closestObj.TerrainBounds, closestObj.Rotation, Vector2.Zero);
                            pt2 = CollisionMath.LowerRightCorner(closestObj.TerrainBounds, closestObj.Rotation, Vector2.Zero);
                        }
                    }

                    //if (this.X > pt1.X + 10 && this.X < pt2.X - 10)
                    {
                        float u = pt2.X - pt1.X;
                        float v = pt2.Y - pt1.Y;
                        float x = pt1.X;
                        float y = pt1.Y;
                        float x1 = m_earthSummonOutSprite.X;

                        y1 = y + (x1 - x) * (v / u);
                        y1 -= (m_earthSummonOutSprite.Bounds.Bottom - m_earthSummonOutSprite.Y);
                        m_earthSummonOutSprite.Y = (float)Math.Round(y1, MidpointRounding.ToEven);
                    }
                }
            }

            Tweener.Tween.To(m_earthSummonOutSprite, 0.5f, Tweener.Ease.Back.EaseOut, "Opacity", "1", "ScaleX", ProjectileScale.X.ToString(), "ScaleY", "1");
        }

        public void CastEarthSpell(float duration)
        {
            m_levelScreen.PhysicsManager.AddObject(m_earthProjectileObj);
            m_earthProjectileObj.Scale = ProjectileScale;
            m_earthProjectileObj.StopAnimation();
            m_earthProjectileObj.Position = m_earthSummonOutSprite.Position;
            m_earthProjectileObj.PlayAnimation("Start", "Grown");
            SoundManager.Play3DSound(this, m_target, "Earth_Wizard_Attack");

            Tweener.Tween.RunFunction(duration, this, "CancelEarthSpell");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //if (m_earthSummonIn != null)
            {
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    m_earthSummonInSprite.Position = new Vector2(this.X + m_spellOffset.X, this.Y + m_spellOffset.Y);
                else
                    m_earthSummonInSprite.Position = new Vector2(this.X - m_spellOffset.X, this.Y + m_spellOffset.Y);
            }

            if (m_fireballSummon != null)
            {
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    m_fireballSummon.Position = new Vector2(this.X + m_spellOffset.X, this.Y + m_spellOffset.Y);
                else
                    m_fireballSummon.Position = new Vector2(this.X - m_spellOffset.X, this.Y + m_spellOffset.Y);
            }

            if (m_iceballSummon != null)
            {
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    m_iceballSummon.Position = new Vector2(this.X + m_spellOffset.X, this.Y + m_spellOffset.Y);
                else
                    m_iceballSummon.Position = new Vector2(this.X - m_spellOffset.X, this.Y + m_spellOffset.Y);
            }


            if (m_earthParticleEffectCounter > 0)
            {
                m_earthParticleEffectCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_earthParticleEffectCounter <= 0)
                {
                    if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                    {
                        if (m_effectCycle == 0)
                            m_levelScreen.ImpactEffectPool.DisplayEarthParticleEffect(this);
                        else if (m_effectCycle == 1)
                            m_levelScreen.ImpactEffectPool.DisplayFireParticleEffect(this);
                        else
                            m_levelScreen.ImpactEffectPool.DisplayIceParticleEffect(this);
                        m_effectCycle++;
                        if (m_effectCycle > 2)
                            m_effectCycle = 0;
                    }
                    else
                        m_levelScreen.ImpactEffectPool.DisplayEarthParticleEffect(this);
                    m_earthParticleEffectCounter = 0.15f;
                }
            }
        }

        #region Fireball Spell Stuff

        public void CastFireball()
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "WizardFireballProjectile_Sprite",
                SourceAnchor = m_spellOffset,
                Target = m_target,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = MiniBossFireballSize,
            };

            if (this.Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                projData.AngleOffset = CDGMath.RandomInt(-25, 25);

            if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                projData.SpriteName = "GhostBossProjectile_Sprite";

            SoundManager.Play3DSound(this, m_target, "FireWizard_Attack_01", "FireWizard_Attack_02", "FireWizard_Attack_03", "FireWizard_Attack_04");
            ProjectileObj fireball = m_levelScreen.ProjectileManager.FireProjectile(projData);
            fireball.Rotation = 0;
            Tweener.Tween.RunFunction(0.15f, this, "ChangeFireballState", fireball);
        }

        public void ChangeFireballState(ProjectileObj fireball)
        {
            fireball.CollidesWithTerrain = true;
        }

        public void SummonFireball()
        {
            ResetFireball();

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "WizardFireballProjectile_Sprite",
                SourceAnchor = m_spellOffset,
                Target = m_target,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = MiniBossFireballSize,
            };

            if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                projData.SpriteName = "GhostBossProjectile_Sprite";

            SoundManager.Play3DSound(this, m_target, "Fire_Wizard_Form");
            //m_fireballSummon = m_levelScreen.ProjectileManager.FireProjectile("WizardFireballProjectile_Sprite", this, m_spellOffset, 0, 0, false, 0, ProjectileDamage);
            m_fireballSummon = m_levelScreen.ProjectileManager.FireProjectile(projData);
            m_fireballSummon.Opacity = 0;
            m_fireballSummon.Scale = Vector2.Zero;
            m_fireballSummon.AnimationDelay = 1 / 10f;
            m_fireballSummon.PlayAnimation(true);
            m_fireballSummon.Rotation = 0;

            Tweener.Tween.To(m_fireballSummon, 0.5f, Tweener.Ease.Back.EaseOut, "Opacity", "1", "ScaleX", MiniBossFireballSize.X.ToString(), "ScaleY", MiniBossFireballSize.Y.ToString());

            projData.Dispose();
        }

        public void ResetFireball()
        {
            if (m_fireballSummon != null)
            {
                m_levelScreen.ProjectileManager.DestroyProjectile(m_fireballSummon);
                m_fireballSummon = null;
            }
        }
        #endregion


        #region Ice Spell Stuff
        public void SummonIceball()
        {
            ResetIceball();

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "WizardIceSpell_Sprite",
                SourceAnchor = m_spellOffset,
                Target = null,
                Speed = new Vector2(0, 0),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = MiniBossIceSize,
            };

            SoundManager.Play3DSound(this, m_target, "Ice_Wizard_Form");
            //m_iceballSummon = m_levelScreen.ProjectileManager.FireProjectile("WizardIceSpell_Sprite", this, m_spellOffset, 0, 0, false, 0, ProjectileDamage);
            m_iceballSummon = m_levelScreen.ProjectileManager.FireProjectile(projData);
            m_iceballSummon.PlayAnimation("Start", "Grown");

            projData.Dispose();
        }

        public void ShatterIceball(int numIceballs)
        {
            SoundManager.Play3DSound(this, m_target, "Ice_Wizard_Attack_Glass");

            if (m_iceballSummon.SpriteName == "WizardIceSpell_Sprite") // Temporary hack fix for crashing iceballs.
                m_iceballSummon.PlayAnimation("Grown", "End");

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "WizardIceProjectile_Sprite",
                SourceAnchor = m_spellOffset,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = MiniBossIceSize,
            };

            float angle = 0;
            float angleDiff = 360 / numIceballs;

            for (int i = 0; i < numIceballs; i++)
            {
                projData.Angle = new Vector2(angle, angle);
                ProjectileObj iceball = m_levelScreen.ProjectileManager.FireProjectile(projData);
                Tweener.Tween.RunFunction(0.15f, this, "ChangeIceballState", iceball);
                angle += angleDiff;
            }

            projData.Dispose();
        }

        public void ChangeIceballState(ProjectileObj iceball)
        {
            iceball.CollidesWithTerrain = true;
        }

        public void ResetIceball()
        {
            if (m_iceballSummon != null)
            {
                m_levelScreen.ProjectileManager.DestroyProjectile(m_iceballSummon);
                m_iceballSummon = null;
            }
        }
        #endregion

        //public override void HitEnemy(int damage, Vector2 position, bool isPlayer = true)
        //{
        //    base.HitEnemy(damage, position, isPlayer);
        //    if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
        //    {
        //        m_currentActiveLB.StopLogicBlock();
        //        DestroyEarthSpell();

        //        //Ensures he doesn't cast a spell immediately after getting hit (since the destroy spell animation is still running).
        //        RunLogicBlock(false, m_generalBasicLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, castSpellLS
        //    }
        //}

        public override void Kill(bool giveXP = true)
        {
            if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
            {
                CancelEarthSpell();
                CancelEarthSpellIn();

                m_currentActiveLB.StopLogicBlock();

                //Ensures he doesn't cast a spell immediately after getting hit (since the destroy spell animation is still running).
                //RunLogicBlock(false, m_generalBasicLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, castSpellLS
            }
            base.Kill(giveXP);
        }

        public override void Draw(Camera2D camera)
        {
            m_earthSummonInSprite.Draw(camera);
            m_earthSummonOutSprite.Draw(camera);
            m_earthProjectileObj.Draw(camera);
            base.Draw(camera);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (otherBox.AbsParent is PlayerObj)
                this.CurrentSpeed = 0;
            if (collisionResponseType != Consts.COLLISIONRESPONSE_TERRAIN)
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
            else if ((otherBox.AbsParent is PlayerObj) == false)// Add this else to turn on terrain collision.
            {
                IPhysicsObj otherPhysicsObj = otherBox.AbsParent as IPhysicsObj;
                if (otherPhysicsObj.CollidesBottom == true && otherPhysicsObj.CollidesTop == true && otherPhysicsObj.CollidesLeft == true && otherPhysicsObj.CollidesRight == true)
                    this.Position += CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
            }
        }

        public override void ResetState()
        {
            Tweener.Tween.StopAllContaining(this, false);
            Tweener.Tween.StopAllContaining(m_earthSummonOutSprite, false);
            Tweener.Tween.StopAllContaining(m_earthSummonInSprite, false);
            m_earthSummonInSprite.Scale = Vector2.Zero;
            m_earthSummonOutSprite.Scale = Vector2.Zero;
            m_earthProjectileObj.StopAnimation();
            m_earthProjectileObj.GoToFrame(m_earthProjectileObj.TotalFrames);

            ResetFireball();
            ResetIceball();
            base.ResetState();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_fireballSummon = null;
                m_iceballSummon = null;

                if (m_earthSummonInSprite != null)
                {
                    m_earthSummonInSprite.Dispose();
                    m_earthSummonInSprite = null;
                }

                if (m_earthSummonOutSprite != null)
                {
                    m_earthSummonOutSprite.Dispose();
                    m_earthSummonOutSprite = null;
                }

                if (m_earthProjectileObj != null)
                {
                    m_earthProjectileObj.Dispose();
                    m_earthProjectileObj = null;
                }

                SpawnRoom = null;
                base.Dispose();
            }
        }

        public SpriteObj EarthProjectile
        {
            get { return m_earthProjectileObj; }
        }
    }
}

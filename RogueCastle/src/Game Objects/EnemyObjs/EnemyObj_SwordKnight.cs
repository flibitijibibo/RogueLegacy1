using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_SwordKnight: EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private float SlashDelay = 0.0f;
        private float SlashTripleDelay = 1.25f;
        private float TripleAttackSpeed = 500f;
        private FrameSoundObj m_walkSound, m_walkSound2;

        protected override void InitializeEV()
        {
            SlashDelay = 0.25f;
            #region Basic Variables - General
            Name = EnemyEV.SwordKnight_Basic_Name;
            LocStringID = EnemyEV.SwordKnight_Basic_Name_locID;

            MaxHealth = EnemyEV.SwordKnight_Basic_MaxHealth;
            Damage = EnemyEV.SwordKnight_Basic_Damage;
            XPValue = EnemyEV.SwordKnight_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.SwordKnight_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.SwordKnight_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.SwordKnight_Basic_DropChance;

            Speed = EnemyEV.SwordKnight_Basic_Speed;
            TurnSpeed = EnemyEV.SwordKnight_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.SwordKnight_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.SwordKnight_Basic_Jump;
            CooldownTime = EnemyEV.SwordKnight_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.SwordKnight_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.SwordKnight_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.SwordKnight_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.SwordKnight_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.SwordKnight_Basic_IsWeighted;

            Scale = EnemyEV.SwordKnight_Basic_Scale;
            ProjectileScale = EnemyEV.SwordKnight_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.SwordKnight_Basic_Tint;

            MeleeRadius = EnemyEV.SwordKnight_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.SwordKnight_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.SwordKnight_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.SwordKnight_Basic_KnockBack;
            #endregion


            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    ForceDraw = true;
                    SlashDelay = 1.05f;
					#region Miniboss Variables - General
					Name = EnemyEV.SwordKnight_Miniboss_Name;
                    LocStringID = EnemyEV.SwordKnight_Miniboss_Name_locID;
					
					MaxHealth = EnemyEV.SwordKnight_Miniboss_MaxHealth;
					Damage = EnemyEV.SwordKnight_Miniboss_Damage;
					XPValue = EnemyEV.SwordKnight_Miniboss_XPValue;
					
					MinMoneyDropAmount = EnemyEV.SwordKnight_Miniboss_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.SwordKnight_Miniboss_MaxDropAmount;
					MoneyDropChance = EnemyEV.SwordKnight_Miniboss_DropChance;
					
					Speed = EnemyEV.SwordKnight_Miniboss_Speed;
					TurnSpeed = EnemyEV.SwordKnight_Miniboss_TurnSpeed;
					ProjectileSpeed = EnemyEV.SwordKnight_Miniboss_ProjectileSpeed;
					JumpHeight = EnemyEV.SwordKnight_Miniboss_Jump;
					CooldownTime = EnemyEV.SwordKnight_Miniboss_Cooldown;
					AnimationDelay = 1 / EnemyEV.SwordKnight_Miniboss_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.SwordKnight_Miniboss_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.SwordKnight_Miniboss_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.SwordKnight_Miniboss_CanBeKnockedBack;
					IsWeighted = EnemyEV.SwordKnight_Miniboss_IsWeighted;
					
					Scale = EnemyEV.SwordKnight_Miniboss_Scale;
					ProjectileScale = EnemyEV.SwordKnight_Miniboss_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.SwordKnight_Miniboss_Tint;
					
					MeleeRadius = EnemyEV.SwordKnight_Miniboss_MeleeRadius;
					ProjectileRadius = EnemyEV.SwordKnight_Miniboss_ProjectileRadius;
					EngageRadius = EnemyEV.SwordKnight_Miniboss_EngageRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.SwordKnight_Miniboss_KnockBack;
					#endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    SlashDelay = 0.25f;
                    TripleAttackSpeed = 500f;
					#region Expert Variables - General
					Name = EnemyEV.SwordKnight_Expert_Name;
                    LocStringID = EnemyEV.SwordKnight_Expert_Name_locID;
					
					MaxHealth = EnemyEV.SwordKnight_Expert_MaxHealth;
					Damage = EnemyEV.SwordKnight_Expert_Damage;
					XPValue = EnemyEV.SwordKnight_Expert_XPValue;
					
					MinMoneyDropAmount = EnemyEV.SwordKnight_Expert_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.SwordKnight_Expert_MaxDropAmount;
					MoneyDropChance = EnemyEV.SwordKnight_Expert_DropChance;
					
					Speed = EnemyEV.SwordKnight_Expert_Speed;
					TurnSpeed = EnemyEV.SwordKnight_Expert_TurnSpeed;
					ProjectileSpeed = EnemyEV.SwordKnight_Expert_ProjectileSpeed;
					JumpHeight = EnemyEV.SwordKnight_Expert_Jump;
					CooldownTime = EnemyEV.SwordKnight_Expert_Cooldown;
					AnimationDelay = 1 / EnemyEV.SwordKnight_Expert_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.SwordKnight_Expert_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.SwordKnight_Expert_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.SwordKnight_Expert_CanBeKnockedBack;
					IsWeighted = EnemyEV.SwordKnight_Expert_IsWeighted;
					
					Scale = EnemyEV.SwordKnight_Expert_Scale;
					ProjectileScale = EnemyEV.SwordKnight_Expert_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.SwordKnight_Expert_Tint;
					
					MeleeRadius = EnemyEV.SwordKnight_Expert_MeleeRadius;
					ProjectileRadius = EnemyEV.SwordKnight_Expert_ProjectileRadius;
					EngageRadius = EnemyEV.SwordKnight_Expert_EngageRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.SwordKnight_Expert_KnockBack;
					#endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    SlashDelay = 0.25f;
					#region Advanced Variables - General
					Name = EnemyEV.SwordKnight_Advanced_Name;
                    LocStringID = EnemyEV.SwordKnight_Advanced_Name_locID;
					
					MaxHealth = EnemyEV.SwordKnight_Advanced_MaxHealth;
					Damage = EnemyEV.SwordKnight_Advanced_Damage;
					XPValue = EnemyEV.SwordKnight_Advanced_XPValue;
					
					MinMoneyDropAmount = EnemyEV.SwordKnight_Advanced_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.SwordKnight_Advanced_MaxDropAmount;
					MoneyDropChance = EnemyEV.SwordKnight_Advanced_DropChance;
					
					Speed = EnemyEV.SwordKnight_Advanced_Speed;
					TurnSpeed = EnemyEV.SwordKnight_Advanced_TurnSpeed;
					ProjectileSpeed = EnemyEV.SwordKnight_Advanced_ProjectileSpeed;
					JumpHeight = EnemyEV.SwordKnight_Advanced_Jump;
					CooldownTime = EnemyEV.SwordKnight_Advanced_Cooldown;
					AnimationDelay = 1 / EnemyEV.SwordKnight_Advanced_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.SwordKnight_Advanced_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.SwordKnight_Advanced_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.SwordKnight_Advanced_CanBeKnockedBack;
					IsWeighted = EnemyEV.SwordKnight_Advanced_IsWeighted;
					
					Scale = EnemyEV.SwordKnight_Advanced_Scale;
					ProjectileScale = EnemyEV.SwordKnight_Advanced_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.SwordKnight_Advanced_Tint;
					
					MeleeRadius = EnemyEV.SwordKnight_Advanced_MeleeRadius;
					EngageRadius = EnemyEV.SwordKnight_Advanced_EngageRadius;
					ProjectileRadius = EnemyEV.SwordKnight_Advanced_ProjectileRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.SwordKnight_Advanced_KnockBack;
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
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightWalk_Character", true, true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(1));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightWalk_Character", true, true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(1));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet attackLS = new LogicSet(this);
            attackLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackLS.AddAction(new DelayLogicAction(SlashDelay));
            attackLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SwordKnight_Attack_v02"));
            attackLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            attackLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightIdle_Character", false, false));
            attackLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet attackTripleLS = new LogicSet(this);
            attackTripleLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackTripleLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackTripleLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackTripleLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackTripleLS.AddAction(new DelayLogicAction(SlashTripleDelay));
            attackTripleLS.AddAction(new MoveDirectionLogicAction(TripleAttackSpeed));
            attackTripleLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 60f));
            attackTripleLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SwordKnight_Attack_v02"));
            attackTripleLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));


            //attackTripleLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackTripleLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackTripleLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SwordKnight_Attack_v02"));
            attackTripleLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));

            //attackTripleLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            //attackTripleLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            //attackTripleLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SwordKnight_Attack_v02"));
            //attackTripleLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));

            attackTripleLS.AddAction(new MoveLogicAction(null, true, 0));
            attackTripleLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / EnemyEV.SwordKnight_Advanced_AnimationDelay));
            attackTripleLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightIdle_Character", false, false));
            attackTripleLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackTripleLS.Tag = GameTypes.LogicSetType_ATTACK;

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "EnemySpearKnightWave_Sprite",
                SourceAnchor = new Vector2(60, 0),
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),                
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = true,
                Scale = ProjectileScale,
            };

            LogicSet attackAdvancedLS = new LogicSet(this);
            attackAdvancedLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackAdvancedLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackAdvancedLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackAdvancedLS.AddAction(new DelayLogicAction(SlashDelay));
            attackAdvancedLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            attackAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            attackAdvancedLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            attackAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightIdle_Character", false, false));
            attackAdvancedLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackAdvancedLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet attackTripleExpertLS = new LogicSet(this);
            attackTripleExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackTripleExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackTripleExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackTripleExpertLS.AddAction(new DelayLogicAction(SlashTripleDelay));
            attackTripleExpertLS.AddAction(new MoveDirectionLogicAction(TripleAttackSpeed));
            attackTripleExpertLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 120f));
            attackTripleExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SwordKnight_Attack_v02"));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));


            attackTripleExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackTripleExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SwordKnight_Attack_v02"));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));

            attackTripleExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackTripleExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SwordKnight_Attack_v02"));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));

            attackTripleExpertLS.AddAction(new MoveLogicAction(null, true, 0));
            attackTripleExpertLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / EnemyEV.SwordKnight_Advanced_AnimationDelay));
            attackTripleExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "SpearKnightAttack1"));
            attackTripleExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            attackTripleExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            attackTripleExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightIdle_Character", false, false));
            attackTripleExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackTripleExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet attackExpertLS = new LogicSet(this);
            attackExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            attackExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            attackExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightAttack_Character", false, false));
            attackExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Windup", false));
            attackExpertLS.AddAction(new DelayLogicAction(SlashDelay));
            attackExpertLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"SpearKnightAttack1"));
            attackExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            attackExpertLS.AddAction(new PlayAnimationLogicAction("Attack", "End", false));
            //
            attackExpertLS.AddAction(new ChangeSpriteLogicAction("EnemySwordKnightIdle_Character", false, false));
            attackExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            attackExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackLS, attackTripleLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, attackLS, attackTripleExpertLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            //SetCooldownLogicBlock(m_generalCooldownLB, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS
            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.BASIC):
                     SetCooldownLogicBlock(m_generalCooldownLB, 14, 11, 75); //walkTowardsLS, walkAwayLS, walkStopLS
                    break;
                case (GameTypes.EnemyDifficulty.ADVANCED):
                case (GameTypes.EnemyDifficulty.EXPERT):
                    SetCooldownLogicBlock(m_generalCooldownLB, 40, 30, 30); //walkTowardsLS, walkAwayLS, walkStopLS
                    break;
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    SetCooldownLogicBlock(m_generalCooldownLB, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS
                    break;
                default:
                    break;
            }
           

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 15, 15, 70, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
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
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 0, 65, 35); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 60, 20, 20, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 100, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
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
                    RunLogicBlock(true, m_generalExpertLB, 0, 0, 0, 62, 38); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 60, 20, 20, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
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
                    //RunLogicBlock(true, m_generalBasicLB, 0, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0, 0); //walkTowardsLS, walkAwayLS, walkStopLS, attackLS
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (this.SpriteName == "EnemySwordKnightWalk_Character")
            {
                m_walkSound.Update();
                m_walkSound2.Update();
            }
            base.Update(gameTime);
        }

        public EnemyObj_SwordKnight(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemySwordKnightIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.SwordKnight;
            m_walkSound = new FrameSoundObj(this, m_target, 1, "KnightWalk1", "KnightWalk2");
            m_walkSound2 = new FrameSoundObj(this, m_target, 6, "KnightWalk1", "KnightWalk2");
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Knight_Hit01", "Knight_Hit02", "Knight_Hit03");
            base.HitEnemy(damage, position, isPlayer);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_walkSound.Dispose();
                m_walkSound = null;
                m_walkSound2.Dispose();
                m_walkSound2 = null;
                base.Dispose();
            }
        }
    }
}

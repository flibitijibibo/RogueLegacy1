using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_FireWizard : EnemyObj
    {

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private float SpellDelay = 0.7f;
        private float SpellInterval = 0.5f;

        private ProjectileObj m_fireballSummon;
        private Vector2 m_spellOffset = new Vector2(40, -80);
        private float TeleportDelay = 0.5f;
        private float TeleportDuration = 1.0f;
        private float MoveDuration = 1.0f;
        private float m_fireParticleEffectCounter = 0.5f;

        protected override void InitializeEV()
        {
            SpellInterval = 0.5f;

            #region Basic Variables - General
            Name = EnemyEV.FireWizard_Basic_Name;
            LocStringID = EnemyEV.FireWizard_Basic_Name_locID;

            MaxHealth = EnemyEV.FireWizard_Basic_MaxHealth;
            Damage = EnemyEV.FireWizard_Basic_Damage;
            XPValue = EnemyEV.FireWizard_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.FireWizard_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.FireWizard_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.FireWizard_Basic_DropChance;

            Speed = EnemyEV.FireWizard_Basic_Speed;
            TurnSpeed = EnemyEV.FireWizard_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.FireWizard_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.FireWizard_Basic_Jump;
            CooldownTime = EnemyEV.FireWizard_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.FireWizard_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.FireWizard_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.FireWizard_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.FireWizard_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.FireWizard_Basic_IsWeighted;

            Scale = EnemyEV.FireWizard_Basic_Scale;
            ProjectileScale = EnemyEV.FireWizard_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.FireWizard_Basic_Tint;

            MeleeRadius = EnemyEV.FireWizard_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.FireWizard_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.FireWizard_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.FireWizard_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.FireWizard_Miniboss_Name;
                    LocStringID = EnemyEV.FireWizard_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.FireWizard_Miniboss_MaxHealth;
                    Damage = EnemyEV.FireWizard_Miniboss_Damage;
                    XPValue = EnemyEV.FireWizard_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.FireWizard_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.FireWizard_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.FireWizard_Miniboss_DropChance;

                    Speed = EnemyEV.FireWizard_Miniboss_Speed;
                    TurnSpeed = EnemyEV.FireWizard_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.FireWizard_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.FireWizard_Miniboss_Jump;
                    CooldownTime = EnemyEV.FireWizard_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.FireWizard_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.FireWizard_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.FireWizard_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.FireWizard_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.FireWizard_Miniboss_IsWeighted;

                    Scale = EnemyEV.FireWizard_Miniboss_Scale;
                    ProjectileScale = EnemyEV.FireWizard_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.FireWizard_Miniboss_Tint;

                    MeleeRadius = EnemyEV.FireWizard_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.FireWizard_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.FireWizard_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.FireWizard_Miniboss_KnockBack;
                    #endregion
                    break;


                case (GameTypes.EnemyDifficulty.EXPERT):
                    m_spellOffset = new Vector2(40, -130);
                    SpellDelay = 1.0f;
                    SpellInterval = 1.0f;//0.5f;

					#region Expert Variables - General
					Name = EnemyEV.FireWizard_Expert_Name;
                    LocStringID = EnemyEV.FireWizard_Expert_Name_locID;
					
					MaxHealth = EnemyEV.FireWizard_Expert_MaxHealth;
					Damage = EnemyEV.FireWizard_Expert_Damage;
					XPValue = EnemyEV.FireWizard_Expert_XPValue;
					
					MinMoneyDropAmount = EnemyEV.FireWizard_Expert_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.FireWizard_Expert_MaxDropAmount;
					MoneyDropChance = EnemyEV.FireWizard_Expert_DropChance;
					
					Speed = EnemyEV.FireWizard_Expert_Speed;
					TurnSpeed = EnemyEV.FireWizard_Expert_TurnSpeed;
					ProjectileSpeed = EnemyEV.FireWizard_Expert_ProjectileSpeed;
					JumpHeight = EnemyEV.FireWizard_Expert_Jump;
					CooldownTime = EnemyEV.FireWizard_Expert_Cooldown;
					AnimationDelay = 1 / EnemyEV.FireWizard_Expert_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.FireWizard_Expert_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.FireWizard_Expert_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.FireWizard_Expert_CanBeKnockedBack;
					IsWeighted = EnemyEV.FireWizard_Expert_IsWeighted;
					
					Scale = EnemyEV.FireWizard_Expert_Scale;
					ProjectileScale = EnemyEV.FireWizard_Expert_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.FireWizard_Expert_Tint;
					
					MeleeRadius = EnemyEV.FireWizard_Expert_MeleeRadius;
					ProjectileRadius = EnemyEV.FireWizard_Expert_ProjectileRadius;
					EngageRadius = EnemyEV.FireWizard_Expert_EngageRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.FireWizard_Expert_KnockBack;
					#endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    SpellInterval = 0.15f;

					#region Advanced Variables - General
					Name = EnemyEV.FireWizard_Advanced_Name;
                    LocStringID = EnemyEV.FireWizard_Advanced_Name_locID;
					
					MaxHealth = EnemyEV.FireWizard_Advanced_MaxHealth;
					Damage = EnemyEV.FireWizard_Advanced_Damage;
					XPValue = EnemyEV.FireWizard_Advanced_XPValue;
					
					MinMoneyDropAmount = EnemyEV.FireWizard_Advanced_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.FireWizard_Advanced_MaxDropAmount;
					MoneyDropChance = EnemyEV.FireWizard_Advanced_DropChance;
					
					Speed = EnemyEV.FireWizard_Advanced_Speed;
					TurnSpeed = EnemyEV.FireWizard_Advanced_TurnSpeed;
					ProjectileSpeed = EnemyEV.FireWizard_Advanced_ProjectileSpeed;
					JumpHeight = EnemyEV.FireWizard_Advanced_Jump;
					CooldownTime = EnemyEV.FireWizard_Advanced_Cooldown;
					AnimationDelay = 1 / EnemyEV.FireWizard_Advanced_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.FireWizard_Advanced_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.FireWizard_Advanced_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.FireWizard_Advanced_CanBeKnockedBack;
					IsWeighted = EnemyEV.FireWizard_Advanced_IsWeighted;
					
					Scale = EnemyEV.FireWizard_Advanced_Scale;
					ProjectileScale = EnemyEV.FireWizard_Advanced_ProjectileScale;
					TintablePart.TextureColor = EnemyEV.FireWizard_Advanced_Tint;
					
					MeleeRadius = EnemyEV.FireWizard_Advanced_MeleeRadius;
					EngageRadius = EnemyEV.FireWizard_Advanced_EngageRadius;
					ProjectileRadius = EnemyEV.FireWizard_Advanced_ProjectileRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.FireWizard_Advanced_KnockBack;
					#endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }		

        }

        protected override void InitializeLogic()
        {
            LogicSet moveTowardsLS = new LogicSet(this);
            moveTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardIdle_Character", true, true));
            moveTowardsLS.AddAction(new ChaseLogicAction(m_target, new Vector2(-255, -175), new Vector2(255, -75), true, MoveDuration));

            LogicSet moveAwayLS = new LogicSet(this);
            moveAwayLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardIdle_Character", true, true));
            moveAwayLS.AddAction(new ChaseLogicAction(m_target, false, MoveDuration));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet castSpellLS = new LogicSet(this);
            castSpellLS.AddAction(new MoveLogicAction(m_target, true, 0));
            castSpellLS.AddAction(new LockFaceDirectionLogicAction(true));
            castSpellLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardSpell_Character"));
            castSpellLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeSpell"));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "SummonFireball", null));
            castSpellLS.AddAction(new DelayLogicAction(SpellDelay));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "ResetFireball", null));
            castSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castSpellLS.AddAction(new DelayLogicAction(0.5f));
            castSpellLS.AddAction(new LockFaceDirectionLogicAction(false));
            castSpellLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet castAdvancedSpellLS = new LogicSet(this);
            castAdvancedSpellLS.AddAction(new MoveLogicAction(m_target, true, 0));
            castAdvancedSpellLS.AddAction(new LockFaceDirectionLogicAction(true));
            castAdvancedSpellLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardSpell_Character"));
            castAdvancedSpellLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeSpell"));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "SummonFireball", null));
            castAdvancedSpellLS.AddAction(new DelayLogicAction(SpellDelay));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castAdvancedSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castAdvancedSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castAdvancedSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castAdvancedSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "ResetFireball", null));
            castAdvancedSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castAdvancedSpellLS.AddAction(new DelayLogicAction(0.5f));
            castAdvancedSpellLS.AddAction(new LockFaceDirectionLogicAction(false));
            castAdvancedSpellLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet castExpertSpellLS = new LogicSet(this);
            castExpertSpellLS.AddAction(new MoveLogicAction(m_target, true, 0));
            castExpertSpellLS.AddAction(new LockFaceDirectionLogicAction(true));
            castExpertSpellLS.AddAction(new ChangeSpriteLogicAction("EnemyWizardSpell_Character"));
            castExpertSpellLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeSpell"));
            castExpertSpellLS.AddAction(new RunFunctionLogicAction(this, "SummonFireball", null));
            castExpertSpellLS.AddAction(new DelayLogicAction(SpellDelay));
            castExpertSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castExpertSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castExpertSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));
            castExpertSpellLS.AddAction(new DelayLogicAction(SpellInterval));
            castExpertSpellLS.AddAction(new RunFunctionLogicAction(this, "ResetFireball", null));
            castExpertSpellLS.AddAction(new RunFunctionLogicAction(this, "CastFireball"));            
            castExpertSpellLS.AddAction(new DelayLogicAction(0.5f));
            castExpertSpellLS.AddAction(new LockFaceDirectionLogicAction(false));
            castExpertSpellLS.Tag = GameTypes.LogicSetType_ATTACK;

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
            m_generalAdvancedLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS, castAdvancedSpellLS, teleportLS);
            m_generalExpertLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS, castExpertSpellLS, teleportLS);
            m_generalCooldownLB.AddLogicSet(moveTowardsLS, moveAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
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
                    RunLogicBlock(true, m_generalAdvancedLB, 40, 0, 0, 60, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalAdvancedLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalAdvancedLB, 0, 0, 100, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
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
                    RunLogicBlock(true, m_generalExpertLB, 40, 0, 0, 60, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalExpertLB, 100, 0, 0, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalExpertLB, 0, 0, 100, 0, 0); // moveTowardsLS, moveAwayLS, walkStopLS castSpellLS, teleportLS
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
                default:
                    break;
            }
        }

        public EnemyObj_FireWizard(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyWizardIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.FireWizard;
            this.PlayAnimation(true);
            this.TintablePart = _objectList[0];
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (m_fireballSummon != null)
            {
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    m_fireballSummon.Position = new Vector2(this.X + m_spellOffset.X, this.Y + m_spellOffset.Y);
                else
                    m_fireballSummon.Position = new Vector2(this.X - m_spellOffset.X, this.Y + m_spellOffset.Y);
            }

            if (m_fireParticleEffectCounter > 0)
            {
                m_fireParticleEffectCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_fireParticleEffectCounter <= 0)
                {
                    m_levelScreen.ImpactEffectPool.DisplayFireParticleEffect(this);
                    m_fireParticleEffectCounter = 0.15f;
                }
            }
        }

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
                Scale = ProjectileScale,
            };

            if (this.Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                projData.AngleOffset = CDGMath.RandomInt(-25, 25);

            if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
            {
                projData.SpriteName = "GhostBossProjectile_Sprite";
                projData.CollidesWithTerrain = false;
            }

            SoundManager.Play3DSound(this, m_target, "FireWizard_Attack_01", "FireWizard_Attack_02", "FireWizard_Attack_03", "FireWizard_Attack_04");
            ProjectileObj fireball = m_levelScreen.ProjectileManager.FireProjectile(projData);
            fireball.Rotation = 0;
            if (this.Difficulty != GameTypes.EnemyDifficulty.EXPERT)
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
                DestroysWithEnemy = false,
                Scale = ProjectileScale,
            };

            if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
            {
                projData.SpriteName = "GhostBossProjectile_Sprite";
                projData.CollidesWithTerrain = false;
            }

            //m_fireballSummon = m_levelScreen.ProjectileManager.FireProjectile("WizardFireballProjectile_Sprite", this, m_spellOffset, 0, 0, false, 0, ProjectileDamage);
            SoundManager.Play3DSound(this, m_target, "Fire_Wizard_Form");
            m_fireballSummon = m_levelScreen.ProjectileManager.FireProjectile(projData);
            m_fireballSummon.Opacity = 0;
            m_fireballSummon.Scale = Vector2.Zero;
            m_fireballSummon.AnimationDelay = 1 / 10f;
            m_fireballSummon.PlayAnimation(true);
            m_fireballSummon.Rotation = 0;

            Tweener.Tween.To(m_fireballSummon, 0.5f, Tweener.Ease.Back.EaseOut, "Opacity", "1", "ScaleX", ProjectileScale.X.ToString(), "ScaleY", ProjectileScale.Y.ToString());

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

        //public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        //{
        //    base.HitEnemy(damage, position, isPlayer);
        //    if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
        //    {
        //        m_currentActiveLB.StopLogicBlock();
        //        ResetFireball();
        //    }
        //}

        public override void Kill(bool giveXP = true)
        {
            if (m_currentActiveLB != null && m_currentActiveLB.IsActive == true)
            {
                m_currentActiveLB.StopLogicBlock();
                ResetFireball();
            }
            base.Kill(giveXP);
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
            ResetFireball();
            base.ResetState();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_fireballSummon = null;
                base.Dispose();
            }
        }
    }
}

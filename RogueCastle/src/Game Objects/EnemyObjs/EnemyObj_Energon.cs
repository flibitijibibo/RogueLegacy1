using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Energon : EnemyObj
    {
        private const byte TYPE_SWORD = 0;
        private const byte TYPE_SHIELD = 1;
        private const byte TYPE_DOWNSWORD = 2;

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private SpriteObj m_shield;
        private DS2DPool<EnergonProjectileObj> m_projectilePool;
        private byte m_poolSize = 10;
        private byte m_currentAttackType = TYPE_SWORD;

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Energon_Basic_Name;
            LocStringID = EnemyEV.Energon_Basic_Name_locID;

            MaxHealth = EnemyEV.Energon_Basic_MaxHealth;
            Damage = EnemyEV.Energon_Basic_Damage;
            XPValue = EnemyEV.Energon_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Energon_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Energon_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Energon_Basic_DropChance;

            Speed = EnemyEV.Energon_Basic_Speed;
            TurnSpeed = EnemyEV.Energon_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Energon_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Energon_Basic_Jump;
            CooldownTime = EnemyEV.Energon_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Energon_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Energon_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Energon_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Energon_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Energon_Basic_IsWeighted;

            Scale = EnemyEV.Energon_Basic_Scale;
            ProjectileScale = EnemyEV.Energon_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Energon_Basic_Tint;

            MeleeRadius = EnemyEV.Energon_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Energon_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Energon_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Energon_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Energon_Miniboss_Name;
                    LocStringID = EnemyEV.Energon_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Energon_Miniboss_MaxHealth;
                    Damage = EnemyEV.Energon_Miniboss_Damage;
                    XPValue = EnemyEV.Energon_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Energon_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Energon_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Energon_Miniboss_DropChance;

                    Speed = EnemyEV.Energon_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Energon_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Energon_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Energon_Miniboss_Jump;
                    CooldownTime = EnemyEV.Energon_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Energon_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Energon_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Energon_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Energon_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Energon_Miniboss_IsWeighted;

                    Scale = EnemyEV.Energon_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Energon_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Energon_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Energon_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Energon_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Energon_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Energon_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Energon_Expert_Name;
                    LocStringID = EnemyEV.Energon_Expert_Name_locID;

                    MaxHealth = EnemyEV.Energon_Expert_MaxHealth;
                    Damage = EnemyEV.Energon_Expert_Damage;
                    XPValue = EnemyEV.Energon_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Energon_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Energon_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Energon_Expert_DropChance;

                    Speed = EnemyEV.Energon_Expert_Speed;
                    TurnSpeed = EnemyEV.Energon_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Energon_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Energon_Expert_Jump;
                    CooldownTime = EnemyEV.Energon_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Energon_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Energon_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Energon_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Energon_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Energon_Expert_IsWeighted;

                    Scale = EnemyEV.Energon_Expert_Scale;
                    ProjectileScale = EnemyEV.Energon_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Energon_Expert_Tint;

                    MeleeRadius = EnemyEV.Energon_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Energon_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Energon_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Energon_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Energon_Advanced_Name;
                    LocStringID = EnemyEV.Energon_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Energon_Advanced_MaxHealth;
                    Damage = EnemyEV.Energon_Advanced_Damage;
                    XPValue = EnemyEV.Energon_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Energon_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Energon_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Energon_Advanced_DropChance;

                    Speed = EnemyEV.Energon_Advanced_Speed;
                    TurnSpeed = EnemyEV.Energon_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Energon_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Energon_Advanced_Jump;
                    CooldownTime = EnemyEV.Energon_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Energon_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Energon_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Energon_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Energon_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Energon_Advanced_IsWeighted;

                    Scale = EnemyEV.Energon_Advanced_Scale;
                    ProjectileScale = EnemyEV.Energon_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Energon_Advanced_Tint;

                    MeleeRadius = EnemyEV.Energon_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Energon_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Energon_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Energon_Advanced_KnockBack;
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
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyEnergonWalk_Character", true, true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(0.2f, 0.75f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new ChangeSpriteLogicAction("EnemyEnergonWalk_Character", true, true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(0.2f, 0.75f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyEnergonIdle_Character", true, true));
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.2f, 0.75f));

            LogicSet fireProjectile = new LogicSet(this);
            fireProjectile.AddAction(new ChangePropertyLogicAction(this, "CurrentSpeed", 0));
            fireProjectile.AddAction(new ChangeSpriteLogicAction("EnemyEnergonAttack_Character", false, false));
            fireProjectile.AddAction(new PlayAnimationLogicAction("Start", "BeforeAttack"));
            fireProjectile.AddAction(new RunFunctionLogicAction(this, "FireCurrentTypeProjectile"));
            fireProjectile.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            fireProjectile.AddAction(new ChangeSpriteLogicAction("EnemyEnergonIdle_Character", true, true));
            fireProjectile.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet changeAttackType = new LogicSet(this);
            changeAttackType.AddAction(new ChangePropertyLogicAction(this, "CurrentSpeed", 0));
            changeAttackType.AddAction(new ChangeSpriteLogicAction("EnemyEnergonAttack_Character", false, false));
            changeAttackType.AddAction(new PlayAnimationLogicAction("Start", "BeforeAttack"));
            changeAttackType.AddAction(new RunFunctionLogicAction(this, "SwitchRandomType"));
            changeAttackType.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            changeAttackType.AddAction(new ChangeSpriteLogicAction("EnemyEnergonIdle_Character", true, true));
            changeAttackType.AddAction(new DelayLogicAction(2.0f));

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, fireProjectile, changeAttackType);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 30, 60, 10); //walkTowardsLS, walkAwayLS, walkStopLS, fireProjectile, changeAttackType
                    break;
                case (STATE_ENGAGE):
                case (STATE_WANDER):
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
                case (STATE_WANDER):
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
                case (STATE_WANDER):
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

        public void FireCurrentTypeProjectile()
        {
            FireProjectile(m_currentAttackType);
        }

        public void FireProjectile(byte type)
        {
            EnergonProjectileObj projectile = m_projectilePool.CheckOut();
            projectile.SetType(type);
            PhysicsMngr.AddObject(projectile);
            projectile.Target = m_target;
            projectile.Visible = true;
            projectile.Position = this.Position;
            projectile.CurrentSpeed = ProjectileSpeed;
            projectile.Flip = this.Flip;
            projectile.Scale = ProjectileScale;
            projectile.Opacity = 0.8f;
            projectile.Damage = this.Damage;
            projectile.PlayAnimation(true);
            //projectile.TurnSpeed = TurnSpeed;
        }

        public void DestroyProjectile(EnergonProjectileObj projectile)
        {
            if (m_projectilePool.ActiveObjsList.Contains(projectile)) // Only destroy projectiles if they haven't already been destroyed. It is possible for two objects to call Destroy on the same projectile.
            {
                projectile.Visible = false;
                projectile.Scale = new Vector2(1, 1);
                projectile.CollisionTypeTag = GameTypes.CollisionType_ENEMY;
                PhysicsMngr.RemoveObject(projectile); // Might be better to keep them in the physics manager and just turn off their collision detection.
                m_projectilePool.CheckIn(projectile);
            }
        }

        public void DestroyAllProjectiles()
        {
            ProjectileObj[] activeProjectilesArray = m_projectilePool.ActiveObjsList.ToArray();
            foreach (EnergonProjectileObj projectile in activeProjectilesArray)
            {
                DestroyProjectile(projectile);
            }
        }

        public void SwitchRandomType()
        {
            byte storedAttackType = m_currentAttackType;
            while (storedAttackType == m_currentAttackType)
                storedAttackType = (byte)CDGMath.RandomInt(0, 2);
            SwitchType(storedAttackType);
        }

        public void SwitchType(byte type)
        {
            m_currentAttackType = type;
            switch (type)
            {
                case (TYPE_SWORD):
                    m_shield.ChangeSprite("EnergonSwordShield_Sprite");
                    break;
                case (TYPE_SHIELD):
                    m_shield.ChangeSprite("EnergonShieldShield_Sprite");
                    break;
                case (TYPE_DOWNSWORD):
                    m_shield.ChangeSprite("EnergonDownSwordShield_Sprite");
                    break;
            }
            m_shield.PlayAnimation(true);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (EnergonProjectileObj projectile in m_projectilePool.ActiveObjsList)
                projectile.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);
            m_shield.Position = this.Position;
            m_shield.Flip = this.Flip;
            m_shield.Draw(camera);
            foreach (ProjectileObj projectile in m_projectilePool.ActiveObjsList)
                projectile.Draw(camera);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if ((collisionResponseType == Consts.COLLISIONRESPONSE_FIRSTBOXHIT) && m_invincibleCounter <= 0 && otherBox.AbsParent is PlayerObj)
            {
                if (m_target.Bounds.Left + m_target.Bounds.Width / 2 < this.X)
                    m_target.AccelerationX = -m_target.EnemyKnockBack.X;
                else
                    m_target.AccelerationX = m_target.EnemyKnockBack.X;
                m_target.AccelerationY = -m_target.EnemyKnockBack.Y;

                //m_target.CancelAttack();
                Point intersectPt = Rectangle.Intersect(thisBox.AbsRect, otherBox.AbsRect).Center;
                Vector2 impactPosition = new Vector2(intersectPt.X, intersectPt.Y);

                m_levelScreen.ImpactEffectPool.DisplayBlockImpactEffect(impactPosition, Vector2.One);
                m_levelScreen.SetLastEnemyHit(this);
                m_invincibleCounter = InvincibilityTime;
                Blink(Color.LightBlue, 0.1f);

                ProjectileObj projectile = otherBox.AbsParent as ProjectileObj;
                if (projectile != null)
                    m_levelScreen.ProjectileManager.DestroyProjectile(projectile);
            }
            else if (otherBox.AbsParent is EnergonProjectileObj)
            {
                //base.CollisionResponse(thisBox, otherBox, collisionResponseType);

                EnergonProjectileObj projectile = otherBox.AbsParent as EnergonProjectileObj;
                if (projectile != null)
                {
                    Point intersectPt = Rectangle.Intersect(thisBox.AbsRect, otherBox.AbsRect).Center;
                    Vector2 impactPosition = new Vector2(intersectPt.X, intersectPt.Y);

                    DestroyProjectile(projectile);
                    if (projectile.AttackType == m_currentAttackType)
                        HitEnemy(projectile.Damage, impactPosition, true);
                    else
                        m_levelScreen.ImpactEffectPool.DisplayBlockImpactEffect(impactPosition, Vector2.One);
                }
            }
            else
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void Kill(bool giveXP = true)
        {
            m_shield.Visible = false;
            DestroyAllProjectiles();
            base.Kill(giveXP);
        }

        public override void Reset()
        {
            m_shield.Visible = true;
            base.Reset();
        }

        public EnemyObj_Energon(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyEnergonIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Energon;
            m_shield = new SpriteObj("EnergonSwordShield_Sprite");
            m_shield.AnimationDelay = 1 / 10f;
            m_shield.PlayAnimation(true);
            m_shield.Opacity = 0.5f;
            m_shield.Scale = new Vector2(1.2f, 1.2f);

            m_projectilePool = new DS2DPool<EnergonProjectileObj>();

            for (int i = 0; i < m_poolSize; i++)
            {
                EnergonProjectileObj projectile = new EnergonProjectileObj("EnergonSwordProjectile_Sprite", this);
                projectile.Visible = false;
                projectile.CollidesWithTerrain = false;
                projectile.PlayAnimation(true);
                projectile.AnimationDelay = 1 / 20f;
                m_projectilePool.AddToPool(projectile);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_projectilePool.Dispose();
                m_projectilePool = null;
                m_shield.Dispose();
                m_shield = null;
                base.Dispose();
            }
        }
    }
}

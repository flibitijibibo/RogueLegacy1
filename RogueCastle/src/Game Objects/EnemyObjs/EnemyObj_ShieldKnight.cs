using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class EnemyObj_ShieldKnight : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();

        private Vector2 ShieldKnockback = new Vector2(900, 1050);
        private float m_blockDmgReduction = 0.6f;//0.8f;//0.9f;
        private FrameSoundObj m_walkSound, m_walkSound2;

        protected override void InitializeEV()
        {
            LockFlip = true;

            #region Basic Variables - General
            Name = EnemyEV.ShieldKnight_Basic_Name;
            LocStringID = EnemyEV.ShieldKnight_Basic_Name_locID;

            MaxHealth = EnemyEV.ShieldKnight_Basic_MaxHealth;
            Damage = EnemyEV.ShieldKnight_Basic_Damage;
            XPValue = EnemyEV.ShieldKnight_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.ShieldKnight_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.ShieldKnight_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.ShieldKnight_Basic_DropChance;

            Speed = EnemyEV.ShieldKnight_Basic_Speed;
            TurnSpeed = EnemyEV.ShieldKnight_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.ShieldKnight_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.ShieldKnight_Basic_Jump;
            CooldownTime = EnemyEV.ShieldKnight_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.ShieldKnight_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.ShieldKnight_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.ShieldKnight_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.ShieldKnight_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.ShieldKnight_Basic_IsWeighted;

            Scale = EnemyEV.ShieldKnight_Basic_Scale;
            ProjectileScale = EnemyEV.ShieldKnight_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.ShieldKnight_Basic_Tint;

            MeleeRadius = EnemyEV.ShieldKnight_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.ShieldKnight_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.ShieldKnight_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.ShieldKnight_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    ShieldKnockback = new Vector2(1200, 1350);

                    #region Miniboss Variables - General
                    Name = EnemyEV.ShieldKnight_Miniboss_Name;
                    LocStringID = EnemyEV.ShieldKnight_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.ShieldKnight_Miniboss_MaxHealth;
                    Damage = EnemyEV.ShieldKnight_Miniboss_Damage;
                    XPValue = EnemyEV.ShieldKnight_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.ShieldKnight_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.ShieldKnight_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.ShieldKnight_Miniboss_DropChance;

                    Speed = EnemyEV.ShieldKnight_Miniboss_Speed;
                    TurnSpeed = EnemyEV.ShieldKnight_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.ShieldKnight_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.ShieldKnight_Miniboss_Jump;
                    CooldownTime = EnemyEV.ShieldKnight_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.ShieldKnight_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.ShieldKnight_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.ShieldKnight_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.ShieldKnight_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.ShieldKnight_Miniboss_IsWeighted;

                    Scale = EnemyEV.ShieldKnight_Miniboss_Scale;
                    ProjectileScale = EnemyEV.ShieldKnight_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.ShieldKnight_Miniboss_Tint;

                    MeleeRadius = EnemyEV.ShieldKnight_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.ShieldKnight_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.ShieldKnight_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.ShieldKnight_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    ShieldKnockback = new Vector2(1550, 1650);

                    #region Expert Variables - General
                    Name = EnemyEV.ShieldKnight_Expert_Name;
                    LocStringID = EnemyEV.ShieldKnight_Expert_Name_locID;

                    MaxHealth = EnemyEV.ShieldKnight_Expert_MaxHealth;
                    Damage = EnemyEV.ShieldKnight_Expert_Damage;
                    XPValue = EnemyEV.ShieldKnight_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.ShieldKnight_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.ShieldKnight_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.ShieldKnight_Expert_DropChance;

                    Speed = EnemyEV.ShieldKnight_Expert_Speed;
                    TurnSpeed = EnemyEV.ShieldKnight_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.ShieldKnight_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.ShieldKnight_Expert_Jump;
                    CooldownTime = EnemyEV.ShieldKnight_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.ShieldKnight_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.ShieldKnight_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.ShieldKnight_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.ShieldKnight_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.ShieldKnight_Expert_IsWeighted;

                    Scale = EnemyEV.ShieldKnight_Expert_Scale;
                    ProjectileScale = EnemyEV.ShieldKnight_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.ShieldKnight_Expert_Tint;

                    MeleeRadius = EnemyEV.ShieldKnight_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.ShieldKnight_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.ShieldKnight_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.ShieldKnight_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    ShieldKnockback = new Vector2(1050, 1150);

                    #region Advanced Variables - General
                    Name = EnemyEV.ShieldKnight_Advanced_Name;
                    LocStringID = EnemyEV.ShieldKnight_Advanced_Name_locID;

                    MaxHealth = EnemyEV.ShieldKnight_Advanced_MaxHealth;
                    Damage = EnemyEV.ShieldKnight_Advanced_Damage;
                    XPValue = EnemyEV.ShieldKnight_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.ShieldKnight_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.ShieldKnight_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.ShieldKnight_Advanced_DropChance;

                    Speed = EnemyEV.ShieldKnight_Advanced_Speed;
                    TurnSpeed = EnemyEV.ShieldKnight_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.ShieldKnight_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.ShieldKnight_Advanced_Jump;
                    CooldownTime = EnemyEV.ShieldKnight_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.ShieldKnight_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.ShieldKnight_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.ShieldKnight_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.ShieldKnight_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.ShieldKnight_Advanced_IsWeighted;

                    Scale = EnemyEV.ShieldKnight_Advanced_Scale;
                    ProjectileScale = EnemyEV.ShieldKnight_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.ShieldKnight_Advanced_Tint;

                    MeleeRadius = EnemyEV.ShieldKnight_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.ShieldKnight_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.ShieldKnight_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.ShieldKnight_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }		


        }

        protected override void InitializeLogic()
        {
            LogicSet walkStopLS = new LogicSet(this);  //Face direction locked, so this only makes him stop moving.
            walkStopLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkStopLS.AddAction(new ChangeSpriteLogicAction("EnemyShieldKnightIdle_Character", true, true));
            //walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new MoveDirectionLogicAction(0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f, 2.0f));

            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyShieldKnightWalk_Character", true, true));
            //walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new MoveDirectionLogicAction());
            walkTowardsLS.AddAction(new DelayLogicAction(0.5f, 2.0f));

            LogicSet turnLS = new LogicSet(this);
            turnLS.AddAction(new LockFaceDirectionLogicAction(true));
            turnLS.AddAction(new MoveDirectionLogicAction(0));
            turnLS.AddAction(new ChangeSpriteLogicAction("EnemyShieldKnightTurnIn_Character", false, false));
            turnLS.AddAction(new PlayAnimationLogicAction(1, 2));
            turnLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"ShieldKnight_Turn"));
            turnLS.AddAction(new PlayAnimationLogicAction(3, this.TotalFrames));
            turnLS.AddAction(new LockFaceDirectionLogicAction(false));
            turnLS.AddAction(new MoveLogicAction(m_target, true, 0));
            turnLS.AddAction(new LockFaceDirectionLogicAction(true));
            turnLS.AddAction(new ChangeSpriteLogicAction("EnemyShieldKnightTurnOut_Character", true, false));
            turnLS.AddAction(new MoveDirectionLogicAction());


            LogicSet turnExpertLS = new LogicSet(this);
            turnExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            turnExpertLS.AddAction(new MoveDirectionLogicAction(0));
            turnExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyShieldKnightTurnIn_Character", false, false));
            turnExpertLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 20f));
            turnExpertLS.AddAction(new PlayAnimationLogicAction(1, 2));
            turnExpertLS.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "ShieldKnight_Turn"));
            turnExpertLS.AddAction(new PlayAnimationLogicAction(3, this.TotalFrames));
            turnExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            turnExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            turnExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            turnExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyShieldKnightTurnOut_Character", true, false));
            turnExpertLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / EnemyEV.ShieldKnight_Expert_AnimationDelay));
            turnExpertLS.AddAction(new MoveDirectionLogicAction());

            m_generalBasicLB.AddLogicSet(walkStopLS, walkTowardsLS, turnLS);
            m_generalExpertLB.AddLogicSet(walkStopLS, walkTowardsLS, turnExpertLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalExpertLB);

            SetCooldownLogicBlock(m_generalBasicLB, 100); 

            base.InitializeLogic();

        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalBasicLB, 0, 100, 0); // walkStopLS, walkTowardsLS, turnLS
                    break;
                case (STATE_WANDER):
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalBasicLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
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
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalBasicLB, 0, 100, 0); // walkStopLS, walkTowardsLS, turnLS
                    break;
                case (STATE_WANDER):
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalBasicLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
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
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalExpertLB, 0, 0, 100); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalExpertLB, 0, 100, 0); // walkStopLS, walkTowardsLS, turnLS
                    break;
                case (STATE_WANDER):
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalExpertLB, 0, 0, 100); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalExpertLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
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
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalBasicLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalBasicLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
                    break;
                case (STATE_WANDER):
                    if ((m_target.X > this.X && this.HeadingX < 0) || (m_target.X < this.X && this.HeadingX >= 0))
                        RunLogicBlock(true, m_generalBasicLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
                    else
                        RunLogicBlock(true, m_generalBasicLB, 100, 0, 0); // walkStopLS, walkTowardsLS, turnLS
                    break;
                default:
                    break;
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            PlayerObj player = otherBox.AbsParent as PlayerObj;
            ProjectileObj proj = otherBox.AbsParent as ProjectileObj;

            //if ((collisionResponseType == Consts.COLLISIONRESPONSE_FIRSTBOXHIT) && ((otherBox.AbsParent is ProjectileObj == false && m_invincibleCounter <= 0) || (otherBox.AbsParent is ProjectileObj && m_invincibleCounterProjectile <= 0) &&
            //    ((this.Flip == SpriteEffects.None && otherBox.AbsParent.AbsPosition.X > this.X) || (this.Flip == SpriteEffects.FlipHorizontally && otherBox.AbsParent.AbsPosition.X < this.X)) && (player != null && player.IsAirAttacking == false)))

            // Enemy blocked.
            if (collisionResponseType == Consts.COLLISIONRESPONSE_FIRSTBOXHIT &&
                ((player != null && m_invincibleCounter <= 0) || (proj != null && m_invincibleCounterProjectile <= 0)) &&
                ((this.Flip == SpriteEffects.None && otherBox.AbsParent.AbsPosition.X > this.X) || (this.Flip == SpriteEffects.FlipHorizontally && otherBox.AbsParent.AbsPosition.X < this.X)) &&
                (player != null && player.SpriteName != "PlayerAirAttack_Character") //player.IsAirAttacking == false)
                )
            {
                if (CanBeKnockedBack == true)
                {
                    CurrentSpeed = 0;
                    m_currentActiveLB.StopLogicBlock();
                    RunLogicBlock(true, m_generalBasicLB, 100, 0, 0);
                    //if (otherBox.AbsParent.Bounds.Left + otherBox.AbsParent.Bounds.Width / 2 > this.X)
                    //    AccelerationX = -KnockBack.X;
                    //else
                    //    AccelerationX = KnockBack.X;
                    //AccelerationY = -KnockBack.Y;
                }


                if (m_target.IsAirAttacking == true)
                {
                    m_target.IsAirAttacking = false; // Only allow one object to perform upwards air knockback on the player.
                    m_target.AccelerationY = -m_target.AirAttackKnockBack;
                    m_target.NumAirBounces++;
                }
                else
                {
                    if (m_target.Bounds.Left + m_target.Bounds.Width / 2 < this.X)
                        m_target.AccelerationX = -ShieldKnockback.X;
                    else
                        m_target.AccelerationX = ShieldKnockback.X;
                    m_target.AccelerationY = -ShieldKnockback.Y;
                    //if (m_target.Bounds.Left + m_target.Bounds.Width / 2 < this.X)
                    //    m_target.AccelerationX = -m_target.EnemyKnockBack.X;
                    //else
                    //    m_target.AccelerationX = m_target.EnemyKnockBack.X;
                    //m_target.AccelerationY = -m_target.EnemyKnockBack.Y;
                }

                // This must be called before the invincible counter is set.
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);

                Point intersectPt = Rectangle.Intersect(thisBox.AbsRect, otherBox.AbsRect).Center;
                Vector2 impactPosition = new Vector2(intersectPt.X, intersectPt.Y);
                m_levelScreen.ImpactEffectPool.DisplayBlockImpactEffect(impactPosition, new Vector2(2,2));
                SoundManager.Play3DSound(this, Game.ScreenManager.Player,"ShieldKnight_Block01", "ShieldKnight_Block02", "ShieldKnight_Block03");
                m_invincibleCounter = InvincibilityTime;
                m_levelScreen.SetLastEnemyHit(this);
                Blink(Color.LightBlue, 0.1f);

                ProjectileObj projectile = otherBox.AbsParent as ProjectileObj;
                if (projectile != null)
                {
                    m_invincibleCounterProjectile = InvincibilityTime;
                    m_levelScreen.ProjectileManager.DestroyProjectile(projectile);
                }
            }
            else
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            if (m_target != null && m_target.CurrentHealth > 0)
            {
                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Knight_Hit01", "Knight_Hit02", "Knight_Hit03");
                if ((this.Flip == SpriteEffects.None && m_target.X > this.X) || (this.Flip == SpriteEffects.FlipHorizontally && m_target.X < this.X))
                {
                    // Air attacks and all other damage other than sword swipes should deal their full damage.
                    if (m_target.SpriteName != "PlayerAirAttack_Character")
                        damage = (int)(damage * (1 - m_blockDmgReduction));
                }
            }
            base.HitEnemy(damage, position, isPlayer);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.SpriteName == "EnemyShieldKnightWalk_Character")
            {
                m_walkSound.Update();
                m_walkSound2.Update();
            }
            base.Update(gameTime);
        }

        public EnemyObj_ShieldKnight(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyShieldKnightIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.ShieldKnight;
            m_walkSound = new FrameSoundObj(this, m_target, 1, "KnightWalk1", "KnightWalk2");
            m_walkSound2 = new FrameSoundObj(this, m_target, 6, "KnightWalk1", "KnightWalk2");
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

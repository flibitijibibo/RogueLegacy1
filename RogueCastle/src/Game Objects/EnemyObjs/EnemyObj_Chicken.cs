using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Chicken : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();

        protected override void InitializeEV()
        {
            this.LockFlip = true;

            #region Basic Variables - General
            Name = EnemyEV.Chicken_Basic_Name;
            LocStringID = EnemyEV.Chicken_Basic_Name_locID;

            MaxHealth = EnemyEV.Chicken_Basic_MaxHealth;
            Damage = EnemyEV.Chicken_Basic_Damage;
            XPValue = EnemyEV.Chicken_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Chicken_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Chicken_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Chicken_Basic_DropChance;

            Speed = EnemyEV.Chicken_Basic_Speed;
            TurnSpeed = EnemyEV.Chicken_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Chicken_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Chicken_Basic_Jump;
            CooldownTime = EnemyEV.Chicken_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Chicken_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Chicken_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Chicken_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Chicken_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Chicken_Basic_IsWeighted;

            Scale = EnemyEV.Chicken_Basic_Scale;
            ProjectileScale = EnemyEV.Chicken_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Chicken_Basic_Tint;

            MeleeRadius = EnemyEV.Chicken_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Chicken_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Chicken_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Chicken_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Chicken_Miniboss_Name;
                    LocStringID = EnemyEV.Chicken_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Chicken_Miniboss_MaxHealth;
                    Damage = EnemyEV.Chicken_Miniboss_Damage;
                    XPValue = EnemyEV.Chicken_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Chicken_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Chicken_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Chicken_Miniboss_DropChance;

                    Speed = EnemyEV.Chicken_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Chicken_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Chicken_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Chicken_Miniboss_Jump;
                    CooldownTime = EnemyEV.Chicken_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Chicken_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Chicken_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Chicken_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Chicken_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Chicken_Miniboss_IsWeighted;

                    Scale = EnemyEV.Chicken_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Chicken_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Chicken_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Chicken_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Chicken_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Chicken_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Chicken_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Chicken_Expert_Name;
                    LocStringID = EnemyEV.Chicken_Expert_Name_locID;

                    MaxHealth = EnemyEV.Chicken_Expert_MaxHealth;
                    Damage = EnemyEV.Chicken_Expert_Damage;
                    XPValue = EnemyEV.Chicken_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Chicken_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Chicken_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Chicken_Expert_DropChance;

                    Speed = EnemyEV.Chicken_Expert_Speed;
                    TurnSpeed = EnemyEV.Chicken_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Chicken_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Chicken_Expert_Jump;
                    CooldownTime = EnemyEV.Chicken_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Chicken_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Chicken_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Chicken_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Chicken_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Chicken_Expert_IsWeighted;

                    Scale = EnemyEV.Chicken_Expert_Scale;
                    ProjectileScale = EnemyEV.Chicken_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Chicken_Expert_Tint;

                    MeleeRadius = EnemyEV.Chicken_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Chicken_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Chicken_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Chicken_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Chicken_Advanced_Name;
                    LocStringID = EnemyEV.Chicken_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Chicken_Advanced_MaxHealth;
                    Damage = EnemyEV.Chicken_Advanced_Damage;
                    XPValue = EnemyEV.Chicken_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Chicken_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Chicken_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Chicken_Advanced_DropChance;

                    Speed = EnemyEV.Chicken_Advanced_Speed;
                    TurnSpeed = EnemyEV.Chicken_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Chicken_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Chicken_Advanced_Jump;
                    CooldownTime = EnemyEV.Chicken_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Chicken_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Chicken_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Chicken_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Chicken_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Chicken_Advanced_IsWeighted;

                    Scale = EnemyEV.Chicken_Advanced_Scale;
                    ProjectileScale = EnemyEV.Chicken_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Chicken_Advanced_Tint;

                    MeleeRadius = EnemyEV.Chicken_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Chicken_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Chicken_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Chicken_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    this.Scale = new Vector2(2, 2);
                    break;
            }
         
            IsWeighted = true;
        }

        protected override void InitializeLogic()
        {
            LogicSet walkLeftLS = new LogicSet(this);
            walkLeftLS.AddAction(new ChangeSpriteLogicAction("EnemyChickenRun_Character", true, true));
            walkLeftLS.AddAction(new ChangePropertyLogicAction(this, "Flip", Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally));
            walkLeftLS.AddAction(new MoveDirectionLogicAction());
            walkLeftLS.AddAction(new DelayLogicAction(0.5f, 1.0f));

            LogicSet walkRightLS = new LogicSet(this);
            walkRightLS.AddAction(new ChangeSpriteLogicAction("EnemyChickenRun_Character", true, true));
            walkRightLS.AddAction(new ChangePropertyLogicAction(this, "Flip", Microsoft.Xna.Framework.Graphics.SpriteEffects.None));
            walkRightLS.AddAction(new MoveDirectionLogicAction());
            walkRightLS.AddAction(new DelayLogicAction(0.5f, 1.0f));

            m_generalBasicLB.AddLogicSet(walkLeftLS, walkRightLS);

            logicBlocksToDispose.Add(m_generalBasicLB);

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
                default:
                        RunLogicBlock(true, m_generalBasicLB, 50, 50);
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
                        RunLogicBlock(true, m_generalBasicLB, 50, 50);
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
                        RunLogicBlock(true, m_generalBasicLB, 50, 50);
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
                        RunLogicBlock(true, m_generalBasicLB, 50, 50);
                    break;
            }
        }

        public void MakeCollideable()
        {
            this.IsCollidable = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_levelScreen != null && m_levelScreen.CurrentRoom != null)
            {
                if (this.IsKilled == false && CollisionMath.Intersects(this.TerrainBounds, this.m_levelScreen.CurrentRoom.Bounds) == false)
                    this.Kill(true);
            }

            base.Update(gameTime);
        }

        public EnemyObj_Chicken(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyChickenRun_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Chicken;
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            SoundManager.Play3DSound(this, m_target, "Chicken_Cluck_01", "Chicken_Cluck_02", "Chicken_Cluck_03");
            base.HitEnemy(damage, collisionPt, isPlayer);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Zombie : EnemyObj
    {
        private LogicBlock m_basicWalkLS = new LogicBlock();
        private LogicBlock m_basicRiseLowerLS = new LogicBlock();

        public bool Risen { get; set; }
        public bool Lowered { get; set; }

        protected override void InitializeEV()
        {
            #region Basic Variables - General
            Name = EnemyEV.Zombie_Basic_Name;
            LocStringID = EnemyEV.Zombie_Basic_Name_locID;

            MaxHealth = EnemyEV.Zombie_Basic_MaxHealth;
            Damage = EnemyEV.Zombie_Basic_Damage;
            XPValue = EnemyEV.Zombie_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Zombie_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Zombie_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Zombie_Basic_DropChance;

            Speed = EnemyEV.Zombie_Basic_Speed;
            TurnSpeed = EnemyEV.Zombie_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Zombie_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Zombie_Basic_Jump;
            CooldownTime = EnemyEV.Zombie_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Zombie_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Zombie_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Zombie_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Zombie_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Zombie_Basic_IsWeighted;

            Scale = EnemyEV.Zombie_Basic_Scale;
            ProjectileScale = EnemyEV.Zombie_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Zombie_Basic_Tint;

            MeleeRadius = EnemyEV.Zombie_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Zombie_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Zombie_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Zombie_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):

                    #region Miniboss Variables - General
                    Name = EnemyEV.Zombie_Miniboss_Name;
                    LocStringID = EnemyEV.Zombie_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Zombie_Miniboss_MaxHealth;
                    Damage = EnemyEV.Zombie_Miniboss_Damage;
                    XPValue = EnemyEV.Zombie_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Zombie_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Zombie_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Zombie_Miniboss_DropChance;

                    Speed = EnemyEV.Zombie_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Zombie_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Zombie_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Zombie_Miniboss_Jump;
                    CooldownTime = EnemyEV.Zombie_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Zombie_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Zombie_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Zombie_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Zombie_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Zombie_Miniboss_IsWeighted;

                    Scale = EnemyEV.Zombie_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Zombie_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Zombie_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Zombie_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Zombie_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Zombie_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Zombie_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):

                    #region Expert Variables - General
                    Name = EnemyEV.Zombie_Expert_Name;
                    LocStringID = EnemyEV.Zombie_Expert_Name_locID;

                    MaxHealth = EnemyEV.Zombie_Expert_MaxHealth;
                    Damage = EnemyEV.Zombie_Expert_Damage;
                    XPValue = EnemyEV.Zombie_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Zombie_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Zombie_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Zombie_Expert_DropChance;

                    Speed = EnemyEV.Zombie_Expert_Speed;
                    TurnSpeed = EnemyEV.Zombie_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Zombie_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Zombie_Expert_Jump;
                    CooldownTime = EnemyEV.Zombie_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Zombie_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Zombie_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Zombie_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Zombie_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Zombie_Expert_IsWeighted;

                    Scale = EnemyEV.Zombie_Expert_Scale;
                    ProjectileScale = EnemyEV.Zombie_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Zombie_Expert_Tint;

                    MeleeRadius = EnemyEV.Zombie_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Zombie_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Zombie_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Zombie_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):

                    #region Advanced Variables - General
                    Name = EnemyEV.Zombie_Advanced_Name;
                    LocStringID = EnemyEV.Zombie_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Zombie_Advanced_MaxHealth;
                    Damage = EnemyEV.Zombie_Advanced_Damage;
                    XPValue = EnemyEV.Zombie_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Zombie_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Zombie_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Zombie_Advanced_DropChance;

                    Speed = EnemyEV.Zombie_Advanced_Speed;
                    TurnSpeed = EnemyEV.Zombie_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Zombie_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Zombie_Advanced_Jump;
                    CooldownTime = EnemyEV.Zombie_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Zombie_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Zombie_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Zombie_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Zombie_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Zombie_Advanced_IsWeighted;

                    Scale = EnemyEV.Zombie_Advanced_Scale;
                    ProjectileScale = EnemyEV.Zombie_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Zombie_Advanced_Tint;

                    MeleeRadius = EnemyEV.Zombie_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Zombie_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Zombie_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Zombie_Advanced_KnockBack;
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
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(false));
            walkTowardsLS.AddAction(new ChangeSpriteLogicAction("EnemyZombieWalk_Character", true, true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new LockFaceDirectionLogicAction(true));
            walkTowardsLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Zombie_Groan_01", "Zombie_Groan_02", "Zombie_Groan_03", "Blank", "Blank", "Blank", "Blank", "Blank"));
            walkTowardsLS.AddAction(new DelayLogicAction(0.5f));

            LogicSet riseLS = new LogicSet(this);
            riseLS.AddAction(new LockFaceDirectionLogicAction(false));
            riseLS.AddAction(new MoveLogicAction(m_target, false, 0));
            //riseLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 /  m_Rise_Animation_Speed));
            riseLS.AddAction(new ChangeSpriteLogicAction("EnemyZombieRise_Character", false, false));
            riseLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Zombie_Rise"));
            riseLS.AddAction(new PlayAnimationLogicAction(false));
            //riseLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", 1 / 14));
            riseLS.AddAction(new ChangePropertyLogicAction(this, "Risen", true));
            riseLS.AddAction(new ChangePropertyLogicAction(this, "Lowered", false));

            LogicSet lowerLS = new LogicSet(this);
            lowerLS.AddAction(new LockFaceDirectionLogicAction(false));
            lowerLS.AddAction(new MoveLogicAction(m_target, false, 0));
            lowerLS.AddAction(new ChangeSpriteLogicAction("EnemyZombieLower_Character", false, false));
            lowerLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Zombie_Lower"));
            lowerLS.AddAction(new PlayAnimationLogicAction(false));            
            lowerLS.AddAction(new ChangePropertyLogicAction(this, "Risen", false));
            lowerLS.AddAction(new ChangePropertyLogicAction(this, "Lowered", true));

            m_basicWalkLS.AddLogicSet(walkTowardsLS);
            m_basicRiseLowerLS.AddLogicSet(riseLS, lowerLS);

            logicBlocksToDispose.Add(m_basicWalkLS);
            logicBlocksToDispose.Add(m_basicRiseLowerLS);

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                        if (this.Risen == false)
                            RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                        else
                            RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //if (Math.Abs(this.Y - m_target.Y) < 100) // Ensures the zombie doesn't appear if the player is above or below him.
                    //{
                    //    if (this.Risen == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    //    else
                    //        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //}
                    //else if (m_target.IsTouchingGround == true)
                    //{
                    //    if (this.Lowered == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
                    //}
                    break;
                case (STATE_WANDER):
                    if (this.Lowered == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
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
                    if (this.Risen == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    else
                        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //if (Math.Abs(this.Y - m_target.Y) < 100) // Ensures the zombie doesn't appear if the player is above or below him.
                    //{
                    //    if (this.Risen == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    //    else
                    //        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //}
                    //else if (m_target.IsTouchingGround == true)
                    //{
                    //    if (this.Lowered == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
                    //}
                    break;
                case (STATE_WANDER):
                    if (this.Lowered == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
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
                    if (this.Risen == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    else
                        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //if (Math.Abs(this.Y - m_target.Y) < 100) // Ensures the zombie doesn't appear if the player is above or below him.
                    //{
                    //    if (this.Risen == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    //    else
                    //        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //}
                    //else if (m_target.IsTouchingGround == true)
                    //{
                    //    if (this.Lowered == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
                    //}
                    break;
                case (STATE_WANDER):
                    if (this.Lowered == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
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
                    if (this.Risen == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    else
                        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //if (Math.Abs(this.Y - m_target.Y) < 100) // Ensures the zombie doesn't appear if the player is above or below him.
                    //{
                    //    if (this.Risen == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 100, 0);
                    //    else
                    //        RunLogicBlock(false, m_basicWalkLS, 100); // walkTowardsLS
                    //}
                    //else if (m_target.IsTouchingGround == true)
                    //{
                    //    if (this.Lowered == false)
                    //        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
                    //}
                    break;
                case (STATE_WANDER):
                    if (this.Lowered == false)
                        RunLogicBlock(false, m_basicRiseLowerLS, 0, 100); // riseLS, lowerLS
                    break;
                default:
                    break;
            }
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Zombie_Hit");
            base.HitEnemy(damage, position, isPlayer);
        }

        public override void Update(GameTime gameTime)
        {
            // Hack to stop zombies in the ground from animating. I don't know why zombies keep animating.
            if ((m_currentActiveLB == null || m_currentActiveLB.IsActive == false) && this.Risen == false && this.IsAnimating == true)
            {
                this.ChangeSprite("EnemyZombieRise_Character");
                this.StopAnimation();
            }

            base.Update(gameTime);
        }

        public override void ResetState()
        {
            Lowered = true;
            Risen = false;
            base.ResetState();

            this.ChangeSprite("EnemyZombieLower_Character");
            this.GoToFrame(this.TotalFrames);
            this.StopAnimation();
        }

        public override void Reset()
        {
            this.ChangeSprite("EnemyZombieRise_Character");
            this.StopAnimation();
            Lowered = true;
            Risen = false;
            base.Reset();
        }

        public EnemyObj_Zombie(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyZombieLower_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.GoToFrame(this.TotalFrames);
            Lowered = true;
            this.ForceDraw = true;
            this.StopAnimation();
            this.Type = EnemyType.Zombie;
            this.PlayAnimationOnRestart = false;
        }
    }
}

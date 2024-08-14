using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;

namespace RogueCastle
{
    public class EnemyObj_Portrait : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        public bool Shake { get; set; }
        public bool Chasing { get; set; }

        protected override void InitializeEV()
        {
            this.LockFlip = true;

            #region Basic Variables - General
            Name = EnemyEV.Portrait_Basic_Name;
            LocStringID = EnemyEV.Portrait_Basic_Name_locID;

            MaxHealth = EnemyEV.Portrait_Basic_MaxHealth;
            Damage = EnemyEV.Portrait_Basic_Damage;
            XPValue = EnemyEV.Portrait_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Portrait_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Portrait_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Portrait_Basic_DropChance;

            Speed = EnemyEV.Portrait_Basic_Speed;
            TurnSpeed = EnemyEV.Portrait_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Portrait_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Portrait_Basic_Jump;
            CooldownTime = EnemyEV.Portrait_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Portrait_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Portrait_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Portrait_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Portrait_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Portrait_Basic_IsWeighted;

            Scale = EnemyEV.Portrait_Basic_Scale;
            ProjectileScale = EnemyEV.Portrait_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Portrait_Basic_Tint;

            MeleeRadius = EnemyEV.Portrait_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Portrait_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Portrait_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Portrait_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Portrait_Miniboss_Name;
                    LocStringID = EnemyEV.Portrait_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Portrait_Miniboss_MaxHealth;
                    Damage = EnemyEV.Portrait_Miniboss_Damage;
                    XPValue = EnemyEV.Portrait_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Portrait_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Portrait_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Portrait_Miniboss_DropChance;

                    Speed = EnemyEV.Portrait_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Portrait_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Portrait_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Portrait_Miniboss_Jump;
                    CooldownTime = EnemyEV.Portrait_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Portrait_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Portrait_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Portrait_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Portrait_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Portrait_Miniboss_IsWeighted;

                    Scale = EnemyEV.Portrait_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Portrait_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Portrait_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Portrait_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Portrait_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Portrait_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Portrait_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Portrait_Expert_Name;
                    LocStringID = EnemyEV.Portrait_Expert_Name_locID;

                    MaxHealth = EnemyEV.Portrait_Expert_MaxHealth;
                    Damage = EnemyEV.Portrait_Expert_Damage;
                    XPValue = EnemyEV.Portrait_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Portrait_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Portrait_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Portrait_Expert_DropChance;

                    Speed = EnemyEV.Portrait_Expert_Speed;
                    TurnSpeed = EnemyEV.Portrait_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Portrait_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Portrait_Expert_Jump;
                    CooldownTime = EnemyEV.Portrait_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Portrait_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Portrait_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Portrait_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Portrait_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Portrait_Expert_IsWeighted;

                    Scale = EnemyEV.Portrait_Expert_Scale;
                    ProjectileScale = EnemyEV.Portrait_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Portrait_Expert_Tint;

                    MeleeRadius = EnemyEV.Portrait_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Portrait_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Portrait_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Portrait_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Portrait_Advanced_Name;
                    LocStringID = EnemyEV.Portrait_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Portrait_Advanced_MaxHealth;
                    Damage = EnemyEV.Portrait_Advanced_Damage;
                    XPValue = EnemyEV.Portrait_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Portrait_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Portrait_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Portrait_Advanced_DropChance;

                    Speed = EnemyEV.Portrait_Advanced_Speed;
                    TurnSpeed = EnemyEV.Portrait_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Portrait_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Portrait_Advanced_Jump;
                    CooldownTime = EnemyEV.Portrait_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Portrait_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Portrait_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Portrait_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Portrait_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Portrait_Advanced_IsWeighted;

                    Scale = EnemyEV.Portrait_Advanced_Scale;
                    ProjectileScale = EnemyEV.Portrait_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Portrait_Advanced_Tint;

                    MeleeRadius = EnemyEV.Portrait_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Portrait_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Portrait_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Portrait_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }
        }

        protected override void InitializeLogic()
        {
            LogicSet basicWarningLS = new LogicSet(this);
            basicWarningLS.AddAction(new ChangePropertyLogicAction(this, "Shake", true));
            basicWarningLS.AddAction(new DelayLogicAction(1));
            basicWarningLS.AddAction(new ChangePropertyLogicAction(this, "Shake", false));
            basicWarningLS.AddAction(new DelayLogicAction(1));

            LogicSet moveTowardsLS = new LogicSet(this);
            moveTowardsLS.AddAction(new ChaseLogicAction(m_target, Vector2.Zero, Vector2.Zero, true, 1));

            LogicSet moveTowardsAdvancedLS = new LogicSet(this);
            moveTowardsAdvancedLS.AddAction(new ChaseLogicAction(m_target, Vector2.Zero, Vector2.Zero, true, 1.75f));
            ThrowAdvancedProjectiles(moveTowardsAdvancedLS, true);

            LogicSet moveTowardsExpertLS = new LogicSet(this);
            moveTowardsExpertLS.AddAction(new ChaseLogicAction(m_target, Vector2.Zero, Vector2.Zero, true, 1.75f));
            ThrowExpertProjectiles(moveTowardsExpertLS, true);

            LogicSet moveTowardsMiniBosstLS = new LogicSet(this);
            moveTowardsMiniBosstLS.AddAction(new ChaseLogicAction(m_target, Vector2.Zero, Vector2.Zero, true, 1.25f));
            ThrowProjectiles(moveTowardsMiniBosstLS, true);

            m_generalBasicLB.AddLogicSet(basicWarningLS, moveTowardsLS);
            m_generalAdvancedLB.AddLogicSet(basicWarningLS, moveTowardsAdvancedLS);
            m_generalExpertLB.AddLogicSet(basicWarningLS, moveTowardsExpertLS);
            m_generalMiniBossLB.AddLogicSet(basicWarningLS, moveTowardsMiniBosstLS);
            m_generalCooldownLB.AddLogicSet(basicWarningLS, moveTowardsLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            base.InitializeLogic();

            // HACK ALERT
            // This is NOT the right way to add collision boxes but it doesn't seem to be working the normal way.  This may cause problems though.
            this.CollisionBoxes.Clear();
            this.CollisionBoxes.Add(new CollisionBox((int)(-18 * this.ScaleX), (int)(-24 * this.ScaleY), (int)(36 * this.ScaleX), (int)(48 * this.ScaleY), 2, this));
            this.CollisionBoxes.Add(new CollisionBox((int)(-15 * this.ScaleX), (int)(-21 * this.ScaleY), (int)(31 * this.ScaleX), (int)(44 * this.ScaleY), 1, this));

            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
            {
                (GetChildAt(0) as SpriteObj).ChangeSprite("GiantPortrait_Sprite");
                this.Scale = new Vector2(2, 2);
                SpriteObj pic = new SpriteObj("Portrait" + CDGMath.RandomInt(0, 7) + "_Sprite");
                pic.OverrideParentScale = true;
                this.AddChild(pic);
                this.CollisionBoxes.Clear();
                this.CollisionBoxes.Add(new CollisionBox(-62 * 2, -88 * 2, 125 * 2, 177 * 2, 2, this));
                this.CollisionBoxes.Add(new CollisionBox(-62 * 2, -88 * 2, 125 * 2, 177 * 2, 1, this));
            }

        }

        private void ThrowProjectiles(LogicSet ls, bool useBossProjectile = false)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "GhostProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
            };

            //if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                //projData.SpriteName = "GhostProjectileBoss_Sprite";

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            projData.Angle = new Vector2(135, 135);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(45, 45);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-45, -45);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-135, -135);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Dispose();
        }

        private void ThrowExpertProjectiles(LogicSet ls, bool useBossProjectile = false)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "GhostProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
            };

            //if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
            //projData.SpriteName = "GhostProjectileBoss_Sprite";

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            projData.Angle = new Vector2(0, 0);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(90, 90);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(180, 180);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Dispose();
        }

        private void ThrowAdvancedProjectiles(LogicSet ls, bool useBossProjectile = false)
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "GhostProjectile_Sprite",
                SourceAnchor = Vector2.Zero,
                Target = null,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(0, 0),
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
            };

            //if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
            //projData.SpriteName = "GhostProjectileBoss_Sprite";

            ls.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"FairyAttack1"));
            projData.Angle = new Vector2(90, 90);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Angle = new Vector2(-90, -90);
            ls.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            projData.Dispose();
        }

        protected override void RunBasicLogic()
        {
            if (Chasing == false)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                        ChasePlayer();
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalBasicLB, 100, 0);
                        break;
                    case (STATE_WANDER):
                        break;
                    default:
                        break;
                }
            }
            else
                RunLogicBlock(true, m_generalBasicLB, 0, 100);
        }

        protected override void RunAdvancedLogic()
        {
            if (Chasing == false)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                        ChasePlayer();
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalAdvancedLB, 100, 0);
                        break;
                    case (STATE_WANDER):
                        break;
                    default:
                        break;
                }
            }
            else
                RunLogicBlock(true, m_generalAdvancedLB, 0, 100);
        }

        protected override void RunExpertLogic()
        {
            if (Chasing == false)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                        ChasePlayer();
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalExpertLB, 100, 0);
                        break;
                    case (STATE_WANDER):
                        break;
                    default:
                        break;
                }
            }
            else
                RunLogicBlock(true, m_generalExpertLB, 0, 100);
        }

        protected override void RunMinibossLogic()
        {
            if (Chasing == false)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                        Chasing = true;
                        break;
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalMiniBossLB, 100, 0);
                        break;
                    case (STATE_WANDER):
                        break;
                    default:
                        break;
                }
            }
            else
                RunLogicBlock(true, m_generalMiniBossLB, 0, 100);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Chasing == false)
            {
                if (this.Difficulty != GameTypes.EnemyDifficulty.MINIBOSS)
                {
                    if (Shake == true)
                        this.Rotation = (float)Math.Sin(Game.TotalGameTime * 15) * 2;
                    else
                        this.Rotation = 0;
                }
            }
            else
            {
                if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                    this.Rotation += (420 * elapsedSeconds);
                //this.Rotation += 7;
                else
                    this.Rotation += (600 * elapsedSeconds);
                    //this.Rotation += 10;

                SpriteObj portrait = this.GetChildAt(0) as SpriteObj;
                if (portrait.SpriteName != "EnemyPortrait" + (int)this.Difficulty + "_Sprite")
                    ChangePortrait();
            }

            base.Update(gameTime);
        }

        public override void HitEnemy(int damage, Vector2 collisionPt, bool isPlayer)
        {
            ChasePlayer();
            base.HitEnemy(damage, collisionPt, isPlayer);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (otherBox.AbsParent is PlayerObj)
                ChasePlayer();
            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public void ChasePlayer()
        {
            if (Chasing == false)
            {
                if (m_currentActiveLB != null)
                    m_currentActiveLB.StopLogicBlock();

                Chasing = true;
                if (m_target.X < this.X)
                    this.Orientation = 0;
                else
                    this.Orientation = MathHelper.ToRadians(180);
            }
        }

        public override void Reset()
        {
            Chasing = false;
            base.Reset();
        }

        public EnemyObj_Portrait(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyPortrait_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.Portrait;
            // Creating the picture frame for the enemy.
            string framePicture = "FramePicture" + CDGMath.RandomInt(1,16) + "_Sprite";
            this.GetChildAt(0).ChangeSprite(framePicture);
            PhysicsObj frame = this.GetChildAt(0) as PhysicsObj;
            this.DisableCollisionBoxRotations = false;
        }

        public void ChangePortrait()
        {
            SoundManager.PlaySound("FinalBoss_St2_BlockLaugh");
            SpriteObj portrait = this.GetChildAt(0) as SpriteObj;
            portrait.ChangeSprite("EnemyPortrait" + (int)this.Difficulty + "_Sprite");
            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                this.GetChildAt(1).Visible = false;
        }
    }
}

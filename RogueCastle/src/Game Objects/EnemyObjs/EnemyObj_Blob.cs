using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;

namespace RogueCastle
{
    public class EnemyObj_Blob: EnemyObj
    {
        //private float SeparationDistance = 50f; // The distance that blobs stay away from other enemies.

        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalAdvancedLB = new LogicBlock();
        private LogicBlock m_generalExpertLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private LogicBlock m_generalMiniBossLB = new LogicBlock();
        private LogicBlock m_generalBossCooldownLB = new LogicBlock();

        private LogicBlock m_generalNeoLB = new LogicBlock();

        private int NumHits = 0; // Number of time blobs can split.

        private Vector2 BlobSizeChange = new Vector2(0.4f, 0.4f);
        private float BlobSpeedChange = 1.2f;

        private float ExpertBlobProjectileDuration = 5.0f;

        public bool MainBlob { get; set; }
        public Vector2 SavedStartingPos { get; set; }
        private float JumpDelay = 0.4f;

        public RoomObj SpawnRoom;

        //6500
        private int m_bossCoins = 40;
        private int m_bossMoneyBags = 11;
        private int m_bossDiamonds = 5;

        private int m_bossEarthWizardLevelReduction = 12;//11; //The amount of levels reduced from earth wizard when spawned from the blob boss.

        private bool m_isNeo = false;

        protected override void InitializeEV()
        {
            SetNumberOfHits(2); //3
            BlobSizeChange = new Vector2(0.725f, 0.725f);
            BlobSpeedChange = 2.0f;

            #region Basic Variables - General
            Name = EnemyEV.Blob_Basic_Name;
            LocStringID = EnemyEV.Blob_Basic_Name_locID;

            MaxHealth = EnemyEV.Blob_Basic_MaxHealth;
            Damage = EnemyEV.Blob_Basic_Damage;
            XPValue = EnemyEV.Blob_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Blob_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Blob_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Blob_Basic_DropChance;

            Speed = EnemyEV.Blob_Basic_Speed;
            TurnSpeed = EnemyEV.Blob_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Blob_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Blob_Basic_Jump;
            CooldownTime = EnemyEV.Blob_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Blob_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Blob_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Blob_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Blob_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Blob_Basic_IsWeighted;

            Scale = EnemyEV.Blob_Basic_Scale;
            ProjectileScale = EnemyEV.Blob_Basic_ProjectileScale;
            //TintablePart.TextureColor = EnemyEV.Blob_Basic_Tint;

            MeleeRadius = EnemyEV.Blob_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Blob_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Blob_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Blob_Basic_KnockBack;
            #endregion		


            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):

                    SetNumberOfHits(5); //6
                    BlobSizeChange = new Vector2(0.6f, 0.6f);
                    BlobSpeedChange = 2.0f;//2.25f;
                    ForceDraw = true;
					#region Miniboss Variables - General
					Name = EnemyEV.Blob_Miniboss_Name;
                    LocStringID = EnemyEV.Blob_Miniboss_Name_locID;
					
					MaxHealth = EnemyEV.Blob_Miniboss_MaxHealth;
					Damage = EnemyEV.Blob_Miniboss_Damage;
					XPValue = EnemyEV.Blob_Miniboss_XPValue;
					
					MinMoneyDropAmount = EnemyEV.Blob_Miniboss_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.Blob_Miniboss_MaxDropAmount;
					MoneyDropChance = EnemyEV.Blob_Miniboss_DropChance;
					
					Speed = EnemyEV.Blob_Miniboss_Speed;
					TurnSpeed = EnemyEV.Blob_Miniboss_TurnSpeed;
					ProjectileSpeed = EnemyEV.Blob_Miniboss_ProjectileSpeed;
					JumpHeight = EnemyEV.Blob_Miniboss_Jump;
					CooldownTime = EnemyEV.Blob_Miniboss_Cooldown;
					AnimationDelay = 1 / EnemyEV.Blob_Miniboss_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.Blob_Miniboss_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.Blob_Miniboss_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.Blob_Miniboss_CanBeKnockedBack;
					IsWeighted = EnemyEV.Blob_Miniboss_IsWeighted;
					
					Scale = EnemyEV.Blob_Miniboss_Scale;
					ProjectileScale = EnemyEV.Blob_Miniboss_ProjectileScale;
					//TintablePart.TextureColor = EnemyEV.Blob_Miniboss_Tint;
					
					MeleeRadius = EnemyEV.Blob_Miniboss_MeleeRadius;
					ProjectileRadius = EnemyEV.Blob_Miniboss_ProjectileRadius;
					EngageRadius = EnemyEV.Blob_Miniboss_EngageRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.Blob_Miniboss_KnockBack;
					#endregion
                    if (LevelEV.WEAKEN_BOSSES == true)
                    {
                        this.MaxHealth = 1;
                        SetNumberOfHits(1);
                    }
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    SetNumberOfHits(4); //5
                    BlobSizeChange = new Vector2(0.65f, 0.65f);
                    BlobSpeedChange = 2.25f;

					#region Expert Variables - General
					Name = EnemyEV.Blob_Expert_Name;
                    LocStringID = EnemyEV.Blob_Expert_Name_locID;
					
					MaxHealth = EnemyEV.Blob_Expert_MaxHealth;
					Damage = EnemyEV.Blob_Expert_Damage;
					XPValue = EnemyEV.Blob_Expert_XPValue;
					
					MinMoneyDropAmount = EnemyEV.Blob_Expert_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.Blob_Expert_MaxDropAmount;
					MoneyDropChance = EnemyEV.Blob_Expert_DropChance;
					
					Speed = EnemyEV.Blob_Expert_Speed;
					TurnSpeed = EnemyEV.Blob_Expert_TurnSpeed;
					ProjectileSpeed = EnemyEV.Blob_Expert_ProjectileSpeed;
					JumpHeight = EnemyEV.Blob_Expert_Jump;
					CooldownTime = EnemyEV.Blob_Expert_Cooldown;
					AnimationDelay = 1 / EnemyEV.Blob_Expert_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.Blob_Expert_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.Blob_Expert_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.Blob_Expert_CanBeKnockedBack;
					IsWeighted = EnemyEV.Blob_Expert_IsWeighted;
					
					Scale = EnemyEV.Blob_Expert_Scale;
					ProjectileScale = EnemyEV.Blob_Expert_ProjectileScale;
					//TintablePart.TextureColor = EnemyEV.Blob_Expert_Tint;
					
					MeleeRadius = EnemyEV.Blob_Expert_MeleeRadius;
					ProjectileRadius = EnemyEV.Blob_Expert_ProjectileRadius;
					EngageRadius = EnemyEV.Blob_Expert_EngageRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.Blob_Expert_KnockBack;
					#endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    SetNumberOfHits(3); //4;
                    BlobSizeChange = new Vector2(0.6f, 0.6f);
                    BlobSpeedChange = 2.25f;

					#region Advanced Variables - General
					Name = EnemyEV.Blob_Advanced_Name;
                    LocStringID = EnemyEV.Blob_Advanced_Name_locID;
					
					MaxHealth = EnemyEV.Blob_Advanced_MaxHealth;
					Damage = EnemyEV.Blob_Advanced_Damage;
					XPValue = EnemyEV.Blob_Advanced_XPValue;
					
					MinMoneyDropAmount = EnemyEV.Blob_Advanced_MinDropAmount;
					MaxMoneyDropAmount = EnemyEV.Blob_Advanced_MaxDropAmount;
					MoneyDropChance = EnemyEV.Blob_Advanced_DropChance;
					
					Speed = EnemyEV.Blob_Advanced_Speed;
					TurnSpeed = EnemyEV.Blob_Advanced_TurnSpeed;
					ProjectileSpeed = EnemyEV.Blob_Advanced_ProjectileSpeed;
					JumpHeight = EnemyEV.Blob_Advanced_Jump;
					CooldownTime = EnemyEV.Blob_Advanced_Cooldown;
					AnimationDelay = 1 / EnemyEV.Blob_Advanced_AnimationDelay;
					
					AlwaysFaceTarget = EnemyEV.Blob_Advanced_AlwaysFaceTarget;
					CanFallOffLedges = EnemyEV.Blob_Advanced_CanFallOffLedges;
					CanBeKnockedBack = EnemyEV.Blob_Advanced_CanBeKnockedBack;
					IsWeighted = EnemyEV.Blob_Advanced_IsWeighted;
					
					Scale = EnemyEV.Blob_Advanced_Scale;
					ProjectileScale = EnemyEV.Blob_Advanced_ProjectileScale;
					//TintablePart.TextureColor = EnemyEV.Blob_Advanced_Tint;
					
					MeleeRadius = EnemyEV.Blob_Advanced_MeleeRadius;
					EngageRadius = EnemyEV.Blob_Advanced_EngageRadius;
					ProjectileRadius = EnemyEV.Blob_Advanced_ProjectileRadius;
					
					ProjectileDamage = Damage;
					KnockBack = EnemyEV.Blob_Advanced_KnockBack;
					#endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                    //this.Scale = new Vector2(2.4f, 2.4f);
                    break;
                default:
                    break;
            }

            if (this.Difficulty == GameTypes.EnemyDifficulty.BASIC)
            {
                _objectList[0].TextureColor = Color.Green;
                _objectList[2].TextureColor = Color.LightGreen;
                _objectList[2].Opacity = 0.8f;
                (_objectList[1] as SpriteObj).OutlineColour = Color.Red;
                _objectList[1].TextureColor = Color.Red;
            }

            else if (this.Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
            {
                _objectList[0].TextureColor = Color.Yellow;
                _objectList[2].TextureColor = Color.LightYellow;
                _objectList[2].Opacity = 0.8f;
                (_objectList[1] as SpriteObj).OutlineColour = Color.Pink;
                _objectList[1].TextureColor = Color.Pink;
            }
            else if (this.Difficulty == GameTypes.EnemyDifficulty.EXPERT)
            {
                _objectList[0].TextureColor = Color.Red;
                _objectList[2].TextureColor = Color.Pink;
                _objectList[2].Opacity = 0.8f;
                (_objectList[1] as SpriteObj).OutlineColour = Color.Yellow;
                _objectList[1].TextureColor = Color.Yellow;
            }

            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                m_resetSpriteName = "EnemyBlobBossIdle_Character";
        }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Move01", "Blob_Move02", "Blob_Move03", "Blank", "Blank", "Blank", "Blank", "Blank"));
            walkTowardsLS.AddAction(new DelayLogicAction(1.10f, 1.9f));

            LogicSet walkAwayLS = new LogicSet(this);
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Move01", "Blob_Move02", "Blob_Move03", "Blank", "Blank", "Blank", "Blank", "Blank"));
            walkAwayLS.AddAction(new DelayLogicAction(1.0f, 1.5f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(0.5f,0.9f));

            LogicSet jumpLS = new LogicSet(this);
            jumpLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpLS.AddAction(new GroundCheckLogicAction());
            jumpLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobJump_Character", false, false));
            jumpLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeJump", false));
            jumpLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpLS.AddAction(new MoveLogicAction(m_target, true, Speed * 6.75f));
            jumpLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Jump"));
            jumpLS.AddAction(new JumpLogicAction());
            jumpLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobAir_Character", true, true));
            jumpLS.AddAction(new DelayLogicAction(0.2f)); // Needs a delay before checking ground collision, otherwise it won't be off the ground before it's flagged as true.
            jumpLS.AddAction(new GroundCheckLogicAction());
            jumpLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Land"));
            jumpLS.AddAction(new MoveLogicAction(m_target, true, Speed)); //Reverting speed back
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobJump_Character", false, false));
            jumpLS.AddAction(new PlayAnimationLogicAction("Start", "Jump", false));
            jumpLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobIdle_Character", true, true));
            jumpLS.AddAction(new DelayLogicAction(0.2f));
            jumpLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpLS.Tag = GameTypes.LogicSetType_ATTACK;

            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "SpellDamageShield_Sprite",
                SourceAnchor = new Vector2(0, 10),
                //Target = m_target,
                Speed = new Vector2(ProjectileSpeed, ProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                Angle = new Vector2(0, 0),
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = ProjectileScale,
                Lifespan = ExpertBlobProjectileDuration,
                LockPosition = true,
            };

            LogicSet jumpAdvancedLS = new LogicSet(this);
            jumpAdvancedLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpAdvancedLS.AddAction(new GroundCheckLogicAction());
            jumpAdvancedLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobJump_Character", false, false));
            jumpAdvancedLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeJump", false));
            jumpAdvancedLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpAdvancedLS.AddAction(new MoveLogicAction(m_target, true, Speed * 6.75f));
            jumpAdvancedLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Jump"));
            jumpAdvancedLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            jumpAdvancedLS.AddAction(new JumpLogicAction());
            jumpAdvancedLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobAir_Character", true, true));
            jumpAdvancedLS.AddAction(new DelayLogicAction(0.2f)); // Needs a delay before checking ground collision, otherwise it won't be off the ground before it's flagged as true.
            jumpAdvancedLS.AddAction(new GroundCheckLogicAction());
            jumpAdvancedLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Land"));
            jumpAdvancedLS.AddAction(new MoveLogicAction(m_target, true, Speed)); //Reverting speed back
            jumpAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobJump_Character", false, false));
            jumpAdvancedLS.AddAction(new PlayAnimationLogicAction("Start", "Jump", false));
            jumpAdvancedLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobIdle_Character", true, true));
            jumpAdvancedLS.AddAction(new DelayLogicAction(0.2f));
            jumpAdvancedLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpAdvancedLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet jumpExpertLS = new LogicSet(this);
            jumpExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpExpertLS.AddAction(new GroundCheckLogicAction());
            jumpExpertLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobJump_Character", false, false));
            jumpExpertLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeJump", false));
            jumpExpertLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpExpertLS.AddAction(new MoveLogicAction(m_target, true, Speed * 6.75f));
            jumpExpertLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Jump"));
            jumpExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            jumpExpertLS.AddAction(new JumpLogicAction());
            jumpExpertLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobAir_Character", true, true));
            jumpExpertLS.AddAction(new DelayLogicAction(0.2f));
            jumpExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            jumpExpertLS.AddAction(new DelayLogicAction(0.2f));
            jumpExpertLS.AddAction(new FireProjectileLogicAction(m_levelScreen.ProjectileManager, projData));
            jumpExpertLS.AddAction(new DelayLogicAction(0.2f)); // Needs a delay before checking ground collision, otherwise it won't be off the ground before it's flagged as true.
            jumpExpertLS.AddAction(new GroundCheckLogicAction());
            jumpExpertLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Land"));
            jumpExpertLS.AddAction(new MoveLogicAction(m_target, true, Speed)); //Reverting speed back
            jumpExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobJump_Character", false, false));
            jumpExpertLS.AddAction(new PlayAnimationLogicAction("Start", "Jump", false));
            jumpExpertLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobIdle_Character", true, true));
            jumpExpertLS.AddAction(new DelayLogicAction(0.2f));
            jumpExpertLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpExpertLS.Tag = GameTypes.LogicSetType_ATTACK;

            ///////////////// BOSS LOGIC SETS /////////////////////////

            LogicSet walkTowardsBossLS = new LogicSet(this);
            walkTowardsBossLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsBossLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Move01", "Blob_Move02", "Blob_Move03", "Blank", "Blank", "Blank", "Blank", "Blank"));
            walkTowardsBossLS.AddAction(new DelayLogicAction(1.10f, 1.9f));

            LogicSet walkAwayBossLS = new LogicSet(this);
            walkAwayBossLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayBossLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Move01", "Blob_Move02", "Blob_Move03", "Blank", "Blank", "Blank", "Blank", "Blank"));
            walkAwayBossLS.AddAction(new DelayLogicAction(1.0f, 1.5f));

            LogicSet walkStopBossLS = new LogicSet(this);
            walkStopBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopBossLS.AddAction(new DelayLogicAction(0.5f, 0.9f));

            LogicSet jumpBossLS = new LogicSet(this);
            jumpBossLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpBossLS.AddAction(new GroundCheckLogicAction());
            jumpBossLS.AddAction(new MoveLogicAction(m_target, true, 0));
            jumpBossLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobBossJump_Character", false, false));
            jumpBossLS.AddAction(new PlayAnimationLogicAction("Start", "BeforeJump", false));
            jumpBossLS.AddAction(new DelayLogicAction(JumpDelay));
            jumpBossLS.AddAction(new MoveLogicAction(m_target, true, Speed * 6.75f));
            jumpBossLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Jump"));
            jumpBossLS.AddAction(new JumpLogicAction());
            jumpBossLS.AddAction(new LockFaceDirectionLogicAction(true));
            jumpBossLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobBossAir_Character", true, true));
            jumpBossLS.AddAction(new DelayLogicAction(0.2f)); // Needs a delay before checking ground collision, otherwise it won't be off the ground before it's flagged as true.
            jumpBossLS.AddAction(new GroundCheckLogicAction());
            jumpBossLS.AddAction(new Play3DSoundLogicAction(this, m_target, "Blob_Land"));
            jumpBossLS.AddAction(new MoveLogicAction(m_target, true, Speed)); //Reverting speed back
            jumpBossLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobBossJump_Character", false, false));
            jumpBossLS.AddAction(new PlayAnimationLogicAction("Start", "Jump", false));
            jumpBossLS.AddAction(new ChangeSpriteLogicAction("EnemyBlobBossIdle_Character", true, true));
            jumpBossLS.AddAction(new DelayLogicAction(0.2f));
            jumpBossLS.AddAction(new LockFaceDirectionLogicAction(false));
            jumpBossLS.Tag = GameTypes.LogicSetType_ATTACK;

            LogicSet neoChaseLB = new LogicSet(this);
            neoChaseLB.AddAction(new ChangeSpriteLogicAction("EnemyBlobBossAir_Character", true, true));
            neoChaseLB.AddAction(new Play3DSoundLogicAction(this, Game.ScreenManager.Player, "FairyMove1", "FairyMove2", "FairyMove3"));
            neoChaseLB.AddAction(new ChaseLogicAction(m_target, true, 1.0f));

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, jumpLS);
            m_generalAdvancedLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, jumpAdvancedLS);
            m_generalExpertLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS, jumpExpertLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            m_generalMiniBossLB.AddLogicSet(walkTowardsBossLS, walkAwayBossLS, walkStopBossLS, jumpBossLS);

            m_generalNeoLB.AddLogicSet(neoChaseLB);

            m_generalBossCooldownLB.AddLogicSet(walkTowardsBossLS, walkAwayBossLS, walkStopBossLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalAdvancedLB);
            logicBlocksToDispose.Add(m_generalExpertLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);
            logicBlocksToDispose.Add(m_generalNeoLB);

            logicBlocksToDispose.Add(m_generalMiniBossLB);
            logicBlocksToDispose.Add(m_generalBossCooldownLB);

            if (Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
            {
                SetCooldownLogicBlock(m_generalBossCooldownLB, 40, 40, 20); //walkTowardsLS, walkAwayLS, walkStopLS
                this.ChangeSprite("EnemyBlobBossIdle_Character");
            }
            else
                SetCooldownLogicBlock(m_generalCooldownLB, 40, 40, 20); //walkTowardsLS, walkAwayLS, walkStopLS

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            if (m_isTouchingGround == true)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                    case (STATE_ENGAGE):
                            RunLogicBlock(true, m_generalBasicLB, 45, 0, 0, 55); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    case (STATE_WANDER):
                            RunLogicBlock(true, m_generalBasicLB, 10, 10, 75, 5); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void RunAdvancedLogic()
        {
            if (m_isTouchingGround == true)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalAdvancedLB, 45, 0, 0, 55); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalAdvancedLB, 10, 10, 75, 5); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void RunExpertLogic()
        {
            if (m_isTouchingGround == true)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                    case (STATE_ENGAGE):
                        RunLogicBlock(true, m_generalExpertLB, 45, 0, 0, 55); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    case (STATE_WANDER):
                        RunLogicBlock(true, m_generalExpertLB, 10, 10, 75, 5); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void RunMinibossLogic()
        {
            if (m_isTouchingGround)
            {
                switch (State)
                {
                    case (STATE_MELEE_ENGAGE):
                    case (STATE_PROJECTILE_ENGAGE):
                    case (STATE_ENGAGE):
                    case (STATE_WANDER):
                        if (IsNeo == false)
                            RunLogicBlock(true, m_generalMiniBossLB, 45, 0, 0, 55); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (IsNeo == true)
                {
                    this.AnimationDelay = 1 / 10f;
                    RunLogicBlock(true, m_generalNeoLB, 100); //walkTowardsLS, walkAwayLS, walkStopLS, jumpLS
                }
            }

        }

        public EnemyObj_Blob(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyBlobIdle_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            MainBlob = true;
            this.TintablePart = _objectList[0];
            this.PlayAnimation(true);
            m_invincibleCounter = 0.5f; // Hack to make sure newly created blobs are invincible for a short period.
            this.Type = EnemyType.Blob;
        }


        public void SetNumberOfHits(int hits)
        {
            this.NumHits = hits;
        }

        private void CreateBlob(GameTypes.EnemyDifficulty difficulty, int numHits, bool isNeo = false)
        {
            EnemyObj_Blob blob = new EnemyObj_Blob(null, null, null, difficulty);
            blob.InitializeEV();
            blob.Position = this.Position;
            if (m_target.X < blob.X)
                blob.Orientation = MathHelper.ToRadians(0);
            else
                blob.Orientation = MathHelper.ToRadians(180);

            // Each subsequent blob gets smaller and faster.
            blob.Level = this.Level; // Must be called before AddEnemyToCurrentRoom() to initialize levels properly.
            m_levelScreen.AddEnemyToCurrentRoom(blob);
            blob.Scale = new Vector2(ScaleX * BlobSizeChange.X, ScaleY * BlobSizeChange.Y);
            blob.SetNumberOfHits(numHits); // Must be set after the call to AddEnemyToCurrentRoom because it calls Initialize(), which overrides SetNumberOfHits.
            blob.Speed *= BlobSpeedChange;
            blob.MainBlob = false;
            blob.SavedStartingPos = blob.Position;

            blob.IsNeo = isNeo;
            if (isNeo == true)
            {
                blob.Name = this.Name;
                blob.LocStringID = this.LocStringID;
                blob.IsWeighted = false;
                blob.TurnSpeed = this.TurnSpeed;
                blob.Speed = this.Speed * BlobSpeedChange;
                blob.Level = this.Level;
                blob.MaxHealth = this.MaxHealth;
                blob.CurrentHealth = blob.MaxHealth;
                blob.Damage = this.Damage;
                blob.ChangeNeoStats(BlobSizeChange.X, BlobSpeedChange, numHits);
            }
            
            int randXKnockback = CDGMath.RandomInt(-500, -300);
            int randYKnockback = CDGMath.RandomInt(300, 700);

            if (blob.X < m_target.X)
                blob.AccelerationX += -(m_target.EnemyKnockBack.X + randXKnockback);
            else
                blob.AccelerationX += (m_target.EnemyKnockBack.X + randXKnockback);
            blob.AccelerationY += -(m_target.EnemyKnockBack.Y + randYKnockback);

            if (blob.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
            {
                for (int i = 0; i < this.NumChildren; i++)
                {
                    blob.GetChildAt(i).Opacity = this.GetChildAt(i).Opacity;
                    blob.GetChildAt(i).TextureColor = this.GetChildAt(i).TextureColor;
                }
                blob.ChangeSprite("EnemyBlobBossAir_Character");
            }
            else
                blob.ChangeSprite("EnemyBlobAir_Character");
            blob.PlayAnimation(true);
            if (LevelEV.SHOW_ENEMY_RADII == true)
                blob.InitializeDebugRadii();
            blob.SaveToFile = false;
            blob.SpawnRoom = m_levelScreen.CurrentRoom;
            blob.GivesLichHealth = false;

            //if (this.IsPaused)
            //    blob.PauseEnemy();
        }

        public void CreateWizard()
        {
            EnemyObj_EarthWizard wizard = new EnemyObj_EarthWizard(null, null, null, GameTypes.EnemyDifficulty.ADVANCED);

            //int randomUnit = CDGMath.RandomInt(1, 3);
            //if (randomUnit == 1)
            //    wizard = new EnemyObj_EarthWizard(null, null, null, GameTypes.EnemyDifficulty.ADVANCED);
            //else if (randomUnit == 2)
            //    wizard = new EnemyObj_FireWizard(null, null, null, GameTypes.EnemyDifficulty.ADVANCED);
            //else
            //    wizard = new EnemyObj_IceWizard(null, null, null, GameTypes.EnemyDifficulty.ADVANCED);

            wizard.PublicInitializeEV();
            wizard.Position = this.Position;
            if (m_target.X < wizard.X)
                wizard.Orientation = MathHelper.ToRadians(0);
            else
                wizard.Orientation = MathHelper.ToRadians(180);

            // Each subsequent blob gets smaller and faster.
            wizard.Level = this.Level; // Must be called before AddEnemyToCurrentRoom() to initialize levels properly.
            wizard.Level -= m_bossEarthWizardLevelReduction;
            m_levelScreen.AddEnemyToCurrentRoom(wizard);
            wizard.SavedStartingPos = wizard.Position;

            int randXKnockback = CDGMath.RandomInt(-500, -300);
            int randYKnockback = CDGMath.RandomInt(300, 700);

            if (wizard.X < m_target.X)
                wizard.AccelerationX += -(m_target.EnemyKnockBack.X + randXKnockback);
            else
                wizard.AccelerationX += (m_target.EnemyKnockBack.X + randXKnockback);
            wizard.AccelerationY += -(m_target.EnemyKnockBack.Y + randYKnockback);

            wizard.PlayAnimation(true);
            if (LevelEV.SHOW_ENEMY_RADII == true)
                wizard.InitializeDebugRadii();
            wizard.SaveToFile = false;
            wizard.SpawnRoom = m_levelScreen.CurrentRoom;
            wizard.GivesLichHealth = false;

            //if (this.IsPaused)
            //    wizard.PauseEnemy();
        }

        //public override void Update(GameTime gameTime)
        //{
        //    foreach (GameObj obj in m_levelScreen.CurrentRoom.ObjectList)
        //    {
        //        EnemyObj_Blob blob = obj as EnemyObj_Blob;
        //        if (blob != null && blob != this)
        //        {
        //            float distanceFromOtherEnemy = Vector2.Distance(this.Position, blob.Position);
        //            if (distanceFromOtherEnemy < this.SeparationDistance)
        //            {
        //                Vector2 seekPosition = 2 * this.Position - blob.Position;
        //                CDGMath.TurnToFace(this, seekPosition);
        //            }
        //        }
        //    }

        //    base.Update(gameTime);
        //}

        public override void Update(GameTime gameTime)
        {
            // Maintains the enemy's speed in the air so that he can jump onto platforms.
            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
            {
                if (m_isTouchingGround == false && IsWeighted == true && CurrentSpeed == 0 && this.SpriteName == "EnemyBlobBossAir_Character")
                    this.CurrentSpeed = this.Speed;

                if (m_currentActiveLB.IsActive == false && m_isTouchingGround == true && SpriteName != "EnemyBlobBossIdle_Character")
                {
                    this.ChangeSprite("EnemyBlobBossIdle_Character");
                    this.PlayAnimation(true);
                }
            }
            else
            {
                if (m_isTouchingGround == false && IsWeighted == true && CurrentSpeed == 0 && this.SpriteName == "EnemyBlobAir_Character")
                    this.CurrentSpeed = this.Speed;

                if (m_currentActiveLB.IsActive == false && m_isTouchingGround == true && SpriteName != "EnemyBlobIdle_Character")
                {
                    this.ChangeSprite("EnemyBlobIdle_Character");
                    this.PlayAnimation(true);
                }
            }

            if (IsNeo == true) // Anti-flocking logic.
            {
                foreach (EnemyObj enemy in m_levelScreen.CurrentRoom.EnemyList)
                {
                    if (enemy != this && enemy is EnemyObj_Blob)
                    {
                        float distanceFromOtherEnemy = Vector2.Distance(this.Position, enemy.Position);
                        if (distanceFromOtherEnemy < 150)
                        {
                            Vector2 seekPosition = 2 * this.Position - enemy.Position;
                            CDGMath.TurnToFace(this, seekPosition);
                        }
                    }
                }

                foreach (EnemyObj enemy in m_levelScreen.CurrentRoom.TempEnemyList)
                {
                    if (enemy != this && enemy is EnemyObj_Blob)
                    {
                        float distanceFromOtherEnemy = Vector2.Distance(this.Position, enemy.Position);
                        if (distanceFromOtherEnemy < 150)
                        {
                            Vector2 seekPosition = 2 * this.Position - enemy.Position;
                            CDGMath.TurnToFace(this, seekPosition);
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer = true)
        {
            if (m_target != null && m_target.CurrentHealth > 0)
            {
                if (m_bossVersionKilled == false)
                {
                    base.HitEnemy(damage, position, isPlayer);

                    if (CurrentHealth <= 0)
                    {
                        CurrentHealth = MaxHealth;
                        NumHits--;

                        if (IsKilled == false && NumHits > 0)
                        {
                            if (IsNeo == false)
                            {
                                if (m_flipTween != null && m_flipTween.TweenedObject == this && m_flipTween.Active == true)
                                    m_flipTween.StopTween(false);
                                this.ScaleX = this.ScaleY; // This is to fix splitting a blob when it's stereoblind flipping.

                                CreateBlob(this.Difficulty, NumHits);
                                this.Scale = new Vector2(ScaleX * BlobSizeChange.X, ScaleY * BlobSizeChange.Y);
                                this.Speed *= BlobSpeedChange;

                                if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                                    CreateWizard();
                            }
                            else
                            {
                                if (m_flipTween != null && m_flipTween.TweenedObject == this && m_flipTween.Active == true)
                                    m_flipTween.StopTween(false);
                                this.ScaleX = this.ScaleY; // This is to fix splitting a blob when it's stereoblind flipping.

                                CreateBlob(this.Difficulty, NumHits, true);
                                this.Scale = new Vector2(ScaleX * BlobSizeChange.X, ScaleY * BlobSizeChange.Y);
                                this.Speed *= BlobSpeedChange;
                            }
                        }
                    }

                    if (NumHits <= 0)
                        Kill(true);
                }
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            // Hits the player in Tanooki mode.
            if (this.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS && m_bossVersionKilled == false)
            {
                PlayerObj player = otherBox.AbsParent as PlayerObj;
                if (player != null && otherBox.Type == Consts.WEAPON_HITBOX && player.IsInvincible == false && player.State == PlayerObj.STATE_TANOOKI)
                    player.HitPlayer(this);
            }

            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void Kill(bool giveXP = true)
        {
            if (Difficulty != GameTypes.EnemyDifficulty.MINIBOSS)
                base.Kill(giveXP);
            else
            {
                if (m_target.CurrentHealth > 0)
                {
                    BlobBossRoom bossRoom = m_levelScreen.CurrentRoom as BlobBossRoom;
                    BlobChallengeRoom challengeRoom = m_levelScreen.CurrentRoom as BlobChallengeRoom;

                    if (((bossRoom != null && bossRoom.NumActiveBlobs == 1) || (challengeRoom != null && challengeRoom.NumActiveBlobs == 1)) && m_bossVersionKilled == false)
                    {
                        Game.PlayerStats.BlobBossBeaten = true;

                        SoundManager.StopMusic(0);
                        //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"PressStart");
                        m_bossVersionKilled = true;
                        m_target.LockControls();
                        m_levelScreen.PauseScreen();
                        m_levelScreen.ProjectileManager.DestroyAllProjectiles(false);
                        m_levelScreen.RunWhiteSlashEffect();
                        Tween.RunFunction(1f, this, "Part2");
                        SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Boss_Flash");
                        SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Boss_Eyeball_Freeze");

                        GameUtil.UnlockAchievement("FEAR_OF_SLIME");

                        if (IsNeo == true)
                        {
                            Tween.To(m_target.AttachedLevel.Camera, 0.5f, Tweener.Ease.Quad.EaseInOut, "Zoom", "1", "X", m_target.X.ToString(), "Y", m_target.Y.ToString());
                            Tween.AddEndHandlerToLastTween(this, "LockCamera");
                        }
                    }
                    else
                        base.Kill(giveXP);
                }
            }
        }

        public void LockCamera()
        {
            m_target.AttachedLevel.CameraLockedToPlayer = true;
        }

        public void Part2()
        {
            m_levelScreen.UnpauseScreen();
            m_target.UnlockControls();

            if (m_currentActiveLB != null)
                m_currentActiveLB.StopLogicBlock();

            m_target.CurrentSpeed = 0;
            m_target.ForceInvincible = true;

            foreach (EnemyObj enemy in m_levelScreen.CurrentRoom.TempEnemyList)
            {
                if (enemy.IsKilled == false)
                    enemy.Kill(true);
            }

            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Boss_Blob_Death");

            base.Kill(true);

            if (IsNeo == false)
            {
                int totalGold = m_bossCoins + m_bossMoneyBags + m_bossDiamonds;
                List<int> goldArray = new List<int>();

                for (int i = 0; i < m_bossCoins; i++)
                    goldArray.Add(0);
                for (int i = 0; i < m_bossMoneyBags; i++)
                    goldArray.Add(1);
                for (int i = 0; i < m_bossDiamonds; i++)
                    goldArray.Add(2);

                CDGMath.Shuffle<int>(goldArray);
                float coinDelay = 0;// 2.5f / goldArray.Count; // The enemy dies for 2.5 seconds.

                for (int i = 0; i < goldArray.Count; i++)
                {
                    Vector2 goldPos = this.Position;
                    if (goldArray[i] == 0)
                        Tween.RunFunction(i * coinDelay, m_levelScreen.ItemDropManager, "DropItemWide", goldPos, ItemDropType.Coin, ItemDropType.CoinAmount);
                    else if (goldArray[i] == 1)
                        Tween.RunFunction(i * coinDelay, m_levelScreen.ItemDropManager, "DropItemWide", goldPos, ItemDropType.MoneyBag, ItemDropType.MoneyBagAmount);
                    else
                        Tween.RunFunction(i * coinDelay, m_levelScreen.ItemDropManager, "DropItemWide", goldPos, ItemDropType.Diamond, ItemDropType.DiamondAmount);
                }
            }
        }

        public override void Reset()
        {
            if (MainBlob == false)
            {
                //m_levelScreen.RemoveEnemyFromCurrentRoom(this, this.SavedStartingPos);
                m_levelScreen.RemoveEnemyFromRoom(this, SpawnRoom, this.SavedStartingPos);
                this.Dispose();
            }
            else
            {
                //this.Scale = new Vector2(1, 1);
                switch (this.Difficulty)
                {
                    case (GameTypes.EnemyDifficulty.BASIC):
                        this.Speed = EnemyEV.Blob_Basic_Speed; // Should not be hardcoded.
                        this.Scale = EnemyEV.Blob_Basic_Scale;
                        this.NumHits = 2;
                        break;
                    case (GameTypes.EnemyDifficulty.ADVANCED):
                        this.Speed = EnemyEV.Blob_Advanced_Speed; // Should not be hardcoded.
                        this.Scale = EnemyEV.Blob_Advanced_Scale;
                        this.NumHits = 3;
                        break;
                    case (GameTypes.EnemyDifficulty.EXPERT):
                        this.Speed = EnemyEV.Blob_Expert_Speed; // Should not be hardcoded.
                        this.Scale = EnemyEV.Blob_Expert_Scale;
                        this.NumHits = 4;
                        break;
                    case (GameTypes.EnemyDifficulty.MINIBOSS):
                        this.Speed = EnemyEV.Blob_Miniboss_Speed; // Should not be hardcoded.
                        this.NumHits = 6;
                        break;
                }
                base.Reset();
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed == false)
            {
                // Done
                SpawnRoom = null;
                base.Dispose();
            }
        }

        public void ChangeNeoStats(float blobSizeChange, float blobSpeedChange, int numHits)
        {
            NumHits = numHits;
            BlobSizeChange = new Vector2(blobSizeChange, blobSizeChange);
            BlobSpeedChange = blobSpeedChange;
        }

        public bool IsNeo
        {
            get { return m_isNeo; }
            set
            {
                m_isNeo = value;
                if (value == true)
                {
                    HealthGainPerLevel = 0;
                    DamageGainPerLevel = 0;
                    MoneyDropChance = 0;
                    ItemDropChance = 0;
                    m_saveToEnemiesKilledList = false;
                }
            }
        }
    }
}

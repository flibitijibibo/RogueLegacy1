using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_BallAndChain : EnemyObj
    {
        private LogicBlock m_generalBasicLB = new LogicBlock();
        private LogicBlock m_generalCooldownLB = new LogicBlock();

        private ProjectileObj m_ballAndChain, m_ballAndChain2;
        private SpriteObj m_chain;
        public float ChainSpeed { get; set; }
        public float ChainSpeed2Modifier { get; set; }

        private int m_numChainLinks = 10;//15;
        private List<Vector2> m_chainLinksList;
        private List<Vector2> m_chainLinks2List;
        private float m_chainRadius;
        private float m_actualChainRadius; // The radius of the actual chain as it grows to reach m_chainRadius.
        private float m_ballAngle = 0;
        private float m_chainLinkDistance;
        private float m_BallSpeedDivider = 1.5f; //The amount the ball speeds get slowed down by (divided).
        private FrameSoundObj m_walkSound, m_walkSound2;

        protected override void InitializeEV()
        {
            ChainSpeed = 2.5f;
            ChainRadius = 260;
            ChainSpeed2Modifier = 1.5f;

            #region Basic Variables - General
            Name = EnemyEV.BallAndChain_Basic_Name;
            LocStringID = EnemyEV.BallAndChain_Basic_Name_locID;

            MaxHealth = EnemyEV.BallAndChain_Basic_MaxHealth;
            Damage = EnemyEV.BallAndChain_Basic_Damage;
            XPValue = EnemyEV.BallAndChain_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.BallAndChain_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.BallAndChain_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.BallAndChain_Basic_DropChance;

            Speed = EnemyEV.BallAndChain_Basic_Speed;
            TurnSpeed = EnemyEV.BallAndChain_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.BallAndChain_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.BallAndChain_Basic_Jump;
            CooldownTime = EnemyEV.BallAndChain_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.BallAndChain_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.BallAndChain_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.BallAndChain_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.BallAndChain_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.BallAndChain_Basic_IsWeighted;

            Scale = EnemyEV.BallAndChain_Basic_Scale;
            ProjectileScale = EnemyEV.BallAndChain_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.BallAndChain_Basic_Tint;

            MeleeRadius = EnemyEV.BallAndChain_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.BallAndChain_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.BallAndChain_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.BallAndChain_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.BallAndChain_Miniboss_Name;
                    LocStringID = EnemyEV.BallAndChain_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.BallAndChain_Miniboss_MaxHealth;
                    Damage = EnemyEV.BallAndChain_Miniboss_Damage;
                    XPValue = EnemyEV.BallAndChain_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.BallAndChain_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.BallAndChain_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.BallAndChain_Miniboss_DropChance;

                    Speed = EnemyEV.BallAndChain_Miniboss_Speed;
                    TurnSpeed = EnemyEV.BallAndChain_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.BallAndChain_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.BallAndChain_Miniboss_Jump;
                    CooldownTime = EnemyEV.BallAndChain_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.BallAndChain_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.BallAndChain_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.BallAndChain_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.BallAndChain_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.BallAndChain_Miniboss_IsWeighted;

                    Scale = EnemyEV.BallAndChain_Miniboss_Scale;
                    ProjectileScale = EnemyEV.BallAndChain_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.BallAndChain_Miniboss_Tint;

                    MeleeRadius = EnemyEV.BallAndChain_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.BallAndChain_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.BallAndChain_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.BallAndChain_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    ChainRadius = 350;
                    ChainSpeed2Modifier = 1.5f;

                    #region Expert Variables - General
                    Name = EnemyEV.BallAndChain_Expert_Name;
                    LocStringID = EnemyEV.BallAndChain_Expert_Name_locID;

                    MaxHealth = EnemyEV.BallAndChain_Expert_MaxHealth;
                    Damage = EnemyEV.BallAndChain_Expert_Damage;
                    XPValue = EnemyEV.BallAndChain_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.BallAndChain_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.BallAndChain_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.BallAndChain_Expert_DropChance;

                    Speed = EnemyEV.BallAndChain_Expert_Speed;
                    TurnSpeed = EnemyEV.BallAndChain_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.BallAndChain_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.BallAndChain_Expert_Jump;
                    CooldownTime = EnemyEV.BallAndChain_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.BallAndChain_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.BallAndChain_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.BallAndChain_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.BallAndChain_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.BallAndChain_Expert_IsWeighted;

                    Scale = EnemyEV.BallAndChain_Expert_Scale;
                    ProjectileScale = EnemyEV.BallAndChain_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.BallAndChain_Expert_Tint;

                    MeleeRadius = EnemyEV.BallAndChain_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.BallAndChain_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.BallAndChain_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.BallAndChain_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    ChainRadius = 275;
                    #region Advanced Variables - General
                    Name = EnemyEV.BallAndChain_Advanced_Name;
                    LocStringID = EnemyEV.BallAndChain_Advanced_Name_locID;

                    MaxHealth = EnemyEV.BallAndChain_Advanced_MaxHealth;
                    Damage = EnemyEV.BallAndChain_Advanced_Damage;
                    XPValue = EnemyEV.BallAndChain_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.BallAndChain_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.BallAndChain_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.BallAndChain_Advanced_DropChance;

                    Speed = EnemyEV.BallAndChain_Advanced_Speed;
                    TurnSpeed = EnemyEV.BallAndChain_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.BallAndChain_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.BallAndChain_Advanced_Jump;
                    CooldownTime = EnemyEV.BallAndChain_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.BallAndChain_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.BallAndChain_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.BallAndChain_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.BallAndChain_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.BallAndChain_Advanced_IsWeighted;

                    Scale = EnemyEV.BallAndChain_Advanced_Scale;
                    ProjectileScale = EnemyEV.BallAndChain_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.BallAndChain_Advanced_Tint;

                    MeleeRadius = EnemyEV.BallAndChain_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.BallAndChain_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.BallAndChain_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.BallAndChain_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }

            // Ball and chain has 2 tintable colour parts.
            _objectList[1].TextureColor = TintablePart.TextureColor;

            m_ballAndChain.Damage = Damage;
            m_ballAndChain.Scale = ProjectileScale;

            m_ballAndChain2.Damage = Damage;
            m_ballAndChain2.Scale = ProjectileScale;
        }

        protected override void InitializeLogic()
        {
            LogicSet walkTowardsLS = new LogicSet(this);
            //walkTowardsLS.AddAction(new PlayAnimationLogicAction(true));
            walkTowardsLS.AddAction(new MoveLogicAction(m_target, true));
            walkTowardsLS.AddAction(new DelayLogicAction(1.25f, 2.75f));

            LogicSet walkAwayLS = new LogicSet(this);
            //walkAwayLS.AddAction(new PlayAnimationLogicAction(true));
            walkAwayLS.AddAction(new MoveLogicAction(m_target, false));
            walkAwayLS.AddAction(new DelayLogicAction(1.25f, 2.75f));

            LogicSet walkStopLS = new LogicSet(this);
            walkStopLS.AddAction(new StopAnimationLogicAction());
            walkStopLS.AddAction(new MoveLogicAction(m_target, true, 0));
            walkStopLS.AddAction(new DelayLogicAction(1.0f, 1.5f));

            m_generalBasicLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);
            m_generalCooldownLB.AddLogicSet(walkTowardsLS, walkAwayLS, walkStopLS);

            logicBlocksToDispose.Add(m_generalBasicLB);
            logicBlocksToDispose.Add(m_generalCooldownLB);

            SetCooldownLogicBlock(m_generalCooldownLB, 40, 40, 20); //walkTowardsLS, walkAwayLS, walkStopLS

            base.InitializeLogic();
        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                    RunLogicBlock(true, m_generalBasicLB, 60, 20, 20); //walkTowardsLS, walkAwayLS, walkStopLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS
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
                    RunLogicBlock(true, m_generalBasicLB, 60, 20, 20); //walkTowardsLS, walkAwayLS, walkStopLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS
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
                    RunLogicBlock(true, m_generalBasicLB, 60, 20, 20); //walkTowardsLS, walkAwayLS, walkStopLS
                    break;
                case (STATE_WANDER):
                    RunLogicBlock(true, m_generalBasicLB, 0, 0, 100); //walkTowardsLS, walkAwayLS, walkStopLS
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

        public EnemyObj_BallAndChain(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyFlailKnight_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            m_ballAndChain = new ProjectileObj("EnemyFlailKnightBall_Sprite"); // At this point, physicsManager is null.
            m_ballAndChain.IsWeighted = false;
            m_ballAndChain.CollidesWithTerrain = false;
            m_ballAndChain.IgnoreBoundsCheck = true;
            m_ballAndChain.OutlineWidth = 2;

            m_ballAndChain2 = m_ballAndChain.Clone() as ProjectileObj;

            m_chain = new SpriteObj("EnemyFlailKnightLink_Sprite");

            m_chainLinksList = new List<Vector2>();
            m_chainLinks2List = new List<Vector2>();
            for (int i = 0; i < m_numChainLinks; i++)
            {
                m_chainLinksList.Add(new Vector2());
            }

            for (int i = 0; i < (int)(m_numChainLinks / 2); i++)
            {
                m_chainLinks2List.Add(new Vector2());
            }
            this.Type = EnemyType.BallAndChain;

            this.TintablePart = _objectList[3];

            m_walkSound = new FrameSoundObj(this, m_target, 1, "KnightWalk1", "KnightWalk2");
            m_walkSound2 = new FrameSoundObj(this, m_target, 6, "KnightWalk1", "KnightWalk2");
        }

        public override void Update(GameTime gameTime)
        {
            if (IsPaused == false)
            {
                if (IsKilled == false && m_initialDelayCounter <= 0)
                {
                    float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (m_actualChainRadius < ChainRadius)
                    {
                        m_actualChainRadius += elapsedSeconds * 200;
                        m_chainLinkDistance = m_actualChainRadius / m_numChainLinks;
                    }

                    float distance = 0;
                    m_ballAndChain.Position = CDGMath.GetCirclePosition(m_ballAngle, m_actualChainRadius, new Vector2(this.X, this.Bounds.Top));
                    for (int i = 0; i < m_chainLinksList.Count; i++)
                    {
                        m_chainLinksList[i] = CDGMath.GetCirclePosition(m_ballAngle, distance, new Vector2(this.X, this.Bounds.Top));
                        distance += m_chainLinkDistance;
                    }

                    distance = 0;
                    if (Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                        m_ballAndChain2.Position = CDGMath.GetCirclePosition(m_ballAngle * ChainSpeed2Modifier, m_actualChainRadius / 2, new Vector2(this.X, this.Bounds.Top));
                    else if (Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                        m_ballAndChain2.Position = CDGMath.GetCirclePosition(-m_ballAngle * ChainSpeed2Modifier, -m_actualChainRadius / 2, new Vector2(this.X, this.Bounds.Top));
                    for (int i = 0; i < m_chainLinks2List.Count; i++)
                    {
                        if (Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                            m_chainLinks2List[i] = CDGMath.GetCirclePosition(m_ballAngle * ChainSpeed2Modifier, distance, new Vector2(this.X, this.Bounds.Top));
                        else if (Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                            m_chainLinks2List[i] = CDGMath.GetCirclePosition(-m_ballAngle * ChainSpeed2Modifier, -distance, new Vector2(this.X, this.Bounds.Top));
                        distance += m_chainLinkDistance;
                    }

                    m_ballAngle += (ChainSpeed * 60 * elapsedSeconds);

                    if (this.IsAnimating == false && this.CurrentSpeed != 0)
                        this.PlayAnimation(true);
                }

                if (this.SpriteName == "EnemyFlailKnight_Character")
                {
                    m_walkSound.Update();
                    m_walkSound2.Update();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(Camera2D camera)
        {
            if (IsKilled == false)
            {
                foreach (Vector2 chain in m_chainLinksList)
                {
                    m_chain.Position = chain;
                    m_chain.Draw(camera);
                }
                m_ballAndChain.Draw(camera);

                if (Difficulty > GameTypes.EnemyDifficulty.BASIC)
                {
                    foreach (Vector2 chain in m_chainLinks2List)
                    {
                        m_chain.Position = chain;
                        m_chain.Draw(camera);
                    }
                    m_ballAndChain2.Draw(camera);
                }

            }
            base.Draw(camera);
        }

        public override void Kill(bool giveXP = true)
        {
            m_levelScreen.PhysicsManager.RemoveObject(m_ballAndChain);
            EnemyObj_BouncySpike spike = new EnemyObj_BouncySpike(m_target, null, m_levelScreen, this.Difficulty);
            spike.SavedStartingPos = this.Position;
            spike.Position = this.Position; // Set the spike's position to this, so that when the room respawns, the ball appears where the enemy was.
            m_levelScreen.AddEnemyToCurrentRoom(spike);
            spike.Position = m_ballAndChain.Position;
            spike.Speed = ChainSpeed * 200 / m_BallSpeedDivider;

            // Must be called afterward since AddEnemyToCurrentRoom overrides heading.
            spike.HeadingX = (float)Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(m_ballAngle + 90)));
            spike.HeadingY = (float)Math.Sin(MathHelper.WrapAngle(MathHelper.ToRadians(m_ballAngle + 90)));

            if (Difficulty > GameTypes.EnemyDifficulty.BASIC)
            {
                m_levelScreen.PhysicsManager.RemoveObject(m_ballAndChain2);

                EnemyObj_BouncySpike spike2 = new EnemyObj_BouncySpike(m_target, null, m_levelScreen, this.Difficulty);
                spike2.SavedStartingPos = this.Position;
                spike2.Position = this.Position;
                m_levelScreen.AddEnemyToCurrentRoom(spike2);
                spike2.Position = m_ballAndChain2.Position;
                spike2.Speed = ChainSpeed * 200 * ChainSpeed2Modifier / m_BallSpeedDivider;

                if (Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                {
                    spike2.HeadingX = (float)Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(m_ballAngle * ChainSpeed2Modifier + 90)));
                    spike2.HeadingY = (float)Math.Sin(MathHelper.WrapAngle(MathHelper.ToRadians(m_ballAngle * ChainSpeed2Modifier + 90)));
                }
                else if (Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                {
                    spike2.HeadingX = (float)Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(-m_ballAngle * ChainSpeed2Modifier + 90)));
                    spike2.HeadingY = (float)Math.Sin(MathHelper.WrapAngle(MathHelper.ToRadians(-m_ballAngle * ChainSpeed2Modifier + 90)));
                }

                spike2.SpawnRoom = m_levelScreen.CurrentRoom;
                spike2.SaveToFile = false;

                if (this.IsPaused)
                    spike2.PauseEnemy();
            }

            spike.SpawnRoom = m_levelScreen.CurrentRoom;
            spike.SaveToFile = false;

            if (this.IsPaused)
                spike.PauseEnemy();

            base.Kill(giveXP);
        }

        public override void ResetState()
        {
            base.ResetState();

            m_actualChainRadius = 0;
            m_chainLinkDistance = m_actualChainRadius / m_numChainLinks;

            float distance = 0;
            m_ballAndChain.Position = CDGMath.GetCirclePosition(m_ballAngle, m_actualChainRadius, new Vector2(this.X, this.Bounds.Top));
            for (int i = 0; i < m_chainLinksList.Count; i++)
            {
                m_chainLinksList[i] = CDGMath.GetCirclePosition(m_ballAngle, distance, new Vector2(this.X, this.Bounds.Top));
                distance += m_chainLinkDistance;
            }

            distance = 0;
            if (Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                m_ballAndChain2.Position = CDGMath.GetCirclePosition(m_ballAngle * ChainSpeed2Modifier, m_actualChainRadius / 2, new Vector2(this.X, this.Bounds.Top));
            else if (Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                m_ballAndChain2.Position = CDGMath.GetCirclePosition(-m_ballAngle * ChainSpeed2Modifier, -m_actualChainRadius / 2, new Vector2(this.X, this.Bounds.Top));
            for (int i = 0; i < m_chainLinks2List.Count; i++)
            {
                if (Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                    m_chainLinks2List[i] = CDGMath.GetCirclePosition(m_ballAngle * ChainSpeed2Modifier, distance, new Vector2(this.X, this.Bounds.Top));
                else if (Difficulty == GameTypes.EnemyDifficulty.EXPERT)
                    m_chainLinks2List[i] = CDGMath.GetCirclePosition(-m_ballAngle * ChainSpeed2Modifier, -distance, new Vector2(this.X, this.Bounds.Top));
                distance += m_chainLinkDistance;
            }
        }

        public override void HitEnemy(int damage, Vector2 position, bool isPlayer)
        {
            SoundManager.Play3DSound(this, m_target, "Knight_Hit01", "Knight_Hit02", "Knight_Hit03");
            base.HitEnemy(damage, position, isPlayer);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_chain.Dispose();
                m_chain = null;
                m_ballAndChain.Dispose();
                m_ballAndChain = null;
                m_ballAndChain2.Dispose();
                m_ballAndChain2 = null;
                m_chainLinksList.Clear();
                m_chainLinksList = null;
                m_chainLinks2List.Clear();
                m_chainLinks2List = null;
                m_walkSound.Dispose();
                m_walkSound = null;
                m_walkSound2.Dispose();
                m_walkSound2 = null;
                base.Dispose();
            }
        }

        private float ChainRadius
        {
            get { return m_chainRadius; }
            set
            {
                m_chainRadius = value;
                //m_chainLinkDistance = m_chainRadius / m_numChainLinks;
            }
        }

        public ProjectileObj BallAndChain
        {
            get { return m_ballAndChain; }
        }

        public ProjectileObj BallAndChain2
        {
            get { return m_ballAndChain2; }
        }
    }
}

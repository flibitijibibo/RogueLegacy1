using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using InputSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Tweener;

namespace RogueCastle
{
    public class PlayerObj : CharacterObj, IDealsDamageObj
    {
        #region Variables
        private Game m_game;

        #region MapInputs
        private const int DEBUG_INPUT_SWAPWEAPON = 0;
        private const int DEBUG_INPUT_GIVEMANA = 1;
        private const int DEBUG_INPUT_GIVEHEALTH = 2;
        private const int DEBUG_INPUT_LEVELUP_STRENGTH = 3;
        private const int DEBUG_INPUT_LEVELUP_HEALTH = 4;
        private const int DEBUG_INPUT_LEVELUP_ENDURANCE = 5;
        private const int DEBUG_INPUT_LEVELUP_EQUIPLOAD = 6;
        private const int DEBUG_INPUT_TRAITSCREEN = 7;
        private const int DEBUG_UNLOCK_ALL_BLUEPRINTS = 8;
        private const int DEBUG_PURCHASE_ALL_BLUEPRINTS = 9;

        public const int STATE_IDLE = 0;
        public const int STATE_WALKING = 1;
        public const int STATE_JUMPING = 2;
        public const int STATE_HURT = 3;
        public const int STATE_DASHING = 4;
        public const int STATE_LEVELUP = 5;
        public const int STATE_BLOCKING = 6;
        public const int STATE_FLYING = 7;
        public const int STATE_TANOOKI = 8;
        public const int STATE_DRAGON = 9;
        #endregion

        // EVs
        private float JumpDeceleration = 0;
        private int BaseDamage { get; set; }
        public float BaseMana { get; internal set; }
        public float CurrentMana
        {
            get { return m_currentMana; }
            internal set
            {
                m_currentMana = value;

                if (m_currentMana < 0)
                    m_currentMana = 0;
                if (m_currentMana > MaxMana)
                    m_currentMana = MaxMana;
            }
        }
        private float m_currentMana = 0;
        private float DashTime = 0;
        private float DashSpeed = 0;
        private float DashCoolDown = 0;
        private float m_manaGain = 0;
        public int BaseHealth { get; internal set; }
        public float ProjectileLifeSpan { get; internal set; }
        public float AttackDelay { get; internal set; }
        public float BaseInvincibilityTime { get; internal set; }
        public float BaseCriticalChance { get; internal set; }
        public float BaseCriticalDamageMod { get; internal set; }
        public int MaxDamage { get; internal set; }
        public int MinDamage { get; internal set; }
        private float ComboDelay = 1f;
        public int LevelModifier { get; internal set; }
        private float AttackAnimationDelay { get; set; }
        private int StrongDamage { get; set; }
        private Vector2 StrongEnemyKnockBack { get; set; }
        private Vector2 m_enemyKnockBack = Vector2.Zero;
        public float AirAttackKnockBack { get; internal set; }
        public bool IsAirAttacking { get; set; }
        private float AirAttackDamageMod { get; set; }
        private float BaseStatDropChance = 0;
        public float StatDropIncrease { get; set; }
        public float FlightSpeedMod { get; internal set; }

        public int BaseWeight { get; internal set; }
        public float BaseMagicDamage { get; set; }

        public float FlightTime { get; internal set; }
        private float m_flightCounter = 0;
        private bool m_isFlying = false;

        private int DamageGainPerLevel = 0;
        private int ManaGainPerLevel = 0;
        private int HealthGainPerLevel = 0;

        private float m_invincibleCounter = 0;
        private float m_dropThroughGroundTimer = 0;
        private float m_dropThroughGroundDuration = 0.1f; // The length of time you are unable to touch 1-ways.

        private float m_blockInvincibleCounter = 0;
        private float BlockInvincibleTime { get; set; }
        public bool ForceInvincible { get; set; }
        public bool InvincibleToSpikes { get; set; }
        public int NumAirBounces { get; set; }

        //////

        private PlayerIndex m_playerIndex;
        private float m_attackCounter; // Counter to determine how long before an attack combo ends.
        private int m_attackNumber; // Keeps track of the current attack number the player is on.

        private bool m_isJumping;
        //private bool m_airDashed = false;

        private byte m_doubleJumpCount = 0;
        private byte m_airDashCount = 0;

        private float m_dashCounter = 0;
        private float m_dashCooldownCounter = 0;

        private float m_startingAnimationDelay = 0;

        private LogicSet m_currentLogicSet;
        private LogicSet m_standingAttack3LogicSet;
        private LogicSet m_airAttackLS;

        private bool m_lockControls;
        private InputMap m_debugInputMap;

        // The separate parts of the player.
        private SpriteObj m_playerHead;
        private SpriteObj m_playerLegs;
        #endregion

        private FrameSoundObj m_walkUpSound, m_walkUpSoundLow, m_walkUpSoundHigh;
        private FrameSoundObj m_walkDownSound, m_walkDownSoundLow, m_walkDownSoundHigh;
        private TeleporterObj m_lastTouchedTeleporter;

        private LogicSet m_externalLS;

        private TextObj m_flightDurationText;

        private byte m_attacksNeededForMana = 0;
        private byte m_numSequentialAttacks = 0;
        private float m_blockManaDrain = 0;
        private int ArmorReductionMod = 0;
        private float RunSpeedMultiplier = 0;

        private ObjContainer m_translocatorSprite;

        private SpriteObj m_swearBubble;
        private float m_swearBubbleCounter = 0;

        private bool m_previousIsTouchingGround = false;

        private TweenObject m_flipTween;

        // Class effect variables.

        private Color m_skinColour1 = new Color(231, 175, 131, 255);
        private Color m_skinColour2 = new Color(199, 109, 112, 255);
        private Color m_lichColour1 = new Color(255, 255, 255, 255);
        private Color m_lichColour2 = new Color(198, 198, 198, 255);

        private float m_assassinSmokeTimer = 0.5f;
        private bool m_assassinSpecialActive = false;
        private float m_assassinDrainCounter = 0;

        private bool m_timeStopCast = false;
        private float m_timeStopDrainCounter = 0;

        private bool m_megaDamageShieldCast = false;
        private bool m_damageShieldCast = false;
        private float m_damageShieldDrainCounter = 0;

        private float m_tanookiDrainCounter = 0;

        private float m_dragonManaRechargeCounter = 0;

        private bool m_lightOn = false;
        private float m_lightDrainCounter = 0;

        private List<byte> m_wizardSpellList; // This is saved because it is accessed often.
        private float m_wizardSparkleCounter = 0.2f;

        private float m_ninjaTeleportDelay = 0;

        private float m_spellCastDelay = 0;
        private float m_rapidSpellCastDelay = 0;

        private IPhysicsObj m_closestGround;
        private bool m_collidingLeft = false; // Used specifically to make sure player doesn't dig into walls.
        private bool m_collidingRight = false; // Used specifically to make sure player doesn't dig into walls.

        private bool m_collidingLeftOnly = false;
        private bool m_collidingRightOnly = false;

        private float m_ambilevousTimer = 0.5f;

        private Vector2 AxeSpellScale = new Vector2(3.0f, 3.0f);
        private float AxeProjectileSpeed = 1100;
        private Vector2 DaggerSpellScale = new Vector2(3.5f, 3.5f);
        private float DaggerProjectileSpeed = 900;//875;//750;
        private float m_Spell_Close_Lifespan = 6;//8;
        private float m_Spell_Close_Scale = 3.5f;
        private ProjectileData m_axeProjData;
        private ProjectileData m_rapidDaggerProjData;

        protected override void InitializeEV()
        {
            //Scale = new Vector2(0.5f, 0.5f);
            this.ForceDraw = true;
            Speed = 500;//450;// 7.25f;//6.0f; //3.75f;
            RunSpeedMultiplier = 3.0f;//1.6f;
            JumpHeight = 1180;//1147;// 18.5f; //18.25f;//16.0f;//14f;//13.5fl //12.0f;//8;
            DoubleJumpHeight = 845;//837;// 13.5f; //12.5f;//11.5f; //13.5f;
            StepUp = 10;//21;
            BaseHealth = 100;//150; //100;//75;//50;//100;

            JumpDeceleration = 5000;//2050;//1500f;//3.745f;//0.375f;//0.23f;//0.2f;
            KnockBack = new Vector2(300f, 450f);//(5.0f, 7.5f);
            BaseInvincibilityTime = 1.00f; //0.75f; //0.5f;

            BaseWeight = 50;
            BaseMagicDamage = 25; //10;

            //BaseDamage = 25;


            DashSpeed = 900.0f;//850.0f;//13.0f;
            DashTime = 0.325f;//0.25f;
            DashCoolDown = 0.25f; //0.5f;

            ProjectileLifeSpan = 10f;
            AnimationDelay = 1 / 10f; // 45 fps.
            AttackDelay = 0.0f;
            BaseCriticalChance = 0.0f;//0.05f;
            BaseCriticalDamageMod = 1.5f;//1.25f;//1.5f;

            EnemyKnockBack = new Vector2(90f, 90f);//(1.5f, 1.5f);//(3.0f, 5.5f); //(5.0f, 7.5f);
            MinDamage = 25;//5;
            MaxDamage = 25; //10;//25;
            ComboDelay = 1.5f;

            AttackAnimationDelay = 1 / (20f + SkillSystem.GetSkill(SkillType.Attack_Speed_Up).ModifierAmount);//1 / 22f;
            StrongDamage = 25; //(int)(MaxDamage * 3.0f);//1.5f);
            StrongEnemyKnockBack = new Vector2(300f, 360f);//(5.0f, 6.0f);//(8.0f, 8.0f);//(10.0f, 15.5f);

            AirAttackKnockBack = 1425f;//1300f;//900f;//600f;
            AirAttackDamageMod = 0.5f;

            LevelModifier = 999999999;//1000;
            DamageGainPerLevel = 0;
            ManaGainPerLevel = 0;
            HealthGainPerLevel = 0;

            //ProjectileDamage = 10;
            //ProjectileSpeed = 500;

            BlockManaDrain = GameEV.KNIGHT_BLOCK_DRAIN; //25;//10;
            BaseMana = 100;//50; //30;
            AttacksNeededForMana = 1;//5;
            ManaGain = 0;//10;
            BaseStatDropChance = 0.01f;
            ArmorReductionMod = GameEV.ARMOR_DIVIDER;//250;

            BlockInvincibleTime = 1.0f;

            FlightTime = GameEV.RUNE_FLIGHT;//5;
            FlightSpeedMod = GameEV.FLIGHT_SPEED_MOD;
            //Scale = new Vector2(0.5f, 0.5f);
        }

        protected override void InitializeLogic()
        {
            if (m_standingAttack3LogicSet != null)
                m_standingAttack3LogicSet.Dispose();
            m_standingAttack3LogicSet = new LogicSet(this);
            m_standingAttack3LogicSet.AddAction(new ChangeSpriteLogicAction("PlayerAttacking3_Character", false, false));
            m_standingAttack3LogicSet.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", AttackAnimationDelay));
            m_standingAttack3LogicSet.AddAction(new PlayAnimationLogicAction(2, 4));
            m_standingAttack3LogicSet.AddAction(new DelayLogicAction(AttackDelay));
            m_standingAttack3LogicSet.AddAction(new RunFunctionLogicAction(this, "PlayAttackSound"));
            //m_standingAttack3LogicSet.AddAction(new PlaySoundLogicAction("Player_Attack01", "Player_Attack02"));
            m_standingAttack3LogicSet.AddAction(new PlayAnimationLogicAction("AttackStart", "End"));
            m_standingAttack3LogicSet.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", AnimationDelay));

            if (m_airAttackLS != null)
                m_airAttackLS.Dispose();
            m_airAttackLS = new LogicSet(this);
            m_airAttackLS.AddAction(new ChangePropertyLogicAction(this, "IsAirAttacking", true));
            m_airAttackLS.AddAction(new ChangeSpriteLogicAction("PlayerAirAttack_Character", false, false));
            m_airAttackLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", AttackAnimationDelay));
            //m_airAttackLS.AddAction(new Play3DSoundLogicAction(this,  Game.ScreenManager.Player,"Player_AttackDown01", "Player_AttackDown02"));
            m_airAttackLS.AddAction(new RunFunctionLogicAction(this, "PlayAttackSound"));

            m_airAttackLS.AddAction(new PlayAnimationLogicAction("Start", "Start"));
            m_airAttackLS.AddAction(new PlayAnimationLogicAction("Frame 3 Test", "Frame 3 Test"));
            m_airAttackLS.AddAction(new PlayAnimationLogicAction("Attack", "Attack"));
            m_airAttackLS.AddAction(new DelayLogicAction(AttackDelay));
            m_airAttackLS.AddAction(new PlayAnimationLogicAction("Attack", "End"));
            m_airAttackLS.AddAction(new ChangePropertyLogicAction(this, "AnimationDelay", AnimationDelay));
            m_airAttackLS.AddAction(new ChangePropertyLogicAction(this, "IsAirAttacking", false));
        }

        public PlayerObj(string spriteName, PlayerIndex playerIndex, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, Game game)
            : base(spriteName, physicsManager, levelToAttachTo)  // Base is called first.
        {
            m_game = game;
            m_playerLegs = this.GetChildAt(PlayerPart.Legs) as SpriteObj;
            m_playerHead = this.GetChildAt(PlayerPart.Head) as SpriteObj;
            m_playerIndex = playerIndex;

            m_currentLogicSet = new LogicSet(null);

            State = STATE_IDLE;
            CollisionTypeTag = GameTypes.CollisionType_PLAYER;

            m_debugInputMap = new InputMap(PlayerIndex.Two, false);
            InitializeInputMap();

            //m_walkDownSound = new FrameSoundObj(m_playerLegs, 4, "Player_Walk_01", "Player_Walk_02", "Player_Walk_03", "Player_Walk_04");
            //m_walkDownSound = new FrameSoundObj(m_playerLegs, 4, "Player_Run_01", "Player_Run_02", "Player_Run_03");
            m_walkDownSound = new FrameSoundObj(m_playerLegs, 4, "Player_WalkDown01", "Player_WalkDown02");
            //m_walkUpSound = new FrameSoundObj(m_playerLegs, 1, "Player_Walk_05", "Player_Walk_06", "Player_Walk_07");
            //m_walkUpSound = new FrameSoundObj(m_playerLegs, 1, "Player_Run_04", "Player_Run_05");
            m_walkUpSound = new FrameSoundObj(m_playerLegs, 1, "Player_WalkUp01", "Player_WalkUp02");

            m_walkUpSoundHigh = new FrameSoundObj(m_playerLegs, 1, "Player_WalkUp01_High", "Player_WalkUp02_High");
            m_walkDownSoundHigh = new FrameSoundObj(m_playerLegs, 4, "Player_WalkDown01_High", "Player_WalkDown02_High");

            m_walkUpSoundLow = new FrameSoundObj(m_playerLegs, 1, "Player_WalkUp01_Low", "Player_WalkUp02_Low");
            m_walkDownSoundLow = new FrameSoundObj(m_playerLegs, 4, "Player_WalkDown01_Low", "Player_WalkDown02_Low");

            m_externalLS = new LogicSet(null);

            m_flightDurationText = new TextObj(Game.JunicodeFont);
            m_flightDurationText.FontSize = 12;
            m_flightDurationText.Align = Types.TextAlign.Centre;
            m_flightDurationText.DropShadow = new Vector2(2, 2);

            this.OutlineWidth = 2;

            m_translocatorSprite = new ObjContainer("PlayerIdle_Character");
            m_translocatorSprite.Visible = false;
            m_translocatorSprite.OutlineColour = Color.Blue;
            m_translocatorSprite.OutlineWidth = 2;

            m_swearBubble = new SpriteObj("SwearBubble1_Sprite");
            m_swearBubble.Scale = new Vector2(2, 2);
            m_swearBubble.Flip = SpriteEffects.FlipHorizontally;
            m_swearBubble.Visible = false;

            m_axeProjData = new ProjectileData(this)
            {
                SpriteName = "SpellAxe_Sprite",
                SourceAnchor = new Vector2(20, -20),
                Target = null,
                Speed = new Vector2(AxeProjectileSpeed, AxeProjectileSpeed),
                IsWeighted = true,
                RotationSpeed = 10,
                Damage = Damage,
                AngleOffset = 0,
                Angle = new Vector2(-90, -90),//(-72, -72),
                CollidesWithTerrain = false,
                Scale = AxeSpellScale,
                IgnoreInvincibleCounter= true,
            };

            m_rapidDaggerProjData = new ProjectileData(this)
            {
                SpriteName = "SpellDagger_Sprite",
                SourceAnchor = Vector2.Zero,
                Speed = new Vector2(DaggerProjectileSpeed, DaggerProjectileSpeed),
                IsWeighted = false,
                RotationSpeed = 0,
                Damage = Damage,
                AngleOffset = 0,
                CollidesWithTerrain = false,
                Scale = DaggerSpellScale,
                FollowArc = true,
                IgnoreInvincibleCounter = true,
            };
        }

        public void Initialize()
        {
            //InitializeTraits();
            InitializeEV();
            m_startingAnimationDelay = AnimationDelay;
            InitializeLogic();

            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;

            this.Scale = new Vector2(2, 2);
            m_internalScale = this.Scale;

            m_wizardSpellList = new List<byte>();
            m_wizardSpellList.Add((byte)Game.PlayerStats.WizardSpellList.X);
            m_wizardSpellList.Add((byte)Game.PlayerStats.WizardSpellList.Y);
            m_wizardSpellList.Add((byte)Game.PlayerStats.WizardSpellList.Z);
        }

        public void UpdateInternalScale()
        {
            m_internalScale = this.Scale;
        }

        private void InitializeInputMap()
        {
            m_debugInputMap.AddInput(DEBUG_INPUT_SWAPWEAPON, Keys.D7);
            m_debugInputMap.AddInput(DEBUG_INPUT_GIVEHEALTH, Keys.D8);
            m_debugInputMap.AddInput(DEBUG_INPUT_GIVEMANA, Keys.D9);
            m_debugInputMap.AddInput(DEBUG_INPUT_LEVELUP_STRENGTH, Keys.D1);
            m_debugInputMap.AddInput(DEBUG_INPUT_LEVELUP_HEALTH, Keys.D2);
            m_debugInputMap.AddInput(DEBUG_INPUT_LEVELUP_ENDURANCE, Keys.D3);
            m_debugInputMap.AddInput(DEBUG_INPUT_LEVELUP_EQUIPLOAD, Keys.D4);
            m_debugInputMap.AddInput(DEBUG_INPUT_TRAITSCREEN, Keys.X);
            m_debugInputMap.AddInput(DEBUG_UNLOCK_ALL_BLUEPRINTS, Keys.H);
            m_debugInputMap.AddInput(DEBUG_PURCHASE_ALL_BLUEPRINTS, Keys.J);
        }

        public override void ChangeSprite(string spriteName)
        {
            base.ChangeSprite(spriteName);

            if (State != STATE_TANOOKI)
            {
                string headPart = (_objectList[PlayerPart.Head] as IAnimateableObj).SpriteName;
                int numberIndex = headPart.IndexOf("_") - 1;
                headPart = headPart.Remove(numberIndex, 1);
                if (Game.PlayerStats.Class == ClassType.Dragon)
                    headPart = headPart.Replace("_", PlayerPart.DragonHelm + "_");
                else if (Game.PlayerStats.Class == ClassType.Traitor)
                    headPart = headPart.Replace("_", PlayerPart.IntroHelm + "_");
                else
                    headPart = headPart.Replace("_", Game.PlayerStats.HeadPiece + "_");
                _objectList[PlayerPart.Head].ChangeSprite(headPart);

                string chestPart = (_objectList[PlayerPart.Chest] as IAnimateableObj).SpriteName;
                numberIndex = chestPart.IndexOf("_") - 1;
                chestPart = chestPart.Remove(numberIndex, 1);
                chestPart = chestPart.Replace("_", Game.PlayerStats.ChestPiece + "_");
                _objectList[PlayerPart.Chest].ChangeSprite(chestPart);

                string shoulderAPart = (_objectList[PlayerPart.ShoulderA] as IAnimateableObj).SpriteName;
                numberIndex = shoulderAPart.IndexOf("_") - 1;
                shoulderAPart = shoulderAPart.Remove(numberIndex, 1);
                shoulderAPart = shoulderAPart.Replace("_", Game.PlayerStats.ShoulderPiece + "_");
                _objectList[PlayerPart.ShoulderA].ChangeSprite(shoulderAPart);

                string shoulderBPart = (_objectList[PlayerPart.ShoulderB] as IAnimateableObj).SpriteName;
                numberIndex = shoulderBPart.IndexOf("_") - 1;
                shoulderBPart = shoulderBPart.Remove(numberIndex, 1);
                shoulderBPart = shoulderBPart.Replace("_", Game.PlayerStats.ShoulderPiece + "_");
                _objectList[PlayerPart.ShoulderB].ChangeSprite(shoulderBPart);

                // This is giving the SpellSword special arms and changing his sword opacity.
                if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                {
                    if (this.State == STATE_WALKING && (m_currentLogicSet != m_standingAttack3LogicSet || (m_currentLogicSet == m_standingAttack3LogicSet && m_currentLogicSet.IsActive == false)))
                        _objectList[PlayerPart.Arms].ChangeSprite("PlayerWalkingArmsSpellSword_Sprite");
                    else if (this.State == STATE_JUMPING && (m_currentLogicSet != m_standingAttack3LogicSet || (m_currentLogicSet == m_standingAttack3LogicSet && m_currentLogicSet.IsActive == false)) && IsAirAttacking == false && IsKilled == false)
                    {
                        if (this.AccelerationY < 0)
                            _objectList[PlayerPart.Arms].ChangeSprite("PlayerFallingArmsSpellSword_Sprite");
                        else
                            _objectList[PlayerPart.Arms].ChangeSprite("PlayerJumpingArmsSpellSword_Sprite");

                    }
                    _objectList[PlayerPart.Sword1].Opacity = 0f;
                    _objectList[PlayerPart.Sword2].Opacity = 0f;
                }
                else
                {
                    _objectList[PlayerPart.Sword1].Opacity = 1f;
                    _objectList[PlayerPart.Sword2].Opacity = 1f;
                }

                _objectList[PlayerPart.Light].Opacity = 0.3f;
                _objectList[PlayerPart.Light].Visible = false;

                if (Game.PlayerStats.Class == ClassType.Banker2 && spriteName != "PlayerDeath_Character" && m_lightOn == true) // turn off the light if the player is dead.
                    _objectList[PlayerPart.Light].Visible = true;

                // This gives the player a shield in case he is the knight class.
                if (Game.PlayerStats.Class == ClassType.Knight || Game.PlayerStats.Class == ClassType.Knight2)
                {
                    string partName = spriteName.Replace("_Character", "Shield_Sprite");
                    _objectList[PlayerPart.Extra].Visible = true;
                    _objectList[PlayerPart.Extra].ChangeSprite(partName);
                }
                // This gives the player a headlamp in case he is the banker (spelunker) class.
                else if (Game.PlayerStats.Class == ClassType.Banker || Game.PlayerStats.Class == ClassType.Banker2)
                {
                    string partName = spriteName.Replace("_Character", "Lamp_Sprite");
                    _objectList[PlayerPart.Extra].Visible = true;
                    _objectList[PlayerPart.Extra].ChangeSprite(partName);
                }
                // This gives the player a headlamp in case he is the ninja.
                else if (Game.PlayerStats.Class == ClassType.Ninja || Game.PlayerStats.Class == ClassType.Ninja2)
                {
                    string partName = spriteName.Replace("_Character", "Headband_Sprite");
                    _objectList[PlayerPart.Extra].Visible = true;
                    _objectList[PlayerPart.Extra].ChangeSprite(partName);
                }
                // This gives the player a headlamp in case he is the wizard.
                else if (Game.PlayerStats.Class == ClassType.Wizard || Game.PlayerStats.Class == ClassType.Wizard2)
                {
                    string partName = spriteName.Replace("_Character", "Beard_Sprite");
                    _objectList[PlayerPart.Extra].Visible = true;
                    _objectList[PlayerPart.Extra].ChangeSprite(partName);
                }
                else if (Game.PlayerStats.Class == ClassType.Barbarian || Game.PlayerStats.Class == ClassType.Barbarian2)
                {
                    string partName = spriteName.Replace("_Character", "Horns_Sprite");
                    _objectList[PlayerPart.Extra].Visible = true;
                    _objectList[PlayerPart.Extra].ChangeSprite(partName);
                }
                else
                    _objectList[PlayerPart.Extra].Visible = false;

                // These are the glasses.
                _objectList[PlayerPart.Glasses].Visible = false;
                if (Game.PlayerStats.SpecialItem == SpecialItemType.Glasses)
                    _objectList[PlayerPart.Glasses].Visible = true;

                // This is for the hair.
                _objectList[PlayerPart.Hair].Visible = true;
                if (Game.PlayerStats.Traits.X == TraitType.Baldness || Game.PlayerStats.Traits.Y == TraitType.Baldness)
                    _objectList[PlayerPart.Hair].Visible = false;

                // This is for male/female counterparts
                if (Game.PlayerStats.IsFemale == false)
                {
                    _objectList[PlayerPart.Boobs].Visible = false;
                    _objectList[PlayerPart.Bowtie].Visible = false;
                }
                else
                {
                    _objectList[PlayerPart.Boobs].Visible = true;
                    _objectList[PlayerPart.Bowtie].Visible = true;
                }

                // Dragon wings.
                _objectList[PlayerPart.Wings].Visible = false;
                _objectList[PlayerPart.Wings].Opacity = 1;
                if (Game.PlayerStats.Class == ClassType.Dragon)
                    _objectList[PlayerPart.Wings].Visible = true;

                //_objectList[PlayerPart.Sword2].Visible = false; // This is needed until the sword is properly separated.

                if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                    this.OutlineColour = Color.White;
                else
                    this.OutlineColour = Color.Black;
            }
        }

        public void LockControls()
        {
            m_lockControls = true;
        }

        public void UnlockControls()
        {
            m_lockControls = false;
        }

        // Called in the LevelScreen.
        public override void HandleInput()
        {
            if (m_lockControls == false && IsKilled == false) // Only check input if the controls are unlocked.
            {
                if (State != STATE_HURT)
                    InputControls();
            }

            if (Game.PlayerStats.Class == ClassType.Traitor && (AttachedLevel.CurrentRoom is CarnivalShoot1BonusRoom) == false && (AttachedLevel.CurrentRoom is CarnivalShoot2BonusRoom) == false)
            {
                if (IsKilled == false && State == STATE_DASHING) // Special controls only for the fountain class.
                {
                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_SPELL1) && m_spellCastDelay <= 0 && CurrentMana >= PlayerEV.TRAITOR_CLOSE_MANACOST)
                    {
                        m_spellCastDelay = 0.5f;
                        CurrentMana -= PlayerEV.TRAITOR_CLOSE_MANACOST;
                        CastCloseShield();
                    }
                }
            }

            if (LevelEV.ENABLE_DEBUG_INPUT == true)
                HandleDebugInput();
        }

        private void HandleDebugInput()
        {
            if (InputManager.JustPressed(Keys.T, null))
            {
                Game.PlayerStats.GodMode = !Game.PlayerStats.GodMode;
                SoundManager.PlaySound("Fart1", "Fart2", "Fart3");
                m_levelScreen.ImpactEffectPool.DisplayFartEffect(this);
            }

            if (m_debugInputMap.JustPressed(DEBUG_INPUT_SWAPWEAPON))
            {
                Game.PlayerStats.Spell++;
                if (Game.PlayerStats.Spell > SpellType.Total)
                    Game.PlayerStats.Spell = 1;
                m_levelScreen.UpdatePlayerSpellIcon();
            }
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_GIVEHEALTH))
                this.CurrentHealth = this.MaxHealth;
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_GIVEMANA))
                this.CurrentMana = this.MaxMana;
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_LEVELUP_STRENGTH))
            {
                Game.PlayerStats.Gold += 1000;
            }
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_LEVELUP_HEALTH))
            {
                Game.PlayerStats.Gold += 10000;

            }
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_LEVELUP_EQUIPLOAD))
            {
                Game.PlayerStats.Gold += 100000;

            }
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_LEVELUP_ENDURANCE))
            {
            }
            if (m_debugInputMap.JustPressed(DEBUG_INPUT_TRAITSCREEN))
            {
                RCScreenManager screenManager = m_levelScreen.ScreenManager as RCScreenManager;
                if (screenManager != null)
                {
                    //screenManager.DisplayScreen(ScreenType.SkillUnlock, true);
                    //screenManager.DisplayScreen(ScreenType.Trait, true);
                    //screenManager.DisplayScreen(ScreenType.Ending, true);
                    //screenManager.DisplayScreen(ScreenType.DeathDefy, true);

                    //List<object> textscreentest = new List<object>();
                    //TextObj test = new TextObj(Game.JunicodeFont);
                    //test.Text = "Cellar Door Games Presents";
                    //test.FontSize = 30;
                    //test.ForceDraw = true;
                    //test.Align = Types.TextAlign.Centre;
                    //test.Position = new Vector2(1320/2f, 720/2f);
                    //textscreentest.Add(1.0f);
                    //textscreentest.Add(0.5f);
                    //textscreentest.Add(3.5f);
                    //textscreentest.Add(true);
                    //textscreentest.Add(test);
                    //screenManager.DisplayScreen(ScreenType.Text, true, textscreentest);
                    //screenManager.DisplayScreen(ScreenType.DiaryFlashback, true);
                    //screenManager.DisplayScreen(ScreenType.Credits, true);
                    //screenManager.DisplayScreen(ScreenType.Ending, true);
                    //screenManager.DisplayScreen(ScreenType.DeathDefy, true);

                    this.Kill();
                    //RunGetItemAnimation();
                }
            }
            if (m_debugInputMap.JustPressed(DEBUG_UNLOCK_ALL_BLUEPRINTS))
                Game.EquipmentSystem.SetBlueprintState(EquipmentState.FoundButNotSeen);
            if (m_debugInputMap.JustPressed(DEBUG_PURCHASE_ALL_BLUEPRINTS))
                Game.EquipmentSystem.SetBlueprintState(EquipmentState.Purchased);

            if ((InputManager.Pressed(Keys.LeftShift, PlayerIndex.One) || InputManager.Pressed(Buttons.LeftShoulder, PlayerIndex.One)) && CanRun == true && m_isTouchingGround == true)
                this.CurrentSpeed *= RunSpeedMultiplier;

            if (InputManager.JustPressed(Keys.B, this.PlayerIndex))
            {
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                objectList.Add(GetItemType.Blueprint);
                objectList.Add(Vector2.Zero);

                (this.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                this.RunGetItemAnimation();
            }
            else if (InputManager.JustPressed(Keys.N, this.PlayerIndex))
            {
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                objectList.Add(GetItemType.Rune);
                objectList.Add(Vector2.Zero);

                (this.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                this.RunGetItemAnimation();
            }
            else if (InputManager.JustPressed(Keys.M, this.PlayerIndex))
            {
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                objectList.Add(GetItemType.StatDrop);
                objectList.Add(new Vector2(ItemDropType.Stat_Strength, ItemDropType.Stat_Strength));

                (this.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                this.RunGetItemAnimation();
            }
            else if (InputManager.JustPressed(Keys.OemComma, this.PlayerIndex))
            {
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                objectList.Add(GetItemType.Spell);
                objectList.Add(new Vector2(SpellType.Axe, 0));

                (this.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                this.RunGetItemAnimation();
            }
            else if (InputManager.JustPressed(Keys.OemPeriod, this.PlayerIndex))
            {
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                objectList.Add(GetItemType.SpecialItem);
                objectList.Add(new Vector2(SpecialItemType.FreeEntrance, 0));

                (this.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                this.RunGetItemAnimation();
            }
            else if (InputManager.JustPressed(Keys.OemQuestion, this.PlayerIndex))
            {
                List<object> objectList = new List<object>();
                objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                objectList.Add(GetItemType.FountainPiece);
                objectList.Add(new Vector2(ItemDropType.FountainPiece1, 0));

                (this.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                this.RunGetItemAnimation();
            }
        }

        private void InputControls()
        {
            //if (InputManager.JustPressed(Keys.R, null))
            //{
            //    if (m_levelScreen.Camera.Zoom != 1)
            //        m_levelScreen.Camera.Zoom = 1;
            //    else
            //        m_levelScreen.Camera.Zoom = 2.5f;

            //}

            //if (Game.GlobalInput.JustPressed(InputMapType.MENU_PAUSE))// && Game.PlayerStats.TutorialComplete == true)
            //{
            //    (m_levelScreen.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Pause, true);
            //}

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_MAP) && Game.PlayerStats.TutorialComplete == true && m_levelScreen.CurrentRoom.Name != "Start" && m_levelScreen.CurrentRoom.Name != "Boss" && m_levelScreen.CurrentRoom.Name != "ChallengeBoss")
                m_levelScreen.DisplayMap(false);
            // Code for blocking.
            if (State != STATE_TANOOKI)
            {
                if (Game.GlobalInput.Pressed(InputMapType.PLAYER_BLOCK) && CanBlock == true && m_currentLogicSet.IsActive == false)
                {
                    if (CurrentMana >= GameEV.KNIGHT_BLOCK_DRAIN)
                    {
                        if (m_isTouchingGround == true)
                            this.CurrentSpeed = 0;

                        if (State == STATE_FLYING) // Stop movement if the player is flying.
                        {
                            this.CurrentSpeed = 0;
                            AccelerationX = 0;
                            AccelerationY = 0;
                        }

                        State = STATE_BLOCKING;
                        if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_BLOCK) == true)
                            SoundManager.PlaySound("Player_Block_Action");
                    }
                    else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_BLOCK))
                        SoundManager.PlaySound("Error_Spell");
                }
                else
                {
                    if (m_isTouchingGround == false)
                    {
                        if (IsFlying == true)
                        {
                            if (State == STATE_DRAGON)
                                State = STATE_DRAGON;
                            else
                                State = STATE_FLYING;
                        }
                        else State = STATE_JUMPING;
                    }
                    else
                        State = STATE_IDLE;
                }
            }

            // Code for moving left and right.
            if (State != STATE_BLOCKING && State != STATE_TANOOKI)
            {
                if (Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2))
                {
                    if (m_isTouchingGround == true)// && State != STATE_CROUCHING)
                        State = STATE_WALKING;

                    if ((Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2)) && (m_collidingRight == false || m_isTouchingGround == true))
                    {
                        this.HeadingX = 1;
                        this.CurrentSpeed = TotalMovementSpeed; // this.Speed;

                    }
                    else if ((Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2))
                        && Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) == false && Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2) == false && (m_collidingLeft == false || m_isTouchingGround == true))
                    {
                        this.HeadingX = -1; 
                        this.CurrentSpeed = TotalMovementSpeed; // this.Speed;
                    }
                    else
                        this.CurrentSpeed = 0;

                    if (m_currentLogicSet.IsActive == false || (m_currentLogicSet.IsActive == true && (Game.PlayerStats.Traits.X == TraitType.Hypermobility || Game.PlayerStats.Traits.Y == TraitType.Hypermobility)))
                    {
                        if (Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2))
                            this.Flip = SpriteEffects.None;
                        else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2))
                            this.Flip = SpriteEffects.FlipHorizontally;
                    }

                    if (m_isTouchingGround == true && m_currentLogicSet == m_standingAttack3LogicSet && m_currentLogicSet.IsActive == true && m_playerLegs.SpriteName != "PlayerWalkingLegs_Sprite")
                    {
                        m_playerLegs.ChangeSprite("PlayerWalkingLegs_Sprite");
                        m_playerLegs.PlayAnimation(this.CurrentFrame, this.TotalFrames);
                        m_playerLegs.Y += 4;
                        m_playerLegs.OverrideParentAnimationDelay = true;
                        m_playerLegs.AnimationDelay = 1 / 10f;
                    }
                }
                else
                {
                    if (m_isTouchingGround == true)
                        State = STATE_IDLE;
                    this.CurrentSpeed = 0;
                }
            }

            bool justJumped = false; // Bool for detect a jump and a flight call.
            // Code for jumping and double jumping.  Also, dragons cannot jump.
            if (State != STATE_BLOCKING && State != STATE_FLYING && State != STATE_TANOOKI && Game.PlayerStats.Class != ClassType.Dragon)
            {
                if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP1)|| Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP2)) && m_isTouchingGround == true && m_dropThroughGroundTimer <= 0)
                {
                    State = STATE_JUMPING;
                    AccelerationY = -JumpHeight;
                    m_isJumping = true;

                    if (Game.PlayerStats.Traits.X == TraitType.Gigantism|| Game.PlayerStats.Traits.Y == TraitType.Gigantism)
                    {
                        SoundManager.PlaySound("Player_Jump_04_Low");
                        SoundManager.PlaySound("Player_WalkUp01_Low");
                    }
                    if (Game.PlayerStats.Traits.X == TraitType.Dwarfism || Game.PlayerStats.Traits.Y == TraitType.Dwarfism)
                    {
                        SoundManager.PlaySound("Player_Jump_04_High");
                        SoundManager.PlaySound("Player_WalkUp01_High");
                    }
                    else
                    {
                        SoundManager.PlaySound("Player_Jump_04");
                        SoundManager.PlaySound("Player_WalkUp01");
                    }

                    if (Game.PlayerStats.Traits.X == TraitType.IBS || Game.PlayerStats.Traits.Y == TraitType.IBS)
                    {
                        if (CDGMath.RandomInt(0, 100) >= GameEV.FART_CHANCE) //82 //70 - TEDDY LOWERING ODDS OF FARTS
                        {
                            SoundManager.PlaySound("Fart1", "Fart2", "Fart3");
                            m_levelScreen.ImpactEffectPool.DisplayDustEffect(this);
                        }
                    }
                    justJumped = true;
                }
                else if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP2)) && m_isTouchingGround == false && m_doubleJumpCount < TotalDoubleJumps && m_dropThroughGroundTimer <= 0)
                {
                    State = STATE_JUMPING;
                    AccelerationY = -DoubleJumpHeight;
                    m_levelScreen.ImpactEffectPool.DisplayDoubleJumpEffect(new Vector2(this.X, this.Bounds.Bottom + 10));
                    m_isJumping = true;
                    //m_doubleJumped = true;

                    m_doubleJumpCount++;
                    //SoundManager.PlaySound("Player_WalkUp01");

                    SoundManager.PlaySound("Player_DoubleJump");

                    if (Game.PlayerStats.Traits.X == TraitType.IBS || Game.PlayerStats.Traits.Y == TraitType.IBS)
                    {
                        if (CDGMath.RandomInt(0, 100) >= GameEV.FART_CHANCE) //82 //70 - TEDDY LOWERING ODDS OF FARTS
                        {
                            SoundManager.PlaySound("Fart1", "Fart2", "Fart3");
                            m_levelScreen.ImpactEffectPool.DisplayDustEffect(this);
                        }
                    }
                    justJumped = true;
                }

                if (m_isTouchingGround == false)
                {
                    if (m_currentLogicSet == m_standingAttack3LogicSet && m_currentLogicSet.IsActive == true)
                    {
                        if (this.AccelerationY > 0 && m_playerLegs.SpriteName != "PlayerAttackFallingLegs_Sprite")
                        {
                            m_playerLegs.ChangeSprite("PlayerAttackFallingLegs_Sprite");
                        }
                        else if (this.AccelerationY < 0 && m_playerLegs.SpriteName != "PlayerAttackJumpingLegs_Sprite")
                        {
                            m_playerLegs.ChangeSprite("PlayerAttackJumpingLegs_Sprite");
                        }
                    }

                    if (State != STATE_FLYING)
                        State = STATE_JUMPING;
                }
            }

            // Code for attacking.
            // Dragons cannot attack. They only shoot spells.
            if (m_currentLogicSet.IsActive == false && State != STATE_BLOCKING && State != STATE_TANOOKI && Game.PlayerStats.Class != ClassType.Dragon) // Only attack when you're not doing an animation.
            {
                if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2)) && CanAirAttackDownward == true
                    && Game.GameConfig.QuickDrop == true && State == STATE_JUMPING && m_dropThroughGroundTimer <= 0)
                {
                    m_currentLogicSet = m_airAttackLS;
                    if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                        FadeSword();

                    if (m_assassinSpecialActive == true)
                        DisableAssassinAbility();

                    m_currentLogicSet.Execute();
                }
                else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_ATTACK))
                {
                    if (State == STATE_JUMPING)
                    {
                        //Tween.RunFunction(0f, this, "PerformDelayedAirAttack");
                        if ((Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN2)) && CanAirAttackDownward == true)
                            m_currentLogicSet = m_airAttackLS;
                        else
                        {
                            m_currentLogicSet = m_standingAttack3LogicSet; // Currently uses the same attack code for when he's standing but we change his legs a bit later in the code.
                            //if (this.AccelerationY > 0)
                            //    this.AccelerationY = -250;
                        }
                        if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                            FadeSword();

                        if (m_assassinSpecialActive == true)
                            DisableAssassinAbility();

                        m_currentLogicSet.Execute();
                    }
                    else
                    {
                        if (m_isTouchingGround == false)
                            this.CurrentSpeed = 0;

                        if (m_attackCounter > 0)
                            m_attackNumber++;

                        m_attackCounter = ComboDelay;
                        if (m_attackNumber == 0)
                            m_currentLogicSet = m_standingAttack3LogicSet;
                        else //if (m_attackNumber == 1)
                        {
                            m_currentLogicSet = m_standingAttack3LogicSet;
                            m_attackNumber = 0;
                            m_attackCounter = 0;
                        }

                        if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                            FadeSword();

                        if (m_assassinSpecialActive == true)
                            DisableAssassinAbility();

                        m_playerLegs.OverrideParentAnimationDelay = false; // Reset his leg animation.
                        m_currentLogicSet.Execute();
                    }
                }
            }

            // Code for subweapons
            if (Game.PlayerStats.TutorialComplete == true)
            {
                bool fireballCasted = false;
                if (Game.PlayerStats.Spell == SpellType.DragonFireNeo && (Game.GlobalInput.Pressed(InputMapType.PLAYER_ATTACK) || Game.GlobalInput.Pressed(InputMapType.PLAYER_SPELL1)) && m_rapidSpellCastDelay <= 0)
                {
                    m_rapidSpellCastDelay = 0.2f;
                    CastSpell(false);
                    fireballCasted = true;
                }

                if (m_spellCastDelay <= 0 || Game.PlayerStats.Class == ClassType.Dragon)
                {
                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_SPELL1) || (Game.PlayerStats.Class == ClassType.Dragon && Game.GlobalInput.JustPressed(InputMapType.PLAYER_ATTACK)))
                    {
                        if ((Game.PlayerStats.Class == ClassType.Dragon && fireballCasted == true) == false) // Prevents casting the spell twice.
                        CastSpell(false);
                    }
                }

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_BLOCK))
                {
                    RoomObj room = m_levelScreen.CurrentRoom;
                    if (room is CarnivalShoot1BonusRoom == false && room is CarnivalShoot2BonusRoom == false && room is ChestBonusRoomObj == false)
                    {
                        if (Game.PlayerStats.Class == ClassType.SpellSword2 && m_spellCastDelay <= 0)
                            CastSpell(false, true);
                        else if (Game.PlayerStats.Class == ClassType.Lich2)
                            ConvertHPtoMP();
                        else if (Game.PlayerStats.Class == ClassType.Assassin2 && CurrentMana > 0)
                        {
                            if (m_assassinSpecialActive == false)
                                ActivateAssassinAbility();
                            else
                                DisableAssassinAbility();
                        }
                        else if (Game.PlayerStats.Class == ClassType.Wizard2)
                            SwapSpells();
                        else if (Game.PlayerStats.Class == ClassType.Ninja2)
                            NinjaTeleport();
                        else if (Game.PlayerStats.Class == ClassType.Knight2)
                        {
                            if (this.State == STATE_TANOOKI)
                                DeactivateTanooki();
                            else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN2))
                                ActivateTanooki();
                        }
                        else if (Game.PlayerStats.Class == ClassType.Barbarian2)
                            CastFuhRohDah();
                        else if (Game.PlayerStats.Class == ClassType.Traitor)
                        {
                            if (CurrentMana >= PlayerEV.TRAITOR_AXE_MANACOST && m_spellCastDelay <= 0)
                            {
                                CurrentMana -= PlayerEV.TRAITOR_AXE_MANACOST;
                                m_spellCastDelay = 0.5f;
                                ThrowAxeProjectiles();
                            }
                        }
                    }
                    else
                    {
                        if (this.State == STATE_TANOOKI)
                            DeactivateTanooki();
                    }
                    
                    if (Game.PlayerStats.Class == ClassType.Dragon)
                    {
                        if (State != STATE_DRAGON)
                        {
                            State = STATE_DRAGON;
                            this.DisableGravity = true;
                            m_isFlying = true;
                            this.AccelerationY = 0;
                        }
                        else
                        {
                            State = STATE_JUMPING;
                            this.DisableGravity = false;
                            m_isFlying = false;
                        }
                    }
                    else if (Game.PlayerStats.Class == ClassType.Banker2)
                    {
                        if (m_lightOn == true)
                        {
                            SoundManager.PlaySound("HeadLampOff");
                            m_lightOn = false;
                            _objectList[PlayerPart.Light].Visible = false;
                        }
                        else
                        {
                            SoundManager.PlaySound("HeadLampOn");
                            m_lightOn = true;
                            _objectList[PlayerPart.Light].Visible = true;
                        }
                    }
                }

                // Extra handling to link jump to dragon flight.
                if (Game.PlayerStats.Class == ClassType.Dragon && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP2)))
                {
                    if (State != STATE_DRAGON)
                    {
                        State = STATE_DRAGON;
                        this.DisableGravity = true;
                        m_isFlying = true;
                        this.AccelerationY = 0;
                    }
                    else
                    {
                        State = STATE_JUMPING;
                        this.DisableGravity = false;
                        m_isFlying = false;
                    }
                }
            }

            // Code for dashing.
            if (m_dashCooldownCounter <= 0 && (m_isTouchingGround == true || (m_isTouchingGround == false && m_airDashCount < TotalAirDashes)) && State != STATE_BLOCKING && State != STATE_TANOOKI)
            {
                if (CanAirDash == true)
                {
                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DASHLEFT))
                    {
                        //if (m_isTouchingGround == false) // Ensures the player can only air dash once.
                            m_airDashCount++;
                        State = STATE_DASHING;
                        this.AccelerationYEnabled = false;
                        m_dashCooldownCounter = DashCoolDown;
                        m_dashCounter = DashTime;
                        this.LockControls();
                        CurrentSpeed = DashSpeed;
                        this.HeadingX = -1;
                        this.AccelerationY = 0;
                        if (m_currentLogicSet.IsActive == true)
                            m_currentLogicSet.Stop();
                        this.AnimationDelay = m_startingAnimationDelay;

                        m_levelScreen.ImpactEffectPool.DisplayDashEffect(new Vector2(this.X, TerrainBounds.Bottom), true);

                        SoundManager.PlaySound("Player_Dash");

                        if (Game.PlayerStats.Traits.X == TraitType.IBS || Game.PlayerStats.Traits.Y == TraitType.IBS)
                        {
                            if (CDGMath.RandomInt(0, 100) >= GameEV.FART_CHANCE)
                            {
                                m_levelScreen.ImpactEffectPool.DisplayDustEffect(this);
                                SoundManager.PlaySound("Fart1", "Fart2", "Fart3");
                            }
                        }
                    }
                    else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DASHRIGHT))
                    {
                        //if (m_isTouchingGround == false)
                            m_airDashCount++;
                        //m_airDashed = true;
                        this.AnimationDelay = m_startingAnimationDelay;

                        State = STATE_DASHING;
                        this.AccelerationYEnabled = false;
                        m_dashCooldownCounter = DashCoolDown;
                        m_dashCounter = DashTime;
                        this.LockControls();
                        CurrentSpeed = DashSpeed;
                        this.HeadingX = 1;
                        this.AccelerationY = 0;
                        if (m_currentLogicSet.IsActive == true)
                            m_currentLogicSet.Stop();

                        m_levelScreen.ImpactEffectPool.DisplayDashEffect(new Vector2(this.X, TerrainBounds.Bottom), false);

                        SoundManager.PlaySound("Player_Dash");

                        if (Game.PlayerStats.Traits.X == TraitType.IBS || Game.PlayerStats.Traits.Y == TraitType.IBS)
                        {
                            if (CDGMath.RandomInt(0, 100) >= GameEV.FART_CHANCE)
                            {
                                m_levelScreen.ImpactEffectPool.DisplayDustEffect(this);
                                SoundManager.PlaySound("Fart1", "Fart2", "Fart3");
                            }
                        }
                    }
                }
            }

            // Code for flying
            if (State == STATE_FLYING || State == STATE_DRAGON)
            {
                // Controls for moving the player while flying.
                if (Game.GlobalInput.Pressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_UP2) || InputManager.Pressed(Buttons.LeftThumbstickUp, PlayerIndex.One))
                    this.AccelerationY = -this.TotalMovementSpeed;
                else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN2) || InputManager.Pressed(Buttons.LeftThumbstickDown, PlayerIndex.One))
                    this.AccelerationY = this.TotalMovementSpeed;
                else
                    this.AccelerationY = 0;

                // Fix the player's legs while attacking and flying.
                if (m_isTouchingGround == false)
                {
                    if (m_currentLogicSet == m_standingAttack3LogicSet && m_currentLogicSet.IsActive == true)
                    {
                        if (this.AccelerationY > 0 && m_playerLegs.SpriteName != "PlayerAttackFallingLegs_Sprite")
                            m_playerLegs.ChangeSprite("PlayerAttackFallingLegs_Sprite");
                        else if (this.AccelerationY <= 0 && m_playerLegs.SpriteName != "PlayerAttackJumpingLegs_Sprite")
                            m_playerLegs.ChangeSprite("PlayerAttackJumpingLegs_Sprite");
                    }

                    //State = STATE_FLYING;
                }

                // Press Jump to disable flight.
                if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP2)) && State != STATE_DRAGON)
                {
                    //m_flightCounter = 0;
                    State = STATE_JUMPING;
                    this.DisableGravity = false;
                    m_isFlying = false;
                }
            }
            else if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP2)) && m_isTouchingGround == false && justJumped == false && m_doubleJumpCount >= TotalDoubleJumps && m_dropThroughGroundTimer <= 0
                 && CanFly == true && m_flightCounter > 0 && State != STATE_FLYING && State != STATE_DRAGON && State != STATE_BLOCKING && State != STATE_TANOOKI)
            {
                this.AccelerationY = 0;
                //m_flightCounter = TotalFlightTime;
                State = STATE_FLYING;
                this.DisableGravity = true;
                m_isFlying = true;
            }

            //else if (m_isTouchingGround == false && CanFly == true && m_flightCounter > 0 && 
            //    (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2)) && State != STATE_FLYING && State != STATE_DRAGON && State != STATE_BLOCKING && State != STATE_TANOOKI)
            //{
            //    this.AccelerationY = 0;
            //    //m_flightCounter = TotalFlightTime;
            //    State = STATE_FLYING;
            //    this.DisableGravity = true;
            //    m_isFlying = true;
            //}
        }

        public void PerformDelayedAirAttack()
        {
            if ((Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN2)) && CanAirAttackDownward == true)
                m_currentLogicSet = m_airAttackLS;
            else
            {
                m_currentLogicSet = m_standingAttack3LogicSet; // Currently uses the same attack code for when he's standing but we change his legs a bit later in the code.
                //if (this.AccelerationY > 0)
                //    this.AccelerationY = -250;
            }
            if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                FadeSword();

            //if (m_assassinSpecialActive == true)
            //    DisableAssassinAbility();

            m_currentLogicSet.Execute();
        }

        public override void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_dropThroughGroundTimer > 0)
                m_dropThroughGroundTimer -= elapsedSeconds;

            if (m_ninjaTeleportDelay > 0)
                m_ninjaTeleportDelay -= elapsedSeconds;

            if (m_rapidSpellCastDelay > 0)
                m_rapidSpellCastDelay -= elapsedSeconds;

            // Spellsword and assassin effects sholud not appear in the ending room.
            if (m_levelScreen.CurrentRoom is EndingRoomObj == false && this.ScaleX > 0.1f) // Scale makes sure he doesn't have this effect while teleporting.
            {
                if ((Game.PlayerStats.Traits.Y == TraitType.Ambilevous || Game.PlayerStats.Traits.X == TraitType.Ambilevous) && CurrentSpeed == 0)
                {
                    if (m_ambilevousTimer > 0)
                    {
                        m_ambilevousTimer -= elapsedSeconds;
                        if (m_ambilevousTimer <= 0)
                        {
                            m_ambilevousTimer = 0.4f;
                            m_levelScreen.ImpactEffectPool.DisplayQuestionMark(new Vector2(this.X, this.Bounds.Top));
                        }
                    }
                }

                // Adds the spellsword sparkle effect.
                if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                {
                    if (m_wizardSparkleCounter > 0)
                    {
                        m_wizardSparkleCounter -= elapsedSeconds;
                        if (m_wizardSparkleCounter <= 0)
                        {
                            m_wizardSparkleCounter = 0.2f;
                            m_levelScreen.ImpactEffectPool.DisplayChestSparkleEffect(this.Position);
                            m_levelScreen.ImpactEffectPool.DisplayChestSparkleEffect(this.Position);
                        }
                    }
                }

                // Adding assassin smoke effect.
                if (Game.PlayerStats.Class == ClassType.Assassin || Game.PlayerStats.Class == ClassType.Assassin2)
                {
                    if (m_assassinSmokeTimer > 0)
                    {
                        m_assassinSmokeTimer -= elapsedSeconds;
                        if (m_assassinSmokeTimer <= 0)
                        {
                            m_assassinSmokeTimer = 0.15f;
                            if (CurrentSpeed > 0)
                                m_assassinSmokeTimer = 0.05f;
                            m_levelScreen.ImpactEffectPool.BlackSmokeEffect(this);
                        }
                    }
                }
            }

            if (m_swearBubbleCounter > 0)
            {
                m_swearBubbleCounter -= elapsedSeconds;
                if (m_swearBubbleCounter <= 0)
                    m_swearBubble.Visible = false;
            }

            if (m_blockInvincibleCounter > 0)
                m_blockInvincibleCounter -= elapsedSeconds;

            if (IsFlying == true)
            {
                if (State != STATE_DRAGON) // Dragon gets infinite flight.
                    m_flightCounter -= elapsedSeconds;

                if (m_flightCounter <= 0 && State != STATE_DRAGON)
                {
                    State = STATE_JUMPING;
                    this.DisableGravity = false;
                    m_isFlying = false;
                }
            }

            // What the heck is this code for?
            if (AccelerationX < 0) AccelerationX += 200f * elapsedSeconds;
            else if (AccelerationX > 0) AccelerationX -= 200f * elapsedSeconds;
            if (AccelerationX < 3.6f && AccelerationX > -3.6f) AccelerationX = 0;

            this.X += this.Heading.X * (this.CurrentSpeed * elapsedSeconds);

            if (State == STATE_WALKING)
            {
                if (Game.PlayerStats.Traits.X == TraitType.Gigantism || Game.PlayerStats.Traits.Y == TraitType.Gigantism)
                {
                    m_walkDownSoundLow.Update();
                    m_walkUpSoundLow.Update();
                }
                else if (Game.PlayerStats.Traits.X == TraitType.Dwarfism || Game.PlayerStats.Traits.Y == TraitType.Dwarfism)
                {
                    m_walkDownSoundHigh.Update();
                    m_walkUpSoundHigh.Update();
                }
                else
                {
                    m_walkDownSound.Update();
                    m_walkUpSound.Update();
                }
            }

            if (m_externalLS.IsActive == false) // Only update the player if an external logic set is not being run on the player.
            {
                if (m_attackCounter > 0)
                    m_attackCounter -= elapsedSeconds;
                else
                    m_attackNumber = 0;

                if (m_currentLogicSet.IsActive)
                    m_currentLogicSet.Update(gameTime);

                if (m_dashCooldownCounter > 0)
                    m_dashCooldownCounter -= elapsedSeconds;

                if (m_dashCounter > 0)
                {
                    m_dashCounter -= elapsedSeconds;
                    if (m_dashCounter <= 0 && State != STATE_HURT)
                    {
                        this.UnlockControls();
                        this.AccelerationYEnabled = true;
                    }
                }

                if (m_invincibleCounter > 0)
                {
                    m_invincibleCounter -= elapsedSeconds;
                    if (m_assassinSpecialActive == false)
                        if (this.Opacity != 0.6f) this.Opacity = 0.6f;
                }
                else
                {
                    if (m_assassinSpecialActive == false)
                        if (this.Opacity == 0.6f) this.Opacity = 1;
                }

                //this.X += this.Heading.X * this.CurrentSpeed;
                if (IsPaused == false && (m_currentLogicSet == null || m_currentLogicSet.IsActive == false))
                    UpdateAnimationState();

                CheckGroundCollision();

                // Jump deceleration code.
                if (this.State != STATE_HURT)
                {
                    if ((((Game.GlobalInput.Pressed(InputMapType.PLAYER_JUMP1) == false && Game.GlobalInput.Pressed(InputMapType.PLAYER_JUMP2) == false)|| (m_currentLogicSet == m_airAttackLS && m_currentLogicSet.IsActive == true && IsAirAttacking == false))) && m_isTouchingGround == false && AccelerationY < 0)
                    {
                        AccelerationY += JumpDeceleration * elapsedSeconds;
                        //if (AccelerationY > 0) // This code doesn't seem to do anything.
                        //    AccelerationY = 0;
                    }
                }

                if (Game.PlayerStats.Class == ClassType.Dragon && this.CurrentMana < MaxMana)
                {
                    m_dragonManaRechargeCounter += elapsedSeconds;
                    if (m_dragonManaRechargeCounter >= GameEV.MANA_OVER_TIME_TIC_RATE)
                    {
                        m_dragonManaRechargeCounter = 0;
                        this.CurrentMana += GameEV.DRAGON_MANAGAIN;
                    }
                }

                // Assassin Active code.
                if (m_assassinSpecialActive == true)
                {
                    m_assassinDrainCounter += elapsedSeconds;
                    if (m_assassinDrainCounter >= GameEV.MANA_OVER_TIME_TIC_RATE)
                    {
                        m_assassinDrainCounter = 0;
                        this.CurrentMana -= GameEV.ASSASSIN_ACTIVE_MANA_DRAIN;
                        if (CurrentMana <= 0)
                            DisableAssassinAbility();
                    }
                }

                if (m_timeStopCast == true)
                {
                    m_timeStopDrainCounter += elapsedSeconds;
                    if (m_timeStopDrainCounter >= GameEV.MANA_OVER_TIME_TIC_RATE)
                    {
                        m_timeStopDrainCounter = 0;
                        this.CurrentMana -= GameEV.TIMESTOP_ACTIVE_MANA_DRAIN;
                        if (CurrentMana <= 0)
                        {
                            //m_levelScreen.UnpauseAllEnemies();
                            AttachedLevel.StopTimeStop();
                            m_timeStopCast = false;
                        }
                    }
                }

                if (m_damageShieldCast == true)
                {
                    m_damageShieldDrainCounter += elapsedSeconds;
                    if (m_damageShieldDrainCounter >= GameEV.MANA_OVER_TIME_TIC_RATE)
                    {
                        m_damageShieldDrainCounter = 0;
                        if (m_megaDamageShieldCast == true)
                            this.CurrentMana -= GameEV.DAMAGESHIELD_ACTIVE_MANA_DRAIN * GameEV.SPELLSWORD_MANACOST_MOD;
                        else
                            this.CurrentMana -= GameEV.DAMAGESHIELD_ACTIVE_MANA_DRAIN;
                        if (CurrentMana <= 0)
                        {
                            m_damageShieldCast = false;
                            m_megaDamageShieldCast = false;
                        }
                    }
                }

                if (m_lightOn == true)
                {
                    m_lightDrainCounter += elapsedSeconds;
                    if (m_lightDrainCounter >= 1)
                    {
                        m_lightDrainCounter = 0;
                        this.CurrentMana -= GameEV.SPELUNKER_LIGHT_DRAIN;
                        if (CurrentMana <= 0)
                        {
                            m_lightOn = false;
                            _objectList[PlayerPart.Light].Visible = false;
                        }
                    }
                }

                if (State == STATE_TANOOKI)
                {
                    m_tanookiDrainCounter += elapsedSeconds;
                    if (m_tanookiDrainCounter >= GameEV.MANA_OVER_TIME_TIC_RATE)
                    {
                        m_tanookiDrainCounter = 0;
                        this.CurrentMana -= GameEV.TANOOKI_ACTIVE_MANA_DRAIN;
                        if (CurrentMana <= 0)
                            DeactivateTanooki();
                    }
                }

                if (m_spellCastDelay > 0)
                    m_spellCastDelay -= elapsedSeconds;

                base.Update(gameTime);
            }
            else if (m_externalLS.IsActive == true)
                m_externalLS.Update(gameTime);
        }

        private void UpdateAnimationState()
        {
            switch (State)
            {
                case (STATE_TANOOKI):
                    if (this.SpriteName != "Tanooki_Character")
                        this.ChangeSprite("Tanooki_Character");
                    break;
                case (STATE_IDLE):
                    if (this.SpriteName != "PlayerIdle_Character")
                        this.ChangeSprite("PlayerIdle_Character");
                    if (this.IsAnimating == false && m_playerHead.SpriteName != "PlayerIdleHeadUp_Sprite")
                        this.PlayAnimation(true);
                    break;
                case (STATE_WALKING):
                    if (this.SpriteName != "PlayerWalking_Character")
                        this.ChangeSprite("PlayerWalking_Character");
                    if (this.IsAnimating == false)
                        this.PlayAnimation(true);
                    break;
                case (STATE_FLYING):
                case(STATE_DRAGON):
                case (STATE_JUMPING):
                    if (this.AccelerationY <= 0)
                    {
                        if (this.SpriteName != "PlayerJumping_Character") // Player just started jumping.
                            this.ChangeSprite("PlayerJumping_Character");
                    }
                    else if (this.AccelerationY > 0)
                    {
                        if (this.SpriteName != "PlayerFalling_Character") // Player falling.
                            this.ChangeSprite("PlayerFalling_Character");
                    }
                    if (this.IsAnimating == false)
                        this.PlayAnimation(true);

                    break;
                case (STATE_HURT):
                    if (this.SpriteName != "PlayerHurt_Character")
                        this.ChangeSprite("PlayerHurt_Character");
                    if (this.IsAnimating == true)
                        this.StopAnimation();
                    break;
                case (STATE_DASHING):
                    if (this.HeadingX < 0 && this.Flip == SpriteEffects.None)
                    {
                        if (this.SpriteName != "PlayerDash_Character")
                            this.ChangeSprite("PlayerDash_Character");
                    }
                    else if (this.HeadingX < 0 && this.Flip == SpriteEffects.FlipHorizontally)
                    {
                        if (this.SpriteName != "PlayerFrontDash_Character")
                            this.ChangeSprite("PlayerFrontDash_Character");
                    }

                    if (this.HeadingX > 0 && this.Flip == SpriteEffects.None)
                    {
                        if (this.SpriteName != "PlayerFrontDash_Character")
                            this.ChangeSprite("PlayerFrontDash_Character");
                    }
                    else if (this.HeadingX > 0 && this.Flip == SpriteEffects.FlipHorizontally)
                    {
                        if (this.SpriteName != "PlayerDash_Character")
                            this.ChangeSprite("PlayerDash_Character");
                    }
                    if (this.IsAnimating == false)
                        this.PlayAnimation(false);
                    break;
                case (STATE_BLOCKING):
                    if (this.SpriteName != "PlayerBlock_Character")
                    {
                        this.ChangeSprite("PlayerBlock_Character");
                        this.PlayAnimation(false);
                    }
                    break;
            }

        }

        private void CheckGroundCollision()
        {
            IPhysicsObj previousClosestGround = m_closestGround;
            m_previousIsTouchingGround = m_isTouchingGround;
            m_isTouchingGround = false;
            m_collidingLeft = m_collidingRight = false;

            m_collidingLeftOnly = m_collidingRightOnly = false;
            bool foundBottomCollision = false;

            //if ((m_isJumping == false && this.AccelerationY >= 0 || (m_isJumping == true && this.AccelerationY >= 0))) // Only check ground collision if falling. Do not check if he's going up, or jumping.
            //if (State != STATE_DRAGON) // Dragons cannot touch the ground.
            {
                IPhysicsObj closestTerrain = null;
                float closestFloor = float.MaxValue;

                IPhysicsObj closestRotatedTerrain = null;
                float closestRotatedFloor = float.MaxValue;

                //Rectangle elongatedPlayerBounds = this.TerrainBounds;
                //elongatedPlayerBounds.Height += 10; // A rectangle slightly larger than the player's height is needed to check for collisions with.

                Rectangle elongatedPlayerBounds = new Rectangle(this.TerrainBounds.Left, this.TerrainBounds.Bottom - 78, this.TerrainBounds.Width, 78 + 10);

                // This is to fix the player hooking to walls and slipping through mouseholes.
                // The consequence is that he can slip off ledges.
                //elongatedPlayerBounds.X -= 10;
                //elongatedPlayerBounds.Width += 20;
                foreach (IPhysicsObj collisionObj in PhysicsMngr.ObjectList)
                {
                    if (Game.PlayerStats.Traits.X == TraitType.NoFurniture || Game.PlayerStats.Traits.Y == TraitType.NoFurniture)
                    {
                        if (collisionObj is PhysicsObj && collisionObj is HazardObj == false) // Disable ground collision check for bookcases when you have this trait active.
                            continue;
                    }

                    if (collisionObj != this && collisionObj.Visible == true && collisionObj.IsCollidable == true && (collisionObj.CollidesTop == true || collisionObj.CollidesLeft == true || collisionObj.CollidesRight == true) && collisionObj.HasTerrainHitBox == true &&
                        (collisionObj.CollisionTypeTag == GameTypes.CollisionType_WALL ||
                        collisionObj.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL ||
                        collisionObj.CollisionTypeTag == GameTypes.CollisionType_WALL_FOR_PLAYER ||
                        collisionObj.CollisionTypeTag == GameTypes.CollisionType_ENEMYWALL))
                    {
                        // This code prevents dragon or flight from landing on one ways.
                        if (collisionObj.CollidesTop == true && collisionObj.CollidesBottom == false && (State == STATE_FLYING || State == STATE_DRAGON))
                            continue;

                        // Don't register ground collision on mouse holes.
                        if (collisionObj.CollidesTop == true && collisionObj.CollidesBottom == true && collisionObj.CollidesLeft == false && collisionObj.CollidesRight == false)
                            continue;

                        // Do separate checks for sloped and non-sloped terrain.
                        HazardObj hazard = collisionObj as HazardObj;
                        if (collisionObj.Rotation == 0 || hazard != null)
                        {
                            Rectangle collisionObjBounds = collisionObj.TerrainBounds;
                            if (hazard != null)
                                collisionObjBounds = collisionObj.Bounds;
                            Vector2 mtd = CollisionMath.CalculateMTD(elongatedPlayerBounds, collisionObjBounds);

                            Rectangle intersectionBounds = new Rectangle(this.TerrainBounds.X, this.TerrainBounds.Y, this.TerrainBounds.Width, this.TerrainBounds.Height);
                            Vector2 intersectionDepth = CollisionMath.GetIntersectionDepth(intersectionBounds, collisionObjBounds);

                            //// VERY SPECIAL CODE TO PREVENT THE PLAYER FROM HOOKING TO WALLS ////////////////
                            Rectangle wallBounds = new Rectangle(elongatedPlayerBounds.X - 10, elongatedPlayerBounds.Y, elongatedPlayerBounds.Width + 20, elongatedPlayerBounds.Height);
                            Vector2 wallMTD = CollisionMath.CalculateMTD(wallBounds, collisionObjBounds);

                            // These flags make sure that pressing left or right no longer moves the player in those directions.
                            if (wallMTD.X > 0 && collisionObj.CollidesRight == true)
                                m_collidingLeft = true;
                            if (wallMTD.X < 0 && collisionObj.CollidesLeft == true)
                                m_collidingRight = true;
                            //////////////////////////////////////////////////////////////////////////////////


                            Vector2 testMTD = CollisionMath.CalculateMTD(this.TerrainBounds, collisionObjBounds);
                            if (testMTD.X > 0)
                                m_collidingLeftOnly = true;
                            else if (testMTD.X < 0)
                                m_collidingRightOnly = true;
                            else if (testMTD.Y != 0)
                                foundBottomCollision = true;

                            if (foundBottomCollision == true)
                                m_collidingRightOnly = m_collidingLeftOnly = false;

                            // Don't include drop through ledges otherwise things screw up.
                            // This is dangerous code because it is a huge cause of 1-way false positives.
                            // This prevents the player from locking to one ways if they're just above him.
                            if (collisionObj.CollidesBottom == false && Math.Abs(collisionObjBounds.Top - this.TerrainBounds.Bottom) > 20 && this.AccelerationY < 1100)
                            {
                                //if ((collisionObj as GameObj).Name == "drop" && CollisionMath.Intersects(this.TerrainBounds, collisionObj.TerrainBounds))
                                //    Console.WriteLine("BREAK! " + Math.Abs(collisionObjBounds.Top - this.TerrainBounds.Bottom) + " Acceleration: " + this.AccelerationY);

                                //if ((collisionObj as GameObj).Name == "test" && CollisionMath.Intersects(collisionObjBounds, this.TerrainBounds))
                                //    Console.WriteLine("BREAK! " + Math.Abs(collisionObjBounds.Top - this.TerrainBounds.Bottom) + " Acceleration: " + this.AccelerationY);
                                continue;
                            }

                            // This check is to see if you are falling and hitting the platform from above, or from the side. If falling from the side, don't include this as a ground obj.
                            //if (Math.Abs(intersectionDepth.X) > 2)
                            int intersectionX = (int)Math.Abs(intersectionDepth.X);
                            int intersectionY = (int)Math.Abs(intersectionDepth.Y);
                            if (intersectionX > 1 && intersectionX < intersectionY)
                                continue;

                            if ((m_isJumping == false && this.AccelerationY >= 0 || (m_isJumping == true && this.AccelerationY >= 0))) // Only check ground collision if falling. Do not check if he's going up, or jumping.
                            {
                                //if (State != STATE_DRAGON) // Dragons cannot touch the ground.

                                if ((mtd.Y < 0 || (mtd.Y == 0 && mtd.X != 0 && collisionObjBounds.Top > this.TerrainBounds.Top && collisionObj.Y > this.Y)))
                                {
                                    int distance = Math.Abs(collisionObjBounds.Top - this.TerrainBounds.Bottom);

                                    if (distance < closestFloor)
                                    {
                                        closestTerrain = collisionObj;
                                        closestFloor = distance;
                                        m_closestGround = closestTerrain;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((m_isJumping == false && this.AccelerationY >= 0 || (m_isJumping == true && this.AccelerationY >= 0))) // Only check ground collision if falling. Do not check if he's going up, or jumping.
                            {
                                if (collisionObj is HazardObj == false)
                                {
                                    Vector2 rotatedMTD = CollisionMath.RotatedRectIntersectsMTD(elongatedPlayerBounds, this.Rotation, Vector2.Zero, collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                                    //if ((collisionObj as GameObj).Name == "rotat")
                                    //{
                                    //    Console.WriteLine(this.SpriteName + " " + elongatedPlayerBounds);
                                    //}
                                    if (rotatedMTD.Y < 0)
                                    {
                                        float distance = rotatedMTD.Y;
                                        if (distance < closestRotatedFloor)
                                        {
                                            closestRotatedTerrain = collisionObj;
                                            closestRotatedFloor = distance;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                if (closestTerrain != null && State != STATE_DRAGON)
                {
                    //if (closestTerrain == m_lastPlatform && m_droppingThroughGround == true)
                    //    m_isTouchingGround = false;
                    //else
                    //  m_isTouchingGround = true;

                    // New logic for determining whether you should fall through a one-way.
                    if (m_dropThroughGroundTimer > 0 && closestTerrain.CollidesBottom == false && closestTerrain.CollidesTop == true)
                        m_isTouchingGround = false;
                    else
                        m_isTouchingGround = true;
                }

                if (closestRotatedTerrain != null && State != STATE_DRAGON)
                {
                    HookToSlope(closestRotatedTerrain);
                    m_isTouchingGround = true;
                }

                if (m_isTouchingGround == true)
                {
                    if (State == STATE_JUMPING || State == STATE_FLYING || State == STATE_HURT) // Player just landed.
                    {
                        if (Game.PlayerStats.Traits.X == TraitType.Gigantism || Game.PlayerStats.Traits.Y == TraitType.Gigantism)
                        {
                            SoundManager.PlaySound("Player_Land_Low");
                            if (this.AccelerationY > 1400)
                            {
                                SoundManager.PlaySound("TowerLand");
                                AttachedLevel.ImpactEffectPool.DisplayDustEffect(new Vector2(this.TerrainBounds.Left, this.Bounds.Bottom));
                                AttachedLevel.ImpactEffectPool.DisplayDustEffect(new Vector2(this.TerrainBounds.X, this.Bounds.Bottom));
                                AttachedLevel.ImpactEffectPool.DisplayDustEffect(new Vector2(this.TerrainBounds.Right, this.Bounds.Bottom));
                            }
                        }
                        if (Game.PlayerStats.Traits.X == TraitType.Dwarfism || Game.PlayerStats.Traits.Y == TraitType.Dwarfism)
                            SoundManager.PlaySound("Player_Land_High");
                        else
                            SoundManager.PlaySound("Player_Land");
                    }

                    if (State == STATE_HURT)
                        m_invincibleCounter = InvincibilityTime;

                    if (IsAirAttacking == true)
                    {
                        IsAirAttacking = false;
                        CancelAttack();
                    }

                    this.AccelerationX = 0;

                    // Code to make sure to disable flying when the player touches the ground.
                    m_flightCounter = TotalFlightTime;
                    if (State != STATE_DASHING)
                    {
                        m_airDashCount = 0;
                        this.AccelerationYEnabled = true;
                    }

                    NumAirBounces = 0;
                    m_isFlying = false;
                    DisableGravity = false;

                    if (State == STATE_TANOOKI)
                        this.CurrentSpeed = 0;

                    // Reset any jumping states.  I.e. air jump, air dash, etc.
                    m_isJumping = false;
                    m_doubleJumpCount = 0;

                    if (State != STATE_BLOCKING && State != STATE_TANOOKI && State != STATE_DASHING) // This line also breaks the player out of his hurt state.
                        State = STATE_IDLE;

                    if (State != STATE_BLOCKING && State != STATE_TANOOKI && State != STATE_DASHING)
                    {
                        if (closestTerrain != null && this.ControlsLocked == false)
                        {
                            if ((((Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN2)) && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_JUMP2)))
                                || (Game.GameConfig.QuickDrop == true && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2)))) && closestTerrain.CollidesBottom == false && State != STATE_TANOOKI) // Code for dropping through ground.
                            {
                                this.AccelerationY = 0;
                                this.Y += 15; // This number must be 1 unit larger than the margin of error in CheckGroundCollision(). Or not? Added a different check using m_droppingThroughGround that should be more reliable.
                                m_isTouchingGround = false;
                                m_isJumping = true;
                                m_dropThroughGroundTimer = m_dropThroughGroundDuration;
                            }
                        }
                    }
                }
            }
        }

        private void HookToSlope(IPhysicsObj collisionObj)
        {
            if (State != STATE_DASHING)
            {
                this.UpdateCollisionBoxes();

                // Code for hooking the player to a slope
                Rectangle elongatedRect = this.TerrainBounds;
                elongatedRect.Height += 20;
                //int checkAmount = 15;
                float x1 = this.X;

                Vector2 elongatedMTD = CollisionMath.RotatedRectIntersectsMTD(elongatedRect, this.Rotation, Vector2.Zero, collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                if (elongatedMTD.Y < 0) // Player is standing on a slope because the mtd is telling you to push him up.
                {
                    bool checkCollision = false;
                    float y1 = float.MaxValue;
                    Vector2 pt1, pt2;
                    if (collisionObj.Width > collisionObj.Height) // If rotated objects are done correctly.
                    {
                        pt1 = CollisionMath.UpperLeftCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                        pt2 = CollisionMath.UpperRightCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);

                        if (Math.Abs(collisionObj.Rotation - 45.0f) <= 1.0f) // Due to floating point imprecision, don't test against 45, but test within a threshold
                            x1 = this.TerrainBounds.Left;
                        else
                            x1 = this.TerrainBounds.Right;

                        if (x1 > pt1.X && x1 < pt2.X)
                            checkCollision = true;
                    }
                    else // If rotated objects are done Teddy's incorrect way.
                    {
                        if (Math.Abs(collisionObj.Rotation - 45.0f) <= 1.0f) // Due to floating point imprecision, don't test against 45, but test within a threshold
                        {
                            pt1 = CollisionMath.LowerLeftCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                            pt2 = CollisionMath.UpperLeftCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                            x1 = this.TerrainBounds.Right;
                            if (x1 > pt1.X && x1 < pt2.X)
                                checkCollision = true;
                        }
                        else
                        {
                            pt1 = CollisionMath.UpperRightCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                            pt2 = CollisionMath.LowerRightCorner(collisionObj.TerrainBounds, collisionObj.Rotation, Vector2.Zero);
                            x1 = this.TerrainBounds.Left;
                            if (x1 > pt1.X && x1 < pt2.X)
                                checkCollision = true;
                        }
                    }

                    if (checkCollision == true)
                    {
                        float u = pt2.X - pt1.X;
                        float v = pt2.Y - pt1.Y;
                        float x = pt1.X;
                        float y = pt1.Y;

                        y1 = y + (x1 - x) * (v / u);
                        y1 -= this.TerrainBounds.Bottom - this.Y - 2; // Up by 2 to ensure collision response doesn't kick in.
                        this.Y = (int)y1;
                    }
                }
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            IPhysicsObj otherBoxParent = otherBox.AbsParent as IPhysicsObj;
            TeleporterObj teleporter = otherBox.Parent as TeleporterObj;

            if (teleporter != null && ControlsLocked == false && IsTouchingGround == true)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    StopAllSpells();
                    this.LockControls();
                    m_lastTouchedTeleporter = teleporter;
                    Tween.RunFunction(0, this.AttachedLevel, "DisplayMap", true);
                    //Tween.AddEndHandlerToLastTween(AttachedLevel.ScreenManager, "DisplayScreen", ScreenType.Map, true, this);
                }
            }

            // Logic for moving the player from a boss entrance to a boss room, and vice versa.
            DoorObj door = otherBox.Parent as DoorObj;
            if (door != null && ControlsLocked == false && IsTouchingGround == true)
            {
                if (door.IsBossDoor == true && door.Locked == false && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2)))
                {
                    // This line ensures you don't access the donation box at the same time as entering a boss room.
                    if (door.Room.DonationBox == null || (door.Room.DonationBox != null && (door.Room.DonationBox.IsTouching == false || door.Room.DonationBox.Visible == false)))
                    {
                        if (door.Name == "FinalBossDoor")
                            Game.ScreenManager.DisplayScreen(ScreenType.Ending, true, null);
                        else
                        {
                            RoomObj linkedRoom = door.Room.LinkedRoom;
                            if (linkedRoom != null)
                            {
                                foreach (DoorObj possibleEntrance in linkedRoom.DoorList)
                                {
                                    if (possibleEntrance.IsBossDoor == true)
                                    {
                                        // Linking the LastBossChallengeRoom to the Boss Entrance you entered from.
                                        if (linkedRoom is LastBossChallengeRoom)
                                            linkedRoom.LinkedRoom = AttachedLevel.CurrentRoom;

                                        StopAllSpells();
                                        this.CurrentSpeed = 0;
                                        this.LockControls();
                                        (m_levelScreen.ScreenManager as RCScreenManager).StartWipeTransition();
                                        Vector2 roomPos = new Vector2(possibleEntrance.X + possibleEntrance.Width / 2f, possibleEntrance.Bounds.Bottom - (this.Bounds.Bottom - this.Y));
                                        Tween.RunFunction(0.2f, this, "EnterBossRoom", roomPos);
                                        Tween.RunFunction(0.2f, m_levelScreen.ScreenManager, "EndWipeTransition");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            BreakableObj breakableObj = otherBoxParent as BreakableObj;
            if (breakableObj != null)
            {
                if (IsAirAttacking == true && thisBox.Type == Consts.WEAPON_HITBOX)
                {
                    this.IsAirAttacking = false; // Only allow one object to perform upwards air knockback on the player.
                    this.AccelerationY = -this.AirAttackKnockBack;
                    this.NumAirBounces++;
                }
            }


            if (Game.PlayerStats.Traits.X == TraitType.NoFurniture || Game.PlayerStats.Traits.Y == TraitType.NoFurniture)
            {
                if (breakableObj != null)
                {
                    if (breakableObj.Broken == false)
                        breakableObj.Break();
                }

                if (otherBoxParent.GetType() == typeof(PhysicsObj) && (otherBoxParent as PhysicsObj).SpriteName != "CastleEntranceGate_Sprite") // Disables collision for things like book cases.
                    return;
            }

            // Check to see if the terrain collision is with a wall, not with another enemy's terrain hitbox.
            if (collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN &&
                (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_WALL || otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_WALL_FOR_PLAYER || otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_ENEMYWALL
                || otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL))
            {
                //if (otherBoxParent.CollidesBottom == false)
                //    m_lastPlatform = otherBoxParent;

                Vector2 mtdPos = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);

                float accelerationYHolder = AccelerationY;
                Vector2 rotatedMTDPos = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);

                bool checkCollision = true;

                if (m_dropThroughGroundTimer > 0 && otherBoxParent.CollidesBottom == false && otherBoxParent.CollidesTop == true)
                    checkCollision = false;

                // Used to make sure he doesn't hook to 1-way ground above him if he's already standing on flooring.
                if (m_isTouchingGround == true && otherBoxParent.CollidesBottom == false && otherBoxParent.CollidesTop == true && otherBoxParent.TerrainBounds.Top < this.TerrainBounds.Bottom - 10)
                    checkCollision = false;

                // This line is for one ways. If he drops down and is going slow enough, disable collision.  This is dangerous as it could result in you
                // dropping through platforms.  Also, doesn't work with really large platforms.
                if (otherBoxParent.CollidesBottom == false && this.Bounds.Bottom > otherBoxParent.TerrainBounds.Top + 10 && m_isTouchingGround == false)//this.AccelerationY < 600)
                    checkCollision = false;

                if (otherBoxParent.CollidesBottom == false && otherBoxParent.CollidesTop == true && (State == STATE_FLYING || State == STATE_DRAGON))
                    checkCollision = false;

                // Used to make sure he doesn't hook to two tiered walls.
                if ((m_collidingLeftOnly == true || m_collidingRightOnly == true) && Math.Abs(mtdPos.X) < 10 && m_isTouchingGround == false && otherBoxParent is HazardObj == false)
                    checkCollision = false;

                // Super hack to prevent player from moving through mouseholes regardless of his speed.
                if (otherBoxParent.CollidesLeft == false && otherBoxParent.CollidesRight == false && otherBoxParent.CollidesTop == true && otherBoxParent.CollidesBottom == true && otherBoxParent is HazardObj == false)
                {
                    if (Game.PlayerStats.Traits.X != TraitType.Dwarfism && Game.PlayerStats.Traits.Y != TraitType.Dwarfism)
                    {
                        if (this.X < otherBoxParent.TerrainBounds.Center.X)
                            this.X -= this.TerrainBounds.Right - otherBoxParent.TerrainBounds.Left;
                        else
                            this.X += otherBoxParent.TerrainBounds.Right - this.TerrainBounds.Left;
                    }
                    else
                        checkCollision = false;
                }

                // This is the step up code.
                //if (Math.Abs(mtdPos.X) < 10 && mtdPos.X != 0 && Math.Abs(mtdPos.Y) < 10 && mtdPos.Y != 0)
                //    checkCollision = false;

                // This code was for handling corners. Without it, if you were falling on a corner, your X translation would be smaller than the Y translation, causing your
                // player to be pushed out sideways when landing on a corner of an object, instead of upward ontop of the object.
                // But now it causes problems when standing on one ledge, and walking into another ledge if the differences are short enough. Instead of pushing you sideways, it pushes you upwards.
                if (m_isTouchingGround == true && m_closestGround == otherBoxParent)// && m_previousIsTouchingGround != m_isTouchingGround)
                {
                    checkCollision = false;
                    if (otherBoxParent is HazardObj && otherBoxParent.Rotation == -90) // I'm so tired. Super hack to make these effing hazard blocks work at this rotation.
                        this.Y += m_closestGround.Bounds.Top - this.TerrainBounds.Bottom + 15;
                    else
                        this.Y += m_closestGround.TerrainBounds.Top - this.TerrainBounds.Bottom;
                    this.AccelerationY = 0;
                }

                if (checkCollision == true)
                    base.CollisionResponse(thisBox, otherBox, collisionResponseType);
                if (rotatedMTDPos.Y != 0 && otherBox.AbsRotation != 0)// && otherBox.AbsRotation >= -SlopeClimbRotation && otherBox.AbsRotation <= SlopeClimbRotation) // Code to prevent player from sliding down rotated objects.
                {
                    //this.CurrentSpeed = 0; // Prevents player from dashing through angled blocks.
                    this.X -= rotatedMTDPos.X; // Prevents player from sliding down
                }
                // Disabled for now because it screws up when standing on turrets that are rotated more than SlopeClimbRotation.
                //else if (otherBox.AbsRotation < -SlopeClimbRotation || otherBox.AbsRotation > SlopeClimbRotation) // Prevents player from climbing slopes that are too steep.
                //    AccelerationY = accelerationYHolder;
            }

            if (thisBox.Type == Consts.BODY_HITBOX && otherBox.Type == Consts.WEAPON_HITBOX &&
                (otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_ENEMY || otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_ENEMYWALL || otherBoxParent.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL) && State != STATE_HURT && m_invincibleCounter <= 0)
            {
                EnemyObj enemy = otherBoxParent as EnemyObj;
                if (enemy != null && enemy.IsDemented == true)
                    return;
                ProjectileObj proj = otherBoxParent as ProjectileObj;
                if (proj != null && proj.IsDemented == true)
                    return;

                //if (proj != null && (proj.Spell == SpellType.Boomerang || proj.Spell == SpellType.Bounce))
                //    return;

                // Enemy hit player.  Response goes here.
                // Player blocked
                if (State == STATE_BLOCKING && (CurrentMana > 0 || m_blockInvincibleCounter > 0) && (proj == null || (proj != null && proj.Spell != SpellType.Boomerang && proj.Spell != SpellType.Bounce)))//CurrentMana >= BlockManaDrain)
                {
                    if (CanBeKnockedBack == true)
                    {
                        Point intersectPt = Rectangle.Intersect(thisBox.AbsRect, otherBox.AbsRect).Center;
                        Vector2 impactPosition = new Vector2(intersectPt.X, intersectPt.Y);
                        if (impactPosition == Vector2.Zero)
                            impactPosition = this.Position;
                        m_levelScreen.ImpactEffectPool.DisplayBlockImpactEffect(impactPosition, Vector2.One);

                        CurrentSpeed = 0;

                        if (otherBox.AbsParent.Bounds.Left + otherBox.AbsParent.Bounds.Width / 2 > this.X)
                            AccelerationX = -KnockBack.X;
                        else
                            AccelerationX = KnockBack.X;
                        AccelerationY = -KnockBack.Y;

                        Blink(Color.LightBlue, 0.1f);
                    }

                    if (m_blockInvincibleCounter <= 0)
                    {
                        CurrentMana -= BlockManaDrain;
                        m_blockInvincibleCounter = BlockInvincibleTime;
                        m_levelScreen.TextManager.DisplayNumberStringText(-GameEV.KNIGHT_BLOCK_DRAIN, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.SkyBlue, new Vector2(this.X, this.Bounds.Top));
                    }

                    SoundManager.PlaySound("Player_Block");
                    //SoundManager.PlaySound("Block1", "Block2", "Block3");
                }
                else // Player got hit.
                {
                    if (m_invincibleCounter <= 0)
                        HitPlayer(otherBox.AbsParent); // Not using otherBoxParent because that has been cast as an IPhysicsObj.
                }

                ProjectileObj projectile = otherBox.AbsParent as ProjectileObj;
                if (projectile != null && projectile.DestroysWithEnemy == true && m_assassinSpecialActive == false)
                    projectile.RunDestroyAnimation(true);
                //m_levelScreen.ProjectileManager.DestroyProjectile(projectile);
            }

            // Maybe this should go into the itemdropobj.
            ItemDropObj itemDrop = otherBoxParent as ItemDropObj;
            if (itemDrop != null && itemDrop.IsCollectable)
            {
                itemDrop.GiveReward(this, m_levelScreen.TextManager);
                itemDrop.IsCollidable = false;
                itemDrop.IsWeighted = false;
                itemDrop.AnimationDelay = 1 / 60f;
                itemDrop.AccelerationY = 0;
                itemDrop.AccelerationX = 0;
                Tweener.Tween.By(itemDrop, 0.4f, Tweener.Ease.Quad.EaseOut, "Y", "-120");
                Tweener.Tween.To(itemDrop, 0.1f, Tweener.Ease.Linear.EaseNone, "delay", "0.6", "Opacity", "0");
                Tweener.Tween.AddEndHandlerToLastTween(m_levelScreen.ItemDropManager, "DestroyItemDrop", itemDrop);
                //SoundManager.PlaySound("CoinCollect1", "CoinCollect2", "CoinCollect3");
                SoundManager.PlaySound("CoinDrop1", "CoinDrop2", "CoinDrop3", "CoinDrop4", "CoinDrop5");

                //m_levelScreen.ItemDropManager.DestroyItemDrop(itemDrop);
                //SoundManager.PlaySound("CoinPickup");
            }

            ChestObj chest = otherBoxParent as ChestObj;
            if (chest != null && ControlsLocked == false && m_isTouchingGround == true)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    if (chest.IsOpen == false)
                        chest.OpenChest(m_levelScreen.ItemDropManager, this);
                }
            }
        }

        public void PlayAttackSound()
        {
            if (Game.PlayerStats.IsFemale == true)
                SoundManager.PlaySound("Player_Female_Effort_03", "Player_Female_Effort_04", "Player_Female_Effort_05", "Player_Female_Effort_06", "Player_Female_Effort_07",
                    "Blank", "Blank", "Blank");
            else
                SoundManager.PlaySound("Player_Male_Effort_01", "Player_Male_Effort_02", "Player_Male_Effort_04", "Player_Male_Effort_05", "Player_Male_Effort_07",
                      "Blank", "Blank", "Blank");

            if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                SoundManager.PlaySound("Player_Attack_Sword_Spell_01", "Player_Attack_Sword_Spell_02", "Player_Attack_Sword_Spell_03");
            else
            {
                if (IsAirAttacking == false)
                {
                    if (Game.PlayerStats.Traits.X == TraitType.Hypergonadism || Game.PlayerStats.Traits.Y == TraitType.Hypergonadism)
                        SoundManager.PlaySound("Player_Attack01_Low", "Player_Attack02_Low");
                    else if (Game.PlayerStats.Traits.X == TraitType.Hypogonadism || Game.PlayerStats.Traits.Y == TraitType.Hypogonadism)
                        SoundManager.PlaySound("Player_Attack01_High", "Player_Attack02_High");
                    else
                        SoundManager.PlaySound("Player_Attack01", "Player_Attack02");
                }
                else
                {
                    if (Game.PlayerStats.Traits.X == TraitType.Hypergonadism || Game.PlayerStats.Traits.Y == TraitType.Hypergonadism)
                        SoundManager.PlaySound("Player_AttackDown01_Low", "Player_AttackDown02_Low");
                    else if (Game.PlayerStats.Traits.X == TraitType.Hypogonadism || Game.PlayerStats.Traits.Y == TraitType.Hypogonadism)
                        SoundManager.PlaySound("Player_AttackDown01_High", "Player_AttackDown02_High");
                    else
                        SoundManager.PlaySound("Player_AttackDown01", "Player_AttackDown02");
                }
            }
        }

        public void EnterBossRoom(Vector2 position)
        {
            this.Position = position;
        }

        public void TeleportPlayer(Vector2 position, TeleporterObj teleporter = null)
        {
            this.CurrentSpeed = 0;
            this.Scale = m_internalScale;

            if (teleporter == null)
                teleporter = m_lastTouchedTeleporter;
            Console.WriteLine("Player pos: " + this.Position + " teleporter: " + teleporter.Position);

            Tween.To(this, 0.4f, Tweener.Ease.Linear.EaseNone, "X", teleporter.X.ToString());
            Tween.To(this, 0.05f, Tweener.Ease.Linear.EaseNone, "delay", "1.5", "ScaleX", "0");
            Vector2 storedScale = this.Scale;
            this.ScaleX = 0;
            Tween.To(this, 0.05f, Tweener.Ease.Linear.EaseNone, "delay", "3.3", "ScaleX", storedScale.X.ToString());
            this.ScaleX = storedScale.X;
            Vector2 playerTeleportPos = new Vector2(position.X, position.Y - (this.TerrainBounds.Bottom - this.Y));

            LogicSet teleportLS = new LogicSet(this);
            teleportLS.AddAction(new RunFunctionLogicAction(this, "LockControls"));
            teleportLS.AddAction(new ChangeSpriteLogicAction("PlayerJumping_Character"));
            teleportLS.AddAction(new JumpLogicAction(500));
            teleportLS.AddAction(new PlaySoundLogicAction("Player_Jump_04"));
            teleportLS.AddAction(new RunFunctionLogicAction(teleporter, "SetCollision", true));
            teleportLS.AddAction(new DelayLogicAction(0.4f));
            teleportLS.AddAction(new GroundCheckLogicAction());
            teleportLS.AddAction(new ChangeSpriteLogicAction("PlayerLevelUp_Character", true, false), Types.Sequence.Parallel);
            teleportLS.AddAction(new DelayLogicAction(0.1f));
            teleportLS.AddAction(new RunFunctionLogicAction(this.AttachedLevel.ImpactEffectPool, "DisplayTeleportEffect", new Vector2(teleporter.X, teleporter.Bounds.Top)));
            teleportLS.AddAction(new DelayLogicAction(1f));
            teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Disappear"));
            teleportLS.AddAction(new RunFunctionLogicAction(this.AttachedLevel.ImpactEffectPool, "MegaTeleport", new Vector2(teleporter.X, teleporter.Bounds.Top), this.Scale));
            teleportLS.AddAction(new DelayLogicAction(0.8f));
            teleportLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ScreenManager, "StartWipeTransition"));
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new RunFunctionLogicAction(teleporter, "SetCollision", false));
            teleportLS.AddAction(new TeleportLogicAction(null, playerTeleportPos));
            teleportLS.AddAction(new DelayLogicAction(0.05f));
            teleportLS.AddAction(new RunFunctionLogicAction(m_levelScreen.ScreenManager, "EndWipeTransition"));
            teleportLS.AddAction(new DelayLogicAction(0.5f));
            teleportLS.AddAction(new RunFunctionLogicAction(this.AttachedLevel.ImpactEffectPool, "MegaTeleportReverse", new Vector2(position.X - 5, position.Y + 57), storedScale));
            teleportLS.AddAction(new PlaySoundLogicAction("Teleport_Reappear"));
            teleportLS.AddAction(new DelayLogicAction(0.2f));
            teleportLS.AddAction(new RunFunctionLogicAction(this, "LastBossDoorHack"));
            //teleportLS.AddAction(new RunFunctionLogicAction(this, "UnlockControls"));
            this.RunExternalLogicSet(teleportLS);
        }

        public void LastBossDoorHack()
        {
            if (m_levelScreen.CurrentRoom is CastleEntranceRoomObj && Game.PlayerStats.EyeballBossBeaten == true && Game.PlayerStats.FairyBossBeaten == true && Game.PlayerStats.BlobBossBeaten == true && Game.PlayerStats.FireballBossBeaten == true
               && Game.PlayerStats.FinalDoorOpened == false)
            {
                (m_levelScreen.CurrentRoom as CastleEntranceRoomObj).PlayBossDoorAnimation();
                Game.PlayerStats.FinalDoorOpened = true;
                m_levelScreen.RunCinematicBorders(6); // Hack to prevent cinematic border conflict.
            }
            else
                UnlockControls();
        }

        public void RunExternalLogicSet(LogicSet ls)
        {
            if (m_currentLogicSet != null && m_currentLogicSet.IsActive == true)
                m_currentLogicSet.Stop();

            this.AnimationDelay = 1 / 10f;

            if (m_externalLS != null)
                m_externalLS.Dispose();

            m_externalLS = ls;
            m_externalLS.Execute();
        }

        public void HitPlayer(GameObj obj)
        {
            bool hitPlayer = true;
            if (obj is HazardObj)
            {
                if ((Game.PlayerStats.SpecialItem == SpecialItemType.SpikeImmunity && obj.Bounds.Top > this.Y) || InvincibleToSpikes == true)
                    hitPlayer = false;
            }

            ProjectileObj projectile = obj as ProjectileObj;
            if (projectile != null)
            {
                if (projectile.IsDemented == true)
                    hitPlayer = false;
                else if (projectile.Spell == SpellType.Bounce || projectile.Spell == SpellType.Boomerang)
                {
                    hitPlayer = false;
                    projectile.KillProjectile();
                    m_levelScreen.ImpactEffectPool.SpellCastEffect(projectile.Position, CDGMath.AngleBetweenPts(this.Position, projectile.Position), false);
                }

                // Check to make sure you don't die at the same time as a boss if you are hit by the boss's projectile the moment you kill him.
                EnemyObj projectileSource = projectile.Source as EnemyObj;
                if (projectileSource != null && (projectileSource.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS || projectileSource is EnemyObj_LastBoss) && projectileSource.CurrentHealth <= 0)
                    hitPlayer = false;
            }

            EnemyObj dementedEnemy = obj as EnemyObj;
            if (dementedEnemy != null && dementedEnemy.IsDemented == true)
                hitPlayer = false;

            // A check to make sure player's that die to enemies at the same time as hitting back, won't screw things up.
            if (dementedEnemy != null && (dementedEnemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS || dementedEnemy is EnemyObj_LastBoss) && dementedEnemy.CurrentHealth <= 0)
                hitPlayer = false;

            // Disable force invincibility on spikes
            if (hitPlayer == true && (ForceInvincible == false || (ForceInvincible == true && obj is HazardObj)))
            {
                Blink(Color.Red, 0.1f);
                this.m_levelScreen.ImpactEffectPool.DisplayPlayerImpactEffect(this.Position);
                //this.TextureColor = Color.White;
                this.AccelerationYEnabled = true; // In case player acceleration is disabled during a dash.
                this.UnlockControls(); // Unlock player controls in case they were locked during a dash.
                int damage = (obj as IDealsDamageObj).Damage;
                damage = (int)((damage - (damage * TotalDamageReduc)) * ClassDamageTakenMultiplier);
                if (damage < 0) damage = 0;

                // Prevent the player from taking any damage in the tutorial.
                if (Game.PlayerStats.TutorialComplete == false)
                    damage = 0;

                if (Game.PlayerStats.GodMode == false)
                    this.CurrentHealth -= damage;

                EnemyObj enemyObj = obj as EnemyObj;
                if (enemyObj != null && CurrentHealth > 0)
                {
                    int damageReturn = (int)(damage * TotalDamageReturn);
                    if (damageReturn > 0)
                        enemyObj.HitEnemy(damageReturn, enemyObj.Position, true);
                }

                // projectile is defined up top.
                if (projectile != null && projectile.CollisionTypeTag == GameTypes.CollisionType_ENEMY)
                {
                    EnemyObj enemy = projectile.Source as EnemyObj;
                    if (enemy != null && enemy.IsKilled == false && enemy.IsDemented == false && CurrentHealth > 0)
                    {
                        int damageReturn = (int)(damage * TotalDamageReturn);
                        if (damageReturn > 0)
                            enemy.HitEnemy(damageReturn, enemy.Position, true);
                    }
                }

                // Needed for chests.
                m_isJumping = false;
                
                // Disables flying.
                m_isFlying = false;
                this.DisableGravity = false;

                if (CanBeKnockedBack == true)//(CanBeKnockedBack == true && (Game.PlayerStats.Traits.X != TraitType.Endomorph && Game.PlayerStats.Traits.Y != TraitType.Endomorph))// && Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.Knockback_Immune) <= 0) // If you have knockback immune trait, don't get knocked back //TEDDY MAKING KNOCKBACK NOT 0 FOR ECTOMORPH
                {
                    if (Game.PlayerStats.Traits.X == TraitType.Tourettes || Game.PlayerStats.Traits.Y == TraitType.Tourettes)
                    {
                        int randBubble = CDGMath.RandomInt(1, 4);
                        m_swearBubble.ChangeSprite("SwearBubble" + randBubble + "_Sprite");
                        m_swearBubble.Visible = true;
                        m_swearBubbleCounter = 1f;
                    }

                    State = STATE_HURT;

                    UpdateAnimationState(); // Force his animation to a hurt state.
                    if (m_currentLogicSet.IsActive)
                        m_currentLogicSet.Stop();
                    IsAirAttacking = false;
                    AnimationDelay = m_startingAnimationDelay;

                    CurrentSpeed = 0;

                    float knockbackMod = 1;// Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.Knockback_Weak);
                    if (Game.PlayerStats.Traits.X == TraitType.Ectomorph || Game.PlayerStats.Traits.Y == TraitType.Ectomorph)
                        knockbackMod = GameEV.TRAIT_ECTOMORPH;

                    if (Game.PlayerStats.Traits.X == TraitType.Endomorph || Game.PlayerStats.Traits.Y == TraitType.Endomorph)
                        knockbackMod = GameEV.TRAIT_ENDOMORPH;

                    if (obj.Bounds.Left + obj.Bounds.Width / 2 > this.X)
                        AccelerationX = -KnockBack.X * knockbackMod;
                    else
                        AccelerationX = KnockBack.X * knockbackMod;
                    AccelerationY = -KnockBack.Y * knockbackMod;
                }
                else
                    m_invincibleCounter = InvincibilityTime; // Kick in the invincible timer automatically if the player is hit and knockback_immune trait exists.

                if (CurrentHealth <= 0)
                {
                    if (Game.PlayerStats.SpecialItem == SpecialItemType.Revive)
                    {
                        this.CurrentHealth = (int)(MaxHealth * 0.25f);
                        Game.PlayerStats.SpecialItem = SpecialItemType.None;
                        (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).UpdatePlayerHUDSpecialItem();
                        m_invincibleCounter = InvincibilityTime; // Kick in the invincible timer automatically if the player is hit and knockback_immune trait exists.
                        (m_levelScreen.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.DeathDefy, true);
                    }
                    else
                    {
                        int chanceToSurvive = CDGMath.RandomInt(1, 100);
                        if (chanceToSurvive <= SkillSystem.GetSkill(SkillType.Death_Dodge).ModifierAmount * 100)
                        {
                            //this.CurrentHealth = 1;
                            this.CurrentHealth = (int)(MaxHealth * 0.1f);
                            m_invincibleCounter = InvincibilityTime; // Kick in the invincible timer automatically if the player is hit and knockback_immune trait exists.
                            (m_levelScreen.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.DeathDefy, true);
                        }
                        else
                        {
                            ChallengeBossRoomObj challengeRoom = AttachedLevel.CurrentRoom as ChallengeBossRoomObj;
                            if (challengeRoom != null)
                            {
                                challengeRoom.KickPlayerOut();
                            }
                            else
                            {
                                this.AttachedLevel.SetObjectKilledPlayer(obj);
                                Kill();
                            }
                        }
                    }
                }

                if (m_levelScreen.IsDisposed == false)
                {
                    if (Game.PlayerStats.Traits.X == TraitType.Hypochondriac || Game.PlayerStats.Traits.Y == TraitType.Hypochondriac)
                        m_levelScreen.TextManager.DisplayNumberText(damage * 100 + CDGMath.RandomInt(1,99), Color.Red, new Vector2(this.X, this.Bounds.Top));
                    else
                        m_levelScreen.TextManager.DisplayNumberText(damage, Color.Red, new Vector2(this.X, this.Bounds.Top));
                }

                if (Game.PlayerStats.SpecialItem == SpecialItemType.LoseCoins)
                {
                    int numCoinsLost = (int)(Game.PlayerStats.Gold * 0.25f / ItemDropType.CoinAmount);
                    if (numCoinsLost > 50) numCoinsLost = 50;
                    if (numCoinsLost > 0 && AttachedLevel.ItemDropManager.AvailableItems > numCoinsLost)
                    {
                        float castleLockGoldModifier = 1;
                        if (Game.PlayerStats.HasArchitectFee == true)
                            castleLockGoldModifier = GameEV.ARCHITECT_FEE;
                        //Game.PlayerStats.Gold -= numCoinsLost * ItemDropType.CoinAmount;
                        int goldAmount = (int)(((numCoinsLost * ItemDropType.CoinAmount) * (1 + this.TotalGoldBonus)) * castleLockGoldModifier);
                        Game.PlayerStats.Gold -= goldAmount;
                        for (int i = 0; i < numCoinsLost; i++)
                            m_levelScreen.ItemDropManager.DropItemWide(this.Position, ItemDropType.Coin, ItemDropType.CoinAmount);

                        if (goldAmount > 0)
                            AttachedLevel.TextManager.DisplayNumberStringText(-(int)goldAmount, "LOC_ID_PLAYER_OBJ_1" /*"gold"*/, Color.Yellow, new Vector2(this.X, this.Bounds.Top));
                        //SoundManager.PlaySound("SonicRingOut");
                    }
                }

                if (Game.PlayerStats.IsFemale == true)
                    SoundManager.PlaySound("Player_Female_Damage_03", "Player_Female_Damage_04", "Player_Female_Damage_05", "Player_Female_Damage_06", "Player_Female_Damage_07");
                else
                    SoundManager.PlaySound("Player_Male_Injury_01", "Player_Male_Injury_02", "Player_Male_Injury_03", "Player_Male_Injury_04", "Player_Male_Injury_05",
                        "Player_Male_Injury_06", "Player_Male_Injury_07", "Player_Male_Injury_08", "Player_Male_Injury_09", "Player_Male_Injury_10");
                SoundManager.PlaySound("EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
            }
        }

        // Really just for exiting compass rooms. Makes you invincible for a short period so that you don't die on exit.
        public void KickInHitInvincibility()
        {
            m_invincibleCounter = InvincibilityTime;
        }

        public override void Kill(bool giveXP = true)
        {
            ChallengeBossRoomObj challengeRoom = AttachedLevel.CurrentRoom as ChallengeBossRoomObj;
            if (challengeRoom != null)
            {
                challengeRoom.LoadPlayerData();
                Game.SaveManager.LoadFiles(AttachedLevel, SaveType.UpgradeData);
                CurrentHealth = 0;
            }

            m_translocatorSprite.Visible = false;
            m_swearBubble.Visible = false;
            m_swearBubbleCounter = 0;

            Game.PlayerStats.IsDead = true;

            m_isKilled = true;
            AttachedLevel.RunGameOver();
            base.Kill(giveXP);
        }

        public void RunDeathAnimation1()
        {
            if (Game.PlayerStats.IsFemale == true)
                SoundManager.PlaySound("Player_Female_Death_01", "Player_Female_Death_02");
            else
                SoundManager.PlaySound("Player_Male_Death_01", "Player_Male_Death_02", "Player_Male_Death_03", "Player_Male_Death_04", "Player_Male_Death_05",
                    "Player_Male_Death_06", "Player_Male_Death_07", "Player_Male_Death_08", "Player_Male_Death_09");
            this.ChangeSprite("PlayerDeath_Character");
            this.PlayAnimation(false);

            if (_objectList[PlayerPart.Wings].Visible == true)
                Tween.To(_objectList[PlayerPart.Wings], 0.5f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
        }

        public void RunGetItemAnimation()
        {
            //m_levelScreen.PauseScreen();
            //(m_levelScreen.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true);

            if (m_currentLogicSet != null && m_currentLogicSet.IsActive == true)
                m_currentLogicSet.Stop();
            AnimationDelay = m_startingAnimationDelay;
            this.ChangeSprite("PlayerLevelUp_Character");
            this.PlayAnimation(false);
        }

        public void CancelAttack()
        {
            if (m_currentLogicSet == m_standingAttack3LogicSet || m_currentLogicSet == m_airAttackLS)
                m_currentLogicSet.Stop();

            AnimationDelay = m_startingAnimationDelay;
        }

        public void RoomTransitionReset()
        {
            m_timeStopCast = false;

            m_translocatorSprite.Visible = false;
            //m_assassinSpecialActive = false;
            //this.Opacity = 1;
            //ForceInvincible = false;
        }

        public override void Reset()
        {
            if (m_currentLogicSet.IsActive)
                m_currentLogicSet.Stop();

            State = STATE_IDLE;
            m_invincibleCounter = 0;

            // These need to be called again, because some traits modify EV and Logic sets so they need to be re-initialized.
            InitializeEV();
            AnimationDelay = m_startingAnimationDelay;
            InitializeLogic();
            IsAirAttacking = false;
            NumSequentialAttacks = 0;

            m_flightCounter = TotalFlightTime;
            m_isFlying = false;
            AccelerationYEnabled = true;
            this.Position = Vector2.One;
            UpdateEquipmentColours();

            m_assassinSpecialActive = false;
            m_wizardSparkleCounter = 0.2f;
            DisableGravity = false;
            InvincibleToSpikes = false;
            ForceInvincible = false;
            this.DisableAllWeight = false;
            base.Reset();
        }

        public void ResetLevels()
        {
            //Game.PlayerStats.CurrentLevel = 0;
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
            //Game.PlayerStats.XP = 0;
        }

        public void StopDash()
        {
            m_dashCounter = 0;
        }

        // Helper method that will update the player's armor colours automatically based on the equipment he is wearing.
        public void UpdateEquipmentColours()
        {
            if (this.State != STATE_TANOOKI)
            {
                for (int i = 0; i < Game.PlayerStats.GetEquippedArray.Length; i++)
                {
                    int equippedItem = Game.PlayerStats.GetEquippedArray[i];

                    if (equippedItem != -1) // -1 means no item equipped.
                    {
                        EquipmentData equipmentData = Game.EquipmentSystem.GetEquipmentData(i, equippedItem);
                        Vector3 playerPart = PlayerPart.GetPartIndices(i);

                        if (playerPart.X != -1)
                            this.GetChildAt((int)playerPart.X).TextureColor = equipmentData.FirstColour;
                        if (playerPart.Y != -1)
                            this.GetChildAt((int)playerPart.Y).TextureColor = equipmentData.SecondColour;
                        if (playerPart.Z != -1)
                            this.GetChildAt((int)playerPart.Z).TextureColor = equipmentData.SecondColour;

                        // Special handling to tint the female's boobs.
                        if (i == EquipmentCategoryType.Chest && playerPart.X != PlayerPart.None)
                            this.GetChildAt(PlayerPart.Boobs).TextureColor = equipmentData.FirstColour;
                    }
                    else // The player is dequipping
                    {
                        Vector3 playerPart = PlayerPart.GetPartIndices(i);

                        // Set all part pieces to white first.
                        if (playerPart.X != -1)
                            this.GetChildAt((int)playerPart.X).TextureColor = Color.White;
                        if (playerPart.Y != -1)
                            this.GetChildAt((int)playerPart.Y).TextureColor = Color.White;
                        if (playerPart.Z != -1)
                            this.GetChildAt((int)playerPart.Z).TextureColor = Color.White;

                        // Special handling to tint the female's boobs.
                        if (i == EquipmentCategoryType.Chest)
                            this.GetChildAt(PlayerPart.Boobs).TextureColor = Color.White;

                        // Special handling to make the default player's cape and helm red instead of white.
                        if (i == EquipmentCategoryType.Helm)
                            this.GetChildAt(PlayerPart.Hair).TextureColor = Color.Red;
                        else if (i == EquipmentCategoryType.Cape)
                        {
                            if (playerPart.X != -1)
                                this.GetChildAt((int)playerPart.X).TextureColor = Color.Red;
                            if (playerPart.Y != -1)
                                this.GetChildAt((int)playerPart.Y).TextureColor = Color.Red;
                        }
                        else if (i == EquipmentCategoryType.Sword)
                        {
                            if (playerPart.Y != -1)
                                this.GetChildAt((int)playerPart.Y).TextureColor = new Color(11, 172, 239); // Light blue
                        }

                        Color darkPink = new Color(251, 156, 172);
                        this.GetChildAt(PlayerPart.Bowtie).TextureColor = darkPink;
                    }
                }
            }
        }

        public void CastSpell(bool activateSecondary, bool megaSpell = false)
        {
            byte spellType = Game.PlayerStats.Spell;

            Color textureColor = Color.White;
            ProjectileData projData = SpellEV.GetProjData(spellType, this);

            float damageMult = SpellEV.GetDamageMultiplier(spellType);
            projData.Damage = (int)(TotalMagicDamage * damageMult);

            int manaCost = (int)(SpellEV.GetManaCost(spellType) * (1 - SkillSystem.GetSkill(SkillType.Mana_Cost_Down).ModifierAmount));
            if (CurrentMana >= manaCost)
            {
                m_spellCastDelay = 0.5f;
                if (AttachedLevel.CurrentRoom is CarnivalShoot1BonusRoom == false && AttachedLevel.CurrentRoom is CarnivalShoot2BonusRoom == false)
                {
                    if (Game.PlayerStats.Traits.X == TraitType.Savant || Game.PlayerStats.Traits.Y == TraitType.Savant)
                    {
                        if (Game.PlayerStats.Class != ClassType.Dragon && Game.PlayerStats.Class != ClassType.Traitor)
                        {
                            byte[] spellList = ClassType.GetSpellList(Game.PlayerStats.Class);
                            do
                            {
                                Game.PlayerStats.Spell = spellList[CDGMath.RandomInt(0, spellList.Length - 1)];
                            } while (Game.PlayerStats.Spell == SpellType.Translocator || Game.PlayerStats.Spell == SpellType.TimeStop || Game.PlayerStats.Spell == SpellType.DamageShield);
                            AttachedLevel.UpdatePlayerSpellIcon();
                        }
                    }
                }
            }

            float altX = SpellEV.GetXValue(spellType);
            float altY = SpellEV.GetYValue(spellType);

            if (megaSpell == true)
            {
                manaCost = (int)(manaCost * GameEV.SPELLSWORD_MANACOST_MOD);
                projData.Scale *= GameEV.SPELLSWORD_SPELL_SCALE;//2;
                projData.Damage = (int)(projData.Damage * GameEV.SPELLSWORD_SPELLDAMAGE_MOD);
            }

            if (this.CurrentMana < manaCost)
                SoundManager.PlaySound("Error_Spell");
            else if (spellType != SpellType.Translocator && spellType != SpellType.Nuke && m_damageShieldCast == false && manaCost > 0)
                m_levelScreen.TextManager.DisplayNumberStringText(-manaCost, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.SkyBlue, new Vector2(this.X, this.Bounds.Top));

            if (spellType != SpellType.Bounce && spellType != SpellType.DamageShield)
            {
                if (Game.PlayerStats.Traits.X == TraitType.Ambilevous || Game.PlayerStats.Traits.Y == TraitType.Ambilevous)
                    projData.SourceAnchor = new Vector2(projData.SourceAnchor.X * -1, projData.SourceAnchor.Y);
            }

            switch (spellType)
            {
                case (SpellType.Dagger):
                case (SpellType.Axe):
                case (SpellType.TimeBomb):
                case (SpellType.Displacer):
                case (SpellType.Close):
                case (SpellType.DualBlades):
                case (SpellType.DragonFire):
                case (SpellType.DragonFireNeo):
                    if (this.CurrentMana >= manaCost && activateSecondary == false)
                    {
                        if (spellType == SpellType.DragonFireNeo)
                        {
                            projData.Lifespan = SpellEV.DRAGONFIRENEO_XVal;
                            projData.WrapProjectile = true;
                        }

                        if (spellType == SpellType.Dagger)
                            SoundManager.PlaySound("Cast_Dagger");
                        else if (spellType == SpellType.Axe)
                            SoundManager.PlaySound("Cast_Axe");
                        else if (spellType == SpellType.DualBlades)
                            SoundManager.PlaySound("Cast_Chakram");
                        else if (spellType == SpellType.Close)
                            SoundManager.PlaySound("Cast_GiantSword");
                        else if (spellType == SpellType.DragonFire || spellType == SpellType.DragonFireNeo)
                            SoundManager.PlaySound("Enemy_WallTurret_Fire_01", "Enemy_WallTurret_Fire_02", "Enemy_WallTurret_Fire_03", "Enemy_WallTurret_Fire_04");

                        ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                        proj.Spell = spellType;
                        proj.TextureColor = textureColor;
                        proj.AltY = altY;
                        proj.AltX = altX;
                        if (spellType == SpellType.Boomerang && this.Flip == SpriteEffects.FlipHorizontally)
                            proj.AltX = -altX;
                        if (spellType == SpellType.Close)
                        {
                            proj.LifeSpan = altX;
                            proj.Opacity = 0;
                            proj.Y -= 20;
                            Tween.By(proj, 0.1f, Tween.EaseNone, "Y", "20");
                            Tween.To(proj, 0.1f, Tween.EaseNone, "Opacity", "1");
                        }
                        if (spellType == SpellType.DualBlades)
                        {
                            projData.Angle = new Vector2(-10, -10);
                            if (Game.PlayerStats.Traits.X == TraitType.Ambilevous || Game.PlayerStats.Traits.Y == TraitType.Ambilevous)
                            {
                                projData.SourceAnchor = new Vector2(-50, -30);
                                m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, -proj.Rotation, megaSpell);
                            }
                            else
                            {
                                projData.SourceAnchor = new Vector2(50, -30);
                                m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);
                            }

                            projData.RotationSpeed = -20;
                            proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                        }
                        if (spellType == SpellType.TimeBomb)
                        {
                            proj.ShowIcon = true;
                            proj.Rotation = 0;
                            proj.BlinkTime = altX / 1.5f;
                            proj.LifeSpan = 20;
                        }
                        if (spellType == SpellType.Displacer)
                        {
                            proj.Rotation = 0;
                            proj.RunDisplacerEffect(m_levelScreen.CurrentRoom, this);
                            proj.KillProjectile();
                        }
                        
                        if (spellType == SpellType.Close)
                            m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, 90, megaSpell);
                        else if (Game.PlayerStats.Traits.X == TraitType.Ambilevous || Game.PlayerStats.Traits.Y == TraitType.Ambilevous)
                            m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, -proj.Rotation, megaSpell);
                        else
                            m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);

                        this.CurrentMana -= manaCost;
                    }
                    break;
                case (SpellType.Boomerang):
                    if (this.CurrentMana >= manaCost && activateSecondary == false)
                    {
                        SoundManager.PlaySound("Cast_Boomerang");
                        ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                        proj.Spell = spellType;
                        proj.IgnoreBoundsCheck = true;
                        proj.TextureColor = textureColor;
                        proj.ShowIcon = true;
                        proj.AltX = altX;
                        if ((this.Flip == SpriteEffects.FlipHorizontally && Game.PlayerStats.Traits.X != TraitType.Ambilevous && Game.PlayerStats.Traits.Y != TraitType.Ambilevous)
                            || (this.Flip == SpriteEffects.None && (Game.PlayerStats.Traits.X == TraitType.Ambilevous || Game.PlayerStats.Traits.Y == TraitType.Ambilevous)))
                            proj.AltX = -altX;
                        proj.AltY = 0.5f;

                        this.CurrentMana -= manaCost;
                        m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);
                    }
                    break;
                case (SpellType.Bounce):
                    if (this.CurrentMana >= manaCost && activateSecondary == false)
                    {
                        SoundManager.PlaySound("Cast_Dagger");
                        for (int i = 0; i < 4; i++)
                        {
                            ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                            proj.Orientation = MathHelper.ToRadians(projData.Angle.X);
                            proj.Spell = spellType;
                            proj.ShowIcon = true;
                            proj.AltX = altX;
                            proj.AltY = 0.5f; // 0.1 secs till it can hit you.
                            switch (i)
                            {
                                case (0): projData.SourceAnchor = new Vector2(10, -10); break;
                                case (1): projData.SourceAnchor = new Vector2(10, 10); break;
                                case (2): projData.SourceAnchor = new Vector2(-10, 10); break;
                            }
                            projData.Angle = new Vector2(projData.Angle.X + 90, projData.Angle.Y + 90);
                            m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);
                        }
                        this.CurrentMana -= manaCost;
                    }
                    break;
                case (SpellType.Nuke):
                    int numEnemies = this.AttachedLevel.CurrentRoom.ActiveEnemies;
                    int spellCap = 9;//10;//15;//20;

                    if (numEnemies > spellCap)
                        numEnemies = spellCap;
                    if (this.CurrentMana >= manaCost && activateSecondary == false && numEnemies > 0)
                    {
                        SoundManager.PlaySound("Cast_Crowstorm");
                        int projectileDistance = 200;
                        float angle = (360f / numEnemies);
                        float startingAngle = 0;

                        int spellCapCounter = 0;
                        foreach (EnemyObj enemy in AttachedLevel.CurrentRoom.EnemyList)
                        {
                            if (enemy.NonKillable == false && enemy.IsKilled == false)
                            {
                                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                                proj.LifeSpan = 10;//99;
                                proj.AltX = 0.25f; // The length of time the birds shoot out before they attack.
                                proj.AltY = 0.05f; // The counter for the crow's smoke.
                                proj.Orientation = MathHelper.ToRadians(startingAngle);
                                proj.Spell = spellType;
                                proj.TurnSpeed = 0.075f;//0.065f;//0.05f;
                                proj.IgnoreBoundsCheck = true;
                                proj.Target = enemy;
                                proj.CollisionTypeTag = GameTypes.CollisionType_WALL;
                                proj.Position = CDGMath.GetCirclePosition(startingAngle, projectileDistance, this.Position);
                                m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);
                                startingAngle += angle;
                                spellCapCounter++;
                            }

                            if (spellCapCounter > spellCap)
                                break;
                        }

                        foreach (EnemyObj enemy in AttachedLevel.CurrentRoom.TempEnemyList)
                        {
                            if (enemy.NonKillable == false && enemy.IsKilled == false)
                            {
                                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                                proj.LifeSpan = 99;
                                proj.AltX = 0.25f; // The length of time the birds shoot out before they attack.
                                proj.AltY = 0.05f; // The counter for the crow's smoke.
                                proj.Orientation = MathHelper.ToRadians(startingAngle);
                                proj.Spell = spellType;
                                proj.TurnSpeed = 0.05f;
                                proj.IgnoreBoundsCheck = true;
                                proj.Target = enemy;
                                proj.CollisionTypeTag = GameTypes.CollisionType_WALL;
                                proj.Position = CDGMath.GetCirclePosition(startingAngle, projectileDistance, this.Position);
                                m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);
                                startingAngle += angle;
                                spellCapCounter++;
                            }

                            if (spellCapCounter > spellCap)
                                break;
                        }

                        this.CurrentMana -= manaCost;
                        m_levelScreen.TextManager.DisplayNumberStringText(-manaCost, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.SkyBlue, new Vector2(this.X, this.Bounds.Top));
                    }
                    break;
                case (SpellType.DamageShield):
                    if (m_damageShieldCast == true)
                    {
                        m_damageShieldCast = false;
                        m_megaDamageShieldCast = false;
                    }
                    else if (this.CurrentMana >= manaCost && activateSecondary == false)
                    {
                        m_damageShieldCast = true;
                        if (megaSpell == true)
                            m_megaDamageShieldCast = true;
                        SoundManager.PlaySound("Cast_FireShield");
                        int projectileDistance = 200;
                        for (int i = 0; i < (int)altY; i++)
                        {
                            float angle = (360f / altY) * i;

                            ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                            proj.LifeSpan = altX;
                            proj.AltX = angle; // AltX and AltY are used as holders to hold the projectiles angle and distance from player respectively.
                            proj.AltY = projectileDistance;
                            proj.Spell = spellType;
                            proj.AccelerationXEnabled = false;
                            proj.AccelerationYEnabled = false;
                            proj.IgnoreBoundsCheck = true;
                            m_levelScreen.ImpactEffectPool.SpellCastEffect(proj.Position, proj.Rotation, megaSpell);
                        }
                        this.CurrentMana -= manaCost;
                    }
                    break;
                case (SpellType.Laser):
                    if (this.CurrentMana >= manaCost && activateSecondary == false)
                    {
                        ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
                        proj.AltX = 1;
                        proj.AltY = 0.5f;
                        proj.Opacity = 0f;
                        proj.X = AttachedLevel.CurrentRoom.X;
                        proj.Y = this.Y;
                        proj.Scale = new Vector2((float)(AttachedLevel.CurrentRoom.Width / proj.Width), 0);
                        proj.IgnoreBoundsCheck = true;
                        proj.Spell = spellType;
                        this.CurrentMana -= manaCost;
                    }
                    break;
                case (SpellType.TimeStop):
                    if (m_timeStopCast == true)
                    {
                        AttachedLevel.StopTimeStop();
                        m_timeStopCast = false;
                    }
                    else
                    {
                        if (this.CurrentMana >= manaCost && activateSecondary == false)
                        {
                            this.CurrentMana -= manaCost;
                            AttachedLevel.CastTimeStop(0);
                            m_timeStopCast = true;
                        }
                    }
                    break;
                case (SpellType.Translocator):
                    if (m_translocatorSprite.Visible == false && this.CurrentMana >= manaCost)
                    {
                        this.CurrentMana -= manaCost;
                        m_translocatorSprite.ChangeSprite(this.SpriteName);
                        m_translocatorSprite.GoToFrame(this.CurrentFrame);
                        m_translocatorSprite.Visible = true;
                        m_translocatorSprite.Position = this.Position;
                        m_translocatorSprite.Flip = this.Flip;
                        m_translocatorSprite.TextureColor = Color.Black;
                        m_translocatorSprite.Scale = Vector2.Zero;

                        for (int i = 0; i < this.NumChildren; i++)
                        {
                            (m_translocatorSprite.GetChildAt(i) as SpriteObj).ChangeSprite((_objectList[i] as SpriteObj).SpriteName);
                            m_translocatorSprite.GetChildAt(i).Visible = _objectList[i].Visible;
                        }

                        m_translocatorSprite.GetChildAt(PlayerPart.Light).Visible = false;
                        if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                        {
                            m_translocatorSprite.GetChildAt(PlayerPart.Sword1).Visible = false;
                            m_translocatorSprite.GetChildAt(PlayerPart.Sword2).Visible = false;
                        }
                        m_levelScreen.TextManager.DisplayNumberStringText(-manaCost, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.SkyBlue, new Vector2(this.X, this.Bounds.Top));

                        AttachedLevel.ImpactEffectPool.StartInverseEmit(m_translocatorSprite.Position);
                        Tween.To(m_translocatorSprite, 0.4f, Tweener.Ease.Linear.EaseNone, "ScaleX", this.ScaleX.ToString(), "ScaleY", this.ScaleY.ToString());
                        SoundManager.PlaySound("mfqt_teleport_out");
                    }
                    else if (m_translocatorSprite.Visible == true && m_translocatorSprite.Scale == this.Scale)
                    {
                        SoundManager.PlaySound("mfqt_teleport_in");
                        //AttachedLevel.ImpactEffectPool.StartTranslocateEmit(this.Position);
                        this.Translocate(m_translocatorSprite.Position);
                        m_translocatorSprite.Visible = false;
                    }
                    break;
                case (SpellType.RapidDagger):
                    if (this.CurrentMana >= manaCost)
                    {
                        this.CurrentMana -= manaCost;
                        ThrowDaggerProjectiles();
                    }
                    break;
            }

            projData.Dispose();
        }

        public void Translocate(Vector2 position)
        {
            this.DisableAllWeight = true;
            Tween.To(this, 0.08f, Tweener.Ease.Linear.EaseNone, "ScaleX", "3", "ScaleY", "0");
            Tween.To(this, 0, Tweener.Ease.Linear.EaseNone, "delay", "0.1", "X", position.X.ToString(), "Y", position.Y.ToString());
            Tween.AddEndHandlerToLastTween(this, "ResetTranslocution");
            //this.Position = m_translocatorSprite.Position;
            this.AttachedLevel.UpdateCamera();
        }

        // A hack method specifically to make quantum translocator work for Alexander neo boss.
        public void OverrideInternalScale(Vector2 internalScale)
        {
            m_internalScale = internalScale;
        }

        public void ResetTranslocution()
        {
            this.DisableAllWeight = false;
            this.Scale = m_internalScale;
        }

        public void ConvertHPtoMP()
        {
            if (MaxHealth > 1)
            {
                int baseHealthLost = (int)((MaxHealth - Game.PlayerStats.LichHealth) * GameEV.LICH_HEALTH_CONVERSION_PERCENT);
                int lichHealthLost = (int)(Game.PlayerStats.LichHealth * GameEV.LICH_HEALTH_CONVERSION_PERCENT);

                int maxMana = (int)((BaseMana +
                            GetEquipmentMana() + (Game.PlayerStats.BonusMana * GameEV.ITEM_STAT_MAXMP_AMOUNT) +
                            SkillSystem.GetSkill(SkillType.Mana_Up).ModifierAmount +
                            SkillSystem.GetSkill(SkillType.Mana_Up_Final).ModifierAmount) * GameEV.LICH_MAX_MP_OFF_BASE);

                if (MaxMana + baseHealthLost + lichHealthLost < maxMana)
                {
                    SoundManager.PlaySound("Lich_Swap");

                    Game.PlayerStats.LichHealthMod *= GameEV.LICH_HEALTH_CONVERSION_PERCENT;
                    Game.PlayerStats.LichHealth -= lichHealthLost;
                    Game.PlayerStats.LichMana += baseHealthLost + lichHealthLost;
                    this.CurrentMana += baseHealthLost + lichHealthLost;
                    if (CurrentHealth > this.MaxHealth)
                        this.CurrentHealth = this.MaxHealth;
                    m_levelScreen.UpdatePlayerHUD();

                    m_levelScreen.TextManager.DisplayNumberStringText((baseHealthLost + lichHealthLost), "LOC_ID_PLAYER_OBJ_2" /*"max mp"*/, Color.RoyalBlue, new Vector2(this.X, this.Bounds.Top - 30));
                    m_levelScreen.TextManager.DisplayNumberStringText(-(baseHealthLost + lichHealthLost), "LOC_ID_PLAYER_OBJ_3" /*"max hp"*/, Color.Red, new Vector2(this.X, this.Bounds.Top - 60));
                }
                else
                {
                    SoundManager.PlaySound("Error_Spell");
                    m_levelScreen.TextManager.DisplayStringText("LOC_ID_PLAYER_OBJ_4" /*"Max MP Converted. Need higher level."*/, Color.RoyalBlue, new Vector2(this.X, this.Bounds.Top - 30));
                }
            }
        }

        public void ActivateAssassinAbility()
        {
            if (CurrentMana >= GameEV.ASSASSIN_ACTIVE_INITIAL_COST)
            {
                SoundManager.PlaySound("Assassin_Stealth_Enter");
                CurrentMana -= GameEV.ASSASSIN_ACTIVE_INITIAL_COST;
                Tween.To(this, 0.2f, Tween.EaseNone, "Opacity", "0.05");
                m_assassinSpecialActive = true;
                ForceInvincible = true;
                m_levelScreen.ImpactEffectPool.AssassinCastEffect(this.Position);
            }
        }

        public void DisableAssassinAbility()
        {
            //this.Opacity = 1;
            Tween.To(this, 0.2f, Tween.EaseNone, "Opacity", "1");
            m_assassinSpecialActive = false;
            ForceInvincible = false;
        }

        public void SwapSpells()
        {
            //if (AttachedLevel.CurrentRoom is CarnivalShoot1BonusRoom == false && AttachedLevel.CurrentRoom is CarnivalShoot2BonusRoom == false)
            {
                SoundManager.PlaySound("Spell_Switch");
                m_wizardSpellList[0] = (byte)Game.PlayerStats.WizardSpellList.X;
                m_wizardSpellList[1] = (byte)Game.PlayerStats.WizardSpellList.Y;
                m_wizardSpellList[2] = (byte)Game.PlayerStats.WizardSpellList.Z;

                int spellIndex = m_wizardSpellList.IndexOf(Game.PlayerStats.Spell);
                spellIndex++;
                if (spellIndex >= m_wizardSpellList.Count)
                    spellIndex = 0;
                Game.PlayerStats.Spell = m_wizardSpellList[spellIndex];
                m_levelScreen.UpdatePlayerSpellIcon();

                if (m_damageShieldCast == true)
                {
                    m_damageShieldCast = false;
                    m_megaDamageShieldCast = false;
                }

                if (m_timeStopCast == true)
                {
                    AttachedLevel.StopTimeStop();
                    m_timeStopCast = false;
                }
            }
        }

        public void NinjaTeleport()
        {
            if (this.CurrentMana >= GameEV.NINJA_TELEPORT_COST && m_ninjaTeleportDelay <= 0)
            {
                this.CurrentMana -= GameEV.NINJA_TELEPORT_COST;
                m_ninjaTeleportDelay = 0.5f;

                m_levelScreen.ImpactEffectPool.NinjaDisappearEffect(this);

                int teleportDistance = GameEV.NINJA_TELEPORT_DISTANCE;
                int distance = int.MaxValue;
                TerrainObj closestWall = CalculateClosestWall(out distance);

                if (closestWall != null)
                {
                    if (distance < teleportDistance)
                    {
                        //Tween.To(closestWall, 10, Tween.EaseNone, "Y", "-1000");
                        if (this.Flip == SpriteEffects.None)
                        {
                            if (closestWall.Rotation == 0)
                                this.X += distance - this.TerrainBounds.Width / 2f;
                            else
                                this.X += distance - this.Width / 2f;
                        }
                        else
                        {
                            if (closestWall.Rotation == 0)
                                this.X -= distance - this.TerrainBounds.Width / 2f;
                            else
                                this.X -= distance - this.Width / 2f;
                        }
                    }
                    else
                    {
                        if (this.Flip == SpriteEffects.FlipHorizontally)
                            this.X -= teleportDistance;
                        else
                            this.X += teleportDistance;
                    }
                }
                else
                {
                    if (this.Flip == SpriteEffects.FlipHorizontally)
                        this.X -= teleportDistance;
                    else
                        this.X += teleportDistance;

                    if (this.X > m_levelScreen.CurrentRoom.Bounds.Right)
                        this.X = m_levelScreen.CurrentRoom.Bounds.Right - 5;
                    else if (this.X < m_levelScreen.CurrentRoom.X)
                        this.X = m_levelScreen.CurrentRoom.X + 5;
                }
                SoundManager.PlaySound("Ninja_Teleport");
                m_levelScreen.ImpactEffectPool.NinjaAppearEffect(this);
                m_levelScreen.UpdateCamera();
            }
        }

        public TerrainObj CalculateClosestWall(out int dist)
        {
            int closestDistance = int.MaxValue;
            TerrainObj closestObj = null;

            Vector2 collisionPt = Vector2.Zero;
            RoomObj room = m_levelScreen.CurrentRoom;

            foreach (TerrainObj terrain in room.TerrainObjList)
            {
                if (terrain.CollidesBottom == true || terrain.CollidesLeft == true || terrain.CollidesRight == true)
                {
                    collisionPt = Vector2.Zero;
                    // Only collide with terrain that the displacer would collide with.
                    float distance = float.MaxValue;
                    if (this.Flip == SpriteEffects.None)
                    {
                        if (terrain.X > this.X && (terrain.Bounds.Top + 5 < this.TerrainBounds.Bottom && terrain.Bounds.Bottom > this.TerrainBounds.Top))
                        {
                            //if (terrain.Rotation == -45)
                            if (terrain.Rotation < 0) // ROTCHECK
                                collisionPt = CollisionMath.LineToLineIntersect(this.Position, new Vector2(this.X + 6600, this.Y),
                                        CollisionMath.UpperLeftCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero), CollisionMath.UpperRightCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero));
                            //else if (terrain.Rotation == 45)
                            else if (terrain.Rotation > 0) // ROTCHECK
                                collisionPt = CollisionMath.LineToLineIntersect(this.Position, new Vector2(this.X + 6600, this.Y),
                                        CollisionMath.LowerLeftCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero), CollisionMath.UpperLeftCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero));

                            if (collisionPt != Vector2.Zero)
                                distance = collisionPt.X - this.X;
                            else
                                distance = terrain.Bounds.Left - this.TerrainBounds.Right;
                        }
                    }
                    else
                    {
                        if (terrain.X < this.X && (terrain.Bounds.Top + 5 < this.TerrainBounds.Bottom && terrain.Bounds.Bottom > this.TerrainBounds.Top))
                        {
                            //if (terrain.Rotation == -45)
                            if (terrain.Rotation < 0) // ROTCHECK
                                collisionPt = CollisionMath.LineToLineIntersect(new Vector2(this.X - 6600, this.Y), this.Position,
                                        CollisionMath.UpperRightCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero), CollisionMath.LowerRightCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero));
                            //else if (terrain.Rotation == 45)
                            else if (terrain.Rotation > 0) // ROTCHECK
                                collisionPt = CollisionMath.LineToLineIntersect(new Vector2(this.X - 6600, this.Y), this.Position,
                                        CollisionMath.UpperLeftCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero), CollisionMath.UpperRightCorner(terrain.NonRotatedBounds, terrain.Rotation, Vector2.Zero));

                            if (collisionPt != Vector2.Zero)
                                distance = this.X - collisionPt.X;
                            else
                                distance = this.TerrainBounds.Left - terrain.Bounds.Right;
                        }
                    }

                    if (distance < closestDistance)
                    {
                        closestDistance = (int)distance;
                        closestObj = terrain;
                    }
                }
            }

            dist = closestDistance;
            return closestObj;
        }

        public void ActivateTanooki()
        {
            if (CurrentMana >= GameEV.TANOOKI_ACTIVE_INITIAL_COST)
            {
                CurrentMana -= GameEV.TANOOKI_ACTIVE_INITIAL_COST;
                m_levelScreen.ImpactEffectPool.DisplayTanookiEffect(this);
                this.TextureColor = Color.White;
                _objectList[0].TextureColor = Color.White;
                this.State = STATE_TANOOKI;
            }
        }

        public void DeactivateTanooki()
        {
            m_levelScreen.ImpactEffectPool.DisplayTanookiEffect(this);
            this.State = STATE_IDLE;
        }

        public void CastFuhRohDah()
        {
            if (CurrentMana >= GameEV.BARBARIAN_SHOUT_INITIAL_COST)
            {
                CurrentMana -= GameEV.BARBARIAN_SHOUT_INITIAL_COST;
                ProjectileData spellData = new ProjectileData(this)
                {
                    SpriteName = "SpellShout_Sprite",
                    IsWeighted = false,
                    Lifespan = 2,
                    CollidesWith1Ways = false,
                    CollidesWithTerrain = false,
                    DestroysWithEnemy = false,
                    DestroysWithTerrain = false,
                    Damage = 0,
                    Scale = Vector2.One,
                };

                ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(spellData);
                proj.Opacity = 0;
                proj.CollisionTypeTag = GameTypes.CollisionType_PLAYER;
                proj.Spell = SpellType.Shout;
                proj.IgnoreBoundsCheck = true;

                Tween.To(proj, 0.2f, Tween.EaseNone, "ScaleX", "100", "ScaleY", "50");
                Tween.AddEndHandlerToLastTween(proj, "KillProjectile");
                SoundManager.PlaySound("Cast_FasRoDus");

                m_levelScreen.ImpactEffectPool.DisplayFusRoDahText(new Vector2(this.X, this.Bounds.Top));
                m_levelScreen.ShoutMagnitude = 0;
                Tween.To(m_levelScreen, 1, Tween.EaseNone, "ShoutMagnitude", "3");
            }
        }

        private void CastCloseShield()
        {
            ProjectileData projData = new ProjectileData(this)
            {
                SpriteName = "SpellClose_Sprite",
                //Angle = new Vector2(90, 90),
                //SourceAnchor = new Vector2(120, -60),//(75,-200),//(50, 0),
                Speed = new Vector2(0, 0),//(450,450),//(1000, 1000),
                IsWeighted = false,
                RotationSpeed = 0f,
                DestroysWithEnemy = false,
                DestroysWithTerrain = false,
                CollidesWithTerrain = false,
                Scale = new Vector2(m_Spell_Close_Scale, m_Spell_Close_Scale),
                Damage = Damage,
                Lifespan = m_Spell_Close_Lifespan,
                LockPosition = true,
            };

            projData.Damage = (int)(TotalMagicDamage * PlayerEV.TRAITOR_CLOSE_DAMAGEMOD);
            
            SoundManager.PlaySound("Cast_GiantSword");
            m_levelScreen.ImpactEffectPool.LastBossSpellCastEffect(this, 90, true);
            ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(projData);
            proj.TextureColor = Color.CadetBlue;

            projData.Dispose();
        }

        private void ThrowAxeProjectiles()
        {
            m_axeProjData.AngleOffset = 0;
            m_axeProjData.Damage = (int)(TotalMagicDamage * PlayerEV.TRAITOR_AXE_DAMAGEMOD);

            Tween.RunFunction(0, this, "CastAxe");
            Tween.RunFunction(0.15f, this, "CastAxe");
            Tween.RunFunction(0.3f, this, "CastAxe");
            Tween.RunFunction(0.45f, this, "CastAxe");
            Tween.RunFunction(0.6f, this, "CastAxe");
        }

        public void CastAxe()
        {
            m_axeProjData.AngleOffset = CDGMath.RandomInt(-70, 70);
            ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(m_axeProjData);
            proj.TextureColor = Color.CadetBlue;
            SoundManager.PlaySound("Cast_Axe");
            m_levelScreen.ImpactEffectPool.LastBossSpellCastEffect(this, 45, true);
        }

        private void ThrowDaggerProjectiles()
        {
            m_rapidDaggerProjData.AngleOffset = 0;
            m_rapidDaggerProjData.Damage = (int)(TotalMagicDamage * SpellEV.GetDamageMultiplier(SpellType.RapidDagger));

            Tween.RunFunction(0, this, "CastDaggers", false);
            Tween.RunFunction(0.05f, this, "CastDaggers", true);
            Tween.RunFunction(0.1f, this, "CastDaggers", true);
            Tween.RunFunction(0.15f, this, "CastDaggers", true);
            Tween.RunFunction(0.2f, this, "CastDaggers", true);
        }

        public void CastDaggers(bool randomize)
        {
            if (randomize == true)
                m_rapidDaggerProjData.AngleOffset = CDGMath.RandomInt(-8, 8);
            ProjectileObj proj = m_levelScreen.ProjectileManager.FireProjectile(m_rapidDaggerProjData);
            proj.TextureColor = Color.CadetBlue;
            SoundManager.PlaySound("Cast_Dagger");
            m_levelScreen.ImpactEffectPool.LastBossSpellCastEffect(this, 0, true);
        }

        public void StopAllSpells()
        {
            if (State == STATE_TANOOKI)
                DeactivateTanooki();

            if (m_damageShieldCast == true)
            {
                m_damageShieldCast = false;
                m_megaDamageShieldCast = false;
            }

            if (m_timeStopCast == true)
            {
                AttachedLevel.StopTimeStop();
                m_timeStopCast = false;
            }
            
            if (m_assassinSpecialActive == true)
                DisableAssassinAbility();

            m_lightOn = false;
            m_translocatorSprite.Visible = false;

            if (State == STATE_DRAGON)
            {
                State = STATE_JUMPING;
                this.DisableGravity = false;
                m_isFlying = false;
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                if (m_currentLogicSet.IsActive)
                    m_currentLogicSet.Stop();
                m_currentLogicSet = null;

                m_standingAttack3LogicSet.Dispose();
                m_standingAttack3LogicSet = null;
                m_airAttackLS.Dispose();
                m_airAttackLS = null;

                m_debugInputMap.Dispose();
                m_debugInputMap = null;

                m_playerHead = null;
                m_playerLegs = null;

                m_walkDownSound.Dispose();
                m_walkDownSound = null;
                m_walkUpSound.Dispose();
                m_walkUpSound = null;

                m_walkDownSoundLow.Dispose();
                m_walkDownSoundLow = null;
                m_walkUpSoundLow.Dispose();
                m_walkUpSoundLow = null;

                m_walkDownSoundHigh.Dispose();
                m_walkDownSoundHigh = null;
                m_walkUpSoundHigh.Dispose();
                m_walkUpSoundHigh = null;

                if (m_externalLS != null)
                    m_externalLS.Dispose();
                m_externalLS = null;

                m_lastTouchedTeleporter = null;

                m_flightDurationText.Dispose();
                m_flightDurationText = null;

                m_game = null;
                
                m_translocatorSprite.Dispose();
                m_translocatorSprite = null;

                m_swearBubble.Dispose();
                m_swearBubble = null;

                m_flipTween = null;

                m_wizardSpellList.Clear();
                m_wizardSpellList = null;

                m_closestGround = null;

                m_rapidDaggerProjData.Dispose();
                m_axeProjData.Dispose();

                base.Dispose();
            }
        }

        public override void Draw(Camera2D camera)
        {
            m_swearBubble.Scale = new Vector2(this.ScaleX * 1.2f, this.ScaleY * 1.2f);
            m_swearBubble.Position = new Vector2(this.X - (30 * this.ScaleX), this.Y - (35 * this.ScaleX));
            m_swearBubble.Draw(camera);

            m_translocatorSprite.Draw(camera);
            base.Draw(camera);

            if (IsFlying == true && State != STATE_DRAGON)
            {
                m_flightDurationText.Text = String.Format("{0:F1}", m_flightCounter);
                m_flightDurationText.Position = new Vector2(this.X, this.TerrainBounds.Top - 70);
                m_flightDurationText.Draw(camera);
            }

            //if (this.Opacity == 1)
            {
                camera.End();
                // This is to maintain the player's helmet tint.
                Game.ColourSwapShader.Parameters["desiredTint"].SetValue(m_playerHead.TextureColor.ToVector4());
                // This is the Tint Removal effect, that removes the tint from his face.
                if (Game.PlayerStats.Class == ClassType.Lich || Game.PlayerStats.Class == ClassType.Lich2)
                {
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(this.Opacity);
                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(m_lichColour1.ToVector4());

                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(m_lichColour2.ToVector4());
                }
                else if (Game.PlayerStats.Class == ClassType.Assassin || Game.PlayerStats.Class == ClassType.Assassin2)
                {
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(this.Opacity);
                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(Color.Black.ToVector4());

                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(Color.Black.ToVector4());
                }
                else
                {
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(this.Opacity);
                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(m_skinColour1.ToVector4());

                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(m_skinColour2.ToVector4());
                }

                // All camera calls changed to deferred mode.
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game.ColourSwapShader, camera.GetTransformation());
                m_playerHead.Draw(camera);
                camera.End();

                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetTransformation());
                if (Game.PlayerStats.IsFemale == true)
                    _objectList[PlayerPart.Bowtie].Draw(camera);
                _objectList[PlayerPart.Glasses].Draw(camera);
                _objectList[PlayerPart.Extra].Draw(camera);
            }
        }

        public void FadeSword()
        {
            //SoundManager.PlaySound("Player_Attack_Sword_Spell_01", "Player_Attack_Sword_Spell_02", "Player_Attack_Sword_Spell_03");
            SpriteObj sword = _objectList[PlayerPart.Sword1] as SpriteObj;
            SpriteObj sword2 = _objectList[PlayerPart.Sword2] as SpriteObj;

            Tween.StopAllContaining(sword, false);
            Tween.StopAllContaining(sword2, false);

            sword.Opacity = 0f;
            sword2.Opacity = 0f;

            sword.TextureColor = Color.White;
            Tween.To(sword, 0.1f, Tween.EaseNone, "Opacity", "1");
            sword.Opacity = 1;
            Tween.To(sword, 0.1f, Tween.EaseNone, "delay", "0.2", "Opacity", "0");
            sword.Opacity = 0f;

            sword2.TextureColor = Color.White;
            Tween.To(sword2, 0.1f, Tween.EaseNone, "Opacity", "1");
            sword2.Opacity = 1;
            Tween.To(sword2, 0.1f, Tween.EaseNone, "delay", "0.2", "Opacity", "0");
            sword2.Opacity = 0f;
        }

        public void ChangePartColour(int playerPart, Color colour)
        {
            if (playerPart == PlayerPart.Cape || playerPart == PlayerPart.Neck)
            {
                GetPlayerPart(PlayerPart.Neck).TextureColor = colour;
                GetPlayerPart(PlayerPart.Cape).TextureColor = colour;
            }
            else
                GetPlayerPart(playerPart).TextureColor = colour;
        }

        public SpriteObj GetPlayerPart(int playerPart)
        {
            return _objectList[playerPart] as SpriteObj;
        }

        private Rectangle GroundCollisionRect
        {
            get { return new Rectangle((int)this.Bounds.X, (int)this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + StepUp); }
        }

        private Rectangle RotatedGroundCollisionRect
        {
            get { return new Rectangle((int)this.Bounds.X, (int)this.Bounds.Y, this.Bounds.Width, this.Bounds.Height + 40); }
        }

        public void AttachLevel(ProceduralLevelScreen level)
        {
            m_levelScreen = level;
        }

        public PlayerIndex PlayerIndex
        {
            get { return m_playerIndex; }
        }

        public ProceduralLevelScreen AttachedLevel
        {
            get { return m_levelScreen; }
        }

        public int Damage
        {
            get
            {
                int damageDealt = 0;
                damageDealt = (int)((RandomDamage + SkillSystem.GetSkill(SkillType.Attack_Up).ModifierAmount + SkillSystem.GetSkill(SkillType.Damage_Up_Final).ModifierAmount) * ClassDamageGivenMultiplier);
                //+ Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.Attack_Damage_Flat)) * ClassDamageGivenMultiplier);
                if (IsAirAttacking)
                    damageDealt = (int)(damageDealt * TotalAirAttackDamageMod);

                if (damageDealt < 1) damageDealt = 1;

                return damageDealt;
            }
        }

        public float TotalAirAttackDamageMod
        {
            get
            {
                float skillModAmount = SkillSystem.GetSkill(SkillType.Down_Strike_Up).ModifierAmount * NumAirBounces;
                float airAttackMod = AirAttackDamageMod + skillModAmount;
                if (airAttackMod > 1)
                    airAttackMod = 1;
                return airAttackMod;
            }
        }

        public int TotalMagicDamage
        {
            get 
            { 
                int intelligence = (int)((BaseMagicDamage + SkillSystem.GetSkill(SkillType.Magic_Damage_Up).ModifierAmount + GetEquipmentMagicDamage() + (Game.PlayerStats.BonusMagic * GameEV.ITEM_STAT_MAGIC_AMOUNT)) * ClassMagicDamageGivenMultiplier);
                if (intelligence < 1)
                    intelligence = 1;
                return intelligence;
            }
        }

        public int InvulnDamage
        {
            get
            {
                return (int)(RandomDamage * (1 + SkillSystem.GetSkill(SkillType.Invuln_Attack_Up).ModifierAmount) + SkillSystem.GetSkill(SkillType.Attack_Up).ModifierAmount +
                            SkillSystem.GetSkill(SkillType.Damage_Up_Final).ModifierAmount);
            }
        }

        public Vector2 EnemyKnockBack
        {
            get
            {
                if (m_currentLogicSet == m_standingAttack3LogicSet)
                    return StrongEnemyKnockBack;
                return m_enemyKnockBack;
            }
            set { m_enemyKnockBack = value; }
        }


        public int RandomDamage
        {
            get
            {
                return CDGMath.RandomInt(MinDamage + GetEquipmentDamage(), MaxDamage + GetEquipmentDamage()) +
                    (Game.PlayerStats.BonusStrength * GameEV.ITEM_STAT_STRENGTH_AMOUNT) +
                    (DamageGainPerLevel * Game.PlayerStats.CurrentLevel);
            }
        }

        public float MaxMana
        {
            get
            {
                // if dextrocardic, return Max health instead of max mana.
                if (Game.PlayerStats.Traits.X == TraitType.Dextrocardia || Game.PlayerStats.Traits.Y == TraitType.Dextrocardia)
                {
                    int maxMana = (int)Math.Round(((BaseHealth + GetEquipmentHealth() +
                   (HealthGainPerLevel * Game.PlayerStats.CurrentLevel) +
                     (Game.PlayerStats.BonusHealth * GameEV.ITEM_STAT_MAXHP_AMOUNT) +
                      SkillSystem.GetSkill(SkillType.Health_Up).ModifierAmount +
                         SkillSystem.GetSkill(SkillType.Health_Up_Final).ModifierAmount) * ClassTotalHPMultiplier * Game.PlayerStats.LichHealthMod), MidpointRounding.AwayFromZero) + Game.PlayerStats.LichHealth; // Lich health is separate from modifiers.

                    if (maxMana < 1)
                        maxMana = 1;
                    return maxMana;
                }
                else
                {
                    int maxMana = (int)((BaseMana +
                        GetEquipmentMana() +
                        (ManaGainPerLevel * Game.PlayerStats.CurrentLevel) +
                        (Game.PlayerStats.BonusMana * GameEV.ITEM_STAT_MAXMP_AMOUNT) +
                        SkillSystem.GetSkill(SkillType.Mana_Up).ModifierAmount +
                        SkillSystem.GetSkill(SkillType.Mana_Up_Final).ModifierAmount) * ClassTotalMPMultiplier) + Game.PlayerStats.LichMana; // Lich mana is separate from modifiers.

                    if (maxMana < 1)
                        maxMana = 1;
                    return maxMana;
                }
            }
        }

        public override int MaxHealth
        {
            get
            {
                // if dextrocardic, return max mana instead of max health.
                if (Game.PlayerStats.Traits.X == TraitType.Dextrocardia || Game.PlayerStats.Traits.Y == TraitType.Dextrocardia)
                {
                    int maxHealth = (int)((BaseMana +
                        GetEquipmentMana() +
                        (ManaGainPerLevel * Game.PlayerStats.CurrentLevel) +
                        (Game.PlayerStats.BonusMana * GameEV.ITEM_STAT_MAXMP_AMOUNT) +
                        SkillSystem.GetSkill(SkillType.Mana_Up).ModifierAmount +
                        SkillSystem.GetSkill(SkillType.Mana_Up_Final).ModifierAmount) * ClassTotalMPMultiplier) + Game.PlayerStats.LichMana; // Lich mana is separate from modifiers.

                    if (maxHealth < 1)
                        maxHealth = 1;
                    return maxHealth;
                }
                else
                {
                    int maxHealth = (int)Math.Round(((BaseHealth + GetEquipmentHealth() +
                        (HealthGainPerLevel * Game.PlayerStats.CurrentLevel) +
                        (Game.PlayerStats.BonusHealth * GameEV.ITEM_STAT_MAXHP_AMOUNT) +
                        SkillSystem.GetSkill(SkillType.Health_Up).ModifierAmount +
                        SkillSystem.GetSkill(SkillType.Health_Up_Final).ModifierAmount) * ClassTotalHPMultiplier * Game.PlayerStats.LichHealthMod), MidpointRounding.AwayFromZero) + Game.PlayerStats.LichHealth; // Lich health is separate from modifiers.

                    if (maxHealth < 1)
                        maxHealth = 1;
                    return maxHealth;
                }
            }
        }

        public float InvincibilityTime
        {
            get
            {
                return BaseInvincibilityTime + SkillSystem.GetSkill(SkillType.Invuln_Time_Up).ModifierAmount;
                // + Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.Invuln_Flat_Up);
            }
        }

        public override Rectangle Bounds
        {
            get
            {
                //return new Rectangle((int)(this.X - 37), (int)(this.Y - 56), 73, 93);
                //return new Rectangle((int)((this.X - 37 * ScaleX)), (int)((this.Y - 56 * ScaleY)), (int)(73 * ScaleX),(int)(93 * ScaleY));
                return this.TerrainBounds;
            }
        }

        public int GetEquipmentDamage()
        {
            int bonusDamage = 0;

            int equipmentCategoryIndex = 0;
            foreach (sbyte equippedItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equippedItemIndex > -1)
                {
                    bonusDamage += Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equippedItemIndex).BonusDamage;
                }
                equipmentCategoryIndex++;
            }
            return bonusDamage;
        }

        public int GetEquipmentMana()
        {
            int bonusMana = 0;

            int equipmentCategoryIndex = 0;
            foreach (sbyte equippedItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equippedItemIndex > -1)
                {
                    bonusMana += Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equippedItemIndex).BonusMana;
                }
                equipmentCategoryIndex++;
            }
            return bonusMana;
        }

        public int GetEquipmentHealth()
        {
            int bonushealth = 0;

            int equipmentCategoryIndex = 0;
            foreach (sbyte equippedItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equippedItemIndex > -1)
                {
                    bonushealth += Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equippedItemIndex).BonusHealth;
                }
                equipmentCategoryIndex++;
            }
            return bonushealth;
        }

        public int GetEquipmentArmor()
        {
            int bonusArmor = 0;

            int equipmentCategoryIndex = 0;
            foreach (sbyte equippedItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equippedItemIndex > -1)
                {
                    bonusArmor += Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equippedItemIndex).BonusArmor;
                }
                equipmentCategoryIndex++;
            }
            return bonusArmor;
        }

        public int GetEquipmentWeight()
        {
            int totalWeight = 0;

            int equipmentCategoryIndex = 0;
            foreach (sbyte equippedItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equippedItemIndex > -1)
                {
                    totalWeight += Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equippedItemIndex).Weight;
                }
                equipmentCategoryIndex++;
            }
            return totalWeight;
        }

        public int GetEquipmentMagicDamage()
        {
            int totalMagic = 0;

            int equipmentCategoryIndex = 0;
            foreach (sbyte equippedItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equippedItemIndex > -1)
                {
                    totalMagic += Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equippedItemIndex).BonusMagic;
                }
                equipmentCategoryIndex++;
            }
            return totalMagic;
        }

        public float GetEquipmentSecondaryAttrib(int secondaryAttribType)
        {
            float value = 0;
            int equipmentCategoryIndex = 0;

            foreach (sbyte equipmentItemIndex in Game.PlayerStats.GetEquippedArray)
            {
                if (equipmentItemIndex > -1)
                {
                    EquipmentData data = Game.EquipmentSystem.GetEquipmentData(equipmentCategoryIndex, equipmentItemIndex);
                    foreach (Vector2 secondaryAttrib in data.SecondaryAttribute)
                    {
                        if ((int)secondaryAttrib.X == secondaryAttribType)
                            value += secondaryAttrib.Y;
                    }
                }
                equipmentCategoryIndex++;
            }
            return value;
        }

        public int CurrentWeight
        {
            get { return GetEquipmentWeight(); }
        }

        public int MaxWeight
        {
            get
            {
                return (int)(BaseWeight + SkillSystem.GetSkill(SkillType.Equip_Up).ModifierAmount + SkillSystem.GetSkill(SkillType.Equip_Up_Final).ModifierAmount)
                  + (Game.PlayerStats.BonusWeight * GameEV.ITEM_STAT_WEIGHT_AMOUNT);
            }
        }

        public bool CanFly
        {
            get 
            {
                if (Game.PlayerStats.Class == ClassType.Dragon)
                    return true;
                return TotalFlightTime > 0; 
            }
        }

        public bool CanAirDash
        {
            get { return TotalAirDashes > 0; }
        }

        public bool CanBlock
        {
            get
            {
                if (Game.PlayerStats.Class == ClassType.Knight2)
                    return true;
                return false;
            }
        }

        public bool CanRun
        {
            get
            {
                return true;
                //if (LevelEV.UNLOCK_ALL_TRAIT_ABILITIES == true) return true;
                //return SkillSystem.GetSkill(SkillType.Run).ModifierAmount > 0; 
            }
        }

        public bool CanAirAttackDownward
        {
            get
            {
                return true;
            }
        }

        public bool IsJumping
        {
            get { return m_isJumping; }
        }

        public byte NumSequentialAttacks // Used to calculate the number of attacks the player has made in succession before giving him more mana.
        {
            get { return m_numSequentialAttacks; }
            set
            {
                m_numSequentialAttacks = value;
            //    if (m_numSequentialAttacks >= AttacksNeededForMana)
            //    {
            //        m_numSequentialAttacks = 0;
            //        if (ManaGain > 0)
            //        {
            //            CurrentMana += ManaGain;
            //            //m_levelScreen.TextManager.DisplayManaText((int)ManaGain, this.Position);
            //            m_levelScreen.TextManager.DisplayNumberStringText((int)ManaGain, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.RoyalBlue, new Vector2(this.X, this.Bounds.Top - 30));
            //        }
            //    }
            }
        }

        public byte AttacksNeededForMana
        {
            get { return m_attacksNeededForMana; }
            set { m_attacksNeededForMana = value; }
        }

        public float ManaGain
        {
            get 
            {
                //int classManaGain = 0;
                //if (Game.PlayerStats.Class == ClassType.Wizard || Game.PlayerStats.Class == ClassType.Wizard2)
                //    classManaGain = GameEV.MAGE_MANA_GAIN;
                //return m_manaGain + classManaGain + SkillSystem.GetSkill(SkillType.Mana_Regen_Up).ModifierAmount + ((Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaGain) + (int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.ManaDrain)) * GameEV.RUNE_MANA_GAIN)
                //     + (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaHPGain) * GameEV.RUNE_MANAHPGAIN); 

                int classManaGain = 0;
                if (Game.PlayerStats.Class == ClassType.Wizard || Game.PlayerStats.Class == ClassType.Wizard2)
                    classManaGain = GameEV.MAGE_MANA_GAIN;
                return (int)((m_manaGain + classManaGain + SkillSystem.GetSkill(SkillType.Mana_Regen_Up).ModifierAmount + ((Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaGain) + (int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.ManaDrain)) * GameEV.RUNE_MANA_GAIN)
                     + (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaHPGain) * GameEV.RUNE_MANAHPGAIN)) * (1 + Game.PlayerStats.TimesCastleBeaten * 0.5f)); 
                
            } //TEDDY MODDING SO MANA GAIN IS AN EV
            set { m_manaGain = value; }
        }

        public float BlockManaDrain
        {
            get { return m_blockManaDrain - (int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.ManaDrain) - (int)SkillSystem.GetSkill(SkillType.Block).ModifierAmount; }
            set { m_blockManaDrain = value; }
        }

        public float TotalStatDropChance
        {
            get { return StatDropIncrease + BaseStatDropChance; }
        }

        public float TotalCritChance
        {
            get
            {
                float CritChanceBonus = BaseCriticalChance + SkillSystem.GetSkill(SkillType.Crit_Chance_Up).ModifierAmount + GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.CritChance);
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Assassin):
                    case (ClassType.Assassin2):
                         return (CritChanceBonus + PlayerEV.ASSASSIN_CRITCHANCE_MOD); 
                    case (ClassType.Ninja):
                    case (ClassType.Ninja2):
                        return 0; // Ninjas have a 0% chance of critical striking.
                }
                return CritChanceBonus;
            }
        }

        public float TotalCriticalDamage
        {
            get
            {
                //return BaseCriticalDamageMod + SkillSystem.GetSkill(SkillType.Crit_Damage_Up).ModifierAmount + GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.CritDamage);
                // +Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.Crit_Damage_Flat);
                float CritDamageBonus = BaseCriticalDamageMod + SkillSystem.GetSkill(SkillType.Crit_Damage_Up).ModifierAmount + GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.CritDamage);

                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Assassin):
                    case (ClassType.Assassin2):
                        return (CritDamageBonus + PlayerEV.ASSASSIN_CRITDAMAGE_MOD);
                }
                return CritDamageBonus;
            }
        }

        public float TotalXPBonus
        {
            get
            {
                return SkillSystem.GetSkill(SkillType.XP_Gain_Up).ModifierAmount + GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.XpBonus);
                    //+ Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.XP_Percentage);
            }
        }

        public float TotalGoldBonus
        {
            get
            {
                float goldBonus = SkillSystem.GetSkill(SkillType.Gold_Gain_Up).ModifierAmount + GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.GoldBonus)
                    + (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.GoldGain) * GameEV.RUNE_GOLDGAIN_MOD) + (GameEV.NEWGAMEPLUS_GOLDBOUNTY * Game.PlayerStats.TimesCastleBeaten);
                   // + Game.TraitSystem.GetModifierAmount(m_traitArray, TraitType.Gold_Percentage);

                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Banker):
                    case (ClassType.Banker2):
                        return (goldBonus + PlayerEV.BANKER_GOLDGAIN_MOD);
                }
                return goldBonus;
            }
        }

        public int TotalVampBonus
        {
            //get { return (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.Vampirism) * 
            //    GameEV.RUNE_VAMPIRISM_HEALTH_GAIN) + 
            //    ((int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.Vampirism) * GameEV.RUNE_VAMPIRISM_HEALTH_GAIN)
            //    + (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaHPGain) * GameEV.RUNE_MANAHPGAIN); }

            get
            {
                return (int)(((Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.Vampirism) *
                    GameEV.RUNE_VAMPIRISM_HEALTH_GAIN) +
                    ((int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.Vampirism) * GameEV.RUNE_VAMPIRISM_HEALTH_GAIN)
                    + (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaHPGain) * GameEV.RUNE_MANAHPGAIN)) * (1 + Game.PlayerStats.TimesCastleBeaten * 0.5f));
            }
        }

        public int TotalAirDashes
        {
            get { return Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.Dash) + (int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.AirDash); }
        }

        public int TotalDoubleJumps
        {
            get { return Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.DoubleJump) + (int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.DoubleJump); }
        }

        public float TotalFlightTime
        {
            get { return FlightTime * (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.Flight) + (int)GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.Flight)); }
        }

        public float TotalArmor
        {
            get { return (SkillSystem.GetSkill(SkillType.Armor_Up).ModifierAmount + (Game.PlayerStats.BonusDefense * GameEV.ITEM_STAT_ARMOR_AMOUNT)) + GetEquipmentArmor(); }
        }

        public float TotalDamageReduc
        {
            get { return TotalArmor / (ArmorReductionMod + TotalArmor); }
        }

        public float TotalMovementSpeed
        {
            get 
            {
                float flightSpeedMod = 0;
                if (State == STATE_FLYING || State == STATE_DRAGON)
                    flightSpeedMod = FlightSpeedMod;
                float moveSpeed = this.Speed * (TotalMovementSpeedPercent + flightSpeedMod);
                //Console.WriteLine("This speed: " + this.Speed + "  speed percent:" + TotalMovementSpeedPercent + "  flightspeedMod: " + flightSpeedMod + "   total speed: " + moveSpeed);

                return moveSpeed;
            }
        }

        public float TotalMovementSpeedPercent
        {
            get
            {
                float traitMod = 0;
                if (Game.PlayerStats.Traits.X == TraitType.Hyperactive || Game.PlayerStats.Traits.Y == TraitType.Hyperactive)
                    traitMod = GameEV.TRAIT_MOVESPEED_AMOUNT;

                //float moveSpeed = (this.Speed * flightSpeedMod * 
                //    (1 + (GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.MoveSpeed) + traitMod + 
                //    (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.MovementSpeed) * GameEV.RUNE_MOVEMENTSPEED_MOD))) * 
                //    ClassMoveSpeedMultiplier);
                float moveSpeed = (1 + GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.MoveSpeed) + traitMod +
                  (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.MovementSpeed) * GameEV.RUNE_MOVEMENTSPEED_MOD) + ClassMoveSpeedMultiplier);
                return moveSpeed;
            }
        }

        public float TotalDamageReturn
        {
            get
            {
                return (GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.DamageReturn)
                    + (Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.DamageReturn) * GameEV.RUNE_DAMAGERETURN_MOD));
            }
        } 

        public bool ControlsLocked
        {
            get { return m_lockControls; }
        }

        public bool IsFlying
        {
            get { return m_isFlying; }
        }

        public Game Game
        {
            get { return m_game; }
        }

        public bool IsInvincible
        {
            get { return m_invincibleCounter > 0; }
        }

        public float ClassDamageGivenMultiplier
        {
            get
            {
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Barbarian):
                    case (ClassType.Barbarian2):
                        return PlayerEV.BARBARIAN_DAMAGE_MOD;
                    case (ClassType.Wizard):
                    case (ClassType.Wizard2):
                        return PlayerEV.MAGE_DAMAGE_MOD;
                    case (ClassType.SpellSword):
                    case (ClassType.SpellSword2):
                        return PlayerEV.SPELLSWORD_DAMAGE_MOD;//0.25f;//0.2f; //0.25f;
                    case (ClassType.Banker):
                    case (ClassType.Banker2):
                        return PlayerEV.BANKER_DAMAGE_MOD;
                    case (ClassType.Ninja):
                    case (ClassType.Ninja2):
                        return PlayerEV.NINJA_DAMAGE_MOD;//1.75f; //1.5f;
                    case (ClassType.Assassin):
                    case (ClassType.Assassin2):
                        return PlayerEV.ASSASSIN_DAMAGE_MOD;
                    case (ClassType.Lich):
                    case (ClassType.Lich2):
                        return PlayerEV.LICH_DAMAGE_MOD;

                }
                return 1f;
            }
        }

        public float ClassDamageTakenMultiplier
        {
            get
            {
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Assassin):
                    case (ClassType.Assassin2):
                        return 1f;
                }
                return 1f;
            }
        }

        public float ClassMagicDamageGivenMultiplier
        {
            get
            {
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Wizard):
                    case (ClassType.Wizard2):
                        return PlayerEV.MAGE_MAGICDAMAGE_MOD; //1.25f;
                    case (ClassType.Lich):
                    case (ClassType.Lich2):
                        return PlayerEV.LICH_MAGICDAMAGE_MOD; //1.25f;
                }
                return 1f;
            }
        }

        public float ClassTotalHPMultiplier
        {
            get
            {
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Wizard):
                    case (ClassType.Wizard2):
                        return PlayerEV.MAGE_HEALTH_MOD; //1.25f;
                    case (ClassType.Banker):
                    case (ClassType.Banker2):
                        return PlayerEV.BANKER_HEALTH_MOD;
                    case (ClassType.Barbarian):
                    case (ClassType.Barbarian2):
                        return PlayerEV.BARBARIAN_HEALTH_MOD;
                    case (ClassType.Ninja):
                    case (ClassType.Ninja2):
                        return PlayerEV.NINJA_HEALTH_MOD;
                    case (ClassType.Assassin):
                    case (ClassType.Assassin2):
                        return PlayerEV.ASSASSIN_HEALTH_MOD;
                    case (ClassType.Lich):
                    case (ClassType.Lich2):
                        return PlayerEV.LICH_HEALTH_MOD;
                    case (ClassType.SpellSword):
                    case (ClassType.SpellSword2):
                        return PlayerEV.SPELLSWORD_HEALTH_MOD;//0.5f;
                    case (ClassType.Dragon):
                        return PlayerEV.DRAGON_HEALTH_MOD;//1.5f;    
                    case (ClassType.Traitor):
                        return PlayerEV.TRAITOR_HEALTH_MOD;
                }
                return 1f;
            }
        }

        public float ClassTotalMPMultiplier
        {
            get
            {
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Wizard):
                    case (ClassType.Wizard2):
                        return PlayerEV.MAGE_MANA_MOD;
                    case (ClassType.Banker):
                    case (ClassType.Banker2):
                        return PlayerEV.BANKER_MANA_MOD;
                    case (ClassType.Barbarian):
                    case (ClassType.Barbarian2):
                        return PlayerEV.BARBARIAN_MANA_MOD;
                    case (ClassType.SpellSword):
                    case (ClassType.SpellSword2):
                        return PlayerEV.SPELLSWORD_MANA_MOD;//0.5f;
                    case (ClassType.Ninja):
                    case (ClassType.Ninja2):
                        return PlayerEV.NINJA_MANA_MOD;
                    case (ClassType.Assassin):
                    case (ClassType.Assassin2):
                        return PlayerEV.ASSASSIN_MANA_MOD;
                    case (ClassType.Lich):
                    case (ClassType.Lich2):
                        return PlayerEV.LICH_MANA_MOD;
                    case (ClassType.Dragon):
                        return PlayerEV.DRAGON_MANA_MOD;//1.5f; 
                    case (ClassType.Traitor):
                        return PlayerEV.TRAITOR_MANA_MOD;
                }
                return 1f;
            }
        }

        public float ClassMoveSpeedMultiplier
        {
            get
            {
                switch (Game.PlayerStats.Class)
                {
                    case (ClassType.Ninja):
                    case (ClassType.Ninja2):
                        return PlayerEV.NINJA_MOVESPEED_MOD;//1.5f;
                    case (ClassType.Dragon):
                        return PlayerEV.DRAGON_MOVESPEED_MOD;//1.5f;                       
                }
                return 0f;
            }
        }

        public float SpellCastDelay
        {
            get { return m_spellCastDelay; }
        }

        public bool LightOn
        {
            get { return m_lightOn; }
        }

        public override SpriteEffects Flip
        {
            get { return base.Flip; }
            set
            {
                if (Game.PlayerStats.Traits.X == TraitType.StereoBlind || Game.PlayerStats.Traits.Y == TraitType.StereoBlind)
                {
                    if (Flip != value)
                    {
                        if (m_flipTween != null && m_flipTween.TweenedObject == this && m_flipTween.Active == true)
                            m_flipTween.StopTween(false);

                        float storedX = m_internalScale.X;
                        this.ScaleX = 0;
                        m_flipTween = Tween.To(this, 0.15f, Tweener.Tween.EaseNone, "ScaleX", storedX.ToString());
                    }
                }
                base.Flip = value;
            }
        }

        public bool CastingDamageShield
        {
            get { return m_damageShieldCast; }
        }
    }
}

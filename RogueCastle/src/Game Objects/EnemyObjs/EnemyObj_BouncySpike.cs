using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_BouncySpike : EnemyObj
    {
        private float RotationSpeed = 250;//600;
        private float m_internalOrientation;

        public Vector2 SavedStartingPos { get; set; }
        public RoomObj SpawnRoom;

        private float m_selfDestructTimer = 0.7f;//1;
        private int m_selfDestructCounter = 0;
        private int m_selfDestructTotalBounces = 12;//14;//11;//7; //Total bounce needed in 1 second to destroy a spike.

        protected override void InitializeEV()
        {
            //this.Orientation = CDGMath.RandomInt(-180, 180);
            int randomizer = CDGMath.RandomInt(0,11);
            if (randomizer >= 9)
                this.Orientation = 0;
            else if (randomizer >= 6)
                this.Orientation = 180;
            else if (randomizer >= 4)
                this.Orientation = 90;
            else if (randomizer >= 1)
                this.Orientation = 270;
            else
                this.Orientation = 45;

            m_internalOrientation = this.Orientation;

            this.HeadingX = (float)Math.Cos(MathHelper.ToRadians(this.Orientation));
            this.HeadingY = (float)Math.Sin(MathHelper.ToRadians(this.Orientation));

            #region Basic Variables - General
            Name = EnemyEV.BouncySpike_Basic_Name;
            LocStringID = EnemyEV.BouncySpike_Basic_Name_locID;

            MaxHealth = EnemyEV.BouncySpike_Basic_MaxHealth;
            Damage = EnemyEV.BouncySpike_Basic_Damage;
            XPValue = EnemyEV.BouncySpike_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.BouncySpike_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.BouncySpike_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.BouncySpike_Basic_DropChance;

            Speed = EnemyEV.BouncySpike_Basic_Speed;
            TurnSpeed = EnemyEV.BouncySpike_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.BouncySpike_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.BouncySpike_Basic_Jump;
            CooldownTime = EnemyEV.BouncySpike_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.BouncySpike_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.BouncySpike_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.BouncySpike_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.BouncySpike_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.BouncySpike_Basic_IsWeighted;

            Scale = EnemyEV.BouncySpike_Basic_Scale;
            ProjectileScale = EnemyEV.BouncySpike_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.BouncySpike_Basic_Tint;

            MeleeRadius = EnemyEV.BouncySpike_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.BouncySpike_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.BouncySpike_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = new Vector2(1, 2);
            LockFlip = true;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.BouncySpike_Miniboss_Name;
                    LocStringID = EnemyEV.BouncySpike_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.BouncySpike_Miniboss_MaxHealth;
                    Damage = EnemyEV.BouncySpike_Miniboss_Damage;
                    XPValue = EnemyEV.BouncySpike_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.BouncySpike_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.BouncySpike_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.BouncySpike_Miniboss_DropChance;

                    Speed = EnemyEV.BouncySpike_Miniboss_Speed;
                    TurnSpeed = EnemyEV.BouncySpike_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.BouncySpike_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.BouncySpike_Miniboss_Jump;
                    CooldownTime = EnemyEV.BouncySpike_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.BouncySpike_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.BouncySpike_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.BouncySpike_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.BouncySpike_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.BouncySpike_Miniboss_IsWeighted;

                    Scale = EnemyEV.BouncySpike_Miniboss_Scale;
                    ProjectileScale = EnemyEV.BouncySpike_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.BouncySpike_Miniboss_Tint;

                    MeleeRadius = EnemyEV.BouncySpike_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.BouncySpike_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.BouncySpike_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = new Vector2(1, 2);
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.BouncySpike_Expert_Name;
                    LocStringID = EnemyEV.BouncySpike_Expert_Name_locID;

                    MaxHealth = EnemyEV.BouncySpike_Expert_MaxHealth;
                    Damage = EnemyEV.BouncySpike_Expert_Damage;
                    XPValue = EnemyEV.BouncySpike_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.BouncySpike_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.BouncySpike_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.BouncySpike_Expert_DropChance;

                    Speed = EnemyEV.BouncySpike_Expert_Speed;
                    TurnSpeed = EnemyEV.BouncySpike_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.BouncySpike_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.BouncySpike_Expert_Jump;
                    CooldownTime = EnemyEV.BouncySpike_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.BouncySpike_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.BouncySpike_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.BouncySpike_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.BouncySpike_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.BouncySpike_Expert_IsWeighted;

                    Scale = EnemyEV.BouncySpike_Expert_Scale;
                    ProjectileScale = EnemyEV.BouncySpike_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.BouncySpike_Expert_Tint;

                    MeleeRadius = EnemyEV.BouncySpike_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.BouncySpike_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.BouncySpike_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = new Vector2(1, 2);
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.BouncySpike_Advanced_Name;
                    LocStringID = EnemyEV.BouncySpike_Advanced_Name_locID;

                    MaxHealth = EnemyEV.BouncySpike_Advanced_MaxHealth;
                    Damage = EnemyEV.BouncySpike_Advanced_Damage;
                    XPValue = EnemyEV.BouncySpike_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.BouncySpike_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.BouncySpike_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.BouncySpike_Advanced_DropChance;

                    Speed = EnemyEV.BouncySpike_Advanced_Speed;
                    TurnSpeed = EnemyEV.BouncySpike_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.BouncySpike_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.BouncySpike_Advanced_Jump;
                    CooldownTime = EnemyEV.BouncySpike_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.BouncySpike_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.BouncySpike_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.BouncySpike_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.BouncySpike_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.BouncySpike_Advanced_IsWeighted;

                    Scale = EnemyEV.BouncySpike_Advanced_Scale;
                    ProjectileScale = EnemyEV.BouncySpike_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.BouncySpike_Advanced_Tint;

                    MeleeRadius = EnemyEV.BouncySpike_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.BouncySpike_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.BouncySpike_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = new Vector2(1, 2);
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }		

        }

        protected override void RunBasicLogic()
        {
            switch (State)
            {
                case (STATE_MELEE_ENGAGE):
                case (STATE_PROJECTILE_ENGAGE):
                case (STATE_ENGAGE):
                case (STATE_WANDER):
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

        public override void Update(GameTime gameTime)
        {
            if (IsPaused == false)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Checking for boundary bounces.
                Vector2 boundaryMTD = Vector2.Zero;
                Rectangle roomBounds = m_levelScreen.CurrentRoom.Bounds;

                if (this.Y < roomBounds.Top + 10)
                    boundaryMTD = CollisionMath.CalculateMTD(this.Bounds, new Rectangle(roomBounds.Left, roomBounds.Top, roomBounds.Width, 10));
                else if (this.Y > roomBounds.Bottom - 10)
                    boundaryMTD = CollisionMath.CalculateMTD(this.Bounds, new Rectangle(roomBounds.Left, roomBounds.Bottom - 10, roomBounds.Width, 10));

                if (this.X > roomBounds.Right - 10)
                    boundaryMTD = CollisionMath.CalculateMTD(this.Bounds, new Rectangle(roomBounds.Right - 10, roomBounds.Top, 10, roomBounds.Height));
                else if (this.X < roomBounds.Left + 10)
                    boundaryMTD = CollisionMath.CalculateMTD(this.Bounds, new Rectangle(roomBounds.Left, roomBounds.Top, 10, roomBounds.Height));

                if (boundaryMTD != Vector2.Zero)
                {
                    Vector2 v = Heading;
                    Vector2 l = new Vector2(boundaryMTD.Y, boundaryMTD.X * -1); // The angle of the side the vector hit is the normal to the MTD.
                    Vector2 newHeading = ((2 * (CDGMath.DotProduct(v, l) / CDGMath.DotProduct(l, l)) * l) - v);
                    this.Heading = newHeading;
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "GiantSpike_Bounce_01", "GiantSpike_Bounce_02", "GiantSpike_Bounce_03");
                    m_selfDestructCounter++;
                    m_selfDestructTimer = 1;
                }

                if (m_selfDestructTimer > 0)
                {
                    m_selfDestructTimer -= elapsedTime;
                    if (m_selfDestructTimer <= 0)
                        m_selfDestructCounter = 0;
                }

                if (m_selfDestructCounter >= m_selfDestructTotalBounces)
                    this.Kill(false);

                if (CurrentSpeed == 0)
                    CurrentSpeed = Speed;

                if (this.HeadingX > 0)
                    this.Rotation += RotationSpeed * elapsedTime;
                else
                    this.Rotation -= RotationSpeed * elapsedTime;
            }
            base.Update(gameTime);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            TerrainObj terrain = otherBox.Parent as TerrainObj;

            if (terrain != null && (terrain is DoorObj) == false)
            {
                if (terrain.CollidesBottom == true && terrain.CollidesLeft == true && terrain.CollidesRight == true && terrain.CollidesTop == true)
                {
                    Vector2 mtd = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, (int)thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, (int)otherBox.AbsRotation, Vector2.Zero);
                    if (mtd != Vector2.Zero)
                    {
                        Vector2 v = Heading;
                        Vector2 l = new Vector2(mtd.Y, mtd.X * -1); // The angle of the side the vector hit is the normal to the MTD.
                        Vector2 newHeading = ((2 * (CDGMath.DotProduct(v, l) / CDGMath.DotProduct(l, l)) * l) - v);
                        this.X += mtd.X;
                        this.Y += mtd.Y;
                        this.Heading = newHeading;
                        SoundManager.Play3DSound(this, Game.ScreenManager.Player,"GiantSpike_Bounce_01", "GiantSpike_Bounce_02", "GiantSpike_Bounce_03");
                        m_selfDestructCounter++;
                        m_selfDestructTimer = 1;
                    }
                }
            }
        }

        public override void Reset()
        {
            if (SpawnRoom != null)
            {
                m_levelScreen.RemoveEnemyFromRoom(this, SpawnRoom, this.SavedStartingPos);
                this.Dispose();
            }
            else
            {
                this.Orientation = m_internalOrientation;
                base.Reset();
            }
        }

        public EnemyObj_BouncySpike(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemyBouncySpike_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            this.Type = EnemyType.BouncySpike;
            NonKillable = true;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                SpawnRoom = null;
                base.Dispose();
            }
        }
    }
}

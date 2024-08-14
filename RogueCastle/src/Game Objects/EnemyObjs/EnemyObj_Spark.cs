using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyObj_Spark : EnemyObj
    {
        private bool m_hookedToGround = false;
        private byte m_collisionBoxSize = 10; // How large the side collision boxes should be. The faster the spark moves, the faster this should be.

        protected override void InitializeEV()
        {
            LockFlip = true;
            this.IsWeighted = false;

            #region Basic Variables - General
            Name = EnemyEV.Spark_Basic_Name;
            LocStringID = EnemyEV.Spark_Basic_Name_locID;

            MaxHealth = EnemyEV.Spark_Basic_MaxHealth;
            Damage = EnemyEV.Spark_Basic_Damage;
            XPValue = EnemyEV.Spark_Basic_XPValue;

            MinMoneyDropAmount = EnemyEV.Spark_Basic_MinDropAmount;
            MaxMoneyDropAmount = EnemyEV.Spark_Basic_MaxDropAmount;
            MoneyDropChance = EnemyEV.Spark_Basic_DropChance;

            Speed = EnemyEV.Spark_Basic_Speed;
            TurnSpeed = EnemyEV.Spark_Basic_TurnSpeed;
            ProjectileSpeed = EnemyEV.Spark_Basic_ProjectileSpeed;
            JumpHeight = EnemyEV.Spark_Basic_Jump;
            CooldownTime = EnemyEV.Spark_Basic_Cooldown;
            AnimationDelay = 1 / EnemyEV.Spark_Basic_AnimationDelay;

            AlwaysFaceTarget = EnemyEV.Spark_Basic_AlwaysFaceTarget;
            CanFallOffLedges = EnemyEV.Spark_Basic_CanFallOffLedges;
            CanBeKnockedBack = EnemyEV.Spark_Basic_CanBeKnockedBack;
            IsWeighted = EnemyEV.Spark_Basic_IsWeighted;

            Scale = EnemyEV.Spark_Basic_Scale;
            ProjectileScale = EnemyEV.Spark_Basic_ProjectileScale;
            TintablePart.TextureColor = EnemyEV.Spark_Basic_Tint;

            MeleeRadius = EnemyEV.Spark_Basic_MeleeRadius;
            ProjectileRadius = EnemyEV.Spark_Basic_ProjectileRadius;
            EngageRadius = EnemyEV.Spark_Basic_EngageRadius;

            ProjectileDamage = Damage;
            KnockBack = EnemyEV.Spark_Basic_KnockBack;
            #endregion

            switch (Difficulty)
            {
                case (GameTypes.EnemyDifficulty.MINIBOSS):
                    #region Miniboss Variables - General
                    Name = EnemyEV.Spark_Miniboss_Name;
                    LocStringID = EnemyEV.Spark_Miniboss_Name_locID;

                    MaxHealth = EnemyEV.Spark_Miniboss_MaxHealth;
                    Damage = EnemyEV.Spark_Miniboss_Damage;
                    XPValue = EnemyEV.Spark_Miniboss_XPValue;

                    MinMoneyDropAmount = EnemyEV.Spark_Miniboss_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Spark_Miniboss_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Spark_Miniboss_DropChance;

                    Speed = EnemyEV.Spark_Miniboss_Speed;
                    TurnSpeed = EnemyEV.Spark_Miniboss_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Spark_Miniboss_ProjectileSpeed;
                    JumpHeight = EnemyEV.Spark_Miniboss_Jump;
                    CooldownTime = EnemyEV.Spark_Miniboss_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Spark_Miniboss_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Spark_Miniboss_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Spark_Miniboss_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Spark_Miniboss_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Spark_Miniboss_IsWeighted;

                    Scale = EnemyEV.Spark_Miniboss_Scale;
                    ProjectileScale = EnemyEV.Spark_Miniboss_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Spark_Miniboss_Tint;

                    MeleeRadius = EnemyEV.Spark_Miniboss_MeleeRadius;
                    ProjectileRadius = EnemyEV.Spark_Miniboss_ProjectileRadius;
                    EngageRadius = EnemyEV.Spark_Miniboss_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Spark_Miniboss_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.EXPERT):
                    #region Expert Variables - General
                    Name = EnemyEV.Spark_Expert_Name;
                    LocStringID = EnemyEV.Spark_Expert_Name_locID;

                    MaxHealth = EnemyEV.Spark_Expert_MaxHealth;
                    Damage = EnemyEV.Spark_Expert_Damage;
                    XPValue = EnemyEV.Spark_Expert_XPValue;

                    MinMoneyDropAmount = EnemyEV.Spark_Expert_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Spark_Expert_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Spark_Expert_DropChance;

                    Speed = EnemyEV.Spark_Expert_Speed;
                    TurnSpeed = EnemyEV.Spark_Expert_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Spark_Expert_ProjectileSpeed;
                    JumpHeight = EnemyEV.Spark_Expert_Jump;
                    CooldownTime = EnemyEV.Spark_Expert_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Spark_Expert_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Spark_Expert_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Spark_Expert_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Spark_Expert_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Spark_Expert_IsWeighted;

                    Scale = EnemyEV.Spark_Expert_Scale;
                    ProjectileScale = EnemyEV.Spark_Expert_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Spark_Expert_Tint;

                    MeleeRadius = EnemyEV.Spark_Expert_MeleeRadius;
                    ProjectileRadius = EnemyEV.Spark_Expert_ProjectileRadius;
                    EngageRadius = EnemyEV.Spark_Expert_EngageRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Spark_Expert_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.ADVANCED):
                    #region Advanced Variables - General
                    Name = EnemyEV.Spark_Advanced_Name;
                    LocStringID = EnemyEV.Spark_Advanced_Name_locID;

                    MaxHealth = EnemyEV.Spark_Advanced_MaxHealth;
                    Damage = EnemyEV.Spark_Advanced_Damage;
                    XPValue = EnemyEV.Spark_Advanced_XPValue;

                    MinMoneyDropAmount = EnemyEV.Spark_Advanced_MinDropAmount;
                    MaxMoneyDropAmount = EnemyEV.Spark_Advanced_MaxDropAmount;
                    MoneyDropChance = EnemyEV.Spark_Advanced_DropChance;

                    Speed = EnemyEV.Spark_Advanced_Speed;
                    TurnSpeed = EnemyEV.Spark_Advanced_TurnSpeed;
                    ProjectileSpeed = EnemyEV.Spark_Advanced_ProjectileSpeed;
                    JumpHeight = EnemyEV.Spark_Advanced_Jump;
                    CooldownTime = EnemyEV.Spark_Advanced_Cooldown;
                    AnimationDelay = 1 / EnemyEV.Spark_Advanced_AnimationDelay;

                    AlwaysFaceTarget = EnemyEV.Spark_Advanced_AlwaysFaceTarget;
                    CanFallOffLedges = EnemyEV.Spark_Advanced_CanFallOffLedges;
                    CanBeKnockedBack = EnemyEV.Spark_Advanced_CanBeKnockedBack;
                    IsWeighted = EnemyEV.Spark_Advanced_IsWeighted;

                    Scale = EnemyEV.Spark_Advanced_Scale;
                    ProjectileScale = EnemyEV.Spark_Advanced_ProjectileScale;
                    TintablePart.TextureColor = EnemyEV.Spark_Advanced_Tint;

                    MeleeRadius = EnemyEV.Spark_Advanced_MeleeRadius;
                    EngageRadius = EnemyEV.Spark_Advanced_EngageRadius;
                    ProjectileRadius = EnemyEV.Spark_Advanced_ProjectileRadius;

                    ProjectileDamage = Damage;
                    KnockBack = EnemyEV.Spark_Advanced_KnockBack;
                    #endregion
                    break;

                case (GameTypes.EnemyDifficulty.BASIC):
                default:
                    break;
            }			

        }

        protected override void InitializeLogic()
        {

            this.CurrentSpeed = Speed;
            base.InitializeLogic();
        }

        public void HookToGround()
        {
            m_hookedToGround = true;
            float closestGround = 1000;
            TerrainObj closestTerrain = null;
            foreach (TerrainObj terrainObj in m_levelScreen.CurrentRoom.TerrainObjList)
            {
                if (terrainObj.Y >= this.Y)
                {
                    if (terrainObj.Y - this.Y < closestGround && CollisionMath.Intersects(terrainObj.Bounds, new Rectangle((int)this.X, (int)(this.Y + (terrainObj.Y - this.Y) + 5), this.Width, (int)(this.Height / 2))))
                    {
                        closestGround = terrainObj.Y - this.Y;
                        closestTerrain = terrainObj;
                    }
                }
            }

            if (closestTerrain != null)
                //this.Y = closestTerrain.Y - (this.TerrainBounds.Bottom - this.Y - 40);
                this.Y = closestTerrain.Y -(this.Height / 2) + 5;
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
            if (m_hookedToGround == false)
                HookToGround();

            CollisionCheckRight();
            if (IsPaused == false)
                this.Position += this.Heading * (this.CurrentSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        private void CollisionCheckRight()
        {
            bool collidesTop = false;
            bool collidesBottom = false;
            bool collidesLeft = false;
            bool collidesRight = false;

            bool collidesTL = false;
            bool collidesTR = false;
            bool collidesBL = false;
            bool collidesBR = false;
            float rotation = 0;

            if (this.Bounds.Right >= m_levelScreen.CurrentRoom.Bounds.Right)
            {
                collidesTR = true;
                collidesRight = true;
                collidesBR = true;
            }
            else if (this.Bounds.Left <= m_levelScreen.CurrentRoom.Bounds.Left)
            {
                collidesTL = true;
                collidesLeft = true;
                collidesBL = true;
            }

            if (this.Bounds.Top <= m_levelScreen.CurrentRoom.Bounds.Top)
            {
                collidesTR = true;
                collidesTop = true;
                collidesTL = true;
            }
            else if (this.Bounds.Bottom >= m_levelScreen.CurrentRoom.Bounds.Bottom)
            {
                collidesBL = true;
                collidesBottom = true;
                collidesBR = true;
            }
               

            foreach (TerrainObj obj in m_levelScreen.CurrentRoom.TerrainObjList)
            {
                Rectangle objAbsRect = new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height);

                if (CollisionMath.RotatedRectIntersects(TopLeftPoint, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                    collidesTL = true;

                if (CollisionMath.RotatedRectIntersects(TopRightPoint, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                    collidesTR = true;

                if (CollisionMath.RotatedRectIntersects(BottomRightPoint, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                {
                    collidesBR = true;
                    if (obj.Rotation != 0)
                    {
                        Vector2 mtd = CollisionMath.RotatedRectIntersectsMTD(this.BottomRightPoint, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero);
                        if (mtd.X < 0 && mtd.Y < 0)
                            rotation = -45;
                    }
                }

                if (CollisionMath.RotatedRectIntersects(BottomLeftPoint, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                {
                    collidesBL = true;
                    if (obj.Rotation != 0)
                    {
                        Vector2 mtd = CollisionMath.RotatedRectIntersectsMTD(BottomLeftPoint, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero);
                        if (mtd.X > 0 && mtd.Y < 0)
                            rotation = 45;
                    }
                }

                if (CollisionMath.RotatedRectIntersects(TopRect, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                    collidesTop = true;

                if (CollisionMath.RotatedRectIntersects(BottomRect, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                    collidesBottom = true;

                if (CollisionMath.RotatedRectIntersects(LeftRect, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                    collidesLeft = true;

                if (CollisionMath.RotatedRectIntersects(RightRect, 0, Vector2.Zero, objAbsRect, obj.Rotation, Vector2.Zero))
                    collidesRight = true;
            }

            if (collidesBR == true && collidesTR == false && collidesRight == false)
                this.Orientation = 0;

            if ((collidesTR == true && collidesBR == true) && collidesTL == false)
                this.Orientation = MathHelper.ToRadians(-90);

            if ((collidesTR == true && collidesTL == true) && collidesBL == false)
                this.Orientation = MathHelper.ToRadians(-180);

            if (collidesTL == true &&  collidesLeft == true && collidesBottom == false)//collidesBL == true && collidesLeft == true && collidesBottom == false)
                this.Orientation = MathHelper.ToRadians(90);

            // Special cliff cases
            if (collidesTR == true && collidesTop == false && collidesRight == false)
                this.Orientation = MathHelper.ToRadians(-90);

            if (collidesTL == true && collidesTop == false && collidesLeft == false)
                this.Orientation = MathHelper.ToRadians(-180);

            if (collidesBL == true && collidesLeft == false && collidesRight == false && collidesBottom == false)
                this.Orientation = MathHelper.ToRadians(90);

            if (collidesBR == true && collidesBottom == false && collidesRight == false)
                this.Orientation = 0;

            if (rotation != 0)
            {
                if ((rotation < 0 && collidesBR == true && collidesRight == true) || (rotation > 0 && collidesBR == false))
                    this.Orientation = MathHelper.ToRadians(rotation);
            }

            this.HeadingX = (float)Math.Cos(this.Orientation);
            this.HeadingY = (float)Math.Sin(this.Orientation);
        }

        public EnemyObj_Spark(PlayerObj target, PhysicsManager physicsManager, ProceduralLevelScreen levelToAttachTo, GameTypes.EnemyDifficulty difficulty)
            : base("EnemySpark_Character", target, physicsManager, levelToAttachTo, difficulty)
        {
            IsWeighted = false;
            ForceDraw = true;
            this.Type = EnemyType.Spark;
            this.NonKillable = true;
        }

        //public override void Draw(Camera2D camera)
        //{
        //    base.Draw(camera);
        //    camera.Draw(Game.GenericTexture, TopLeftPoint, Color.Green * 0.5f);
        //    camera.Draw(Game.GenericTexture, TopRightPoint, Color.Green * 0.5f);
        //    camera.Draw(Game.GenericTexture, BottomLeftPoint, Color.Green * 0.5f);
        //    camera.Draw(Game.GenericTexture, BottomRightPoint, Color.Green * 0.5f);

        //    camera.Draw(Game.GenericTexture, LeftRect, Color.Red * 0.5f);
        //    camera.Draw(Game.GenericTexture, RightRect, Color.Red * 0.5f);
        //    camera.Draw(Game.GenericTexture, BottomRect, Color.Red * 0.5f);
        //    camera.Draw(Game.GenericTexture, TopRect, Color.Red * 0.5f);
        //}

        private Rectangle TopRect
        {
            get { return new Rectangle(this.Bounds.Left + m_collisionBoxSize, this.Bounds.Top, this.Width - (m_collisionBoxSize * 2) , m_collisionBoxSize); }
        }

        private Rectangle BottomRect
        {
            get { return new Rectangle(this.Bounds.Left + m_collisionBoxSize, this.Bounds.Bottom - m_collisionBoxSize, this.Width - (m_collisionBoxSize * 2), m_collisionBoxSize); }
        }

        private Rectangle LeftRect
        {
            get { return new Rectangle(this.Bounds.Left, this.Bounds.Top + m_collisionBoxSize, m_collisionBoxSize, this.Height - (m_collisionBoxSize * 2)); }
        }

        private Rectangle RightRect
        {
            get { return new Rectangle(this.Bounds.Right - m_collisionBoxSize, this.Bounds.Top + m_collisionBoxSize, m_collisionBoxSize, this.Height - (m_collisionBoxSize * 2)); }
        }


        private Rectangle TopLeftPoint
        {
            get { return new Rectangle(this.Bounds.Left, this.Bounds.Top, m_collisionBoxSize, m_collisionBoxSize); }
        }

        private Rectangle TopRightPoint
        {
            get { return new Rectangle(this.Bounds.Right - m_collisionBoxSize, this.Bounds.Top, m_collisionBoxSize, m_collisionBoxSize); }
        }

        private Rectangle BottomLeftPoint
        {
            get { return new Rectangle(this.Bounds.Left, this.Bounds.Bottom - m_collisionBoxSize, m_collisionBoxSize, m_collisionBoxSize); }
        }

        private Rectangle BottomRightPoint
        {
            get { return new Rectangle(this.Bounds.Right - m_collisionBoxSize, this.Bounds.Bottom - m_collisionBoxSize, m_collisionBoxSize, m_collisionBoxSize); }
        }
    }
}

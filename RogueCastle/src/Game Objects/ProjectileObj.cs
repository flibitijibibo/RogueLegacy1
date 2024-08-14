using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener.Ease;
using Tweener;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class ProjectileObj : PhysicsObj, IDealsDamageObj//, IPoolableObj
    {
        public int Damage { get; set; }
        public float RotationSpeed { get; set; }
        public float LifeSpan = 0;
        private float m_elapsedLifeSpan = 0;
        public bool IsAlive { get; internal set; }
        public bool CollidesWithTerrain { get; set; }
        public bool DestroysWithTerrain { get; set; } // Is destroyed upon hitting terrain.
        public bool DestroysWithEnemy { get; set; }
        public GameObj Target { get; set; }
        public bool ChaseTarget { get; set; }
        public bool FollowArc { get; set; }
        public ProjectileIconObj AttachedIcon { get; set; }
        public bool ShowIcon { get; set; }
        public bool IgnoreBoundsCheck { get; set; }
        public bool CollidesWith1Ways { get; set; }
        public bool GamePaused { get; set; }
        public bool DestroyOnRoomTransition { get; set; }
        public bool CanBeFusRohDahed { get; set; }
        public bool IgnoreInvincibleCounter { get; set; }

        public bool IsDying { get; internal set; }

        // Property used exclusively for player spells.
        public int Spell { get; set; }
        public float AltX { get; set; }
        public float AltY { get; set; }
        public float BlinkTime { get; set; }

        private Color m_blinkColour = Color.White;
        private float m_blinkTimer = 0;

        public GameObj Source { get; set; }
        public bool WrapProjectile { get; set; }

        public ProjectileObj(string spriteName)
            : base(spriteName)
        {
            CollisionTypeTag = GameTypes.CollisionType_ENEMY;
            CollidesWithTerrain = true;
            ChaseTarget = false;
            IsDying = false;
            DestroysWithEnemy = true;
            DestroyOnRoomTransition = true;
        }

        public void Reset()
        {
            Source = null;
            CollidesWithTerrain = true;
            DestroysWithTerrain = true;
            DestroysWithEnemy = true;
            IsCollidable = true;
            IsWeighted = true;
            IsDying = false;
            IsAlive = true;
            m_elapsedLifeSpan = 0;
            Rotation = 0;
            this.TextureColor = Color.White;
            Spell = 0;
            AltY = 0;
            AltX = 0;
            BlinkTime = 0;
            IgnoreBoundsCheck = false;
            Scale = Vector2.One;
            DisableHitboxUpdating = false;
            m_blinkColour = Color.White;
            m_blinkTimer = 0;
            AccelerationYEnabled = true;
            AccelerationXEnabled = true;
            GamePaused = false;
            DestroyOnRoomTransition = true;
            CanBeFusRohDahed = true;
            this.Flip = SpriteEffects.None;
            IgnoreInvincibleCounter = false;
            WrapProjectile = false;

            DisableCollisionBoxRotations = true; // Don't do rotated rect collision detection on projectiles.  This is for performance purposes.
            Tween.StopAllContaining(this, false);
        }

        public void UpdateHeading()
        {
            if (ChaseTarget == true && Target != null)
            {
                Vector2 seekPosition = Target.Position;
                TurnToFace(seekPosition, TurnSpeed, 1/60f);

                this.HeadingX = (float)Math.Cos(this.Orientation);
                this.HeadingY = (float)Math.Sin(this.Orientation);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsPaused == false)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                switch (Spell)
                {
                    case (SpellType.TimeBomb):
                        if (BlinkTime >= AltX && AltX != 0)
                        {
                            Blink(Color.Red, 0.1f);
                            BlinkTime = AltX / 1.5f;
                        }
                        if (AltX > 0)
                        {
                            AltX -= elapsedTime;
                            if (AltX <= 0)
                                this.ActivateEffect();
                        }
                        break;
                    case (SpellType.Nuke):
                        // Smoke effect
                        if (AltY > 0)
                        {
                            AltY -= elapsedTime;
                            if (AltY <= 0)
                            {
                                ProceduralLevelScreen level = Game.ScreenManager.CurrentScreen as ProceduralLevelScreen;
                                if (level != null)
                                {
                                    level.ImpactEffectPool.CrowSmokeEffect(this.Position);
                                    AltY = 0.05f;
                                }
                            }
                        }

                        if (AltX <= 0)
                        {
                            Vector2 seekPosition = Target.Position;
                            TurnToFace(seekPosition, TurnSpeed, elapsedTime); // Turn to face already calls WrapAngle()
                        }
                        else
                        {
                            AltX -= elapsedTime;
                            this.Orientation = MathHelper.WrapAngle(Orientation);
                        }

                        this.HeadingX = (float)Math.Cos(this.Orientation);
                        this.HeadingY = (float)Math.Sin(this.Orientation);
                        this.AccelerationX = 0;
                        this.AccelerationY = 0;
                        this.Position += this.Heading * (this.CurrentSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);

                        if (this.HeadingX > 0)
                        {
                            this.Flip = SpriteEffects.None;
                            this.Rotation = MathHelper.ToDegrees(this.Orientation);
                        }
                        else
                        {
                            this.Flip = SpriteEffects.FlipHorizontally;
                            float angle = MathHelper.ToDegrees(this.Orientation);
                            if (angle < 0)
                                this.Rotation = (180 + angle) * 60 * elapsedTime;
                            else
                                this.Rotation = (-180 + angle) * 60 * elapsedTime;
                        }
                        this.Rotation = MathHelper.Clamp(this.Rotation, -90, 90);

                        // Destroy the spell if the enemy dies.
                        if (this.Target != null)
                        {
                            EnemyObj target = this.Target as EnemyObj;
                            if (target != null && target.IsKilled == true)
                                RunDestroyAnimation(false);
                        }
                        else
                            RunDestroyAnimation(false);

                        break;
                    case (SpellType.DamageShield):
                        PlayerObj player = Game.ScreenManager.Player;
                        if (player.CastingDamageShield == true || Source is EnemyObj) // Special handling for the last boss since he casts this spell too.
                        {
                            AltX += (CurrentSpeed * 60 * elapsedTime);
                            this.Position = CDGMath.GetCirclePosition(AltX, AltY, Target.Position);
                        }
                        else
                            this.KillProjectile();
                        break;
                    case (SpellType.Bounce):
                        this.AccelerationX = 0;
                        this.AccelerationY = 0;
                        this.HeadingX = (float)Math.Cos(this.Orientation);
                        this.HeadingY = (float)Math.Sin(this.Orientation);
                        //this.Rotation = MathHelper.ToDegrees(this.Orientation);
                        this.Position += this.Heading * (this.CurrentSpeed * elapsedTime);
                        if (this.AltY > 0)
                        {
                            this.AltY -= elapsedTime;
                            if (AltY <= 0)
                                ActivateEffect();
                        }
                        break;
                    case (SpellType.Boomerang):
                        this.AccelerationX -= (AltX * 60 * elapsedTime);
                        if (this.AltY > 0)
                        {
                            this.AltY -= elapsedTime;
                            if (AltY <= 0)
                                ActivateEffect();
                        }
                        break;
                    case (SpellType.Laser):
                        if (AltX > 0)
                        {
                            AltX -= elapsedTime;
                            this.Opacity = 0.9f - AltX;
                            this.ScaleY = 1 - AltX;
                            if (AltX <= 0)
                                ActivateEffect();
                        }
                        break;
                }

                if (ChaseTarget == true && Target != null)
                {
                    Vector2 seekPosition = Target.Position;
                    TurnToFace(seekPosition, TurnSpeed, elapsedTime);

                    this.HeadingX = (float)Math.Cos(this.Orientation);
                    this.HeadingY = (float)Math.Sin(this.Orientation);

                    this.AccelerationX = 0;
                    this.AccelerationY = 0;
                    this.Position += this.Heading * (this.CurrentSpeed * elapsedTime);
                    this.Rotation = MathHelper.ToDegrees(this.Orientation);
                }

                if (FollowArc == true && ChaseTarget == false && IsDying == false)
                {
                    float desiredAngle = (float)Math.Atan2(AccelerationY, AccelerationX);
                    this.Rotation = MathHelper.ToDegrees(desiredAngle);
                }
                else if (ChaseTarget == false)
                    this.Rotation += (this.RotationSpeed * 60 * elapsedTime);

                m_elapsedLifeSpan += elapsedTime;
                if (m_elapsedLifeSpan >= LifeSpan)
                    IsAlive = false; // Might want to change this to RunDestroyAnimation().

                if (m_blinkTimer > 0)
                {
                    m_blinkTimer -= elapsedTime;
                    this.TextureColor = m_blinkColour;
                }
                else if (this.TextureColor == m_blinkColour)
                    this.TextureColor = Color.White;
            }
        }

        public void Blink(Color blinkColour, float duration)
        {
            m_blinkColour = blinkColour;
            m_blinkTimer = duration;
        }

        private void TurnToFace(Vector2 facePosition, float turnSpeed, float elapsedSeconds)
        {
            float x = facePosition.X - this.Position.X;
            float y = facePosition.Y - this.Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = MathHelper.WrapAngle(desiredAngle - this.Orientation);

            float internalTurnSpeed = turnSpeed * 60 * elapsedSeconds;

            difference = MathHelper.Clamp(difference, -internalTurnSpeed, internalTurnSpeed);
            this.Orientation = MathHelper.WrapAngle(this.Orientation + difference);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            // Special handling for shout.
            if (this.Spell == SpellType.Shout)
            {
                ProjectileObj proj = otherBox.AbsParent as ProjectileObj;
                if (proj != null && proj.CollisionTypeTag != GameTypes.CollisionType_PLAYER && proj.CanBeFusRohDahed == true)
                    proj.RunDestroyAnimation(false);
            }

            TerrainObj terrain = otherBox.Parent as TerrainObj;
            if (CollidesWithTerrain == true && !(otherBox.Parent is DoorObj) && terrain != null && 
                ((terrain.CollidesTop == true && terrain.CollidesBottom == true && terrain.CollidesLeft == true && terrain.CollidesRight == true) || this.CollidesWith1Ways == true))
            {
                switch (Spell)
                {
                    case (SpellType.Displacer):
                        base.CollisionResponse(thisBox, otherBox, collisionResponseType);
                        this.IsWeighted = false;
                        ActivateEffect();
                        break;
                    case (SpellType.Bounce):
                        Vector2 mtd = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
                        if (mtd != Vector2.Zero)
                        {
                            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Spike_Bounce_01", "Spike_Bounce_02", "Spike_Bounce_03");
                            Vector2 v = Heading;
                            Vector2 l = new Vector2(mtd.Y, mtd.X * -1); // The angle of the side the vector hit is the normal to the MTD.
                            Vector2 newHeading = ((2 * (CDGMath.DotProduct(v, l) / CDGMath.DotProduct(l, l)) * l) - v);
                            this.X += mtd.X;
                            this.Y += mtd.Y;
                            this.Orientation = MathHelper.ToRadians(CDGMath.VectorToAngle(newHeading));
                            //this.CollisionTypeTag = GameTypes.CollisionType_GLOBAL_DAMAGE_WALL;
                            //this.Heading = newHeading;
                        }
                        break;
                    case (SpellType.TimeBomb): // Proper logic for weighted objects.
                        if (terrain.CollidesBottom == true && terrain.CollidesTop == true && terrain.CollidesLeft == true && terrain.CollidesRight == true)
                        {
                            Vector2 mtd2 = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
                            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
                            if ((mtd2.Y <= 0 && mtd2.X == 0) || otherBox.AbsRotation != 0)
                            {
                                this.AccelerationY = 0;
                                this.AccelerationX = 0;
                                this.IsWeighted = false;
                            }
                        }
                        else if (terrain.CollidesBottom == false && terrain.CollidesTop == true && terrain.CollidesLeft == false && terrain.CollidesRight == false)
                        {
                            Vector2 mtd2 = CollisionMath.RotatedRectIntersectsMTD(thisBox.AbsRect, thisBox.AbsRotation, Vector2.Zero, otherBox.AbsRect, otherBox.AbsRotation, Vector2.Zero);
                            if (mtd2.Y <= 0 && this.AccelerationY > 0)
                            {
                                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
                                if ((mtd2.Y <= 0 && mtd2.X == 0) || otherBox.AbsRotation != 0)
                                {
                                    this.AccelerationY = 0;
                                    this.AccelerationX = 0;
                                    this.IsWeighted = false;
                                }
                            }
                        }
                        break;
                    default: // Default sets AccelerationX to 0, effectively sticking the projectile to walls.
                        if (DestroysWithTerrain == true)
                            RunDestroyAnimation(false);
                        else
                        {
                            this.AccelerationY = 0;
                            this.AccelerationX = 0;
                            this.IsWeighted = false;
                        }
                        break;
                }
            }
            else if (otherBox.Type != Consts.TERRAIN_HITBOX) // We don't want projectile terrain boxes hitting player or enemy hitboxes.
            {
                switch (Spell)
                {
                    case (SpellType.Nuke):
                        if (otherBox.AbsParent == this.Target) // Ensures each crow only hits its target.
                            this.CollisionTypeTag = GameTypes.CollisionType_PLAYER;
                        break;
                    default:
                        base.CollisionResponse(thisBox, otherBox, collisionResponseType);
                        break;
                }
            }
        }

        // An effect made specifically for the displacer.
        public void RunDisplacerEffect(RoomObj room, PlayerObj player)
        {
            int closestDistance = int.MaxValue;
            TerrainObj closestObj = null;

            Vector2 collisionPt = Vector2.Zero;

            foreach (TerrainObj terrain in room.TerrainObjList)
            {
                collisionPt = Vector2.Zero;
                // Only collide with terrain that the displacer would collide with.
                float distance = float.MaxValue;
                if (player.Flip == SpriteEffects.None)
                {
                    if (terrain.X > this.X && (terrain.Bounds.Top < this.Bounds.Bottom && terrain.Bounds.Bottom > this.Bounds.Top))
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
                            distance = terrain.Bounds.Left - this.Bounds.Right;
                    }
                }
                else
                {
                    if (terrain.X < this.X && (terrain.Bounds.Top < this.Bounds.Bottom && terrain.Bounds.Bottom > this.Bounds.Top))
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
                            distance = this.Bounds.Left - terrain.Bounds.Right;
                    }
                }

                if (distance < closestDistance)
                {
                    closestDistance = (int)distance;
                    closestObj = terrain;
                }
            }

            if (closestObj != null)
            {
                if (player.Flip == SpriteEffects.None)
                {
                    if (closestObj.Rotation == 0)
                        player.X += closestDistance - player.TerrainBounds.Width / 2f;
                    else
                        player.X += closestDistance - player.Width / 2f;
                }
                else
                {
                    if (closestObj.Rotation == 0)
                        player.X -= closestDistance - player.TerrainBounds.Width / 2f;
                    else
                        player.X -= closestDistance - player.Width / 2f;
                }
                //Console.WriteLine(closestDistance);
                //Tween.By(closestObj, 10, Tween.EaseNone, "Y", "-1000");
            }
        }

        public void RunDestroyAnimation(bool hitPlayer)
        {
            if (IsDying == false && IsDemented == false)
            {
                this.CurrentSpeed = 0;
                this.AccelerationX = 0;
                this.AccelerationY = 0;
                IsDying = true;

                switch (SpriteName)
                {
                    case ("ArrowProjectile_Sprite"):
                    case ("SpellClose_Sprite"):
                    case ("SpellDagger_Sprite"):
                        if (hitPlayer == true)
                        {
                            Tween.By(this, 0.3f, Linear.EaseNone, "Rotation", "270");
                            int randArrowX = CDGMath.RandomInt(-50, 50);
                            int randArrowY = CDGMath.RandomInt(-100, -50);
                            Tween.By(this, 0.3f, Linear.EaseNone, "X", randArrowX.ToString(), "Y", randArrowY.ToString());
                            Tween.To(this, 0.3f, Linear.EaseNone, "Opacity", "0");
                            Tween.AddEndHandlerToLastTween(this, "KillProjectile");
                        }
                        else
                        {
                            this.IsWeighted = false;
                            this.IsCollidable = false;
                            Tween.To(this, 0.3f, Linear.EaseNone, "delay", "0.3", "Opacity", "0");
                            Tween.AddEndHandlerToLastTween(this, "KillProjectile");
                        }
                        break;
                    case ("ShurikenProjectile1_Sprite"):
                    case ("BoneProjectile_Sprite"):
                    case ("SpellBounce_Sprite"):
                    case ("LastBossSwordVerticalProjectile_Sprite"):
                        Tween.StopAllContaining(this, false); // Special handle since the last boss uses tweens to move this projectile instead of the normal way.
                        this.IsCollidable = false;
                        int randX = CDGMath.RandomInt(-50, 50);
                        int randY = CDGMath.RandomInt(-100, 100);
                        Tween.By(this, 0.3f, Linear.EaseNone, "X", randX.ToString(), "Y", randY.ToString());
                        Tween.To(this, 0.3f, Linear.EaseNone, "Opacity", "0");
                        Tween.AddEndHandlerToLastTween(this, "KillProjectile");
                        break;
                    case ("HomingProjectile_Sprite"):
                        ProceduralLevelScreen screen = Game.ScreenManager.CurrentScreen as ProceduralLevelScreen;
                        if (screen != null)
                            screen.ImpactEffectPool.DisplayExplosionEffect(this.Position);
                        SoundManager.Play3DSound(this, Game.ScreenManager.Player, "MissileExplosion_01", "MissileExplosion_02");
                        KillProjectile();
                        break;
                    case ("SpellAxe_Sprite"):
                    case ("SpellDualBlades_Sprite"):
                        this.IsCollidable = false;
                        this.AccelerationX = 0;
                        this.AccelerationY = 0;
                        Tween.To(this, 0.3f, Tween.EaseNone, "Opacity", "0");
                        Tween.AddEndHandlerToLastTween(this, "KillProjectile");
                        break;
                    case ("SpellDamageShield_Sprite"):
                    case ("SpellDisplacer_Sprite"):
                        Tween.To(this, 0.2f, Tween.EaseNone, "Opacity", "0");
                        Tween.AddEndHandlerToLastTween(this, "KillProjectile");
                        break;
                    case ("LastBossSwordProjectile_Sprite"):
                        this.IsCollidable = false;
                        Tween.StopAllContaining(this, false); // Special handle since the last boss uses tweens to move this projectile instead of the normal way.
                        Tween.By(this, 0.3f, Linear.EaseNone, "Rotation", "270");
                        int randArrow = CDGMath.RandomInt(-100, -50);
                        Tween.By(this, 0.3f, Linear.EaseNone, "Y", randArrow.ToString());
                        Tween.To(this, 0.3f, Linear.EaseNone, "Opacity", "0");
                        Tween.AddEndHandlerToLastTween(this, "KillProjectile");
                        break;
                    case("SpellNuke_Sprite"):
                        ProceduralLevelScreen level = Game.ScreenManager.CurrentScreen as ProceduralLevelScreen;
                        if (level != null)
                            level.ImpactEffectPool.CrowDestructionEffect(this.Position);
                        KillProjectile();
                        break;
                    case ("EnemyFlailKnightBall_Sprite"):
                    case ("WizardIceSpell_Sprite"):
                    case ("WizardEarthSpell_Sprite"):
                    case ("SpellTimeBomb_Sprite"):
                    case ("SpellLaser_Sprite"):
                    case ("SpellBoomerang_Sprite"):
                        // Do nothing for projectiles that don't get destroyed on impact.
                        KillProjectile();
                        break;
                    default:
                        if (SpriteName == "WizardIceProjectile_Sprite")
                            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Ice_Wizard_Break_01", "Ice_Wizard_Break_02", "Ice_Wizard_Break_03");
                        string explosionName = SpriteName.Replace("_", "Explosion_");
                        this.ChangeSprite(explosionName);
                        this.AnimationDelay = 1 / 30f;
                        this.PlayAnimation(false);
                        this.IsWeighted = false;
                        this.IsCollidable = false;
                        if (explosionName != "EnemySpearKnightWaveExplosion_Sprite" && explosionName != "WizardIceProjectileExplosion_Sprite")
                            this.Rotation = 0;
                        Tweener.Tween.RunFunction(0.5f, this, "KillProjectile");
                        break;
                }
            }
        }

        public void ActivateEffect()
        {
            switch (Spell)
            {
                case (SpellType.TimeBomb):
                    //this.RunDestroyAnimation(false);
                    this.IsWeighted = false;
                    this.ChangeSprite("SpellTimeBombExplosion_Sprite");
                    this.PlayAnimation(false);
                    this.IsDying = true;
                    this.CollisionTypeTag = GameTypes.CollisionType_GLOBAL_DAMAGE_WALL;
                    this.AnimationDelay = 1 / 30f;
                    this.Scale = new Vector2(4, 4);
                    Tweener.Tween.RunFunction(0.5f, this, "KillProjectile");
                    (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).ImpactEffectPool.DisplayExplosionEffect(this.Position);
                    break;
                case (SpellType.Nuke):
                    this.RunDestroyAnimation(false);
                    (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).DamageAllEnemies((int)this.Damage);
                    (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).ImpactEffectPool.DisplayDeathEffect(this.Position);
                    break;
                case (SpellType.Displacer):
                    this.RunDestroyAnimation(false);
                    PlayerObj player = (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).Player;
                    //(Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).ImpactEffectPool.StartInverseEmit;
                    player.Translocate(this.Position);
                    break;
                case (SpellType.Boomerang):
                case (SpellType.Bounce):
                    this.CollisionTypeTag = GameTypes.CollisionType_GLOBAL_DAMAGE_WALL;
                    break;
                case (SpellType.Laser):
                    this.CollisionTypeTag = GameTypes.CollisionType_GLOBAL_DAMAGE_WALL;
                    this.LifeSpan = AltY;
                    m_elapsedLifeSpan = 0;
                    this.IsCollidable = true;
                    this.Opacity = 1;
                    break;
            }
        }

        public void KillProjectile()
        {
            IsAlive = false;
            IsDying = false;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                this.Target = null; 
                AttachedIcon = null;
                Source = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new ProjectileObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            ProjectileObj clone = obj as ProjectileObj;
            clone.Target = this.Target;
            clone.CollidesWithTerrain = this.CollidesWithTerrain;
            clone.ChaseTarget = this.ChaseTarget;
            clone.FollowArc = this.FollowArc;
            clone.ShowIcon = this.ShowIcon;
            clone.DestroysWithTerrain = this.DestroysWithTerrain;
            clone.AltX = this.AltX;
            clone.AltY = this.AltY;
            clone.Spell = this.Spell;
            clone.IgnoreBoundsCheck = this.IgnoreBoundsCheck;
            clone.DestroysWithEnemy = this.DestroysWithEnemy;
            clone.DestroyOnRoomTransition = this.DestroyOnRoomTransition;
            clone.Source = this.Source;
            clone.CanBeFusRohDahed = this.CanBeFusRohDahed;
            clone.WrapProjectile = this.WrapProjectile;
            clone.IgnoreInvincibleCounter = this.IgnoreInvincibleCounter;
        }

        public bool IsDemented
        {
            get
            {
                EnemyObj enemy = Source as EnemyObj;
                if (enemy != null && enemy.IsDemented == true)
                    return true;
                return false;
            }
        }
    }
}

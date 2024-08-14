using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ProjectileManager: IDisposable
    {
        private ProceduralLevelScreen m_levelScreen;
        private DS2DPool<ProjectileObj> m_projectilePool;
        private List<ProjectileObj> m_projectilesToRemoveList;
        private int m_poolSize = 0;

        private bool m_isDisposed = false;

        public ProjectileManager(ProceduralLevelScreen level, int poolSize)
        {
            //POOL_SIZE = poolSize;
            //m_projectileStack = new Stack<ProjectileObj>();
            //m_activeProjectilesList = new List<ProjectileObj>();
            m_projectilesToRemoveList = new List<ProjectileObj>();
            //m_numActiveProjectiles = 0;
            m_levelScreen = level;

            m_projectilePool = new DS2DPool<ProjectileObj>();
            m_poolSize = poolSize;
        }

        public void Initialize()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                //m_projectileStack.Push(new ProjectileObj("BoneProjectile") { Visible = false });
                ProjectileObj projectile = new ProjectileObj("BoneProjectile_Sprite");
                projectile.Visible = false;
                projectile.AnimationDelay = 1 / 20f;
                projectile.OutlineWidth = 2;
                //m_levelScreen.PhysicsManager.AddObject(projectile); // Might be better to just add the projectiles to the physics manager as opposed to constantly adding and removing via CheckIn()/CheckOut().
                m_projectilePool.AddToPool(projectile);
            }
        }

        public ProjectileObj FireProjectile(ProjectileData data)
        {
            if (data.Source == null)
                throw new Exception("Cannot have a projectile with no source");

            ProjectileObj projectile = m_projectilePool.CheckOut();
            projectile.Reset(); // Must be called to reset the life span of the projectile as well as revive them. "Dead" projectiles are ones with life spans that have run out.

            projectile.LifeSpan = data.Lifespan;
            GameObj source = data.Source;

            projectile.ChaseTarget = data.ChaseTarget;
            projectile.Source = source;
            projectile.Target = data.Target;
            projectile.UpdateHeading();
            projectile.TurnSpeed = data.TurnSpeed;
            projectile.CollidesWithTerrain = data.CollidesWithTerrain;
            projectile.DestroysWithTerrain = data.DestroysWithTerrain;
            projectile.DestroysWithEnemy = data.DestroysWithEnemy;
            projectile.FollowArc = data.FollowArc;
            projectile.Orientation = MathHelper.ToRadians(data.StartingRotation);
            projectile.ShowIcon = data.ShowIcon;
            projectile.IsCollidable = data.IsCollidable;
            projectile.CollidesWith1Ways = data.CollidesWith1Ways;
            projectile.DestroyOnRoomTransition = data.DestroyOnRoomTransition;
            projectile.CanBeFusRohDahed = data.CanBeFusRohDahed;
            projectile.IgnoreInvincibleCounter = data.IgnoreInvincibleCounter;
            projectile.WrapProjectile = data.WrapProjectile;

            float angle = 0;
            if (data.Target != null)
            {
                float x = data.Target.X - source.X;
                float y = data.Target.Y - source.Y - data.SourceAnchor.Y;
                if (source.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                {
                    angle = 180 - angle;
                    x += data.SourceAnchor.X;
                    angle = MathHelper.ToDegrees((float)Math.Atan2(y, x));

                    angle -= data.AngleOffset;
                }
                else
                {
                    angle = MathHelper.ToDegrees((float)Math.Atan2(y, x));
                    x -= data.SourceAnchor.X;

                    angle += data.AngleOffset;
                }
            }
            else
            {
                angle = data.Angle.X + data.AngleOffset;
                if (data.Angle.X != data.Angle.Y)
                    angle = CDGMath.RandomFloat(data.Angle.X, data.Angle.Y) + data.AngleOffset;

                if (source.Flip != Microsoft.Xna.Framework.Graphics.SpriteEffects.None && source.Rotation != 0)
                    angle -= 180;
                else if (source.Flip != Microsoft.Xna.Framework.Graphics.SpriteEffects.None && source.Rotation == 0)
                    angle = 180 - angle;
            }

            if (data.LockPosition == false)
                projectile.Rotation = angle;
            angle = MathHelper.ToRadians(angle);

            //projectile.Rotation = 0;
            projectile.Damage = data.Damage;
            m_levelScreen.PhysicsManager.AddObject(projectile);  // Might be better to keep them in the physics manager and just turn off their collision detection.
            projectile.ChangeSprite(data.SpriteName);
            projectile.RotationSpeed = data.RotationSpeed;
            projectile.Visible = true;

            //if (data.LockPosition == false)
            {
                if (source.Flip != Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    projectile.X = source.AbsX - data.SourceAnchor.X;
                else
                    projectile.X = source.AbsX + data.SourceAnchor.X;
            }

            //projectile.Flip = source.Flip;

            //if (data.LockPosition == false)
                projectile.Y = source.AbsY + data.SourceAnchor.Y;

            projectile.IsWeighted = data.IsWeighted;
            Vector2 targetPoint = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            float speed = data.Speed.X;
            if (data.Speed.X != data.Speed.Y)
                speed = CDGMath.RandomFloat(data.Speed.X, data.Speed.Y);

            projectile.AccelerationX = targetPoint.X * speed;
            projectile.AccelerationY = targetPoint.Y * speed;
            projectile.CurrentSpeed = speed;

            //if (source.Flip != Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
            //    projectile.AccelerationX = -projectile.AccelerationX;

            if (source is PlayerObj)
            {
                if (projectile.LifeSpan == 0)
                    projectile.LifeSpan = (source as PlayerObj).ProjectileLifeSpan;
                projectile.CollisionTypeTag = GameTypes.CollisionType_PLAYER;
                projectile.Scale = data.Scale;
            }
            else
            {
                if (projectile.LifeSpan == 0)
                    projectile.LifeSpan = 15; // All projectiles are given an arbitrary lifespan of 15 seconds (unless they die earlier than that).
                projectile.CollisionTypeTag = GameTypes.CollisionType_ENEMY;
                projectile.Scale = data.Scale;
            }

            if (data.Target != null && data.Source.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally && data.ChaseTarget == true)
                projectile.Orientation = MathHelper.ToRadians(180);

            if (data.Source is PlayerObj && (Game.PlayerStats.Traits.X == TraitType.Ambilevous ||Game.PlayerStats.Traits.Y == TraitType.Ambilevous))
            {
                projectile.AccelerationX *= -1;
                if (data.LockPosition == false)
                {
                    if (data.Source.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                        projectile.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                    else
                        projectile.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                }
            }

            projectile.PlayAnimation(true);
            return projectile;
        }

        public void DestroyProjectile(ProjectileObj projectile)
        {
            if (m_projectilePool.ActiveObjsList.Contains(projectile)) // Only destroy projectiles if they haven't already been destroyed. It is possible for two objects to call Destroy on the same projectile.
            {
                projectile.CollidesWithTerrain = true;
                projectile.DestroyOnRoomTransition = true;
                projectile.ChaseTarget = false;
                projectile.Target = null;
                projectile.Visible = false;
                projectile.Scale = new Vector2(1, 1);

                projectile.CurrentSpeed = 0;
                projectile.Opacity = 1;
                projectile.IsAlive = false;

                projectile.IsCollidable = true;
                m_levelScreen.PhysicsManager.RemoveObject(projectile); // Might be better to keep them in the physics manager and just turn off their collision detection.
                m_projectilePool.CheckIn(projectile);
            }
        }

        public void DestroyAllProjectiles(bool destroyRoomTransitionProjectiles)
        {
            ProjectileObj[] activeProjectilesArray = m_projectilePool.ActiveObjsList.ToArray();
            foreach (ProjectileObj projectile in activeProjectilesArray)
            {
                if (destroyRoomTransitionProjectiles == true || (destroyRoomTransitionProjectiles == false && projectile.DestroyOnRoomTransition == true))
                    DestroyProjectile(projectile);
            }

            PerformProjectileCleanup();
        }

        public void PauseAllProjectiles(bool pausePlayerProjectiles)
        {
            foreach (ProjectileObj projectile in m_projectilePool.ActiveObjsList)
            {
                if (projectile.CollisionTypeTag != GameTypes.CollisionType_PLAYER || pausePlayerProjectiles == true)
                {
                    projectile.GamePaused = true;
                    projectile.PauseAnimation();
                    if (projectile.Spell != SpellType.DamageShield || (projectile.Spell == SpellType.DamageShield && projectile.CollisionTypeTag == GameTypes.CollisionType_ENEMY))
                    {
                        projectile.AccelerationXEnabled = false;
                        projectile.AccelerationYEnabled = false;
                    }
                }
            }

            Tweener.Tween.PauseAllContaining(typeof(ProjectileObj));
        }

        public void UnpauseAllProjectiles()
        {
            foreach (ProjectileObj projectile in m_projectilePool.ActiveObjsList)
            {
                if (projectile.GamePaused == true)
                {
                    projectile.GamePaused = false;
                    projectile.ResumeAnimation();
                    if (projectile.Spell != SpellType.DamageShield || (projectile.Spell == SpellType.DamageShield && projectile.CollisionTypeTag == GameTypes.CollisionType_ENEMY))
                    {
                        projectile.AccelerationXEnabled = true;
                        projectile.AccelerationYEnabled = true;
                    }
                }
            }
            Tweener.Tween.ResumeAllContaining(typeof(ProjectileObj));
        }

        public void Update(GameTime gameTime)
        {
            RoomObj currentRoom = m_levelScreen.CurrentRoom;

            foreach (ProjectileObj projectile in m_projectilePool.ActiveObjsList)
            {
                if (projectile.WrapProjectile == true)
                {
                    if (projectile.X < currentRoom.X)
                        projectile.X = currentRoom.X + currentRoom.Width;
                    else if (projectile.X > currentRoom.X + currentRoom.Width)
                        projectile.X = currentRoom.X;
                }
                projectile.Update(gameTime);
            }

            // Removes any projectiles that exit the camera logic bounds.
            if (currentRoom != null)
            {
                if (m_projectilesToRemoveList.Count > 0)
                    m_projectilesToRemoveList.Clear();

                //foreach (ProjectileObj projectileToRemove in m_activeProjectilesList)
                //{
                //    if (projectileToRemove.X < m_levelScreen.CurrentRoom.X || projectileToRemove.X > m_levelScreen.CurrentRoom.X + m_levelScreen.CurrentRoom.Width)
                //        m_projectilesToRemoveList.Add(projectileToRemove);
                //    else if (projectileToRemove.Y < m_levelScreen.CurrentRoom.Y || projectileToRemove.Y > m_levelScreen.CurrentRoom.Y + m_levelScreen.CurrentRoom.Height)
                //        m_projectilesToRemoveList.Add(projectileToRemove);
                //}

                foreach (ProjectileObj projectileToRemove in m_projectilePool.ActiveObjsList)
                {
                    if (projectileToRemove.IsAlive == true && projectileToRemove.IsDying == false && projectileToRemove.IgnoreBoundsCheck == false)
                    {
                        // Projectiles cannot exit bounds of room.
                        if (projectileToRemove.Bounds.Left < m_levelScreen.CurrentRoom.Bounds.Left - 200 || projectileToRemove.Bounds.Right > m_levelScreen.CurrentRoom.Bounds.Right + 200)
                            m_projectilesToRemoveList.Add(projectileToRemove);
                        else if (projectileToRemove.Bounds.Bottom > m_levelScreen.CurrentRoom.Bounds.Bottom + 200)
                            m_projectilesToRemoveList.Add(projectileToRemove);

                        /*
                        // Projectiles cannot exit camera bounds.
                        if (projectileToRemove.Bounds.Left < m_levelScreen.Camera.LogicBounds.Left || projectileToRemove.Bounds.Right > m_levelScreen.Camera.LogicBounds.Right)
                            m_projectilesToRemoveList.Add(projectileToRemove);
                        else if (projectileToRemove.Bounds.Top < m_levelScreen.Camera.LogicBounds.Top || projectileToRemove.Bounds.Bottom > m_levelScreen.Camera.LogicBounds.Bottom)
                            m_projectilesToRemoveList.Add(projectileToRemove);
                        */
                    }
                    else if (projectileToRemove.IsAlive == false)
                        m_projectilesToRemoveList.Add(projectileToRemove);
                }

                foreach (ProjectileObj obj in m_projectilesToRemoveList)
                    DestroyProjectile(obj);
            }
        }

        // This function is necessary because cleanup is done during the update call. But sometimes the game chops and the update call is late, causing problems with 
        // the pool determining which projectiles are alive, and which ones are awaiting cleanup. This function forces a cleanup call.
        public void PerformProjectileCleanup()
        {
            if (m_levelScreen.CurrentRoom != null)
            {
                if (m_projectilesToRemoveList.Count > 0)
                    m_projectilesToRemoveList.Clear();

                foreach (ProjectileObj projectileToRemove in m_projectilePool.ActiveObjsList)
                {
                    if (projectileToRemove.IsAlive == true && projectileToRemove.IsDying == false && projectileToRemove.IgnoreBoundsCheck == false)
                    {
                        // Projectiles cannot exit bounds of room.
                        if (projectileToRemove.Bounds.Left < m_levelScreen.CurrentRoom.Bounds.Left - 200 || projectileToRemove.Bounds.Right > m_levelScreen.CurrentRoom.Bounds.Right + 200)
                            m_projectilesToRemoveList.Add(projectileToRemove);
                        else if (projectileToRemove.Bounds.Bottom > m_levelScreen.CurrentRoom.Bounds.Bottom + 200)
                            m_projectilesToRemoveList.Add(projectileToRemove);
                    }
                    else if (projectileToRemove.IsAlive == false)
                        m_projectilesToRemoveList.Add(projectileToRemove);
                }

                foreach (ProjectileObj obj in m_projectilesToRemoveList)
                    DestroyProjectile(obj);
            }
        }

        public void Draw(Camera2D camera)
        {
            foreach (ProjectileObj projectile in m_projectilePool.ActiveObjsList)
            {
                //projectile.DrawOutline(camera, 2);
                projectile.Draw(camera);
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed;}
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Projectile Manager");

                m_levelScreen= null;
                m_projectilePool.Dispose();
                m_projectilePool = null;

                m_projectilesToRemoveList.Clear();
                m_projectilesToRemoveList = null;
                
                m_isDisposed = true;
            }
        }

        public List<ProjectileObj> ActiveProjectileList
        {
            get { return m_projectilePool.ActiveObjsList; }
        }

        public int ActiveProjectiles
        {
            get { return m_projectilePool.NumActiveObjs; }
        }

        public int TotalPoolSize
        {
            get { return m_projectilePool.TotalPoolSize; }
        }

        public int CurrentPoolSize
        {
            get { return TotalPoolSize - ActiveProjectiles; }
        }
    }

    public class ProjectileData : IDisposable
    {
        bool m_isDisposed = false;

        public bool IsWeighted;
        public string SpriteName;
        public GameObj Source;
        public GameObj Target;
        public Vector2 SourceAnchor;
        public float AngleOffset;
        public Vector2 Angle;
        public float RotationSpeed;
        public int Damage;
        public Vector2 Speed;
        public Vector2 Scale = new Vector2(1, 1);
        public bool CollidesWithTerrain = true;
        public bool DestroysWithTerrain = true;
        public float Lifespan = 10;
        public float TurnSpeed = 10;
        public bool ChaseTarget = false;
        public bool FollowArc = false;
        public float StartingRotation;
        public bool ShowIcon = true;
        public bool IsCollidable = true;
        public bool DestroysWithEnemy = true;
        public bool LockPosition = false;
        public bool CollidesWith1Ways = false;
        public bool DestroyOnRoomTransition = true;
        public bool CanBeFusRohDahed = true;
        public bool IgnoreInvincibleCounter = false;
        public bool WrapProjectile = false;

        public ProjectileData(GameObj source)
        {
            if (source == null)
                throw new Exception("Cannot create a projectile without a source");
            Source = source;
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                //Done
                Source = null;
                Target = null;
                m_isDisposed = true;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }

        public ProjectileData Clone()
        {
            return new ProjectileData(this.Source)
            {
                IsWeighted = this.IsWeighted,
                SpriteName = this.SpriteName,
                Source = this.Source,
                Target = this.Target,
                SourceAnchor = this.SourceAnchor,
                AngleOffset = this.AngleOffset,
                Angle = this.Angle,
                RotationSpeed = this.RotationSpeed,
                Damage = this.Damage,
                Speed = this.Speed,
                Scale = this.Scale,
                CollidesWithTerrain = this.CollidesWithTerrain,
                Lifespan = this.Lifespan,
                ChaseTarget = this.ChaseTarget,
                TurnSpeed = this.TurnSpeed,
                FollowArc = this.FollowArc,
                StartingRotation = this.StartingRotation,
                ShowIcon = this.ShowIcon,
                DestroysWithTerrain = this.DestroysWithTerrain,
                IsCollidable = this.IsCollidable,
                DestroysWithEnemy = this.DestroysWithEnemy,
                LockPosition = this.LockPosition,
                CollidesWith1Ways = this.CollidesWith1Ways,
                DestroyOnRoomTransition = this.DestroyOnRoomTransition,
                CanBeFusRohDahed = this.CanBeFusRohDahed,
                IgnoreInvincibleCounter = this.IgnoreInvincibleCounter,
                WrapProjectile = this.WrapProjectile,
            };
        }
    }
}

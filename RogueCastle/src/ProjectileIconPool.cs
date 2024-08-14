using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class ProjectileIconPool : IDisposable
    {
        private bool m_isDisposed = false;
        private DS2DPool<ProjectileIconObj> m_resourcePool;
        private int m_poolSize = 0;
        private ProjectileManager m_projectileManager;
        private RCScreenManager m_screenManager;

        public ProjectileIconPool(int poolSize, ProjectileManager projectileManager, RCScreenManager screenManager)
        {
            m_poolSize = poolSize; // Please keep this pool small
            m_resourcePool = new DS2DPool<ProjectileIconObj>();

            m_projectileManager = projectileManager;
            m_screenManager = screenManager;
        }

        public void Initialize()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                ProjectileIconObj poolObj = new ProjectileIconObj();
                //poolObj.AnimationDelay = 1 / 30f;
                poolObj.Visible = false;
                poolObj.ForceDraw = true;
                poolObj.TextureColor = Color.White;
                m_resourcePool.AddToPool(poolObj);
            }
        }

        public void AddIcon(ProjectileObj projectile)
        {
            ProjectileIconObj icon = m_resourcePool.CheckOut();
            icon.Visible = true;
            icon.ForceDraw = true;
            icon.AttachedProjectile = projectile; // Linking the projectile and the icon.
            projectile.AttachedIcon = icon;
        }


        public void DestroyIcon(ProjectileObj projectile)
        {
            ProjectileIconObj icon = projectile.AttachedIcon;
            icon.Visible = false;
            icon.Rotation = 0;
            icon.TextureColor = Color.White;
            icon.Opacity = 1;
            icon.Flip = SpriteEffects.None;
            icon.Scale = new Vector2(1, 1);
            m_resourcePool.CheckIn(icon);

            //obj.AnimationDelay = 1 / 30f;

            icon.AttachedProjectile = null; // De-linking the projectile and the icon.
            projectile.AttachedIcon = null;
        }

        public void DestroyAllIcons()
        {
            foreach (ProjectileObj projectile in m_projectileManager.ActiveProjectileList)
            {
                if (projectile.AttachedIcon != null)
                    DestroyIcon(projectile);
            }
        }

        public void Update(Camera2D camera)
        {
            PlayerObj player = m_screenManager.Player;

            foreach (ProjectileObj projectile in m_projectileManager.ActiveProjectileList)
            {
                if (projectile.ShowIcon == true) // Make sure to only show icons if this flag is set to true.
                {
                    if (projectile.AttachedIcon == null)
                    {
                        if (CollisionMath.Intersects(projectile.Bounds, camera.Bounds) == false) // Projectile is outside of the camera bounds.
                        {
                            // Using 1 because it needs a margin of error.
                            if ((projectile.AccelerationX > 1 && projectile.X < player.X && (projectile.Y > camera.Bounds.Top && projectile.Y < camera.Bounds.Bottom)) ||
                                (projectile.AccelerationX < -1 && projectile.X > player.X && (projectile.Y > camera.Bounds.Top && projectile.Y < camera.Bounds.Bottom)) ||
                                (projectile.AccelerationY > 1 && projectile.Y < player.Y && (projectile.X > camera.Bounds.Left && projectile.X < camera.Bounds.Right)) ||
                                (projectile.AccelerationY < -1 && projectile.Y > player.Y && (projectile.X > camera.Bounds.Left && projectile.X < camera.Bounds.Right)))
                                AddIcon(projectile);
                        }
                    }
                    else
                    {
                        if (CollisionMath.Intersects(projectile.Bounds, camera.Bounds) == true) // Destroy projectile icons if they get in camera range.
                            DestroyIcon(projectile);
                    }
                }
            }

            // A check to make sure projectiles that die do not have a lingering icon attached to them.
            for (int i = 0; i < m_resourcePool.ActiveObjsList.Count; i++)
            {
                if (m_resourcePool.ActiveObjsList[i].AttachedProjectile.IsAlive == false)
                {
                    DestroyIcon(m_resourcePool.ActiveObjsList[i].AttachedProjectile);
                    i--;
                }
            }

            foreach (ProjectileIconObj projIcon in m_resourcePool.ActiveObjsList)
                projIcon.Update(camera);
        }

        public void Draw(Camera2D camera)
        {
            if (Game.PlayerStats.Traits.X != TraitType.TunnelVision && Game.PlayerStats.Traits.Y != TraitType.TunnelVision)
            {
                foreach (ProjectileIconObj projIcon in m_resourcePool.ActiveObjsList)
                    projIcon.Draw(camera);
            }
        }


        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Projectile Icon Pool");

                m_resourcePool.Dispose();
                m_resourcePool = null;
                m_isDisposed = true;
                m_projectileManager = null;
                m_screenManager = null;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }


        public int ActiveTextObjs
        {
            get { return m_resourcePool.NumActiveObjs; }
        }

        public int TotalPoolSize
        {
            get { return m_resourcePool.TotalPoolSize; }
        }

        public int CurrentPoolSize
        {
            get { return TotalPoolSize - ActiveTextObjs; }
        }
    }
}

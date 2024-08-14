using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ItemDropManager : IDisposableObj
    {
        private DS2DPool<ItemDropObj> m_itemDropPool;
        private List<ItemDropObj> m_itemDropsToRemoveList;
        private int m_poolSize = 0;

        private PhysicsManager m_physicsManager;

        private bool m_isDisposed = false;

        public ItemDropManager(int poolSize, PhysicsManager physicsManager)
        {
            m_itemDropPool = new DS2DPool<ItemDropObj>();
            m_itemDropsToRemoveList = new List<ItemDropObj>();
            m_poolSize = poolSize;
            m_physicsManager = physicsManager;
        }

        public void Initialize()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                ItemDropObj itemDrop = new ItemDropObj("Coin_Sprite");
                itemDrop.Visible = false;
                m_itemDropPool.AddToPool(itemDrop);
            }
        }

        public void DropItem(Vector2 position, int dropType, float amount)
        {
            //ItemDropObj item = m_itemDropPool.CheckOut();
            ItemDropObj item = m_itemDropPool.CheckOutReturnNull();
            if (item == null)
                return;

            item.ConvertDrop(dropType, amount);
            m_physicsManager.AddObject(item);

            item.Position = position;
            item.AccelerationY = CDGMath.RandomFloat(-720, -480);
            item.AccelerationX = CDGMath.RandomFloat(-120, 120);
            item.Visible = true;
            item.IsWeighted = true;
            item.IsCollidable = true;
            item.AnimationDelay = 1 / 20f;
            item.Opacity = 1;
            item.CollectionCounter = 0.2f;

            //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"CoinDrop1", "CoinDrop2", "CoinDrop3", "CoinDrop4", "CoinDrop5");
            SoundManager.Play3DSound(item, Game.ScreenManager.Player,"CoinCollect1", "CoinCollect2", "CoinCollect3");
        }

        // For player only.
        public void DropItemWide(Vector2 position, int dropType, float amount)
        {
            //ItemDropObj item = m_itemDropPool.CheckOut();
            ItemDropObj item = m_itemDropPool.CheckOutReturnNull();
            if (item == null)
                return;

            item.ConvertDrop(dropType, amount);
            m_physicsManager.AddObject(item);

            item.Position = position;
            item.AccelerationY = CDGMath.RandomFloat(-1200, -300);
            item.AccelerationX = CDGMath.RandomFloat(-600, 600);
            item.Visible = true;
            item.IsWeighted = true;
            item.IsCollidable = true;
            item.AnimationDelay = 1 / 20f;
            item.Opacity = 1;
            item.CollectionCounter = 0.2f;

            //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"CoinDrop1", "CoinDrop2", "CoinDrop3", "CoinDrop4", "CoinDrop5");
            //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"CoinCollect1", "CoinCollect2", "CoinCollect3");
        }

        public void DestroyItemDrop(ItemDropObj itemDrop)
        {
            if (m_itemDropPool.ActiveObjsList.Contains(itemDrop)) // Only destroy items if they haven't already been destroyed. It is possible for two objects to call Destroy on the same projectile.
            {
                itemDrop.Visible = false;
                itemDrop.TextureColor = Color.White;
                m_physicsManager.RemoveObject(itemDrop); // Might be better to keep them in the physics manager and just turn off their collision detection.
                m_itemDropPool.CheckIn(itemDrop);
            }
        }

        public void DestroyAllItemDrops()
        {
            ItemDropObj[] itemDropArray = m_itemDropPool.ActiveObjsList.ToArray();
            foreach (ItemDropObj item in itemDropArray)
            {
                DestroyItemDrop(item);
            }
        }

        public void PauseAllAnimations()
        {
            foreach (ItemDropObj item in m_itemDropPool.ActiveObjsList)
                item.PauseAnimation();
        }

        public void ResumeAllAnimations()
        {
            foreach (ItemDropObj item in m_itemDropPool.ActiveObjsList)
                item.ResumeAnimation();
        }

        public void Draw(Camera2D camera)
        {
            foreach (ItemDropObj item in m_itemDropPool.ActiveObjsList)
                item.Draw(camera);
        }

        public int AvailableItems
        {
            get { return m_itemDropPool.CurrentPoolSize; }
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Item Drop Manager");

                DestroyAllItemDrops();
                m_itemDropsToRemoveList.Clear();
                m_itemDropsToRemoveList = null;
                m_itemDropPool.Dispose();
                m_itemDropPool = null;
                m_physicsManager = null; // This must be set null last, since all item drops need to be removed from the physics manager first.

                m_isDisposed = true;
            }
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}

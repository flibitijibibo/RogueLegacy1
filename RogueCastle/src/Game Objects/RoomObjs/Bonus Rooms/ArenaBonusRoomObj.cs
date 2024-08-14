using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;

namespace RogueCastle
{
    public class ArenaBonusRoom : BonusRoomObj
    {
        private ChestObj m_chest;
        private float m_chestStartingY;
        private bool m_chestRevealed = false;

        public override void Initialize()
        {
            foreach (GameObj obj in GameObjList)
            {
                if (obj is ChestObj)
                {
                    m_chest = obj as ChestObj;
                    break;
                }
            }

            m_chest.ChestType = ChestType.Gold;
            m_chestStartingY = m_chest.Y - 200 + m_chest.Height + 6;

            base.Initialize();
        }

        public override void OnEnter()
        {
            UpdateEnemyNames();

            m_chest.Y = m_chestStartingY;
            m_chest.ChestType = ChestType.Gold;

            if (RoomCompleted == true)
            {
                m_chest.Opacity = 1;
                m_chest.Y = m_chestStartingY + 200;
                m_chest.IsEmpty = true;
                m_chest.ForceOpen();
                m_chestRevealed = true;

                foreach (EnemyObj enemy in EnemyList)
                {
                    if (enemy.IsKilled == false)
                        enemy.KillSilently();
                }
            }
            else
            {
                if (ActiveEnemies == 0)
                {
                    m_chest.Opacity = 1;
                    m_chest.Y = m_chestStartingY + 200;
                    m_chest.IsEmpty = false;
                    m_chest.IsLocked = false;
                    m_chestRevealed = true;
                }
                else
                {
                    m_chest.Opacity = 0;
                    m_chest.Y = m_chestStartingY;
                    m_chest.IsLocked = true;
                    m_chestRevealed = false;
                }
            }

            if (m_chest.PhysicsMngr == null)
                Player.PhysicsMngr.AddObject(m_chest);

            base.OnEnter();
        }

        private void UpdateEnemyNames()
        {
            bool firstNamed = false;
            foreach (EnemyObj enemy in this.EnemyList)
            {
                if (enemy is EnemyObj_EarthWizard)
                {
                    if (firstNamed == false)
                    {
                        enemy.Name = "Barbatos";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_106";
                        firstNamed = true;
                    }
                    else
                    {
                        enemy.Name = "Amon";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_109";
                    }
                }
                else if (enemy is EnemyObj_Skeleton)
                {
                    if (firstNamed == false)
                    {
                        enemy.Name = "Berith";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_108";
                        firstNamed = true;
                    }
                    else
                    {
                        enemy.Name = "Halphas";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_111";
                    }
                }
                else if (enemy is EnemyObj_Plant)
                {
                    if (firstNamed == false)
                    {
                        enemy.Name = "Stolas";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_107";
                        firstNamed = true;
                    }
                    else
                    {
                        enemy.Name = "Focalor";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_110";
                    }
                }
            }

        }

        public override void Update(GameTime gameTime)
        {
            if (m_chest.IsOpen == false)
            {
                if (ActiveEnemies == 0 && m_chest.Opacity == 0 && m_chestRevealed == false)
                {
                    m_chestRevealed = true;
                    DisplayChest();
                }
            }
            else
            {
                if (RoomCompleted == false)
                    RoomCompleted = true;
            }

            base.Update(gameTime);
        }

        public override void OnExit()
        {
            bool skeletonMBKilled = false;
            bool plantMBKilled = false;
            bool paintingMBKilled = false;
            bool knightMBKilled = false;
            bool wizardMBKilled = false;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Skeleton].W > 0)
                skeletonMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Plant].W > 0)
                plantMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Portrait].W > 0)
                paintingMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.Knight].W > 0)
                knightMBKilled = true;
            if (Game.PlayerStats.EnemiesKilledList[EnemyType.EarthWizard].W > 0)
                wizardMBKilled = true;

            if (skeletonMBKilled && plantMBKilled && paintingMBKilled && knightMBKilled && wizardMBKilled)
                GameUtil.UnlockAchievement("FEAR_OF_ANIMALS");

            base.OnExit();
        }

        private void DisplayChest()
        {
            m_chest.IsLocked = false;
            Tween.To(m_chest, 2, Tween.EaseNone, "Opacity", "1");
            Tween.By(m_chest, 2, Tweener.Ease.Quad.EaseOut, "Y", "200");
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_chest = null;
                base.Dispose();
            }
        }
    }
}

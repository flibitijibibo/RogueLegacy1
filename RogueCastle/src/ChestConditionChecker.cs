using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class ChestConditionChecker
    {
        public const byte STATE_LOCKED = 0;
        public const byte STATE_FREE = 1;
        public const byte STATE_FAILED = 2;

        public static float HelperFloat = 0;

        public static void SetConditionState(FairyChestObj chest, PlayerObj player)
        {
            int distanceCheck = 100;
            switch (chest.ConditionType)
            {
                case (ChestConditionType.InvisibleChest):
                case (ChestConditionType.None):
                    if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck)
                        chest.SetChestUnlocked();
                    break;
                case (ChestConditionType.KillAllEnemies):
                    if (player.AttachedLevel.CurrentRoom.ActiveEnemies <= 0)
                        chest.SetChestUnlocked();
                    break;
                case (ChestConditionType.DontLook):
                    SpriteEffects flipCheck = SpriteEffects.None;
                    if (chest.AbsPosition.X < player.AbsPosition.X) // Chest is to the left of the player.
                        flipCheck = SpriteEffects.FlipHorizontally;
                    if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < 375 && Vector2.Distance(chest.AbsPosition, player.AbsPosition) > distanceCheck && player.Flip == flipCheck)
                        chest.SetChestFailed();
                    else if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck)
                        chest.SetChestUnlocked();
                    break;
                case (ChestConditionType.NoJumping):
                    if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < 10000 && player.IsJumping == true && player.AccelerationY < 0 && Vector2.Distance(chest.AbsPosition, player.AbsPosition) > distanceCheck)
                        chest.SetChestFailed();
                    else if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck)
                        chest.SetChestUnlocked();
                    break;
                case (ChestConditionType.NoFloor): // This one isn't being used.
                    if (player.IsTouchingGround == true && Vector2.Distance(chest.AbsPosition, player.AbsPosition) > distanceCheck && Vector2.Distance(chest.AbsPosition, player.AbsPosition) < 1000)
                        chest.SetChestFailed();
                    else if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck)
                        chest.SetChestUnlocked();
                    break;
                case (ChestConditionType.NoAttackingEnemies):
                    foreach (EnemyObj enemy in player.AttachedLevel.CurrentRoom.EnemyList)
                    {
                        if (enemy.CurrentHealth < enemy.MaxHealth)
                        {
                            chest.SetChestFailed();
                            break;
                        }
                    }
                    if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck && chest.State == ChestConditionChecker.STATE_LOCKED)
                        chest.SetChestUnlocked();
                    break;
                case (ChestConditionType.ReachIn5Seconds):
                    if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) > distanceCheck && chest.Timer <= 0)
                        chest.SetChestFailed();
                    else if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck && chest.Timer > 0)
                        chest.SetChestUnlocked();
                    break;
                case(ChestConditionType.TakeNoDamage):
                    if (player.State == PlayerObj.STATE_HURT)
                        chest.SetChestFailed();
                    else if (Vector2.Distance(chest.AbsPosition, player.AbsPosition) < distanceCheck)
                        chest.SetChestUnlocked();
                    break;
            }
        }
    }
}

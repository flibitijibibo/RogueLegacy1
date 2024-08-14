using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Tweener.Ease;
using Tweener;
using InputSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class CompassRoomObj : RoomObj
    {
        public CompassRoomObj()
        {
        }

        public override void OnEnter()
        {
            if (Game.PlayerStats.SpecialItem == SpecialItemType.Compass)
            {
                Game.PlayerStats.SpecialItem = SpecialItemType.None;
                Player.AttachedLevel.UpdatePlayerHUDSpecialItem();
            }

            SoundManager.StopMusic(2);
            Player.UnlockControls();
            foreach (GameObj obj in GameObjList)
            {
                ChestObj chest = obj as ChestObj;
                if (chest != null)
                {
                    chest.ResetChest();
                    chest.ChestType = ChestType.Gold;
                }
            }
            base.OnEnter();
        }

        public override void OnExit()
        {
            Player.AttachedLevel.RemoveCompassDoor();
            Player.UnlockControls();
            Player.KickInHitInvincibility();
            base.OnExit();
        }

        protected override GameObj CreateCloneInstance()
        {
            return new CompassRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }
    }
}

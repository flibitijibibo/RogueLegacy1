using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tweener.Ease;
using Tweener;

namespace RogueCastle
{
    public class ChestBonusRoomObj : BonusRoomObj
    {
        private const int TOTAL_UNLOCKED_CHESTS = 1;

        private bool m_paid = false;
        private List<ChestObj> m_chestList;

        private NpcObj m_elf;
        private PhysicsObj m_gate;

        public ChestBonusRoomObj()
        {
            m_chestList = new List<ChestObj>();
            m_elf = new NpcObj("Elf_Character");
            m_elf.Scale = new Vector2(2, 2);

            m_gate = new PhysicsObj("CastleEntranceGate_Sprite");
            m_gate.IsWeighted = false;
            m_gate.IsCollidable = true;
            m_gate.CollisionTypeTag = GameTypes.CollisionType_WALL;
            m_gate.Layer = -1;
        }

        public override void Initialize()
        {
            Vector2 gatePosition = Vector2.Zero;
            Vector2 elfPosition = Vector2.Zero;

            foreach (GameObj obj in GameObjList)
            {
                if (obj is WaypointObj)
                    elfPosition.X = obj.X;
            }

            foreach (TerrainObj terrain in TerrainObjList)
            {
                if (terrain.Name == "GatePosition")
                    gatePosition = new Vector2(terrain.X, terrain.Bounds.Bottom);

                if (terrain.Name == "Floor")
                    elfPosition.Y = terrain.Y;
            }

            m_gate.Position = new Vector2(gatePosition.X, gatePosition.Y);

            if (IsReversed == false)
                m_elf.Flip = SpriteEffects.FlipHorizontally;
            m_elf.Position = new Vector2(elfPosition.X, elfPosition.Y - (m_elf.Bounds.Bottom - m_elf.AnchorY) - 2);

            GameObjList.Add(m_elf);
            GameObjList.Add(m_gate);

            base.Initialize();
        }

        public override void OnEnter()
        {
            //if (this.ID != 1 && this.ID != 2 && this.ID != 3)
              //  ID = CDGMath.RandomInt(1, 3);
            ID = 1;

            foreach (GameObj obj in GameObjList)
            {
                ChestObj chest = obj as ChestObj;
                if (chest != null)
                {
                    chest.ChestType = ChestType.Silver;
                    //if (ID == 1)
                    //    chest.ChestType = ChestType.Brown;
                    //else if (ID == 2)
                    //    chest.ChestType = ChestType.Silver;
                    //else
                    //    chest.ChestType = ChestType.Gold;
                    chest.IsEmpty = true;
                    chest.IsLocked = true;
                }
            }

            (m_elf.GetChildAt(2) as SpriteObj).StopAnimation();
            base.OnEnter();
        }

        private void ShuffleChests(int goldPaid)
        {
            int[] chestRand = new int[] { 1, 2, 3};
            CDGMath.Shuffle<int>(chestRand);

            int chestCounter = 0;
            foreach (GameObj obj in GameObjList)
            {
                ChestObj chest = obj as ChestObj;
                if (chest != null)
                {
                    chest.ForcedItemType = ItemDropType.Coin;
                    int chestReward = chestRand[chestCounter];
                    if (chestReward == 1)
                        chest.IsEmpty = true;
                    else if (chestReward == 2)
                        chest.IsEmpty = true;
                    else
                    {
                        chest.IsEmpty = false;
                        chest.ForcedAmount = goldPaid * 3f;
                    }
                    chestCounter++;
                    m_chestList.Add(chest);
                    chest.IsLocked = false;

                    chest.TextureColor = Color.White;
                    if (chestReward == 3 && Game.PlayerStats.SpecialItem == SpecialItemType.Glasses)
                        chest.TextureColor = Color.Gold;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            m_elf.Update(gameTime, Player);

            if (RoomCompleted == false)
            {
                if (m_paid == true)
                {
                    if (this.IsReversed == false && Player.X < (this.X + 50))
                        Player.X = this.X + 50;
                    else if (this.IsReversed == true && Player.X > (this.X + this.Width - 50))
                        Player.X = this.X + this.Width - 50;
                }

                if (NumberOfChestsOpen >= TOTAL_UNLOCKED_CHESTS)
                {
                    bool lostGame = false;
                    foreach (ChestObj chest in m_chestList)
                    {
                        if (chest.IsEmpty == true && chest.IsOpen == true)
                            lostGame = true;
                        chest.IsLocked = true;
                    }

                    RoomCompleted = true;

                    RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                    if (lostGame == false)
                        manager.DialogueScreen.SetDialogue("ChestBonusRoom1-Won");
                    else
                        manager.DialogueScreen.SetDialogue("ChestBonusRoom1-Lost");
                    Game.ScreenManager.DisplayScreen(ScreenType.Dialogue, true);
                }
            }

            HandleInput();

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            if (m_elf.IsTouching == true)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;

                    if (RoomCompleted == false)
                    {
                        if (m_paid == false)
                        {
                            manager.DialogueScreen.SetDialogue("ChestBonusRoom" + this.ID + "-Start");
                            manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                            manager.DialogueScreen.SetConfirmEndHandler(this, "PlayChestGame");
                            manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                        }
                        else
                            manager.DialogueScreen.SetDialogue("ChestBonusRoom1-Choose");
                    }
                    else
                        manager.DialogueScreen.SetDialogue("ChestBonusRoom1-End");

                    Game.ScreenManager.DisplayScreen(ScreenType.Dialogue, true);
                }
            }
        }

        public void PlayChestGame()
        {
            if (Game.PlayerStats.Gold >= 4)
            {
                m_paid = true;
                float percent = 1;
                if (ID == 1)
                    percent = 0.25f;
                else if (ID == 2)
                    percent = 0.5f;
                else
                    percent = 0.75f;
                int goldPaid = (int)(Game.PlayerStats.Gold * percent);
                Game.PlayerStats.Gold -= goldPaid;
                ShuffleChests(goldPaid);
                Player.AttachedLevel.TextManager.DisplayNumberStringText(-(int)goldPaid, "LOC_ID_CARNIVAL_BONUS_ROOM_4" /*"gold"*/, Color.Yellow, new Vector2(Player.X, Player.Bounds.Top));
                Tween.By(m_gate, 1, Quad.EaseInOut, "Y", (-m_gate.Height).ToString());
            }
            else
            {
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DialogueScreen.SetDialogue("ChestBonusRoom1-NoMoney");
                Tween.To(this, 0, Linear.EaseNone);
                Tween.AddEndHandlerToLastTween(Player.AttachedLevel.ScreenManager, "DisplayScreen", ScreenType.Dialogue, true, typeof(List<object>));
            }
        }

        public override void Reset()
        {
            foreach (ChestObj chest in m_chestList)
                chest.ResetChest();
            if (m_paid == true)
            {
                m_gate.Y += m_gate.Height;
                m_paid = false;
            }
            base.Reset();
        }

        protected override GameObj CreateCloneInstance()
        {
            return new ChestBonusRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_elf = null;
                m_gate = null;
                m_chestList.Clear();
                m_chestList = null;

                base.Dispose();
            }
        }

        private int NumberOfChestsOpen
        {
            get
            {
                int numChestsOpen = 0;
                foreach (ChestObj chest in m_chestList)
                {
                    if (chest.IsOpen == true)
                        numChestsOpen++;
                }
                return numChestsOpen;
            }
        }
    }
}

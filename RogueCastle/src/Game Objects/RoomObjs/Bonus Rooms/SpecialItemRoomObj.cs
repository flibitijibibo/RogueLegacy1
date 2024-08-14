using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class SpecialItemRoomObj : BonusRoomObj
    {
        public byte ItemType { get; set; }
        private SpriteObj m_pedestal;
        private SpriteObj m_icon;
        private float m_iconYPos;

        private SpriteObj m_speechBubble;

        public SpecialItemRoomObj()
        {
        }

        public override void Initialize()
        {
            m_speechBubble = new SpriteObj("UpArrowSquare_Sprite");
            m_speechBubble.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;

            m_icon = new SpriteObj("Blank_Sprite");
            m_icon.Scale = new Vector2(2, 2);

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "pedestal")
                {
                    m_pedestal = obj as SpriteObj;
                    break;
                }
            }

            m_pedestal.OutlineWidth = 2;

            m_icon.X = m_pedestal.X;
            m_icon.Y = m_pedestal.Y - (m_pedestal.Y - m_pedestal.Bounds.Top) - m_icon.Height / 2f - 10;
            m_icon.OutlineWidth = 2;

            m_iconYPos = m_icon.Y;
            GameObjList.Add(m_icon);

            m_speechBubble.Y = m_icon.Y - 30;
            m_speechBubble.X = m_icon.X;
            m_speechBubble.Visible = false;
            GameObjList.Add(m_speechBubble);

            base.Initialize();
        }

        private void RandomizeItem()
        {
            if (Game.PlayerStats.Traits.X == TraitType.NearSighted || Game.PlayerStats.Traits.Y == TraitType.NearSighted ||
                Game.PlayerStats.Traits.X == TraitType.FarSighted || Game.PlayerStats.Traits.Y == TraitType.FarSighted ||
                 Game.PlayerStats.Traits.X == TraitType.Vertigo || Game.PlayerStats.Traits.Y == TraitType.Vertigo ||
                 Game.PlayerStats.Traits.X == TraitType.ColorBlind || Game.PlayerStats.Traits.Y == TraitType.ColorBlind ||
                 Game.PlayerStats.Traits.X == TraitType.Nostalgic || Game.PlayerStats.Traits.Y == TraitType.Nostalgic)
            {
                if (CDGMath.RandomInt(1, 100) <= 30) // 30% chance of getting glasses.
                    ItemType = SpecialItemType.Glasses;
                else
                    ItemType = GetRandomItem();
                    //ItemType = (byte)(CDGMath.RandomInt(1, SpecialItemType.Total - 1));
            }
            else
                ItemType = GetRandomItem();
                //ItemType = (byte)(CDGMath.RandomInt(1, SpecialItemType.Total - 1));
            m_icon.ChangeSprite(SpecialItemType.SpriteName(ItemType));
            ID = ItemType;
            //RoomCompleted = true;
        }

        private byte GetRandomItem()
        {
            List<byte> possibleItems = new List<byte>();

            for (int i = 1; i < SpecialItemType.Total; i++)
                possibleItems.Add((byte)i);

            if ((Game.PlayerStats.EyeballBossBeaten == true && Game.PlayerStats.TimesCastleBeaten > 0) && Game.PlayerStats.ChallengeEyeballUnlocked == false && Game.PlayerStats.ChallengeEyeballBeaten == false)
            {
                possibleItems.Add(SpecialItemType.EyeballToken);
                possibleItems.Add(SpecialItemType.EyeballToken);
            }
            if ((Game.PlayerStats.FairyBossBeaten == true && Game.PlayerStats.TimesCastleBeaten > 0) && Game.PlayerStats.ChallengeSkullUnlocked == false && Game.PlayerStats.ChallengeSkullBeaten == false)
            {
                possibleItems.Add(SpecialItemType.SkullToken);
                possibleItems.Add(SpecialItemType.SkullToken);
            }
            if ((Game.PlayerStats.FireballBossBeaten == true && Game.PlayerStats.TimesCastleBeaten > 0) && Game.PlayerStats.ChallengeFireballUnlocked == false && Game.PlayerStats.ChallengeFireballBeaten == false)
            {
                possibleItems.Add(SpecialItemType.FireballToken);
                possibleItems.Add(SpecialItemType.FireballToken);
            }
            if ((Game.PlayerStats.BlobBossBeaten == true && Game.PlayerStats.TimesCastleBeaten > 0) && Game.PlayerStats.ChallengeBlobUnlocked == false && Game.PlayerStats.ChallengeBlobBeaten == false)
            {
                possibleItems.Add(SpecialItemType.BlobToken);
                possibleItems.Add(SpecialItemType.BlobToken);
                possibleItems.Add(SpecialItemType.BlobToken);
            }

            if (Game.PlayerStats.ChallengeLastBossUnlocked == false && Game.PlayerStats.ChallengeLastBossBeaten == false
                && Game.PlayerStats.ChallengeEyeballBeaten == true
                && Game.PlayerStats.ChallengeSkullBeaten == true
                && Game.PlayerStats.ChallengeFireballBeaten == true
                && Game.PlayerStats.ChallengeBlobBeaten == true && Game.PlayerStats.TimesCastleBeaten > 0)
            {
                possibleItems.Add(SpecialItemType.LastBossToken);
                possibleItems.Add(SpecialItemType.LastBossToken);
                possibleItems.Add(SpecialItemType.LastBossToken);
                possibleItems.Add(SpecialItemType.LastBossToken);
                possibleItems.Add(SpecialItemType.LastBossToken);
            }

            return possibleItems[CDGMath.RandomInt(0, possibleItems.Count - 1)];
        }

        public override void OnEnter()
        {
            m_icon.Visible = false;

            //if (RoomCompleted == false) // Make sure not to randomize the item if it's already been randomized once.
            if (ID == -1 && RoomCompleted == false)
            {
                do
                {
                    RandomizeItem();
                } while (ItemType == Game.PlayerStats.SpecialItem); // Make sure the room doesn't randomize to the item the player already has.
            }
            else if (ID != -1)
            {
                ItemType = (byte)ID;
                m_icon.ChangeSprite(SpecialItemType.SpriteName(ItemType));

                if (RoomCompleted == true)
                {
                    m_icon.Visible = false;
                    m_speechBubble.Visible = false;
                }
            }

            if (RoomCompleted == true)
                m_pedestal.TextureColor = new Color(100, 100, 100);
            else
                m_pedestal.TextureColor = Color.White;

            base.OnEnter();
        }

        public override void Update(GameTime gameTime)
        {
            m_icon.Y = m_iconYPos + (float)Math.Sin(Game.TotalGameTime * 2) * 5;
            m_speechBubble.Y = m_iconYPos - 30 + (float)Math.Sin(Game.TotalGameTime * 20) * 2;

            //if (ItemType != 0)
            if (RoomCompleted == false)
            {
                //m_icon.Visible = true;

                if (CollisionMath.Intersects(Player.Bounds, m_pedestal.Bounds))
                {
                    m_speechBubble.Visible = true;
                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    {
                        RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                        manager.DialogueScreen.SetDialogue("Special Item Prayer");
                        manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                        manager.DialogueScreen.SetConfirmEndHandler(this, "TakeItem");
                        manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                        manager.DisplayScreen(ScreenType.Dialogue, true);
                    }
                }
                else
                    m_speechBubble.Visible = false;
            }
            else
                m_icon.Visible = false;

            base.Update(gameTime);
        }

        public void TakeItem()
        {
            RoomCompleted = true;
            Game.PlayerStats.SpecialItem = ItemType;
            m_pedestal.TextureColor = new Color(100, 100, 100);

            if (ItemType == SpecialItemType.Glasses)
            {
                Player.GetChildAt(10).Visible = true;
                Player.PlayAnimation(true);
            }
            else
                Player.GetChildAt(10).Visible = false;

            ItemType = 0;
            m_speechBubble.Visible = false;
            m_icon.Visible = false;
            (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).UpdatePlayerHUDSpecialItem();

            (Game.ScreenManager.CurrentScreen as ProceduralLevelScreen).UpdatePlayerSpellIcon();
            List<object> objectList = new List<object>();
            objectList.Add(new Vector2(m_pedestal.X, m_pedestal.Y - m_pedestal.Height / 2f));
            objectList.Add(GetItemType.SpecialItem);
            objectList.Add(new Vector2(Game.PlayerStats.SpecialItem, 0));

            // Saves the map and player Data the moment you take an item, to ensure you don't exploit.
            (Player.AttachedLevel.ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.MapData);

            (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
            Tweener.Tween.RunFunction(0, Player, "RunGetItemAnimation"); // Necessary to delay this call by one update.
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_pedestal = null;
                m_icon = null;
                m_speechBubble = null;
                base.Dispose();
            }
        }

    }
}

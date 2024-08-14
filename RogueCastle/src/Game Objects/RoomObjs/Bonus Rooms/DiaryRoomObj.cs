using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;

namespace RogueCastle
{
    public class DiaryRoomObj : BonusRoomObj
    {
        private SpriteObj m_speechBubble;
        private SpriteObj m_diary;
        private int m_diaryIndex = 0;

        public override void Initialize()
        {
            m_speechBubble = new SpriteObj("UpArrowSquare_Sprite");
            m_speechBubble.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            GameObjList.Add(m_speechBubble);

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "diary")
                {
                    m_diary = obj as SpriteObj;
                    break;
                }
            }

            m_diary.OutlineWidth = 2;
            m_speechBubble.Position = new Vector2(m_diary.X, m_diary.Y - m_speechBubble.Height - 20);

            base.Initialize();
        }

        public override void OnEnter()
        {
            if (RoomCompleted == false)
                m_speechBubble.Visible = true;
            else
                m_speechBubble.Visible = false;

            if (RoomCompleted == false)
                m_diaryIndex = Game.PlayerStats.DiaryEntry;

            if (m_diaryIndex >= LevelEV.TOTAL_JOURNAL_ENTRIES - 1)
                m_speechBubble.Visible = false;

            base.OnEnter();
        }

        public override void Update(GameTime gameTime)
        {
            m_speechBubble.Y = m_diary.Y - m_speechBubble.Height - 20 - 30 + (float)Math.Sin(Game.TotalGameTime * 20) * 2;

            Rectangle diaryBound = m_diary.Bounds;
            diaryBound.X -= 50;
            diaryBound.Width += 100;

            if (CollisionMath.Intersects(Player.Bounds, diaryBound) == false && m_speechBubble.SpriteName == "UpArrowSquare_Sprite")
                m_speechBubble.ChangeSprite("ExclamationSquare_Sprite");

            if (RoomCompleted == false || CollisionMath.Intersects(Player.Bounds, diaryBound) == true)
                m_speechBubble.Visible = true;
            else if (RoomCompleted == true && CollisionMath.Intersects(Player.Bounds, diaryBound) == false)
                m_speechBubble.Visible = false;

            if (m_diaryIndex >= LevelEV.TOTAL_JOURNAL_ENTRIES - 1)
                m_speechBubble.Visible = false;

            if (CollisionMath.Intersects(Player.Bounds, diaryBound) && Player.IsTouchingGround == true)
            {
                if (m_speechBubble.SpriteName == "ExclamationSquare_Sprite")
                    m_speechBubble.ChangeSprite("UpArrowSquare_Sprite");

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    if (RoomCompleted == false && Game.PlayerStats.DiaryEntry < LevelEV.TOTAL_JOURNAL_ENTRIES - 1)
                    {
                        RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                        manager.DialogueScreen.SetDialogue("DiaryEntry" + m_diaryIndex);
                        manager.DisplayScreen(ScreenType.Dialogue, true, null);

                        Game.PlayerStats.DiaryEntry++;
                        RoomCompleted = true;
                    }
                    else
                    {
                        RoomCompleted = true;
                        RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                        manager.DisplayScreen(ScreenType.DiaryEntry, true);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_diary = null;
                m_speechBubble = null;
                base.Dispose();
            }
        }
    }
}

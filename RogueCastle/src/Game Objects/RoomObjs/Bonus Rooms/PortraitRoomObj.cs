using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class PortraitRoomObj : BonusRoomObj
    {
        private SpriteObj m_portraitFrame;
        private int m_portraitIndex = 0;
        private SpriteObj m_portrait;

        public override void Initialize()
        {
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "portrait")
                {
                    m_portraitFrame = obj as SpriteObj;
                    break;
                }
            }

            m_portraitFrame.ChangeSprite("GiantPortrait_Sprite");
            m_portraitFrame.Scale = new Vector2(2,2);
            m_portrait = new SpriteObj("Blank_Sprite");
            m_portrait.Position = m_portraitFrame.Position;
            m_portrait.Scale = new Vector2(0.95f, 0.95f);
            GameObjList.Add(m_portrait);

            base.Initialize();
        }

        public override void OnEnter()
        {
            if (RoomCompleted == false && ID == -1)
            {
                //RoomCompleted = true;
                m_portraitIndex = CDGMath.RandomInt(0, 8); // m_portraitIndex = 0; // override to test different portraits
                m_portrait.ChangeSprite("Portrait" + m_portraitIndex + "_Sprite");
                ID = m_portraitIndex;
                base.OnEnter();
            }
            else if (ID != -1)
            {
                m_portraitIndex = ID;
                m_portrait.ChangeSprite("Portrait" + m_portraitIndex + "_Sprite");
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
            {
                Rectangle collideRect = new Rectangle(this.Bounds.Center.X - 100, this.Bounds.Bottom - 300, 200, 200);
                if (CollisionMath.Intersects(Player.Bounds, collideRect) && Player.IsTouchingGround == true && ID > -1)
                {
                    RCScreenManager manager = Game.ScreenManager;
                    manager.DialogueScreen.SetDialogue("PortraitRoomText" + ID);
                    manager.DisplayScreen(ScreenType.Dialogue, true);
                }
            }

            base.Update(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_portraitFrame = null;
                m_portrait = null;
                base.Dispose();
            }
        }
    }
}

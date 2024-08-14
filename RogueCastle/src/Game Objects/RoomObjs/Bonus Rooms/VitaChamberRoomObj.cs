using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class VitaChamberRoomObj : BonusRoomObj
    {
        private GameObj m_fountain;
        private SpriteObj m_speechBubble;

        public override void Initialize()
        {
            m_speechBubble = new SpriteObj("UpArrowSquare_Sprite");
            m_speechBubble.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            GameObjList.Add(m_speechBubble);

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "fountain")
                {
                    m_fountain = obj;
                    break;
                }
            }

            (m_fountain as SpriteObj).OutlineWidth = 2;
            m_speechBubble.X = m_fountain.X;
            base.Initialize();
        }

        public override void OnEnter()
        {
            if (RoomCompleted == true)
            {
                m_speechBubble.Visible = false;
                m_fountain.TextureColor = new Color(100, 100, 100);
            }
            else
                m_fountain.TextureColor = Color.White;
            base.OnEnter();
        }

        public override void Update(GameTime gameTime)
        {
            if (RoomCompleted == false)
            {
                Rectangle bounds = m_fountain.Bounds;
                bounds.X -= 50;
                bounds.Width += 100;
                if (CollisionMath.Intersects(Player.Bounds, bounds) && Player.IsTouchingGround == true)
                {
                    m_speechBubble.Y = m_fountain.Y - 150 + (float)Math.Sin(Game.TotalGameTime * 20) * 2;
                    m_speechBubble.Visible = true;
                }
                else
                    m_speechBubble.Visible = false;

                if (m_speechBubble.Visible == true)
                {
                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    {
                        int healthGain = (int)(Player.MaxHealth * GameEV.ROOM_VITA_CHAMBER_REGEN_AMOUNT);
                        int manaGain = (int)(Player.MaxMana * GameEV.ROOM_VITA_CHAMBER_REGEN_AMOUNT);
                        Player.CurrentHealth += healthGain;
                        Player.CurrentMana += manaGain;
                        Console.WriteLine("Healed");
                        SoundManager.PlaySound("Collect_Mana");
                        Player.AttachedLevel.TextManager.DisplayNumberStringText(healthGain, "LOC_ID_ITEM_DROP_OBJ_2" /*"hp recovered"*/, Color.LawnGreen, new Vector2(Player.X, Player.Bounds.Top - 30));
                        Player.AttachedLevel.TextManager.DisplayNumberStringText(manaGain, "LOC_ID_ITEM_DROP_OBJ_3" /*"mp recovered"*/, Color.CornflowerBlue, new Vector2(Player.X, Player.Bounds.Top));
                        //Tweener.Tween.RunFunction(0.2f, Player.AttachedLevel.TextManager, "DisplayNumberStringText", manaGain, "mp recovered", Color.LawnGreen, new Vector2(Player.X, Player.Bounds.Top - 50));
                        RoomCompleted = true;
                        m_fountain.TextureColor = new Color(100, 100, 100);
                        m_speechBubble.Visible = false;
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
                m_fountain = null;
                m_speechBubble = null;
                base.Dispose();
            }
        }
    }
}

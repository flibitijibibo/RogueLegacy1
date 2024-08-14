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
    public class ThroneRoomObj : RoomObj
    {
        private KeyIconTextObj m_tutorialText;
        private bool m_displayText = false;

        private KingObj m_king;
        private bool m_kingKilled = false;

        public ThroneRoomObj()
        {
        }

        public override void Initialize()
        {
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "fountain")
                {
                    (obj as ObjContainer).OutlineWidth = 2;
                    obj.Y -= 2;
                    break;
                }
            }

            foreach (DoorObj door in DoorList)
                door.Locked = true;
            base.Initialize();
        }

        public override void LoadContent(GraphicsDevice graphics)
        {
            m_tutorialText = new KeyIconTextObj(Game.JunicodeLargeFont);
            m_tutorialText.FontSize = 28;
            m_tutorialText.Text = LocaleBuilder.getString("LOC_ID_THRONE_ROOM_OBJ_1_NEW", m_tutorialText);
            m_tutorialText.Align = Types.TextAlign.Centre;
            m_tutorialText.OutlineWidth = 2;

            m_king = new KingObj("King_Sprite");
            m_king.OutlineWidth = 2;
            m_king.AnimationDelay = 1 / 10f;
            m_king.PlayAnimation(true);
            m_king.IsWeighted = false;
            m_king.IsCollidable = true;
            m_king.Scale = new Vector2(2, 2);
            base.LoadContent(graphics);
        }

        public override void OnEnter()
        {
            SoundManager.StopMusic(1);

            if (m_king.PhysicsMngr == null)
                Player.PhysicsMngr.AddObject(m_king);

            m_kingKilled = false;
            Player.UnlockControls();
            m_displayText = false;
            m_tutorialText.Opacity = 0;

            m_king.UpdateCollisionBoxes();
            m_king.Position = new Vector2(this.Bounds.Right - 500, this.Y + 1440 - (m_king.Bounds.Bottom - m_king.Y) - 182);
        }

        public override void OnExit()
        {
            //Game.PlayerStats.TutorialComplete = true;
            //Game.PlayerStats.Gold = 0;
            //Game.PlayerStats.HeadPiece = (byte)CDGMath.RandomInt(1, PlayerPart.NumHeadPieces);// Necessary to change his headpiece so he doesn't look like the first dude.
            //Game.PlayerStats.EnemiesKilledInRun.Clear();
            //(Player.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData);
            base.OnExit();
        }

        public override void Update(GameTime gameTime)
        {
            if (m_displayText == false && CDGMath.DistanceBetweenPts(Player.Position, m_king.Position) < 200)
            {
                m_displayText = true;
                Tween.StopAllContaining(m_tutorialText, false);
                Tween.To(m_tutorialText, 0.5f, Tween.EaseNone, "Opacity", "1");
            }
            else if (m_displayText == true && CDGMath.DistanceBetweenPts(Player.Position, m_king.Position) > 200)
            {
                m_displayText = false;
                Tween.StopAllContaining(m_tutorialText, false);
                Tween.To(m_tutorialText, 0.5f, Tween.EaseNone, "Opacity", "0");
            }

            if (Player.X > m_king.X - 100)
                Player.X = m_king.X - 100;

            if (m_kingKilled == false && m_king.WasHit == true)
            {
                m_kingKilled = true;
                //SoundManager.PlaySound("Boss_Title_Exit");
                //SoundManager.PlaySound("Player_Death_Grunt");
                Game.ScreenManager.DisplayScreen(ScreenType.TitleWhite, false);
            }

            base.Update(gameTime);
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);
            m_king.Draw(camera);

            m_tutorialText.Position = Game.ScreenManager.Camera.Position;
            m_tutorialText.Y -= 200;
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_tutorialText.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_tutorialText.Dispose();
                m_tutorialText = null;
                m_king.Dispose();
                m_king = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new ThroneRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public override void RefreshTextObjs()
        {
            //m_tutorialText.Text = LocaleBuilder.getResourceString("LOC_ID_THRONE_ROOM_OBJ_1") /*"Press"*/ + " [Input:" + InputMapType.PLAYER_ATTACK + "] " + LocaleBuilder.getResourceString("LOC_ID_THRONE_ROOM_OBJ_2") /*"to Attack"*/;
            base.RefreshTextObjs();
        }
    }
}

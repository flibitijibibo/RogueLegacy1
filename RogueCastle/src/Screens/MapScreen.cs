using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class MapScreen : Screen
    {
        private MapObj m_mapDisplay;
        private ObjContainer m_legend;

        // Teleporter objects.
        private SpriteObj[] m_teleporterList;

        private SpriteObj m_titleText;
        private ObjContainer m_playerIcon;
        private int m_selectedTeleporter;

        private KeyIconTextObj m_continueText;
        private KeyIconTextObj m_recentreText;
        private KeyIconTextObj m_navigationText;
        private TextObj m_alzheimersQuestionMarks;

        public MapScreen(ProceduralLevelScreen level)
        {
            m_mapDisplay = new MapObj(false, level);

            m_alzheimersQuestionMarks = new TextObj(Game.JunicodeLargeFont);
            m_alzheimersQuestionMarks.FontSize = 30;
            m_alzheimersQuestionMarks.ForceDraw = true;
            m_alzheimersQuestionMarks.Text = "?????";
            m_alzheimersQuestionMarks.Align = Types.TextAlign.Centre;
            m_alzheimersQuestionMarks.Position = new Vector2(1320 / 2f, 720 / 2f - m_alzheimersQuestionMarks.Height / 2f);
        }

        private void FindRoomTitlePos(List<RoomObj> roomList, GameTypes.LevelType levelType, out Vector2 pos)
        {
            float leftMost = float.MaxValue;
            float rightMost = -float.MaxValue;
            float topMost = float.MaxValue;
            float bottomMost = -float.MaxValue;

            foreach (RoomObj room in roomList)
            {
                if (room.Name != "Boss")
                {
                    if (room.LevelType == levelType || (room.LevelType == GameTypes.LevelType.CASTLE && (room.Name == "Start" || room.Name == "CastleEntrance")))
                    {
                        if (room.X < leftMost)
                            leftMost = room.X;
                        if (room.X + room.Width > rightMost)
                            rightMost = room.X + room.Width;
                        if (room.Y < topMost)
                            topMost = room.Y;
                        if (room.Y + room.Height > bottomMost)
                            bottomMost = room.Y + room.Height;
                    }
                }
            }

            pos = new Vector2((rightMost + leftMost) / 2f, (bottomMost + topMost) / 2f);
            pos = new Vector2(pos.X / 1320f * 60, pos.Y / 720f * 32);
        }

        public void SetPlayer(PlayerObj player)
        {
            m_mapDisplay.SetPlayer(player);
        }

        public override void LoadContent()
        {
            //m_mapDisplay.SetPlayer((ScreenManager as RCScreenManager).GetLevelScreen().Player);
            m_mapDisplay.InitializeAlphaMap(new Rectangle(50, 50, 1320 - 100, 720 - 100), Camera);
            //m_mapDisplay.CameraOffset = new Vector2(100, 720 / 2f);
            m_mapDisplay.CameraOffset = new Vector2(1320 / 2, 720 / 2);

            m_legend = new ObjContainer();
            m_legend.ForceDraw = true;
            SpriteObj legendBG = new SpriteObj("TraitsScreenPlate_Sprite");
            m_legend.AddChild(legendBG);
            legendBG.Scale = new Vector2(0.75f, 0.58f);

            TextObj legendTitle = new TextObj(Game.JunicodeFont);
            legendTitle.Align = Types.TextAlign.Centre;
            legendTitle.Position = new Vector2(m_legend.Width/2 * legendBG.ScaleX, m_legend.Bounds.Top + 10);
            legendTitle.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_5", legendTitle); //"Legend"
            legendTitle.FontSize = 12;
            legendTitle.DropShadow = new Vector2(2, 2);
            legendTitle.TextureColor = new Color(213, 213, 173);

            m_legend.AddChild(legendTitle);
            m_legend.AnimationDelay = 1 / 30f;
            m_legend.Position = new Vector2(1320 - m_legend.Width - 20, 720 - m_legend.Height - 20);

            SpriteObj playerIcon = new SpriteObj("MapPlayerIcon_Sprite");
            playerIcon.Position = new Vector2(30, 60);
            playerIcon.PlayAnimation();
            m_legend.AddChild(playerIcon);

            int iconYOffset = 30;
            SpriteObj bossIcon = new SpriteObj("MapBossIcon_Sprite");
            bossIcon.Position = new Vector2(playerIcon.X, playerIcon.Y + iconYOffset);
            bossIcon.PlayAnimation();
            m_legend.AddChild(bossIcon);

            SpriteObj lockedChestIcon = new SpriteObj("MapLockedChestIcon_Sprite");
            lockedChestIcon.Position = new Vector2(playerIcon.X, bossIcon.Y + iconYOffset);
            lockedChestIcon.PlayAnimation();
            m_legend.AddChild(lockedChestIcon);

            SpriteObj fairyChestIcon = new SpriteObj("MapFairyChestIcon_Sprite");
            fairyChestIcon.Position = new Vector2(playerIcon.X, lockedChestIcon.Y + iconYOffset);
            fairyChestIcon.PlayAnimation();
            m_legend.AddChild(fairyChestIcon);

            SpriteObj openedChestIcon = new SpriteObj("MapChestUnlocked_Sprite");
            openedChestIcon.Position = new Vector2(playerIcon.X, fairyChestIcon.Y + iconYOffset);
            m_legend.AddChild(openedChestIcon);

            SpriteObj teleporterIcon = new SpriteObj("MapTeleporterIcon_Sprite");
            teleporterIcon.Position = new Vector2(playerIcon.X, openedChestIcon.Y + iconYOffset);
            teleporterIcon.PlayAnimation();
            m_legend.AddChild(teleporterIcon);

            SpriteObj bonusIcon = new SpriteObj("MapBonusIcon_Sprite");
            bonusIcon.Position = new Vector2(playerIcon.X, teleporterIcon.Y + iconYOffset);
            bonusIcon.PlayAnimation(true);
            m_legend.AddChild(bonusIcon);

            TextObj legendText;

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_6", legendText); //"You are here"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55 + iconYOffset);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_7", legendText); //"Boss location"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55 + iconYOffset + iconYOffset);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_8", legendText); //"Unopened chest"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55 + iconYOffset + iconYOffset + iconYOffset);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_9", legendText); //"Fairy chest"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55 + iconYOffset + iconYOffset + iconYOffset + iconYOffset);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_10", legendText); //"Opened chest"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55 + iconYOffset + iconYOffset + iconYOffset + iconYOffset + iconYOffset);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_11", legendText); //"Teleporter"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            legendText = new TextObj(Game.JunicodeFont);
            legendText.Position = new Vector2(playerIcon.X + 40, 55 + iconYOffset + iconYOffset + iconYOffset + iconYOffset + iconYOffset + iconYOffset);
            legendText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_12", legendText); //"Bonus Room"
            legendText.FontSize = 10;
            legendText.DropShadow = new Vector2(2, 2);
            m_legend.AddChild(legendText);

            playerIcon.X += 4;// hack to properly position player icon.
            playerIcon.Y += 4;// hack to properly position player icon.

            // Loading Teleporter objects.
            m_titleText = new SpriteObj("TeleporterTitleText_Sprite");
            m_titleText.ForceDraw = true;
            m_titleText.X = GlobalEV.ScreenWidth / 2;
            m_titleText.Y = GlobalEV.ScreenHeight * 0.1f;

            m_playerIcon = new ObjContainer("PlayerWalking_Character");
            m_playerIcon.Scale = new Vector2(0.6f, 0.6f);
            m_playerIcon.AnimationDelay = 1 / 10f;
            m_playerIcon.PlayAnimation(true);
            m_playerIcon.ForceDraw = true;
            m_playerIcon.OutlineWidth = 2;

            m_playerIcon.GetChildAt(PlayerPart.Cape).TextureColor = Color.Red;
            m_playerIcon.GetChildAt(PlayerPart.Hair).TextureColor = Color.Red;
            m_playerIcon.GetChildAt(PlayerPart.Neck).TextureColor = Color.Red;
            m_playerIcon.GetChildAt(PlayerPart.Light).Visible = false;
            m_playerIcon.GetChildAt(PlayerPart.Boobs).Visible = false;
            m_playerIcon.GetChildAt(PlayerPart.Bowtie).Visible = false;
            m_playerIcon.GetChildAt(PlayerPart.Wings).Visible = false;
            m_playerIcon.GetChildAt(PlayerPart.Extra).Visible = false;
            m_playerIcon.GetChildAt(PlayerPart.Glasses).Visible = false;

            m_continueText = new KeyIconTextObj(Game.JunicodeFont);
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_continueText); // dummy locID to add TextObj to language refresh list
            m_continueText.FontSize = 12;
            m_continueText.ForceDraw = true;
            m_continueText.Position = new Vector2(50, 200 - m_continueText.Height - 40);
            //m_continueText.ForcedScale = new Vector2(0.5f, 0.5f); // Added for PS3

            m_recentreText = new KeyIconTextObj(Game.JunicodeFont);
            m_recentreText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_recentreText); // dummy locID to add TextObj to language refresh list
            m_recentreText.FontSize = 12;
            m_recentreText.Position = new Vector2(m_continueText.X, 200 - m_continueText.Height - 80);
            m_recentreText.ForceDraw = true;

            m_navigationText = new KeyIconTextObj(Game.JunicodeFont);
            m_navigationText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_navigationText); // dummy locID to add TextObj to language refresh list
            m_navigationText.FontSize = 12;
            m_navigationText.Position = new Vector2(m_continueText.X, 200 - m_continueText.Height - 120);
            m_navigationText.ForceDraw = true;

            base.LoadContent();
        }

        public override void ReinitializeRTs()
        {
            m_mapDisplay.InitializeAlphaMap(new Rectangle(50, 50, 1320 - 100, 720 - 100), Camera);
            base.ReinitializeRTs();
        }

        public void AddRooms(List<RoomObj> roomList)
        {
            m_mapDisplay.AddAllRooms(roomList);
        }

        public void RefreshMapChestIcons(RoomObj room)
        {
            m_mapDisplay.RefreshChestIcons(room);
        }

        public override void OnEnter()
        {
            SoundManager.PlaySound("Map_On");

            m_mapDisplay.CentreAroundPlayer();

            if (IsTeleporter == false && (Game.PlayerStats.Traits.X == TraitType.Alzheimers || Game.PlayerStats.Traits.Y == TraitType.Alzheimers))
                m_mapDisplay.DrawNothing = true;
            else
                m_mapDisplay.DrawNothing = false;

            m_continueText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_1_NEW", m_continueText); //"to close map"
            m_recentreText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_2_NEW", m_recentreText); //"to center on player"
            if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_3", m_navigationText); //"Use arrow keys to move map"
            else
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_MAP_SCREEN_4_NEW", m_navigationText); //"to move map"

            if (IsTeleporter && m_teleporterList.Length > 0)
            {
                //PlayerObj player = (ScreenManager as RCScreenManager).GetLevelScreen().Player;
                //for (int i = 0; i < m_playerIcon.NumChildren; i++)
                //    m_playerIcon.GetChildAt(i).TextureColor = player.GetChildAt(i).TextureColor;

                //m_selectedTeleporter = 0;
                SpriteObj icon = m_teleporterList[m_selectedTeleporter];
                m_playerIcon.Position = new Vector2(icon.X + 14 / 2f, icon.Y - 20);
                m_mapDisplay.CentreAroundTeleporter(m_selectedTeleporter);
            }

            Game.ChangeBitmapLanguage(m_titleText, "TeleporterTitleText_Sprite");

            base.OnEnter();
        }

        public override void OnExit()
        {
            SoundManager.PlaySound("Map_Off");

            IsTeleporter = false;
            base.OnExit();
        }

        public override void HandleInput()
        {
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_MAP) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                Game.ScreenManager.Player.UnlockControls();
                (this.ScreenManager as RCScreenManager).HideCurrentScreen();
            }

            if (IsTeleporter == false)
            {
                float movementAmount = 5f;
                if (Game.GlobalInput.Pressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_UP2))
                    m_mapDisplay.CameraOffset.Y += movementAmount;
                else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_DOWN2))
                    m_mapDisplay.CameraOffset.Y -= movementAmount;

                if (Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2))
                    m_mapDisplay.CameraOffset.X += movementAmount;
                else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2))
                    m_mapDisplay.CameraOffset.X -= movementAmount;

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                    m_mapDisplay.CentreAroundPlayer();
            }
            else
            {
                int previousSelection = m_selectedTeleporter;

                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
                {
                    m_selectedTeleporter++;
                    if (m_selectedTeleporter >= m_teleporterList.Length)
                        m_selectedTeleporter = 0;
                }
                else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2))
                {
                    m_selectedTeleporter--;
                    if (m_selectedTeleporter < 0 && m_teleporterList.Length > 0)
                        m_selectedTeleporter = m_teleporterList.Length - 1;
                    else if (m_selectedTeleporter < 0 && m_teleporterList.Length <= 0)
                        m_selectedTeleporter = 0;
                }

                if (previousSelection != m_selectedTeleporter)
                    m_mapDisplay.CentreAroundTeleporter(m_selectedTeleporter, true);

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                {
                    m_mapDisplay.TeleportPlayer(m_selectedTeleporter);
                    (this.ScreenManager as RCScreenManager).HideCurrentScreen();
                }
            }

            base.HandleInput();
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            m_mapDisplay.DrawRenderTargets(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            Camera.GraphicsDevice.SetRenderTarget(Game.ScreenManager.RenderTarget);
            Camera.GraphicsDevice.Clear(Color.Black);

            Camera.Draw((ScreenManager as RCScreenManager).GetLevelScreen().RenderTarget, Vector2.Zero, Color.White * 0.3f); // Comment in this line if you want the back screen drawn.
            m_mapDisplay.Draw(Camera);

            if (IsTeleporter == true && m_teleporterList.Length > 0)
            {
                m_titleText.Draw(Camera);
                SpriteObj icon = m_teleporterList[m_selectedTeleporter];
                m_playerIcon.Position = new Vector2(icon.X + 14, icon.Y - 20);
                m_playerIcon.Draw(Camera);
            }

            if (IsTeleporter == false)
            {
                m_recentreText.Draw(Camera);
                m_navigationText.Draw(Camera);
            }

            if (IsTeleporter == false && (Game.PlayerStats.Traits.X == TraitType.Alzheimers || Game.PlayerStats.Traits.Y == TraitType.Alzheimers))
                m_alzheimersQuestionMarks.Draw(Camera);

            m_continueText.Draw(Camera);
            m_legend.Draw(Camera);
            Camera.End();
            base.Draw(gameTime); // Doesn't do anything.
        }

        public void AddAllIcons(List<RoomObj> roomList)
        {
            m_mapDisplay.AddAllIcons(roomList);
        }

        public override void DisposeRTs()
        {
            m_mapDisplay.DisposeRTs();
            base.DisposeRTs();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Map Screen");

                if (m_mapDisplay != null)
                    m_mapDisplay.Dispose();
                m_mapDisplay = null;
                if (m_legend != null)
                    m_legend.Dispose();
                m_legend = null;

                if (m_playerIcon != null)
                    m_playerIcon.Dispose();
                m_playerIcon = null;
                if (m_teleporterList != null)
                    Array.Clear(m_teleporterList, 0, m_teleporterList.Length);
                m_teleporterList = null;
                if (m_titleText != null)
                    m_titleText.Dispose();
                m_titleText = null;
                if (m_continueText != null)
                    m_continueText.Dispose();
                m_continueText = null;
                if (m_recentreText != null)
                    m_recentreText.Dispose();
                m_recentreText = null;
                if (m_navigationText != null)
                    m_navigationText.Dispose();
                m_navigationText = null;

                m_alzheimersQuestionMarks.Dispose();
                m_alzheimersQuestionMarks = null;

                base.Dispose();
            }
        }

        private bool m_isTeleporter = false;
        public bool IsTeleporter 
        {
            get { return m_isTeleporter; }
            set
            {
                m_mapDisplay.DrawTeleportersOnly = value;
                m_isTeleporter = value;
                if (value == true)
                {
                    if (m_teleporterList != null)
                        Array.Clear(m_teleporterList, 0, m_teleporterList.Length);
                    m_teleporterList = m_mapDisplay.TeleporterList();
                }
            }
        }

        public override void RefreshTextObjs()
        {
            /*
            m_continueText.Text = "[Input:" + InputMapType.MENU_MAP + "]  " + LocaleBuilder.getResourceString("LOC_ID_MAP_SCREEN_1"); //"to close map"
            m_recentreText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "]  " + LocaleBuilder.getResourceString("LOC_ID_MAP_SCREEN_2"); //"to center on player"
            if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                m_navigationText.Text = LocaleBuilder.getResourceString("LOC_ID_MAP_SCREEN_3"); //"Use arrow keys to move map"
            else
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_MAP_SCREEN_4"); //"to move map"
             */
            Game.ChangeBitmapLanguage(m_titleText, "TeleporterTitleText_Sprite");

            base.RefreshTextObjs();
        }
    }
}

//#define OLD_CONSOLE_CREDITS
//#define SWITCH_CREDITS

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
    public class TutorialRoomObj : RoomObj
    {
        private KeyIconTextObj m_tutorialText;
        private int m_waypointIndex = 0;
        private List<GameObj> m_waypointList;
        //private string[,] m_tutorialTextList;
        //private string[,] m_tutorialControllerTextList;
        private string[] m_tutorialTextList;
        private string[] m_tutorialControllerTextList;

        private TextObj m_creditsText;
        private TextObj m_creditsTitleText;
        private string[] m_creditsTextList;
        private string[] m_creditsTextTitleList;
        private Vector2 m_creditsPosition;
        private int m_creditsIndex = 0;

        private SpriteObj m_diary;
        private SpriteObj m_doorSprite;
        private DoorObj m_door;
        private SpriteObj m_speechBubble;

        public TutorialRoomObj()
        {
            m_waypointList = new List<GameObj>();
        }

        public override void Initialize()
        {
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "diary")
                    m_diary = obj as SpriteObj;

                if (obj.Name == "doorsprite")
                    m_doorSprite = obj as SpriteObj;
            }

            m_door = DoorList[0];

            m_speechBubble = new SpriteObj("ExclamationSquare_Sprite");
            m_speechBubble.Flip = SpriteEffects.FlipHorizontally;
            m_speechBubble.Scale = new Vector2(1.2f, 1.2f);
            GameObjList.Add(m_speechBubble);

            m_diary.OutlineWidth = 2;
            m_speechBubble.Position = new Vector2(m_diary.X, m_diary.Y - m_speechBubble.Height - 20);

            /*
            m_tutorialTextList = new string[,]
            {
                // { textID, action, textID, action, textID }
                { "LOC_ID_TUTORIAL_ROOM_OBJ_1", " [Input:" + InputMapType.PLAYER_JUMP2 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_2", ""                                            , ""                           },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_3", " [Input:" + InputMapType.PLAYER_JUMP2 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_4", ""                                            , ""                           },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_1", " [Input:" + InputMapType.PLAYER_ATTACK + "] ", "LOC_ID_TUTORIAL_ROOM_OBJ_5", ""                                            , ""                           },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_3", " [Input:" + InputMapType.PLAYER_DOWN2 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_6", " [Input:" + InputMapType.PLAYER_JUMP2 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_7" },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_8", " [Input:" + InputMapType.PLAYER_DOWN2 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_6", " [Input:" + InputMapType.PLAYER_ATTACK + "] ", "LOC_ID_TUTORIAL_ROOM_OBJ_9" }
            };

            m_tutorialControllerTextList = new string[,]
            {
                // { textID, action, textID, action, textID }
                { "LOC_ID_TUTORIAL_ROOM_OBJ_1", " [Input:" + InputMapType.PLAYER_JUMP1 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_2", ""                                            , ""                           },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_3", " [Input:" + InputMapType.PLAYER_JUMP1 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_4", ""                                            , ""                           },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_1", " [Input:" + InputMapType.PLAYER_ATTACK + "] ", "LOC_ID_TUTORIAL_ROOM_OBJ_5", ""                                            , ""                           },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_3", " [Input:" + InputMapType.PLAYER_DOWN1 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_6", " [Input:" + InputMapType.PLAYER_JUMP1 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_7" },
                { "LOC_ID_TUTORIAL_ROOM_OBJ_8", " [Input:" + InputMapType.PLAYER_DOWN1 + "] " , "LOC_ID_TUTORIAL_ROOM_OBJ_6", " [Input:" + InputMapType.PLAYER_ATTACK + "] ", "LOC_ID_TUTORIAL_ROOM_OBJ_9" }
            };
            */

            // NEW PLACEHOLDER TEXT
            m_tutorialTextList = new string[]
            {
                "LOC_ID_TUTORIAL_ROOM_OBJ_11",
                "LOC_ID_TUTORIAL_ROOM_OBJ_12",
                "LOC_ID_TUTORIAL_ROOM_OBJ_13",
                "LOC_ID_TUTORIAL_ROOM_OBJ_14",
                "LOC_ID_TUTORIAL_ROOM_OBJ_15"
            };

            m_tutorialControllerTextList = new string[]
            {
                "LOC_ID_TUTORIAL_ROOM_OBJ_16",
                "LOC_ID_TUTORIAL_ROOM_OBJ_17",
                "LOC_ID_TUTORIAL_ROOM_OBJ_18",
                "LOC_ID_TUTORIAL_ROOM_OBJ_19",
                "LOC_ID_TUTORIAL_ROOM_OBJ_20"
            };

            m_creditsTextTitleList = new string[]
                {
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_1",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_2",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_3",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_4",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_5",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_6",
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_7",
#if OLD_CONSOLE_CREDITS || SWITCH_CREDITS
                    "LOC_ID_CREDITS_SCREEN_26",
                    //"Japanese Localization & Production By",  // This is not translated
#endif
                    "LOC_ID_TUTORIAL_CREDITS_TITLE_8"
                };

            m_creditsTextList = new string[]
                {
                    "Cellar Door Games",
                    "Teddy Lee",
                    "Kenny Lee", 
#if SWITCH_CREDITS
                    "Ryan Lee",
#else
                    "Marie-Christine Bourdua",
#endif
                    "Glauber Kotaki",
                    "Gordon McGladdery",
                    "Judson Cowan",
#if OLD_CONSOLE_CREDITS
                    "Abstraction Games",
                    //"8-4, Ltd.", // The Japanese Localization text above needs to be translated before this can be uncommented out.
#endif
#if SWITCH_CREDITS
                    "BlitWorks SL",
#endif
                    "Rogue Legacy",
                };

            m_creditsPosition = new Vector2(50, 580);

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "waypoint1")
                    m_waypointList.Add(obj);
                if (obj.Name == "waypoint2")
                    m_waypointList.Add(obj);
                if (obj.Name == "waypoint3")
                    m_waypointList.Add(obj);
                if (obj.Name == "waypoint4")
                    m_waypointList.Add(obj);
                if (obj.Name == "waypoint5")
                    m_waypointList.Add(obj);
            }

            base.Initialize();
        }

        public override void LoadContent(GraphicsDevice graphics)
        {
            m_tutorialText = new KeyIconTextObj(Game.JunicodeLargeFont);
            m_tutorialText.FontSize = 28;
            m_tutorialText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_tutorialText); // dummy locID to add TextObj to language refresh list
            m_tutorialText.Align = Types.TextAlign.Centre;
            m_tutorialText.OutlineWidth = 2;
            m_tutorialText.ForcedScale = new Vector2(0.8f, 0.8f);

            m_creditsText = new TextObj(Game.JunicodeFont);
            m_creditsText.FontSize = 20;
            m_creditsText.Text = "Cellar Door Games"; // placeholder text
            m_creditsText.DropShadow = new Vector2(2, 2);

            m_creditsTitleText = m_creditsText.Clone() as TextObj;
            m_creditsTitleText.FontSize = 14;

            TextObj attackThisText = new TextObj(Game.JunicodeFont);
            attackThisText.FontSize = 12;
            attackThisText.Text = LocaleBuilder.getString("LOC_ID_TUTORIAL_ROOM_OBJ_10", attackThisText);
            attackThisText.OutlineWidth = 2;
            attackThisText.Align = Types.TextAlign.Centre;
            attackThisText.Position = m_waypointList[m_waypointList.Count - 1].Position;
            attackThisText.X -= 25;
            attackThisText.Y -= 70;
            this.GameObjList.Add(attackThisText);

            base.LoadContent(graphics);
        }

        public override void OnEnter()
        {
            m_speechBubble.Visible = false;
            m_diary.Visible = false;
            m_doorSprite.ChangeSprite("CastleDoorOpen_Sprite");

            // All code regarding second time entering tutorial room goes here.
            if (Game.PlayerStats.TutorialComplete == true)
            {
                if (Game.PlayerStats.ReadLastDiary == false)
                {
                    m_door.Locked = true;
                    m_doorSprite.ChangeSprite("CastleDoor_Sprite");
                }
                else
                {
                    m_door.Locked = false;
                }

                m_diary.Visible = true;
                Player.UpdateCollisionBoxes();
                Player.Position = new Vector2(this.X + 240 + Player.Width, this.Bounds.Bottom - 120 - (Player.Bounds.Bottom - Player.Y));
            }

            m_creditsTitleText.Opacity = 0;
            m_creditsText.Opacity = 0;

            foreach (EnemyObj enemy in EnemyList)
                enemy.Damage = 0;

            m_tutorialText.Opacity = 0;
            Player.UnlockControls();

            if (Game.PlayerStats.TutorialComplete == false)
                SoundManager.PlayMusic("EndSong", true, 4);
            else
                SoundManager.StopMusic(4);

            //Tween.RunFunction(2, this, "DisplayCreditsText");
            Tween.RunFunction(2, Player.AttachedLevel, "DisplayCreditsText", true);
            base.OnEnter();
        }

        // Hmm...looks like this function doesn't get executed --Dave
        public void DisplayCreditsText()
        {
            if (m_creditsIndex < m_creditsTextList.Length)
            {
                m_creditsTitleText.Opacity = 0;
                m_creditsText.Opacity = 0;

                m_creditsTitleText.Text = LocaleBuilder.getString(m_creditsTextTitleList[m_creditsIndex], m_creditsTitleText);
                m_creditsText.Text = m_creditsTextList[m_creditsIndex];

                // Tween text in.
                Tween.To(m_creditsTitleText, 0.5f, Tween.EaseNone, "Opacity", "1");
                Tween.To(m_creditsText, 0.5f, Tween.EaseNone, "delay", "0.2", "Opacity", "1");
                m_creditsTitleText.Opacity = 1;
                m_creditsText.Opacity = 1;

                // Tween text out.
                Tween.To(m_creditsTitleText, 0.5f, Tween.EaseNone, "delay", "4", "Opacity", "0");
                Tween.To(m_creditsText, 0.5f, Tween.EaseNone, "delay", "4.2", "Opacity", "0");
                m_creditsTitleText.Opacity = 0;
                m_creditsText.Opacity = 0;

                m_creditsIndex++;
                Tween.RunFunction(8, this, "DisplayCreditsText");
            }
        }

        private int PlayerNearWaypoint()
        {
            for (int i = 0; i < m_waypointList.Count; i++)
            {
                if (CDGMath.DistanceBetweenPts(Player.Position, m_waypointList[i].Position) < 500)
                    return i;
            }

            return -1;
        }

        public override void Update(GameTime gameTime)
        {
            if (Game.PlayerStats.TutorialComplete == false)
            {
                int previousIndex = m_waypointIndex;
                m_waypointIndex = PlayerNearWaypoint();

                if (m_waypointIndex != previousIndex)
                {
                    Tween.StopAllContaining(m_tutorialText, false);
                    if (m_waypointIndex != -1)
                    {
                        if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                            m_tutorialText.Text = LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex], m_tutorialText);
                            //m_tutorialText.Text = LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex, 0], m_tutorialText) + m_tutorialTextList[m_waypointIndex, 1] +
                            //    LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex, 2], m_tutorialText) + m_tutorialTextList[m_waypointIndex, 3] +
                            //    LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex, 4], m_tutorialText);
                        else
                            m_tutorialText.Text = LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex], m_tutorialText);
                            //m_tutorialText.Text = LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex, 0], m_tutorialText) + m_tutorialControllerTextList[m_waypointIndex, 1] +
                            //    LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex, 2], m_tutorialText) + m_tutorialControllerTextList[m_waypointIndex, 3] +
                            //    LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex, 4], m_tutorialText);
                        Tween.To(m_tutorialText, 0.25f, Tween.EaseNone, "Opacity", "1");
                    }
                    else
                        Tween.To(m_tutorialText, 0.25f, Tween.EaseNone, "Opacity", "0");
                }
            }
            else
            {
                Rectangle diaryBound = m_diary.Bounds;
                diaryBound.X -= 50;
                diaryBound.Width += 100;
                m_speechBubble.Y = m_diary.Y - m_speechBubble.Height - 20 - 30 + ((float)Math.Sin(Game.TotalGameTime * 20) * 2);
                if (CollisionMath.Intersects(Player.Bounds, diaryBound) && Player.IsTouchingGround == true)
                {
                    if (m_speechBubble.SpriteName == "ExclamationSquare_Sprite")
                        m_speechBubble.ChangeSprite("UpArrowSquare_Sprite");

                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    {
                        if (Game.PlayerStats.ReadLastDiary == false)
                        {
                            RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                            manager.DialogueScreen.SetDialogue("DiaryEntry" + (LevelEV.TOTAL_JOURNAL_ENTRIES - 1));
                            manager.DialogueScreen.SetConfirmEndHandler(this, "RunFlashback");
                            manager.DisplayScreen(ScreenType.Dialogue, true, null);
                        }
                        else
                        {
                            RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                            manager.DisplayScreen(ScreenType.DiaryEntry, true);
                        }
                    }
                }
                else
                {
                    if (m_speechBubble.SpriteName == "UpArrowSquare_Sprite")
                        m_speechBubble.ChangeSprite("ExclamationSquare_Sprite");
                }

                if (Game.PlayerStats.ReadLastDiary == false || CollisionMath.Intersects(Player.Bounds, diaryBound) == true)
                    m_speechBubble.Visible = true;
                else if (Game.PlayerStats.ReadLastDiary == true && CollisionMath.Intersects(Player.Bounds, diaryBound) == false)
                    m_speechBubble.Visible = false;

            }
            base.Update(gameTime);
        }

        public void RunFlashback()
        {
            Player.LockControls();
            (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.DiaryFlashback, true);
            Tween.RunFunction(0.5f, this, "OpenDoor");
        }

        public void OpenDoor()
        {
            Player.UnlockControls();

            m_doorSprite.ChangeSprite("CastleDoorOpen_Sprite");
            m_door.Locked = false;
            Game.PlayerStats.ReadLastDiary = true;
            Game.PlayerStats.DiaryEntry = LevelEV.TOTAL_JOURNAL_ENTRIES;
            (Player.AttachedLevel.ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData);

            // Door opening smoke effect and sfx goes here.
        }

        public override void Draw(Camera2D camera)
        {
            Vector2 cameraPos = Game.ScreenManager.Camera.TopLeftCorner;
            m_creditsTitleText.Position = new Vector2(cameraPos.X + m_creditsPosition.X, cameraPos.Y + m_creditsPosition.Y);
            m_creditsText.Position = m_creditsTitleText.Position;
            m_creditsText.Y += 35;
            m_creditsTitleText.X += 5;

            base.Draw(camera);

            m_tutorialText.Position = Game.ScreenManager.Camera.Position;
            m_tutorialText.Y -= 200;
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_tutorialText.Draw(camera);
            m_creditsText.Draw(camera);
            m_creditsTitleText.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_tutorialText.Dispose();
                m_tutorialText = null;
                m_waypointList.Clear();
                m_waypointList = null;
                m_creditsText.Dispose();
                m_creditsText = null;
                m_creditsTitleText.Dispose();
                m_creditsTitleText = null;

                Array.Clear(m_tutorialTextList, 0, m_tutorialTextList.Length);
                Array.Clear(m_tutorialControllerTextList, 0, m_tutorialControllerTextList.Length);
                Array.Clear(m_creditsTextTitleList, 0, m_creditsTextTitleList.Length);
                Array.Clear(m_creditsTextList, 0, m_creditsTextList.Length);

                m_tutorialTextList = null;
                m_creditsTextTitleList = null;
                m_creditsTextList = null;
                m_tutorialControllerTextList = null;

                m_door = null;
                m_doorSprite = null;
                m_diary = null;
                m_speechBubble = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new TutorialRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public override void RefreshTextObjs()
        {
            /*
            if (m_waypointIndex != -1 && m_tutorialText != null)
            {
                if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                    m_tutorialText.Text = LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex, 0], m_tutorialText) + m_tutorialTextList[m_waypointIndex, 1] +
                        LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex, 2], m_tutorialText) + m_tutorialTextList[m_waypointIndex, 3] +
                        LocaleBuilder.getString(m_tutorialTextList[m_waypointIndex, 4], m_tutorialText);
                else
                    m_tutorialText.Text = LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex, 0], m_tutorialText) + m_tutorialControllerTextList[m_waypointIndex, 1] +
                        LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex, 2], m_tutorialText) + m_tutorialControllerTextList[m_waypointIndex, 3] +
                        LocaleBuilder.getString(m_tutorialControllerTextList[m_waypointIndex, 4], m_tutorialText);
            }
             */
            base.RefreshTextObjs();
        }
    }
}

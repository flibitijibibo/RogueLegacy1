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
    public class CastleEntranceRoomObj : RoomObj
    {
        private bool m_gateClosed = false;
        private PhysicsObj m_castleGate; // This doesn't need to be disposed manually because the room automatically disposes of all objects in its GameObjList.
        private TeleporterObj m_teleporter; // This doesn't need to be disposed manually because the room automatically disposes of all objects in its GameObjList.

        private ObjContainer m_bossDoorSprite;
        private DoorObj m_bossDoor;

        private SpriteObj m_diary;
        private SpriteObj m_speechBubble;

        private TextObj m_mapText;
        private KeyIconObj m_mapIcon;

        private bool m_allFilesSaved = false;
        private bool m_bossDoorOpening = false;

        public CastleEntranceRoomObj()
        {
            m_castleGate = new PhysicsObj("CastleEntranceGate_Sprite");
            m_castleGate.IsWeighted = false;
            m_castleGate.IsCollidable = true;
            m_castleGate.CollisionTypeTag = GameTypes.CollisionType_WALL;
            m_castleGate.Layer = -1;
            m_castleGate.OutlineWidth = 2;
            this.GameObjList.Add(m_castleGate);

            m_teleporter = new TeleporterObj();
            this.GameObjList.Add(m_teleporter);
        }

        public override void Initialize()
        {
            //m_speechBubble = new SpriteObj("TalkBubbleUpArrow_Sprite");
            m_speechBubble = new SpriteObj("ExclamationSquare_Sprite");
            m_speechBubble.Flip = SpriteEffects.FlipHorizontally;
            m_speechBubble.Scale = new Vector2(1.2f, 1.2f);
            GameObjList.Add(m_speechBubble);

            m_mapText = new KeyIconTextObj(Game.JunicodeFont);
            m_mapText.Text = LocaleBuilder.getString("LOC_ID_CASTLE_ENTRANCE_ROOM_OBJ_1", m_mapText); //"view map any time"
            m_mapText.Align = Types.TextAlign.Centre;
            m_mapText.FontSize = 12;
            m_mapText.OutlineWidth = 2;
            GameObjList.Add(m_mapText);

            m_mapIcon = new KeyIconObj();
            m_mapIcon.Scale = new Vector2(0.5f, 0.5f);
            GameObjList.Add(m_mapIcon);

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "diary")
                    m_diary = obj as SpriteObj;

                if (obj.Name == "map")
                {
                    (obj as SpriteObj).OutlineWidth = 2;
                    m_mapText.Position = new Vector2(obj.X, obj.Bounds.Top - 50);
                    m_mapIcon.Position = new Vector2(m_mapText.X, m_mapText.Y - 20);
                }
            }

            m_diary.OutlineWidth = 2;
            m_speechBubble.Position = new Vector2(m_diary.X, m_diary.Y - m_speechBubble.Height - 20);

            DoorObj leftDoor = null;
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "LastDoor")
                {
                    m_bossDoorSprite = obj as ObjContainer;
                    break;
                }
            }

            foreach (DoorObj door in DoorList)
            {
                if (door.DoorPosition == "Left")
                    leftDoor = door;

                if (door.IsBossDoor == true)
                {
                    m_bossDoor = door;
                    m_bossDoor.Locked = true; // Set to false for debugging right now.
                }
            }

            for (int i = 1; i < m_bossDoorSprite.NumChildren; i++)
                m_bossDoorSprite.GetChildAt(i).Opacity = 0;
            m_bossDoorSprite.AnimationDelay = 1 / 10f;

            m_castleGate.Position = new Vector2(leftDoor.Bounds.Right - m_castleGate.Width, leftDoor.Y - m_castleGate.Height);
            m_teleporter.Position = new Vector2(this.X + this.Width / 2f - 600, this.Y + 720 - 120);
            
            base.Initialize();
        }

        public void RevealSymbol(GameTypes.LevelType levelType, bool tween)
        {
            int index = 0;
            bool changeColour = false;
            // index layers are completely arbitrary and determined by their layers in the spritesheet.
            switch (levelType)
            {
                case (GameTypes.LevelType.CASTLE):
                    index = 1; // Eyeball symbol.
                    if (Game.PlayerStats.ChallengeEyeballBeaten == true)
                        changeColour = true;
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    index = 4; // Blob symbol.
                    if (Game.PlayerStats.ChallengeBlobBeaten == true)
                        changeColour = true;
                    break;
                case (GameTypes.LevelType.GARDEN):
                    index = 3; // Fairy symbol.
                    if (Game.PlayerStats.ChallengeSkullBeaten == true)
                        changeColour = true;
                    break;
                case (GameTypes.LevelType.TOWER):
                    index = 2; // Fireball symbol.
                    if (Game.PlayerStats.ChallengeFireballBeaten == true)
                        changeColour = true;
                    break;
                default:
                    index = 5; // Last Boss Door.
                    if (Game.PlayerStats.ChallengeLastBossBeaten == true)
                        changeColour = true;
                    break;
            }

            if (changeColour == true)
                m_bossDoorSprite.GetChildAt(index).TextureColor = Color.Yellow;
            else
                m_bossDoorSprite.GetChildAt(index).TextureColor = Color.White;

            if (tween == true)
            {
                m_bossDoorSprite.GetChildAt(index).Opacity = 0;
                Tween.To(m_bossDoorSprite.GetChildAt(index), 0.5f, Quad.EaseInOut, "delay", "1.5", "Opacity", "1");
            }
            else
                m_bossDoorSprite.GetChildAt(index).Opacity = 1;
        }

        public override void OnEnter()
        {
            m_bossDoorOpening = false;

            if (Game.PlayerStats.ReadLastDiary == true && LinkedRoom.LinkedRoom != null)
                LinkedRoom = LinkedRoom.LinkedRoom;

            Game.PlayerStats.LoadStartingRoom = false;

            if (Game.PlayerStats.DiaryEntry < 1)
                m_speechBubble.Visible = true;
            else
                m_speechBubble.Visible = false;

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == true)
            {
                m_mapIcon.SetButton(Game.GlobalInput.ButtonList[InputMapType.MENU_MAP]);
                m_mapIcon.Scale = new Vector2(1, 1);
            }
            else
            {
                m_mapIcon.SetKey(Game.GlobalInput.KeyList[InputMapType.MENU_MAP]);
                m_mapIcon.Scale = new Vector2(0.5f, 0.5f);
            }

            if (m_allFilesSaved == false)
            {
                this.Player.Game.SaveManager.SaveAllFileTypes(false); // Save the map the moment the player enters the castle entrance.
                m_allFilesSaved = true; // Prevents you from constantly saving the map over and over again.
            }

            // Setting all boss states.
            if (Game.PlayerStats.EyeballBossBeaten == true)
                RevealSymbol(GameTypes.LevelType.CASTLE, false);
            if (Game.PlayerStats.FairyBossBeaten == true)
                RevealSymbol(GameTypes.LevelType.GARDEN, false);
            if (Game.PlayerStats.BlobBossBeaten == true)
                RevealSymbol(GameTypes.LevelType.DUNGEON, false);
            if (Game.PlayerStats.FireballBossBeaten == true)
                RevealSymbol(GameTypes.LevelType.TOWER, false);

            if (Game.PlayerStats.EyeballBossBeaten == true && Game.PlayerStats.FairyBossBeaten == true && Game.PlayerStats.BlobBossBeaten == true && Game.PlayerStats.FireballBossBeaten == true
                && Game.PlayerStats.FinalDoorOpened == false && Player.ScaleX > 0.1f) // only run the animation if the player is actually in there.
            {
                //// Animation for opening the final door goes here.
                PlayBossDoorAnimation();
            }
            else if (Game.PlayerStats.FinalDoorOpened == true)
            {
                m_bossDoor.Locked = false;
                m_bossDoorSprite.ChangeSprite("LastDoorOpen_Character");
                m_bossDoorSprite.GoToFrame(m_bossDoorSprite.TotalFrames);
            }

            if (m_gateClosed == false)
                CloseGate(true);

            if (Game.PlayerStats.EyeballBossBeaten == true && Game.PlayerStats.FairyBossBeaten == true && Game.PlayerStats.BlobBossBeaten == true && Game.PlayerStats.FireballBossBeaten == true
               && Game.PlayerStats.FinalDoorOpened == false && Player.ScaleX > 0.1f) // only run the animation if the player is actually in there.
            {
                Game.PlayerStats.FinalDoorOpened = true;
                Player.AttachedLevel.RunCinematicBorders(6); // Hack to prevent cinematic border conflict.
            }

            base.OnEnter();
        }

        public void PlayBossDoorAnimation()
        {
            Player.StopDash();
            m_bossDoorOpening = true;
            m_bossDoor.Locked = false;
            Player.AttachedLevel.UpdateCamera();
            RevealSymbol(GameTypes.LevelType.NONE, true);
            Player.CurrentSpeed = 0;
            Player.LockControls();
            Player.AttachedLevel.CameraLockedToPlayer = false;
            float storedX = Player.AttachedLevel.Camera.X;
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "X", this.Bounds.Center.X.ToString());
            Tween.RunFunction(2.2f, this, "PlayBossDoorAnimation2", storedX);
        }

        public void PlayBossDoorAnimation2(float storedX)
        {
            m_bossDoorSprite.ChangeSprite("LastDoorOpen_Character");
            m_bossDoorSprite.PlayAnimation(false);
            SoundManager.PlaySound("LastDoor_Open");
            Tween.To(Player.AttachedLevel.Camera, 1, Quad.EaseInOut, "delay", "2", "X", storedX.ToString());
            Tween.RunFunction(3.1f, this, "BossDoorAnimationComplete");
        }

        public void BossDoorAnimationComplete()
        {
            m_bossDoorOpening = false;
            Player.UnlockControls();
            Player.AttachedLevel.CameraLockedToPlayer = true;
        }

        public void ForceGateClosed()
        {
            m_castleGate.Y += m_castleGate.Height;
            m_gateClosed = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_bossDoorOpening == true && Player.ControlsLocked == false)
                Player.LockControls();

            if (SoundManager.IsMusicPlaying == false)
                SoundManager.PlayMusic("CastleSong", true, 0);

            // Prevents the player from going backwards.
            // This is preventing the player from entering the last boss room.
            if (Player.X < m_castleGate.Bounds.Right)
            {
                Player.X = m_castleGate.Bounds.Right + 20;
                Player.AttachedLevel.UpdateCamera();
            }

            // Diary logic
            Rectangle diaryBound = m_diary.Bounds;
            diaryBound.X -= 50;
            diaryBound.Width += 100;
            m_speechBubble.Y = m_diary.Y - m_speechBubble.Height - 20 - 30 + ((float)Math.Sin(Game.TotalGameTime * 20) * 2);
            if (CollisionMath.Intersects(Player.Bounds, diaryBound) && Player.IsTouchingGround == true)
            {
                if (m_speechBubble.SpriteName == "ExclamationSquare_Sprite")
                    m_speechBubble.ChangeSprite("UpArrowSquare_Sprite");
            }
            else
            {
                if (m_speechBubble.SpriteName == "UpArrowSquare_Sprite")
                    m_speechBubble.ChangeSprite("ExclamationSquare_Sprite");
            }

            if (Game.PlayerStats.DiaryEntry < 1 || CollisionMath.Intersects(Player.Bounds, diaryBound) == true)
                m_speechBubble.Visible = true;
            else if (Game.PlayerStats.DiaryEntry >= 1 && CollisionMath.Intersects(Player.Bounds, diaryBound) == false)
                m_speechBubble.Visible = false;


            if (CollisionMath.Intersects(Player.Bounds, diaryBound) && Player.IsTouchingGround == true)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    if (Game.PlayerStats.DiaryEntry < 1)
                    {
                        RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                        manager.DialogueScreen.SetDialogue("DiaryEntry0");
                        manager.DisplayScreen(ScreenType.Dialogue, true, null);

                        Game.PlayerStats.DiaryEntry++;
                    }
                    else
                    {
                        RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                        manager.DisplayScreen(ScreenType.DiaryEntry, true);
                    }
                }
            }

            base.Update(gameTime);
        }

        public void CloseGate(bool animate)
        {
            if (animate == true)
            {
                Player.Y = 381;
                Player.X += 10;
                Player.State = 1; // Force the player into a walking state. This state will not update until the logic set is complete.
                LogicSet playerMoveLS = new LogicSet(Player);
                playerMoveLS.AddAction(new RunFunctionLogicAction(Player, "LockControls"));
                playerMoveLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
                playerMoveLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character"));
                playerMoveLS.AddAction(new PlayAnimationLogicAction(true));
                playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "CurrentSpeed", 200));
                playerMoveLS.AddAction(new DelayLogicAction(0.2f));
                playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "CurrentSpeed", 0));
                Player.RunExternalLogicSet(playerMoveLS); // Do not dispose this logic set. The player object will do it on its own.

                Tween.By(m_castleGate, 1.5f, Quad.EaseOut, "Y", m_castleGate.Height.ToString());
                Tween.AddEndHandlerToLastTween(Player, "UnlockControls");

                Player.AttachedLevel.RunCinematicBorders(1.5f);
            }
            else
                m_castleGate.Y += m_castleGate.Height;

            m_gateClosed = true;
        }

        public override void Reset()
        {
            if (m_gateClosed == true)
            {
                m_castleGate.Y -= m_castleGate.Height;
                m_gateClosed = false;
            }
            base.Reset();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_castleGate = null;
                m_teleporter = null;
                m_bossDoor = null;
                m_bossDoorSprite = null;

                m_diary = null;
                m_speechBubble = null;
                m_mapText = null;
                m_mapIcon = null;
                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new CastleEntranceRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }
    }
}

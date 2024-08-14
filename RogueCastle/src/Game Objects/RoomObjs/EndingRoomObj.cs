using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using InputSystem;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

namespace RogueCastle
{
    class EndingRoomObj : RoomObj
    {
        private SpriteObj m_endingMask;
        private List<Vector2> m_cameraPosList;
        private int m_waypointIndex;

        private List<SpriteObj> m_frameList;
        private List<TextObj> m_nameList;
        private List<TextObj> m_slainCountText;
        private List<SpriteObj> m_plaqueList;

        private BackgroundObj m_background;
        private KeyIconTextObj m_continueText;
        private bool m_displayingContinueText;
        private float m_waypointSpeed = 5;

        private EnemyObj_Blob m_blobBoss;

        public EndingRoomObj()
        {
            m_plaqueList = new List<SpriteObj>();
        }

        public override void InitializeRenderTarget(RenderTarget2D bgRenderTarget)
        {
            if (m_background != null)
                m_background.Dispose();
            m_background = new BackgroundObj("LineageScreenBG_Sprite");
            m_background.SetRepeated(true, true, Game.ScreenManager.Camera);
            m_background.X -= 1320 * 5;
            m_background.Opacity = 0.7f;

            base.InitializeRenderTarget(bgRenderTarget);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
        {
            m_continueText = new KeyIconTextObj(Game.JunicodeFont);
            m_continueText.FontSize = 14;
            m_continueText.Align = Types.TextAlign.Right;
            m_continueText.Position = new Vector2(1320 - 50, 650);
            m_continueText.ForceDraw = true;
            m_continueText.Opacity = 0;
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_continueText); // dummy locID to add TextObj to language refresh list

            m_background = new BackgroundObj("LineageScreenBG_Sprite");
            m_background.SetRepeated(true, true, Game.ScreenManager.Camera);
            m_background.X -= 1320 * 5;
            m_background.Opacity = 0.7f;

            m_endingMask = new SpriteObj("Blank_Sprite");
            m_endingMask.ForceDraw = true;
            m_endingMask.TextureColor = Color.Black;
            m_endingMask.Scale = new Vector2(1330f/m_endingMask.Width, 730f/m_endingMask.Height);

            m_cameraPosList = new List<Vector2>();
            m_frameList = new List<SpriteObj>();
            m_nameList = new List<TextObj>();
            m_slainCountText = new List<TextObj>();

            foreach (GameObj obj in GameObjList)
            {
                if (obj is WaypointObj)
                    m_cameraPosList.Add(new Vector2());
            }

            // flibit didn't like this
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            foreach (GameObj obj in GameObjList)
            {
                if (obj is WaypointObj)
                {
                    int index = int.Parse(obj.Name, NumberStyles.Any, ci);
                    m_cameraPosList[index] = obj.Position;
                }
            }

            float frameOffset = 150;

            foreach (EnemyObj enemy in EnemyList)
            {
                enemy.Initialize();
                enemy.PauseEnemy(true);
                enemy.IsWeighted = false;
                enemy.PlayAnimation(true);
                enemy.UpdateCollisionBoxes();

                SpriteObj frame = new SpriteObj("LineageScreenFrame_Sprite");
                frame.DropShadow = new Vector2(4, 6);
                //if (enemy.Width > 150 || enemy.Height > 150)
                if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                {
                    frame.ChangeSprite("GiantPortrait_Sprite");
                         FixMiniboss(enemy);
                }

                frame.Scale = new Vector2((enemy.Width + frameOffset) / frame.Width, (enemy.Height + frameOffset) / frame.Height);
                if (frame.ScaleX < 1) frame.ScaleX = 1;
                if (frame.ScaleY < 1) frame.ScaleY = 1;

                frame.Position = new Vector2(enemy.X, enemy.Bounds.Top + (enemy.Height / 2f));
                m_frameList.Add(frame);

                TextObj name = new TextObj(Game.JunicodeFont);
                name.FontSize = 12;
                name.Align = Types.TextAlign.Centre;
                name.Text = LocaleBuilder.getString(enemy.LocStringID, name);
                name.OutlineColour = new Color(181, 142, 39);
                name.OutlineWidth = 2;
                name.Position = new Vector2(frame.X, frame.Bounds.Bottom + 40);
                m_nameList.Add(name);

                TextObj numSlain = new TextObj(Game.JunicodeFont);
                numSlain.FontSize = 10;
                numSlain.Align = Types.TextAlign.Centre;
                numSlain.OutlineColour = new Color(181, 142, 39);
                numSlain.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", numSlain); // dummy locID to add TextObj to language refresh list
                numSlain.OutlineWidth = 2;
                numSlain.HeadingX = enemy.Type;
                numSlain.HeadingY = (int)enemy.Difficulty;
                numSlain.Position = new Vector2(frame.X, frame.Bounds.Bottom + 80);
                m_slainCountText.Add(numSlain);

                // Some enemy fixes needed here.
                switch (enemy.Type)
                {
                    case (EnemyType.Skeleton):
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                        {
                            if (enemy.Flip == SpriteEffects.None)
                                enemy.X -= 25;
                            else
                                enemy.X += 25;
                        }
                        break;
                    case (EnemyType.Fairy):
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                        {
                            enemy.X += 30;
                            enemy.Y -= 20;
                        }
                        break;
                    case (EnemyType.Zombie):
                        enemy.ChangeSprite("EnemyZombieWalk_Character");
                        enemy.PlayAnimation(true);
                        break;
                    case (EnemyType.BallAndChain):
                        (enemy as EnemyObj_BallAndChain).BallAndChain.Visible = false;
                        (enemy as EnemyObj_BallAndChain).BallAndChain2.Visible = false;
                        break;
                    case (EnemyType.Portrait):
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.MINIBOSS)
                            frame.Visible = false;
                        break;
                    case (EnemyType.LastBoss):
                        if (enemy.Difficulty == GameTypes.EnemyDifficulty.ADVANCED)
                        {
                            (enemy as EnemyObj_LastBoss).ForceSecondForm(true);
                            enemy.ChangeSprite("EnemyLastBossIdle_Character");
                            enemy.PlayAnimation(true);
                        }

                        frame.ChangeSprite("GiantPortrait_Sprite");
                        frame.Scale = Vector2.One;
                        frame.Scale = new Vector2((enemy.Width + frameOffset) / frame.Width, (enemy.Height + frameOffset) / frame.Height);
                        name.Position = new Vector2(frame.X, frame.Bounds.Bottom + 40);
                        numSlain.Position = new Vector2(frame.X, frame.Bounds.Bottom + 80);
                        break;
                }

                SpriteObj plaque = new SpriteObj("LineageScreenPlaque1Long_Sprite");
                plaque.Scale = new Vector2(1.8f, 1.8f);
                plaque.Position = new Vector2(frame.X, frame.Bounds.Bottom + 80);
                m_plaqueList.Add(plaque);
            }

            base.LoadContent(graphics);
        }

        // Bosses aren't displayed properly, so they need to be manually fixed.
        private void FixMiniboss(EnemyObj enemy)
        {
            switch (enemy.Type)
            {
                case (EnemyType.Eyeball):
                    enemy.ChangeSprite("EnemyEyeballBossEye_Character");
                    (enemy as EnemyObj_Eyeball).ChangeToBossPupil();
                    break;
                case (EnemyType.Blob):
                    m_blobBoss = enemy as EnemyObj_Blob;
                    enemy.ChangeSprite("EnemyBlobBossIdle_Character");
                    enemy.GetChildAt(0).TextureColor = Color.White;
                    enemy.GetChildAt(2).TextureColor = Color.LightSkyBlue;
                    enemy.GetChildAt(2).Opacity = 0.8f;
                    (enemy.GetChildAt(1) as SpriteObj).OutlineColour = Color.Black;
                    enemy.GetChildAt(1).TextureColor = Color.Black;
                    break;
                case (EnemyType.Fireball):
                    enemy.ChangeSprite("EnemyGhostBossIdle_Character");
                    break;
                case (EnemyType.Fairy):
                    enemy.ChangeSprite("EnemyFairyGhostBossIdle_Character");
                    break;
                case (EnemyType.Skeleton):
                    if (enemy.Flip == SpriteEffects.None)
                    {
                        enemy.Name = "Berith";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_108";
                    }
                    else
                    {
                        enemy.Name = "Halphas";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_111";
                    }
                    break;
                case (EnemyType.EarthWizard):
                    if (enemy.Flip == SpriteEffects.None)
                    {
                        enemy.Name = "Amon";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_109";
                    }
                    else
                    {
                        enemy.Name = "Barbatos";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_106";
                    }
                    break;
                case (EnemyType.Plant):
                    if (enemy.Flip == SpriteEffects.None)
                    {
                        enemy.Name = "Stolas";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_107";
                    }
                    else
                    {
                        enemy.Name = "Focalor";
                        enemy.LocStringID = "LOC_ID_ENEMY_NAME_110";
                    }
                    break;
            }

            enemy.PlayAnimation(true);
        }

        public override void OnEnter()
        {
            // Special handling for some enemies since InitializeEV overrides anything I set in LoadContent().
            m_blobBoss.PlayAnimation(true);

            foreach (EnemyObj enemy in EnemyList)
            {
                if (enemy.Type == EnemyType.EarthWizard)
                    (enemy as EnemyObj_EarthWizard).EarthProjectile.Visible = false;
            }

            m_displayingContinueText = false;
            UpdateEnemiesSlainText();
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_ENDING_ROOM_OBJ_2_NEW", m_continueText);
            m_continueText.Opacity = 0;
            //Tween.To(m_continueText, 1, Tween.EaseNone, "delay", "2", "Opacity", "1");

            Player.AttachedLevel.Camera.Position = new Vector2(0, 720 / 2f);
            Player.Position = new Vector2(100, 100);
            m_waypointIndex = 1;
            Player.ForceInvincible = true;
            Player.AttachedLevel.SetMapDisplayVisibility(false);
            Player.AttachedLevel.SetPlayerHUDVisibility(false);

            SoundManager.PlayMusic("EndSongDrums", true, 1);
            Game.PlayerStats.TutorialComplete = true;
            Player.LockControls();
            Player.Visible = false;
            Player.Opacity = 0;
            Player.AttachedLevel.CameraLockedToPlayer = false;
            base.OnEnter();
            ChangeWaypoints();
        }

        private void UpdateEnemiesSlainText()
        {
            // for simplicity, HeadingX and HeadingY in the m_numSlainTextObjs are the enemy type and difficulty respectively.
            foreach (TextObj slainText in m_slainCountText)
            {
                int enemyType = (byte)slainText.HeadingX;
                int enemyDifficulty = (int)slainText.HeadingY;
                int numEnemiesSlain = 0;

                switch (enemyDifficulty)
                {
                    case (0):
                        numEnemiesSlain = (int)Game.PlayerStats.EnemiesKilledList[enemyType].X;
                        break;
                    case (1):
                        numEnemiesSlain = (int)Game.PlayerStats.EnemiesKilledList[enemyType].Y;
                        break;
                    case (2):
                        numEnemiesSlain = (int)Game.PlayerStats.EnemiesKilledList[enemyType].Z;
                        break;
                    case (3):
                        numEnemiesSlain = (int)Game.PlayerStats.EnemiesKilledList[enemyType].W;
                        break;
                }

                slainText.Text = LocaleBuilder.getResourceString("LOC_ID_ENDING_ROOM_OBJ_1") + ": " + numEnemiesSlain;
            }
        }

        public void ChangeWaypoints()
        {
            if (m_waypointIndex < m_cameraPosList.Count)
            {
                Tween.To(Player.AttachedLevel.Camera, 1.5f, Quad.EaseInOut, "X", m_cameraPosList[m_waypointIndex].X.ToString(), "Y", m_cameraPosList[m_waypointIndex].Y.ToString());
                Tween.To(Player, 1.5f, Quad.EaseInOut, "X", m_cameraPosList[m_waypointIndex].X.ToString(), "Y", m_cameraPosList[m_waypointIndex].Y.ToString()); // Player needs to follow camera otherwise barbarian shout effect will appear.

                m_waypointIndex++;
                if (m_waypointIndex > m_cameraPosList.Count - 1)
                {
                    m_waypointIndex = 0;
                    Tween.RunFunction(0, Player.AttachedLevel.ScreenManager, "DisplayScreen", ScreenType.Credits, true, typeof(List<object>));
                }
                else
                    Tween.RunFunction(m_waypointSpeed, this, "ChangeWaypoints");
            }
        }

        public void ChangeLevelType()
        {
            this.LevelType = GameTypes.LevelType.DUNGEON;
            Player.AttachedLevel.UpdateLevel(this.LevelType);
        }

        public override void Update(GameTime gameTime)
        {
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) 
                || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                 || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                if (m_displayingContinueText == true)
                {
                    Tween.StopAll(false);
                    Game.ScreenManager.DisplayScreen(ScreenType.Credits, true);
                }
                else
                {
                    m_displayingContinueText = true;
                    Tween.StopAllContaining(m_continueText, false);
                    Tween.To(m_continueText, 0.5f, Tween.EaseNone, "Opacity", "1");
                    Tween.RunFunction(4, this, "HideContinueText");
                }
            }

            base.Update(gameTime);
        }

        public void HideContinueText()
        {
            m_displayingContinueText = false;
            Tween.To(m_continueText, 0.5f, Tween.EaseNone, "delay", "0", "Opacity", "0");
        }


        public override void Draw(Camera2D camera)
        {
            m_continueText.Position = new Vector2(camera.Bounds.Right - 50, camera.Bounds.Bottom - 70);

            m_endingMask.Position = camera.Position - new Vector2(660, 360);
            m_endingMask.Draw(camera);

            // Hack to make an infinite scroll.
            if (camera.X > m_background.X + 1320 * 5)
                m_background.X = camera.X;
            if (camera.X < m_background.X)
                m_background.X = camera.X - 1320;
            m_background.Draw(camera);

            foreach (SpriteObj frame in m_frameList)
                frame.Draw(camera);

            foreach (SpriteObj plaque in m_plaqueList)
                plaque.Draw(camera);

            base.Draw(camera);
            camera.End();
            if (LevelEV.SHOW_ENEMY_RADII == false)
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, camera.GetTransformation()); // Set SpriteSortMode to immediate to allow instant changes to samplerstates.
            else
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, camera.GetTransformation());

            foreach (TextObj name in m_nameList)
                name.Draw(camera);

            foreach (TextObj numSlain in m_slainCountText)
                numSlain.Draw(camera);

            m_continueText.Draw(camera);

            camera.End();

            if (LevelEV.SHOW_ENEMY_RADII == false)
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.GetTransformation()); // Set SpriteSortMode to immediate to allow instant changes to samplerstates.
            else
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, null, null, null, camera.GetTransformation());
        }

        protected override GameObj CreateCloneInstance()
        {
            return new EndingRoomObj();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                foreach (SpriteObj frame in m_frameList)
                    frame.Dispose();
                m_frameList.Clear();
                m_frameList = null;

                foreach (SpriteObj plaque in m_plaqueList)
                    plaque.Dispose();
                m_plaqueList.Clear();
                m_plaqueList = null;

                m_cameraPosList.Clear();
                m_cameraPosList = null;

                foreach (TextObj name in m_nameList)
                    name.Dispose();
                m_nameList.Clear();
                m_nameList = null;

                foreach (TextObj numSlain in m_slainCountText)
                    numSlain.Dispose();
                m_slainCountText.Clear();
                m_slainCountText = null;

                m_endingMask.Dispose();
                m_endingMask = null;

                m_continueText.Dispose();
                m_continueText = null;

                m_background.Dispose();
                m_background = null;

                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            UpdateEnemiesSlainText();
            //m_continueText.Text = LocaleBuilder.getResourceString("LOC_ID_ENDING_ROOM_OBJ_2") + " [Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_ENDING_ROOM_OBJ_3");
            base.RefreshTextObjs();
        }
    }
}

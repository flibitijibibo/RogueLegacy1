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
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class IntroRoomObj : RoomObj
    {
        private List<RaindropObj> m_rainFG;
        private Cue m_rainSFX;
        private TextObj m_introText;

        private SpriteObj m_mountain1, m_mountain2;
        private GameObj m_tree1, m_tree2, m_tree3;
        private GameObj m_fern1, m_fern2, m_fern3;

        private float m_smokeCounter = 0;
        private bool m_inSecondPart = false;

        public IntroRoomObj()
        {
            m_rainFG = new List<RaindropObj>();
            for (int i = 0; i < 500; i++)
            {
                RaindropObj rain = new RaindropObj(new Vector2(CDGMath.RandomInt(-100, 1320 * 2), CDGMath.RandomInt(-400, 720)));
                m_rainFG.Add(rain);
            }
        }

        public override void Initialize()
        {
            TerrainObj blacksmithTerrain = null;
            TerrainObj enchantressTerrain = null;
            TerrainObj architectTerrain = null;
            TerrainObj signTerrain = null;

            foreach (TerrainObj obj in TerrainObjList)
            {
                if (obj.Name == "BlacksmithBlock")
                    blacksmithTerrain = obj;
                if (obj.Name == "EnchantressBlock")
                    enchantressTerrain = obj;
                if (obj.Name == "ArchitectBlock")
                    architectTerrain = obj;
                if (obj.Name == "SignBlock")
                    signTerrain = obj;
            }

            if (blacksmithTerrain != null)
                TerrainObjList.Remove(blacksmithTerrain);
            if (enchantressTerrain != null)
                TerrainObjList.Remove(enchantressTerrain);
            if (architectTerrain != null)
                TerrainObjList.Remove(architectTerrain);
            if (signTerrain != null)
                TerrainObjList.Remove(signTerrain);

            if (m_tree1 == null)
            {
                foreach (GameObj obj in GameObjList)
                {
                    if (obj.Name == "Mountains 1")
                        m_mountain1 = obj as SpriteObj;
                    else if (obj.Name == "Mountains 2")
                        m_mountain2 = obj as SpriteObj;
                    else if (obj.Name == "Sign")
                        obj.Visible = false;

                    if (obj.Name == "Tree1")
                        m_tree1 = obj;
                    else if (obj.Name == "Tree2")
                        m_tree2 = obj;
                    else if (obj.Name == "Tree3")
                        m_tree3 = obj;
                    else if (obj.Name == "Fern1")
                        m_fern1 = obj;
                    else if (obj.Name == "Fern2")
                        m_fern2 = obj;
                    else if (obj.Name == "Fern3")
                        m_fern3 = obj;
                }
            }

            EnemyList.Clear();

            base.Initialize();
        }

        public override void LoadContent(GraphicsDevice graphics)
        {
            m_introText = new TextObj(Game.JunicodeLargeFont);
            m_introText.Text = LocaleBuilder.getString("LOC_ID_INTRO_ROOM_OBJ_1", m_introText); // "My duties are to my family...";//"I did everything they asked...";
            m_introText.FontSize = 25;
            m_introText.ForceDraw = true;
            m_introText.Align = Types.TextAlign.Centre;
            m_introText.Position = new Vector2(1320 / 2f, 720 / 2f - 100);
            m_introText.OutlineWidth = 2;

            base.LoadContent(graphics);
        }

        public override void OnEnter()
        {
            SoundManager.StopMusic();
            Player.CurrentHealth = Player.MaxHealth;
            Player.CurrentMana = Player.MaxMana;

            Game.PlayerStats.HeadPiece = 7;
            Player.AttachedLevel.SetMapDisplayVisibility(false);
            Player.AttachedLevel.SetPlayerHUDVisibility(false);

            if (m_rainSFX != null)
                m_rainSFX.Dispose();
            m_rainSFX = SoundManager.PlaySound("Rain1");

            //Player.AttachedLevel.LightningEffectTwice();

            Player.UpdateCollisionBoxes(); // Necessary check since the OnEnter() is called before player can update its collision boxes.
            Player.Position = new Vector2(10, 720 - 60 - (Player.Bounds.Bottom - Player.Y));
            Player.State = 1; // Force the player into a walking state. This state will not update until the logic set is complete.
            LogicSet playerMoveLS = new LogicSet(Player);
            playerMoveLS.AddAction(new RunFunctionLogicAction(Player, "LockControls"));
            playerMoveLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
            playerMoveLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character"));
            playerMoveLS.AddAction(new PlayAnimationLogicAction(true));
            playerMoveLS.AddAction(new DelayLogicAction(2.5f));
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "CurrentSpeed", 0));
            Player.RunExternalLogicSet(playerMoveLS); // Do not dispose this logic set. The player object will do it on its own.

            Tween.RunFunction(2, this, "Intro1");
            base.OnEnter();
        }

        public void Intro1()
        {
            List<object> introTextObj = new List<object>();
            introTextObj.Add(1.0f);
            introTextObj.Add(0.2f);
            introTextObj.Add(4f);
            introTextObj.Add(true);
            introTextObj.Add(m_introText);
            introTextObj.Add(false);
            Game.ScreenManager.DisplayScreen(ScreenType.Text, false, introTextObj);
            Tween.RunFunction(3, this, "Intro2");
        }

        public void Intro2()
        {
            m_inSecondPart = true;
            //Player.AttachedLevel.RunCinematicBorders(10.5f);
            //Player.Position = new Vector2(this.Bounds.Right - 450, this.Bounds.Bottom - 300);
            Tween.RunFunction(3, Player.AttachedLevel, "LightningEffectTwice");
            //Player.State = 1; // Force the player into a walking state. This state will not update until the logic set is complete.
            LogicSet playerMoveLS = new LogicSet(Player);
            playerMoveLS.AddAction(new DelayLogicAction(5f));
            playerMoveLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
            playerMoveLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character"));
            playerMoveLS.AddAction(new PlayAnimationLogicAction(true));
            playerMoveLS.AddAction(new DelayLogicAction(0.7f));
            playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "CurrentSpeed", 0));
            Player.RunExternalLogicSet(playerMoveLS);
            Tween.RunFunction(5.3f, this, "Intro3");
        }

        public void Intro3()
        {
            List<object> introTextObj = new List<object>();
            m_introText.Text = LocaleBuilder.getString("LOC_ID_INTRO_ROOM_OBJ_2", m_introText); //"But I am loyal only to myself.";//"And I received nothing in return...";
            introTextObj.Add(1.0f);
            introTextObj.Add(0.2f);
            introTextObj.Add(4f);
            introTextObj.Add(true);
            introTextObj.Add(m_introText);
            introTextObj.Add(false);
            Game.ScreenManager.DisplayScreen(ScreenType.Text, false, introTextObj);
            Tween.RunFunction(4, this, "Intro4");
        }

        public void Intro4()
        {
            Player.Position = new Vector2(this.Bounds.Right - 450, this.Bounds.Bottom - 300);
            Player.X += 700;
            Player.Y = 600;
            m_inSecondPart = false;
        }

        public override void OnExit()
        {
            if (m_rainSFX != null && m_rainSFX.IsDisposed == false)
                m_rainSFX.Stop(AudioStopOptions.Immediate);
        }

        public override void Update(GameTime gameTime)
        {
            float totalGameTime = Game.TotalGameTime;

            m_tree1.Rotation = -(float)Math.Sin(totalGameTime) * 2;
            m_tree2.Rotation = (float)Math.Sin(totalGameTime * 2);
            m_tree3.Rotation = (float)Math.Sin(totalGameTime * 2) * 2;
            m_fern1.Rotation = (float)Math.Sin(totalGameTime * 3f) / 2;
            m_fern2.Rotation = -(float)Math.Sin(totalGameTime * 4f);
            m_fern3.Rotation = (float)Math.Sin(totalGameTime * 4f) / 2f;

            foreach (RaindropObj raindrop in m_rainFG)
                raindrop.Update(this.TerrainObjList, gameTime);

            if (m_inSecondPart == true)
            {
                if (m_smokeCounter < 0.2f)
                {
                    m_smokeCounter += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_smokeCounter >= 0.2f)
                    {
                        m_smokeCounter = 0;
                        Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 300, Player.AttachedLevel.Camera.Bounds.Top));
                        Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 380, Player.AttachedLevel.Camera.Bounds.Top));
                        //Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 460, Player.AttachedLevel.Camera.Bounds.Top));
                        //Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 540, Player.AttachedLevel.Camera.Bounds.Top));
                        //Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 620, Player.AttachedLevel.Camera.Bounds.Top));
                        //Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 700, Player.AttachedLevel.Camera.Bounds.Top));
                        //Player.AttachedLevel.ImpactEffectPool.DisplayMassiveSmoke(new Vector2(Player.AttachedLevel.Camera.X + 780, Player.AttachedLevel.Camera.Bounds.Top));
                    }
                }

            }

            base.Update(gameTime);
        }

        public override void Draw(Camera2D camera)
        {
            m_mountain1.X = camera.TopLeftCorner.X * 0.5f;
            m_mountain2.X = m_mountain1.X + 2640; // 2640 not 1320 because it is mountain1 flipped.

            base.Draw(camera);
            camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320 * 2, 720), Color.Black * 0.6f);
            foreach (RaindropObj raindrop in m_rainFG)
                raindrop.Draw(camera);

            if (m_inSecondPart == true)
                camera.Draw(Game.GenericTexture, new Rectangle(1650, 0, 3000, 720), Color.Black);
        }


        public override void PauseRoom()
        {
            foreach (RaindropObj rainDrop in m_rainFG)
                rainDrop.PauseAnimation();

            if (m_rainSFX != null)
                m_rainSFX.Pause();

            //base.PauseRoom();
        }

        public override void UnpauseRoom()
        {
            foreach (RaindropObj rainDrop in m_rainFG)
                rainDrop.ResumeAnimation();

            if (m_rainSFX != null && m_rainSFX.IsPaused)
                m_rainSFX.Resume();

            //base.UnpauseRoom();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                foreach (RaindropObj raindrop in m_rainFG)
                    raindrop.Dispose();
                m_rainFG.Clear();
                m_rainFG = null;

                m_mountain1 = null;
                m_mountain2 = null;

                if (m_rainSFX != null)
                    m_rainSFX.Dispose();
                m_rainSFX = null;

                m_tree1 = null;
                m_tree2 = null;
                m_tree3 = null;
                m_fern1 = null;
                m_fern2 = null;
                m_fern3 = null;

                m_introText.Dispose();
                m_introText = null;

                base.Dispose();
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new IntroRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }
    }
}

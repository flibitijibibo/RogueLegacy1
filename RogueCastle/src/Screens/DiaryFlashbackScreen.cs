using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    class DiaryFlashbackScreen : Screen
    {

        private BackgroundObj m_background;
        private List<LineageObj> m_lineageArray;
        private Vector2 m_storedCameraPos;
        private RenderTarget2D m_sepiaRT;
        private SpriteObj m_filmGrain;

        public float BackBufferOpacity { get; set; }

        public DiaryFlashbackScreen()
        {
            m_lineageArray = new List<LineageObj>();
        }

        public override void LoadContent()
        {
            m_filmGrain = new SpriteObj("FilmGrain_Sprite");
            m_filmGrain.ForceDraw = true;
            m_filmGrain.Scale = new Vector2(2.015f, 2.05f);
            m_filmGrain.X -= 5;
            m_filmGrain.Y -= 5;
            m_filmGrain.PlayAnimation(true);
            m_filmGrain.AnimationDelay = 1 / 30f;

            base.LoadContent();
        }

        public override void ReinitializeRTs()
        {
            m_sepiaRT = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            if (m_background != null)
                m_background.Dispose();
            m_background = new BackgroundObj("LineageScreenBG_Sprite");
            m_background.SetRepeated(true, true, Camera);
            m_background.X -= 1320 * 5;
            base.ReinitializeRTs();
        }

        public override void OnEnter()
        {
            GameUtil.UnlockAchievement("LOVE_OF_BOOKS");

            BackBufferOpacity = 0;
            Tween.To(this, 0.05f, Tween.EaseNone, "BackBufferOpacity", "1");
            BackBufferOpacity = 1;
            Tween.To(this, 1, Tween.EaseNone, "delay", "0.1", "BackBufferOpacity", "0");
            BackBufferOpacity = 0;

            m_storedCameraPos = Camera.Position;
            Camera.Position = Vector2.Zero;

            // Yet another hack. Can't put this in LoadContent because Camera isn't assigned at that time.
            if (m_background == null)
            {
                m_sepiaRT = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                m_background = new BackgroundObj("LineageScreenBG_Sprite");
                m_background.SetRepeated(true, true, Camera);
                m_background.X -= 1320 * 5;
            }

            //CreateLineageObjs();
            CreateLineageObjDebug();
            Camera.X = m_lineageArray[m_lineageArray.Count - 1].X;
            SoundManager.PlaySound("Cutsc_Thunder");
            Tween.RunFunction(1, this, "Cutscene1");

            base.OnEnter();
        }

        public void Cutscene1()
        {
            SoundManager.PlaySound("Cutsc_PictureMove");
            Tween.To(Camera, m_lineageArray.Count * 0.2f, Quad.EaseInOut, "X", m_lineageArray[0].X.ToString());
            Tween.AddEndHandlerToLastTween(this, "Cutscene2");
        }

        public void Cutscene2()
        {
            LineageObj frame = m_lineageArray[0];
            frame.ForceDraw = true;
            Tween.RunFunction(1, frame, "DropFrame");
            Tween.RunFunction(4.5f, this, "ExitTransition");
        }

        public void ExitTransition()
        {
            SoundManager.PlaySound("Cutsc_Picture_Break");
            Tween.To(this, 0.05f, Tween.EaseNone, "BackBufferOpacity", "1");
            Tween.RunFunction(0.1f, ScreenManager, "HideCurrentScreen");
        }

        public override void OnExit()
        {
            foreach (LineageObj obj in m_lineageArray)
                obj.Dispose();
            m_lineageArray.Clear();
            Camera.Position = m_storedCameraPos;
            base.OnExit();
        }

        private void CreateLineageObjs()
        {
            int currentEra = GameEV.BASE_ERA;
            int xPosOffset = 400;
            int xPos = 0;

            if (Game.PlayerStats.FamilyTreeArray.Count > 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    FamilyTreeNode treeData = Game.PlayerStats.FamilyTreeArray[i];

                    LineageObj lineageObj = new LineageObj(null, true);
                    lineageObj.IsDead = true;
                    lineageObj.Age = treeData.Age;
                    lineageObj.ChildAge = treeData.ChildAge;
                    lineageObj.Class = treeData.Class;
                    lineageObj.RomanNumeral = treeData.RomanNumeral;
                    lineageObj.IsFemale = treeData.IsFemale;
                    lineageObj.PlayerName = treeData.Name;
                    lineageObj.SetPortrait(treeData.HeadPiece, treeData.ShoulderPiece, treeData.ChestPiece);
                    lineageObj.NumEnemiesKilled = treeData.NumEnemiesBeaten;
                    lineageObj.BeatenABoss = treeData.BeatenABoss;
                    lineageObj.SetTraits(treeData.Traits);
                    lineageObj.UpdateAge(currentEra);
                    lineageObj.UpdateData();
                    lineageObj.UpdateClassRank();
                    currentEra = currentEra + lineageObj.Age;

                    lineageObj.X = xPos;
                    xPos += xPosOffset;

                    m_lineageArray.Add(lineageObj);
                }
            }
            else
            {
                foreach (FamilyTreeNode treeData in Game.PlayerStats.FamilyTreeArray)
                {
                    LineageObj lineageObj = new LineageObj(null, true);
                    lineageObj.IsDead = true;
                    lineageObj.Age = treeData.Age;
                    lineageObj.ChildAge = treeData.ChildAge;
                    lineageObj.Class = treeData.Class;
                    lineageObj.IsFemale = treeData.IsFemale;
                    lineageObj.RomanNumeral = treeData.RomanNumeral;
                    lineageObj.PlayerName = treeData.Name;
                    lineageObj.SetPortrait(treeData.HeadPiece, treeData.ShoulderPiece, treeData.ChestPiece);
                    lineageObj.NumEnemiesKilled = treeData.NumEnemiesBeaten;
                    lineageObj.BeatenABoss = treeData.BeatenABoss;
                    lineageObj.SetTraits(treeData.Traits);
                    lineageObj.UpdateAge(currentEra);
                    lineageObj.UpdateData();
                    lineageObj.UpdateClassRank();
                    currentEra = currentEra + lineageObj.Age;

                    lineageObj.X = xPos;
                    xPos += xPosOffset;

                    m_lineageArray.Add(lineageObj);
                }
            }
        }

        private void CreateLineageObjDebug()
        {
            int currentEra = GameEV.BASE_ERA;
            int xPosOffset = 400;
            int xPos = 0;

            for (int i = 0; i < 10; i++)
            {
                FamilyTreeNode treeData;

                if (i > Game.PlayerStats.FamilyTreeArray.Count - 1)
                {
                    treeData = new FamilyTreeNode()
                    {
                        Age = (byte)CDGMath.RandomInt(15, 30),
                        ChildAge = (byte)CDGMath.RandomInt(15, 30),
                        Name = Game.NameArray[CDGMath.RandomInt(0, Game.NameArray.Count - 1)],
                        HeadPiece = (byte)CDGMath.RandomInt(1, 5),
                        ShoulderPiece = (byte)CDGMath.RandomInt(1, 5),
                        ChestPiece = (byte)CDGMath.RandomInt(1, 5),
                    };
                }
                else
                    treeData = Game.PlayerStats.FamilyTreeArray[i];

                LineageObj lineageObj = new LineageObj(null, true);
                lineageObj.IsDead = true;
                lineageObj.Age = treeData.Age;
                lineageObj.ChildAge = treeData.ChildAge;
                lineageObj.Class = treeData.Class;
                lineageObj.RomanNumeral = treeData.RomanNumeral;
                lineageObj.IsFemale = treeData.IsFemale;
                lineageObj.PlayerName = treeData.Name;
                lineageObj.SetPortrait(treeData.HeadPiece, treeData.ShoulderPiece, treeData.ChestPiece);
                lineageObj.NumEnemiesKilled = treeData.NumEnemiesBeaten;
                lineageObj.BeatenABoss = treeData.BeatenABoss;
                lineageObj.SetTraits(treeData.Traits);
                lineageObj.UpdateAge(currentEra);
                lineageObj.UpdateData();
                lineageObj.UpdateClassRank();
                currentEra = currentEra + lineageObj.Age;

                lineageObj.X = xPos;
                xPos += xPosOffset;

                m_lineageArray.Add(lineageObj);
            }
        }

        public override void Draw(GameTime gametime)
        {
            // Hack to make an infinite scroll.
            if (Camera.X > m_background.X + 1320 * 5)
                m_background.X = Camera.X;
            if (Camera.X < m_background.X)
                m_background.X = Camera.X - 1320;

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Camera.GetTransformation());
            m_background.Draw(Camera);
            foreach (LineageObj obj in m_lineageArray)
                obj.Draw(Camera);
            Camera.End();

            Camera.GraphicsDevice.SetRenderTarget(m_sepiaRT);
            Game.HSVEffect.Parameters["Saturation"].SetValue(0.2f);
            Game.HSVEffect.Parameters["Brightness"].SetValue(0.1f);
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, Game.HSVEffect);
            Camera.Draw((ScreenManager as RCScreenManager).RenderTarget, Vector2.Zero, Color.White);
            Camera.End();

            Camera.GraphicsDevice.SetRenderTarget((ScreenManager as RCScreenManager).RenderTarget);
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
            Color sepia = new Color(180, 150, 80);
            Camera.Draw(m_sepiaRT, Vector2.Zero, sepia);

            m_filmGrain.Draw(Camera);
            Camera.End();

            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.White * BackBufferOpacity);
            Camera.End();


            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Diary Flashback Screen");

                if (m_background != null)
                    m_background.Dispose();
                m_background = null;
                foreach (LineageObj obj in m_lineageArray)
                    obj.Dispose();
                m_lineageArray.Clear();
                m_lineageArray = null;
                m_filmGrain.Dispose();
                m_filmGrain = null;
                if (m_sepiaRT != null)
                    m_sepiaRT.Dispose();
                m_sepiaRT = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            foreach (LineageObj obj in m_lineageArray)
            {
                obj.RefreshTextObjs();
            }
            base.RefreshTextObjs();
        }
    }
}

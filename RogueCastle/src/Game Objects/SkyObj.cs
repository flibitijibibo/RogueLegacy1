using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class SkyObj : GameObj
    {
        private SpriteObj m_moon;
        private BackgroundObj m_differenceCloud;
        private BackgroundObj m_differenceCloud2;
        private BackgroundObj m_differenceCloud3;

        private Vector2 m_moonPos;
        private SpriteObj m_silhouette;
        private bool m_silhouetteFlying;
        private float m_silhouetteTimer;
        private ProceduralLevelScreen m_levelScreen;

        public float MorningOpacity { get; set; }
        
        public SkyObj(ProceduralLevelScreen levelScreen)
        {
            m_levelScreen = levelScreen;
        }

        public void LoadContent(Camera2D camera)
        {
            Vector2 rtScale = new Vector2(2, 2);

            m_moon = new SpriteObj("ParallaxMoon_Sprite");
            m_moon.Position = new Vector2(900, 200);
            if (LevelEV.SAVE_FRAMES == true)
            {
                m_moon.Position /= 2;
                rtScale = Vector2.One;
            }
            m_moon.Scale = rtScale;
            m_moon.ForceDraw = true;

            m_moonPos = m_moon.Position;

            m_differenceCloud = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_differenceCloud.SetRepeated(true, true, camera, SamplerState.LinearWrap);
            m_differenceCloud.Scale = rtScale;

            m_differenceCloud.TextureColor = new Color(10, 10, 80);
            m_differenceCloud.ParallaxSpeed = new Vector2(0.2f, 0);

            m_differenceCloud2 = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_differenceCloud2.SetRepeated(true, true, camera, SamplerState.LinearWrap);
            m_differenceCloud2.Scale = rtScale;
            m_differenceCloud2.Flip = SpriteEffects.FlipHorizontally;
            m_differenceCloud2.TextureColor = new Color(80, 80, 160);
            m_differenceCloud2.X -= 500;
            m_differenceCloud2.ParallaxSpeed = new Vector2(0.4f, 0);

            m_differenceCloud3 = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_differenceCloud3.SetRepeated(true, true, camera, SamplerState.LinearWrap);
            m_differenceCloud3.Scale = rtScale;
            m_differenceCloud3.Flip = SpriteEffects.FlipHorizontally;
            m_differenceCloud3.TextureColor = Color.White;
            m_differenceCloud3.X -= 500;
            m_differenceCloud3.ParallaxSpeed = new Vector2(0.4f, 0);

            m_silhouette = new SpriteObj("GardenBat_Sprite");
            m_silhouette.ForceDraw = true;
            m_silhouette.AnimationDelay = 1 / 20f;
            m_silhouette.Scale = rtScale;
        }

        public void ReinitializeRT(Camera2D camera)
        {
            Vector2 rtScale = new Vector2(2, 2);
            if (LevelEV.SAVE_FRAMES == true)
            {
                m_moon.Position /= 2;
                rtScale = Vector2.One;
            }

            if (m_differenceCloud != null) m_differenceCloud.Dispose();
            m_differenceCloud = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_differenceCloud.SetRepeated(true, true, camera, SamplerState.LinearWrap);
            m_differenceCloud.Scale = rtScale;
            m_differenceCloud.TextureColor = new Color(10, 10, 80);
            m_differenceCloud.ParallaxSpeed = new Vector2(0.2f, 0);

            if (m_differenceCloud2 != null) m_differenceCloud2.Dispose();
            m_differenceCloud2 = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_differenceCloud2.SetRepeated(true, true, camera, SamplerState.LinearWrap);
            m_differenceCloud2.Scale = rtScale;
            m_differenceCloud2.Flip = SpriteEffects.FlipHorizontally;
            m_differenceCloud2.TextureColor = new Color(80, 80, 160);
            m_differenceCloud2.X -= 500;
            m_differenceCloud2.ParallaxSpeed = new Vector2(0.4f, 0);

            if (m_differenceCloud3 != null) m_differenceCloud3.Dispose();
            m_differenceCloud3 = new BackgroundObj("ParallaxDifferenceClouds_Sprite");
            m_differenceCloud3.SetRepeated(true, true, camera, SamplerState.LinearWrap);
            m_differenceCloud3.Scale = rtScale;
            m_differenceCloud3.Flip = SpriteEffects.FlipHorizontally;
            m_differenceCloud3.TextureColor = new Color(80, 80, 160);
            m_differenceCloud3.X -= 500;
            m_differenceCloud3.ParallaxSpeed = new Vector2(0.4f, 0);
        }

        public void Update(GameTime gameTime)
        {
            float elapsedSeconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (m_silhouetteFlying == false)
            {
                if (m_silhouetteTimer > 0)
                {
                    m_silhouetteTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_silhouetteTimer <= 0)
                    {
                        int randInt = CDGMath.RandomInt(1, 100);
                        if (randInt > 95) // 5% chance of showing santa.
                            ShowSilhouette(true);
                        else if (randInt > 65) // 30% chance of other silhouette happening.
                            ShowSilhouette(false);
                        else
                            m_silhouetteTimer = 5;
                    }
                }
            }

            if (m_silhouetteFlying == false && m_silhouetteTimer <= 0)
                m_silhouetteTimer = 5;

            if (m_silhouette.SpriteName == "GardenPerson_Sprite")
                m_silhouette.Rotation += (2f * 60 * elapsedSeconds);
        }

        private void ShowSilhouette(bool showSanta)
        {
            if (m_levelScreen != null)
            {
                m_silhouetteFlying = true;
                m_silhouette.Rotation = 0;
                m_silhouette.Flip = SpriteEffects.None;
                bool appearLeft = false;
                if (CDGMath.RandomInt(0, 1) > 0) // 50% chance of silhouette coming from left or right.
                    appearLeft = true;

                string[] randImage = new string[] { "GardenBat_Sprite", "GardenCrow_Sprite", "GardenBat_Sprite", "GardenCrow_Sprite", "GardenPerson_Sprite" };

                if (showSanta == false)
                    m_silhouette.ChangeSprite(randImage[CDGMath.RandomInt(0, 4)]);
                else
                    m_silhouette.ChangeSprite("GardenSanta_Sprite");

                m_silhouette.PlayAnimation(true);

                Vector2 spawnPt = Vector2.Zero;
                if (appearLeft == true)
                    m_silhouette.X = -m_silhouette.Width;
                else
                {
                    m_silhouette.Flip = SpriteEffects.FlipHorizontally;
                    m_silhouette.X = m_levelScreen.CurrentRoom.Width + m_silhouette.Width;
                }

                m_silhouette.Y = CDGMath.RandomFloat(100, 500);

                int xDistance = m_levelScreen.CurrentRoom.Bounds.Width + m_silhouette.Width * 2;

                if (appearLeft == false)
                    xDistance = -xDistance;

                Tweener.Tween.By(m_silhouette, CDGMath.RandomFloat(10, 15), Tweener.Tween.EaseNone, "X", xDistance.ToString(), "Y", CDGMath.RandomInt(-200, 200).ToString());
                Tweener.Tween.AddEndHandlerToLastTween(this, "SilhouetteComplete");
            }
        }

        public void SilhouetteComplete()
        {
            m_silhouetteFlying = false;
        }

        public override void Draw(Camera2D camera)
        {
            m_moon.X = m_moonPos.X - (camera.TopLeftCorner.X * 0.01f);
            m_moon.Y = m_moonPos.Y - (camera.TopLeftCorner.Y * 0.01f);
            
            camera.GraphicsDevice.Clear(new Color(4, 29, 86));
            camera.Draw(Game.GenericTexture, new Rectangle(-10, -10, 1400, 800), Color.SkyBlue * MorningOpacity);

            m_moon.Opacity = (1 - MorningOpacity);
            m_silhouette.Opacity = (1 - MorningOpacity);
            m_differenceCloud.Opacity = (1 - MorningOpacity);
            m_differenceCloud2.Opacity = (1 - MorningOpacity);
            m_differenceCloud3.Opacity = MorningOpacity;

            m_moon.Draw(camera);
            m_differenceCloud.Draw(camera);
            m_differenceCloud2.Draw(camera);
            m_differenceCloud3.Draw(camera);
            m_silhouette.Draw(camera);

            //Console.WriteLine(m_silhouette.Position + " " + m_levelScreen.CurrentRoom.Position);

            base.Draw(camera);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new SkyObj(m_levelScreen);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // DONE
                m_differenceCloud.Dispose();
                m_differenceCloud = null;
                m_differenceCloud2.Dispose();
                m_differenceCloud2 = null;
                m_differenceCloud3.Dispose();
                m_differenceCloud3 = null;

                m_moon.Dispose();
                m_moon = null;

                m_silhouette.Dispose();
                m_silhouette = null;

                m_levelScreen = null;

                base.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class RaindropObj : SpriteObj
    {
        private float m_speedY, m_speedX;
        public bool IsCollidable { get; set; }
        private Vector2 m_startingPos;
        public Vector2 MaxYSpeed { get; set; }
        public Vector2 MaxXSpeed { get; set; }

        private bool m_splashing = false;
        private bool m_isSnowflake = false;
        private bool m_isParticle = false;

        public RaindropObj(Vector2 startingPos)
            : base("Raindrop_Sprite")
        {
            ChangeToRainDrop();

            m_speedY = CDGMath.RandomFloat(MaxYSpeed.X, MaxYSpeed.Y);
            m_speedX = CDGMath.RandomFloat(MaxXSpeed.X, MaxXSpeed.Y);
            IsCollidable = true;
            m_startingPos = startingPos;
            this.Position = m_startingPos;
            AnimationDelay = 1/30f;
            this.Scale = new Vector2(2, 2);
            //this.TextureColor = new Color(136, 180, 182);
        }

        public void ChangeToSnowflake()
        {
            this.ChangeSprite("Snowflake_Sprite");
            m_isSnowflake = true;
            m_isParticle = false;
            this.Rotation = 0;
            MaxYSpeed = new Vector2(200, 400);
            MaxXSpeed = new Vector2(-200, 0);
            this.Position = m_startingPos;
            m_speedY = CDGMath.RandomFloat(MaxYSpeed.X, MaxYSpeed.Y);
            m_speedX = CDGMath.RandomFloat(MaxXSpeed.X, MaxXSpeed.Y);
        }

        public void ChangeToRainDrop()
        {
            m_isSnowflake = false;
            m_isParticle = false;
            MaxYSpeed = new Vector2(800, 1200);
            MaxXSpeed = new Vector2(-200, -200);
            this.Rotation = 5;
            m_speedY = CDGMath.RandomFloat(MaxYSpeed.X, MaxYSpeed.Y);
            m_speedX = CDGMath.RandomFloat(MaxXSpeed.X, MaxXSpeed.Y);
        }

        public void ChangeToParticle()
        {
            m_isSnowflake = false;
            m_isParticle = true;
            MaxYSpeed = new Vector2(0, 0);
            MaxXSpeed = new Vector2(500, 1500);
            this.Rotation = -90;
            m_speedY = CDGMath.RandomFloat(MaxYSpeed.X, MaxYSpeed.Y);
            m_speedX = CDGMath.RandomFloat(MaxXSpeed.X, MaxXSpeed.Y);
            float randScale = CDGMath.RandomFloat(2, 8);
            this.Scale = new Vector2(randScale, randScale);
        }

        public void Update(List<TerrainObj> collisionList, GameTime gameTime)
        {
            if (m_splashing == false)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                this.Y += m_speedY * elapsedTime;
                this.X += m_speedX * elapsedTime;

                if (IsCollidable == true)
                {
                    Rectangle thisBounds = this.Bounds; // Optimization.  Getting this.Bounds hits memory and it's an expensive function

                    foreach (TerrainObj obj in collisionList)
                    {
                        TerrainObj copyObj = obj; // Optimization -> obj is a reference - rather than hit memory to access values, we'll cache it
                        Rectangle objBounds = copyObj.Bounds; // Optimization.  Bounds is an expensive calc.

                        if (copyObj.Visible == true && copyObj.CollidesTop == true && copyObj.Y > 120 && CollisionMath.Intersects(thisBounds, objBounds))
                        {
                            if (copyObj.Rotation == 0)
                            {
                                if (m_isSnowflake == false)
                                    this.Y = objBounds.Top - 10;
                                RunSplashAnimation();
                            }
                            else if (copyObj.Rotation != 0)
                            {
                                if (CollisionMath.RotatedRectIntersects(thisBounds, 0, Vector2.Zero, new Rectangle((int)copyObj.X, (int)copyObj.Y, copyObj.Width, copyObj.Height), copyObj.Rotation, Vector2.Zero))
                                {
                                    if (m_isSnowflake == false)
                                        this.Y -= 12;
                                    RunSplashAnimation();
                                }
                            }
                            break;
                        }
                    }
                }

                if (this.Y > 720) 
                    RunSplashAnimation(); // force kill the raindrop.
            }

            if (this.IsAnimating == false && m_splashing == true && m_isSnowflake == false)
                KillDrop();
        }

        public void UpdateNoCollision(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.Y += m_speedY * elapsedTime;
            this.X += m_speedX * elapsedTime;

            if ((this.X > m_startingPos.X + 4000) || (this.X < m_startingPos.X - 4000))
                KillDrop();

            if ((this.Y > m_startingPos.Y + 4000) || (this.Y < m_startingPos.Y - 4000))
                KillDrop();
        }

        private void RunSplashAnimation()
        {
            //this.Scale = new Vector2(2, 2);
            m_splashing = true;
            this.Rotation = 0;
            if (m_isSnowflake == false)
                this.PlayAnimation(2, this.TotalFrames, false);
            else
            {
                Tweener.Tween.To(this, 0.25f, Tweener.Tween.EaseNone, "Opacity", "0");
                Tweener.Tween.AddEndHandlerToLastTween(this, "KillDrop");
            }
        }

        public void KillDrop()
        {
            //this.Scale = Vector2.One;
            m_splashing = false;
            this.GoToFrame(1);
            this.Rotation = 5;
            this.X = m_startingPos.X;
            this.Y = CDGMath.RandomInt(-100, 0);
            if (m_isParticle == true)
            {
                this.Y = m_startingPos.Y;
                this.Rotation = -90;
            }
            m_speedY = CDGMath.RandomFloat(MaxYSpeed.X, MaxYSpeed.Y);
            m_speedX = CDGMath.RandomFloat(MaxXSpeed.X, MaxXSpeed.Y);
            this.Opacity = 1;
        }
    }
}

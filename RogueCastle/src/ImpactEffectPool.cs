using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
    public class ImpactEffectPool : IDisposableObj
    {
        private int m_poolSize = 0;
        private DS2DPool<SpriteObj> m_resourcePool;
        private bool m_isPaused;

        public bool IsDisposed { get { return m_isDisposed; } }
        private bool m_isDisposed = false;

        public ImpactEffectPool(int poolSize)
        {
            m_poolSize = poolSize;
            m_resourcePool = new DS2DPool<SpriteObj>();
        }

        public void Initialize()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                SpriteObj poolObj = new SpriteObj("Blank_Sprite");
                poolObj.AnimationDelay = 1 / 30f;
                poolObj.Visible = false;
                poolObj.TextureColor = Color.White;
                m_resourcePool.AddToPool(poolObj);
            }
        }

        public SpriteObj DisplayEffect(Vector2 position, string spriteName)
        {
            SpriteObj effect = m_resourcePool.CheckOut();
            effect.ChangeSprite(spriteName);
            effect.TextureColor = Color.White;
            effect.Visible = true;
            effect.Position = position;
            effect.PlayAnimation(false);
            return effect;
        }

        public void DisplayEnemyImpactEffect(Vector2 position)
        {
            SpriteObj impactEffect = m_resourcePool.CheckOut();
            impactEffect.ChangeSprite("ImpactEnemy_Sprite");
            impactEffect.TextureColor = Color.White;
            impactEffect.Rotation = CDGMath.RandomInt(0, 360);
            impactEffect.Visible = true;
            impactEffect.Position = position;
            impactEffect.PlayAnimation(false);
        }

        public void DisplayPlayerImpactEffect(Vector2 position)
        {
            SpriteObj impactEffect = m_resourcePool.CheckOut();
            impactEffect.ChangeSprite("ImpactEnemy_Sprite");
            impactEffect.TextureColor = Color.Orange;
            impactEffect.Rotation = CDGMath.RandomInt(0, 360);
            impactEffect.Visible = true;
            impactEffect.Position = position;
            impactEffect.PlayAnimation(false);
        }

        public void DisplayBlockImpactEffect(Vector2 position, Vector2 scale)
        {
            SpriteObj impactEffect = m_resourcePool.CheckOut();
            impactEffect.ChangeSprite("ImpactBlock_Sprite");
            impactEffect.TextureColor = Color.White;
            impactEffect.Rotation = CDGMath.RandomInt(0, 360);
            impactEffect.Visible = true;
            impactEffect.Position = position;
            impactEffect.PlayAnimation(false);
            impactEffect.Scale = scale;

        }

        public void DisplayDeathEffect(Vector2 position)
        {
            for (int i = 0; i < 10; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("ExplosionBlue_Sprite");
                smoke.Visible = true;
                smoke.Position = position;
                float randomScale = CDGMath.RandomFloat(0.7f, 0.8f);
                int movementAmount = 50;
                smoke.Scale = new Vector2(randomScale, randomScale);
                smoke.Rotation = CDGMath.RandomInt(0, 90);
                smoke.PlayAnimation(true);

                float duration = CDGMath.RandomFloat(0.5f, 1.0f);
                float randScaleShrink = CDGMath.RandomFloat(0, 0.1f);
                Tween.To(smoke, duration - 0.2f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
                Tween.To(smoke, duration, Back.EaseIn, "ScaleX", randScaleShrink.ToString(), "ScaleY", randScaleShrink.ToString());
                Tween.By(smoke, duration, Quad.EaseOut, "X", CDGMath.RandomInt(-movementAmount, movementAmount).ToString(), "Y", CDGMath.RandomInt(-movementAmount, movementAmount).ToString());
                Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");
                Tween.By(smoke, duration - 0.1f, Tweener.Ease.Quad.EaseOut, "Rotation", CDGMath.RandomInt(145, 190).ToString());
            }
        }


        public void DisplaySpawnEffect(Vector2 position)
        {
            for (int i = 0; i < 10; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("ExplosionOrange_Sprite");
                smoke.Visible = true;
                smoke.Position = position;
                float randomScale = CDGMath.RandomFloat(0.7f, 0.8f);
                int movementAmount = 50;
                smoke.Scale = new Vector2(randomScale, randomScale);
                smoke.Rotation = CDGMath.RandomInt(0, 90);
                smoke.PlayAnimation(true);

                float duration = CDGMath.RandomFloat(0.5f, 1.0f);
                float randScaleShrink = CDGMath.RandomFloat(0, 0.1f);
                Tween.To(smoke, duration - 0.2f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
                Tween.To(smoke, duration, Back.EaseIn, "ScaleX", randScaleShrink.ToString(), "ScaleY", randScaleShrink.ToString());
                Tween.By(smoke, duration, Quad.EaseOut, "X", CDGMath.RandomInt(-movementAmount, movementAmount).ToString(), "Y", CDGMath.RandomInt(-movementAmount, movementAmount).ToString());
                Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");
                Tween.By(smoke, duration - 0.1f, Tweener.Ease.Quad.EaseOut, "Rotation", CDGMath.RandomInt(145, 190).ToString());
            }
        }

        public void DisplayChestSparkleEffect(Vector2 position)
        {
            SpriteObj particle = m_resourcePool.CheckOut();
            particle.ChangeSprite("LevelUpParticleFX_Sprite");
            particle.Visible = true;
            float randomScale = CDGMath.RandomFloat(0.2f, 0.5f);
            particle.Scale = new Vector2(randomScale, randomScale);
            particle.Opacity = 0;
            particle.Position = position;
            particle.Rotation = CDGMath.RandomInt(0, 90);
            particle.PlayAnimation(false);
            particle.Position += new Vector2(CDGMath.RandomInt(-40, 40), CDGMath.RandomInt(-40, 40));
            Tween.To(particle, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.AddEndHandlerToLastTween(particle, "StopAnimation");
        }

        public void DisplayDoubleJumpEffect(Vector2 position)
        {
            SpriteObj doubleJumpFX = m_resourcePool.CheckOut();
            doubleJumpFX.ChangeSprite("DoubleJumpFX_Sprite");
            doubleJumpFX.Visible = true;
            doubleJumpFX.Position = position;
            //doubleJumpFX.Opacity = 0.6f;
            doubleJumpFX.PlayAnimation(false);
        }

        public void DisplayDashEffect(Vector2 position, bool flip)
        {
            SpriteObj dashFX = m_resourcePool.CheckOut();
            dashFX.ChangeSprite("DashFX_Sprite");
            if (flip == true)
                dashFX.Flip = SpriteEffects.FlipHorizontally;
            dashFX.Position = position;
            dashFX.Visible = true;
            dashFX.PlayAnimation(false);
        }

        public void DisplayTeleportEffect(Vector2 position)
        {
            // Animation for the floating rocks.
            float delay = 0.1f;
            for (int i = 0; i < 5; i++)
            {
                SpriteObj rock = m_resourcePool.CheckOut();
                rock.Visible = true;
                rock.ChangeSprite("TeleportRock" + (i + 1) + "_Sprite");
                rock.PlayAnimation(true);
                rock.Position = new Vector2(CDGMath.RandomFloat(position.X - 70, position.X + 70), position.Y + CDGMath.RandomInt(-50, -30));
                rock.Opacity = 0;
                float duration = 1f;

                Tween.To(rock, 0.5f, Linear.EaseNone, "delay", delay.ToString(), "Opacity", "1");
                Tween.By(rock, duration, Linear.EaseNone, "delay", delay.ToString(), "Y", "-150");
                rock.Opacity = 1;
                Tween.To(rock, 0.5f, Linear.EaseNone, "delay", (duration + delay - 0.5f).ToString(), "Opacity", "0");
                Tween.AddEndHandlerToLastTween(rock, "StopAnimation");
                rock.Opacity = 0;
                delay += CDGMath.RandomFloat(0.1f, 0.3f);
            }

            // Animation for the actual beam.
            SpriteObj teleportBeam = m_resourcePool.CheckOut();
            teleportBeam.AnimationDelay = 1/20f;
            teleportBeam.Opacity = 0.8f;
            teleportBeam.Visible = true;
            teleportBeam.ChangeSprite("TeleporterBeam_Sprite");
            teleportBeam.Position = position;
            teleportBeam.ScaleY = 0;
            teleportBeam.PlayAnimation(true);
            Tween.To(teleportBeam, 0.05f, Linear.EaseNone, "ScaleY", "1");
            Tween.To(teleportBeam, 2, Linear.EaseNone);
            Tween.AddEndHandlerToLastTween(teleportBeam, "StopAnimation");
        }

        // Needed so that the thrust dust always appears where the object is currently standing.
        public void DisplayThrustDustEffect(GameObj obj, int numClouds, float duration)
        {
            float cloudDelay = duration / numClouds;
            float initialDelay = 0;

            for (int i = 0; i < numClouds; i++)
            {
                Tween.RunFunction(initialDelay, this, "DisplayDustEffect", obj);
                Tween.RunFunction(initialDelay, this, "DisplayDustEffect", obj);
                initialDelay += cloudDelay;
            }
        }

        // Displays the actual smoke.
        public void DisplayDustEffect(GameObj obj)
        {
            int randX = CDGMath.RandomInt(-30, 30);
            int randY = CDGMath.RandomInt(-30, 30);
            SpriteObj cloud1 = m_resourcePool.CheckOut();
            cloud1.ChangeSprite("ExplosionBrown_Sprite");
            cloud1.Opacity = 0;
            cloud1.Visible = true;
            cloud1.Rotation = CDGMath.RandomInt(0, 270);
            cloud1.Position = new Vector2(obj.X, obj.Bounds.Bottom);
            cloud1.Scale = new Vector2(0.8f, 0.8f);
            cloud1.PlayAnimation(true);
            Tween.To(cloud1, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.By(cloud1, 0.7f, Linear.EaseNone, "Rotation", "180");
            Tween.By(cloud1, 0.7f, Quad.EaseOut, "X", randX.ToString(), "Y", randY.ToString());
            Tween.AddEndHandlerToLastTween(cloud1, "StopAnimation");
            cloud1.Opacity = 1;
            Tween.To(cloud1, 0.5f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
            cloud1.Opacity = 0;
        }

        public void DisplayDustEffect(Vector2 pos)
        {
            int randX = CDGMath.RandomInt(-30, 30);
            int randY = CDGMath.RandomInt(-30, 30);
            SpriteObj cloud1 = m_resourcePool.CheckOut();
            cloud1.ChangeSprite("ExplosionBrown_Sprite");
            cloud1.Opacity = 0;
            cloud1.Visible = true;
            cloud1.Rotation = CDGMath.RandomInt(0, 270);
            cloud1.Position = pos;
            cloud1.Scale = new Vector2(0.8f, 0.8f);
            cloud1.PlayAnimation(true);
            Tween.To(cloud1, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.By(cloud1, 0.7f, Linear.EaseNone, "Rotation", "180");
            Tween.By(cloud1, 0.7f, Quad.EaseOut, "X", randX.ToString(), "Y", randY.ToString());
            Tween.AddEndHandlerToLastTween(cloud1, "StopAnimation");
            cloud1.Opacity = 1;
            Tween.To(cloud1, 0.5f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
            cloud1.Opacity = 0;
        }

        public void TurretFireEffect(Vector2 pos, Vector2 scale)
        {
            int randX = CDGMath.RandomInt(-20, 20);
            int randY = CDGMath.RandomInt(-20, 20);
            SpriteObj cloud1 = m_resourcePool.CheckOut();
            cloud1.ChangeSprite("ExplosionBrown_Sprite");
            cloud1.Opacity = 0;
            cloud1.Visible = true;
            cloud1.Rotation = CDGMath.RandomInt(0, 270);
            cloud1.Position = pos;
            cloud1.Scale = scale;
            cloud1.PlayAnimation(true);
            Tween.To(cloud1, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.By(cloud1, 0.7f, Linear.EaseNone, "Rotation", CDGMath.RandomInt(-50,50).ToString());
            Tween.By(cloud1, 0.7f, Quad.EaseOut, "X", randX.ToString(), "Y", randY.ToString());
            Tween.AddEndHandlerToLastTween(cloud1, "StopAnimation");
            cloud1.Opacity = 1;
            Tween.To(cloud1, 0.4f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
            cloud1.Opacity = 0;
        }

        public void DisplayFartEffect(GameObj obj)
        {
            int randX = CDGMath.RandomInt(-10, 10);
            if (obj.Flip == SpriteEffects.FlipHorizontally)
                randX = 20;
            else
                randX = -20;
            int randY = CDGMath.RandomInt(-10, 10);
            randY = 0;
            SpriteObj cloud1 = m_resourcePool.CheckOut();
            cloud1.ChangeSprite("ExplosionBrown_Sprite");
            cloud1.Opacity = 0;
            cloud1.Visible = true;
            cloud1.Rotation = CDGMath.RandomInt(0, 270);
            cloud1.Position = new Vector2(obj.X, obj.Bounds.Bottom);
            if (obj.Flip == SpriteEffects.FlipHorizontally)
                cloud1.X += 30;
            else
                cloud1.X -= 30;
            cloud1.Scale = new Vector2(0.8f, 0.8f);
            cloud1.PlayAnimation(true);
            Tween.To(cloud1, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.By(cloud1, 0.7f, Linear.EaseNone, "Rotation", "180");
            Tween.By(cloud1, 0.7f, Quad.EaseOut, "X", randX.ToString(), "Y", randY.ToString());
            Tween.AddEndHandlerToLastTween(cloud1, "StopAnimation");
            cloud1.Opacity = 1;
            Tween.To(cloud1, 0.5f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
            cloud1.Opacity = 0;
        }

        // Displays the actual smoke.
        public void DisplayExplosionEffect(Vector2 position)
        {
            SpriteObj deathFX1 = m_resourcePool.CheckOut();
            deathFX1.ChangeSprite("EnemyDeathFX1_Sprite");
            deathFX1.Visible = true;
            deathFX1.Position = position;
            deathFX1.PlayAnimation(false);
            deathFX1.AnimationDelay = 1 / 30f;
            //deathFX1.Scale = new Vector2(1.5f, 1.5f);
            deathFX1.Scale = new Vector2(2f, 2f);
        }

        public void StartInverseEmit(Vector2 pos)
        {
            float duration = 0.4f;
            int numPixels = 30;
            float delay = (float)(duration / numPixels);
            float delayCounter = 0;

            for (int i = 0; i < numPixels; i++)
            {
                SpriteObj pixel = m_resourcePool.CheckOut();
                pixel.ChangeSprite("Blank_Sprite");
                pixel.TextureColor = Color.Black;
                pixel.Visible = true;
                pixel.PlayAnimation(true);
                pixel.Scale = Vector2.Zero;

                pixel.X = pos.X + CDGMath.RandomInt(-100, 100);
                pixel.Y = pos.Y + CDGMath.RandomInt(-100, 100);
                Tween.To(pixel, delayCounter, Linear.EaseNone, "ScaleX", "2", "ScaleY", "2");
                Tween.To(pixel, duration - delayCounter, Quad.EaseInOut, "delay", delayCounter.ToString(), "X", pos.X.ToString(), "Y", pos.Y.ToString());
                Tween.AddEndHandlerToLastTween(pixel, "StopAnimation");
                delayCounter += delay;
            }
        }

        public void StartTranslocateEmit(Vector2 pos)
        {
            int numPixels = 30;

            for (int i = 0; i < numPixels; i++)
            {
                SpriteObj pixel = m_resourcePool.CheckOut();
                pixel.ChangeSprite("Blank_Sprite");
                pixel.TextureColor = Color.White;
                pixel.Visible = true;
                pixel.PlayAnimation(true);
                pixel.Scale = new Vector2(2, 2);
                pixel.Position = pos;

                Tween.To(pixel, 0.5f, Linear.EaseNone,"delay", "0.5", "ScaleX", "0", "ScaleY", "0");
                Tween.By(pixel, 1, Quad.EaseIn, "X", CDGMath.RandomInt(-250, 250).ToString());
                Tween.By(pixel, 1, Quad.EaseIn, "Y", CDGMath.RandomInt(500, 800).ToString());
                Tween.AddEndHandlerToLastTween(pixel, "StopAnimation");
            }
        }

        public void BlackSmokeEffect(GameObj obj)
        {
            for (int i = 0; i < 2; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("BlackSmoke_Sprite");
                smoke.Visible = true;
                smoke.Y = obj.Y;
                smoke.Y += CDGMath.RandomInt(-40, 40);
                smoke.X = obj.Bounds.Left;

                smoke.Opacity = 0;
                smoke.PlayAnimation(true);
                smoke.Rotation = CDGMath.RandomInt(-30, 30);
                if (CDGMath.RandomPlusMinus() < 0)
                    smoke.Flip = SpriteEffects.FlipHorizontally;
                if (CDGMath.RandomPlusMinus() < 0)
                    smoke.Flip = SpriteEffects.FlipVertically;

                int heading = -1;
                if (obj.Flip == SpriteEffects.FlipHorizontally)
                {
                    heading = 1;
                    smoke.X = obj.Bounds.Right;
                }

                Tween.To(smoke, 0.4f, Tween.EaseNone, "Opacity", "1");
                Tween.By(smoke, 1.5f, Quad.EaseInOut, "X", (CDGMath.RandomInt(50, 100) * heading).ToString(), "Y", CDGMath.RandomInt(-100, 100).ToString(), "Rotation", CDGMath.RandomInt(-45, 45).ToString());
                //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", smoke);
             
                smoke.Opacity = 1;
                Tween.To(smoke, 1f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
                smoke.Opacity = 0;
                Tween.RunFunction(2, smoke, "StopAnimation");
            }
        }

        public void BlackSmokeEffect(Vector2 pos, Vector2 scale)
        {
            SpriteObj smoke = m_resourcePool.CheckOut();
            smoke.ChangeSprite("BlackSmoke_Sprite");
            smoke.Visible = true;
            smoke.Y = pos.Y;
            smoke.Y += CDGMath.RandomInt(-40, 40);
            smoke.X = pos.X;
            smoke.X += CDGMath.RandomInt(-40, 40);
            smoke.Scale = scale;

            smoke.Opacity = 0;
            smoke.PlayAnimation(true);
            smoke.Rotation = CDGMath.RandomInt(-30, 30);

            Tween.To(smoke, 0.4f, Tween.EaseNone, "Opacity", "1");
            Tween.By(smoke, 1.5f, Quad.EaseInOut, "X", CDGMath.RandomInt(50, 100).ToString(), "Y", CDGMath.RandomInt(-100, 100).ToString(), "Rotation", CDGMath.RandomInt(-45, 45).ToString());
            //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", smoke);         
            smoke.Opacity = 1;
            Tween.To(smoke, 1f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
            smoke.Opacity = 0;
            Tween.RunFunction(2, smoke, "StopAnimation");
        }

        public void CrowDestructionEffect(Vector2 pos)
        {
            for (int i = 0; i < 2; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("BlackSmoke_Sprite");
                smoke.Visible = true;
                smoke.Position = pos;
                smoke.Opacity = 0;
                smoke.PlayAnimation(true);
                smoke.Rotation = CDGMath.RandomInt(-30, 30);
                Tween.To(smoke, 0.4f, Tween.EaseNone, "Opacity", "1");
                Tween.By(smoke, 1.5f, Quad.EaseInOut, "X", CDGMath.RandomInt(-50, 50).ToString(), "Y", CDGMath.RandomInt(-50, 50).ToString(), "Rotation", CDGMath.RandomInt(-45, 45).ToString());
                //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", smoke);
                Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");
             
                smoke.Opacity = 1;
                Tween.To(smoke, 1f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
                smoke.Opacity = 0;
            }

            for (int i = 0; i < 4; i++)
            {
                SpriteObj feather = m_resourcePool.CheckOut();
                feather.ChangeSprite("CrowFeather_Sprite");
                feather.Visible = true;
                feather.Scale = new Vector2(2, 2);
                feather.Position = pos;
                feather.X += CDGMath.RandomInt(-20, 20);
                feather.Y += CDGMath.RandomInt(-20, 20);
                feather.Opacity = 0;
                feather.PlayAnimation(true);
                feather.Rotation = CDGMath.RandomInt(-30, 30);
                Tween.To(feather, 0.1f, Tween.EaseNone, "Opacity", "1");
                Tween.By(feather, 1.5f, Quad.EaseInOut, "X", CDGMath.RandomInt(-50, 50).ToString(), "Y", CDGMath.RandomInt(-50, 50).ToString(), "Rotation", CDGMath.RandomInt(-180, 180).ToString());
                //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", feather);
                Tween.AddEndHandlerToLastTween(feather, "StopAnimation");
             
                feather.Opacity = 1;
                Tween.To(feather, 1f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
                feather.Opacity = 0;
            }
        }

        public void CrowSmokeEffect(Vector2 pos)
        {
            SpriteObj smoke = m_resourcePool.CheckOut();
            smoke.ChangeSprite("BlackSmoke_Sprite");
            smoke.Visible = true;
            smoke.Y = pos.Y;
            smoke.Y += CDGMath.RandomInt(-40, 40);
            smoke.X = pos.X;
            smoke.X += CDGMath.RandomInt(-40, 40);
            smoke.Scale = new Vector2(0.7f, 0.7f);

            smoke.Opacity = 0;
            smoke.PlayAnimation(true);
            smoke.Rotation = CDGMath.RandomInt(-30, 30);

            Tween.To(smoke, 0.2f, Tween.EaseNone, "Opacity", "1");
            Tween.By(smoke, 1f, Quad.EaseInOut, "X", CDGMath.RandomInt(50, 100).ToString(), "Y", CDGMath.RandomInt(-100, 100).ToString(), "Rotation", CDGMath.RandomInt(-45, 45).ToString());
            smoke.Opacity = 1;
            Tween.To(smoke, 0.5f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
            smoke.Opacity = 0;
            //Tween.RunFunction(1.05f, this, "DestroyEffect", smoke);
            Tween.RunFunction(1.05f, smoke, "StopAnimation");
        }

        public void WoodChipEffect(Vector2 pos)
        {
            for (int i = 0; i < 5; i++)
            {
                SpriteObj woodChip = m_resourcePool.CheckOut();
                woodChip.ChangeSprite("WoodChip_Sprite");
                woodChip.Visible = true;
                woodChip.Position = pos;
                woodChip.PlayAnimation(true);
                woodChip.Scale = new Vector2(2, 2);

                int randRotation = CDGMath.RandomInt(-360, 360);
                woodChip.Rotation = randRotation;
                Vector2 newPos = new Vector2(CDGMath.RandomInt(-60, 60), CDGMath.RandomInt(-60, 60));

                Tween.By(woodChip, 0.3f, Tween.EaseNone, "X", newPos.X.ToString(), "Y", newPos.Y.ToString());
                Tween.By(woodChip, 0.3f, Tween.EaseNone, "Rotation", CDGMath.RandomInt(-360,360).ToString());
                //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", woodChip);
                Tween.AddEndHandlerToLastTween(woodChip, "StopAnimation");
                Tween.To(woodChip, 0.2f, Tween.EaseNone, "delay", "0.1", "Opacity", "0");
            }
        }

        public void SpellCastEffect(Vector2 pos, float angle, bool megaSpell)
        {
            SpriteObj portal = m_resourcePool.CheckOut();
            portal.ChangeSprite("SpellPortal_Sprite");
            if (megaSpell == true)
                portal.TextureColor = Color.Red;
            portal.Visible = true;
            portal.Position = pos;
            portal.PlayAnimation(false);
            portal.Scale = new Vector2(2, 2);
            portal.Rotation = angle;
            portal.OutlineWidth = 2;
        }

        public void LastBossSpellCastEffect(GameObj obj, float angle, bool megaSpell)
        {
            SpriteObj portal = m_resourcePool.CheckOut();
            portal.ChangeSprite("SpellPortal_Sprite");
            if (megaSpell == true)
                portal.TextureColor = Color.Red;
            portal.Visible = true;
            portal.Position = obj.Position;
            portal.PlayAnimation(false);
            portal.Scale = new Vector2(2, 2);
            if (obj.Flip == SpriteEffects.None)
                portal.Rotation = -angle;
            else
                portal.Rotation = angle;
            portal.OutlineWidth = 2;
        }

        public void LoadingGateSmokeEffect(int numEntities)
        {
            float xOffset = 1320f / numEntities;

            for (int i = 0; i < numEntities; i++)
            {
                int randX = CDGMath.RandomInt(-50, 50);
                int randY = CDGMath.RandomInt(-50, 0);

                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("ExplosionBrown_Sprite");
                smoke.Visible = true;
                smoke.ForceDraw = true;
                smoke.Position = new Vector2(xOffset * i, 720);
                smoke.PlayAnimation(true);
                smoke.Opacity = 0;
                smoke.Scale = new Vector2(1.5f, 1.5f);

                Tween.To(smoke, 0.2f, Linear.EaseNone, "Opacity", "0.8");
                Tween.By(smoke, 0.7f, Linear.EaseNone, "Rotation", CDGMath.RandomFloat(-180,180).ToString());
                Tween.By(smoke, 0.7f, Quad.EaseOut, "X", randX.ToString(), "Y", randY.ToString());
                Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");             
                smoke.Opacity = 0.8f;
                Tween.To(smoke, 0.5f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
                smoke.Opacity = 0;
            }
        }

        public void MegaTeleport(Vector2 pos, Vector2 scale)
        {
            SpriteObj teleport = m_resourcePool.CheckOut() as SpriteObj;
            teleport.ChangeSprite("MegaTeleport_Sprite");
            teleport.TextureColor = Color.LightSkyBlue;
            teleport.Scale = scale;
            teleport.Visible = true;
            teleport.Position = pos;
            teleport.PlayAnimation(false);
            teleport.AnimationDelay = 1 / 60f;
            Tween.By(teleport, 0.1f, Tween.EaseNone, "delay", "0.15", "Y", "-720");
        }

        public void MegaTeleportReverse(Vector2 pos, Vector2 scale)
        {
            SpriteObj teleport = m_resourcePool.CheckOut() as SpriteObj;
            teleport.ChangeSprite("MegaTeleportReverse_Sprite");
            teleport.TextureColor = Color.LightSkyBlue;
            teleport.Scale = scale;
            teleport.Visible = true;
            teleport.Position = pos;
            teleport.Y -= 720;
            teleport.PlayAnimation(1, 1, true);
            teleport.AnimationDelay = 1 / 60f;
            Tween.By(teleport, 0.1f, Tween.EaseNone, "Y", "720");
            Tween.AddEndHandlerToLastTween(teleport, "PlayAnimation", false);
        }

        public void DestroyFireballBoss(Vector2 pos)
        {
            float angle = 0;
            float angleDiff = 360 / 15f;

            for (int i = 0; i < 15; i++)
            {
                float delay = CDGMath.RandomFloat(0.5f, 1.1f);
                int distance = CDGMath.RandomInt(50, 200);
                float randScale = CDGMath.RandomFloat(2, 5);

                SpriteObj fireball = m_resourcePool.CheckOut() as SpriteObj;
                fireball.ChangeSprite("SpellDamageShield_Sprite");
                fireball.Visible = true;
                fireball.Scale = new Vector2(randScale, randScale);
                fireball.Position = pos;
                fireball.PlayAnimation(true);
                Vector2 tweenPos = CDGMath.AngleToVector(angle);
                Tween.By(fireball, 1.5f, Quad.EaseOut, "X", (tweenPos.X * distance).ToString(), "Y", (tweenPos.Y * distance).ToString());
                Tween.To(fireball, 0.5f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "0");
                Tween.AddEndHandlerToLastTween(fireball, "StopAnimation");

                angle += angleDiff;
            }
        }

        // 0 = bottom, 1 = left, 2 = right
        public void SkillTreeDustEffect(Vector2 pos, bool horizontal, float length)
        {
            int numClouds = (int)(length / 20f);
            float cloudDisplacement = length / numClouds;
            Vector2 cloudDisplacementVector = Vector2.Zero;
            if (horizontal == true)
                cloudDisplacementVector = new Vector2(1, 0);
            else
                cloudDisplacementVector = new Vector2(0, -1);

            for (int i = 0; i < numClouds; i++)
            {
                int randX = CDGMath.RandomInt(-10, 10);
                int randY = CDGMath.RandomInt(-20, 0);
                SpriteObj cloud = m_resourcePool.CheckOut();
                cloud.ChangeSprite("ExplosionBrown_Sprite");
                cloud.Opacity = 0;
                cloud.Visible = true;
                cloud.Scale = new Vector2(0.5f, 0.5f);
                cloud.Rotation = CDGMath.RandomInt(0, 270);
                cloud.Position = pos + (cloudDisplacementVector * cloudDisplacement * (i + 1));
                cloud.PlayAnimation(true);
                Tween.To(cloud, 0.5f, Linear.EaseNone, "Opacity", "1");
                Tween.By(cloud, 1.5f, Linear.EaseNone, "Rotation", CDGMath.RandomFloat(-30,30).ToString());
                Tween.By(cloud, 1.5f, Quad.EaseOut, "X", randX.ToString(), "Y", randY.ToString());
                float randScale = CDGMath.RandomFloat(0.5f, 0.8f);
                Tween.To(cloud, 1.5f, Quad.EaseOut, "ScaleX", randScale.ToString(), "ScaleY", randScale.ToString());
                Tween.AddEndHandlerToLastTween(cloud, "StopAnimation");             
                cloud.Opacity = 1;
                Tween.To(cloud, 0.7f, Linear.EaseNone, "delay", "0.6", "Opacity", "0");
                cloud.Opacity = 0;
            }
        }

        public void SkillTreeDustDuration(Vector2 pos, bool horizontal, float length, float duration)
        {
            float tick = 0.25f;
            int numTicks = (int)(duration / tick);
            for (int i = 0; i < numTicks; i++)
                Tween.RunFunction(tick * i, this, "SkillTreeDustEffect", pos, horizontal, length);
        }

        public void CarnivalGoldEffect(Vector2 startPos, Vector2 endPos, int numCoins)
        {
            float delay = 0.32f;
            for (int i = 0; i < numCoins; i++)
            {
                SpriteObj coin = m_resourcePool.CheckOut() as SpriteObj;
                coin.ChangeSprite("Coin_Sprite");
                coin.Visible = true;
                coin.Position = startPos;
                coin.PlayAnimation(true);
                int randX = CDGMath.RandomInt(-30, 30);
                int randY = CDGMath.RandomInt(-30, 30);
                Tween.By(coin, 0.3f, Quad.EaseInOut, "X", randX.ToString(), "Y", randY.ToString());
                coin.X += randX;
                coin.Y += randY;
                Tween.To(coin, 0.5f, Quad.EaseIn, "delay", delay.ToString(), "X", endPos.X.ToString(), "Y", endPos.Y.ToString());
                Tween.AddEndHandlerToLastTween(coin, "StopAnimation");
                coin.X -= randX;
                coin.Y -= randY;
                delay += 0.05f;
            }
        }

        public void AssassinCastEffect(Vector2 pos)
        {
            int numEffects = 10;
            float angle = 0;
            float angleMod = 360 / numEffects;

            for (int i = 0; i < numEffects; i++)
            {
                Vector2 direction = CDGMath.AngleToVector(angle);

                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("ExplosionBrown_Sprite");
                smoke.Visible = true;
                smoke.Position = pos;
                smoke.TextureColor = new Color(20, 20, 20);
                
                smoke.Opacity = 0;
                smoke.PlayAnimation(true);
                float randScale = CDGMath.RandomFloat(0.5f, 1.5f);
                smoke.Scale = new Vector2(randScale, randScale);
                smoke.Rotation = CDGMath.RandomInt(-30, 30);
                direction.X += CDGMath.RandomInt(-5, 5);
                direction.Y += CDGMath.RandomInt(-5, 5);

                Tween.To(smoke, 0.1f, Tween.EaseNone, "Opacity", "0.5");
                Tween.By(smoke, 1f, Quad.EaseOut, "X", (direction.X * CDGMath.RandomInt(20,25)).ToString(), "Y", (direction.Y * CDGMath.RandomInt(20,25)).ToString(), "Rotation", CDGMath.RandomInt(-180, 180).ToString());
                //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", smoke);
                Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");              
                smoke.Opacity = 0.5f;
                Tween.To(smoke, 0.5f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
                smoke.Opacity = 0;

                angle += angleMod;
            }
        }

        public void NinjaDisappearEffect(GameObj obj)
        {
            SpriteObj log = m_resourcePool.CheckOut();
            log.ChangeSprite("Log_Sprite");
            log.AnimationDelay = 1 / 20f;
            log.Position = obj.Position;
            log.Visible = true;
            log.Scale = obj.Scale / 2f;
            log.PlayAnimation(true);

            Tween.By(log, 0.3f, Quad.EaseIn, "delay", "0.2", "Y", "50");
            Tween.To(log, 0.2f, Linear.EaseNone, "delay", "0.3", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(log, "StopAnimation");

            SpriteObj smoke = m_resourcePool.CheckOut();
            smoke.ChangeSprite("NinjaSmoke_Sprite");
            smoke.AnimationDelay = 1 / 20f;
            smoke.Position = obj.Position;
            smoke.Visible = true;
            smoke.Scale = obj.Scale * 2;
            smoke.PlayAnimation(false);
        }

        public void NinjaAppearEffect(GameObj obj)
        {
            SpriteObj smoke = m_resourcePool.CheckOut();
            smoke.ChangeSprite("NinjaSmoke_Sprite");
            smoke.AnimationDelay = 1 / 20f;
            smoke.Position = obj.Position;
            smoke.Visible = true;
            smoke.Scale = obj.Scale * 2;
            smoke.PlayAnimation(false);
        }

        public void DisplayCriticalText(Vector2 position)
        {
            SpriteObj critical = m_resourcePool.CheckOut();
            critical.ChangeSprite("CriticalText_Sprite");
            Game.ChangeBitmapLanguage(critical, "CriticalText_Sprite");
            critical.Visible = true;
            critical.Rotation = CDGMath.RandomInt(-20, 20);
            critical.Position = CDGMath.GetCirclePosition(critical.Rotation - 90, 20, position);
            critical.Scale = Vector2.Zero;
            critical.PlayAnimation(true);
            Tween.To(critical, 0.2f, Back.EaseOutLarge, "ScaleX", "1", "ScaleY", "1");
            Tween.To(critical, 0.2f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(critical, "StopAnimation");
        }

        public void DisplayFusRoDahText(Vector2 position)
        {
            SpriteObj critical = m_resourcePool.CheckOut();
            critical.ChangeSprite("FusRoDahText_Sprite");
            critical.Visible = true;
            critical.Rotation = CDGMath.RandomInt(-20, 20);
            critical.Position = CDGMath.GetCirclePosition(critical.Rotation - 90, 40, position);
            critical.Scale = Vector2.Zero;
            critical.PlayAnimation(true);
            Tween.To(critical, 0.2f, Back.EaseOutLarge, "ScaleX", "1", "ScaleY", "1");
            Tween.To(critical, 0.2f, Tween.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(critical, "StopAnimation");
        }

        public void DisplayTanookiEffect(GameObj obj)
        {
            for (int i = 0; i < 10; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("ExplosionBrown_Sprite");
                smoke.Visible = true;
                smoke.Position = obj.Position;
                smoke.X += CDGMath.RandomInt(-30, 30);
                smoke.Y += CDGMath.RandomInt(-30, 30);
                float randomScale = CDGMath.RandomFloat(0.7f, 0.8f);
                int movementAmount = 50;
                smoke.Scale = new Vector2(randomScale, randomScale);
                smoke.Rotation = CDGMath.RandomInt(0, 90);
                smoke.PlayAnimation(true);

                float duration = CDGMath.RandomFloat(0.5f, 1.0f);
                float randScaleShrink = CDGMath.RandomFloat(0, 0.1f);
                Tween.To(smoke, duration - 0.2f, Linear.EaseNone, "delay", "0.2", "Opacity", "0");
                Tween.To(smoke, duration, Back.EaseIn, "ScaleX", randScaleShrink.ToString(), "ScaleY", randScaleShrink.ToString());
                Tween.By(smoke, duration, Quad.EaseOut, "X", CDGMath.RandomInt(-movementAmount, movementAmount).ToString(), "Y", CDGMath.RandomInt(-movementAmount, movementAmount).ToString());
                Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");
                Tween.By(smoke, duration - 0.1f, Tweener.Ease.Quad.EaseOut, "Rotation", CDGMath.RandomInt(145, 190).ToString());
            }
        }

        public void DisplayMusicNote(Vector2 pos)
        {
            SpriteObj note = m_resourcePool.CheckOut();
            note.ChangeSprite("NoteWhite_Sprite");
            note.Visible = true;
            note.Position = pos;
            note.Scale = new Vector2(2, 2);
            note.Opacity = 0;
            note.PlayAnimation(true);

            if (CDGMath.RandomPlusMinus() < 0)
                note.Flip = SpriteEffects.FlipHorizontally;

            Tween.By(note, 1f, Quad.EaseOut, "Y", "-50");
            Tween.To(note, 0.5f, Tween.EaseNone, "Opacity", "1");
            note.Opacity = 1;
            Tween.To(note, 0.2f, Tween.EaseNone, "delay", "0.8", "Opacity", "0");
            note.Opacity = 0;
            Tween.RunFunction(1, note, "StopAnimation");
        }

        public void DisplayQuestionMark(Vector2 pos)
        {
            SpriteObj note = m_resourcePool.CheckOut();
            note.ChangeSprite("QuestionMark_Sprite");
            note.Visible = true;
            note.Position = pos;
            float randScale = CDGMath.RandomFloat(0.8f, 2f);
            note.Scale = new Vector2(randScale, randScale);
            note.Opacity = 0;
            note.PlayAnimation(true);

            Tween.By(note, 1f, Quad.EaseOut, "Y", CDGMath.RandomInt(-70,-50).ToString());
            Tween.By(note, 1f, Quad.EaseOut, "X", CDGMath.RandomInt(-20,20).ToString());
            Tween.To(note, 0.5f, Tween.EaseNone, "Opacity", "1");
            note.Opacity = 1;
            Tween.To(note, 0.2f, Tween.EaseNone, "delay", "0.8", "Opacity", "0");
            note.Opacity = 0;
            Tween.RunFunction(1, note, "StopAnimation");
        }

        public void DisplayMassiveSmoke(Vector2 topLeft)
        {
            Vector2 pos = topLeft;
            for (int i = 0; i < 20; i++)
            {
                this.IntroSmokeEffect(pos);
                pos.Y += 40;
            }
        }

        public void IntroSmokeEffect(Vector2 pos)
        {
            SpriteObj smoke = m_resourcePool.CheckOut();
            smoke.ChangeSprite("BlackSmoke_Sprite");
            smoke.Visible = true;
            smoke.Y = pos.Y;
            smoke.Y += CDGMath.RandomInt(-40, 40);
            smoke.X = pos.X;
            smoke.X += CDGMath.RandomInt(-40, 40);
            smoke.Scale = new Vector2(2.5f, 2.5f);

            smoke.Opacity = 0;
            smoke.PlayAnimation(true);
            smoke.Rotation = CDGMath.RandomInt(-30, 30);

            float delay = CDGMath.RandomFloat(0, 0.2f);

            Tween.To(smoke, 0.2f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
            Tween.By(smoke, 1f, Quad.EaseInOut, "delay", delay.ToString(), "X", CDGMath.RandomInt(50, 100).ToString(), "Y", CDGMath.RandomInt(-100, 100).ToString(), "Rotation", CDGMath.RandomInt(-45, 45).ToString());
            smoke.Opacity = 1;
            Tween.To(smoke, 0.5f, Tween.EaseNone, "delay", (delay + 0.5f).ToString(), "Opacity", "0");
            smoke.Opacity = 0;
            //Tween.AddEndHandlerToLastTween(this, "DestroyEffect", smoke);
            Tween.AddEndHandlerToLastTween(smoke, "StopAnimation");
        }

        public void DisplayIceParticleEffect(GameObj sprite)
        {
            SpriteObj particle = m_resourcePool.CheckOut();
            particle.ChangeSprite("WizardIceParticle_Sprite");
            particle.Visible = true;
            particle.Scale = Vector2.Zero;
            particle.Position = new Vector2(CDGMath.RandomInt((int)sprite.Bounds.Left, sprite.Bounds.Right), CDGMath.RandomInt((int)sprite.Bounds.Top, sprite.Bounds.Bottom));
            particle.Opacity = 0;
            Tween.To(particle, 0.1f, Tween.EaseNone, "Opacity", "1");
            Tween.To(particle, 0.9f, Tween.EaseNone, "ScaleX", "2.5", "ScaleY", "2.5");
            Tween.By(particle, 0.9f, Tween.EaseNone, "Rotation", (CDGMath.RandomInt(90, 270) * CDGMath.RandomPlusMinus()).ToString());
            particle.Opacity = 1;
            float delay = CDGMath.RandomFloat(0.4f, 0.7f);
            Tween.To(particle, 0.2f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "0");
            Tween.By(particle, 0.2f + delay, Tween.EaseNone, "X", CDGMath.RandomInt(-20, 20).ToString(), "Y", CDGMath.RandomInt(-20, 20).ToString());
            particle.Opacity = 0;
            particle.PlayAnimation(true);
            Tween.RunFunction(1f, particle, "StopAnimation");
        }

        public void DisplayFireParticleEffect(GameObj sprite)
        {
            SpriteObj particle = m_resourcePool.CheckOut();
            particle.ChangeSprite("WizardFireParticle_Sprite");
            particle.Visible = true;
            particle.Scale = Vector2.Zero;
            particle.Position = new Vector2(CDGMath.RandomInt((int)sprite.Bounds.Left, sprite.Bounds.Right), CDGMath.RandomInt((int)sprite.Bounds.Top, sprite.Bounds.Bottom));
            particle.Opacity = 0;
            Tween.To(particle, 0.1f, Tween.EaseNone, "Opacity", "1");
            Tween.To(particle, 0.9f, Tween.EaseNone, "ScaleX", "4", "ScaleY", "4");
            particle.Opacity = 1;
            float delay = CDGMath.RandomFloat(0.4f, 0.7f);
            Tween.To(particle, 0.2f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "0");
            Tween.By(particle, 0.2f + delay, Tween.EaseNone, "Y", CDGMath.RandomInt(-20, -5).ToString());
            particle.Opacity = 0;
            particle.PlayAnimation(true);
            Tween.RunFunction(1f, particle, "StopAnimation");
        }

        public void DisplayEarthParticleEffect(GameObj sprite)
        {
            int randBlossom = CDGMath.RandomInt(1, 4);
            SpriteObj particle = m_resourcePool.CheckOut();
            particle.ChangeSprite("Blossom" + randBlossom + "_Sprite");
            particle.Visible = true;
            particle.Scale = new Vector2(0.2f, 0.2f);
            particle.Position = new Vector2(CDGMath.RandomInt((int)sprite.Bounds.Left, sprite.Bounds.Right), CDGMath.RandomInt((int)sprite.Bounds.Top, sprite.Bounds.Bottom));
            particle.Opacity = 0;
            Tween.To(particle, 0.1f, Tween.EaseNone, "Opacity", "1");
            Tween.To(particle, 0.9f, Tween.EaseNone, "ScaleX", "0.7", "ScaleY", "0.7");
            Tween.By(particle, 0.9f, Tween.EaseNone, "Rotation", (CDGMath.RandomInt(10, 45) * CDGMath.RandomPlusMinus()).ToString());
            particle.Opacity = 1;
            float delay = CDGMath.RandomFloat(0.4f, 0.7f);
            Tween.To(particle, 0.2f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "0");
            Tween.By(particle, 0.2f + delay, Tween.EaseNone, "Y", CDGMath.RandomInt(5, 20).ToString());
            particle.Opacity = 0;
            particle.PlayAnimation(true);
            Tween.RunFunction(1f, particle, "StopAnimation");
        }

        public void DisplayFountainShatterSmoke(GameObj sprite)
        {
            int numParticles = 15;
            float width = (float)sprite.Width / (float)numParticles;
            float startingX = sprite.Bounds.Left;
            for (int i = 0; i < numParticles; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("FountainShatterSmoke_Sprite");
                smoke.Visible = true;
                smoke.PlayAnimation(true);
                smoke.Opacity = 0;
                smoke.Scale = Vector2.Zero;
                smoke.Position = new Vector2(startingX, sprite.Y);

                float randScale = CDGMath.RandomFloat(2,4);
                Tween.To(smoke, 0.5f, Tween.EaseNone, "Opacity", "1");
                Tween.By(smoke, 4, Tween.EaseNone, "Rotation", CDGMath.RandomInt(-40, 40).ToString());
                Tween.By(smoke, 3, Tween.EaseNone, "X", CDGMath.RandomInt(-20, 20).ToString(), "Y", CDGMath.RandomInt(-50, -30).ToString());
                Tween.To(smoke, 3, Tween.EaseNone, "ScaleX", randScale.ToString(), "ScaleY", randScale.ToString());
                smoke.Opacity = 1;
                Tween.To(smoke, 2, Tween.EaseNone, "delay", CDGMath.RandomFloat(1f, 2f).ToString(), "Opacity", "0");
                smoke.Opacity = 0;
                Tween.RunFunction(4.5f, smoke, "StopAnimation");
                startingX += width;
            }

            numParticles /= 2;
            startingX = sprite.Bounds.Left + 50;
            width = (float)(sprite.Width - 50) / (float)numParticles;
            for (int i = 0; i < numParticles; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("FountainShatterSmoke_Sprite");
                smoke.Visible = true;
                smoke.PlayAnimation(true);
                smoke.Scale = Vector2.Zero;
                smoke.Opacity = 0;
                smoke.Position = new Vector2(startingX, sprite.Y - 100);

                float randScale = CDGMath.RandomFloat(2, 4);
                Tween.To(smoke, 0.5f, Tween.EaseNone, "Opacity", "1");
                Tween.By(smoke, 4, Tween.EaseNone, "Rotation", CDGMath.RandomInt(-180, 180).ToString());
                Tween.By(smoke, 3, Tween.EaseNone, "X", CDGMath.RandomInt(-20, 20).ToString(), "Y", CDGMath.RandomInt(-50, -30).ToString());
                Tween.To(smoke, 3, Tween.EaseNone, "ScaleX", randScale.ToString(), "ScaleY", randScale.ToString());
                smoke.Opacity = 1;
                Tween.To(smoke, 2, Tween.EaseNone, "delay", CDGMath.RandomFloat(1f, 2f).ToString(), "Opacity", "0");
                smoke.Opacity = 0;
                Tween.RunFunction(4.5f, smoke, "StopAnimation");
                startingX += width;
            }

            numParticles /= 2;
            startingX = sprite.Bounds.Left + 100;
            width = (float)(sprite.Width - 100) / (float)numParticles;
            for (int i = 0; i < numParticles; i++)
            {
                SpriteObj smoke = m_resourcePool.CheckOut();
                smoke.ChangeSprite("FountainShatterSmoke_Sprite");
                smoke.Visible = true;
                smoke.PlayAnimation(true);
                smoke.Scale = Vector2.Zero;
                smoke.Opacity = 0;
                smoke.Position = new Vector2(startingX, sprite.Y - 200);

                float randScale = CDGMath.RandomFloat(2, 4);
                Tween.To(smoke, 0.5f, Tween.EaseNone, "Opacity", "1");
                Tween.By(smoke, 4, Tween.EaseNone, "Rotation", CDGMath.RandomInt(-180, 180).ToString());
                Tween.By(smoke, 3, Tween.EaseNone, "X", CDGMath.RandomInt(-20, 20).ToString(), "Y", CDGMath.RandomInt(-50, -30).ToString());
                Tween.To(smoke, 3, Tween.EaseNone, "ScaleX", randScale.ToString(), "ScaleY", randScale.ToString());
                smoke.Opacity = 1;
                Tween.To(smoke, 2, Tween.EaseNone, "delay", CDGMath.RandomFloat(1f, 2f).ToString(), "Opacity", "0");
                smoke.Opacity = 0;
                Tween.RunFunction(4.5f, smoke, "StopAnimation");
                startingX += width;
            }
        }

        public void DoorSparkleEffect(Rectangle rect)
        {
            SpriteObj particle = m_resourcePool.CheckOut();
            particle.ChangeSprite("LevelUpParticleFX_Sprite");
            particle.Visible = true;
            float randomScale = CDGMath.RandomFloat(0.3f, 0.5f);
            particle.Scale = new Vector2(randomScale, randomScale);
            particle.Opacity = 0;
            particle.Position = new Vector2(CDGMath.RandomInt(rect.X, rect.X + rect.Width), CDGMath.RandomInt(rect.Y, rect.Y + rect.Height));
            particle.Rotation = CDGMath.RandomInt(0, 90);
            particle.PlayAnimation(false);
            Tween.To(particle, 0.4f, Linear.EaseNone, "Opacity", "1");//, "ScaleX", "0", "ScaleY", "0");
            Tween.By(particle, 0.6f, Linear.EaseNone, "Rotation", CDGMath.RandomInt(-45, 45).ToString(), "Y", "-50");//, "ScaleX", "0", "ScaleY", "0");
            Tween.To(particle, 0.7f, Linear.EaseNone, "ScaleX", "0", "ScaleY", "0");
            Tween.AddEndHandlerToLastTween(particle, "StopAnimation");
        }

        public void DestroyEffect(SpriteObj obj)
        {
            obj.OutlineWidth = 0;
            obj.Visible = false;
            obj.Rotation = 0;
            obj.TextureColor = Color.White;
            obj.Opacity = 1;
            m_resourcePool.CheckIn(obj);
            obj.Flip = SpriteEffects.None;
            obj.Scale = new Vector2(1, 1);
            obj.AnimationDelay = 1 / 30f;
        }

        public void PauseAllAnimations()
        {
            m_isPaused = true;
            foreach (SpriteObj sprite in m_resourcePool.ActiveObjsList)
                sprite.PauseAnimation();
        }

        public void ResumeAllAnimations()
        {
            m_isPaused = false;
            foreach (SpriteObj sprite in m_resourcePool.ActiveObjsList)
                sprite.ResumeAnimation();
        }

        public void DestroyAllEffects()
        {
            foreach (SpriteObj sprite in m_resourcePool.ActiveObjsList)
                sprite.StopAnimation();
        }

        public void Draw(Camera2D camera)
        {
            for (int i = 0; i < m_resourcePool.ActiveObjsList.Count; i++)
            {
                if (m_resourcePool.ActiveObjsList[i].IsAnimating == false && m_isPaused == false)
                {
                    DestroyEffect(m_resourcePool.ActiveObjsList[i]);
                    i--;
                }
                else
                    m_resourcePool.ActiveObjsList[i].Draw(camera);
            }
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Impact Effect Pool");

                m_isDisposed = true;
                m_resourcePool.Dispose();
                m_resourcePool = null;
            }
        }

        public int ActiveTextObjs
        {
            get { return m_resourcePool.NumActiveObjs; }
        }

        public int TotalPoolSize
        {
            get { return m_resourcePool.TotalPoolSize; }
        }

        public int CurrentPoolSize
        {
            get { return TotalPoolSize - ActiveTextObjs; }
        }
    }
}

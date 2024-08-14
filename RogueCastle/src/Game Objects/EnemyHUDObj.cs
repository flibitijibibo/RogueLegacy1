using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class EnemyHUDObj : SpriteObj
    {
        private TextObj m_enemyNameText;
        private TextObj m_enemyLevelText;
        private SpriteObj m_enemyHPBar;
        private float m_enemyHPPercent;
        private int m_enemyHPBarLength;

        private int m_blinkNumber = 13; // Must be end for it to disappear.
        private float m_blinkDuration = 0.05f;

        private int m_blinkCounter = 0;
        private float m_blinkDurationCounter = 0;
        private float m_opacity = 1;

        public EnemyHUDObj()
            : base("EnemyHUD_Sprite")
        {
            this.ForceDraw = true;

            m_enemyNameText = new TextObj();
            m_enemyNameText.Font = Game.JunicodeFont;
            m_enemyNameText.FontSize = 10;
            m_enemyNameText.Align = Types.TextAlign.Right;

            m_enemyLevelText = new TextObj();
            m_enemyLevelText.Font = Game.EnemyLevelFont;

            m_enemyHPBar = new SpriteObj("EnemyHPBar_Sprite");
            m_enemyHPBar.ForceDraw = true;
            m_enemyHPBarLength = m_enemyHPBar.SpriteRect.Width;
        }

        public void UpdateEnemyInfo(string enemyNameID, int enemyLevel, float enemyHPPercent)
        {
            m_blinkDurationCounter = 0;
            m_blinkCounter = 0;
            m_enemyHPBar.Opacity = 1;
            m_enemyLevelText.Opacity = 1;
            m_enemyNameText.Opacity = 1;
            this.Opacity = 1;

            if (enemyNameID == null)
                m_enemyNameText.Text = "Default Enemy";
            else
            {
                m_enemyNameText.Text = LocaleBuilder.getString(enemyNameID, m_enemyNameText);
                // TODO, adjust limit for different languages and fonts?
                if (m_enemyNameText.Text.Length > 17)
                    m_enemyNameText.Text = m_enemyNameText.Text.Substring(0, 14) + "...";
            }

            m_enemyLevelText.Text = ((int)(enemyLevel * LevelEV.ENEMY_LEVEL_FAKE_MULTIPLIER)).ToString();
            m_enemyHPPercent = enemyHPPercent;

            if (enemyHPPercent <= 0)
            {
                m_blinkCounter = m_blinkNumber;
                m_blinkDurationCounter = m_blinkDuration;

                m_opacity = 0.5f;
                m_enemyHPBar.Opacity = 0.5f;
                m_enemyLevelText.Opacity = 0.5f;
                m_enemyNameText.Opacity = 0.5f;
                this.Opacity = 0.5f;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (m_blinkDurationCounter > 0)
                m_blinkDurationCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_blinkCounter > 0 && m_blinkDurationCounter <= 0)
            {
                if (m_opacity > 0)
                    m_opacity = 0;
                else
                    m_opacity = 0.5f;

                m_enemyHPBar.Opacity = m_opacity;
                m_enemyLevelText.Opacity = m_opacity;
                m_enemyNameText.Opacity = m_opacity;
                this.Opacity = m_opacity;
                m_blinkCounter--;
                m_blinkDurationCounter = m_blinkDuration;
            }
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);

            m_enemyHPBar.Position = new Vector2(this.X + 8, this.Y + 17);
            m_enemyHPBar.SpriteRect = new Rectangle(m_enemyHPBar.SpriteRect.X, m_enemyHPBar.SpriteRect.Y, (int)(m_enemyHPBarLength * m_enemyHPPercent), m_enemyHPBar.SpriteRect.Height);
            m_enemyHPBar.Draw(camera);

            m_enemyNameText.Position = new Vector2(this.X + this.Width - 5, this.Y - 10);
            m_enemyNameText.Draw(camera);

            m_enemyLevelText.Position = new Vector2(this.X + 22, this.Y - 8);
            m_enemyLevelText.Draw(camera);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_enemyNameText.Dispose();
                m_enemyNameText = null;
                m_enemyLevelText.Dispose();
                m_enemyLevelText = null;
                m_enemyHPBar.Dispose();
                m_enemyHPBar = null;
                base.Dispose();
            }
        }

        public void RefreshTextObjs()
        {
            // TODO, adjust limit for different languages and fonts?
            if (m_enemyNameText.Text.Length > 17)
                m_enemyNameText.Text = m_enemyNameText.Text.Substring(0, 14) + "..";
        }
    }
}

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
    public class TextManager : IDisposableObj
    {
        private int m_poolSize = 0;
        private DS2DPool<TextObj> m_resourcePool;

        public bool IsDisposed { get { return m_isDisposed; } }
        private bool m_isDisposed = false;

        public TextManager(int poolSize)
        {
            m_poolSize = poolSize;
            m_resourcePool = new DS2DPool<TextObj>();
        }

        public void Initialize()
        {
            for (int i = 0; i < m_poolSize; i++)
            {
                TextObj poolObj = new TextObj(null);
                poolObj.Visible = false;
                poolObj.TextureColor = Color.White;
                poolObj.OutlineWidth = 2;
                m_resourcePool.AddToPool(poolObj);
            }
        }

        public void DisplayNumberStringText(int amount, string textLocID, Color color, Vector2 position)
        {
            // If pool is depleted don't display number string text. In theory,
            // this should never happen. The only place this happens is when
            // player plays ChestBonusRoomObj game, bets a lot of money, wins,
            // don't collect the gold, then collects all the gold at the same
            // time triggering a bunch of "XYZ coins" text.   --Dave
            if (m_resourcePool.CurrentPoolSize < 2) return; // needs 2 objects for display

            TextObj fullTextCheck = m_resourcePool.CheckOut();
            fullTextCheck.Font = Game.JunicodeFont;
            fullTextCheck.FontSize = 14;
            fullTextCheck.Text = amount + " " + LocaleBuilder.getString(textLocID, fullTextCheck);

            int textWidth = fullTextCheck.Width;
            m_resourcePool.CheckIn(fullTextCheck);

            // Changing text settings
            TextObj amountText = m_resourcePool.CheckOut();
            amountText.Font = Game.HerzogFont;
            amountText.Text = amount.ToString();
            amountText.Align = Types.TextAlign.Left;
            amountText.FontSize = 18;
            amountText.TextureColor = color;
            amountText.Position = new Vector2(position.X - textWidth / 2f, position.Y - amountText.Height / 2f);
            amountText.Visible = true;
            //amountText.Y -= amountText.Height / 2f;

            TextObj regularText = m_resourcePool.CheckOut();
            regularText.Font = Game.JunicodeFont;
            regularText.Text = " " + LocaleBuilder.getString(textLocID, regularText);
            regularText.FontSize = 14;
            regularText.Align = Types.TextAlign.Left;
            regularText.TextureColor = color;
            regularText.Position = amountText.Position;
            regularText.X += amountText.Width;
            regularText.Y -= 5;
            regularText.Visible = true;

            // Applying effect to text
            Tween.By(amountText, 0.3f, Quad.EaseOut, "Y", "-60");
            Tween.To(amountText, 0.2f, Linear.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "DestroyText", amountText);

            Tween.By(regularText, 0.3f, Quad.EaseOut, "Y", "-60");
            Tween.To(regularText, 0.2f, Linear.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "DestroyText", regularText);
        }

        public void DisplayStringNumberText(string textLocID, int amount, Color color, Vector2 position)
        {
            TextObj fullTextCheck = m_resourcePool.CheckOut();
            fullTextCheck.Font = Game.JunicodeFont;
            fullTextCheck.FontSize = 14;
            fullTextCheck.Text = LocaleBuilder.getString(textLocID, fullTextCheck) + " " + amount;

            int textWidth = fullTextCheck.Width;
            m_resourcePool.CheckIn(fullTextCheck);

            // Changing text settings
            TextObj regularText = m_resourcePool.CheckOut();
            regularText.Font = Game.JunicodeFont;
            regularText.Text = LocaleBuilder.getString(textLocID, regularText) + " ";
            regularText.FontSize = 14;
            regularText.TextureColor = color;
            regularText.Position = new Vector2(position.X - textWidth / 2f, position.Y - regularText.Height / 2f);
            regularText.Visible = true;

            TextObj amountText = m_resourcePool.CheckOut();
            amountText.Font = Game.HerzogFont;
            amountText.Text = amount.ToString();
            amountText.FontSize = 18;
            amountText.TextureColor = color;
            amountText.Position = new Vector2(regularText.X + regularText.Width, regularText.Y + 5);
            amountText.Visible = true;

            // Applying effect to text
            Tween.By(amountText, 0.3f, Quad.EaseOut, "Y", "-60");
            Tween.To(amountText, 0.2f, Linear.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "DestroyText", amountText);

            Tween.By(regularText, 0.3f, Quad.EaseOut, "Y", "-60");
            Tween.To(regularText, 0.2f, Linear.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "DestroyText", regularText);
        }

        public void DisplayNumberText(int amount, Color color, Vector2 position)
        {
            // Changing text settings
            TextObj amountText = m_resourcePool.CheckOut();
            amountText.Font = Game.HerzogFont;
            amountText.Text = amount.ToString();
            amountText.FontSize = 18;
            amountText.TextureColor = color;
            amountText.Align = Types.TextAlign.Centre;
            amountText.Visible = true;
            amountText.Position = position;
            amountText.Y -= amountText.Height / 2f;

            // Applying effect to text
            Tween.By(amountText, 0.3f, Quad.EaseOut, "Y", "-60");
            Tween.To(amountText, 0.2f, Linear.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "DestroyText", amountText);
        }

        public void DisplayStringText(string textLocID, Color color, Vector2 position)
        {
            TextObj regularText = m_resourcePool.CheckOut();
            regularText.Font = Game.JunicodeFont;
            regularText.Text = LocaleBuilder.getString(textLocID, regularText);
            regularText.Align = Types.TextAlign.Centre;
            regularText.FontSize = 14;
            regularText.TextureColor = color;
            regularText.Position = position;
            regularText.Visible = true;

            Tween.By(regularText, 0.3f, Quad.EaseOut, "Y", "-60");
            Tween.To(regularText, 0.2f, Linear.EaseNone, "delay", "0.5", "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "DestroyText", regularText);
        }

        public void DestroyText(TextObj obj)
        {
            obj.Opacity = 1;
            obj.Align = Types.TextAlign.Left;
            obj.Visible = false;
            m_resourcePool.CheckIn(obj);
        }

        public void Draw(Camera2D camera)
        {
            foreach (TextObj obj in m_resourcePool.ActiveObjsList)
                obj.Draw(camera);
        }

        public void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Text Manager");

                m_resourcePool.Dispose();
                m_resourcePool = null;
                m_isDisposed = true;
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

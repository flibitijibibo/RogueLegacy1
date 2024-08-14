using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class PlayerHUDObj : SpriteObj
    {
        private int m_maxBarLength = 360;

        private TextObj m_playerLevelText;
        private SpriteObj m_coin;
        private TextObj m_goldText;

        private SpriteObj m_hpBar;
        private TextObj m_hpText;

        private SpriteObj m_mpBar;
        private TextObj m_mpText;

        private SpriteObj[] m_abilitiesSpriteArray;

        private ObjContainer m_hpBarContainer;
        private ObjContainer m_mpBarContainer;

        private SpriteObj m_specialItemIcon;
        private SpriteObj m_spellIcon;
        private TextObj m_spellCost;

        private SpriteObj m_iconHolder1, m_iconHolder2;

        public bool ShowBarsOnly { get; set; }

        public int forcedPlayerLevel { get; set; }

        public PlayerHUDObj() :
            base("PlayerHUDLvlText_Sprite")
        {
            this.ForceDraw = true;
            forcedPlayerLevel = -1;

            m_playerLevelText = new TextObj();
            m_playerLevelText.Text = Game.PlayerStats.CurrentLevel.ToString();
            m_playerLevelText.Font = Game.PlayerLevelFont;

            m_coin = new SpriteObj("PlayerUICoin_Sprite");
            m_coin.ForceDraw = true;

            m_goldText = new TextObj();
            m_goldText.Text = "0";
            m_goldText.Font = Game.GoldFont;
            m_goldText.FontSize = 25;

            m_hpBar = new SpriteObj("HPBar_Sprite");
            m_hpBar.ForceDraw = true;

            m_mpBar = new SpriteObj("MPBar_Sprite");
            m_mpBar.ForceDraw = true;

            m_hpText = new TextObj(Game.JunicodeFont);
            m_hpText.FontSize = 8;
            m_hpText.DropShadow = new Vector2(1, 1);
            m_hpText.ForceDraw = true;

            m_mpText = new TextObj(Game.JunicodeFont);
            m_mpText.FontSize = 8;
            m_mpText.DropShadow = new Vector2(1, 1);
            m_mpText.ForceDraw = true;

            m_abilitiesSpriteArray = new SpriteObj[5]; // Can only have 5 abilities equipped at a time.
            Vector2 startPos = new Vector2(130, 690);
            int xOffset = 35;
            for (int i = 0; i < m_abilitiesSpriteArray.Length; i++)
            {
                m_abilitiesSpriteArray[i] = new SpriteObj("Blank_Sprite");
                m_abilitiesSpriteArray[i].ForceDraw = true;
                m_abilitiesSpriteArray[i].Position = startPos;
                m_abilitiesSpriteArray[i].Scale = new Vector2(0.5f, 0.5f);
                startPos.X += xOffset;
            }

            m_hpBarContainer = new ObjContainer("PlayerHUDHPBar_Character");
            m_hpBarContainer.ForceDraw = true;

            m_mpBarContainer = new ObjContainer("PlayerHUDMPBar_Character");
            m_mpBarContainer.ForceDraw = true;

            m_specialItemIcon = new SpriteObj("Blank_Sprite");
            m_specialItemIcon.ForceDraw = true;
            m_specialItemIcon.OutlineWidth = 1;
            m_specialItemIcon.Scale = new Vector2(1.7f, 1.7f);
            m_specialItemIcon.Visible = false;

            m_spellIcon = new SpriteObj(SpellType.Icon(SpellType.None));
            m_spellIcon.ForceDraw = true;
            m_spellIcon.OutlineWidth = 1;
            m_spellIcon.Visible = false;

            m_iconHolder1 = new SpriteObj("BlacksmithUI_IconBG_Sprite");
            m_iconHolder1.ForceDraw = true;
            m_iconHolder1.Opacity = 0.5f;
            m_iconHolder1.Scale = new Vector2(0.8f, 0.8f);
            m_iconHolder2 = m_iconHolder1.Clone() as SpriteObj;

            m_spellCost = new TextObj(Game.JunicodeFont);
            m_spellCost.Align = Types.TextAlign.Centre;
            m_spellCost.ForceDraw = true;
            m_spellCost.OutlineWidth = 2;
            m_spellCost.FontSize = 8;
            m_spellCost.Visible = false;

            UpdateSpecialItemIcon();
            UpdateSpellIcon();
        }

        public void SetPosition(Vector2 position)
        {
            SpriteObj mpBar, hpBar;
            ObjContainer mpContainer, hpContainer;

            if (Game.PlayerStats.Traits.X == TraitType.Dextrocardia || Game.PlayerStats.Traits.Y == TraitType.Dextrocardia)
            {
                mpBar = m_hpBar;
                hpBar = m_mpBar;
                mpContainer = m_hpBarContainer;
                hpContainer = m_mpBarContainer;
            }
            else
            {
                mpBar = m_mpBar;
                hpBar = m_hpBar;
                mpContainer = m_mpBarContainer;
                hpContainer = m_hpBarContainer;
            }

            this.Position = position;
            mpBar.Position = new Vector2(this.X + 7, this.Y + 60);
            hpBar.Position = new Vector2(this.X + 8, this.Y + 29);
            m_playerLevelText.Position = new Vector2(this.X + 30, this.Y - 20);

            if (Game.PlayerStats.Traits.X == TraitType.Dextrocardia || Game.PlayerStats.Traits.Y == TraitType.Dextrocardia)
            {
                m_mpText.Position = new Vector2(this.X + 5, this.Y + 16);
                m_mpText.X += 8;

                m_hpText.Position = m_mpText.Position;
                m_hpText.Y += 28;
            }
            else
            {
                m_hpText.Position = new Vector2(this.X + 5, this.Y + 16);
                m_hpText.X += 8;
                m_hpText.Y += 5;

                m_mpText.Position = m_hpText.Position;
                m_mpText.Y += 30;
            }

            hpContainer.Position = new Vector2(this.X, this.Y + 17);
            if (hpBar == m_hpBar)
                hpBar.Position = new Vector2(hpContainer.X + 2, hpContainer.Y + 7); // Small hack to properly align dextrocardia
            else
                hpBar.Position = new Vector2(hpContainer.X + 2, hpContainer.Y + 6);

            mpContainer.Position = new Vector2(this.X, hpContainer.Bounds.Bottom);
            if (mpBar == m_mpBar)
                mpBar.Position = new Vector2(mpContainer.X + 2, mpContainer.Y + 6);
            else
                mpBar.Position = new Vector2(mpContainer.X + 2, mpContainer.Y + 7); // Small hack to properly align dextrocardia

            m_coin.Position = new Vector2(this.X, mpContainer.Bounds.Bottom + 2);
            m_goldText.Position = new Vector2(m_coin.X + 28, m_coin.Y - 2);


            m_iconHolder1.Position = new Vector2(m_coin.X + 25, m_coin.Y + 60);
            m_iconHolder2.Position = new Vector2(m_iconHolder1.X + 55, m_iconHolder1.Y);

            m_spellIcon.Position = m_iconHolder1.Position;
            m_specialItemIcon.Position = m_iconHolder2.Position;
            m_spellCost.Position = new Vector2(m_spellIcon.X, m_spellIcon.Bounds.Bottom + 10);
        }

        public void Update(PlayerObj player)
        {
            int playerLevel = Game.PlayerStats.CurrentLevel;
            if (playerLevel < 0)
                playerLevel = 0;
            if (forcedPlayerLevel >= 0)
                playerLevel = forcedPlayerLevel;
            m_playerLevelText.Text = playerLevel.ToString();
            //m_playerLevelText.Text = Game.PlayerStats.CurrentLevel.ToString();

            int playerGold = Game.PlayerStats.Gold;
            if (playerGold < 0)
                playerGold = 0;
            m_goldText.Text = playerGold.ToString();
            //m_goldText.Text = Game.PlayerStats.Gold.ToString();

            m_hpText.Text = (player.CurrentHealth + "/" + player.MaxHealth);
            m_mpText.Text = (player.CurrentMana + "/" + player.MaxMana);

            UpdatePlayerHP(player);
            UpdatePlayerMP(player);
        }

        private void UpdatePlayerHP(PlayerObj player)
        {
            // Each piece is 32 pixels in width.
            // Total bar length is 88;
            int hpBarIncreaseAmount = player.MaxHealth - player.BaseHealth; // The amount of bonus HP the player has compared to his base health.
            float hpPercent = player.CurrentHealth / (float)player.MaxHealth; // The current percent of health player has compared to his max health.

            int hpBarIncreaseWidth = (int)(88 + (hpBarIncreaseAmount / 5f));
            if (hpBarIncreaseWidth > m_maxBarLength)
                hpBarIncreaseWidth = m_maxBarLength;
            float midBarScaleX = (hpBarIncreaseWidth - 28 - 28) / 32f;
            m_hpBarContainer.GetChildAt(1).ScaleX = midBarScaleX;
            m_hpBarContainer.GetChildAt(2).X = m_hpBarContainer.GetChildAt(1).Bounds.Right;
            m_hpBarContainer.CalculateBounds();

            m_hpBar.ScaleX = 1;
            m_hpBar.ScaleX = ((m_hpBarContainer.Width - 8) / (float)m_hpBar.Width) * hpPercent;
        }

        private void UpdatePlayerMP(PlayerObj player)
        {
            int mpBarIncreaseAmount = (int)(player.MaxMana - player.BaseMana);
            float mpPercent = player.CurrentMana / player.MaxMana;

            int mpBarIncreaseWidth = (int)(88 + (mpBarIncreaseAmount / 5f));
            if (mpBarIncreaseWidth > m_maxBarLength)
                mpBarIncreaseWidth = m_maxBarLength;
            float midBarScaleX = (mpBarIncreaseWidth - 28 - 28) / 32f;
            m_mpBarContainer.GetChildAt(1).ScaleX = midBarScaleX;
            m_mpBarContainer.GetChildAt(2).X = m_mpBarContainer.GetChildAt(1).Bounds.Right;
            m_mpBarContainer.CalculateBounds();

            m_mpBar.ScaleX = 1;
            m_mpBar.ScaleX = ((m_mpBarContainer.Width - 8) / (float)m_mpBar.Width) * mpPercent;
        }

        public void UpdatePlayerLevel()
        {
            m_playerLevelText.Text = Game.PlayerStats.CurrentLevel.ToString();
        }

        public void UpdateAbilityIcons()
        {
            foreach (SpriteObj sprite in m_abilitiesSpriteArray)
                sprite.ChangeSprite("Blank_Sprite"); // Zeroing out each sprite.

            int spriteArrayIndex = 0;
            foreach (sbyte index in Game.PlayerStats.GetEquippedRuneArray)
            {
                if (index != -1)
                {
                    m_abilitiesSpriteArray[spriteArrayIndex].ChangeSprite(EquipmentAbilityType.Icon(index));
                    spriteArrayIndex++;
                }
            }
        }

        public void UpdateSpecialItemIcon()
        {
            m_specialItemIcon.Visible = false;
            m_iconHolder2.Opacity = 0.5f;
            if (Game.PlayerStats.SpecialItem != SpecialItemType.None)
            {
                m_specialItemIcon.Visible = true;
                m_specialItemIcon.ChangeSprite(SpecialItemType.SpriteName(Game.PlayerStats.SpecialItem));
                m_iconHolder2.Opacity = 1;
            }
        }

        public void UpdateSpellIcon()
        {
            m_spellIcon.Visible = false;
            m_iconHolder1.Opacity = 0.5f;
            m_spellCost.Visible = false;

            if (Game.PlayerStats.Spell != SpellType.None)
            {
                m_spellIcon.ChangeSprite(SpellType.Icon(Game.PlayerStats.Spell));
                m_spellIcon.Visible = true;
                m_iconHolder1.Opacity = 1;
                m_spellCost.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_spellCost));
                m_spellCost.Text = (int)(SpellEV.GetManaCost(Game.PlayerStats.Spell) * (1 - SkillSystem.GetSkill(SkillType.Mana_Cost_Down).ModifierAmount)) + " " + LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_15", null);
                m_spellCost.Visible = true;
            }
        }

        public override void Draw(Camera2D camera)
        {
            if (this.Visible == true)
            {
                if (ShowBarsOnly == false)
                {
                    base.Draw(camera);
                    m_coin.Draw(camera);

                    m_playerLevelText.Draw(camera);
                    m_goldText.Draw(camera);

                    camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                    foreach (SpriteObj sprite in m_abilitiesSpriteArray)
                        sprite.Draw(camera);

                    m_iconHolder1.Draw(camera);
                    m_iconHolder2.Draw(camera);
                    m_spellIcon.Draw(camera);
                    m_specialItemIcon.Draw(camera);

                    camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                    m_spellCost.Draw(camera);
                }

                m_mpBar.Draw(camera);
                m_mpText.Draw(camera);
                if (Game.PlayerStats.Traits.X != TraitType.CIP && Game.PlayerStats.Traits.Y != TraitType.CIP)
                {
                    m_hpBar.Draw(camera);
                    m_hpText.Draw(camera);
                }

                m_mpBarContainer.Draw(camera);
                m_hpBarContainer.Draw(camera);
            }
        }

        public void RefreshTextObjs()
        {
            if (Game.PlayerStats.Spell != SpellType.None)
            {
                m_spellCost.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_spellCost));
                m_spellCost.Text = (int)(SpellEV.GetManaCost(Game.PlayerStats.Spell) * (1 - SkillSystem.GetSkill(SkillType.Mana_Cost_Down).ModifierAmount)) + " " + LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_15", null);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                foreach (SpriteObj sprite in m_abilitiesSpriteArray)
                    sprite.Dispose();
                Array.Clear(m_abilitiesSpriteArray, 0, m_abilitiesSpriteArray.Length);
                m_abilitiesSpriteArray = null;

                m_coin.Dispose();
                m_coin = null;
                m_mpBar.Dispose();
                m_mpBar = null;
                m_hpBar.Dispose();
                m_hpBar = null;
                m_playerLevelText.Dispose();
                m_playerLevelText = null;
                m_goldText.Dispose();
                m_goldText = null;
                m_hpText.Dispose();
                m_hpText = null;
                m_mpText.Dispose();
                m_mpText = null;

                m_hpBarContainer.Dispose();
                m_hpBarContainer = null;
                m_mpBarContainer.Dispose();
                m_mpBarContainer = null;

                m_specialItemIcon.Dispose();
                m_specialItemIcon = null;
                m_spellIcon.Dispose();
                m_spellIcon = null;

                m_spellCost.Dispose();
                m_spellCost = null;

                m_iconHolder1.Dispose();
                m_iconHolder1 = null;
                m_iconHolder2.Dispose();
                m_iconHolder2 = null;
                base.Dispose();
            }
        }
    }
}

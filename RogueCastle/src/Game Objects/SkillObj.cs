using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;
using Microsoft.Xna.Framework.Input;

namespace RogueCastle
{
    public class SkillObj : SpriteObj
    {
        public string NameLocID { get; set; }
        public string DescLocID { get; set; }
        public string InputDescLocID { get; set; }
        //public string[] InputDescLocIDs; // locID, text, locID
        public string[] UnitLocIDs; // text, locID (some measurement units require leading space, some don't)

        // Not used anymore
        //public string Description { get; set; }
        //public string InputDescription { get; set; }

        public float PerLevelModifier { get; set; }
        public int BaseCost { get; set; }
        public int Appreciation { get; set; }
        public int MaxLevel { get; set; }
        public SkillType TraitType { get; set; }
        public string IconName { get; set; }
        private TextObj LevelText;
        public string UnitOfMeasurement { get; set; }
        public byte StatType { get; set; } // Dummy property. No longer used.
        public bool DisplayStat { get; set; } // Dummy property. No longer used.

        private SpriteObj m_coinIcon;

        public SkillObj(string spriteName)
            : base(spriteName)
        {
            StatType = TraitStatType.PlayerMaxHealth;
            DisplayStat = false;
            this.Visible = false;
            this.ForceDraw = true;
            LevelText = new TextObj(Game.JunicodeFont);
            LevelText.FontSize = 10;
            LevelText.Align = Types.TextAlign.Centre;
            LevelText.OutlineWidth = 2;
            //InputDescription = "";
            this.OutlineWidth = 2;
            //LevelText.DropShadow = new Vector2(1, 1);

            m_coinIcon = new SpriteObj("UpgradeIcon_Sprite");
            //m_coinIcon.Scale = new Vector2(0.7f, 0.7f);
        }

        public override void Draw(Camera2D camera)
        {
            if (this.Opacity > 0)
            {
                float storedOpacity = this.Opacity;
                this.TextureColor = Color.Black;
                this.Opacity = 0.5f;
                this.X += 8;
                this.Y += 8;
                base.Draw(camera);

                this.X -= 8;
                this.Y -= 8;
                this.TextureColor = Color.White;
                this.Opacity = storedOpacity;
            }
            base.Draw(camera);

            LevelText.Position = new Vector2(this.X, this.Bounds.Bottom - LevelText.Height/2);
            LevelText.Text = this.CurrentLevel + "/" + this.MaxLevel;
            LevelText.Opacity = this.Opacity;
            if (this.CurrentLevel >= this.MaxLevel)
            {
                LevelText.TextureColor = Color.Yellow;
                LevelText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_19", LevelText); //"Max Level"; //Teddy Changed Max Level to Max
            }
            else
                LevelText.TextureColor = Color.White;

            LevelText.Draw(camera);

            if (Game.PlayerStats.Gold >= this.TotalCost && this.CurrentLevel < this.MaxLevel)
            {
                m_coinIcon.Opacity = this.Opacity;
                m_coinIcon.Position = new Vector2(this.X + 18, this.Y - 40);
                m_coinIcon.Draw(camera);
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new SkillObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            SkillObj clone = obj as SkillObj;
            //clone.Description = this.Description;
            clone.PerLevelModifier = this.PerLevelModifier;
            clone.BaseCost = this.BaseCost;
            clone.Appreciation = this.Appreciation;
            clone.MaxLevel = this.MaxLevel;
            clone.CurrentLevel = this.CurrentLevel;
            clone.TraitType = this.TraitType;
            //clone.InputDescription = this.InputDescription;
            clone.UnitOfMeasurement = this.UnitOfMeasurement;
            clone.StatType = this.StatType;
            clone.DisplayStat = this.DisplayStat;
            clone.NameLocID = this.NameLocID;
            clone.DescLocID = this.DescLocID;
            clone.InputDescLocID = this.InputDescLocID;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                LevelText.Dispose();
                LevelText = null;

                m_coinIcon.Dispose();
                m_coinIcon = null;

                base.Dispose();
            }
        }

        private int m_currentLevel = 0;
        public int CurrentLevel // Prevent the current level from going beyond the max level.
        {
            get { return m_currentLevel; }
            set
            {
                if (value > MaxLevel)
                    m_currentLevel = MaxLevel;
                else
                    m_currentLevel = value;
            }
        }

        public int TotalCost
        {
            get { return BaseCost + (CurrentLevel * Appreciation) + (GameEV.SKILL_LEVEL_COST_INCREASE * Game.PlayerStats.CurrentLevel); }
        }

        public float ModifierAmount
        {
            get 
            {
                return CurrentLevel * PerLevelModifier; 
            }
        }
    }
}

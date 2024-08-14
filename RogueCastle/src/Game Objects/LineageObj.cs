using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using System.Text.RegularExpressions;

namespace RogueCastle
{
    public class LineageObj : ObjContainer // An object specific to the lineage screen, represents a single individual in a line of lineage.
    {
        private ObjContainer m_playerSprite;
        private TextObj m_playerName;
        private TextObj m_trait1Title;
        private TextObj m_trait2Title;
        private TextObj m_ageText;
        private TextObj m_classTextObj;

        private SpriteObj m_frameSprite;
        private SpriteObj m_plaqueSprite;

        private SpriteObj m_spellIcon;
        private SpriteObj m_spellIconHolder;

        private bool m_isDead = false;

        public bool BeatenABoss = false;
        public int NumEnemiesKilled = 0;

        public byte Age = 30;
        public byte ChildAge = 4;
        public byte Class = 0;
        public byte Spell = 0;
        public bool IsFemale = false;
        public bool FlipPortrait = false;
        public string RomanNumeral = "";
        private string m_playerNameString = "";

        public string PlayerName
        {
            get { return m_playerNameString; }
            set
            {
                try
                {
                    m_playerName.ChangeFontNoDefault(m_playerName.defaultFont);
                    m_playerNameString = value;
                    m_playerName.Text = Game.NameHelper(m_playerNameString, RomanNumeral, IsFemale);
                    if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(m_playerName.Text, @"\p{IsCyrillic}"))
                        m_playerName.ChangeFontNoDefault(Game.RobotoSlabFont);
                }
                catch
                {
                    m_playerName.ChangeFontNoDefault(Game.NotoSansSCFont);
                    m_playerNameString = value;
                    m_playerName.Text = Game.NameHelper(m_playerNameString, RomanNumeral, IsFemale);
                }
            }
        }

        public byte ChestPiece { get; set; }
        public byte HeadPiece { get; set; }
        public byte ShoulderPiece { get; set; }

        public bool DisablePlaque { get; set; }

        public Vector2 Traits { get; internal set; }

        private int m_textYPos = 140;

        private Color m_skinColour1 = new Color(231, 175, 131, 255);
        private Color m_skinColour2 = new Color(199, 109, 112, 255);
        private Color m_lichColour1 = new Color(255, 255, 255, 255);
        private Color m_lichColour2 = new Color(198, 198, 198, 255);

        public LineageObj(LineageScreen screen, bool createEmpty = false)
        {
            this.Name = "";

            m_frameSprite = new SpriteObj("LineageScreenFrame_Sprite");
            //m_frameSprite.ForceDraw = true;
            m_frameSprite.Scale = new Vector2(2.8f, 2.8f);
            m_frameSprite.DropShadow = new Vector2(4, 6);

            m_plaqueSprite = new SpriteObj("LineageScreenPlaque1Long_Sprite");
            //m_plaqueSprite.ForceDraw = true;
            m_plaqueSprite.Scale = new Vector2(1.8f, 2);

            m_playerSprite = new ObjContainer("PlayerIdle_Character");
            //m_playerSprite.ForceDraw = true;
            //m_playerSprite.PlayAnimation(true);
            m_playerSprite.AnimationDelay = 1 / 10f;
            m_playerSprite.Scale = new Vector2(2, 2);
            m_playerSprite.OutlineWidth = 2;
            m_playerSprite.GetChildAt(PlayerPart.Sword1).Visible = false;
            m_playerSprite.GetChildAt(PlayerPart.Sword2).Visible = false;

            m_playerSprite.GetChildAt(PlayerPart.Cape).TextureColor = Color.Red;
            m_playerSprite.GetChildAt(PlayerPart.Hair).TextureColor = Color.Red;
            m_playerSprite.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_playerSprite.GetChildAt(PlayerPart.Light).Visible = false;

            Color darkPink = new Color(251, 156, 172);
            m_playerSprite.GetChildAt(PlayerPart.Bowtie).TextureColor = darkPink;

            m_playerName = new TextObj(Game.JunicodeFont);
            m_playerName.FontSize = 10;
            m_playerName.Text = "Sir Skunky IV";
            m_playerName.Align = Types.TextAlign.Centre;
            m_playerName.OutlineColour = new Color(181, 142, 39);
            m_playerName.OutlineWidth = 2;
            m_playerName.Y = m_textYPos;
            m_playerName.LimitCorners = true;
            m_playerName.X = 5;
            this.AddChild(m_playerName);

            m_classTextObj = new TextObj(Game.JunicodeFont);
            m_classTextObj.FontSize = 8;
            m_classTextObj.Align = Types.TextAlign.Centre;
            m_classTextObj.OutlineColour = new Color(181, 142, 39);
            m_classTextObj.OutlineWidth = 2;
            m_classTextObj.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_classTextObj); // dummy locID to add TextObj to language refresh list
            m_classTextObj.Y = m_playerName.Y + m_playerName.Height - 8;
            m_classTextObj.LimitCorners = true;
            m_classTextObj.X = 5;
            this.AddChild(m_classTextObj);

            m_trait1Title = new TextObj(Game.JunicodeFont);
            m_trait1Title.FontSize = 8;
            m_trait1Title.Align = Types.TextAlign.Centre;
            m_trait1Title.OutlineColour = new Color(181, 142, 39);
            m_trait1Title.OutlineWidth = 2;
            m_trait1Title.X = 5;
            m_trait1Title.Y = m_classTextObj.Y + m_classTextObj.Height + 2;
            m_trait1Title.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_trait1Title); // dummy locID to add TextObj to language refresh list
            m_trait1Title.LimitCorners = true;
            this.AddChild(m_trait1Title);

            m_trait2Title = m_trait1Title.Clone() as TextObj;
            m_trait2Title.Y += 20;
            m_trait2Title.Text = "";
            m_trait2Title.LimitCorners = true;
            this.AddChild(m_trait2Title);

            m_ageText = m_trait1Title.Clone() as TextObj;
            m_ageText.Text = "xxx - xxx";
            m_ageText.Visible = false;
            m_ageText.LimitCorners = true;
            this.AddChild(m_ageText);

            m_spellIcon = new SpriteObj("Blank_Sprite");
            m_spellIcon.OutlineWidth = 1;

            m_spellIconHolder = new SpriteObj("BlacksmithUI_IconBG_Sprite");

            if (createEmpty == false)
            {
                // Setting gender.
                IsFemale = false;
                if (CDGMath.RandomInt(0, 1) > 0)
                    IsFemale = true;

                // Creating random name.
                if (IsFemale == true)
                    RomanNumeral = CreateFemaleName(screen);
                else
                    RomanNumeral = CreateMaleName(screen);

                // Selecting random traits.
                this.Traits = TraitType.CreateRandomTraits();

                // Selecting random class.
                this.Class = ClassType.GetRandomClass();

                string classText = "";
                if (LocaleBuilder.languageType == LanguageType.English)
                {
                    classText += LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_2_MALE", true);
                    classText += " ";
                    //m_classTextObj.Text = LocaleBuilder.getResourceString(this.IsFemale ? "LOC_ID_LINEAGE_OBJ_2_FEMALE" : "LOC_ID_LINEAGE_OBJ_2_MALE") +
                    //     " " + LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale));
                }
                m_classTextObj.Text = classText + LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale));

                // Special check to make sure lich doesn't get dextrocardia.
                while ((this.Class == ClassType.Lich || this.Class == ClassType.Lich2) && (this.Traits.X == TraitType.Dextrocardia || this.Traits.Y == TraitType.Dextrocardia))
                    this.Traits = TraitType.CreateRandomTraits();

                // Special check to make sure wizard don't get savantism.
                while ((this.Class == ClassType.Wizard || this.Class == ClassType.Wizard2 || this.Class == ClassType.Dragon) && (this.Traits.X == TraitType.Savant || this.Traits.Y == TraitType.Savant))
                    this.Traits = TraitType.CreateRandomTraits();

                // Selecting random spell.  There's a check to make sure savants don't get particular spells.
                byte[] spellList = ClassType.GetSpellList(this.Class);
                do
                {
                    this.Spell = spellList[CDGMath.RandomInt(0, spellList.Length - 1)];
                } while ((this.Spell == SpellType.DamageShield || this.Spell == SpellType.TimeStop || this.Spell == SpellType.Translocator) && (this.Traits.X == TraitType.Savant || this.Traits.Y == TraitType.Savant));
                Array.Clear(spellList, 0, spellList.Length);

                // Setting age.
                Age = (byte)CDGMath.RandomInt(18, 30);
                ChildAge = (byte)CDGMath.RandomInt(2, 5);

                // This call updates the player's graphics.
                UpdateData();
            }
        }

        private string CreateMaleName(LineageScreen screen)
        {
            string name = Game.NameArray[CDGMath.RandomInt(0, Game.NameArray.Count - 1)];
            if (screen != null) // Make sure the current branch never has matching names.
            {
                int countBreaker = 0;
                while (screen.CurrentBranchNameCopyFound(name) == true)
                {
                    name = Game.NameArray[CDGMath.RandomInt(0, Game.NameArray.Count - 1)];
                    countBreaker++;
                    if (countBreaker > 20)
                        break;
                }
            }

            if (name != null)
            {
                if (name.Length > 10)
                    name = name.Substring(0, 9) + ".";

                int nameNumber = 0;
                string romanNumerals = "";

                if (screen != null)
                    nameNumber = screen.NameCopies(name);
                if (nameNumber > 0)
                    romanNumerals = CDGMath.ToRoman(nameNumber + 1);

                RomanNumeral = romanNumerals;
                PlayerName = name;
                return romanNumerals;
            }
            else
                PlayerName = "Hero";

            return "";
        }

        private string CreateFemaleName(LineageScreen screen)
        {
            string name = Game.FemaleNameArray[CDGMath.RandomInt(0, Game.FemaleNameArray.Count - 1)];
            if (screen != null) // Make sure the current branch never has matching names.
            {
                int countBreaker = 0;
                while (screen.CurrentBranchNameCopyFound(name) == true)
                {
                    name = Game.FemaleNameArray[CDGMath.RandomInt(0, Game.FemaleNameArray.Count - 1)];
                    countBreaker++;
                    if (countBreaker > 20)
                        break;
                }
            }

            if (name != null)
            {
                if (name.Length > 10)
                    name = name.Substring(0, 9) + ".";

                int nameNumber = 0;
                string romanNumerals = "";

                if (screen != null)
                    nameNumber = screen.NameCopies(name);
                if (nameNumber > 0)
                    romanNumerals = CDGMath.ToRoman(nameNumber + 1);

                RomanNumeral = romanNumerals;
                PlayerName = name;
                return romanNumerals;
            }
            else
                PlayerName = "Heroine";

            return "";
        }

        public void RandomizePortrait()
        {
            int randHeadPiece = CDGMath.RandomInt(1, PlayerPart.NumHeadPieces);
            int randShoulderPiece = CDGMath.RandomInt(1, PlayerPart.NumShoulderPieces);
            int randChestPiece = CDGMath.RandomInt(1, PlayerPart.NumChestPieces);

            if (this.Class == ClassType.Traitor)
                randHeadPiece = PlayerPart.IntroHelm; // Force the head piece to be Johanne's headpiece if you are playing as the fountain.
            else if (this.Class == ClassType.Dragon)
                randHeadPiece = PlayerPart.DragonHelm;

            SetPortrait((byte)randHeadPiece, (byte)randShoulderPiece, (byte)randChestPiece);
        }

        public void SetPortrait(byte headPiece, byte shoulderPiece, byte chestPiece)
        {
            HeadPiece = headPiece;
            ShoulderPiece = shoulderPiece;
            ChestPiece = chestPiece;

            string headPart = (m_playerSprite.GetChildAt(PlayerPart.Head) as IAnimateableObj).SpriteName;
            int numberIndex = headPart.IndexOf("_") - 1;
            headPart = headPart.Remove(numberIndex, 1);
            headPart = headPart.Replace("_", HeadPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.Head).ChangeSprite(headPart);

            string chestPart = (m_playerSprite.GetChildAt(PlayerPart.Chest) as IAnimateableObj).SpriteName;
            numberIndex = chestPart.IndexOf("_") - 1;
            chestPart = chestPart.Remove(numberIndex, 1);
            chestPart = chestPart.Replace("_", ChestPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.Chest).ChangeSprite(chestPart);

            string shoulderAPart = (m_playerSprite.GetChildAt(PlayerPart.ShoulderA) as IAnimateableObj).SpriteName;
            numberIndex = shoulderAPart.IndexOf("_") - 1;
            shoulderAPart = shoulderAPart.Remove(numberIndex, 1);
            shoulderAPart = shoulderAPart.Replace("_", ShoulderPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.ShoulderA).ChangeSprite(shoulderAPart);

            string shoulderBPart = (m_playerSprite.GetChildAt(PlayerPart.ShoulderB) as IAnimateableObj).SpriteName;
            numberIndex = shoulderBPart.IndexOf("_") - 1;
            shoulderBPart = shoulderBPart.Remove(numberIndex, 1);
            shoulderBPart = shoulderBPart.Replace("_", ShoulderPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.ShoulderB).ChangeSprite(shoulderBPart);
        }

        public void UpdateAge(int currentEra)
        {
            int startingEra = currentEra - ChildAge;
            int endingEra = currentEra + Age;
            m_ageText.Text = startingEra + " - " + endingEra;
        }

        public void UpdateData()
        {
            SetTraits(Traits);

            m_playerSprite.GetChildAt(PlayerPart.Hair).Visible = true;
            if (this.Traits.X == TraitType.Baldness || this.Traits.Y == TraitType.Baldness)
                m_playerSprite.GetChildAt(PlayerPart.Hair).Visible = false;

            // flibit added this.
            FlipPortrait = false;
            m_playerSprite.Rotation = 0;
            if (this.Traits.X == TraitType.Vertigo || this.Traits.Y == TraitType.Vertigo)
                FlipPortrait = true;

            string classText = "";
            if (LocaleBuilder.languageType == LanguageType.English)
            {
                classText += LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_2_MALE", true);
                classText += " ";
                //m_classTextObj.Text = LocaleBuilder.getResourceString(this.IsFemale ? "LOC_ID_LINEAGE_OBJ_2_FEMALE" : "LOC_ID_LINEAGE_OBJ_2_MALE") +
                //     " " + LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale));
            }
            m_classTextObj.Text = classText + LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale));

            m_spellIcon.ChangeSprite(SpellType.Icon(this.Spell));

            if (this.Class == ClassType.Knight || this.Class == ClassType.Knight2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("PlayerIdleShield_Sprite");
            }
            else if (this.Class == ClassType.Banker || this.Class == ClassType.Banker2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("PlayerIdleLamp_Sprite");
            }
            else if (this.Class == ClassType.Wizard || this.Class == ClassType.Wizard2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("PlayerIdleBeard_Sprite");
            }
            else if (this.Class == ClassType.Ninja || this.Class == ClassType.Ninja2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("PlayerIdleHeadband_Sprite");
            }
            else if (this.Class == ClassType.Barbarian || this.Class == ClassType.Barbarian2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("PlayerIdleHorns_Sprite");
            }
            else
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = false;

            // Special code for dragon.
            m_playerSprite.GetChildAt(PlayerPart.Wings).Visible = false;
            if (this.Class == ClassType.Dragon)
            {
                m_playerSprite.GetChildAt(PlayerPart.Wings).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerIdleHead" + PlayerPart.DragonHelm + "_Sprite");
            }

            //Special code for traitor.
            if (this.Class == ClassType.Traitor)
                m_playerSprite.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerIdleHead" + PlayerPart.IntroHelm + "_Sprite");

            // This is for male/female counterparts
            if (IsFemale == false)
            {
                m_playerSprite.GetChildAt(PlayerPart.Boobs).Visible = false;
                m_playerSprite.GetChildAt(PlayerPart.Bowtie).Visible = false;
            }
            else
            {
                m_playerSprite.GetChildAt(PlayerPart.Boobs).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Bowtie).Visible = true;
            }

            m_playerSprite.Scale = new Vector2(2);
            if (Traits.X == TraitType.Gigantism || Traits.Y == TraitType.Gigantism)
                m_playerSprite.Scale = new Vector2(GameEV.TRAIT_GIGANTISM, GameEV.TRAIT_GIGANTISM);
            if (Traits.X == TraitType.Dwarfism|| Traits.Y == TraitType.Dwarfism)
                m_playerSprite.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            if (Traits.X == TraitType.Ectomorph || Traits.Y == TraitType.Ectomorph)
            {
                m_playerSprite.ScaleX *= 0.825f;
                m_playerSprite.ScaleY *= 1.25f;
            }

            if (Traits.X == TraitType.Endomorph || Traits.Y == TraitType.Endomorph)
            {
                m_playerSprite.ScaleX *= 1.25f;
                m_playerSprite.ScaleY *= 1.175f;
            }

            if (this.Class == ClassType.SpellSword || this.Class == ClassType.SpellSword2)
                m_playerSprite.OutlineColour = Color.White;
            else
                m_playerSprite.OutlineColour = Color.Black;
        }

        bool m_frameDropping = false;
        public void DropFrame()
        {
            m_frameDropping = true;
            Tween.By(m_frameSprite, 0.7f, Tweener.Ease.Back.EaseOut, "X", (-m_frameSprite.Width/2f - 2).ToString(), "Y", "30", "Rotation", "45");
            Tween.By(m_playerSprite, 0.7f, Tweener.Ease.Back.EaseOut, "X", (-m_frameSprite.Width / 2f - 2).ToString(), "Y", "30", "Rotation", "45");
            Tween.RunFunction(1.5f, this, "DropFrame2");
        }

        public void DropFrame2()
        {
            SoundManager.PlaySound("Cutsc_Picture_Fall");
            Tween.By(m_frameSprite, 0.5f, Tweener.Ease.Quad.EaseIn, "Y", "1000");
            Tween.By(m_playerSprite, 0.5f, Tweener.Ease.Quad.EaseIn, "Y", "1000");
        }

        public override void Draw(Camera2D camera)
        {
            //m_playerSprite.Rotation = 0;
            if (FlipPortrait == true)
                m_playerSprite.Rotation = 180;

            if (m_frameDropping == false)
            {
                m_frameSprite.Position = this.Position;
                m_frameSprite.Y -= 12;
                m_frameSprite.X += 5;
            }
            m_frameSprite.Opacity = this.Opacity;
            m_frameSprite.Draw(camera);

            if (this.IsDead == false && this.Spell != SpellType.None)
            {
                m_spellIconHolder.Position = new Vector2(m_frameSprite.X, m_frameSprite.Bounds.Bottom - 20);
                m_spellIcon.Position = m_spellIconHolder.Position;
                m_spellIconHolder.Draw(camera);
                m_spellIcon.Draw(camera);
            }

            m_playerSprite.OutlineColour = this.OutlineColour;
            m_playerSprite.OutlineWidth = this.OutlineWidth;
            if (m_frameDropping == false)
            {
                m_playerSprite.Position = this.Position;
                m_playerSprite.X += 10;
                if (FlipPortrait == true)
                {
                    m_playerSprite.X -= 10;
                    m_playerSprite.Y -= 30;
                }
            }
            m_playerSprite.Opacity = this.Opacity;
            m_playerSprite.Draw(camera);

            // only apply the lich effect if the lineageObj is being drawn.
            if (CollisionMath.Intersects(this.Bounds, camera.Bounds))
            {
                if (this.Class == ClassType.Lich || this.Class == ClassType.Lich2)
                {
                    // This is the Tint Removal effect, that removes the tint from his face.
                    Game.ColourSwapShader.Parameters["desiredTint"].SetValue(Color.White.ToVector4());
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(m_playerSprite.Opacity);

                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(m_lichColour1.ToVector4());

                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(m_lichColour2.ToVector4());

                    camera.End();
                    camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game.ColourSwapShader, camera.GetTransformation());
                    m_playerSprite.GetChildAt(PlayerPart.Head).Draw(camera);
                    camera.End();
                    camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetTransformation());
                    if (IsFemale == true)
                        m_playerSprite.GetChildAt(PlayerPart.Bowtie).Draw(camera);
                    m_playerSprite.GetChildAt(PlayerPart.Extra).Draw(camera);
                }
                else if (this.Class == ClassType.Assassin || this.Class == ClassType.Assassin2)
                {
                    // This is the Tint Removal effect, that removes the tint from his face.
                    Game.ColourSwapShader.Parameters["desiredTint"].SetValue(Color.White.ToVector4());
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(m_playerSprite.Opacity);

                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(Color.Black.ToVector4());

                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(Color.Black.ToVector4());

                    camera.End();
                    camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game.ColourSwapShader, camera.GetTransformation());
                    m_playerSprite.GetChildAt(PlayerPart.Head).Draw(camera);
                    camera.End();
                    camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetTransformation());
                    if (IsFemale == true)
                        m_playerSprite.GetChildAt(PlayerPart.Bowtie).Draw(camera);
                    m_playerSprite.GetChildAt(PlayerPart.Extra).Draw(camera);
                }
            }

            if (DisablePlaque == false)
            {
                if (m_frameDropping == false)
                {
                    m_plaqueSprite.Position = this.Position;
                    m_plaqueSprite.X += 5;
                    m_plaqueSprite.Y = m_frameSprite.Y + m_frameSprite.Height - 30;
                }
                m_plaqueSprite.Draw(camera);
                camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                base.Draw(camera); // Base draws the text.
                camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            }

            // Makes sure the frame is drawn in front of the plaque when it's doing its fall animation.
            if (m_frameDropping == true)
            {
                m_frameSprite.Draw(camera);
                m_playerSprite.Draw(camera);
            }
        }

        public void SetTraits(Vector2 traits)
        {
            this.Traits = traits;
            string traitString = "";
            if (Traits.X != 0)
                traitString += LocaleBuilder.getResourceString(TraitType.ToStringID((byte)Traits.X));
            //m_trait1Title.Text = TraitType.ToString((byte)Traits.X);
            else
                m_trait1Title.Text = "";
            if (Traits.Y != 0)
            {
                if (traits.X != 0)
                    traitString += ", " + LocaleBuilder.getResourceString(TraitType.ToStringID((byte)Traits.Y));
                else
                    traitString += LocaleBuilder.getResourceString(TraitType.ToStringID((byte)Traits.Y));
            }
                //m_trait2Title.Text = TraitType.ToString((byte)Traits.Y);

            m_trait1Title.Text = traitString;

            if (IsDead == false)
            {
                m_plaqueSprite.ScaleX = 1.8f;
                // Auto-scale the plaque if the trait text is too large.
                if (traits.X != TraitType.None)
                {
                    float maxWidth = m_plaqueSprite.Width;
                    float traitWidth = m_trait1Title.Width + 50;
                    if (traitWidth > maxWidth)
                        m_plaqueSprite.ScaleX *= traitWidth / maxWidth;
                }
                //m_trait1Title.WordWrap(200);
            }
        }

        public void ClearTraits()
        {
            Traits = Vector2.Zero;
            m_trait1Title.Text = LocaleBuilder.getString("LOC_ID_LINEAGE_OBJ_1", m_trait1Title);
            m_trait2Title.Text = "";
        }

        public void OutlineLineageObj(Color color, int width)
        {
            m_plaqueSprite.OutlineColour = color;
            m_plaqueSprite.OutlineWidth = width;

            m_frameSprite.OutlineColour = color;
            m_frameSprite.OutlineWidth = width;
        }

        public void UpdateClassRank()
        {
            string className = "";
            string locIDToUse = "LOC_ID_LINEAGE_OBJ_4_NEW";

            if (LocaleBuilder.languageType == LanguageType.English)
            {
                //!this.IsFemale ? LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_2_MALE") : LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_2_FEMALE");

                className += LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_2_MALE", true);
                className += " ";
            }

            if (BeatenABoss == true)
                locIDToUse = "LOC_ID_LINEAGE_OBJ_3_NEW";
            else
            {
                if (NumEnemiesKilled < 5)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_4_NEW";
                else if (NumEnemiesKilled >= 5 && NumEnemiesKilled < 10)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_5_NEW";
                else if (NumEnemiesKilled >= 10 && NumEnemiesKilled < 15)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_6_NEW";
                else if (NumEnemiesKilled >= 15 && NumEnemiesKilled < 20)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_7_NEW";
                else if (NumEnemiesKilled >= 20 && NumEnemiesKilled < 25)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_8_NEW";
                else if (NumEnemiesKilled >= 25 && NumEnemiesKilled < 30)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_9_NEW";
                else if (NumEnemiesKilled >= 30 && NumEnemiesKilled < 35)
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_10_NEW";
                else
                    locIDToUse = "LOC_ID_LINEAGE_OBJ_11_NEW";
            }

            className += string.Format(LocaleBuilder.getResourceStringCustomFemale(locIDToUse, this.IsFemale), LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale)));
            m_classTextObj.Text = className;

            m_plaqueSprite.ScaleX = 1.8f;
            // Auto-scale the plaque if the class rank text is too large.
            float maxWidth = m_plaqueSprite.Width;
            float classRankWidth = m_classTextObj.Width + 50;
            if (classRankWidth > maxWidth)
                m_plaqueSprite.ScaleX *= classRankWidth / maxWidth;
        }

        public bool IsDead
        {
            get { return m_isDead; }
            set
            {
                m_isDead = value;
                if (value == true)
                {
                    m_trait1Title.Visible = false;
                    m_trait2Title.Visible = false;
                    m_ageText.Visible = true;
                }
                else
                {
                    if (m_hasProsopagnosia == true)
                    {
                        m_trait1Title.Visible = false;
                        m_trait2Title.Visible = false;
                    }
                    else
                    {
                        m_trait1Title.Visible = true;
                        m_trait2Title.Visible = true;
                    }
                    m_ageText.Visible = false;
                }
            }
        }

        
        private bool m_hasProsopagnosia = false;
        public bool HasProsopagnosia
        {
            set
            {
                m_hasProsopagnosia = value;
                if (m_isDead == false)
                {
                    if (value == true)
                    {
                        m_trait1Title.Visible = false;
                        m_trait2Title.Visible = false;
                    }
                    else
                    {
                        m_trait1Title.Visible = true;
                        m_trait2Title.Visible = true;
                    }
                }
            }
            get { return m_hasProsopagnosia; }
        }


        public override Rectangle Bounds
        {
            get { return m_playerSprite.Bounds; }
        }

        public override Rectangle AbsBounds
        {
            get { return m_playerSprite.Bounds; }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new LineageObj(null); // Not used.
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_playerSprite.Dispose();
                m_playerSprite = null;

                m_trait1Title = null;
                m_trait2Title = null;
                m_ageText = null;
                m_playerName = null;
                m_classTextObj = null;

                m_frameSprite.Dispose();
                m_frameSprite = null;
                m_plaqueSprite.Dispose();
                m_plaqueSprite = null;

                m_spellIcon.Dispose();
                m_spellIcon = null;

                m_spellIconHolder.Dispose();
                m_spellIconHolder = null;

                base.Dispose();
            }
        }

        public void RefreshTextObjs()
        {
            if (this.IsDead)
                UpdateClassRank();
            else
            {
                string classText = "";
                if (LocaleBuilder.languageType == LanguageType.English)
                {
                    classText += LocaleBuilder.getResourceString("LOC_ID_LINEAGE_OBJ_2_MALE", true);
                    classText += " ";
                    //m_classTextObj.Text = LocaleBuilder.getResourceString(this.IsFemale ? "LOC_ID_LINEAGE_OBJ_2_FEMALE" : "LOC_ID_LINEAGE_OBJ_2_MALE") +
                    //     " " + LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale));
                }
                m_classTextObj.Text = classText + LocaleBuilder.getResourceString(ClassType.ToStringID(this.Class, this.IsFemale));
            }

            PlayerName = PlayerName; // This refreshes the name.
            SetTraits(this.Traits);
        }
    }
}

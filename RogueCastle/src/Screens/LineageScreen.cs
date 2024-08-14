using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DS2DEngine;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using InputSystem;
using System.Text.RegularExpressions;

namespace RogueCastle
{
    public class LineageScreen : Screen
    {
        private SpriteObj m_titleText;

        private LineageObj m_startingLineageObj;
        private LineageObj m_selectedLineageObj;

        private int m_selectedLineageIndex = 0;
        private List<LineageObj> m_currentBranchArray;
        private List<LineageObj> m_masterArray;

        private Vector2 m_startingPoint;
        private Vector2 m_currentPoint;

        private BackgroundObj m_background;
        private SpriteObj m_bgShadow;
        private Tweener.TweenObject m_selectTween;

        private int m_xPosOffset = 400;

        private ObjContainer m_descriptionPlate;
        private int m_xShift = 0;

        private float m_storedMusicVol;

        private KeyIconTextObj m_confirmText;
        private KeyIconTextObj m_navigationText;
        private KeyIconTextObj m_rerollText;

        private bool m_lockControls;

        public LineageScreen()
        {
            m_startingPoint = new Vector2(1320 / 2, 720 / 2);
            m_currentPoint = m_startingPoint;
        }

        public override void LoadContent()
        {
            Game.HSVEffect.Parameters["Saturation"].SetValue(0); // What is this for?

            m_background = new BackgroundObj("LineageScreenBG_Sprite");
            m_background.SetRepeated(true, true, Camera);
            m_background.X -= 1320 * 5;

            m_bgShadow = new SpriteObj("LineageScreenShadow_Sprite");
            m_bgShadow.Scale = new Vector2(11, 11);
            m_bgShadow.Y -= 10;
            m_bgShadow.ForceDraw = true;
            m_bgShadow.Opacity = 0.9f;
            m_bgShadow.Position = new Vector2(1320 / 2f, 720 / 2f);

            m_titleText = new SpriteObj("LineageTitleText_Sprite");
            m_titleText.X = GlobalEV.ScreenWidth / 2;
            m_titleText.Y = GlobalEV.ScreenHeight * 0.1f;
            m_titleText.ForceDraw = true;

            int xPlatePos = 20;
            m_descriptionPlate = new ObjContainer("LineageScreenPlate_Character");
            m_descriptionPlate.ForceDraw = true;
            m_descriptionPlate.Position = new Vector2(1320 - m_descriptionPlate.Width - 30, (720 - m_descriptionPlate.Height)/2f);

            TextObj playerTitle = new TextObj(Game.JunicodeFont);
            playerTitle.FontSize = 12;
            playerTitle.Align = Types.TextAlign.Centre;
            playerTitle.OutlineColour = new Color(181, 142, 39);
            playerTitle.OutlineWidth = 2;
            playerTitle.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", playerTitle); // dummy locID to add TextObj to language refresh list
            playerTitle.OverrideParentScale = true;
            playerTitle.Position = new Vector2(m_descriptionPlate.Width / 2f, 15);
            playerTitle.LimitCorners = true;
            m_descriptionPlate.AddChild(playerTitle);

            TextObj className = playerTitle.Clone() as TextObj;
            className.FontSize = 10;
            className.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", className); // dummy locID to add TextObj to language refresh list
            className.Align = Types.TextAlign.Left;
            className.X = xPlatePos;
            className.Y += 40;
            m_descriptionPlate.AddChild(className);

            KeyIconTextObj classDescription = new KeyIconTextObj(Game.JunicodeFont);
            classDescription.FontSize = 8;
            classDescription.OutlineColour = className.OutlineColour;
            classDescription.OutlineWidth = 2;
            classDescription.OverrideParentScale = true;
            classDescription.Position = className.Position;
            classDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", classDescription); // dummy locID to add TextObj to language refresh list
            classDescription.Align = Types.TextAlign.Left;
            classDescription.Y += 30;
            classDescription.X = xPlatePos + 20;
            classDescription.LimitCorners = true;
            m_descriptionPlate.AddChild(classDescription);

            for (int i = 0; i < 2; i++)
            {
                TextObj traitName = className.Clone() as TextObj;
                traitName.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", traitName); // dummy locID to add TextObj to language refresh list
                traitName.X = xPlatePos;
                traitName.Align = Types.TextAlign.Left;
                if (i > 0)
                    traitName.Y = m_descriptionPlate.GetChildAt(m_descriptionPlate.NumChildren -1).Y + 50;
                m_descriptionPlate.AddChild(traitName);

                TextObj traitDescription = className.Clone() as TextObj;
                traitDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", traitDescription); // dummy locID to add TextObj to language refresh list
                traitDescription.X = xPlatePos + 20;
                traitDescription.FontSize = 8;
                traitDescription.Align = Types.TextAlign.Left;
                m_descriptionPlate.AddChild(traitDescription);
            }

            TextObj spellName = className.Clone() as TextObj;
            spellName.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", spellName); // dummy locID to add TextObj to language refresh list
            spellName.FontSize = 10;
            spellName.X = xPlatePos;
            spellName.Align = Types.TextAlign.Left;
            m_descriptionPlate.AddChild(spellName);

            //TextObj spellDescription = className.Clone() as TextObj;
            KeyIconTextObj spellDescription = new KeyIconTextObj(Game.JunicodeFont);
            spellDescription.OutlineColour = new Color(181, 142, 39);
            spellDescription.OutlineWidth = 2;
            spellDescription.OverrideParentScale = true;
            spellDescription.Position = new Vector2(m_descriptionPlate.Width / 2f, 15);
            spellDescription.Y += 40;
            spellDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", spellDescription); // dummy locID to add TextObj to language refresh list
            spellDescription.X = xPlatePos + 20;
            spellDescription.FontSize = 8;
            spellDescription.Align = Types.TextAlign.Left;
            spellDescription.LimitCorners = true;
            //spellDescription.ForcedScale = new Vector2(1.5f, 1.5f);
            m_descriptionPlate.AddChild(spellDescription);

            m_masterArray = new List<LineageObj>();
            m_currentBranchArray = new List<LineageObj>();

            Vector2 startingPos = Vector2.Zero; // The starting position for the current branch to be added.

            m_confirmText = new KeyIconTextObj(Game.JunicodeFont);
            m_confirmText.ForceDraw = true;
            m_confirmText.FontSize = 12;
            m_confirmText.DropShadow = new Vector2(2, 2);
            m_confirmText.Position = new Vector2(1320 - 40, 630);
            m_confirmText.Align = Types.TextAlign.Right;
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_confirmText); // dummy locID to add TextObj to language refresh list

            m_navigationText = new KeyIconTextObj(Game.JunicodeFont);
            m_navigationText.Align = Types.TextAlign.Right;
            m_navigationText.FontSize = 12;
            m_navigationText.DropShadow = new Vector2(2, 2);
            m_navigationText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 40);
            m_navigationText.ForceDraw = true;
            m_navigationText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_navigationText); // dummy locID to add TextObj to language refresh list

            m_rerollText = new KeyIconTextObj(Game.JunicodeFont);
            m_rerollText.Align = Types.TextAlign.Left;
            m_rerollText.FontSize = 12;
            m_rerollText.DropShadow = new Vector2(2, 2);
            m_rerollText.ForceDraw = true;
            m_rerollText.Position = new Vector2(0 + 30, GlobalEV.ScreenHeight - 50);
            m_rerollText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_rerollText); // dummy locID to add TextObj to language refresh list

            base.LoadContent();
        }

        public override void ReinitializeRTs()
        {
            m_background.SetRepeated(true, true, Camera);
            base.ReinitializeRTs();
        }

        private void UpdateDescriptionPlate()
        {
            LineageObj selectedObj = m_currentBranchArray[m_selectedLineageIndex];

            TextObj knightName = m_descriptionPlate.GetChildAt(1) as TextObj;
            try
            {
                knightName.ChangeFontNoDefault(knightName.defaultFont);
                knightName.Text = Game.NameHelper(selectedObj.PlayerName, selectedObj.RomanNumeral, selectedObj.IsFemale);
                if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(knightName.Text, @"\p{IsCyrillic}"))
                    knightName.ChangeFontNoDefault(Game.RobotoSlabFont);
            }
            catch
            {
                knightName.ChangeFontNoDefault(Game.NotoSansSCFont);
                knightName.Text = Game.NameHelper(selectedObj.PlayerName, selectedObj.RomanNumeral, selectedObj.IsFemale);
            }

            TextObj className = m_descriptionPlate.GetChildAt(2) as TextObj;
            className.Text = LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_5") + " - " + LocaleBuilder.getResourceString(ClassType.ToStringID(selectedObj.Class, selectedObj.IsFemale)); // "Class - ???"
            KeyIconTextObj classDescription = m_descriptionPlate.GetChildAt(3) as KeyIconTextObj;
            classDescription.Text = LocaleBuilder.getResourceStringCustomFemale(ClassType.DescriptionID(selectedObj.Class), selectedObj.IsFemale);
            classDescription.WordWrap(340);

            TextObj trait1Name = m_descriptionPlate.GetChildAt(4) as TextObj;
            trait1Name.Y = classDescription.Y + classDescription.Height + 5;
            TextObj trait1Description = m_descriptionPlate.GetChildAt(5) as TextObj;
            trait1Description.Y = trait1Name.Y + 30;

            int spellY = (int)trait1Name.Y;

            if (selectedObj.Traits.X > 0)
            {
                trait1Name.Text = LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_6") + " - " + LocaleBuilder.getResourceString(TraitType.ToStringID((byte)selectedObj.Traits.X));
                trait1Description.Text = LocaleBuilder.getResourceString(TraitType.DescriptionID((byte)selectedObj.Traits.X, selectedObj.IsFemale));
                trait1Description.WordWrap(340);

                spellY = (int)trait1Description.Y + trait1Description.Height + 5;
            }
            else
            {
                spellY = (int)trait1Name.Y + trait1Name.Height + 5;
                trait1Name.Text = LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_7");
                trait1Description.Text = "";
            }

            TextObj trait2Name = m_descriptionPlate.GetChildAt(6) as TextObj;
            trait2Name.Y = trait1Description.Y + trait1Description.Height + 5;
            TextObj trait2Description = m_descriptionPlate.GetChildAt(7) as TextObj;
            trait2Description.Y = trait2Name.Y + 30;

            if (selectedObj.Traits.Y > 0)
            {
                trait2Name.Text = LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_6") + " - " + LocaleBuilder.getResourceString(TraitType.ToStringID((byte)selectedObj.Traits.Y));
                trait2Description.Text = LocaleBuilder.getResourceString(TraitType.DescriptionID((byte)selectedObj.Traits.Y, selectedObj.IsFemale));
                trait2Description.WordWrap(340);

                spellY = (int)trait2Description.Y + trait2Description.Height + 5;
            }
            else
            {
                trait2Name.Text = "";
                trait2Description.Text = "";
            }

            if (Game.PlayerStats.HasProsopagnosia == true)
            {
                trait1Name.Visible = false;
                trait2Name.Visible = false;
                trait1Description.Visible = false;
                trait2Description.Visible = false;

                spellY = (int)trait1Name.Y;
            }
            else
            {
                trait1Name.Visible = true;
                trait2Name.Visible = true;
                trait1Description.Visible = true;
                trait2Description.Visible = true;
            }

            TextObj spellName = m_descriptionPlate.GetChildAt(8) as TextObj;
            spellName.Text = LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_8") + " - " + LocaleBuilder.getResourceString(SpellType.ToStringID(selectedObj.Spell));
            spellName.Y = spellY;
            KeyIconTextObj spellDescription = m_descriptionPlate.GetChildAt(9) as KeyIconTextObj;
            spellDescription.Text = "[Input:" + InputMapType.PLAYER_SPELL1 + "]  " + LocaleBuilder.getResourceString(SpellType.DescriptionID(selectedObj.Spell));
            spellDescription.Y = spellName.Y + 30;
            spellDescription.WordWrap(340);
        }

        /// <summary>
        /// Creates a current branch for the lineage.
        /// </summary>
        /// <param name="numLineages">The number of lineages in a branch to create</param>
        /// <param name="position">Where to start placing the centre lineage</param>
        /// <param name="createEmpty">Set this to false if you don't want to create random traits for this lineage. Useful when only loading lineages.</param>
        /// <param name="randomizePortrait">Whether to randomize the player portrait or not. Setting false sets all armor pieces to 1.</param>
        /// <param name="setActive">Whether this is an active branch or not. Active branches will be coloured, non-actives will be greyed out.</param>
        private void AddLineageRow(int numLineages, Vector2 position, bool createEmpty, bool randomizePortrait)
        {
            // Setting whether this lineage obj was the path the player went down.
            if (m_selectedLineageObj != null)
            {
                m_selectedLineageObj.ForceDraw = false;
                m_selectedLineageObj.Y = 0;
            }

            m_currentPoint = position;
            m_currentBranchArray.Clear();

            int[] yPosArray3 = new int[] { -450, 0, 450 };
            int[] yPosArray2 = new int[] { -200, 200 };

            for (int i = 0; i < numLineages; i++)
            {
                LineageObj obj = new LineageObj(this, createEmpty);

                if (randomizePortrait == true)
                    obj.RandomizePortrait();

                obj.ForceDraw = true;
                obj.X = position.X + m_xPosOffset;

                int[] posArray = yPosArray3;
                if (numLineages == 2)
                    posArray = yPosArray2;

                obj.Y = posArray[i];

                m_currentBranchArray.Add(obj);

                if (obj.Traits.X == TraitType.Vertigo || obj.Traits.Y == TraitType.Vertigo)
                    obj.FlipPortrait = true;

                obj.HasProsopagnosia = Game.PlayerStats.HasProsopagnosia;
            }

            m_currentPoint = m_currentBranchArray[1].Position;
            Camera.Position = m_currentPoint;
            m_selectedLineageObj = m_currentBranchArray[1];
            m_selectedLineageIndex = 1;
        }

        public override void OnEnter()
        {
            (Game.ScreenManager.Game as RogueCastle.Game).InitializeMaleNameArray(false);
            (Game.ScreenManager.Game as RogueCastle.Game).InitializeFemaleNameArray(false);

            m_lockControls = false;
            SoundManager.PlayMusic("SkillTreeSong", true, 1);

            m_storedMusicVol = SoundManager.GlobalMusicVolume;
            SoundManager.GlobalMusicVolume = 0;
            if (SoundManager.AudioEngine != null)
                SoundManager.AudioEngine.GetCategory("Legacy").SetVolume(m_storedMusicVol);

            // This is probably unnecessary.
            //if (Game.LineageSongCue != null && Game.LineageSongCue.IsPlaying)
            //{
            //    Game.LineageSongCue.Stop(AudioStopOptions.Immediate);
            //    Game.LineageSongCue.Dispose();
            //}

            //Game.LineageSongCue = SoundManager.GetMusicCue("LegacySong");
            //Game.LineageSongCue.Play();

            if (Game.LineageSongCue != null && Game.LineageSongCue.IsPlaying)
            {
                Game.LineageSongCue.Stop(AudioStopOptions.Immediate);
                Game.LineageSongCue.Dispose();
            }
            Game.LineageSongCue = SoundManager.GetMusicCue("LegacySong");
            if (Game.LineageSongCue != null)
                Game.LineageSongCue.Play();

            //Camera.Position = new Vector2(1320 / 2, 720 / 2);

            // Lineage Data loaded on start menu.

            LoadFamilyTreeData();
            LoadCurrentBranches();

            Camera.Position = m_selectedLineageObj.Position;
            UpdateDescriptionPlate();

            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_LINEAGE_SCREEN_1_NEW", m_confirmText);

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == true)
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_LINEAGE_SCREEN_2_NEW", m_navigationText);
            else
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_LINEAGE_SCREEN_3", m_navigationText);

            m_rerollText.Text = LocaleBuilder.getString("LOC_ID_LINEAGE_SCREEN_4_NEW", m_rerollText);
            if (SkillSystem.GetSkill(SkillType.Randomize_Children).ModifierAmount > 0 && Game.PlayerStats.RerolledChildren == false)
                m_rerollText.Visible = true;
            else
                m_rerollText.Visible = false;

            Game.ChangeBitmapLanguage(m_titleText, "LineageTitleText_Sprite");

            base.OnEnter();
        }

        public void LoadFamilyTreeData()
        {
            m_masterArray.Clear();

            // Create a base entry to represent the very first knight (not playable and not saved).
            int currentEra = GameEV.BASE_ERA;

            if (Game.PlayerStats.FamilyTreeArray != null && Game.PlayerStats.FamilyTreeArray.Count > 0)
            {
                int xPos = 0;

                // Loading the family tree.
                foreach (FamilyTreeNode treeData in Game.PlayerStats.FamilyTreeArray)
                {
                    LineageObj lineageObj = new LineageObj(this, true);
                    lineageObj.IsDead = true;
                    lineageObj.Age = treeData.Age;
                    lineageObj.ChildAge = treeData.ChildAge;
                    lineageObj.Class = treeData.Class;
                    lineageObj.IsFemale = treeData.IsFemale; // Must go before PlayerName
                    lineageObj.RomanNumeral = treeData.RomanNumeral; // Must go before PlayerName
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
                    xPos += m_xPosOffset;

                    m_masterArray.Add(lineageObj);

                    if (lineageObj.Traits.X == TraitType.Vertigo || lineageObj.Traits.Y == TraitType.Vertigo)
                        lineageObj.FlipPortrait = true;
                }
            }
            else
            {
                // This automatically adds one person to the family tree if it starts empty.
                int xPos = 0;

                LineageObj lineageObj = new LineageObj(this, true);
                lineageObj.IsDead = true;
                lineageObj.Age = 30;
                lineageObj.ChildAge = 5;
                lineageObj.Class = ClassType.Knight;
                lineageObj.PlayerName = "Johannes";
                lineageObj.SetPortrait(1, 1, 1);
                lineageObj.NumEnemiesKilled = 50;
                lineageObj.BeatenABoss = false;
                lineageObj.UpdateAge(currentEra);
                lineageObj.UpdateData();
                lineageObj.UpdateClassRank();
                currentEra = currentEra + lineageObj.Age;

                lineageObj.X = xPos;
                xPos += m_xPosOffset;

                m_masterArray.Add(lineageObj);

                if (lineageObj.Traits.X == TraitType.Vertigo || lineageObj.Traits.Y == TraitType.Vertigo)
                    lineageObj.FlipPortrait = true;
            }
        }

        public void LoadCurrentBranches()
        {
            if (Game.PlayerStats.CurrentBranches == null || Game.PlayerStats.CurrentBranches.Count < 1)
            {
                AddLineageRow(3, m_masterArray[m_masterArray.Count - 1].Position, false, true);

                // Save the current branches if a new set is saved.
                List<PlayerLineageData> branchArray = new List<PlayerLineageData>();
                for (int i = 0; i < m_currentBranchArray.Count; i++)
                {
                    PlayerLineageData newBranch = new PlayerLineageData();
                    newBranch.IsFemale = m_currentBranchArray[i].IsFemale; // Must go before name
                    newBranch.RomanNumeral = m_currentBranchArray[i].RomanNumeral; // Must go before name
                    newBranch.Name = m_currentBranchArray[i].PlayerName;
                    newBranch.HeadPiece = m_currentBranchArray[i].HeadPiece;
                    newBranch.ShoulderPiece = m_currentBranchArray[i].ShoulderPiece;
                    newBranch.ChestPiece = m_currentBranchArray[i].ChestPiece;
                    newBranch.Class = m_currentBranchArray[i].Class;
                    newBranch.Spell = m_currentBranchArray[i].Spell;
                    newBranch.Traits= m_currentBranchArray[i].Traits;
                    newBranch.Age = m_currentBranchArray[i].Age;
                    newBranch.ChildAge = m_currentBranchArray[i].ChildAge;
                    newBranch.IsFemale = m_currentBranchArray[i].IsFemale;
                    branchArray.Add(newBranch);
                }
                Game.PlayerStats.CurrentBranches = branchArray;
                (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.Lineage);
            }
            else
            {
                AddLineageRow(3, m_masterArray[m_masterArray.Count - 1].Position, true, true);
                List<PlayerLineageData> loadedLineageArray = Game.PlayerStats.CurrentBranches;
                for (int i = 0; i < m_currentBranchArray.Count; i++)
                {
                    m_currentBranchArray[i].IsFemale = loadedLineageArray[i].IsFemale; // Must go before name
                    m_currentBranchArray[i].RomanNumeral = loadedLineageArray[i].RomanNumeral; // Must go before name
                    m_currentBranchArray[i].PlayerName = loadedLineageArray[i].Name;
                    m_currentBranchArray[i].SetPortrait(loadedLineageArray[i].HeadPiece, loadedLineageArray[i].ShoulderPiece, loadedLineageArray[i].ChestPiece);
                    m_currentBranchArray[i].Spell = loadedLineageArray[i].Spell;
                    m_currentBranchArray[i].Class = loadedLineageArray[i].Class;
                    m_currentBranchArray[i].ClearTraits(); // Necessary to reset the lineage objs trait index pointer.
                    m_currentBranchArray[i].Traits = loadedLineageArray[i].Traits;
                    m_currentBranchArray[i].Age = loadedLineageArray[i].Age;
                    m_currentBranchArray[i].ChildAge = loadedLineageArray[i].ChildAge;
                    m_currentBranchArray[i].UpdateData();
                }
            }
        }

        public override void OnExit()
        {
            float delayAmount = 1 / 60f;
            float volume = m_storedMusicVol;
            float volumeAmount = m_storedMusicVol / 120f;
            for (int i = 0; i < 120; i++)
            {
                Tweener.Tween.RunFunction(delayAmount * i, this, "ReduceMusic", volume);
                volume -= volumeAmount;
            }
            Tweener.Tween.RunFunction(2, this, "StopLegacySong");

            Game.PlayerStats.CurrentBranches = null;

            //SaveFamilyTree(false);
            // 2 pieces of info should be passed into Game.PlayerStats before calling save:
            // 1. The current selected traits for the player. (Happens in StartGame())
            // 2. The family tree. (Happens just above these comments)
            // Save the current lineage stats.
            //(ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.Lineage);

            if (Game.PlayerStats.Class == ClassType.Dragon)
                GameUtil.UnlockAchievement("FEAR_OF_GRAVITY");

            if (Game.PlayerStats.Traits == Vector2.Zero)
                GameUtil.UnlockAchievement("FEAR_OF_IMPERFECTIONS");

            base.OnExit();
        }

        public void ReduceMusic(float newVolume)
        {
            //Console.WriteLine(newVolume + " " + (m_storedMusicVol - newVolume));
            if (SoundManager.AudioEngine != null)
            {
                SoundManager.AudioEngine.GetCategory("Legacy").SetVolume(newVolume);
                SoundManager.GlobalMusicVolume += m_storedMusicVol - newVolume;
                if (SoundManager.GlobalMusicVolume > m_storedMusicVol)
                    SoundManager.GlobalMusicVolume = m_storedMusicVol;
            }
        }

        public void StopLegacySong()
        {
            if (Game.LineageSongCue != null && Game.LineageSongCue.IsPlaying == true)
                Game.LineageSongCue.Stop(AudioStopOptions.Immediate);
            if (Game.LineageSongCue != null)
            {
                Game.LineageSongCue.Dispose();
                Game.LineageSongCue = null;
            }

            SoundManager.GlobalMusicVolume = m_storedMusicVol;
        }

        public override void Update(GameTime gameTime)
        {
            if (m_background != null && m_background.isContentLost == true)
                ReinitializeRTs();

            m_bgShadow.Opacity = 0.8f + (0.05f * ((float)Math.Sin(Game.TotalGameTime * 4)));

            if (Game.LineageSongCue != null && Game.LineageSongCue.IsPlaying == false)
            {
                Game.LineageSongCue.Dispose();
                Game.LineageSongCue = SoundManager.GetMusicCue("LegacySong");
                Game.LineageSongCue.Play();
                SoundManager.StopMusic(0);
                SoundManager.PlayMusic("SkillTreeSong", true, 1);
            }

            base.Update(gameTime);
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                if (m_selectTween == null || (m_selectTween != null && m_selectTween.Active == false))
                {
                    LineageObj previousLineageObj = m_selectedLineageObj;
                    int previousLineageIndex = m_selectedLineageIndex;

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_MAP) && SkillSystem.GetSkill(SkillType.Randomize_Children).ModifierAmount > 0 && Game.PlayerStats.RerolledChildren == false)
                    {
                        m_lockControls = true;
                        SoundManager.PlaySound("frame_woosh_01", "frame_woosh_02");

                        if (m_xShift != 0)
                        {
                            m_xShift = 0;
                            Tweener.Tween.By(m_descriptionPlate, 0.2f, Tweener.Ease.Back.EaseOut, "delay", "0.2", "X", "-600");
                            m_selectTween = Tweener.Tween.To(Camera, 0.3f, Tweener.Ease.Quad.EaseOut, "delay", "0.2", "X", (m_masterArray.Count * m_xPosOffset).ToString());
                        }

                        (ScreenManager as RCScreenManager).StartWipeTransition();
                        Tweener.Tween.RunFunction(0.2f, this, "RerollCurrentBranch");
                    }

                    //if (Game.GlobalInput.JustPressed(InputMapType.MENU_MAP))
                    //{
                    //    //SaveFamilyTree(true);
                    //    AddLineageRow(3, m_selectedLineageObj.Position, false, true);
                    //    m_selectedLineageIndex = 1;
                    //    UpdateDescriptionPlate();

                    //    FamilyTreeNode newNode = new FamilyTreeNode()
                    //    {
                    //        Name = m_selectedLineageObj.PlayerName,
                    //        Age = (byte)CDGMath.RandomInt(18, 30),
                    //        ChildAge = (byte)CDGMath.RandomInt(2, 5),
                    //        Class = m_selectedLineageObj.Class,
                    //        HeadPiece = m_selectedLineageObj.HeadPiece,
                    //        ChestPiece = m_selectedLineageObj.ChestPiece,
                    //        ShoulderPiece = m_selectedLineageObj.ShoulderPiece,
                    //        NumEnemiesBeaten = CDGMath.RandomInt(0, 50),
                    //        BeatenABoss = false,
                    //        Traits = m_selectedLineageObj.Traits,
                    //    };
                    //    Game.PlayerStats.FamilyTreeArray.Add(newNode);
                    //    LoadFamilyTreeData();
                    //}

                    if (Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_LEFT2))
                    {
                        if (Camera.X > m_masterArray[0].X + 10) // Make sure the camera doesn't go too far to the left.
                        {
                            SoundManager.PlaySound("frame_swoosh_01");
                            m_selectTween = Tweener.Tween.By(Camera, 0.3f, Tweener.Ease.Quad.EaseOut, "X", (-m_xPosOffset).ToString());
                            if (m_xShift == 0)
                                Tweener.Tween.By(m_descriptionPlate, 0.2f, Tweener.Ease.Back.EaseIn, "X", "600");
                            m_xShift--;
                        }
                    }
                    else if (Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.Pressed(InputMapType.PLAYER_RIGHT2))
                    {
                        if (m_xShift < 0)
                        {
                            SoundManager.PlaySound("frame_swoosh_01");
                            m_selectTween = Tweener.Tween.By(Camera, 0.3f, Tweener.Ease.Quad.EaseOut, "X", (m_xPosOffset).ToString());
                            m_xShift++;
                            if (m_xShift == 0)
                                Tweener.Tween.By(m_descriptionPlate, 0.2f, Tweener.Ease.Back.EaseOut, "X", "-600");
                        }
                    }

                    if (m_xShift == 0)
                    {
                        if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                        {
                            if (m_selectedLineageIndex > 0)
                                SoundManager.PlaySound("frame_swap");

                            m_selectedLineageIndex--;

                            if (m_selectedLineageIndex < 0)
                                m_selectedLineageIndex = 0;

                            if (m_selectedLineageIndex != previousLineageIndex)
                            {
                                UpdateDescriptionPlate();

                                m_selectTween = Tweener.Tween.By(m_currentBranchArray[0], 0.3f, Tweener.Ease.Quad.EaseOut, "Y", "450");
                                Tweener.Tween.By(m_currentBranchArray[1], 0.3f, Tweener.Ease.Quad.EaseOut, "Y", "450");
                                Tweener.Tween.By(m_currentBranchArray[2], 0.3f, Tweener.Ease.Quad.EaseOut, "Y", "450");
                            }
                        }
                        else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
                        {
                            if (m_selectedLineageIndex < m_currentBranchArray.Count - 1)
                                SoundManager.PlaySound("frame_swap");

                            m_selectedLineageIndex++;

                            if (m_selectedLineageIndex > m_currentBranchArray.Count - 1)
                                m_selectedLineageIndex = m_currentBranchArray.Count - 1;

                            if (m_selectedLineageIndex != previousLineageIndex)
                            {
                                UpdateDescriptionPlate();

                                m_selectTween = Tweener.Tween.By(m_currentBranchArray[0], 0.3f, Tweener.Ease.Quad.EaseOut, "Y", "-450");
                                Tweener.Tween.By(m_currentBranchArray[1], 0.3f, Tweener.Ease.Quad.EaseOut, "Y", "-450");
                                Tweener.Tween.By(m_currentBranchArray[2], 0.3f, Tweener.Ease.Quad.EaseOut, "Y", "-450");
                            }
                        }
                    }

                    m_selectedLineageObj = m_currentBranchArray[m_selectedLineageIndex];

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                    {
                        if (m_xShift == 0)
                        {
                            if (previousLineageObj == m_selectedLineageObj)
                            {
                                RCScreenManager manager = ScreenManager as RCScreenManager;

                                m_storedFemalePlayerStat = Game.PlayerStats.IsFemale;
                                Game.PlayerStats.IsFemale = m_selectedLineageObj.IsFemale;
                                manager.DialogueScreen.SetDialogue("LineageChoiceWarning");

                                manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                                manager.DialogueScreen.SetConfirmEndHandler(this, "StartGame");
                                //manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                                manager.DialogueScreen.SetCancelEndHandler(this, "CancelSelection");
                                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true);
                            }
                        }
                        else
                        {
                            m_xShift = 0;
                            SoundManager.PlaySound("frame_woosh_01", "frame_woosh_02");
                            Tweener.Tween.By(m_descriptionPlate, 0.2f, Tweener.Ease.Back.EaseOut, "X", "-600");
                            m_selectTween = Tweener.Tween.To(Camera, 0.3f, Tweener.Ease.Quad.EaseOut, "X", (m_masterArray.Count * m_xPosOffset).ToString());
                        }
                    }
                    base.HandleInput();
                }

                if (LevelEV.ENABLE_DEBUG_INPUT == true)
                    HandleDebugInput();
            }
        }

        private bool m_storedFemalePlayerStat;
        public void CancelSelection()
        {
            Console.WriteLine("Canceling selection.");
            Game.PlayerStats.IsFemale = m_storedFemalePlayerStat;
        }

        private int m_debugNameArrayIndex = 0;
        private void HandleDebugInput()
        {
            // Debug for easier localization testing.
            Vector2 traits = m_currentBranchArray[m_selectedLineageIndex].Traits;
            Vector2 previousTraits = traits;
            bool lineageObjChanged = false;

            if (InputManager.JustPressed(Keys.OemQuotes, null) ||InputManager.JustPressed(Keys.OemSemicolon, null))
            {
                (ScreenManager.Game as RogueCastle.Game).InitializeFemaleNameArray(true);
                (ScreenManager.Game as RogueCastle.Game).InitializeMaleNameArray(true);
            }

            List<string> nameArray = Game.NameArray;
            if (m_currentBranchArray[m_selectedLineageIndex].IsFemale == true)
                nameArray = Game.FemaleNameArray;
            int previousNameIndex = m_debugNameArrayIndex;

            if (InputManager.JustPressed(Keys.B, PlayerIndex.One))
                m_debugNameArrayIndex--;
            else if (InputManager.JustPressed(Keys.N, PlayerIndex.One))
                m_debugNameArrayIndex++;

            if (m_debugNameArrayIndex < 0)
                m_debugNameArrayIndex = nameArray.Count - 1;
            else if (m_debugNameArrayIndex >= nameArray.Count)
                m_debugNameArrayIndex = 0;

            if (previousNameIndex != m_debugNameArrayIndex)
            {
                lineageObjChanged = true;
                m_currentBranchArray[m_selectedLineageIndex].PlayerName = nameArray[m_debugNameArrayIndex];
            }

            if (InputManager.JustPressed(Keys.M, PlayerIndex.One))
                traits.X--;
            else if (InputManager.JustPressed(Keys.OemComma, PlayerIndex.One))
                traits.X++;

            if (InputManager.JustPressed(Keys.OemPeriod, PlayerIndex.One))
                traits.Y--;
            else if (InputManager.JustPressed(Keys.OemQuestion, PlayerIndex.One))
                traits.Y++;

            if (traits.X >= TraitType.Total)
                traits.X = TraitType.None;
            else if (traits.X < TraitType.None)
                traits.X = TraitType.Total - 1;
            if (traits.Y >= TraitType.Total)
                traits.Y = TraitType.None;
            else if (traits.Y < TraitType.None)
                traits.Y = TraitType.Total - 1;

            if (traits != previousTraits)
            {
                m_currentBranchArray[m_selectedLineageIndex].Traits = traits;
                lineageObjChanged = true;
            }

            byte spell = m_currentBranchArray[m_selectedLineageIndex].Spell;
            byte previousSpell = spell;
            if (InputManager.JustPressed(Keys.OemOpenBrackets, PlayerIndex.One))
            {
                if (spell == SpellType.None)
                    spell = SpellType.Total - 1;
                else
                    spell--;
            }
            else if (InputManager.JustPressed(Keys.OemCloseBrackets, PlayerIndex.One))
                spell++;

            if (spell < SpellType.None)
                spell = SpellType.Total - 1;
            else if (spell >= SpellType.Total)
                spell = SpellType.None;

            if (spell != previousSpell)
            {
                m_currentBranchArray[m_selectedLineageIndex].Spell = spell;
                lineageObjChanged = true;
            }

            byte currentClass = m_currentBranchArray[m_selectedLineageIndex].Class;
            byte previousClass = currentClass;
            if (InputManager.JustPressed(Keys.OemMinus, PlayerIndex.One))
            {
                if (currentClass == ClassType.Knight)
                    currentClass = ClassType.Traitor;
                else
                    currentClass--;
            }
            else if (InputManager.JustPressed(Keys.OemPlus, PlayerIndex.One))
                currentClass++;

            if (currentClass < ClassType.Knight)
                currentClass = ClassType.Traitor;
            else if (currentClass > ClassType.Traitor)
                currentClass = ClassType.Knight;

            if (currentClass != previousClass)
            {
                m_currentBranchArray[m_selectedLineageIndex].Class = currentClass;
                lineageObjChanged = true;
            }

            if (lineageObjChanged == true)
            {
                m_currentBranchArray[m_selectedLineageIndex].UpdateData();
                UpdateDescriptionPlate();
            }
        }

        public void RerollCurrentBranch()
        {
            m_rerollText.Visible = false;
            Game.PlayerStats.RerolledChildren = true;
            (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData); // Hopefully saving player data here doesn't screw things up.

            Game.PlayerStats.CurrentBranches.Clear();
            LoadCurrentBranches(); // Clears the current branches, creates 3 new ones, and saves them.
            (ScreenManager as RCScreenManager).EndWipeTransition();
            UpdateDescriptionPlate();
            m_lockControls = false;
        }

        public void StartGame()
        {
            //m_selectedLineageObj.TraitArray[0] = new PlayerTraitData() {TraitType = TraitType.Knockback_Immune };
            //Game.PlayerStats.CurrentLineageData = m_selectedLineageObj.Data.Clone();
            Game.PlayerStats.HeadPiece = m_selectedLineageObj.HeadPiece;
            Game.PlayerStats.ShoulderPiece = m_selectedLineageObj.ShoulderPiece;
            Game.PlayerStats.ChestPiece = m_selectedLineageObj.ChestPiece;
            Game.PlayerStats.IsFemale = m_selectedLineageObj.IsFemale;
            Game.PlayerStats.Class = m_selectedLineageObj.Class;
            Game.PlayerStats.Traits = m_selectedLineageObj.Traits;
            Game.PlayerStats.Spell = m_selectedLineageObj.Spell;
            Game.PlayerStats.PlayerName = m_selectedLineageObj.PlayerName;
            Game.PlayerStats.Age = m_selectedLineageObj.Age;
            Game.PlayerStats.ChildAge = m_selectedLineageObj.ChildAge;
            Game.PlayerStats.RomanNumeral = m_selectedLineageObj.RomanNumeral;
            Game.PlayerStats.HasProsopagnosia = false;

            // Save the next 3 spells for regular wizard in case you level up with wizard class in the next run.
            if (Game.PlayerStats.Class == ClassType.Wizard || Game.PlayerStats.Class == ClassType.Wizard2)
            {
                //byte[] spellArray = ClassType.GetSpellList(ClassType.Wizard2);
                //List<byte> spellList = new List<byte>();
                //foreach (byte spell in spellArray)
                //    spellList.Add(spell);
                //int spellIndex = spellList.IndexOf(Game.PlayerStats.Spell);
                //spellList.Clear();
                //spellList = null;

                //byte[] wizardSpells = new byte[3];
                //for (int i = 0; i < 3; i++)
                //{
                //    wizardSpells[i] = spellArray[spellIndex];
                //    spellIndex++;
                //    if (spellIndex >= spellArray.Length)
                //        spellIndex = 0;
                //}

                //Game.PlayerStats.WizardSpellList = new Vector3(wizardSpells[0], wizardSpells[1], wizardSpells[2]);
                Game.PlayerStats.WizardSpellList = SpellType.GetNext3Spells();
            }

            // Clear out the saved Branches in PlayerStats.CurrentBranches
            Game.PlayerStats.CurrentBranches.Clear();

            // This loads the starting room, then puts the skill screen on top of it.  How does this work?
            // The call is done in LoadingScreen.  First it adds the level, disables anything that could cause problems in the level, then adds the skill tree.
            (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.StartingRoom, true);
        }


        public override void Draw(GameTime gameTime)
        {
            // Hack to make an infinite scroll.
            if (Camera.X > m_background.X + 1320 * 5)
                m_background.X = Camera.X;
            if (Camera.X < m_background.X)
                m_background.X = Camera.X - 1320;

            //float parallaxAmount = 0;// 0.25f;
            //m_background.X = Camera.Bounds.Center.X * parallaxAmount;
            //m_background.Y = Camera.Bounds.Center.Y * parallaxAmount;
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, Camera.GetTransformation());
            m_background.Draw(Camera);
            Camera.End();

            //Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
            //m_bgShadow.Draw(Camera);
            //Camera.End();

            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetTransformation());

            foreach (LineageObj lineageObj in m_masterArray)
                lineageObj.Draw(Camera);

            foreach (LineageObj lineageObj in m_currentBranchArray)
                lineageObj.Draw(Camera);

            Camera.End();

            if (Camera.Zoom >= 1)
            {
                Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
                m_bgShadow.Draw(Camera);

                m_titleText.Draw(Camera);
                m_confirmText.Draw(Camera);
                m_navigationText.Draw(Camera);
                m_rerollText.Draw(Camera);
                //m_controlsUpDown.Draw(Camera);
                //m_controlsLeftRight.Draw(Camera);

                //m_infoPlate.Draw(Camera);
                m_descriptionPlate.Draw(Camera);
                //Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
                Camera.End();
            }

            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Lineage Screen");

                m_titleText.Dispose();
                m_titleText = null;

                m_selectedLineageObj = null;

                foreach (LineageObj obj in m_currentBranchArray)
                    obj.Dispose();

                m_currentBranchArray.Clear();
                m_currentBranchArray = null;

                foreach (LineageObj obj in m_masterArray)
                {
                    if (obj.IsDisposed == false)
                        obj.Dispose();
                }

                m_masterArray.Clear();
                m_masterArray = null;

                if (m_startingLineageObj != null)
                    m_startingLineageObj.Dispose();
                m_startingLineageObj = null;

                m_background.Dispose();
                m_background = null;
                m_bgShadow.Dispose();
                m_bgShadow = null;
                m_selectTween = null;
                m_descriptionPlate.Dispose();
                m_descriptionPlate = null;

                m_confirmText.Dispose();
                m_confirmText = null;
                m_navigationText.Dispose();
                m_navigationText = null;
                m_rerollText.Dispose();
                m_rerollText = null;
                //if (Game.LineageSongCue != null && Game.LineageSongCue.IsPlaying == true)
                //    Game.LineageSongCue.Stop(AudioStopOptions.Immediate);
                //if (Game.LineageSongCue != null)
                //    Game.LineageSongCue.Dispose();
                //Game.LineageSongCue = null;
                base.Dispose();
            }
        }

        // Number of times a name is used.
        public int NameCopies(string name)
        {
            int nameCopies = 0;
            foreach (LineageObj data in m_masterArray)
            {
                if (data.PlayerName.Contains(name))
                    nameCopies++;
            }

            return nameCopies;
        }

        public bool CurrentBranchNameCopyFound(string name)
        {
            foreach (LineageObj data in m_currentBranchArray)
            {
                if (Game.PlayerStats.RevisionNumber <= 0)
                {
                    if (data.PlayerName.Contains(name))
                        return true;
                }
                else
                {
                    if (data.PlayerName == name) // This should be okay to do now because of the name format change.
                        return true;
                }
            }
            return false;
        }

        public override void RefreshTextObjs()
        {
            foreach (LineageObj data in m_masterArray)
                data.RefreshTextObjs();
            foreach (LineageObj data in m_currentBranchArray)
                data.RefreshTextObjs();

            Game.ChangeBitmapLanguage(m_titleText, "LineageTitleText_Sprite");

            UpdateDescriptionPlate();

            /*
            m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_1");
            
            if (InputManager.GamePadIsConnected(PlayerIndex.One) == true)
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_2");
            else
                m_navigationText.Text = LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_3");

            m_rerollText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_LINEAGE_SCREEN_4");
             */

            base.RefreshTextObjs();
        }
    }
}

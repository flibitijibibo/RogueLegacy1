using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class GetItemScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        /// Level Up Animation Variables ///
        private SpriteObj m_levelUpBGImage;
        private SpriteObj[] m_levelUpParticles;

        private byte m_itemType = GetItemType.None;
        private Vector2 m_itemInfo;
        private SpriteObj m_itemSprite;
        private SpriteObj m_itemFoundSprite;
        private TextObj m_itemFoundText;
        private Vector2 m_itemStartPos;
        private Vector2 m_itemEndPos;
        private bool m_itemSpinning = false;

        private KeyIconTextObj m_continueText;

        private bool m_lockControls = false;

        private Cue m_buildUpSound;
        private string m_songName;
        private float m_storedMusicVolume;

        // Only for trip stat drops.
        private SpriteObj m_tripStat1, m_tripStat2;
        private TextObj m_tripStat1FoundText, m_tripStat2FoundText; 
        private Vector2 m_tripStatData;

        public GetItemScreen()
        {
            this.DrawIfCovered = true;
            BackBufferOpacity = 0;
            m_itemEndPos = new Vector2(1320 / 2f, 720 / 2f + 50);
        }

        public override void LoadContent()
        {
            ////////// Loading Level up animation effects.
            m_levelUpBGImage = new SpriteObj("BlueprintFoundBG_Sprite");
            m_levelUpBGImage.ForceDraw = true;
            m_levelUpBGImage.Visible = false;

            m_levelUpParticles = new SpriteObj[10];
            for (int i = 0; i < m_levelUpParticles.Length; i++)
            {
                m_levelUpParticles[i] = new SpriteObj("LevelUpParticleFX_Sprite");
                m_levelUpParticles[i].AnimationDelay = 1 / 24f;
                m_levelUpParticles[i].ForceDraw = true;
                m_levelUpParticles[i].Visible = false;
            }

            m_itemSprite = new SpriteObj("BlueprintIcon_Sprite");
            m_itemSprite.ForceDraw = true;
            m_itemSprite.OutlineWidth = 2;

            m_tripStat1 = m_itemSprite.Clone() as SpriteObj;
            m_tripStat2 = m_itemSprite.Clone() as SpriteObj;

            m_itemFoundText = new TextObj(Game.JunicodeFont);
            m_itemFoundText.FontSize = 18;
            m_itemFoundText.Align = Types.TextAlign.Centre;
            m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_itemFoundText); // dummy locID to add TextObj to language refresh list
            m_itemFoundText.Position = m_itemEndPos;
            m_itemFoundText.Y += 70;
            m_itemFoundText.ForceDraw = true;
            m_itemFoundText.OutlineWidth = 2;

            m_tripStat1FoundText = m_itemFoundText.Clone() as TextObj;
            m_tripStat1FoundText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_tripStat1FoundText); // dummy locID to add TextObj to language refresh list
            m_tripStat2FoundText = m_itemFoundText.Clone() as TextObj;
            m_tripStat2FoundText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_tripStat2FoundText); // dummy locID to add TextObj to language refresh list

            m_itemFoundSprite = new SpriteObj("BlueprintFoundText_Sprite");
            m_itemFoundSprite.ForceDraw = true;
            m_itemFoundSprite.Visible = false;

            m_continueText = new KeyIconTextObj(Game.JunicodeFont);
            m_continueText.FontSize = 14;
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_continueText); // dummy locID to add TextObj to language refresh list
            //m_continueText.Align = Types.TextAlign.Centre;
            //m_continueText.Position = new Vector2(1320 - m_continueText.Width, 720 - m_continueText.Height - 10);
            m_continueText.Align = Types.TextAlign.Right;
            m_continueText.Position = new Vector2(1320 - 20, 720 - m_continueText.Height - 10);
            m_continueText.ForceDraw = true;

            base.LoadContent();
        }

        public override void PassInData(List<object> objList)
        {
            m_itemStartPos = (Vector2)objList[0];
            m_itemType = (byte)objList[1];
            m_itemInfo = (Vector2)objList[2];
            if (m_itemType == GetItemType.TripStatDrop)
                m_tripStatData = (Vector2)objList[3];
   
            base.PassInData(objList);
        }

        public override void OnEnter()
        {
            m_tripStat1.Visible = false;
            m_tripStat2.Visible = false;
            m_tripStat1.Scale = Vector2.One;
            m_tripStat2.Scale = Vector2.One;

            // Do not auto save the game for fountain pieces. Otherwise the 'neo' player stats and equipment will be saved as well.
            if (m_itemType != GetItemType.FountainPiece)
                (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.UpgradeData);
            m_itemSprite.Rotation = 0;
            m_itemSprite.Scale = Vector2.One;
            m_itemStartPos.X -= Camera.TopLeftCorner.X;
            m_itemStartPos.Y -= Camera.TopLeftCorner.Y;

            m_storedMusicVolume = SoundManager.GlobalMusicVolume;
            m_songName = SoundManager.GetCurrentMusicName();
            m_lockControls = true;
            m_continueText.Opacity = 0;

            m_continueText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_1_NEW", m_continueText);

            m_itemFoundText.Position = m_itemEndPos;
            m_itemFoundText.Y += 70;

            m_itemFoundText.Scale = Vector2.Zero;

            m_tripStat1FoundText.Position = m_itemFoundText.Position;
            m_tripStat2FoundText.Position = m_itemFoundText.Position;
            m_tripStat1FoundText.Scale = Vector2.Zero;
            m_tripStat2FoundText.Scale = Vector2.Zero;
            m_tripStat1FoundText.Visible = false;
            m_tripStat2FoundText.Visible = false;

            switch (m_itemType)
            {
                case (GetItemType.Blueprint):
                    m_itemSpinning = true;
                    m_itemSprite.ChangeSprite("BlueprintIcon_Sprite");
                    m_itemFoundSprite.ChangeSprite("BlueprintFoundText_Sprite");
                    m_itemFoundText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentBaseType.ToStringID((int)m_itemInfo.Y), true), LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID((int)m_itemInfo.X), true));

                    break;
                case (GetItemType.Rune):
                    m_itemSpinning = true;
                    m_itemSprite.ChangeSprite("RuneIcon_Sprite");
                    m_itemFoundSprite.ChangeSprite("RuneFoundText_Sprite");
                    //m_itemFoundText.Text = LocaleBuilder.getResourceString(EquipmentAbilityType.ToStringID((int)m_itemInfo.Y)) + " " + LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_13") /* Rune */ + " (" + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID2((int)m_itemInfo.X)) + ")";
                    m_itemFoundText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_RUNE_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentAbilityType.ToStringID((int)m_itemInfo.Y), true), LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_13"), true) /*"Rune"*/ + "\n(" + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID2((int)m_itemInfo.X), true) + ")";
                    m_itemSprite.AnimationDelay = 1 / 20f;

                    GameUtil.UnlockAchievement("LOVE_OF_MAGIC");
                    break;
                case (GetItemType.StatDrop):
                case (GetItemType.TripStatDrop):

                    m_itemSprite.ChangeSprite(this.GetStatSpriteName((int)m_itemInfo.X));
                    m_itemFoundText.Text = this.GetStatText((int)m_itemInfo.X);
                    m_itemSprite.AnimationDelay = 1 / 20f;
                    m_itemFoundSprite.ChangeSprite("StatFoundText_Sprite");

                    if (m_itemType == GetItemType.TripStatDrop)
                    {
                        m_tripStat1FoundText.Visible = true;
                        m_tripStat2FoundText.Visible = true;

                        m_tripStat1.ChangeSprite(this.GetStatSpriteName((int)m_tripStatData.X));
                        m_tripStat2.ChangeSprite(this.GetStatSpriteName((int)m_tripStatData.Y));
                        m_tripStat1.Visible = true;
                        m_tripStat2.Visible = true;
                        m_tripStat1.AnimationDelay = 1 / 20f;
                        m_tripStat2.AnimationDelay = 1 / 20f;
                        Tween.RunFunction(0.1f, m_tripStat1, "PlayAnimation", true); // Adds a delay to the trip stat animations (so that they don't animate all at the same time).
                        Tween.RunFunction(0.2f, m_tripStat2, "PlayAnimation", true); // Adds a delay to the trip stat animations (so that they don't animate all at the same time).
                        m_tripStat1FoundText.Text = this.GetStatText((int)m_tripStatData.X);
                        m_tripStat2FoundText.Text = this.GetStatText((int)m_tripStatData.Y);

                        m_itemFoundText.Y += 50;
                        m_tripStat1FoundText.Y = m_itemFoundText.Y + 50;
                    }

                    break;
                case (GetItemType.SpecialItem):
                    m_itemSprite.ChangeSprite(SpecialItemType.SpriteName((byte)m_itemInfo.X));
                    m_itemFoundSprite.ChangeSprite("ItemFoundText_Sprite");
                    m_itemFoundText.Text = LocaleBuilder.getString(SpecialItemType.ToStringID((byte)m_itemInfo.X), m_itemFoundText);
                    break;
                case (GetItemType.Spell):
                    m_itemSprite.ChangeSprite(SpellType.Icon((byte)m_itemInfo.X));
                    m_itemFoundSprite.ChangeSprite("SpellFoundText_Sprite");
                    m_itemFoundText.Text = LocaleBuilder.getString(SpellType.ToStringID((byte)m_itemInfo.X), m_itemFoundText);
                    break;
                case (GetItemType.FountainPiece):
                    m_itemSprite.ChangeSprite(GetMedallionImage((int)m_itemInfo.X)); // This needs to change to reflect the stat stored in m_itemInfo.X
                    m_itemFoundSprite.ChangeSprite("ItemFoundText_Sprite");
                    if (m_itemInfo.X == ItemDropType.FountainPiece5)
                        m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_8", m_itemFoundText);
                    else
                        m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_9", m_itemFoundText);
                    break;
            }

            m_itemSprite.PlayAnimation(true);
            ItemSpinAnimation();

            RefreshTextObjs();

            base.OnEnter();
        }

        private void ItemSpinAnimation()
        {
            m_itemSprite.Scale = Vector2.One;
            m_itemSprite.Position = m_itemStartPos;

            m_buildUpSound = SoundManager.PlaySound("GetItemBuildupStinger");
            Tween.To(typeof(SoundManager), 1, Tween.EaseNone, "GlobalMusicVolume", (m_storedMusicVolume * 0.1f).ToString());
            m_itemSprite.Scale = new Vector2(35f / m_itemSprite.Height, 35f / m_itemSprite.Height);
            Tween.By(m_itemSprite, 0.5f, Back.EaseOut, "Y", "-150");
            Tween.RunFunction(0.7f, this, "ItemSpinAnimation2");

            // Trip stat code.
            m_tripStat1.Scale = Vector2.One;
            m_tripStat2.Scale = Vector2.One;

            m_tripStat1.Position = m_itemStartPos;
            m_tripStat2.Position = m_itemStartPos;
            Tween.By(m_tripStat1, 0.5f, Back.EaseOut, "Y", "-150", "X", "50");
            m_tripStat1.Scale = new Vector2(35f / m_tripStat1.Height, 35f / m_tripStat1.Height);
            Tween.By(m_tripStat2, 0.5f, Back.EaseOut, "Y", "-150", "X", "-50");
            m_tripStat2.Scale = new Vector2(35f / m_tripStat2.Height, 35f / m_tripStat2.Height);
        }

        public void ItemSpinAnimation2()
        {
            Tween.RunFunction(0.2f, typeof(SoundManager), "PlaySound", "GetItemStinger3");
            if (m_buildUpSound != null && m_buildUpSound.IsPlaying)
                m_buildUpSound.Stop(AudioStopOptions.AsAuthored);
            Tween.To(m_itemSprite, 0.2f, Quad.EaseOut, "ScaleX", "0.1", "ScaleY", "0.1");
            Tween.AddEndHandlerToLastTween(this, "ItemSpinAnimation3");

            ////////////// Animations for trip stat.
            Tween.To(m_tripStat1, 0.2f, Quad.EaseOut, "ScaleX", "0.1", "ScaleY", "0.1");
            Tween.To(m_tripStat2, 0.2f, Quad.EaseOut, "ScaleX", "0.1", "ScaleY", "0.1");
        }

        public void ItemSpinAnimation3()
        {
            Vector2 spriteScale = m_itemSprite.Scale;
            m_itemSprite.Scale = Vector2.One;
            float itemScale = 130f / m_itemSprite.Height;
            m_itemSprite.Scale = spriteScale;
            //Tween.To(m_itemSprite, 0.2f, Tween.EaseNone, "ScaleX", "1.2", "ScaleY", "1.2");
            Tween.To(m_itemSprite, 0.2f, Tween.EaseNone, "ScaleX", itemScale.ToString(), "ScaleY", itemScale.ToString());
            Tween.To(m_itemSprite, 0.2f, Tween.EaseNone, "X", (1320 / 2).ToString(), "Y", (720 / 2 + 30).ToString());
            Tween.To(m_itemFoundText, 0.3f, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_continueText, 0.3f, Linear.EaseNone, "Opacity", "1");

            ////////////////// Animations for trip stats.
            spriteScale = m_tripStat1.Scale;
            m_tripStat1.Scale = Vector2.One;
            itemScale = 130f / m_tripStat1.Height;
            m_tripStat1.Scale = spriteScale;
            Tween.To(m_tripStat1, 0.2f, Tween.EaseNone, "ScaleX", itemScale.ToString(), "ScaleY", itemScale.ToString());
            Tween.To(m_tripStat1, 0.2f, Tween.EaseNone, "X", (1320 / 2 + 170).ToString(), "Y", (720 / 2 + 30).ToString());

            spriteScale = m_tripStat2.Scale;
            m_tripStat2.Scale = Vector2.One;
            itemScale = 130f / m_tripStat2.Height;
            m_tripStat2.Scale = spriteScale;
            Tween.To(m_tripStat2, 0.2f, Tween.EaseNone, "ScaleX", itemScale.ToString(), "ScaleY", itemScale.ToString());
            Tween.To(m_tripStat2, 0.2f, Tween.EaseNone, "X", (1320 / 2 - 170).ToString(), "Y", (720 / 2 + 30).ToString());

            Tween.To(m_tripStat1FoundText, 0.3f, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_tripStat2FoundText, 0.3f, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");

            /////////////////////////////////////

            for (int i = 0; i < m_levelUpParticles.Length; i++)
            {
                m_levelUpParticles[i].AnimationDelay = 0;
                m_levelUpParticles[i].Visible = true;
                m_levelUpParticles[i].Scale = new Vector2(0.1f, 0.1f);
                m_levelUpParticles[i].Opacity = 0;
                m_levelUpParticles[i].Position = new Vector2(GlobalEV.ScreenWidth / 2f, GlobalEV.ScreenHeight / 2f);
                m_levelUpParticles[i].Position += new Vector2(CDGMath.RandomInt(-100, 100), CDGMath.RandomInt(-50, 50));
                float randDelay = CDGMath.RandomFloat(0, 0.5f);
                Tween.To(m_levelUpParticles[i], 0.2f, Tweener.Ease.Linear.EaseNone, "delay", randDelay.ToString(), "Opacity", "1");
                Tween.To(m_levelUpParticles[i], 0.5f, Tweener.Ease.Linear.EaseNone, "delay", randDelay.ToString(), "ScaleX", "2", "ScaleY", "2");
                Tween.To(m_levelUpParticles[i], randDelay, Tweener.Ease.Linear.EaseNone);
                Tween.AddEndHandlerToLastTween(m_levelUpParticles[i], "PlayAnimation", false);
            }

            m_itemFoundSprite.Position = new Vector2(GlobalEV.ScreenWidth / 2f, GlobalEV.ScreenHeight / 2f - 170);
            m_itemFoundSprite.Scale = Vector2.Zero;
            m_itemFoundSprite.Visible = true;
            Tween.To(m_itemFoundSprite, 0.5f, Tweener.Ease.Back.EaseOut, "delay", "0.05", "ScaleX", "1", "ScaleY", "1");

            m_levelUpBGImage.Position = m_itemFoundSprite.Position;
            m_levelUpBGImage.Y += 30;
            m_levelUpBGImage.Scale = Vector2.Zero;
            m_levelUpBGImage.Visible = true;
            Tween.To(m_levelUpBGImage, 0.5f, Tweener.Ease.Back.EaseOut, "ScaleX", "1", "ScaleY", "1");

            Tween.To(this, 0.5f, Linear.EaseNone, "BackBufferOpacity", "0.5");
            if (m_itemSpinning == true)
                m_itemSprite.Rotation = -25;
            m_itemSpinning = false;

            Tween.RunFunction(0.5f, this, "UnlockControls");
        }

        public void UnlockControls()
        {
            m_lockControls = false;
        }

        public void ExitScreenTransition()
        {
            if ((int)m_itemInfo.X == ItemDropType.FountainPiece5)
            {
                m_itemInfo = Vector2.Zero;
                List<object> data = new List<object>();
                data.Add(SkillUnlockType.Traitor);
                Game.ScreenManager.DisplayScreen(ScreenType.SkillUnlock, true, data);
            }
            else
            {
                m_lockControls = true;
                Tween.To(typeof(SoundManager), 1, Tween.EaseNone, "GlobalMusicVolume", m_storedMusicVolume.ToString());

                Tween.To(m_itemSprite, 0.4f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                Tween.To(m_itemFoundText, 0.4f, Back.EaseIn, "delay", "0.1", "ScaleX", "0", "ScaleY", "0");
                Tween.To(this, 0.4f, Back.EaseIn, "BackBufferOpacity", "0");
                Tween.To(m_levelUpBGImage, 0.4f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                Tween.To(m_itemFoundSprite, 0.4f, Back.EaseIn, "delay", "0.1", "ScaleX", "0", "ScaleY", "0");
                Tween.To(m_continueText, 0.4f, Linear.EaseNone, "delay", "0.1", "Opacity", "0");
                Tween.AddEndHandlerToLastTween((ScreenManager as RCScreenManager), "HideCurrentScreen");

                Tween.To(m_tripStat1, 0.4f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                Tween.To(m_tripStat2, 0.4f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                Tween.To(m_tripStat1FoundText, 0.4f, Back.EaseIn, "delay", "0.1", "ScaleX", "0", "ScaleY", "0");
                Tween.To(m_tripStat2FoundText, 0.4f, Back.EaseIn, "delay", "0.1", "ScaleX", "0", "ScaleY", "0");
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (m_itemSpinning == true)
                m_itemSprite.Rotation += (1200 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //m_itemSprite.Rotation += 20;
            base.Update(gameTime);
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) 
                    || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                     || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    ExitScreenTransition();

                if (LevelEV.ENABLE_DEBUG_INPUT == true)
                    HandleDebugInput();
            }

            base.HandleInput();
        }

        private void HandleDebugInput()
        {
            Vector2 previousItemInfo = m_itemInfo;
            switch (m_itemType)
            {
                case (GetItemType.Blueprint):
                    if (InputManager.JustPressed(Keys.OemOpenBrackets, null))
                    {
                        m_itemInfo.Y--;
                        if (m_itemInfo.Y < 0)
                        {
                            m_itemInfo.X--;
                            m_itemInfo.Y = EquipmentBaseType.Total - 1;
                            if (m_itemInfo.X < 0)
                                m_itemInfo.X = EquipmentCategoryType.Total - 1;
                        }
                    }
                    else if (InputManager.JustPressed(Keys.OemCloseBrackets, null))
                    {
                        m_itemInfo.Y++;
                        if (m_itemInfo.Y >= EquipmentBaseType.Total)
                        {
                            m_itemInfo.X++;
                            m_itemInfo.Y = 0;
                            if (m_itemInfo.X >= EquipmentCategoryType.Total)
                                m_itemInfo.X = 0;
                        }
                    }
                    if (previousItemInfo != m_itemInfo)
                        m_itemFoundText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentBaseType.ToStringID((int)m_itemInfo.Y), true), LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID((int)m_itemInfo.X), true));
                    break;
                case (GetItemType.Rune):
                    if (InputManager.JustPressed(Keys.OemOpenBrackets, null))
                    {
                        m_itemInfo.Y--;
                        if (m_itemInfo.Y < 0)
                        {
                            m_itemInfo.X--;
                            m_itemInfo.Y = EquipmentAbilityType.Total - 1;
                            if (m_itemInfo.X < 0)
                                m_itemInfo.X = EquipmentCategoryType.Total - 1;
                        }
                    }
                    else if (InputManager.JustPressed(Keys.OemCloseBrackets, null))
                    {
                        m_itemInfo.Y++;
                        if (m_itemInfo.Y >= EquipmentAbilityType.Total)
                        {
                            m_itemInfo.X++;
                            m_itemInfo.Y = 0;
                            if (m_itemInfo.X >= EquipmentCategoryType.Total)
                                m_itemInfo.X = 0;
                        }
                    }
                    if (previousItemInfo != m_itemInfo)
                        m_itemFoundText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_RUNE_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentAbilityType.ToStringID((int)m_itemInfo.Y), true), LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_13"), true) /*"Rune"*/ + " (" + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID2((int)m_itemInfo.X), true) + ")";
                    break;
                case (GetItemType.StatDrop):
                    if (InputManager.JustPressed(Keys.OemOpenBrackets, null))
                    {
                        m_itemInfo.X--;
                        if (m_itemInfo.X < ItemDropType.Stat_Strength)
                            m_itemInfo.X = ItemDropType.Stat_Weight;
                    }
                    else if (InputManager.JustPressed(Keys.OemCloseBrackets, null))
                    {
                        m_itemInfo.X++;
                        if (m_itemInfo.X > ItemDropType.Stat_Weight)
                            m_itemInfo.X = ItemDropType.Stat_Strength;
                    }

                    if (previousItemInfo != m_itemInfo)
                        m_itemFoundText.Text = this.GetStatText((int)m_itemInfo.X);
                    break;
                case (GetItemType.SpecialItem):
                    if (InputManager.JustPressed(Keys.OemOpenBrackets, null))
                    {
                        m_itemInfo.X--;
                        if (m_itemInfo.X == SpecialItemType.Total)
                            m_itemInfo.X--;
                        if (m_itemInfo.X < SpecialItemType.FreeEntrance)
                            m_itemInfo.X = SpecialItemType.LastBossToken;
                    }
                    else if (InputManager.JustPressed(Keys.OemCloseBrackets, null))
                    {
                        m_itemInfo.X++;
                        if (m_itemInfo.X == SpecialItemType.Total)
                            m_itemInfo.X++;
                        if (m_itemInfo.X > SpecialItemType.LastBossToken)
                            m_itemInfo.X = SpecialItemType.FreeEntrance;
                    }

                    if (previousItemInfo != m_itemInfo)
                        m_itemFoundText.Text = LocaleBuilder.getString(SpecialItemType.ToStringID((byte)m_itemInfo.X), m_itemFoundText);
                    break;
                case (GetItemType.Spell):
                    if (InputManager.JustPressed(Keys.OemOpenBrackets, null))
                    {
                        m_itemInfo.X--;
                        if (m_itemInfo.X < SpellType.Dagger)
                            m_itemInfo.X = SpellType.Total - 1;
                    }
                    else if (InputManager.JustPressed(Keys.OemCloseBrackets, null))
                    {
                        m_itemInfo.X++;
                        if (m_itemInfo.X > SpellType.Total - 1)
                            m_itemInfo.X = SpellType.Dagger;
                    }

                    if (previousItemInfo != m_itemInfo)
                        m_itemFoundText.Text = LocaleBuilder.getString(SpellType.ToStringID((byte)m_itemInfo.X), m_itemFoundText);

                    break;
                case (GetItemType.FountainPiece):
                    if (InputManager.JustPressed(Keys.OemOpenBrackets, null) || (InputManager.JustPressed(Keys.OemCloseBrackets, null)))
                    {
                        if (m_itemInfo.X != ItemDropType.FountainPiece5)
                            m_itemInfo.X = ItemDropType.FountainPiece5;
                        else
                            m_itemInfo.X = ItemDropType.FountainPiece1;
                    }

                    if (previousItemInfo != m_itemInfo)
                    {
                        if (m_itemInfo.X == ItemDropType.FountainPiece5)
                            m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_8", m_itemFoundText);
                        else
                            m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_9", m_itemFoundText);
                    }
                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin(SpriteSortMode.Immediate, null, SamplerState.LinearClamp, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight), Color.Black * BackBufferOpacity);

            //// Level up animation sprites.
            m_levelUpBGImage.Draw(Camera);
            foreach (SpriteObj sprite in m_levelUpParticles)
                sprite.Draw(Camera);

            m_itemFoundSprite.Draw(Camera);
            m_itemFoundText.Draw(Camera);
            m_tripStat1FoundText.Draw(Camera);
            m_tripStat2FoundText.Draw(Camera);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_itemSprite.Draw(Camera);
            m_tripStat1.Draw(Camera);
            m_tripStat2.Draw(Camera);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_continueText.Draw(Camera);
            Camera.End();

            base.Draw(gameTime);
        }

        private string GetStatSpriteName(int type)
        {
            switch (type)
            {
                case (ItemDropType.Stat_MaxHealth):
                    return "Heart_Sprite";
                case (ItemDropType.Stat_MaxMana):
                    return "ManaCrystal_Sprite";
                case (ItemDropType.Stat_Strength):
                    return "Sword_Sprite";
                case (ItemDropType.Stat_Magic):
                    return "MagicBook_Sprite";
                case (ItemDropType.Stat_Defense):
                    return "Shield_Sprite";
                case (ItemDropType.Stat_Weight):
                    return "Backpack_Sprite";
            }
            return "";
        }

        private string GetStatText(int type)
        {
            switch (type)
            {
                case (ItemDropType.Stat_MaxHealth):
                    return LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_2") + ": +" + (GameEV.ITEM_STAT_MAXHP_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten));
                case (ItemDropType.Stat_MaxMana):
                    return LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_3") + ": +" + (GameEV.ITEM_STAT_MAXMP_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten));
                case (ItemDropType.Stat_Strength):
                    return LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_4") + ": +" + (GameEV.ITEM_STAT_STRENGTH_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten));
                case (ItemDropType.Stat_Magic):
                    return LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_5") + ": +" + (GameEV.ITEM_STAT_MAGIC_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten));
                case (ItemDropType.Stat_Defense):
                    return LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_6") + ": +" + (GameEV.ITEM_STAT_ARMOR_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten));
                case (ItemDropType.Stat_Weight):
                    return LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_7") + ": +" + (GameEV.ITEM_STAT_WEIGHT_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten));
            }
            return "";
        }

        private string GetMedallionImage(int medallionType)
        {
            switch (medallionType)
            {
                case (ItemDropType.FountainPiece1):
                    return "MedallionPiece1_Sprite";
                case(ItemDropType.FountainPiece2):
                    return "MedallionPiece2_Sprite";
                case (ItemDropType.FountainPiece3):
                    return "MedallionPiece3_Sprite";
                case (ItemDropType.FountainPiece4):
                    return "MedallionPiece4_Sprite";
                case (ItemDropType.FountainPiece5):
                    return "MedallionPiece5_Sprite";
            }
            return "";
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Get Item Screen");

                m_continueText.Dispose();
                m_continueText = null;
                m_levelUpBGImage.Dispose();
                m_levelUpBGImage = null;

                foreach (SpriteObj sprite in m_levelUpParticles)
                    sprite.Dispose();
                Array.Clear(m_levelUpParticles, 0, m_levelUpParticles.Length);
                m_levelUpParticles = null;

                m_buildUpSound = null;

                m_itemSprite.Dispose();
                m_itemSprite = null;
                m_itemFoundSprite.Dispose();
                m_itemFoundSprite = null;
                m_itemFoundText.Dispose();
                m_itemFoundText = null;

                m_tripStat1.Dispose();
                m_tripStat2.Dispose();
                m_tripStat1 = null;
                m_tripStat2 = null;
                m_tripStat1FoundText.Dispose();
                m_tripStat2FoundText.Dispose();
                m_tripStat1FoundText = null;
                m_tripStat2FoundText = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            //m_continueText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "]  " + LocaleBuilder.getResourceString("LOC_ID_GET_ITEM_SCREEN_1");
            switch (m_itemType)
            {
                case (GetItemType.Blueprint):
                    m_itemFoundText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentBaseType.ToStringID((int)m_itemInfo.Y), true), LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID((int)m_itemInfo.X), true));
                    break;
                case (GetItemType.Rune):
                    m_itemFoundText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_RUNE_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentAbilityType.ToStringID((int)m_itemInfo.Y), true), LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_13"), true) /*"Rune"*/ + " (" + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID2((int)m_itemInfo.X), true) + ")";
                    //m_itemFoundText.Text = LocaleBuilder.getResourceString(EquipmentAbilityType.ToStringID((int)m_itemInfo.Y)) + " " + LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_13") /* Rune */ + " (" + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID2((int)m_itemInfo.X)) + ")";
                    break;
                case (GetItemType.StatDrop):
                case (GetItemType.TripStatDrop):
                    m_itemFoundText.Text = this.GetStatText((int)m_itemInfo.X);
                    if (m_itemType == GetItemType.TripStatDrop)
                    {
                        m_tripStat1FoundText.Text = this.GetStatText((int)m_tripStatData.X);
                        m_tripStat2FoundText.Text = this.GetStatText((int)m_tripStatData.Y);
                    }
                    break;
                case (GetItemType.SpecialItem):
                    m_itemFoundText.Text = LocaleBuilder.getString(SpecialItemType.ToStringID((byte)m_itemInfo.X), m_itemFoundText);
                    break;
                case (GetItemType.Spell):
                    m_itemFoundText.Text = LocaleBuilder.getString(SpellType.ToStringID((byte)m_itemInfo.X), m_itemFoundText);
                    break;
                case (GetItemType.FountainPiece):
                    if (m_itemInfo.X == ItemDropType.FountainPiece5)
                        m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_8", m_itemFoundText);
                    else
                        m_itemFoundText.Text = LocaleBuilder.getString("LOC_ID_GET_ITEM_SCREEN_9", m_itemFoundText);
                    break;
            }

            RefreshBitmaps();

            base.RefreshTextObjs();
        }

        private void RefreshBitmaps()
        {
            switch (m_itemType)
            {
                case (GetItemType.Blueprint):
                    Game.ChangeBitmapLanguage(m_itemFoundSprite, "BlueprintFoundText_Sprite");
                    break;
                case (GetItemType.Rune):
                    Game.ChangeBitmapLanguage(m_itemFoundSprite, "RuneFoundText_Sprite");
                    break;
                case (GetItemType.StatDrop):
                case (GetItemType.TripStatDrop):
                    Game.ChangeBitmapLanguage(m_itemFoundSprite, "StatFoundText_Sprite");
                    break;
                case (GetItemType.SpecialItem):
                case (GetItemType.FountainPiece):
                    Game.ChangeBitmapLanguage(m_itemFoundSprite, "ItemFoundText_Sprite");
                    break;
                case (GetItemType.Spell):
                    Game.ChangeBitmapLanguage(m_itemFoundSprite, "SpellFoundText_Sprite");
                    break;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;
using Tweener;
using Tweener.Ease;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class EnchantressScreen : Screen
    {
        private const int m_startingCategoryIndex = 6;

        private ObjContainer m_enchantressUI;
        private SpriteObj m_selectionIcon;
        public float BackBufferOpacity { get; set; }
        private int m_currentCategoryIndex = 0;
        private int m_currentEquipmentIndex = 0;

        private List<SpriteObj[]> m_masterIconArray;
        private SpriteObj[] m_activeIconArray;

        private List<SpriteObj> m_newIconList;
        private int m_newIconListIndex;
        private TextObj m_playerMoney;

        private SpriteObj m_equippedIcon;

        // Text objects
        private TextObj m_equipmentDescriptionText;
        private TextObj m_descriptionText;
        private ObjContainer m_unlockCostContainer;
        private TextObj m_instructionsTitleText;
        private KeyIconTextObj m_instructionsText;

        private TextObj m_equipmentTitleText; // The actual title of the currently selected piece of equipment.

        private bool m_inCategoryMenu = true;

        public PlayerObj Player { get; set; }

        private bool m_lockControls = false;

        private KeyIconTextObj m_confirmText;
        private KeyIconTextObj m_cancelText;
        private KeyIconTextObj m_navigationText;

        private Cue m_rainSound;

        public EnchantressScreen()
        {
            m_currentCategoryIndex = m_startingCategoryIndex;

            m_masterIconArray = new List<SpriteObj[]>();
            for (int i = 0; i < EquipmentCategoryType.Total; i++)
            {
                m_masterIconArray.Add(new SpriteObj[EquipmentAbilityType.Total]);
            }

        }

        public override void LoadContent()
        {
            m_enchantressUI = new ObjContainer("BlacksmithUI_Character");
            m_enchantressUI.Position = new Vector2(1320 / 2, 720 / 2);

            m_playerMoney = new TextObj(Game.GoldFont);
            m_playerMoney.Align = Types.TextAlign.Left;
            m_playerMoney.Text = "1000";
            m_playerMoney.FontSize = 30;
            m_playerMoney.OverrideParentScale = true;
            m_playerMoney.Position = new Vector2(210, -225);
            m_playerMoney.AnchorY = 10;
            m_enchantressUI.AddChild(m_playerMoney);

            // Changing the title text from "The Blacksmith" to "The Enchantress".
            m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 3).ChangeSprite("EnchantressUI_Title_Sprite");

            for (int i = 0; i < m_enchantressUI.NumChildren; i++)
                m_enchantressUI.GetChildAt(i).Scale = Vector2.Zero;

            m_selectionIcon = new SpriteObj("BlacksmithUI_SelectionIcon_Sprite");
            m_selectionIcon.PlayAnimation(true);
            m_selectionIcon.Scale = Vector2.Zero;
            m_selectionIcon.AnimationDelay = 1 / 10f;
            m_selectionIcon.ForceDraw = true;

            m_equipmentDescriptionText = new TextObj(Game.JunicodeFont);
            m_equipmentDescriptionText.Align = Types.TextAlign.Centre;
            m_equipmentDescriptionText.FontSize = 12;
            m_equipmentDescriptionText.Position = new Vector2(230, -20);
            m_equipmentDescriptionText.Text = LocaleBuilder.getString("LOC_ID_ENCHANTRESS_SCREEN_1", m_equipmentDescriptionText); //"Select a category"
            m_equipmentDescriptionText.WordWrap(190);
            m_equipmentDescriptionText.Scale = Vector2.Zero;
            m_enchantressUI.AddChild(m_equipmentDescriptionText);

            foreach (SpriteObj[] iconArray in m_masterIconArray)
            {
                Vector2 initialPosition = m_enchantressUI.GetChildAt(m_startingCategoryIndex).AbsPosition;
                initialPosition.X += 85;
                float startingX = initialPosition.X;
                float iconXOffset = 70;
                float iconYOffset = 80;

                for (int i = 0; i < iconArray.Length; i++)
                {
                    iconArray[i] = new SpriteObj("BlacksmithUI_QuestionMarkIcon_Sprite");
                    iconArray[i].Position = initialPosition;
                    iconArray[i].Scale = Vector2.Zero;
                    iconArray[i].ForceDraw = true;

                    initialPosition.X += iconXOffset;
                    if (initialPosition.X > startingX + (iconXOffset * 4))
                    {
                        initialPosition.X = startingX;
                        initialPosition.Y += iconYOffset;
                    }
                }
            }

            InitializeTextObjs();

            m_equippedIcon = new SpriteObj("BlacksmithUI_EquippedIcon_Sprite");

            m_confirmText = new KeyIconTextObj(Game.JunicodeFont);
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_confirmText); // dummy locID to add TextObj to language refresh list
            m_confirmText.FontSize = 12;
            m_confirmText.Position = new Vector2(50, 550);
            m_confirmText.ForceDraw = true;

            m_cancelText = new KeyIconTextObj(Game.JunicodeFont);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_cancelText); // dummy locID to add TextObj to language refresh list
            m_cancelText.FontSize = 12;
            m_cancelText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 40);
            m_cancelText.ForceDraw = true;

            m_navigationText = new KeyIconTextObj(Game.JunicodeFont);
            m_navigationText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_navigationText); // dummy locID to add TextObj to language refresh list
            m_navigationText.FontSize = 12;
            m_navigationText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 80);
            m_navigationText.ForceDraw = true;

            m_newIconList = new List<SpriteObj>();
            for (int i = 0; i < EquipmentAbilityType.Total * 5; i++)
            {
                SpriteObj newIcon = new SpriteObj("BlacksmithUI_NewIcon_Sprite");
                newIcon.Visible = false;
                newIcon.Scale = new Vector2(1.1f, 1.1f);
                m_newIconList.Add(newIcon);
            }

            base.LoadContent();
        }

        private void InitializeTextObjs()
        {
            m_descriptionText = new TextObj(Game.JunicodeFont);
            m_descriptionText.FontSize = 9;
            m_descriptionText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_descriptionText); // dummy locID to add TextObj to language refresh list
            //m_descriptionText.DropShadow = new Vector2(2, 2);

            m_instructionsTitleText = new TextObj();
            m_instructionsTitleText.Font = Game.JunicodeFont;
            m_instructionsTitleText.FontSize = 10;
            m_instructionsTitleText.TextureColor = new Color(237, 202, 138);
            m_instructionsTitleText.Text = LocaleBuilder.getString("LOC_ID_ENCHANTRESS_SCREEN_5", m_instructionsTitleText) + ":"; //"Instructions:"

            m_instructionsText = new KeyIconTextObj();
            m_instructionsText.Font = Game.JunicodeFont;
            m_instructionsText.FontSize = 10;
            m_instructionsText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_instructionsText); // dummy locID to add TextObj to language refresh list

            m_unlockCostContainer = new ObjContainer();
            TextObj coinText = new TextObj();
            coinText.Font = Game.JunicodeFont;
            coinText.FontSize = 10;
            coinText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", coinText); // dummy locID to add TextObj to language refresh list
            coinText.TextureColor = Color.Yellow;
            coinText.Position = new Vector2(50, 9);
            m_unlockCostContainer.AddChild(new SpriteObj("BlacksmithUI_CoinBG_Sprite"));
            m_unlockCostContainer.AddChild(coinText);

            m_descriptionText.Position = new Vector2(m_enchantressUI.X + 140, m_enchantressUI.Y - m_enchantressUI.Height / 2 + 20 + 40);
            m_instructionsTitleText.Position = new Vector2(m_enchantressUI.X + 140, m_descriptionText.Bounds.Bottom + 20);
            m_instructionsText.Position = new Vector2(m_instructionsTitleText.X, m_instructionsTitleText.Bounds.Bottom);
            m_unlockCostContainer.Position = new Vector2(m_enchantressUI.X + 114, 485);

            m_equipmentTitleText = new TextObj(Game.JunicodeFont);
            m_equipmentTitleText.ForceDraw = true;
            m_equipmentTitleText.FontSize = 10;
            m_equipmentTitleText.DropShadow = new Vector2(2, 2);
            m_equipmentTitleText.TextureColor = new Color(237, 202, 138);
            m_equipmentTitleText.Position = new Vector2(m_enchantressUI.X + 140, m_descriptionText.Y - 50);
            m_equipmentTitleText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_equipmentTitleText); // dummy locID to add TextObj to language refresh list

            m_descriptionText.Visible = false;
            m_instructionsTitleText.Visible = false;
            m_instructionsText.Visible = false;
            m_unlockCostContainer.Visible = false;
        }

        // Displays the icons in a category when you move up and down the category list.
        private void DisplayCategory(int equipmentType)
        {
            float tweenSpeed = 0.2f;
            float delay = 0;
            // Tween out the current active icon array.
            if (m_activeIconArray != null)
            {
                for (int i = 0; i < EquipmentAbilityType.Total; i++)
                {
                    Tween.StopAllContaining(m_activeIconArray[i], false);
                    Tween.To(m_activeIconArray[i], tweenSpeed, Back.EaseIn, "delay", delay.ToString(), "ScaleX", "0", "ScaleY", "0");
                }
            }

            m_activeIconArray = m_masterIconArray[equipmentType];
            delay = 0.25f;
            for (int i = 0; i < EquipmentAbilityType.Total; i++)
            {
                Tween.To(m_activeIconArray[i], tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            }

            foreach (SpriteObj icon in m_newIconList)
            {
                Tween.StopAllContaining(icon, false);
                icon.Scale = Vector2.Zero;
                Tween.To(icon, tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            }

            UpdateNewIcons();

            m_equippedIcon.Scale = Vector2.Zero;
            Tween.StopAllContaining(m_equippedIcon, false);
            Tween.To(m_equippedIcon, tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
        }

        public void EaseInMenu()
        {
            float tweenSpeed = 0.4f;

            // Tween in the menu background.
            Tween.To(m_enchantressUI.GetChildAt(0), tweenSpeed, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");

            // Tween in the selection halo icon.
            Tween.To(m_selectionIcon, tweenSpeed, Back.EaseOut, "delay", "0.25", "ScaleX", "1", "ScaleY", "1");
            float delay = 0.2f;

            // Tween in the Category icons and the title text.
            for (int i = m_startingCategoryIndex; i < m_enchantressUI.NumChildren - 3; i++)
            {
                delay += 0.05f;
                // Scaling in the Category icons.
                Tween.To(m_enchantressUI.GetChildAt(i), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            }

            // Tween in the description text.
            Tween.To(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 1), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 2), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 3), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            Tween.AddEndHandlerToLastTween(this, "EaseInComplete");
        }

        public void EaseInComplete()
        {
            m_lockControls = false;
        }

        private void EaseOutMenu()
        {
            foreach (SpriteObj icon in m_newIconList)
                icon.Visible = false;
            m_equippedIcon.Visible = false;

            Tween.To(m_confirmText, 0.2f, Linear.EaseNone, "Opacity", "0");
            Tween.To(m_cancelText, 0.2f, Linear.EaseNone, "Opacity", "0");
            Tween.To(m_navigationText, 0.2f, Linear.EaseNone, "Opacity", "0");

            float tweenSpeed = 0.4f;
            float delay = 0f;

            // Tween out the description text and title text and equipped icon.
            Tween.To(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 2), tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0"); // Money text
            Tween.To(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 3), tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0"); // Coin Icon
            Tween.To(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 4), tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0"); // Title Text

            for (int i = m_startingCategoryIndex; i < m_startingCategoryIndex + 5; i++)
            {
                // Tween out the selection halo icon.
                if (m_currentCategoryIndex == i)
                    Tween.To(m_selectionIcon, tweenSpeed, Back.EaseIn, "delay", delay.ToString(), "ScaleX", "0", "ScaleY", "0");

                Tween.To(m_enchantressUI.GetChildAt(i), tweenSpeed, Back.EaseIn, "delay", delay.ToString(), "ScaleX", "0", "ScaleY", "0");
                delay += 0.05f;
            }


            // Resets all the category backgrounds.
            for (int i = m_startingCategoryIndex - 5; i < m_startingCategoryIndex; i++)
            {
                m_enchantressUI.GetChildAt(i).Scale = Vector2.Zero;
            }

            // Tween out the visible equipment icons.
            for (int i = 0; i < m_activeIconArray.Length; i++)
            {
                Tween.To(m_activeIconArray[i], tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            }

            // Tween out the menu background.
            Tween.To(m_enchantressUI.GetChildAt(0), tweenSpeed, Back.EaseIn, "delay", "0.3", "ScaleX", "0", "ScaleY", "0");
            //Tween.AddEndHandlerToLastTween((ScreenManager as RCScreenManager), "HideCurrentScreen");
            Tween.RunFunction(tweenSpeed + 0.35f, ScreenManager, "HideCurrentScreen");
        }

        private void UpdateIconStates()
        {
            for (int i = 0; i < Game.PlayerStats.GetRuneArray.Count; i++)
            {
                for (int k = 0; k < Game.PlayerStats.GetRuneArray[i].Length; k++)
                {
                    byte state = Game.PlayerStats.GetRuneArray[i][k];
                    if (state == EquipmentState.NotFound)
                        m_masterIconArray[i][k].ChangeSprite("BlacksmithUI_QuestionMarkIcon_Sprite");
                    else
                    {
                        m_masterIconArray[i][k].ChangeSprite(EquipmentAbilityType.Icon(k));
                        m_masterIconArray[i][k].Opacity = 0.2f;
                    }

                    if (state >= EquipmentState.Purchased)
                        m_masterIconArray[i][k].Opacity = 1f;
                }
            }
        }

        public override void OnEnter()
        {
            if (m_rainSound != null)
                m_rainSound.Dispose();

            bool isSnowing = (DateTime.Now.Month == 12 || DateTime.Now.Month == 1);
            if (isSnowing == false)
                m_rainSound = SoundManager.PlaySound("Rain1_Filtered");
            else
                m_rainSound = SoundManager.PlaySound("snowloop_filtered");

            if (Game.PlayerStats.TotalRunesFound >= (EquipmentAbilityType.Total * EquipmentCategoryType.Total))
                GameUtil.UnlockAchievement("LOVE_OF_KNOWLEDGE");

            m_lockControls = true;
            SoundManager.PlaySound("ShopMenuOpen");

            m_confirmText.Opacity = 0;
            m_cancelText.Opacity = 0;
            m_navigationText.Opacity = 0;

            Tween.To(m_confirmText, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.To(m_cancelText, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.To(m_navigationText, 0.2f, Linear.EaseNone, "Opacity", "1");

            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_ENCHANTRESS_SCREEN_6_NEW", m_confirmText); //"select/equip"
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_ENCHANTRESS_SCREEN_7_NEW", m_cancelText); //"cancel/close menu"

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_ENCHANTRESS_SCREEN_8", m_navigationText); //"Arrow keys to navigate"
            else
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_ENCHANTRESS_SCREEN_9_NEW", m_navigationText); //"to navigate"

            m_currentEquipmentIndex = 0;
            m_inCategoryMenu = true;
            m_selectionIcon.Position = m_enchantressUI.GetChildAt(m_startingCategoryIndex).AbsPosition;
            m_currentCategoryIndex = m_startingCategoryIndex;
            UpdateIconStates();
            DisplayCategory(EquipmentCategoryType.Sword);
            EaseInMenu();
            Tween.To(this, 0.2f, Linear.EaseNone, "BackBufferOpacity", "0.5");
            UpdateIconSelectionText();

            RefreshTextObjs();
            base.OnEnter();
        }

        public override void OnExit()
        {
            if (m_rainSound != null)
                m_rainSound.Stop(AudioStopOptions.Immediate);

            for (int i = 0; i < m_enchantressUI.NumChildren; i++)
            {
                m_enchantressUI.GetChildAt(i).Scale = Vector2.Zero;
            }

            foreach (SpriteObj[] iconArray in m_masterIconArray)
            {
                for (int i = 0; i < iconArray.Length; i++)
                {
                    iconArray[i].Scale = Vector2.Zero;
                }
            }
            m_selectionIcon.Scale = Vector2.Zero;

            (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.UpgradeData);

            bool unlockAchievement = true;
            foreach (sbyte equipmentType in Game.PlayerStats.GetEquippedRuneArray)
            {
                if (equipmentType == -1)
                {
                    unlockAchievement = false;
                    break;
                }
            }
            if (unlockAchievement == true)
                GameUtil.UnlockAchievement("LOVE_OF_CHANGE");

            base.OnExit();
        }

        public override void HandleInput()
        {
            if (m_lockControls == false)
            {
                if (m_inCategoryMenu == true)
                    CategorySelectionInput();
                else
                    EquipmentSelectionInput();
            }

            base.HandleInput();
        }

        private void CategorySelectionInput()
        {
            int currentCategoryIndex = m_currentCategoryIndex;
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
            {
                m_currentCategoryIndex--;
                if (m_currentCategoryIndex < m_startingCategoryIndex)
                    m_currentCategoryIndex = m_startingCategoryIndex + 4;
            }
            else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
            {
                m_currentCategoryIndex++;
                if (m_currentCategoryIndex > m_startingCategoryIndex + 4)
                    m_currentCategoryIndex = m_startingCategoryIndex;
            }

            if (currentCategoryIndex != m_currentCategoryIndex)
            {
                SoundManager.PlaySound("ShopBSMenuMove");

                m_selectionIcon.Position = m_enchantressUI.GetChildAt(m_currentCategoryIndex).AbsPosition;
                for (int i = 1; i < 6; i++)
                {
                    if (i == 1)
                        m_enchantressUI.GetChildAt(i).Scale = new Vector2(1, 1);
                    else
                        m_enchantressUI.GetChildAt(i).Scale = Vector2.Zero;
                }

                if (m_currentCategoryIndex != m_startingCategoryIndex)
                    m_enchantressUI.GetChildAt(m_currentCategoryIndex - 5).Scale = new Vector2(1, 1);
                else
                    m_enchantressUI.GetChildAt(m_currentCategoryIndex - 5).Scale = Vector2.Zero;

                DisplayCategory(m_currentCategoryIndex - m_startingCategoryIndex);
            }

            // Player is stepping out of the category menu, effectively closing the blacksmith screen.
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                m_lockControls = true;
                Tween.To(this, 0.2f, Linear.EaseNone, "delay", "0.5", "BackBufferOpacity", "0");
                EaseOutMenu();
                Tween.RunFunction(0.13f, typeof(SoundManager), "PlaySound", "ShopMenuClose");
            }

            // Player is stepping into the category, to select a specific piece of equipment to purchase or equip.
            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                m_inCategoryMenu = false;
                m_currentEquipmentIndex = 0;
                m_selectionIcon.Position = m_activeIconArray[m_currentEquipmentIndex].AbsPosition;
                byte state = Game.PlayerStats.GetRuneArray[CurrentCategoryIndex][m_currentEquipmentIndex];
                if (state == EquipmentState.FoundButNotSeen)
                    Game.PlayerStats.GetRuneArray[CurrentCategoryIndex][m_currentEquipmentIndex] = EquipmentState.FoundAndSeen;
                UpdateNewIcons();
                UpdateIconSelectionText();
                SoundManager.PlaySound("ShopMenuConfirm");
            }
        }

        private void EquipmentSelectionInput()
        {
            int storedEquipmentIndex = m_currentEquipmentIndex;
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
            {
                if (m_currentEquipmentIndex > 4)
                    m_currentEquipmentIndex -= 5;
                if (m_currentEquipmentIndex < 0)
                    m_currentEquipmentIndex = 0;
            }
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
            {
                if (m_currentEquipmentIndex < EquipmentAbilityType.Total - 5)
                    m_currentEquipmentIndex += 5;
                if (m_currentEquipmentIndex > EquipmentAbilityType.Total - 1)
                    m_currentEquipmentIndex -= 5;
            }
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2))
            {
                m_currentEquipmentIndex--;
                if ((m_currentEquipmentIndex + 1) % 5 == 0)
                    m_currentEquipmentIndex++;
            }
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
            {
                m_currentEquipmentIndex++;
                if (m_currentEquipmentIndex % 5 == 0 || (m_currentEquipmentIndex > EquipmentAbilityType.Total - 1))
                    m_currentEquipmentIndex--;
            }

            if (storedEquipmentIndex != m_currentEquipmentIndex)
            {
                byte state = Game.PlayerStats.GetRuneArray[CurrentCategoryIndex][m_currentEquipmentIndex];
                if (state == EquipmentState.FoundButNotSeen)
                    Game.PlayerStats.GetRuneArray[CurrentCategoryIndex][m_currentEquipmentIndex] = EquipmentState.FoundAndSeen;
                UpdateNewIcons();
                UpdateIconSelectionText();
                m_selectionIcon.Position = m_activeIconArray[m_currentEquipmentIndex].AbsPosition;
                SoundManager.PlaySound("ShopBSMenuMove");
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                SoundManager.PlaySound("ShopMenuCancel");
                m_inCategoryMenu = true;
                m_selectionIcon.Position = m_enchantressUI.GetChildAt(m_currentCategoryIndex).AbsPosition;
                UpdateIconSelectionText();
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                int equipmentCategory = m_currentCategoryIndex - m_startingCategoryIndex;
                int purchasedState = Game.PlayerStats.GetRuneArray[equipmentCategory][m_currentEquipmentIndex];
                int equippedIndex = Game.PlayerStats.GetEquippedRuneArray[equipmentCategory];

                // Purchasing a previously locked piece of equipment.
                if (purchasedState < EquipmentState.Purchased && purchasedState > EquipmentState.NotFound)
                {
                    int cost = Game.EquipmentSystem.GetAbilityCost(equipmentCategory, m_currentEquipmentIndex);

                    if (Game.PlayerStats.Gold >= cost)
                    {
                        SoundManager.PlaySound("ShopMenuUnlock");
                        Game.PlayerStats.Gold -= cost;
                        Game.PlayerStats.GetRuneArray[equipmentCategory][m_currentEquipmentIndex] = EquipmentState.Purchased;
                        Game.PlayerStats.GetEquippedRuneArray[equipmentCategory] = (sbyte)m_currentEquipmentIndex;
                        Player.AttachedLevel.UpdatePlayerHUDAbilities();

                        SpriteObj icon = m_masterIconArray[equipmentCategory][m_currentEquipmentIndex];
                        //icon.ChangeSprite(EquipmentAbilityType.Icon(m_currentEquipmentIndex % 5));
                        icon.Opacity = 1f;

                        purchasedState = EquipmentState.Purchased;
                        UpdateIconSelectionText();
                    }
                }

                // Changing the currently equipped equipment piece.
                if (equippedIndex != m_currentEquipmentIndex && purchasedState == EquipmentState.Purchased)
                {
                    m_equippedIcon.Scale = new Vector2(1, 1);
                    m_equippedIcon.Position = m_activeIconArray[m_currentEquipmentIndex].AbsPosition;
                    m_equippedIcon.Position += new Vector2(18, 18);
                    Game.PlayerStats.GetEquippedRuneArray[equipmentCategory] = (sbyte)m_currentEquipmentIndex;
                    Player.AttachedLevel.UpdatePlayerHUDAbilities();
                    SoundManager.PlaySound("ShopBSEquip"); // this should be an equip sound.

                    UpdateIconSelectionText();
                    UpdateNewIcons();
                }
                else if (equippedIndex == m_currentEquipmentIndex) // Unequipping
                {
                    m_equippedIcon.Scale = Vector2.Zero;
                    Game.PlayerStats.GetEquippedRuneArray[equipmentCategory] = -1;
                    Player.AttachedLevel.UpdatePlayerHUDAbilities();
                    UpdateNewIcons();
                }
            }
        }

        private void UpdateIconSelectionText()
        {
            m_equipmentDescriptionText.Position = new Vector2(-1000, -1000);
            m_descriptionText.Visible = false;
            m_instructionsTitleText.Visible = false;
            m_instructionsText.Visible = false;
            m_unlockCostContainer.Visible = false;
            m_equipmentTitleText.Visible = false;

            if (m_inCategoryMenu == true)
            {
                m_equipmentDescriptionText.Text = LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_1"); //"Select a category"
            }
            else
            {
                if (Game.PlayerStats.GetRuneArray[m_currentCategoryIndex - m_startingCategoryIndex][m_currentEquipmentIndex] == EquipmentState.NotFound)
                {
                    m_equipmentDescriptionText.Position = new Vector2(230, -20);
                    m_equipmentDescriptionText.Text = LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_10"); //"Rune needed"
                }
                else if (Game.PlayerStats.GetRuneArray[m_currentCategoryIndex - m_startingCategoryIndex][m_currentEquipmentIndex] < EquipmentState.Purchased)
                {
                    m_equipmentDescriptionText.Text = LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_11"); //"Purchase Info Here"
                    (m_unlockCostContainer.GetChildAt(1) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_12_NEW"), Game.EquipmentSystem.GetAbilityCost(m_currentCategoryIndex - m_startingCategoryIndex, m_currentEquipmentIndex).ToString());
                    //(m_unlockCostContainer.GetChildAt(1) as TextObj).Text = Game.EquipmentSystem.GetAbilityCost(m_currentCategoryIndex - m_startingCategoryIndex, m_currentEquipmentIndex).ToString() + " " + LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_12"); //"to unlock"

                    m_unlockCostContainer.Visible = true;
                    m_descriptionText.Visible = true;
                    m_instructionsTitleText.Visible = true;
                    m_instructionsText.Visible = true;
                    m_equipmentTitleText.Visible = true;

                    m_descriptionText.Opacity = 0.5f;
                    m_instructionsTitleText.Opacity = 0.5f;
                    m_instructionsText.Opacity = 0.5f;
                    m_equipmentTitleText.Opacity = 0.5f;

                    UpdateEquipmentDataText();
                }
                else
                {
                    // This code displays all the information for the stats.
                    m_descriptionText.Visible = true;
                    m_instructionsTitleText.Visible = true;
                    m_instructionsText.Visible = true;
                    m_equipmentTitleText.Visible = true;

                    m_descriptionText.Opacity = 1;
                    m_instructionsTitleText.Opacity = 1;
                    m_instructionsText.Opacity = 1;
                    m_equipmentTitleText.Opacity = 1;

                    UpdateEquipmentDataText();

                }
            }
        }

        // Updates the base player stats text and the bonus attribute text that a piece of equipment gives you.
        private void UpdateEquipmentDataText()
        {
            m_equipmentTitleText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_RUNE_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentAbilityType.ToStringID(m_currentEquipmentIndex), true), LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_13"), true) /*"Rune"*/ + "\n(" + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID2(m_currentCategoryIndex - m_startingCategoryIndex), true) + ")";

            m_descriptionText.Text = LocaleBuilder.getResourceString(EquipmentAbilityType.DescriptionID(m_currentEquipmentIndex));
            m_descriptionText.WordWrap(195);
            m_descriptionText.Y = m_equipmentTitleText.Y + 60;

            m_instructionsTitleText.Position = new Vector2(m_enchantressUI.X + 140, m_descriptionText.Bounds.Bottom + 20); // Must reposition to take word wrap into account.
            m_instructionsText.Text = EquipmentAbilityType.Instructions(m_currentEquipmentIndex);
            m_instructionsText.WordWrap(200);

            m_instructionsText.Position = new Vector2(m_instructionsTitleText.X, m_instructionsTitleText.Bounds.Bottom);
            //m_equipmentTitleText.Text = EquipmentAbilityType.ToString(m_currentEquipmentIndex) + " Enchantment";
            //m_equipmentTitleText.Text = EquipmentCategoryType.ToString2(m_currentCategoryIndex - m_startingCategoryIndex) + " " + EquipmentAbilityType.ToString(m_currentEquipmentIndex) + " Rune";
        }

        private void UpdateNewIcons()
        {
            UpdateMoneyText();

            m_newIconListIndex = 0;
            foreach (SpriteObj sprite in m_newIconList)
                sprite.Visible = false;

            for (int i = 0; i < Game.PlayerStats.GetRuneArray[CurrentCategoryIndex].Length; i++)
            {
                byte state = Game.PlayerStats.GetRuneArray[CurrentCategoryIndex][i];
                if (state == EquipmentState.FoundButNotSeen)
                {
                    SpriteObj equipmentIcon = m_masterIconArray[CurrentCategoryIndex][i];

                    SpriteObj newIcon = m_newIconList[m_newIconListIndex];
                    newIcon.Visible = true;
                    newIcon.Position = m_masterIconArray[CurrentCategoryIndex][i].AbsPosition;
                    newIcon.X -= 20;
                    newIcon.Y -= 30;
                    m_newIconListIndex++;
                }
            }


            sbyte equippedItemIndex = Game.PlayerStats.GetEquippedRuneArray[CurrentCategoryIndex];
            if (equippedItemIndex > -1)
            {
                m_equippedIcon.Position = new Vector2(m_activeIconArray[equippedItemIndex].AbsPosition.X + 18, m_activeIconArray[equippedItemIndex].AbsPosition.Y + 18);
                m_equippedIcon.Visible = true;
            }
            else
                m_equippedIcon.Visible = false;
        }


        private void UpdateMoneyText()
        {
            m_playerMoney.Text = Game.PlayerStats.Gold.ToString();
            ProceduralLevelScreen level = Game.ScreenManager.GetLevelScreen();
            if (level != null)
                level.UpdatePlayerHUD();
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin();
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.Black * BackBufferOpacity);
            m_enchantressUI.Draw(Camera);
            m_selectionIcon.Draw(Camera);

            //m_textInfoContainer.DrawOutline(Camera, 2);
            m_descriptionText.Draw(Camera);
            if (Game.PlayerStats.GetRuneArray[CurrentCategoryIndex][m_currentEquipmentIndex] > EquipmentState.FoundAndSeen)
            {
                m_instructionsTitleText.Draw(Camera);
                m_instructionsText.Draw(Camera);
            }
            m_unlockCostContainer.Draw(Camera);
            m_equipmentTitleText.Draw(Camera);

            foreach (SpriteObj[] iconArray in m_masterIconArray)
            {
                for (int i = 0; i < iconArray.Length; i++)
                    iconArray[i].Draw(Camera);
            }

            m_navigationText.Draw(Camera);
            m_cancelText.Draw(Camera);
            m_confirmText.Draw(Camera);

            m_equippedIcon.Draw(Camera);

            foreach (SpriteObj icon in m_newIconList)
                icon.Draw(Camera);

            Camera.End();
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Enchantress Screen");

                if (m_rainSound != null)
                    m_rainSound.Dispose();
                m_rainSound = null;

                m_enchantressUI.Dispose();
                m_enchantressUI = null;
                m_equipmentDescriptionText.Dispose();
                m_equipmentDescriptionText = null;
                m_selectionIcon.Dispose();
                m_selectionIcon = null;

                m_equipmentTitleText.Dispose();
                m_equipmentTitleText = null;

                m_activeIconArray = null;

                foreach (SpriteObj[] iconArray in m_masterIconArray)
                {
                    for (int i = 0; i < iconArray.Length; i++)
                    {
                        iconArray[i].Dispose();
                        iconArray[i] = null;
                    }
                    Array.Clear(iconArray, 0, iconArray.Length);
                }
                m_masterIconArray.Clear();
                m_masterIconArray = null;

                m_descriptionText.Dispose();
                m_descriptionText = null;
                m_unlockCostContainer.Dispose();
                m_unlockCostContainer = null;
                m_instructionsText.Dispose();
                m_instructionsText = null;
                m_instructionsTitleText.Dispose();
                m_instructionsTitleText = null;
                m_equippedIcon.Dispose();
                m_equippedIcon = null;
                Player = null;

                m_confirmText.Dispose();
                m_confirmText = null;
                m_cancelText.Dispose();
                m_cancelText = null;
                m_navigationText.Dispose();
                m_navigationText = null;

                m_playerMoney = null;
                foreach (SpriteObj sprite in m_newIconList)
                    sprite.Dispose();
                m_newIconList.Clear();
                m_newIconList = null;

                base.Dispose();
            }
        }

        private int CurrentCategoryIndex
        {
            get { return m_currentCategoryIndex - m_startingCategoryIndex; }
        }

        public override void RefreshTextObjs()
        {
            m_instructionsTitleText.Text = LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_5") + ":"; //"Instructions:"

            /*
            m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "]  " + LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_6"); //"select/equip"
            m_cancelText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "]  " + LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_7"); //"cancel/close menu"

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                m_navigationText.Text = LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_8"); //"Arrow keys to navigate"
            else
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_ENCHANTRESS_SCREEN_9"); //"to navigate"
             */

            (m_unlockCostContainer.GetChildAt(1) as TextObj).ScaleX = 1;
            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.French):
                case (LanguageType.Spanish_Spain):
                case (LanguageType.German):
                case (LanguageType.Polish):
                    (m_unlockCostContainer.GetChildAt(1) as TextObj).ScaleX = 0.9f;
                    break;
            }

            Game.ChangeBitmapLanguage(m_enchantressUI.GetChildAt(m_enchantressUI.NumChildren - 4) as SpriteObj, "EnchantressUI_Title_Sprite");
            foreach (SpriteObj icon in m_newIconList)
                Game.ChangeBitmapLanguage(icon, "BlacksmithUI_NewIcon_Sprite");

            UpdateIconSelectionText();
            UpdateEquipmentDataText();
            base.RefreshTextObjs();
        }
    }
}

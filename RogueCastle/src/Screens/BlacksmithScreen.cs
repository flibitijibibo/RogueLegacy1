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
    public class BlacksmithScreen : Screen
    {
        private const int m_startingCategoryIndex = 6;

        private ObjContainer m_blacksmithUI;
        private SpriteObj m_selectionIcon;
        public float BackBufferOpacity { get; set; }
        private int m_currentCategoryIndex = 0;
        private int m_currentEquipmentIndex = 0;

        private List<ObjContainer[]> m_masterIconArray;
        private ObjContainer[] m_activeIconArray;

        private List<SpriteObj> m_newIconList;
        private int m_newIconListIndex;
        private TextObj m_playerMoney;

        private SpriteObj m_equippedIcon;

        // Text objects
        private TextObj m_equipmentDescriptionText;
        private ObjContainer m_textInfoTitleContainer;
        private ObjContainer m_textInfoStatContainer;
        private ObjContainer m_textInfoStatModContainer;
        private ObjContainer m_unlockCostContainer;
        private TextObj m_addPropertiesTitleText;
        private TextObj m_addPropertiesText;
        private TextObj m_equipmentTitleText;

        private bool m_inCategoryMenu = true;

        public PlayerObj Player { get; set; }

        private KeyIconTextObj m_confirmText;
        private KeyIconTextObj m_cancelText;
        private KeyIconTextObj m_navigationText;

        private bool m_lockControls = false;
        private Cue m_rainSound;

        public BlacksmithScreen()
        {
            m_currentCategoryIndex = m_startingCategoryIndex;

            m_masterIconArray = new List<ObjContainer[]>();
            for (int i = 0; i < EquipmentCategoryType.Total; i++)
            {
                m_masterIconArray.Add(new ObjContainer[EquipmentBaseType.Total]);
            }
        }

        public override void LoadContent()
        {
            m_blacksmithUI = new ObjContainer("BlacksmithUI_Character");
            m_blacksmithUI.Position = new Vector2(1320 / 2, 720 / 2);

            m_playerMoney = new TextObj(Game.GoldFont);
            m_playerMoney.Align = Types.TextAlign.Left;
            m_playerMoney.Text = "1000";
            m_playerMoney.FontSize = 30;
            m_playerMoney.OverrideParentScale = true;
            m_playerMoney.Position = new Vector2(210, -225);
            m_playerMoney.AnchorY = 10;
            m_blacksmithUI.AddChild(m_playerMoney);

            for (int i = 0; i < m_blacksmithUI.NumChildren; i++)
                m_blacksmithUI.GetChildAt(i).Scale = Vector2.Zero;

            m_selectionIcon = new SpriteObj("BlacksmithUI_SelectionIcon_Sprite");
            m_selectionIcon.PlayAnimation(true);
            m_selectionIcon.Scale = Vector2.Zero;
            m_selectionIcon.AnimationDelay = 1 / 10f;
            m_selectionIcon.ForceDraw = true;

            m_equipmentDescriptionText = new TextObj(Game.JunicodeFont);
            m_equipmentDescriptionText.Align = Types.TextAlign.Centre;
            m_equipmentDescriptionText.FontSize = 12;
            m_equipmentDescriptionText.Position = new Vector2(230, -20);
            m_equipmentDescriptionText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_12", m_equipmentDescriptionText);
            m_equipmentDescriptionText.WordWrap(190);
            m_equipmentDescriptionText.Scale = Vector2.Zero;
            m_blacksmithUI.AddChild(m_equipmentDescriptionText);

            foreach (ObjContainer[] iconArray in m_masterIconArray)
            {
                Vector2 initialPosition = m_blacksmithUI.GetChildAt(m_startingCategoryIndex).AbsPosition;
                initialPosition.X += 85;
                float startingX = initialPosition.X;
                float iconXOffset = 70;
                float iconYOffset = 80;

                for (int i = 0; i < iconArray.Length; i++)
                {
                    iconArray[i] = new ObjContainer("BlacksmithUI_QuestionMarkIcon_Character");
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
            for (int i = 0; i < 25; i++)
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
            m_textInfoTitleContainer = new ObjContainer();
            m_textInfoStatContainer = new ObjContainer();
            m_textInfoStatModContainer = new ObjContainer();

            string[] nameIDArray = { "LOC_ID_BLACKSMITH_SCREEN_1", "LOC_ID_BLACKSMITH_SCREEN_2", "LOC_ID_BLACKSMITH_SCREEN_3", "LOC_ID_BLACKSMITH_SCREEN_4", "LOC_ID_BLACKSMITH_SCREEN_5", "LOC_ID_BLACKSMITH_SCREEN_6" };
            Vector2 textPosition = Vector2.Zero;

            TextObj textObj = new TextObj();
            textObj.Font = Game.JunicodeFont;
            textObj.FontSize = 10;
            textObj.Text = "0";
            textObj.ForceDraw = true;

            for (int i = 0; i < nameIDArray.Length; i++)
            {
                textObj.Position = textPosition;
                m_textInfoTitleContainer.AddChild(textObj.Clone() as TextObj);
                m_textInfoStatContainer.AddChild(textObj.Clone() as TextObj);
                m_textInfoStatModContainer.AddChild(textObj.Clone() as TextObj);
                (m_textInfoTitleContainer.GetChildAt(i) as TextObj).Align = Types.TextAlign.Right;
                (m_textInfoTitleContainer.GetChildAt(i) as TextObj).Text = LocaleBuilder.getString(nameIDArray[i], m_textInfoTitleContainer.GetChildAt(i) as TextObj);

                textPosition.Y += m_textInfoTitleContainer.GetChildAt(i).Height - 5;
            }

            m_addPropertiesTitleText = new TextObj();
            m_addPropertiesTitleText.Font = Game.JunicodeFont;
            m_addPropertiesTitleText.FontSize = 8;
            m_addPropertiesTitleText.TextureColor = new Color(237, 202, 138);
            m_addPropertiesTitleText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_7", m_addPropertiesTitleText);

            m_addPropertiesText = new TextObj();
            m_addPropertiesText.Font = Game.JunicodeFont;
            m_addPropertiesText.FontSize = 8;
            m_addPropertiesText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_addPropertiesText); // dummy locID to add TextObj to language refresh list

            m_unlockCostContainer = new ObjContainer();
            TextObj coinText = new TextObj();
            coinText.Font = Game.JunicodeFont;
            coinText.FontSize = 10;
            coinText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", coinText); // dummy locID to add TextObj to language refresh list
            coinText.TextureColor = Color.Yellow;
            coinText.Position = new Vector2(50, 9);
            m_unlockCostContainer.AddChild(new SpriteObj("BlacksmithUI_CoinBG_Sprite"));
            m_unlockCostContainer.AddChild(coinText);

            m_equipmentTitleText = new TextObj(Game.JunicodeFont);
            m_equipmentTitleText.ForceDraw = true;
            m_equipmentTitleText.FontSize = 12;
            m_equipmentTitleText.DropShadow = new Vector2(2, 2);
            m_equipmentTitleText.ScaleX = 0.9f;
            m_equipmentTitleText.TextureColor = new Color(237, 202, 138);
            m_equipmentTitleText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_equipmentTitleText); // dummy locID to add TextObj to language refresh list

            // This is where all the plate textobjs are positioned.
            m_textInfoTitleContainer.Position = new Vector2(m_blacksmithUI.X + 205, m_blacksmithUI.Y - m_blacksmithUI.Height / 2 + 45);
            m_textInfoStatContainer.Position = new Vector2(m_textInfoTitleContainer.X + 15, m_textInfoTitleContainer.Y);
            m_textInfoStatModContainer.Position = new Vector2(m_textInfoStatContainer.X + 75, m_textInfoStatContainer.Y);
            m_addPropertiesTitleText.Position = new Vector2(m_blacksmithUI.X + 125, m_textInfoStatModContainer.Bounds.Bottom - 3);
            m_addPropertiesText.Position = new Vector2(m_addPropertiesTitleText.X + 15, m_addPropertiesTitleText.Bounds.Bottom);
            m_unlockCostContainer.Position = new Vector2(m_blacksmithUI.X + 114, 485);
            m_equipmentTitleText.Position = new Vector2(m_blacksmithUI.X + 125, m_textInfoTitleContainer.Y - 45);

            m_textInfoTitleContainer.Visible = false;
            m_textInfoStatContainer.Visible = false;
            m_textInfoStatModContainer.Visible = false;
            m_addPropertiesTitleText.Visible = false;
            m_addPropertiesText.Visible = false;
            m_unlockCostContainer.Visible = false;
            m_equipmentTitleText.Visible = false;
        }

        // Displays the icons in a category when you move up and down the category list.
        private void DisplayCategory(int equipmentType)
        {
            float tweenSpeed = 0.2f;
            float delay = 0;
            // Tween out the current active icon array.
            if (m_activeIconArray != null)
            {
                for (int i = 0; i < EquipmentBaseType.Total; i++)
                {
                    Tween.StopAllContaining(m_activeIconArray[i], false);
                    Tween.To(m_activeIconArray[i], tweenSpeed, Back.EaseIn, "delay", delay.ToString(), "ScaleX", "0", "ScaleY", "0");
                }
            }

            m_activeIconArray = m_masterIconArray[equipmentType];
            delay = 0.2f;
            for (int i = 0; i < EquipmentBaseType.Total; i++)
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
            Tween.To(m_blacksmithUI.GetChildAt(0), tweenSpeed, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");

            // Tween in the selection halo icon.
            Tween.To(m_selectionIcon, tweenSpeed, Back.EaseOut, "delay", "0.25", "ScaleX", "1", "ScaleY", "1");
            float delay = 0.2f;

            // Tween in the Category icons and the title text.
            for (int i = m_startingCategoryIndex; i < m_blacksmithUI.NumChildren - 3; i++)
            {
                delay += 0.05f;
                // Scaling in the Category icons.
                Tween.To(m_blacksmithUI.GetChildAt(i), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            }

            // Tween in the description text.
            Tween.To(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 1), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 2), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 3), tweenSpeed, Back.EaseOut, "delay", delay.ToString(), "ScaleX", "1", "ScaleY", "1");
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

            // Tween out the description text and title text.
            Tween.To(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 2), tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0"); // Money text
            Tween.To(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 3), tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0"); // Coin Icon
            Tween.To(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 4), tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0"); // Title Text

            for (int i = m_startingCategoryIndex; i < m_startingCategoryIndex + 5; i++)
            {
                // Tween out the selection halo icon.
                if (m_currentCategoryIndex == i)
                    Tween.To(m_selectionIcon, tweenSpeed, Back.EaseIn, "delay", delay.ToString(), "ScaleX", "0", "ScaleY", "0");

                Tween.To(m_blacksmithUI.GetChildAt(i), tweenSpeed, Back.EaseIn, "delay", delay.ToString(), "ScaleX", "0", "ScaleY", "0");
                delay += 0.05f;
            }


            // Resets all the category backgrounds.
            for (int i = m_startingCategoryIndex - 5; i < m_startingCategoryIndex; i++)
            {
                m_blacksmithUI.GetChildAt(i).Scale = Vector2.Zero;
            }

            // Tween out the visible equipment icons.
            for (int i = 0; i < m_activeIconArray.Length; i++)
            {
                Tween.To(m_activeIconArray[i], tweenSpeed, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            }

            // Tween out the menu background.
            Tween.To(m_blacksmithUI.GetChildAt(0), tweenSpeed, Back.EaseIn, "delay", "0.3", "ScaleX", "0", "ScaleY", "0");
            //Tween.AddEndHandlerToLastTween((ScreenManager as RCScreenManager), "HideCurrentScreen");
            Tween.RunFunction(tweenSpeed + 0.35f, ScreenManager, "HideCurrentScreen");
        }

        private void UpdateIconStates()
        {
            for (int i = 0; i < Game.PlayerStats.GetBlueprintArray.Count; i++)
            {
                for (int k = 0; k < Game.PlayerStats.GetBlueprintArray[i].Length; k++)
                {
                    byte state = Game.PlayerStats.GetBlueprintArray[i][k];
                    if (state == EquipmentState.NotFound)
                        m_masterIconArray[i][k].ChangeSprite("BlacksmithUI_QuestionMarkIcon_Character");
                    else
                    {
                        m_masterIconArray[i][k].ChangeSprite("BlacksmithUI_" + EquipmentCategoryType.ToStringEN(i) + ((k % 5) + 1).ToString() + "Icon_Character");
                        for (int j = 1; j < m_masterIconArray[i][k].NumChildren; j++)
                            m_masterIconArray[i][k].GetChildAt(j).Opacity = 0.2f;
                    }
                    
                    if (state > EquipmentState.FoundAndSeen)
                    {
                        for (int j = 1; j < m_masterIconArray[i][k].NumChildren; j++)
                            m_masterIconArray[i][k].GetChildAt(j).Opacity = 1f;

                        int colourIndex = 1;
                        if (i == EquipmentCategoryType.Sword)
                            colourIndex = 2;

                        EquipmentData equipmentData = Game.EquipmentSystem.GetEquipmentData(i, k);
                        m_masterIconArray[i][k].GetChildAt(colourIndex).TextureColor = equipmentData.FirstColour;

                        if (i != EquipmentCategoryType.Cape)
                        {
                            colourIndex++;
                            m_masterIconArray[i][k].GetChildAt(colourIndex).TextureColor = equipmentData.SecondColour;
                        }
                    }
                }
            }
        }

        private void UpdateNewIcons()
        {
            // Updates the player hud in case you equip the blood armor.
            if (Player != null)
            {
                if (Player.CurrentMana > Player.MaxMana)
                    Player.CurrentMana = Player.MaxMana;
                if (Player.CurrentHealth > Player.MaxHealth)
                    Player.CurrentHealth = Player.MaxHealth;
            }

            UpdateMoneyText();

            m_newIconListIndex = 0;
            foreach (SpriteObj sprite in m_newIconList)
                sprite.Visible = false;

            for (int i = 0; i < Game.PlayerStats.GetBlueprintArray[CurrentCategoryIndex].Length; i++)
            {
                byte state = Game.PlayerStats.GetBlueprintArray[CurrentCategoryIndex][i];

                if (state == EquipmentState.FoundButNotSeen)
                {
                    ObjContainer equipmentIcon = m_masterIconArray[CurrentCategoryIndex][i];

                    SpriteObj newIcon = m_newIconList[m_newIconListIndex];
                    newIcon.Visible = true;
                    newIcon.Position = m_masterIconArray[CurrentCategoryIndex][i].AbsPosition;
                    newIcon.X -= 20;
                    newIcon.Y -= 30;
                    m_newIconListIndex++;
                }
            }


            sbyte equippedItemIndex = Game.PlayerStats.GetEquippedArray[CurrentCategoryIndex];
            if (equippedItemIndex > -1)
            {
                m_equippedIcon.Position = new Vector2(m_activeIconArray[equippedItemIndex].AbsPosition.X + 18, m_activeIconArray[equippedItemIndex].AbsPosition.Y + 18);
                m_equippedIcon.Visible = true;
            }
            else
                m_equippedIcon.Visible = false;
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

            if (Game.PlayerStats.TotalBlueprintsFound >= (EquipmentBaseType.Total * EquipmentCategoryType.Total))
                GameUtil.UnlockAchievement("FEAR_OF_THROWING_STUFF_OUT");

            m_lockControls = true;
            SoundManager.PlaySound("ShopMenuOpen");

            m_confirmText.Opacity = 0;
            m_cancelText.Opacity = 0;
            m_navigationText.Opacity = 0;

            Tween.To(m_confirmText, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.To(m_cancelText, 0.2f, Linear.EaseNone, "Opacity", "1");
            Tween.To(m_navigationText, 0.2f, Linear.EaseNone, "Opacity", "1");

            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_8_NEW", m_confirmText);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_9_NEW", m_cancelText);

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_10", m_navigationText);
            else
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_11_NEW", m_navigationText);

            m_currentEquipmentIndex = 0;
            m_inCategoryMenu = true;
            m_selectionIcon.Position = m_blacksmithUI.GetChildAt(m_startingCategoryIndex).AbsPosition;
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

            for (int i = 0; i < m_blacksmithUI.NumChildren; i++)
                m_blacksmithUI.GetChildAt(i).Scale = Vector2.Zero;

            foreach (ObjContainer[] iconArray in m_masterIconArray)
            {
                for (int i = 0; i < iconArray.Length; i++)
                    iconArray[i].Scale = Vector2.Zero;
            }
            m_selectionIcon.Scale = Vector2.Zero;


            // Hack to make sure player's HP/MP goes up and down according to the item equipped.
            Player.CurrentHealth = Player.MaxHealth;
            Player.CurrentMana = Player.MaxMana;

            (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData, SaveType.UpgradeData);

            bool unlockAchievement = true;
            foreach (sbyte equipmentType in Game.PlayerStats.GetEquippedArray)
            {
                if (equipmentType == -1)
                {
                    unlockAchievement = false;
                    break;
                }
            }
            if (unlockAchievement == true)
                GameUtil.UnlockAchievement("FEAR_OF_NUDITY");

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

                m_selectionIcon.Position = m_blacksmithUI.GetChildAt(m_currentCategoryIndex).AbsPosition;
                for (int i = 1; i < 6; i++)
                {
                    if (i == 1)
                        m_blacksmithUI.GetChildAt(i).Scale = new Vector2(1, 1);
                    else
                        m_blacksmithUI.GetChildAt(i).Scale = Vector2.Zero;
                }

                if (m_currentCategoryIndex != m_startingCategoryIndex)
                    m_blacksmithUI.GetChildAt(m_currentCategoryIndex - 5).Scale = new Vector2(1, 1);
                else
                    m_blacksmithUI.GetChildAt(m_currentCategoryIndex - 5).Scale = Vector2.Zero;

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
                byte state = Game.PlayerStats.GetBlueprintArray[CurrentCategoryIndex][m_currentEquipmentIndex];
                if (state == EquipmentState.FoundButNotSeen)
                    Game.PlayerStats.GetBlueprintArray[CurrentCategoryIndex][m_currentEquipmentIndex] = EquipmentState.FoundAndSeen;
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
                m_currentEquipmentIndex -= 5;
                if (m_currentEquipmentIndex < 0)
                    m_currentEquipmentIndex += 15;
            }
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
            {
                m_currentEquipmentIndex += 5;
                if (m_currentEquipmentIndex > 14)
                    m_currentEquipmentIndex -= 15;
            }
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2))
            {
                m_currentEquipmentIndex--;
                if ((m_currentEquipmentIndex + 1) % 5 == 0)
                    m_currentEquipmentIndex += 5;
            }
            if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2))
            {
                m_currentEquipmentIndex++;
                if (m_currentEquipmentIndex % 5 == 0)
                    m_currentEquipmentIndex -= 5;
            }

            if (storedEquipmentIndex != m_currentEquipmentIndex)
            {
                byte state = Game.PlayerStats.GetBlueprintArray[CurrentCategoryIndex][m_currentEquipmentIndex];
                if (state == EquipmentState.FoundButNotSeen)
                    Game.PlayerStats.GetBlueprintArray[CurrentCategoryIndex][m_currentEquipmentIndex] = EquipmentState.FoundAndSeen;
                UpdateNewIcons();
                UpdateIconSelectionText();
                m_selectionIcon.Position = m_activeIconArray[m_currentEquipmentIndex].AbsPosition;
                SoundManager.PlaySound("ShopBSMenuMove");
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
            {
                SoundManager.PlaySound("ShopMenuCancel");
                m_inCategoryMenu = true;
                m_selectionIcon.Position = m_blacksmithUI.GetChildAt(m_currentCategoryIndex).AbsPosition;
                UpdateIconSelectionText();
            }

            if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
            {
                int equipmentCategory = m_currentCategoryIndex - m_startingCategoryIndex;
                int purchasedState = Game.PlayerStats.GetBlueprintArray[equipmentCategory][m_currentEquipmentIndex];
                int equippedIndex = Game.PlayerStats.GetEquippedArray[equipmentCategory];

                // Purchasing a previously locked piece of equipment.
                if (purchasedState < EquipmentState.Purchased && purchasedState > EquipmentState.NotFound)
                {
                    EquipmentData equipmentData = Game.EquipmentSystem.GetEquipmentData(equipmentCategory, m_currentEquipmentIndex);                    
                    if (Game.PlayerStats.Gold >= equipmentData.Cost)
                    {
                        SoundManager.PlaySound("ShopMenuUnlock");
                        Game.PlayerStats.Gold -= equipmentData.Cost;
                        UpdateMoneyText(); // flibit added this, purchases didn't update display!
                        Game.PlayerStats.GetBlueprintArray[equipmentCategory][m_currentEquipmentIndex] = EquipmentState.Purchased;

                        ObjContainer icon = m_masterIconArray[equipmentCategory][m_currentEquipmentIndex];
                        icon.ChangeSprite("BlacksmithUI_" + EquipmentCategoryType.ToStringEN(equipmentCategory) + ((m_currentEquipmentIndex % 5) + 1).ToString() + "Icon_Character");

                        for (int j = 1; j < icon.NumChildren; j++)
                            icon.GetChildAt(j).Opacity = 1f;

                        int colourIndex = 1;
                        if (equipmentCategory == EquipmentCategoryType.Sword)
                            colourIndex = 2;

                        icon.GetChildAt(colourIndex).TextureColor = equipmentData.FirstColour;

                        if (equipmentCategory != EquipmentCategoryType.Cape)
                        {
                            colourIndex++;
                            icon.GetChildAt(colourIndex).TextureColor = equipmentData.SecondColour;
                        }

                        purchasedState = EquipmentState.Purchased;
                        UpdateIconSelectionText();
                    }
                    else
                        SoundManager.PlaySound("ShopMenuUnlockFail");
                }

                // Changing the currently equipped equipment piece.
                if (equippedIndex != m_currentEquipmentIndex && purchasedState == EquipmentState.Purchased)
                {
                    EquipmentData equipmentData = Game.EquipmentSystem.GetEquipmentData(equipmentCategory, m_currentEquipmentIndex);
                    int currentEquippedItem = Game.PlayerStats.GetEquippedArray[equipmentCategory];
                    int weightReduction = 0;
                    if (currentEquippedItem != -1)
                        weightReduction = Game.EquipmentSystem.GetEquipmentData(equipmentCategory, currentEquippedItem).Weight;

                    if (equipmentData.Weight + Player.CurrentWeight - weightReduction <= Player.MaxWeight)
                    {
                        SoundManager.PlaySound("ShopBSEquip"); // this should be an equip sound.

                        Game.PlayerStats.GetEquippedArray[equipmentCategory] = (sbyte)m_currentEquipmentIndex;
                        UpdateIconSelectionText();
                        Vector3 playerPart = PlayerPart.GetPartIndices(equipmentCategory);
                     
                        if (playerPart.X != PlayerPart.None)
                            Player.GetChildAt((int)playerPart.X).TextureColor = equipmentData.FirstColour;
                        if (playerPart.Y != PlayerPart.None)
                            Player.GetChildAt((int)playerPart.Y).TextureColor = equipmentData.SecondColour;
                        if (playerPart.Z != PlayerPart.None)
                            Player.GetChildAt((int)playerPart.Z).TextureColor = equipmentData.SecondColour;

                        // Special handling to tint the female's boobs.
                        if (equipmentCategory == EquipmentCategoryType.Chest && playerPart.X != PlayerPart.None)
                            Player.GetChildAt(PlayerPart.Boobs).TextureColor = equipmentData.FirstColour;

                        UpdateNewIcons();
                    }
                    else
                        Console.WriteLine("cannot equip. too heavy. Weight:" + (equipmentData.Weight + Player.CurrentWeight - weightReduction));
                }
                else if (equippedIndex == m_currentEquipmentIndex) // Unequipping
                {
                    Game.PlayerStats.GetEquippedArray[equipmentCategory] = -1;
                    Player.UpdateEquipmentColours();
                    UpdateIconSelectionText();
                    UpdateNewIcons();
                }
            }
        }

        private void UpdateIconSelectionText()
        {
            m_equipmentDescriptionText.Position = new Vector2(-1000, -1000);
            m_textInfoTitleContainer.Visible = false;
            m_textInfoStatContainer.Visible = false;
            m_textInfoStatModContainer.Visible = false;
            m_addPropertiesTitleText.Visible = false;
            m_addPropertiesText.Visible = false;
            m_unlockCostContainer.Visible = false;
            m_equipmentTitleText.Visible = false;

            if (m_inCategoryMenu == true)
            {
                m_equipmentDescriptionText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_12", m_equipmentDescriptionText);
            }
            else
            {
                if (Game.PlayerStats.GetBlueprintArray[m_currentCategoryIndex - m_startingCategoryIndex][m_currentEquipmentIndex] == EquipmentState.NotFound)
                {
                    m_equipmentDescriptionText.Position = new Vector2(230, -20);
                    m_equipmentDescriptionText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_13", m_equipmentDescriptionText);
                }
                else if (Game.PlayerStats.GetBlueprintArray[m_currentCategoryIndex - m_startingCategoryIndex][m_currentEquipmentIndex] < EquipmentState.Purchased)
                {
                    m_equipmentDescriptionText.Text = LocaleBuilder.getString("LOC_ID_BLACKSMITH_SCREEN_14", m_equipmentDescriptionText);
                    (m_unlockCostContainer.GetChildAt(1) as TextObj).Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_16_NEW"), Game.EquipmentSystem.GetEquipmentData(m_currentCategoryIndex - m_startingCategoryIndex, m_currentEquipmentIndex).Cost.ToString());
                    //(m_unlockCostContainer.GetChildAt(1) as TextObj).Text = Game.EquipmentSystem.GetEquipmentData(m_currentCategoryIndex - m_startingCategoryIndex, m_currentEquipmentIndex).Cost.ToString() + " " + LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_16");
                    
                    m_unlockCostContainer.Visible = true;
                    m_textInfoTitleContainer.Visible = true;
                    m_textInfoStatContainer.Visible = true;
                    m_textInfoStatModContainer.Visible = true;
                    m_addPropertiesTitleText.Visible = true;
                    m_addPropertiesText.Visible = true;
                    m_equipmentTitleText.Visible = true;

                    m_textInfoTitleContainer.Opacity = 0.5f;
                    m_textInfoStatContainer.Opacity = 0.5f;
                    m_textInfoStatModContainer.Opacity = 0.5f;
                    m_addPropertiesTitleText.Opacity = 0.5f;
                    m_addPropertiesText.Opacity = 0.5f;
                    m_equipmentTitleText.Opacity = 0.5f;

                    UpdateEquipmentDataText();
                }
                else
                {
                    // This code displays all the information for the stats.
                    m_textInfoTitleContainer.Visible = true;
                    m_textInfoStatContainer.Visible = true;
                    m_textInfoStatModContainer.Visible = true;
                    m_addPropertiesTitleText.Visible = true;
                    m_addPropertiesText.Visible = true;
                    m_equipmentTitleText.Visible = true;

                    m_textInfoTitleContainer.Opacity = 1;
                    m_textInfoStatContainer.Opacity = 1;
                    m_textInfoStatModContainer.Opacity = 1;
                    m_addPropertiesTitleText.Opacity = 1;
                    m_addPropertiesText.Opacity = 1;
                    m_equipmentTitleText.Opacity = 1;

                    UpdateEquipmentDataText();
                }
            }
        }

        // Updates the base player stats text and the bonus attribute text that a piece of equipment gives you.
        private void UpdateEquipmentDataText()
        {
            (m_textInfoStatContainer.GetChildAt(0) as TextObj).Text = Player.MaxHealth.ToString();
            (m_textInfoStatContainer.GetChildAt(1) as TextObj).Text = Player.MaxMana.ToString();
            (m_textInfoStatContainer.GetChildAt(2) as TextObj).Text = Player.Damage.ToString();
            (m_textInfoStatContainer.GetChildAt(3) as TextObj).Text = Player.TotalMagicDamage.ToString();
            (m_textInfoStatContainer.GetChildAt(4) as TextObj).Text = Player.TotalArmor.ToString();
            (m_textInfoStatContainer.GetChildAt(5) as TextObj).Text = Player.CurrentWeight.ToString() + "/" + Player.MaxWeight.ToString();

            int selectedCategoryIndex = m_currentCategoryIndex - m_startingCategoryIndex;
            EquipmentData equipmentData = Game.EquipmentSystem.GetEquipmentData(selectedCategoryIndex, m_currentEquipmentIndex);
            int currentEquippedIndex = Game.PlayerStats.GetEquippedArray[selectedCategoryIndex];
            EquipmentData currentEquipmentData = new EquipmentData();
            if (currentEquippedIndex > -1)
                currentEquipmentData = Game.EquipmentSystem.GetEquipmentData(selectedCategoryIndex, currentEquippedIndex);

            //if (Game.PlayerStats.GetEquippedArray[m_currentCategoryIndex - m_startingCategoryIndex] != m_currentEquipmentIndex)
            //{

            bool isEquippedItem = Game.PlayerStats.GetEquippedArray[CurrentCategoryIndex] == m_currentEquipmentIndex;

            int bonusHealth = equipmentData.BonusHealth - currentEquipmentData.BonusHealth;

            // In case we want equipment to accurately reflect class multiplier effects.
            //int bonusHealth = (int)Math.Round((equipmentData.BonusHealth - currentEquipmentData.BonusHealth) * Player.ClassTotalHPMultiplier, MidpointRounding.AwayFromZero);

            if (isEquippedItem == true)
                bonusHealth = -equipmentData.BonusHealth;
            TextObj textObj1 = m_textInfoStatModContainer.GetChildAt(0) as TextObj;

            if (bonusHealth > 0)
            {
                textObj1.TextureColor = Color.Cyan;
                textObj1.Text = "+" + bonusHealth.ToString();
            }
            else if (bonusHealth < 0)
            {
                textObj1.TextureColor = Color.Red;
                textObj1.Text = bonusHealth.ToString();
            }
            else
                textObj1.Text = "";

            TextObj textObj2 = m_textInfoStatModContainer.GetChildAt(1) as TextObj;
            int bonusMana = equipmentData.BonusMana - currentEquipmentData.BonusMana;
            if (isEquippedItem == true)
                bonusMana = -equipmentData.BonusMana;
            if (bonusMana > 0)
            {
                textObj2.TextureColor = Color.Cyan;
                textObj2.Text = "+" + bonusMana.ToString();
            }
            else if (bonusMana < 0)
            {
                textObj2.TextureColor = Color.Red;
                textObj2.Text = bonusMana.ToString();
            }
            else
                textObj2.Text = "";

            TextObj textObj3 = m_textInfoStatModContainer.GetChildAt(2) as TextObj;
            int bonusDamage = equipmentData.BonusDamage - currentEquipmentData.BonusDamage;
            if (isEquippedItem == true)
                bonusDamage = -equipmentData.BonusDamage;
            if (bonusDamage > 0)
            {
                textObj3.TextureColor = Color.Cyan;
                textObj3.Text = "+" + bonusDamage.ToString();
            }
            else if (bonusDamage < 0)
            {
                textObj3.TextureColor = Color.Red;
                textObj3.Text = bonusDamage.ToString();
            }
            else
                textObj3.Text = "";

            TextObj textObj4 = m_textInfoStatModContainer.GetChildAt(3) as TextObj;
            int bonusMagic = equipmentData.BonusMagic - currentEquipmentData.BonusMagic;
            if (isEquippedItem == true)
                bonusMagic = -equipmentData.BonusMagic;
            if (bonusMagic > 0)
            {
                textObj4.TextureColor = Color.Cyan;
                textObj4.Text = "+" + bonusMagic.ToString();
            }
            else if (bonusMagic < 0)
            {
                textObj4.TextureColor = Color.Red;
                textObj4.Text = bonusMagic.ToString();
            }
            else
                textObj4.Text = "";

            TextObj textObj5 = m_textInfoStatModContainer.GetChildAt(4) as TextObj;
            int bonusArmor = equipmentData.BonusArmor - currentEquipmentData.BonusArmor;
            if (isEquippedItem == true)
                bonusArmor = -equipmentData.BonusArmor;
            if (bonusArmor > 0)
            {
                textObj5.TextureColor = Color.Cyan;
                textObj5.Text = "+" + bonusArmor.ToString();
            }
            else if (bonusArmor < 0)
            {
                textObj5.TextureColor = Color.Red;
                textObj5.Text = bonusArmor.ToString();
            }
            else
                textObj5.Text = "";

            TextObj textObj6 = m_textInfoStatModContainer.GetChildAt(5) as TextObj;
            int weight = equipmentData.Weight - currentEquipmentData.Weight;
            if (isEquippedItem == true)
                weight = -equipmentData.Weight;
            if (weight > 0)
            {
                textObj6.TextureColor = Color.Red;
                textObj6.Text = "+" + weight.ToString();
            }
            else if (weight < 0)
            {
                textObj6.TextureColor = Color.Cyan;
                textObj6.Text = weight.ToString();
            }
            else
                textObj6.Text = "";
            //}
            //else
            //{
            //    for (int i = 0; i < 5; i++)
            //        (m_textInfoStatModContainer.GetChildAt(i) as TextObj).Text = "";
            //}

            Vector2[] specialAttributeArray = equipmentData.SecondaryAttribute;
            m_addPropertiesText.Text = "";
            if (specialAttributeArray != null)
            {
                foreach (Vector2 attrib in specialAttributeArray)
                {
                    if (attrib.X != 0)
                    {
                        if (attrib.X < 7) // Temp hack to get stats displaying percentages properly.
                            m_addPropertiesText.Text += "+" + (attrib.Y * 100).ToString() + "% " + LocaleBuilder.getResourceString(EquipmentSecondaryDataType.ToStringID((int)attrib.X)) + "\n";
                        else
                            m_addPropertiesText.Text += "+" + attrib.Y.ToString() + " " + LocaleBuilder.getResourceString(EquipmentSecondaryDataType.ToStringID((int)attrib.X)) + "\n";
                    }
                }
                if (specialAttributeArray.Length == 0)
                    m_addPropertiesText.Text = LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_15"); // "None"
            }
            else
                m_addPropertiesText.Text = LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_15"); // "None"

            //m_equipmentTitleText.Text = LocaleBuilder.getResourceString(EquipmentBaseType.ToStringID(m_currentEquipmentIndex)) + " " + LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID(selectedCategoryIndex));
            m_equipmentTitleText.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentBaseType.ToStringID(m_currentEquipmentIndex), true), LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID(selectedCategoryIndex), true));
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
            m_blacksmithUI.Draw(Camera);
            m_selectionIcon.Draw(Camera);

            //m_textInfoContainer.DrawOutline(Camera, 2);
            m_textInfoTitleContainer.Draw(Camera);
            m_textInfoStatContainer.Draw(Camera);
            m_textInfoStatModContainer.Draw(Camera);
            m_addPropertiesTitleText.Draw(Camera);
            m_addPropertiesText.Draw(Camera);
            m_unlockCostContainer.Draw(Camera);
            m_equipmentTitleText.Draw(Camera);

            foreach (ObjContainer[] iconArray in m_masterIconArray)
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
                Console.WriteLine("Disposing Blacksmith Screen");

                if (m_rainSound != null)
                    m_rainSound.Dispose();
                m_rainSound = null;

                m_blacksmithUI.Dispose();
                m_blacksmithUI = null;
                m_equipmentDescriptionText.Dispose();
                m_equipmentDescriptionText = null;
                m_selectionIcon.Dispose();
                m_selectionIcon = null;
                m_equipmentTitleText.Dispose();
                m_equipmentTitleText = null;

                m_activeIconArray = null;

                foreach (ObjContainer[] iconArray in m_masterIconArray)
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

                m_textInfoStatContainer.Dispose();
                m_textInfoStatContainer = null;
                m_textInfoTitleContainer.Dispose();
                m_textInfoTitleContainer = null;
                m_textInfoStatModContainer.Dispose();
                m_textInfoStatModContainer = null;
                m_unlockCostContainer.Dispose();
                m_unlockCostContainer = null;
                m_addPropertiesText.Dispose();
                m_addPropertiesText = null;
                m_addPropertiesTitleText.Dispose();
                m_addPropertiesTitleText = null;
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
            /*
            m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "]  " + LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_8");
            m_cancelText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "]  " + LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_9");

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == false)
                m_navigationText.Text = LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_10");
            else
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_BLACKSMITH_SCREEN_11");
             */

            m_addPropertiesTitleText.ScaleX = 1;
            (m_unlockCostContainer.GetChildAt(1) as TextObj).ScaleX = 1;
            switch(LocaleBuilder.languageType)
            {
                case(LanguageType.French):
                    (m_unlockCostContainer.GetChildAt(1) as TextObj).ScaleX = 0.9f;
                    m_addPropertiesTitleText.ScaleX = 0.9f;
                    break;
                case(LanguageType.Spanish_Spain):
                case(LanguageType.German):
                case(LanguageType.Polish):
                    (m_unlockCostContainer.GetChildAt(1) as TextObj).ScaleX = 0.9f;
                    break;
            }

            Game.ChangeBitmapLanguage(m_blacksmithUI.GetChildAt(m_blacksmithUI.NumChildren - 4) as SpriteObj, "BlacksmithUI_Title_Sprite");
            foreach (SpriteObj icon in m_newIconList)
                Game.ChangeBitmapLanguage(icon, "BlacksmithUI_NewIcon_Sprite");

            UpdateIconSelectionText();
            UpdateEquipmentDataText();
            base.RefreshTextObjs();
        }
    }
}

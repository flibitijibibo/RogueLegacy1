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
using System.Text.RegularExpressions;

namespace RogueCastle
{
    public class ProfileCardScreen : Screen
    {
        public float BackBufferOpacity { get; set; }
        private ObjContainer m_frontCard;
        private ObjContainer m_backCard;

        private PlayerHUDObj m_playerHUD;

        // Front card objects.
        private TextObj m_playerName;
        private TextObj m_money;
        private TextObj m_levelClass;
        private SpriteObj m_playerBG;
        private TextObj m_frontTrait1;
        private TextObj m_frontTrait2;
        private TextObj m_author;
        private TextObj m_playerStats;
        private SpriteObj m_spellIcon;
        private TextObj m_classDescription;

        // Back card objects.
        private List<TextObj> m_dataList1, m_dataList2;
        private TextObj m_equipmentTitle;
        private TextObj m_runesTitle;
        private List<TextObj> m_equipmentList;
        private List<TextObj> m_runeBackTitleList;
        private List<TextObj> m_runeBackDescriptionList;

        private ObjContainer m_playerSprite;
        private SpriteObj m_tombStoneSprite;
        private bool m_playerInAir = false;

        private Color m_skinColour1 = new Color(231, 175, 131, 255);
        private Color m_skinColour2 = new Color(199, 109, 112, 255);
        private Color m_lichColour1 = new Color(255, 255, 255, 255);
        private Color m_lichColour2 = new Color(198, 198, 198, 255);

        private KeyIconTextObj m_cancelText;

        public ProfileCardScreen()
        {
            m_equipmentList = new List<TextObj>();
            m_runeBackTitleList = new List<TextObj>();
            m_runeBackDescriptionList = new List<TextObj>();
        }

        public override void LoadContent()
        {
            m_frontCard = new ObjContainer("CardFront_Character");
            m_frontCard.ForceDraw = true;
            m_frontCard.Position = new Vector2(145, 30);
            m_frontCard.GetChildAt(0).TextureColor = Color.Red; // BG
            m_frontCard.GetChildAt(2).TextureColor = Color.Red; // Border
            LoadFrontCard();            

            m_backCard = new ObjContainer("CardBack_Character");
            m_backCard.ForceDraw = true;
            m_backCard.Position = new Vector2(m_frontCard.X + m_backCard.Width + 100, m_frontCard.Y);
            m_backCard.AddChild(m_playerName.Clone() as GameObj);
            m_backCard.GetChildAt(0).TextureColor = Color.Red; // BG
            m_backCard.GetChildAt(2).TextureColor = Color.Red; // Border
            LoadBackCard();

            m_playerSprite = new ObjContainer("PlayerIdle_Character");
            m_playerSprite.ForceDraw = true;
            m_playerSprite.Scale = new Vector2(2, 2);
            m_playerSprite.OutlineWidth = 2;

            m_tombStoneSprite = new SpriteObj("Tombstone_Sprite");
            m_tombStoneSprite.ForceDraw = true;
            m_tombStoneSprite.Scale = new Vector2(3, 3);
            m_tombStoneSprite.OutlineWidth = 2;

            m_spellIcon = new SpriteObj(SpellType.Icon(12));
            m_spellIcon.Position = new Vector2(350, 295);
            m_spellIcon.OutlineWidth = 2;
            m_spellIcon.ForceDraw = true;

            m_cancelText = new KeyIconTextObj(Game.JunicodeFont);
            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_cancelText); // dummy locID to add TextObj to language refresh list
            m_cancelText.Align = Types.TextAlign.Right;
            m_cancelText.DropShadow = new Vector2(2, 2);
            m_cancelText.FontSize = 12;
            m_cancelText.Position = new Vector2(1290, 650);
            m_cancelText.ForceDraw = true;

            base.LoadContent();
        }

        private void LoadFrontCard()
        {
            TextObj templateText = new TextObj(Game.JunicodeFont);
            templateText.Text = "";
            templateText.FontSize = 10;
            templateText.ForceDraw = true;
            templateText.TextureColor = Color.Black;
            //templateText.DropShadow = new Vector2(2, 2);

            m_playerName = templateText.Clone() as TextObj;
            m_playerName.Text = "Sir Archibald the IV";
            m_playerName.Position = new Vector2(50, 43);
            m_frontCard.AddChild(m_playerName);

            m_money = templateText.Clone() as TextObj;
            m_money.Position = new Vector2(m_frontCard.GetChildAt(3).X + 30, m_playerName.Y); // Child at index 3 is the coin icon.
            m_money.Text = "0";
            m_frontCard.AddChild(m_money);

            m_levelClass = templateText.Clone() as TextObj;
            m_levelClass.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_levelClass); // dummy locID to add TextObj to language refresh list
            //m_levelClass.Position = new Vector2(m_playerName.X, 370);
            m_levelClass.Position = new Vector2(m_playerName.X, 260);
            m_frontCard.AddChild(m_levelClass);

            m_playerBG = new SpriteObj("CardDungeonBG_Sprite");
            //m_playerBG.Position = new Vector2(45, 80);
            m_playerBG.Position = new Vector2(45, 220 + 80);
            m_frontCard.AddChildAt(1, m_playerBG);

            m_playerHUD = new PlayerHUDObj();
            m_playerHUD.ForceDraw = true;
            m_playerHUD.ShowBarsOnly = true;
            //m_playerHUD.SetPosition(new Vector2(m_frontCard.X + 46, m_frontCard.Y + 64));
            m_playerHUD.SetPosition(new Vector2(m_frontCard.X + 46, m_frontCard.Y + 220 + 64));
            m_frontCard.AddChild(m_playerHUD);

            // Loading front card stats.

            m_frontTrait1 = new TextObj(Game.JunicodeFont);
            m_frontTrait1.FontSize = 7;
            m_frontTrait1.TextureColor = Color.Black;
            //m_frontTrait1.Position = new Vector2(50, 550);
            m_frontTrait1.Position = new Vector2(50, 550 - 320);
            m_frontTrait1.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_frontTrait1); // dummy locID to add TextObj to language refresh list
            m_frontCard.AddChild(m_frontTrait1);

            m_frontTrait2 = m_frontTrait1.Clone() as TextObj;
            m_frontTrait2.Y -= 20;
            m_frontTrait2.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_frontTrait2); // dummy locID to add TextObj to language refresh list
            m_frontCard.AddChild(m_frontTrait2);

            m_classDescription = new TextObj(Game.JunicodeFont);
            m_classDescription.FontSize = 8;
            m_classDescription.TextureColor = Color.Black;
            m_classDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_classDescription); // dummy locID to add TextObj to language refresh list
            //m_classDescription.Position = new Vector2(50, 410);
            m_classDescription.Position = new Vector2(50, 410 - 320);
            m_frontCard.AddChild(m_classDescription);

            m_author = new TextObj(Game.JunicodeFont);
            m_author.FontSize = 8;
            m_author.TextureColor = Color.White;
            m_author.Text = "Glauber Kotaki";
            m_author.X = m_playerName.X;
            m_author.Y = 590;
            //m_frontCard.AddChild(m_author);

            m_playerStats = templateText.Clone() as TextObj;
            m_playerStats.Text = "10/10";
            m_playerStats.Align = Types.TextAlign.Centre;
            m_playerStats.Position = new Vector2(387, 579);
            //m_frontCard.AddChild(m_playerStats);
        }

        private void LoadBackCard()
        {
            TextObj templateText = new TextObj(Game.JunicodeFont);
            templateText.Text = "";
            templateText.FontSize = 9;
            templateText.ForceDraw = true;
            templateText.TextureColor = Color.Black;

            m_dataList1 = new List<TextObj>();
            m_dataList2 = new List<TextObj>();

            string[] list1LocIds = new string[] { "LOC_ID_PROFILE_CARD_SCREEN_1", "LOC_ID_PROFILE_CARD_SCREEN_2", "LOC_ID_PROFILE_CARD_SCREEN_3", "LOC_ID_PROFILE_CARD_SCREEN_4" };
            string[] list2LocIds = new string[] { "LOC_ID_PROFILE_CARD_SCREEN_5", "LOC_ID_PROFILE_CARD_SCREEN_6", "LOC_ID_PROFILE_CARD_SCREEN_7", "LOC_ID_PROFILE_CARD_SCREEN_8" };

            int startingY = 90;
            for (int i = 0; i < list1LocIds.Length; i++)
            {
                TextObj listTitle = templateText.Clone() as TextObj;
                listTitle.Align = Types.TextAlign.Right;
                listTitle.Text = LocaleBuilder.getString(list1LocIds[i], listTitle);
                listTitle.Position = new Vector2(120, startingY);
                m_backCard.AddChild(listTitle);

                TextObj listData = templateText.Clone() as TextObj;
                listData.Text = "0";
                listData.Position = new Vector2(listTitle.X + 15, startingY);
                m_dataList1.Add(listData);
                m_backCard.AddChild(listData);

                TextObj listTitle2 = templateText.Clone() as TextObj;
                listTitle2.Align = Types.TextAlign.Right;
                listTitle2.Text = LocaleBuilder.getString(list2LocIds[i], listTitle2);
                listTitle2.Position = new Vector2(350, startingY);
                m_backCard.AddChild(listTitle2);

                TextObj listData2 = templateText.Clone() as TextObj;
                listData2.Text = "0";
                listData2.Position = new Vector2(listTitle2.X + 15, startingY);
                m_dataList2.Add(listData2);
                m_backCard.AddChild(listData2);

                startingY += 20;
            }

            m_equipmentTitle = templateText.Clone() as TextObj;
            m_equipmentTitle.FontSize = 12;
            m_equipmentTitle.Text = LocaleBuilder.getString("LOC_ID_PROFILE_CARD_SCREEN_9", m_equipmentTitle) + ":";
            m_equipmentTitle.Position = new Vector2(50, 180);
            m_backCard.AddChild(m_equipmentTitle);

            m_runesTitle = templateText.Clone() as TextObj;
            m_runesTitle.FontSize = 12;
            m_runesTitle.Text = LocaleBuilder.getString("LOC_ID_PROFILE_CARD_SCREEN_10", m_runesTitle) + ":";
            m_runesTitle.Position = new Vector2(m_equipmentTitle.X, 330);
            m_backCard.AddChild(m_runesTitle);

            for (int i = 0; i < Game.PlayerStats.GetEquippedArray.Length; i++)
            {
                TextObj equipment = templateText.Clone() as TextObj;
                equipment.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", equipment); // dummy locID to add TextObj to language refresh list
                equipment.Position = new Vector2(80, m_equipmentTitle.Y + 50);
                m_equipmentList.Add(equipment);
                m_backCard.AddChild(equipment);
            }

            // Loading runes

            for (int i = 0; i < EquipmentAbilityType.Total - 1; i++) // -1 because the last one is ManaHPGain, which don't display since its shown in Vamp and ManaGain seperately.
            {
                TextObj runeTitle = templateText.Clone() as TextObj;
                runeTitle.X = 60;
                runeTitle.Text = LocaleBuilder.getString(EquipmentAbilityType.ToStringID2(i), runeTitle);
                runeTitle.FontSize = 7;
                m_runeBackTitleList.Add(runeTitle);
                m_backCard.AddChild(runeTitle);

                TextObj runeDescription = templateText.Clone() as TextObj;
                runeDescription.X = runeTitle.Bounds.Right + 10;
                runeDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", runeDescription); // dummy locID to add TextObj to language refresh list
                runeDescription.FontSize = 7;
                m_runeBackDescriptionList.Add(runeDescription);
                m_backCard.AddChild(runeDescription);
            }
            
            // Special architect's fee rune.
            TextObj architectFeeTitle = templateText.Clone() as TextObj;
            architectFeeTitle.X = 60;
            architectFeeTitle.Text = LocaleBuilder.getString(EquipmentAbilityType.ToStringID2(EquipmentAbilityType.ArchitectFee), architectFeeTitle);
            architectFeeTitle.FontSize = 7;
            m_runeBackTitleList.Add(architectFeeTitle);
            m_backCard.AddChild(architectFeeTitle);

            TextObj architectFeeDescription = templateText.Clone() as TextObj;
            architectFeeDescription.X = architectFeeTitle.Bounds.Right + 10;
            architectFeeDescription.FontSize = 7;
            m_runeBackDescriptionList.Add(architectFeeDescription);
            m_backCard.AddChild(architectFeeDescription);

            // Special architect's fee rune.
            TextObj newGamePlusGoldTitle = templateText.Clone() as TextObj;
            newGamePlusGoldTitle.X = 60;
            newGamePlusGoldTitle.Text = LocaleBuilder.getString(EquipmentAbilityType.ToStringID2(EquipmentAbilityType.NewGamePlusGoldBonus), newGamePlusGoldTitle);
            newGamePlusGoldTitle.FontSize = 7;
            m_runeBackTitleList.Add(newGamePlusGoldTitle);
            m_backCard.AddChild(newGamePlusGoldTitle);

            TextObj newGamePlusGoldDescription = templateText.Clone() as TextObj;
            newGamePlusGoldDescription.X = newGamePlusGoldTitle.Bounds.Right + 10;
            newGamePlusGoldDescription.FontSize = 7;
            m_runeBackDescriptionList.Add(newGamePlusGoldDescription);
            m_backCard.AddChild(newGamePlusGoldDescription);
        }

        public override void OnEnter()
        {
            m_classDebugCounter = 0;
            m_traitsDebugCounter = Vector2.Zero;

            SoundManager.PlaySound("StatCard_In");
            LoadCardColour();

            m_spellIcon.ChangeSprite(SpellType.Icon(Game.PlayerStats.Spell));
            string[] randBG = new string[] { "CardCastleBG_Sprite", "CardGardenBG_Sprite", "CardDungeonBG_Sprite", "CardTowerBG_Sprite" };
            m_playerBG.ChangeSprite(randBG[CDGMath.RandomInt(0, 3)]);
            randBG = null;

            m_frontCard.Y = 1500;
            m_backCard.Y = 1500;
            Tween.To(this, 0.2f, Tween.EaseNone, "BackBufferOpacity", "0.7");
            Tween.To(m_frontCard, 0.4f, Back.EaseOut, "Y", "30");
            Tween.To(m_backCard, 0.4f, Back.EaseOut, "delay", "0.2", "Y", "30");

            PlayerObj player = (ScreenManager as RCScreenManager).Player;
            LoadFrontCardStats(player);
            LoadBackCardStats(player);

            ChangeParts(player);

            m_playerHUD.Update(player);

            if (InputManager.GamePadIsConnected(PlayerIndex.One))
                m_cancelText.ForcedScale = new Vector2(0.7f, 0.7f);
            else
                m_cancelText.ForcedScale = new Vector2(1f, 1f);

            m_cancelText.Text = LocaleBuilder.getString("LOC_ID_PROFILE_CARD_SCREEN_11_NEW", m_cancelText);
            //m_cancelText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "] " + LocaleBuilder.getString("LOC_ID_PROFILE_CARD_SCREEN_11", m_cancelText);
            m_cancelText.Opacity = 0;
            Tween.To(m_cancelText, 0.2f, Tween.EaseNone, "Opacity", "1");

            string[] authorNames = new string[] { "Glauber Kotaki", "Kenny Lee", "Teddy Lee", "Gordon McGladdery", "Judson Cowan" };
            m_author.Text = authorNames[CDGMath.RandomInt(0, authorNames.Length - 1)];
            Array.Clear(authorNames, 0, authorNames.Length);

            base.OnEnter();
        }

        private void ChangeParts(PlayerObj player)
        {
            // Player style must be set before changing his actual parts, otherwise the parts will reset to their defaults (1).

            // Attack animations must be removed for dragon class.
            string[] playerGroundActions = null;
            if (Game.PlayerStats.Class == ClassType.Dragon)
                playerGroundActions = new string[] { "Idle", "Walking", "LevelUp", "Dash", "FrontDash" };
            else
                playerGroundActions = new string[] { "Idle", "Attacking3", "Walking", "LevelUp", "Dash", "FrontDash" };

            string[] playerAirActions = null;
            if (Game.PlayerStats.Class == ClassType.Dragon)
                playerAirActions = new string[] { "Jumping", "Falling" };
            else
                playerAirActions = new string[] { "Jumping", "AirAttack", "Falling" };

            if (CDGMath.RandomInt(0, 1) == 0)
            {
                m_playerInAir = true;
                SetPlayerStyle(playerAirActions[CDGMath.RandomInt(0, playerAirActions.Length - 1)]);
            }
            else
            {
                m_playerInAir = false;
                SetPlayerStyle(playerGroundActions[CDGMath.RandomInt(0, playerGroundActions.Length - 1)]);
            }

            for (int i = 0; i < player.NumChildren; i++)
            {
                SpriteObj playerPart = player.GetChildAt(i) as SpriteObj;
                SpriteObj playerSpritePart = m_playerSprite.GetChildAt(i) as SpriteObj;
                playerSpritePart.TextureColor = playerPart.TextureColor;
            }

            string headPart = (m_playerSprite.GetChildAt(PlayerPart.Head) as IAnimateableObj).SpriteName;
            int numberIndex = headPart.IndexOf("_") - 1;
            headPart = headPart.Remove(numberIndex, 1);
            if (Game.PlayerStats.Class == ClassType.Dragon)
                headPart = headPart.Replace("_", PlayerPart.DragonHelm + "_");
            else if (Game.PlayerStats.Class == ClassType.Traitor)
                headPart = headPart.Replace("_", PlayerPart.IntroHelm + "_");
            else
                headPart = headPart.Replace("_", Game.PlayerStats.HeadPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.Head).ChangeSprite(headPart);

            string chestPart = (m_playerSprite.GetChildAt(PlayerPart.Chest) as IAnimateableObj).SpriteName;
            numberIndex = chestPart.IndexOf("_") - 1;
            chestPart = chestPart.Remove(numberIndex, 1);
            chestPart = chestPart.Replace("_", Game.PlayerStats.ChestPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.Chest).ChangeSprite(chestPart);

            string shoulderAPart = (m_playerSprite.GetChildAt(PlayerPart.ShoulderA) as IAnimateableObj).SpriteName;
            numberIndex = shoulderAPart.IndexOf("_") - 1;
            shoulderAPart = shoulderAPart.Remove(numberIndex, 1);
            shoulderAPart = shoulderAPart.Replace("_", Game.PlayerStats.ShoulderPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.ShoulderA).ChangeSprite(shoulderAPart);

            string shoulderBPart = (m_playerSprite.GetChildAt(PlayerPart.ShoulderB) as IAnimateableObj).SpriteName;
            numberIndex = shoulderBPart.IndexOf("_") - 1;
            shoulderBPart = shoulderBPart.Remove(numberIndex, 1);
            shoulderBPart = shoulderBPart.Replace("_", Game.PlayerStats.ShoulderPiece + "_");
            m_playerSprite.GetChildAt(PlayerPart.ShoulderB).ChangeSprite(shoulderBPart);
        }

        public void SetPlayerStyle(string animationType)
        {
            m_playerSprite.ChangeSprite("Player" + animationType + "_Character");

            PlayerObj player = (ScreenManager as RCScreenManager).Player;
            for (int i = 0; i < m_playerSprite.NumChildren; i++)
            {
                m_playerSprite.GetChildAt(i).TextureColor = player.GetChildAt(i).TextureColor;
                m_playerSprite.GetChildAt(i).Visible = player.GetChildAt(i).Visible;
            }
            m_playerSprite.GetChildAt(PlayerPart.Light).Visible = false;
            m_playerSprite.Scale = player.Scale;

            if (Game.PlayerStats.Traits.X == TraitType.Baldness || Game.PlayerStats.Traits.Y == TraitType.Baldness)
                m_playerSprite.GetChildAt(PlayerPart.Hair).Visible = false;

            m_playerSprite.GetChildAt(PlayerPart.Glasses).Visible = false;
            if (Game.PlayerStats.SpecialItem == SpecialItemType.Glasses)
                m_playerSprite.GetChildAt(PlayerPart.Glasses).Visible = true;

            if (Game.PlayerStats.Class == ClassType.Knight || Game.PlayerStats.Class == ClassType.Knight2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("Player" + animationType + "Shield_Sprite");
            }
            else if (Game.PlayerStats.Class == ClassType.Banker || Game.PlayerStats.Class == ClassType.Banker2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("Player" + animationType + "Lamp_Sprite");
            }
            else if (Game.PlayerStats.Class == ClassType.Wizard || Game.PlayerStats.Class == ClassType.Wizard2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("Player" + animationType + "Beard_Sprite");
            }
            else if (Game.PlayerStats.Class == ClassType.Ninja || Game.PlayerStats.Class == ClassType.Ninja2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("Player" + animationType + "Headband_Sprite");
            }
            else if (Game.PlayerStats.Class == ClassType.Barbarian || Game.PlayerStats.Class == ClassType.Barbarian2)
            {
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Extra).ChangeSprite("Player" + animationType + "Horns_Sprite");
            }
            else
                m_playerSprite.GetChildAt(PlayerPart.Extra).Visible = false;

            // Special code for dragon.
            m_playerSprite.GetChildAt(PlayerPart.Wings).Visible = false;
            if (Game.PlayerStats.Class == ClassType.Dragon)
            {
                m_playerSprite.GetChildAt(PlayerPart.Wings).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Head).ChangeSprite("Player" + animationType + "Head" + PlayerPart.DragonHelm + "_Sprite");
            }

            // This is for male/female counterparts
            if (Game.PlayerStats.IsFemale == false)
            {
                m_playerSprite.GetChildAt(PlayerPart.Boobs).Visible = false;
                m_playerSprite.GetChildAt(PlayerPart.Bowtie).Visible = false;
            }
            else
            {
                m_playerSprite.GetChildAt(PlayerPart.Boobs).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Bowtie).Visible = true;
            }

            if (Game.PlayerStats.Traits.X == TraitType.Gigantism || Game.PlayerStats.Traits.Y == TraitType.Gigantism)
                m_playerSprite.Scale = new Vector2(GameEV.TRAIT_GIGANTISM, GameEV.TRAIT_GIGANTISM);
            if (Game.PlayerStats.Traits.X == TraitType.Dwarfism || Game.PlayerStats.Traits.Y == TraitType.Dwarfism)
                m_playerSprite.Scale = new Vector2(GameEV.TRAIT_DWARFISM, GameEV.TRAIT_DWARFISM);

            if (Game.PlayerStats.Traits.X == TraitType.Ectomorph || Game.PlayerStats.Traits.Y == TraitType.Ectomorph)
            {
                m_playerSprite.ScaleX *= 0.825f;
                m_playerSprite.ScaleY *= 1.25f;
            }

            if (Game.PlayerStats.Traits.X == TraitType.Endomorph || Game.PlayerStats.Traits.Y == TraitType.Endomorph)
            {
                m_playerSprite.ScaleX *= 1.25f;
                m_playerSprite.ScaleY *= 1.175f;
            }

            if (Game.PlayerStats.Class == ClassType.SpellSword || Game.PlayerStats.Class == ClassType.SpellSword2)
                m_playerSprite.OutlineColour = Color.White;
            else
                m_playerSprite.OutlineColour = Color.Black;

            // Reposition the player's Y after changing his scale.
            m_playerSprite.CalculateBounds();
            m_playerSprite.Y = 435 - (m_playerSprite.Bounds.Bottom - m_playerSprite.Y);
        }

        public void ExitScreenTransition()
        {
            SoundManager.PlaySound("StatCard_Out");

            Tween.To(m_cancelText, 0.2f, Tween.EaseNone, "Opacity", "0");

            m_frontCard.Y = 30;
            m_backCard.Y = 30;

            Tween.To(this, 0.2f, Tween.EaseNone, "delay", "0.3", "BackBufferOpacity", "0");
            Tween.To(m_frontCard, 0.4f, Back.EaseIn, "Y", "1500");
            Tween.To(m_backCard, 0.4f, Back.EaseIn, "delay", "0.2", "Y", "1500");
            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");

            base.OnExit();
        }

        private void LoadCardColour()
        {
            Color frontColor = Color.Red;
            Color backColor = Color.Red;

            switch (Game.PlayerStats.Class)
            {
                case (ClassType.Assassin):
                case (ClassType.Assassin2):
                    frontColor = backColor = Color.Green;
                    break;
                case (ClassType.Knight):
                case (ClassType.Knight2):
                    frontColor = backColor = Color.White;
                    break;
                case (ClassType.Barbarian):
                case (ClassType.Barbarian2):
                    frontColor = backColor = Color.Red;
                    break;
                case (ClassType.Wizard):
                case (ClassType.Wizard2):
                    frontColor = backColor = Color.Blue;
                    break;
                case (ClassType.Banker):
                case (ClassType.Banker2):
                    frontColor = backColor = Color.Gold;
                    break;
                case (ClassType.Lich):
                case (ClassType.Lich2):
                    frontColor = backColor = Color.Black;
                    break;
                case (ClassType.SpellSword):
                case (ClassType.SpellSword2):
                    frontColor = Color.Blue;
                    backColor = Color.Red;
                    break;
                case (ClassType.Ninja):
                case (ClassType.Ninja2):
                    frontColor = backColor = Color.Gray;
                    break;
                case(ClassType.Dragon):
                    frontColor = Color.White;
                    backColor = Color.Green;
                    break;
            }

            m_frontCard.GetChildAt(0).TextureColor = frontColor; // BG
            m_frontCard.GetChildAt(3).TextureColor = frontColor; // Border
            m_backCard.GetChildAt(0).TextureColor = backColor;
            m_backCard.GetChildAt(2).TextureColor = backColor;

            m_frontCard.GetChildAt(2).TextureColor = new Color(235, 220, 185);//new Color(230, 215, 175); // Border
            m_backCard.GetChildAt(1).TextureColor = new Color(235, 220, 185);
        }

        private void LoadFrontCardStats(PlayerObj player)
        {
            // Setting trait data and positions.
            m_frontTrait1.Visible = false;
            m_frontTrait2.Visible = false;

            byte trait1Data = (byte)(Game.PlayerStats.Traits.X + m_traitsDebugCounter.X);
            if (trait1Data != TraitType.None)
            {
                m_frontTrait1.Text = LocaleBuilder.getResourceString(TraitType.ToStringID(trait1Data)) + ": " + LocaleBuilder.getResourceString(TraitType.ProfileCardDescriptionID(trait1Data));
                m_frontTrait1.Visible = true;
            }

            byte trait2Data = (byte)(Game.PlayerStats.Traits.Y + m_traitsDebugCounter.Y);
            if (trait2Data != TraitType.None)
            {
                m_frontTrait2.Y = m_frontTrait1.Y;
                if (trait1Data != TraitType.None)
                    m_frontTrait2.Y -= 20;
                m_frontTrait2.Text = LocaleBuilder.getResourceString(TraitType.ToStringID(trait2Data)) + ": " + LocaleBuilder.getResourceString(TraitType.ProfileCardDescriptionID(trait2Data));
                m_frontTrait2.Visible = true;
            }

            try
            {
                m_playerName.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_playerName));
                m_playerName.Text = Game.NameHelper();
                if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch(m_playerName.Text, @"\p{IsCyrillic}"))
                    m_playerName.ChangeFontNoDefault(Game.RobotoSlabFont);
            }
            catch
            {
                m_playerName.ChangeFontNoDefault(Game.NotoSansSCFont);
                m_playerName.Text = Game.NameHelper();
            }
            //m_playerStats.Text = (int)(player.Damage / 10f) + "/" + (int)(player.MaxHealth / 100f);
            m_playerStats.Text = (int)(player.Damage / 20f) + "/" + (int)(player.MaxHealth / 50f);
            m_levelClass.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_CARD_SCREEN_12") + " " + Game.PlayerStats.CurrentLevel + " - " + LocaleBuilder.getResourceString(ClassType.ToStringID((byte)(Game.PlayerStats.Class + m_classDebugCounter), Game.PlayerStats.IsFemale));
            m_money.Text = Game.PlayerStats.Gold.ToString();
            m_classDescription.Text = ClassType.ProfileCardDescription((byte)(Game.PlayerStats.Class + m_classDebugCounter));
        }

        private void LoadBackCardStats(PlayerObj player)
        {
            // Loading player base stats.
            for (int i = 0; i < m_dataList1.Count; i++)
            {
                switch (i)
                {
                    case (0):
                        m_dataList1[i].Text = player.MaxHealth.ToString();
                        m_dataList2[i].Text = player.Damage.ToString();
                        break;
                    case (1):
                        m_dataList1[i].Text = player.MaxMana.ToString();
                        m_dataList2[i].Text = player.TotalMagicDamage.ToString();
                        break;
                    case (2):
                        m_dataList1[i].Text = player.TotalArmor.ToString() + "(" + (int)(player.TotalDamageReduc * 100) + "%)";
                        float critChance = player.TotalCritChance * 100;
                        m_dataList2[i].Text = ((int)(Math.Round(critChance, MidpointRounding.AwayFromZero))).ToString() + "%";
                        break;
                    case (3):
                        m_dataList1[i].Text = player.CurrentWeight + "/" + player.MaxWeight;
                        m_dataList2[i].Text = ((int)(player.TotalCriticalDamage * 100)).ToString() + "%";
                        break;
                }
            }

            // Loading equipment.
            sbyte[] playerEquipment = Game.PlayerStats.GetEquippedArray;
            int startingY = (int)m_equipmentTitle.Y + 40;
            for (int i = 0; i < Game.PlayerStats.GetEquippedArray.Length; i++)
            {
                m_equipmentList[i].Visible = false;
                m_equipmentList[i].Y = startingY;
                if (playerEquipment[i] != -1)
                {
                    m_equipmentList[i].Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_EQUIPMENT_BASE_FORMAT", true), LocaleBuilder.getResourceString(EquipmentBaseType.ToStringID(playerEquipment[i]), true), LocaleBuilder.getResourceString(EquipmentCategoryType.ToStringID(i), true));
                    m_equipmentList[i].Visible = true;
                    startingY += 20;
                }
            }

            // Loading runes
            startingY = (int)m_runesTitle.Y + 40;
            for (int i = 0; i < m_runeBackTitleList.Count; i++)
            {
                m_runeBackTitleList[i].Y = startingY;
                m_runeBackDescriptionList[i].Y = startingY;

                m_runeBackTitleList[i].Visible = false;
                m_runeBackDescriptionList[i].Visible = false;

                float dataNumber = 0;

                switch (i)
                {
                    case (EquipmentAbilityType.Dash):
                        dataNumber = player.TotalAirDashes;
                        break;
                    case (EquipmentAbilityType.DoubleJump):
                        dataNumber = player.TotalDoubleJumps;
                        break;
                    case (EquipmentAbilityType.Flight):
                        dataNumber = player.TotalFlightTime;
                        break;
                    case (EquipmentAbilityType.Vampirism):
                        dataNumber = player.TotalVampBonus;
                        break;
                    case (EquipmentAbilityType.ManaGain):
                        dataNumber = player.ManaGain;
                        break;
                    case (EquipmentAbilityType.DamageReturn):
                        dataNumber = player.TotalDamageReturn * 100;
                        break;
                    case (EquipmentAbilityType.GoldGain):
                        dataNumber = player.TotalGoldBonus * 100;
                        break;
                    case (EquipmentAbilityType.MovementSpeed):
                        //float traitMod = 0;
                        //if (Game.PlayerStats.Traits.X == TraitType.Hyperactive || Game.PlayerStats.Traits.Y == TraitType.Hyperactive)
                        //    traitMod = GameEV.TRAIT_MOVESPEED_AMOUNT;
                        //dataNumber = (player.GetEquipmentSecondaryAttrib(EquipmentSecondaryDataType.MoveSpeed) + traitMod +
                        //(Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.MovementSpeed) * GameEV.RUNE_MOVEMENTSPEED_MOD)) *
                        //player.ClassMoveSpeedMultiplier * 100;
                        dataNumber = (player.TotalMovementSpeedPercent * 100) - 100;
                        break;
                    //case (EquipmentAbilityType.ManaHPGain): // DO NOT ADD. This is replaced by vamp and siphon
                    //    dataNumber = Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.ManaHPGain) * GameEV.RUNE_MANAHPGAIN;
                    //    break;
                    case (EquipmentAbilityType.RoomLevelDown):
                        dataNumber = Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.RoomLevelDown) * GameEV.RUNE_GRACE_ROOM_LEVEL_LOSS;
                        break;
                    case (EquipmentAbilityType.RoomLevelUp):
                        dataNumber = Game.PlayerStats.GetNumberOfEquippedRunes(EquipmentAbilityType.RoomLevelUp) * GameEV.RUNE_CURSE_ROOM_LEVEL_GAIN;
                        break;
                }

                if (dataNumber > 0)
                {
                    m_runeBackDescriptionList[i].Text = "(" + EquipmentAbilityType.ShortDescription(i, dataNumber) + ")";
                    m_runeBackDescriptionList[i].X = m_runeBackTitleList[i].Bounds.Right + 10;
                    m_runeBackTitleList[i].Visible = true;
                    m_runeBackDescriptionList[i].Visible = true;
                    startingY += 20;
                }
            }

            // Special handling for the architect's fee. The architect rune is always the last one on the list.
            if (Game.PlayerStats.HasArchitectFee == true)
            {
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 2].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 2]));
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 2].Text = "(" + EquipmentAbilityType.ShortDescription(EquipmentAbilityType.ArchitectFee, 0) + ")";
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 2].X = m_runeBackTitleList[m_runeBackDescriptionList.Count - 2].Bounds.Right + 10;
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 2].Visible = true;
                m_runeBackTitleList[m_runeBackDescriptionList.Count - 2].Visible = true;
                startingY += 20;
            }
            // Special handling for the architect's fee. The architect rune is always the last one on the list.
            if (Game.PlayerStats.TimesCastleBeaten > 0)
            {
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 1].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 1]));
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 1].Text = "(" + EquipmentAbilityType.ShortDescription(EquipmentAbilityType.NewGamePlusGoldBonus, ((int)(GameEV.NEWGAMEPLUS_GOLDBOUNTY *100) * Game.PlayerStats.TimesCastleBeaten)) + ")";
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 1].X = m_runeBackTitleList[m_runeBackDescriptionList.Count - 1].Bounds.Right + 10;
                m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 1].Visible = true;
                m_runeBackTitleList[m_runeBackDescriptionList.Count - 1].Visible = true;
                if (Game.PlayerStats.HasArchitectFee == true)
                {
                    m_runeBackDescriptionList[m_runeBackDescriptionList.Count - 1].Y = startingY;
                    m_runeBackTitleList[m_runeBackDescriptionList.Count - 1].Y = startingY;
                }
                //startingY += 20;
            }

            try
            {
                (m_backCard.GetChildAt(3) as TextObj).ChangeFontNoDefault(LocaleBuilder.GetLanguageFont((m_backCard.GetChildAt(3) as TextObj)));
                (m_backCard.GetChildAt(3) as TextObj).Text = Game.NameHelper();
                if (LocaleBuilder.languageType != LanguageType.Chinese_Simp && Regex.IsMatch((m_backCard.GetChildAt(3) as TextObj).Text, @"\p{IsCyrillic}"))
                    (m_backCard.GetChildAt(3) as TextObj).ChangeFontNoDefault(Game.RobotoSlabFont);
            }
            catch
            {
                (m_backCard.GetChildAt(3) as TextObj).ChangeFontNoDefault(Game.NotoSansSCFont);
                (m_backCard.GetChildAt(3) as TextObj).Text = Game.NameHelper();
            }
        }

        public override void HandleInput()
        {
            if (m_frontCard.Y == 30) // Only accept input once intro tween is complete.
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3)
                    || Game.GlobalInput.JustPressed(InputMapType.MENU_PROFILECARD))
                 ExitScreenTransition();
            }

            if (LevelEV.ENABLE_DEBUG_INPUT == true)
                HandleDebugInput();

            base.HandleInput();
        }

        private sbyte m_classDebugCounter = 0;
        private Vector2 m_traitsDebugCounter = Vector2.Zero;
        private void HandleDebugInput()
        {
            // Debug testing for class localization.
            sbyte currentDebugClass = (sbyte)(Game.PlayerStats.Class + m_classDebugCounter);
            sbyte previousDebugClass = currentDebugClass;

            if (InputManager.JustPressed(Keys.OemMinus, PlayerIndex.One))
            {
                if (currentDebugClass == ClassType.Knight)
                    m_classDebugCounter = (sbyte)(ClassType.Traitor - Game.PlayerStats.Class);
                else
                    m_classDebugCounter--;
                currentDebugClass = (sbyte)(Game.PlayerStats.Class + m_classDebugCounter);
            }
            else if (InputManager.JustPressed(Keys.OemPlus, PlayerIndex.One))
            {
                if (currentDebugClass == ClassType.Traitor)
                    m_classDebugCounter = (sbyte)(-Game.PlayerStats.Class);
                else
                    m_classDebugCounter++;
                currentDebugClass = (sbyte)(Game.PlayerStats.Class + m_classDebugCounter);
            }

            if (previousDebugClass != currentDebugClass)
            {
                m_classDescription.Text = ClassType.ProfileCardDescription((byte)currentDebugClass);
                m_levelClass.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_CARD_SCREEN_12") + " " + Game.PlayerStats.CurrentLevel + " - " + LocaleBuilder.getResourceString(ClassType.ToStringID((byte)currentDebugClass, Game.PlayerStats.IsFemale));
            }

            // Debug testing for traits localization.
            Vector2 currentDebugTraits = Game.PlayerStats.Traits + m_traitsDebugCounter;
            Vector2 previousTraits = currentDebugTraits;

            if (InputManager.JustPressed(Keys.OemPeriod, PlayerIndex.One))
            {
                if (currentDebugTraits.X == TraitType.None)
                    m_traitsDebugCounter.X = (TraitType.Total - 1) - Game.PlayerStats.Traits.X;
                else
                    m_traitsDebugCounter.X--;

                currentDebugTraits = Game.PlayerStats.Traits + m_traitsDebugCounter;
            }
            else if (InputManager.JustPressed(Keys.OemQuestion, PlayerIndex.One))
            {
                if (currentDebugTraits.X == TraitType.Total - 1)
                    m_traitsDebugCounter.X = -Game.PlayerStats.Traits.X;
                else
                    m_traitsDebugCounter.X++;

                currentDebugTraits = Game.PlayerStats.Traits + m_traitsDebugCounter;
            }

            if (InputManager.JustPressed(Keys.M, PlayerIndex.One))
            {
                if (currentDebugTraits.Y == TraitType.None)
                    m_traitsDebugCounter.Y = (TraitType.Total - 1) - Game.PlayerStats.Traits.Y;
                else
                    m_traitsDebugCounter.Y--;

                currentDebugTraits = Game.PlayerStats.Traits + m_traitsDebugCounter;
            }
            else if (InputManager.JustPressed(Keys.OemComma, PlayerIndex.One))
            {
                if (currentDebugTraits.Y == TraitType.Total - 1)
                    m_traitsDebugCounter.Y = -Game.PlayerStats.Traits.Y;
                else
                    m_traitsDebugCounter.Y++;

                currentDebugTraits = Game.PlayerStats.Traits + m_traitsDebugCounter;
            }

            if (currentDebugTraits.X >= TraitType.Total)
                currentDebugTraits.X = TraitType.None;
            else if (currentDebugTraits.X < TraitType.None)
                currentDebugTraits.X = TraitType.Total - 1;
            if (currentDebugTraits.Y >= TraitType.Total)
                currentDebugTraits.Y = TraitType.None;
            else if (currentDebugTraits.Y < TraitType.None)
                currentDebugTraits.Y = TraitType.Total - 1;

            if (currentDebugTraits != previousTraits)
            {
                m_frontTrait1.Visible = false;
                m_frontTrait2.Visible = false;

                byte trait1Data = (byte)currentDebugTraits.X;
                if (trait1Data != TraitType.None)
                {
                    m_frontTrait1.Text = LocaleBuilder.getResourceString(TraitType.ToStringID(trait1Data)) + ": " + LocaleBuilder.getResourceString(TraitType.ProfileCardDescriptionID(trait1Data));
                    m_frontTrait1.Visible = true;
                }

                byte trait2Data = (byte)currentDebugTraits.Y;
                if (trait2Data != TraitType.None)
                {
                    m_frontTrait2.Y = m_frontTrait1.Y;
                    if (trait1Data != TraitType.None)
                        m_frontTrait2.Y -= 20;
                    m_frontTrait2.Text = LocaleBuilder.getResourceString(TraitType.ToStringID(trait2Data)) + ": " + LocaleBuilder.getResourceString(TraitType.ProfileCardDescriptionID(trait2Data));
                    m_frontTrait2.Visible = true;
                }
            }
        }

        public override void Draw(GameTime gametime)
        {
            m_playerHUD.SetPosition(new Vector2(m_frontCard.X + 46, m_frontCard.Y + 220 + 64));

            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight), Color.Black * BackBufferOpacity);
            m_frontCard.Draw(Camera);
            m_backCard.Draw(Camera);

            m_cancelText.Draw(Camera);

            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            if (Game.PlayerStats.IsDead == true)
            {
                //m_tombStoneSprite.Position = new Vector2(m_frontCard.X + 240, m_frontCard.Y + 280);
                m_tombStoneSprite.Position = new Vector2(m_frontCard.X + 240, m_frontCard.Y + 280 + 220);
                m_tombStoneSprite.Draw(Camera);
            }
            else
            {
                if (m_playerInAir == true)
                    //m_playerSprite.Position = new Vector2(m_frontCard.X + 180, m_frontCard.Y + 202);
                    m_playerSprite.Position = new Vector2(m_frontCard.X + 180, m_frontCard.Y + 202 + 220);
                else
                {
                    //m_playerSprite.Position = new Vector2(m_frontCard.X + 160, m_frontCard.Y + 280 - (m_playerSprite.Bounds.Bottom - m_playerSprite.Y));
                    m_playerSprite.Position = new Vector2(m_frontCard.X + 160, m_frontCard.Y + 280 - (m_playerSprite.Bounds.Bottom - m_playerSprite.Y) + 220);
                }
                m_playerSprite.Draw(Camera);
                Game.ColourSwapShader.Parameters["desiredTint"].SetValue(m_playerSprite.GetChildAt(PlayerPart.Head).TextureColor.ToVector4());
                if (Game.PlayerStats.Class == ClassType.Lich || Game.PlayerStats.Class == ClassType.Lich2)
                {
                    // This is the Tint Removal effect, that removes the tint from his face.
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(m_playerSprite.Opacity);
                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(m_lichColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(m_lichColour2.ToVector4());
                }
                else if (Game.PlayerStats.Class == ClassType.Assassin || Game.PlayerStats.Class == ClassType.Assassin2)
                {
                    // This is the Tint Removal effect, that removes the tint from his face.
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(m_playerSprite.Opacity);
                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(Color.Black.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(Color.Black.ToVector4());
                }
                else
                {
                    Game.ColourSwapShader.Parameters["Opacity"].SetValue(1);
                    Game.ColourSwapShader.Parameters["ColourSwappedOut1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn1"].SetValue(m_skinColour1.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedOut2"].SetValue(m_skinColour2.ToVector4());
                    Game.ColourSwapShader.Parameters["ColourSwappedIn2"].SetValue(m_skinColour2.ToVector4());
                }

                Camera.End();
                Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game.ColourSwapShader);
                m_playerSprite.GetChildAt(PlayerPart.Head).Draw(Camera);
                Camera.End();
                Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
                if (Game.PlayerStats.IsFemale == true)
                    m_playerSprite.GetChildAt(PlayerPart.Bowtie).Draw(Camera);
                m_playerSprite.GetChildAt(PlayerPart.Extra).Draw(Camera);
            }

            //m_spellIcon.Position = new Vector2(m_frontCard.X + 380, m_frontCard.Y + 320);
            m_spellIcon.Position = new Vector2(m_frontCard.X + 380, m_frontCard.Y + 320 + 220);
            m_spellIcon.Draw(Camera);

            Camera.End();

            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Profile Card Screen");

                m_frontCard.Dispose();
                m_frontCard = null;
                m_backCard.Dispose();
                m_backCard = null;

                // Disposing front card objects.
                m_playerName = null;
                m_money = null;
                m_levelClass = null;
                m_playerHUD = null;
                m_frontTrait1 = null;
                m_frontTrait2 = null;
                m_playerBG = null;
                m_classDescription = null;
                m_author = null;
                m_playerStats = null;

                // Disposing back card objects.
                m_equipmentTitle = null;
                m_runesTitle = null;
                m_equipmentList.Clear();
                m_equipmentList = null;
                m_runeBackTitleList.Clear();
                m_runeBackTitleList = null;
                m_runeBackDescriptionList.Clear();
                m_runeBackDescriptionList = null;

                m_playerSprite.Dispose();
                m_playerSprite = null;

                m_spellIcon.Dispose();
                m_spellIcon = null;

                m_tombStoneSprite.Dispose();
                m_tombStoneSprite = null;

                m_cancelText.Dispose();
                m_cancelText = null;

                // Even though text objs are added to the data lists, they're also added to the cards, which dispose them when the cards are disposed.
                m_dataList1.Clear();
                m_dataList1 = null;
                m_dataList2.Clear();
                m_dataList2 = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            m_equipmentTitle.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_CARD_SCREEN_9") + ":";
            m_runesTitle.Text = LocaleBuilder.getResourceString("LOC_ID_PROFILE_CARD_SCREEN_10") + ":";
            //m_cancelText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "] " + LocaleBuilder.getResourceString("LOC_ID_PROFILE_CARD_SCREEN_11");

            PlayerObj player = (ScreenManager as RCScreenManager).Player;
            LoadFrontCardStats(player);
            LoadBackCardStats(player);

            // Massive hack.  14 is the index of a text line that is too long in russian.
            m_backCard.GetChildAt(14).ScaleX = 1;
            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.Russian):
                    m_backCard.GetChildAt(14).ScaleX = 0.9f;
                    break;
            }

            base.RefreshTextObjs();
        }
    }
}

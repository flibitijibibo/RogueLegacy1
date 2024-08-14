using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;
using Microsoft.Xna.Framework.Graphics;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Tweener;
using Tweener.Ease;

namespace RogueCastle
{
    public class SkillScreen : Screen
    {
        /* Dialogue plate object order
         * 0 = Plate BG
         * 1 = Skill Icon
         * 2 = Skill Title
         * 3 = Skill Description
         * 4 = Input Description
         * 5 = Stat Base Amount
         * 6 = Stat Increase Amount
         * 7 = Skill Cost
         */

        private SpriteObj m_bg;
        private SpriteObj m_cloud1, m_cloud2, m_cloud3, m_cloud4, m_cloud5;
        private SpriteObj m_selectionIcon;
        private Vector2 m_selectedTraitIndex;

        private KeyIconTextObj m_continueText;
        private KeyIconTextObj m_toggleIconsText;
        private KeyIconTextObj m_confirmText;
        private KeyIconTextObj m_navigationText;
        private SpriteObj m_titleText;
        
        private ObjContainer m_dialoguePlate;
        private ObjContainer m_manor;
        private bool m_fadingIn = false;
        private bool m_cameraTweening = false;

        private bool m_lockControls = false;

        private ImpactEffectPool m_impactEffectPool;
        private GameObj m_shakeObj;
        private float m_shakeTimer = 0;
        private int m_shakeAmount = 2;
        private float m_shakeDelay = 0.01f;
        private bool m_shookLeft = false;
        private float m_shakeDuration;

        private TextObj m_playerMoney;
        private SpriteObj m_coinIcon;

        private SpriteObj m_skillIcon;
        private TextObj m_skillTitle;
        private TextObj m_skillDescription;
        private KeyIconTextObj m_inputDescription;
        private SpriteObj m_descriptionDivider;
        private TextObj m_skillCurrent;
        private TextObj m_skillUpgrade;
        private TextObj m_skillLevel;

        private TextObj m_skillCost;
        private SpriteObj m_skillCostBG;

        private SkillObj m_lastSkillObj; // for switching languages and updating display

        public SkillScreen()
        {
            m_selectedTraitIndex = new Vector2(5, 9); // Hack. That is the array index for the starting trait.
            m_impactEffectPool = new ImpactEffectPool(1000);
            this.DrawIfCovered = true;
        }

        public override void LoadContent()
        {
            m_impactEffectPool.Initialize();

            m_manor = new ObjContainer("TraitsCastle_Character");
            m_manor.Scale = new Vector2(2, 2);
            m_manor.ForceDraw = true;
            for (int i = 0; i < m_manor.NumChildren; i++)
            {
                m_manor.GetChildAt(i).Visible = false;
                m_manor.GetChildAt(i).Opacity = 0;
            }

            // Adding Dialogue Plate skill info text
            m_dialoguePlate = new ObjContainer("TraitsScreenPlate_Container");
            m_dialoguePlate.ForceDraw = true;
            m_dialoguePlate.Position = new Vector2(1320 - m_dialoguePlate.Width / 2, 720 / 2);

            m_skillIcon = new SpriteObj("Icon_Health_Up_Sprite");
            m_skillIcon.Position = new Vector2(-110, -200);
            m_dialoguePlate.AddChild(m_skillIcon);

            m_skillTitle = new TextObj(Game.JunicodeFont);
            m_skillTitle.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_skillTitle); // dummy locID to add TextObj to language refresh list
            m_skillTitle.DropShadow = new Vector2(2, 2);
            m_skillTitle.TextureColor = new Color(236, 197, 132);
            m_skillTitle.Position = new Vector2(m_skillIcon.Bounds.Right + 15, m_skillIcon.Y);
            m_skillTitle.FontSize = 12;
            m_dialoguePlate.AddChild(m_skillTitle);

            m_skillDescription = new TextObj(Game.JunicodeFont);
            m_skillDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_skillDescription); // dummy locID to add TextObj to language refresh list
            m_skillDescription.Position = new Vector2(m_dialoguePlate.GetChildAt(1).X - 30, m_dialoguePlate.GetChildAt(1).Bounds.Bottom + 20);
            m_skillDescription.FontSize = 10;
            m_skillDescription.DropShadow = new Vector2(2, 2);
            m_skillDescription.TextureColor = new Color(228, 218, 208);
            m_skillDescription.WordWrap(m_dialoguePlate.Width - 50);
            m_dialoguePlate.AddChild(m_skillDescription);

            m_inputDescription = new KeyIconTextObj(Game.JunicodeFont);
            m_inputDescription.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_inputDescription); // dummy locID to add TextObj to language refresh list
            m_inputDescription.Position = new Vector2(m_skillIcon.X - 30, m_skillDescription.Bounds.Bottom + 20);
            m_inputDescription.FontSize = 10;
            m_inputDescription.DropShadow = new Vector2(2, 2);
            m_inputDescription.TextureColor = new Color(228, 218, 208);
            m_inputDescription.WordWrap(m_dialoguePlate.Width - 50);
            m_dialoguePlate.AddChild(m_inputDescription);

            m_descriptionDivider = new SpriteObj("Blank_Sprite");
            m_descriptionDivider.ScaleX = 250f / m_descriptionDivider.Width;
            m_descriptionDivider.ScaleY = 0.25f;
            m_descriptionDivider.ForceDraw = true;
            m_descriptionDivider.DropShadow = new Vector2(2, 2);

            m_skillCurrent = new TextObj(Game.JunicodeFont);
            m_skillCurrent.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_skillCurrent); // dummy locID to add TextObj to language refresh list
            m_skillCurrent.Position = new Vector2(m_inputDescription.X, m_inputDescription.Bounds.Bottom + 10);
            m_skillCurrent.FontSize = 10;
            m_skillCurrent.DropShadow = new Vector2(2, 2);
            m_skillCurrent.TextureColor = new Color(228, 218, 208);
            m_skillCurrent.WordWrap(m_dialoguePlate.Width - 50);
            m_dialoguePlate.AddChild(m_skillCurrent);

            m_skillUpgrade = m_skillCurrent.Clone() as TextObj;
            m_skillUpgrade.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_skillUpgrade); // dummy locID to add TextObj to language refresh list
            m_skillUpgrade.Y += 15;
            //m_skillUpgrade.TextureColor = Color.Yellow;
            m_dialoguePlate.AddChild(m_skillUpgrade);

            m_skillLevel = m_skillUpgrade.Clone() as TextObj;
            m_skillLevel.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_skillLevel); // dummy locID to add TextObj to language refresh list
            m_skillLevel.Y += 15;
            //m_skillLevel.TextureColor = new Color(228, 218, 208);
            m_dialoguePlate.AddChild(m_skillLevel);

            m_skillCost = new TextObj(Game.JunicodeFont);
            m_skillCost.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_skillCost); // dummy locID to add TextObj to language refresh list
            //skillCost.Position = new Vector2(skillIcon.X, m_dialoguePlate.Bounds.Bottom - 20);
            m_skillCost.X = m_skillIcon.X;
            m_skillCost.Y = 182;
            m_skillCost.FontSize = 10;
            m_skillCost.DropShadow = new Vector2(2, 2);
            m_skillCost.TextureColor = Color.Yellow;
            m_dialoguePlate.AddChild(m_skillCost);

            m_skillCostBG = new SpriteObj("SkillTreeGoldIcon_Sprite");
            m_skillCostBG.Position = new Vector2(-180, 180);
            m_dialoguePlate.AddChild(m_skillCostBG);

            m_dialoguePlate.ForceDraw = true;

            ////////////////////////////////////////////

            m_bg = new SpriteObj("TraitsBG_Sprite");
            m_bg.Scale = new Vector2(1320f / m_bg.Width, 1320f / m_bg.Width);
            m_bg.ForceDraw = true;

            m_cloud1 = new SpriteObj("TraitsCloud1_Sprite") { ForceDraw = true };
            m_cloud2 = new SpriteObj("TraitsCloud2_Sprite") { ForceDraw = true };
            m_cloud3 = new SpriteObj("TraitsCloud3_Sprite") { ForceDraw = true };
            m_cloud4 = new SpriteObj("TraitsCloud4_Sprite") { ForceDraw = true };
            m_cloud5 = new SpriteObj("TraitsCloud5_Sprite") { ForceDraw = true };
            float opacity = 1f;
            m_cloud1.Opacity = opacity;
            m_cloud2.Opacity = opacity;
            m_cloud3.Opacity = opacity;
            m_cloud4.Opacity = opacity;
            m_cloud5.Opacity = opacity;
            m_cloud1.Position = new Vector2(CDGMath.RandomInt(0, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            m_cloud2.Position = new Vector2(CDGMath.RandomInt(0, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            m_cloud3.Position = new Vector2(CDGMath.RandomInt(0, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            m_cloud4.Position = new Vector2(CDGMath.RandomInt(0, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            m_cloud5.Position = new Vector2(CDGMath.RandomInt(0, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));

            m_selectionIcon = new SpriteObj("IconHalo_Sprite");
            m_selectionIcon.ForceDraw = true;
            m_selectionIcon.AnimationDelay = 1 / 10f;
            m_selectionIcon.PlayAnimation(true);
            m_selectionIcon.Scale = new Vector2(1.1f, 1.1f);

            m_titleText = new SpriteObj("ManorTitleText_Sprite");
            m_titleText.X = m_titleText.Width/2f + 20;
            m_titleText.Y = GlobalEV.ScreenHeight * 0.09f;
            m_titleText.ForceDraw = true;

            m_continueText = new KeyIconTextObj(Game.JunicodeFont);
            m_continueText.ForceDraw = true;
            m_continueText.FontSize = 12;
            m_continueText.DropShadow = new Vector2(2, 2);
            m_continueText.Position = new Vector2(1320 - 20, 630);
            m_continueText.Align = Types.TextAlign.Right;
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_continueText); // dummy locID to add TextObj to language refresh list

            m_toggleIconsText = new KeyIconTextObj(Game.JunicodeFont);
            m_toggleIconsText.ForceDraw = true;
            m_toggleIconsText.FontSize = 12;
            m_toggleIconsText.DropShadow = new Vector2(2, 2);
            m_toggleIconsText.Position = new Vector2(m_continueText.X, m_continueText.Y + 40);
            m_toggleIconsText.Align = Types.TextAlign.Right;
            m_toggleIconsText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_toggleIconsText); // dummy locID to add TextObj to language refresh list

            m_confirmText = new KeyIconTextObj(Game.JunicodeFont);
            m_confirmText.Align = Types.TextAlign.Right;
            m_confirmText.FontSize = 12;
            m_confirmText.DropShadow = new Vector2(2, 2);
            m_confirmText.Position = new Vector2(1320 - 20, 10);
            m_confirmText.ForceDraw = true;
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_confirmText); // dummy locID to add TextObj to language refresh list

            m_navigationText = new KeyIconTextObj(Game.JunicodeFont);
            m_navigationText.Align = Types.TextAlign.Right;
            m_navigationText.FontSize = 12;
            m_navigationText.DropShadow = new Vector2(2, 2);
            m_navigationText.Position = new Vector2(m_confirmText.X, m_confirmText.Y + 40);
            m_navigationText.ForceDraw = true;
            m_navigationText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_navigationText); // dummy locID to add TextObj to language refresh list

            m_coinIcon = new SpriteObj("CoinIcon_Sprite");
            m_coinIcon.Position = new Vector2(1100, 585);
            m_coinIcon.Scale = new Vector2(0.9f, 0.9f);
            m_coinIcon.ForceDraw = true;

            m_playerMoney = new TextObj(Game.GoldFont);
            m_playerMoney.Align = Types.TextAlign.Left;
            m_playerMoney.Text = "1000";
            m_playerMoney.FontSize = 30;
            m_playerMoney.Position = new Vector2(m_coinIcon.X + 35, m_coinIcon.Y);
            m_playerMoney.ForceDraw = true;

            base.LoadContent();
        }

        public override void OnEnter()
        {
            bool unlockAchievement = true;
            foreach (SkillObj skillAchievement in SkillSystem.SkillArray)
            {
                if (skillAchievement.CurrentLevel < 1)
                {
                    unlockAchievement = false;
                    break;
                }
            }

            if (unlockAchievement == true)
                GameUtil.UnlockAchievement("FEAR_OF_DECISIONS");

            if (Game.PlayerStats.CurrentLevel >= 50)
                GameUtil.UnlockAchievement("FEAR_OF_WEALTH");

            m_lockControls = false;

            m_manor.GetChildAt(23).Visible = true;
            m_manor.GetChildAt(23).Opacity = 1;

            Camera.Position = new Vector2(1320 / 2f, 720 / 2f);

            SkillObj[] traitArray = SkillSystem.GetSkillArray();
            for (int i = 0; i < traitArray.Length; i++)
            {
                if (traitArray[i].CurrentLevel > 0)
                    SetVisible(traitArray[i], false);
            }

            if (SoundManager.IsMusicPlaying == false)
                SoundManager.PlayMusic("SkillTreeSong", true, 1);

            SkillObj skill = SkillSystem.GetSkill((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y);
            m_selectionIcon.Position = SkillSystem.GetSkillPosition(skill);

            UpdateDescriptionPlate(skill);

            m_dialoguePlate.Visible = true;
            m_confirmText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_1_NEW", m_confirmText);
            //m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_1");
            m_toggleIconsText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_2_NEW", m_toggleIconsText);
            //m_toggleIconsText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_2");
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_3_NEW", m_continueText);
            //m_continueText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_3");

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == true)
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_4_NEW", m_navigationText);
                //m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_4");
            else
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_5", m_navigationText);

            SkillSystem.UpdateAllTraitSprites();

            RefreshTextObjs();

            base.OnEnter();
        }

        public override void OnExit()
        {
            Game.ScreenManager.Player.AttachedLevel.UpdatePlayerSpellIcon();
            SoundManager.StopMusic(0.5f);
            (ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.UpgradeData, SaveType.PlayerData);
            base.OnExit();
        }

        public void SetVisible(SkillObj trait, bool fadeIn)
        {
            int manorIndex = SkillSystem.GetManorPiece(trait);

            if (fadeIn == true)
                SetManorPieceVisible(manorIndex, trait);
            else
            {
                GameObj manorObj = m_manor.GetChildAt(manorIndex);
                manorObj.Opacity = 1;
                manorObj.Visible = true;
                foreach (SkillObj linkedTrait in SkillSystem.GetAllConnectingTraits(trait))
                {
                    if (linkedTrait.Visible == false)
                    {
                        linkedTrait.Visible = true;
                        linkedTrait.Opacity = 1;
                    }
                }

                // Hack to add shadows to specific pieces of the manor.
                if (m_manor.GetChildAt(7).Visible == true && m_manor.GetChildAt(16).Visible == true)
                    (m_manor.GetChildAt(7) as SpriteObj).GoToFrame(2);

                if (m_manor.GetChildAt(6).Visible == true && m_manor.GetChildAt(16).Visible == true)
                    (m_manor.GetChildAt(6) as SpriteObj).GoToFrame(2);

                if (m_manor.GetChildAt(2).Visible == true)
                {
                    SpriteObj flag = m_manor.GetChildAt(32) as SpriteObj;
                    flag.Visible = true;
                    flag.Opacity = 1;
                    flag.PlayAnimation(true);
                    flag.OverrideParentAnimationDelay = true;
                    flag.AnimationDelay = 1 / 30f;
                    flag.Visible = true;
                }
            }
        }

        public void SetManorPieceVisible(int manorIndex, SkillObj skillObj)
        {
            GameObj manorPiece = m_manor.GetChildAt(manorIndex);
            float transitionDuration = 0;

            if (manorPiece.Visible == false)
            {
                m_lockControls = true;
                //Console.WriteLine(manorIndex);
                manorPiece.Visible = true;
                Vector2 cloudPos = new Vector2(manorPiece.AbsPosition.X, manorPiece.AbsBounds.Bottom);

                switch (manorIndex)
                {
                    case (0):
                    case (11):
                    case (17):
                    case (22):
                    case (24):
                    case (27):
                    case (28):
                        // fade code goes here.
                        transitionDuration = 0.5f;
                        manorPiece.Opacity = 0;
                        Tween.To(manorPiece, transitionDuration, Tween.EaseNone, "Opacity", "1");
                        break;
                    case (4):
                        // Special case for 4.
                        cloudPos.Y -= 50;
                        manorPiece.Opacity = 1;
                        transitionDuration = 3;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_01", "skill_tree_reveal_02");
                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2 * 0.25f, transitionDuration);
                        break;
                    case (7):
                        // Special case for 7.
                        cloudPos.X = manorPiece.AbsBounds.Right - (manorPiece.Width * 2 * 0.25f);
                        manorPiece.Opacity = 1;
                        transitionDuration = 3;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_01", "skill_tree_reveal_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2 * 0.25f, transitionDuration);
                        break;
                    case (18):
                    case (19):
                        // Special case for 18 and 19
                        manorPiece.Opacity = 1;
                        transitionDuration = 3;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_01", "skill_tree_reveal_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2 * 0.2f, transitionDuration);
                        break;
                    case (16):
                        // Special case for 16.
                        manorPiece.Opacity = 1;
                        transitionDuration = 3;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_01", "skill_tree_reveal_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2 * 0.5f, transitionDuration);
                        break;
                    case (8):
                        // Special case for 8.
                        cloudPos.X = manorPiece.AbsBounds.Right - (manorPiece.Width * 2 * 0.25f);
                        manorPiece.Opacity = 1;
                        transitionDuration = 3;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_01", "skill_tree_reveal_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2 * 0.25f, transitionDuration);
                        break;
                    case (10):
                    case (21):
                        // Grow Upward Slowly
                        manorPiece.Opacity = 1;
                        transitionDuration = 3;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_01", "skill_tree_reveal_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2, transitionDuration);
                        break;
                    case (2):
                        // Special case for 2.  Grow quickly + flag appearing.
                        manorPiece.Opacity = 1;
                        transitionDuration = 1.5f;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_short_01", "skill_tree_reveal_short_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2, transitionDuration);

                        SpriteObj flag = m_manor.GetChildAt(32) as SpriteObj;
                        flag.PlayAnimation(true);
                        flag.OverrideParentAnimationDelay = true;
                        flag.AnimationDelay = 1 / 30f;
                        flag.Visible = true;
                        flag.Opacity = 0;
                        Tween.To(flag, 0.5f, Tween.EaseNone, "delay", transitionDuration.ToString(), "Opacity", "1");
                        break;
                    case (3):
                    case(6):
                    case(9):
                    case(13):
                    case(15):
                    case(25):
                    case(20):
                        // Grow upward quickly.
                        manorPiece.Opacity = 1;
                        transitionDuration = 1f;
                        manorPiece.Y += manorPiece.Height * 2;
                        SoundManager.PlaySound("skill_tree_reveal_short_01", "skill_tree_reveal_short_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "Y", (-(manorPiece.Height * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, true, manorPiece.Width * 2, transitionDuration);
                        break;
                    case (1):
                    case (5):
                        // Grow Right
                        manorPiece.Opacity = 1;
                        transitionDuration = 1f;
                        manorPiece.X -= manorPiece.Width * 2;
                        SoundManager.PlaySound("skill_tree_reveal_short_01", "skill_tree_reveal_short_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "X", (manorPiece.Width * 2).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, false, manorPiece.Height * 2, transitionDuration);
                        break;
                    case (12):
                    case (14):
                        // Grow Left
                        manorPiece.Opacity = 1;
                        transitionDuration = 1;
                        manorPiece.X += manorPiece.Width * 2;
                        cloudPos.X = manorPiece.AbsPosition.X - 60;
                        SoundManager.PlaySound("skill_tree_reveal_short_01", "skill_tree_reveal_short_02");

                        Tween.By(manorPiece, transitionDuration, Quad.EaseOut, "X", (-(manorPiece.Width * 2)).ToString());
                        m_impactEffectPool.SkillTreeDustDuration(cloudPos, false, manorPiece.Height * 2, transitionDuration);
                        break;
                    case (29):
                    case (30):
                    case (31):
                        // Scale up.
                        Tween.RunFunction(0.25f, typeof(SoundManager), "PlaySound", "skill_tree_reveal_bounce");
                        manorPiece.Opacity = 1;
                        manorPiece.Scale = Vector2.Zero;
                        transitionDuration = 1;
                        Tween.To(manorPiece, transitionDuration, Bounce.EaseOut, "ScaleX", "1", "ScaleY", "1");
                        break;
                    default:
                        // Drop the very first piece.
                        transitionDuration = 0.7f;
                        Vector2 cloudPos2 = new Vector2(manorPiece.AbsPosition.X, manorPiece.AbsBounds.Bottom);
                        manorPiece.Opacity = 1;
                        manorPiece.Y -= 720;
                        Tween.By(manorPiece, transitionDuration, Quad.EaseIn, "Y", "720");
                        Tween.AddEndHandlerToLastTween(m_impactEffectPool, "SkillTreeDustEffect", cloudPos2, true, manorPiece.Width * 2);
                        Tween.RunFunction(transitionDuration, this, "ShakeScreen", 5, true, true);
                        Tween.RunFunction(transitionDuration + 0.2f, this, "StopScreenShake");
                        //SoundManager.PlaySound("Upgrade_Splash_Tower");
                        break;
                    case (23): // Don't do anything for 23 (ground piece)
                        break;
                }
            }

            Tween.RunFunction(transitionDuration, this, "SetSkillIconVisible", skillObj);

            // Hack to add shadows to specific pieces of the manor.
            if (m_manor.GetChildAt(7).Visible == true && m_manor.GetChildAt(16).Visible == true)
                (m_manor.GetChildAt(7) as SpriteObj).GoToFrame(2);

            if (m_manor.GetChildAt(6).Visible == true && m_manor.GetChildAt(16).Visible == true)
                (m_manor.GetChildAt(6) as SpriteObj).GoToFrame(2);
        }

        public void SetSkillIconVisible(SkillObj skill)
        {
            float delay = 0;
            foreach (SkillObj linkedTrait in SkillSystem.GetAllConnectingTraits(skill))
            {
                if (linkedTrait.Visible == false)
                {
                    linkedTrait.Visible = true;
                    linkedTrait.Opacity = 0;
                    Tweener.Tween.To(linkedTrait, 0.2f, Tweener.Ease.Linear.EaseNone, "Opacity", "1");
                    delay += 0.2f;
                }
            }
            Tween.RunFunction(delay, this, "UnlockControls");
            Tween.RunFunction(delay, this, "CheckForSkillUnlock", skill, true);
        }

        public void CheckForSkillUnlock(SkillObj skill, bool displayScreen)
        {
            byte skillUnlockType = SkillUnlockType.None;
            switch(skill.TraitType)
            {
                case (SkillType.Smithy):
                    skillUnlockType = SkillUnlockType.Blacksmith;
                    break;
                case (SkillType.Enchanter):
                    skillUnlockType = SkillUnlockType.Enchantress;
                    break;
                case (SkillType.Architect):
                    skillUnlockType = SkillUnlockType.Architect;
                    break;
                case (SkillType.Ninja_Unlock):
                    skillUnlockType = SkillUnlockType.Ninja;
                    break;
                case (SkillType.Banker_Unlock):
                    skillUnlockType = SkillUnlockType.Banker;
                    break;
                case (SkillType.Lich_Unlock):
                    skillUnlockType = SkillUnlockType.Lich;
                    break;
                case (SkillType.Spellsword_Unlock):
                    skillUnlockType = SkillUnlockType.SpellSword;
                    break;
                case (SkillType.Knight_Up):
                    skillUnlockType = SkillUnlockType.KnightUp;
                    if (Game.PlayerStats.Class == ClassType.Knight) Game.PlayerStats.Class = ClassType.Knight2;
                    break;
                case (SkillType.Barbarian_Up):
                    skillUnlockType = SkillUnlockType.BarbarianUp;
                    if (Game.PlayerStats.Class == ClassType.Barbarian) Game.PlayerStats.Class = ClassType.Barbarian2;
                    break;
                case (SkillType.Mage_Up):
                    skillUnlockType = SkillUnlockType.WizardUp;
                    if (Game.PlayerStats.Class == ClassType.Wizard) Game.PlayerStats.Class = ClassType.Wizard2;
                    break;
                case (SkillType.Ninja_Up):
                    skillUnlockType = SkillUnlockType.NinjaUp;
                    if (Game.PlayerStats.Class == ClassType.Ninja) Game.PlayerStats.Class = ClassType.Ninja2;
                    break;
                case (SkillType.Assassin_Up):
                    skillUnlockType = SkillUnlockType.AssassinUp;
                    if (Game.PlayerStats.Class == ClassType.Assassin) Game.PlayerStats.Class = ClassType.Assassin2;
                    break;
                case (SkillType.Banker_Up):
                    skillUnlockType = SkillUnlockType.BankerUp;
                    if (Game.PlayerStats.Class == ClassType.Banker) Game.PlayerStats.Class = ClassType.Banker2;
                    break;
                case (SkillType.SpellSword_Up):
                    skillUnlockType = SkillUnlockType.SpellSwordUp;
                    if (Game.PlayerStats.Class == ClassType.SpellSword) Game.PlayerStats.Class = ClassType.SpellSword2;
                    break;
                case (SkillType.Lich_Up):
                    skillUnlockType = SkillUnlockType.LichUp;
                    if (Game.PlayerStats.Class == ClassType.Lich) Game.PlayerStats.Class = ClassType.Lich2;
                    break;
                case (SkillType.SuperSecret):
                    skillUnlockType = SkillUnlockType.Dragon;
                    break;
            }

            if (skillUnlockType != SkillUnlockType.None && displayScreen == true)
            {
                List<object> data = new List<object>();
                data.Add(skillUnlockType);
                (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.SkillUnlock, true, data);
            }
        }

        public void UnlockControls()
        {
            m_lockControls = false;
        }

        public void StartShake(GameObj obj, float shakeDuration)
        {
            m_shakeDuration = shakeDuration;
            m_shakeObj = obj;
            m_shakeTimer = m_shakeDelay;
            m_shookLeft = false;
        }

        public void EndShake()
        {
            if (m_shookLeft == true)
                m_shakeObj.X += m_shakeAmount;

            m_shakeObj = null;
            m_shakeTimer = 0;
        }

        public void FadingComplete()
        {
            m_fadingIn = false;
        }

        public override void Update(GameTime gameTime)
        {
            // Special Handling for secret trait.
            if (m_cameraTweening == false)
            {
                if (m_selectedTraitIndex != new Vector2(7, 1) && Camera.Y != (720 / 2f))
                {
                    m_cameraTweening = true;
                    Tween.To(Camera, 0.5f, Quad.EaseOut, "Y", (720 / 2f).ToString());
                    Tween.AddEndHandlerToLastTween(this, "EndCameraTween");
                }
            }

            float elapsedTime = (float)(gameTime.ElapsedGameTime.TotalSeconds);

            if (m_cloud1.Bounds.Right < -100)
                m_cloud1.Position = new Vector2(CDGMath.RandomInt(1320 + 100, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            if (m_cloud2.Bounds.Right < -100)
                m_cloud2.Position = new Vector2(CDGMath.RandomInt(1320 + 100, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            if (m_cloud3.Bounds.Right < -100)
                m_cloud3.Position = new Vector2(CDGMath.RandomInt(1320 + 100, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            if (m_cloud4.Bounds.Right < -100)
                m_cloud4.Position = new Vector2(CDGMath.RandomInt(1320 + 100, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));
            if (m_cloud5.Bounds.Right < -100)
                m_cloud5.Position = new Vector2(CDGMath.RandomInt(1320 + 100, 1320 + 200), CDGMath.RandomInt(0, 720 / 2));

            m_cloud1.X -= 20 * elapsedTime;
            m_cloud2.X -= 16 * elapsedTime;
            m_cloud3.X -= 15 * elapsedTime;
            m_cloud4.X -= 5 * elapsedTime;
            m_cloud5.X -= 10 * elapsedTime;

            if (m_shakeDuration > 0)
            {
                m_shakeDuration -= elapsedTime;
                if (m_shakeTimer > 0 && m_shakeObj != null)
                {
                    m_shakeTimer -= elapsedTime;
                    if (m_shakeTimer <= 0)
                    {
                        m_shakeTimer = m_shakeDelay;
                        if (m_shookLeft == true)
                        {
                            m_shookLeft = false;
                            m_shakeObj.X += m_shakeAmount;
                        }
                        else
                        {
                            m_shakeObj.X -= m_shakeAmount;
                            m_shookLeft = true;
                        }
                    }
                }
            }

            if (m_shakeScreen == true)
                UpdateShake();

            base.Update(gameTime);
        }

        public override void HandleInput()
        {
            if (m_cameraTweening == false && m_lockControls == false)
            {
                bool toggledIcons = false;

                if (Game.GlobalInput.JustPressed(InputMapType.MENU_MAP))
                {
                    if (SkillSystem.IconsVisible == true)
                    {
                        m_toggleIconsText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_6_NEW", m_toggleIconsText);
                        //m_toggleIconsText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_6");
                        m_confirmText.Visible = false;
                        m_continueText.Visible = false;
                        m_navigationText.Visible = false;

                        SkillSystem.HideAllIcons();
                        m_selectionIcon.Opacity = 0;
                        m_dialoguePlate.Opacity = 0;
                        m_descriptionDivider.Opacity = 0;
                        m_coinIcon.Opacity = 0;
                        m_playerMoney.Opacity = 0;
                        m_titleText.Opacity = 0;
                    }
                    else
                    {
                        m_toggleIconsText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_2_NEW", m_toggleIconsText);
                        //m_toggleIconsText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_2");
                        m_confirmText.Visible = true;
                        m_continueText.Visible = true;
                        m_navigationText.Visible = true;

                        SkillSystem.ShowAllIcons();
                        m_selectionIcon.Opacity = 1;
                        m_dialoguePlate.Opacity = 1;
                        m_descriptionDivider.Opacity = 1;
                        m_coinIcon.Opacity = 1;
                        m_playerMoney.Opacity = 1;
                        m_titleText.Opacity = 1;
                    }
                    toggledIcons = true;
                }

                if (SkillSystem.IconsVisible == true)
                {
                    Vector2 previousTraitIndex = m_selectedTraitIndex;
                    //m_descriptionText.Visible = false;
                    //m_dialoguePlate.Visible = false;

                    Vector2 traitSelected = new Vector2(-1, -1);


                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    {
                        traitSelected = SkillSystem.GetSkillLink((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y).TopLink;

                        // Special handling for the secret trait.
                        SkillObj secretSkill = SkillSystem.GetSkill(SkillType.SuperSecret);
                        if (m_cameraTweening == false && secretSkill.Visible == true)
                        {
                            if (traitSelected == new Vector2(7, 1))
                            {
                                m_cameraTweening = true;
                                Tween.To(Camera, 0.5f, Quad.EaseOut, "Y", ((720 / 2f - 300)).ToString());
                                Tween.AddEndHandlerToLastTween(this, "EndCameraTween");
                            }
                        }
                    }
                    else if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2)))
                    {
                        traitSelected = SkillSystem.GetSkillLink((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y).BottomLink;
                    }

                    if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_LEFT2)))
                    {
                        traitSelected = SkillSystem.GetSkillLink((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y).LeftLink;
                    }
                    else if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_RIGHT2)))
                    {
                        traitSelected = SkillSystem.GetSkillLink((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y).RightLink;
                    }

                    if (traitSelected.X != -1 && traitSelected.Y != -1)
                    {
                        SkillObj traitToCheck = SkillSystem.GetSkill((int)traitSelected.X, (int)traitSelected.Y);
                        if (traitToCheck.TraitType != SkillType.Null && traitToCheck.Visible == true)
                            m_selectedTraitIndex = traitSelected;
                    }

                    if (previousTraitIndex != m_selectedTraitIndex)
                    {
                        SkillObj trait = SkillSystem.GetSkill((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y);
                        m_selectionIcon.Position = SkillSystem.GetSkillPosition(trait);

                        UpdateDescriptionPlate(trait);
                        SoundManager.PlaySound("ShopMenuMove");

                        trait.Scale = new Vector2(1.1f, 1.1f);
                        Tween.To(trait, 0.1f, Back.EaseOutLarge, "ScaleX", "1", "ScaleY", "1");

                        m_dialoguePlate.Visible = true;
                    }

                    SkillObj selectedTrait = SkillSystem.GetSkill((int)m_selectedTraitIndex.X, (int)m_selectedTraitIndex.Y);
                    if ((Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3)) 
                        && Game.PlayerStats.Gold >= selectedTrait.TotalCost && selectedTrait.CurrentLevel < selectedTrait.MaxLevel)
                    { //TEDDY SWITCHED XP TO GOLD HERE TOO.
                        SoundManager.PlaySound("TraitUpgrade");
                        if (m_fadingIn == false)
                        {
                            //TEDDY  TEST TO MAKE EVERYTHING COST GOLD //Game.PlayerStats.XP -= selectedTrait.TotalCost;
                            Game.PlayerStats.Gold -= selectedTrait.TotalCost;
                            SetVisible(selectedTrait, true);
                            SkillSystem.LevelUpTrait(selectedTrait, true);

                            if (selectedTrait.CurrentLevel >= selectedTrait.MaxLevel)
                                SoundManager.PlaySound("TraitMaxxed");
                            //(ScreenManager.Game as Game).SaveManager.SaveGame(SaveGameManager.SaveType.UpgradeData, null);
                            //Tweener.Tween.RunFunction(0.5f, (ScreenManager.Game as Game).SaveManager, "SaveGame", SaveGameManager.SaveType.PlayerData, typeof(ProceduralLevelScreen));
                            //(ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.UpgradeData, SaveType.PlayerData);
                            UpdateDescriptionPlate(selectedTrait);
                        }
                    }
                    else if ((Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3)) 
                        && Game.PlayerStats.Gold < selectedTrait.TotalCost)
                        SoundManager.PlaySound("TraitPurchaseFail"); //TEDDY SWITCHED XP TO GOLD HERE TOO

                    if ((Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)  || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3)) && toggledIcons == false)
                    {
                        m_lockControls = true;
                        RCScreenManager screenManager = ScreenManager as RCScreenManager;
                        ProceduralLevelScreen levelScreen = screenManager.GetLevelScreen();
                        levelScreen.Reset();
                        //screenManager.HideCurrentScreen();
                        if (levelScreen.CurrentRoom is StartingRoomObj)
                        {
                            screenManager.StartWipeTransition();
                            Tween.RunFunction(0.2f, screenManager, "HideCurrentScreen");
                            Tween.RunFunction(0.2f, levelScreen.CurrentRoom, "OnEnter");
                            //levelScreen.CurrentRoom.OnEnter(); // Hack to force starting room OnEnter() when exiting trait screen.
                        }
                        else
                            (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.StartingRoom, true, null);

                        //(ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Lineage, true);
                    }

                    if (LevelEV.ENABLE_DEBUG_INPUT == true)
                        HandleDebugInput();

                    //if (InputManager.JustPressed(Keys.H, null))
                    //    ZoomOutAllObjects();
                }
                base.HandleInput();
            }
        }

        private void HandleDebugInput()
        {
            if (InputManager.JustPressed(Keys.Q, PlayerIndex.One))
            {
                foreach (SkillObj skill in SkillSystem.SkillArray)
                {
                    if (skill.CurrentLevel < skill.MaxLevel)
                    {
                        SetVisible(skill, false);
                        SkillSystem.LevelUpTrait(skill, false);
                        CheckForSkillUnlock(skill, false);
                    }
                }
            }
        }

        public void EndCameraTween()
        {
            m_cameraTweening = false;
        }

        public void UpdateDescriptionPlate(SkillObj trait)
        {
            /* Dialogue plate object order
             * 0 = Plate BG
             * 1 = Skill Icon
             * 2 = Skill Title
             * 3 = Skill Description
             * 4 = Input Description
             * 5 = Stat Base Amount
             * 6 = Stat Increase Amount
             * 7 = Skill Cost
             */
            string spriteName = trait.IconName;
            spriteName = spriteName.Replace("Locked", "");
            spriteName = spriteName.Replace("Max", "");

            m_skillIcon.ChangeSprite(spriteName);
            m_skillTitle.Text = LocaleBuilder.getResourceString(trait.NameLocID);
            m_skillDescription.Text = LocaleBuilder.getResourceString(trait.DescLocID);
            m_skillDescription.WordWrap(280);
            m_inputDescription.Text = LocaleBuilder.getResourceString(trait.InputDescLocID);
            //m_inputDescription.Text = LocaleBuilder.getResourceString(trait.InputDescLocIDs[0]) + trait.InputDescLocIDs[1] + LocaleBuilder.getResourceString(trait.InputDescLocIDs[2]);
            m_inputDescription.WordWrap(280);
            m_inputDescription.Y = m_skillDescription.Bounds.Bottom + 10;

            float statBaseAmount = TraitStatType.GetTraitStat(trait.TraitType);
            if (statBaseAmount > -1)
            {
                // Setting the text for the current base data for the skil
                // Hack added to make sure crit chance up and gold gain up stats are displayed correctly.
                if (statBaseAmount < 1 || trait.TraitType == SkillType.Gold_Gain_Up || trait.TraitType == SkillType.Crit_Chance_Up) // Converting statBaseAmount to a percentage.
                {
                    statBaseAmount *= 100;
                    statBaseAmount = (int)(Math.Round(statBaseAmount, MidpointRounding.AwayFromZero));
                }

                if (statBaseAmount == 0)
                {
                    statBaseAmount = trait.ModifierAmount;

                    // Special handling for crit chance up.
                    if (trait.TraitType == SkillType.Crit_Chance_Up)
                    {
                        statBaseAmount *= 100;
                        statBaseAmount = (int)(Math.Round(statBaseAmount, MidpointRounding.AwayFromZero));
                    }
                }

                string colon = ": ";
                switch (LocaleBuilder.languageType)
                {
                    case(LanguageType.French):
                        colon = " : ";
                        break;
                }
                m_skillCurrent.Text = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_7") + colon + statBaseAmount + trait.UnitLocIDs[0] + LocaleBuilder.getResourceString(trait.UnitLocIDs[1]);

                // Setting the text for the upgrade data for the skill.
                if (trait.CurrentLevel < trait.MaxLevel)
                {
                    float traitPerLevelModifier = trait.PerLevelModifier;
                    if (traitPerLevelModifier < 1 && trait.TraitType != SkillType.Invuln_Time_Up) // Converting trait's per level modifier to percentage.
                    {
                        traitPerLevelModifier *= 100;
                        if (trait.TraitType != SkillType.Death_Dodge)
                            traitPerLevelModifier = (int)(Math.Round(traitPerLevelModifier, MidpointRounding.AwayFromZero));
                    }
                    m_skillUpgrade.Text = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_8") + colon + "+" + traitPerLevelModifier + trait.UnitLocIDs[0] + LocaleBuilder.getResourceString(trait.UnitLocIDs[1]);
                }
                else
                    m_skillUpgrade.Text = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_8") + colon + "--";

                // Setting current skill level text.
                m_skillLevel.Text = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_9") + colon + trait.CurrentLevel + "/" + trait.MaxLevel;

                // Changeds periods to commas for decimal values in certain languages (i.e. 1.5 = 1,5).
                switch (LocaleBuilder.languageType)
                {
                    case(LanguageType.French):
                        m_skillCurrent.Text = m_skillCurrent.Text.Replace('.', ',');
                        m_skillUpgrade.Text = m_skillUpgrade.Text.Replace('.', ',');
                        break;
                }

                // Setting skill cost
                string upgrade = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_11"); //unlock
                if (trait.CurrentLevel > 0)
                    upgrade = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_12"); //upgrade
                m_skillCost.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_10_NEW"), trait.TotalCost.ToString(), upgrade);

                // Positioning the text.
                if (m_inputDescription.Text != " " && m_inputDescription.Text != "")
                    m_skillCurrent.Y = m_inputDescription.Bounds.Bottom + 40;
                else
                    m_skillCurrent.Y = m_skillDescription.Bounds.Bottom + 40;

                m_skillUpgrade.Y = m_skillCurrent.Y + 30;
                m_skillLevel.Y = m_skillUpgrade.Y + 30;
                m_descriptionDivider.Visible = true;
            }
            else
            {
                m_skillCurrent.Text = "";
                m_skillUpgrade.Text = "";
                m_skillLevel.Text = "";
                m_descriptionDivider.Visible = false;

                string upgrade = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_11"); //unlock
                if (trait.CurrentLevel > 0)
                    upgrade = LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_12"); //upgrade
                m_skillCost.Text = string.Format(LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_10_NEW"), trait.TotalCost.ToString(), upgrade);

            }

            m_descriptionDivider.Position = new Vector2(m_skillCurrent.AbsX, m_skillCurrent.AbsY - 20);

            if (trait.CurrentLevel >= trait.MaxLevel)
            {
                m_skillCost.Visible = false;
                m_skillCostBG.Visible = false;
            }
            else
            {
                m_skillCost.Visible = true;
                m_skillCostBG.Visible = true;
            }

            //if (trait.CurrentLevel < trait.MaxLevel)
            //    (m_dialoguePlate.GetChildAt(7) as TextObj).Text = "Costs " + trait.TotalCost + " Gold";//(m_dialoguePlate.GetChildAt(7) as TextObj).Text = "Costs " + trait.TotalCost + " Tribute"; //TEDDY REPLACING IT TO DISPLAY GOLD
            //else
            //    (m_dialoguePlate.GetChildAt(7) as TextObj).Text = "Skill Mastered";

            ////if (trait.DisplayStat== true)
            //    (m_dialoguePlate.GetChildAt(7) as TextObj).Y = m_dialoguePlate.GetChildAt(5).Bounds.Bottom + 0;
            ////else
            //    //(m_dialoguePlate.GetChildAt(7) as TextObj).Y = m_dialoguePlate.GetChildAt(4).Bounds.Bottom + 0;

            //m_dialoguePlate.GetChildAt(3).Y = m_dialoguePlate.Height / 2f - m_dialoguePlate.GetChildAt(3).Height - 100;

            m_playerMoney.Text = Game.PlayerStats.Gold.ToString();

            m_lastSkillObj = trait;
            LanguageFormatLastSkill();
        }

        private void LanguageFormatLastSkill()
        {
            m_skillTitle.ScaleX = 1;
            m_skillTitle.FontSize = 12;
            switch (LocaleBuilder.languageType)
            {
                case (LanguageType.French):
                    switch (m_lastSkillObj.TraitType)
                    {
                        case (SkillType.Spellsword_Unlock):
                        case (SkillType.SpellSword_Up):
                            m_skillTitle.ScaleX = 0.9f;
                            m_skillTitle.FontSize = 11;
                            break;
                        case (SkillType.Banker_Unlock):
                        case (SkillType.Banker_Up):
                        case (SkillType.Knight_Up):
                        case (SkillType.Assassin_Up):
                        case (SkillType.Barbarian_Up):
                            m_skillTitle.ScaleX = 0.9f;
                            break;
                    }
                    break;
                case (LanguageType.German):
                    if (m_lastSkillObj.TraitType == SkillType.Banker_Unlock || m_lastSkillObj.TraitType == SkillType.Invuln_Time_Up)
                        m_skillTitle.ScaleX = 0.9f;
                    break;
                case (LanguageType.Spanish_Spain):
                    if (m_lastSkillObj.TraitType == SkillType.Equip_Up || m_lastSkillObj.TraitType == SkillType.Spellsword_Unlock || m_lastSkillObj.TraitType == SkillType.SpellSword_Up)
                        m_skillTitle.ScaleX = 0.9f;
                    break;
                case (LanguageType.Portuguese_Brazil):
                    switch (m_lastSkillObj.TraitType)
                    {
                        case (SkillType.Spellsword_Unlock):
                        case (SkillType.SpellSword_Up):
                            m_skillTitle.ScaleX = 0.9f;
                            m_skillTitle.FontSize = 11;
                            break;
                        case (SkillType.Banker_Up):
                        case (SkillType.Mana_Cost_Down):
                        case (SkillType.Down_Strike_Up):
                            m_skillTitle.ScaleX = 0.9f;
                            break;
                    }
                    break;
                case (LanguageType.Polish):
                    if (m_lastSkillObj.TraitType == SkillType.Spellsword_Unlock || m_lastSkillObj.TraitType == SkillType.SpellSword_Up)
                        m_skillTitle.ScaleX = 0.9f;
                    break;
                case (LanguageType.Russian):
                    if (m_lastSkillObj.TraitType == SkillType.Crit_Chance_Up)
                        m_skillTitle.ScaleX = 0.9f;

                    if (m_lastSkillObj.TraitType == SkillType.Crit_Damage_Up)
                        m_skillTitle.ScaleX = 0.8f;

                    break;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            m_cloud1.Y = m_cloud2.Y = m_cloud3.Y = m_cloud4.Y = m_cloud5.Y = Camera.TopLeftCorner.Y * 0.2f;
            m_bg.Y = Camera.TopLeftCorner.Y * 0.2f;
            
            //m_manor.Y = Camera.TopLeftCorner.Y * 0.1f;

            //Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Camera.GetTransformation());
            m_bg.Draw(Camera);
            m_cloud1.Draw(Camera);
            m_cloud2.Draw(Camera);
            m_cloud3.Draw(Camera);
            m_cloud4.Draw(Camera);
            m_cloud5.Draw(Camera);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_manor.Draw(Camera);
            m_impactEffectPool.Draw(Camera);

            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            m_selectionIcon.Draw(Camera);

            foreach (SkillObj trait in SkillSystem.GetSkillArray())
            {
                if (trait.TraitType != SkillType.Filler && trait.TraitType != SkillType.Null && trait.Visible == true)
                    trait.Draw(Camera);
            }

            Camera.End();

            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);

            m_dialoguePlate.Draw(Camera);

            //m_titleText.Draw(Camera);
            m_continueText.Draw(Camera);
            m_toggleIconsText.Draw(Camera);
            m_confirmText.Draw(Camera);
            m_navigationText.Draw(Camera);

            m_playerMoney.Draw(Camera);

            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_descriptionDivider.Draw(Camera);
            m_coinIcon.Draw(Camera);

            Camera.End();
            base.Draw(gameTime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Skill Screen");

                m_titleText.Dispose();
                m_titleText = null;
                m_bg.Dispose();
                m_bg = null;
                m_cloud1.Dispose();
                m_cloud1 = null;
                m_cloud2.Dispose();
                m_cloud2 = null;
                m_cloud3.Dispose();
                m_cloud3 = null;
                m_cloud4.Dispose();
                m_cloud4 = null;
                m_cloud5.Dispose();
                m_cloud5 = null;

                m_continueText.Dispose();
                m_continueText = null;
                m_toggleIconsText.Dispose();
                m_toggleIconsText = null;
                m_confirmText.Dispose();
                m_confirmText = null;
                m_navigationText.Dispose();
                m_navigationText = null;

                m_dialoguePlate.Dispose();
                m_dialoguePlate = null;

                m_selectionIcon.Dispose();
                m_selectionIcon = null;

                m_impactEffectPool.Dispose();
                m_impactEffectPool = null;

                m_manor.Dispose();
                m_manor = null;

                m_shakeObj = null;

                m_playerMoney.Dispose();
                m_playerMoney = null;
                m_coinIcon.Dispose();
                m_coinIcon = null;

                // These aren't disposed because they're added to m_dialoguePlate, which will dispose them when it is disposed.
                m_skillCurrent = null;
                m_skillCost = null;
                m_skillCostBG = null;
                m_skillDescription = null;
                m_inputDescription = null;
                m_skillUpgrade = null;
                m_skillLevel = null;
                m_skillIcon = null;
                m_skillTitle = null;
                m_descriptionDivider.Dispose();
                m_descriptionDivider = null;
                m_lastSkillObj = null;
                base.Dispose();
            }
        }

        private bool m_horizontalShake;
        private bool m_verticalShake;
        private bool m_shakeScreen;
        private float m_screenShakeMagnitude;

        public void ShakeScreen(float magnitude, bool horizontalShake = true, bool verticalShake = true)
        {
            SoundManager.PlaySound("TowerLand");
            m_screenShakeMagnitude = magnitude;
            m_horizontalShake = horizontalShake;
            m_verticalShake = verticalShake;
            m_shakeScreen = true;
        }

        public void UpdateShake()
        {
            if (m_horizontalShake == true)
            {
                m_bg.X = CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);
                m_manor.X = CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);
            }

            if (m_verticalShake == true)
            {
                m_bg.Y = CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);
                m_manor.Y = CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);

            }
        }

        public void StopScreenShake()
        {
            m_shakeScreen = false;
            m_bg.X = 0;
            m_bg.Y = 0;
            m_manor.X = 0;
            m_manor.Y = 0;
        }

        public override void RefreshTextObjs()
        {
            /*
            m_confirmText.Text = "[Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_1");
            m_toggleIconsText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_2");
            m_continueText.Text = "[Input:" + InputMapType.MENU_CANCEL1 + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_3");

            if (InputManager.GamePadIsConnected(PlayerIndex.One) == true)
                m_navigationText.Text = "[Button:LeftStick] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_4");
            else
                m_navigationText.Text = LocaleBuilder.getString("LOC_ID_SKILL_SCREEN_5", m_navigationText);

            if (SkillSystem.IconsVisible == true)
                m_toggleIconsText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_6");
            else
                m_toggleIconsText.Text = "[Input:" + InputMapType.MENU_MAP + "] " + LocaleBuilder.getResourceString("LOC_ID_SKILL_SCREEN_2");
             */
            SkillSystem.RefreshTextObjs();

            if (m_lastSkillObj != null) 
                UpdateDescriptionPlate(m_lastSkillObj);

            base.RefreshTextObjs();
        }
    }
}

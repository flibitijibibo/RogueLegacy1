using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class SkillUnlockScreen : Screen
    {
        public float BackBufferOpacity { get; set; }

        private ObjContainer m_plate;
        private ObjContainer m_picturePlate;
        private SpriteObj m_picture;
        private TextObj m_text;

        private SpriteObj m_titlePlate;
        private SpriteObj m_title;

        private byte m_skillUnlockType;

        public SkillUnlockScreen()
        {

        }

        public override void LoadContent()
        {
            m_plate = new ObjContainer("SkillUnlockPlate_Character");
            m_plate.Position = new Vector2(1320 / 2f, 720 / 2f);
            m_plate.ForceDraw = true;

            m_text = new TextObj(Game.JunicodeFont);
            m_text.ForceDraw = true;
            m_text.Text = "This is temporary text to see how many sentences can be fit into this area.  Hopefully it is quite a bit, as this is the area where we will be comprehensively describing the unlock.  It will also be used to determine word wrap length.";
            m_text.FontSize = 10;
            m_text.WordWrap(340);
            m_text.DropShadow = new Vector2(2, 2);
            m_plate.AddChild(m_text);
            m_text.Position = new Vector2(-110, -100);

            m_picturePlate = new ObjContainer("SkillUnlockPicturePlate_Character");
            m_picturePlate.Position = new Vector2(1320/2f - 300, 720/2f + 50);
            m_picturePlate.Rotation = -15;
            m_picturePlate.ForceDraw = true;

            m_picture = new SpriteObj("BlacksmithUnlockPicture_Sprite");
            m_picture.ForceDraw = true;
            m_picture.OutlineWidth = 1;
            m_picture.Position = m_picturePlate.Position;

            m_titlePlate = new SpriteObj("SkillUnlockTitlePlate_Sprite");
            m_titlePlate.Position = new Vector2(1320 / 2f, 720 / 2f - 200); 
            m_titlePlate.ForceDraw = true;

            m_title = new SpriteObj("ClassUnlockedText_Sprite");
            m_title.Position = m_titlePlate.Position;
            m_title.Y -=40;
            m_title.ForceDraw = true;

            base.LoadContent();
        }

        public override void PassInData(List<object> objList)
        {
            m_skillUnlockType = (byte)(objList[0]);
        }

        public override void OnEnter()
        {
            SoundManager.PlaySound("Upgrade_Splash_In");

            SetData();

            BackBufferOpacity = 0;

            m_plate.Scale = Vector2.Zero;
            //m_picturePlate.Scale = Vector2.Zero;
            m_titlePlate.Scale = Vector2.Zero;
            m_title.Scale = Vector2.Zero;

            Tween.To(this, 0.2f, Tween.EaseNone, "BackBufferOpacity", "0.7");

            Tween.To(m_titlePlate, 0.5f, Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
            Tween.To(m_title, 0.5f, Back.EaseOut, "delay", "0.1", "ScaleX", "1", "ScaleY", "1");

            Tween.To(m_plate, 0.5f, Back.EaseOut, "delay", "0.3", "ScaleX", "1", "ScaleY", "1");
            //Tween.To(m_picturePlate, 0.5f, Back.EaseOut, "delay", "0.4", "ScaleX", "1", "ScaleY", "1");

            // Different animation.
            m_picturePlate.Scale = new Vector2(2, 2);
            m_picturePlate.Opacity = 0;
            m_picturePlate.Rotation = 0;

            Tween.To(m_picturePlate, 0.3f, Tween.EaseNone, "delay", "0.4", "ScaleX", "1", "ScaleY", "1", "Rotation", "-15");
            Tween.To(m_picturePlate, 0.1f, Tween.EaseNone, "delay", "0.4", "Opacity", "1");

            m_picture.Scale = new Vector2(2, 2);
            m_picture.Opacity = 0;
            m_picture.Rotation = 0;

            Tween.To(m_picture, 0.3f, Tween.EaseNone, "delay", "0.4", "ScaleX", "1", "ScaleY", "1", "Rotation", "-15");
            Tween.To(m_picture, 0.1f, Tween.EaseNone, "delay", "0.4", "Opacity", "1");

            RefreshTextObjs();

            base.OnEnter();
        }

        private void SetData()
        {
            switch (m_skillUnlockType)
            {
                case (SkillUnlockType.Blacksmith):
                    m_picture.ChangeSprite("BlacksmithUnlockPicture_Sprite");
                    m_title.ChangeSprite("SmithyUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Enchantress):
                    m_picture.ChangeSprite("EnchantressUnlockPicture_Sprite");
                    m_title.ChangeSprite("EnchantressUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Architect):
                    m_picture.ChangeSprite("ArchitectUnlockPicture_Sprite");
                    m_title.ChangeSprite("ArchitectUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Ninja):
                    m_picture.ChangeSprite("NinjaUnlockPicture_Sprite");
                    m_title.ChangeSprite("ClassUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Banker):
                    m_picture.ChangeSprite("BankerUnlockPicture_Sprite");
                    m_title.ChangeSprite("ClassUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Lich):
                    m_picture.ChangeSprite("LichUnlockPicture_Sprite");
                    m_title.ChangeSprite("ClassUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.SpellSword):
                    m_picture.ChangeSprite("SpellSwordUnlockPicture_Sprite");
                    m_title.ChangeSprite("ClassUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.KnightUp):
                    m_picture.ChangeSprite("KnightUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.WizardUp):
                    m_picture.ChangeSprite("MageUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.BarbarianUp):
                    m_picture.ChangeSprite("BarbarianUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.AssassinUp):
                    m_picture.ChangeSprite("AssassinUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.LichUp):
                    m_picture.ChangeSprite("LichUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.SpellSwordUp):
                    m_picture.ChangeSprite("SpellSwordUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.NinjaUp):
                    m_picture.ChangeSprite("NinjaUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.BankerUp):
                    m_picture.ChangeSprite("BankerUpgradePicture_Sprite");
                    m_title.ChangeSprite("ClassUpgradedText_Sprite");
                    break;
                case (SkillUnlockType.Dragon):
                    m_picture.ChangeSprite("DragonUnlockPicture_Sprite");
                    m_title.ChangeSprite("ClassUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Traitor):
                    m_picture.ChangeSprite("TraitorUnlockPicture_Sprite");
                    m_title.ChangeSprite("ClassUnlockedText_Sprite");
                    break;
            }

            m_text.Text = LocaleBuilder.getString(SkillUnlockType.DescriptionID(m_skillUnlockType), m_text);
            m_text.WordWrap(340);
        }

        private void ExitTransition()
        {
            SoundManager.PlaySound("Upgrade_Splash_Out");

            Tween.To(m_picture, 0.5f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            Tween.To(m_picturePlate, 0.5f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            Tween.To(m_titlePlate, 0.5f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            Tween.To(m_title, 0.5f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
            Tween.To(m_plate, 0.5f, Back.EaseIn, "ScaleX", "0", "ScaleY", "0");

            Tween.To(this, 0.2f, Tween.EaseNone, "delay", "0.4", "BackBufferOpacity", "0");

            Tween.AddEndHandlerToLastTween(ScreenManager, "HideCurrentScreen");
        }

        public override void HandleInput()
        {
            if (m_plate.ScaleX == 1)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2)
                    || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                     || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    ExitTransition();
            }

            base.HandleInput();
        }

        public override void Draw(GameTime gametime)
        {
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight), Color.Black * BackBufferOpacity);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            m_plate.Draw(Camera);
            m_titlePlate.Draw(Camera);
            m_title.Draw(Camera);
            m_picturePlate.Draw(Camera);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            m_picture.Draw(Camera);
            Camera.End();
            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing SkillUnlock Screen");

                m_picturePlate.Dispose();
                m_picturePlate = null;
                m_picture.Dispose();
                m_picture = null;
                m_text = null;
                m_title.Dispose();
                m_title = null;
                m_titlePlate.Dispose();
                m_titlePlate = null;
                m_plate.Dispose();
                m_plate = null;

                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            RefreshBitmaps();

            m_text.ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_text));
            m_text.Text = LocaleBuilder.getString(SkillUnlockType.DescriptionID(m_skillUnlockType), null);
            m_text.WordWrap(340);

            base.RefreshTextObjs();
        }

        private void RefreshBitmaps()
        {
            switch (m_skillUnlockType)
            {
                case (SkillUnlockType.Blacksmith):
                    Game.ChangeBitmapLanguage(m_title, "SmithyUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Enchantress):
                    Game.ChangeBitmapLanguage(m_title, "EnchantressUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Architect):
                    Game.ChangeBitmapLanguage(m_title, "ArchitectUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.Ninja):
                case (SkillUnlockType.Banker):
                case (SkillUnlockType.Lich):
                case (SkillUnlockType.SpellSword):
                case (SkillUnlockType.Dragon):
                case (SkillUnlockType.Traitor):
                    Game.ChangeBitmapLanguage(m_title, "ClassUnlockedText_Sprite");
                    break;
                case (SkillUnlockType.KnightUp):
                case (SkillUnlockType.WizardUp):
                case (SkillUnlockType.BarbarianUp):
                case (SkillUnlockType.AssassinUp):
                case (SkillUnlockType.LichUp):
                case (SkillUnlockType.SpellSwordUp):
                case (SkillUnlockType.NinjaUp):
                case (SkillUnlockType.BankerUp):
                    Game.ChangeBitmapLanguage(m_title, "ClassUpgradedText_Sprite");
                    break;
            }
        }
    }
}

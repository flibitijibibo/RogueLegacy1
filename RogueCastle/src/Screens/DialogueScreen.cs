using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace RogueCastle
{
    
    public class DialogueScreen : Screen
    {
        private ObjContainer m_dialogContainer;
        private byte m_dialogCounter;
        private string[] m_dialogTitles;
        private string[] m_dialogText;
        private float m_dialogContinueIconY;
        private string m_dialogueObjName;
        public float BackBufferOpacity { get; set; }

        // Variables for endhandlers
        private MethodInfo m_confirmMethodInfo;
        private object m_confirmMethodObj;
        private object[] m_confirmArgs;

        private MethodInfo m_cancelMethodInfo;
        private object m_cancelMethodObj;
        private object[] m_cancelArgs;

        // Variables for dialogue choice system
        private bool m_runChoiceDialogue = false;
        private ObjContainer m_dialogChoiceContainer;
        private bool m_runCancelEndHandler = false;
        private byte m_highlightedChoice = 2;
        private bool m_lockControls = false;

        private float m_textScrollSpeed = 0.03f;

        private float m_inputDelayTimer = 0; // The timer that makes sure you can't press inputs too soon.
        private bool m_forceMaleDialogue = false;

        private object[] m_stringFormatArgs;

        public DialogueScreen()
        {
        }

        public override void LoadContent()
        {
            TextObj dialogText = new TextObj(Game.JunicodeFont);
            dialogText.FontSize = 12;
            dialogText.Align = Types.TextAlign.Left;

            TextObj dialogSpeakerText = new TextObj(Game.JunicodeFont);
            dialogSpeakerText.FontSize = 14;
            dialogSpeakerText.Text = "Blacksmith";
            dialogSpeakerText.DropShadow = new Vector2(2, 2);
            dialogSpeakerText.TextureColor = new Color(236, 197, 132);

            m_dialogContainer = new ObjContainer("DialogBox_Character");
            //m_dialogContainer.Position = new Vector2(1320 / 2, m_dialogContainer.Height);
            m_dialogContainer.Position = new Vector2(1320 / 2, 100);
            m_dialogContainer.AddChild(dialogSpeakerText);
            m_dialogContainer.ForceDraw = true;
            dialogSpeakerText.Position = new Vector2(-m_dialogContainer.Width / 2.2f, -m_dialogContainer.Height / 1.6f);

            m_dialogContainer.AddChild(dialogText);
            dialogText.Position = new Vector2(-m_dialogContainer.Width / 2.15f, -m_dialogContainer.Height / 3.5f);
            dialogText.Text = "This is a test to see how much text I can fit onto this dialog box without it running out of space. The text " +
            "needs to be defined after the dialog text position is set, because the dialogtext width affects the entire width of the dialog container, " +
            "which in END.";
            dialogText.WordWrap(850);
            dialogText.DropShadow = new Vector2(2, 3);

            SpriteObj continueTextIcon = new SpriteObj("ContinueTextIcon_Sprite");
            continueTextIcon.Position = new Vector2(m_dialogContainer.GetChildAt(2).Bounds.Right, m_dialogContainer.GetChildAt(2).Bounds.Bottom);
            m_dialogContainer.AddChild(continueTextIcon);
            m_dialogContinueIconY = continueTextIcon.Y;
            //m_dialogContainer.Scale = Vector2.Zero;

            TextObj choice1Text = new TextObj(Game.JunicodeFont);
            choice1Text.FontSize = 12;
            choice1Text.Text = "Yes";
            choice1Text.Align = Types.TextAlign.Centre;
            TextObj choice2Text = new TextObj(Game.JunicodeFont);
            choice2Text.FontSize = 12;
            choice2Text.Text = "No";
            choice2Text.Align = Types.TextAlign.Centre;

            m_dialogChoiceContainer = new ObjContainer();

            SpriteObj statPlate = new SpriteObj("GameOverStatPlate_Sprite");
            m_dialogChoiceContainer.AddChild(statPlate);
            SpriteObj choiceHighlight = new SpriteObj("DialogueChoiceHighlight_Sprite");
            m_dialogChoiceContainer.AddChild(choiceHighlight);
            m_dialogChoiceContainer.ForceDraw = true;
            m_dialogChoiceContainer.Position = new Vector2(1320 / 2, 720/2);

            m_dialogChoiceContainer.AddChild(choice1Text);
            choice1Text.Y -= 40;
            m_dialogChoiceContainer.AddChild(choice2Text);
            choice2Text.Y += 7;
            choiceHighlight.Position = new Vector2(choice1Text.X, choice1Text.Y + choiceHighlight.Height / 2 + 3);
            m_dialogChoiceContainer.Scale = Vector2.Zero;
            m_dialogChoiceContainer.Visible = false;

            base.LoadContent();
        }

        public void SetDialogue(string dialogueObjName)
        {
            m_dialogueObjName = dialogueObjName;

            m_forceMaleDialogue = false;
            //if (dialogueObjName == "Meet Blacksmith" || dialogueObjName.Contains("DiaryEntry"))
            if (dialogueObjName.Contains("DiaryEntry"))
                m_forceMaleDialogue = true;

            m_confirmMethodObj = null;
            m_confirmMethodInfo = null;
            if (m_confirmArgs != null)
                Array.Clear(m_confirmArgs, 0, m_confirmArgs.Length);

            m_cancelMethodObj = null;
            m_cancelMethodInfo = null;
            if (m_cancelArgs != null)
                Array.Clear(m_cancelArgs, 0, m_cancelArgs.Length);

            if (m_stringFormatArgs != null)
                Array.Clear(m_stringFormatArgs, 0, m_stringFormatArgs.Length);
            m_stringFormatArgs = null;
        }

        public void SetDialogue(string dialogueObjName, params object[] args)
        {
            SetDialogue(dialogueObjName);
            m_stringFormatArgs = args; // Must be called after SetDialogue(string).
        }

        public void SetConfirmEndHandler(object methodObject, string functionName, params object[] args)
        {
            m_confirmMethodObj = methodObject;
            m_confirmMethodInfo = methodObject.GetType().GetMethod(functionName);
            m_confirmArgs = args;
        }

        public void SetConfirmEndHandler(Type methodType, string functionName, params object[] args)
        {
            Type[] argList = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
                argList[i] = args[i].GetType();

            m_confirmMethodInfo = methodType.GetMethod(functionName, argList);
            m_confirmArgs = args;

            if (m_confirmMethodInfo == null)
            {
                m_confirmMethodInfo = methodType.GetMethod(functionName, new Type[] { args[0].GetType().MakeArrayType() });
                m_confirmArgs = new object[1];
                m_confirmArgs[0] = args;
            }

            m_confirmMethodObj = null;
        }

        public void SetCancelEndHandler(object methodObject, string functionName, params object[] args)
        {
            m_cancelMethodObj = methodObject;
            m_cancelMethodInfo = methodObject.GetType().GetMethod(functionName);
            m_cancelArgs = args;
        }

        public void SetCancelEndHandler(Type methodType, string functionName, params object[] args)
        {
            Type[] argList = new Type[args.Length];
            for (int i = 0; i < args.Length; i++)
                argList[i] = args[i].GetType();

            m_cancelMethodInfo = methodType.GetMethod(functionName, argList);
            m_cancelArgs = args;

            if (m_cancelMethodInfo == null)
            {
                m_cancelMethodInfo = methodType.GetMethod(functionName, new Type[] { args[0].GetType().MakeArrayType() });
                m_cancelArgs = new object[1];
                m_cancelArgs[0] = args;
            }

            m_cancelMethodObj = null;
        }

        public void SetDialogueChoice(string dialogueObjName)
        {
            DialogueObj choiceDialogue = DialogueManager.GetText(dialogueObjName);
            (m_dialogChoiceContainer.GetChildAt(2) as TextObj).Text = LocaleBuilder.getString(choiceDialogue.Speakers[0], m_dialogChoiceContainer.GetChildAt(2) as TextObj);
            (m_dialogChoiceContainer.GetChildAt(3) as TextObj).Text = LocaleBuilder.getString(choiceDialogue.Dialogue[0], m_dialogChoiceContainer.GetChildAt(3) as TextObj);

            if (Game.PlayerStats.Traits.X == TraitType.Dyslexia || Game.PlayerStats.Traits.Y == TraitType.Dyslexia)
            {
                (m_dialogChoiceContainer.GetChildAt(2) as TextObj).RandomizeSentence(false);
                (m_dialogChoiceContainer.GetChildAt(3) as TextObj).RandomizeSentence(false);
            }

            m_runChoiceDialogue = true;
        }

        public override void HandleInput()
        {
            if (m_lockControls == false && m_inputDelayTimer <= 0)
            {
                if (m_dialogChoiceContainer.Visible == false)
                {
                    //    // Special code in case you start the dialogue with a choice sequence, instead of showing dialogue, pressing confirm, then showing the choice.
                    //    if (m_dialogCounter == m_dialogText.Length - 1 && m_runChoiceDialogue == true)
                    //    {
                    //        SpriteObj continueTextIcon1 = m_dialogContainer.GetChildAt(3) as SpriteObj;
                    //        continueTextIcon1.ChangeSprite("EndTextIcon_Sprite");
                    //        if (m_runChoiceDialogue == true)
                    //        {
                    //            TextObj dialogueText = (m_dialogContainer.GetChildAt(2) as TextObj);
                    //            dialogueText.StopTypeWriting(true);
                    //            m_dialogChoiceContainer.Visible = true;
                    //            Tweener.Tween.To(m_dialogChoiceContainer, 0.3f, Tweener.Ease.Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
                    //        }
                    //    }

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) 
                        || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                         || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    {
                        if (m_dialogCounter < m_dialogText.Length - 1)
                        {
                            TextObj dialogueTitle = (m_dialogContainer.GetChildAt(1) as TextObj);
                            TextObj dialogueText = (m_dialogContainer.GetChildAt(2) as TextObj);
                            if (dialogueText.IsTypewriting == false)
                            {
                                m_dialogCounter++;
                                dialogueTitle.Text = LocaleBuilder.getString(m_dialogTitles[m_dialogCounter], dialogueTitle, m_forceMaleDialogue);
                                if (m_stringFormatArgs != null)
                                    dialogueText.Text = string.Format(LocaleBuilder.getString(m_dialogText[m_dialogCounter], dialogueText, m_forceMaleDialogue), m_stringFormatArgs);
                                else
                                    dialogueText.Text = LocaleBuilder.getString(m_dialogText[m_dialogCounter], dialogueText, m_forceMaleDialogue);
                                if (Game.PlayerStats.Traits.X == TraitType.Dyslexia || Game.PlayerStats.Traits.Y == TraitType.Dyslexia)
                                {
                                    dialogueTitle.RandomizeSentence(false);
                                    dialogueText.RandomizeSentence(false);
                                }
                                dialogueText.WordWrap(850);
                                dialogueText.BeginTypeWriting(m_dialogText[m_dialogCounter].Length * m_textScrollSpeed, "dialogue_tap");
                            }
                            else
                                dialogueText.StopTypeWriting(true);
                        }
                        else
                        {
                            if (m_runChoiceDialogue == false && (m_dialogContainer.GetChildAt(2) as TextObj).IsTypewriting == false)
                            {
                                m_lockControls = true;
                                SoundManager.PlaySound("DialogMenuClose");

                                //Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                                Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Quad.EaseIn, "Opacity", "0", "Y", "0");
                                Tweener.Tween.To(this, 0.3f, Tweener.Ease.Linear.EaseNone, "BackBufferOpacity", "0");
                                Tweener.Tween.AddEndHandlerToLastTween(this, "ExitScreen");
                            }
                            else
                                (m_dialogContainer.GetChildAt(2) as TextObj).StopTypeWriting(true);
                        }

                        SpriteObj continueTextIcon = m_dialogContainer.GetChildAt(3) as SpriteObj;
                        if (m_dialogCounter == m_dialogText.Length - 1)
                        {
                            continueTextIcon.ChangeSprite("EndTextIcon_Sprite");
                            if (m_runChoiceDialogue == true)
                            {
                                TextObj dialogueText = (m_dialogContainer.GetChildAt(2) as TextObj);
                                dialogueText.StopTypeWriting(true);
                                m_dialogChoiceContainer.Visible = true;
                                Tweener.Tween.To(m_dialogChoiceContainer, 0.3f, Tweener.Ease.Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
                                SoundManager.PlaySound("DialogOpenBump");
                            }
                        }
                        else
                            continueTextIcon.ChangeSprite("ContinueTextIcon_Sprite");
                    }
                }
                else
                {
                    if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_DOWN2))
                    {
                        SoundManager.PlaySound("frame_swap");
                        m_highlightedChoice++;
                        if (m_highlightedChoice > 3)
                            m_highlightedChoice = 2;
                        m_dialogChoiceContainer.GetChildAt(1).Y = m_dialogChoiceContainer.GetChildAt(m_highlightedChoice).Y + m_dialogChoiceContainer.GetChildAt(1).Height / 2 + 3;
                    }
                    else if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    {
                        SoundManager.PlaySound("frame_swap");
                        m_highlightedChoice--;
                        if (m_highlightedChoice < 2)
                            m_highlightedChoice = 3;
                        m_dialogChoiceContainer.GetChildAt(1).Y = m_dialogChoiceContainer.GetChildAt(m_highlightedChoice).Y + m_dialogChoiceContainer.GetChildAt(1).Height / 2 + 3;
                    }

                    if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3))
                    {
                        m_runCancelEndHandler = false;
                        if (m_highlightedChoice == 3)
                        {
                            m_runCancelEndHandler = true;
                            SoundManager.PlaySound("DialogueMenuCancel");
                        }
                        else
                            SoundManager.PlaySound("DialogueMenuConfirm");

                        m_lockControls = true;
                        SoundManager.PlaySound("DialogMenuClose");

                        //Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                        Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Quad.EaseInOut, "Opacity", "0", "Y", "100");
                        Tweener.Tween.To(this, 0.3f, Tweener.Ease.Linear.EaseNone, "BackBufferOpacity", "0");
                        Tweener.Tween.To(m_dialogChoiceContainer, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                        Tweener.Tween.AddEndHandlerToLastTween(this, "ExitScreen");
                    }
                    else if (Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                    {
                        m_highlightedChoice = 3;
                        m_dialogChoiceContainer.GetChildAt(1).Y = m_dialogChoiceContainer.GetChildAt(m_highlightedChoice).Y + m_dialogChoiceContainer.GetChildAt(1).Height / 2 + 3;
                        m_runCancelEndHandler = true;
                        SoundManager.PlaySound("DialogueMenuCancel");

                        m_lockControls = true;
                        SoundManager.PlaySound("DialogMenuClose");

                        //Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                        Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Quad.EaseInOut, "Opacity", "0", "Y", "100");
                        Tweener.Tween.To(this, 0.3f, Tweener.Ease.Linear.EaseNone, "BackBufferOpacity", "0");
                        Tweener.Tween.To(m_dialogChoiceContainer, 0.3f, Tweener.Ease.Back.EaseIn, "ScaleX", "0", "ScaleY", "0");
                        Tweener.Tween.AddEndHandlerToLastTween(this, "ExitScreen");
                    }
                }
            }

            base.HandleInput();
        }

        public override void Update(GameTime gameTime)
        {
            if (m_inputDelayTimer > 0)
                m_inputDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (m_dialogChoiceContainer.Visible == false)
            {
                // Special code in case you start the dialogue with a choice sequence, instead of showing dialogue, pressing confirm, then showing the choice.
                if (m_dialogCounter == m_dialogText.Length - 1 && m_runChoiceDialogue == true)
                {
                    SpriteObj continueTextIcon1 = m_dialogContainer.GetChildAt(3) as SpriteObj;
                    continueTextIcon1.ChangeSprite("EndTextIcon_Sprite");
                    if (m_runChoiceDialogue == true)
                    {
                        //SoundManager.PlaySound("DialogOpenBump");
                        TextObj dialogueText = (m_dialogContainer.GetChildAt(2) as TextObj);
                        dialogueText.StopTypeWriting(true);
                        m_dialogChoiceContainer.Visible = true;
                        Tweener.Tween.To(m_dialogChoiceContainer, 0.3f, Tweener.Ease.Back.EaseOut, "ScaleX", "1", "ScaleY", "1");
                        Tweener.Tween.RunFunction(0.1f, typeof(SoundManager), "PlaySound", "DialogOpenBump");
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);
            Camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320, 720), Color.Black * BackBufferOpacity);
            m_dialogContainer.Draw(Camera);
            if (m_dialogContainer.ScaleX > 0)
                m_dialogContainer.GetChildAt(3).Y = m_dialogContinueIconY + (float)Math.Sin(Game.TotalGameTime * 20) * 2;

            m_dialogChoiceContainer.Draw(Camera);
            Camera.End();
            base.Draw(gameTime);
        }

        public override void OnEnter()
        {
            m_inputDelayTimer = 0.5f;

            //SoundManager.PlaySound("DialogueMenuOpen");
            SoundManager.PlaySound("DialogOpen");
            m_lockControls = false;
            m_runCancelEndHandler = false;
            m_highlightedChoice = 2; // 2 == Confirm. 3 == Cancel
            m_dialogChoiceContainer.Scale = new Vector2(1, 1);
            m_dialogChoiceContainer.GetChildAt(1).Y = m_dialogChoiceContainer.GetChildAt(m_highlightedChoice).Y + m_dialogChoiceContainer.GetChildAt(1).Height / 2 + 3;
            m_dialogChoiceContainer.Scale = Vector2.Zero;

            DialogueObj dialogueObj = DialogueManager.GetText(m_dialogueObjName);
            string[] names = dialogueObj.Speakers;
            string[] text = dialogueObj.Dialogue;
            SpriteObj continueTextIcon = m_dialogContainer.GetChildAt(3) as SpriteObj;
            if (text.Length > 1)
                continueTextIcon.ChangeSprite("ContinueTextIcon_Sprite");
            else
                continueTextIcon.ChangeSprite("EndTextIcon_Sprite");

            m_dialogCounter = 0;
            m_dialogTitles = names;
            m_dialogText = text;

            m_dialogContainer.Scale = Vector2.One;
            m_dialogContainer.Opacity = 0;
            //m_dialogContainer.Y -= 100;

            if (m_stringFormatArgs != null)
                (m_dialogContainer.GetChildAt(2) as TextObj).Text = string.Format(LocaleBuilder.getString(text[m_dialogCounter], m_dialogContainer.GetChildAt(2) as TextObj, m_forceMaleDialogue), m_stringFormatArgs);
            else
                (m_dialogContainer.GetChildAt(2) as TextObj).Text = LocaleBuilder.getString(text[m_dialogCounter], m_dialogContainer.GetChildAt(2) as TextObj, m_forceMaleDialogue);
            (m_dialogContainer.GetChildAt(2) as TextObj).WordWrap(850);
            (m_dialogContainer.GetChildAt(1) as TextObj).Text = LocaleBuilder.getString(names[m_dialogCounter], m_dialogContainer.GetChildAt(1) as TextObj, m_forceMaleDialogue);
            if (Game.PlayerStats.Traits.X == TraitType.Dyslexia || Game.PlayerStats.Traits.Y == TraitType.Dyslexia)
            {
                (m_dialogContainer.GetChildAt(2) as TextObj).RandomizeSentence(false);
                (m_dialogContainer.GetChildAt(1) as TextObj).RandomizeSentence(false);
            }
            (m_dialogContainer.GetChildAt(2) as TextObj).BeginTypeWriting(text[m_dialogCounter].Length * m_textScrollSpeed, "dialogue_tap");

            Tweener.Tween.To(m_dialogContainer, 0.3f, Tweener.Ease.Quad.EaseInOut, "Opacity", "1", "Y", "150");
            Tweener.Tween.To(this, 0.3f, Tweener.Ease.Linear.EaseNone, "BackBufferOpacity", "0.5");

            base.OnEnter();
        }

        public void ExitScreen()
        {
            // All this needs to be set BEFORE you call the end handler.
            (ScreenManager as RCScreenManager).HideCurrentScreen();
            m_runChoiceDialogue = false;
            m_dialogChoiceContainer.Visible = false;
            m_dialogChoiceContainer.Scale = Vector2.Zero;

            if (m_runCancelEndHandler == false)
            {
                if (m_confirmMethodInfo != null)
                    m_confirmMethodInfo.Invoke(m_confirmMethodObj, m_confirmArgs);
            }
            else
            {
                if (m_cancelMethodInfo != null)
                    m_cancelMethodInfo.Invoke(m_cancelMethodObj, m_cancelArgs);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Dialogue Screen");

                m_confirmMethodObj = null;
                m_confirmMethodInfo = null;
                if (m_confirmArgs != null)
                    Array.Clear(m_confirmArgs, 0, m_confirmArgs.Length);
                m_confirmArgs = null;

                m_cancelMethodObj = null;
                m_cancelMethodInfo = null;
                if (m_cancelArgs != null)
                    Array.Clear(m_cancelArgs, 0, m_cancelArgs.Length);
                m_cancelArgs = null;

                m_dialogContainer.Dispose();
                m_dialogContainer = null;
                m_dialogChoiceContainer.Dispose();
                m_dialogChoiceContainer = null;

                //if (m_dialogText != null)
                //    Array.Clear(m_dialogText, 0, m_dialogText.Length);
                m_dialogText = null;
                //if (m_dialogTitles != null)
                 //   Array.Clear(m_dialogTitles, 0, m_dialogTitles.Length);
                m_dialogTitles = null;

                if (m_stringFormatArgs != null)
                    Array.Clear(m_stringFormatArgs, 0, m_stringFormatArgs.Length);
                m_stringFormatArgs = null;
                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            // need to manually reapply word wrap since TextObj.Text assignment doesn't do it
            if (m_dialogContainer != null && m_dialogContainer.GetChildAt(2) != null)
                (m_dialogContainer.GetChildAt(2) as TextObj).WordWrap(850);

            base.RefreshTextObjs();
        }
    }
}

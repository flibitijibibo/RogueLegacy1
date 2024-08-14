//#define XBOX_CREDITS
//#define PLAYSTATION_CREDITS
//#define SWITCH_CREDITS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class CreditsScreen : Screen
    {
        private const float m_scrollDuration = 75;
        private bool m_allowExit;
        public bool IsEnding;

        private List<TextObj> m_creditsNameList;
        private List<TextObj> m_creditsTitleList;

        private SpriteObj m_bg1, m_bg2, m_bg3, m_bgOutside;
        private SpriteObj m_ground1, m_ground2, m_ground3;
        private SpriteObj m_border1, m_border2, m_border3;
        private SpriteObj m_prop1;
        private ObjContainer m_prop2, m_prop3;

        private ObjContainer m_playerSprite;
        private ObjContainer m_wifeSprite;
        private ObjContainer m_childSprite1, m_childSprite2;
        private int m_wifeChest, m_wifeShoulders, m_wifeHead;
        private int m_child1Chest, m_child1Shoulders, m_child1Head;
        private int m_child2Chest, m_child2Shoulders, m_child2Head;

        private SpriteObj m_sideBorderLeft;
        private SpriteObj m_sideBorderRight;
        private SpriteObj m_sideBorderTop;
        private SpriteObj m_sideBorderBottom;

        private ObjContainer m_manor;
        private string[] m_backgroundStrings;
        private int m_backgroundIndex = 0;

        private float m_backgroundSwapTimer;

        private TextObj m_thanksForPlayingText;
        private TextObj m_totalPlayTime;
        private TextObj m_totalDeaths;

        private SkyObj m_sky;
        private RenderTarget2D m_skyRenderTarget;
        private RenderTarget2D m_backgroundRenderTarget;

        private KeyIconTextObj m_continueText;
        private bool m_displayingContinueText;
        float m_scrollDistance = 0;

        private SpriteObj m_glauber;
        private SpriteObj m_teddy;
        private SpriteObj m_kenny;
        private SpriteObj m_gordon;
        private SpriteObj m_judson;
        private SpriteObj m_mc;

        private Color m_skinColour1 = new Color(231, 175, 131, 255);
        private Color m_skinColour2 = new Color(199, 109, 112, 255);
        private Color m_lichColour1 = new Color(255, 255, 255, 255);
        private Color m_lichColour2 = new Color(198, 198, 198, 255);

        public override void LoadContent()
        {
            m_bgOutside = new SpriteObj("TraitsBG_Sprite");
            m_bgOutside.ForceDraw = true;
            m_bgOutside.Visible = false;
            m_bgOutside.Scale = new Vector2(1320f / m_bgOutside.Width, 1320f / m_bgOutside.Width);

            m_bg1 = new SpriteObj("CastleBG1_Sprite");
            m_bg1.Position = new Vector2(1320 / 2f, 200);
            m_bg1.Scale = new Vector2(2, 2);
            m_bg2 = m_bg1.Clone() as SpriteObj;
            m_bg2.X -= m_bg1.Width;
            m_bg3 = m_bg2.Clone() as SpriteObj;
            m_bg3.X -= m_bg2.Width;

            m_ground1 = new SpriteObj("CastleFG1_Sprite");
            m_ground1.Position = new Vector2(1320 / 2f, 440);
            m_ground1.Scale = new Vector2(2, 2);
            m_ground2 = m_ground1.Clone() as SpriteObj;
            m_ground2.X -= m_ground1.Width;
            m_ground3 = m_ground2.Clone() as SpriteObj;
            m_ground3.X -= m_ground2.Width;

            m_border1 = new SpriteObj("CastleBorder_Sprite"); // TowerBorder2_Sprite
            m_border1.Position = new Vector2(1320 / 2f, 440);
            m_border1.Scale = new Vector2(2, 2);
            m_border2 = m_border1.Clone() as SpriteObj;
            m_border2.X -= m_border1.Width;
            m_border3 = m_border2.Clone() as SpriteObj;
            m_border3.X -= m_border2.Width;

            m_prop1 = new SpriteObj("CastleAssetWindow1_Sprite");
            m_prop1.Position = new Vector2(0, 220);
            m_prop1.Scale = new Vector2(2, 2);
            m_prop2 = new ObjContainer("CastleAssetBackTorch_Character");
            m_prop2.Position = new Vector2(500, 330);
            m_prop2.Scale = new Vector2(2, 2);
            m_prop2.AnimationDelay = 1 / 10f;
            m_prop2.PlayAnimation(true);
            m_prop3 = new ObjContainer("CastleAssetCandle1_Character");
            m_prop3.Position = new Vector2(1000, 440);
            m_prop3.Scale = new Vector2(2, 2);

            m_playerSprite = new ObjContainer("PlayerWalking_Character");
            m_playerSprite.Position = new Vector2(1320 / 2f - 20, 400);
            m_playerSprite.PlayAnimation(true);
            m_playerSprite.AnimationDelay = 1 / 10f;
            m_playerSprite.Flip = SpriteEffects.FlipHorizontally;
            m_playerSprite.OutlineWidth = 2;

            Color darkPink = new Color(251, 156, 172);
            m_wifeSprite = new ObjContainer("PlayerWalking_Character");
            m_wifeSprite.Position = new Vector2(-200, 400);
            m_wifeSprite.PlayAnimation(true);
            m_wifeSprite.AnimationDelay = 1 / 10f;
            m_wifeSprite.OutlineWidth = 2;
            m_wifeSprite.Scale = new Vector2(2, 2);
            m_wifeSprite.GetChildAt(PlayerPart.Bowtie).TextureColor = darkPink;
            m_wifeSprite.GetChildAt(PlayerPart.Hair).TextureColor = Color.Red;
            m_wifeSprite.GetChildAt(PlayerPart.Cape).TextureColor = Color.Red;
            m_wifeSprite.GetChildAt(PlayerPart.Neck).TextureColor = Color.Red;
            m_wifeSprite.GetChildAt(PlayerPart.Sword2).TextureColor = new Color(11, 172, 239);

            m_childSprite1 = new ObjContainer("PlayerWalking_Character");
            m_childSprite1.Position = new Vector2(-270, 415);
            m_childSprite1.PlayAnimation(true);
            m_childSprite1.AnimationDelay = 1 / 10f;
            m_childSprite1.OutlineWidth = 2;
            m_childSprite1.Scale = new Vector2(1.2f, 1.2f);
            m_childSprite1.GetChildAt(PlayerPart.Bowtie).TextureColor = darkPink;
            m_childSprite1.GetChildAt(PlayerPart.Hair).TextureColor = Color.Red;
            m_childSprite1.GetChildAt(PlayerPart.Cape).TextureColor = Color.Red;
            m_childSprite1.GetChildAt(PlayerPart.Neck).TextureColor = Color.Red;
            m_childSprite1.GetChildAt(PlayerPart.Sword2).TextureColor = new Color(11, 172, 239);

            m_childSprite2 = new ObjContainer("PlayerWalking_Character");
            m_childSprite2.Position = new Vector2(-330, 420);
            m_childSprite2.PlayAnimation(true);
            m_childSprite2.AnimationDelay = 1 / 10f;
            m_childSprite2.OutlineWidth = 2;
            m_childSprite2.Scale = new Vector2(1, 1);
            m_childSprite2.GetChildAt(PlayerPart.Bowtie).TextureColor = darkPink;
            m_childSprite2.GetChildAt(PlayerPart.Hair).TextureColor = Color.Red;
            m_childSprite2.GetChildAt(PlayerPart.Cape).TextureColor = Color.Red;
            m_childSprite2.GetChildAt(PlayerPart.Neck).TextureColor = Color.Red;
            m_childSprite2.GetChildAt(PlayerPart.Sword2).TextureColor = new Color(11, 172, 239);

            m_sideBorderLeft = new SpriteObj("Blank_Sprite");
            m_sideBorderLeft.Scale = new Vector2(900f / m_sideBorderLeft.Width, 500f / m_sideBorderLeft.Height);
            m_sideBorderLeft.Position = new Vector2(-450, 0);
            m_sideBorderLeft.TextureColor = Color.Black;
            m_sideBorderLeft.ForceDraw = true;
            m_sideBorderRight = m_sideBorderLeft.Clone() as SpriteObj;
            m_sideBorderRight.Position = new Vector2(850, 0);
            m_sideBorderTop = m_sideBorderLeft.Clone() as SpriteObj;
            m_sideBorderTop.Scale = new Vector2(1,1);
            m_sideBorderTop.Scale = new Vector2(1320f/ m_sideBorderTop.Width, 240/m_sideBorderTop.Height);
            m_sideBorderTop.Position = Vector2.Zero;
            m_sideBorderBottom= m_sideBorderLeft.Clone() as SpriteObj;
            m_sideBorderBottom.Scale = new Vector2(1,1);
            m_sideBorderBottom.Scale = new Vector2(1340f / m_sideBorderBottom.Width, 720f / m_sideBorderBottom.Height);
            m_sideBorderBottom.Position = new Vector2(0, 460);

            m_manor = new ObjContainer("TraitsCastle_Character");
            m_manor.Scale = new Vector2(2, 2);
            m_manor.Visible = false;

            for (int i = 0; i < m_manor.NumChildren; i++)
            {
                m_manor.GetChildAt(i).Visible = false;
            }

            foreach (SkillObj skill in SkillSystem.SkillArray)
            {
                if (skill.CurrentLevel > 0)
                    m_manor.GetChildAt(SkillSystem.GetManorPiece(skill)).Visible = true;
            }

            m_thanksForPlayingText = new TextObj(Game.JunicodeLargeFont);
            m_thanksForPlayingText.FontSize = 32;
            m_thanksForPlayingText.Align = Types.TextAlign.Centre;
            m_thanksForPlayingText.Text = LocaleBuilder.getString("LOC_ID_CREDITS_SCREEN_1", m_thanksForPlayingText); //"Thanks for playing!"
            //m_thanksForPlayingText.OutlineWidth = 2;
            m_thanksForPlayingText.DropShadow = new Vector2(2, 2);
            m_thanksForPlayingText.Position = new Vector2(1320 / 2f, 480);
            m_thanksForPlayingText.Opacity = 0;

            m_totalDeaths = m_thanksForPlayingText.Clone() as TextObj;
            m_totalDeaths.FontSize = 20;
            m_totalDeaths.Position = m_thanksForPlayingText.Position;
            m_totalDeaths.Y += 90;
            m_totalDeaths.Opacity = 0;
            m_totalDeaths.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_totalDeaths); // dummy locID to add TextObj to language refresh list
            m_totalPlayTime = m_thanksForPlayingText.Clone() as TextObj;
            m_totalPlayTime.FontSize = 20;
            m_totalPlayTime.Position = m_totalDeaths.Position;
            m_totalPlayTime.Y += 50;
            m_totalPlayTime.Opacity = 0;
            m_totalPlayTime.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_totalPlayTime); // dummy locID to add TextObj to language refresh list

            m_continueText = new KeyIconTextObj(Game.JunicodeFont);
            m_continueText.FontSize = 14;
            m_continueText.Align = Types.TextAlign.Right;
            m_continueText.Position = new Vector2(1320 - 50, 650);
            m_continueText.ForceDraw = true;
            m_continueText.Opacity = 0;
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", m_continueText); // dummy locID to add TextObj to language refresh list

            int teamXPos = 200;
            Vector2 teamScale = new Vector2(1.5f, 1.5f);
            m_glauber = new SpriteObj("Glauber_Sprite");
            m_glauber.Scale = teamScale;
            m_glauber.ForceDraw = true;
            m_glauber.OutlineWidth = 2;
            m_glauber.X = teamXPos;
            m_teddy = new SpriteObj("Teddy_Sprite");
            m_teddy.Scale = teamScale;
            m_teddy.ForceDraw = true;
            m_teddy.OutlineWidth = 2;
            m_teddy.X = teamXPos;
            m_kenny = new SpriteObj("Kenny_Sprite");
            m_kenny.Scale = teamScale;
            m_kenny.ForceDraw = true;
            m_kenny.OutlineWidth = 2;
            m_kenny.X = teamXPos;
            m_gordon = new SpriteObj("Gordon_Sprite");
            m_gordon.Scale = teamScale;
            m_gordon.ForceDraw = true;
            m_gordon.OutlineWidth = 2;
            m_gordon.X = teamXPos;
            m_judson = new SpriteObj("Judson_Sprite");
            m_judson.Scale = teamScale;
            m_judson.ForceDraw = true;
            m_judson.OutlineWidth = 2;
            m_judson.X = teamXPos;
            m_mc = new SpriteObj("MC_Sprite");
            m_mc.Scale = teamScale;
            m_mc.ForceDraw = true;
            m_mc.OutlineWidth = 2;
            m_mc.X = teamXPos;

            InitializeCredits();

            base.LoadContent();
        }

        public override void ReinitializeRTs()
        {
            m_sky.ReinitializeRT(Camera);
            m_skyRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            m_backgroundRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            base.ReinitializeRTs();
        }

        private void InitializeCredits()
        {
            m_creditsNameList = new List<TextObj>();
            m_creditsTitleList = new List<TextObj>();

            m_backgroundStrings = new string[] { "Garden", "Tower", "Dungeon", "Outside", "Manor" };

            string[] creditsNames = null;
            creditsNames = new string[] 
                {
                    "[@LOC_ID_CREDITS_SCREEN_6" /*"Developed By"*/, "Cellar Door Games",
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_7" /*"Design & Story"*/, "Teddy Lee",
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_8" /*"Programming"*/, "Kenny Lee",
                    "", "",
#if SWITCH_CREDITS
                    "[@LOC_ID_CREDITS_SCREEN_9" /*"Production"*/, "Ryan Lee",
#else
                    "[@LOC_ID_CREDITS_SCREEN_9" /*"Production"*/, "Marie-Christine Bourdua",
#endif
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_10" /*"Art"*/, "Glauber Kotaki",
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_11" /*"Music & Audio Design"*/, "Gordon McGladdery (A Shell in the Pit)",
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_12" /*"Music"*/, "Judson Cowan (Tettix)",
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_13" /*"Marketing & Story"*/, "Benny Lee",
                    "", "",
                    "[@LOC_ID_CREDITS_SCREEN_25" /*"Additional Programming"*/, "David Man",
                    "", "",

                    "[@LOC_ID_CREDITS_SCREEN_14" /*"Additional Audio Design"*/,"Alessio Mellina",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_15" /*"Additional Background Art"*/, "John Won", 
                    "","",
#if SWITCH_CREDITS
                    "[@LOC_ID_CREDITS_SCREEN_16" /*"Business Support"*/, "Michelle Lee",
#else
                    "[@LOC_ID_CREDITS_SCREEN_16" /*"Business Support"*/, "Ryan & Michelle Lee",
#endif
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_17" /*"Super Special Thanks: Turbo Edition"*/, "Jenny Lee",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_18" /*"Special Thanks"*/, "Amber Campbell (Phedran), Amir Rao, Blair Hurm Cowan, Caitlin Groves", "Doug Culp, Eric Lee Lewis, Nathan Vella, Priscila Garcia, Rima Singh", "Scott Barcik, Stephen Goldstein, Tyler Mayes, Will Turnbull", "John 'Duke' Wain", 
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_19" /*"Additional Thanks"*/, "Jake Hirshey, Joshua Hornsby, Mark Wallace", "Peter Lee, Sean Fleming",
#if XBOX_CREDITS || PLAYSTATION_CREDITS
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_26",/*"Adaptation by"*/ "Abstraction Games",
                    "","",
                    "Ralph Egas",
                    "Erik Bastianen",
                    "Wilco Schroo",
                    "Coen Campman",
                    "Adrian Francis",
                    "Rutger Janssen",
                    "Jorge Lorenzon",
                    "Frédéric Schertenleib",
                    "Tj'ièn Twijnstra",
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_QA", "Testronic Labs",
#endif
#if SWITCH_CREDITS
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_26",/*"Adaptation by"*/ "BlitWorks SL",
                    "","",
                    "Tony Cabello",
                    "Oscar Serrano",
                    "Guillermo NWDD",
                    "Julio Garcia",
                    "Daniel Lancha",
                    "Javier Moya",
                    "Miguel Pascual",
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_QA", "Lollipop Robot",
                    "","",
                    "Pablo Granada",
                    "Luis Moyano",
                    "Oscar Navalon",
                    "Francesc Sanchez",
                    "Daniel Segarra",
#endif
#if !XBOX_CREDITS && !PLAYSTATION_CREDITS && !SWITCH_CREDITS
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_20" /*"Mac/Linux Adaptation by"*/, "Ethan 'flibitijibibo' Lee",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_21" /*"Mac/Linux QA Team"*/, "David Gow, Forrest Loomis,", "Jorgen Tjerno, Marcus Moller, Matthias Niess, ", "Stanislaw Gackowski, Stefano Angeleri",
#endif
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_23" /*"Primary Localization by"*/, "Babel Media",
                    "","",
                    "[@LOC_ID_CREDITS_SCREEN_24" /*"Chinese & Add'l Localization by"*/, "Universally Speaking", 
                    "Tobias Gut (" + LocaleBuilder.getResourceString("LOC_ID_OPTIONS_LANGUAGE_GERMAN") + ")", 
                    "Virtualname (" + LocaleBuilder.getResourceString("LOC_ID_OPTIONS_LANGUAGE_CHINESE") + ")",
#if XBOX_CREDITS
                    "","",
                    "","",
                    "","",
                    "[Microsoft ID@Xbox", "Chris Charla", "Dave Mianowski", "Wally Barger",
#endif
#if PLAYSTATION_CREDITS
                    "","",
                    "","",
                    "","",
                    "[@LOC_ID_CREDITS_JAPAN " /*"Japanese Localization & Production By"*/, "8-4, Ltd.",
                    "","",
                    "","",
                    "","",
                    "[Sony Computer Entertainment", "Alessandro Bovenzi", "Annie Meltzer", "Ben Andac", "Eddie Ramirez", "Justin Massongil", "Laura Casey", "Maimoona Block", "Norma Green", "Rey Gutierrez", "Ryan Clements", "Shahid Kamal Ahmad", "Shane Bettenhausen", "Akinari Ito", "Teppei Fujita", 
#endif
#if SWITCH_CREDITS
                    "","",
                    "","",
                    "","",
                    "[Nintendo", "Kirk Scott", "Sara Popescu",
#endif
                    "","",
                    "",
                    "@LOC_ID_CREDITS_SCREEN_22" /*"Thanks to all our fans for their support!"*/,
                };

            int yCounter = 0;
            for (int i = 0; i < creditsNames.Length; i++)
            {
                TextObj textObj = new TextObj(Game.JunicodeFont);
                textObj.FontSize = 12;
                string text = creditsNames[i];
                if (text.Length > 0 && text.StartsWith("["))
                {
                    text = text.Remove(0, 1);
                    textObj.FontSize = 10;
                }
                textObj.DropShadow = new Vector2(2, 2);
                textObj.Align = Types.TextAlign.Centre;
                textObj.Position = new Vector2(1320 / 2f, 720 + yCounter);

                try
                {
                    if (text.Length > 0 && text.StartsWith("@"))
                    {
                        text = text.Remove(0, 1);
                        textObj.Text = LocaleBuilder.getString(text, textObj);
                    }
                    else
                        textObj.Text = text;
                }
                catch
                {
                    textObj.ChangeFontNoDefault(Game.NotoSansSCFont);
                    if (text.Length > 0 && text.StartsWith("@"))
                    {
                        text = text.Remove(0, 1);
                        textObj.Text = LocaleBuilder.getString(text, textObj);
                    }
                    else
                        textObj.Text = text;
                }


                yCounter += 30;

                PositionTeam(creditsNames[i], new Vector2(textObj.Bounds.Left - 50, textObj.Y));
                m_creditsNameList.Add(textObj);

                //TextObj titlesTextObj = new TextObj(Game.JunicodeFont);
                //titlesTextObj.FontSize = 12;
                ////titlesTextObj.OutlineWidth = 2;
                //titlesTextObj.DropShadow = new Vector2(2, 2);
                //titlesTextObj.Align = Types.TextAlign.Centre;
                //titlesTextObj.Position = new Vector2(1320 / 2f, 720 + yCounter);

                //if (i < creditsTitles.Length)
                //{
                //    titlesTextObj.Text = creditsTitles[i];
                //    m_creditsTitleList.Add(titlesTextObj);
                //    if (i < creditsTitles.Length - 1)
                //        yCounter += 200;
                //    else
                //        yCounter += 40;
                //}
                //else
                //    yCounter += 40;

                //TextObj nameTextObj = titlesTextObj.Clone() as TextObj;
                //nameTextObj.Text = creditsNames[i];
                //nameTextObj.FontSize = 16;
                //nameTextObj.Y += 40;
                //m_creditsNameList.Add(nameTextObj);

                //PositionTeam(creditsNames[i], new Vector2(nameTextObj.Bounds.Left - 50, nameTextObj.Y));
            }
        }

        private void PositionTeam(string name, Vector2 position)
        {
            if (name.Contains("Teddy"))
                m_teddy.Position = position;
            else if (name.Contains("Kenny"))
            {
                m_kenny.Position = position;
                m_kenny.X -= 10;
            }
            else if (name.Contains("Glauber"))
            {
                m_glauber.Position = position;
                m_glauber.X -= 0;
            }
            else if (name.Contains("Gordon"))
            {
                m_gordon.Position = position;
                m_gordon.X += 135;
            }
            else if (name.Contains("Judson"))
            {
                m_judson.Position = position;
                m_judson.X += 60;
            }
            else if (name.Contains("Marie"))
            {
                m_mc.Position = position;
                m_mc.X += 60;
            }
        }

        public override void OnEnter()
        {
            m_allowExit = false;

            float hoursPlayed = (Game.PlayerStats.TotalHoursPlayed + Game.HoursPlayedSinceLastSave);
            int minutes = (int)((hoursPlayed - (int)(hoursPlayed)) * 60f);
            Console.WriteLine("Hours played: " + hoursPlayed + " minutes: " + minutes);

            m_totalDeaths.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_2") /*"Total Children"*/ + ": " + Game.PlayerStats.TimesDead.ToString();
            if (minutes < 10)
                m_totalPlayTime.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_3") /*"Time Played"*/ + " - " + (int)hoursPlayed + ":0" + minutes;
            else
                m_totalPlayTime.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_3") /*"Time Played"*/ + " - " + (int)hoursPlayed + ":" + minutes;

            Camera.Position = Vector2.Zero;
            m_displayingContinueText = false;
            m_continueText.Text = LocaleBuilder.getString("LOC_ID_CREDITS_SCREEN_4_NEW", m_continueText);
            //Tween.To(m_continueText, 1, Tween.EaseNone, "delay", "2", "Opacity", "1");

            if (m_sky == null) // Hack
            {
                m_sky = new SkyObj(null);
                m_skyRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                m_sky.LoadContent(Camera);
                m_backgroundRenderTarget = new RenderTarget2D(Camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            }

            SetPlayerStyle("Walking");

            // Only play a song if you're entering from the title screen.  Otherwise just let the other song leak in.
            if (IsEnding == false)
                SoundManager.PlayMusic("CreditsSong", true, 1);

            //if (IsEnding == true)
            //    SoundManager.PlayMusic("EndSongDrums", true, 1);
            //else
            //    SoundManager.PlayMusic("CreditsSong", true, 1);

            m_scrollDistance = -(m_creditsNameList[m_creditsNameList.Count - 1].Y + 100);

            foreach (TextObj title in m_creditsTitleList)
                Tween.By(title, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            foreach (TextObj name in m_creditsNameList)
                Tween.By(name, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());

            Tween.By(m_teddy, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_kenny, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_glauber, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_gordon, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_judson, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_mc, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());

            if (IsEnding == false)
            {
                m_sideBorderLeft.X += 200;
                m_sideBorderRight.X -= 200;
                Tween.RunFunction(m_scrollDuration + 1, this, "ResetScroll");
            }

            base.OnEnter();
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

            if (Game.PlayerStats.Class == ClassType.Traitor)
                m_playerSprite.GetChildAt(PlayerPart.Head).ChangeSprite("Player" + animationType + "Head" + PlayerPart.IntroHelm + "_Sprite");

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
            {
                m_playerSprite.OutlineColour = Color.White;
                m_playerSprite.GetChildAt(PlayerPart.Sword1).Visible = false;
                m_playerSprite.GetChildAt(PlayerPart.Sword2).Visible = false;
            }
            else
            {
                m_playerSprite.OutlineColour = Color.Black;
                m_playerSprite.GetChildAt(PlayerPart.Sword1).Visible = true;
                m_playerSprite.GetChildAt(PlayerPart.Sword2).Visible = true;
            }

            // Setting the player's proper parts.
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

            // Reposition the player's Y after changing his scale.
            m_playerSprite.PlayAnimation(true);
            m_playerSprite.CalculateBounds();
            m_playerSprite.Y = 435 - (m_playerSprite.Bounds.Bottom - m_playerSprite.Y);
        }

        public override void OnExit()
        {
            Tween.StopAllContaining(this, false);
            base.OnExit();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsEnding == true)
            {
                m_sky.Update(gameTime);

                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                UpdateBackground(elapsedTime);

                if (m_backgroundIndex < m_backgroundStrings.Length)
                {
                    m_backgroundSwapTimer += elapsedTime;

                    if (m_backgroundSwapTimer >= m_scrollDuration / m_backgroundStrings.Length)
                    {
                        SwapBackground(m_backgroundStrings[m_backgroundIndex]);
                        m_backgroundIndex++;
                        m_backgroundSwapTimer = 0;
                    }
                }
            }

            base.Update(gameTime);
        }

        public void ResetScroll()
        {
            foreach (TextObj title in m_creditsTitleList)
            {
                title.Y += -m_scrollDistance;
                Tween.By(title, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            }

            foreach (TextObj name in m_creditsNameList)
            {
                name.Y += -m_scrollDistance;
                Tween.By(name, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());

                PositionTeam(name.Text, new Vector2(name.Bounds.Left - 50, name.Y));
            }

            Tween.By(m_teddy, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_kenny, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_glauber, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_gordon, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_judson, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());
            Tween.By(m_mc, m_scrollDuration, Tween.EaseNone, "Y", m_scrollDistance.ToString());

            Tween.RunFunction(m_scrollDuration + 1, this, "ResetScroll");
        }

        public override void HandleInput()
        {
            if (IsEnding == false || (IsEnding == true && m_allowExit == true))
            {
                if (Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM2) 
                    || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL1) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL2)
                     || Game.GlobalInput.JustPressed(InputMapType.MENU_CONFIRM3) || Game.GlobalInput.JustPressed(InputMapType.MENU_CANCEL3))
                {
                    if (m_displayingContinueText == true)
                    {
                        Tween.StopAll(false);
                        (ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Title, true);
                    }
                    else
                    {
                        m_displayingContinueText = true;
                        Tween.StopAllContaining(m_continueText, false);
                        Tween.To(m_continueText, 0.5f, Tween.EaseNone, "Opacity", "1");
                        Tween.RunFunction(4, this, "HideContinueText");
                    }
                }
            }

            base.HandleInput();
        }

        public void HideContinueText()
        {
            m_displayingContinueText = false;
            Tween.To(m_continueText, 0.5f, Tween.EaseNone, "delay", "0", "Opacity", "0");
        }

        private void UpdateBackground(float elapsedTime)
        {
            int movespeed = 200;

            m_bg1.X += movespeed * elapsedTime;
            m_bg2.X += movespeed * elapsedTime;
            m_bg3.X += movespeed * elapsedTime;

            if (m_bg1.X > 930)
                m_bg1.X = m_bg3.X - m_bg3.Width;
            if (m_bg2.X > 930)
                m_bg2.X = m_bg1.X - m_bg1.Width;
            if (m_bg3.X > 930)
                m_bg3.X = m_bg2.X - m_bg2.Width;

            m_ground1.X += movespeed * elapsedTime;
            m_ground2.X += movespeed * elapsedTime;
            m_ground3.X += movespeed * elapsedTime;

            if (m_ground1.X > 930)
                m_ground1.X = m_ground3.X - m_ground3.Width;
            if (m_ground2.X > 930)
                m_ground2.X = m_ground1.X - m_ground1.Width;
            if (m_ground3.X > 930)
                m_ground3.X = m_ground2.X - m_ground2.Width;

            m_border1.X += movespeed * elapsedTime;
            m_border2.X += movespeed * elapsedTime;
            m_border3.X += movespeed * elapsedTime;

            if (m_border1.X > 930)
                m_border1.X = m_border3.X - m_border3.Width;
            if (m_border2.X > 930)
                m_border2.X = m_border1.X - m_border1.Width;
            if (m_border3.X > 930)
                m_border3.X = m_border2.X - m_border2.Width;

            m_prop1.X += movespeed * elapsedTime;
            m_prop2.X += movespeed * elapsedTime;
            m_prop3.X += movespeed * elapsedTime;

            if (m_prop1.X > 930)
                m_prop1.X -= CDGMath.RandomInt(1000, 3000);
            if (m_prop2.X > 930)
                m_prop2.X -= CDGMath.RandomInt(1000, 3000);
            if (m_prop3.X > 930)
                m_prop3.X -= CDGMath.RandomInt(1000, 3000);
        }

        public void SwapBackground(string levelType)
        {
            Tween.By(m_sideBorderLeft, 0.5f, Tween.EaseNone, "X", "200");
            Tween.By(m_sideBorderRight, 0.5f, Tween.EaseNone, "X", "-200");
            Tween.AddEndHandlerToLastTween(this, "PerformSwap", levelType);
        }

        public void PerformSwap(string levelType)
        {
            m_manor.Y = 0;
            m_bgOutside.Y = 0;
            m_bgOutside.Visible = false;
            m_manor.Visible = false;
            m_ground1.Visible = m_ground2.Visible = m_ground3.Visible = true;
            m_border1.Visible = m_border2.Visible = m_border3.Visible = true;
            m_prop1.Visible = m_prop2.Visible = m_prop3.Visible = true;

            switch (levelType)
            {
                case ("Castle"):
                    m_bg1.ChangeSprite("CastleBG1_Sprite");
                    m_bg2.ChangeSprite("CastleBG1_Sprite");
                    m_bg3.ChangeSprite("CastleBG1_Sprite");
                    m_ground1.ChangeSprite("CastleFG1_Sprite");
                    m_ground2.ChangeSprite("CastleFG1_Sprite");
                    m_ground3.ChangeSprite("CastleFG1_Sprite");
                    m_border1.ChangeSprite("CastleBorder_Sprite");
                    m_border2.ChangeSprite("CastleBorder_Sprite");
                    m_border3.ChangeSprite("CastleBorder_Sprite");

                    m_prop1.ChangeSprite("CastleAssetWindow1_Sprite");
                    m_prop2.ChangeSprite("CastleAssetBackTorch_Character");
                    m_prop2.PlayAnimation(true);
                    m_prop2.AnimationDelay = 1 / 10f;
                    m_prop3.ChangeSprite("CastleAssetCandle1_Character");
                    break;
                case ("Tower"):
                    m_bg1.ChangeSprite("TowerBG2_Sprite");
                    m_bg2.ChangeSprite("TowerBG2_Sprite");
                    m_bg3.ChangeSprite("TowerBG2_Sprite");
                    m_ground1.ChangeSprite("TowerFG2_Sprite");
                    m_ground2.ChangeSprite("TowerFG2_Sprite");
                    m_ground3.ChangeSprite("TowerFG2_Sprite");
                    m_border1.ChangeSprite("TowerBorder2_Sprite");
                    m_border2.ChangeSprite("TowerBorder2_Sprite");
                    m_border3.ChangeSprite("TowerBorder2_Sprite");

                    m_prop1.ChangeSprite("TowerHole4_Sprite");
                    m_prop2.Visible = false;
                    m_prop3.ChangeSprite("TowerPedestal2_Character");
                    break;
                case ("Dungeon"):
                    m_bg1.ChangeSprite("DungeonBG1_Sprite");
                    m_bg2.ChangeSprite("DungeonBG1_Sprite");
                    m_bg3.ChangeSprite("DungeonBG1_Sprite");
                    m_ground1.ChangeSprite("DungeonFG1_Sprite");
                    m_ground2.ChangeSprite("DungeonFG1_Sprite");
                    m_ground3.ChangeSprite("DungeonFG1_Sprite");
                    m_border1.ChangeSprite("DungeonBorder_Sprite");
                    m_border2.ChangeSprite("DungeonBorder_Sprite");
                    m_border3.ChangeSprite("DungeonBorder_Sprite");

                    m_prop1.ChangeSprite("DungeonPrison1_Sprite");
                    m_prop2.ChangeSprite("DungeonChain2_Character");
                    m_prop3.ChangeSprite("DungeonTorch2_Character");
                    break;
                case ("Garden"):
                    m_bg1.ChangeSprite("GardenBG_Sprite");
                    m_bg2.ChangeSprite("GardenBG_Sprite");
                    m_bg3.ChangeSprite("GardenBG_Sprite");
                    m_ground1.ChangeSprite("GardenFG_Sprite");
                    m_ground2.ChangeSprite("GardenFG_Sprite");
                    m_ground3.ChangeSprite("GardenFG_Sprite");
                    m_border1.ChangeSprite("GardenBorder_Sprite");
                    m_border2.ChangeSprite("GardenBorder_Sprite");
                    m_border3.ChangeSprite("GardenBorder_Sprite");

                    m_prop1.ChangeSprite("GardenFloatingRock3_Sprite");
                    m_prop2.ChangeSprite("GardenFairy_Character");
                    m_prop2.PlayAnimation(true);
                    m_prop2.AnimationDelay = 1 / 10f;
                    m_prop3.ChangeSprite("GardenLampPost1_Character");
                    break;
                case ("Outside"):
                    m_bg1.ChangeSprite("GardenBG_Sprite");
                    m_bg2.ChangeSprite("GardenBG_Sprite");
                    m_bg3.ChangeSprite("GardenBG_Sprite");
                    m_ground1.ChangeSprite("GardenFG_Sprite");
                    m_ground2.ChangeSprite("GardenFG_Sprite");
                    m_ground3.ChangeSprite("GardenFG_Sprite");
                    m_border1.ChangeSprite("StartingRoomFloor_Sprite");
                    m_border2.ChangeSprite("StartingRoomFloor_Sprite");
                    m_border3.ChangeSprite("StartingRoomFloor_Sprite");

                    m_prop1.ChangeSprite("DungeonPrison1_Sprite");
                    m_prop1.Visible = false;
                    m_prop2.ChangeSprite("CreditsGrass_Character");
                    m_prop2.Y = 440; // Hack to link it to the ground.
                    m_prop3.ChangeSprite("CreditsTree_Character");
                    m_bgOutside.Visible = true;
                    break;
                case ("Manor"):
                    m_bgOutside.Visible = true;
                    m_manor.Visible = true;
                    m_ground1.Visible = m_ground2.Visible = m_ground3.Visible = false;
                    m_border1.Visible = m_border2.Visible = m_border3.Visible = false;
                    m_manor.Y = -260;
                    m_bgOutside.Y = -260;
                    m_manor.X -= 300;
                    m_bgOutside.X -= 300;

                    m_prop1.Visible = false;
                    m_prop2.Visible = false;
                    m_prop3.Visible = false;

                    Tween.By(m_manor, 3f, Tween.EaseNone, "X", "300");
                    Tween.By(m_bgOutside, 3f, Tween.EaseNone, "X", "300");
                    Tween.By(m_playerSprite, 3.5f, Tween.EaseNone, "X", "-150");
                    Tween.AddEndHandlerToLastTween(this, "CreditsComplete");

                    Tween.By(m_sideBorderTop, 2.5f, Tween.EaseNone, "Y", "-500");
                    break;
            }

            if (levelType != "Manor")
            {
                Tween.By(m_sideBorderLeft, 0.5f, Tween.EaseNone, "X", "-200");
                Tween.By(m_sideBorderRight, 0.5f, Tween.EaseNone, "X", "200");
            }
            else
            {
                Tween.By(m_sideBorderLeft, 3f, Tween.EaseNone, "X", "-800");
                Tween.By(m_sideBorderRight, 3f, Tween.EaseNone, "X", "800");
            }
        }

        public void CreditsComplete()
        {
            SetPlayerStyle("Idle");

            Tween.RunFunction(0.5f, this, "SetPlayerStyle", "LevelUp");
            Tween.RunFunction(0.6f, m_playerSprite, "PlayAnimation", false);
            Tween.To(m_thanksForPlayingText, 2, Tween.EaseNone, "Opacity", "1");
            Tween.To(m_totalDeaths, 2, Tween.EaseNone,"delay", "0.2", "Opacity", "1");
            Tween.To(m_totalPlayTime, 2, Tween.EaseNone, "delay", "0.4", "Opacity", "1");

            Tween.RunFunction(1, this, "BringWife");
            Tween.RunFunction(1.1f, this, "BringChild1");
            Tween.RunFunction(3f, this, "BringChild2");
        }

        public void BringWife()
        {
            m_wifeSprite.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_wifeSprite.GetChildAt(PlayerPart.Extra).Visible = false;
            m_wifeSprite.GetChildAt(PlayerPart.Light).Visible = false;
            m_wifeSprite.GetChildAt(PlayerPart.Wings).Visible = false;

            m_wifeChest = CDGMath.RandomInt(1, PlayerPart.NumChestPieces);
            m_wifeHead = CDGMath.RandomInt(1, PlayerPart.NumHeadPieces);
            m_wifeShoulders = CDGMath.RandomInt(1, PlayerPart.NumShoulderPieces);

            m_wifeSprite.GetChildAt(PlayerPart.Chest).ChangeSprite("PlayerWalkingChest" + m_wifeChest + "_Sprite");
            m_wifeSprite.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerWalkingHead" + m_wifeHead + "_Sprite");
            m_wifeSprite.GetChildAt(PlayerPart.ShoulderA).ChangeSprite("PlayerWalkingShoulderA" + m_wifeShoulders + "_Sprite");
            m_wifeSprite.GetChildAt(PlayerPart.ShoulderB).ChangeSprite("PlayerWalkingShoulderB" + m_wifeShoulders + "_Sprite");

            if ((Game.PlayerStats.IsFemale == false && (Game.PlayerStats.Traits.X != TraitType.Gay && Game.PlayerStats.Traits.Y != TraitType.Gay)) ||
                ((Game.PlayerStats.IsFemale == true && (Game.PlayerStats.Traits.X == TraitType.Gay || Game.PlayerStats.Traits.Y == TraitType.Gay))))
            {
                m_wifeSprite.GetChildAt(PlayerPart.Bowtie).Visible = true;
                m_wifeSprite.GetChildAt(PlayerPart.Boobs).Visible = true;
            }
            else
            {
                m_wifeSprite.GetChildAt(PlayerPart.Bowtie).Visible = false;
                m_wifeSprite.GetChildAt(PlayerPart.Boobs).Visible = false;
            }

            m_wifeSprite.PlayAnimation(true);
            Tween.By(m_wifeSprite, 3, Tween.EaseNone, "X", "600");
            Tween.AddEndHandlerToLastTween(this, "LevelUpWife");
        }

        public void LevelUpWife()
        {
            m_wifeSprite.ChangeSprite("PlayerLevelUp_Character");
            m_wifeSprite.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_wifeSprite.GetChildAt(PlayerPart.Extra).Visible = false;
            m_wifeSprite.GetChildAt(PlayerPart.Light).Visible = false;
            m_wifeSprite.GetChildAt(PlayerPart.Wings).Visible = false;

            if ((Game.PlayerStats.IsFemale == false && (Game.PlayerStats.Traits.X != TraitType.Gay && Game.PlayerStats.Traits.Y != TraitType.Gay)) ||
            ((Game.PlayerStats.IsFemale == true && (Game.PlayerStats.Traits.X == TraitType.Gay || Game.PlayerStats.Traits.Y == TraitType.Gay))))
            {
                m_wifeSprite.GetChildAt(PlayerPart.Bowtie).Visible = true;
                m_wifeSprite.GetChildAt(PlayerPart.Boobs).Visible = true;
            }
            else
            {
                m_wifeSprite.GetChildAt(PlayerPart.Bowtie).Visible = false;
                m_wifeSprite.GetChildAt(PlayerPart.Boobs).Visible = false;
            }

            m_wifeSprite.GetChildAt(PlayerPart.Chest).ChangeSprite("PlayerLevelUpChest" + m_wifeChest + "_Sprite");
            m_wifeSprite.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerLevelUpHead" + m_wifeHead + "_Sprite");
            m_wifeSprite.GetChildAt(PlayerPart.ShoulderA).ChangeSprite("PlayerLevelUpShoulderA" + m_wifeShoulders + "_Sprite");
            m_wifeSprite.GetChildAt(PlayerPart.ShoulderB).ChangeSprite("PlayerLevelUpShoulderB" + m_wifeShoulders + "_Sprite");
            m_wifeSprite.PlayAnimation(false);
        }

        public void BringChild1()
        {
            m_childSprite1.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_childSprite1.GetChildAt(PlayerPart.Extra).Visible = false;
            m_childSprite1.GetChildAt(PlayerPart.Light).Visible = false;
            m_childSprite1.GetChildAt(PlayerPart.Wings).Visible = false;

            m_child1Chest = CDGMath.RandomInt(1, PlayerPart.NumChestPieces);
            m_child1Head = CDGMath.RandomInt(1, PlayerPart.NumHeadPieces);
            m_child1Shoulders = CDGMath.RandomInt(1, PlayerPart.NumShoulderPieces);

            bool isFemale = false;
            if (CDGMath.RandomInt(0, 1) > 0) isFemale = true;
            if (isFemale == true)
            {
                m_childSprite1.GetChildAt(PlayerPart.Bowtie).Visible = true;
                m_childSprite1.GetChildAt(PlayerPart.Boobs).Visible = true;
            }
            else
            {
                m_childSprite1.GetChildAt(PlayerPart.Bowtie).Visible = false;
                m_childSprite1.GetChildAt(PlayerPart.Boobs).Visible = false;
            }

            m_childSprite1.GetChildAt(PlayerPart.Chest).ChangeSprite("PlayerWalkingChest" + m_child1Chest + "_Sprite");
            m_childSprite1.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerWalkingHead" + m_child1Head + "_Sprite");
            m_childSprite1.GetChildAt(PlayerPart.ShoulderA).ChangeSprite("PlayerWalkingShoulderA" + m_child1Shoulders + "_Sprite");
            m_childSprite1.GetChildAt(PlayerPart.ShoulderB).ChangeSprite("PlayerWalkingShoulderB" + m_child1Shoulders + "_Sprite");
            m_childSprite1.PlayAnimation(true);
            Tween.By(m_childSprite1, 3, Tween.EaseNone, "X", "600");
            Tween.AddEndHandlerToLastTween(this, "LevelUpChild1", isFemale);
        }

        public void LevelUpChild1(bool isFemale)
        {
            m_childSprite1.ChangeSprite("PlayerLevelUp_Character");
            m_childSprite1.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_childSprite1.GetChildAt(PlayerPart.Extra).Visible = false;
            m_childSprite1.GetChildAt(PlayerPart.Light).Visible = false;
            m_childSprite1.GetChildAt(PlayerPart.Wings).Visible = false;

            if (isFemale == true)
            {
                m_childSprite1.GetChildAt(PlayerPart.Bowtie).Visible = true;
                m_childSprite1.GetChildAt(PlayerPart.Boobs).Visible = true;
            }
            else
            {
                m_childSprite1.GetChildAt(PlayerPart.Bowtie).Visible = false;
                m_childSprite1.GetChildAt(PlayerPart.Boobs).Visible = false;
            }

            m_childSprite1.GetChildAt(PlayerPart.Chest).ChangeSprite("PlayerLevelUpChest" + m_child1Chest + "_Sprite");
            m_childSprite1.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerLevelUpHead" + m_child1Head + "_Sprite");
            m_childSprite1.GetChildAt(PlayerPart.ShoulderA).ChangeSprite("PlayerLevelUpShoulderA" + m_child1Shoulders + "_Sprite");
            m_childSprite1.GetChildAt(PlayerPart.ShoulderB).ChangeSprite("PlayerLevelUpShoulderB" + m_child1Shoulders + "_Sprite");
            m_childSprite1.PlayAnimation(false);
        }

        public void BringChild2()
        {
            m_childSprite2.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_childSprite2.GetChildAt(PlayerPart.Extra).Visible = false;
            m_childSprite2.GetChildAt(PlayerPart.Light).Visible = false;
            m_childSprite2.GetChildAt(PlayerPart.Wings).Visible = false;

            bool isFemale = false;
            if (CDGMath.RandomInt(0, 1) > 0) isFemale = true;
            if (isFemale == true)
            {
                m_childSprite2.GetChildAt(PlayerPart.Bowtie).Visible = true;
                m_childSprite2.GetChildAt(PlayerPart.Boobs).Visible = true;
            }
            else
            {
                m_childSprite2.GetChildAt(PlayerPart.Bowtie).Visible = false;
                m_childSprite2.GetChildAt(PlayerPart.Boobs).Visible = false;
            }

            m_child2Chest = CDGMath.RandomInt(1, PlayerPart.NumChestPieces);
            m_child2Head = CDGMath.RandomInt(1, PlayerPart.NumHeadPieces);
            m_child2Shoulders = CDGMath.RandomInt(1, PlayerPart.NumShoulderPieces);

            m_childSprite2.GetChildAt(PlayerPart.Chest).ChangeSprite("PlayerWalkingChest" + m_child2Chest + "_Sprite");
            m_childSprite2.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerWalkingHead" + m_child2Head + "_Sprite");
            m_childSprite2.GetChildAt(PlayerPart.ShoulderA).ChangeSprite("PlayerWalkingShoulderA" + m_child2Shoulders + "_Sprite");
            m_childSprite2.GetChildAt(PlayerPart.ShoulderB).ChangeSprite("PlayerWalkingShoulderB" + m_child2Shoulders + "_Sprite");
            m_childSprite2.PlayAnimation(true);
            Tween.By(m_childSprite2, 2, Tween.EaseNone, "X", "600");
            Tween.AddEndHandlerToLastTween(this, "LevelUpChild2", isFemale);
        }

        public void LevelUpChild2(bool isFemale)
        {
            m_childSprite2.ChangeSprite("PlayerLevelUp_Character");
            m_childSprite2.GetChildAt(PlayerPart.Glasses).Visible = false;
            m_childSprite2.GetChildAt(PlayerPart.Extra).Visible = false;
            m_childSprite2.GetChildAt(PlayerPart.Light).Visible = false;
            m_childSprite2.GetChildAt(PlayerPart.Wings).Visible = false;

            if (isFemale == true)
            {
                m_childSprite2.GetChildAt(PlayerPart.Bowtie).Visible = true;
                m_childSprite2.GetChildAt(PlayerPart.Boobs).Visible = true;
            }
            else
            {
                m_childSprite2.GetChildAt(PlayerPart.Bowtie).Visible = false;
                m_childSprite2.GetChildAt(PlayerPart.Boobs).Visible = false;
            }

            m_childSprite2.GetChildAt(PlayerPart.Chest).ChangeSprite("PlayerLevelUpChest" + m_child2Chest + "_Sprite");
            m_childSprite2.GetChildAt(PlayerPart.Head).ChangeSprite("PlayerLevelUpHead" + m_child2Head + "_Sprite");
            m_childSprite2.GetChildAt(PlayerPart.ShoulderA).ChangeSprite("PlayerLevelUpShoulderA" + m_child2Shoulders + "_Sprite");
            m_childSprite2.GetChildAt(PlayerPart.ShoulderB).ChangeSprite("PlayerLevelUpShoulderB" + m_child2Shoulders + "_Sprite");
            m_childSprite2.PlayAnimation(false);

            //This is the last moment of animation. Allow the player to skip.
            m_allowExit = true;
            m_displayingContinueText = true;
            Tween.StopAllContaining(m_continueText, false);
            Tween.To(m_continueText, 0.5f, Tween.EaseNone, "Opacity", "1");
        }


        public override void Draw(GameTime gametime)
        {
            Camera.GraphicsDevice.SetRenderTarget(m_skyRenderTarget);
            Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, null);            
            m_sky.Draw(Camera);
            Camera.End();

            Camera.GraphicsDevice.SetRenderTarget(m_backgroundRenderTarget);
            Camera.GraphicsDevice.Clear(Color.Black);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            m_bg1.Draw(Camera);
            m_bg2.Draw(Camera);
            m_bg3.Draw(Camera);

            m_bgOutside.Draw(Camera);

            m_ground1.Draw(Camera);
            m_ground2.Draw(Camera);
            m_ground3.Draw(Camera);

            m_border1.Draw(Camera);
            m_border2.Draw(Camera);
            m_border3.Draw(Camera);

            m_manor.Draw(Camera);

            m_prop1.Draw(Camera);
            m_prop2.Draw(Camera);
            m_prop3.Draw(Camera);

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

            m_wifeSprite.Draw(Camera);
            m_childSprite1.Draw(Camera);
            m_childSprite2.Draw(Camera);

            m_sideBorderLeft.Draw(Camera);
            m_sideBorderRight.Draw(Camera);
            m_sideBorderTop.Draw(Camera);
            m_sideBorderBottom.Draw(Camera);

            m_teddy.Draw(Camera);
            m_kenny.Draw(Camera);
            m_glauber.Draw(Camera);
            m_gordon.Draw(Camera);
            m_judson.Draw(Camera);
            m_mc.Draw(Camera);

            Camera.End();

            Camera.GraphicsDevice.SetRenderTarget((ScreenManager as RCScreenManager).RenderTarget);
            Camera.GraphicsDevice.Textures[1] = m_skyRenderTarget;
            Camera.GraphicsDevice.Textures[1].GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            Camera.GraphicsDevice.SetRenderTarget((ScreenManager as RCScreenManager).RenderTarget);
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game.ParallaxEffect);
            Camera.Draw(m_backgroundRenderTarget, Vector2.Zero, Color.White);
            Camera.End();

            // Drawing the text separately because it causes purple outlines with the Parallax effect.
            Camera.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null);
            Camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            Rectangle bounds = new Rectangle(0, 0, 1320, 720);
            foreach (TextObj title in m_creditsTitleList)
            {
                if (CollisionMath.Intersects(title.Bounds, bounds))
                   title.Draw(Camera);
            }

            foreach (TextObj name in m_creditsNameList)
            {
                if (CollisionMath.Intersects(name.Bounds, bounds))
                    name.Draw(Camera);
            }

            m_thanksForPlayingText.Draw(Camera);
            m_totalDeaths.Draw(Camera);
            m_totalPlayTime.Draw(Camera);
            
            m_continueText.Draw(Camera);
            Camera.End();

            base.Draw(gametime);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                Console.WriteLine("Disposing Credits Screen");

                Array.Clear(m_backgroundStrings, 0, m_backgroundStrings.Length);
                m_backgroundStrings = null;

                m_playerSprite.Dispose();
                m_playerSprite = null;

                m_wifeSprite.Dispose();
                m_wifeSprite = null;

                m_childSprite1.Dispose();
                m_childSprite1 = null;
                m_childSprite2.Dispose();
                m_childSprite2 = null;

                m_manor.Dispose();
                m_manor = null;

                m_thanksForPlayingText.Dispose();
                m_thanksForPlayingText = null;

                m_sideBorderRight.Dispose();
                m_sideBorderRight = null;
                m_sideBorderLeft.Dispose();
                m_sideBorderLeft = null;
                m_sideBorderTop.Dispose();
                m_sideBorderTop = null;
                m_sideBorderBottom.Dispose();
                m_sideBorderBottom = null;

                m_bgOutside.Dispose();
                m_bgOutside = null;

                if (m_sky != null)
                    m_sky.Dispose();
                m_sky = null;
                if (m_skyRenderTarget != null)
                    m_skyRenderTarget.Dispose();
                m_skyRenderTarget = null;
                if (m_backgroundRenderTarget != null)
                    m_backgroundRenderTarget.Dispose();
                m_backgroundRenderTarget = null;

                foreach (TextObj title in m_creditsTitleList)
                    title.Dispose();
                m_creditsTitleList.Clear();
                m_creditsTitleList = null;

                foreach (TextObj name in m_creditsNameList)
                    name.Dispose();
                m_creditsNameList.Clear();
                m_creditsNameList = null;

                m_bg1.Dispose();
                m_bg2.Dispose();
                m_bg3.Dispose();

                m_ground1.Dispose();
                m_ground2.Dispose();
                m_ground3.Dispose();

                m_border1.Dispose();
                m_border2.Dispose();
                m_border3.Dispose();

                m_prop1.Dispose();
                m_prop2.Dispose();
                m_prop3.Dispose();

                m_prop1 = null;
                m_prop2 = null;
                m_prop3 = null;

                m_bg1 = null;
                m_bg2 = null;
                m_bg3 = null;

                m_ground1 = null;
                m_ground2 = null;
                m_ground3 = null;

                m_border1 = null;
                m_border2 = null;
                m_border3 = null;

                m_teddy.Dispose();
                m_kenny.Dispose();
                m_glauber.Dispose();
                m_gordon.Dispose();
                m_judson.Dispose();
                m_mc.Dispose();

                m_teddy = null;
                m_kenny = null;
                m_glauber = null;
                m_gordon = null;
                m_judson = null;
                m_mc = null;

                m_continueText.Dispose();
                m_continueText = null;

                m_totalDeaths.Dispose();
                m_totalDeaths = null;
                m_totalPlayTime.Dispose();
                m_totalPlayTime = null;

                base.Dispose();
            }
        }

        public override void RefreshTextObjs()
        {
            float hoursPlayed = (Game.PlayerStats.TotalHoursPlayed + Game.HoursPlayedSinceLastSave);
            int minutes = (int)((hoursPlayed - (int)(hoursPlayed)) * 60f);
            m_totalDeaths.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_2") /*"Total Children"*/ + ": " + Game.PlayerStats.TimesDead.ToString();
            if (minutes < 10)
                m_totalPlayTime.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_3") /*"Time Played"*/ + " - " + (int)hoursPlayed + ":0" + minutes;
            else
                m_totalPlayTime.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_3") /*"Time Played"*/ + " - " + (int)hoursPlayed + ":" + minutes;
            //m_continueText.Text = LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_4") + " [Input:" + InputMapType.MENU_CONFIRM1 + "] " + LocaleBuilder.getResourceString("LOC_ID_CREDITS_SCREEN_5");
            base.RefreshTextObjs();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using DS2DEngine;

namespace RogueCastle
{
    static class LocaleBuilder
    {

#region Fields

        private static Dictionary<string, string> m_stringDict = new Dictionary<string, string>();
        private static List<TextObj> m_textObjRefreshList = new List<TextObj>();
        private static LanguageType m_languageType;
        private static CultureInfo DEFAULT_CULTUREINFO = new CultureInfo("en-US", false);
        private static string SPACE_SEPARATOR = " ";

#endregion

#region Properties

        public static LanguageType languageType
        {
            get { return m_languageType; }
            set
            {
                if (m_languageType != value)
                {
                    m_languageType = value;

                    CultureInfo newCI = null;
                    switch (value)
                    {
                        default:
                        case (LanguageType.English):
                            newCI = new CultureInfo("en-US", false);
                            break;
                        case(LanguageType.French):
                            newCI = new CultureInfo("fr", false);
                            break;
                        case(LanguageType.German):
                            newCI = new CultureInfo("de", false);
                            break;
                        case(LanguageType.Portuguese_Brazil):
                            newCI = new CultureInfo("pt-BR", false);
                            break;
                        case(LanguageType.Spanish_Spain):
                            newCI = new CultureInfo("es-ES", false);
                            break;
                        case (LanguageType.Russian):
                            newCI = new CultureInfo("ru-RU", false);
                            break;
                        case (LanguageType.Polish):
                            newCI = new CultureInfo("pl", false);
                            break;
                        case (LanguageType.Chinese_Simp):
                            newCI = new CultureInfo("zh-CHS", false);
                            break;
                    }

                    newCI.NumberFormat.CurrencyDecimalSeparator = ".";
                    Resources.LocStrings.Culture = newCI;
                    if (m_languageType == LanguageType.Chinese_Simp)
                        SPACE_SEPARATOR = "";
                    else
                        SPACE_SEPARATOR = " ";
                }
                else
                    m_languageType = value;
            }
        }
        
        public static string getResourceString(string stringID, bool forceMale = false)
        {
            return getResourceStringCustomFemale(stringID, Game.PlayerStats.IsFemale, forceMale);
        }

        public static string getResourceStringCustomFemale(string stringID, bool isFemale, bool forceMale = false)
        {
            if (forceMale == true || LocaleBuilder.languageType == LanguageType.English || LocaleBuilder.languageType == LanguageType.Chinese_Simp)
                isFemale = false;

            if (stringID.Length > 0)
            {
                string resourceString = "";
                if (isFemale == false)
                    resourceString = Resources.LocStrings.ResourceManager.GetString(stringID, Resources.LocStrings.Culture);
                else
                    resourceString = Resources.LocStrings.ResourceManager.GetString(stringID + "_F", Resources.LocStrings.Culture);
                if (resourceString == null)
                {
                    // There is no female version, try again with the male version.
                    if (isFemale == true)
                        resourceString = Resources.LocStrings.ResourceManager.GetString(stringID, Resources.LocStrings.Culture);

                    // If it's still null, then the entire string is missing both a male and female version (i.e. missing completely).
                    if (isFemale == false || resourceString == null)
                        resourceString = "{NULLSTRING: " + stringID + "}";
                }

                resourceString = resourceString.Replace("\\n", "\n");
                return resourceString;
            }
            else
                return "";
        }

        public static int getResourceInt(string stringID)
        {
            // First try getting from language
            string resourceString = Resources.LocStrings.ResourceManager.GetString(stringID, Resources.LocStrings.Culture);
            if (resourceString == null || resourceString.Length == 0)
                // If empty string or not found, use default english values
                resourceString = Resources.LocStrings.ResourceManager.GetString(stringID, DEFAULT_CULTUREINFO);

            int resourceInt = 0;
            if (resourceString != null && resourceString.Length > 0)
                resourceInt = Convert.ToInt32(resourceString);

            return resourceInt;
        }

        // Uncomment this to read off loc text file instead for easier debugging.
        //public static string getString(string stringID)
        //{
        //    string returnString = "{NULL: " + stringID + "}";
        //    try
        //    {
        //        returnString = m_stringDict[stringID];
        //    }
        //    catch
        //    {
        //        //CDConsole.Log("ERROR: Cannot find locale string id: " + stringID);
        //    }

        //    return returnString;
        //}

        public static string getString(string stringID, TextObj textObj, bool forceMale = false)
        {
            if (textObj != null)
            {
                textObj.locStringID = stringID;
                AddToTextRefreshList(textObj);

                if (languageType != LanguageType.English)
                {
                    textObj.Text = "";
                    
                    textObj.isLogographic = false;
                    if (languageType == LanguageType.Chinese_Simp)
                        textObj.isLogographic = true;

                    textObj.ChangeFontNoDefault(GetLanguageFont(textObj));
                }
            }

            string textString = LocaleBuilder.getResourceString(stringID, forceMale);
            return textString;
        }

        public static SpriteFont GetLanguageFont(TextObj textObj)
        {
            bool ignoreLanguage = false;

            SpriteFont font = textObj.defaultFont;

            if (font == Game.BitFont || font == Game.EnemyLevelFont || font == Game.PlayerLevelFont
                || font == Game.NotoSansSCFont || font == Game.GoldFont || font == Game.PixelArtFont || font == Game.PixelArtFontBold)
                ignoreLanguage = true;

            if (ignoreLanguage == true)
                return font;

            switch (languageType)
            {
                case(LanguageType.Chinese_Simp):
                    return Game.NotoSansSCFont;
                case(LanguageType.Russian):
                   return Game.RobotoSlabFont;
                default:
                    return font;
            }

            /*
            switch (fontName)
            {
                case ("LoveYa15"):
                    switch (languageType)
                    {
                        case (LanguageType.Chinese_Simp):
                            return "NotoSans35CJK";
                        default:
                            return "RobotoSlab15";
                    }
                case ("LoveYa35"):
                    switch (languageType)
                    {
                        case (LanguageType.Chinese_Simp):
                            return "NotoSans35CJK";
                        default:
                            return "RobotoSlab35";
                    }
                case ("RobotoBold20Squeezed"):
                    switch (languageType)
                    {
                        case (LanguageType.Chinese_Simp):
                            return "NotoSans35CJK";
                        default:
                            return fontName;
                    }
                case ("Banger10"):
                case ("Banger35"):
                    switch (languageType)
                    {
                        case (LanguageType.Chinese_Simp):
                            return "NotoSans35CJK";
                        default:
                            return fontName;
                    }
                case ("AllertaStencil35"):
                case ("Allerta15"):
                    switch (languageType)
                    {
                        case (LanguageType.Chinese_Simp):
                            return "NotoSans35CJK";
                        default:
                            return "NotoSans35";
                    }
            }

            return fontName;
             */
        }

        /*
        public static void setString(string stringID, string value)
        {
            if (m_stringDict.ContainsKey(stringID))
                CDConsole.Log("WARNING: String ID: " + stringID + " already found in dictionary. Overwriting string...");

            m_stringDict.Add(stringID, value);
        }*/

#endregion

#region Methods

        public static void LoadLanguageFile(ContentManager content, string filePath)
        {
           // Console.WriteLine("Loading language file: " + filePath);

            string line = null;
            string languageText = null;
            m_stringDict.Clear(); // Clear the dictionary first.

            //using (StreamReader reader = new StreamReader(content.RootDirectory + "\\Languages\\" + filePath))
#if UNSHARPER
            //Blit: todo correct path
            using (StreamReader reader = new StreamReader( content.RootDirectory + "\\Languages\\" + filePath))
#else
            using (StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\" + content.RootDirectory + "\\Languages\\" + filePath))
#endif
            {
                try
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                      languageText += line + ";";
                      languageText = languageText.Replace("\\n", "\n");
                    }

                }
                catch //(Exception e)
                {
                   // Console.WriteLine("Could not load language file - Error: " + e.Message);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }

            // Removes all tab instances.
            languageText = languageText.Replace("\t", "");
            // Putting all lines into a list, delineated by the semi-colon.
            List<string> languageList = languageText.Split(';').ToList();
            // Remove the last entry. ToList() adds a blank entry at the end because the code ends with a semi-colon.
            languageList.RemoveAt(languageList.Count -1);

            foreach (string value in languageList)
            {
                // Ignore blank strings.
                if (value.Length <= 0)
                    continue;
                int indexOfFirstComma = value.IndexOf(",");
                string stringID = value.Substring(0, indexOfFirstComma);
                string text = value.Substring(indexOfFirstComma + 1);
                text = text.TrimStart(' '); // Trims any leading whitespaces.

                if (m_stringDict.ContainsKey(stringID) == false)
                    m_stringDict.Add(stringID, text);
               // else
                  //  Console.WriteLine("WARNING: Cannot add StringID: " + stringID + ", Value: " + text + " as it already exists in the dictionary");
            }
        }

        public static void RefreshAllText()
        {
            foreach (TextObj textObj in m_textObjRefreshList)
            {
                if (textObj != null)
                {
                    textObj.Text = "";

                    if (textObj != null)
                    {
                        textObj.Text = "";
                        textObj.ChangeFontNoDefault(GetLanguageFont(textObj));
                        textObj.Text = getResourceString(textObj.locStringID, false);

                        textObj.isLogographic = false;
                        if (languageType == LanguageType.Chinese_Simp)
                            textObj.isLogographic = true;
                    }

//                    if (languageType == LanguageType.Chinese_Simp)
//                    {
//                        textObj.ChangeFontNoDefault(Game.NotoSansSCFont);
//                        textObj.isLogographic = true;
//                    }
//#if false
//                    else if (languageType == LanguageType.Russian)
//                    {
//                        if (textObj.defaultFont == Game.JunicodeFont || textObj.defaultFont == Game.JunicodeLargeFont)
//                            textObj.ChangeFontNoDefault(Game.RussianFont);
//                        textObj.isLogographic = false;
//                    }
//#endif
//                    else
//                    {
//                        textObj.ChangeFontNoDefault(textObj.defaultFont);
//                        textObj.isLogographic = false;
//                    }
//                    textObj.Text = getResourceString(textObj.locStringID);
                }
            }

            Screen[] screenList = Game.ScreenManager.GetScreens();
            foreach (Screen screen in screenList)
                screen.RefreshTextObjs();
        }

        public static void AddToTextRefreshList(TextObj textObj)
        {
            if (m_textObjRefreshList.Contains(textObj) == false)
                m_textObjRefreshList.Add(textObj);
        }

        public static void RemoveFromTextRefreshList(TextObj textObj)
        {
            if (m_textObjRefreshList.Contains(textObj) == true)
                m_textObjRefreshList.Remove(textObj);
        }

        public static void ClearTextRefreshList()
        {
            m_textObjRefreshList.Clear();
        }

        public static bool TextRefreshListContains(TextObj textObj)
        {
            return m_textObjRefreshList.Contains(textObj);
        }

#endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace SpriteSystem
{
    public class SpriteLibrary
    {
        //SpritesheetData items contain all the spritesheet data for a particular object in a spritesheet, NOT the data for all objects in a spritesheet.

        private static Dictionary<string, SpritesheetObj> _spriteDict; // string is name of graphic (groupName). SpritesheetData holds a list of info for each item index.
        private static Dictionary<string, Texture2D> _textureDict; // string is name of graphics (groupName).
        private static Dictionary<string, List<CharacterData>> _charDataDict;

        private static HashSet<string> m_loadedPathesList;

        public SpriteLibrary()
        {
        }

        public static void Init()
        {
            _spriteDict = new Dictionary<string, SpritesheetObj>();
            _textureDict = new Dictionary<string, Texture2D>();
            _charDataDict = new Dictionary<string, List<CharacterData>>();
            m_loadedPathesList = new HashSet<string>();
        }

        public static List<string> LoadSpritesheet(ContentManager content, string spritesheetName, bool returnCharDataNames)
        {
            // flibit added this
            spritesheetName = spritesheetName.Replace('\\', '/');

            if (m_loadedPathesList.Contains(spritesheetName))
            {
                Console.WriteLine("Spritesheet: " + spritesheetName + " already loaded in the Sprite Library.");
                return null;
            }
            m_loadedPathesList.Add(spritesheetName);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            Texture2D newTexture2D = content.Load<Texture2D>(spritesheetName);
            XmlReader reader = XmlReader.Create(TitleContainer.OpenStream(content.RootDirectory + Path.DirectorySeparatorChar + spritesheetName + ".xml"), settings);
            return ParseData(reader, newTexture2D, returnCharDataNames, spritesheetName);
        }

        /// <summary>
        /// Overloaded method that stores the spritesheet via full string path, as opposed to a content manager.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device that you want to be used to draw this spritesheet.</param>
        /// <param name="fullSpritePath">The full string path of the spritesheet.</param>
        /// <param name="returnCharDataNames">If true, returns a list of all CharData names. If false, returns all Sprite names.</param>
        /// <param name="premultiplyAlpha">If true, premultiplies alpha for the image.</param>
        public static List<string> LoadSpritesheet(GraphicsDevice graphicsDevice, string fullSpritePath, bool returnCharDataNames, bool premultiplyAlpha = false)
        {
            // flibit added this
            fullSpritePath = fullSpritePath.Replace('\\', '/');

            if (m_loadedPathesList.Contains(fullSpritePath))
            {
                Console.WriteLine("Spritesheet Path: " + fullSpritePath + " already loaded in the Sprite Library.");
                return null;
            }
            m_loadedPathesList.Add(fullSpritePath);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            FileStream fs = new FileStream(fullSpritePath, FileMode.Open, FileAccess.Read);
            Texture2D newTexture2D = Texture2D.FromStream(graphicsDevice, fs);
            fs.Close();

            if (newTexture2D == null)
                throw new Exception("File not found at path: " + fullSpritePath);

            if (premultiplyAlpha)
            {
                Color[] data = new Color[newTexture2D.Width * newTexture2D.Height];
                newTexture2D.GetData(data);
                for (int i = 0; i < data.Length; i += 1)
                {
                    data[i] = Color.FromNonPremultiplied(data[i].ToVector4());
                }
                newTexture2D.SetData(data);
            }

            string xmlPath = fullSpritePath.Substring(0, fullSpritePath.Length - 4) + ".xml";
            XmlReader reader = XmlReader.Create(xmlPath, settings);

            return ParseData(reader, newTexture2D, returnCharDataNames, fullSpritePath);
        }

        private static List<string> ParseData(XmlReader reader, Texture2D newTexture2D, bool returnCharDataNames, string spritesheetName)
        {
            List<string> namesList = new List<string>();
            // flibit didn't like this!
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            string lastFrameName = "";
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "instance")
                    {
                        reader.MoveToAttribute("name");
                        string name = reader.Value;
                        if (returnCharDataNames == false && namesList.Contains(name) == false)
                            namesList.Add(name);
                        reader.MoveToAttribute("index");
                        int index = int.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("posX");
                        int posX = int.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("posY");
                        int posY = int.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("anchorX");
                        int anchorX = (int)float.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("anchorY");
                        int anchorY = (int)float.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("width");
                        int width = (int)float.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("height");
                        int height = (int)float.Parse(reader.Value, NumberStyles.Any, ci);

                        if (_textureDict.ContainsKey(name) == false) // If the texture dictionary does not have this image, create a new one.
                        {
                            _textureDict.Add(name, newTexture2D);
                        }

                        if (_spriteDict.ContainsKey(name) == false) // If the sprite dictionary does not have this image's data, create a new one.
                        {
                            SpritesheetObj newSpriteSheet = new SpritesheetObj(name, spritesheetName);
                            newSpriteSheet.AddImageData(index, posX, posY, anchorX, anchorY, width, height);
                            _spriteDict.Add(name, newSpriteSheet);
                        }
                        else
                        {
                            SpritesheetObj spritesheetToAddDataTo = _spriteDict[name];
                            spritesheetToAddDataTo.AddImageData(index, posX, posY, anchorX, anchorY, width, height);
                        }
                    }
                    else if (reader.Name == "animation")
                    {
                        reader.MoveToAttribute("name");
                        string name = reader.Value;
                        lastFrameName = name; // This variable is used so that the rectangle nodes know which animation frame they're tied to.
                        reader.MoveToAttribute("index");
                        int index = int.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("duration");
                        int duration = int.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("frame");
                        int frame = int.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("label");
                        string label = reader.Value;

                        if (_spriteDict.ContainsKey(name) == true)
                        {
                            SpritesheetObj spritesheetToAddDataTo = _spriteDict[name];
                            spritesheetToAddDataTo.AddFrameData(frame, index, duration, label);
                        }
                        else
                            throw new Exception("Attempting to add frame data to non-existent sprite called: " + name);
                    }
                    else if (reader.Name == "rectangle")
                    {
                        reader.MoveToAttribute("x");
                        int x = int.Parse(reader.Value, NumberStyles.Any, ci);
                        //int x = (int)float.Parse(reader.Value);
                        reader.MoveToAttribute("y");
                        int y = int.Parse(reader.Value, NumberStyles.Any, ci);
                        //int y = (int)float.Parse(reader.Value);
                        reader.MoveToAttribute("width");
                        int width = int.Parse(reader.Value, NumberStyles.Any, ci);
                        //int width = (int)float.Parse(reader.Value);
                        reader.MoveToAttribute("height");
                        //int height = int.Parse(reader.Value);
                        int height = (int)float.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("rotation");
                        float rotation = float.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("type");
                        int type = int.Parse(reader.Value, NumberStyles.Any, ci);
                        //int type = (int)float.Parse(reader.Value);

                        if (_spriteDict.ContainsKey(lastFrameName) == true)
                        {
                            SpritesheetObj spritesheetToAddDataTo = _spriteDict[lastFrameName];
                            spritesheetToAddDataTo.AddHitboxToLastFrameData(x, y, width, height, rotation, type);
                        }
                        else
                            throw new Exception("Attempting to add hitbox data to non-existent sprite called: " + lastFrameName);
                        
                    }
                    else if (reader.Name == "container")
                    {
                        reader.MoveToAttribute("name");
                        string parentName = reader.Value;
                        if (returnCharDataNames == true && namesList.Contains(parentName) == false)
                            namesList.Add(parentName);
                        reader.MoveToAttribute("child");
                        string childName = reader.Value;
                        reader.MoveToAttribute("childX");
                        int childX = (int)float.Parse(reader.Value, NumberStyles.Any, ci);
                        reader.MoveToAttribute("childY");
                        int childY = (int)float.Parse(reader.Value, NumberStyles.Any, ci);

                        AddCharacterData(parentName, childName, childX, childY);
                    }
                }
            }

            foreach (KeyValuePair<string, SpritesheetObj> pair in _spriteDict)
            {
                pair.Value.SortImageData();
            }

            return namesList;
        }

        private static void AddCharacterData(string parent, string child, int childX, int childY)
        {
            if (_charDataDict.ContainsKey(parent) == false)
            {
                List<CharacterData> newCharList = new List<CharacterData>();
                CharacterData newCharData = new CharacterData();
                newCharData.Child = child;
                newCharData.ChildX = childX;
                newCharData.ChildY = childY;
                newCharList.Add(newCharData);
                _charDataDict.Add(parent, newCharList);
            }
            else
            {
                List<CharacterData> oldCharList = _charDataDict[parent];
                CharacterData newCharData = new CharacterData();
                newCharData.Child = child;
                newCharData.ChildX = childX;
                newCharData.ChildY = childY;
                oldCharList.Add(newCharData);
            }
        }

        public static bool ContainsSprite(string spriteName)
        {
            return _spriteDict.ContainsKey(spriteName);
        }

        public static bool ContainsCharacter(string charDataName)
        {
            return _charDataDict.ContainsKey(charDataName);
        }

        public static void ClearLibrary()
        {
            // Creating a copy of m_loadedPathesList first because m_loadedPathesList is modified when calling ClearSpritesheet().
            List<string> spritesheetNamesList = new List<string>();
            foreach (string name in m_loadedPathesList)
                spritesheetNamesList.Add(name);

            foreach (string spritesheetName in spritesheetNamesList)
                ClearSpritesheet(spritesheetName);

            foreach (KeyValuePair<string, SpritesheetObj> entry in _spriteDict)
            {
                if (entry.Value.IsDisposed == false)
                    entry.Value.Dispose();
            }
            _spriteDict.Clear();

            foreach (KeyValuePair<string, Texture2D> entry in _textureDict)
            {
                if (entry.Value.IsDisposed == false)
                    entry.Value.Dispose();
            }
            _textureDict.Clear();
            _charDataDict.Clear();
            m_loadedPathesList.Clear();
        }

        // Clears an item from the Sprite Library. The actual texture will not be disposed unless all items that reference that texture are removed.
        public static void ClearItem(string name)
        {
            if (_spriteDict.ContainsKey(name))
            {
                _spriteDict[name].Dispose();
                _spriteDict.Remove(name);
            }

            // Check to see if other SpritesheetObjs reference the texture of the object just removed.  
            //If yes, do not remove texture. 
            //If no, remove and dispose texture.
            bool removeTexture = true;
            Texture2D textureCheck = null;
            if (_textureDict.ContainsKey(name))
                textureCheck = _textureDict[name];

            foreach (KeyValuePair<string, SpritesheetObj> pair in _spriteDict)
            {
                if (_textureDict[pair.Value.SpritesheetObjName] == textureCheck)
                {
                    removeTexture = false;
                    break;
                }
            }

            if (removeTexture == true && textureCheck != null)
            {
                Texture2D textureToDispose = _textureDict[name];
                _textureDict.Remove(name);
                textureToDispose.Dispose();
            }

            if (_charDataDict.ContainsKey(name))
                _charDataDict.Remove(name);
        }

        //Unlike clear item, clear spritesheet will remove every single texture and SpritesheetObj that was loaded from a spritesheet name
        public static void ClearSpritesheet(string spritesheetName)
        {
            // flibit added this
            spritesheetName = spritesheetName.Replace('\\', '/');

            if (m_loadedPathesList.Contains(spritesheetName))
            {
                List<string> spritesheetObjsToRemove = new List<string>();
                Texture2D textureToDispose = null;

                foreach (KeyValuePair<string, SpritesheetObj> pair in _spriteDict)
                {
                    if (pair.Value.SpritesheetName == spritesheetName)
                    {
                        spritesheetObjsToRemove.Add(pair.Value.SpritesheetObjName);
                        if (textureToDispose == null)
                            textureToDispose = _textureDict[pair.Value.SpritesheetObjName];
                    }
                }

                List<string> charDataToRemove = new List<string>();
                foreach (string objName in spritesheetObjsToRemove)
                {
                    if (_spriteDict.ContainsKey(objName)) // These look ups may not be necessary.
                    {
                        _spriteDict[objName].Dispose();
                        _spriteDict.Remove(objName);
                    }

                    if (_textureDict.ContainsKey(objName))
                        _textureDict.Remove(objName);

                    //Check to see if any children of a characterdata object uses these sprites.  If so, remove that characterdata entry.
                    foreach (KeyValuePair<string, List<CharacterData>> pair in _charDataDict)
                    {
                        foreach (CharacterData data in pair.Value)
                        {
                            if (data.Child == objName)
                            {
                                charDataToRemove.Add(pair.Key);
                                break;
                            }
                        }
                    }
                }

                foreach (string charDataName in charDataToRemove)
                {
                    _charDataDict.Remove(charDataName);
                }

                if (textureToDispose != null && textureToDispose.IsDisposed == false)
                    textureToDispose.Dispose();

                m_loadedPathesList.Remove(spritesheetName);
            }
            else
                Console.WriteLine("Could not clear Spritesheet: " + spritesheetName + " from the Sprite Library as it does not exist.");
        }

        public static FrameData GetFrameData(string name, int index)
        {
            return _spriteDict[name].FrmData(index);
        }

        public static List<FrameData> GetFrameDataList(string name)
        {
            return _spriteDict[name].FrmDataList();
        }

        public static int GetFrameCount(string name)
        {
            return _spriteDict[name].FrameCount();
        }

        public static ImageData GetImageData(string name, int index)
        {
            return _spriteDict[name].ImgData(index);
        }

        public static List<ImageData> GetImageDataList(string name)
        {
            return _spriteDict[name].ImgDataList();
        }

        public static Texture2D GetSprite(string name)
        {
            return _textureDict[name];
        }

        public static string GetSpritesheetName(string name)
        {
            return _spriteDict[name].SpritesheetName;
        }

        public static Vector2 GetSSPos(string name, int index)
        {
            return _spriteDict[name].SSPos(index);
        }

        public static Vector2 GetAnchor(string name, int index)
        {
            return _spriteDict[name].Anchor(index);
        }

        public static int GetWidth(string name, int index)
        {
            return _spriteDict[name].Width(index);
        }

        public static int GetHeight(string name, int index)
        {
            return _spriteDict[name].Height(index);
        }

        public static List<CharacterData> GetCharData(string name)
        {
            return _charDataDict[name];
        }

        public static List<String> GetAllSpriteNames()
        {
            return new List<String>(_spriteDict.Keys);
        }

        public static List<String> GetAllSpriteNames(string filePath)
        {
            List<string> listToReturn = new List<string>();
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            string xmlPath = filePath.Substring(0, filePath.Length - 4) + ".xml";
            XmlReader reader = XmlReader.Create(xmlPath, settings);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "instance")
                    {
                        reader.MoveToAttribute("name");
                        string name = reader.Value;
                        if (listToReturn.Contains(name) == false)
                            listToReturn.Add(name);
                    }
                }
            }
            return listToReturn;
        }

        public static List<String> GetAllCharDataNames()
        {
            return new List<String>(_charDataDict.Keys);
        }

        public static List<String> GetAllCharDataNames(string filePath)
        {
             List<string> listToReturn = new List<string>();
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            string xmlPath = filePath.Substring(0, filePath.Length - 4) + ".xml";
            XmlReader reader = XmlReader.Create(xmlPath, settings);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "container")
                    {
                        reader.MoveToAttribute("name");
                        string name = reader.Value;
                        if (listToReturn.Contains(name) == false)
                            listToReturn.Add(name);
                    }
                }
            }
            return listToReturn;
        }

        public static int NumItems
        {
            get { return _charDataDict.Count; }
        }
    }
}

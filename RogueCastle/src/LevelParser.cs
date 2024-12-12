using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Globalization;
using System.Xml;
using DS2DEngine;

namespace RogueCastle
{
    class LevelParser
    {
        private const int MAX_ROOM_SIZE = 4;
        private const int ROOM_HEIGHT = 720;
        private const int ROOM_WIDTH = 1320;

        public static void ParseRooms(string filePath, ContentManager contentManager = null, bool isDLCMap = false)
        {
            // flibit didn't like this
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            XmlReader reader = null;

            string levelPath;
            if (contentManager == null)
                levelPath = filePath;
            else
                levelPath = System.IO.Path.Combine(contentManager.RootDirectory, "Levels", filePath + ".xml");
            reader = XmlReader.Create(TitleContainer.OpenStream(levelPath), settings);

            // STEPS:
            // 1. Finds a room object in the XML doc and creates a new RoomObj based off that data.
            // 2. Creates and stores various object data (like doors, terrain, enemies, etc.) into that RoomObj.
            // 3. That newly and fully populated RoomObj is stored in LevelBuilder via LevelBuilder.StoreRoom().

            RoomObj currentRoom = null;
            RoomObj castleRoom = null;
            RoomObj dungeonRoom = null;
            RoomObj gardenRoom = null;
            RoomObj towerRoom = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // Parsing room object.
                    if (reader.Name == "RoomObject")
                    {
                        currentRoom = new RoomObj();
                        ParseGenericXML(reader, currentRoom);

                        if (isDLCMap == true)
                            currentRoom.IsDLCMap = true;

                        // Start with 5 copies of a room (including the generic one).
                        castleRoom = currentRoom.Clone() as RoomObj;
                        dungeonRoom = currentRoom.Clone() as RoomObj;
                        gardenRoom = currentRoom.Clone() as RoomObj;
                        towerRoom = currentRoom.Clone() as RoomObj;
                    }

                    // Parsing game object.
                    if (reader.Name == "GameObject")
                    {
                        reader.MoveToAttribute("Type");
                        string type = reader.Value;
                        GameObj obj = null;

                        switch (type)
                        {
                            case ("CollHullObj"):
                                obj = new TerrainObj(0, 0);
                                break;
                            case ("DoorObj"):
                                obj = new DoorObj(currentRoom, 0, 0, GameTypes.DoorType.OPEN); // The door type needs to be saved via editor and placed here.
                                break;
                            case ("ChestObj"):
                                if (reader.MoveToAttribute("Fairy"))
                                {
                                    if (bool.Parse(reader.Value) == true)
                                    {
                                        obj = new FairyChestObj(null);
                                        (obj as ChestObj).ChestType = ChestType.Fairy;
                                    }
                                    else
                                        obj = new ChestObj(null);
                                }
                                else
                                    obj = new ChestObj(null);
                                break;
                            case ("HazardObj"):
                                obj = new HazardObj(0, 0);
                                break;
                            case ("BorderObj"):
                                obj = new BorderObj();
                                break;
                            case ("EnemyObj"):
                                reader.MoveToAttribute("Procedural");
                                bool isProcedural = bool.Parse(reader.Value);
                                if (isProcedural == false) // The enemy is not procedural so create him now and add him to the room.
                                {
                                    reader.MoveToAttribute("EnemyType");
                                    byte enemyType = byte.Parse(reader.Value, NumberStyles.Any, ci);

                                    reader.MoveToAttribute("Difficulty");
                                    GameTypes.EnemyDifficulty difficulty = (GameTypes.EnemyDifficulty)Enum.Parse(typeof(GameTypes.EnemyDifficulty), reader.Value, true);
                                    obj = EnemyBuilder.BuildEnemy(enemyType, null, null, null, difficulty);
                                    if (reader.MoveToAttribute("Flip"))
                                    {
                                        if (bool.Parse(reader.Value) == true)
                                        {
                                            obj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                                           // (obj as EnemyObj).InternalFlip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                                        }
                                    }
                                    if (reader.MoveToAttribute("InitialDelay"))
                                        (obj as EnemyObj).InitialLogicDelay = float.Parse(reader.Value, NumberStyles.Any, ci);
                                }
                                else // The enemy is procedural, so leave an EnemyTagObj so that the enemy can be added later.
                                {
                                    reader.MoveToAttribute("EnemyType");
                                    string enemyType = reader.Value;
                                    obj = new EnemyTagObj();
                                    (obj as EnemyTagObj).EnemyType = enemyType;
                                }
                                break;
                            case ("EnemyOrbObj"):
                                reader.MoveToAttribute("OrbType");
                                int orbType = int.Parse(reader.Value, NumberStyles.Any, ci);
                                bool isWaypoint = false;
                                if (reader.MoveToAttribute("IsWaypoint"))
                                    isWaypoint = bool.Parse(reader.Value);

                                if (isWaypoint == true)
                                {
                                    obj = new WaypointObj();
                                    (obj as WaypointObj).OrbType = orbType;
                                }
                                else
                                {
                                    obj = new EnemyOrbObj();
                                    (obj as EnemyOrbObj).OrbType = orbType;
                                    if (reader.MoveToAttribute("ForceFlying"))
                                        (obj as EnemyOrbObj).ForceFlying = bool.Parse(reader.Value);
                                }
                                break;
                            case ("SpriteObj"):
                                reader.MoveToAttribute("SpriteName");
                                if (reader.Value == "LightSource_Sprite")
                                    obj = new LightSourceObj();
                                else
                                    obj = new SpriteObj(reader.Value);
                                break;
                            case ("PhysicsObj"):
                                reader.MoveToAttribute("SpriteName");
                                obj = new PhysicsObj(reader.Value);
                                PhysicsObj physObj = obj as PhysicsObj;
                                //physObj.CollisionTypeTag = GameTypes.CollisionType_WALL;
                                physObj.CollisionTypeTag = GameTypes.CollisionType_WALL_FOR_PLAYER;
                                physObj.CollidesBottom = false;
                                physObj.CollidesLeft = false;
                                physObj.CollidesRight = false;
                                break;
                            case ("PhysicsObjContainer"):
                                bool breakable = false;
                                if (reader.MoveToAttribute("Breakable"))
                                    breakable = bool.Parse(reader.Value);
                                reader.MoveToAttribute("SpriteName");
                                if (breakable == true)
                                    obj = new BreakableObj(reader.Value);
                                else
                                    obj = new PhysicsObjContainer(reader.Value);
                                break;
                            case ("ObjContainer"):
                                reader.MoveToAttribute("SpriteName");
                                obj = new ObjContainer(reader.Value);
                                break;
                            case ("PlayerStartObj"):
                                obj = new PlayerStartObj();
                                break;
                        }

                        ParseGenericXML(reader, obj);

                        GameTypes.LevelType levelType = GameTypes.LevelType.NONE;
                        if (reader.MoveToAttribute("LevelType"))
                            levelType = (GameTypes.LevelType)int.Parse(reader.Value, NumberStyles.Any, ci);

                        if (levelType == GameTypes.LevelType.CASTLE)
                        {
                            StoreObj(obj, castleRoom);
                            StoreSwappedObj(obj, GameTypes.LevelType.DUNGEON, dungeonRoom);
                            StoreSwappedObj(obj, GameTypes.LevelType.TOWER, towerRoom);
                            StoreSwappedObj(obj, GameTypes.LevelType.GARDEN, gardenRoom);

                            // Special code to change the pictures in the picture frames.
                            SpriteObj sprite = obj as SpriteObj;
                            if (sprite != null && sprite.SpriteName == "CastleAssetFrame_Sprite")
                                sprite.ChangeSprite("FramePicture" + CDGMath.RandomInt(1, 16) + "_Sprite");
                        }
                        else if (levelType == GameTypes.LevelType.DUNGEON)
                            StoreObj(obj, dungeonRoom);
                        else if (levelType == GameTypes.LevelType.TOWER)
                            StoreObj(obj, towerRoom);
                        else if (levelType == GameTypes.LevelType.GARDEN)
                            StoreObj(obj, gardenRoom);
                        else
                        {
                            // If the object is generic, put it into all four room types.
                            StoreObj(obj, castleRoom);
                            StoreObj(obj, dungeonRoom);
                            StoreObj(obj, towerRoom);
                            StoreObj(obj, gardenRoom);
                            StoreObj(obj, currentRoom);
                        }

                        // Extra debug code so that when testing a room, the correct LevelType objects will appear in that room.
                        // Removing this code doesn't hurt the game. It just turns Test Rooms into generic rooms.
                        if (LevelEV.RUN_TESTROOM == true && (levelType == LevelEV.TESTROOM_LEVELTYPE || levelType == GameTypes.LevelType.CASTLE))
                        {
                            if (levelType == LevelEV.TESTROOM_LEVELTYPE)
                                StoreObj(obj, currentRoom);
                            else if (levelType == GameTypes.LevelType.CASTLE)
                                StoreSwappedObj(obj, LevelEV.TESTROOM_LEVELTYPE, currentRoom);
                        }

                        // Special handling for test rooms.
                        if (obj is PlayerStartObj)
                            currentRoom.Name += "DEBUG_ROOM";
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "RoomObject")
                {
                    if (currentRoom.X < 10000 && currentRoom.Name != "Boss" && currentRoom.Name != "ChallengeBoss")
                    {
                        if (currentRoom.Name.Contains("DEBUG_ROOM") == false)
                        {
                            if (currentRoom.AddToCastlePool == true)
                            {
                                LevelBuilder2.StoreRoom(castleRoom, GameTypes.LevelType.CASTLE);
                                LevelBuilder2.StoreSpecialRoom(castleRoom, GameTypes.LevelType.CASTLE);
                            }
                            if (currentRoom.AddToDungeonPool == true)
                            {
                                LevelBuilder2.StoreRoom(dungeonRoom, GameTypes.LevelType.DUNGEON);
                                LevelBuilder2.StoreSpecialRoom(dungeonRoom, GameTypes.LevelType.DUNGEON);
                            }
                            if (currentRoom.AddToGardenPool == true)
                            {
                                LevelBuilder2.StoreRoom(gardenRoom, GameTypes.LevelType.GARDEN);
                                LevelBuilder2.StoreSpecialRoom(gardenRoom, GameTypes.LevelType.GARDEN);
                            }
                            if (currentRoom.AddToTowerPool == true)
                            {
                                LevelBuilder2.StoreRoom(towerRoom, GameTypes.LevelType.TOWER);
                                LevelBuilder2.StoreSpecialRoom(towerRoom, GameTypes.LevelType.TOWER);
                            }
                        }

                        // If the room is a debug room, store the debug room, but then store the regular version of the room in the normal list.
                        if (currentRoom.Name.Contains("DEBUG_ROOM"))
                        {
                            currentRoom.Name = currentRoom.Name.Replace("DEBUG_ROOM", "");
                            if (LevelEV.TESTROOM_LEVELTYPE != GameTypes.LevelType.CASTLE)
                                LevelBuilder2.StoreSpecialRoom(currentRoom, GameTypes.LevelType.CASTLE, true); // Store a castle version because SequencedRoomList in LevelBuilder2 checks for the castle version.
                            LevelBuilder2.StoreSpecialRoom(currentRoom, LevelEV.TESTROOM_LEVELTYPE, true);

                            //RoomObj testRoom = currentRoom.Clone() as RoomObj;
                            //testRoom.Name = "DEBUG_ROOM";
                            //LevelBuilder2.StoreSpecialRoom(testRoom);
                            //currentRoom.Name = currentRoom.Name.Replace("DEBUG_ROOM", "");
                        }

                        // If the room is special, store it here.
                        // LevelBuilder2.StoreSpecialRoom(currentRoom, GameTypes.LevelType.CASTLE);
                    }

                    if (currentRoom.X < 10000 && (currentRoom.Name == "Boss" || currentRoom.Name == "ChallengeBoss")) // Special handling for storing boss rooms.
                        LevelBuilder2.StoreSpecialRoom(currentRoom, GameTypes.LevelType.CASTLE);
                }
            }
        }

        public static void ParseGenericXML(XmlReader reader, GameObj obj)
        {
            // flibit didn't like this
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            obj.PopulateFromXMLReader(reader, ci); // Most important line.  Populates the properties for each object.

            // Special reader attribute to determine whether the object is on the front or back layer.
            bool bgLayer = false;
            if (reader.MoveToAttribute("BGLayer"))
                bgLayer = bool.Parse(reader.Value);
            if (bgLayer == true)
                obj.Layer = -1;

            // Force breakable objects to always be collidable. This is needed because breakable objs are just physics obj, and their weighted settings are set to false. Hack.
            BreakableObj breakableObj = obj as BreakableObj;
            if (breakableObj != null)
                breakableObj.IsCollidable = true;
        }

        public static void StoreSwappedObj(GameObj obj, GameTypes.LevelType levelType, RoomObj currentRoom)
        {
            string[] swapList = null;
            switch (levelType)
            {
                case (GameTypes.LevelType.DUNGEON):
                    swapList = LevelEV.DUNGEON_ASSETSWAP_LIST;
                    break;
                case(GameTypes.LevelType.TOWER):
                    swapList = LevelEV.TOWER_ASSETSWAP_LIST;
                    break;
                case (GameTypes.LevelType.GARDEN):
                    swapList = LevelEV.GARDEN_ASSETSWAP_LIST;
                    break;
                default:
                    throw new Exception("Cannot find asset swaplist for leveltype " + levelType);
            }

            // Hack to disable top collisions for Castle Urns.
            BreakableObj breakableObj = obj as BreakableObj;
            if (breakableObj != null && breakableObj.SpriteName.Contains("CastleAssetUrn"))
                breakableObj.CollidesTop = false;

            bool storeObj = false;
            IAnimateableObj clone = obj.Clone() as IAnimateableObj;
            if (clone != null)
            {
                for (int i = 0; i < LevelEV.CASTLE_ASSETSWAP_LIST.Length; i++)
                {
                    if (clone.SpriteName == LevelEV.CASTLE_ASSETSWAP_LIST[i])
                    {
                        string newSprite = swapList[i];
                        if (newSprite.Contains("RANDOM"))
                        {
                            int numRandoms = int.Parse(Convert.ToString(newSprite[(newSprite.IndexOf("RANDOM") + 6)]));
                            int randChoice = CDGMath.RandomInt(1,numRandoms);
                            newSprite = newSprite.Replace("RANDOM" + numRandoms.ToString(), randChoice.ToString());

                            // Special handling to reposition tower holes.
                            if (newSprite.Contains("TowerHole"))
                            {
                                (clone as GameObj).X += CDGMath.RandomInt(-50, 50);
                                (clone as GameObj).Y += CDGMath.RandomInt(-50, 50);
                                if (CDGMath.RandomInt(1, 100) > 70) // 70% chance of the hole being visible
                                    (clone as GameObj).Visible = false;
                            }

                            // Special handling for garden floating rocks.
                            if (newSprite.Contains("GardenFloatingRock"))
                            {
                                HoverObj hover = new HoverObj(newSprite);
                                hover.Position = (clone as GameObj).Position;
                                hover.Amplitude = CDGMath.RandomFloat(-50, 50);
                                hover.HoverSpeed = CDGMath.RandomFloat(-2, 2);
                                hover.Scale = (clone as GameObj).Scale;
                                hover.Layer = (clone as GameObj).Layer;
                                clone = hover;
                            }
                        }

                        // Special code to change the pictures in the picture frames for TOWERS ONLY. Castle done somewhere else. I know, it's awful.
                        if (newSprite == "CastleAssetFrame_Sprite")
                            newSprite = "FramePicture" + CDGMath.RandomInt(1, 16) + "_Sprite";

                        if (newSprite != "")
                        {
                            clone.ChangeSprite(newSprite);
                            storeObj = true;

                            // Hack to make garden fairy sprites translucent.
                            if (newSprite.Contains("GardenFairy"))
                            {
                                (clone as GameObj).X += CDGMath.RandomInt(-25, 25);
                                (clone as GameObj).Y += CDGMath.RandomInt(-25, 25);
                                (clone as GameObj).Opacity = 0.8f;
                            }

                        }
                        break;
                    }
                }
            }

            if (storeObj == true)
                StoreObj(clone as GameObj, currentRoom);
        }

        public static void StoreObj(GameObj obj, RoomObj currentRoom)
        {
            if (obj is EnemyObj)
                currentRoom.EnemyList.Add(obj as EnemyObj);
            else if (obj is DoorObj) // Must go before TerrainObj since for reason they inherit from TerrainObj.
                currentRoom.DoorList.Add(obj as DoorObj);
            else if (obj is TerrainObj)
                currentRoom.TerrainObjList.Add(obj as TerrainObj);
            else if (obj is BorderObj)
                currentRoom.BorderList.Add(obj as BorderObj);
            else
                currentRoom.GameObjList.Add(obj);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DS2DEngine;
using System.Globalization;

namespace RogueCastle
{
    class LevelBuilder2
    {
        private const int MAX_ROOM_SIZE = 4;

        private static List<RoomObj>[,] m_castleRoomArray = new List<RoomObj>[MAX_ROOM_SIZE, MAX_ROOM_SIZE];
        private static List<RoomObj>[,] m_dungeonRoomArray = new List<RoomObj>[MAX_ROOM_SIZE, MAX_ROOM_SIZE];
        private static List<RoomObj>[,] m_towerRoomArray = new List<RoomObj>[MAX_ROOM_SIZE, MAX_ROOM_SIZE];
        private static List<RoomObj>[,] m_gardenRoomArray = new List<RoomObj>[MAX_ROOM_SIZE, MAX_ROOM_SIZE];

        private static List<RoomObj> m_bossRoomArray = null;

        private static RoomObj m_startingRoom = null;
        private static RoomObj m_testRoom = null;
        private static RoomObj m_castleEntranceRoom = null;

        private static RoomObj m_linkerCastleRoom = null;
        private static RoomObj m_linkerDungeonRoom = null;
        private static RoomObj m_linkerGardenRoom = null;
        private static RoomObj m_linkerTowerRoom = null;

        private static RoomObj m_bossCastleEntranceRoom = null;
        private static RoomObj m_bossDungeonEntranceRoom = null;
        private static RoomObj m_bossGardenEntranceRoom = null;
        private static RoomObj m_bossTowerEntranceRoom = null;

        private static List<RoomObj> m_secretCastleRoomArray = null;
        private static List<RoomObj> m_secretGardenRoomArray = null;
        private static List<RoomObj> m_secretTowerRoomArray = null;
        private static List<RoomObj> m_secretDungeonRoomArray = null;

        private static List<RoomObj> m_bonusCastleRoomArray = null;
        private static List<RoomObj> m_bonusGardenRoomArray = null;
        private static List<RoomObj> m_bonusTowerRoomArray = null;
        private static List<RoomObj> m_bonusDungeonRoomArray = null;

        private static List<RoomObj> m_dlcCastleRoomArray = null;
        private static List<RoomObj> m_dlcGardenRoomArray = null;
        private static List<RoomObj> m_dlcTowerRoomArray = null;
        private static List<RoomObj> m_dlcDungeonRoomArray = null;

        private static RoomObj m_tutorialRoom = null;
        private static RoomObj m_throneRoom = null;
        private static RoomObj m_endingRoom = null;
        private static CompassRoomObj m_compassRoom = null;

        // Challenge Room Content.
        private static List<RoomObj> m_challengeRoomArray = null;

        public static void Initialize()
        {
            for (int i = 0; i < MAX_ROOM_SIZE; i++)
            {
                for (int k = 0; k < MAX_ROOM_SIZE; k++)
                {
                    m_castleRoomArray[i, k] = new List<RoomObj>();
                    m_dungeonRoomArray[i, k] = new List<RoomObj>();
                    m_towerRoomArray[i, k] = new List<RoomObj>();
                    m_gardenRoomArray[i, k] = new List<RoomObj>();
                }
            }

            m_secretCastleRoomArray = new List<RoomObj>();
            m_secretGardenRoomArray = new List<RoomObj>();
            m_secretTowerRoomArray = new List<RoomObj>();
            m_secretDungeonRoomArray = new List<RoomObj>();

            m_bonusCastleRoomArray = new List<RoomObj>();
            m_bonusGardenRoomArray = new List<RoomObj>();
            m_bonusTowerRoomArray = new List<RoomObj>();
            m_bonusDungeonRoomArray = new List<RoomObj>();

            m_bossRoomArray = new List<RoomObj>();
            m_challengeRoomArray = new List<RoomObj>();

            m_dlcCastleRoomArray = new List<RoomObj>();
            m_dlcDungeonRoomArray = new List<RoomObj>();
            m_dlcGardenRoomArray = new List<RoomObj>();
            m_dlcTowerRoomArray = new List<RoomObj>();
        }

        public static void StoreRoom(RoomObj room, GameTypes.LevelType levelType)
        {
            if (room.Name != "Start" && room.Name != "Linker" &&
                room.Name != "Boss" && room.Name != "EntranceBoss" &&
                room.Name != "Secret" && room.Name != "Bonus" &&
                room.Name != "CastleEntrance" && room.Name != "Throne" &&
                room.Name != "Tutorial" && room.Name != "Ending" && 
                room.Name != "Compass" && room.Name != "DEBUG_ROOM" && room.Name != "ChallengeBoss")
            {

                if (room.Width % GlobalEV.ScreenWidth != 0)
                    throw new Exception("Room Name: " + room.Name + " is not a width divisible by " + GlobalEV.ScreenWidth + ". Cannot parse the file.");

                if (room.Height % GlobalEV.ScreenHeight != 0)
                    throw new Exception("Room Name: " + room.Name + " is not a height divisible by " + GlobalEV.ScreenHeight + ". Cannot parse the file.");

                int i = (int)(room.Width / GlobalEV.ScreenWidth);
                int k = (int)(room.Height / GlobalEV.ScreenHeight);

                if (room.IsDLCMap == false)
                {
                    List<RoomObj>[,] roomArray = null;
                    switch (levelType)
                    {
                        case (GameTypes.LevelType.CASTLE):
                            roomArray = m_castleRoomArray;
                            break;
                        case (GameTypes.LevelType.DUNGEON):
                            roomArray = m_dungeonRoomArray;
                            break;
                        case (GameTypes.LevelType.TOWER):
                            roomArray = m_towerRoomArray;
                            break;
                        case (GameTypes.LevelType.GARDEN):
                            roomArray = m_gardenRoomArray;
                            break;
                    }

                    roomArray[i - 1, k - 1].Add(room.Clone() as RoomObj);
                    RoomObj roomClone = room.Clone() as RoomObj;
                    roomClone.Reverse(); // Do not reverse the actual room.
                    roomArray[i - 1, k - 1].Add(roomClone);
                }
                else // Storing DLC maps in a separate array.
                {
                    List<RoomObj> dlcRoomArray = GetSequencedDLCRoomList(levelType);

                    dlcRoomArray.Add(room.Clone() as RoomObj);
                    RoomObj roomClone = room.Clone() as RoomObj;
                    roomClone.Reverse(); // Do not reverse the actual room.
                    dlcRoomArray.Add(roomClone);
                }
            }
        }

        public static void StoreSpecialRoom(RoomObj room, GameTypes.LevelType levelType, bool storeDebug = false)
        {
            if (storeDebug == true)
            {
                m_testRoom = room.Clone() as RoomObj;
                m_testRoom.LevelType = LevelEV.TESTROOM_LEVELTYPE;
            }
            //else
            {
                switch (room.Name)
                {
                    case ("Start"):
                        //if (m_bossEntranceRoom != null) throw new Exception("More than 1 boss entrance room found");
                        if (m_startingRoom == null) // Only store the first found copy of the starting room.
                        {
                            m_startingRoom = new StartingRoomObj();
                            m_startingRoom.CopyRoomProperties(room);
                            m_startingRoom.CopyRoomObjects(room);
                        }
                        break;
                    case ("Linker"):
                        //if (m_linkerRoom != null) throw new Exception("More than 1 linker room found");
                        RoomObj linkerRoom = room.Clone() as RoomObj;
                        
                        switch (levelType)
                        {
                            case (GameTypes.LevelType.CASTLE):
                                m_linkerCastleRoom = linkerRoom;
                                break;
                            case (GameTypes.LevelType.DUNGEON):
                                m_linkerDungeonRoom = linkerRoom;
                                break;
                            case (GameTypes.LevelType.TOWER):
                                m_linkerTowerRoom = linkerRoom;
                               break;
                            case (GameTypes.LevelType.GARDEN):
                                m_linkerGardenRoom = linkerRoom;
                               break;
                        }

                        TeleporterObj teleporter = new TeleporterObj();
                        teleporter.Position = new Vector2(linkerRoom.X + linkerRoom.Width / 2f - (teleporter.Bounds.Right - teleporter.AnchorX), linkerRoom.Y + linkerRoom.Height - 60);
                        linkerRoom.GameObjList.Add(teleporter);
                        break;
                    case ("Boss"):
                        foreach (DoorObj door in room.DoorList) // Locking the doors to boss rooms so you can't exit a boss fight.
                        {
                            if (door.IsBossDoor == true)
                                door.Locked = true;
                        }
                        m_bossRoomArray.Add(room.Clone() as RoomObj);
                        break;
                    case ("EntranceBoss"):
                        //if (m_bossEntranceRoom != null) throw new Exception("More than 1 boss entrance room found");
                        RoomObj bossEntranceRoom = room.Clone() as RoomObj;
                        TeleporterObj bossTeleporter = new TeleporterObj();
                        bossTeleporter.Position = new Vector2(bossEntranceRoom.X + bossEntranceRoom.Width / 2f - (bossTeleporter.Bounds.Right - bossTeleporter.AnchorX), bossEntranceRoom.Y + bossEntranceRoom.Height - 60);
                        bossEntranceRoom.GameObjList.Add(bossTeleporter);

                        NpcObj donationBox = null;
                        foreach (GameObj obj in bossEntranceRoom.GameObjList)
                        {
                            if (obj.Name == "donationbox")
                            {
                                donationBox = new NpcObj((obj as ObjContainer).SpriteName);
                                donationBox.Position = obj.Position;
                                donationBox.Y -= 2;
                                donationBox.Name = obj.Name;
                                donationBox.Scale = obj.Scale;
                                donationBox.useArrowIcon = true;
                                obj.Visible = false;
                            }
                        }

                        if (donationBox != null)
                            bossEntranceRoom.GameObjList.Add(donationBox);

                        switch (levelType)
                        {
                            case (GameTypes.LevelType.CASTLE):
                                m_bossCastleEntranceRoom = bossEntranceRoom;
                                break;
                            case (GameTypes.LevelType.DUNGEON):
                                m_bossDungeonEntranceRoom = bossEntranceRoom;
                                break;
                            case (GameTypes.LevelType.TOWER):
                                m_bossTowerEntranceRoom = bossEntranceRoom;
                                break;
                            case (GameTypes.LevelType.GARDEN):
                                m_bossGardenEntranceRoom = bossEntranceRoom;
                                break;
                        }

                        break;
                    case ("CastleEntrance"):
                        // Only store the first copy of the castle entrance room.
                        if (m_castleEntranceRoom == null)
                        {
                            m_castleEntranceRoom = new CastleEntranceRoomObj();
                            m_castleEntranceRoom.CopyRoomProperties(room);
                            m_castleEntranceRoom.CopyRoomObjects(room);
                            m_castleEntranceRoom.LevelType = GameTypes.LevelType.CASTLE;
                        }
                        break;
                    case ("Compass"):
                        if (m_compassRoom == null)
                        {
                            m_compassRoom = new CompassRoomObj();
                            m_compassRoom.CopyRoomProperties(room);
                            m_compassRoom.CopyRoomObjects(room);
                        }
                        break;
                    case ("Secret"):
                        List<RoomObj> secretRoomArray = null;

                        switch (levelType)
                        {
                            case (GameTypes.LevelType.CASTLE):
                                secretRoomArray = m_secretCastleRoomArray;
                                break;
                            case (GameTypes.LevelType.DUNGEON):
                                secretRoomArray = m_secretDungeonRoomArray;
                                break;
                            case (GameTypes.LevelType.TOWER):
                                secretRoomArray = m_secretTowerRoomArray;
                                break;
                            case (GameTypes.LevelType.GARDEN):
                                secretRoomArray = m_secretGardenRoomArray;
                                break;
                        }

                        secretRoomArray.Add(room.Clone() as RoomObj);
                        RoomObj roomClone = room.Clone() as RoomObj;
                        roomClone.Reverse(); // Do not reverse the actual room.
                        secretRoomArray.Add(roomClone);
                        break;
                    case ("Bonus"):
                        List<RoomObj> bonusRoomArray = null;

                        switch (levelType)
                        {
                            case (GameTypes.LevelType.CASTLE):
                                bonusRoomArray = m_bonusCastleRoomArray;
                                break;
                            case (GameTypes.LevelType.DUNGEON):
                                bonusRoomArray = m_bonusDungeonRoomArray;
                                break;
                            case (GameTypes.LevelType.TOWER):
                                bonusRoomArray = m_bonusTowerRoomArray;
                                break;
                            case (GameTypes.LevelType.GARDEN):
                                bonusRoomArray = m_bonusGardenRoomArray;
                                break;
                        }

                        bonusRoomArray.Add(room.Clone() as RoomObj);
                        RoomObj bonusRoomClone = room.Clone() as RoomObj;
                        bonusRoomClone.Reverse(); // Do not reverse the actual room.
                        bonusRoomArray.Add(bonusRoomClone);
                        break;
                    case ("Tutorial"):
                        if (m_tutorialRoom == null)
                        {
                            m_tutorialRoom = new TutorialRoomObj();
                            m_tutorialRoom.CopyRoomProperties(room);
                            m_tutorialRoom.CopyRoomObjects(room);
                        }
                        break;
                    case("Throne"):
                        if (m_throneRoom == null)
                        {
                            m_throneRoom = new ThroneRoomObj();
                            m_throneRoom.CopyRoomProperties(room);
                            m_throneRoom.CopyRoomObjects(room);
                        }
                        break;
                    case ("Ending"):
                        if (m_endingRoom == null)
                        {
                            m_endingRoom = new EndingRoomObj();
                            m_endingRoom.CopyRoomProperties(room);
                            m_endingRoom.CopyRoomObjects(room);
                        }
                        break;
                    case ("ChallengeBoss"):
                        foreach (DoorObj door in room.DoorList) // Locking the doors to challenge rooms so you can't exit the challenge.
                        {
                            if (door.IsBossDoor == true)
                                door.Locked = true;
                        }
                        m_challengeRoomArray.Add(room.Clone() as RoomObj);
                        break;
                }
            }
        }

        //public static List<RoomObj> CreateArea(int roomSize, GameTypes.LevelType levelType, List<RoomObj> roomsToCheckCollisionsList, RoomObj startingRoom, bool firstRoom)
        public static List<RoomObj> CreateArea(int areaSize, AreaStruct areaInfo, List<RoomObj> roomsToCheckCollisionsList, RoomObj startingRoom, bool firstRoom)
        {
            // The way boss rooms work, is as more and more rooms get built, the chance of a boss room appearing increases, until it hits 100% for the final room.
            bool bossAdded = false;
            float bossRoomChance = -100;
            float bossRoomChanceIncrease = (100f / areaSize);
            if (areaInfo.BossInArea == true)
                bossRoomChance = 0;
            else
                bossAdded = true;

            int secretRoomsNeeded = CDGMath.RandomInt((int)areaInfo.SecretRooms.X, (int)areaInfo.SecretRooms.Y);
            int secretRoomTotal = secretRoomsNeeded;
            int secretRoomIncrease = (int)(areaSize / (secretRoomsNeeded + 1)); // The number of rooms you need to increase by before adding a secret room.
            int nextSecretRoomToAdd = secretRoomIncrease; // This is where the first secret room should be added.
            List<RoomObj> secretRoomArrayCopy = new List<RoomObj>(); // Make a clone of the secret room array.

            List<RoomObj> secretRoomArray = null;
            List<RoomObj> bonusRoomArray = null;
            switch (areaInfo.LevelType)
            {
                case(GameTypes.LevelType.CASTLE):
                    secretRoomArray = m_secretCastleRoomArray;
                    bonusRoomArray = m_bonusCastleRoomArray;
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    secretRoomArray = m_secretDungeonRoomArray;
                    bonusRoomArray = m_bonusDungeonRoomArray;
                    break;
                case (GameTypes.LevelType.GARDEN):
                    secretRoomArray = m_secretGardenRoomArray;
                    bonusRoomArray = m_bonusGardenRoomArray;
                    break;
                case (GameTypes.LevelType.TOWER):
                    secretRoomArray = m_secretTowerRoomArray;
                    bonusRoomArray = m_bonusTowerRoomArray;
                    break;
            }

            secretRoomArrayCopy.AddRange(secretRoomArray);

            int bonusRoomsNeeded = CDGMath.RandomInt((int)areaInfo.BonusRooms.X, (int)areaInfo.BonusRooms.Y);
            int bonusRoomTotal = bonusRoomsNeeded;
            int bonusRoomIncrease = (int)(areaSize / (bonusRoomsNeeded + 1));
            int nextBonusRoomToAdd = bonusRoomIncrease;
            List<RoomObj> bonusRoomArrayCopy = new List<RoomObj>(); // Make a clone of the bonus room array.
            bonusRoomArrayCopy.AddRange(bonusRoomArray);

            if (areaInfo.SecretRooms.Y > secretRoomArray.Count)
                throw new Exception("Cannot add " + (int)areaInfo.SecretRooms.Y + " secret rooms from pool of " + secretRoomArray.Count + " secret rooms.");

            if (areaInfo.BonusRooms.Y > bonusRoomArray.Count)
                throw new Exception("Cannot add " + (int)areaInfo.BonusRooms.Y + " bonus rooms from pool of " + bonusRoomArray.Count + " bonus rooms.");

            GameTypes.LevelType levelType = areaInfo.LevelType;

            //Creating a copy of all the rooms in the level so that I can manipulate this list at will without affecting the actual list.
            List<RoomObj> tempRoomsToCheckCollisionsList = new List<RoomObj>();

            tempRoomsToCheckCollisionsList.AddRange(roomsToCheckCollisionsList);

            List<DoorObj> doorsToLink = new List<DoorObj>(); // This might run better as a linked list or maybe a FIFO queue.
            List<RoomObj> roomList = new List<RoomObj>();
            int numRoomsLeftToCreate = areaSize;
            int leftDoorPercent = 0;
            int rightDoorPercent = 0;
            int topDoorPercent = 0;
            int bottomDoorPercent = 0;
            string startingDoorPosition = "NONE";

            switch (levelType)
            {
                case (GameTypes.LevelType.CASTLE):
                    leftDoorPercent = LevelEV.LEVEL_CASTLE_LEFTDOOR;
                    rightDoorPercent = LevelEV.LEVEL_CASTLE_RIGHTDOOR;
                    topDoorPercent = LevelEV.LEVEL_CASTLE_TOPDOOR;
                    bottomDoorPercent = LevelEV.LEVEL_CASTLE_BOTTOMDOOR;
                    startingDoorPosition = "Right";
                    break;
                case (GameTypes.LevelType.GARDEN):
                    leftDoorPercent = LevelEV.LEVEL_GARDEN_LEFTDOOR;
                    rightDoorPercent = LevelEV.LEVEL_GARDEN_RIGHTDOOR;
                    topDoorPercent = LevelEV.LEVEL_GARDEN_TOPDOOR;
                    bottomDoorPercent = LevelEV.LEVEL_GARDEN_BOTTOMDOOR;
                    startingDoorPosition = "Right"; //"Right"; TEDDY - SO GARDEN CAN CONNECT TOP
                    break;
                case (GameTypes.LevelType.TOWER):
                    leftDoorPercent = LevelEV.LEVEL_TOWER_LEFTDOOR;
                    rightDoorPercent = LevelEV.LEVEL_TOWER_RIGHTDOOR;
                    topDoorPercent = LevelEV.LEVEL_TOWER_TOPDOOR;
                    bottomDoorPercent = LevelEV.LEVEL_TOWER_BOTTOMDOOR;
                    startingDoorPosition = "Top";
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    leftDoorPercent = LevelEV.LEVEL_DUNGEON_LEFTDOOR;
                    rightDoorPercent = LevelEV.LEVEL_DUNGEON_RIGHTDOOR;
                    topDoorPercent = LevelEV.LEVEL_DUNGEON_TOPDOOR;
                    bottomDoorPercent = LevelEV.LEVEL_DUNGEON_BOTTOMDOOR;
                    startingDoorPosition = "Bottom";//"Bottom"; TEDDY - SO GARDEN CAN CONNECT TOP
                    break;
            }
            DoorObj startingDoor = null;

            if (firstRoom == true)
            {
                roomList.Add(startingRoom);
                tempRoomsToCheckCollisionsList.Add(startingRoom);

                startingRoom.LevelType = GameTypes.LevelType.NONE;
                numRoomsLeftToCreate--; // Because the starting room is added to the list so reduce the number of rooms that need to be made by 1.
                MoveRoom(startingRoom, Vector2.Zero);// Sets the starting room to position (0,0) for simplicity.

                // Adding the castle entrance room to the game after the starting room.
                RoomObj castleEntrance = m_castleEntranceRoom.Clone() as RoomObj;
                roomList.Add(castleEntrance);
                tempRoomsToCheckCollisionsList.Add(castleEntrance);
                //castleEntrance.LevelType = GameTypes.LevelType.NONE;//GameTypes.LevelType.CASTLE; // Why?
                numRoomsLeftToCreate--;
                MoveRoom(castleEntrance, new Vector2(startingRoom.X + startingRoom.Width, startingRoom.Bounds.Bottom - castleEntrance.Height));

                startingRoom = castleEntrance; // This last line is extremely important. It sets this room as the new starting room to attach new rooms to.
            }

            //Finding the first door to the right in the starting room
            foreach (DoorObj door in startingRoom.DoorList)
            {
                if (door.DoorPosition == startingDoorPosition)
                {
                    doorsToLink.Add(door);
                    startingDoor = door;
                    break;
                }
            }

            // Close remaining doors in the very first room of the level.
            // This shouldn't be necessary since the starting room is manually created, so no extra doors should exist.
            //if (firstRoom == true)
            //{
            //    for (int i = 0; i < startingRoom.DoorList.Count; i++)
            //    {
            //        if (startingRoom.DoorList[i] != startingDoor)
            //        {
            //            RemoveDoorFromRoom(startingRoom.DoorList[i]);
            //            i--;
            //        }
            //    }
            //}

            if (doorsToLink.Count == 0) // Making sure the starting room has a door positioned to the right.
                throw new Exception("The starting room does not have a " + startingDoorPosition + " positioned door. Cannot create level.");

            while (numRoomsLeftToCreate > 0)
            {
                if (doorsToLink.Count <= 0)
                {
                    Console.WriteLine("ERROR: Ran out of available rooms to make.");
                    break;
                }

                bool linkDoor = false; // Each door has a percentage chance of linking to another room. This bool keeps track of that.
                DoorObj needsLinking = doorsToLink[0]; // Always link the first door in the list. Once that's done, remove the door from the list and go through the list again.
                //if (doorsToLink.Count == 1 && numRoomsLeftToCreate > 0) // If there are still rooms to build but this is the last available door in the list, force it open.
                if ((doorsToLink.Count <= 5 && needsLinking != startingDoor && numRoomsLeftToCreate > 0) || needsLinking == startingDoor) // If there are still rooms to build but this is the last available door in the list, force it open.
                    linkDoor = true;
                else // Each door has a percentage chance of linking to another room. This code determines that chance.
                {
                    int percentChance = 100;
                    switch (needsLinking.DoorPosition)
                    {
                        case ("Left"):
                            percentChance = leftDoorPercent;
                            break;
                        case ("Right"):
                            percentChance = rightDoorPercent;
                            break;
                        case ("Top"):
                            percentChance = topDoorPercent;
                            break;
                        case ("Bottom"):
                            percentChance = bottomDoorPercent;
                            break;
                    }

                    if (percentChance - CDGMath.RandomInt(1, 100) >= 0)
                        linkDoor = true;
                }

                if (linkDoor == false)
                {
                    doorsToLink.Remove(needsLinking);
                    continue; // This continue forces the while loop to go through the next iteration.
                }

                List<DoorObj> suitableDoorList = null;

                bool addingBossRoom = false;
                if (bossRoomChance >= CDGMath.RandomInt(50, 100) && bossAdded == false)
                {
                    RoomObj bossEntranceRoom = null;

                    switch (areaInfo.LevelType)
                    {
                        case (GameTypes.LevelType.CASTLE):
                            bossEntranceRoom = m_bossCastleEntranceRoom;
                            break;
                        case (GameTypes.LevelType.DUNGEON):
                            bossEntranceRoom = m_bossDungeonEntranceRoom;
                            break;
                        case (GameTypes.LevelType.GARDEN):
                            bossEntranceRoom = m_bossGardenEntranceRoom;
                            break;
                        case (GameTypes.LevelType.TOWER):
                            bossEntranceRoom = m_bossTowerEntranceRoom;
                            break;
                    }

                    addingBossRoom = true;
                    string doorNeeded = GetOppositeDoorPosition(needsLinking.DoorPosition);
                    suitableDoorList = new List<DoorObj>();
                    foreach (DoorObj door in bossEntranceRoom.DoorList)
                    {
                        if (door.DoorPosition == doorNeeded && CheckForRoomCollision(needsLinking, tempRoomsToCheckCollisionsList, door) == false)
                        {
                            if (suitableDoorList.Contains(door) == false)
                            {
                                bossAdded = true;
                                suitableDoorList.Add(door);
                                break;
                            }
                        }
                    }
                }
                else
                    bossRoomChance += bossRoomChanceIncrease;

                bool addingSpecialRoom = false;
                bool addingSecretRoom = false;

                if ((addingBossRoom == true && bossAdded == false) || addingBossRoom == false) // Only continue adding rooms if the boss room isn't being added at the moment or if no suitable boss room could be found.
                {
                    if (roomList.Count >= nextSecretRoomToAdd && secretRoomsNeeded > 0) // Add a secret room instead of a normal room.
                    {
                        addingSpecialRoom = true;
                        addingSecretRoom = true;
                        suitableDoorList = FindSuitableDoors(needsLinking, secretRoomArrayCopy, tempRoomsToCheckCollisionsList);
                    }
                    else if (roomList.Count >= nextBonusRoomToAdd && bonusRoomsNeeded > 0) // Add a bonus room instead of a normal room.
                    {
                        addingSpecialRoom = true;
                        suitableDoorList = FindSuitableDoors(needsLinking, bonusRoomArrayCopy, tempRoomsToCheckCollisionsList);
                    }

                    // If you are not adding a special room, or if you are adding a special room but you cannot find a suitable room, add a normal room.
                    if (addingSpecialRoom == false || (addingSpecialRoom == true && suitableDoorList.Count == 0))
                    {
                        if (roomList.Count < 5) // When building a level early, make sure you don't accidentally choose rooms with no doors (or only 1 door) in them.
                            suitableDoorList = FindSuitableDoors(needsLinking, MAX_ROOM_SIZE, MAX_ROOM_SIZE, tempRoomsToCheckCollisionsList, levelType, true);  // Searches all rooms up to the specified size and finds suitable doors to connect to the current door.
                        else // The list of rooms already added needs to be passed in to check for room collisions.
                            suitableDoorList = FindSuitableDoors(needsLinking, MAX_ROOM_SIZE, MAX_ROOM_SIZE, tempRoomsToCheckCollisionsList, levelType, false);  // Searches all rooms up to the specified size and finds suitable doors to connect to the current door.
                    }
                    else // You are adding a special room and you have found a suitable list of rooms it can connect to. Yay!
                    {
                        if (addingSecretRoom == true)
                        {
                            nextSecretRoomToAdd = roomList.Count + secretRoomIncrease;
                            secretRoomsNeeded--;
                        }
                        else if (addingSecretRoom == false)
                        {
                            nextBonusRoomToAdd = roomList.Count + bonusRoomIncrease;
                            bonusRoomsNeeded--;
                        }
                    }
                }

                // If for some reason not a single suitable room could be found, remove this door from the list of doors that need rooms connected to them.
                if (suitableDoorList.Count == 0)
                    doorsToLink.Remove(needsLinking);
                else
                {
                    int randomDoorIndex = CDGMath.RandomInt(0, suitableDoorList.Count - 1); // Choose a random door index from the suitableDoorList to attach to the door.
                    CDGMath.Shuffle<DoorObj>(suitableDoorList);
                    DoorObj doorToLinkTo = suitableDoorList[randomDoorIndex];

                    // This code prevents the same special rooms from being added to an area twice.
                    if (addingSpecialRoom == true)
                    {
                        if (addingSecretRoom == true)
                            secretRoomArrayCopy.Remove(doorToLinkTo.Room);
                        else if (addingSecretRoom == false)
                            bonusRoomArrayCopy.Remove(doorToLinkTo.Room);
                    }

                    RoomObj roomToAdd = doorToLinkTo.Room.Clone() as RoomObj; // Make sure to get a clone of the room since suitableDoorList returns a list of suitable rooms from the actual LevelBuilder array.
                    //roomToAdd.LevelType = levelType; // Set the room level type.
                    foreach (DoorObj door in roomToAdd.DoorList)
                    {
                        if (door.Position == doorToLinkTo.Position)
                        {
                            doorToLinkTo = door;  // Because roomToAdd is cloned from doorToLinkTo, doorToLinkTo needs to relink itself to the cloned room.
                            break;
                        }
                    }

                    roomToAdd.LevelType = levelType; // Setting the room LevelType.
                    roomToAdd.TextureColor = areaInfo.Color; // Setting the room Color.
                    roomList.Add(roomToAdd);
                    tempRoomsToCheckCollisionsList.Add(roomToAdd); // Add the newly selected room to the list that checks for room collisions.

                    // Positioning the newly linked room next to the starting room based on the door's position.
                    Vector2 newRoomPosition = Vector2.Zero;
                    switch (needsLinking.DoorPosition)
                    {
                        case ("Left"):
                            newRoomPosition = new Vector2(needsLinking.X - doorToLinkTo.Room.Width, needsLinking.Y - (doorToLinkTo.Y - doorToLinkTo.Room.Y));
                            break;
                        case ("Right"):
                            newRoomPosition = new Vector2(needsLinking.X + needsLinking.Width, needsLinking.Y - (doorToLinkTo.Y - doorToLinkTo.Room.Y));
                            break;
                        case ("Top"):
                            newRoomPosition = new Vector2(needsLinking.X - (doorToLinkTo.X - doorToLinkTo.Room.X), needsLinking.Y - doorToLinkTo.Room.Height);
                            break;
                        case ("Bottom"):
                            newRoomPosition = new Vector2(needsLinking.X - (doorToLinkTo.X - doorToLinkTo.Room.X), needsLinking.Y + needsLinking.Height);
                            break;
                    }

                    MoveRoom(roomToAdd, newRoomPosition);

                    numRoomsLeftToCreate--; // Reducing the number of rooms that need to be made.
                    doorsToLink.Remove(needsLinking); // Remove the door that was just linked from the list of doors that need linking.
                    foreach (DoorObj door in roomToAdd.DoorList) // Add all doors (except the door that is linked to) in the newly created room to the list of doors that need linking.
                    {
                        //if (door != doorToLinkTo && door.Room != startingRoom && door.X > 0) // Prevents checking for doors where xPos == 0.
                        if (door != doorToLinkTo && door.Room != startingRoom && door.X >= m_startingRoom.Width) // Prevents the creation of rooms above the starting room.
                            doorsToLink.Add(door);
                        //   else
                        //       Console.WriteLine("this should be called");
                    }

                    // And finally what is perhaps the most important part, set the Attached flag to each door to true so that there is a way to know if this door is connected to another door.
                    doorToLinkTo.Attached = true;
                    needsLinking.Attached = true;
                }
            }

            //CloseRemainingDoors(roomList); //This is called in the level so that other levels can be added to the room at a later date.
            //Console.WriteLine("Rooms created: " + roomList.Count);
            //Console.Write("{ ");
            //foreach (RoomObj room in roomList)
            //    Console.Write(room.PoolIndex + " ");
            //Console.WriteLine("}");

            if (secretRoomsNeeded != 0)
                Console.WriteLine("WARNING: Only " + (secretRoomTotal - secretRoomsNeeded) + " secret rooms of " + secretRoomTotal + " creation attempts were successful");
            if (bonusRoomsNeeded != 0)
                Console.WriteLine("WARNING: Only " + (bonusRoomTotal - bonusRoomsNeeded) + " secret rooms of " + bonusRoomTotal + " creation attempts were successful");

            return roomList;
        }

        private static List<DoorObj> FindSuitableDoors(DoorObj doorToCheck, int roomWidth, int roomHeight, List<RoomObj> roomList, GameTypes.LevelType levelType, bool findRoomsWithMoreDoors)
        {
            List<DoorObj> suitableDoorList = new List<DoorObj>();
            string doorPositionToCheck = GetOppositeDoorPosition(doorToCheck.DoorPosition);

            for (int i = 1; i <= roomWidth; i++)
            {
                for (int k = 1; k <= roomHeight; k++)
                {
                    List<RoomObj> gameRoomList = LevelBuilder2.GetRoomList(i, k, levelType);
                    foreach (RoomObj room in gameRoomList)
                    {
                        bool allowRoom = false; // A check to see if this room is allowed in the pool of rooms to check. Based on level type.
                        if (findRoomsWithMoreDoors == false || (findRoomsWithMoreDoors == true && room.DoorList.Count > 1)) // If findRoomsWithMoreDoors == true, it will only add rooms with more than 1 door (no dead ends).
                        {
                            allowRoom = true;
                        }

                        if (allowRoom == true)
                        {
                            foreach (DoorObj door in room.DoorList)
                            {
                                if (door.DoorPosition == doorPositionToCheck)
                                {
                                    bool collisionFound = CheckForRoomCollision(doorToCheck, roomList, door);

                                    if (collisionFound == false && suitableDoorList.Contains(door) == false)
                                        suitableDoorList.Add(door);
                                }
                            }
                        }
                    }
                }
            }

            // Appending the DLC rooms.
            List<RoomObj> dlcRoomList = GetSequencedDLCRoomList(levelType);
            foreach (RoomObj room in dlcRoomList)
            {
                bool allowRoom = false; // A check to see if this room is allowed in the pool of rooms to check. Based on level type.
                if (findRoomsWithMoreDoors == false || (findRoomsWithMoreDoors == true && room.DoorList.Count > 1)) // If findRoomsWithMoreDoors == true, it will only add rooms with more than 1 door (no dead ends).
                {
                    allowRoom = true;
                }

                if (allowRoom == true)
                {
                    foreach (DoorObj door in room.DoorList)
                    {
                        if (door.DoorPosition == doorPositionToCheck)
                        {
                            bool collisionFound = CheckForRoomCollision(doorToCheck, roomList, door);

                            if (collisionFound == false && suitableDoorList.Contains(door) == false)
                                suitableDoorList.Add(door);
                        }
                    }
                }
            }

            return suitableDoorList;
        }

        // Same as method above, except simpler. Finds a suitable list of doors from the provided room list.  Used for adding secret and bonus rooms.
        private static List<DoorObj> FindSuitableDoors(DoorObj doorToCheck, List<RoomObj> roomList, List<RoomObj> roomCollisionList)
        {
            List<DoorObj> suitableDoorList = new List<DoorObj>();
            string doorPositionToCheck = GetOppositeDoorPosition(doorToCheck.DoorPosition);

            foreach (RoomObj room in roomList)
            {
                // Do not add diary rooms to the procedural generation if you have unlocked all the diaries.
                if ((Game.PlayerStats.DiaryEntry >= LevelEV.TOTAL_JOURNAL_ENTRIES - 1) && room.Name == "Bonus" && room.Tag == BonusRoomTypes.Diary.ToString())
                    continue;

                foreach (DoorObj door in room.DoorList)
                {
                    if (door.DoorPosition == doorPositionToCheck)
                    {
                        bool collisionFound = CheckForRoomCollision(doorToCheck, roomCollisionList, door);

                        if (collisionFound == false && suitableDoorList.Contains(door) == false)
                            suitableDoorList.Add(door);
                    }
                }
            }
            return suitableDoorList;
        }

        private static void RemoveDoorFromRoom(DoorObj doorToRemove)
        {
            //Add a wall to the room where the door is and remove the door, effectively closing it.
            RoomObj roomToCloseDoor = doorToRemove.Room;

            TerrainObj doorTerrain = new TerrainObj(doorToRemove.Width, doorToRemove.Height);
            doorTerrain.AddCollisionBox(0, 0, doorTerrain.Width, doorTerrain.Height, Consts.TERRAIN_HITBOX);
            doorTerrain.AddCollisionBox(0, 0, doorTerrain.Width, doorTerrain.Height, Consts.BODY_HITBOX);
            doorTerrain.Position = doorToRemove.Position;
            roomToCloseDoor.TerrainObjList.Add(doorTerrain);

            // Add the newly created wall's borders.
            BorderObj borderObj = new BorderObj();
            borderObj.Position = doorTerrain.Position;
            borderObj.SetWidth(doorTerrain.Width);
            borderObj.SetHeight(doorTerrain.Height);

            switch (doorToRemove.DoorPosition)
            {
                case ("Left"):
                    borderObj.BorderRight = true;
                    break;
                case ("Right"):
                    borderObj.BorderLeft = true;
                    break;
                case ("Top"):
                    borderObj.BorderBottom = true;
                    break;
                case ("Bottom"):
                    borderObj.BorderTop = true;
                    break;
            }
            roomToCloseDoor.BorderList.Add(borderObj);

            roomToCloseDoor.DoorList.Remove(doorToRemove);
            doorToRemove.Dispose();
        }

        public static void CloseRemainingDoors(List<RoomObj> roomList)
        {
            List<DoorObj> doorList = new List<DoorObj>();
            List<DoorObj> allDoorList = new List<DoorObj>();

            foreach (RoomObj room in roomList)
            {
                foreach (DoorObj door in room.DoorList)
                {
                    if (door.DoorPosition != "None")
                    {
                        allDoorList.Add(door);
                        if (door.Attached == false)
                            doorList.Add(door);
                    }
                }
            }

            foreach (DoorObj firstDoor in doorList)
            {
                bool removeDoor = true;

                foreach (DoorObj secondDoor in allDoorList)
                {
                    if (firstDoor != secondDoor)
                    {
                        switch (firstDoor.DoorPosition)
                        {
                            case ("Left"):
                                if (secondDoor.X < firstDoor.X && CollisionMath.Intersects(new Rectangle((int)(firstDoor.X - 5), (int)(firstDoor.Y), firstDoor.Width, firstDoor.Height), secondDoor.Bounds))
                                    removeDoor = false;
                                break;
                            case ("Right"):
                                if (secondDoor.X > firstDoor.X && CollisionMath.Intersects(new Rectangle((int)(firstDoor.X + 5), (int)(firstDoor.Y), firstDoor.Width, firstDoor.Height), secondDoor.Bounds))
                                    removeDoor = false;
                                break;
                            case ("Top"):
                                if (secondDoor.Y < firstDoor.Y && CollisionMath.Intersects(new Rectangle((int)(firstDoor.X), (int)(firstDoor.Y - 5), firstDoor.Width, firstDoor.Height), secondDoor.Bounds))
                                    removeDoor = false;
                                break;
                            case ("Bottom"):
                                if (secondDoor.Y > firstDoor.Y && CollisionMath.Intersects(new Rectangle((int)(firstDoor.X - 5), (int)(firstDoor.Y + 5), firstDoor.Width, firstDoor.Height), secondDoor.Bounds))
                                    removeDoor = false;
                                break;
                            case ("None"):
                                removeDoor = false;
                                break;
                        }
                    }
                }

                if (removeDoor == true)
                    RemoveDoorFromRoom(firstDoor);
                else
                    firstDoor.Attached = true;
            }
        }

        public static void AddDoorBorders(List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
            {
                foreach (DoorObj door in room.DoorList)
                {
                    // Code for attaching borders to the doors.
                    switch (door.DoorPosition)
                    {
                        case ("Left"):
                        case ("Right"):
                            BorderObj bottomBorder = new BorderObj();
                            bottomBorder.Position = new Vector2(door.Room.X + (door.X - door.Room.X), door.Room.Y + (door.Y - door.Room.Y) - 60); // -60 because a grid tile is 60 large.
                            bottomBorder.SetWidth(door.Width);
                            bottomBorder.SetHeight(60);
                            bottomBorder.BorderBottom = true;
                            door.Room.BorderList.Add(bottomBorder);

                            BorderObj topBorder = new BorderObj();
                            topBorder.Position = new Vector2(door.Room.X + (door.X - door.Room.X), door.Room.Y + (door.Y - door.Room.Y) + door.Height);
                            topBorder.SetWidth(door.Width);
                            topBorder.SetHeight(60);
                            topBorder.BorderTop = true;
                            door.Room.BorderList.Add(topBorder);
                            break;
                        case ("Top"):
                        case ("Bottom"):
                            int yOffset = 0;
                            //if (door.DoorPosition == "Bottom")
                                //yOffset = 30;

                            BorderObj rightBorder = new BorderObj();
                            rightBorder.Position = new Vector2(door.Room.X + (door.X - door.Room.X) + door.Width, door.Room.Y + (door.Y - door.Room.Y) + yOffset);
                            rightBorder.SetWidth(60);
                            rightBorder.SetHeight(door.Height);
                            rightBorder.BorderLeft = true;
                            door.Room.BorderList.Add(rightBorder);

                            BorderObj leftBorder = new BorderObj();
                            leftBorder.Position = new Vector2(door.Room.X + (door.X - door.Room.X) - 60, door.Room.Y + (door.Y - door.Room.Y) + yOffset); // +30 because bottom doors have top collide platforms.
                            leftBorder.SetWidth(60);
                            leftBorder.SetHeight(door.Height);
                            leftBorder.BorderRight = true;
                            door.Room.BorderList.Add(leftBorder);
                            break;
                    }
                }
            }
        }

        //1. Search all the rooms, and find the lowest room with an available door. Available doors are all doors that are not the opposite of doorPositionWanted.
        //2. Add a linker room to that door.
        //3. Keep only the door that is connecting the room and the linker room and the doorPositionWanted door open.
        //4. Return the doorPositionWanted door of the linker room.
        //NOTE: furthestRoomDirection is the furthest room you want to find in a specific direction. 
        //      doorPositionWanted is the door position that should be returned when the room is found and linker room is added.
        public static DoorObj FindFurthestDoor(List<RoomObj> roomList, string furthestRoomDirection, string doorPositionWanted, bool addLinkerRoom, bool castleOnly)
        {
            string oppositeOfDoorWanted = GetOppositeDoorPosition(doorPositionWanted);

            RoomObj startingRoom = roomList[0];
            float furthestDistance = -10;
            RoomObj furthestRoom = null;
            DoorObj doorToReturn = null;
            DoorObj doorToLinkTo = null; // Only used if doorToReturn is null at the end.

            foreach (RoomObj room in roomList)
            {
                if (room != startingRoom && ((room.LevelType == GameTypes.LevelType.CASTLE && castleOnly == true) || castleOnly == false))
                {
                    float distance = 0;
                    switch (furthestRoomDirection)
                    {
                        case ("Right"):
                            distance = room.X - startingRoom.X; // Furthest room to the right.
                            break;
                        case ("Left"):
                            distance = startingRoom.X - room.X; // Leftmost room.
                            break;
                        case ("Top"):
                            distance = startingRoom.Y - room.Y; // Highest room.
                            break;
                        case ("Bottom"):
                            distance = room.Y - startingRoom.Y; // Lowest room.
                            break;
                    }

                    if (distance >= furthestDistance) // Will find rooms the same distance away but will not override doorToReturn if one is found.
                    {
                        if (doorToReturn == null || distance > furthestDistance)
                        {
                            doorToReturn = null;
                            foreach (DoorObj door in room.DoorList)
                            {
                                if (door.DoorPosition != "None")
                                {
                                    if (door.DoorPosition == doorPositionWanted)
                                    {
                                        bool addRoom = true;
                                        foreach (RoomObj collidingRoom in roomList)
                                        {
                                            if (collidingRoom != door.Room && CollisionMath.Intersects(new Rectangle((int)collidingRoom.X - 10, (int)collidingRoom.Y - 10, collidingRoom.Width + 20, collidingRoom.Height + 20), door.Bounds))
                                            {
                                                addRoom = false;
                                                break;
                                            }
                                        }

                                        if (addRoom == true)
                                        {
                                            //doorToReturn = door;
                                            doorToLinkTo = door; // Comment this and uncomment above if you don't want to force linker rooms.
                                            furthestDistance = distance;
                                            furthestRoom = room;
                                            break;
                                        }
                                    }
                                    else if (door.DoorPosition != oppositeOfDoorWanted)
                                    {
                                        bool addRoom = true;
                                        foreach (RoomObj collidingRoom in roomList)
                                        {
                                            if (collidingRoom != door.Room && CollisionMath.Intersects(new Rectangle((int)collidingRoom.X - 10, (int)collidingRoom.Y - 10, collidingRoom.Width + 20, collidingRoom.Height + 20), door.Bounds))
                                            {
                                                addRoom = false;
                                                break;
                                            }
                                        }

                                        if (addRoom == true)
                                        {
                                            furthestDistance = distance;
                                            furthestRoom = room;
                                            doorToLinkTo = door;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (doorToLinkTo == null)
            {
                Console.WriteLine("Could not find suitable furthest door. That's a problem");
                return null;
            }

            if (addLinkerRoom == true)
                return AddLinkerToRoom(roomList, doorToLinkTo, doorPositionWanted);
            else
                return doorToLinkTo;
        }

        public static DoorObj AddLinkerToRoom(List<RoomObj> roomList, DoorObj needsLinking, string doorPositionWanted)
        {
            DoorObj doorToReturn = null;
            RoomObj linkerRoom = null;
            switch (needsLinking.Room.LevelType)
            {
                case (GameTypes.LevelType.CASTLE):
                    linkerRoom = m_linkerCastleRoom.Clone() as RoomObj;
                    break;
                case (GameTypes.LevelType.DUNGEON):
                    linkerRoom = m_linkerDungeonRoom.Clone() as RoomObj;
                    break;
                case (GameTypes.LevelType.TOWER):
                    linkerRoom = m_linkerTowerRoom.Clone() as RoomObj;
                    break;
                case (GameTypes.LevelType.GARDEN):
                    linkerRoom = m_linkerGardenRoom.Clone() as RoomObj;
                    break;
            }
            
            linkerRoom.TextureColor = needsLinking.Room.TextureColor;

            DoorObj doorToLinkTo = null;
            string doorToLinkToPosition = GetOppositeDoorPosition(needsLinking.DoorPosition);
            foreach (DoorObj door in linkerRoom.DoorList)
            {
                if (door.DoorPosition == doorToLinkToPosition)
                {
                    doorToLinkTo = door;
                    break;
                }
            }

            // Positioning the newly linked room next to the starting room based on the door's position.
            Vector2 newRoomPosition = Vector2.Zero;
            switch (needsLinking.DoorPosition)
            {
                case ("Left"):
                    newRoomPosition = new Vector2(needsLinking.X - doorToLinkTo.Room.Width, needsLinking.Y - (doorToLinkTo.Y - doorToLinkTo.Room.Y));
                    break;
                case ("Right"):
                    newRoomPosition = new Vector2(needsLinking.X + needsLinking.Width, needsLinking.Y - (doorToLinkTo.Y - doorToLinkTo.Room.Y));
                    break;
                case ("Top"):
                    newRoomPosition = new Vector2(needsLinking.X - (doorToLinkTo.X - doorToLinkTo.Room.X), needsLinking.Y - doorToLinkTo.Room.Height);
                    break;
                case ("Bottom"):
                    newRoomPosition = new Vector2(needsLinking.X - (doorToLinkTo.X - doorToLinkTo.Room.X), needsLinking.Y + needsLinking.Height);
                    break;
            }

            MoveRoom(linkerRoom, newRoomPosition);

            needsLinking.Attached = true;
            doorToLinkTo.Attached = true;

            for (int i = 0; i < linkerRoom.DoorList.Count; i++)
            {
                DoorObj door = linkerRoom.DoorList[i];

                if (door.DoorPosition == doorPositionWanted)
                    doorToReturn = door;
                //else if (door.DoorPosition != doorPositionWanted && door.Attached == false)
                //{
                //    RemoveDoorFromRoom(door);
                //    i--;
                //}
            }

            linkerRoom.LevelType = needsLinking.Room.LevelType;
            roomList.Add(linkerRoom);
            return doorToReturn;
        }

        private static bool hasTopDoor = false;
        private static bool hasBottomDoor = false;
        private static bool hasLeftDoor = false;
        private static bool hasRightDoor = false;

        private static bool hasTopLeftDoor = false;
        private static bool hasTopRightDoor = false;
        private static bool hasBottomLeftDoor = false;
        private static bool hasBottomRightDoor = false;
        private static bool hasRightTopDoor = false;
        private static bool hasRightBottomDoor = false;
        private static bool hasLeftTopDoor = false;
        private static bool hasLeftBottomDoor = false;

        public static void AddRemoveExtraObjects(List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
            {
                hasTopDoor = false;
                hasBottomDoor = false;
                hasLeftDoor = false;
                hasRightDoor = false;

                hasTopLeftDoor = false;
                hasTopRightDoor = false;
                hasBottomLeftDoor = false;
                hasBottomRightDoor = false;
                hasRightTopDoor = false;
                hasRightBottomDoor = false;
                hasLeftTopDoor = false;
                hasLeftBottomDoor = false;

                foreach (DoorObj door in room.DoorList)
                {
                    switch (door.DoorPosition)
                    {
                        case ("Top"):
                            hasTopDoor = true;
                            if (door.X - room.X == 540)
                                hasTopLeftDoor = true;
                            if (room.Bounds.Right - door.X == 780)
                                hasTopRightDoor = true;
                            break;
                        case ("Bottom"):
                            hasBottomDoor = true;
                            if (door.X - room.X == 540)
                                hasBottomLeftDoor = true;
                            if (room.Bounds.Right - door.X == 780)
                                hasBottomRightDoor = true;
                            break;
                        case ("Left"):
                            hasLeftDoor = true;
                            if (door.Y - room.Y == 240)
                                hasLeftTopDoor = true;
                            if (room.Bounds.Bottom - door.Y == 480)
                                hasLeftBottomDoor = true;
                            break;
                        case ("Right"):
                            hasRightDoor = true;
                            if (door.Y - room.Y == 240)
                                hasRightTopDoor = true;
                            if (room.Bounds.Bottom - door.Y == 480)
                                hasRightBottomDoor = true;
                            break;
                    }
                }
                RemoveFromListHelper<TerrainObj>(room.TerrainObjList);
                RemoveFromListHelper<GameObj>(room.GameObjList);
                RemoveFromListHelper<EnemyObj>(room.EnemyList);
                RemoveFromListHelper<BorderObj>(room.BorderList);
            }
        }

        private static void RemoveFromListHelper<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string name = (list[i] as GameObj).Name;
                if (name != null)
                {
                    //if ((hasTopDoor == false && name.IndexOf("Top") != -1 && name.IndexOf("!Top") == -1) || (hasTopDoor == true && name.IndexOf("!Top") != -1))
                    if (((hasTopLeftDoor == false && name.IndexOf("TopLeft") != -1 && name.IndexOf("!TopLeft") == -1) || (hasTopLeftDoor == true && name.IndexOf("!TopLeft") != -1)) ||
                        ((hasTopRightDoor == false && name.IndexOf("TopRight") != -1 && name.IndexOf("!TopRight") == -1) || (hasTopRightDoor == true && name.IndexOf("!TopRight") != -1)) ||
                        ((hasTopDoor == false && name.IndexOf("Top") != -1 && name.IndexOf("!Top") == -1 && name.Length == 3) || (hasTopDoor == true && name.IndexOf("!Top") != -1 && name.Length == 4)))
                    {
                        list.Remove(list[i]);
                        i--;
                    }

                    //if ((hasBottomDoor == false && name.IndexOf("Bottom") != -1 && name.IndexOf("!Bottom") == -1) || (hasBottomDoor == true && name.IndexOf("!Bottom") != -1))
                    if (((hasBottomLeftDoor == false && name.IndexOf("BottomLeft") != -1 && name.IndexOf("!BottomLeft") == -1) || (hasBottomLeftDoor == true && name.IndexOf("!BottomLeft") != -1)) ||
                        ((hasBottomRightDoor == false && name.IndexOf("BottomRight") != -1 && name.IndexOf("!BottomRight") == -1) || (hasBottomRightDoor == true && name.IndexOf("!BottomRight") != -1)) ||
                        ((hasBottomDoor == false && name.IndexOf("Bottom") != -1 && name.IndexOf("!Bottom") == -1 && name.Length == 6) || (hasBottomDoor == true && name.IndexOf("!Bottom") != -1 && name.Length == 7)))
                    {
                        list.Remove(list[i]);
                        i--;
                    }

                    //if ((hasLeftDoor == false && name.IndexOf("Left") != -1 && name.IndexOf("!Left") == -1) || (hasLeftDoor == true && name.IndexOf("!Left") != -1))
                    if (((hasLeftTopDoor == false && name.IndexOf("LeftTop") != -1 && name.IndexOf("!LeftTop") == -1) || (hasLeftTopDoor == true && name.IndexOf("!LeftTop") != -1)) ||
                        ((hasLeftBottomDoor == false && name.IndexOf("LeftBottom") != -1 && name.IndexOf("!LeftBottom") == -1) || (hasLeftBottomDoor == true && name.IndexOf("!LeftBottom") != -1)) ||
                        ((hasLeftDoor == false && name.IndexOf("Left") != -1 && name.IndexOf("!Left") == -1 && name.Length == 4) || (hasLeftDoor == true && name.IndexOf("!Left") != -1 && name.Length == 5)))
                    {
                        list.Remove(list[i]);
                        i--;
                    }

                    //if ((hasRightDoor == false && name.IndexOf("Right") != -1 && name.IndexOf("!Right") == -1) || (hasRightDoor == true && name.IndexOf("!Right") != -1))
                    if (((hasRightTopDoor == false && name.IndexOf("RightTop") != -1 && name.IndexOf("!RightTop") == -1) || (hasRightTopDoor == true && name.IndexOf("!RightTop") != -1)) ||
                        ((hasRightBottomDoor == false && name.IndexOf("RightBottom") != -1 && name.IndexOf("!RightBottom") == -1) || (hasRightBottomDoor == true && name.IndexOf("!RightBottom") != -1)) ||
                        ((hasRightDoor == false && name.IndexOf("Right") != -1 && name.IndexOf("!Right") == -1 && name.Length == 5) || (hasRightDoor == true && name.IndexOf("!Right") != -1 && name.Length == 6)))
                    {
                        list.Remove(list[i]);
                        i--;
                    }
                }
            }
        }


        public static void AddProceduralEnemies(List<RoomObj> roomList)
        {
            Vector2 startingRoomPos = roomList[0].Position;

            foreach (RoomObj room in roomList)
            {
                // Setting the pool of the types of the enemies the room can select from.
                byte[] enemyPool = null;
                byte[] enemyDifficultyPool = null;
                switch (room.LevelType)
                {
                    default:
                    case (GameTypes.LevelType.CASTLE):
                        enemyPool = LevelEV.CASTLE_ENEMY_LIST;
                        enemyDifficultyPool = LevelEV.CASTLE_ENEMY_DIFFICULTY_LIST;
                        break;
                    case (GameTypes.LevelType.GARDEN):
                        enemyPool = LevelEV.GARDEN_ENEMY_LIST;
                        enemyDifficultyPool = LevelEV.GARDEN_ENEMY_DIFFICULTY_LIST;
                        break;
                    case (GameTypes.LevelType.TOWER):
                        enemyPool = LevelEV.TOWER_ENEMY_LIST;
                        enemyDifficultyPool = LevelEV.TOWER_ENEMY_DIFFICULTY_LIST;
                        break;
                    case (GameTypes.LevelType.DUNGEON):
                        enemyPool = LevelEV.DUNGEON_ENEMY_LIST;
                        enemyDifficultyPool = LevelEV.DUNGEON_ENEMY_DIFFICULTY_LIST;
                        break;
                }

                if (enemyPool.Length != enemyDifficultyPool.Length)
                    throw new Exception("Cannot create enemy. Enemy pool != enemy difficulty pool - LevelBuilder2.cs - AddProceduralEnemies()");

                // Selecting the random enemy types for each specific orb colour.
                int randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                int storedEnemyIndex = randomEnemyOrbIndex;

                // Storing red enemy type.
                byte redEnemyType = enemyPool[randomEnemyOrbIndex];
                byte redEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
             
                //Shuffling to get a new enemy type.
                while (randomEnemyOrbIndex == storedEnemyIndex) // Code to prevent two different coloured orbs from having the same enemy type. Disabled for now since the castle only has 1 enemy type.
                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                storedEnemyIndex = randomEnemyOrbIndex;

                // Storing blue enemy type.
                byte blueEnemyType = enemyPool[randomEnemyOrbIndex];
                byte blueEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
                
                // Shuffling to get a new enemy type.
                while (randomEnemyOrbIndex == storedEnemyIndex)
                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                storedEnemyIndex = randomEnemyOrbIndex;

                // Storing green enemy type.
                byte greenEnemyType = enemyPool[randomEnemyOrbIndex];
                byte greenEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];

                // Shuffling to get a new enemy type.
                while (randomEnemyOrbIndex == storedEnemyIndex)
                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                storedEnemyIndex = randomEnemyOrbIndex;

                // Storing white enemy type.
                byte whiteEnemyType = enemyPool[randomEnemyOrbIndex];
                byte whiteEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];

                // Shuffling to get a new enemy type.
                while (randomEnemyOrbIndex == storedEnemyIndex)
                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                storedEnemyIndex = randomEnemyOrbIndex;

                // Storing black enemy type.
                byte blackEnemyType = enemyPool[randomEnemyOrbIndex];
                byte blackEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];

                // Shuffling to get new boss type. No need to prevent two of the same enemy types of appearing.
                randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                // Storing yellow enemy type.
                byte yellowEnemyType = enemyPool[randomEnemyOrbIndex];
                while (yellowEnemyType == EnemyType.BouncySpike) // Prevent expert enemy from being bouncy spike
                {
                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                    yellowEnemyType = enemyPool[randomEnemyOrbIndex];
                }

                for (int i = 0; i < room.GameObjList.Count; i ++)
                {
                    // Creating a random enemy from an orb in the room.
                    EnemyOrbObj enemyOrb = room.GameObjList[i] as EnemyOrbObj;
                    if (enemyOrb != null)
                    {
                        //if (CDGMath.RandomInt(1, 100) <= 100) // Currently 100% chance of spawning the enemy.
                        {
                            EnemyObj newEnemy = null;
                            if (enemyOrb.OrbType == 0) // Red orb.
                                newEnemy = EnemyBuilder.BuildEnemy(redEnemyType, null, null, null, (GameTypes.EnemyDifficulty)redEnemyDifficulty);
                            else if (enemyOrb.OrbType == 1) // Blue orb.
                                newEnemy = EnemyBuilder.BuildEnemy(blueEnemyType, null, null, null, (GameTypes.EnemyDifficulty)blueEnemyDifficulty);
                            else if (enemyOrb.OrbType == 2) // Green orb.
                                newEnemy = EnemyBuilder.BuildEnemy(greenEnemyType, null, null, null, (GameTypes.EnemyDifficulty)greenEnemyDifficulty);
                            else if (enemyOrb.OrbType == 3) // White orb.
                                newEnemy = EnemyBuilder.BuildEnemy(whiteEnemyType, null, null, null, (GameTypes.EnemyDifficulty)whiteEnemyDifficulty);
                            else if (enemyOrb.OrbType == 4) // Black orb.
                                newEnemy = EnemyBuilder.BuildEnemy(blackEnemyType, null, null, null, (GameTypes.EnemyDifficulty)blackEnemyDifficulty);
                            else
                                newEnemy = EnemyBuilder.BuildEnemy(yellowEnemyType, null, null, null, GameTypes.EnemyDifficulty.EXPERT); // In procedurallevelscreen, expert enemies will be given +10 levels.

                            // A check to ensure a forceflying orb selects a flying enemy.
                            while (enemyOrb.ForceFlying == true && newEnemy.IsWeighted == true)
                            {
                                if (newEnemy != null)
                                    newEnemy.Dispose();

                                if (enemyOrb.OrbType == 0) // Red
                                {
                                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                                    redEnemyType = enemyPool[randomEnemyOrbIndex];
                                    redEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
                                    newEnemy = EnemyBuilder.BuildEnemy(redEnemyType, null, null, null, (GameTypes.EnemyDifficulty)redEnemyDifficulty);
                                }
                                else if (enemyOrb.OrbType == 1) // Blue
                                {
                                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                                    blueEnemyType = enemyPool[randomEnemyOrbIndex];
                                    blueEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
                                    newEnemy = EnemyBuilder.BuildEnemy(blueEnemyType, null, null, null, (GameTypes.EnemyDifficulty)blueEnemyDifficulty);
                                }
                                else if (enemyOrb.OrbType == 2) // Green
                                {
                                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                                    greenEnemyType = enemyPool[randomEnemyOrbIndex];
                                    greenEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
                                    newEnemy = EnemyBuilder.BuildEnemy(greenEnemyType, null, null, null, (GameTypes.EnemyDifficulty)greenEnemyDifficulty);
                                }
                                else if (enemyOrb.OrbType == 3) // White
                                {
                                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                                    whiteEnemyType = enemyPool[randomEnemyOrbIndex];
                                    whiteEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
                                    newEnemy = EnemyBuilder.BuildEnemy(whiteEnemyType, null, null, null, (GameTypes.EnemyDifficulty)whiteEnemyDifficulty);
                                }
                                else if (enemyOrb.OrbType == 4) // Black
                                {
                                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                                    blackEnemyType = enemyPool[randomEnemyOrbIndex];
                                    blackEnemyDifficulty = enemyDifficultyPool[randomEnemyOrbIndex];
                                    newEnemy = EnemyBuilder.BuildEnemy(blackEnemyType, null, null, null, (GameTypes.EnemyDifficulty)blackEnemyDifficulty);
                                }
                                else // Yellow
                                {
                                    randomEnemyOrbIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                                    yellowEnemyType = enemyPool[randomEnemyOrbIndex];
                                    newEnemy = EnemyBuilder.BuildEnemy(yellowEnemyType, null, null, null, GameTypes.EnemyDifficulty.EXPERT);
                                }
                            }

                            newEnemy.Position = enemyOrb.Position;
                            newEnemy.IsProcedural = true;
                            room.EnemyList.Add(newEnemy);
                        }

                        // Remove the orb from the list.
                        room.GameObjList.Remove(enemyOrb);
                        enemyOrb.Dispose();
                        i--;
                        continue;
                    }

                    // Creating a random enemy for the room.
                    EnemyTagObj tag = room.GameObjList[i] as EnemyTagObj;
                    if (tag != null)
                    {
                        //if (CDGMath.RandomInt(1, 100) <= 100) // Currently 100% chance of spawning the enemy.
                        {
                            int randomEnemyIndex = CDGMath.RandomInt(0, enemyPool.Length - 1);
                            EnemyObj newEnemy = EnemyBuilder.BuildEnemy(enemyPool[randomEnemyIndex], null, null, null, GameTypes.EnemyDifficulty.BASIC);
                            newEnemy.Position = tag.Position;
                            newEnemy.IsProcedural = true;
                            room.EnemyList.Add(newEnemy);

                            // Changing the enemy's level based on its distance from the start of the level.
                            //float enemyDistanceFromStart = Math.Abs(CDGMath.DistanceBetweenPts(startingRoomPos, newEnemy.Position));
                            //newEnemy.Level = (int)Math.Ceiling(enemyDistanceFromStart / 1500);
                            //Console.WriteLine(newEnemy.Level);
                        }

                        // Remove the extra tag from the list.
                        room.GameObjList.Remove(tag);
                        tag.Dispose();
                        i--;
                        continue;
                    }
                }
            }
        }

        public static void OverrideProceduralEnemies(ProceduralLevelScreen level, byte[] enemyTypeData, byte[] enemyDifficultyData)
        {
            Console.WriteLine("////////////////// OVERRIDING CREATED ENEMIES. LOADING PRE-CONSTRUCTED ENEMY LIST ////////");

            //foreach (RoomObj room in level.RoomList)
            //{
            //    int count = 0;
            //    foreach (EnemyObj enemy in room.EnemyList)
            //    {
            //        if (enemy.IsProcedural == true)
            //            count++;
            //    }
            //    Console.WriteLine(count);
            //}

            int indexCounter = 0;
            // Store the enemies in a separate array.
            foreach (RoomObj room in level.RoomList)
            {
                //if (room.Name != "Bonus" && room.Name != "Boss") // This shouldn't be necessary since bonus room enemies and boss enemies are not (or should not be) saved.
                //if (room.Name != "Boss" && room.Name != "Bonus" && room.Name != "Ending" && room.Name != "Tutorial" && room.Name != "Compass")
                {
                    for (int i = 0; i < room.EnemyList.Count; i++)
                    {
                        EnemyObj enemyToOverride = room.EnemyList[i];
                        if (enemyToOverride.IsProcedural == true) // Only replace procedural enemies.
                        {
                            EnemyObj newEnemy = EnemyBuilder.BuildEnemy(enemyTypeData[indexCounter], level.Player, null, level, GameTypes.EnemyDifficulty.BASIC, true);
                            newEnemy.IsProcedural = true;
                            newEnemy.Position = enemyToOverride.Position;

                            newEnemy.Level = enemyToOverride.Level;
                            newEnemy.SetDifficulty((GameTypes.EnemyDifficulty)enemyDifficultyData[indexCounter], false);
                            //newEnemy.SetDifficulty(enemyToOverride.Difficulty, false);

                            room.EnemyList[i].Dispose();
                            room.EnemyList[i] = newEnemy;
                            indexCounter++;
                        }
                    }
                }
            }

            Console.WriteLine("//////////////// PRE-CONSTRUCTED ENEMY LIST LOADED ////////////////");
        }

        public static void AddBottomPlatforms(List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
            {
                foreach (DoorObj door in room.DoorList)
                {
                    if (door.DoorPosition == "Bottom")
                    {
                        TerrainObj bottomPlatform = new TerrainObj(door.Width, door.Height);
                        bottomPlatform.AddCollisionBox(0, 0, bottomPlatform.Width, bottomPlatform.Height, Consts.TERRAIN_HITBOX);
                        bottomPlatform.AddCollisionBox(0, 0, bottomPlatform.Width, bottomPlatform.Height, Consts.BODY_HITBOX);
                        bottomPlatform.Position = door.Position;
                        bottomPlatform.CollidesBottom = false;
                        bottomPlatform.CollidesLeft = false;
                        bottomPlatform.CollidesRight = false;
                        bottomPlatform.SetHeight(30); // Each grid in the game is 60 pixels. 60/2 = 30.
                        room.TerrainObjList.Add(bottomPlatform);

                        BorderObj border = new BorderObj();
                        border.Position = bottomPlatform.Position;
                        border.SetWidth(bottomPlatform.Width);
                        border.SetHeight(bottomPlatform.Height);
                        //border.BorderBottom = true;
                        border.BorderTop = true;
                        room.BorderList.Add(border);
                    }
                }
            }
        }

        public static void AddCompassRoom(List<RoomObj> roomList)
        {
            CompassRoomObj compassRoom = m_compassRoom.Clone() as CompassRoomObj;
            MoveRoom(compassRoom, new Vector2(-999999, -999999));
            roomList.Add(compassRoom);
        }

        public static ProceduralLevelScreen CreateEndingRoom()
        {
            ProceduralLevelScreen endingScreen = new ProceduralLevelScreen();
            RoomObj endingRoom = m_endingRoom.Clone() as RoomObj;
            MoveRoom(endingRoom, Vector2.Zero);
            endingScreen.AddRoom(endingRoom);
            AddDoorBorders(endingScreen.RoomList);
            AddBottomPlatforms(endingScreen.RoomList);
            AddRemoveExtraObjects(endingScreen.RoomList);
            AddProceduralEnemies(endingScreen.RoomList);
            LinkAllBossEntrances(endingScreen.RoomList);
            ConvertBonusRooms(endingScreen.RoomList);
            ConvertBossRooms(endingScreen.RoomList);
            ConvertChallengeBossRooms(endingScreen.RoomList);            
            InitializeRooms(endingScreen.RoomList);

            return endingScreen;
        }

        public static ProceduralLevelScreen CreateStartingRoom()
        {
            ProceduralLevelScreen startingRoomScreen = new ProceduralLevelScreen();
            RoomObj startingRoom = m_startingRoom.Clone() as RoomObj;
            MoveRoom(startingRoom, Vector2.Zero);
            startingRoomScreen.AddRoom(startingRoom);
            AddDoorBorders(startingRoomScreen.RoomList);
            AddBottomPlatforms(startingRoomScreen.RoomList);
            AddRemoveExtraObjects(startingRoomScreen.RoomList);
            AddProceduralEnemies(startingRoomScreen.RoomList);
            LinkAllBossEntrances(startingRoomScreen.RoomList);
            ConvertBonusRooms(startingRoomScreen.RoomList);
            ConvertBossRooms(startingRoomScreen.RoomList);
            ConvertChallengeBossRooms(startingRoomScreen.RoomList);
            InitializeRooms(startingRoomScreen.RoomList);

            return startingRoomScreen;
        }

        public static ProceduralLevelScreen CreateTutorialRoom()
        {
            ProceduralLevelScreen tutorialRoomScreen = new ProceduralLevelScreen();

            // Adding intro cutscene room.
            IntroRoomObj introRoom = new IntroRoomObj();
            introRoom.CopyRoomProperties(m_startingRoom);
            introRoom.CopyRoomObjects(m_startingRoom);
            MoveRoom(introRoom, Vector2.Zero);
            tutorialRoomScreen.AddRoom(introRoom);
            Game.ScreenManager.Player.Position = new Vector2(150, 150);

            // Adding tutorial room.
            TutorialRoomObj tutorialRoom = m_tutorialRoom.Clone() as TutorialRoomObj;
            MoveRoom(tutorialRoom, new Vector2(introRoom.Width, -tutorialRoom.Height + introRoom.Height));
            tutorialRoomScreen.AddRoom(tutorialRoom);

            // Adding throne room.
            ThroneRoomObj throneRoom = m_throneRoom.Clone() as ThroneRoomObj;
            MoveRoom(throneRoom, new Vector2(-10000,-10000));
            tutorialRoomScreen.AddRoom(throneRoom);

            // Linking tutorial room to throne room.
            tutorialRoom.LinkedRoom = throneRoom;

            // Initializing rooms.
            AddDoorBorders(tutorialRoomScreen.RoomList);
            AddBottomPlatforms(tutorialRoomScreen.RoomList);
            AddRemoveExtraObjects(tutorialRoomScreen.RoomList);
            AddProceduralEnemies(tutorialRoomScreen.RoomList);
            LinkAllBossEntrances(tutorialRoomScreen.RoomList);
            ConvertBonusRooms(tutorialRoomScreen.RoomList);
            ConvertBossRooms(tutorialRoomScreen.RoomList);
            ConvertChallengeBossRooms(tutorialRoomScreen.RoomList);
            InitializeRooms(tutorialRoomScreen.RoomList);

            //Game.ScreenManager.Player.Position = new Vector2(2800, 150);
            //Game.ScreenManager.Player.Position = new Vector2(2800 + 1500, tutorialRoom.Y + 150);

            return tutorialRoomScreen;
        }

        // Creates a level based purely on knowing the room indexes used, and the type of the room. Used for loading maps.
        public static ProceduralLevelScreen CreateLevel(Vector4[] roomInfoList, Vector3[] roomColorList)
        {
            Console.WriteLine("///////////// LOADING PRE-CONSTRUCTED LEVEL //////");
            List<RoomObj> sequencedRoomList = SequencedRoomList;
            List<RoomObj> dlcCastleRoomList = GetSequencedDLCRoomList(GameTypes.LevelType.CASTLE);
            List<RoomObj> dlcGardenRoomList = GetSequencedDLCRoomList(GameTypes.LevelType.GARDEN);
            List<RoomObj> dlcTowerRoomList = GetSequencedDLCRoomList(GameTypes.LevelType.TOWER);
            List<RoomObj> dlcDungeonRoomList = GetSequencedDLCRoomList(GameTypes.LevelType.DUNGEON);

            ProceduralLevelScreen createdLevel = new ProceduralLevelScreen();
            List<RoomObj> roomList = new List<RoomObj>();

            int counter = 0;
            foreach (Vector4 roomData in roomInfoList)
            {
                //RoomObj room = sequencedRoomList[(int)roomData.W].Clone() as RoomObj;

                // New logic to load levels from the DLC lists.
                RoomObj room = null;
                int roomIndex = (int)roomData.W;
                if (roomIndex < 10000) // Take a room from the original room list.
                    room = sequencedRoomList[roomIndex].Clone() as RoomObj;
                else if (roomIndex >= 10000 && roomIndex < 20000) // Take a room from the Castle DLC list.
                    room = dlcCastleRoomList[roomIndex - 10000].Clone() as RoomObj;
                else if (roomIndex >= 20000 && roomIndex < 30000) // Take a room from the Garden DLC list.
                    room = dlcGardenRoomList[roomIndex - 20000].Clone() as RoomObj;
                else if (roomIndex >= 30000 && roomIndex < 40000) // Take a room from the Tower DLC list.
                    room = dlcTowerRoomList[roomIndex - 30000].Clone() as RoomObj;
                else // Take a room from the Dungeon DLC list.
                    room = dlcDungeonRoomList[roomIndex - 40000].Clone() as RoomObj;

                room.LevelType = (GameTypes.LevelType)roomData.X;
                MoveRoom(room, new Vector2(roomData.Y, roomData.Z));
                roomList.Add(room);

                room.TextureColor = new Color((byte)roomColorList[counter].X, (byte)roomColorList[counter].Y, (byte)roomColorList[counter].Z);
                counter++;
            }
            createdLevel.AddRooms(roomList);

            // Linking all boss rooms.
            CloseRemainingDoors(createdLevel.RoomList);
            AddDoorBorders(createdLevel.RoomList); // Must be called after all doors are closed. Adds the borders that are created by doors existing/not existing.
            AddBottomPlatforms(createdLevel.RoomList); // Must be called after all doors are closed.
            AddRemoveExtraObjects(createdLevel.RoomList); // Adds all the Top/!Top objects. Must be called after all doors are closed, and before enemies are added.
            AddProceduralEnemies(createdLevel.RoomList);
            LinkAllBossEntrances(createdLevel.RoomList);
            ConvertBonusRooms(createdLevel.RoomList);
            ConvertBossRooms(createdLevel.RoomList);
            ConvertChallengeBossRooms(createdLevel.RoomList);
            AddCompassRoom(createdLevel.RoomList); // The last room always has to be the compass room.
            InitializeRooms(createdLevel.RoomList); // Initializes any special things for the created rooms, like special door entrances, bosses, etc.

            Console.WriteLine("///////////// PRE-CONSTRUCTED LEVEL LOADED //////");

            return createdLevel;
        }

        public static ProceduralLevelScreen CreateLevel(RoomObj startingRoom = null, params AreaStruct[] areaStructs)
        {
            if (m_testRoom != null && LevelEV.RUN_TESTROOM == true)
            {
                Console.WriteLine("OVERRIDING ROOM CREATION. RUNNING TEST ROOM");
                ProceduralLevelScreen debugLevel = new ProceduralLevelScreen();
                RoomObj debugRoom = m_testRoom.Clone() as RoomObj;
                if (LevelEV.TESTROOM_REVERSE == true)
                    debugRoom.Reverse();
                MoveRoom(debugRoom, Vector2.Zero);
                debugLevel.AddRoom(debugRoom);
                if (LevelEV.CLOSE_TESTROOM_DOORS == true)
                    CloseRemainingDoors(debugLevel.RoomList);
                AddDoorBorders(debugLevel.RoomList);
                AddBottomPlatforms(debugLevel.RoomList);
                AddRemoveExtraObjects(debugLevel.RoomList);
                AddProceduralEnemies(debugLevel.RoomList);
                LinkAllBossEntrances(debugLevel.RoomList);

                ConvertBonusRooms(debugLevel.RoomList);
                ConvertBossRooms(debugLevel.RoomList);
                ConvertChallengeBossRooms(debugLevel.RoomList);
                InitializeRooms(debugLevel.RoomList);

                debugLevel.RoomList[0].LevelType = LevelEV.TESTROOM_LEVELTYPE; // A hack for special rooms, since they're set to none level type.

                return debugLevel;
            }

            ProceduralLevelScreen createdLevel = new ProceduralLevelScreen(); // Create a blank level.
            List<RoomObj> masterRoomList = new List<RoomObj>();

            List<AreaStruct> sequentialStructs= new List<AreaStruct>();
            List<AreaStruct> nonSequentialStructs= new List<AreaStruct>();

            // Separating the level structs into sequentially attached and non-sequentially attached ones.
            foreach (AreaStruct areaStruct in areaStructs)
            {
                if (areaStruct.LevelType == GameTypes.LevelType.CASTLE || areaStruct.LevelType == GameTypes.LevelType.GARDEN) //TEDDY COMMENT THIS OUT TO DISABLE OUT GARDEN CONNECT RIGHT
                    sequentialStructs.Add(areaStruct);
                else
                    nonSequentialStructs.Add(areaStruct);
            }

            int numSequentialAreas = sequentialStructs.Count;
            int numNonSequentialAreas = nonSequentialStructs.Count;
            List<RoomObj>[] sequentialAreas = new List<RoomObj>[numSequentialAreas];
            List<RoomObj>[] nonSequentialAreas = new List<RoomObj>[numNonSequentialAreas];

            ///////////// ROOM CREATION STARTS///////////////
            restartAll:
            masterRoomList.Clear();
            // Build all the sequentially attached rooms first.
            for (int i = 0; i < sequentialStructs.Count; i++)
            {
                int creationCounter = 0;
            restartSequential:
                sequentialAreas[i] = null;
                AreaStruct areaInfo = sequentialStructs[i]; 
                int numRooms = CDGMath.RandomInt((int)areaInfo.TotalRooms.X, (int)areaInfo.TotalRooms.Y);

                // Keep recreating the area until the requested number of rooms are built.
                DoorObj linkerDoorAdded = null;
                bool addedBossRoom = true;
                while (sequentialAreas[i] == null || sequentialAreas[i].Count < numRooms || addedBossRoom == false)
                {
                    addedBossRoom = true;
                    if (areaInfo.BossInArea == true)
                        addedBossRoom = false;

                    if (i == 0)
                    {
                        if (startingRoom == null)
                            sequentialAreas[i] = LevelBuilder2.CreateArea(numRooms, areaInfo, masterRoomList, StartingRoom.Clone() as StartingRoomObj, true);
                        else
                            sequentialAreas[i] = LevelBuilder2.CreateArea(numRooms, areaInfo, masterRoomList, startingRoom.Clone() as StartingRoomObj, true);
                    }
                    else
                    {
                        List<RoomObj> masterListCopy = new List<RoomObj>(); // Only use a copy in case rooms couldn't be found and this keeps getting added.
                        masterListCopy.AddRange(masterRoomList);

                        linkerDoorAdded = FindFurthestDoor(masterListCopy, "Right", "Right", true, LevelEV.LINK_TO_CASTLE_ONLY);
                        if (linkerDoorAdded == null)
                            goto restartAll; // This isn't supposed to be possible.
                        sequentialAreas[i] = LevelBuilder2.CreateArea(numRooms, areaInfo, masterListCopy, linkerDoorAdded.Room, false);
                    }

                    // Checking to make sure the area has a boss entrance in it.
                    foreach (RoomObj room in sequentialAreas[i])
                    {
                        if (room.Name == "EntranceBoss")
                        {
                            addedBossRoom = true;
                            break;
                        }
                    }

                    if (addedBossRoom == false)
                        Console.WriteLine("Could not find suitable boss room for area. Recreating sequential area.");
                    else
                        Console.WriteLine("Created sequential area of size: " + sequentialAreas[i].Count);

                    // A safety check. If the sequential rooms cannot be added after 15 attempts, recreate the entire map.
                    creationCounter++;
                    if (creationCounter > 15)
                    {
                        Console.WriteLine("Could not create non-sequential area after 15 attempts. Recreating entire level.");
                        goto restartAll;
                    }
                }
                if (i != 0)
                    masterRoomList.Add(linkerDoorAdded.Room);

                // Making sure each area has a top, bottom, and rightmost exit door.  If not, recreate sequential area.
                if (LevelBuilder2.FindFurthestDoor(sequentialAreas[i], "Right", "Right", false, false) == null
                    || LevelBuilder2.FindFurthestDoor(sequentialAreas[i], "Top", "Top", false, false) == null
                    || LevelBuilder2.FindFurthestDoor(sequentialAreas[i], "Bottom", "Bottom", false, false) == null)
                {
                    bool removedLinkedRoom = false;
                    if (i != 0)
                        removedLinkedRoom = masterRoomList.Remove(linkerDoorAdded.Room);
                    else
                        removedLinkedRoom = true;
                    Console.WriteLine("Attempting re-creation of sequential area. Linker Room removed: " + removedLinkedRoom);
                    goto restartSequential;  // Didn't want to use gotos, but only way I could think of restarting loop.  Goes back to the start of this forloop.
                }
                else // A sequential area with all exits has been created. Add it to the masterRoomList;
                    masterRoomList.AddRange(sequentialAreas[i]);
            }

            Console.WriteLine("////////// ALL SEQUENTIAL AREAS SUCCESSFULLY ADDED");

            // Now create all non-sequential areas.
            for (int i = 0; i < nonSequentialStructs.Count; i++)
            {
                int creationCounter = 0;
                restartNonSequential:
                nonSequentialAreas[i] = null;
                AreaStruct areaInfo = nonSequentialStructs[i];
                int numRooms = CDGMath.RandomInt((int)areaInfo.TotalRooms.X, (int)areaInfo.TotalRooms.Y);
                string furthestDoorDirection = "";
                switch (areaInfo.LevelType)
                {
                    case (GameTypes.LevelType.TOWER):
                        furthestDoorDirection = "Top";
                        break;
                    case (GameTypes.LevelType.DUNGEON):
                        furthestDoorDirection = "Bottom";//"Bottom";
                        break;
                    case (GameTypes.LevelType.GARDEN):
                        furthestDoorDirection = "Right"; //TEDDY - COMMENTED OUT DUNGEON CONNECT TOP, AND ADDED GARDEN CONNECT TOP
                        break;
                    default:
                        throw new Exception("Could not create non-sequential area of type " + areaInfo.LevelType);
                }

                //int creationCounter = 0;
                DoorObj linkerDoorAdded = null;
                bool addedBossRoom = true;
                // Keep recreating the area until the requested number of rooms are built.
                while (nonSequentialAreas[i] == null || nonSequentialAreas[i].Count < numRooms || addedBossRoom == false)
                {
                     addedBossRoom = true;
                    if (areaInfo.BossInArea == true)
                        addedBossRoom = false;

                    List<RoomObj> masterListCopy = new List<RoomObj>(); // Only use a copy in case rooms couldn't be found and this keeps getting added.
                    masterListCopy.AddRange(masterRoomList);

                    // Finds the furthest door of the previous area, and adds a linker room to it (on a separate list so as not to modify the original room list).
                    // Creates a new area from the linker room.
                    linkerDoorAdded = FindFurthestDoor(masterListCopy, furthestDoorDirection, furthestDoorDirection, true, LevelEV.LINK_TO_CASTLE_ONLY);
                    if (linkerDoorAdded == null)
                        goto restartAll; // This isn't supposed to be possible.
                    nonSequentialAreas[i] = LevelBuilder2.CreateArea(numRooms, areaInfo, masterListCopy, linkerDoorAdded.Room, false);

                    // A safety check. If the non-sequential rooms cannot be added after 15 attempts, recreate the entire map.
                    creationCounter++;
                    if (creationCounter > 15)
                    {
                        Console.WriteLine("Could not create non-sequential area after 15 attempts. Recreating entire level.");
                        goto restartAll;
                    }

                          // Checking to make sure the area has a boss entrance in it.
                    foreach (RoomObj room in nonSequentialAreas[i])
                    {
                        if (room.Name == "EntranceBoss")
                        {
                            addedBossRoom = true;
                            break;
                        }
                    }

                    if (addedBossRoom == false)
                        Console.WriteLine("Could not find suitable boss room for area. Recreating non-sequential area.");
                    else
                        Console.WriteLine("Created non-sequential area of size: " + nonSequentialAreas[i].Count);
                }
                masterRoomList.Add(linkerDoorAdded.Room);

                // Making sure each area has a top, bottom, and rightmost exit door.  If not, recreate all sequential areas.
                if ((areaInfo.LevelType == GameTypes.LevelType.TOWER && (LevelBuilder2.FindFurthestDoor(nonSequentialAreas[i], "Right", "Right", false, false) == null || 
                    LevelBuilder2.FindFurthestDoor(nonSequentialAreas[i], "Top", "Top", false, false) == null)) ||
                    (areaInfo.LevelType == GameTypes.LevelType.DUNGEON && (LevelBuilder2.FindFurthestDoor(nonSequentialAreas[i], "Right", "Right", false, false) == null || 
                    LevelBuilder2.FindFurthestDoor(nonSequentialAreas[i], "Bottom", "Bottom", false, false) == null)))
                {
                    bool removedLinkedRoom = false;
                    removedLinkedRoom = masterRoomList.Remove(linkerDoorAdded.Room);
                    Console.WriteLine("Attempting re-creation of a non-sequential area. Linker Room removed: " + removedLinkedRoom);
                    goto restartNonSequential;
                }
                else // A non-sequential area with all exits has been created. Add it to the masterRoomList;
                    masterRoomList.AddRange(nonSequentialAreas[i]);
            }

            Console.WriteLine("////////// ALL NON-SEQUENTIAL AREAS SUCCESSFULLY ADDED");

            createdLevel.AddRooms(masterRoomList); // Add the rooms to the level.
            CloseRemainingDoors(createdLevel.RoomList);
            AddDoorBorders(createdLevel.RoomList); // Must be called after all doors are closed. Adds the borders that are created by doors existing/not existing.
            AddBottomPlatforms(createdLevel.RoomList); // Must be called after all doors are closed.
            AddRemoveExtraObjects(createdLevel.RoomList); // Adds all the Top/!Top objects. Must be called after all doors are closed, and before enemies are added.
            AddProceduralEnemies(createdLevel.RoomList);
            LinkAllBossEntrances(createdLevel.RoomList); // Links all boss entrances to actual boss rooms.
            ConvertBonusRooms(createdLevel.RoomList);
            ConvertBossRooms(createdLevel.RoomList);
            ConvertChallengeBossRooms(createdLevel.RoomList);
            AddCompassRoom(createdLevel.RoomList); // The last room always has to be the compass room.
            InitializeRooms(createdLevel.RoomList); // Initializes any special things for the created rooms, like special door entrances, bosses, etc.

            Console.WriteLine("////////// LEVEL CREATION SUCCESSFUL");

            return createdLevel;
        }

        private static void ConvertBonusRooms(List<RoomObj> roomList)
        {
            // flibit didn't like this
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomObj room = roomList[i];
                if (room.Name == "Bonus")
                {
                    if (room.Tag == "")
                        room.Tag = "0";

                    RoomObj roomToAdd = null;

                    switch (int.Parse(room.Tag, NumberStyles.Any, ci))
                    {
                        case (BonusRoomTypes.PickChest):
                            roomToAdd = new ChestBonusRoomObj();
                            break;
                        case (BonusRoomTypes.SpecialItem):
                            roomToAdd = new SpecialItemRoomObj();
                            break;
                        case (BonusRoomTypes.RandomTeleport):
                            roomToAdd = new RandomTeleportRoomObj();
                            break;
                        case (BonusRoomTypes.SpellSwap):
                            roomToAdd = new SpellSwapRoomObj();
                            break;
                        case (BonusRoomTypes.VitaChamber):
                            roomToAdd = new VitaChamberRoomObj();
                            break;
                        case (BonusRoomTypes.Diary):
                            roomToAdd = new DiaryRoomObj();
                            break;
                        case (BonusRoomTypes.Portrait):
                            roomToAdd = new PortraitRoomObj();
                            break;
                        case (BonusRoomTypes.CarnivalShoot1):
                            roomToAdd = new CarnivalShoot1BonusRoom();
                            break;
                        case (BonusRoomTypes.CarnivalShoot2):
                            roomToAdd = new CarnivalShoot2BonusRoom();
                            break;
                        case(BonusRoomTypes.Arena):
                            roomToAdd = new ArenaBonusRoom();
                            break;
                        case(BonusRoomTypes.Jukebox):
                            roomToAdd = new JukeboxBonusRoom();
                            break;
                    }

                    if (roomToAdd != null)
                    {
                        roomToAdd.CopyRoomProperties(room);
                        roomToAdd.CopyRoomObjects(room);
                        roomList.Insert(roomList.IndexOf(room), roomToAdd);
                        roomList.Remove(room);
                    }
                }
            }
        }

        private static void ConvertBossRooms(List<RoomObj> roomList)
        {
            // flibit didn't like this
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomObj room = roomList[i];
                if (room.Name == "Boss")
                {
                    if (room.Tag == "")
                        room.Tag = "0";

                    RoomObj bossRoom = null;
                    int bossRoomType = int.Parse(room.Tag, NumberStyles.Any, ci);

                    switch (bossRoomType)
                    {
                        case (BossRoomType.EyeballBossRoom):
                            bossRoom = new EyeballBossRoom();
                            break;
                        case (BossRoomType.LastBossRoom):
                            bossRoom = new LastBossRoom();
                            break;
                        case (BossRoomType.BlobBossRoom):
                            bossRoom = new BlobBossRoom();
                            break;
                        case (BossRoomType.FairyBossRoom):
                            bossRoom = new FairyBossRoom();
                            break;
                        case (BossRoomType.FireballBossRoom):
                            bossRoom = new FireballBossRoom();
                            break;
                    }

                    if (bossRoom != null)
                    {
                        bossRoom.CopyRoomProperties(room);
                        bossRoom.CopyRoomObjects(room);
                        if (bossRoom.LinkedRoom != null) // Adding this so you can test the room without linking it.
                        {
                            bossRoom.LinkedRoom = room.LinkedRoom;
                            bossRoom.LinkedRoom.LinkedRoom = bossRoom; // A roundabout way of relinking the boss entrance room to the newly made eyeball boss.
                        }
                        roomList.Insert(roomList.IndexOf(room), bossRoom);
                        roomList.Remove(room);
                    }
                }
            }
        }

        private static void ConvertChallengeBossRooms(List<RoomObj> roomList)
        {
            // flibit didn't like this
            // CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            // ci.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo ci = CultureInfo.InvariantCulture;

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomObj room = roomList[i];
                if (room.Name == "ChallengeBoss")
                {
                    if (room.Tag == "")
                        room.Tag = "0";

                    RoomObj challengeRoom = null;
                    int challengeRoomType = int.Parse(room.Tag, NumberStyles.Any, ci);

                    switch (challengeRoomType)
                    {
                        case (BossRoomType.EyeballBossRoom):
                            challengeRoom = new EyeballChallengeRoom();
                            MoveRoom(challengeRoom, new Vector2(0, 5000000));
                            break;
                        case (BossRoomType.FairyBossRoom):
                            challengeRoom = new FairyChallengeRoom();
                            MoveRoom(challengeRoom, new Vector2(0, -6000000));
                            break;
                        case (BossRoomType.FireballBossRoom):
                            challengeRoom = new FireballChallengeRoom();
                            MoveRoom(challengeRoom, new Vector2(0, -7000000));
                            break;
                        case (BossRoomType.BlobBossRoom):
                            challengeRoom = new BlobChallengeRoom();
                            MoveRoom(challengeRoom, new Vector2(0, -8000000));
                            break;
                        case (BossRoomType.LastBossRoom):
                            challengeRoom = new LastBossChallengeRoom();
                            MoveRoom(challengeRoom, new Vector2(0, -9000000));
                            break;
                        default:
                            challengeRoom = new EyeballChallengeRoom();
                            MoveRoom(challengeRoom, new Vector2(0, -5000000));
                            break;
                    }

                    if (challengeRoom != null)
                    {
                        Vector2 storedPos = challengeRoom.Position;
                        challengeRoom.CopyRoomProperties(room);
                        challengeRoom.CopyRoomObjects(room);
                        MoveRoom(challengeRoom, storedPos);
                        if (challengeRoom.LinkedRoom != null) // Adding this so you can test the room without linking it.
                        {
                            challengeRoom.LinkedRoom = room.LinkedRoom;
                        }
                        roomList.Insert(roomList.IndexOf(room), challengeRoom);
                        roomList.Remove(room);
                    }
                }
            }
        }


        private static void InitializeRooms(List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
                room.Initialize();
        }

        private static string GetOppositeDoorPosition(string doorPosition)
        {
            string doorPosToReturn = "";
            switch (doorPosition)
            {
                case ("Left"):
                    doorPosToReturn = "Right";
                    break;
                case ("Right"):
                    doorPosToReturn = "Left";
                    break;
                case ("Top"):
                    doorPosToReturn = "Bottom";
                    break;
                case ("Bottom"):
                    doorPosToReturn = "Top";
                    break;
            }
            return doorPosToReturn;
        }

        private static bool CheckForRoomCollision(DoorObj doorToCheck, List<RoomObj> roomList, DoorObj otherDoorToCheck)
        {
            //Code to make sure the room does not collide with other rooms in the list.
            Vector2 newRoomPosition = Vector2.Zero;
            switch (doorToCheck.DoorPosition)
            {
                case ("Left"):
                    newRoomPosition = new Vector2(doorToCheck.Room.X - otherDoorToCheck.Room.Width, doorToCheck.Y - (otherDoorToCheck.Y - otherDoorToCheck.Room.Y));
                    break;
                case ("Right"):
                    newRoomPosition = new Vector2(doorToCheck.X + doorToCheck.Width, doorToCheck.Y - (otherDoorToCheck.Y - otherDoorToCheck.Room.Y));
                    break;
                case ("Top"):
                    newRoomPosition = new Vector2(doorToCheck.X - (otherDoorToCheck.X - otherDoorToCheck.Room.X), doorToCheck.Y - otherDoorToCheck.Room.Height);
                    break;
                case ("Bottom"):
                    newRoomPosition = new Vector2(doorToCheck.X - (otherDoorToCheck.X - otherDoorToCheck.Room.X), doorToCheck.Y + doorToCheck.Height);
                    break;
            }

            foreach (RoomObj roomObj in roomList)
            {
                if (CollisionMath.Intersects(new Rectangle((int)(roomObj.X), (int)(roomObj.Y), roomObj.Width, roomObj.Height), new Rectangle((int)(newRoomPosition.X), (int)(newRoomPosition.Y), otherDoorToCheck.Room.Width, otherDoorToCheck.Room.Height))
                     || newRoomPosition.X < 0) // Do not allow rooms be made past xPos = 0.
                    return true;
            }

            return false;
        }


        public static void MoveRoom(RoomObj room, Vector2 newPosition)
        {
            Vector2 positionShift = room.Position - newPosition; // The amount everything in the room needs to shift by in order to put them in their new position next to the room they are linking to.
            //Shifting the room to link to and its contents next to the room that needs linking.
            room.Position = newPosition;
            foreach (TerrainObj obj in room.TerrainObjList)
                obj.Position -= positionShift;
            foreach (GameObj obj in room.GameObjList)
                obj.Position -= positionShift;
            foreach (DoorObj door in room.DoorList)
                door.Position -= positionShift;
            foreach (EnemyObj enemy in room.EnemyList)
                enemy.Position -= positionShift;
            foreach (BorderObj border in room.BorderList)
                border.Position -= positionShift;
        }

        public static void LinkAllBossEntrances(List<RoomObj> roomList)
        {
            Vector2 newRoomPosition = new Vector2(-100000, 0); // This is where all the boss rooms will float in. It must be left of the level so that it doesn't accidentally run into any of the level's rooms.
            int maxRoomIndex = m_bossRoomArray.Count - 1;

            RoomObj bossRoom = null;
            List<RoomObj> bossRoomsToAdd = new List<RoomObj>();

            List<RoomObj> challengeRoomsToAdd = new List<RoomObj>();
            RoomObj challengeRoom = null;

            foreach (RoomObj room in roomList)
            {
                byte bossRoomType = 0;
                switch(room.LevelType)
                {
                    case (GameTypes.LevelType.CASTLE):
                        bossRoomType = LevelEV.CASTLE_BOSS_ROOM;
                        break;
                    case(GameTypes.LevelType.TOWER):
                        bossRoomType = LevelEV.TOWER_BOSS_ROOM;
                        break;
                    case(GameTypes.LevelType.DUNGEON):
                        bossRoomType = LevelEV.DUNGEON_BOSS_ROOM;
                        break;
                    case(GameTypes.LevelType.GARDEN):
                        bossRoomType = LevelEV.GARDEN_BOSS_ROOM;
                        break;
                }

                if (room.Name == "EntranceBoss")
                {
                    bossRoom = GetSpecificBossRoom(bossRoomType);
                    if (bossRoom != null)
                        bossRoom = bossRoom.Clone() as RoomObj;
                    if (bossRoom == null)
                        bossRoom = GetBossRoom(CDGMath.RandomInt(0, maxRoomIndex)).Clone() as RoomObj;
                    bossRoom.LevelType = room.LevelType;
                    MoveRoom(bossRoom, newRoomPosition);
                    newRoomPosition.X += bossRoom.Width;
                    room.LinkedRoom = bossRoom;
                    bossRoom.LinkedRoom = room;

                    if (bossRoom != null)
                        bossRoomsToAdd.Add(bossRoom);
                    else
                        throw new Exception("Could not find a boss room for the boss entrance. This should NOT be possible. LinkAllBossEntrances()");

                    // Now linking challenge boss rooms
                    challengeRoom = GetChallengeRoom(bossRoomType);
                    if (challengeRoom != null)
                    {
                        challengeRoom = challengeRoom.Clone() as RoomObj;
                        challengeRoom.LevelType = room.LevelType;
                        challengeRoom.LinkedRoom = room;

                        challengeRoomsToAdd.Add(challengeRoom);
                    }
                }
                else if (room.Name == "CastleEntrance") // Creating the special Last boss room that links to tutorial room.
                {
                    // Creating tutorial room and boss room.
                    TutorialRoomObj tutorialRoom = m_tutorialRoom.Clone() as TutorialRoomObj;
                    bossRoom = GetSpecificBossRoom(BossRoomType.LastBossRoom).Clone() as RoomObj;

                    // Moving tutorial room and boss room to proper positions.
                    MoveRoom(tutorialRoom, new Vector2(100000, -100000)); // Special positioning for the last boss room. Necessary since CastleEntranceRoomObj blocks you from going past 0.
                    MoveRoom(bossRoom, new Vector2(150000, -100000));

                    // Linking castle entrance to tutorial room.
                    room.LinkedRoom = tutorialRoom;

                    // Linking tutorial room to boss room.
                    tutorialRoom.LinkedRoom = bossRoom;
                    bossRoom.LinkedRoom = tutorialRoom;

                    if (bossRoom != null)
                    {
                        bossRoomsToAdd.Add(bossRoom);
                        bossRoomsToAdd.Add(tutorialRoom);
                    }
                    else
                        throw new Exception("Could not find a boss room for the boss entrance. This should NOT be possible. LinkAllBossEntrances()");
                }
            }

            // Adding the Last boss challenge room.
            challengeRoom = GetChallengeRoom(BossRoomType.LastBossRoom);
            if (challengeRoom != null)
            {
                challengeRoom = challengeRoom.Clone() as RoomObj;
                challengeRoom.LevelType = GameTypes.LevelType.CASTLE;
                challengeRoom.LinkedRoom = null;

                challengeRoomsToAdd.Add(challengeRoom);
            }

            //Console.WriteLine("Adding boss rooms to level");
            roomList.AddRange(bossRoomsToAdd);
            roomList.AddRange(challengeRoomsToAdd);
        }

        public static List<RoomObj> GetRoomList(int roomWidth, int roomHeight, GameTypes.LevelType levelType)
        {
            return GetLevelTypeRoomArray(levelType)[roomWidth - 1, roomHeight - 1];
        }

        public static RoomObj StartingRoom
        {
            get { return m_startingRoom; }
        }

        public static RoomObj GetBossRoom(int index)
        {
            return m_bossRoomArray[index];
        }

        public static RoomObj GetSpecificBossRoom(byte bossRoomType)
        {
            foreach (RoomObj room in m_bossRoomArray)
            {
                if (room.Tag != "" && byte.Parse(room.Tag) == bossRoomType)
                    return room;
            }
            return null;
        }

        public static RoomObj GetChallengeRoom(byte bossRoomType)
        {
            foreach (RoomObj room in m_challengeRoomArray)
            {
                if (room.Tag != "" && byte.Parse(room.Tag) == bossRoomType)
                    return room;
            }
            return null;
        }

        public static RoomObj GetChallengeBossRoomFromRoomList(GameTypes.LevelType levelType, List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
            {
                if (room.Name == "ChallengeBoss")
                {
                    if (room.LevelType == levelType)
                        return room;
                }
            }
            return null;
        }

        public static List<RoomObj>[,] GetLevelTypeRoomArray(GameTypes.LevelType levelType)
        {
            switch (levelType)
            {
                default:
                case (GameTypes.LevelType.NONE):
                    throw new Exception("Cannot create level of type NONE");
                case (GameTypes.LevelType.CASTLE):
                    return m_castleRoomArray;
                case (GameTypes.LevelType.GARDEN):
                    return m_gardenRoomArray;
                case (GameTypes.LevelType.TOWER):
                    return m_towerRoomArray;
                case (GameTypes.LevelType.DUNGEON):
                    return m_dungeonRoomArray;
            }
        }

        public static void IndexRoomList()
        {
            int index = 0;
            foreach (RoomObj room in SequencedRoomList)
            {
                room.PoolIndex = index;
                index++;
            }

            // Storing DLC maps. For easy differentiating (for extensibility in the future):
            // 10000 is for Castle areas.
            // 20000 is for Garden areas.
            // 30000 is for Tower areas.
            // 40000 is for Dungeon areas.

            index = 10000;
            List<RoomObj> roomList = GetSequencedDLCRoomList(GameTypes.LevelType.CASTLE);
            foreach (RoomObj room in roomList)
            {
                room.PoolIndex = index;
                index++;
            }

            index = 20000;
            roomList = GetSequencedDLCRoomList(GameTypes.LevelType.GARDEN);
            foreach (RoomObj room in roomList)
            {
                room.PoolIndex = index;
                index++;
            }

            index = 30000;
            roomList = GetSequencedDLCRoomList(GameTypes.LevelType.TOWER);
            foreach (RoomObj room in roomList)
            {
                room.PoolIndex = index;
                index++;
            }

            index = 40000;
            roomList = GetSequencedDLCRoomList(GameTypes.LevelType.DUNGEON);
            foreach (RoomObj room in roomList)
            {
                room.PoolIndex = index;
                index++;
            }
        }

        public static List<RoomObj> SequencedRoomList
        {
            get
            {
                List<RoomObj> sequencedRoomList = new List<RoomObj>();

                // Add the special rooms first.
                sequencedRoomList.Add(m_startingRoom);

                sequencedRoomList.Add(m_linkerCastleRoom);
                sequencedRoomList.Add(m_linkerTowerRoom);
                sequencedRoomList.Add(m_linkerDungeonRoom);
                sequencedRoomList.Add(m_linkerGardenRoom);

                sequencedRoomList.Add(m_bossCastleEntranceRoom);
                sequencedRoomList.Add(m_bossTowerEntranceRoom);
                sequencedRoomList.Add(m_bossDungeonEntranceRoom);
                sequencedRoomList.Add(m_bossGardenEntranceRoom);

                sequencedRoomList.Add(m_castleEntranceRoom);

                // Add the normal rooms.
                foreach (List<RoomObj> roomList in m_castleRoomArray)
                    sequencedRoomList.AddRange(roomList);
                foreach (List<RoomObj> roomList in m_dungeonRoomArray)
                    sequencedRoomList.AddRange(roomList);
                foreach (List<RoomObj> roomList in m_towerRoomArray)
                    sequencedRoomList.AddRange(roomList);
                foreach (List<RoomObj> roomList in m_gardenRoomArray)
                    sequencedRoomList.AddRange(roomList);

                // Add the secret rooms.
                sequencedRoomList.AddRange(m_secretCastleRoomArray);
                sequencedRoomList.AddRange(m_secretTowerRoomArray);
                sequencedRoomList.AddRange(m_secretDungeonRoomArray);
                sequencedRoomList.AddRange(m_secretGardenRoomArray);

                // Add the bonus rooms.
                sequencedRoomList.AddRange(m_bonusCastleRoomArray);
                sequencedRoomList.AddRange(m_bonusTowerRoomArray);
                sequencedRoomList.AddRange(m_bonusDungeonRoomArray);
                sequencedRoomList.AddRange(m_bonusGardenRoomArray);

                // Add the boss room array.
                sequencedRoomList.AddRange(m_bossRoomArray);

                // Add the challenge room array.
                sequencedRoomList.AddRange(m_challengeRoomArray);

                //Add the compass room.
                sequencedRoomList.Add(m_compassRoom);

                for (int i = 0; i < sequencedRoomList.Count; i++)
                {
                    if (sequencedRoomList[i] == null)
                    {
                        Console.WriteLine("WARNING: Null room found at index " + i + " of sequencedRoomList.  Removing room...");
                        sequencedRoomList.RemoveAt(i);
                        i--;
                    }
                }
                return sequencedRoomList;
            }
        }

        // Only for DLC rooms.
        // The logic is different from Non-DLC rooms to allow for easily adding more rooms in the future.
        public static List<RoomObj> GetSequencedDLCRoomList(GameTypes.LevelType levelType)
        {
            switch (levelType)
            {
                case GameTypes.LevelType.CASTLE:
                    return m_dlcCastleRoomArray;
                case GameTypes.LevelType.DUNGEON:
                    return m_dlcDungeonRoomArray;
                case GameTypes.LevelType.GARDEN:
                    return m_dlcGardenRoomArray;
                case GameTypes.LevelType.TOWER:
                    return m_dlcTowerRoomArray;
            }
            return null;
        }

        public static void RefreshTextObjs()
        {
            // TODO, empty for now
            //if (m_tutorialRoom != null) m_tutorialRoom.RefrestTextObjs();
        }
    }

    public struct AreaStruct
    {
        public string Name;
        public GameTypes.LevelType LevelType;
        public Vector2 EnemyLevel;
        public Vector2 TotalRooms;
        public Vector2 BonusRooms;
        public Vector2 SecretRooms;
        public int BossLevel;
        public int EnemyLevelScale;
        public bool BossInArea;
        public bool IsFinalArea;
        public Color Color;
        public Color MapColor;
        public bool LinkToCastleOnly;
        public byte BossType;
    }
}

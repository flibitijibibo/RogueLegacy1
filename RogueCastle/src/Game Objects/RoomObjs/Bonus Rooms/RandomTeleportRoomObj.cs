using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class RandomTeleportRoomObj : BonusRoomObj
    {
        private TeleporterObj m_teleporter;

        public RandomTeleportRoomObj()
        {
        }

        public override void Initialize()
        {
            m_teleporter = new TeleporterObj();
            SpriteObj fauxTeleporter = null;

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Name == "teleporter")
                {
                    m_teleporter.Position = obj.Position;
                    fauxTeleporter = obj as SpriteObj;
                    break;
                }
            }

            GameObjList.Remove(fauxTeleporter);
            GameObjList.Add(m_teleporter);

            m_teleporter.OutlineWidth = 2;
            m_teleporter.TextureColor = Color.PaleVioletRed;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (CollisionMath.Intersects(Player.Bounds, m_teleporter.Bounds) && Player.IsTouchingGround == true)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                    TeleportPlayer();
            }
        }

        private void TeleportPlayer()
        {
            ProceduralLevelScreen level = Game.ScreenManager.GetLevelScreen();
            PlayerObj player = Game.ScreenManager.Player;
            player.UpdateCollisionBoxes();

            if (level != null)
            {
                Vector2 teleportPos = new Vector2();

                int roomCount = level.RoomList.Count - 1;
                if (roomCount < 1) roomCount = 1;
                int randRoomIndex = CDGMath.RandomInt(0, roomCount);

                RoomObj roomToTeleportTo = level.RoomList[randRoomIndex];

                // Ensures the room to teleport to isn't a specific type, and has at least one left/right/bottom door.
                while (roomToTeleportTo.Name == "Boss" || roomToTeleportTo.Name == "Start" || roomToTeleportTo.Name == "Ending" || roomToTeleportTo.Name== "Compass" || roomToTeleportTo.Name == "ChallengeBoss" ||
                     roomToTeleportTo.Name == "Throne" || roomToTeleportTo.Name == "Tutorial" || roomToTeleportTo.Name == "EntranceBoss" || roomToTeleportTo.Name == "Bonus" ||
                      roomToTeleportTo.Name == "Linker" || roomToTeleportTo.Name=="Secret" || roomToTeleportTo.Name == "CastleEntrance" || roomToTeleportTo.DoorList.Count < 2)
                {
                    randRoomIndex = CDGMath.RandomInt(0, roomCount);
                    roomToTeleportTo = level.RoomList[randRoomIndex];
                }
                
                foreach (DoorObj door in roomToTeleportTo.DoorList)
                {
                    bool breakout = false;
                    teleportPos.X = door.Bounds.Center.X;
                    switch (door.DoorPosition)
                    {
                        case("Left"):
                        case ("Right"):
                            breakout = true;
                            teleportPos.Y = door.Bounds.Bottom - (player.Bounds.Bottom - player.Y);
                            break;
                        case ("Bottom"):
                            teleportPos.Y = door.Bounds.Top - (player.Bounds.Bottom - player.Y);
                            breakout = true;
                            break;
                    }

                    if (breakout == true)
                        break;
                }

                player.TeleportPlayer(teleportPos, m_teleporter);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_teleporter = null;
                base.Dispose();
            }
        }
    }
}

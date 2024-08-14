using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;

namespace RogueCastle
{
    public class MapObj : GameObj
    {
        private PlayerObj m_player;
        private ProceduralLevelScreen m_level;

        private List<SpriteObj> m_roomSpriteList;
        private List<SpriteObj> m_doorSpriteList;
        private List<SpriteObj> m_iconSpriteList;
        private List<Vector2> m_roomSpritePosList;
        private List<Vector2> m_doorSpritePosList;
        private List<Vector2> m_iconSpritePosList;
        private SpriteObj m_playerSprite;

        public Vector2 CameraOffset;
        private Vector2 m_spriteScale;
        private List<RoomObj> m_addedRooms; // Necessary to keep track of the rooms added so that they don't get added multiple times.

        private RenderTarget2D m_alphaMaskRT, m_mapScreenRT; // Two render targets. The first is a black and white mask, where white represents invisible and black represents drawable area.
                                                             // The second is an RT of the actual map screen. The two are then merged and drawn to the actual screen.
        private Rectangle m_alphaMaskRect;                   // A rectangle that stores the actual rectangle size of the current alpha mask being used (assuming it's rectangular).

        private List<SpriteObj> m_teleporterList;
        private List<Vector2> m_teleporterPosList;
        public bool DrawTeleportersOnly { get; set; }
        public bool DrawNothing { get; set; }

        TweenObject m_xOffsetTween = null;
        TweenObject m_yOffsetTween = null;

        public bool FollowPlayer { get; set; }

        public MapObj(bool followPlayer, ProceduralLevelScreen level)
        {
            m_level = level;
            FollowPlayer = followPlayer;

            this.Opacity = 0.3f;
            m_roomSpriteList = new List<SpriteObj>();
            m_doorSpriteList = new List<SpriteObj>();
            m_iconSpriteList = new List<SpriteObj>();
            m_roomSpritePosList = new List<Vector2>();
            m_doorSpritePosList = new List<Vector2>();
            m_iconSpritePosList = new List<Vector2>();

            CameraOffset = new Vector2(20, 560);

            m_playerSprite = new SpriteObj("MapPlayerIcon_Sprite");
            m_playerSprite.AnimationDelay = 1 / 30f;
            m_playerSprite.ForceDraw = true;
            m_playerSprite.PlayAnimation(true);

            m_spriteScale = new Vector2(22f, 22.5f);
            m_addedRooms = new List<RoomObj>();
            m_teleporterList = new List<SpriteObj>();
            m_teleporterPosList = new List<Vector2>();
        }
        
        public void InitializeAlphaMap(Rectangle mapSize, Camera2D camera)
        {
            m_alphaMaskRect = mapSize;
            m_mapScreenRT = new RenderTarget2D(camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            m_alphaMaskRT = new RenderTarget2D(camera.GraphicsDevice, 1320, 720, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            CameraOffset = new Vector2(mapSize.X, mapSize.Y);

            SpriteObj alphaMaskSprite = new SpriteObj("MapMask_Sprite");
            alphaMaskSprite.ForceDraw = true;
            alphaMaskSprite.Position = new Vector2(mapSize.X, mapSize.Y);
            alphaMaskSprite.Scale = new Vector2((float)mapSize.Width / alphaMaskSprite.Width, (float)mapSize.Height / alphaMaskSprite.Height);
            camera.GraphicsDevice.SetRenderTarget(m_alphaMaskRT);
            camera.GraphicsDevice.Clear(Color.White);
            camera.Begin();
            alphaMaskSprite.Draw(camera);
            camera.End();
            camera.GraphicsDevice.SetRenderTarget(Game.ScreenManager.RenderTarget);
        }

        public void InitializeAlphaMap(RenderTarget2D mapScreenRT, RenderTarget2D alphaMaskRT, Rectangle mapSize)
        {
            m_mapScreenRT = mapScreenRT;
            m_alphaMaskRT = alphaMaskRT;
            m_alphaMaskRect = mapSize;
            CameraOffset = new Vector2(mapSize.X, mapSize.Y);
        }

        public void DisposeRTs()
        {
            m_mapScreenRT.Dispose();
            m_mapScreenRT = null;
            m_alphaMaskRT.Dispose();
            m_alphaMaskRT = null;
        }

        public void SetPlayer(PlayerObj player)
        {
            m_player = player;
        }

        public void AddRoom(RoomObj room)
        {
            if (m_addedRooms.Contains(room) == false && room.Width / 1320 < 5) // A small check to make sure a too large a room is not added to the map.
            {
                SpriteObj roomSprite = new SpriteObj("MapRoom" + (int)(room.Width / 1320) + "x" + (int)(room.Height / 720) + "_Sprite");
                roomSprite.Position = new Vector2(room.X / m_spriteScale.X, room.Y / m_spriteScale.Y);
                roomSprite.Scale = new Vector2((roomSprite.Width - 3f) / roomSprite.Width, (roomSprite.Height - 3f) / roomSprite.Height);
                roomSprite.ForceDraw = true;
                roomSprite.TextureColor = room.TextureColor;
                m_roomSpriteList.Add(roomSprite);
                m_roomSpritePosList.Add(roomSprite.Position);
                foreach (DoorObj door in room.DoorList)
                {
                    if (room.Name == "CastleEntrance" && door.DoorPosition == "Left") // Special code to hide the left door for the Castle Entrance.
                        continue;

                    bool addDoor = false;

                    SpriteObj doorSprite = new SpriteObj("MapDoor_Sprite");
                    doorSprite.ForceDraw = true;
                    switch (door.DoorPosition)
                    {
                        case ("Left"):
                            doorSprite.Position = new Vector2(room.Bounds.Left / m_spriteScale.X - doorSprite.Width + 2, door.Y / m_spriteScale.Y - 2);
                            addDoor = true;
                            break;
                        case ("Right"):
                            doorSprite.Position = new Vector2(room.Bounds.Right / m_spriteScale.X - 5, door.Y / m_spriteScale.Y - 2);
                            addDoor = true;
                            break;
                        case ("Bottom"):
                            doorSprite.Rotation = -90;
                            doorSprite.Position = new Vector2(door.X / m_spriteScale.X, (door.Y + door.Height) / m_spriteScale.Y + 2);
                            addDoor = true;
                            break;
                        case ("Top"):
                            doorSprite.Rotation = -90;
                            doorSprite.Position = new Vector2(door.X / m_spriteScale.X, door.Y / m_spriteScale.Y + 2);
                            addDoor = true;
                            break;
                    }

                    if (addDoor == true)
                    {
                        m_doorSpritePosList.Add(doorSprite.Position);
                        m_doorSpriteList.Add(doorSprite);
                    }
                }

                // Don't add these if you're the spelunker (banker2) because it's already added at the start of the game.
                if (room.Name != "Bonus" && Game.PlayerStats.Class != ClassType.Banker2) // Some bonus rooms have tons of chests. Don't litter it with chest icons.
                {
                    foreach (GameObj obj in room.GameObjList)
                    {
                        ChestObj chest = obj as ChestObj;
                        if (chest != null)
                        {
                            SpriteObj chestSprite = null;
                            if (chest.IsOpen == true)
                                chestSprite = new SpriteObj("MapChestUnlocked_Sprite");
                            else if (chest is FairyChestObj)
                            {
                                chestSprite = new SpriteObj("MapFairyChestIcon_Sprite");
                                if ((chest as FairyChestObj).ConditionType == ChestConditionType.InvisibleChest)
                                    chestSprite.Opacity = 0.2f;
                            }
                            else
                                chestSprite = new SpriteObj("MapLockedChestIcon_Sprite");
                            m_iconSpriteList.Add(chestSprite);
                            chestSprite.AnimationDelay = 1 / 30f;
                            chestSprite.PlayAnimation(true);
                            chestSprite.ForceDraw = true;
                            chestSprite.Position = new Vector2(obj.X / m_spriteScale.X - 8, obj.Y / m_spriteScale.Y - 12);
                            if (room.IsReversed == true)
                                chestSprite.X -= (obj.Width / m_spriteScale.X);
                            m_iconSpritePosList.Add(chestSprite.Position);
                        }
                    }
                }

                if (room.Name == "EntranceBoss")
                {
                    SpriteObj bossSprite = new SpriteObj("MapBossIcon_Sprite");
                    bossSprite.AnimationDelay = 1 / 30f;
                    bossSprite.ForceDraw = true;
                    bossSprite.PlayAnimation(true);
                    bossSprite.Position = new Vector2((room.X + room.Width / 2f) / m_spriteScale.X - bossSprite.Width / 2 - 1, (room.Y + room.Height / 2f) / m_spriteScale.Y - bossSprite.Height / 2 - 2);
                    m_iconSpriteList.Add(bossSprite);
                    m_iconSpritePosList.Add(bossSprite.Position);

                    m_teleporterList.Add(bossSprite);
                    m_teleporterPosList.Add(bossSprite.Position);
                }
                else if (room.Name == "Linker")
                {
                    SpriteObj teleporter = new SpriteObj("MapTeleporterIcon_Sprite");
                    teleporter.AnimationDelay = 1 / 30f;
                    teleporter.ForceDraw = true;
                    teleporter.PlayAnimation(true);
                    teleporter.Position = new Vector2((room.X + room.Width / 2f) / m_spriteScale.X - teleporter.Width / 2 - 1, (room.Y + room.Height / 2f) / m_spriteScale.Y - teleporter.Height / 2 - 2);
                    m_iconSpriteList.Add(teleporter);
                    m_iconSpritePosList.Add(teleporter.Position);

                    m_teleporterList.Add(teleporter);
                    m_teleporterPosList.Add(teleporter.Position);
                }
                else if (room.Name == "CastleEntrance")
                {
                    SpriteObj teleporter = new SpriteObj("MapTeleporterIcon_Sprite");
                    teleporter.AnimationDelay = 1 / 30f;
                    teleporter.ForceDraw = true;
                    teleporter.PlayAnimation(true);
                    teleporter.Position = new Vector2((room.X + room.Width / 2f) / m_spriteScale.X - teleporter.Width / 2 - 1, (room.Y + room.Height / 2f) / m_spriteScale.Y - teleporter.Height / 2 - 2);
                    m_iconSpriteList.Add(teleporter);
                    m_iconSpritePosList.Add(teleporter.Position);

                    m_teleporterList.Add(teleporter);
                    m_teleporterPosList.Add(teleporter.Position);
                }

                if (Game.PlayerStats.Class != ClassType.Banker2) // Don't add bonus room icon if you're the spelunker for the same reason you don't add chests.
                {
                    if (room.Name == "Bonus")
                    {
                        SpriteObj bonus = new SpriteObj("MapBonusIcon_Sprite");
                        bonus.PlayAnimation(true);
                        bonus.AnimationDelay = 1 / 30f;
                        bonus.ForceDraw = true;
                        bonus.Position = new Vector2((room.X + room.Width / 2f) / m_spriteScale.X - bonus.Width / 2 - 1, (room.Y + room.Height / 2f) / m_spriteScale.Y - bonus.Height / 2 - 2);
                        m_iconSpriteList.Add(bonus);
                        m_iconSpritePosList.Add(bonus.Position);
                    }
                }

                m_addedRooms.Add(room);
            }
        }

        public void AddAllRooms(List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
                AddRoom(room);
        }

        public void AddAllIcons(List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
            {
                if (m_addedRooms.Contains(room) == false)
                {
                    if (room.Name != "Bonus") // Some bonus rooms have tons of chests. Don't litter it with chest icons.
                    {
                        foreach (GameObj obj in room.GameObjList)
                        {
                            ChestObj chest = obj as ChestObj;
                            if (chest != null)
                            {
                                SpriteObj chestSprite = null;
                                if (chest.IsOpen == true)
                                    chestSprite = new SpriteObj("MapChestUnlocked_Sprite");
                                else if (chest is FairyChestObj)
                                {
                                    chestSprite = new SpriteObj("MapFairyChestIcon_Sprite");
                                    if ((chest as FairyChestObj).ConditionType == ChestConditionType.InvisibleChest)
                                        chestSprite.Opacity = 0.2f;
                                }
                                else
                                    chestSprite = new SpriteObj("MapLockedChestIcon_Sprite");
                                m_iconSpriteList.Add(chestSprite);
                                chestSprite.AnimationDelay = 1 / 30f;
                                chestSprite.PlayAnimation(true);
                                chestSprite.ForceDraw = true;
                                chestSprite.Position = new Vector2(obj.X / m_spriteScale.X - 8, obj.Y / m_spriteScale.Y - 12);
                                if (room.IsReversed == true)
                                    chestSprite.X -= (obj.Width / m_spriteScale.X);
                                m_iconSpritePosList.Add(chestSprite.Position);
                            }
                        }
                    }
                    else if (room.Name == "Bonus")
                    {
                        SpriteObj bonus = new SpriteObj("MapBonusIcon_Sprite");
                        bonus.PlayAnimation(true);
                        bonus.AnimationDelay = 1 / 30f;
                        bonus.ForceDraw = true;
                        bonus.Position = new Vector2((room.X + room.Width / 2f) / m_spriteScale.X - bonus.Width / 2 - 1, (room.Y + room.Height / 2f) / m_spriteScale.Y - bonus.Height / 2 - 2);
                        m_iconSpriteList.Add(bonus);
                        m_iconSpritePosList.Add(bonus.Position);
                    }
                }
            }
        }

        public void AddLinkerRoom(GameTypes.LevelType levelType, List<RoomObj> roomList)
        {
            foreach (RoomObj room in roomList)
            {
                if (room.Name == "Linker" && room.LevelType == levelType)
                    this.AddRoom(room);
            }
        }

        public void RefreshChestIcons(RoomObj room)
        {
            foreach (GameObj obj in room.GameObjList)
            {
                ChestObj chest = obj as ChestObj;
                if (chest != null && chest.IsOpen == true)
                {
                    Vector2 chestPosition = new Vector2(chest.X / m_spriteScale.X - 8, chest.Y / m_spriteScale.Y - 12);
                    for (int i = 0; i < m_iconSpritePosList.Count; i++)
                    {
                        if (CDGMath.DistanceBetweenPts(chestPosition,m_iconSpritePosList[i]) < 15)
                        {
                            m_iconSpriteList[i].ChangeSprite("MapChestUnlocked_Sprite");
                            m_iconSpriteList[i].Opacity = 1; // Special code needed since Invisible chests set opacity to 0.2f
                            break;
                        }
                    }
                }
            }
        }

        public void CentreAroundPos(Vector2 pos, bool tween = false)
        {
            if (tween == false)
            {
                CameraOffset.X = m_alphaMaskRect.X + m_alphaMaskRect.Width / 2f - ((pos.X / 1320f) * 60);
                CameraOffset.Y = m_alphaMaskRect.Y + m_alphaMaskRect.Height / 2f - ((pos.Y / 720f) * 32);
            }
            else
            {
                if (m_xOffsetTween != null && m_xOffsetTween.TweenedObject == this)
                    m_xOffsetTween.StopTween(false);
                if (m_yOffsetTween != null && m_yOffsetTween.TweenedObject == this)
                    m_yOffsetTween.StopTween(false);

                m_xOffsetTween = Tween.To(this, 0.3f, Tweener.Ease.Quad.EaseOut, "CameraOffsetX", (m_alphaMaskRect.X + m_alphaMaskRect.Width / 2f - ((pos.X / 1320f) * 60)).ToString());
                m_yOffsetTween = Tween.To(this, 0.3f, Tweener.Ease.Quad.EaseOut, "CameraOffsetY", (m_alphaMaskRect.Y + m_alphaMaskRect.Height / 2f - ((pos.Y / 720f) * 32)).ToString());
            }
        }

        public void CentreAroundObj(GameObj obj)
        {
            CentreAroundPos(obj.Position);
        }

        public void CentreAroundPlayer()
        {
            CentreAroundObj(m_player);
        }

        public void TeleportPlayer(int index)
        {
            if (m_teleporterList.Count > 0) // Make sure teleporters exist before you teleport.
            {
                Vector2 pos = m_teleporterPosList[index];
                //Console.WriteLine(pos);
                //pos.X += m_teleporterList[index].Width / 2f;
                //pos.Y += m_teleporterList[index].Height / 2f;
                pos.X += 10;
                pos.Y += 10;
                pos.X *= m_spriteScale.X;
                pos.Y *= m_spriteScale.Y;
                pos.X += 30;
                if (index == 0) // A hack for the CastleEntranceRoom, since its teleporter is not one grid from the ground, but 5.
                {
                    pos.X -= 610;
                    pos.Y += 720 / 2 - (130);
                }
                else
                    pos.Y += 720 / 2 - 70;
                m_player.TeleportPlayer(pos);
            }
        }

        public void CentreAroundTeleporter(int index, bool tween = false)
        {
            Vector2 pos = m_teleporterPosList[index];
            pos.X *= m_spriteScale.X;
            pos.Y *= m_spriteScale.Y;
            CentreAroundPos(pos, tween);
        }

        public void DrawRenderTargets(Camera2D camera)
        {
            if (FollowPlayer == true)
            {
                CameraOffset.X = (int)(m_alphaMaskRect.X + m_alphaMaskRect.Width / 2f - ((m_player.X / 1320) * 60));
                CameraOffset.Y = m_alphaMaskRect.Y + m_alphaMaskRect.Height / 2f - (((int)m_player.Y / 720f) * 32);
            }

            camera.GraphicsDevice.SetRenderTarget(m_mapScreenRT);
            camera.GraphicsDevice.Clear(Color.Transparent);
            //camera.Begin(0, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null);

            for (int i = 0; i < m_roomSpriteList.Count; i++)
            {
                m_roomSpriteList[i].Position = CameraOffset + m_roomSpritePosList[i];
                m_roomSpriteList[i].Draw(camera);
            }

            for (int i = 0; i < m_doorSpriteList.Count; i++)
            {
                m_doorSpriteList[i].Position = CameraOffset + m_doorSpritePosList[i];
                m_doorSpriteList[i].Draw(camera);
            }

            if (DrawTeleportersOnly == false)
            {
                for (int i = 0; i < m_iconSpriteList.Count; i++)
                {
                    m_iconSpriteList[i].Position = CameraOffset + m_iconSpritePosList[i];
                    m_iconSpriteList[i].Draw(camera);
                }
            }
            else
            {
                for (int i = 0; i < m_teleporterList.Count; i++)
                {
                    m_teleporterList[i].Position = CameraOffset + m_teleporterPosList[i];
                    m_teleporterList[i].Draw(camera);
                }
            }

            if (Game.PlayerStats.Traits.X == TraitType.EideticMemory || Game.PlayerStats.Traits.Y == TraitType.EideticMemory)
            {
                m_playerSprite.TextureColor = Color.Red;
                foreach (RoomObj room in m_addedRooms)
                {
                    foreach (EnemyObj enemy in room.EnemyList)
                    {
                        if (enemy.IsKilled == false && enemy.IsDemented == false && enemy.SaveToFile == true && enemy.Type != EnemyType.SpikeTrap && enemy.Type != EnemyType.Platform && enemy.Type != EnemyType.Turret)
                        {
                            m_playerSprite.Position = new Vector2(enemy.X / m_spriteScale.X - 9, enemy.Y / m_spriteScale.Y - 10) + CameraOffset;
                            m_playerSprite.Draw(camera);
                        }
                    }
                }
            }
            m_playerSprite.TextureColor = Color.White;

            m_playerSprite.Position = new Vector2(m_level.Player.X / m_spriteScale.X - 9, m_level.Player.Y / m_spriteScale.Y - 10) + CameraOffset; //-10 to compensate for player width/height.
            m_playerSprite.Draw(camera);
            //camera.End();
        }

        public override void Draw(Camera2D camera)
        {
            if (this.Visible == true)
            {
                camera.End();
                camera.GraphicsDevice.Textures[1] = m_alphaMaskRT;
                camera.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, Game.MaskEffect);
                if (DrawNothing == false)
                    camera.Draw(m_mapScreenRT, Vector2.Zero, Color.White);
                camera.End();

                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null);
                if (DrawNothing == true)
                    m_playerSprite.Draw(camera);
            }
        }

        public void ClearRoomsAdded()
        {
            m_addedRooms.Clear();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                //Done 
                m_player = null;
                m_level = null;
                if (m_alphaMaskRT != null && m_alphaMaskRT.IsDisposed == false)
                    m_alphaMaskRT.Dispose();
                m_alphaMaskRT = null;
                if (m_mapScreenRT != null && m_mapScreenRT.IsDisposed == false)
                    m_mapScreenRT.Dispose();
                m_mapScreenRT = null;

                foreach (SpriteObj sprite in m_roomSpriteList)
                    sprite.Dispose();
                m_roomSpriteList.Clear();
                m_roomSpriteList = null;

                foreach (SpriteObj sprite in m_doorSpriteList)
                    sprite.Dispose();
                m_doorSpriteList.Clear();
                m_doorSpriteList = null;

                foreach (SpriteObj sprite in m_iconSpriteList)
                    sprite.Dispose();
                m_iconSpriteList.Clear();
                m_iconSpriteList = null;

                m_addedRooms.Clear();
                m_addedRooms = null;

                m_roomSpritePosList.Clear();
                m_roomSpritePosList = null;
                m_doorSpritePosList.Clear();
                m_doorSpritePosList = null;
                m_iconSpritePosList.Clear();
                m_iconSpritePosList = null;

                m_playerSprite.Dispose();
                m_playerSprite = null;

                foreach (SpriteObj sprite in m_teleporterList)
                    sprite.Dispose();
                m_teleporterList.Clear();
                m_teleporterList = null;
                m_teleporterPosList.Clear();
                m_teleporterPosList = null;

                m_xOffsetTween = null;
                m_yOffsetTween = null;

                base.Dispose();
            }
        }

        public List<RoomObj> AddedRoomsList
        {
            get { return m_addedRooms; }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new MapObj(FollowPlayer, m_level);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            MapObj clone = obj as MapObj;

            clone.DrawTeleportersOnly = this.DrawTeleportersOnly;
            clone.CameraOffsetX = this.CameraOffsetX;
            clone.CameraOffsetY = this.CameraOffsetY;

            clone.InitializeAlphaMap(m_mapScreenRT, m_alphaMaskRT, m_alphaMaskRect);
            clone.SetPlayer(m_player);
            clone.AddAllRooms(m_addedRooms);
        }

        public SpriteObj[] TeleporterList()
        {
            return m_teleporterList.ToArray();
        }

        public float CameraOffsetX
        {
            get { return CameraOffset.X; }
            set { CameraOffset.X = value; }
        }

        public float CameraOffsetY
        {
            get { return CameraOffset.Y; }
            set { CameraOffset.Y = value; }
        }

    }
}

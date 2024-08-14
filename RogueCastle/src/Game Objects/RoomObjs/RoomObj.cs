using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;
using Tweener;

namespace RogueCastle
{
    public class RoomObj : GameObj
    {
        public bool AddToCastlePool { get; set; }
        public bool AddToGardenPool { get; set; }
        public bool AddToTowerPool { get; set; }
        public bool AddToDungeonPool { get; set; }
        public bool IsDLCMap { get; set; }

        public GameTypes.LevelType LevelType { get; set; }
        public int Level { get; set; }
        public List<DoorObj> DoorList { get; internal set; }
        public List<EnemyObj> EnemyList { get; internal set; }
        public List<TerrainObj> TerrainObjList { get; internal set; }
        public List<GameObj> GameObjList { get; internal set; }
        public List<BorderObj> BorderList { get; internal set; }
        public List<EnemyObj> TempEnemyList { get; internal set; }  // A list used for enemies that are created temporarily, like split blobs.

        public int PoolIndex = -1; // The index number of the room from the pool in LevelBuilder.
        public bool IsReversed { get; internal set; }
        private int m_roomNumber = -1; // The order number of when this room was built.  Used for debugging.

        protected float m_roomActivityDelay = 0.2f;//0.5f;m_empower
        private float m_roomActivityCounter = 0;

        protected float m_doorSparkleDelay = 0.2f;

        public RoomObj LinkedRoom { get; set; } // Linked room. Primarily used for linking boss rooms to boss entrances.

        public string RedEnemyType;
        public string BlueEnemyType;
        public string GreenEnemyType;

        private TextObj m_fairyChestText;

        private TextObj m_indexText; // purely for debug.  Remove later.
        private TextObj m_roomInfoText; // more debug.
        private Vector2 m_debugRoomPosition;

        private RenderTarget2D m_bwRender;

        public PlayerObj Player;
        private SpriteObj m_pauseBG;

        public NpcObj DonationBox { get; set; }

        private int[] m_empowerCostArray =  {7500, 15000, 30000, 45000, 67500, 90000, 120000, 150000, 187500, 225000}; //Super cheap, 17.5% increase Max at 175%
        //{10000, 25000, 47500, 77500, 115000, 160000, 212500, 272500, 340000, 415000}; //50% of costs, and 15%stats and hp.  Might be good.  Max at 150% everything.
        //{ 7500, 15000, 30000, 52500, 82500, 120000, 165000, 217500, 277500, 345000}; 12.5%stats, 12.5% hp.  25% of cost. Not tested
        //{ 5000, 10000, 20000, 40000, 80000, 160000, 320000, 640000, 1280000, 2560000 }; //7.5% stats, 1%hp. Too expensive, too little buffs

        public RoomObj()
        {
            GameObjList = new List<GameObj>();
            TerrainObjList = new List<TerrainObj>();
            DoorList = new List<DoorObj>();
            EnemyList = new List<EnemyObj>();
            BorderList = new List<BorderObj>();
            TempEnemyList = new List<EnemyObj>();

            LevelType = GameTypes.LevelType.NONE;

            m_indexText = new TextObj(Game.PixelArtFontBold);
            m_indexText.FontSize = 150;
            m_indexText.Align = Types.TextAlign.Centre;

            m_roomInfoText = new TextObj(Game.PixelArtFontBold);
            m_roomInfoText.FontSize = 30;
            m_roomInfoText.Align = Types.TextAlign.Centre;

            m_fairyChestText = new TextObj(Game.JunicodeFont);
            m_fairyChestText.FontSize = 26;
            m_fairyChestText.Position = new Vector2(300, 20);
            m_fairyChestText.DropShadow = new Vector2(2, 2);
            m_fairyChestText.Text = "";
            //m_fairyChestText.Align = Types.TextAlign.Centre;
            
            m_pauseBG = new SpriteObj("Blank_Sprite");
            m_pauseBG.TextureColor = Color.Black;
            m_pauseBG.Opacity = 0;

            IsReversed = false;
        }

        public virtual void Initialize() { }

        public virtual void LoadContent(GraphicsDevice graphics) { }

        public virtual void OnEnter() 
        {
            m_roomActivityCounter = m_roomActivityDelay;
            foreach (GameObj obj in GameObjList)
            {
                FairyChestObj fairyChest = obj as FairyChestObj;
                if (fairyChest != null && fairyChest.State == ChestConditionChecker.STATE_FAILED && fairyChest.ConditionType != ChestConditionType.InvisibleChest && fairyChest.ConditionType != ChestConditionType.None)
                    Player.AttachedLevel.ObjectiveFailed();
            }

            bool doorClosed = false;
            if (DonationBox != null)
            {
                DonationBox.CanTalk = true;
                DonationBox.Visible = true;

                foreach (DoorObj door in DoorList)
                {
                    if (door.Name == "BossDoor" && door.Locked == true)
                    {
                        doorClosed = true;
                        break;
                    }
                }

                if (doorClosed == true || (this.LinkedRoom is ChallengeBossRoomObj == false))
                {
                    DonationBox.CanTalk = false;
                    DonationBox.Visible = false;
                }
            }
        }

        public virtual void OnExit() 
        {
            Player.AttachedLevel.ResetObjectivePlate(false);

            for (int i = 0; i < this.TempEnemyList.Count; i++)
            {
                if (TempEnemyList[i].IsDemented == true)
                {
                    Player.AttachedLevel.RemoveEnemyFromRoom(TempEnemyList[i], this);
                    i--;
                }
            }
        }

        public virtual void InitializeRenderTarget(RenderTarget2D bgRenderTarget)
        {
            //m_bwRender = new RenderTarget2D(device, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight);
            //m_bwRender = new RenderTarget2D(device, GlobalEV.ScreenWidth, GlobalEV.ScreenHeight, false, SurfaceFormat., null);
            m_bwRender = bgRenderTarget;
        }

        public void SetWidth(int width)
        {
            _width = width;
            m_pauseBG.Scale = Vector2.One;
            m_pauseBG.Scale = new Vector2((this.Width + 20) / m_pauseBG.Width, (this.Height + 20) / m_pauseBG.Height);
        }

        public void SetHeight(int height)
        {
            _height = height;
            m_pauseBG.Scale = Vector2.One;
            m_pauseBG.Scale = new Vector2((this.Width + 20) / m_pauseBG.Width, (this.Height + 20) / m_pauseBG.Height);
        }

        public void BuffPlayer()
        {
            int cost = GetChallengeRoomCost();
            if (Game.PlayerStats.Gold >= cost && cost > 0)
            {
                Game.PlayerStats.Gold -= cost;
                switch (GetChallengeRoomType())
                {
                    case (0):
                        Game.PlayerStats.ChallengeEyeballTimesUpgraded++;
                        break;
                    case (1):
                        Game.PlayerStats.ChallengeSkullTimesUpgraded++;
                        break;
                    case (2):
                        Game.PlayerStats.ChallengeFireballTimesUpgraded++;
                        break;
                    case (3):
                        Game.PlayerStats.ChallengeBlobTimesUpgraded++;
                        break;
                    case (4):
                        Game.PlayerStats.ChallengeLastBossTimesUpgraded++;
                        break;
                }

                (Game.ScreenManager.Game as RogueCastle.Game).SaveManager.SaveFiles(SaveType.PlayerData);

                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("DonationBoxTalkUpgraded");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);

                // Text effect of the gold cost.
                Player.AttachedLevel.TextManager.DisplayNumberStringText(-(int)cost, "LOC_ID_CARNIVAL_BONUS_ROOM_4" /*"gold"*/, Color.Yellow, new Vector2(Player.X, Player.Bounds.Top));
            }
            else if (cost < 0)
            {
                // Already maxxed empowered
                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("DonationBoxTalkMaxxed");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
            }
            else
            {
                // Can't afford it.
                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("DonationBoxTalkPoor");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
            }
        }

        private sbyte GetChallengeRoomTimesUpgraded()
        {
            sbyte timesUpgraded = 0;

            switch (GetChallengeRoomType())
            {
                case (0):
                    timesUpgraded = Game.PlayerStats.ChallengeEyeballTimesUpgraded;
                    break;
                case (1):
                    timesUpgraded = Game.PlayerStats.ChallengeSkullTimesUpgraded;
                    break;
                case (2):
                    timesUpgraded = Game.PlayerStats.ChallengeFireballTimesUpgraded;
                    break;
                case (3):
                    timesUpgraded = Game.PlayerStats.ChallengeBlobTimesUpgraded;
                    break;
                case (4):
                    timesUpgraded = Game.PlayerStats.ChallengeLastBossTimesUpgraded;
                    break;
            }
            return timesUpgraded;
        }

        private int GetChallengeRoomCost()
        {
            int cost = int.MaxValue;
            sbyte timesUpgraded = GetChallengeRoomTimesUpgraded();
            if (timesUpgraded < m_empowerCostArray.Length - 1)
                cost = m_empowerCostArray[timesUpgraded];
            else
                cost = -1;

            return cost;
        }

        private int GetChallengeRoomType() // -1 is not challenge room
        {
            if (LinkedRoom is EyeballChallengeRoom)
                return 0;
            if (LinkedRoom is FairyChallengeRoom)
                return 1;
            if (LinkedRoom is FireballChallengeRoom)
                return 2;
            if (LinkedRoom is BlobChallengeRoom)
                return 3;
            if (LinkedRoom is LastBossChallengeRoom)
                return 4;

            return -1;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (this.Name == "EntranceBoss" && this.LinkedRoom is ChallengeBossRoomObj)
            {
                if (DonationBox != null)
                {
                    DonationBox.Update(gameTime, Player);

                    if (DonationBox.IsTouching == true)
                    {
                        if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                        {
                            sbyte timesUpgraded = GetChallengeRoomTimesUpgraded();
                            RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                            if (timesUpgraded < m_empowerCostArray.Length - 1)
                            {
                                manager.DialogueScreen.SetDialogue("DonationBoxTalk01", GetChallengeRoomCost(), timesUpgraded);
                                manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                                manager.DialogueScreen.SetConfirmEndHandler(this, "BuffPlayer");
                                manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
                            }
                            else
                            {
                                // Already maxxed empowered
                                manager.DialogueScreen.SetDialogue("DonationBoxTalkMaxxed");
                                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
                            }
                        }
                    }
                }

                if (m_doorSparkleDelay <= 0)
                {
                    m_doorSparkleDelay = 0.1f;
                    Player.AttachedLevel.ImpactEffectPool.DoorSparkleEffect(new Rectangle((int)this.X + 590, (int)this.Y + 140, 190, 150));
                }
                else
                    m_doorSparkleDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (m_roomActivityCounter <= 0)
            {
                foreach (EnemyObj enemy in this.EnemyList)
                {
                    if (enemy.IsKilled == false)
                        enemy.Update(gameTime);
                }

                foreach (EnemyObj enemy in this.TempEnemyList)
                    if (enemy.IsKilled == false)
                        enemy.Update(gameTime);
            }
            else
                m_roomActivityCounter -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // This method creates a Black/White Render target of the room's collision hulls. The render target will be used as a mask for applying a foreground texture to these collision hulls.
        public void DrawRenderTargets(Camera2D camera)
        {
            camera.GraphicsDevice.SetRenderTarget(m_bwRender);
            camera.GraphicsDevice.Clear(Color.White);

            foreach (TerrainObj obj in TerrainObjList)
            {
                if (obj.Visible == true && obj.Height > 40 && obj.ShowTerrain == true)
                {
                    obj.ForceDraw = true;
                    obj.TextureColor = Color.Black;
                    obj.Draw(camera);
                    obj.ForceDraw = false;
                    obj.TextureColor = Color.White;
                }
            }
        }

        public void DrawBGObjs(Camera2D camera)
        {
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Layer == -1)
                    obj.Draw(camera);
            }
        }

        public override void Draw(Camera2D camera)
        {
            if (camera.Zoom != 1)
            {
                foreach (TerrainObj obj in TerrainObjList)
                {
                    obj.Draw(camera); // Keep this so you can see levels when zoomed out.
                    //(obj as TerrainObj).DrawBorders(camera);
                }
            }

            foreach (BorderObj border in BorderList)
                border.Draw(camera);

            //SamplerState storedState = camera.GraphicsDevice.SamplerStates[0];

            //camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            foreach (BorderObj border in BorderList)
            {
                if (border.Rotation == 0)
                    border.DrawCorners(camera);
            }

            //camera.GraphicsDevice.SamplerStates[0] = storedState;

            foreach (GameObj obj in GameObjList)
            {
                if (obj.Layer == 0)
                    obj.Draw(camera);
            }

            // Temporary fix for chests.  Not very performance efficient though.
            foreach (GameObj obj in GameObjList)
            {
                if (obj.Layer == 1)
                    obj.Draw(camera); // For chests
            }

            m_pauseBG.Position = this.Position;
            m_pauseBG.Draw(camera);

            foreach (EnemyObj enemy in EnemyList)
                enemy.Draw(camera);

            foreach (EnemyObj enemy in TempEnemyList)
                enemy.Draw(camera);

            // These need to be drawn to display door arrows.
            foreach (DoorObj door in DoorList)
                door.Draw(camera);

            if (LevelEV.SHOW_DEBUG_TEXT == true)
            {
                m_indexText.Position = new Vector2(this.Position.X + this.Width / 2, this.Position.Y + this.Height / 2 - m_indexText.Height / 2);
                m_indexText.Draw(camera);
                m_roomInfoText.Position = new Vector2(this.Position.X + this.Width / 2, this.Position.Y);
                m_roomInfoText.Draw(camera);
            }

            m_fairyChestText.Draw(camera);
        }

        public virtual void Reset()
        {
            int enemyNum = EnemyList.Count;
            //foreach (EnemyObj enemy in EnemyList)
            for (int i = 0; i < enemyNum; i++)
            {
                EnemyList[i].Reset(); 
                // Needed in case something like a generated blob needs to be removed from the room.
                if (enemyNum != EnemyList.Count)
                {
                    i--;
                    enemyNum = EnemyList.Count;
                }
            }

            for (int i = 0; i < TempEnemyList.Count; i++)
            {
                TempEnemyList[i].Reset();
                i--; // Enemies are removed from the templist on reset.
            }
        }

        // Flips all objects in the room along the horizontal axis, including doors.
        public void Reverse()
        {
            IsReversed = true;
            float centrePtX = this.X + this.Width / 2;

            foreach (TerrainObj obj in TerrainObjList)
            {
                if (obj.Name == "Left") obj.Name = "Right";
                else if (obj.Name == "Right") obj.Name = "Left";
                else if (obj.Name == "!Left") obj.Name = "!Right";
                else if (obj.Name == "!Right") obj.Name = "!Left";
                else if (obj.Name == "!RightTop") obj.Name = "!LeftTop";
                else if (obj.Name == "!RightBottom") obj.Name = "!LeftBottom";
                else if (obj.Name == "!LeftTop") obj.Name = "!RightTop";
                else if (obj.Name == "!LeftBottom") obj.Name = "!RightBottom";
                else if (obj.Name == "RightTop") obj.Name = "LeftTop";
                else if (obj.Name == "RightBottom") obj.Name = "LeftBottom";
                else if (obj.Name == "LeftTop") obj.Name = "RightTop";
                else if (obj.Name == "LeftBottom") obj.Name = "RightBottom";
                else if (obj.Name == "!BottomLeft") obj.Name = "!BottomRight";
                else if (obj.Name == "!BottomRight") obj.Name = "!BottomLeft";
                else if (obj.Name == "!TopLeft") obj.Name = "!TopRight";
                else if (obj.Name == "!TopRight") obj.Name = "!TopLeft";
                else if (obj.Name == "BottomLeft") obj.Name = "BottomRight";
                else if (obj.Name == "BottomRight") obj.Name = "BottomLeft";
                else if (obj.Name == "TopLeft") obj.Name = "TopRight";
                else if (obj.Name == "TopRight") obj.Name = "TopLeft";

                float distFromCentreX = obj.X - centrePtX;
                if (distFromCentreX >= 0)
                {
                    if (obj.Rotation == 0)
                        obj.X -= (distFromCentreX * 2 + obj.Width);
                    else
                    {
                        Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                        topRight.X -= (topRight.X - centrePtX) * 2;
                        obj.Position = topRight;
                        obj.Rotation = -obj.Rotation;
                    }
                }
                else
                {
                    if (obj.Rotation == 0)
                        obj.X = (centrePtX - distFromCentreX) - obj.Width;
                    else
                    {
                        Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                        topRight.X += (centrePtX - topRight.X) * 2;
                        obj.Position = topRight;
                        obj.Rotation = -obj.Rotation;

                    }
                }
            }

            foreach (GameObj obj in GameObjList)
            {
                ReverseObjNames(obj);

                if ((obj is HazardObj) == false && (obj is ChestObj == false))
                {
                    if (obj.Flip == SpriteEffects.None)
                        obj.Flip = SpriteEffects.FlipHorizontally;
                    else
                        obj.Flip = SpriteEffects.None;

                    float distanceFromLeftWall = obj.X - this.X;
                    obj.X = this.Bounds.Right - distanceFromLeftWall;
                    if (obj.Rotation != 0)
                        obj.Rotation = -obj.Rotation;
                }
                else
                {
                    if (obj is ChestObj)
                    {
                        if (obj.Flip == SpriteEffects.None)
                            obj.Flip = SpriteEffects.FlipHorizontally;
                        else
                            obj.Flip = SpriteEffects.None;
                    }

                    float distFromCentreX = obj.X - centrePtX;
                    if (distFromCentreX >= 0)
                    {
                        if (obj.Rotation == 0)
                            obj.X -= (distFromCentreX * 2 + obj.Width);
                        else
                        {
                            Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                            topRight.X -= (topRight.X - centrePtX) * 2;
                            obj.Position = topRight;
                            obj.Rotation = -obj.Rotation;
                        }
                    }
                    else
                    {
                        if (obj.Rotation == 0)
                            obj.X = (centrePtX - distFromCentreX) - obj.Width;
                        else
                        {
                            Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                            topRight.X += (centrePtX - topRight.X) * 2;
                            obj.Position = topRight;
                            obj.Rotation = -obj.Rotation;

                        }
                    }
                }
            }

            foreach (EnemyObj obj in EnemyList)
            {
                if (obj.Flip == SpriteEffects.None)
                {
                    obj.Flip = SpriteEffects.FlipHorizontally;
                    obj.InternalFlip = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    obj.Flip = SpriteEffects.None;
                    obj.InternalFlip = SpriteEffects.None;
                }

                ReverseObjNames(obj);

                float distFromCentreX = obj.X - centrePtX;
                if (distFromCentreX >= 0)
                {
                    //if (obj.Rotation == 0)
                        obj.X -= (distFromCentreX * 2);
                        obj.Rotation = -obj.Rotation;
                    //else
                    //{
                    //    Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                    //    topRight.X -= (topRight.X - centrePtX) * 2;
                    //    obj.Position = topRight;
                    //    obj.Rotation = -obj.Rotation;
                    //}
                }
                else
                {
                    //if (obj.Rotation == 0)
                    obj.X = (centrePtX - distFromCentreX);
                        obj.Rotation = -obj.Rotation;
                    //else
                    //{
                    //    Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                    //    topRight.X += (centrePtX - topRight.X) * 2;
                    //    obj.Position = topRight;
                    //    obj.Rotation = -obj.Rotation;

                    //}
                }
            }

            // Each spriteobj may need to be drawn flipped.
            foreach (BorderObj obj in BorderList)
            {
                ReverseObjNames(obj);

                float distFromCentreX = obj.X - centrePtX;
                if (distFromCentreX >= 0)
                {
                    if (obj.Rotation == 0)
                        obj.X -= (distFromCentreX * 2 + obj.Width);
                    else
                    {
                        Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                        topRight.X -= (topRight.X - centrePtX) * 2;
                        obj.Position = topRight;
                        obj.Rotation = -obj.Rotation;
                    }
                }
                else
                {
                    if (obj.Rotation == 0)
                        obj.X = (centrePtX - distFromCentreX) - obj.Width;
                    else
                    {
                        Vector2 topRight = CollisionMath.UpperRightCorner(new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero);
                        topRight.X += (centrePtX - topRight.X) * 2;
                        obj.Position = topRight;
                        obj.Rotation = -obj.Rotation;

                    }
                }

                if (obj.BorderRight == true)
                {
                    if (obj.BorderLeft == false)
                        obj.BorderRight = false;
                    obj.BorderLeft = true;
                }
                else if (obj.BorderLeft == true)
                {
                    if (obj.BorderRight == false)
                        obj.BorderLeft = false;
                    obj.BorderRight = true;
                }
            }

            foreach (DoorObj door in DoorList)
            {
                if (door.DoorPosition == "Left")
                {
                    door.X = this.X + this.Width - door.Width;
                    door.DoorPosition = "Right";
                }
                else if (door.DoorPosition == "Right")
                {
                    door.X = this.X;
                    door.DoorPosition = "Left";
                }
                else if (door.DoorPosition == "Top")
                {
                    float distFromCentreX = door.X - centrePtX;
                    if (distFromCentreX >= 0)
                        door.X -= (distFromCentreX * 2 + door.Width);
                    else
                        door.X = (centrePtX - distFromCentreX) - door.Width;
                }
                else if (door.DoorPosition == "Bottom")
                {
                    float distFromCentreX = door.X - centrePtX;
                    if (distFromCentreX >= 0)
                        door.X -= (distFromCentreX * 2 + door.Width);
                    else
                        door.X = (centrePtX - distFromCentreX) - door.Width;
                }
            }
        }

        private void ReverseObjNames(GameObj obj)
        {
            if (obj.Name == "Left") obj.Name = "Right";
            else if (obj.Name == "Right") obj.Name = "Left";
            else if (obj.Name == "!Left") obj.Name = "!Right";
            else if (obj.Name == "!Right") obj.Name = "!Left";
            else if (obj.Name == "!RightTop") obj.Name = "!LeftTop";
            else if (obj.Name == "!RightBottom") obj.Name = "!LeftBottom";
            else if (obj.Name == "!LeftTop") obj.Name = "!RightTop";
            else if (obj.Name == "!LeftBottom") obj.Name = "!RightBottom";
            else if (obj.Name == "RightTop") obj.Name = "LeftTop";
            else if (obj.Name == "RightBottom") obj.Name = "LeftBottom";
            else if (obj.Name == "LeftTop") obj.Name = "RightTop";
            else if (obj.Name == "LeftBottom") obj.Name = "RightBottom";
            else if (obj.Name == "!BottomLeft") obj.Name = "!BottomRight";
            else if (obj.Name == "!BottomRight") obj.Name = "!BottomLeft";
            else if (obj.Name == "!TopLeft") obj.Name = "!TopRight";
            else if (obj.Name == "!TopRight") obj.Name = "!TopLeft";
            else if (obj.Name == "BottomLeft") obj.Name = "BottomRight";
            else if (obj.Name == "BottomRight") obj.Name = "BottomLeft";
            else if (obj.Name == "TopLeft") obj.Name = "TopRight";
            else if (obj.Name == "TopRight") obj.Name = "TopLeft";
        }

        protected override GameObj CreateCloneInstance()
        {
            return new RoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            //WARNING: ANYTHING CHANGED HERE NEEDS TO CHANGE IN STARTINGROOMOBJ.CONVERTTOSTARTINGROOM().
            RoomObj clone = obj as RoomObj;
            clone.SetWidth(_width);
            clone.SetHeight(_height);

            foreach (TerrainObj terrainObj in TerrainObjList)
                clone.TerrainObjList.Add(terrainObj.Clone() as TerrainObj);

            foreach (GameObj gameObj in GameObjList)
            {
                GameObj cloneObj = gameObj.Clone() as GameObj;
                if (cloneObj is NpcObj && cloneObj.Name == "donationbox")
                    clone.DonationBox = cloneObj as NpcObj;
                clone.GameObjList.Add(cloneObj);
            }

            foreach (DoorObj door in DoorList)
            {
                DoorObj cloneDoor = door.Clone() as DoorObj;
                cloneDoor.Room = clone; // Necessary since a cloned door returns a reference to the old room, NOT the cloned room.
                clone.DoorList.Add(cloneDoor);
            }

            foreach (EnemyObj enemy in EnemyList)
                clone.EnemyList.Add(enemy.Clone() as EnemyObj);

            foreach (BorderObj borderObj in BorderList)
                clone.BorderList.Add(borderObj.Clone() as BorderObj);


            clone.AddToCastlePool = this.AddToCastlePool;
            clone.AddToGardenPool = this.AddToGardenPool;
            clone.AddToDungeonPool = this.AddToDungeonPool;
            clone.AddToTowerPool = this.AddToTowerPool;
            clone.PoolIndex = this.PoolIndex;

            clone.RedEnemyType = this.RedEnemyType;
            clone.BlueEnemyType = this.BlueEnemyType;
            clone.GreenEnemyType = this.GreenEnemyType;

            clone.LinkedRoom = this.LinkedRoom;
            clone.IsReversed = this.IsReversed;
            clone.DebugRoomPosition = this.DebugRoomPosition;
            clone.LevelType = this.LevelType;
            clone.Level = this.Level;
            clone.IsDLCMap = this.IsDLCMap;
        }

        public void DisplayFairyChestInfo()
        {
            FairyChestObj fairyChest = null;
            WaypointObj waypoint = null;

            foreach (GameObj obj in GameObjList)
            {
                if (obj is FairyChestObj)
                    fairyChest = obj as FairyChestObj;

                if (obj is WaypointObj)
                    waypoint = obj as WaypointObj;
            }

            if (fairyChest != null && fairyChest.IsOpen == false && fairyChest.ConditionType != ChestConditionType.None && fairyChest.ConditionType != ChestConditionType.InvisibleChest 
                && fairyChest.State != ChestConditionChecker.STATE_FREE)
            //if (fairyChest != null && fairyChest.IsOpen == false && waypoint != null && fairyChest.ConditionType != 0)
            {
                ////(Player.AttachedLevel.ScreenManager as RCScreenManager).DialogueScreen.SetDialogue("Chest_Locked " + fairyChest.ConditionType);
                ////(Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true);
                //m_fairyChestText.Text = DialogueManager.GetText("Chest_Locked " + fairyChest.ConditionType).Dialogue[0];
                //m_fairyChestText.X = waypoint.X - m_fairyChestText.Width / 2f;
                //m_fairyChestText.Y = waypoint.Y - m_fairyChestText.Height / 2f;
                //m_fairyChestText.BeginTypeWriting(1);

                Player.AttachedLevel.DisplayObjective("LOC_ID_LEVEL_SCREEN_2", //"Fairy Chest Objective"
                    DialogueManager.GetText("Chest_Locked " + fairyChest.ConditionType).Speakers[0],
                    DialogueManager.GetText("Chest_Locked " + fairyChest.ConditionType).Dialogue[0],
                    true);
            }
            else
                m_fairyChestText.Text = "";
        }

        public void DarkenRoom()
        {
            Tweener.Tween.To(m_pauseBG, 0.1f, Tweener.Tween.EaseNone, "Opacity", "0.7");
        }

        public virtual void PauseRoom()
        {
            //Tweener.Tween.To(m_pauseBG, 0.1f, Tweener.Tween.EaseNone, "Opacity", "0.7");

            foreach (GameObj obj in GameObjList)
            {
                IAnimateableObj sprite = obj as IAnimateableObj;
                if (sprite != null)
                    sprite.PauseAnimation();

                TextObj textObj = obj as TextObj;
                if (textObj != null && textObj.IsTypewriting == true)
                    textObj.PauseTypewriting();
            }

            foreach (EnemyObj enemy in EnemyList)
                enemy.PauseAnimation();

            foreach (EnemyObj enemy in TempEnemyList)
                enemy.PauseAnimation();
        }

        public virtual void UnpauseRoom()
        {
            //m_pauseBG.Opacity = 0;
            Tweener.Tween.To(m_pauseBG, 0.1f, Tweener.Tween.EaseNone, "Opacity", "0");

            foreach (GameObj obj in GameObjList)
            {
                IAnimateableObj sprite = obj as IAnimateableObj;
                if (sprite != null)
                    sprite.ResumeAnimation();

                TextObj textObj = obj as TextObj;
                if (textObj != null && textObj.IsTypewriting == true)
                    textObj.ResumeTypewriting();
            }

            foreach (EnemyObj enemy in EnemyList)
                enemy.ResumeAnimation();

            foreach (EnemyObj enemy in TempEnemyList)
                enemy.ResumeAnimation();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                foreach (DoorObj door in DoorList)
                    door.Dispose();
                DoorList.Clear();
                DoorList = null;

                foreach (TerrainObj obj in TerrainObjList)
                    obj.Dispose();
                TerrainObjList.Clear();
                TerrainObjList = null;

                foreach (GameObj obj in GameObjList)
                    obj.Dispose();
                GameObjList.Clear();
                GameObjList = null;

                foreach (EnemyObj enemy in EnemyList)
                    enemy.Dispose();
                EnemyList.Clear();
                EnemyList = null;

                foreach (BorderObj borderObj in BorderList)
                    borderObj.Dispose();
                BorderList.Clear();
                BorderList = null;

                m_bwRender = null; // The procedural level screen disposes of this render target.

                LinkedRoom = null;

                foreach (EnemyObj enemy in TempEnemyList)
                    enemy.Dispose();
                TempEnemyList.Clear();
                TempEnemyList = null;

                Player = null;

                m_fairyChestText.Dispose();
                m_fairyChestText = null;

                m_pauseBG.Dispose();
                m_pauseBG = null;

                m_indexText.Dispose();
                m_indexText = null;
                m_roomInfoText.Dispose();
                m_roomInfoText = null;

                Array.Clear(m_empowerCostArray, 0, m_empowerCostArray.Length);
                m_empowerCostArray = null;

                base.Dispose();
            }
        }

        public void CopyRoomProperties(RoomObj room)
        {
            this.Name = room.Name;
            this.SetWidth(room.Width);
            this.SetHeight(room.Height);
            this.Position = room.Position;

            this.AddToCastlePool = room.AddToCastlePool;
            this.AddToGardenPool = room.AddToGardenPool;
            this.AddToDungeonPool = room.AddToDungeonPool;
            this.AddToTowerPool = room.AddToTowerPool;
            this.PoolIndex = room.PoolIndex;

            this.RedEnemyType = room.RedEnemyType;
            this.BlueEnemyType = room.BlueEnemyType;
            this.GreenEnemyType = room.GreenEnemyType;

            this.LinkedRoom = room.LinkedRoom;
            this.IsReversed = room.IsReversed;
            this.DebugRoomPosition = room.DebugRoomPosition;
            this.Tag = room.Tag;
            this.LevelType = room.LevelType;
            this.IsDLCMap = room.IsDLCMap;

            this.TextureColor = room.TextureColor;
        }

        public void CopyRoomObjects(RoomObj room)
        {
            foreach (TerrainObj obj in room.TerrainObjList)
                TerrainObjList.Add(obj.Clone() as TerrainObj);

            foreach (GameObj obj in room.GameObjList)
            {
                GameObj cloneObj = obj.Clone() as GameObj;
                if (cloneObj is NpcObj && cloneObj.Name == "donationbox")
                    DonationBox = cloneObj as NpcObj;
                GameObjList.Add(cloneObj);
            }

            foreach (DoorObj door in room.DoorList)
            {
                DoorObj cloneDoor = door.Clone() as DoorObj;
                cloneDoor.Room = this; // Necessary since a cloned door returns a reference to the old room, NOT the cloned room.
                DoorList.Add(cloneDoor);
            }

            foreach (EnemyObj enemy in room.EnemyList)
                EnemyList.Add(enemy.Clone() as EnemyObj);

            foreach (BorderObj borderObj in room.BorderList)
                BorderList.Add(borderObj.Clone() as BorderObj);
        }

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci)
        {
            base.PopulateFromXMLReader(reader, ci);

            SetWidth(_width);
            SetHeight(_height);
            //if (reader.MoveToAttribute("Width"))
            //    SetWidth(int.Parse(reader.Value, NumberStyles.Any, ci));
            //if (reader.MoveToAttribute("Height"))
            //    SetHeight(int.Parse(reader.Value, NumberStyles.Any, ci));

            if (reader.MoveToAttribute("CastlePool"))
                this.AddToCastlePool = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("GardenPool"))
                this.AddToGardenPool = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("TowerPool"))
                this.AddToTowerPool = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("DungeonPool"))
                this.AddToDungeonPool = bool.Parse(reader.Value);

            this.DebugRoomPosition = this.Position;
        }

        public int RoomNumber
        {
            get { return m_roomNumber; }
            set
            {
                m_roomNumber = value;
                if (this.Name == "Linker")
                    m_indexText.Text = "Linker " + m_roomNumber.ToString();
                else if (this.Name == "EntranceBoss")
                    m_indexText.Text = "Boss\nEnt. " + m_roomNumber.ToString();
                else if (this.Name == "Boss")
                    m_indexText.Text = "Boss " + m_roomNumber.ToString();
                else if (this.Name == "Secret")
                    m_indexText.Text = "Secret " + m_roomNumber.ToString();
                else if (this.Name == "Bonus")
                    m_indexText.Text = "Bonus " + m_roomNumber.ToString();
                else if (this.Name == "Start")
                    m_indexText.Text = "Starting Room";
                else
                    m_indexText.Text = m_roomNumber.ToString();
            }
        }

        public Vector2 DebugRoomPosition
        {
            get { return m_debugRoomPosition; }
            set
            {
                m_debugRoomPosition = value;
                m_roomInfoText.Text = "Level Editor Pos: " + m_debugRoomPosition.ToString() + "\nReversed: " + IsReversed;
            }
        }

        public RenderTarget2D BGRender
        {
            get { return m_bwRender; }
        }

        public bool HasFairyChest
        {
            get
            {
                foreach (GameObj obj in GameObjList)
                {
                    if (obj is FairyChestObj)
                        return true;
                }
                return false;
            }
        }

        public int ActiveEnemies
        {
            get
            {
                int activeEnemies = 0;
                foreach (EnemyObj enemy in EnemyList)
                {
                    if (enemy.NonKillable == false && enemy.IsKilled == false)
                        activeEnemies++;
                }

                foreach (EnemyObj enemy in TempEnemyList)
                {
                    if (enemy.NonKillable == false && enemy.IsKilled == false)
                        activeEnemies++;
                }
                return activeEnemies;
            }
        }

        public virtual void RefreshTextObjs() { }
    }
}

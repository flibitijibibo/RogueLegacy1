using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Tweener;
using InputSystem;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class CarnivalShoot1BonusRoom : BonusRoomObj
    {
        private GameObj m_line;
        private List<BreakableObj> m_targetList;
        private byte m_storedPlayerSpell;
        private int m_daggersThrown = 0;
        private float m_storedPlayerMana;

        private int m_currentTargetIndex;
        private BreakableObj m_currentTarget;
        private bool m_targetMovingUp = false;
        private bool m_isPlayingGame = false;
        private bool m_spokeToNPC = false;

        private NpcObj m_elf;
        private ObjContainer m_daggerIcons;
        private ObjContainer m_targetIcons;

        private int m_numTries = 12;
        private int m_numTargets = 8;
        private float m_targetSpeed = 200f;
        private float m_targetSpeedMod = 100f;

        private List<ObjContainer> m_balloonList;

        private ChestObj m_rewardChest;

        public CarnivalShoot1BonusRoom()
        {
            m_elf = new NpcObj("Clown_Character");
            m_elf.Scale = new Vector2(2, 2);
            m_balloonList = new List<ObjContainer>();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
        {
            m_daggerIcons = new ObjContainer();
            int daggerX = 0;
            int daggerY = 10;
            for (int i = 0; i < m_numTries; i++)
            {
                SpriteObj dagger = new SpriteObj("SpellDagger_Sprite");
                dagger.Scale = new Vector2(2, 2);
                dagger.X = daggerX + 10;
                dagger.Y = daggerY;

                daggerX += dagger.Width;
                if (i == m_numTries / 2 - 1)
                {
                    daggerX = 0;
                    daggerY += 20;
                }

                m_daggerIcons.AddChild(dagger);
            }
            m_daggerIcons.OutlineWidth = 2;

            m_targetIcons = new ObjContainer();
            for (int i = 0; i < m_numTargets; i++)
            {
                SpriteObj target = new SpriteObj("Target2Piece1_Sprite");
                target.Scale = new Vector2(2, 2);
                target.X += i * (target.Width + 10);
                m_targetIcons.AddChild(target);
            }
            m_targetIcons.OutlineWidth = 2;

            GameObjList.Add(m_targetIcons);
            GameObjList.Add(m_daggerIcons);

            base.LoadContent(graphics);
        }

        public override void Initialize()
        {
            Color[] randomColours = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Purple, Color.Pink, Color.MediumTurquoise, Color.CornflowerBlue };

            foreach (GameObj obj in this.GameObjList)
            {
                if (obj is WaypointObj)
                    m_elf.X = obj.X;

                if (obj.Name == "Line")
                    m_line = obj;

                if (obj.Name == "Balloon")
                {
                    m_balloonList.Add(obj as ObjContainer);
                    (obj as ObjContainer).GetChildAt(1).TextureColor = randomColours[CDGMath.RandomInt(0, randomColours.Length - 1)];
                }
            }

            float floorY = 0;

            foreach (TerrainObj terrain in TerrainObjList)
            {
                if (terrain.Name == "Floor")
                {
                    m_elf.Y = terrain.Y - (m_elf.Bounds.Bottom - m_elf.Y);
                    floorY = terrain.Y;
                    break;
                }
            }

            if (IsReversed == false)
                m_elf.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            GameObjList.Add(m_elf);
            m_elf.Y -= 2;

            m_targetList = new List<BreakableObj>();
            for (int i = 0; i < m_numTargets; i++)
            {
                BreakableObj breakableObj = new BreakableObj("Target1_Character");
                breakableObj.Scale = new Vector2(2, 2);
                breakableObj.Visible = false;
                breakableObj.DropItem = false;
                breakableObj.HitBySpellsOnly = true;
                breakableObj.Position = m_line.Position;
                if (this.IsReversed == false)
                    breakableObj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                else
                    breakableObj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                m_targetList.Add(breakableObj);
                GameObjList.Add(breakableObj);
            }

            m_rewardChest = new ChestObj(null);
            m_rewardChest.ChestType = ChestType.Gold;
            if (this.IsReversed == false)
            {
                m_rewardChest.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                m_rewardChest.Position = new Vector2(m_elf.X + 100, floorY - m_rewardChest.Height - 8);
            }
            else
                m_rewardChest.Position = new Vector2(m_elf.X - 150, floorY - m_rewardChest.Height - 8);
            m_rewardChest.Visible = false;
            GameObjList.Add(m_rewardChest);

            base.Initialize();
        }

        public override void OnEnter()
        {
            // Force this chest to be gold.
            m_rewardChest.ChestType = ChestType.Gold;

            // This shouldn't be needed since the chest is added to the GameObjList
            if (m_rewardChest.PhysicsMngr == null)
                Player.PhysicsMngr.AddObject(m_rewardChest);

            m_spokeToNPC = false;
            Player.AttachedLevel.CameraLockedToPlayer = false;
            if (IsReversed == false)
                Player.AttachedLevel.Camera.Position = new Vector2(this.Bounds.Left + Player.AttachedLevel.Camera.Width / 2, this.Bounds.Top + Player.AttachedLevel.Camera.Height / 2);
            else
                Player.AttachedLevel.Camera.Position = new Vector2(this.Bounds.Right - Player.AttachedLevel.Camera.Width / 2, this.Bounds.Top + Player.AttachedLevel.Camera.Height / 2);

            m_currentTargetIndex = 0;
            m_daggersThrown = 0;
            m_storedPlayerMana = Player.CurrentMana;
            m_storedPlayerSpell = Game.PlayerStats.Spell;

            InitializeTargetSystem();

            if (this.IsReversed == false)
            {
                m_targetIcons.Position = new Vector2(this.Bounds.Right - 100 - m_targetIcons.Width, this.Bounds.Bottom - 40);
                m_daggerIcons.Position = m_targetIcons.Position;
                m_daggerIcons.X -= 400 + m_daggerIcons.Width;
            }
            else
            {
                m_targetIcons.Position = new Vector2(this.Bounds.Left + 150, this.Bounds.Bottom - 40);
                m_daggerIcons.Position = m_targetIcons.Position;
                m_daggerIcons.X += m_targetIcons.Width + 400;
            }
            m_daggerIcons.Y -= 30;

            ReflipPosters();
            base.OnEnter();
        }

        private void ReflipPosters()
        {
            foreach (GameObj obj in GameObjList)
            {
                SpriteObj sprite = obj as SpriteObj;
                if (sprite != null && sprite.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally)
                {
                    if (sprite.SpriteName == "CarnivalPoster1_Sprite" || sprite.SpriteName == "CarnivalPoster2_Sprite" || sprite.SpriteName == "CarnivalPoster3_Sprite"
                        || sprite.SpriteName == "CarnivalTent_Sprite")
                        sprite.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                }
            }
        }

        public void BeginGame()
        {
            Player.AttachedLevel.ProjectileManager.DestroyAllProjectiles(true);
            Player.StopAllSpells();
            m_isPlayingGame = true;
            m_spokeToNPC = true;

            Player.AttachedLevel.CameraLockedToPlayer = false;
            Player.AttachedLevel.Camera.Y = this.Bounds.Center.Y;
            if (this.IsReversed == false)
                Tween.To(Player.AttachedLevel.Camera, 1, Tweener.Ease.Quad.EaseInOut, "X", (m_line.X + 500).ToString());
            else
                Tween.To(Player.AttachedLevel.Camera, 1, Tweener.Ease.Quad.EaseInOut, "X", (m_line.X - 500).ToString());
            EquipPlayer();
            ActivateTarget();
        }

        public void EndGame()
        {
            if (this.IsReversed == false)
                Tween.To(Player.AttachedLevel.Camera, 1, Tweener.Ease.Quad.EaseInOut, "X", (this.X + Player.AttachedLevel.Camera.Width/2f).ToString());
            else
                Tween.To(Player.AttachedLevel.Camera, 1, Tweener.Ease.Quad.EaseInOut, "X", (this.Bounds.Right - Player.AttachedLevel.Camera.Width/2f).ToString());

            Tween.AddEndHandlerToLastTween(this, "CheckPlayerReward");
            m_isPlayingGame = false;
            Game.PlayerStats.Spell = SpellType.None;
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            this.RoomCompleted = true;
        }

        public void CheckPlayerReward()
        {
            if (ActiveTargets <= 0)
            {
                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("CarnivalRoom1-Reward");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
                RevealChest();
                GameUtil.UnlockAchievement("LOVE_OF_CLOWNS");
            }
            else
            {
                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("CarnivalRoom1-Fail");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
            }
        }

        public void RevealChest()
        {
            Player.AttachedLevel.ImpactEffectPool.DisplayDeathEffect(m_rewardChest.Position);
            m_rewardChest.Visible = true;
        }

        private void InitializeTargetSystem()
        {
            foreach (BreakableObj obj in m_targetList)
            {
                obj.Reset();
                obj.Visible = false;
                if (this.IsReversed == false)
                {
                    obj.Position = new Vector2(this.Bounds.Right, this.Bounds.Center.Y);
                    obj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                }
                else
                {
                    obj.Position = new Vector2(this.Bounds.Left, this.Bounds.Center.Y);
                    obj.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                }
            }
        }

        private void EquipPlayer()
        {
            Game.PlayerStats.Spell = SpellType.Dagger;
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.CurrentMana = Player.MaxMana;
        }

        public void UnequipPlayer()
        {
            Game.PlayerStats.Spell = m_storedPlayerSpell;
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.CurrentMana = m_storedPlayerMana;
        }

        public override void OnExit()
        {
            UnequipPlayer();
            Player.AttachedLevel.CameraLockedToPlayer = true;
            base.OnExit();
        }

        private void HandleInput()
        {
            if (m_isPlayingGame == true)
            {
                if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_SPELL1) || (Game.GlobalInput.JustPressed(InputMapType.PLAYER_ATTACK) && Game.PlayerStats.Class== ClassType.Dragon)) 
                    && Player.SpellCastDelay <= 0)
                {
                    m_daggersThrown++;
                    Player.CurrentMana = Player.MaxMana;
                    if (m_daggersThrown <= m_numTries)
                        m_daggerIcons.GetChildAt(m_numTries - m_daggersThrown).Visible = false;
                    if (m_daggersThrown > m_numTries)
                        Game.PlayerStats.Spell = SpellType.None;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            m_elf.Update(gameTime, Player);

            // Bounds to prevent the player from moving past.
            if (this.IsReversed == false)
            {
                if (Player.X >= m_line.X - 150)
                    Player.X = (int)m_line.X - 150;
            }
            else
            {
                if (Player.X < m_line.X + 150)
                    Player.X = m_line.X + 150;
            }

            if (this.IsReversed == false)
            {
                if (m_isPlayingGame == true)
                {
                    if (Player.X < Player.AttachedLevel.Camera.Bounds.Left)
                        Player.X = Player.AttachedLevel.Camera.Bounds.Left;
                }
                if (Player.X > this.Bounds.Right - 1320)
                    Player.X = this.Bounds.Right - 1320;
            }
            else
            {
                if (m_isPlayingGame == true)
                {
                    if (Player.X > Player.AttachedLevel.Camera.Bounds.Right)
                        Player.X = Player.AttachedLevel.Camera.Bounds.Right;
                }
                if (Player.X < this.Bounds.Left + 1320)
                    Player.X = this.Bounds.Left + 1320;
            }

            // Target active logic.
            if (m_currentTarget != null && m_currentTarget.Broken == false)
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                
                // Moving up logic.
                if (m_targetMovingUp == true && m_currentTarget.Bounds.Top > this.Bounds.Top + 80) //  a little bigger than 60 so it doesn't get stuck.
                    m_currentTarget.Y -= elapsedTime * m_targetSpeed;
                else if (m_targetMovingUp == true)
                {
                    m_currentTarget.Y += elapsedTime * m_targetSpeed;
                    m_targetMovingUp = false;
                }

                // Moving down logic.
                if (m_targetMovingUp == false && m_currentTarget.Bounds.Bottom < this.Bounds.Bottom - 140)
                    m_currentTarget.Y += elapsedTime * m_targetSpeed;
                else if (m_targetMovingUp == false)
                {
                    m_currentTarget.Y -= elapsedTime * m_targetSpeed;
                    m_targetMovingUp = true;
                }
            }

            if (m_isPlayingGame == true)
            {
                if ((m_daggersThrown >= m_numTries && Player.AttachedLevel.ProjectileManager.ActiveProjectiles < 1 && ActiveTargets > 0) || ActiveTargets <= 0)
                    EndGame();
            }

            // Code that sets up the next target.
            if (m_currentTarget != null && m_currentTarget.Broken == true && ActiveTargets >= 0)
            {
                m_currentTargetIndex++;
                ActivateTarget();
            }

            // Elf code.
            if (m_elf.IsTouching == true && RoomCompleted == false && m_spokeToNPC == false)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                    manager.DialogueScreen.SetDialogue("CarnivalRoom1-Start");
                    manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                    manager.DialogueScreen.SetConfirmEndHandler(this, "BeginGame");
                    manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
                }
            }
            else if (m_elf.IsTouching == true && RoomCompleted == true)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                    manager.DialogueScreen.SetDialogue("CarnivalRoom1-End");
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
                }
            }

            if (m_isPlayingGame == true)
                m_elf.CanTalk = false;
            else
                m_elf.CanTalk = true;

            // Rotating the balloons.
            float totalSeconds = Game.TotalGameTime;
            float rotationOffset = 2f;
            foreach (ObjContainer balloon in m_balloonList)
            {
                balloon.Rotation = (float)Math.Sin(totalSeconds * rotationOffset) * rotationOffset;
                rotationOffset += 0.2f;
            }

            HandleInput(); // This has to go after everything.

            base.Update(gameTime);
        }

        public void ActivateTarget()
        {
            // Target has been destroyed
            if (m_numTargets - m_currentTargetIndex < m_targetIcons.NumChildren)
            {
                m_targetIcons.GetChildAt(m_numTargets - m_currentTargetIndex).Visible = false;
                GiveGold();
            }

            if (m_currentTargetIndex < m_numTargets)
            {
                if (m_currentTarget != null)
                    m_targetSpeed += m_targetSpeedMod;
                m_currentTarget = m_targetList[m_currentTargetIndex];
                m_currentTarget.Visible = true;

                if (this.IsReversed == false)
                    Tween.By(m_currentTarget, 2, Tweener.Ease.Quad.EaseOut, "X", (-400 + CDGMath.RandomInt(-200, 200)).ToString());
                else
                    Tween.By(m_currentTarget, 2, Tweener.Ease.Quad.EaseOut, "X", (400 + CDGMath.RandomInt(-200, 200)).ToString());
            }
            else
                m_currentTarget = null;
        }

        public void GiveGold()
        {
            int numCoins = m_numTargets - ActiveTargets;
            if (ActiveTargets > 0)
                Player.AttachedLevel.ImpactEffectPool.CarnivalGoldEffect(m_currentTarget.Position, new Vector2(Player.AttachedLevel.Camera.TopLeftCorner.X + 50, Player.AttachedLevel.Camera.TopLeftCorner.Y + 135), numCoins);
            Player.AttachedLevel.TextManager.DisplayNumberStringText(numCoins * 10, "LOC_ID_CARNIVAL_BONUS_ROOM_4" /*"gold"*/, Color.Yellow, m_currentTarget.Position);
            Game.PlayerStats.Gold += numCoins * 10;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_targetList.Clear();
                m_targetList = null;
                m_line = null;
                m_currentTarget = null;
                m_elf = null;
                m_daggerIcons = null;
                m_targetIcons = null;

                m_balloonList.Clear();
                m_balloonList = null;

                m_rewardChest = null;
                base.Dispose();
            }
        }

        private int ActiveTargets
        {
            get
            {
                int activeTargets = 0;
                foreach (BreakableObj obj in m_targetList)
                {
                    if (obj.Broken == false)
                        activeTargets++;
                }
                return activeTargets;
            }
        }
    }
}

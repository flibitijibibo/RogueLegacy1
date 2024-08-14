using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tweener;
using DS2DEngine;
using Tweener.Ease;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastle
{
    public class CarnivalShoot2BonusRoom: BonusRoomObj
    {
        private int m_numTries = 5;

        private Rectangle m_targetBounds;
        private byte m_storedPlayerSpell;
        private float m_storedPlayerMana;
        private bool m_isPlayingGame;
        private int m_axesThrown;

        private List<BreakableObj> m_targetList;
        private ObjContainer m_axeIcons;
        private NpcObj m_elf;
        private PhysicsObj m_gate;
        private bool m_gateClosed;

        private List<TextObj> m_targetDataText;
        private List<TextObj> m_targetText;

        private List<ObjContainer> m_balloonList;

        private ChestObj m_rewardChest;

        public CarnivalShoot2BonusRoom()
        {
            m_targetList = new List<BreakableObj>();
            m_elf = new NpcObj("Clown_Character");
            m_elf.Scale = new Vector2(2, 2);

            m_targetText = new List<TextObj>();
            m_targetDataText = new List<TextObj>();
            m_balloonList = new List<ObjContainer>();
        }

        public override void LoadContent(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphics)
        {
            TextObj genericText = new TextObj(Game.JunicodeFont);
            genericText.FontSize = 25;
            genericText.Text = "test text"; // placeholder text
            genericText.DropShadow = new Vector2(2, 2);
            if (this.IsReversed == false)
                genericText.Position = new Vector2(this.Bounds.Right - 1000, this.Bounds.Top + 200);
            else
                genericText.Position = new Vector2(this.Bounds.Left + 300, this.Bounds.Top + 200);

            for (int i = 0; i < 3; i++)
            {
                TextObj text = genericText.Clone() as TextObj;
                text.Text = "";// LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", text); // dummy locID to add TextObj to language refresh list
                text.Y += i * 100;
                m_targetText.Add(text);

                TextObj data = genericText.Clone() as TextObj;
                data.Text = "0";// LocaleBuilder.getString("LOC_ID_CLASS_NAME_1_MALE", data); // dummy locID to add TextObj to language refresh list
                data.Y += i * 100;
                data.X = text.X + 500;

                m_targetDataText.Add(data);
            }


            m_axeIcons = new ObjContainer();
            int daggerX = 0;
            int daggerY = 10;
            for (int i = 0; i < m_numTries; i++)
            {
                SpriteObj dagger = new SpriteObj("SpellAxe_Sprite");
                dagger.Scale = new Vector2(2, 2);
                dagger.X = daggerX + 10;
                dagger.Y = daggerY;

                daggerX += dagger.Width + 5;

                m_axeIcons.AddChild(dagger);
            }
            m_axeIcons.OutlineWidth = 2;
            this.GameObjList.Add(m_axeIcons);
            base.LoadContent(graphics);
        }

        public override void Initialize()
        {
            m_gate = new PhysicsObj("CastleEntranceGate_Sprite");
            m_gate.IsWeighted = false;
            m_gate.IsCollidable = true;
            m_gate.CollisionTypeTag = GameTypes.CollisionType_WALL;
            m_gate.Layer = -1;
            m_gate.OutlineWidth = 2;
            this.GameObjList.Add(m_gate);

            Rectangle gatePosition = new Rectangle();

            Color[] randomColours = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.Orange, Color.Purple, Color.Pink, Color.MediumTurquoise, Color.CornflowerBlue };

            foreach (GameObj obj in this.GameObjList)
            {
                if (obj is WaypointObj)
                    m_elf.X = obj.X;

                if (obj.Name == "Balloon")
                {
                    m_balloonList.Add(obj as ObjContainer);
                    (obj as ObjContainer).GetChildAt(1).TextureColor = randomColours[CDGMath.RandomInt(0, randomColours.Length - 1)];
                }
            }

            // Positioning gate.
            foreach (TerrainObj terrain in TerrainObjList)
            {
                if (terrain.Name == "Floor")
                    m_elf.Y = terrain.Y - (m_elf.Bounds.Bottom - m_elf.Y) - 2;

                if (terrain.Name == "GatePosition")
                    gatePosition = terrain.Bounds;
            }

            m_gate.X = gatePosition.X;
            m_gate.Y = gatePosition.Bottom;

            if (IsReversed == false)
                m_elf.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
            GameObjList.Add(m_elf);

            foreach (TerrainObj obj in TerrainObjList)
            {
                if (obj.Name == "Boundary")
                {
                    obj.Visible = false;
                    m_targetBounds = obj.Bounds;
                    break;
                }
            }

            float arraySize = 10;
            float startingX = m_targetBounds.X + 40;
            float startingY = m_targetBounds.Y;
            float xDiff = m_targetBounds.Width / arraySize;
            float yDiff = m_targetBounds.Height / arraySize;
            for (int i = 0; i < arraySize * arraySize; i++)
            {
                BreakableObj target = new BreakableObj("Target2_Character");
                target.X = startingX;
                target.Y = startingY;
                target.Scale = new Vector2(1.6f, 1.6f);
                target.OutlineWidth = 2;
                target.HitBySpellsOnly = true;
                target.IsWeighted = false;
                m_targetList.Add(target);
                target.SameTypesCollide = false;
                target.DropItem = false;
                GameObjList.Add(target);
                startingX += xDiff;

                if ((i + 1) % arraySize == 0)
                {
                    startingX = m_targetBounds.X + 40;
                    startingY += yDiff;
                }
            }

            m_rewardChest = new ChestObj(null);
            m_rewardChest.ChestType = ChestType.Gold;
            if (this.IsReversed == false)
            {
                m_rewardChest.Flip = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                m_rewardChest.Position = new Vector2(m_elf.X + 100, m_elf.Bounds.Bottom - m_rewardChest.Height - 8);
            }
            else
                m_rewardChest.Position = new Vector2(m_elf.X - 150, m_elf.Bounds.Bottom - m_rewardChest.Height - 8);
            m_rewardChest.Visible = false;
            GameObjList.Add(m_rewardChest);

            base.Initialize();
        }

        public override void OnEnter()
        {
            // Force this chest to be gold.
            m_rewardChest.ChestType = ChestType.Gold;

            if (IsReversed == false)
                m_axeIcons.Position = new Vector2(this.Bounds.Right - 200 - m_axeIcons.Width, this.Bounds.Bottom -60);
            else
                m_axeIcons.Position = new Vector2(this.Bounds.Left + 900, this.Bounds.Bottom - 60);

            try
            {
                m_targetText[0].ChangeFontNoDefault(m_targetText[0].defaultFont);
                m_targetText[1].ChangeFontNoDefault(m_targetText[1].defaultFont);
                m_targetText[2].ChangeFontNoDefault(m_targetText[2].defaultFont);
                m_targetText[0].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_1") + ":"; //"Targets Destroyed:"
                m_targetText[1].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_2") + ":"; //"Targets Remaining:"
                m_targetText[2].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_3") + ":"; //"Reward:"
            }
            catch
            {
                m_targetText[0].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetText[1].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetText[2].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetText[0].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_1") + ":"; //"Targets Destroyed:"
                m_targetText[1].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_2") + ":"; //"Targets Remaining:"
                m_targetText[2].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_3") + ":"; //"Reward:"
            }

            m_targetDataText[0].Text = "50"; // placeholder text
            m_targetDataText[1].Text = "10"; // placeholder text
            m_targetDataText[2].Text = "100 gold"; // placeholder text

            for (int i = 0; i < m_targetText.Count; i++)
            {
                m_targetText[i].Opacity = 0;
                m_targetDataText[i].Opacity = 0;
            }

            foreach (BreakableObj obj in m_targetList)
            {
                obj.Opacity = 0;
                obj.Visible = false;
            }

            m_gateClosed = true;
            m_storedPlayerSpell = Game.PlayerStats.Spell;
            m_storedPlayerMana = Player.CurrentMana;

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

        public override void OnExit()
        {
            Game.PlayerStats.Spell = m_storedPlayerSpell;
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.CurrentMana = m_storedPlayerMana;
            base.OnExit();
        }

        public void BeginGame()
        {
            Player.AttachedLevel.ProjectileManager.DestroyAllProjectiles(true);
            Player.StopAllSpells();
            m_gateClosed = false;
            Tween.By(m_gate, 0.5f, Quad.EaseInOut, "Y", (-(m_gate.Height)).ToString());
            m_isPlayingGame = true;
            EquipPlayer();

            float delay = 0;
            foreach (BreakableObj obj in m_targetList)
            {
                obj.Visible = true;
                Tween.To(obj, 0.5f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
                delay += 0.01f;
            }
        }

        private void EndGame()
        {
            Player.LockControls();
            Player.CurrentSpeed = 0;
            foreach (BreakableObj obj in m_targetList)
                Tween.To(obj, 0.5f, Tween.EaseNone, "Opacity", "0");
            Tween.AddEndHandlerToLastTween(this, "EndGame2");

            m_isPlayingGame = false;
        }

        public void EndGame2()
        {
            int gold = (int)(TargetsDestroyed / 2f) * 10;

            try
            {
                m_targetDataText[0].ChangeFontNoDefault(m_targetDataText[0].defaultFont);
                m_targetDataText[1].ChangeFontNoDefault(m_targetDataText[1].defaultFont);
                m_targetDataText[2].ChangeFontNoDefault(m_targetDataText[2].defaultFont);
                m_targetDataText[0].Text = TargetsDestroyed.ToString();
                m_targetDataText[1].Text = TargetsRemaining.ToString();
                m_targetDataText[2].Text = gold + " " + LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_4"); //"gold"
            }
            catch
            {
                m_targetDataText[0].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetDataText[1].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetDataText[2].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetDataText[0].Text = TargetsDestroyed.ToString();
                m_targetDataText[1].Text = TargetsRemaining.ToString();
                m_targetDataText[2].Text = gold + " " + LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_4"); //"gold"
            }

            float delay = 0;
            for (int i = 0; i < m_targetDataText.Count; i++)
            {
                Tween.To(m_targetText[i], 0.5f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
                Tween.To(m_targetDataText[i], 0.5f, Tween.EaseNone, "delay", delay.ToString(), "Opacity", "1");
                delay += 0.5f;
            }

            Tween.AddEndHandlerToLastTween(this, "GiveReward", gold);
        }

        public void GiveReward(int gold)
        {
            if ((this.IsReversed == false && Player.X < this.Bounds.Right - Player.AttachedLevel.Camera.Width / 2f) || (this.IsReversed == true && Player.X > this.Bounds.Left + Player.AttachedLevel.Camera.Width / 2f))
            {
                Tween.To(Player.AttachedLevel.Camera, 0.5f, Quad.EaseInOut, "X", Player.X.ToString());
                Tween.AddEndHandlerToLastTween(this, "ResetCamera");
            }
            else
                this.ResetCamera();

            Player.AttachedLevel.TextManager.DisplayNumberStringText(gold, "LOC_ID_CARNIVAL_BONUS_ROOM_4" /*"gold"*/, Color.Yellow, Player.Position);
            Game.PlayerStats.Gold += gold;
            Tween.By(m_gate, 0.5f, Quad.EaseInOut, "Y", (-(m_gate.Height)).ToString());
            m_gateClosed = false;
            this.RoomCompleted = true;

            Tween.AddEndHandlerToLastTween(this, "CheckPlayerReward");
        }


        public void CheckPlayerReward()
        {
            if (TargetsRemaining <= 10)
            {
                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("CarnivalRoom2-Reward");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
                RevealChest();
                GameUtil.UnlockAchievement("LOVE_OF_CLOWNS");
            }
            else
            {
                RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                manager.DialogueScreen.SetDialogue("CarnivalRoom2-Fail");
                (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true, null);
            }
        }

        public void RevealChest()
        {
            Player.AttachedLevel.ImpactEffectPool.DisplayDeathEffect(m_rewardChest.Position);
            m_rewardChest.Visible = true;
        }

        public void ResetCamera()
        {
            Player.UnlockControls();
            Player.AttachedLevel.CameraLockedToPlayer = true;
        }


        private void EquipPlayer()
        {
            Game.PlayerStats.Spell = SpellType.Axe;
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.CurrentMana = Player.MaxMana;
        }

        public void UnequipPlayer()
        {
            Game.PlayerStats.Spell = m_storedPlayerSpell;
            Player.AttachedLevel.UpdatePlayerSpellIcon();
            Player.CurrentMana = m_storedPlayerMana;
        }

        public override void Update(GameTime gameTime)
        {
            if ((m_axesThrown >= m_numTries && m_isPlayingGame == true && Player.AttachedLevel.ProjectileManager.ActiveProjectiles < 1) ||(m_isPlayingGame == true && TargetsDestroyed >= 100))
                EndGame();

            if (m_isPlayingGame == true && m_gateClosed == false)
            {
                if ((this.IsReversed == false && Player.X > m_gate.Bounds.Right) || (this.IsReversed == true && Player.X < m_gate.Bounds.Left))
                {
                    Player.LockControls();
                    Player.CurrentSpeed = 0;
                    Player.AccelerationX = 0;
                    Tween.By(m_gate, 0.5f, Quad.EaseInOut, "Y", m_gate.Height.ToString());
                    Tween.AddEndHandlerToLastTween(Player, "UnlockControls");
                    m_gateClosed = true;
                    Player.AttachedLevel.CameraLockedToPlayer = false;
                    if (this.IsReversed == false)
                        Tween.To(Player.AttachedLevel.Camera, 1f, Quad.EaseInOut, "X", (this.Bounds.Right - Player.AttachedLevel.Camera.Width / 2f).ToString());
                    else
                        Tween.To(Player.AttachedLevel.Camera, 1f, Quad.EaseInOut, "X", (this.Bounds.Left + Player.AttachedLevel.Camera.Width / 2f).ToString());
                }
            }

            // Elf code.
            m_elf.Update(gameTime, Player);

            if (m_isPlayingGame == true)
                m_elf.CanTalk = false;
            else
                m_elf.CanTalk = true;

            if (m_elf.IsTouching == true && RoomCompleted == false && m_isPlayingGame == false)
            {
                if (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2))
                {
                    RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                    manager.DialogueScreen.SetDialogue("CarnivalRoom2-Start");
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

            if (IsReversed == false && m_isPlayingGame == true && Player.X < this.Bounds.Left + 10)
                Player.X = this.Bounds.Left + 10;
            else if (IsReversed == true && m_isPlayingGame == true && Player.X > this.Bounds.Right - 10)
                Player.X = this.Bounds.Right - 10;

            // Rotating the balloons.
            float totalSeconds = Game.TotalGameTime;
            float rotationOffset = 2f;
            foreach (ObjContainer balloon in m_balloonList)
            {
                balloon.Rotation = (float)Math.Sin(totalSeconds * rotationOffset) * rotationOffset;
                rotationOffset += 0.2f;
            }

            HandleInput();
            base.Update(gameTime);
        }

        private void HandleInput()
        {
            if (m_isPlayingGame == true)
            {
                if ((Game.GlobalInput.JustPressed(InputMapType.PLAYER_SPELL1) || (Game.GlobalInput.JustPressed(InputMapType.PLAYER_ATTACK) && Game.PlayerStats.Class== ClassType.Dragon)) 
                    && Player.SpellCastDelay <= 0 && m_gateClosed == true)
                {
                    m_axesThrown++;
                    Player.CurrentMana = Player.MaxMana;
                    if (m_axesThrown <= m_numTries)
                        m_axeIcons.GetChildAt(m_numTries - m_axesThrown).Visible = false;
                    if (m_axesThrown > m_numTries)
                        Game.PlayerStats.Spell = SpellType.None;
                }
            }
        }

        public override void Draw(Camera2D camera)
        {
            base.Draw(camera);
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            for (int i = 0; i < m_targetText.Count; i++)
            {
                m_targetText[i].Draw(camera);
                m_targetDataText[i].Draw(camera);
            }
            camera.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_targetList.Clear();
                m_targetList = null;
                m_elf = null;
                m_axeIcons = null;
                m_gate = null;

                for (int i = 0; i < m_targetText.Count; i++)
                {
                    m_targetText[i].Dispose();
                    m_targetDataText[i].Dispose();
                }
                m_targetText.Clear();
                m_targetText = null;
                m_targetDataText.Clear();
                m_targetDataText = null;
                m_balloonList.Clear();
                m_balloonList = null;
                m_rewardChest = null;
                base.Dispose();
            }
        }

        public int TargetsDestroyed
        {
            get
            {
                int destroyed = 0;
                foreach (BreakableObj obj in m_targetList)
                {
                    if (obj.Broken == true)
                        destroyed++;
                }
                return destroyed;
            }
        }

        public int TargetsRemaining
        {
            get
            {
                int remaining = 0;
                foreach (BreakableObj obj in m_targetList)
                {
                    if (obj.Broken == false)
                        remaining++;
                }
                return remaining;
            }
        }

        public override void RefreshTextObjs()
        {
            m_targetText[0].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_targetText[0]));
            m_targetText[1].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_targetText[1]));
            m_targetText[2].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_targetText[2]));

            m_targetDataText[0].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_targetDataText[0]));
            m_targetDataText[1].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_targetDataText[1]));
            m_targetDataText[2].ChangeFontNoDefault(LocaleBuilder.GetLanguageFont(m_targetDataText[2]));


            try
            {
                m_targetText[0].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_1") + ":"; //"Targets Destroyed:"
                m_targetText[1].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_2") + ":"; //"Targets Remaining:"
                m_targetText[2].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_3") + ":"; //"Reward:"

                int gold = (int)(TargetsDestroyed / 2f) * 10;
                m_targetDataText[0].Text = TargetsDestroyed.ToString();
                m_targetDataText[1].Text = TargetsRemaining.ToString();
                m_targetDataText[2].Text = gold + " " + LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_4"); //"gold"
            }
            catch
            {
                m_targetText[0].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetText[1].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetText[2].ChangeFontNoDefault(Game.NotoSansSCFont);

                m_targetText[0].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_1") + ":"; //"Targets Destroyed:"
                m_targetText[1].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_2") + ":"; //"Targets Remaining:"
                m_targetText[2].Text = LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_3") + ":"; //"Reward:"

                m_targetDataText[0].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetDataText[1].ChangeFontNoDefault(Game.NotoSansSCFont);
                m_targetDataText[2].ChangeFontNoDefault(Game.NotoSansSCFont);

                int gold = (int)(TargetsDestroyed / 2f) * 10;
                m_targetDataText[0].Text = TargetsDestroyed.ToString();
                m_targetDataText[1].Text = TargetsRemaining.ToString();
                m_targetDataText[2].Text = gold + " " + LocaleBuilder.getResourceString("LOC_ID_CARNIVAL_BONUS_ROOM_4"); //"gold"
            }

            base.RefreshTextObjs();
        }
    }
}

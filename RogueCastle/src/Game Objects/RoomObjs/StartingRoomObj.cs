using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using InputSystem;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Tweener.Ease;
using Tweener;
using Microsoft.Xna.Framework.Audio;

namespace RogueCastle
{
    public class StartingRoomObj : RoomObj
    {
        private BlacksmithObj m_blacksmith;
        private SpriteObj m_blacksmithIcon;
        private Vector2 m_blacksmithIconPosition;

        private const int ENCHANTRESS_HEAD_LAYER = 4;
        private ObjContainer m_enchantress;
        private SpriteObj m_enchantressIcon;
        private Vector2 m_enchantressIconPosition;

        private const byte ARCHITECT_HEAD_LAYER = 1;
        private ObjContainer m_architect;
        private SpriteObj m_architectIcon;
        private Vector2 m_architectIconPosition;
        private bool m_architectRenovating;
        private float m_screenShakeCounter = 0;

        private FrameSoundObj m_blacksmithAnvilSound;
        private GameObj m_tree1, m_tree2, m_tree3;
        private GameObj m_fern1, m_fern2, m_fern3;

        private bool m_isRaining = false;
        private List<RaindropObj> m_rainFG;
        private Cue m_rainSFX;

        private SpriteObj m_tent;
        private SpriteObj m_blacksmithBoard;
        private SpriteObj m_screw;

        private PhysicsObjContainer m_tollCollector;
        private SpriteObj m_tollCollectorIcon;

        private bool m_playerWalkedOut = false;

        private SpriteObj m_mountain1, m_mountain2;
        private float m_lightningTimer = 0;

        private SpriteObj m_blacksmithNewIcon;
        private SpriteObj m_enchantressNewIcon;

        private TerrainObj m_blacksmithBlock;
        private TerrainObj m_enchantressBlock;
        private TerrainObj m_architectBlock;

        private bool m_controlsLocked = false;

        private bool m_isSnowing = false;

        public StartingRoomObj()
        {
            //TraitSystem.LevelUpTrait(TraitSystem.GetTrait(TraitType.Smithy)); // Debug just so I can see the smithy.
            //TraitSystem.LevelUpTrait(TraitSystem.GetTrait(TraitType.Enchanter)); // Debug just so I can see the enchantress.
            //TraitSystem.LevelUpTrait(TraitSystem.GetTrait(TraitType.Architect)); // Debug just so I can see the architect.

            m_blacksmith = new BlacksmithObj();
            m_blacksmith.Flip = SpriteEffects.FlipHorizontally;
            m_blacksmith.Scale = new Vector2(2.5f, 2.5f);
            m_blacksmith.Position = new Vector2(700, 720 - 60 - (m_blacksmith.Bounds.Bottom - m_blacksmith.Y) - 1); // -60 to subtract one tile.
            m_blacksmith.OutlineWidth = 2;

            m_blacksmithBoard = new SpriteObj("StartRoomBlacksmithBoard_Sprite");
            m_blacksmithBoard.Scale = new Vector2(2, 2);
            m_blacksmithBoard.OutlineWidth = 2;
            m_blacksmithBoard.Position = new Vector2(m_blacksmith.X - m_blacksmithBoard.Width / 2 - 35, m_blacksmith.Bounds.Bottom - m_blacksmithBoard.Height - 1);

            //m_blacksmithIcon = new SpriteObj("TalkBubbleUpArrow_Sprite");
            m_blacksmithIcon = new SpriteObj("UpArrowBubble_Sprite");
            m_blacksmithIcon.Scale = new Vector2(2, 2);
            m_blacksmithIcon.Visible = false;
            m_blacksmithIconPosition = new Vector2(m_blacksmith.X - 60, m_blacksmith.Y - 10);
            m_blacksmithIcon.Flip = m_blacksmith.Flip;
            m_blacksmithIcon.OutlineWidth = 2;

            //m_blacksmithNewIcon = new SpriteObj("TalkBubble2_Sprite");
            m_blacksmithNewIcon = new SpriteObj("ExclamationSquare_Sprite");
            m_blacksmithNewIcon.Visible = false;
            m_blacksmithNewIcon.OutlineWidth = 2;
            m_enchantressNewIcon = m_blacksmithNewIcon.Clone() as SpriteObj;

            m_enchantress = new ObjContainer("Enchantress_Character");
            m_enchantress.Scale = new Vector2(2f, 2f);
            m_enchantress.Flip = SpriteEffects.FlipHorizontally;
            m_enchantress.Position = new Vector2(1150, 720 - 60 - (m_enchantress.Bounds.Bottom - m_enchantress.AnchorY) - 2);
            m_enchantress.PlayAnimation();
            m_enchantress.AnimationDelay = 1 / 10f;
            (m_enchantress.GetChildAt(ENCHANTRESS_HEAD_LAYER) as IAnimateableObj).StopAnimation();
            m_enchantress.OutlineWidth = 2;

            m_tent = new SpriteObj("StartRoomGypsyTent_Sprite");
            m_tent.Scale = new Vector2(1.5f, 1.5f);
            m_tent.OutlineWidth = 2;
            m_tent.Position = new Vector2(m_enchantress.X - m_tent.Width / 2 + 5, m_enchantress.Bounds.Bottom - m_tent.Height);

            //m_enchantressIcon = new SpriteObj("TalkBubbleUpArrow_Sprite");
            m_enchantressIcon = new SpriteObj("UpArrowBubble_Sprite");
            m_enchantressIcon.Scale = new Vector2(2f, 2f);
            m_enchantressIcon.Visible = false;
            m_enchantressIconPosition = new Vector2(m_enchantress.X - 60, m_enchantress.Y - 100);
            m_enchantressIcon.Flip = m_enchantress.Flip;
            m_enchantressIcon.OutlineWidth = 2;

            m_architect = new ObjContainer("ArchitectIdle_Character");
            m_architect.Flip = SpriteEffects.FlipHorizontally;
            m_architect.Scale = new Vector2(2, 2);
            m_architect.Position = new Vector2(1550, 720 - 60 - (m_architect.Bounds.Bottom - m_architect.AnchorY) - 2);
            m_architect.PlayAnimation(true);
            m_architect.AnimationDelay = 1 / 10f;
            m_architect.OutlineWidth = 2;
            (m_architect.GetChildAt(ARCHITECT_HEAD_LAYER) as IAnimateableObj).StopAnimation();

            //m_architectIcon = new SpriteObj("TalkBubbleUpArrow_Sprite");
            m_architectIcon = new SpriteObj("UpArrowBubble_Sprite");
            m_architectIcon.Scale = new Vector2(2, 2);
            m_architectIcon.Visible = false;
            m_architectIconPosition = new Vector2(m_architect.X - 60, m_architect.Y - 100);
            m_architectIcon.Flip = m_architect.Flip;
            m_architectIcon.OutlineWidth = 2;
            m_architectRenovating = false;

            m_screw = new SpriteObj("ArchitectGear_Sprite");
            m_screw.Scale = new Vector2(2, 2);
            m_screw.OutlineWidth = 2;
            m_screw.Position = new Vector2(m_architect.X + 30, m_architect.Bounds.Bottom - 1);
            m_screw.AnimationDelay = 1 / 10f;

            m_tollCollector = new PhysicsObjContainer("NPCTollCollectorIdle_Character");
            m_tollCollector.Flip = SpriteEffects.FlipHorizontally;
            m_tollCollector.Scale = new Vector2(2.5f, 2.5f);
            m_tollCollector.IsWeighted = false;
            m_tollCollector.IsCollidable = true;
            m_tollCollector.Position = new Vector2(2565, 720 - 60 * 5 - (m_tollCollector.Bounds.Bottom - m_tollCollector.AnchorY));
            m_tollCollector.PlayAnimation(true);
            m_tollCollector.AnimationDelay = 1 / 10f;
            m_tollCollector.OutlineWidth = 2;
            m_tollCollector.CollisionTypeTag = GameTypes.CollisionType_WALL;

            //m_tollCollectorIcon = new SpriteObj("TalkBubbleUpArrow_Sprite");
            m_tollCollectorIcon = new SpriteObj("UpArrowBubble_Sprite");
            m_tollCollectorIcon.Scale = new Vector2(2, 2);
            m_tollCollectorIcon.Visible = false;
            m_tollCollectorIcon.Flip = m_tollCollector.Flip;
            m_tollCollectorIcon.OutlineWidth = 2;

            m_rainFG = new List<RaindropObj>();
            int numRainDrops = 400;
            if (LevelEV.SAVE_FRAMES == true)
                numRainDrops /= 2;

            for (int i = 0; i < numRainDrops; i++)
            {
                RaindropObj rain = new RaindropObj(new Vector2(CDGMath.RandomInt(-100, 1270 * 2), CDGMath.RandomInt(-400, 720)));
                m_rainFG.Add(rain);
            }
        }

        public override void Initialize()
        {
            foreach (TerrainObj obj in TerrainObjList)
            {
                if (obj.Name == "BlacksmithBlock")
                    m_blacksmithBlock = obj;
                if (obj.Name == "EnchantressBlock")
                    m_enchantressBlock = obj;
                if (obj.Name == "ArchitectBlock")
                    m_architectBlock = obj;

                if (obj.Name == "bridge")
                    obj.ShowTerrain = false;
            }       

            for (int i = 0; i < GameObjList.Count; i++)
            {
                if (GameObjList[i].Name == "Mountains 1")
                    m_mountain1 = GameObjList[i] as SpriteObj;

                if (GameObjList[i].Name == "Mountains 2")
                    m_mountain2 = GameObjList[i] as SpriteObj;
            }

            base.Initialize();
        }

        public override void LoadContent(GraphicsDevice graphics)
        {
            if (m_tree1 == null)
            {
                foreach (GameObj obj in GameObjList)
                {
                    if (obj.Name == "Tree1")
                        m_tree1 = obj;
                    else if (obj.Name == "Tree2")
                        m_tree2 = obj;
                    else if (obj.Name == "Tree3")
                        m_tree3 = obj;
                    else if (obj.Name == "Fern1")
                        m_fern1 = obj;
                    else if (obj.Name == "Fern2")
                        m_fern2 = obj;
                    else if (obj.Name == "Fern3")
                        m_fern3 = obj;
                }
            }

            base.LoadContent(graphics);
        }

        public override void OnEnter()
        {
            // Extra check to make sure sure you don't have a challenge token for a challenge already beaten.
            if (Game.PlayerStats.SpecialItem == SpecialItemType.EyeballToken && Game.PlayerStats.ChallengeEyeballBeaten == true)
                Game.PlayerStats.SpecialItem = SpecialItemType.None;
            if (Game.PlayerStats.SpecialItem == SpecialItemType.SkullToken && Game.PlayerStats.ChallengeSkullBeaten == true)
                Game.PlayerStats.SpecialItem = SpecialItemType.None;
            if (Game.PlayerStats.SpecialItem == SpecialItemType.FireballToken && Game.PlayerStats.ChallengeFireballBeaten == true)
                Game.PlayerStats.SpecialItem = SpecialItemType.None;
            if (Game.PlayerStats.SpecialItem == SpecialItemType.BlobToken && Game.PlayerStats.ChallengeBlobBeaten == true)
                Game.PlayerStats.SpecialItem = SpecialItemType.None;
            if (Game.PlayerStats.SpecialItem == SpecialItemType.LastBossToken && Game.PlayerStats.ChallengeLastBossBeaten == true)
                Game.PlayerStats.SpecialItem = SpecialItemType.None;
            Player.AttachedLevel.UpdatePlayerHUDSpecialItem();

            m_isSnowing = (DateTime.Now.Month == 12 || DateTime.Now.Month == 1); // Only snows in Dec. and Jan.
            if (m_isSnowing == true)
            {
                foreach (RaindropObj rainDrop in m_rainFG)
                {
                    rainDrop.ChangeToSnowflake();
                }
            }

            if ((Game.ScreenManager.Game as Game).SaveManager.FileExists(SaveType.Map) == false && Game.PlayerStats.HasArchitectFee == true)
                Game.PlayerStats.HasArchitectFee = false;

            Game.PlayerStats.TutorialComplete = true; // This needs to be removed later.
            Game.PlayerStats.IsDead = false;

            m_lightningTimer = 5;
            Player.CurrentHealth = Player.MaxHealth;
            Player.CurrentMana = Player.MaxMana;
            Player.ForceInvincible = false;
            (Player.AttachedLevel.ScreenManager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData);

            if (TollCollectorAvailable == true)
            {
                if (Player.AttachedLevel.PhysicsManager.ObjectList.Contains(m_tollCollector) == false)
                    Player.AttachedLevel.PhysicsManager.AddObject(m_tollCollector);
            }

            if (m_blacksmithAnvilSound == null)
                m_blacksmithAnvilSound = new FrameSoundObj(m_blacksmith.GetChildAt(5) as IAnimateableObj, Player, 7, "Anvil1", "Anvil2", "Anvil3");

            // Special check for Glaucoma
            if (Game.PlayerStats.Traits.X == TraitType.Glaucoma || Game.PlayerStats.Traits.Y == TraitType.Glaucoma)
                Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0.7f);
            else
                Game.ShadowEffect.Parameters["ShadowIntensity"].SetValue(0);

            m_playerWalkedOut = false;
            //if (Game.PlayerSaveData.SaveLoaded == false)
            {
                Player.UpdateCollisionBoxes(); // Necessary check since the OnEnter() is called before player can update its collision boxes.
                Player.Position = new Vector2(0, 720 - 60 - (Player.Bounds.Bottom - Player.Y));
                Player.State = 1; // Force the player into a walking state. This state will not update until the logic set is complete.
                Player.IsWeighted = false;
                Player.IsCollidable = false;
                LogicSet playerMoveLS = new LogicSet(Player);
                playerMoveLS.AddAction(new RunFunctionLogicAction(Player, "LockControls"));
                playerMoveLS.AddAction(new MoveDirectionLogicAction(new Vector2(1, 0)));
                playerMoveLS.AddAction(new ChangeSpriteLogicAction("PlayerWalking_Character"));
                playerMoveLS.AddAction(new PlayAnimationLogicAction(true));
                playerMoveLS.AddAction(new DelayLogicAction(0.5f));
                playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "CurrentSpeed", 0));
                playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "IsWeighted", true));
                playerMoveLS.AddAction(new ChangePropertyLogicAction(Player, "IsCollidable", true));
                Player.RunExternalLogicSet(playerMoveLS); // Do not dispose this logic set. The player object will do it on its own.
                Tween.By(this, 1.0f, Linear.EaseNone);
                Tween.AddEndHandlerToLastTween(Player, "UnlockControls");
            }

            SoundManager.StopMusic(1);

            m_isRaining = CDGMath.RandomPlusMinus() > 0;
            m_isRaining = true;

            if (m_isRaining == true)
            {
                if (m_rainSFX != null) 
                    m_rainSFX.Dispose();

                if (m_isSnowing == false)
                    m_rainSFX = SoundManager.PlaySound("Rain1");
                else
                    m_rainSFX = SoundManager.PlaySound("snowloop_filtered");
                //m_rainSFX = SoundManager.Play3DSound(m_blacksmith, Player, "Rain1");
            }

            m_tent.TextureColor = new Color(200, 200, 200);
            m_blacksmithBoard.TextureColor = new Color(200, 200, 200);
            m_screw.TextureColor = new Color(200, 200, 200);

            if (Game.PlayerStats.LockCastle == true)
            {
                m_screw.GoToFrame(m_screw.TotalFrames);
                m_architectBlock.Position = new Vector2(1492, 439 + 140);
            }
            else
            {
                m_screw.GoToFrame(1);
                m_architectBlock.Position = new Vector2(1492, 439);
            }

            Player.UpdateEquipmentColours();


            base.OnEnter();
        }

        public override void OnExit()
        {
            if (m_rainSFX != null && m_rainSFX.IsDisposed == false)
                m_rainSFX.Stop(AudioStopOptions.Immediate);
        }

        public override void Update(GameTime gameTime)
        {
            // Player should have max hp and mp while in the starting room.
            Player.CurrentMana = Player.MaxMana;
            Player.CurrentHealth = Player.MaxHealth; 

            m_enchantressBlock.Visible = EnchantressAvailable;
            m_blacksmithBlock.Visible = SmithyAvailable;
            m_architectBlock.Visible = ArchitectAvailable;

            float totalSeconds = Game.TotalGameTime;

            if (m_playerWalkedOut == false)
            {
                if (Player.ControlsLocked == false && Player.X < this.Bounds.Left)
                {
                    m_playerWalkedOut = true;
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).StartWipeTransition();
                    Tween.RunFunction(0.2f, Player.AttachedLevel.ScreenManager, "DisplayScreen", ScreenType.Skill, true, typeof(List<object>));
                }
                else if (Player.ControlsLocked == false && Player.X > this.Bounds.Right && TollCollectorAvailable == false) // Make sure the player can't pass the toll collector
                {
                    m_playerWalkedOut = true;
                    LoadLevel();
                }
            }

            if (m_isRaining == true)
            {
                foreach (TerrainObj obj in TerrainObjList) // Optimization
                    obj.UseCachedValues = true; // No need to set it back to false.  The physics manager will do that

                foreach (RaindropObj raindrop in m_rainFG)
                    raindrop.Update(this.TerrainObjList, gameTime);
            }

            m_tree1.Rotation = -(float)Math.Sin(totalSeconds) * 2;
            m_tree2.Rotation = (float)Math.Sin(totalSeconds * 2);
            m_tree3.Rotation = (float)Math.Sin(totalSeconds * 2) * 2;
            m_fern1.Rotation = (float)Math.Sin(totalSeconds * 3f) / 2;
            m_fern2.Rotation = -(float)Math.Sin(totalSeconds * 4f);
            m_fern3.Rotation = (float)Math.Sin(totalSeconds * 4f) / 2f;

            if (m_architectRenovating == false)
                HandleInput();

            if (SmithyAvailable)
            {
                if (m_blacksmithAnvilSound != null)
                    m_blacksmithAnvilSound.Update();
                m_blacksmith.Update(gameTime);
            }

            m_blacksmithIcon.Visible = false;
            if (Player != null && CollisionMath.Intersects(Player.TerrainBounds, m_blacksmith.Bounds) && Player.IsTouchingGround == true && SmithyAvailable == true)
                m_blacksmithIcon.Visible = true;
            m_blacksmithIcon.Position = new Vector2(m_blacksmithIconPosition.X, m_blacksmithIconPosition.Y - 70 + (float)Math.Sin(totalSeconds * 20) * 2);

            m_enchantressIcon.Visible = false;
            Rectangle enchantressBounds = new Rectangle((int)(m_enchantress.X - 100), (int)m_enchantress.Y, m_enchantress.Bounds.Width + 100, m_enchantress.Bounds.Height);
            if (Player != null && CollisionMath.Intersects(Player.TerrainBounds, enchantressBounds) && Player.IsTouchingGround == true && EnchantressAvailable == true)
                m_enchantressIcon.Visible = true;
            m_enchantressIcon.Position = new Vector2(m_enchantressIconPosition.X + 20, m_enchantressIconPosition.Y + (float)Math.Sin(totalSeconds * 20) * 2);

            if (Player != null && CollisionMath.Intersects(Player.TerrainBounds, new Rectangle((int)m_architect.X - 100, (int)m_architect.Y, m_architect.Width + 200, m_architect.Height))
                && Player.X < m_architect.X && Player.Flip == SpriteEffects.None && ArchitectAvailable == true)
                m_architectIcon.Visible = true;
            else
                m_architectIcon.Visible = false;
            m_architectIcon.Position = new Vector2(m_architectIconPosition.X, m_architectIconPosition.Y + (float)Math.Sin(totalSeconds * 20) * 2);

            if (Player != null && CollisionMath.Intersects(Player.TerrainBounds, new Rectangle((int)m_tollCollector.X - 100, (int)m_tollCollector.Y, m_tollCollector.Width + 200, m_tollCollector.Height))
                 && Player.X < m_tollCollector.X && Player.Flip == SpriteEffects.None && TollCollectorAvailable == true && m_tollCollector.SpriteName == "NPCTollCollectorIdle_Character")
                m_tollCollectorIcon.Visible = true;
            else
                m_tollCollectorIcon.Visible = false;
            m_tollCollectorIcon.Position = new Vector2(m_tollCollector.X - m_tollCollector.Width/2 - 10, m_tollCollector.Y - m_tollCollectorIcon.Height - m_tollCollector.Height/2 + (float)Math.Sin(totalSeconds * 20) * 2);

            // Setting blacksmith new icons settings.
            m_blacksmithNewIcon.Visible = false;
            if (SmithyAvailable == true)
            {
                if (m_blacksmithIcon.Visible == true && m_blacksmithNewIcon.Visible == true)
                    m_blacksmithNewIcon.Visible = false;
                else if (m_blacksmithIcon.Visible == false && BlacksmithNewIconVisible == true)
                    m_blacksmithNewIcon.Visible = true;
                m_blacksmithNewIcon.Position = new Vector2(m_blacksmithIcon.X + 50, m_blacksmithIcon.Y - 30);
            }

            // Setting enchantress new icons settings.
            m_enchantressNewIcon.Visible = false;
            if (EnchantressAvailable == true)
            {
                if (m_enchantressIcon.Visible == true && m_enchantressNewIcon.Visible == true)
                    m_enchantressNewIcon.Visible = false;
                else if (m_enchantressIcon.Visible == false && EnchantressNewIconVisible == true)
                    m_enchantressNewIcon.Visible = true;
                m_enchantressNewIcon.Position = new Vector2(m_enchantressIcon.X + 40, m_enchantressIcon.Y - 0);
            }

            // lightning effect.
            if (m_isRaining == true && m_isSnowing == false)
            {
                if (m_lightningTimer > 0)
                {
                    m_lightningTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_lightningTimer <= 0)
                    {
                        if (CDGMath.RandomInt(0, 100) > 70)
                        {
                            if (CDGMath.RandomInt(0, 1) > 0)
                                Player.AttachedLevel.LightningEffectTwice();
                            else
                                Player.AttachedLevel.LightningEffectOnce();
                        }
                        m_lightningTimer = 5;
                    }
                }
            }

            if (m_shakeScreen == true)
                UpdateShake();

            // Prevents the player from getting passed the Toll Collector.
            if (Player.Bounds.Right > m_tollCollector.Bounds.Left && TollCollectorAvailable == true)// && Game.PlayerStats.Traits.X != TraitType.Dwarfism && Game.PlayerStats.Traits.Y != TraitType.Dwarfism)
            {
                Player.X = m_tollCollector.Bounds.Left - (Player.Bounds.Right - Player.X);
                Player.AttachedLevel.UpdateCamera();
            }

            base.Update(gameTime);
        }

        private void LoadLevel()
        {
            Game.ScreenManager.DisplayScreen(ScreenType.Level, true, null);
        }

        private void HandleInput()
        {
            if (m_controlsLocked == false)
            {
                if (Player.State != PlayerObj.STATE_DASHING)
                {
                    if (m_blacksmithIcon.Visible == true && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2)))
                    {
                        MovePlayerTo(m_blacksmith);
                    }

                    if (m_enchantressIcon.Visible == true && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2)))
                    {
                        MovePlayerTo(m_enchantress);
                    }

                    if (m_architectIcon.Visible == true && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2)))
                    {
                        RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                        if ((Game.ScreenManager.Game as Game).SaveManager.FileExists(SaveType.Map))
                        {
                            if (Game.PlayerStats.LockCastle == false)
                            {
                                if (Game.PlayerStats.SpokeToArchitect == false)
                                {
                                    Game.PlayerStats.SpokeToArchitect = true;
                                    //(manager.Game as Game).SaveManager.SaveFiles(SaveType.PlayerData);
                                    manager.DialogueScreen.SetDialogue("Meet Architect");
                                }
                                else
                                    manager.DialogueScreen.SetDialogue("Meet Architect 2");

                                manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
                                manager.DialogueScreen.SetConfirmEndHandler(this, "ActivateArchitect");
                                manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
                            }
                            else
                                manager.DialogueScreen.SetDialogue("Castle Already Locked Architect");
                        }
                        else
                            manager.DialogueScreen.SetDialogue("No Castle Architect");

                        manager.DisplayScreen(ScreenType.Dialogue, true);
                    }
                }

                if (m_tollCollectorIcon.Visible == true && (Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP1) || Game.GlobalInput.JustPressed(InputMapType.PLAYER_UP2)))
                {
                    RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
                    if (Game.PlayerStats.SpecialItem == SpecialItemType.FreeEntrance)
                    {
                        Tween.RunFunction(0.1f, this, "TollPaid", false);
                        manager.DialogueScreen.SetDialogue("Toll Collector Obol");
                        manager.DisplayScreen(ScreenType.Dialogue, true);
                    }
                    else if (Game.PlayerStats.SpecialItem == SpecialItemType.EyeballToken)
                    {
                        manager.DialogueScreen.SetDialogue("Challenge Icon Eyeball");
                        RunTollPaidSelection(manager);
                    }
                    else if (Game.PlayerStats.SpecialItem == SpecialItemType.SkullToken)
                    {
                        manager.DialogueScreen.SetDialogue("Challenge Icon Skull");
                        RunTollPaidSelection(manager);
                    }
                    else if (Game.PlayerStats.SpecialItem == SpecialItemType.FireballToken)
                    {
                        manager.DialogueScreen.SetDialogue("Challenge Icon Fireball");
                        RunTollPaidSelection(manager);
                    }
                    else if (Game.PlayerStats.SpecialItem == SpecialItemType.BlobToken)
                    {
                        manager.DialogueScreen.SetDialogue("Challenge Icon Blob");
                        RunTollPaidSelection(manager);
                    }
                    else if (Game.PlayerStats.SpecialItem == SpecialItemType.LastBossToken)
                    {
                        manager.DialogueScreen.SetDialogue("Challenge Icon Last Boss");
                        RunTollPaidSelection(manager);
                    }
                    else
                    {
                        if (Game.PlayerStats.SpokeToTollCollector == false)
                            manager.DialogueScreen.SetDialogue("Meet Toll Collector 1");
                        else
                        {
                            float amount = SkillSystem.GetSkill(SkillType.Prices_Down).ModifierAmount * 100;
                            manager.DialogueScreen.SetDialogue("Meet Toll Collector Skip" + (int)Math.Round(amount, MidpointRounding.AwayFromZero));
                        }

                        RunTollPaidSelection(manager);
                    }
                }
            }
        }

        private void RunTollPaidSelection(RCScreenManager manager)
        {
            manager.DialogueScreen.SetDialogueChoice("ConfirmTest1");
            manager.DialogueScreen.SetConfirmEndHandler(this, "TollPaid", true);
            manager.DialogueScreen.SetCancelEndHandler(typeof(Console), "WriteLine", "Canceling Selection");
            manager.DisplayScreen(ScreenType.Dialogue, true);
        }

        public void MovePlayerTo(GameObj target)
        {
            m_controlsLocked = true;
            if (Player.X != target.X - 150)
            {
                if (Player.X > target.Position.X - 150)
                    Player.Flip = SpriteEffects.FlipHorizontally;

                float duration = CDGMath.DistanceBetweenPts(Player.Position, new Vector2(target.X - 150, target.Y)) / (float)(Player.Speed);

                Player.UpdateCollisionBoxes(); // Necessary check since the OnEnter() is called before player can update its collision boxes.
                Player.State = 1; // Force the player into a walking state. This state will not update until the logic set is complete.
                Player.IsWeighted = false;
                Player.AccelerationY = 0;
                Player.AccelerationX = 0;
                Player.IsCollidable = false;
                Player.CurrentSpeed = 0;
                Player.LockControls();
                Player.ChangeSprite("PlayerWalking_Character");
                Player.PlayAnimation(true);
                LogicSet playerMoveLS = new LogicSet(Player);
                playerMoveLS.AddAction(new DelayLogicAction(duration));
                Player.RunExternalLogicSet(playerMoveLS);
                Tween.To(Player, duration, Tween.EaseNone, "X", (target.Position.X - 150).ToString());
                Tween.AddEndHandlerToLastTween(this, "MovePlayerComplete", target);
            }
            else
                MovePlayerComplete(target);
        }

        public void MovePlayerComplete(GameObj target)
        {
            m_controlsLocked = false;
            Player.IsWeighted = true;
            Player.IsCollidable = true;
            Player.UnlockControls();
            Player.Flip = SpriteEffects.None;
            if (target == m_blacksmith)
            {
                if (Game.PlayerStats.SpokeToBlacksmith == false)
                {
                    Game.PlayerStats.SpokeToBlacksmith = true;
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DialogueScreen.SetDialogue("Meet Blacksmith");
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DialogueScreen.SetConfirmEndHandler(Player.AttachedLevel.ScreenManager, "DisplayScreen", ScreenType.Blacksmith, true, null);
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true);
                }
                else
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Blacksmith, true);
            }
            else if (target == m_enchantress)
            {
                if (Game.PlayerStats.SpokeToEnchantress == false)
                {
                    Game.PlayerStats.SpokeToEnchantress = true;
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DialogueScreen.SetDialogue("Meet Enchantress");
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DialogueScreen.SetConfirmEndHandler(Player.AttachedLevel.ScreenManager, "DisplayScreen", ScreenType.Enchantress, true, null);
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Dialogue, true);
                }
                else
                    (Player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.Enchantress, true);
            }
        }

        public void TollPaid(bool chargeFee)
        {
            if (chargeFee == true)
            {
                float goldLost = Game.PlayerStats.Gold * (GameEV.GATEKEEPER_TOLL_COST - SkillSystem.GetSkill(SkillType.Prices_Down).ModifierAmount);
                Game.PlayerStats.Gold -= (int)(goldLost);
                if (goldLost > 0)
                    Player.AttachedLevel.TextManager.DisplayNumberStringText(-(int)goldLost, "LOC_ID_PLAYER_OBJ_1" /*"gold"*/, Color.Yellow, new Vector2(Player.X, Player.Bounds.Top));
            }

            if (Game.PlayerStats.SpokeToTollCollector == true && Game.PlayerStats.SpecialItem != SpecialItemType.FreeEntrance
                && Game.PlayerStats.SpecialItem != SpecialItemType.BlobToken
                && Game.PlayerStats.SpecialItem != SpecialItemType.LastBossToken
                && Game.PlayerStats.SpecialItem != SpecialItemType.FireballToken
                && Game.PlayerStats.SpecialItem != SpecialItemType.EyeballToken
                && Game.PlayerStats.SpecialItem != SpecialItemType.SkullToken)
            {
                Player.AttachedLevel.ImpactEffectPool.DisplayDeathEffect(m_tollCollector.Position);
                SoundManager.PlaySound("Charon_Laugh");
                HideTollCollector();
            }
            else
            {
                Game.PlayerStats.SpokeToTollCollector = true;
                SoundManager.PlaySound("Charon_Laugh");
                m_tollCollector.ChangeSprite("NPCTollCollectorLaugh_Character");
                m_tollCollector.AnimationDelay = 1 / 20f;
                m_tollCollector.PlayAnimation(true);
                Tween.RunFunction(1, Player.AttachedLevel.ImpactEffectPool, "DisplayDeathEffect", m_tollCollector.Position);
                Tween.RunFunction(1, this, "HideTollCollector");
            }

            if (Game.PlayerStats.SpecialItem == SpecialItemType.FreeEntrance ||
                Game.PlayerStats.SpecialItem == SpecialItemType.SkullToken ||
                Game.PlayerStats.SpecialItem == SpecialItemType.EyeballToken ||
                Game.PlayerStats.SpecialItem == SpecialItemType.LastBossToken ||
                Game.PlayerStats.SpecialItem == SpecialItemType.FireballToken||
                Game.PlayerStats.SpecialItem == SpecialItemType.BlobToken)
            {
                if (Game.PlayerStats.SpecialItem == SpecialItemType.EyeballToken)
                    Game.PlayerStats.ChallengeEyeballUnlocked = true;
                else if (Game.PlayerStats.SpecialItem == SpecialItemType.SkullToken)
                    Game.PlayerStats.ChallengeSkullUnlocked = true;
                else if (Game.PlayerStats.SpecialItem == SpecialItemType.FireballToken)
                    Game.PlayerStats.ChallengeFireballUnlocked = true;
                else if (Game.PlayerStats.SpecialItem == SpecialItemType.BlobToken)
                    Game.PlayerStats.ChallengeBlobUnlocked = true;
                else if (Game.PlayerStats.SpecialItem == SpecialItemType.LastBossToken)
                    Game.PlayerStats.ChallengeLastBossUnlocked = true;

                Game.PlayerStats.SpecialItem = SpecialItemType.None;
                Player.AttachedLevel.UpdatePlayerHUDSpecialItem();
            }
        }

        public void HideTollCollector()
        {
            SoundManager.Play3DSound(this, Player, "Charon_Poof");
            m_tollCollector.Visible = false;
            Player.AttachedLevel.PhysicsManager.RemoveObject(m_tollCollector);
        }

        public void ActivateArchitect()
        {
            Player.LockControls();
            Player.CurrentSpeed = 0;
            m_architectIcon.Visible = false;
            m_architectRenovating = true;
            m_architect.ChangeSprite("ArchitectPull_Character");
            (m_architect.GetChildAt(1) as SpriteObj).PlayAnimation(false);
            m_screw.AnimationDelay = 1 / 30f;

            Game.PlayerStats.ArchitectUsed = true;

            Tween.RunFunction(0.5f, m_architect.GetChildAt(0), "PlayAnimation", true);
            Tween.RunFunction(0.5f, typeof(SoundManager), "PlaySound", "Architect_Lever");
            Tween.RunFunction(1, typeof(SoundManager), "PlaySound", "Architect_Screw");
            Tween.RunFunction(1f, m_screw, "PlayAnimation", false);
            Tween.By(m_architectBlock, 0.8f, Tween.EaseNone, "delay", "1.1", "Y", "135");
            Tween.RunFunction(1f, this, "ShakeScreen", 2, true, false);
            Tween.RunFunction(1.5f, this, "StopScreenShake");
            Tween.RunFunction(1.5f, Player.AttachedLevel.ImpactEffectPool, "SkillTreeDustEffect", new Vector2(m_screw.X - m_screw.Width / 2f, m_screw.Y - 40), true, m_screw.Width);
            Tween.RunFunction(3f, this, "StopArchitectActivation");
        }

        public void StopArchitectActivation()
        {
            m_architectRenovating = false;
            m_architectIcon.Visible = true;
            Player.UnlockControls();

            Game.PlayerStats.LockCastle = true;
            Game.PlayerStats.HasArchitectFee = true;

            foreach (ChestObj chest in Player.AttachedLevel.ChestList) // Resetting all fairy chests.
            {
                FairyChestObj fairyChest = chest as FairyChestObj;
                if (fairyChest != null)
                {
                    if (fairyChest.State == ChestConditionChecker.STATE_FAILED)
                        fairyChest.ResetChest();
                }
            }

            foreach (RoomObj room in Player.AttachedLevel.RoomList)
            {
                foreach (GameObj obj in room.GameObjList)
                {
                    BreakableObj breakableObj = obj as BreakableObj;
                    if (breakableObj != null)
                        breakableObj.Reset();
                }
            }

            RCScreenManager manager = Player.AttachedLevel.ScreenManager as RCScreenManager;
            manager.DialogueScreen.SetDialogue("Castle Lock Complete Architect");
            manager.DisplayScreen(ScreenType.Dialogue, true);
        }

        public override void Draw(Camera2D camera)
        {
            // Hacked parallaxing.
            m_mountain1.X = camera.TopLeftCorner.X * 0.5f;
            m_mountain2.X = m_mountain1.X + 2640; // 2640 not 1320 because it is mountain1 flipped.

            base.Draw(camera);

            if (m_isRaining == true)
                camera.Draw(Game.GenericTexture, new Rectangle(0, 0, 1320 * 2, 720), Color.Black * 0.3f);

            if (m_screenShakeCounter > 0)
            {
                camera.X += CDGMath.RandomPlusMinus();
                camera.Y += CDGMath.RandomPlusMinus();
                m_screenShakeCounter -= (float)camera.GameTime.ElapsedGameTime.TotalSeconds;
            }
            
            if (SmithyAvailable)
            {
                m_blacksmithBoard.Draw(camera);
                m_blacksmith.Draw(camera);
                m_blacksmithIcon.Draw(camera);
            }

            if (EnchantressAvailable)
            {
                m_tent.Draw(camera);
                m_enchantress.Draw(camera);
                m_enchantressIcon.Draw(camera);
            }

            if (ArchitectAvailable)
            {
                m_screw.Draw(camera);
                m_architect.Draw(camera);
                m_architectIcon.Draw(camera);
            }

            if (TollCollectorAvailable)
            {
                m_tollCollector.Draw(camera);
                m_tollCollectorIcon.Draw(camera);
            }

            m_blacksmithNewIcon.Draw(camera);
            m_enchantressNewIcon.Draw(camera);

            if (m_isRaining == true)
            {
                foreach (RaindropObj raindrop in m_rainFG)
                    raindrop.Draw(camera);
            }
        }

        public override void PauseRoom()
        {
            foreach (RaindropObj rainDrop in m_rainFG)
                rainDrop.PauseAnimation();

            if (m_rainSFX != null)
                m_rainSFX.Pause();

            m_enchantress.PauseAnimation();
            m_blacksmith.PauseAnimation();
            m_architect.PauseAnimation();
            m_tollCollector.PauseAnimation();

            base.PauseRoom();
        }

        public override void UnpauseRoom()
        {
            foreach (RaindropObj rainDrop in m_rainFG)
                rainDrop.ResumeAnimation();

            if (m_rainSFX != null && m_rainSFX.IsPaused)
                m_rainSFX.Resume();

            m_enchantress.ResumeAnimation();
            m_blacksmith.ResumeAnimation();
            m_architect.ResumeAnimation();
            m_tollCollector.ResumeAnimation();

            base.UnpauseRoom();
        }

        private bool m_horizontalShake;
        private bool m_verticalShake;
        private bool m_shakeScreen;
        private float m_screenShakeMagnitude;
        private Vector2 m_shakeStartingPos;

        public void ShakeScreen(float magnitude, bool horizontalShake = true, bool verticalShake = true)
        {
            m_shakeStartingPos = Player.AttachedLevel.Camera.Position;
            Player.AttachedLevel.CameraLockedToPlayer = false;
            m_screenShakeMagnitude = magnitude;
            m_horizontalShake = horizontalShake;
            m_verticalShake = verticalShake;
            m_shakeScreen = true;
        }

        public void UpdateShake()
        {
            if (m_horizontalShake == true)
                Player.AttachedLevel.Camera.X = m_shakeStartingPos.X + CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);

            if (m_verticalShake == true)
                Player.AttachedLevel.Camera.Y = m_shakeStartingPos.Y + CDGMath.RandomPlusMinus() * (CDGMath.RandomFloat(0, 1) * m_screenShakeMagnitude);
        }

        public void StopScreenShake()
        {
            Player.AttachedLevel.CameraLockedToPlayer = true;
            m_shakeScreen = false;
        }
        protected override GameObj CreateCloneInstance()
        {
            return new StartingRoomObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_blacksmith.Dispose();
                m_blacksmith = null;
                m_blacksmithIcon.Dispose();
                m_blacksmithIcon = null;
                m_blacksmithNewIcon.Dispose();
                m_blacksmithNewIcon = null;
                m_blacksmithBoard.Dispose();
                m_blacksmithBoard = null;

                m_enchantress.Dispose();
                m_enchantress = null;
                m_enchantressIcon.Dispose();
                m_enchantressIcon = null;
                m_enchantressNewIcon.Dispose();
                m_enchantressNewIcon = null;
                m_tent.Dispose();
                m_tent = null;

                m_architect.Dispose();
                m_architect = null;
                m_architectIcon.Dispose();
                m_architectIcon = null;
                m_screw.Dispose();
                m_screw = null;

                if (m_blacksmithAnvilSound != null) // If the blacksmith is never unlocked, this stays null and cannot be disposed.
                    m_blacksmithAnvilSound.Dispose();
                m_blacksmithAnvilSound = null;

                m_tree1 = null;
                m_tree2 = null;
                m_tree3 = null;
                m_fern1 = null;
                m_fern2 = null;
                m_fern3 = null;

                foreach (RaindropObj raindrop in m_rainFG)
                    raindrop.Dispose();
                m_rainFG.Clear();
                m_rainFG = null;

                m_mountain1 = null;
                m_mountain2 = null;

                m_tollCollector.Dispose();
                m_tollCollector = null;
                m_tollCollectorIcon.Dispose();
                m_tollCollectorIcon = null;

                m_blacksmithBlock = null;
                m_enchantressBlock = null;
                m_architectBlock = null;

                if (m_rainSFX != null)
                    m_rainSFX.Dispose();
                m_rainSFX = null;

                base.Dispose();
            }
        }

        private bool BlacksmithNewIconVisible
        {
            get
            {
                foreach (byte[] category in Game.PlayerStats.GetBlueprintArray)
                {
                    foreach (byte state in category)
                    {
                        if (state == EquipmentState.FoundButNotSeen)
                            return true;
                    }
                }
                return false;
            }
        }

        private bool EnchantressNewIconVisible
        {
            get
            {
                foreach (byte[] category in Game.PlayerStats.GetRuneArray)
                {
                    foreach (byte state in category)
                    {
                        if (state == EquipmentState.FoundButNotSeen)
                            return true;
                    }
                }
                return false;
            }
        }


        private bool SmithyAvailable
        {
            get { return SkillSystem.GetSkill(SkillType.Smithy).ModifierAmount > 0; }
        }

        private bool EnchantressAvailable
        {
            get { return SkillSystem.GetSkill(SkillType.Enchanter).ModifierAmount > 0; }
        }

        private bool ArchitectAvailable
        {
            get { return SkillSystem.GetSkill(SkillType.Architect).ModifierAmount > 0; }
        }

        private bool TollCollectorAvailable
        {
            get { return (Game.PlayerStats.TimesDead > 0 && m_tollCollector.Visible == true); }
        }
    }
}

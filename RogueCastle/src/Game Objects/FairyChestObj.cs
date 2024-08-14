using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class FairyChestObj : ChestObj
    {
        public int State { get; internal set; }
        private SpriteObj m_lockSprite;
        private SpriteObj m_errorSprite;
        private int m_conditionType = 0;
        private PlayerObj m_player;

        private const float SparkleDelay = 1;
        private float m_sparkleCounter = SparkleDelay;
        private TextObj m_timerText;

        public float Timer { get; set; }

        public FairyChestObj(PhysicsManager physicsManager)
            : base(physicsManager)
        {
            m_lockSprite = new SpriteObj("Chest4Unlock_Sprite");
            m_errorSprite = new SpriteObj("CancelIcon_Sprite");
            m_errorSprite.Visible = false;

            m_timerText = new TextObj(Game.JunicodeFont);
            m_timerText.FontSize = 18;
            m_timerText.DropShadow = new Vector2(2, 2);
            m_timerText.Align = Types.TextAlign.Centre;

            m_player = Game.ScreenManager.Player;
        }

        public void SetConditionType(int conditionType = 0)
        {
            if (conditionType != 0)
                m_conditionType = conditionType;
            else
                int.TryParse(Tag, out m_conditionType);

            if (m_conditionType == ChestConditionType.ReachIn5Seconds)
                Timer = 5;
        }

        public void SetChestUnlocked()
        {
            if (ConditionType != ChestConditionType.InvisibleChest && ConditionType != ChestConditionType.None)
                m_player.AttachedLevel.ObjectiveComplete();

            State = ChestConditionChecker.STATE_FREE;
            m_lockSprite.PlayAnimation(false);
            Tweener.Tween.By(m_lockSprite, 0.2f, Tweener.Ease.Linear.EaseNone, "Y", "40");
            Tweener.Tween.To(m_lockSprite, 0.2f, Tweener.Ease.Linear.EaseNone,"delay", "0.1", "Opacity", "0");
        }

        public void SetChestFailed(bool skipTween = false)
        {
            if (skipTween == false)
                m_player.AttachedLevel.ObjectiveFailed();
            State = ChestConditionChecker.STATE_FAILED;
            this.m_errorSprite.Visible = true;
            this.m_errorSprite.Opacity = 0f;
            m_errorSprite.Scale = Vector2.One;
            m_errorSprite.Position = new Vector2(this.X, this.Y - this.Height / 2);

            if (skipTween == false)
            {
                SoundManager.Play3DSound(this, Game.ScreenManager.Player,"FairyChest_Fail");
                Tweener.Tween.To(m_errorSprite, 0.5f, Tweener.Ease.Quad.EaseIn, "ScaleX", "0.5", "ScaleY", "0.5", "Opacity", "1");
            }
            else
            {
                m_errorSprite.Scale = new Vector2(0.5f, 0.5f);
                m_errorSprite.Opacity = 1;
            }
        }

        public override void OpenChest(ItemDropManager itemDropManager, PlayerObj player)
        {
            if (State == ChestConditionChecker.STATE_FREE)
            {
                if (IsOpen == false && IsLocked == false)
                {
                    this.GoToFrame(2);
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Chest_Open_Large");
                    // Give gold if all runes have been found
                    if (Game.PlayerStats.TotalRunesFound >= EquipmentCategoryType.Total * EquipmentAbilityType.Total)
                    {
                        GiveStatDrop(itemDropManager, m_player, 1, 0);
                        player.AttachedLevel.RefreshMapChestIcons();
                    }
                    else
                    {
                        List<byte[]> runeArray = Game.PlayerStats.GetRuneArray;
                        List<Vector2> possibleRunes = new List<Vector2>();

                        int categoryCounter = 0;
                        foreach (byte[] itemArray in runeArray)
                        {
                            int itemCounter = 0;
                            foreach (byte itemState in itemArray)
                            {
                                if (itemState == EquipmentState.NotFound)
                                    possibleRunes.Add(new Vector2(categoryCounter, itemCounter));
                                itemCounter++;
                            }
                            categoryCounter++;
                        }

                        if (possibleRunes.Count > 0)
                        {
                            Vector2 chosenRune = possibleRunes[CDGMath.RandomInt(0, possibleRunes.Count - 1)];
                            Game.PlayerStats.GetRuneArray[(int)chosenRune.X][(int)chosenRune.Y] = EquipmentState.FoundButNotSeen;
                            List<object> objectList = new List<object>();
                            objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                            objectList.Add(GetItemType.Rune);
                            objectList.Add(new Vector2(chosenRune.X, chosenRune.Y));

                            (player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                            player.RunGetItemAnimation();

                            Console.WriteLine("Unlocked item index " + chosenRune.X + " of type " + chosenRune.Y);
                        }
                        else
                            GiveGold(itemDropManager);

                        player.AttachedLevel.RefreshMapChestIcons();
                    }
                }
                //base.OpenChest(itemDropManager, player); // Regular chest opening logic.
            }
        }

        public override void Draw(Camera2D camera)
        {
            if (this.State == ChestConditionChecker.STATE_LOCKED)
                ChestConditionChecker.SetConditionState(this, m_player);

            if (this.IsOpen == false)
            {
                // Adds the chest sparkle effect.
                if (Game.ScreenManager.CurrentScreen is ProceduralLevelScreen) // Only sparkle the chest if the game is playing.  Otherwise it's probably paused.
                {
                    if (m_sparkleCounter > 0)
                    {
                        m_sparkleCounter -= (float)camera.GameTime.ElapsedGameTime.TotalSeconds;
                        if (m_sparkleCounter <= 0)
                        {
                            m_sparkleCounter = SparkleDelay;
                            float delay = 0;
                            for (int i = 0; i < 2; i++)
                            {
                                Tweener.Tween.To(this, delay, Tweener.Ease.Linear.EaseNone);
                                Tweener.Tween.AddEndHandlerToLastTween(m_player.AttachedLevel.ImpactEffectPool, "DisplayChestSparkleEffect", new Vector2(this.X, this.Y - this.Height / 2));
                                delay += 1 / 2f;
                            }
                        }
                    }
                }

                if (ConditionType == ChestConditionType.ReachIn5Seconds && this.State == ChestConditionChecker.STATE_LOCKED)
                {
                    if (m_player.AttachedLevel.IsPaused == false)
                        Timer -= (float)camera.GameTime.ElapsedGameTime.TotalSeconds;
                    m_timerText.Position = new Vector2(this.Position.X, this.Y - 50);
                    m_timerText.Text = ((int)Timer + 1).ToString();
                    m_timerText.Draw(camera);

                    // TODO: does this need to be refreshed on language change?
                    m_player.AttachedLevel.UpdateObjectiveProgress(
                        (LocaleBuilder.getResourceString(DialogueManager.GetText("Chest_Locked " + this.ConditionType).Dialogue[0]) + (int)(Timer + 1)).ToString()
                        );
                }
            }

            if (ConditionType != ChestConditionType.InvisibleChest || IsOpen == true)
            {
                base.Draw(camera);
                m_lockSprite.Flip = this.Flip;
                if (this.Flip == Microsoft.Xna.Framework.Graphics.SpriteEffects.None)
                    m_lockSprite.Position = new Vector2(this.X - 10, this.Y - this.Height / 2);
                else
                    m_lockSprite.Position = new Vector2(this.X + 10, this.Y - this.Height / 2);

                m_lockSprite.Draw(camera);

                m_errorSprite.Position = new Vector2(this.X, this.Y - this.Height / 2);
                m_errorSprite.Draw(camera);
            }

            //base.Draw(camera);
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (this.State == ChestConditionChecker.STATE_FREE)
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void ForceOpen()
        {
            this.State = ChestConditionChecker.STATE_FREE;
            m_errorSprite.Visible = false;
            m_lockSprite.Visible = false;
            base.ForceOpen();
        }

        public override void ResetChest()
        {
            this.State = ChestConditionChecker.STATE_LOCKED;
            m_errorSprite.Visible = false;
            m_lockSprite.Visible = true;
            m_lockSprite.Opacity = 1;
            this.Opacity = 1;
            m_lockSprite.PlayAnimation(1, 1, false);
            this.TextureColor = Color.White;
         
            if (ConditionType == ChestConditionType.ReachIn5Seconds)
                Timer = 5;

            base.ResetChest();
        }

        protected override GameObj CreateCloneInstance()
        {
            return new FairyChestObj(PhysicsMngr);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            FairyChestObj clone = obj as FairyChestObj;
            clone.State = this.State;
            SetConditionType();
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_player = null;
                m_lockSprite.Dispose();
                m_lockSprite = null;
                m_errorSprite.Dispose();
                m_errorSprite = null;
                m_timerText.Dispose();
                m_timerText = null;
                base.Dispose();
            }
        }

        public int ConditionType
        {
            get { return m_conditionType; }
        }
    }
}

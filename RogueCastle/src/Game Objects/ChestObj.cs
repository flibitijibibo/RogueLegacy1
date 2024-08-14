using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ChestObj : PhysicsObj
    {
        private byte m_chestType;

        //private int BrownChestGoldChance = 90;//100;//90; //Chance to get gold over getting a blueprint
        //private int SilverChestGoldChance = 40; //35;//65;

        private float GoldIncreasePerLevel = 1.425f;//1.375f;//1.35f;//1.25f;//1.15f;//1.0f;//0.90f;//0.70f; //How many coins each chest gives per room level, this takes room_level into account TESTED TRUE.

        //private int GoldBaseMod = 10;//300; //Minimum amount a chest will give the player.
        private Vector2 BronzeChestGoldRange = new Vector2(9, 14);//(7, 12);//(5, 15);//(10, 30); //How much gold a Bronze Chest gives.
        private Vector2 SilverChestGoldRange = new Vector2(20, 28);//(18, 25);//(15, 30);//(30, 50); //How much gold a Silver Chest gives.
        private Vector2 GoldChestGoldRange = new Vector2(47, 57);//(43, 52);//(45, 60);//(35, 60);//(30, 50); //How much gold a Gold Chest gives.

        public bool IsEmpty { get; set; }
        public bool IsLocked { get; set; }

        public int ForcedItemType { get; set; }
        public float ForcedAmount { get; set; }
        public bool IsProcedural { get; set; }

        public int Level = 0;

        private SpriteObj m_arrowIcon;

        public ChestObj(PhysicsManager physicsManager)
            : base("Chest1_Sprite", physicsManager)
        {
            DisableHitboxUpdating = true;
            this.IsWeighted = false;
            this.Layer = 1;
            this.OutlineWidth = 2;
            IsProcedural = true;

            m_arrowIcon = new SpriteObj("UpArrowSquare_Sprite");
            m_arrowIcon.OutlineWidth = 2;
            m_arrowIcon.Visible = false;
        }

        public virtual void OpenChest(ItemDropManager itemDropManager, PlayerObj player)
        {
            if (IsOpen == false && IsLocked == false)
            {
                SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Chest_Open_Large");

                this.GoToFrame(2);
                if (IsEmpty == true)
                    return;

                if (this.ChestType == RogueCastle.ChestType.Gold)
                    GameUtil.UnlockAchievement("LOVE_OF_GOLD");

                if (ForcedItemType == 0)
                {
                    // Item drop chance for MONEY/STAT DROP/BLUEPRINT
                    int dropRoll = CDGMath.RandomInt(1, 100);
                    int itemType = 0;
                    int[] dropChance = null;
                    if (this.ChestType == RogueCastle.ChestType.Brown)
                        dropChance = GameEV.BRONZECHEST_ITEMDROP_CHANCE;
                    else if (this.ChestType == RogueCastle.ChestType.Silver)
                        dropChance = GameEV.SILVERCHEST_ITEMDROP_CHANCE;
                    else
                        dropChance = GameEV.GOLDCHEST_ITEMDROP_CHANCE;

                    int itemChance = 0;
                    for (int i = 0; i < dropChance.Length; i++)
                    {
                        itemChance += dropChance[i];
                        if (dropRoll <= itemChance)
                        {
                            itemType = i;
                            break;
                        }
                    }

                    if (itemType == 0)
                        GiveGold(itemDropManager);
                    else if (itemType == 1)
                        GiveStatDrop(itemDropManager, player, 1, 0);
                    else
                        GivePrint(itemDropManager, player);
                }
                else
                {
                    switch (ForcedItemType)
                    {
                        case (ItemDropType.Coin):
                        case (ItemDropType.MoneyBag):
                        case (ItemDropType.Diamond):
                        case(ItemDropType.BigDiamond):
                            GiveGold(itemDropManager, (int)ForcedAmount);
                            break;
                        case (ItemDropType.Stat_Defense):
                        case (ItemDropType.Stat_Magic):
                        case (ItemDropType.Stat_MaxHealth):
                        case (ItemDropType.Stat_MaxMana):
                        case (ItemDropType.Stat_Weight):
                        case (ItemDropType.Stat_Strength):
                            GiveStatDrop(itemDropManager, player, 1, ForcedItemType);
                            break;
                        case (ItemDropType.Blueprint):
                        case (ItemDropType.Redprint):
                            GivePrint(itemDropManager, player);
                            break;
                        case (ItemDropType.TripStatDrop):
                            GiveStatDrop(itemDropManager, player, 3, 0);
                            break;
                        case (ItemDropType.FountainPiece1):
                        case (ItemDropType.FountainPiece2):
                        case (ItemDropType.FountainPiece3):
                        case (ItemDropType.FountainPiece4):
                        case (ItemDropType.FountainPiece5):
                            GiveStatDrop(itemDropManager, player, 1, ForcedItemType);
                            break;
                    }
                }

                player.AttachedLevel.RefreshMapChestIcons();
                //SoundManager.Play3DSound(this, Game.ScreenManager.Player,"ChestOpen1");
            }
        }

        public void GiveGold(ItemDropManager itemDropManager, int amount = 0)
        {
            int goldAmount = 0;
            if (this.ChestType == RogueCastle.ChestType.Brown)
                goldAmount = CDGMath.RandomInt((int)BronzeChestGoldRange.X, (int)BronzeChestGoldRange.Y) * 10;
            else if (this.ChestType == RogueCastle.ChestType.Silver || this.ChestType == RogueCastle.ChestType.Fairy)
                goldAmount = CDGMath.RandomInt((int)SilverChestGoldRange.X, (int)SilverChestGoldRange.Y) * 10;
            else
                goldAmount = CDGMath.RandomInt((int)GoldChestGoldRange.X, (int)GoldChestGoldRange.Y) * 10; //TEDDY ADDED ELSE IF SO GOLD CHESTS COULD DROP MONEY
            goldAmount += (int)Math.Floor(GoldIncreasePerLevel * this.Level * 10);
            
            if (amount != 0) // Override the randomized gold amount if a method parameter is passed in.
                goldAmount = amount;

            int numBigDiamonds = (int)(goldAmount / ItemDropType.BigDiamondAmount);
            goldAmount -= numBigDiamonds * ItemDropType.BigDiamondAmount;

            int numDiamonds = (int)(goldAmount / ItemDropType.DiamondAmount);
            goldAmount -= numDiamonds * ItemDropType.DiamondAmount;

            int numMoneyBags = (int)(goldAmount / ItemDropType.MoneyBagAmount);
            goldAmount -= numMoneyBags * ItemDropType.MoneyBagAmount;

            int numCoins = goldAmount / ItemDropType.CoinAmount;

            float delay = 0f;

            for (int i = 0; i < numBigDiamonds; i++)
            {
                Tweener.Tween.To(this, delay, Tweener.Ease.Linear.EaseNone);
                Tweener.Tween.AddEndHandlerToLastTween(itemDropManager, "DropItem", new Vector2(this.Position.X, this.Position.Y - this.Height / 2), ItemDropType.BigDiamond, ItemDropType.BigDiamondAmount);
                delay += 0.1f;
            }
            delay = 0f;

            for (int i = 0; i < numDiamonds; i++)
            {
                Tweener.Tween.To(this, delay, Tweener.Ease.Linear.EaseNone);
                Tweener.Tween.AddEndHandlerToLastTween(itemDropManager, "DropItem", new Vector2(this.Position.X, this.Position.Y - this.Height / 2), ItemDropType.Diamond, ItemDropType.DiamondAmount);
                delay += 0.1f;
            }
            delay = 0f;
            for (int i = 0; i < numMoneyBags; i++)
            {
                Tweener.Tween.To(this, delay, Tweener.Ease.Linear.EaseNone);
                Tweener.Tween.AddEndHandlerToLastTween(itemDropManager, "DropItem", new Vector2(this.Position.X, this.Position.Y - this.Height / 2), ItemDropType.MoneyBag, ItemDropType.MoneyBagAmount);
                delay += 0.1f;
            }
            delay = 0f;
            for (int i = 0; i < numCoins; i++)
            {
                Tweener.Tween.To(this, delay, Tweener.Ease.Linear.EaseNone);
                Tweener.Tween.AddEndHandlerToLastTween(itemDropManager, "DropItem", new Vector2(this.Position.X, this.Position.Y - this.Height / 2), ItemDropType.Coin, ItemDropType.CoinAmount);
                delay += 0.1f;
            }
        }

        public void GiveStatDrop(ItemDropManager manager, PlayerObj player, int numDrops, int statDropType)
        {
            int[] dropArray = new int[numDrops];

            for (int k = 0; k < numDrops; k++)
            {
                if (statDropType == 0)
                {
                    //Dropping Stat drop of type STR/MAG/DEF/HP/MP/WEIGHT
                    int dropRoll = CDGMath.RandomInt(1, 100);
                    int dropType = 0;

                    for (int i = 0; i < GameEV.STATDROP_CHANCE.Length; i++)
                    {
                        dropType += GameEV.STATDROP_CHANCE[i];
                        if (dropRoll <= dropType)
                        {
                            if (i == 0)
                            {
                                dropArray[k] = ItemDropType.Stat_Strength;
                                Game.PlayerStats.BonusStrength += 1 + Game.PlayerStats.TimesCastleBeaten;
                            }
                            else if (i == 1)
                            {
                                dropArray[k] = ItemDropType.Stat_Magic;
                                Game.PlayerStats.BonusMagic += 1 + Game.PlayerStats.TimesCastleBeaten;

                            }
                            else if (i == 2)
                            {
                                dropArray[k] = ItemDropType.Stat_Defense;
                                Game.PlayerStats.BonusDefense += 1 + Game.PlayerStats.TimesCastleBeaten;
                            }
                            else if (i == 3)
                            {
                                dropArray[k] = ItemDropType.Stat_MaxHealth;
                                Game.PlayerStats.BonusHealth += 1 + Game.PlayerStats.TimesCastleBeaten;
                            }
                            else if (i == 4)
                            {
                                dropArray[k] = ItemDropType.Stat_MaxMana;
                                Game.PlayerStats.BonusMana += 1 + Game.PlayerStats.TimesCastleBeaten;
                            }
                            else
                            {
                                dropArray[k] = ItemDropType.Stat_Weight;
                                Game.PlayerStats.BonusWeight += 1 + Game.PlayerStats.TimesCastleBeaten;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    switch (statDropType)
                    {
                        case (ItemDropType.Stat_MaxHealth):
                            Game.PlayerStats.BonusHealth += 1 + Game.PlayerStats.TimesCastleBeaten;
                            break;
                        case (ItemDropType.Stat_MaxMana):
                            Game.PlayerStats.BonusMana += 1 + Game.PlayerStats.TimesCastleBeaten;
                            break;
                        case (ItemDropType.Stat_Defense):
                            Game.PlayerStats.BonusDefense += 1 + Game.PlayerStats.TimesCastleBeaten;
                            break;
                        case (ItemDropType.Stat_Magic):
                            Game.PlayerStats.BonusMagic += 1 + Game.PlayerStats.TimesCastleBeaten;
                            break;
                        case (ItemDropType.Stat_Strength):
                            Game.PlayerStats.BonusStrength += 1 + Game.PlayerStats.TimesCastleBeaten;
                            break;
                        case (ItemDropType.Stat_Weight):
                            Game.PlayerStats.BonusWeight += 1 + Game.PlayerStats.TimesCastleBeaten;
                            break;
                    }
                    dropArray[k] = statDropType;
                }
            }

            List<object> objectList = new List<object>();
            // First data stored in the object is its position.
            // Second data stored in the object is the GetItemType
            // Third data stored in the object is misc data stored in a vector2
            // Fourth data stored only applies to triple stats, and is a vector2 that contains 2 of the 3 random stats for the player to collect.

            objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));

            if (statDropType >= ItemDropType.FountainPiece1 && statDropType <= ItemDropType.FountainPiece5)
            {
                objectList.Add(GetItemType.FountainPiece);
            }
            else
            {
                if (numDrops <= 1)
                    objectList.Add(GetItemType.StatDrop);
                else
                    objectList.Add(GetItemType.TripStatDrop);
            }

            objectList.Add(new Vector2(dropArray[0], 0));

            if (numDrops > 1)
                objectList.Add(new Vector2(dropArray[1], dropArray[2]));

            player.AttachedLevel.UpdatePlayerHUD();
            (player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
            player.RunGetItemAnimation();
        }

        public void GivePrint(ItemDropManager manager, PlayerObj player)
        {
            // Give gold if all blueprints have been found
            if (Game.PlayerStats.TotalBlueprintsFound >= EquipmentCategoryType.Total * EquipmentBaseType.Total)
            {
                if (this.ChestType == RogueCastle.ChestType.Gold)
                    GiveStatDrop(manager, player, 1, 0);
                else
                    GiveGold(manager);
            }
            else
            {
                List<byte[]> blueprintArray = Game.PlayerStats.GetBlueprintArray;
                List<Vector2> possibleBlueprints = new List<Vector2>();

                int categoryCounter = 0;
                foreach (byte[] itemArray in blueprintArray)
                {
                    int itemCounter = 0;
                    foreach (byte itemState in itemArray)
                    {
                        if (itemState == EquipmentState.NotFound)
                        {
                            EquipmentData item = Game.EquipmentSystem.GetEquipmentData(categoryCounter, itemCounter);
                            if (this.Level >= item.LevelRequirement && this.ChestType >= item.ChestColourRequirement)
                                possibleBlueprints.Add(new Vector2(categoryCounter, itemCounter));
                        }
                        itemCounter++;
                    }
                    categoryCounter++;
                }

                if (possibleBlueprints.Count > 0)
                {
                    Vector2 chosenBlueprint = possibleBlueprints[CDGMath.RandomInt(0, possibleBlueprints.Count - 1)];
                    Game.PlayerStats.GetBlueprintArray[(int)chosenBlueprint.X][(int)chosenBlueprint.Y] = EquipmentState.FoundButNotSeen;
                    List<object> objectList = new List<object>();
                    objectList.Add(new Vector2(this.X, this.Y - this.Height / 2f));
                    objectList.Add(GetItemType.Blueprint);
                    objectList.Add(new Vector2(chosenBlueprint.X, chosenBlueprint.Y));

                    (player.AttachedLevel.ScreenManager as RCScreenManager).DisplayScreen(ScreenType.GetItem, true, objectList);
                    player.RunGetItemAnimation();

                    Console.WriteLine("Unlocked item index " + chosenBlueprint.X + " of type " + chosenBlueprint.Y);
                }
                else
                    GiveGold(manager);
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            PlayerObj player = otherBox.AbsParent as PlayerObj;
            if (this.IsLocked == false && this.IsOpen == false && player != null && player.IsTouchingGround == true)
                m_arrowIcon.Visible = true;
            base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public override void Draw(Camera2D camera)
        {
            if (m_arrowIcon.Visible == true)
            {
                m_arrowIcon.Position = new Vector2(this.Bounds.Center.X, this.Bounds.Top - 50 + (float)Math.Sin(Game.TotalGameTime * 20) * 3);
                m_arrowIcon.Draw(camera);
                m_arrowIcon.Visible = false;
            }

            base.Draw(camera);
        }

        // Opens the chest but gives none of the good stuff to the player.
        public virtual void ForceOpen()
        {
            this.GoToFrame(2);
        }

        public virtual void ResetChest()
        {
            this.GoToFrame(1);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new ChestObj(this.PhysicsMngr);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            ChestObj clone = obj as ChestObj;

            clone.IsProcedural = this.IsProcedural;
            clone.ChestType = this.ChestType;
            clone.Level = this.Level;
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_arrowIcon.Dispose();
                m_arrowIcon = null;
                base.Dispose();
            }
        }

        public byte ChestType
        {
            get { return m_chestType; }
            set
            {
                m_chestType = value;
                bool openChest = this.IsOpen;
                if (m_chestType == RogueCastle.ChestType.Boss)
                {
                    ForcedItemType = ItemDropType.TripStatDrop;
                    this.ChangeSprite("BossChest_Sprite");
                }
                else if (m_chestType == RogueCastle.ChestType.Fairy)
                    this.ChangeSprite("Chest4_Sprite");
                else if (m_chestType == RogueCastle.ChestType.Gold)
                    this.ChangeSprite("Chest3_Sprite");
                else if (m_chestType == RogueCastle.ChestType.Silver)
                    this.ChangeSprite("Chest2_Sprite");
                else
                    this.ChangeSprite("Chest1_Sprite");

                if (openChest == true)
                    this.GoToFrame(2);

                //System.Diagnostics.Debug.Assert((value == RogueCastle.ChestType.Fairy && this is FairyChestObj) || value != RogueCastle.ChestType.Fairy);
            }
        }

        public bool IsOpen
        {
            get { return this.CurrentFrame == 2; }
        }
    }
    
}

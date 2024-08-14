using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class ItemDropObj : PhysicsObj
    {
        public int DropType = ItemDropType.None;
        private float m_amount = 0;
        public float CollectionCounter { get; set; }

        public ItemDropObj(string spriteName)
            : base(spriteName)
        {
            this.IsCollidable = true;
            this.IsWeighted = true;
            this.CollisionTypeTag = GameTypes.CollisionType_ENEMY;
            this.StopAnimation();
            this.OutlineWidth = 2;
        }

        public void ConvertDrop(int dropType, float amount)
        {
            this.Scale = new Vector2(1);
            this.TextureColor = Color.White;

            switch (dropType)
            {
                case (ItemDropType.Health):
                    this.ChangeSprite("ChickenLeg_Sprite");
                    this.PlayAnimation(true);
                    break;
                case (ItemDropType.Mana):
                    this.ChangeSprite("ManaPotion_Sprite");
                    this.PlayAnimation(true);
                    break;
                case(ItemDropType.BigDiamond):
                    this.Scale = new Vector2(1.5f);
                    this.TextureColor = Color.LightGreen;
                    this.ChangeSprite("Diamond_Sprite");
                    this.PlayAnimation(true);
                    break;
                case (ItemDropType.Diamond):
                    this.ChangeSprite("Diamond_Sprite");
                    this.PlayAnimation(true);
                    break;
                case (ItemDropType.MoneyBag):
                    this.ChangeSprite("MoneyBag_Sprite");
                    this.PlayAnimation(1, 1, false);
                    break;
                case (ItemDropType.Stat_Strength):
                    this.ChangeSprite("Sword_Sprite");
                    this.PlayAnimation(true);
                    break;
                case(ItemDropType.Stat_Defense):
                    this.ChangeSprite("Shield_Sprite");
                    this.PlayAnimation(true);
                    break;
                case(ItemDropType.Stat_Weight):
                    this.ChangeSprite("Backpack_Sprite");
                    this.PlayAnimation(true);
                    break;
                case (ItemDropType.Stat_MaxMana):
                    this.ChangeSprite("Heart_Sprite");
                    this.PlayAnimation(true);
                    this.TextureColor = Color.Blue;
                    break;
                case (ItemDropType.Stat_MaxHealth):
                    this.ChangeSprite("Heart_Sprite");
                    this.PlayAnimation(true);
                    break;
                case (ItemDropType.Coin):
                default:
                    this.ChangeSprite("Coin_Sprite");
                    this.PlayAnimation(true);
                    break;
            }

            this.DropType = dropType;
            m_amount = amount;
            this.ClearCollisionBoxes();
            this.AddCollisionBox(0, 0, this.Width, this.Height, Consts.TERRAIN_HITBOX);
        }

        public void GiveReward(PlayerObj player, TextManager textManager)
        {
            switch (DropType)
            {
                case (ItemDropType.Coin):
                case (ItemDropType.Diamond):
                case(ItemDropType.BigDiamond):
                case (ItemDropType.MoneyBag):
                    player.AttachedLevel.ItemDropCollected(DropType);

                    float castleLockGoldModifier = 1;
                    if (Game.PlayerStats.HasArchitectFee == true)
                        castleLockGoldModifier = GameEV.ARCHITECT_FEE;
                    int goldAmount = (int)Math.Round(m_amount * (1 + player.TotalGoldBonus) * castleLockGoldModifier, MidpointRounding.AwayFromZero);
                    Game.PlayerStats.Gold += goldAmount;
                    textManager.DisplayNumberStringText(goldAmount, "LOC_ID_ITEM_DROP_OBJ_1" /*"gold"*/, Color.Yellow, new Vector2(this.X, this.Bounds.Top));
                    if (DropType == ItemDropType.MoneyBag)
                        this.PlayAnimation(1, 1, false);
                    break;
                case (ItemDropType.Health):
                    int healthAmount = (int)(player.MaxHealth * (m_amount + SkillSystem.GetSkill(SkillType.Potion_Up).ModifierAmount));
                    player.CurrentHealth += healthAmount;
                    textManager.DisplayNumberStringText(healthAmount, "LOC_ID_ITEM_DROP_OBJ_2" /*"hp recovered"*/, Color.LawnGreen, new Vector2(this.X, this.Bounds.Top));
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Collect_Health");
                    break;
                case (ItemDropType.Mana):
                    int manaAmount = (int)(player.MaxMana * (m_amount + SkillSystem.GetSkill(SkillType.Potion_Up).ModifierAmount));
                    player.CurrentMana += manaAmount;
                    textManager.DisplayNumberStringText(manaAmount, "LOC_ID_ITEM_DROP_OBJ_3" /*"mp recovered"*/, Color.LawnGreen, new Vector2(this.X, this.Bounds.Top));
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Collect_Mana");
                    break;
                case (ItemDropType.Stat_Defense):
                    Game.PlayerStats.BonusDefense += 1 + Game.PlayerStats.TimesCastleBeaten;
                    textManager.DisplayStringNumberText("LOC_ID_ITEM_DROP_OBJ_4" /*"Armor Up"*/, GameEV.ITEM_STAT_ARMOR_AMOUNT * (1  + Game.PlayerStats.TimesCastleBeaten), Color.LightSteelBlue, new Vector2(this.X, this.Bounds.Top));
                    break;
                case (ItemDropType.Stat_MaxHealth):
                    Game.PlayerStats.BonusHealth += 1 + Game.PlayerStats.TimesCastleBeaten;
                    textManager.DisplayStringNumberText("LOC_ID_ITEM_DROP_OBJ_5" /*"Max Health Up"*/, GameEV.ITEM_STAT_MAXHP_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten), Color.LightSteelBlue, new Vector2(this.X, this.Bounds.Top));
                    player.CurrentHealth += GameEV.ITEM_STAT_MAXHP_AMOUNT;
                    break;
                case (ItemDropType.Stat_MaxMana):
                    Game.PlayerStats.BonusMana += 1 + Game.PlayerStats.TimesCastleBeaten;
                    textManager.DisplayStringNumberText("LOC_ID_ITEM_DROP_OBJ_6" /*"Max Mana Up"*/, GameEV.ITEM_STAT_MAXMP_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten), Color.LightSteelBlue, new Vector2(this.X, this.Bounds.Top));
                    player.CurrentMana += GameEV.ITEM_STAT_MAXMP_AMOUNT;
                    break;
                case (ItemDropType.Stat_Strength):
                    Game.PlayerStats.BonusStrength += 1 + Game.PlayerStats.TimesCastleBeaten;
                    textManager.DisplayStringNumberText("LOC_ID_ITEM_DROP_OBJ_7" /*"Attack Up"*/, GameEV.ITEM_STAT_STRENGTH_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten), Color.LightSteelBlue, new Vector2(this.X, this.Bounds.Top));
                    break;
                case (ItemDropType.Stat_Magic):
                    Game.PlayerStats.BonusMagic += 1 + Game.PlayerStats.TimesCastleBeaten;
                    textManager.DisplayStringNumberText("LOC_ID_ITEM_DROP_OBJ_8" /*"Magic Up"*/, GameEV.ITEM_STAT_MAGIC_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten), Color.LightSteelBlue, new Vector2(this.X, this.Bounds.Top));
                    break;
                case (ItemDropType.Stat_Weight):
                    Game.PlayerStats.BonusWeight += 1 + Game.PlayerStats.TimesCastleBeaten;
                    textManager.DisplayStringNumberText("LOC_ID_ITEM_DROP_OBJ_9" /*"Max Weight Up"*/, GameEV.ITEM_STAT_WEIGHT_AMOUNT * (1 + Game.PlayerStats.TimesCastleBeaten), Color.LightSteelBlue, new Vector2(this.X, this.Bounds.Top));
                    break;
            }
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            if (collisionResponseType == Consts.COLLISIONRESPONSE_TERRAIN && ((otherBox.AbsParent is TerrainObj) || otherBox.AbsParent is HazardObj) && (otherBox.AbsParent is DoorObj) == false)
            {
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
                this.AccelerationX = 0;
                this.Y = (int)this.Y;
                if (this.DropType == ItemDropType.MoneyBag && this.CurrentFrame == 1)
                {
                    Vector2 mtd = CollisionMath.CalculateMTD(thisBox.AbsRect, otherBox.AbsRect);
                    //if (mtd == Vector2.Zero)
                    if (mtd.Y  < 0)
                        this.PlayAnimation(2, this.TotalFrames);
                }
            }
        }

        public override void Draw(Camera2D camera)
        {
            if (CollectionCounter > 0)
                CollectionCounter -= (float)camera.GameTime.ElapsedGameTime.TotalSeconds;
            base.Draw(camera);
        }

        public bool IsCollectable
        {
            get { return CollectionCounter <= 0; }
        }
    }
}

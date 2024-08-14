using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class BreakableObj : PhysicsObjContainer
    {
        public bool Broken { get; internal set; }
        private bool m_internalIsWeighted = false;
        public bool DropItem { get; set; }
        public bool HitBySpellsOnly { get; set; }

        public BreakableObj(string spriteName)
            : base(spriteName)
        {
            this.DisableCollisionBoxRotations = true;
            Broken = false;
            this.OutlineWidth = 2;
            this.SameTypesCollide = true;
            CollisionTypeTag = GameTypes.CollisionType_WALL_FOR_PLAYER;
            this.CollidesLeft = false;
            this.CollidesRight = false;
            this.CollidesBottom = false;

            foreach (GameObj obj in _objectList)
                obj.Visible = false;
            _objectList[0].Visible = true;

            DropItem = true;
        }

        public override void CollisionResponse(CollisionBox thisBox, CollisionBox otherBox, int collisionResponseType)
        {
            PlayerObj player = otherBox.AbsParent as PlayerObj;

            if (player != null && otherBox.Type == Consts.WEAPON_HITBOX && HitBySpellsOnly == false)
            {
                if (Broken == false)
                    Break();
            }

            ProjectileObj projectile = otherBox.AbsParent as ProjectileObj;
            if (projectile != null && (projectile.CollisionTypeTag == GameTypes.CollisionType_PLAYER || projectile.CollisionTypeTag == GameTypes.CollisionType_GLOBAL_DAMAGE_WALL) && otherBox.Type == Consts.WEAPON_HITBOX)
            {
                if (Broken == false)
                    Break();

                if (projectile.DestroysWithTerrain == true && this.SpriteName == "Target1_Character")
                    projectile.RunDestroyAnimation(false);
            }

            // Hack to make only terrain or other breakables collide with this.
            if ((otherBox.AbsRect.Y > thisBox.AbsRect.Y || otherBox.AbsRotation != 0) && (otherBox.Parent is TerrainObj || otherBox.AbsParent is BreakableObj))
                base.CollisionResponse(thisBox, otherBox, collisionResponseType);
        }

        public void Break()
        {
            PlayerObj player = Game.ScreenManager.Player;

            foreach (GameObj obj in _objectList)
                obj.Visible = true;

            this.GoToFrame(2);
            Broken = true;
            m_internalIsWeighted = this.IsWeighted;
            this.IsWeighted = false;
            this.IsCollidable = false;

            // Chance of dropping item.   HP/MP/COIN/MONEYBAG/NOTHING.
            if (DropItem == true)
            {
                // Special code to force drop an item.
                bool droppedItem = false;
                if (this.Name == "Health")
                {
                    player.AttachedLevel.ItemDropManager.DropItem(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                    droppedItem = true;
                }
                else if (this.Name == "Mana")
                {
                    player.AttachedLevel.ItemDropManager.DropItem(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                    droppedItem = true;
                }

                if (droppedItem == true)
                {
                    for (int i = 0; i < this.NumChildren; i++)
                    {
                        Tweener.Tween.By(this.GetChildAt(i), 0.3f, Tweener.Ease.Linear.EaseNone, "X", CDGMath.RandomInt(-50, 50).ToString(), "Y", "50", "Rotation", CDGMath.RandomInt(-360, 360).ToString());
                        Tweener.Tween.To(this.GetChildAt(i), 0.1f, Tweener.Ease.Linear.EaseNone, "delay", "0.2", "Opacity", "0");
                    }
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
                    SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Break1", "Break2", "Break3");

                    if (Game.PlayerStats.Traits.X == TraitType.OCD || Game.PlayerStats.Traits.Y == TraitType.OCD)
                    {
                        player.CurrentMana += 1;
                        player.AttachedLevel.TextManager.DisplayNumberStringText(1, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.RoyalBlue, new Vector2(player.X, player.Bounds.Top - 30));
                    }
                    return;
                }

                int dropRoll = CDGMath.RandomInt(1, 100);
                int dropChance = 0;
                for (int i = 0; i < GameEV.BREAKABLE_ITEMDROP_CHANCE.Length; i++)
                {
                    dropChance += GameEV.BREAKABLE_ITEMDROP_CHANCE[i];
                    if (dropRoll <= dropChance)
                    {
                        if (i == 0)
                        {
                            if (Game.PlayerStats.Traits.X != TraitType.Alektorophobia && Game.PlayerStats.Traits.Y != TraitType.Alektorophobia)
                                player.AttachedLevel.ItemDropManager.DropItem(this.Position, ItemDropType.Health, GameEV.ITEM_HEALTHDROP_AMOUNT);
                            else
                            {
                                EnemyObj_Chicken chicken = new EnemyObj_Chicken(null, null, null, GameTypes.EnemyDifficulty.BASIC);
                                chicken.AccelerationY = -500;
                                chicken.Position = this.Position;
                                chicken.Y -= 50;
                                chicken.SaveToFile = false;
                                player.AttachedLevel.AddEnemyToCurrentRoom(chicken);
                                chicken.IsCollidable = false;
                                Tweener.Tween.RunFunction(0.2f, chicken, "MakeCollideable");
                                SoundManager.Play3DSound(this, Game.ScreenManager.Player, "Chicken_Cluck_01", "Chicken_Cluck_02", "Chicken_Cluck_03");
                            }
                        }
                        else if (i == 1)
                            player.AttachedLevel.ItemDropManager.DropItem(this.Position, ItemDropType.Mana, GameEV.ITEM_MANADROP_AMOUNT);
                        else if (i == 2)
                            player.AttachedLevel.ItemDropManager.DropItem(this.Position, ItemDropType.Coin, ItemDropType.CoinAmount);
                        else if (i == 3)
                            player.AttachedLevel.ItemDropManager.DropItem(this.Position, ItemDropType.MoneyBag, ItemDropType.MoneyBagAmount);
                        break;
                    }
                }
            }

            for (int i = 0; i < this.NumChildren; i++)
            {
                Tweener.Tween.By(this.GetChildAt(i), 0.3f, Tweener.Ease.Linear.EaseNone, "X", CDGMath.RandomInt(-50, 50).ToString(), "Y", "50", "Rotation", CDGMath.RandomInt(-360,360).ToString());
                Tweener.Tween.To(this.GetChildAt(i), 0.1f, Tweener.Ease.Linear.EaseNone, "delay", "0.2", "Opacity", "0");
            }
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"EnemyHit1", "EnemyHit2", "EnemyHit3", "EnemyHit4", "EnemyHit5", "EnemyHit6");
            SoundManager.Play3DSound(this, Game.ScreenManager.Player,"Break1", "Break2", "Break3");

            if (Game.PlayerStats.Traits.X == TraitType.OCD || Game.PlayerStats.Traits.Y == TraitType.OCD)
            {
                player.CurrentMana += 1;
                player.AttachedLevel.TextManager.DisplayNumberStringText(1, "LOC_ID_SKILL_SCREEN_15" /*"mp"*/, Color.RoyalBlue, new Vector2(player.X, player.Bounds.Top - 30));
            }
        }

        public void ForceBreak()
        {
            foreach (GameObj obj in _objectList)
            {
                obj.Visible = true;
                obj.Opacity = 0;
            }

            this.GoToFrame(2);
            Broken = true;
            m_internalIsWeighted = this.IsWeighted;
            this.IsWeighted = false;
            this.IsCollidable = false;
        }

        public void Reset()
        {
            this.GoToFrame(1);
            Broken = false;
            this.IsWeighted = m_internalIsWeighted;
            this.IsCollidable = true;
            this.ChangeSprite(_spriteName);
            for (int i = 0; i < this.NumChildren; i++)
            {
                this.GetChildAt(i).Opacity = 1;
                this.GetChildAt(i).Rotation = 0;
            }

            // Call this after ChangeSprite, since ChangeSprite() overrides object visibility.
            foreach (GameObj obj in _objectList)
                obj.Visible = false;
            _objectList[0].Visible = true;
        }

        public void UpdateTerrainBox()
        {
            foreach (CollisionBox box in CollisionBoxes)
            {
                if (box.Type == Consts.TERRAIN_HITBOX)
                {
                    m_terrainBounds = box.AbsRect;
                    break;
                }
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new BreakableObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
            BreakableObj clone = obj as BreakableObj;
            clone.HitBySpellsOnly = this.HitBySpellsOnly;
            clone.DropItem = this.DropItem;
        }
    }
}

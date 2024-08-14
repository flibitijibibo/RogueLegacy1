using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RogueCastle
{
    public class HazardObj : PhysicsObj, IDealsDamageObj
    {
        //public int Damage { get; internal set; }

        private Texture2D m_texture;

        public HazardObj(int width, int height)
            : base("Spikes_Sprite", null)
        {
            this.IsWeighted = false;
            this.IsCollidable = true;
            CollisionTypeTag = GameTypes.CollisionType_GLOBAL_DAMAGE_WALL; //GameTypes.CollisionType_ENEMYWALL;
            //Damage = 25;//10;
            this.DisableHitboxUpdating = true;
        }

        public void InitializeTextures(Camera2D camera)
        {
            Vector2 scaleMod = new Vector2(60 / (float)_width, 60 / (float)_height);
            _width = (int)(_width * scaleMod.X);
            _height = (int)(_height * scaleMod.Y);

            m_texture = this.ConvertToTexture(camera);
            _width = (int)Math.Ceiling(_width / scaleMod.X);
            _height = (int)Math.Ceiling(_height / scaleMod.Y);

            this.Scale = new Vector2((_width / ((_width/60f) * 64)), 1);
        }

        public void SetWidth(int width)
        {
            //m_actualWidth = width;
            _width = width;
            foreach (CollisionBox box in CollisionBoxes)
            {
                if (box.Type == Consts.TERRAIN_HITBOX)
                    box.Width = _width - 25;
                else
                    box.Width = _width;
            }
        }

        public void SetHeight(int height)
        {
            //m_actualHeight = height;
            _height = height;
        }

        public override void Draw(Camera2D camera)
        {
            camera.Draw(m_texture, this.Position, new Rectangle(0, 0, (int)((Width / 60f) * 64),(int)( Height)), this.TextureColor, MathHelper.ToRadians(Rotation), Vector2.Zero, this.Scale, SpriteEffects.None, 1);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new HazardObj(_width, _height);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            HazardObj clone = obj as HazardObj;
            clone.SetWidth(Width);
            clone.SetHeight(Height);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                m_texture.Dispose();
                m_texture = null;
                base.Dispose();
            }
        }

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci)
        {
            base.PopulateFromXMLReader(reader, ci);

            this.SetWidth(_width);
            this.SetHeight(_height);
        }

        public override int Width
        {
            get { return _width; }
        }

        public override int Height
        {
            get { return _height; }
        }

        public override Rectangle TerrainBounds
        {
            get
            {
                foreach (CollisionBox box in this.CollisionBoxes)
                {
                    if (box.Type == Consts.TERRAIN_HITBOX)
                        return box.AbsRect;
                }
                return this.Bounds;
            }
        }

        public int Damage
        {
            get
            {
                PlayerObj player = Game.ScreenManager.Player;
                //int playerHealth = (int)Math.Round(((player.BaseHealth + player.GetEquipmentHealth() +
                //        (Game.PlayerStats.BonusHealth * GameEV.ITEM_STAT_MAXHP_AMOUNT) +
                //        SkillSystem.GetSkill(SkillType.Health_Up).ModifierAmount +
                //        SkillSystem.GetSkill(SkillType.Health_Up_Final).ModifierAmount) * Game.PlayerStats.LichHealthMod), MidpointRounding.AwayFromZero) + Game.PlayerStats.LichHealth;

                int playerHealth = (int)Math.Round(((player.BaseHealth + player.GetEquipmentHealth() +
           (Game.PlayerStats.BonusHealth * GameEV.ITEM_STAT_MAXHP_AMOUNT) +
           SkillSystem.GetSkill(SkillType.Health_Up).ModifierAmount +
           SkillSystem.GetSkill(SkillType.Health_Up_Final).ModifierAmount)), MidpointRounding.AwayFromZero);

                int damage = (int)(playerHealth * GameEV.HAZARD_DAMAGE_PERCENT);

                if (damage < 1)
                    damage = 1;
                return damage;
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace RogueCastle
{
    public class TerrainObj : BlankObj
    {
        public bool ShowTerrain = true;

        public TerrainObj(int width, int height)
            : base(width, height)
        {
            this.CollisionTypeTag = GameTypes.CollisionType_WALL;
            this.IsCollidable = true;
            this.IsWeighted = false;
        }

        public override void Draw(Camera2D camera)
        {
            if (ShowTerrain == true && CollisionMath.Intersects(this.Bounds, camera.Bounds) || this.ForceDraw == true)
                camera.Draw(Game.GenericTexture, this.Position, new Rectangle(0, 0, this.Width, this.Height), this.TextureColor, MathHelper.ToRadians(Rotation), Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new TerrainObj(_width, _height);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            TerrainObj clone = obj as TerrainObj;
            clone.ShowTerrain = this.ShowTerrain;
            foreach (CollisionBox box in this.CollisionBoxes)
                clone.AddCollisionBox(box.X, box.Y, box.Width, box.Height, box.Type);
        }

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci)
        {
            base.PopulateFromXMLReader(reader, ci);

            SetWidth(_width);
            SetHeight(_height);
            this.AddCollisionBox(0, 0, _width, _height, Consts.TERRAIN_HITBOX); // This adds the terrain collision box for terrain objects.
            this.AddCollisionBox(0, 0, _width, _height, Consts.BODY_HITBOX); // This adds a body collision box to terrain objects.

            if (this.CollidesTop == true &&
                this.CollidesBottom == false &&
                this.CollidesLeft == false &&
                this.CollidesRight == false) // Decreases the height of a fall-through platform.
                this.SetHeight(this.Height / 2);
        }

        public Rectangle NonRotatedBounds
        {
            get
            {
                 return new Rectangle((int)this.X, (int)this.Y, this.Width, this.Height);
            }
        }

        private struct CornerPoint
        {
            public Vector2 Position;
            public float Rotation;

            public CornerPoint(Vector2 position, float rotation)
            {
                Position = position;
                Rotation = MathHelper.ToRadians(rotation);
            }
        }
    }
}

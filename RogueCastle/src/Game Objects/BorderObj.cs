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
    public class BorderObj : GameObj
    {
        public bool BorderTop = false;
        public bool BorderBottom = false;
        public bool BorderLeft = false;
        public bool BorderRight = false;

        public GameTypes.LevelType LevelType = GameTypes.LevelType.CASTLE;
        public Texture2D BorderTexture { get; internal set; }
        public SpriteObj CornerTexture { get; internal set; }
        public SpriteObj CornerLTexture { get; internal set; }

        public Texture2D NeoTexture { get; set; }
        public Vector2 TextureScale { get; set; }
        public Vector2 TextureOffset { get; set; }

        public BorderObj()
        {
            TextureScale = new Vector2(1, 1);
            CornerTexture = new SpriteObj("Blank_Sprite");
            CornerTexture.Scale = new Vector2(2, 2);
            CornerLTexture = new SpriteObj("Blank_Sprite");
            CornerLTexture.Scale = new Vector2(2, 2);
        }

        public void SetBorderTextures(Texture2D borderTexture, string cornerTextureString, string cornerLTextureString)
        {
            BorderTexture = borderTexture;
            CornerTexture.ChangeSprite(cornerTextureString);
            CornerLTexture.ChangeSprite(cornerLTextureString);
        }

        public void SetWidth(int width)
        {
            _width = width;
        }

        public void SetHeight(int height)
        {
            _height = height;
            if (height < 60)
            {
                BorderBottom = false;
                BorderLeft = false;
                BorderRight = false;
                this.TextureColor = new Color(150, 150, 150);
            }
        }

        public override void Draw(Camera2D camera)
        {
            Texture2D borderTexture = BorderTexture;
            if (Game.PlayerStats.Traits.X == TraitType.TheOne || Game.PlayerStats.Traits.Y == TraitType.TheOne)
            {
                TextureOffset = Vector2.Zero;
                borderTexture = NeoTexture;
            }

            // Drawing the sides.
            if (BorderBottom == true)
                camera.Draw(borderTexture, new Vector2(this.Bounds.Right - CornerTexture.Width + TextureOffset.X, this.Bounds.Bottom - TextureOffset.Y), new Rectangle(0, 0, (int)(Width * 1 / TextureScale.X) - CornerTexture.Width * 2, borderTexture.Height), this.TextureColor, MathHelper.ToRadians(180), Vector2.Zero, this.TextureScale, SpriteEffects.None, 0);

            if (BorderLeft == true)
                camera.Draw(borderTexture, new Vector2(this.X + TextureOffset.Y, this.Bounds.Bottom - CornerTexture.Width - TextureOffset.X), new Rectangle(0, 0, (int)(Height * 1 / TextureScale.Y) - CornerTexture.Width * 2, borderTexture.Height), this.TextureColor, MathHelper.ToRadians(-90), Vector2.Zero, this.TextureScale, SpriteEffects.None, 0);

            if (BorderRight == true)
                camera.Draw(borderTexture, new Vector2(this.Bounds.Right - TextureOffset.Y, this.Y + CornerTexture.Width + TextureOffset.X), new Rectangle(0, 0, (int)(Height * 1 / TextureScale.Y) - CornerTexture.Width * 2, borderTexture.Height), this.TextureColor, MathHelper.ToRadians(90), Vector2.Zero, this.TextureScale, SpriteEffects.None, 0);

            if (BorderTop == true)
            {
                if (this.Rotation == 0)
                    camera.Draw(borderTexture, new Vector2(this.X + CornerTexture.Width + TextureOffset.X, this.Y + TextureOffset.Y), new Rectangle(0, 0, (int)(Width * 1 / TextureScale.X) - CornerTexture.Width * 2, borderTexture.Height), this.TextureColor, MathHelper.ToRadians(this.Rotation), Vector2.Zero, this.TextureScale, SpriteEffects.None, 0);
                else
                {
                    // Special borders for angled terrain.
                    Vector2 topLeftCorner = CollisionMath.UpperLeftCorner(new Rectangle((int)this.X, (int)this.Y, _width, _height), this.Rotation, Vector2.Zero);
                    Vector2 topRightCorner = CollisionMath.UpperRightCorner(new Rectangle((int)this.X, (int)this.Y, _width, _height), this.Rotation, Vector2.Zero);

                    //if (this.Rotation == 45) // ROTCHECK
                    if (this.Rotation > 0 && this.Rotation < 80)
                    {
                        CornerTexture.Flip = SpriteEffects.FlipHorizontally;
                        CornerTexture.Position = topLeftCorner;
                        CornerTexture.Rotation = 0;
                        CornerTexture.Draw(camera);

                        CornerTexture.Flip = SpriteEffects.None;
                        CornerTexture.Position = new Vector2(topRightCorner.X - CornerTexture.Width /2f, topRightCorner.Y);
                        CornerTexture.Rotation = 0;
                        CornerTexture.Draw(camera);

                    }
                    //else if (this.Rotation == -45) // ROTCHECK
                    if (this.Rotation < 0 && this.Rotation > -80)
                    {
                        CornerTexture.Flip = SpriteEffects.FlipHorizontally;
                        CornerTexture.Position = topLeftCorner;
                        CornerTexture.X += CornerTexture.Width / 2f;
                        CornerTexture.Rotation = 0;
                        CornerTexture.Draw(camera);


                        CornerTexture.Flip = SpriteEffects.None;
                        CornerTexture.Position = topRightCorner;
                        CornerTexture.Rotation = 0;
                        CornerTexture.Draw(camera);
                    }

                    camera.Draw(borderTexture, new Vector2(this.X + TextureOffset.X - ((float)Math.Sin(MathHelper.ToRadians(this.Rotation)) * TextureOffset.Y), this.Y + ((float)Math.Cos(MathHelper.ToRadians(this.Rotation)) * TextureOffset.Y)), new Rectangle(0, 0, (int)(Width * 1 / TextureScale.X), borderTexture.Height), this.TextureColor, MathHelper.ToRadians(this.Rotation), Vector2.Zero, this.TextureScale, SpriteEffects.None, 0);
                }
            }

            base.Draw(camera);
        }

        public void DrawCorners(Camera2D camera)
        {
            CornerTexture.TextureColor = this.TextureColor;
            CornerLTexture.TextureColor = this.TextureColor;
            CornerLTexture.Flip = SpriteEffects.None;
            CornerTexture.Flip = SpriteEffects.None;
            CornerLTexture.Rotation = 0;

            // Drawing the top borders.
            if (BorderTop == true)
            {
                if (BorderRight == true) // Draw top right corner L
                {
                    CornerLTexture.Position = new Vector2(this.Bounds.Right - CornerLTexture.Width, this.Bounds.Top);
                    CornerLTexture.Draw(camera);
                }
                else // Drop a top right corner only
                {
                    CornerTexture.Position = new Vector2(this.Bounds.Right - CornerTexture.Width, this.Bounds.Top);
                    CornerTexture.Draw(camera);
                }

                CornerLTexture.Flip = SpriteEffects.FlipHorizontally;
                CornerTexture.Flip = SpriteEffects.FlipHorizontally;
                if (BorderLeft == true) // Draw top left corner L
                {
                    CornerLTexture.Position = new Vector2(this.Bounds.Left + CornerLTexture.Width, this.Bounds.Top);
                    CornerLTexture.Draw(camera);
                }
                else // Drop a top left corner only
                {
                    CornerTexture.Position = new Vector2(this.Bounds.Left + CornerTexture.Width, this.Bounds.Top);
                    CornerTexture.Draw(camera);
                }

            }

            // Drawing the bottom borders.
            if (BorderBottom == true)
            {
                CornerTexture.Flip = SpriteEffects.FlipVertically;
                CornerLTexture.Flip = SpriteEffects.FlipVertically;

                if (BorderRight == true) // Draw bottom right corner L
                {
                    CornerLTexture.Position = new Vector2(this.Bounds.Right - CornerLTexture.Width, this.Bounds.Bottom - CornerLTexture.Height);
                    CornerLTexture.Draw(camera);
                }
                else // Draw a bottom right corner only
                {
                    CornerTexture.Flip = SpriteEffects.FlipVertically;
                    CornerTexture.Position = new Vector2(this.Bounds.Right - CornerTexture.Width, this.Bounds.Bottom - CornerTexture.Height);
                    CornerTexture.Draw(camera);
                }

                if (BorderLeft == true) // Draw bottom left corner L
                {
                    CornerLTexture.Position = new Vector2(this.Bounds.Left + CornerLTexture.Width, this.Bounds.Bottom - CornerLTexture.Height);
                    CornerLTexture.Rotation = 90;
                    CornerLTexture.Draw(camera);
                    CornerLTexture.Rotation = 0;
                }
                else // Drop a bottom left corner only
                {
                    CornerTexture.Flip = SpriteEffects.None;
                    CornerTexture.Position = new Vector2(this.Bounds.Left + CornerTexture.Width, this.Bounds.Bottom);
                    CornerTexture.Rotation = 180;
                    CornerTexture.Draw(camera);
                    CornerTexture.Rotation = 0;
                }
            }

            // Drawing left side corners.
            if (BorderLeft == true)
            {
                CornerTexture.Flip = SpriteEffects.None;
                CornerLTexture.Flip = SpriteEffects.None;

                if (BorderBottom == false)
                {
                    CornerTexture.Position = new Vector2(this.Bounds.Left, this.Bounds.Bottom - CornerTexture.Width);
                    CornerTexture.Flip = SpriteEffects.FlipHorizontally;
                    CornerTexture.Rotation = -90;
                    CornerTexture.Draw(camera);
                    CornerTexture.Rotation = 0;
                }

                if (BorderTop == false)
                {
                    CornerTexture.Position = new Vector2(this.Bounds.Left, this.Bounds.Top + CornerTexture.Width);
                    CornerTexture.Flip = SpriteEffects.None;
                    CornerTexture.Rotation = -90;
                    CornerTexture.Draw(camera);
                    CornerTexture.Rotation = 0;
                }
            }

            // Drawing right side corners
            if (BorderRight == true)
            {
                CornerTexture.Flip = SpriteEffects.None;
                CornerLTexture.Flip = SpriteEffects.None;

                if (BorderBottom == false)
                {
                    CornerTexture.Position = new Vector2(this.Bounds.Right, this.Bounds.Bottom - CornerTexture.Width);
                    CornerTexture.Rotation = 90;
                    CornerTexture.Draw(camera);
                    CornerTexture.Rotation = 0;
                }

                if (BorderTop == false)
                {
                    CornerTexture.Position = new Vector2(this.Bounds.Right, this.Bounds.Top + CornerTexture.Width);
                    CornerTexture.Flip = SpriteEffects.FlipHorizontally;
                    CornerTexture.Rotation = 90;
                    CornerTexture.Draw(camera);
                    CornerTexture.Rotation = 0;
                }
            }
        }

        protected override GameObj CreateCloneInstance()
        {
            return new BorderObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            BorderObj clone = obj as BorderObj;
            clone.LevelType = this.LevelType;
            clone.BorderTop = this.BorderTop;
            clone.BorderBottom = this.BorderBottom;
            clone.BorderLeft = this.BorderLeft;
            clone.BorderRight = this.BorderRight;
            clone.SetHeight(_height);
            clone.SetWidth(_width);
            clone.NeoTexture = this.NeoTexture;

            clone.SetBorderTextures(this.BorderTexture, this.CornerTexture.SpriteName, this.CornerLTexture.SpriteName);
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                // Done
                BorderTexture = null; // Do not dispose. SetBorderTexture passes a reference to a texture. Whatever classes passing that texture should dispose of it.
                CornerTexture.Dispose(); // You can dispose this one because it isn't a texture2D.  It's a spriteobj.
                CornerTexture = null;
                CornerLTexture.Dispose(); // You can dispose this one because it isn't a texture2D.  It's a spriteobj.
                CornerLTexture = null;
                NeoTexture = null;
                base.Dispose();
            }
        }

        public override void PopulateFromXMLReader(System.Xml.XmlReader reader, System.Globalization.CultureInfo ci)
        {
            base.PopulateFromXMLReader(reader, ci);

            SetWidth(_width);
            SetHeight(_height);
            if (reader.MoveToAttribute("CollidesTop"))
                this.BorderTop = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesBottom"))
                this.BorderBottom = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesLeft"))
                this.BorderLeft = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesRight"))
                this.BorderRight = bool.Parse(reader.Value);
        }
    }
}

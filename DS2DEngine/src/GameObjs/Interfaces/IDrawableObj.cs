using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DS2DEngine
{
    public interface IDrawableObj
    {
        float Opacity { get; set; }
        Color TextureColor { get; set; }

        void Draw(Camera2D camera);
        void ChangeSprite(string spriteName);
    }
}

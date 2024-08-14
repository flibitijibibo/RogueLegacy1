
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Randomchaos2DGodRays
{
    public class LightSourceMask : BasePostProcess
    {
        public Texture lishsourceTexture;
        public Vector2 lighScreenSourcePos;
        public string lightSourceasset;
        public float lightSize = 1500;

        public LightSourceMask(Game game, Vector2 sourcePos, string lightSourceasset, float lightSize)
            : base(game)
        {
            UsesVertexShader = true;
            lighScreenSourcePos = sourcePos;
            this.lightSourceasset = lightSourceasset;
            this.lightSize = lightSize;
        }

        public override void Draw(GameTime gameTime)
        {

            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/LightSourceMask");
                lishsourceTexture = Game.Content.Load<Texture2D>(lightSourceasset);
            }

            effect.Parameters["screenRes"].SetValue(new Vector2(16, 9));
            effect.Parameters["halfPixel"].SetValue(HalfPixel);
            effect.CurrentTechnique = effect.Techniques["LightSourceMask"];

            effect.Parameters["flare"].SetValue(lishsourceTexture);

            effect.Parameters["SunSize"].SetValue(lightSize);
            effect.Parameters["lightScreenPosition"].SetValue(lighScreenSourcePos);

            // Set Params.
            base.Draw(gameTime);

        }
    }
}

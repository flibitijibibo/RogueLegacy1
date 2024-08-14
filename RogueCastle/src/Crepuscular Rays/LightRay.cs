using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Randomchaos2DGodRays
{
    public class LightRay : BasePostProcess
    {
        public Vector2 lighScreenSourcePos;
        public float Density = .5f;
        public float Decay = .95f;
        public float Weight = 1.0f;
        public float Exposure = .15f;

        public LightRay(Game game, Vector2 sourcePos, float density, float decay, float weight, float exposure)
            : base(game)
        {
            lighScreenSourcePos = sourcePos;

            Density = density;
            Decay = decay;
            Weight = weight;
            Exposure = exposure;
            UsesVertexShader = true;
        }


        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/LightRays");

            effect.CurrentTechnique = effect.Techniques["LightRayFX"];

            effect.Parameters["halfPixel"].SetValue(HalfPixel);

            effect.Parameters["Density"].SetValue(Density);
            effect.Parameters["Decay"].SetValue(Decay);
            effect.Parameters["Weight"].SetValue(Weight);
            effect.Parameters["Exposure"].SetValue(Exposure);

            effect.Parameters["lightScreenPosition"].SetValue(lighScreenSourcePos);

            // Set Params.
            base.Draw(gameTime);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Randomchaos2DGodRays
{
    public class CrepuscularRays : BasePostProcessingEffect
    {
        public LightSourceMask lsMask;
        public LightRay rays;

        public Vector2 lightSource
        {
            set
            {
                lsMask.lighScreenSourcePos = value;
                rays.lighScreenSourcePos = value;
            }
            get
            {
                return rays.lighScreenSourcePos;
            }
        }

        public float X
        {
            set
            {
                lsMask.lighScreenSourcePos.X = value;
                rays.lighScreenSourcePos.X = value;
            }
            get
            { return rays.lighScreenSourcePos.X; }
        }

        public float Y
        {
            set
            {
                lsMask.lighScreenSourcePos.Y = value;
                rays.lighScreenSourcePos.Y = value;
            }
            get
            { return rays.lighScreenSourcePos.Y; }
        }

        public Texture lightTexture
        {
            get { return lsMask.lishsourceTexture; }
            set { lsMask.lishsourceTexture = value; } 
        }

        public float LightSourceSize
        {
            set { lsMask.lightSize = value; }
            get { return lsMask.lightSize; }
        }

        public float Density
        {
            get { return rays.Density; }
            set { rays.Density = value; }
        }

        public float Decay
        {
            get { return rays.Decay; }
            set { rays.Decay = value; }
        }

        public float Weight
        {
            get { return rays.Weight; }
            set { rays.Weight = value; }
        }

        public float Exposure
        {
            get { return rays.Exposure; }
            set { rays.Exposure = value; }
        }

        
        public CrepuscularRays(Game game, Vector2 lightScreenSourcePos, string lightSourceImage, float lightSourceSize, float density, float decay, float weight, float exposure)
            : base(game)
        {
            lsMask = new LightSourceMask(game, lightScreenSourcePos, lightSourceImage, lightSourceSize);
            rays = new LightRay(game, lightScreenSourcePos, density, decay, weight, exposure);
            AddPostProcess(lsMask);
            AddPostProcess(rays);
            
        }

    }
}

float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

sampler RenderSampler : register(s0);
sampler ParallaxBGSampler : register(s1);

float discrepancy = 0.9f;

float4 ApplyParallax(float2 texCoord : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
  float4 parallaxColour = tex2D(ParallaxBGSampler, texCoord);
  float4 renderColour = tex2D(RenderSampler, texCoord);

  if (renderColour.r > discrepancy && renderColour.g < 1 - discrepancy && renderColour.b > discrepancy)
	return parallaxColour;
  else
	return renderColour;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 ApplyParallax();
    }
}

// TODO: add effect parameters here.

sampler TextureSampler : register(s0);
sampler ShadowSampler : register(s1);

float ShadowIntensity = 0;

float4 ApplyShadow(float2 texCoord : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
  float4 bgColour = tex2D(TextureSampler, texCoord);
  float4 shadowColour = tex2D(ShadowSampler, texCoord);
  bgColour.a = ShadowIntensity - shadowColour.a; // Change the first value to set shadow amount. Setting to 0 means no shadow.

  return bgColour;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 ApplyShadow();
    }
}

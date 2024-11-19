
sampler TextureSampler : register(s0);
sampler LightSampler : register(s1);

float4 BasicString(float2 texCoord : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
  float4 colorGround = tex2D(TextureSampler, texCoord);
  float4 colorShadow = tex2D(LightSampler, texCoord);
  colorShadow.a = 0; // Sets the shadow's alpha to 0, making it invisible instead of black.

  float4 resultColor = colorGround * colorShadow + colorShadow * colorGround;
  return resultColor;
}

technique Basic
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 BasicString();
	}
}
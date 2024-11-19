
sampler TextureSampler : register(s1);
sampler LightSampler : register(s0);

float4 MaskShade(float2 texCoord : TEXCOORD0, float4 color : COLOR0) : COLOR0
{
  float4 colorGround = tex2D(TextureSampler, texCoord);
  float4 colorShadow = tex2D(LightSampler, texCoord);
  float4 resultColor = 0;

  if (colorShadow.r == 0 && colorShadow.g == 0 && colorShadow.b == 0)
	resultColor = colorGround;
	
  return resultColor;
}

technique Basic
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 MaskShade();
	}
}
sampler TextureSampler : register(s0);
sampler mask : register(s1);

float4 InvertShader(float2 Tex:TEXCOORD0) : COLOR0
{
	float4 texColor = tex2D(TextureSampler, Tex);
	float4 maskColor = tex2D(mask, Tex);

	if (maskColor.a >= 1)
		texColor = 1.0f - texColor;

	return texColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 InvertShader();
    }
}

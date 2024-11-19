sampler TextureSampler : register(s0);

// TODO: add effect parameters here.

float4 desiredTint;
float4 ColourSwappedOut1;
float4 ColourSwappedIn1;
float4 ColourSwappedOut2;
float4 ColourSwappedIn2;

float Opacity = 1;

const float colourBuffer = 0.2f;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(TextureSampler, texCoord);

	if (color.r == ColourSwappedOut1.r && color.g == ColourSwappedOut1.g && color.b == ColourSwappedOut1.b)
		return ColourSwappedIn1 * Opacity;

	if (color.r == ColourSwappedOut2.r && color.g == ColourSwappedOut2.g && color.b == ColourSwappedOut2.b)
		return ColourSwappedIn2 * Opacity;

	return color * desiredTint * Opacity;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

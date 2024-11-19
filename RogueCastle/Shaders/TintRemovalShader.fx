sampler TextureSampler : register(s0);

// TODO: add effect parameters here.

float4 desiredTint;
float4 nonTintedColour;
float4 nonTintedColour2;
const float colourBuffer = 0.2f;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // TODO: add your pixel shader code here.

	float4 color = tex2D(TextureSampler, texCoord);
	if ( ( (color.r >= nonTintedColour.r - colourBuffer && color.r <= nonTintedColour.r + colourBuffer) && (color.g >= nonTintedColour.g - colourBuffer && color.g <= nonTintedColour.g + colourBuffer) && (color.b >= nonTintedColour.b - colourBuffer && color.b <= nonTintedColour.b + colourBuffer) )
	||
	( (color.r >= nonTintedColour2.r - colourBuffer && color.r <= nonTintedColour2.r + colourBuffer) && (color.g >= nonTintedColour2.g - colourBuffer && color.g <= nonTintedColour2.g + colourBuffer) && (color.b >= nonTintedColour2.b - colourBuffer && color.b <= nonTintedColour2.b + colourBuffer) ))
		return color;
	else
		return color * desiredTint;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

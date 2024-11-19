//sampler TextureSampler : register(s0);

const float4 MaskRect;

float4x4 MatrixTransform;
float2 ViewportDimensions; 

struct VS_INPUT
{
   float4 Position : POSITION0;
};

struct VS_OUTPUT
{
   float4 Position : POSITION0;
   float4 VPos : TEXCOORD0;
   float4 color : COLOR0;
};

float4 ConvertToVPos( float4 screenPosition )
{
	float2 screenPos = screenPosition.xy / screenPosition.w;
    return float4((screenPos.x + 1) * ViewportDimensions.x * 0.5f, ((screenPos.y * -1) + 1) * ViewportDimensions.y * 0.5f, screenPosition.z, screenPosition.w);
}

VS_OUTPUT vs_main(VS_INPUT Input, float4 color: COLOR)
{
   VS_OUTPUT Output;
   Output.color = color;
   Output.Position = mul( Input.Position, MatrixTransform );
   Output.VPos = ConvertToVPos(Output.Position);
   return( Output );
}

float4 main(VS_OUTPUT output) : COLOR0
{
	if (output.VPos.x > MaskRect.x && output.VPos.x < (MaskRect.x + MaskRect.z) && 
	    output.VPos.y > MaskRect.y && output.VPos.y < (MaskRect.y + MaskRect.w))
		return output.color;
	else
		return 0;
}

technique Mask
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 vs_main();
        PixelShader = compile ps_2_0 main();
    }
}
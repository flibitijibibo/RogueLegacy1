texture SourceTexture; 
          
sampler inputSampler = sampler_state      
{
            Texture   = <SourceTexture>;
            
            MipFilter = Point;
            MinFilter = Point;
            MagFilter = Point;
            
            AddressU  = Clamp;
            AddressV  = Clamp;
};
   
float2 TextureDimensions;

struct VS_OUTPUT
{
    float4 Pos  : POSITION;
    float2 Tex  : TEXCOORD0;
};

VS_OUTPUT VS(
    float3 InPos  : POSITION,
    float2 InTex  : TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    // transform the position to the screen
    Out.Pos = float4(InPos,1) + float4(-TextureDimensions.x, TextureDimensions.y, 0, 0);
    Out.Tex = InTex;

    return Out;
}


float4 HorizontalReductionPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
	  float2 color = tex2D(inputSampler, TexCoord);
	  float2 colorR = tex2D(inputSampler, TexCoord + float2(TextureDimensions.x,0));
	  float2 result = min(color,colorR);
      return float4(result,0,1);
}

float4 CopyPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
	return tex2D(inputSampler, TexCoord);
}


technique HorizontalReduction
{
    pass P0
    {          
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 HorizontalReductionPS();
    }
}

technique Copy
{
    pass P0
    {          
        VertexShader = compile vs_2_0 VS();
        PixelShader  = compile ps_2_0 CopyPS();
    }
}
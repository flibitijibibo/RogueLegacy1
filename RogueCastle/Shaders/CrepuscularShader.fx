
float2 ScreenLightPos;
float Density = .5f;
float Decay = .95f;
float Weight = 1.0f;
float Exposure = .08f;
sampler DryImage : register(s0)
{
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;   
    AddressU  = Wrap;
    AddressV  = Wrap;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float2 deltaTexCoord = (texCoord - ScreenLightPos.xy);
    deltaTexCoord *= 1.0f / 50 * Density;
    float4 color = tex2D(DryImage, texCoord);
    float illuminationDecay = 1.0f;
    for (int i = 0; i < 20; i++)
	{
       texCoord -= deltaTexCoord;
       float4 sample = tex2D(DryImage, texCoord);
       sample *= illuminationDecay * Weight;
       color += sample;
       illuminationDecay *= Decay;
	}
	 return color * Exposure;
}

technique Technique1
{
    pass Pass1
    {
       PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

uniform extern float BloomThreshold;
float2 halfPixel;
sampler TextureSampler : register(s0);
float4 BrightPassPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    texCoord -= halfPixel;
    // Look up the original image color.
    float4 c = tex2D(TextureSampler, texCoord);
    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}
technique BloomExtract
{
    pass P0
    {
        PixelShader = compile ps_2_0 BrightPassPS();
    }
}


sampler2D Scene: register(s0){
    AddressU = Mirror;
    AddressV = Mirror;
};
texture OrgScene;
sampler2D orgScene = sampler_state
{
    Texture = <OrgScene>;
    AddressU = CLAMP;
    AddressV = CLAMP;
};


float4 BlendPS(float2 texCoord : TEXCOORD0 ) : COLOR0
{
    texCoord -= halfPixel;
    float4 col = tex2D(orgScene,texCoord) * tex2D(Scene,texCoord);
    return col;
}
float4 AditivePS(float2 texCoord : TEXCOORD0 ) : COLOR0
{
    texCoord -= halfPixel;
    float4 col = tex2D(orgScene,texCoord) + tex2D(Scene,texCoord);
    return col;
}
technique Blend
{
    pass p0
    {
        PixelShader = compile ps_2_0 BlendPS();
    }
}
technique Aditive
{
    pass p0
    {
        PixelShader = compile ps_2_0 AditivePS();
    }
}
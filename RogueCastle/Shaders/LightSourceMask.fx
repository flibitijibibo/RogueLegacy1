#include "PPVertexShader.fxh"

float2 lightScreenPosition;

float2 screenRes = float2(4,3);

float4x4 matVP;

float2 halfPixel;

float SunSize = 1500;

sampler2D Scene: register(s0){
	AddressU = Clamp;
	AddressV = Clamp;
};

texture flare;
sampler Flare = sampler_state
{
    Texture = (flare);
    AddressU = CLAMP;
    AddressV = CLAMP;
};

float4 LightSourceMaskPS(float2 texCoord : TEXCOORD0 ) : COLOR0
{
	texCoord -= halfPixel;

	// Get the scene
	float4 col = 0;
	
	// Find the suns position in the world and map it to the screen space.
		float2 coord;
		
		float size = SunSize / 1;
					
		float2 center = lightScreenPosition;

		coord = .5 - ((texCoord - center) * screenRes) / size * .5f;
		
		col += (pow(tex2D(Flare,coord),2) * 1) * 2;						
	
	
	return col * tex2D(Scene,texCoord);	
}

technique LightSourceMask
{
	pass p0
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 LightSourceMaskPS();
	}
}
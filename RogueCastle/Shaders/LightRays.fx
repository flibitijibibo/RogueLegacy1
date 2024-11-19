#include "PPVertexShader.fxh"

#define NUM_SAMPLES 25

float2 lightScreenPosition;


float4x4 matVP;

float2 halfPixel;

float Density = .5f;
float Decay = .95f;
float Weight = 1.0f;
float Exposure = .15f;

sampler2D Scene: register(s0){
	AddressU = Clamp;
	AddressV = Clamp;
};


float4 lightRayPS( float2 texCoord : TEXCOORD0 ) : COLOR0
{
	// Find light pixel position
	
	float2 TexCoord = texCoord - halfPixel;

	float2 DeltaTexCoord = (TexCoord - lightScreenPosition);
	DeltaTexCoord *= (1.0f / 128 * Density);
	//DeltaTexCoord *= (1.0f/ NUM_SAMPLES * Density);

	DeltaTexCoord = DeltaTexCoord ;

	float3 col = tex2D(Scene,TexCoord);
	float IlluminationDecay = 1.0;
	float3 Sample;
	
	for( int i = 0; i < NUM_SAMPLES; ++i )
	{
		TexCoord -= DeltaTexCoord;
		Sample = tex2D(Scene, TexCoord);
		Sample *= IlluminationDecay * Weight;
		col += Sample;
		IlluminationDecay *= Decay;			
	}

	return float4(col * Exposure,1);
	
}

technique LightRayFX
{
	pass p0
	{
		//VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 lightRayPS();
	}
}

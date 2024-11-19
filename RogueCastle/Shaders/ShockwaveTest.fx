sampler ScreenS : register(s0);
float2 center = (0.5,0.5);
float time;
float2 centerCoord = (0.5, 0.5);

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR
{
    float2 distance = abs(texCoord - centerCoord);
    float4 outputColor = tex2D(ScreenS, texCoord);

	float boop = length(distance);
	//    boop = abs(1 - boop);

	if (boop <= time + 0.025 && boop >= time - 0.025) 
	{
		float ecart = (distance - time); // value between -0.02 & 0.02
		float powEcart = 1.0-pow(abs(ecart*40.0),0.4); // value between -1 & 1 (because 0.02 * 50 = 1)
		float ecartTime = ecart  * powEcart; // value between -0.02 & 0.02
		float2 diff = normalize(texCoord - center); // get the direction
		float2 newTexCoord = texCoord + (diff * ecartTime);
		float4 color = tex2D(ScreenS, newTexCoord);
		return color;
	}

    return outputColor;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

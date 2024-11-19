sampler ScreenS : register(s0);

float wave;                // pi/.75 is a good default
float distortion;        // 1 is a good default
float2 centerCoord;        // 0.5,0.5 is the screen center

float4 RippleEffect(float2 texCoord: TEXCOORD0) : COLOR
{
    float2 distance = abs(texCoord - centerCoord);
    float scalar = length(distance);

    // invert the scale so 1 is centerpoint
    scalar = abs(1 - scalar);
	    
    // calculate how far to distort for this pixel    
    float sinoffset = sin(wave / scalar);
    sinoffset = clamp(sinoffset, 0, 1);
    
    // calculate which direction to distort
    float sinsign = cos(wave / scalar);    
    
    // reduce the distortion effect
    sinoffset = sinoffset * distortion/32;

    // pick a pixel on the screen for this pixel, based on
    // the calculated offset and direction
    float4 color = tex2D(ScreenS, texCoord+(sinoffset*sinsign));    
            
    return color;
}

technique
{
    pass P0
    {
        PixelShader = compile ps_2_0 RippleEffect();
    }
}
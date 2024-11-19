// The default numbers represent no change in HSV
float Hue = 0; // 0 to 360.  360 == 0 as hue loops back around.
float Brightness = 0;  // -1 to 1
float Contrast = 0; // -1 to 1
float Saturation = 1; // 1 is normal saturation. 0 is no saturation (no colour). < 0 is inverse saturation.
bool UseMask = false;

sampler Samp : register(S0);
sampler Mask : register(s1);

float3x3 QuaternionToMatrix(float4 quat)
{
    float3 cross = quat.yzx * quat.zxy;
    float3 square= quat.xyz * quat.xyz;
    float3 wimag = quat.w * quat.xyz;

    square = square.xyz + square.yzx;

    float3 diag = 0.5 - square;
    float3 a = (cross + wimag);
    float3 b = (cross - wimag);

    return float3x3(
    2.0 * float3(diag.x, b.z, a.y),
    2.0 * float3(a.z, diag.y, b.x),
    2.0 * float3(b.y, a.x, diag.z));
}

const float3 lumCoeff = float3(0.2125, 0.7154, 0.0721);

float4 mainPS(float2 uv : TEXCOORD) : COLOR
{
    float4 outputColor = tex2D(Samp, uv);
	float4 maskColor = tex2D(Mask, uv);

	if (UseMask == false || maskColor.a >= 1)
	{
		float3 hsv; 
		float3 intensity;           
        float3 root3 = float3(0.57735, 0.57735, 0.57735);
        float half_angle = 0.5 * radians(Hue); // Hue is radians of 0 to 360 degree
        float4 rot_quat = float4( (root3 * sin(half_angle)), cos(half_angle));
        float3x3 rot_Matrix = QuaternionToMatrix(rot_quat);     
        outputColor.rgb = mul(rot_Matrix, outputColor.rgb);
        outputColor.rgb = (outputColor.rgb - 0.5) *(Contrast + 1.0) + 0.5;  
        outputColor.rgb = outputColor.rgb + Brightness;         
        intensity = float(dot(outputColor.rgb, lumCoeff));
        outputColor.rgb = lerp(intensity, outputColor.rgb, Saturation );            
	}
    return outputColor;
}
technique TransformTexture {
    pass p0 {
        PixelShader = compile ps_2_0 mainPS();
    }
}
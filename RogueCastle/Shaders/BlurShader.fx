float BlurDistance = 0.003f;

sampler TextureSampler : register(s0);

float4 BlurShader(float2 Tex:TEXCOORD0) : COLOR0
{
   float4 Color;
 
    // Get the texel from ColorMapSampler using a modified texture coordinate. This
    // gets the texels at the neighbour texels and adds it to Color.
    Color  = tex2D( TextureSampler, float2(Tex.x+BlurDistance, Tex.y+BlurDistance));
    Color += tex2D( TextureSampler, float2(Tex.x-BlurDistance, Tex.y-BlurDistance));
    Color += tex2D( TextureSampler, float2(Tex.x+BlurDistance, Tex.y-BlurDistance));
    Color += tex2D( TextureSampler, float2(Tex.x-BlurDistance, Tex.y+BlurDistance));
    // We need to devide the color with the amount of times we added
    // a color to it, in this case 4, to get the avg. color
    Color = Color / 4; 

    // returned the blurred color
    return Color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 BlurShader();
    }
}

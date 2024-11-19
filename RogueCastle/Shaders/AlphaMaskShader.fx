sampler ScreenSampler : register(s0);
sampler MaskSampler : register(s1);

// here we do the real work.
float4 PixelShaderFunction(float2 inCoord: TEXCOORD0) : COLOR
{
  // we retrieve the color in the original texture at
  // the current coordinate remember that this function
  // is run on every pixel in our texture.
  float4 color = tex2D(ScreenSampler, inCoord);
 
  // Since we are using a black and white mask the black
  // area will have a value of 0 and the white areas will
  // have a value of 255. Hence the black areas will subtract
  // nothing from our original color, and the white areas of
  // our mask will subtract all color from the color.
  color.rgba = color.rgba - tex2D(MaskSampler, inCoord).r;
 
  // return the new color of the pixel.
  return color;
}
 
technique
{
  pass P0
  {
    PixelShader = compile ps_2_0 PixelShaderFunction();
  }
}

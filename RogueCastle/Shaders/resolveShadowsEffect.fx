texture InputTexture; 
sampler inputSampler = sampler_state      
{
            Texture   = <InputTexture>;
            MipFilter = Point;
            MinFilter = Point;
            MagFilter = Point;
            AddressU  = Clamp;
            AddressV  = Clamp;
};

texture ShadowMapTexture; 
sampler shadowMapSampler = sampler_state      
{
            Texture   = <ShadowMapTexture>;
            MipFilter = Point;
            MinFilter = Point;
            MagFilter = Point;
            AddressU  = Clamp;
            AddressV  = Clamp;
};

float2 renderTargetSize;

struct VS_OUTPUT
{
    float4 Position  : POSITION;
    float2 TexCoords  : TEXCOORD0;
};

VS_OUTPUT FullScreenVS( float3 InPos  : POSITION,
						float2 InTex  : TEXCOORD0)
{
    VS_OUTPUT Out = (VS_OUTPUT)0;
    // Offset the position by half a pixel to correctly align texels to pixels
    Out.Position = float4(InPos,1) + 0.5f* float4(-1.0f/renderTargetSize.x, 1.0f/renderTargetSize.y, 0, 0);
    Out.TexCoords = InTex;
    return Out;
}

float4 ComputeDistancesPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
	  float4 color = tex2D(inputSampler, TexCoord);
	  //compute distance from center
	  float distance = color.a>0.3f?length(TexCoord - 0.5f):1.0f;
	  //save it to the Red channel
      return float4(distance,0,0,1);
}

float4 DistortPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
	  //translate u and v into [-1 , 1] domain
	  float u0 = TexCoord.x * 2 - 1;
	  float v0 = TexCoord.y * 2 - 1;
	  
	  //then, as u0 approaches 0 (the center), v should also approach 0 
	  v0 = v0 * abs(u0);

      //convert back from [-1,1] domain to [0,1] domain
	  v0 = (v0 + 1) / 2;

	  //we now have the coordinates for reading from the initial image
	  float2 newCoords = float2(TexCoord.x, v0);

	  //read for both horizontal and vertical direction and store them in separate channels
	  float horizontal = tex2D(inputSampler, newCoords).r;
	  float vertical = tex2D(inputSampler, newCoords.yx).r;
      return float4(horizontal,vertical ,0,1);
}


float GetShadowDistanceH(float2 TexCoord, float displacementV)
{
		float u = TexCoord.x;
		float v = TexCoord.y;

		u = abs(u-0.5f) * 2;
		v = v * 2 - 1;
		float v0 = v/u;
		v0+=displacementV;
		v0 = (v0 + 1) / 2;
		
		float2 newCoords = float2(TexCoord.x,v0);
		//horizontal info was stored in the Red component
		return tex2D(shadowMapSampler, newCoords).r;
}

float GetShadowDistanceV(float2 TexCoord, float displacementV)
{
		float u = TexCoord.y;
		float v = TexCoord.x;
		
		u = abs(u-0.5f) * 2;
		v = v * 2 - 1;
		float v0 = v/u;
		v0+=displacementV;
		v0 = (v0 + 1) / 2;
		
		float2 newCoords = float2(TexCoord.y,v0);
		//vertical info was stored in the Green component
		return tex2D(shadowMapSampler, newCoords).g;
}

float4 DrawShadowsPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
      // distance of this pixel from the center
      float distance = length(TexCoord - 0.5f);

      //distance stored in the shadow map
      float shadowMapDistance;

      //coords in [-1,1]
      float nY = 2.0f*( TexCoord.y - 0.5f);
      float nX = 2.0f*( TexCoord.x - 0.5f);

      //we use these to determine which quadrant we are in
      if(abs(nY)<abs(nX))
      {
        shadowMapDistance = GetShadowDistanceH(TexCoord,0);
      }
      else
      {
        shadowMapDistance = GetShadowDistanceV(TexCoord,0);
      }

      //if distance to this pixel is lower than distance from shadowMap, 
      //then we are not in shadow
      float light = distance < shadowMapDistance ? 1:0;

      float4 result = light;
      result.b = length(TexCoord - 0.5f);
      result.a = 1;
      return result;
}

static const float minBlur = 0.0f;
static const float maxBlur = 15.0f; // 5.0f;
static const int g_cKernelSize = 13;
static const float2 OffsetAndWeight[g_cKernelSize] =
{
    { -6, 0.002216 },
    { -5, 0.008764 },
    { -4, 0.026995 },
    { -3, 0.064759 },
    { -2, 0.120985 },
    { -1, 0.176033 },
    {  0, 0.199471 },
    {  1, 0.176033 },
    {  2, 0.120985 },
    {  3, 0.064759 },
    {  4, 0.026995 },
    {  5, 0.008764 },
    {  6, 0.002216 },
};


float4 BlurHorizontallyPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
	  float sum=0;
	  float distance = tex2D( inputSampler, TexCoord).b;
	  
      for (int i = 0; i < g_cKernelSize; i++)
	  {    
        sum += tex2D( inputSampler, TexCoord + OffsetAndWeight[i].x * lerp(minBlur, maxBlur , distance)/renderTargetSize.x * float2(1,0) ).r * OffsetAndWeight[i].y;
      }
	  
	  float4 result = sum;
	  result.b = distance;
	  result.a = 1;
      return result;
}

float4 BlurVerticallyPS(float2 TexCoord  : TEXCOORD0) : COLOR0
{
	  float sum=0;
	  float distance = tex2D( inputSampler, TexCoord).b;
	  
      for (int i = 0; i < g_cKernelSize; i++)
	  {    
        sum += tex2D( inputSampler, TexCoord + OffsetAndWeight[i].x * lerp(minBlur, maxBlur , distance)/renderTargetSize.x * float2(0,1) ).r * OffsetAndWeight[i].y;
      }
	  
	  float d = 2 * length(TexCoord - 0.5f);
	  float attenuation = pow( saturate(1.0f - d),1.0f);
	  
	  float4 result = sum * attenuation;
	  result.a = 1;
      return result;
}

technique ComputeDistances
{
    pass P0
    {          
        VertexShader = compile vs_2_0 FullScreenVS();
        PixelShader  = compile ps_2_0 ComputeDistancesPS();
    }
}

technique Distort
{
    pass P0
    {          
        VertexShader = compile vs_2_0 FullScreenVS();
        PixelShader  = compile ps_2_0 DistortPS();
    }
}

technique DrawShadows
{
    pass P0
    {          
        VertexShader = compile vs_2_0 FullScreenVS();
        PixelShader  = compile ps_2_0 DrawShadowsPS();
    }
}

technique BlurHorizontally
{
    pass P0
    {          
        VertexShader = compile vs_2_0 FullScreenVS();
        PixelShader  = compile ps_2_0 BlurHorizontallyPS();
    }
}

technique BlurVerticallyAndAttenuate
{
    pass P0
    {          
        VertexShader = compile vs_2_0 FullScreenVS();
        PixelShader  = compile ps_2_0 BlurVerticallyPS();
    }
}
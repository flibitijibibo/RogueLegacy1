//-----------------------------------------------------------------------------
// PostprocessEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Settings controlling the edge detection filter.
float EdgeWidth = 1;
float EdgeIntensity = 1;

// How sensitive should the edge detection be to tiny variations in the input data?
// Smaller settings will make it pick up more subtle edges, while larger values get
// rid of unwanted noise.
float NormalThreshold = 0.5;
float DepthThreshold = 0.1;

// How dark should the edges get in response to changes in the input data?
float NormalSensitivity = 1;
float DepthSensitivity = 10;

// How should the sketch effect respond to changes of brightness in the input scene?
float SketchThreshold = 0.1;
float SketchBrightness = 0.333;

// Randomly offsets the sketch overlay pattern to create a hand-drawn animation effect.
float2 SketchJitter;

// Pass in the current screen resolution.
float2 ScreenResolution;


// This texture contains the main scene image, which the edge detection
// and/or sketch filter are being applied over the top of.
texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains normals (in the color channels) and depth (in alpha)
// for the main scene image. Differences in the normal and depth data are used
// to detect where the edges of the model are.
texture NormalDepthTexture;

sampler NormalDepthSampler : register(s1) = sampler_state
{
    Texture = (NormalDepthTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains an overlay sketch pattern, used to create the hatched
// pencil drawing effect.
texture SketchTexture;

sampler SketchSampler : register(s2) = sampler_state
{
    Texture = (SketchTexture);

    AddressU = Wrap;
    AddressV = Wrap;
};


// Pixel shader applies the edge detection and/or sketch filter postprocessing.
// It is compiled several times using different settings for the uniform boolean
// parameters, producing different optimized versions of the shader depending on
// which combination of processing effects is desired.
float4 PixelShaderFunction(float2 texCoord : TEXCOORD0, uniform bool applyEdgeDetect,
                                                        uniform bool applySketch,
                                                        uniform bool sketchInColor) : COLOR0
{
    // Look up the original color from the main scene.
    float3 scene = tex2D(SceneSampler, texCoord);
    
    // Apply the sketch effect?
    if (applySketch)
    {
        // Adjust the scene color to remove very dark values and increase the contrast.
        float3 saturatedScene = saturate((scene - SketchThreshold) * 2);
        
        // Look up into the sketch pattern overlay texture.
        float3 sketchPattern = tex2D(SketchSampler, texCoord + SketchJitter);
    
        // Convert into negative color space, and combine the scene color with the
        // sketch pattern. We need to do this multiply in negative space to get good
        // looking results, because pencil sketching works by drawing black ink
        // over an initially white page, rather than adding light to an initially
        // black background as would be more common in computer graphics.
        float3 negativeSketch = (1 - saturatedScene) * (1 - sketchPattern);
        
        // Convert the result into a positive color space greyscale value.
        float sketchResult = dot(1 - negativeSketch, SketchBrightness);
        
        // Apply the sketch result to the main scene color.
        if (sketchInColor)
            scene *= sketchResult;
        else
            scene = sketchResult;
    }
    
    // Apply the edge detection filter?
    if (applyEdgeDetect)
    {
        // Look up four values from the normal/depth texture, offset along the
        // four diagonals from the pixel we are currently shading.
        float2 edgeOffset = EdgeWidth / ScreenResolution;
        
        float4 n1 = tex2D(NormalDepthSampler, texCoord + float2(-1, -1) * edgeOffset);
        float4 n2 = tex2D(NormalDepthSampler, texCoord + float2( 1,  1) * edgeOffset);
        float4 n3 = tex2D(NormalDepthSampler, texCoord + float2(-1,  1) * edgeOffset);
        float4 n4 = tex2D(NormalDepthSampler, texCoord + float2( 1, -1) * edgeOffset);

        // Work out how much the normal and depth values are changing.
        float4 diagonalDelta = abs(n1 - n2) + abs(n3 - n4);

        float normalDelta = dot(diagonalDelta.xyz, 1);
        float depthDelta = diagonalDelta.w;
        
        // Filter out very small changes, in order to produce nice clean results.
        normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
        depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

        // Does this pixel lie on an edge?
        float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;
        
        // Apply the edge detection result to the main scene color.
        scene *= (1 - edgeAmount);
    }

    return float4(scene, 1);
}


// Compile the pixel shader for doing edge detection without any sketch effect.
technique EdgeDetect
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction(true, false, false);
    }
}

// Compile the pixel shader for doing edge detection with a monochrome sketch effect.
technique EdgeDetectMonoSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction(true, true, false);
    }
}

// Compile the pixel shader for doing edge detection with a colored sketch effect.
technique EdgeDetectColorSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction(true, true, true);
    }
}

// Compile the pixel shader for doing a monochrome sketch effect without edge detection.
technique MonoSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction(false, true, false);
    }
}

// Compile the pixel shader for doing a colored sketch effect without edge detection.
technique ColorSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction(false, true, true);
    }
}

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TexCoord0;    
};
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TexCoord0;	
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    
    output.Position = float4(input.Position.xyz,1);
    output.TexCoord = input.TexCoord;        
    
    return output;
}


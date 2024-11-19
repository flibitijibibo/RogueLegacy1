// shockwave.fx
sampler samplerState; 

float xcenter = 0.5;
float ycenter = 0.5;
float mag = 0;
float width = 0;

float4 Shockwave(float2 texCoord: TEXCOORD) : COLOR0 
{ 
	float4 col = 0.0;
	float2 tex = texCoord;
	float xdif = tex.x - xcenter;
	float ydif = tex.y - ycenter;
	float d = sqrt(xdif * xdif + ydif * ydif) - width;
	float t = abs(d);
	
	if (d < 0.1 && d > -0.2) 
	{
		if (d < 0.0) 
		{
			t = (0.2 - t) / 2.0;
			tex.x = tex.x - (xdif * t * mag);
			tex.y = tex.y - (ydif * t * mag);
			col = tex2D(samplerState, tex);
		} 
		else 
		{
			t = (0.1 - t);
			tex.x = tex.x - (xdif * t * mag);
			tex.y = tex.y - (ydif * t * mag);
			col = tex2D(samplerState, tex);
		}
		
	//	col.a = t * 12.0;
	} 
	
	col = tex2D(samplerState, tex);	
	return col; 
}



technique Technique1 { 
	pass P0{ 
		PixelShader = compile ps_2_0 Shockwave(); 
	} 
	
}
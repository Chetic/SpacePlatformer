float test;

sampler s0;
float4 PixelShaderFunction(float2 coord: TEXCOORD0) : COLOR0
{
	float4 color = tex2D(s0, coord);
	return color;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader  = compile ps_2_0 PixelShaderFunction();
		//VertexShader =  compile vs_2_0 VertexShaderFunction();
	}
}

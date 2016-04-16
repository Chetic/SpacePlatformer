float4 Color;

float4 PixelShaderFunction(void) : COLOR0
{
	float4 color = float4(0,1,1,1);

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

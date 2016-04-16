sampler s0;
texture lightMask;
sampler lightSampler = sampler_state{Texture = lightMask;};
float blurSize = 1.01;

float4 PixelShaderFunction(float2 coord: TEXCOORD0) : COLOR0
{/*
   float4 sum = float4(0.0, 0.0, 0.0, 0.0);

   // blur in y (vertical)
   // take nine samples, with the distance blurSize between them
   sum += tex2D(s0, float2(coord.x - 4.0*blurSize, coord.y)) * 0.05;
   sum += tex2D(s0, float2(coord.x - 3.0*blurSize, coord.y)) * 0.09;
   sum += tex2D(s0, float2(coord.x - 2.0*blurSize, coord.y)) * 0.12;
   sum += tex2D(s0, float2(coord.x - blurSize, coord.y)) * 0.15;
   sum += tex2D(s0, float2(coord.x, coord.y)) * 0.16;
   sum += tex2D(s0, float2(coord.x + blurSize, coord.y)) * 0.15;
   sum += tex2D(s0, float2(coord.x + 2.0*blurSize, coord.y)) * 0.12;
   sum += tex2D(s0, float2(coord.x + 3.0*blurSize, coord.y)) * 0.09;
   sum += tex2D(s0, float2(coord.x + 4.0*blurSize, coord.y)) * 0.05;

   return sum;
   */
    float4 color = tex2D(s0, coord);
    float4 lightColor = tex2D(lightSampler, coord);
    return (color/10.0) + color * lightColor;
}
  
technique Technique1  
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

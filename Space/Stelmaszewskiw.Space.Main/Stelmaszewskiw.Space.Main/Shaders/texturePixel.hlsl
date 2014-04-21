////////////////////////////////////
////	GLOBALS
////////////////////////////////////
Texture2D shaderTexture;
SamplerState SamplerType;

////////////////////////////////////
////	TYPES
////////////////////////////////////
struct PixelInputType
{
	float4 position: SV_POSITION;
	float2 tex: TEXCOORD;
};

////////////////////////////////////
////	PIXEL SHADER
////////////////////////////////////
float4 TexturePixelShader(PixelInputType input) : SV_TARGET
{
	float4 textureColor;

	//Sample the pixel color from the texture using the sampler at this texture coordinate location.
	textureColor = shaderTexture.Sample(SamplerType, input.tex);

	return textureColor;
}
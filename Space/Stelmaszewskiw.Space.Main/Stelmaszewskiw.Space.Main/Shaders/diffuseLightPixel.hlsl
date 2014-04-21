////////////////////////////////////
////	GLOBALS
////////////////////////////////////
Texture2D shaderTexture;
SamplerState SamplerType;

cbuffer lightBuffer
{
	float4 diffuseColor;
	float3 lightDirection;
	float padding; //Because buffer must have size 16n bytes.
};

////////////////////////////////////
////	TYPES
////////////////////////////////////
struct PixelInputType
{
	float4 position: SV_POSITION;
	float2 tex: TEXCOORD;
	float3 normal: NORMAL;
};

////////////////////////////////////
////	PIXEL SHADER
////////////////////////////////////
float4 DiffuseLightPixelShader(PixelInputType input) : SV_TARGET
{
	float4 textureColor;
	float3 lightDir;
	float lightIntensity;
	float4 lightColor;

	//Sample the pixel color from the texture using the sampler at this texture coordinate location.
	textureColor = shaderTexture.Sample(SamplerType, input.tex);

	//Invert the light direction for calculations.
	lightDir = -lightDirection;

	//Calculate the amount of the light on this pixel.
	lightIntensity = saturate(dot(input.normal, lightDir));

	//Determine the final amount of diffuse color based on the diffuse color combined with the light intensity.
	lightColor = saturate(diffuseColor * lightIntensity);

	//Multiply the texture pixel and the final diffuse color to get the final pixel color result.
	lightColor = lightColor * textureColor;

	return lightColor;
}
////////////////////////////////////
////	GLOBALS
////////////////////////////////////
cbuffer matrixBuffer
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projectionMatrix;
};

////////////////////////////////////
////	TYPES
////////////////////////////////////
struct VertexInputType
{
	float4 position: POSITION;
	float2 tex: TEXCOORD;
	float3 normal: NORMAL;
};

struct PixelInputType
{
	float4 position: SV_POSITION;
	float2 tex: TEXCOORD;
	float3 normal: NORMAL;
};

////////////////////////////////////
////	VERTEX SHADER
////////////////////////////////////
PixelInputType DiffuseLightVertexShader(VertexInputType input)
{
	PixelInputType output;

	//Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;

	//Calculate the position of the vertex agains the world, view and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	//Store the texture coordinates for the pixel shader to use.
	output.tex = input.tex;

	//Calculate the normal vector against the world matrix only.
	output.normal = mul(input.normal, (float3x3)worldMatrix); //Because only rotation matters.

	//Normalize the normal vector. (If input was normalize, output is also - rotation does not change vectors lengths.
	output.normal = normalize(output.normal);

	return output;
}
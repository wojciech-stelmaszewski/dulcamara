﻿////////////////////////////////////
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
	float4 color: COLOR;
};

struct PixelInputType
{
	float4 position: SV_POSITION;
	float4 color: COLOR;
};

////////////////////////////////////
////	VERTEX SHADER
////////////////////////////////////
PixelInputType ColorVertexShader(VertexInputType input)
{
	PixelInputType output;

	//Change the position vector to be 4 units for proper matrix calculations.
	input.position.w = 1.0f;

	//Calculate the position of the vertex agains the world, view and projection matrices.
	output.position = mul(input.position, worldMatrix);
	output.position = mul(output.position, viewMatrix);
	output.position = mul(output.position, projectionMatrix);

	//Store the input color for the pixel shader to use.
	output.color = input.color;

	return output;
}
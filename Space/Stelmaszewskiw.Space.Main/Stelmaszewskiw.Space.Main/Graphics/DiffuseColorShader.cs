using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NLog;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Stelmaszewskiw.Space.Main.Graphics
{
    public class DiffuseColorShader : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Vertex
        {
            public const int AppendAlignedElement1 = 12;
            public const int AppendAlignedElement2 = 20;

            public Vector3 Position;
            public Vector2 Texture;
            public Vector3 Normal;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MatrixBuffer
        {
            public Matrix World;
            public Matrix View;
            public Matrix Projection;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LightBuffer
        {
            public Vector4 DiffuseColor;
            public Vector3 LightDirection;
            public float padding; //Added extra padding so structure size is a multiple of 16 bytes for CreateBuffer function requirements.
        }

        private SharpDX.Direct3D11.VertexShader VertexShader { get; set; }
        private SharpDX.Direct3D11.PixelShader PixelShader { get; set; }
        private SharpDX.Direct3D11.InputLayout InputLayout { get; set; }
        private SharpDX.Direct3D11.Buffer ConstantMatrixBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer ConstantLightBuffer { get; set; }
        private SharpDX.Direct3D11.SamplerState SamplerState { get; set; }

        //The name of vertex shader function name.
        private const string VertexShaderName = "DiffuseLightVertexShader";
        //The name of pixel shader function name.
        private const string PixelShaderName = "DiffuseLightPixelShader";

        //My GraphicsDevice do not support vs_5_0. :(
        public const string VertexShaderVersion = "vs_4_0";
        //My GraphicsDevice do not support ps_5_0. :(
        public const string PixelShaderVersion = "ps_4_0";

        //TODO Should be a parameter. (We should copy shader files to output directory !?).
        private const string VertexShaderFilename = "Shaders/diffuseLightVertex.hlsl";

        //TODO Should be a parameter. (We should copy shader files to output directory !?).
        private const string PixelShaderFilename = "Shaders/diffuseLightPixel.hlsl";

        private readonly Logger logger;

        public DiffuseColorShader()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        private bool SetShaderParameters(SharpDX.Direct3D11.DeviceContext deviceContext, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix, 
            ShaderResourceView texture, Vector3 lightDirection, Color4 diffuseColor)
        {
            try
            {
                //Transpose the matrices to prepare them for shader.
                worldMatrix.Transpose();
                viewMatrix.Transpose();
                projectionMatrix.Transpose();

                //Lock the constant buffer so it can be written to.
                DataStream mappedResource;
                deviceContext.MapSubresource(ConstantMatrixBuffer, MapMode.WriteDiscard, MapFlags.None,
                                             out mappedResource);

                //Copy the matrices into the constant buffer.
                var matrixBuffer = new SolidColorShader.MatrixBuffer
                                       {
                                           World = worldMatrix,
                                           View = viewMatrix,
                                           Projection = projectionMatrix
                                       };
                mappedResource.Write(matrixBuffer);

                //Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantMatrixBuffer, 0);

                //Set the position of the constant buffer in the vertex shader.
                var bufferNumber = 0;

                //Finally set the constant buffer in the vertex shader with the uploaded values.
                deviceContext.VertexShader.SetConstantBuffer(bufferNumber, ConstantMatrixBuffer);

                //Set shader resource in the pixel shader.
                deviceContext.PixelShader.SetShaderResource(0, texture);

                //Lock the light constant buffer so it can be written to.
                deviceContext.MapSubresource(ConstantLightBuffer, MapMode.WriteDiscard, MapFlags.None,
                                             out mappedResource);

                //Copy tge lighting variables into the constant buffer.
                var lightBuffer = new LightBuffer
                                      {
                                          DiffuseColor = diffuseColor.ToVector4(),
                                          LightDirection = lightDirection,
                                          padding = 0
                                      };

                mappedResource.Write(lightBuffer);

                //Unlock the constant buffer.
                deviceContext.UnmapSubresource(ConstantLightBuffer, 0);

                //Set the position of the light constant buffer in the pixel shader.
                bufferNumber = 0;

                //Finally set the light constant buffer in the pixel shader with the updated values.
                deviceContext.PixelShader.SetConstantBuffer(bufferNumber, ConstantLightBuffer);

                return true;
            }
            catch (Exception exception)
            {
                logger.FatalException("Setting shader parameters failed.", exception);
                return false;
            }
        }

        public bool Render(DeviceContext deviceContext, int indexCount, Matrix worldMatrix, Matrix viewMatrix, Matrix projectionMatrix,
            ShaderResourceView texture, Vector3 lightDirection, Color4 diffuseColor)
        {
           //Set the shader parameters that it will use for rendering.
            if(!SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix, 
                texture, lightDirection, diffuseColor))
            {
                return false;
            }

            //Now render the prepared buffers with the shader.
            RenderShader(deviceContext, indexCount);

            return true;
        }

        private void RenderShader(DeviceContext deviceContext, int indexCount)
        {
            //Set the vertex input layout.
            deviceContext.InputAssembler.InputLayout = InputLayout;

            //Set the vertex and pixel shaders that will be used to render this triangle.
            deviceContext.VertexShader.Set(VertexShader);
            deviceContext.PixelShader.Set(PixelShader);

            //Set the sampler state in the pixel shader.
            deviceContext.PixelShader.SetSampler(0, SamplerState);
            //Render the triangle.
            deviceContext.DrawIndexed(indexCount, 0, 0);
        }

        public bool Initialize(SharpDX.Direct3D11.Device device)
        {
            return InitializeShader(device, VertexShaderFilename, PixelShaderFilename);
        }

        public void Dispose()
        {
            //Shutdown the vertex and pixel shader as well as the related objects.
            ShutdownShader();
        }

        private void ShutdownShader()
        {
            //Release the light constant buffer.
            if(ConstantLightBuffer != null)
            {
                ConstantLightBuffer.Dispose();
                ConstantLightBuffer = null;
            }

            //Release the sampler state.
            if(SamplerState != null)
            {
                SamplerState.Dispose();
                SamplerState = null;
            }

            //Release the matrix constant buffer.
            if(ConstantMatrixBuffer != null)
            {
                ConstantMatrixBuffer.Dispose();
                ConstantMatrixBuffer = null;
            }

            //Release the layout,
            if(InputLayout != null)
            {
                InputLayout.Dispose();
                InputLayout = null;
            }

            //Release the pixel shader.
            if(PixelShader != null)
            {
                PixelShader.Dispose();
                PixelShader = null;
            }

            //Release the vertex shader.
            if(VertexShader != null)
            {
                VertexShader.Dispose();
                VertexShader = null;
            }
        }

        private bool InitializeShader(SharpDX.Direct3D11.Device device, string vertexShaderFilename, string pixelShaderFilename)
        {
            try
            {
                logger.Debug("Compiling vertex shader from file '{0}'...", vertexShaderFilename);
                //Compile the vertex shader code.
                //After compiling and usinging vertex shader code should be release, since it is no longer needed.
                using (var vertexShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(vertexShaderFilename,
                                                                                              VertexShaderName,
                                                                                              VertexShaderVersion,
                                                                                              ShaderFlags.EnableStrictness,
                                                                                              EffectFlags.None)
                                                                                              )
                {
                    //Create the vertex shader from the buffer.
                    VertexShader = new VertexShader(device, vertexShaderByteCode);

                    logger.Debug("Compiling pixel shader from file '{0}'...", vertexShaderFilename);
                    //Compile the pixel shader code.
                    //After compiling and usinging pixel shader code should be release, since it is no longer needed.
                    using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile(pixelShaderFilename, PixelShaderName,
                                                                             PixelShaderVersion, ShaderFlags.EnableStrictness,
                                                                             EffectFlags.None)
                                                                             )
                    {
                        //Create the pixel shader from the buffer.
                        PixelShader = new PixelShader(device, pixelShaderByteCode);
                    }


                    //Now setup the layout of the data that goes into the shader.
                    //This setup needs to match the VertexType structure in the Model an in the shader.
                    var inputElements = new[]
                                            {
                                                new InputElement
                                                    {
                                                        SemanticName = "POSITION",
                                                        SemanticIndex = 0,
                                                        Format = Format.R32G32B32A32_Float,
                                                        Slot = 0,
                                                        AlignedByteOffset = 0,
                                                        Classification = InputClassification.PerVertexData,
                                                        InstanceDataStepRate = 0
                                                    },
                                                new InputElement
                                                    {
                                                        SemanticName = "TEXCOORD",
                                                        SemanticIndex = 0,
                                                        Format = Format.R32G32_Float,
                                                        Slot = 0,
                                                        AlignedByteOffset = Vertex.AppendAlignedElement1,
                                                        Classification = InputClassification.PerVertexData,
                                                        InstanceDataStepRate = 0
                                                    },
                                                new InputElement
                                                    {
                                                        SemanticName = "NORMAL",
                                                        SemanticIndex = 0,
                                                        Format = Format.R32G32B32_Float,
                                                        Slot = 0,
                                                        AlignedByteOffset = Vertex.AppendAlignedElement2,
                                                        Classification = InputClassification.PerVertexData,
                                                        InstanceDataStepRate = 0
                                                    } 
                                            };

                    InputLayout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode),
                                                  inputElements);

                }


                //Setup the discription of the dynamic matrix constant buffer that is in the vertex shader.
                var matrixBufferDescription = new BufferDescription
                                                  {
                                                      Usage = ResourceUsage.Dynamic,
                                                      SizeInBytes = Utilities.SizeOf<DiffuseColorShader.MatrixBuffer>(),
                                                      BindFlags = BindFlags.ConstantBuffer,
                                                      CpuAccessFlags = CpuAccessFlags.Write,
                                                      OptionFlags = ResourceOptionFlags.None,
                                                      StructureByteStride = 0
                                                  };


                //Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
                ConstantMatrixBuffer = new SharpDX.Direct3D11.Buffer(device, matrixBufferDescription);

                //Create a texture sampler state description.
                var samplerStateDescription = new SamplerStateDescription
                                                  {
                                                      Filter = Filter.MinMagMipLinear,
                                                      AddressU = TextureAddressMode.Wrap,
                                                      AddressV = TextureAddressMode.Wrap,
                                                      AddressW = TextureAddressMode.Wrap,
                                                      MipLodBias = 0,
                                                      MaximumAnisotropy = 1,
                                                      ComparisonFunction = Comparison.Always,
                                                      BorderColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f),
                                                      MaximumLod = 0,
                                                      MinimumLod = 0
                                                  };
                
                //Create the texture sampler state.
                SamplerState = new SamplerState(device, samplerStateDescription);

                //Setup the description of the light dynamic constant buffer thet is used in the pixel shader.
                //Note that ByteWidth always needs to be a multiple of 16 bytes if using D3D11_BIND_CONSTANT_BUFFER or CreateBuffer will fail.
                var lightBufferDescription = new BufferDescription
                                                 {
                                                     Usage = ResourceUsage.Dynamic,
                                                     SizeInBytes = Utilities.SizeOf<LightBuffer>(),
                                                     BindFlags = BindFlags.ConstantBuffer,
                                                     CpuAccessFlags = CpuAccessFlags.Write,
                                                     OptionFlags = ResourceOptionFlags.None,
                                                     StructureByteStride = 0
                                                 };

                //Create the constant buffer pointer so we can access the pixel shader constant buffer from within this class.
                ConstantLightBuffer = new SharpDX.Direct3D11.Buffer(device, lightBufferDescription);

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Error initializing shader. '{0}'", exception));
                logger.FatalException("Error initializing shader.", exception);
                return false;
            }
        }
    }
}

using System;
using NLog;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Stelmaszewskiw.Space.Main.Graphics;

namespace Stelmaszewskiw.Space.Main
{
    public class SimpleModel : IDisposable
    {
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }

        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }

        public Texture Texture { get; private set; }

        private readonly Logger logger;

        public SimpleModel()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public bool Initialize(SharpDX.Direct3D11.Device device, string textureFilename)
        {
            //Initialize the vertex buffer that hold the geometry for the triangle.
            if(!InitializeBuffers(device))
            {
                return false;
            }

            //Load the texture object.
            if(!LoadTexture(device, textureFilename))
            {
                return false;
            }

            return true;
        }

        public void Render(DeviceContext deviceContext)
        {
            //Put the vertex and index buffers on the graphics pipeline to prepare for drawings.
            RenderBuffers(deviceContext);
        }

        public void Dispose()
        {
            Shutdown();
        }

        private bool LoadTexture(SharpDX.Direct3D11.Device device, string textureFilename)
        {
            //Create the texture object.
            Texture = new Texture();

            //Initialize the texture object.
            Texture.Initialize(device, textureFilename);

            return true;
        }

        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                //Set number of vertices in the vertex array.
                VertexCount = 4;
                //Set number of indices in the index array.
                IndexCount = 6;

                //Create the vertex array and load it with data.
                var vertices = new[]
                                   {
                                       ////Bottom left.
                                       new DiffuseColorShader.Vertex
                                           {
                                               Position = new Vector3(-1.0f, -1.0f, 0.0f),
                                               Texture = new Vector2(0.0f, 1.0f),
                                               Normal = -Vector3.UnitZ
                                           },
                                       ////Bottom right.
                                       new DiffuseColorShader.Vertex
                                           {
                                               Position = new Vector3(1.0f, -1.0f, 0.0f),
                                               Texture = new Vector2(1.0f, 1.0f),
                                               Normal = -Vector3.UnitZ
                                           },
                                       ////Top right.
                                       new DiffuseColorShader.Vertex
                                           {
                                               Position = new Vector3(1.0f, 1.0f, 0.0f),
                                               Texture = new Vector2(1.0f, 0.0f),
                                               Normal = -Vector3.UnitZ
                                           },
                                       ////Top left.
                                       new DiffuseColorShader.Vertex
                                           {
                                               Position = new Vector3(-1.0f, 1.0f, 0.0f),
                                               Texture = new Vector2(0.0f, 0.0f),
                                               Normal = -Vector3.UnitZ
                                           },
                                       
                                   };

                var indices = new[]
                                  {
                                      0, 2, 1, 
                                      0, 3, 2
                                  };

                //Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                //Create the index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch (Exception exception)
            {
                logger.FatalException("Initializing model buffers failed.", exception);
                return false;
            }
        }

        private void Shutdown()
        {
            //Release the model texture.
            ReleaseTexture();

            //Relese the vertex and index buffers.
            ShutdownBuffers();
        }

        private void ReleaseTexture()
        {
            //Release the texture object.
            if(Texture != null)
            {
                Texture.Dispose();
                Texture = null;
            }
        }

        private void ShutdownBuffers()
        {
            //Release the index buffer.
            if(IndexBuffer != null)
            {
                IndexBuffer.Dispose();
                IndexBuffer = null;
            }

            //Release the vertex buffer.
            if(VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }
        }

        private void RenderBuffers(DeviceContext deviceContext)
        {
            //Set the vertex buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetVertexBuffers(0,
                                                          new VertexBufferBinding(VertexBuffer,
                                                                                  Utilities.SizeOf
                                                                                      <DiffuseColorShader.Vertex>(), 0));

            //Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            //Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles. 
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}

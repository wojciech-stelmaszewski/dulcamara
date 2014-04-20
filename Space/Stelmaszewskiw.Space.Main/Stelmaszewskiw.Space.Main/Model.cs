using System;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Stelmaszewskiw.Space.Main.Graphics;

namespace Stelmaszewskiw.Space.Main
{
    public class Model : IDisposable
    {
        private SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }
        private SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }

        private int VertexCount { get; set; }
        public int IndexCount { get; private set; }

        public bool Initialize(SharpDX.Direct3D11.Device device)
        {
            //Initialize the vertex buffer that hold the geometry for the triangle.
            return InitializeBuffers(device);
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

        private bool InitializeBuffers(SharpDX.Direct3D11.Device device)
        {
            try
            {
                //Set number of vertices in the vertex array.
                VertexCount = 4;
                //Set number of indices in the index array.
                IndexCount = 6;

                var firstVertexColor = new SharpDX.Vector4(0.5843137255f, 0.7294117647f, 0.2352941176f, 1.0f);
                var secondVertexColor = new SharpDX.Vector4(0.9411764706f, 0.5568627451f, 0.4901960784f, 1.0f);

                //Create the vertex array and load it with data.
                var vertices = new[]
                                   {
                                       ////Bottom left.
                                       //new SolidColorShader.Vertex
                                       //    {
                                       //        Position = new Vector3(-1.0f, -1.0f, 0.0f),
                                       //        Color = firstVertexColor
                                       //    },
                                       ////Top middle.
                                       //new SolidColorShader.Vertex
                                       //    {
                                       //        Position = new Vector3(0.0f, 1.0f, 0.0f),
                                       //        Color = secondVertexColor                                           
                                       //    },
                                       ////Bottom right.
                                       //new SolidColorShader.Vertex
                                       //    {
                                       //        Position = new Vector3(1.0f, -1.0f, 0.0f),
                                       //        Color = firstVertexColor
                                       //    }

                                       ////Bottom left.
                                       new SolidColorShader.Vertex
                                           {
                                               Position = new Vector3(-1.0f, -1.0f, 0.0f),
                                               Color = secondVertexColor
                                           },
                                       ////Bottom right.
                                       new SolidColorShader.Vertex
                                           {
                                               Position = new Vector3(1.0f, -1.0f, 0.0f),
                                               Color = firstVertexColor
                                           },
                                       ////Top right.
                                       new SolidColorShader.Vertex
                                           {
                                               Position = new Vector3(1.0f, 1.0f, 0.0f),
                                               Color = secondVertexColor
                                           },
                                       ////Top left.
                                       new SolidColorShader.Vertex
                                           {
                                               Position = new Vector3(-1.0f, 1.0f, 0.0f),
                                               Color = firstVertexColor
                                           },
                                       
                                   };

                var indices = new[]
                                  {
                                      0, 2, 1, 
                                      0, 3, 2
                                      //0, //Bottom left.
                                      //1, //Top middle.
                                      //2 //Bottom right.
                                  };

                //Create the vertex buffer.
                VertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

                //Create the index buffer.
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indices);

                return true;
            }
            catch (Exception)
            {
                //TODO Log the error.
                return false;
            }
        }

        private void Shutdown()
        {
            //Relese the vertex and index buffers.
            ShutdownBuffers();
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
                                                                                      <SolidColorShader.Vertex>(), 0));

            //Set the index buffer to active in the input assembler so it can be rendered.
            deviceContext.InputAssembler.SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            //Set the type of the primitive that should be rendered from this vertex buffer, in this case triangles. 
            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
        }
    }
}

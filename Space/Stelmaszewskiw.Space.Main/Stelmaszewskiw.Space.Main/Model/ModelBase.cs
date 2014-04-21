using System;
using Stelmaszewskiw.Space.Main.Engine;

namespace Stelmaszewskiw.Space.Main.Model
{
    public class ModelBase : IRenderable
    {
        protected SharpDX.Direct3D11.Buffer vertexBuffer;
        protected SharpDX.Direct3D11.Buffer indexBuffer;



        private bool InitializeBuffers()
        {
            return false;
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}

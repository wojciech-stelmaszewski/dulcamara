using System;
using NLog;
using SharpDX.Direct3D11;

namespace Stelmaszewskiw.Space.Main
{
    public class Texture: IDisposable
    {
        public ShaderResourceView TextureResource { get; private set; }

        private readonly Logger logger;

        public Texture()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public bool Initialize(SharpDX.Direct3D11.Device device, string filename)
        {
            try
            {
                //Load the texture file.
                TextureResource = ShaderResourceView.FromFile(device, filename);
                return true;
            }
            catch (Exception exception)
            {
                logger.DebugException("Loading texture failed.", exception);
                return false;
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            //Release the texture resource.
            if(TextureResource != null)
            {
                TextureResource.Dispose();
                TextureResource = null;
            }
        }
    }
}

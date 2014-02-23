using System;

namespace Stelmaszewskiw.Space.Main
{
    public class GraphicsManager
    {
        public GraphicsManager(SystemConfiguration systemConfiguration, IntPtr windowPointer)
        {
            Initialize(systemConfiguration, windowPointer);
        }

        public bool Initialize(SystemConfiguration systemConfiguration, IntPtr windowPointer)
        {
            return true;
        }

        public void Shutdown()
        {
            
        }

        public bool Frame()
        {
            return true;
        }

        public bool Render()
        {
            return true;
        }
    }
}

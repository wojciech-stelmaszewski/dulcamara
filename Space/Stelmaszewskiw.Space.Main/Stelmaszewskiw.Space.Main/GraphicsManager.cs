using System;
using System.Windows.Forms;

namespace Stelmaszewskiw.Space.Main
{
    public class GraphicsManager : IDisposable
    {
        private DX3D11 dx3d11;

        public GraphicsManager(SystemConfiguration systemConfiguration, IntPtr windowPointer)
        {
            Initialize(systemConfiguration, windowPointer);
        }

        public bool Initialize(SystemConfiguration systemConfiguration, IntPtr windowPointer)
        {
            try
            {
                //Create the Direct3D object.
                dx3d11 = new DX3D11();
                //Initialize the Direct3D object.
                dx3d11.Initialize(systemConfiguration, windowPointer);
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Could not initialize Direct3D\nError is '{0}'", exception.Message));
                return false;
            }

            return true;
        }

        public bool Frame()
        {
            //Render the graphics scene.
            return Render();
        }

        public bool Render()
        {
            //Clear buffer to begin the scene.
            dx3d11.BeginScene();


            //Present the rendered scene to the screen.
            dx3d11.EndScene();

            return true;
        }

        public void Dispose()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            if(dx3d11 == null)
            {
                return;
            }

            dx3d11.Dispose();
            dx3d11 = null;
        }
    }
}

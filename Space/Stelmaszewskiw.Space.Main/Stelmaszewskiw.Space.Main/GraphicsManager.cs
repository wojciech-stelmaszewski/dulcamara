using System;
using System.Windows.Forms;
using SharpDX;
using Stelmaszewskiw.Space.Main.Graphics;

namespace Stelmaszewskiw.Space.Main
{
    public class GraphicsManager : IDisposable
    {
        private DX3D11 dx3d11;

        private Camera.Camera camera;
        private Model model;
        private Graphics.SolidColorShader solidColorShader;

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
                if(!dx3d11.Initialize(systemConfiguration, windowPointer))
                {
                    return false;
                }

                //Create camera object.
                camera = new Camera.Camera();

                //Set the initial position of the camera.
                camera.Position = new Vector3(0.0f, 0.0f, -5.0f);

                //Create the model object.
                model = new Model();

                //Initialize the model object.
                if(!model.Initialize(dx3d11.Device))
                {
                    return false;
                }

                //Crate the solid color shader object.
                solidColorShader = new SolidColorShader();

                //Initialize the color shader object.
                if(!solidColorShader.Initialize(dx3d11.Device))
                {
                    return false;
                }

                return true;
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

            //Generate the view matrix based on the camera position.
            camera.Render();

            //Get the world, view and projection matrices from the camera and dx3d11 objects.
            var worldMatrix = dx3d11.WorldMatrix;
            var viewMatrix = camera.ViewMatrix;
            var projectionMatrix = dx3d11.ProjectionMatrix;

            //Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            model.Render(dx3d11.DeviceContext);

            //Render the model using the solid color shader.
            if(!solidColorShader.Render(dx3d11.DeviceContext, model.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
            {
                return false;
            }

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
            //Release the solid color shader object.
            if(solidColorShader != null)
            {
                solidColorShader.Dispose();
                solidColorShader = null;
            }

            //Release the model object.
            if(model != null)
            {
                model.Dispose();
                model = null;
            }

            //Release the camera object.
            if(camera != null)
            {
                camera = null;
            }

            //Release the Direct3D object.
            if(dx3d11 == null)
            {
                return;
            }

            dx3d11.Dispose();
            dx3d11 = null;
        }
    }
}

using System;
using System.Windows.Forms;
using NLog;
using SharpDX;
using Stelmaszewskiw.Space.Main.Graphics;

namespace Stelmaszewskiw.Space.Main
{
    public class GraphicsManager : IDisposable
    {
        private DX3D11 dx3d11;

        private Camera.Camera camera;
        private SimpleModel model;
        private Graphics.SolidColorShader solidColorShader;
        private Graphics.TextureShader textureShader;
        private Graphics.DiffuseColorShader diffuseColorShader;
        private DirectionalLight directionalLight;


        private float rotation;

        private readonly Logger logger;

        private const string ModelTextureFilename = "texture.dds";

        public GraphicsManager(SystemConfiguration systemConfiguration, IntPtr windowPointer)
        {
            logger = LogManager.GetCurrentClassLogger();

            Initialize(systemConfiguration, windowPointer);

            RegisterCoreElementsInContainer();
        }

        private void RegisterCoreElementsInContainer()
        {
            Container.Kernel.Bind<SharpDX.Direct3D11.Device>().ToConstant(dx3d11.Device);
            Container.Kernel.Bind<SharpDX.Direct3D11.DeviceContext>().ToConstant(dx3d11.DeviceContext);
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
                model = new SimpleModel();

                //Initialize the model object.
                if(!model.Initialize(dx3d11.Device, ModelTextureFilename))
                {
                    return false;
                }

                //Create the solid color shader object.
                solidColorShader = new SolidColorShader();

                //Initialize the color shader object.
                if(!solidColorShader.Initialize(dx3d11.Device))
                {
                    return false;
                }

                //Create the texture shader object.
                textureShader = new TextureShader();

                //Initialize the texture shader object.
                if(!textureShader.Initialize(dx3d11.Device))
                {
                    return false;
                }

                //Create the diffuse light shader object.
                diffuseColorShader = new DiffuseColorShader();

                //Initialize the diffuse light shader object.
                if(!diffuseColorShader.Initialize(dx3d11.Device))
                {
                    return false;
                }

                //Crate the light object.
                directionalLight = new DirectionalLight();

                //Initialize the light object.
                directionalLight.DiffuseColor = new Color4(0.0f, 1.0f, 1.0f, 1.0f);
                directionalLight.Direction = -Vector3.UnitX;

                return true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Could not initialize Direct3D\nError is '{0}'", exception.Message));
                logger.FatalException("Could not initialize Direct3D.", exception);
                return false;
            }

            return true;
        }

        public bool Frame()
        {
            //Update the rotation variables each frame.
            Rotate();

            //Render the graphics scene.
            return Render(rotation);
        }

        public bool Render(float rotation)
        {
            //Clear buffer to begin the scene.
            dx3d11.BeginScene();

            //Generate the view matrix based on the camera position.
            camera.Render();

            //Get the world, view and projection matrices from the camera and dx3d11 objects.
            var worldMatrix = dx3d11.WorldMatrix;
            var viewMatrix = camera.ViewMatrix;
            var projectionMatrix = dx3d11.ProjectionMatrix;

            //Rotate the world matrix by the rotation value so that the triangle will spin.
            Matrix.RotationY(rotation, out worldMatrix);

            //Put the model vertex and index buffers on the graphics pipeline to prepare them for drawing.
            model.Render(dx3d11.DeviceContext);

            ////Render the model using the solid color shader.
            //if(!solidColorShader.Render(dx3d11.DeviceContext, model.IndexCount, worldMatrix, viewMatrix, projectionMatrix))
            //{
            //    return false;
            //}

            ////Render the model using the texture shader.
            //if(!textureShader.Render(dx3d11.DeviceContext, model.IndexCount, worldMatrix, viewMatrix, projectionMatrix, model.Texture.TextureResource))
            //{
            //    return false;
            //}

            //Render the model using the diffuse light shader.
            if(!diffuseColorShader.Render(dx3d11.DeviceContext, model.IndexCount,
                worldMatrix, viewMatrix, projectionMatrix, model.Texture.TextureResource, directionalLight.Direction, directionalLight.DiffuseColor))
            {
                return false;
            }

            //Present the rendered scene to the screen.
            dx3d11.EndScene();

            return true;
        }

        private void Rotate()
        {
            rotation += 0.02f;
            if(rotation > MathConstsFloat.TwoPi)
            {
                rotation -= MathConstsFloat.TwoPi;
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        private void Shutdown()
        {
            //Release the diffuse light shader object.
            if(diffuseColorShader != null)
            {
                diffuseColorShader.Dispose();
                diffuseColorShader = null;
            }

            //Release the texture shader object.
            if(textureShader != null)
            {
                textureShader.Dispose();
                textureShader = null;
            }

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

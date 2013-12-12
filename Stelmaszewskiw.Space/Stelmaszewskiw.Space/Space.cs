using Ninject;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stelmaszewskiw.Space.Cameras;
using Stelmaszewskiw.Space.Core.Game;
using Stelmaszewskiw.Space.Input;

namespace Stelmaszewskiw.Space
{
    public class Space : GameBase
    {
        private readonly GraphicsDeviceManager _graphicDeviceManager;
        private BasicEffect _basicEffect;
        private GeometricPrimitive _torus;
        private ICameraManager _cameraManager;
        private PrimitiveBatch<VertexPositionColor> _batch;
        private VertexPositionColor[] _vertexPositionColorList;
        private BasicEffect _primitiveEffect;

        public Space()
        {
            _graphicDeviceManager = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            Window.Title = "Space by Wojciech Stelmaszewski (2013)";

            IsMouseVisible = true;

            Kernel.Bind<ICameraManager>().To<CameraManager>().InSingletonScope();
            _cameraManager = Kernel.Get<ICameraManager>();

            Kernel.Bind<IKeyboardManager>()
                  .To<KeyboardManager>()
                  .InSingletonScope()
                  .WithConstructorArgument("game", this);
            var keyboardManager = Kernel.Get<IKeyboardManager>();
            RegisterGameComponents(keyboardManager);

            var camera = new Camera(this);
            camera.Position = 2*Vector3.UnitZ;
            RegisterGameComponents(camera);
            _cameraManager.RegisterCamera(camera);

            _graphicDeviceManager.PreferMultiSampling = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Creates a basic effect

            _primitiveEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            _primitiveEffect.VertexColorEnabled = true;
            
            _basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            _basicEffect.EnableDefaultLighting();

            _torus = ToDisposeContent(GeometricPrimitive.Torus.New(GraphicsDevice, 20.0f, 5.0f, 128));
            //_torus = ToDisposeContent(GeometricPrimitive.Cube.New(GraphicsDevice));

            _batch = new PrimitiveBatch<VertexPositionColor>(GraphicsDevice);
            _vertexPositionColorList = new VertexPositionColor[6];
            _vertexPositionColorList[0] = new VertexPositionColor(Vector3.Zero, Color.Crimson);
            _vertexPositionColorList[1] = new VertexPositionColor(Vector3.UnitX, Color.Crimson);
            _vertexPositionColorList[2] = new VertexPositionColor(Vector3.Zero, Color.Green);
            _vertexPositionColorList[3] = new VertexPositionColor(Vector3.UnitY, Color.Green);
            _vertexPositionColorList[4] = new VertexPositionColor(Vector3.Zero, Color.RoyalBlue);
            _vertexPositionColorList[5] = new VertexPositionColor(Vector3.UnitZ, Color.RoyalBlue);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _basicEffect.View = _cameraManager.GetCurrentCamera().ViewMatrix;
            _basicEffect.Projection = _cameraManager.GetCurrentCamera().ProjectionMatrix;

            _primitiveEffect.View = _cameraManager.GetCurrentCamera().ViewMatrix;
            _primitiveEffect.Projection = _cameraManager.GetCurrentCamera().ProjectionMatrix;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _torus.Draw(_basicEffect);

            foreach (var pass in _primitiveEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _batch.Begin();
                //_batch.DrawLine(new VertexPositionColor(Vector3.Zero, Color.Crimson), new VertexPositionColor(Vector3.UnitX, Color.Crimson));
                _batch.Draw(PrimitiveType.LineList, _vertexPositionColorList);
                _batch.End();
            }


            base.Draw(gameTime);
        }
    }
}

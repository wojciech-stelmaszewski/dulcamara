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
            RegisterGameComponents(camera);
            _cameraManager.RegisterCamera(camera);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Creates a basic effect
            _basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice));
            _basicEffect.PreferPerPixelLighting = true;
            _basicEffect.EnableDefaultLighting();

            _torus = ToDisposeContent(GeometricPrimitive.Torus.New(GraphicsDevice, 20.0f, 5.0f, 128));
            //_torus = ToDisposeContent(GeometricPrimitive.Cube.New(GraphicsDevice));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _basicEffect.View = _cameraManager.GetCurrentCamera().ViewMatrix;
            _basicEffect.Projection = _cameraManager.GetCurrentCamera().ProjectionMatrix;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _torus.Draw(_basicEffect);

            base.Draw(gameTime);
        }
    }
}

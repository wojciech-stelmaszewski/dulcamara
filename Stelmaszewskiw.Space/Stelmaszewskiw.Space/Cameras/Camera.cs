using Ninject;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Stelmaszewskiw.Space.Core.Game;
using Stelmaszewskiw.Space.Extensions;
using Stelmaszewskiw.Space.Input;

namespace Stelmaszewskiw.Space.Cameras
{
    public class Camera : UpdateableGameComponentBase, IProjectionCamera
    {
        private readonly IKeyboardManager _keyboardManager;

        private const float RotationSpeed = 0.01f;

        private const float DefaultFov = MathUtil.PiOverFour;

        public const float DefaultNearPlane = 0.1f;

        public const float DefaultFarPlane = 100.0f;

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                UpdateViewMatrix();
            }
        }

        public Matrix ProjectionMatrix { get; private set; }

        public Matrix InvertedProjectionMatrix { get; private set; }

        public Matrix ViewMatrix { get; private set; }
        
        public Matrix InvertedViewMatrix { get; private set; }

        public float Fov { get; private set; }
        
        public float FarPlane { get; private set; }
        
        public float NearPlane { get; private set; }
        
        public float ScreenWidth { get; private set; }
        
        public float ScreenHeight { get; private set; }

        public Vector3 Forward { get { return _forward; } }

        public Vector3 Left { get { return _left; } }

        public Vector3 Up { get { return _up; } }

        private Vector3 _up = Vector3.UnitY;
        private Vector3 _forward = -Vector3.UnitZ;
        private Vector3 _left = -Vector3.UnitX;
        private Vector3 orientation;
        private Vector3 position;

        public Camera(IGame game) : base(game)
        {
            Fov = DefaultFov;
            NearPlane = 0.1f;
            FarPlane = 100.0f;

            ScreenWidth = Game.GraphicsDevice.BackBuffer.Width;
            ScreenHeight = Game.GraphicsDevice.BackBuffer.Height;

            ProjectionMatrix = Matrix.PerspectiveFovRH(Fov, ScreenWidth/ScreenHeight, NearPlane, FarPlane);
            InvertedProjectionMatrix = Matrix.Invert(ProjectionMatrix);

            UpdateViewMatrix();

            _keyboardManager = Game.Kernel.Get<IKeyboardManager>();
        }

        private void UpdateViewMatrix()
        {
            ViewMatrix = Matrix.Translation(-Position)*Matrix.RotationY(Orientation.Y)*Matrix.RotationX(Orientation.X)*
                         Matrix.RotationZ(Orientation.Z);

            InvertedViewMatrix = Matrix.Invert(ViewMatrix);
        }

        public override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var startPosition = position;
            var startOrientation = orientation;

            if (_keyboardManager.IsKeyDown(Keys.W))
            {
                position += _forward * delta;
            }

            if (_keyboardManager.IsKeyDown(Keys.S))
            {
                position -= _forward * delta;
            }

            if (_keyboardManager.IsKeyDown(Keys.D))
            {
                position -= _left * delta;
            }

            if (_keyboardManager.IsKeyDown(Keys.A))
            {
                position += _left * delta;          
            }

            if (_keyboardManager.IsKeyDown(Keys.E))
            {
                position -= _up*delta;
            }

            if (_keyboardManager.IsKeyDown(Keys.Q))
            {
                position += _up*delta;
            }

            if(_keyboardManager.IsKeyDown(Keys.NumPad1))
            {
                orientation += Vector3.UnitX*RotationSpeed;
            }
            if(_keyboardManager.IsKeyDown(Keys.NumPad2))
            {
                orientation += -Vector3.UnitX*RotationSpeed;
            }
            
            if(_keyboardManager.IsKeyDown(Keys.NumPad4))
            {
                orientation += Vector3.UnitY*RotationSpeed;
            }
            if(_keyboardManager.IsKeyDown(Keys.NumPad5))
            {
                orientation += -Vector3.UnitY*RotationSpeed;
            }
            
            if(_keyboardManager.IsKeyDown(Keys.NumPad7))
            {
                orientation += Vector3.UnitZ*RotationSpeed;
            }
            if(_keyboardManager.IsKeyDown(Keys.NumPad8))
            {
                orientation += -Vector3.UnitZ*RotationSpeed;
            }

            if(orientation != startOrientation)
            {
                var rotationMatrix = Matrix.RotationYawPitchRoll(-Orientation.Y, -Orientation.X, -Orientation.Z);

                _forward = Vector3.Transform(-Vector3.UnitZ, rotationMatrix).ToVector3();
                _left = Vector3.Transform(-Vector3.UnitX, rotationMatrix).ToVector3();
                _up = Vector3.Transform(Vector3.UnitY, rotationMatrix).ToVector3();    
            }
            
            if(position != startPosition || orientation != startOrientation)
            {
                UpdateViewMatrix();                
            }
        }
    }
}

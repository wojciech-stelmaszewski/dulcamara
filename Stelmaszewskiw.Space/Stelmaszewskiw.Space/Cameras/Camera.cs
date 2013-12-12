using System;
using Ninject;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Stelmaszewskiw.Space.Core.Game;
using Stelmaszewskiw.Space.Input;

namespace Stelmaszewskiw.Space.Cameras
{
    public class Camera : UpdateableGameComponentBase, ICamera
    {
        private readonly IKeyboardManager _keyboardManager;
        
        public Vector3 Position { get; set; }
        
        public Vector3 Orientation { get; set; }
        
        public Matrix ProjectionMatrix { get; private set; }
        
        public Matrix ViewMatrix { get; private set; }

        private Vector3 _up = Vector3.UnitY;
        private Vector3 _forward = Vector3.UnitZ;
        private Vector3 _left = Vector3.UnitX;

        public Camera(IGame game) : base(game)
        {
            ProjectionMatrix = Matrix.PerspectiveFovRH(MathUtil.PiOverFour,
                                                       (float) Game.GraphicsDevice.BackBuffer.Width/
                                                       Game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);

            UpdateViewMatrix();

            _keyboardManager = Game.Kernel.Get<IKeyboardManager>();
        }

        private void UpdateViewMatrix()
        {
            ViewMatrix = Matrix.Translation(-Position)*Matrix.RotationY(Orientation.Y)*Matrix.RotationX(Orientation.X)*
                         Matrix.RotationZ(Orientation.Z);
        }

        public override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_keyboardManager.IsKeyDown(Keys.W))
            {
                Position += _forward * delta;                
            }

            if (_keyboardManager.IsKeyDown(Keys.S))
            {
                Position -= _forward * delta;
            }

            if (_keyboardManager.IsKeyDown(Keys.D))
            {
                Position -= _left * delta;                
            }

            if (_keyboardManager.IsKeyDown(Keys.A))
            {
                Position += _left * delta;                
            }

            if (_keyboardManager.IsKeyDown(Keys.E))
            {
                Position -= _up * delta;                
            }

            if (_keyboardManager.IsKeyDown(Keys.Q))
            {
                Position += _up * delta;                
            }

            //TODO Fix this ASAP;
            UpdateViewMatrix();
        }
    }
}

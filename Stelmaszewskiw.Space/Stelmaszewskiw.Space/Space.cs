using System.Collections.Generic;
using System.Linq;
using Ninject;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Stelmaszewskiw.Space.Cameras;
using Stelmaszewskiw.Space.Core.Game;
using Stelmaszewskiw.Space.Input;
using KeyboardManager = Stelmaszewskiw.Space.Input.KeyboardManager;

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

        private IList<VertexPositionColor> _innerCellSquare;

        private BasicEffect _primitiveEffect;
        private SpriteBatch _captionContainer;
        private SpriteFont _arial16Font;

        private Grid _grid;
        private MouseManager _mouseManager;
        private Vector3 _intersectionData;
        private IKeyboardManager _keyboardManager;

        public Space()
        {
            _graphicDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
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
            _keyboardManager = Kernel.Get<IKeyboardManager>();
            RegisterGameComponents(_keyboardManager);

            var camera = new Camera(this);
            camera.Position = new Vector3(4, 4, 14);
            RegisterGameComponents(camera);
            _cameraManager.RegisterCamera(camera);

            _graphicDeviceManager.PreferMultiSampling = true;

            _grid = new Grid(this);

            _mouseManager = new MouseManager(this);
            _mouseManager.Initialize();

            _innerCellSquare = new List<VertexPositionColor>();

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

            _captionContainer = ToDisposeContent(new SpriteBatch(GraphicsDevice));
            //_arial16Font = Content.Load<SpriteFont>("Arial16");

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

            var currentMouseState = _mouseManager.GetState();

            var currentCamera = _cameraManager.GetCurrentCamera();
            var projectionCamera = currentCamera as IProjectionCamera;
            if(projectionCamera != null)
            {
                _intersectionData = ScreenCoordinatesToProjectedPlaneCoordinatesCalculator.CalculateIntersectionPoint(
                    new Vector2(currentMouseState.X, currentMouseState.Y),
                    projectionCamera);

                _innerCellSquare.Clear();

                var innerCellIndices = _grid.GetCellIndex(_intersectionData.X, _intersectionData.Z);

                if(_intersectionData != Vector3.Zero && innerCellIndices.X >= 0 && innerCellIndices.X < _grid.CellsCountX && innerCellIndices.Y >= 0 && innerCellIndices.Y < _grid.CellsCountY)
                {
                    var innerCellVertices = _grid.GetInnerCellVertices((int)innerCellIndices.X, (int)innerCellIndices.Y, 0.05f).ToArray();

                    for (var i = 0; i < innerCellVertices.Length; i++)
                    {
                        _innerCellSquare.Add(new VertexPositionColor(new Vector3(innerCellVertices[i].X, 0.0f, innerCellVertices[i].Y), Color.Crimson));
                        _innerCellSquare.Add(new VertexPositionColor(new Vector3(innerCellVertices[(i + 1) % innerCellVertices.Length].X, 0.0f, innerCellVertices[(i + 1) % innerCellVertices.Length].Y), Color.Crimson));
                    }    
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 30));

            //_torus.Draw(_basicEffect);

            //_captionContainer.Begin();
            //_captionContainer.DrawString(_arial16Font, "Alice has a cat.", new Vector2(30, 30), Color.WhiteSmoke);
            //_captionContainer.End();

            foreach (var pass in _primitiveEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                
                _grid.Draw(gameTime);
                
                _batch.Begin();
                if(_innerCellSquare.Any())
                {
                    _batch.Draw(PrimitiveType.LineList, _innerCellSquare.ToArray());
                }
                _batch.Draw(PrimitiveType.LineList, _vertexPositionColorList);
                _batch.End();
            }


            base.Draw(gameTime);
        }
    }
}

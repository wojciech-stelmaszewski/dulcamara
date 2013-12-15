using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stelmaszewskiw.Space.Core.Game;
using IDrawable = Stelmaszewskiw.Space.Core.Game.IDrawable;

namespace Stelmaszewskiw.Space
{
    public class Grid : IDrawable
    {
        private float cellSizeX;
        private float cellSizeY;
        private int cellsCountX;
        private int cellsCountY;
        private Color color;

        public const float DefaultCellSizeX = 1.0f;
        public const float DefaultCellSizeY = 1.0f;
        public const int DefaultCellsCountX = 8;
        public const int DefaultCellsCountY = 8;

        private readonly PrimitiveBatch<VertexPositionColor> _batch;
        private VertexPositionColor[] _vertexPositionColorList;

        public static readonly Color DefaultColor = Color.WhiteSmoke;

        public float CellSizeX
        {
            get { return cellSizeX; }
            protected set { cellSizeX = value; }
        }

        public float CellSizeY
        {
            get { return cellSizeY; }
            protected set { cellSizeY = value; }
        }

        public int CellsCountX
        {
            get { return cellsCountX; }
            protected set { cellsCountX = value; }
        }

        public int CellsCountY
        {
            get { return cellsCountY; }
            protected set { cellsCountY = value; }
        }

        public Color Color
        {
            get { return color; }
            protected set { color = value; }
        }

        public Grid(IGame game)
        {
            CellSizeX = DefaultCellSizeX;
            CellSizeY = DefaultCellSizeY;
            CellsCountX = DefaultCellsCountX;
            CellsCountY = DefaultCellsCountY;
            Color = DefaultColor;

            _batch = new PrimitiveBatch<VertexPositionColor>(game.GraphicsDevice);
        
            Initialize();
        }

        protected void Initialize()
        {
            var verticesList = new List<VertexPositionColor>();

            var sizeX = CellsCountX*CellSizeX;
            var sizeY = CellsCountY*CellSizeY;

            for (var x = 0; x <= cellsCountX; x++)
            {
                verticesList.Add(new VertexPositionColor(new Vector3(x*cellSizeX, 0.0f, 0.0f), Color));
                verticesList.Add(new VertexPositionColor(new Vector3(x*cellSizeX, 0.0f, sizeY), Color));
            }

            for (var y = 0; y <= cellsCountY; y++)
            {
                verticesList.Add(new VertexPositionColor(new Vector3(0.0f, 0.0f, y*CellSizeY), Color));
                verticesList.Add(new VertexPositionColor(new Vector3(sizeX, 0.0f, y*CellSizeY), Color));
            }

            _vertexPositionColorList = verticesList.ToArray();
        }

        public void Draw(GameTime gameTime)
        {
            _batch.Begin();
            _batch.Draw(PrimitiveType.LineList, _vertexPositionColorList);
            _batch.End();
        }
    }
}

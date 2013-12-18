using System;
using System.Collections.Generic;
using SharpDX;

namespace Stelmaszewskiw.Space
{
    public static class GridExtensions
    {
        public static Vector2 GetCellIndex(this Grid @this, Vector2 coordinates)
        {
            return @this.GetCellIndex(coordinates.X, coordinates.Y);
        }

        public static Vector2 GetCellIndex(this Grid @this, float x, float y)
        {
            return new Vector2((float)Math.Floor(x/@this.CellSizeX),
                                       (float)Math.Floor(y/@this.CellSizeY));
        }

        public static Vector2 GetCellPosition(this Grid @this, float x, float y)
        {
            var cellIndices = @this.GetCellIndex(x, y);
            return new Vector2(cellIndices.X*@this.CellSizeX, cellIndices.Y*@this.CellSizeY);
        }

        public static IList<Vector2> GetInnerCellVertices(this Grid @this, int cellIndexX, int cellIndexY, float margin)
        {
            var result = new List<Vector2>();

            var cellPosition = new Vector2(cellIndexX*@this.CellSizeX, cellIndexY*@this.CellSizeY);

            result.Add(new Vector2(cellPosition.X + margin, cellPosition.Y + margin));
            result.Add(new Vector2(cellPosition.X + @this.CellSizeX - margin, cellPosition.Y + margin));
            result.Add(new Vector2(cellPosition.X + @this.CellSizeX - margin, cellPosition.Y + @this.CellSizeY - margin));
            result.Add(new Vector2(cellPosition.X + margin, cellPosition.Y + @this.CellSizeX - margin));

            return result;
        }
    }

    public class GridHelper
    {
    }
}

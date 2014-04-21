using System.Runtime.InteropServices;
using SharpDX;

namespace Stelmaszewskiw.Space.Main.Graphics.Vertices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColor
    {
        public const int ColorElementAlingment = 12;

        public Vector3 Position { get; set; }
        public Color4 Color { get; set; }
    }
}
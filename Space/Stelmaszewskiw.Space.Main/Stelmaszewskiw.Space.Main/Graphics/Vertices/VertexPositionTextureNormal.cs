using System.Runtime.InteropServices;
using SharpDX;

namespace Stelmaszewskiw.Space.Main.Graphics.Vertices
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureNormal
    {
        public const int TextureElementAlignment = 12;
        public const int NormalElementAligbment = 20;

        public Vector3 Position { get; set; }
        public Vector2 Texture { get; set; }
        public Vector3 Normal { get; set; }
    }
}

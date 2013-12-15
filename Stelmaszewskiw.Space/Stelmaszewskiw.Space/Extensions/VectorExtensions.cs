using System;
using SharpDX;

namespace Stelmaszewskiw.Space.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this Vector4 @this)
        {
            const float comparisonEpsilon = 0.000001f;

            if (Math.Abs(@this.W - 1.0f) < comparisonEpsilon)
            {
                return new Vector3(@this.X, @this.Y, @this.Z);
            }
            return new Vector3(@this.X/@this.W, @this.Y/@this.W, @this.Z/@this.W);
        }
    }
}

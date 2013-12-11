using SharpDX;

namespace Stelmaszewskiw.Space.Core.Interfaces
{
    public interface IHaveOrientation
    {
        /// <summary>
        /// X - Pitch
        /// Y - Yaw
        /// Z - Roll
        /// </summary>
        Vector3 Orientation { get; set; }
    }
}

using SharpDX;
using SharpDX.Toolkit;
using Stelmaszewskiw.Space.Core.Game;
using Stelmaszewskiw.Space.Core.Interfaces;

namespace Stelmaszewskiw.Space.Cameras
{
    public interface ICamera : IHaveOrientation, IHavePosition, IUpdateable, IGameComponent
    {
        Matrix ProjectionMatrix { get; }

        Matrix ViewMatrix { get; }
    }
}

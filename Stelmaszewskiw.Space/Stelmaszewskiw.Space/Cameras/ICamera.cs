using SharpDX;
using Stelmaszewskiw.Space.Core.Game;
using Stelmaszewskiw.Space.Core.Interfaces;

namespace Stelmaszewskiw.Space.Cameras
{
    public interface ICamera : IHaveOrientation, IHavePosition, SharpDX.Toolkit.IUpdateable, IGameComponent
    {
        Matrix ProjectionMatrix { get; }

        Matrix ViewMatrix { get; }
    }
}

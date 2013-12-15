using SharpDX;

namespace Stelmaszewskiw.Space.Cameras
{
    public interface IProjectionCamera : ICamera
    {
        float Fov { get; }
        
        float FarPlane { get; }
        
        float NearPlane { get; }

        float ScreenWidth { get; }

        float ScreenHeight { get; }

        Vector3 Forward { get; }

        Vector3 Left { get; }

        Vector3 Up { get; }
    }
}

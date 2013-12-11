namespace Stelmaszewskiw.Space.Cameras
{
    public interface ICameraManager
    {
        void RegisterCamera(ICamera camera);
        ICamera GetCurrentCamera();
    }
}

using System.Collections.Generic;

namespace Stelmaszewskiw.Space.Cameras
{
    public class CameraManager : ICameraManager
    {
        private readonly IList<ICamera> _registredCameras;
        private int _currentCameraIndex;

        public CameraManager()
        {
            _registredCameras = new List<ICamera>();
            _currentCameraIndex = -1;
        }

        public void RegisterCamera(ICamera camera)
        {
            _registredCameras.Add(camera);
            _currentCameraIndex++;
        }

        public ICamera GetCurrentCamera()
        {
            return _registredCameras[_currentCameraIndex];
        }
    }
}
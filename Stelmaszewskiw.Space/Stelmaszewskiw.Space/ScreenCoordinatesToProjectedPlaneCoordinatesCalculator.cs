using System;
using SharpDX;
using Stelmaszewskiw.Space.Cameras;

namespace Stelmaszewskiw.Space
{
    /// <summary>
    /// Calculates intersection point 
    /// </summary>
    public class ScreenCoordinatesToProjectedPlaneCoordinatesCalculator
    {
        public static Vector3 CalculateIntersectionPoint(Vector2 screenCoodrinates, IProjectionCamera camera)
        {
            var horizontalFov = camera.Fov;
            var verticalFov = camera.ScreenHeight*horizontalFov/camera.ScreenWidth;

            var horizontalAngle = Math.Atan((-(camera.ScreenWidth*(screenCoodrinates.X - 1.0f/2.0f)))*Math.Tan(horizontalFov/2.0f)/camera.ScreenWidth);
            var verticalAngle = Math.Atan(-(camera.ScreenWidth*(screenCoodrinates.Y - 1.0f/2.0f))*Math.Tan(verticalFov/2.0f)/camera.ScreenHeight);

            var directionVector = camera.Forward + (float)Math.Tan(horizontalAngle) * camera.Left + (float)Math.Tan(verticalAngle) * camera.Up;

            var t = -camera.Position.Y/directionVector.Y;

            return camera.Position + directionVector*t;
        }
    }
}

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
            //Great article at http://antongerdelan.net/opengl/raycasting.html

            var x = 2.0f*(screenCoodrinates.X - 0.5f);
            var y = -2.0f*(screenCoodrinates.Y - 0.5f);

            var clippedRay = new Vector4(x, y, -1.0f, 1.0f);

            var rayEye = Vector4.Transform(clippedRay, camera.InvertedProjectionMatrix);
            rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);

            var ray = Vector4.Transform(rayEye, camera.InvertedViewMatrix);

            var resultRay = new Vector3(ray.X, ray.Y, ray.Z);

            var t = -(camera.Position.Y) / resultRay.Y;

            return t > 0 ? camera.Position + resultRay*t : Vector3.Zero;
        }
    }
}

namespace Stelmaszewskiw.Space.Main.Camera
{
    public class Camera
    {
        public SharpDX.Vector3 Position { get; set; }
        public SharpDX.Vector3 Rotation { get; set; }

        public SharpDX.Matrix ViewMatrix { get; private set; }

        public void Render()
        {
            //Setup the position of the camera in the world.
            var position = Position;

            //Setup where the camera is looking by default.
            var lookAt = SharpDX.Vector3.ForwardLH;

            //Set the yaw (Y axis), pitch (X axis) and roll (Z axis) rotations in radians.
            var pitch = Rotation.X;
            var yaw = Rotation.Y;
            var roll = Rotation.Z;

            //Create ther rotation matrix from the yaw, pitch and roll values.
            var rotationMatrix = SharpDX.Matrix.RotationYawPitchRoll(yaw, pitch, roll);

            //Transform the lookAtt and Up vector by the rotation matrix so the view is correctly rotated at the origin.
            lookAt = SharpDX.Vector3.TransformCoordinate(lookAt, rotationMatrix);
            var up = SharpDX.Vector3.TransformCoordinate(SharpDX.Vector3.Up, rotationMatrix);

            //Translate the rotated camera position tho the location of the viewer.
            lookAt = position + lookAt;

            //Finally crate the view matrix from the three updated vectors.
            ViewMatrix = SharpDX.Matrix.LookAtLH(position, lookAt, up);
        }
    }
}

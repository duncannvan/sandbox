using Godot;

public partial class Player : CharacterBody3D
{
    private const float MOUSE_SENSE = 0.002f; // TODO: Make adjustable in settings

    private Camera3D _camera;
    private float _cameraRotationX = 0f;

    private void SetupCamera()
    {
        _camera = GetNode<Camera3D>("%PlayerCamera");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void HandleCameraRotation(InputEventMouseMotion mouseMotion)
    {
        const float MAX_CAM_ANGEL_RAD = Mathf.Pi / 2;

        // Rotate player body
        RotateY(-mouseMotion.Relative.X * MOUSE_SENSE);

        // Rotate camera up or down
        _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseMotion.Relative.Y * MOUSE_SENSE, -MAX_CAM_ANGEL_RAD, MAX_CAM_ANGEL_RAD);
        _camera.Rotation = new Vector3(_cameraRotationX, 0, 0);
    }
}
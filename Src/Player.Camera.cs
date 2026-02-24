using Godot;

public partial class Player : CharacterBody3D
{
    private const float MouseSense = 0.002f; // TODO: Make adjustable in settings

    private Camera3D _camera;
    private float _cameraRotationX = 0f;

    private void SetupCamera()
    {
        if(!IsMultiplayerAuthority()) return;
        _camera = GetNode<Camera3D>("%PlayerCamera");
        _camera.MakeCurrent();
        // Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private void HandleCameraRotation(InputEventMouseMotion mouseMotion)
    {
        // TEMP: Easy to use with multiple debug instances
        if (!Input.IsMouseButtonPressed(MouseButton.Left))
        {
            return;
        }
        const float MAX_CAM_ANGEL_RAD = Mathf.Pi / 2;

        // Rotate player body
        RotateY(-mouseMotion.Relative.X * MouseSense);

        // Rotate camera up or down
        _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseMotion.Relative.Y * MouseSense, -MAX_CAM_ANGEL_RAD, MAX_CAM_ANGEL_RAD);
        _camera.Rotation = new Vector3(_cameraRotationX, 0, 0);
    }
}
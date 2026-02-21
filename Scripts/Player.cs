using Godot;

public partial class Player : CharacterBody3D
{
    private const byte SPEED = 50;
    private const float MOUSE_SENSE = 0.002f;
    private const float MAX_CAM_ANGEL_RAD = Mathf.Pi / 2;

    private Camera3D _camera;
    private float _cameraRotationX = 0f;

    public override void _Ready()
    {
        SetupCamera();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            HandleCameraRotation(mouseMotion);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleMovement();
    }

    private void HandleCameraRotation(InputEventMouseMotion mouseMotion)
    {
        // Rotate body
        RotateY(-mouseMotion.Relative.X * MOUSE_SENSE);

        // Rotate camera up/down
        _cameraRotationX = Mathf.Clamp(_cameraRotationX - mouseMotion.Relative.Y * MOUSE_SENSE, -MAX_CAM_ANGEL_RAD, MAX_CAM_ANGEL_RAD);
        _camera.Rotation = new Vector3(_cameraRotationX, 0, 0);
    }

    private void HandleMovement()
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        if(inputDirection == Vector2.Zero) return;

        Vector3 frontBackDirection = _camera.GlobalTransform.Basis.Z * inputDirection.Y;
        Vector3 leftRightDirection =  _camera.GlobalTransform.Basis.X * inputDirection.X;
        Vector3 direction = frontBackDirection + leftRightDirection;
        direction.Y = 0;

        Velocity = direction.Normalized() * SPEED;
        MoveAndSlide();
    }

    private void SetupCamera()
    {
        _camera = GetNode<Camera3D>("%PlayerCamera");
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }


}

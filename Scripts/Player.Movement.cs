using Godot;

public partial class Player
{
    private const byte SPEED = 50; // TODO: Move to stat

    private void HandleMovement()
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_back");

        if(inputDirection == Vector2.Zero) return;

        // Set the move direction relative to the camera rotation
        Vector3 frontBackDirection = _camera.GlobalTransform.Basis.Z * inputDirection.Y;
        Vector3 leftRightDirection =  _camera.GlobalTransform.Basis.X * inputDirection.X;
        Vector3 direction = frontBackDirection + leftRightDirection;
        direction.Y = 0;

        Velocity = direction.Normalized() * SPEED;
        MoveAndSlide();
    }
}
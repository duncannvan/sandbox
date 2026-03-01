using Godot;

public partial class Player : CharacterBody3D
{
    public override void _Ready()
    {
        AddToGroup("Players");
        GetNode<Label3D>("%NameTag").Text = Name;
        SetupAuthority();
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
}

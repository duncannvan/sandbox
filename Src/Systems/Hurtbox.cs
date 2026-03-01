using Godot;

[GlobalClass]
public partial class Hurtbox : Area2D
{
	[Signal] public delegate void HitSuccessEventHandler();

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public virtual void OnAreaEntered(Area2D area)
    {

    }
}
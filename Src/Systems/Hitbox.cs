using Godot;
using System;

[GlobalClass]
public partial class Hitbox : Area2D
{
	[Signal] public delegate void HitSuccessEventHandler();

    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
    }

    public virtual void OnAreaEntered(Area2D area)
    {
        if (area as Hurtbox is not Hurtbox hurtbox) { return; }

		EmitSignal(SignalName.HitSuccess);
    }
}
using Godot;

public partial class Stats : Resource
{
	[Export] public sbyte Health{ get; set; } = 100;
	[Export] public byte Speed{ get; set; } = 10;
}

using Godot;

public partial class MainMenu : Control
{
	[Signal]
	public delegate void StartGameEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Button offlineButton = GetNode<Button>("StartVBoxContainer/OfflineButton");
		//offlineButton.Pressed += () => EmitSignal(SignalName.StartGame);
	}

}

using Godot;
using System.Linq;

public partial class GameManager : Node
{
    [Export] private PackedScene _worldScene;
    [Export] private PackedScene _playerScene;

    private const byte DefaultServerPeerId = 1;

	public void CreateWorld()
	{
		Node worldInstance = _worldScene.Instantiate();
		GetTree().CurrentScene.AddChild(worldInstance);
	}

    public void AddPlayer(long peerId = DefaultServerPeerId)
    {
        Player playerInstance = _playerScene.Instantiate<Player>();
        playerInstance.Name = peerId.ToString();
        GetTree().CurrentScene.AddChild(playerInstance, true);
    }

    public void RemovePlayer(long peerId)
    {
        Node playerToRemove = GetTree().GetNodesInGroup("Players").FirstOrDefault(player => player.Name == peerId.ToString());
        playerToRemove?.QueueFree();
    }
}
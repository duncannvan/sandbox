using Godot;

public partial class Main : Node
{
    private MainMenu _menu;
    private NetworkManager _networkManager;
    private GameManager _gameManager;

    public override void _Ready()
    {
        _menu = GetNode<MainMenu>("%MainMenu");
        _networkManager = GetNode<NetworkManager>("%NetworkManager");
        _gameManager = GetNode<GameManager>("%GameManager");

        _menu.PlayOffline += OnPlayOffline;
        _menu.HostGame += OnHostGame;
        _menu.JoinGame += OnJoinGame;

        _networkManager.PlayerConnected += OnPlayerConnected;
        _networkManager.PlayerDisconnected += OnPlayerDisconnected;
        _networkManager.ServerDisconnected += OnServerDisconnected;
    }

	private void OnPlayOffline()
	{
		_gameManager.CreateWorld();
		_gameManager.AddPlayer();
        _menu.Hide();
	}

	private void OnHostGame()
	{
		_networkManager.HostGame();
	}

	private void OnJoinGame(string ipAddr)
	{
        _networkManager.JoinGame(ipAddr);
	}

    private void OnPlayerConnected(long peerId)
    {
        _gameManager.CreateWorld();
        _gameManager.AddPlayer(peerId);
        _menu.Hide();
    }

    private void OnPlayerDisconnected(long peerId)
    {
        _gameManager.RemovePlayer(peerId);
    }

    private void OnServerDisconnected()
    {
        GetTree().ReloadCurrentScene();
    }
}
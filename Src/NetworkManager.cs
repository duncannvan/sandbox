using Godot;
using GodotPlugins.Game;

public partial class NetworkManager : Node
{
    public static NetworkManager Instance { get; private set; }

    [Signal] public delegate void PlayerConnectedEventHandler(int peerId, Godot.Collections.Dictionary<string, string> playerInfo);
    [Signal] public delegate void PlayerDisconnectedEventHandler(int peerId);
    [Signal] public delegate void ServerDisconnectedEventHandler();

	[Export] private PackedScene GameScene;
    [Export] private PackedScene PlayerScene;

    private Node _gameInstance;
    private const ushort Port = 7000;
    private const string DefaultServerIP = "127.0.0.1";
    private const int MaxConnections = 4;
    const byte ServerPeerId = 1;

    // This will contain peer info for every peer, with the keys being each peer's unique IDs.
    private readonly Godot.Collections.Dictionary<long, Godot.Collections.Dictionary<string, string>> _players = [];
    private Godot.Collections.Dictionary<string, string> _playerInfo = new()
    {
        { "Name", "PlayerName" },
    };

    private int _playersLoaded = 0;

    public override void _Ready()
    {
        Instance = this;
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectOk;
        Multiplayer.ConnectionFailed += OnConnectionFail;
        Multiplayer.ServerDisconnected += OnServerDisconnected;

        _gameInstance = GameScene.Instantiate();
    }

    public Error HostGame()
    {
        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateServer(Port, MaxConnections);

        if (error != Error.Ok) return error;

        Multiplayer.MultiplayerPeer = peer;
        _playerInfo["Name"] = "Player1"; // TEMP
        _players[ServerPeerId] = _playerInfo;

        AddPlayer();
        LoadGame();
        EmitSignal(SignalName.PlayerConnected, ServerPeerId, _playerInfo);

        GD.Print("Hosting Game");
        return Error.Ok;
    }

    public Error JoinGame(string address = DefaultServerIP)
    {
        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateClient(address, Port);

        if (error != Error.Ok) return error;

        Multiplayer.MultiplayerPeer = peer;
        return Error.Ok;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void AddPlayer()
    {
        Player playerScene = PlayerScene.Instantiate<Player>();
        playerScene.Name = "Player" + Multiplayer.GetUniqueId();
        playerScene.SetMultiplayerAuthority(Multiplayer.GetUniqueId());
        _gameInstance.AddChild(playerScene);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void LoadGame()
    {
        GetTree().ChangeSceneToNode(_gameInstance);
    }

    // Send local info to peer
    private void OnPeerConnected(long peerId)
    {
        GD.Print("peer id: ", peerId);
        RpcId(peerId, MethodName.RegisterPlayer, _playerInfo);
        RpcId(peerId, MethodName.AddPlayer);
        if(Multiplayer.IsServer())
        {
            RpcId(peerId, MethodName.LoadGame);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    private void RegisterPlayer(Godot.Collections.Dictionary<string, string> newPlayerInfo)
    {
        int newPlayerId = Multiplayer.GetRemoteSenderId();
        _players[newPlayerId] = newPlayerInfo;
        EmitSignal(SignalName.PlayerConnected, newPlayerId, newPlayerInfo);
    }

    private void OnPeerDisconnected(long id)
    {
        _players.Remove(id);
        EmitSignal(SignalName.PlayerDisconnected, id);
    }

    private void OnConnectOk()
    {
        int peerId = Multiplayer.GetUniqueId();
        _playerInfo["Name"] = "Player" + peerId.ToString(); // TEMP
        _players[peerId] = _playerInfo;
        EmitSignal(SignalName.PlayerConnected, peerId, _playerInfo);
    }

    private void OnConnectionFail()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
        Multiplayer.MultiplayerPeer = null;
        _players.Clear();
        EmitSignal(SignalName.ServerDisconnected);
    }
}
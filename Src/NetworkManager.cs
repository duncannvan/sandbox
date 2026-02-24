using System.Linq;
using Godot;

public partial class NetworkManager : Node
{
    public static NetworkManager Instance { get; private set; }

    [Export] private PackedScene PlayerScene;

    private const ushort Port = 7000;
    private const string DefaultServerIP = "localhost";
    private const int MaxConnections = 4;
    private const byte ServerPeerId = 1;

    public override void _Ready()
    {
        Instance = this;
        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectedToServer += OnConnectOk;
        Multiplayer.ConnectionFailed += OnConnectionFail;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
    }

    public Error HostGame()
    {
        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateServer(Port, MaxConnections);

        if (error != Error.Ok) return error;

        Multiplayer.MultiplayerPeer = peer;
        AddPlayer(Multiplayer.GetUniqueId());

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

    private void OnPeerConnected(long peerId)
    {
        AddPlayer(peerId);
    }

    private void OnPeerDisconnected(long peerId)
    {
        RemovePlayer(peerId);
    }

    private void OnConnectOk()
    {
        // TODO: Create world
        int peerId = Multiplayer.GetUniqueId();
        AddPlayer(peerId);
    }

    private void OnConnectionFail()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
        QuitServer();
    }

    public void AddPlayer(long peerId = ServerPeerId)
    {
        Player playerInstance = PlayerScene.Instantiate<Player>();
        playerInstance.Name = peerId.ToString();
        GetTree().CurrentScene.AddChild(playerInstance, true);
    }

    private void RemovePlayer(long peerId)
    {
        if(peerId == ServerPeerId)
        {
            QuitServer();
        }

        Node playerToRemove = GetTree().GetNodesInGroup("Players").FirstOrDefault(player => player.Name == peerId.ToString());
        playerToRemove?.QueueFree();
    }

    private void QuitServer()
    {
        Multiplayer.MultiplayerPeer.Close();
        Multiplayer.MultiplayerPeer = null;
        GetTree().ReloadCurrentScene();
    }
}
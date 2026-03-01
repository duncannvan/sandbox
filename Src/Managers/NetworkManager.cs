using Godot;

public partial class NetworkManager : Node
{
    [Signal] public delegate void PlayerConnectedEventHandler(long peerId);
    [Signal] public delegate void PlayerDisconnectedEventHandler(long peerId);
    [Signal] public delegate void ServerDisconnectedEventHandler();

    private const ushort Port = 7000;
    private const string DefaultServerIP = "localhost";
    private const int MaxConnections = 4;

    public override void _Ready()
    {
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
        EmitSignal(SignalName.PlayerConnected, Multiplayer.GetUniqueId());
        return Error.Ok;
    }

    public Error JoinGame(string ipAddr = DefaultServerIP)
    {
        GD.Print(ipAddr);
        ENetMultiplayerPeer peer = new();
        Error error = peer.CreateClient(ipAddr, Port);

        if (error != Error.Ok) return error;

        Multiplayer.MultiplayerPeer = peer;
        return Error.Ok;
    }

    private void OnPeerConnected(long peerId)
    {
        EmitSignal(SignalName.PlayerConnected, peerId);
    }

    private void OnPeerDisconnected(long peerId)
    {
        EmitSignal(SignalName.PlayerDisconnected, peerId);
    }

    private void OnConnectOk()
    {
        EmitSignal(SignalName.PlayerConnected, Multiplayer.GetUniqueId());
    }

    private void OnConnectionFail()
    {
        Multiplayer.MultiplayerPeer = null;
    }

    private void OnServerDisconnected()
    {
        QuitServer();
    }

    private void QuitServer()
    {
        Multiplayer.MultiplayerPeer.Close();
        Multiplayer.MultiplayerPeer = null;
        EmitSignal(SignalName.ServerDisconnected);
    }
}
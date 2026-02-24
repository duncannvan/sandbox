using System;
using System.Collections.Generic;
using Godot;

public partial class MainMenu : CanvasLayer
{
	private enum MenuState
	{
		MainMenu,
		HostMenu,
		JoinMenu
	}

	[Export] private PackedScene WorldScene;
	private readonly Dictionary<MenuState, Control> _menus = [];
	private Button _backButton;

	public override void _Ready()
	{
		RegisterMenus();

		Button offlineButton = GetNode<Button>("%OfflineButton");
		offlineButton.Pressed += OnOfflinePressed;

		Button hostButton = GetNode<Button>("%HostButton");
		hostButton.Pressed += () => DisplayMenu(MenuState.HostMenu);

		Button startHostButton = GetNode<Button>("%StartHostButton");
		startHostButton.Pressed += OnHostPressed;

		Button joinButton = GetNode<Button>("%JoinButton");
		joinButton.Pressed += OnJoinPressed;

		_backButton = GetNode<Button>("%BackButton");
		_backButton.Pressed += () => DisplayMenu(MenuState.MainMenu);
	}

	private void OnOfflinePressed()
	{
		CreateWorld();
		NetworkManager.Instance.AddPlayer();
	}

	private void OnHostPressed()
	{
		if(NetworkManager.Instance.HostGame() == Error.Ok)
		{
			CreateWorld();
		}
	}

	private void OnJoinPressed()
	{
		if(NetworkManager.Instance.JoinGame() == Error.Ok)
		{
			CreateWorld();
		}
	}

	private void CreateWorld()
	{
		Node worldInstance = WorldScene.Instantiate();
		GetTree().CurrentScene.AddChild(worldInstance);
		Hide();
	}

	private void RegisterMenus()
	{
		const string MenuSuffix = "menu";
		Node menuContainer = GetNode<Node>("%MenuContainer");

		foreach(Node child in menuContainer.GetChildren())
		{
			string childName = child.Name.ToString();

			if(!childName.Contains(MenuSuffix, StringComparison.CurrentCultureIgnoreCase)) continue;

			if (Enum.TryParse(childName, true, out MenuState result))
			{
				_menus.Add(result, (Control)child);
			}
		}
	}

	private void DisplayMenu(MenuState menu)
	{
		foreach(var keyValuePair in _menus)
		{
			keyValuePair.Value.Visible = keyValuePair.Key == menu;
		}
		_backButton.Visible = menu != MenuState.MainMenu;
	}
}

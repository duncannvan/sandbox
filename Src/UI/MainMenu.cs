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

	[Signal] public delegate void PlayOfflineEventHandler();
	[Signal] public delegate void HostGameEventHandler();
	[Signal] public delegate void JoinGameEventHandler(string ipAddr);

	private readonly Dictionary<MenuState, Control> _menus = [];
	private Button _backButton;

	public override void _Ready()
	{
		RegisterMenus();
		SetupHandlers();
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

	private void SetupHandlers()
	{
		GetNode<Button>("%OfflineButton").Pressed += () => EmitSignal(SignalName.PlayOffline);
		GetNode<Button>("%HostButton").Pressed += () => DisplayMenu(MenuState.HostMenu);
		GetNode<Button>("%StartHostButton").Pressed += () => EmitSignal(SignalName.HostGame);
		GetNode<Button>("%JoinButton").Pressed += () => DisplayMenu(MenuState.JoinMenu);
		GetNode<Button>("%StartJoinButton").Pressed += () => EmitSignal(SignalName.JoinGame, GetNode<LineEdit>("%InviteCode").Text);

		_backButton = GetNode<Button>("%BackButton");
		_backButton.Pressed += () => DisplayMenu(MenuState.MainMenu);
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

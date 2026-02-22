using System;
using System.Collections.Generic;
using Godot;

public partial class MainMenu : Control
{
	private enum MenuState
	{
		MainMenu,
		HostMenu,
		JoinMenu
	}

	[Export]
	private PackedScene GameScene { get; set; }

	private readonly Dictionary<MenuState, Control> _menus = [];
	private Button _backButton;

	public override void _Ready()
	{
		RegisterMenus();

		Button offlineButton = GetNode<Button>("%OfflineButton");
		offlineButton.Pressed += () => GetTree().ChangeSceneToPacked(GameScene);

		Button hostButton = GetNode<Button>("%HostButton");
		hostButton.Pressed += () => DisplayMenu(MenuState.HostMenu);

		Button startHostButton = GetNode<Button>("%StartHostButton");
		startHostButton.Pressed += () => GetTree().ChangeSceneToPacked(GameScene);

		Button joinButton = GetNode<Button>("%JoinButton");
		joinButton.Pressed += () => DisplayMenu(MenuState.JoinMenu);

		_backButton = GetNode<Button>("%BackButton");
		_backButton.Pressed += () => DisplayMenu(MenuState.MainMenu);
	}

	private void RegisterMenus()
	{
		const string MENU_SUFFIX = "menu";
		Node menuContainer = GetNode<Node>("%MenuContainer");

		foreach(Node child in menuContainer.GetChildren())
		{
			string childName = child.Name.ToString();

			if(!childName.Contains(MENU_SUFFIX, StringComparison.CurrentCultureIgnoreCase)) continue;

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

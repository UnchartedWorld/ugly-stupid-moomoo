using Godot;
using System;

public partial class GameUI : CanvasLayer
{
    private Control _currentMenu;
    private Control _UIRoot;

    [Export] public PackedScene PauseMenuScene;
    [Export] public PackedScene PartyMenuScene;

    public static GameUI Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        _UIRoot = GetNode<Control>("UIRoot");
    }

    public void ShowPauseMenu()
    {
        CloseCurrentlyOpenMenu();

        _currentMenu = PauseMenuScene.Instantiate<Control>();
        _UIRoot.AddChild(_currentMenu);
    }

    public void ShowPartyMenu()
    {
        if (_currentMenu != null) _currentMenu.Visible = false;
        CloseCurrentlyOpenMenu();

        _currentMenu = PartyMenuScene.Instantiate<Control>();
        _UIRoot.AddChild(_currentMenu);
    }

    public void CloseCurrentlyOpenMenu()
    {
        if (_currentMenu != null && IsInstanceValid(_currentMenu))
        {
            _currentMenu.QueueFree();
            _currentMenu = null;
        }
    }
}

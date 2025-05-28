using Godot;
using System;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        GetNode<Button>("%Start").Pressed += Play;
        GetNode<Button>("%Exit").Pressed += Exit;
    }


    #region Private Methods
    private void Play()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Levels/starting_zone.tscn");
    }

    private void Exit()
    {
        GetTree().Quit();
    }

    #endregion
}
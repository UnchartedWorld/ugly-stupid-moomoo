using Godot;
using System;

public partial class StartingZone : Node2D
{
    public override void _UnhandledInput(InputEvent @event)
    {
        // Pause menu - need to start it here, but can unpause it via the PauseMenu script.
        if (@event.IsActionPressed("escape") && GetTree().Paused == false)
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        GetTree().Paused = true;
        GameUI.Instance.ShowPauseMenu();
    }

}

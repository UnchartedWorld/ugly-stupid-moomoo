using Godot;
using System;

public partial class PauseMenu : Control
{
    public override void _Ready()
    {
        Button resumeButton = GetNode<Button>("%Resume");
        resumeButton.GrabFocus();
        resumeButton.Pressed += Resume;

        GetNode<Button>("%Exit").Pressed += Exit;

        GetNode<Button>("%Party").Pressed += OpenPartyMenu;

        SetProcessUnhandledInput(true);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("escape"))
        {
            Resume();
        }
    }

    #region Private Methods
    private void Resume()
    {
        GetTree().Paused = false;
        QueueFree();
    }

    private void Exit()
    {
        GetTree().Quit();
    }

    private void OpenPartyMenu()
    {
        GameUI.Instance.ShowPartyMenu();
    }

    #endregion
}

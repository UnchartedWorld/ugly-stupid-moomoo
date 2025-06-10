using Godot;
using System;

public partial class PlayerCommandMenu : Control
{
    [Export] public Button AttackButton;
    [Export] public Button SkillButton;
    [Export] public Button BlockButton;

    [Signal] public delegate void AttackSelectedEventHandler();
    [Signal] public delegate void SkillSelectedEventHandler();
    [Signal] public delegate void BlockSelectedEventHandler();

    public override void _Ready()
    {
        AttackButton.Pressed += () => EmitSignal(SignalName.AttackSelected);
        SkillButton.Pressed += () => EmitSignal(SignalName.SkillSelected);
        BlockButton.Pressed += () => EmitSignal(SignalName.BlockSelected);

        SetProcessUnhandledInput(true);
    }


    
}

using Godot;
using System;
using System.Collections.Generic;

public partial class SkillMenu : Control
{
    [Export] private VBoxContainer _skillListContainer;
    [Export] public PackedScene SkillButtonScene;

    [Signal] public delegate void SkillSelectedEventHandler(Skill skill);

    public override void _Ready()
    {
    }

    public void Initialize(List<Skill> skills)
    {
        // Removes skills from view from previous character as we change focus,  ideal for each character.
        if (_skillListContainer.GetChildren() != null)
        {
            foreach (Node skill in _skillListContainer.GetChildren())
            {
                skill.QueueFree();
            }
        }

        foreach (Skill skill in skills)
        {
            SkillButton button = SkillButtonScene.Instantiate<SkillButton>();

            button.SetSkill(skill.Name, skill.MMPCost);

            button.Pressed += () => EmitSignal(SignalName.SkillSelected, skill);

            _skillListContainer.AddChild(button);
        }
    }

}

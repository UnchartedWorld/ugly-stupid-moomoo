using Godot;
using System;

public partial class SkillButton : Button
{
    [Export] private Label _skillName;
    [Export] private RichTextLabel _skillCost;

    public override void _Ready()
    {
    }

    public void SetSkill(string skillName, int mmpCost)
    {
        _skillName.Text = skillName;

        string color = mmpCost > 0 ? "#4169e1" : "#FFFFFF";
        _skillCost.BbcodeEnabled = true;
        _skillCost.Text = $"[color={color}]{mmpCost} MMP[/color]";
    }

}

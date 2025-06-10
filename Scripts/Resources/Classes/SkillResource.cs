using Godot;
using System;

[GlobalClass]
public partial class SkillResource : Resource
{
    [Export] public string SkillName;
    [Export] public int BaseDamage;
    [Export] public ElementType Element = ElementType.Neutral;
    [Export] public int MMPCost = 0;
    [Export] public bool isHealing = false;
    [Export] public int healAmount = 0;
    [Export] public TargetType TargetType;
}

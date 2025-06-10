using Godot;
using System;

public partial class Skill : GodotObject
{
    public string Name;
    public int BaseDamage;
    public ElementType Element;
    public int MMPCost;
    public bool IsHealing;
    public int HealAmount;
    public TargetType TargetType;

    public Skill(string name, int baseDamage, ElementType type, int mmpCost, bool isHealing, int healAmount, TargetType target)
    {
        Name = name;
        BaseDamage = baseDamage;
        Element = type;
        MMPCost = mmpCost;
        IsHealing = isHealing;
        HealAmount = healAmount;
        TargetType = target;
    }
}

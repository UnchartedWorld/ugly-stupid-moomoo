using Godot;
using System;

[GlobalClass]
public partial class CharacterResource : Resource
{
    [Export] public string CharacterName;
    [Export] public int MaxHealth;
    [Export] public int MaxMoomooPoints;
    [Export] public int Attack;
    [Export] public int Defense;
    [Export] public int Agility;

    [Export] public Godot.Collections.Array<SkillResource> StartingSkills = [];
}

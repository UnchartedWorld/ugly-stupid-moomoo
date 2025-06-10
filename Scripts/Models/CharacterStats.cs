using System;
using System.Collections.Generic;

public class CharacterStats
{
    public string CharacterName;
    public int Health;
    public int MaxHealth;
    public int MoomooPoints;
    public int MaxMoomooPoints;
    public int Attack;
    public int Defense;
    public int Agility;

    public Dictionary<ElementType, float> ElementalModifiers = [];
    public List<Skill> CharacterSkills = [];

    public CharacterStats(
        string name,
        int maxHP,
        int maxMMP,
        int attack,
        int defence,
        int agility,
        Dictionary<ElementType, float> resistances,
        List<Skill> skills)
    {
        CharacterName = name;
        MaxHealth = maxHP;
        Health = maxHP;
        MaxMoomooPoints = maxMMP;
        MoomooPoints = maxMMP;
        Attack = attack;
        Defense = defence;
        Agility = agility;
        ElementalModifiers = resistances;
        CharacterSkills = skills;
    }
}

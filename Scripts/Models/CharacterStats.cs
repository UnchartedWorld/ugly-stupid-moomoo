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

    public CharacterStats(string name, int maxHP, int maxMMP, int attack, int defence, int agility)
    {
        CharacterName = name;
        MaxHealth = maxHP;
        Health = maxHP;
        MaxMoomooPoints = maxMMP;
        MoomooPoints = maxMMP;
        Attack = attack;
        Defense = defence;
        Agility = agility;
    }
}

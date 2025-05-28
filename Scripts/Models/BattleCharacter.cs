using Godot;
using System;
using System.Collections.Generic;

public class BattleCharacter
{
    public CharacterStats CharacterStats { get; private set; }

    public BattleCharacter(CharacterStats stats)
    {
        CharacterStats = stats;
    }

    public bool IsAlive => CharacterStats.Health > 0;

    public void Heal(int amount)
    {
        CharacterStats.Health = Mathf.Clamp(CharacterStats.Health + amount, 0, CharacterStats.MaxHealth);
    }

    public bool TakeDamage(int amount, ElementType element = ElementType.Neutral)
    {
        float modifier = CharacterStats.ElementalModifiers.GetValueOrDefault(element, 1.0f);
        int damageAfterModifier = Mathf.RoundToInt(amount * modifier);

        CharacterStats.Health = Mathf.Clamp(CharacterStats.Health - damageAfterModifier, 0, CharacterStats.Health);
        return IsAlive;
    }

    public bool SpendMoomooPoints(int amount)
    {
        if (CharacterStats.MoomooPoints >= amount)
        {
            CharacterStats.MoomooPoints -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }
}

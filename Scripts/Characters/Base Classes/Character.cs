using Godot;
using System.Collections.Generic;

public partial class Character : CharacterBody2D
{
	public string characterName;
	public int health;
	public int maxHealth;
	public int magicPoints;
	public int maxMagicPoints;
	public int attack;
	public int defense;
	public int agility;
	
	public enum Resistance
	{
		Light,
		Dark,
		Shock,
		TheseHands
	} 
	
	public Dictionary<Resistance, int> Resistances = new();
}

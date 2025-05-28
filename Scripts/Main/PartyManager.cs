using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PartyManager : Node
{
	private const string PlayerCharacterStatPath = "res://Scripts/Resources/Characters/Linh.tres";
	public static PartyManager Instance { get; private set; }

	public IReadOnlyCollection<PartyMember> PartyMembers => CurrentParty;
	public List<PartyMember> CurrentParty { get; private set; } = [];

	public override void _Ready()
	{
		CharacterResource loadedStats = ResourceLoader.Load<CharacterResource>(PlayerCharacterStatPath);
		if (loadedStats != null)
		{
			CharacterStats mainCharacterStats = new(
				loadedStats.CharacterName,
				loadedStats.MaxHealth,
				loadedStats.MaxMoomooPoints,
				loadedStats.Attack,
				loadedStats.Defense,
				loadedStats.Agility);

			AddPartyMember(mainCharacterStats);
		}

		Instance = this;
    }

	public void AddPartyMember(CharacterStats stats)
	{
		CurrentParty.Add(new PartyMember(stats));
	}

	public void RemovePartyMember(PartyMember member)
	{
		CurrentParty.Remove(member);
	}

	public PartyMember GetPartyMember(int index)
	{
		return CurrentParty.ElementAtOrDefault(index);
	}
}

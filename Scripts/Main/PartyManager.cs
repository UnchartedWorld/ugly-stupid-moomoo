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
			List<Skill> listOfSkills = [];
			foreach (var skill in loadedStats.StartingSkills)
			{
				Skill translatedSkill = new
				(
					skill.SkillName,
					skill.BaseDamage,
					skill.Element,
					skill.MMPCost,
					skill.isHealing,
					skill.healAmount,
					skill.TargetType
				);

				listOfSkills.Add(translatedSkill);
			}
			CharacterStats mainCharacterStats = new(
				loadedStats.CharacterName,
				loadedStats.MaxHealth,
				loadedStats.MaxMoomooPoints,
				loadedStats.Attack,
				loadedStats.Defense,
				loadedStats.Agility,
				loadedStats.Resistances.ToDictionary(),
				listOfSkills);

			AddPartyMember(mainCharacterStats);
		}

		Instance = this;
	}

	public void AddPartyMember(CharacterStats stats)
	{
		CurrentParty.Add(new PartyMember(stats));
	}

	public void AddPartyMemberByDialogue(Variant arg)
	{
		CharacterResource character = arg.As<CharacterResource>();

		List<Skill> listOfSkills = [];
		foreach (var skill in character.StartingSkills)
		{
			Skill translatedSkill = new
			(
				skill.SkillName,
				skill.BaseDamage,
				skill.Element,
				skill.MMPCost,
				skill.isHealing,
				skill.healAmount,
				skill.TargetType
			);

			listOfSkills.Add(translatedSkill);

			CharacterStats characterStats = new
			(
				character.CharacterName,
				character.MaxHealth,
				character.MaxMoomooPoints,
				character.Attack,
				character.Defense,
				character.Agility,
				character.Resistances.ToDictionary(),
				listOfSkills
			);

			CurrentParty.Add(new PartyMember(characterStats));

		}
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

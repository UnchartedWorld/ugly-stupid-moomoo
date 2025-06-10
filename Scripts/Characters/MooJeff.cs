using DialogueManagerRuntime;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class MooJeff : NPCCharacter
{
    [Export] public CharacterResource CharacterData { get; set; }
    [Export] private Resource _recruitmentDialogue;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("interact") && PlayerIsInInteractionRange)
        {
            OnInteract();
        }
    }

    public void OnInteract()
    {
        DialogueManager.ShowDialogueBalloon(_recruitmentDialogue, "start", extraGameStates: [this]);
    }
    
    public async Task JoinParty()
    {
        if (CharacterData == null)
        {
            return;
        }

        List<Skill> listOfSkills = [];
        foreach (var skill in CharacterData.StartingSkills)
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

        CharacterStats mooJeffStats = new(
            CharacterData.CharacterName,
            CharacterData.MaxHealth,
            CharacterData.MaxMoomooPoints,
            CharacterData.Attack,
            CharacterData.Defense,
            CharacterData.Agility,
            CharacterData.Resistances.ToDictionary(),
            listOfSkills);

        PartyManager.Instance.AddPartyMember(mooJeffStats);

        Tween tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0, 0.5);
        await ToSignal(tween, "finished");

        QueueFree();
    }
}

using DialogueManagerRuntime;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Moomoo : NPCCharacter
{
    [Export] public CharacterResource CharacterData { get; set; }
    [Export] private Resource _bossDialogue;

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
        DialogueManager.ShowDialogueBalloon(_bossDialogue, "start", extraGameStates: [this]);
    }

    // Add battle start logic here - will need to call to PartyManager, then add Moomoo as an
    // "enemy".
}

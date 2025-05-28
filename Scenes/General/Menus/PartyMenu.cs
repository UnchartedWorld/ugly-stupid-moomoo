using Godot;
using System;

public partial class PartyMenu : Control
{
    [Export] private HFlowContainer CardContainer;
    [Export] private PackedScene PartyCardScene;

    public override void _Ready()
    {
        PartyManager partyManager = PartyManager.Instance;

        foreach (var partyMember in partyManager.CurrentParty)
        {
            PartyMemberRow card = PartyCardScene.Instantiate<PartyMemberRow>();
            card.SetPartyMember(partyMember);
            CardContainer.AddChild(card);
        }

        GetNode<Button>("%BackButton").GrabFocus();
        GetNode<Button>("%BackButton").Pressed += Return;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("escape"))
        {
            Return();
        }
    }


    private void Return()
    {
        GameUI.Instance.ShowPauseMenu();
    }

}

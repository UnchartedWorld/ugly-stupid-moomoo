using Godot;
using System;

public partial class PartyMemberRow : Control
{
    [Export] private TextureRect characterPortrait;
    [Export] private Label characterName;
    [Export] private RichTextLabel HPLabel;
    [Export] private RichTextLabel MMPLabel;

    public void SetPartyMember(PartyMember partyMember)
    {
        CharacterStats stats = partyMember.CharacterStats;

        characterName.Text = stats.CharacterName;

        UpdateHpDisplay(partyMember);
        UpdateMMpDisplay(partyMember);
    }

        private void UpdateHpDisplay(PartyMember member)
    {
        float ratio = (float)member.CharacterStats.Health / member.CharacterStats.MaxHealth;
        string color = ratio <= 0.25f ? "red" : ratio <= 0.5f ? "orange" : "yellow";

        HPLabel.Text = $"[color={color}]{member.CharacterStats.Health}[/color] / {member.CharacterStats.MaxHealth}";
    }

    private void UpdateMMpDisplay(PartyMember member)
    {
        float ratio = (float)member.CharacterStats.MoomooPoints / member.CharacterStats.MaxMoomooPoints;
        string color = ratio <= 0.25f ? "darkblue" : ratio <= 0.5f ? "blue" : "cyan";

        MMPLabel.Text = $"[color={color}]{member.CharacterStats.MoomooPoints}[/color] / {member.CharacterStats.MaxMoomooPoints}";
    }
}

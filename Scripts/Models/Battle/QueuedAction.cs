public class QueuedAction
{
    public PartyMember Member { get; }
    public Skill Skill { get; }

    public QueuedAction(PartyMember member, Skill skill)
    {
        Member = member;
        Skill = skill;
    }
}
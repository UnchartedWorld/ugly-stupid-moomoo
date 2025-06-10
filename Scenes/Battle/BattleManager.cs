using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class BattleManager : Control
{
    public static BattleManager Instance { get; private set; }

    [Export] private PackedScene battleUI;

    #region Signals 
    // Needed signals

    /// <summary>
    /// Signal to indicate battle has started - primarily used right at the 
    /// beginning.
    /// </summary>
    [Signal] public delegate void BattleStartedEventHandler();
    /// <summary>
    /// Signal to indicate player turn has begun. Typically used shortly before
    /// an action is requested.
    /// </summary>
    [Signal] public delegate void PlayerTurnStartedEventHandler(int partyIndex);
    /// <summary>
    /// 
    /// </summary>
    [Signal] public delegate void ProcessPartyActionsEventHandler();
    /// <summary>
    /// Signal to indicate enemy turn has begun.
    /// </summary>
    [Signal] public delegate void EnemyTurnStartedEventHandler();
    /// <summary>
    /// Signal to indicate an action from the user is expected.
    /// </summary>
    [Signal] public delegate void ActionRequestedEventHandler();
    /// <summary>
    /// Signal to indicate turn has changed and moving to a new battle state.
    /// </summary>
    /// <param name="newState"><see cref="BattleState"/> indicating whom has the turn.</param>
    [Signal] public delegate void TurnChangedEventHandler(BattleState newState);
    /// <summary>
    /// Signal to indicate battle has ended. Boolean indicates whether party won or lost.
    /// Could be expanded to indicate type of losses i.e party runs away.
    /// </summary>
    /// <param name="partyWon">True if party has won, false otherwise.</param>
    [Signal] public delegate void BattleEndedEventHandler(bool partyWon);

    [Signal] public delegate void HealthChangedEventHandler(string targetName, int newHealth);

    [Signal] public delegate void MoomooPointsChangedEventHandler(string targetName, int newHealth);

    [Signal] public delegate void EnemyUpdatedEventHandler();

    #endregion

    // Makes it easier to track the state internally.
    private BattleState _battleState;
    private Queue<QueuedAction> _actionQueue = new();
    private List<PartyMember> _playerParty = [];
    private BattleCharacter _enemyCharacter;
    private int _currentPartyIndex = 0;

    public override void _Ready()
    {
        Instance = this;


    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerSkill"></param>
    public void QueuePlayerAction(Skill playerSkill)
    {
        PartyMember currentPartyMember = _playerParty[_currentPartyIndex];
        _actionQueue.Enqueue(new QueuedAction(currentPartyMember, playerSkill));
        _currentPartyIndex++;
        GD.Print($"[QueuePlayerAction] index={_currentPartyIndex}, queueSize={_actionQueue.Count}, partyCount={_playerParty.Count}");

        if (_currentPartyIndex < _playerParty.Count)
        {
            EmitSignal(SignalName.PlayerTurnStarted, _currentPartyIndex);
            EmitSignal(SignalName.ActionRequested);
        }
        else
        {
            ChangeState(BattleState.PerformingAction);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enemyCharacter"></param>
    /// <returns></returns>
    public void StartBattle(BattleCharacter enemyCharacter)
    {
        _playerParty = PartyManager.Instance.CurrentParty;
        _enemyCharacter = enemyCharacter;
        _currentPartyIndex = 0;
        EmitSignal(SignalName.BattleStarted);

        _battleState = BattleState.Start;
        EmitSignal(SignalName.EnemyUpdated);

        AdvanceToNextPartyMemberTurn();
    }

    public BattleCharacter GetCurrentEnemy() => _enemyCharacter;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public PartyMember GetCurrentPartyMember()
    {
        if (_currentPartyIndex < _playerParty.Count) return _playerParty[_currentPartyIndex];
        else return null;
    }

    #region Private Methods

    // private async Task ProcessActionQueue()
    // {
    //     while (_actionQueue.Count > 0)
    //     {
    //         Skill action = _actionQueue.Dequeue();
    //         await PerformAction(action);
    //     }

    //     if (_enemyCharacter.CharacterStats.Health <= 0)
    //     {
    //         EndBattle(partyWon: true);
    //         return;
    //     }

    //     _battleState = BattleState.EnemyTurn;
    //     EmitSignal(SignalName.TurnChanged, (int)_battleState);
    //     EmitSignal(SignalName.EnemyTurnStarted);

    //     await ProcessEnemyTurn();
    // }

    private async Task ProcessPlayerActions()
    {
        foreach (QueuedAction action in _actionQueue)
        {
            await PerformAction(action);
        }

        _actionQueue.Clear();

        if (_enemyCharacter.IsAlive == false)
        {
            ChangeState(BattleState.Victory);
            return;
        }

        ChangeState(BattleState.EnemyTurn);
    }

    private void AdvanceToNextPartyMemberTurn()
    {
        GD.Print($"Player party count = {_playerParty.Count}");
        if (_playerParty.Count == 0)
        {
            // Party is clearly dead/gone.
            EndBattle(false);
            return;
        }

        _currentPartyIndex = 0;

        ChangeState(BattleState.PlayerTurn);
    }

    private async Task PerformAction(QueuedAction action)
    {
        switch (action.Skill.TargetType)
        {
            case TargetType.SingleEnemy:
            case TargetType.AllEnemies:
                _enemyCharacter.TakeDamage(action.Skill.BaseDamage, action.Skill.Element);
                EmitSignal(SignalName.HealthChanged, _enemyCharacter.CharacterStats.CharacterName, _enemyCharacter.CharacterStats.Health);
                // If we decide to make skills that can drain their MP, this call becomes needed.
                // EmitSignal(SignalName.MoomooPointsChanged, _enemyCharacter.CharacterStats.CharacterName, _enemyCharacter.CharacterStats.MoomooPoints);
                if (action.Skill.MMPCost != 0)
                {
                    action.Member.SpendMoomooPoints(action.Skill.MMPCost);
                    EmitSignal(SignalName.MoomooPointsChanged, action.Member.CharacterStats.CharacterName, action.Member.CharacterStats.MoomooPoints);
                }
                break;

            case TargetType.Self:
                action.Member.Heal(action.Skill.HealAmount);
                EmitSignal(SignalName.HealthChanged, action.Member.CharacterStats.CharacterName, action.Member.CharacterStats.Health);
                EmitSignal(SignalName.MoomooPointsChanged, action.Member.CharacterStats.CharacterName, action.Member.CharacterStats.MoomooPoints);
                break;

            case TargetType.Party:
                foreach (var member in _playerParty)
                {
                    member.Heal(action.Skill.HealAmount);
                    EmitSignal(SignalName.HealthChanged, member.CharacterStats.CharacterName, member.CharacterStats.Health);
                }
                EmitSignal(SignalName.MoomooPointsChanged, action.Member.CharacterStats.CharacterName, action.Member.CharacterStats.MoomooPoints);
                break;
        }

        // TODO: Replace this with call to BattleUI, obviously.
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
    }

    private async Task ProcessEnemyTurn()
    {
        GD.Print($"Enemy has begun attack");

        // TODO: Add skill to party/enemy member for easier calling.
        PartyMember targetPartyMember = EnemySelectBestPartyTarget(new Skill("Punch", 20, ElementType.TheseHands, 0, false, 0, TargetType.SingleEnemy));

        if (targetPartyMember == null)
        {
            EndBattle(true);
            return;
        }

        // Actually take damage. Will also factor in character weaknesses.
        targetPartyMember.TakeDamage(20, ElementType.TheseHands);
        EmitSignal(SignalName.HealthChanged, targetPartyMember.CharacterStats.CharacterName, targetPartyMember.CharacterStats.Health);

        // TODO: Replace this with call to BattleUI, obviously.
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

        if (_playerParty.TrueForAll(x => x.CharacterStats.Health <= 0))
        {
            EndBattle(false);
            return;
        }

        ChangeState(BattleState.PlayerTurn);

        _currentPartyIndex = 0;
        AdvanceToNextPartyMemberTurn();

        // TODO: Replace this with call to BattleUI, obviously.
        await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
    }

    private void EndBattle(bool partyWon)
    {
        if (partyWon == true)
        {
            foreach (var partyMember in _playerParty)
            {
                if (partyMember.CharacterStats.Health == 0) partyMember.CharacterStats.Health = 1;
                else continue;
            }
        }
        ChangeState(partyWon ? BattleState.Victory : BattleState.Defeat);
    }

    private PartyMember EnemySelectBestPartyTarget(Skill enemySkill)
    {
        List<PartyMember> livingTargets = _playerParty.FindAll(x => x.CharacterStats.Health > 0);
        if (livingTargets.Count == 0) return null;

        int diceRoll = GD.RandRange(1, 100);

        // If they get lucky, enemy targets party member closest to death.
        if (diceRoll <= 30)
        {
            return livingTargets.MinBy(x => x.CharacterStats.Health);
        }
        // Otherwise it primarily targets those with the biggest weakness to their attacks.
        else if (diceRoll <= 70)
        {
            var exploitableMembers = livingTargets.FindAll
                (x => x.CharacterStats.ElementalModifiers.ContainsKey(enemySkill.Element))
                .Where(y => y.CharacterStats.ElementalModifiers.ContainsValue(GameConfig.WeaknessFloat))
                .ToList();

            if (exploitableMembers.Count > 0)
            {
                return exploitableMembers[GD.RandRange(0, exploitableMembers.Count - 1)];
            }
        }
        // If somehow neither are met, attack randomly.

        return livingTargets[GD.RandRange(0, livingTargets.Count - 1)];
    }
    
    private void ChangeState(BattleState battleState)
    {
        _battleState = battleState;
        EmitSignal(SignalName.TurnChanged, (int)_battleState);

        switch (_battleState)
        {
            case BattleState.PlayerTurn:
                _currentPartyIndex = 0;
                EmitSignal(SignalName.PlayerTurnStarted, _currentPartyIndex);
                EmitSignal(SignalName.ActionRequested);
                break;
            case BattleState.PerformingAction:
                EmitSignal(SignalName.ProcessPartyActions);
                _ = ProcessPlayerActions();
                break;
            case BattleState.EnemyTurn:
                EmitSignal(SignalName.EnemyTurnStarted);
                _ = ProcessEnemyTurn();
                break;
            case BattleState.Victory:
            case BattleState.Defeat:
                EmitSignal(SignalName.BattleEnded, _battleState == BattleState.Victory);
                break;
        }
    }

    #endregion

}

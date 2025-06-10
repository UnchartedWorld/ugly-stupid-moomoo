using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BattleUI : CanvasLayer
{
    [Export] public PackedScene CommandMenu;
    [Export] public PackedScene ActorPanel;
    [Export] public PackedScene BattleEnemy;
    [Export] public PackedScene BattlePlayer;
    [Export] public PackedScene SkillMenu;
    [Export] public PackedScene TargetSelector;

    [Export] private VBoxContainer CommandMenuContainer;
    [Export] private VBoxContainer SkillMenuContainer;
    [Export] private HBoxContainer EnemyDisplayContainer;
    [Export] private HBoxContainer PartyMemberContainer;
    [Export] private HBoxContainer ActorPanelContainer;

    private List<ActorPanel> _actorPanelList = [];
    private ActorPanel _enemyPanel;
    private Stack<(Control menu, Control container)> _menuStack = [];
    private Control _currentMenu;
    private Control _currentContainer;
    private Control _rootMenu;
    private Control _rootContainer;


    public override void _Ready()
    {
        CommandMenuContainer.Hide();
        SkillMenuContainer.Hide();

        BattleManager.Instance.PlayerTurnStarted += OnPlayerTurnStarted;
        BattleManager.Instance.ActionRequested += OnActionRequested;
        BattleManager.Instance.BattleStarted += OnBattleStarted;
        BattleManager.Instance.ProcessPartyActions += OnActionsProcessed;
        BattleManager.Instance.HealthChanged += OnHealthCharged;
        BattleManager.Instance.MoomooPointsChanged += OnMoomooPointsCharged;
        BattleManager.Instance.EnemyUpdated += CreateEnemyPanel;
    }

    public override void _ExitTree()
    {
        BattleManager.Instance.PlayerTurnStarted -= OnPlayerTurnStarted;
        BattleManager.Instance.ActionRequested -= OnActionRequested;
        BattleManager.Instance.HealthChanged -= OnHealthCharged;
        BattleManager.Instance.ProcessPartyActions -= OnActionsProcessed;
        BattleManager.Instance.MoomooPointsChanged -= OnMoomooPointsCharged;
        BattleManager.Instance.BattleStarted -= OnBattleStarted;
        BattleManager.Instance.EnemyUpdated -= CreateEnemyPanel;

        base._ExitTree();
    }

    public void CreateActorPanels(List<PartyMember> partyMembers)
    {
        // Clear any existing panels, just in case we already have some in memory.
        _actorPanelList.Clear();

        for (int i = 0; i < partyMembers.Count; i++)
        {
            PartyMember partyMember = partyMembers[i];
            ActorPanel panel = ActorPanel.Instantiate<ActorPanel>();
            panel.SetData(partyMember.CharacterStats);

            ActorPanelContainer.AddChild(panel);
            _actorPanelList.Add(panel);
        }
        ActorPanelContainer.Show();

        GD.Print($"Printing actor panels, should have {ActorPanelContainer.GetChildren().Count}.");
    }

    public void CreateEnemyPanel()
    {
        BattleCharacter enemy = BattleManager.Instance.GetCurrentEnemy();

        ActorPanel panel = ActorPanel.Instantiate<ActorPanel>();
        panel.SetData(enemy.CharacterStats);

        EnemyDisplayContainer.AddChild(panel);
        _enemyPanel = panel;

        EnemyDisplayContainer.Show();
    }

    public void OnHealthCharged(string targetName, int newHealth)
    {
        ActorPanel panel = _actorPanelList.Find(x => x.CharacterLabel.Text == targetName);
        if (panel != null)
        {
            panel.SetHP(newHealth);
            return;
        }
        if (_enemyPanel.CharacterLabel.Text == targetName)
        {
            _enemyPanel.SetHP(newHealth);
            return;
        }
    }

    public void OnMoomooPointsCharged(string targetName, int newMMP)
    {
        ActorPanel panel = _actorPanelList.Find(x => x.CharacterLabel.Text == targetName);
        if (panel != null)
        {
            panel.SetMMP(newMMP);
            return;
        }
        if (_enemyPanel.CharacterLabel.Text == targetName)
        {
            _enemyPanel.SetMMP(newMMP);
            return;
        }
    }

    public void OnActionRequested()
    {
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("escape"))
        {
            PopMenu();
        }
    }


    #region Private Methods
    private void OnPlayerTurnStarted(int index)
    {
        HighlightCurrentPartyMember(index);
        // Do this at the end as to avoid having to show the skill menu straight away.
        if (_currentMenu is PlayerCommandMenu && _currentMenu.Visible) return;

        ShowCommandMenu();
    }

    private void OnActionsProcessed()
    {
        HideAllMenus();
    }

    private void ShowCommandMenu()
    {
        CommandMenuContainer.Show();
        PlayerCommandMenu commandMenu = CommandMenu.Instantiate<PlayerCommandMenu>();

        commandMenu.AttackSelected += () =>
        {
            Skill basicAttack = new("Slap", 20, ElementType.Neutral, 0, false, 0, TargetType.SingleEnemy);
            BattleManager.Instance.QueuePlayerAction(basicAttack);
            PopMenu();
        };

        commandMenu.SkillSelected += () =>
        {
            PopMenu();
            ShowSkillMenu();
        };

        commandMenu.BlockSelected += () =>
        {
            // Need to build blocking functionality.
        };

        _rootMenu = commandMenu;
        _rootContainer = CommandMenuContainer;
        PushMenu(commandMenu, CommandMenuContainer);
    }

    private void OnBattleStarted()
    {
        CreateActorPanels(PartyManager.Instance.CurrentParty);
    }

    private void ShowSkillMenu()
    {
        SkillMenuContainer.Show();
        _currentMenu?.Hide();

        SkillMenu skillMenu = SkillMenu.Instantiate<SkillMenu>();

        PartyMember currentPartyMember = BattleManager.Instance.GetCurrentPartyMember();
        if (currentPartyMember == null) return;

        skillMenu.Initialize(currentPartyMember.CharacterStats.CharacterSkills);
        skillMenu.SkillSelected += skill =>
        {
            BattleManager.Instance.QueuePlayerAction(skill);
            PopMenu();
        };

        PushMenu(skillMenu, SkillMenuContainer);
    }

    private void PushMenu(Control menu, Control parentContainer)
    {
        if (_currentMenu != null && _currentMenu.GetType() != typeof(PlayerCommandMenu))
        {
            _currentMenu.ReleaseFocus();
            _currentMenu.Hide();
            _menuStack.Push((_currentMenu, _currentContainer));
        }

        parentContainer.AddChild(menu);
        _currentMenu = menu;
        _currentContainer = parentContainer;
        _currentMenu.Show();

        Button firstButton = FindFirstButton(_currentMenu);
        firstButton?.GrabFocus();
    }

    private void PopMenu()
    {
        if (_currentMenu == null) return;

        _currentMenu.ReleaseFocus();
        _currentMenu.Hide();

        if (_currentMenu is PlayerCommandMenu)
        {
            _currentMenu.Show();
            Button firstButton = FindFirstButton(_currentMenu);
            firstButton?.GrabFocus();
            return;
        }

        _currentMenu.QueueFree();
        _currentMenu = null;
        _currentContainer = null;

        if (_menuStack.Count > 0)
        {
            var (menu, container) = _menuStack.Pop();
            _currentMenu = menu;
            _currentContainer = container;
            _currentMenu.Show();
        }
        else
        {
            _currentMenu = null;
            _currentMenu = _rootMenu;
            _currentContainer = _rootContainer;
            _currentMenu.Show();

            Button firstButton = FindFirstButton(_currentMenu);
            firstButton?.GrabFocus();
        }
    }

    private void HideAllMenus()
    {
        // TODO: Find a more reliable way to hide the menus - this sorta works, but breaks if
        // everyone attacks.
        SkillMenuContainer.Hide();
        CommandMenuContainer.Hide();
    }


    private void HighlightCurrentPartyMember(int index)
    {
        for (int i = 0; i < _actorPanelList.Count; i++)
        {
            _actorPanelList[i].SetHighlight(i == index);
        }
    }

    private Button FindFirstButton(Control node)
    {
        if (node is Button button)
            return button;

        foreach (Node child in node.GetChildren())
        {
            if (child is Control control)
            {
                var result = FindFirstButton(control);
                if (result != null)
                    return result;
            }
        }

        return null;
    }

    #endregion

}

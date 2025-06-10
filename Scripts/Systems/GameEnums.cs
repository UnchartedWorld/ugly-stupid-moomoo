using Godot;
using System;

public static class GameEnums
{
}

public enum ElementType
{
    Neutral,
    Light,
    Dark,
    Shock,
    TheseHands
}

public enum ActionType
{
    Attack,
    Skill,
    Item
}

public enum BattleState
{
    Start,
    PlayerTurn,
    PerformingAction,
    EnemyTurn,
    EnemyAction,
    Victory,
    Defeat
}

public enum TargetType
{
    SingleEnemy,
    AllEnemies,
    Self,
    Party
}

public enum PlayerCommandType
{
    Attack,
    UseSkill,
    UseItem,
    Block
}

public enum BattlePortraitAction
{
    Idle,
    Physical,
    Magic
}
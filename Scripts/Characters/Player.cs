using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public Movement _movement;
	private AnimationPlayer animationPlayer;

	public const float ACCELERATION = 1500.0f;


	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_movement = new Movement(this, GameConfig.DefaultSpeed, animationPlayer);

		foreach (Node npcNode in GetTree().GetNodesInGroup("npc"))
		{
			if (npcNode is NPCCharacter npc)
			{
				npc.SetPlayer(this);
			}
		}
    }


	public override void _PhysicsProcess(double delta)
	{
		_movement.UpdateMovementAnimation();
		_movement.ProcessMovement();
	}
}

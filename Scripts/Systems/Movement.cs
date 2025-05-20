using Godot;
using System;

public class Movement(CharacterBody2D player, float speed, AnimationPlayer animationPlayer)
{
	private CharacterBody2D _player = player;
	private float _speed = speed;
	private Vector2 _velocity = Vector2.Zero;
	private AnimationPlayer _animationPlayer = animationPlayer;

	public void UpdateMovementAnimation()
	{
		if (_velocity.Length() == 0)
		{
			_animationPlayer.Stop();
		}
		else
		{
			string direction = "Down";

			if (_velocity.X < 0) direction = "Left";
			else if (_velocity.X > 0) direction = "Right";
			else if (_velocity.Y < 0) direction = "Up";

			_animationPlayer.Play("walk" + direction);

		}
	}

	public void ProcessMovement()
	{
		Vector2 direction = new();

		if (Input.IsActionPressed(GameConfig.MoveRight)) direction.X += 1;
		if (Input.IsActionPressed(GameConfig.MoveLeft)) direction.X -= 1;
		if (Input.IsActionPressed(GameConfig.MoveDown)) direction.Y += 1;
		if (Input.IsActionPressed(GameConfig.MoveUp)) direction.Y -= 1;

		if (direction.Length() > 0) direction = direction.Normalized();

		_player.Velocity = direction * _speed;
		_velocity = _player.Velocity;

		_player.MoveAndSlide();
	}
}

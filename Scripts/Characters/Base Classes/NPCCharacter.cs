using Godot;

public partial class NPCCharacter : Node2D
{
    [Export]
    public NodePath AnimatedSpritePath;

    private AnimatedSprite2D _animatedSprite;
    private Player _player;
    private bool playerIsInRange = false;


    public override void _Ready()
    {
        // Adding each NPC to the group should make it easier to get them to face the player.
        AddToGroup("npc");
        _animatedSprite = GetNode<AnimatedSprite2D>(AnimatedSpritePath);

        Area2D faceDetectionRadius = GetNode<Area2D>("FaceDetectionRadius");
        faceDetectionRadius.BodyEntered += OnBodyEntered;
        faceDetectionRadius.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node body)
    {
        // If the player enters the "body" or circle, activate the facing script.
        if (body is Player player)
        {
            _player = player;
            playerIsInRange = true;
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body == _player)
        {
            playerIsInRange = false;
            _player = null;
            ResetFacingDirection();
        }
    }

    public override void _Process(double delta)
    {
        if (_player != null && playerIsInRange)
        {
            Vector2 directionToPlayer = (_player.GlobalPosition - GlobalPosition).Normalized();
            UpdateFacingDirection(directionToPlayer);
        }
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    protected virtual void UpdateFacingDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
        {
            _animatedSprite.Play(direction.X > 0 ? "lookRight" : "lookLeft");
        }
        else
        {
            _animatedSprite.Play(direction.Y > 0 ? "lookDown" : "lookUp");
        }
    }

    protected virtual void ResetFacingDirection()
    {
        _animatedSprite.Play("lookDown");
    }


}

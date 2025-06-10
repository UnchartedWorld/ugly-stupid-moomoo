using DialogueManagerRuntime;
using Godot;

public partial class NPCCharacter : Node2D
{
    [Export]
    public NodePath AnimatedSpritePath;

    [Export]
    public string DialoguePath;

    private AnimatedSprite2D _animatedSprite;
    private Player _player;
    private Control prompt;
    private Tween tween;
    private bool playerIsInRange = false;
    public bool PlayerIsInInteractionRange = false;


    public override void _Ready()
    {
        // Adding each NPC to the group should make it easier to get them to face the player.
        AddToGroup("npc");
        _animatedSprite = GetNode<AnimatedSprite2D>(AnimatedSpritePath);

        Area2D faceDetectionRadius = GetNode<Area2D>("FaceDetectionRadius");
        faceDetectionRadius.BodyEntered += OnTurnDetectionRadiusEntered;
        faceDetectionRadius.BodyExited += OnTurnDetectionRadiusExited;

        prompt = GetNode<Control>("InteractPrompt");

        // Make it invisible and fully transparent initially
        prompt.Modulate = new Color(1, 1, 1, 0);
        prompt.Visible = false;

        // Get the interactive area - different to the other box, but the same in concept.
        Area2D interactionDetectionBox = GetNode<Area2D>("InteractionBox");
        interactionDetectionBox.BodyEntered += OnInteractEntered;
        interactionDetectionBox.BodyExited += OnInteractExited;
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

    protected virtual void StartDialogue()
    {
        var dialogueResource = ResourceLoader.Load(DialoguePath);
        if (dialogueResource != null)
        {
            DialogueManager.ShowDialogueBalloon(dialogueResource, "start");
        }
        else
        {
            GD.PrintErr("Failed to load dialogue resource at: " + DialoguePath);
        }
    }

    #region Private methods
    private void OnTurnDetectionRadiusEntered(Node body)
    {
        // If the player enters the "body" or circle, activate the facing script.
        if (body is Player player)
        {
            _player = player;
            playerIsInRange = true;
        }
    }

    private void OnTurnDetectionRadiusExited(Node body)
    {
        if (body == _player)
        {
            playerIsInRange = false;
            _player = null;
            ResetFacingDirection();
        }
    }

    private void OnInteractEntered(Node body)
    {
        if (body is Player)
        {
            tween?.Kill();
            prompt.Visible = true;
            tween = CreateTween();
            tween.TweenProperty(prompt, "modulate:a", 1.0f, 0.4f);
            PlayerIsInInteractionRange = true;
        }
    }

    private void OnInteractExited(Node body)
    {
        if (body is Player)
        {
            tween?.Kill();
            tween = CreateTween();
            PropertyTweener step = tween.TweenProperty(prompt, "modulate:a", 0.0f, 0.4f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
            // Lambda function, basically lets me signal that the prompt should politely go away.
            step.Finished += () =>
            {
                if (prompt.Modulate.A <= 0.01f) prompt.Visible = false;
            };
            PlayerIsInInteractionRange = false;
        }
    }

    #endregion


}

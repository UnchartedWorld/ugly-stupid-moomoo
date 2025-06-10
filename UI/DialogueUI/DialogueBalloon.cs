using Godot;
using Godot.Collections;

namespace DialogueManagerRuntime
{
  public partial class DialogueBalloon : CanvasLayer
  {
    [Export] public string NextAction = "ui_accept";
    [Export] public string SkipAction = "ui_cancel";

    [Export] private AudioStreamPlayer _streamPlayer;
    [Export] private DialogueAudioManager _audioManager;
    [Export] private TextureRect _dialogueDoneIndicator;

    Control balloon;
    RichTextLabel characterLabel;
    RichTextLabel dialogueLabel;
    TextureRect portrait;
    VBoxContainer responsesMenu;

    Resource resource;
    Array<Variant> temporaryGameStates = [];

    private Callable _spokeCallable;

    private bool _isWaitingForInput;
    
    bool IsWaitingForInput
    {
      get => _isWaitingForInput;
      set
      {
        _isWaitingForInput = value;
        _dialogueDoneIndicator.Visible = value;
      }
    }
    
    bool willHideBalloon = false;

    DialogueLine dialogueLine;
    DialogueLine DialogueLine
    {
      get => dialogueLine;
      set
      {
        if (value == null)
        {
          QueueFree();
          return;
        }

        dialogueLine = value;
        ApplyDialogueLine();
      }
    }

    Timer MutationCooldown = new Timer();


    public override void _Ready()
    {
      balloon = GetNode<Control>("%Balloon");
      characterLabel = GetNode<RichTextLabel>("%CharacterLabel");
      dialogueLabel = GetNode<RichTextLabel>("%DialogueLabel");
      responsesMenu = GetNode<VBoxContainer>("%ResponsesMenu");
      portrait = GetNode<TextureRect>("%CharacterPortrait");

      _audioManager.SetAudioPlayer(_streamPlayer);
      _dialogueDoneIndicator.Hide();

      balloon.Hide();

      balloon.GuiInput += (@event) =>
      {
        if ((bool)dialogueLabel.Get("is_typing"))
        {
          bool mouseWasClicked = @event is InputEventMouseButton && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left && @event.IsPressed();
          bool skipButtonWasPressed = @event.IsActionPressed(SkipAction);
          if (mouseWasClicked || skipButtonWasPressed)
          {
            GetViewport().SetInputAsHandled();
            dialogueLabel.Call("skip_typing");
            return;
          }
        }

        if (!IsWaitingForInput) return;
        if (dialogueLine.Responses.Count > 0) return;

        GetViewport().SetInputAsHandled();

        if (@event is InputEventMouseButton && @event.IsPressed() && (@event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
        {
          Next(dialogueLine.NextId);
        }
        else if (@event.IsActionPressed(NextAction) && GetViewport().GuiGetFocusOwner() == balloon)
        {
          Next(dialogueLine.NextId);
        }
      };

      if (string.IsNullOrEmpty((string)responsesMenu.Get("next_action")))
      {
        responsesMenu.Set("next_action", NextAction);
      }
      responsesMenu.Connect("response_selected", Callable.From((DialogueResponse response) =>
      {
        Next(response.NextId);
      }));


      // Hide the balloon when a mutation is running
      MutationCooldown.Timeout += () =>
      {
        if (willHideBalloon)
        {
          willHideBalloon = false;
          balloon.Hide();
        }
      };
      AddChild(MutationCooldown);

      DialogueManager.Mutated += OnMutated;

      _spokeCallable = Callable.From((string letter, int index, float speed) =>
      {
        OnLetterSpoken(letter, index, speed);
      });

      dialogueLabel.Connect("spoke", _spokeCallable);
    }


    public override void _ExitTree()
    {
      DialogueManager.Mutated -= OnMutated;
      dialogueLabel.Disconnect("spoke", _spokeCallable);
    }


    public override void _UnhandledInput(InputEvent @event)
    {
      // Only the balloon is allowed to handle input while it's showing
      GetViewport().SetInputAsHandled();
    }


    public override async void _Notification(int what)
    {
      // Detect a change of locale and update the current dialogue line to show the new language
      if (what == NotificationTranslationChanged && IsInstanceValid(dialogueLabel))
      {
        float visibleRatio = dialogueLabel.VisibleRatio;
        DialogueLine = await DialogueManager.GetNextDialogueLine(resource, DialogueLine.Id, temporaryGameStates);
        if (visibleRatio < 1.0f)
        {
          dialogueLabel.Call("skip_typing");
        }
      }
    }


    public async void Start(Resource dialogueResource, string title, Array<Variant> extraGameStates = null)
    {
      temporaryGameStates = new Array<Variant> { this } + (extraGameStates ?? new Array<Variant>());
      IsWaitingForInput = false;
      resource = dialogueResource;

      DialogueLine = await DialogueManager.GetNextDialogueLine(resource, title, temporaryGameStates);
    }


    public async void Next(string nextId)
    {
      DialogueLine = await DialogueManager.GetNextDialogueLine(resource, nextId, temporaryGameStates);
    }


    #region Helpers


    private async void ApplyDialogueLine()
    {
      MutationCooldown.Stop();

      IsWaitingForInput = false;
      balloon.FocusMode = Control.FocusModeEnum.All;
      balloon.GrabFocus();

      // Set up the character name
      characterLabel.Visible = !string.IsNullOrEmpty(dialogueLine.Character);
      characterLabel.Text = Tr(dialogueLine.Character, "dialogue");
      // Set up the character portrait. NOTE: This is set up so that it changes dependent on line tag.
      Array<string> dialogueCharacterMood = dialogueLine.Tags;
      foreach (string mood in dialogueCharacterMood)
      {
        // Assuming it will always provide a portrait path. Failing that, nothing loads anyway.
        string portraitPath = $"res://Assets/Characters/Character-Portraits/{dialogueLine.Character}/{dialogueLine.Character}-{mood ?? "Neutral"}.png";
        if (FileAccess.FileExists(portraitPath) == true)
        {
          portrait.Texture = GD.Load<Texture2D>(portraitPath);
        }
        else
        {
          portrait.Texture = null;
        }
      }

      // Set up the dialogue
      dialogueLabel.Hide();
      dialogueLabel.Set("dialogue_line", dialogueLine);

      // Set up the responses
      responsesMenu.Hide();
      responsesMenu.Set("responses", dialogueLine.Responses);

      // Type out the text
      balloon.Show();
      willHideBalloon = false;
      dialogueLabel.Show();
      if (!string.IsNullOrEmpty(dialogueLine.Text))
      {
        dialogueLabel.Call("type_out");
        await ToSignal(dialogueLabel, "finished_typing");
      }

      // Wait for input
      if (dialogueLine.Responses.Count > 0)
      {
        balloon.FocusMode = Control.FocusModeEnum.None;
        responsesMenu.Show();
      }
      else if (!string.IsNullOrEmpty(dialogueLine.Time))
      {
        float time = 0f;
        if (!float.TryParse(dialogueLine.Time, out time))
        {
          time = dialogueLine.Text.Length * 0.02f;
        }
        await ToSignal(GetTree().CreateTimer(time), "timeout");
        Next(dialogueLine.NextId);
      }
      else
      {
        IsWaitingForInput = true;
        _dialogueDoneIndicator.Visible = true;

        balloon.FocusMode = Control.FocusModeEnum.All;
        balloon.GrabFocus();
      }
    }


    #endregion


    #region signals


    private void OnMutated(Dictionary _mutation)
    {
      IsWaitingForInput = false;
      willHideBalloon = true;
      MutationCooldown.Start(0.1f);
    }

    private void OnLetterSpoken(string letter, int letterIndex, float speed)
    {
      _audioManager.PlayLetter(characterLabel.Text, letter, letterIndex, speed);
    }


    #endregion
  }
}

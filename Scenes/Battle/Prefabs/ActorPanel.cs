using Godot;
using System;

public partial class ActorPanel : Panel
{
    [Export] public Label CharacterLabel;
    [Export] public ProgressBar HPProgBar;
    [Export] public ProgressBar MMPProgBar;
    [Export] private ColorRect Outline;

    public void SetData(CharacterStats stats)
    {
        CharacterLabel.Text = stats.CharacterName;

        HPProgBar.MaxValue = stats.MaxHealth;
        HPProgBar.Value = stats.Health;

        MMPProgBar.MaxValue = stats.MaxMoomooPoints;
        MMPProgBar.Value = stats.MoomooPoints;
    }

    public void SetHP(int HP)
    {
        AnimateBar(HPProgBar, HP);
    }

    public void SetMMP(int MMP)
    {
        AnimateBar(MMPProgBar, MMP);
    }

    public void SetHighlight(bool isEnabled)
    {
        if (Outline != null) Outline.Visible = isEnabled;
    }

    #region Private Methods

    private void AnimateBar(ProgressBar bar, float to)
    {
        Tween tween = CreateTween();

        tween.TweenProperty(bar, "value", to, 0.7f)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);
    }
    
    #endregion
}

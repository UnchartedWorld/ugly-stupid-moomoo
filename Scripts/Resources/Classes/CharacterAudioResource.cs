using Godot;
using System;

[GlobalClass]
public partial class CharacterAudioResource : Resource
{
    [Export] public string CharacterName;
    [Export] public Godot.Collections.Array<AudioStream> DialogueSounds;
}

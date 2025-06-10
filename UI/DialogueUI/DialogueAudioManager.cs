using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DialogueAudioManager : Node
{
    [Export] public Godot.Collections.Array<CharacterAudioResource> AudioResources;

    private Dictionary<string, CharacterAudioResource> _lookup = [];
    private AudioStreamPlayer _player;

    public override void _Ready()
    {
        foreach (CharacterAudioResource audio in AudioResources)
        {
            _lookup[audio.CharacterName] = audio;
        }

        GD.Print("Audio Manager is initialized");

        if (_player == null)
        {
            GD.PushError($"{nameof(DialogueAudioManager)}: the Audio Stream Player wasn't set. Call {nameof(SetAudioPlayer)}.");
        }

        if (AudioResources == null || AudioResources.Count == 0)
        {
            GD.PushError($"{nameof(DialogueAudioManager)}: Audio profiles for characters weren't set.");
        }
    }

    public void SetAudioPlayer(AudioStreamPlayer player)
    {
        _player = player;
        if (_player == null)
        {
            GD.PushError($"{nameof(DialogueAudioManager)}: {nameof(SetAudioPlayer)} received null!");
        }
    }


    public void PlayLetter(string characterName, string letter, int letterIndex, float speed)
    {
        if (!_lookup.TryGetValue(characterName, out CharacterAudioResource profile)) return;

        if (profile.DialogueSounds.Count == 0) return;

        List<string> listOfSkippedCharacters = [",", " ", "."];

        if (listOfSkippedCharacters.Contains(letter) == false)
        {
            AudioStream sound = profile.DialogueSounds.PickRandom();
            _player.Stream = sound;

            // Replace this at some point with a better reference - hardcoding is BAD.
            if (characterName == "Moomoo")
            {
                _player.PitchScale = (float)GD.RandRange(1.1, 1.3);
                _player.Play();
            }
            else
            {
                _player.PitchScale = 1;
                _player.Play();
            }
        }
    }

}

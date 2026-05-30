using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public DialogueList Dialogues;

    private List<AudioSourceTag> Sources;

    private void Awake()
    {
        Sources = new List<AudioSourceTag>();
    }

    public void RegisterSource(AudioSourceTag source)
    {
        Sources.Add(source);
    }

    public void PlayDialogue(int id)
    {
        if (!Dialogues) return;
        if (id > Dialogues.dialogueList.Length) return;
        
        var dialogue = Dialogues.dialogueList[id];

        var source = Sources.First(x => x.SourceName == dialogue.SpeakerTag);
        source.GetAudioSource().PlayOneShot(Dialogues.dialogueList[id].Clip);
    }
}

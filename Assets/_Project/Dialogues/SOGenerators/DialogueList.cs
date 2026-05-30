using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueList", menuName = "Jam/DialogueList")]
public class DialogueList : ScriptableObject
{
    public Dialogue[] dialogueList;
}

[Serializable]
public struct Dialogue
{
    public string SpeakerTag;
    public AudioClip Clip;
    public string Description;
}
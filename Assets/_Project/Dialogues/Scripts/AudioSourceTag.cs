using Sisus.Init;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceTag : MonoBehaviour<DialogueController>
{
    public string SourceName;
    
    private AudioSource audioSource;
    
    public AudioSource GetAudioSource() => audioSource;
    
    protected override void Init(DialogueController argument)
    {
        audioSource = GetComponent<AudioSource>();
        argument.RegisterSource(this);
    }
}

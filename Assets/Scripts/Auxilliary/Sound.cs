using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioSource Source { get; private set; }

    [field:SerializeField] 
    public string Name { get; private set; }

    [field:SerializeField] 
    public AudioClip Clip { get; private set; }

    [field: SerializeField]
    public AudioMixerGroup Output { get; private set; }

    [field: Range(0f, 1f)]
    [field:SerializeField]
    public float Volume { get; private set; }

    [field: Range(1f, 3f)]
    [field:SerializeField] 
    public float Pitch { get; private set; }

    [field: SerializeField]
    public bool Loop { get; private set; }

    public void SetUp(AudioSource source)
    {
        Source = source;
        Source.clip = Clip;
        Source.volume = Volume;
        Source.pitch = Pitch;
        Source.loop = Loop;
        Source.outputAudioMixerGroup = Output;
    }
}

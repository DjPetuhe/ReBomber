using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;

    private static AudioManager s_instance;

    private void Awake()
    {
        if (s_instance is null) s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound sound in sounds)
            sound.SetUp(gameObject.AddComponent<AudioSource>());
    }

    private void Start() => Play("Theme");

    public void Play(string name)
    {
        Sound s = sounds.FirstOrDefault(s => s.Name == name);
        if (s is not null) s.Source.Play();
    }
}

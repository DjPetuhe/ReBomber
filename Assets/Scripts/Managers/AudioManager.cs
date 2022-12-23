using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] sounds;

    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        foreach (Sound sound in sounds)
            sound.SetUp(gameObject.AddComponent<AudioSource>());
    }

    private void Start()
    {
        Play("Theme");
    }   

    public void Play(string name)
    {
        Sound s = sounds.FirstOrDefault(s => s.Name == name);
        if (s == null) return;
        s.Source.Play();
    }
}

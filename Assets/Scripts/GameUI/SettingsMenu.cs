using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Difficulty = DifficultyManager.Difficulty;

public class SettingsMenu : MonoBehaviour
{
    [Header("Mixers")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;

    [Header("Toggles")]
    [SerializeField] Toggle easyToggle;
    [SerializeField] Toggle mediumToggle;
    [SerializeField] Toggle hardToggle;

    [Header("Buttons")]
    [SerializeField] Button saveButton;

    private int _currentLevelRecordID;
    private DifficultyManager _difficultyManager;

    private const float DEFAULT_SOUND_VOLUME = 0.5f;
    private const float DEFAULT_MUSIC_VOLUME = 0.1f;
    private const Difficulty DEFAULT_DIFFICULTY = Difficulty.Easy;
    private const int DEFAULT_LEVEL_RECORD_ID = 5;

    private const string SOUND_KEY = "SoundVolume";
    private const string MUSIC_KEY = "MusicVolume";

    private void Awake()
    {
        _difficultyManager = GameObject.FindGameObjectsWithTag("DifficultyManager")[0]
                                      .GetComponent<DifficultyManager>();
    }

    public void Start() => LoadSettingsPreferences();

    public void LoadSettingsPreferences()
    {
        PlayerPrefs prefs = SaveManager.LoadPreferences();
        if (prefs is null)
        {
            _currentLevelRecordID = DEFAULT_LEVEL_RECORD_ID;
            ConfigureSettings(DEFAULT_SOUND_VOLUME, DEFAULT_MUSIC_VOLUME, DEFAULT_DIFFICULTY);
        }
        else
        {
            _currentLevelRecordID = prefs.RecordLevelID;
            ConfigureSettings(prefs.SoundVolume, prefs.MusicVolume, (Difficulty)prefs.Difficulty);
        }
    }

    private void ConfigureSettings(float soundVol, float musicVol, Difficulty dif)
    {
        _difficultyManager.GameDifficulty = dif;
        SetDifficultyToggle(dif);
        soundSlider.value = soundVol;
        musicSlider.value = musicVol;
        SaveSettings();
    }

    public void SetSoundVolume(float volume) => audioMixer.SetFloat(SOUND_KEY, Mathf.Log10(volume) * 20);

    public void SetMusicVolume(float volume) => audioMixer.SetFloat(MUSIC_KEY, Mathf.Log10(volume) * 20);

    private void SetDifficulty(bool isOn, Difficulty dif)
    {
        if (!isOn) return;
        _difficultyManager.GameDifficulty = dif;
        SaveButtonInteractable(true);
    }

    public void SetDifficultyEasy() => SetDifficulty(easyToggle.isOn, Difficulty.Easy);

    public void SetDifficultyMedium() => SetDifficulty(mediumToggle.isOn, Difficulty.Medium);

    public void SetDifficultyHard() => SetDifficulty(hardToggle.isOn, Difficulty.Hard);

    public void SaveButtonInteractable(bool interact)
    {
        saveButton.interactable = interact;
        if (interact) saveButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(255,255,255);
        else saveButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(100, 100, 100);
    }

    public void SaveSettings()
    {
        SaveManager.SavePreferences(soundSlider.value, musicSlider.value, _difficultyManager.GameDifficulty, _currentLevelRecordID);
        SaveButtonInteractable(false);
    }

    private void SetDifficultyToggle(Difficulty dif)
    {
        switch (dif)
        {
            case Difficulty.Medium:
                mediumToggle.isOn = true;
                break;
            case Difficulty.Hard:
                hardToggle.isOn = true;
                break;
            default:
                easyToggle.isOn = true;
                break;
        }
    }
}

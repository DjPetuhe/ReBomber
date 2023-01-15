using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Difficulty = DifficultyManager.Difficulty;

public class LevelEditorMenu : MonoBehaviour
{
    [Header("Button prefab")]
    [SerializeField] GameObject addLevelButtonPrefab;
    [SerializeField] GameObject levelEditorButtonPrefab;

    [Header("Scrollbar")]
    [SerializeField] Scrollbar scrollbar;

    [Header("Menu fields")]
    [SerializeField] GameObject content;

    private Difficulty[] _customDifficulties;
    private string[] _customNames;
    private int _customLevelsAmount;

    private void Awake()
    {
        _customLevelsAmount = 0; //TODO: Change to loading custom data from json
    }

    private void Start()
    {
        LoadLevels();
        scrollbar.value = 1;
    }

    private void LoadLevels()
    {
        //load custom levels
    }

    public void LoadMainMenu() => SceneManager.LoadScene("MainMenuScene");
}

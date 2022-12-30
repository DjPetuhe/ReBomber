using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Difficulty = DifficultyManager.Difficulty;

public class LevelsMenu : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject levelButtonPrefab;

    [Header("Buttons")]
    [SerializeField] Button originalButton;
    [SerializeField] Button customButton;

    [Header("Scrollbars")]
    [SerializeField] Scrollbar originalScrollbar;
    [SerializeField] Scrollbar customScrollbar;

    [Header("Menu Image")]
    [SerializeField] Sprite originalMenu;
    [SerializeField] Sprite customMenu;

    [Header("Menu fields")]
    [SerializeField] GameObject originalContent;
    [SerializeField] GameObject customContent;

    [Header("Original levels settings")]
    [SerializeField] Difficulty[] difficulties;
    [SerializeField] string[] names;

    private Difficulty[] _customDifficulties;
    private string[] _customNames;
    private int _customLevelsAmount;

    private int _levelRecordID;

    public const int CUSTOM_LEVEL_ID = 4;
    public const int FIRST_LEVEL_ID = 5;
    public const int LAST_LEVEL_ID = 14;

    private void Awake()
    {
        _levelRecordID = SaveManager.LoadPreferences().RecordLevelID;
        _customLevelsAmount = 0; //TODO: Change to loading custom data from json
    }

    private void Start()
    {
        LoadOriginalLevels();
        LoadCustomLevels();
        originalScrollbar.value = 0;
        customScrollbar.value = 0;
    }

    private void LoadOriginalLevels() => LoadLevels(true, LAST_LEVEL_ID - FIRST_LEVEL_ID + 1);

    private void LoadCustomLevels() => LoadLevels(false, _customLevelsAmount);

    private void LoadLevels(bool original, int amount)
    {
        GameObject parent = original ? originalContent : customContent;
        float height = parent.GetComponent<RectTransform>().sizeDelta.y;
        float width = parent.GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 0; i < amount; i++)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab);
            levelButton.transform.SetParent(parent.transform);
            LevelButton buttonScript = levelButton.GetComponent<LevelButton>();
            if ((i + 1) * LevelButton.ButtonHeight > height)
            {
                height = (i + 1) * LevelButton.ButtonHeight + 3;
                parent.GetComponent<RectTransform>().sizeDelta = new(width, height);
            }
            if (original) ConfigureButton(buttonScript, i + FIRST_LEVEL_ID, i + 1, difficulties[i], names[i]);
            else ConfigureButton(buttonScript, CUSTOM_LEVEL_ID, i + 1, _customDifficulties[i], _customNames[i]);
            if (original && (i + FIRST_LEVEL_ID) > _levelRecordID) buttonScript.MakeNotInteractible();
        }
    }

    private void ConfigureButton(LevelButton button, int index, int number, Difficulty dif, string name)
    {
        button.SetLevelIndex(index);
        button.SetNumberText(number);
        button.SetDifficulty(dif);
        button.SetNameText(name);
        button.SetPosition(number);
    }

    public void LoadMainMenu() => SceneManager.LoadScene("MainMenuScene");

    public void ToCustomLevels() => SwitchMenu(originalButton, customButton, customMenu);

    public void ToOriginalLevels() => SwitchMenu(customButton, originalButton, originalMenu);

    private void SwitchMenu(Button from, Button to, Sprite toMenu)
    {
        from.interactable = true;
        to.interactable = false;
        from.GetComponentInChildren<TextMeshProUGUI>().color = new Color(50, 50, 50);
        to.GetComponentInChildren<TextMeshProUGUI>().color = new Color(150, 150, 150);
        GetComponent<Image>().sprite = toMenu;
    }
}

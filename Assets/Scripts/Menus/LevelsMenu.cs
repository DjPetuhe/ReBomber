using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Difficulty = DifficultyManager.Difficulty;
using System.Collections.Generic;

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

    private List<LevelData> _customLevels;

    private int _levelRecordID;

    public const int MENU_SCENE_ID = 0;
    public const int CUSTOM_LEVEL_ID = 4;
    public const int FIRST_LEVEL_ID = 5;
    public const int LAST_LEVEL_ID = 14;

    private void Awake()
    {
        _levelRecordID = SaveManager.LoadPreferences().RecordLevelID;
        _customLevels = SaveManager.LoadAllLevels();
    }

    private void Start()
    {
        LoadOriginalLevels();
        LoadCustomLevels();
        originalScrollbar.value = 1;
        customScrollbar.value = 1;
    }

    private void LoadOriginalLevels() => LoadLevels(true, LAST_LEVEL_ID - FIRST_LEVEL_ID + 1);

    private void LoadCustomLevels() => LoadLevels(false, _customLevels.Count);

    private void LoadLevels(bool original, int amount)
    {
        GameObject parent = original ? originalContent : customContent;
        float height = parent.GetComponent<RectTransform>().sizeDelta.y;
        float width = parent.GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 0; i < amount; i++)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab);
            levelButton.transform.SetParent(parent.transform);
            LevelMenuButton buttonScript = levelButton.GetComponent<LevelMenuButton>();
            if ((i + 1) * LevelMenuButton.ButtonHeight > height)
            {
                height = (i + 1) * LevelMenuButton.ButtonHeight + 3;
                parent.GetComponent<RectTransform>().sizeDelta = new(width, height);
            }
            if (original) ConfigureButton(buttonScript, i + FIRST_LEVEL_ID, i + 1, difficulties[i], names[i]);
            else ConfigureButton(buttonScript, CUSTOM_LEVEL_ID, i + 1, (Difficulty)_customLevels[i].Difficulty, _customLevels[i].Name);
            if (original && (i + FIRST_LEVEL_ID) > _levelRecordID) buttonScript.MakeNotInteractible();
        }
    }

    private void ConfigureButton(LevelMenuButton button, int index, int number, Difficulty dif, string name)
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

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Difficulty = DifficultyManager.Difficulty;

public class LevelEditorMenu : MonoBehaviour
{
    [Header("Button prefab")]
    [SerializeField] GameObject levelEditorButtonPrefab;

    [Header("Add button Rect Transform")]
    [SerializeField] RectTransform addLevelButtonRect;

    [Header("Scrollbar")]
    [SerializeField] Scrollbar scrollbar;

    [Header("Menu fields")]
    [SerializeField] GameObject content;

    private void Start()
    {
        LoadLevels();
        scrollbar.value = 1;
    }

    private void LoadLevels()
    {
        float height = content.GetComponent<RectTransform>().sizeDelta.y;
        float width = content.GetComponent<RectTransform>().sizeDelta.x;
        List<LevelData> levels = SaveManager.LoadAllLevels();
        for (int i = 0; i < levels.Count; i++)
        {
            GameObject levelButton = Instantiate(levelEditorButtonPrefab);
            levelButton.transform.SetParent(content.transform);
            LevelButton buttonScript = levelButton.GetComponent<LevelButton>();
            if ((i + 2) * LevelMenuButton.ButtonHeight > height)
            {
                height = (i + 2) * LevelMenuButton.ButtonHeight + 13;
                content.GetComponent<RectTransform>().sizeDelta = new(width, height);
            }
            buttonScript.SetDifficulty((Difficulty)levels[i].Difficulty);
            buttonScript.SetNameText(levels[i].Name);
            buttonScript.SetPosition(i + 1);
        }
        addLevelButtonRect.anchoredPosition = new()
        {
            x = 0,
            y = -25 - 50 * levels.Count - 10
        };
    }

    public void CreateLevel()
    {
        LevelEditorManager.Editing = false;
        LevelEditorManager.LevelName = "";
        SceneManager.LoadScene("LevelEditorScene");
    }

    public void LoadMainMenu() => SceneManager.LoadScene("MainMenuScene");
}

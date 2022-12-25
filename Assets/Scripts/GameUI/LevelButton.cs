using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Difficulty = DifficultyManager.Difficulty;

public class LevelButton : MonoBehaviour
{
    [Header("Button rect transform")]
    [SerializeField] RectTransform buttonRectTransofrm;

    [Header("Button components")]
    [SerializeField] GameObject numberText;
    [SerializeField] GameObject nameText;
    [SerializeField] Image difficultyImage;

    private static readonly Color32 s_easyDifficultyColor = new(40, 255, 0, 255);
    private static readonly Color32 s_mediumDifficultyColor = new(230, 255, 0, 255);
    private static readonly Color32 s_hardDifficultyColor = new(255, 0, 0, 255);
    private static readonly Color32 s_NotInteractibleTextColor = new(30, 30, 30, 255);
    private int _levelSceneIndex;

    public static int ButtonHeight { get; } = 50;

    public void SetLevelIndex(int index) => _levelSceneIndex = index;

    public void SetNumberText(int number) => numberText.GetComponent<TextMeshProUGUI>().text = number.ToString();

    public void SetNameText(string name) => nameText.GetComponent<TextMeshProUGUI>().text = "Π³βενό: " + name;

    public void SetDifficulty(Difficulty dif)
    {
        difficultyImage.color = dif switch
        {
            Difficulty.Medium => s_mediumDifficultyColor,
            Difficulty.Hard => s_hardDifficultyColor,
            _ => s_easyDifficultyColor
        };
    }

    public void SetPosition(int pos)
    {
        Vector2 newPos = new()
        {
            x = 0,
            y = -25 - ButtonHeight * (pos - 1),
        };
        buttonRectTransofrm.anchoredPosition = newPos;
        buttonRectTransofrm.localScale = new(1, 1, 1);
    }

    public void MakeNotInteractible()
    {
        difficultyImage.color = Color.clear;
        nameText.GetComponent<TextMeshProUGUI>().color = Color.clear;
        numberText.GetComponent<TextMeshProUGUI>().color = s_NotInteractibleTextColor;
        gameObject.GetComponent<Button>().interactable = false;
    }

    public void LoadLevelScene()
    {
        if (!PossibleIndex(_levelSceneIndex))
        {
            Debug.Log($"Can't load level scene with ID {_levelSceneIndex}!");
            return;
        }
        SceneManager.LoadScene(_levelSceneIndex);
    }

    private bool PossibleIndex(int index)
    {
        if (index < LevelsMenu.CUSTOM_LEVEL_ID) return false;
        if (index > LevelsMenu.LAST_LEVEL_ID) return false;
        return true;
    }
}

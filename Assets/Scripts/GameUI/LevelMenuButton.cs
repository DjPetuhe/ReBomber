using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenuButton : LevelButton
{
    [Header("Button components")]
    [SerializeField] GameObject numberText;

    protected int _levelSceneIndex;

    protected static readonly Color32 s_NotInteractibleTextColor = new(30, 30, 30, 255);

    public void SetLevelIndex(int index) => _levelSceneIndex = index;
    public void SetNumberText(int number) => numberText.GetComponent<TextMeshProUGUI>().text = number.ToString();

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
        GameObject gameManager = Instantiate(gameManagerPrefab);
        if (_levelSceneIndex == LevelsMenu.CUSTOM_LEVEL_ID) gameManager.GetComponent<GameManager>().CustomName = _name;
        SceneManager.LoadScene(_levelSceneIndex);
    }

    protected bool PossibleIndex(int index)
    {
        if (index < LevelsMenu.CUSTOM_LEVEL_ID) return false;
        if (index > LevelsMenu.LAST_LEVEL_ID) return false;
        return true;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static DifficultyManager;

public class LevelButton : MonoBehaviour
{
    [Header("Button rect transform")]
    [SerializeField] protected RectTransform buttonRectTransofrm;

    [Header("Button components")]
    [SerializeField] protected GameObject nameText;
    [SerializeField] protected Image difficultyImage;

    [Header("Prefabs")]
    [SerializeField] protected GameObject gameManagerPrefab;

    protected string _name;

    public static int ButtonHeight { get; } = 50;

    protected static readonly Color32 s_easyDifficultyColor = new(40, 255, 0, 255);
    protected static readonly Color32 s_mediumDifficultyColor = new(230, 255, 0, 255);
    protected static readonly Color32 s_hardDifficultyColor = new(255, 0, 0, 255);

    public void SetNameText(string name)
    {
        _name = name;
        nameText.GetComponent<TextMeshProUGUI>().text = "Π³βενό: " + name;
    }

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
            y = -25 - ButtonHeight * (pos - 1) - 5
        };
        buttonRectTransofrm.anchoredPosition = newPos;
        buttonRectTransofrm.localScale = new(1, 1, 1);
    }

    public void EditLevel()
    {
        LevelEditorManager.Editing = true;
        LevelEditorManager.LevelName = _name;
        SceneManager.LoadScene("LevelEditorScene");
    }

    public void DeleteLevel()
    {
        SaveManager.DeleteLevel(_name);
        GameObject[] levelbuttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject levelbutton in levelbuttons)
        {
            LevelButton button = levelbutton.GetComponent<LevelButton>();
            if (button.buttonRectTransofrm.anchoredPosition.y < buttonRectTransofrm.anchoredPosition.y)
            {
                button.buttonRectTransofrm.anchoredPosition = new()
                {
                    x = 0,
                    y = button.buttonRectTransofrm.anchoredPosition.y + ButtonHeight
                };
            }
        }
        RectTransform addButtonRect = GameObject.FindGameObjectWithTag("AddButton").GetComponent<RectTransform>();
        addButtonRect.anchoredPosition = new()
        {
            x = 0,
            y = addButtonRect.anchoredPosition.y + ButtonHeight
        };
        Destroy(gameObject);
    }
}

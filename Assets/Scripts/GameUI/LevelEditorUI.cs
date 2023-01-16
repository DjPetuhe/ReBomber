using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tile = TilemapManager.Tile;
using Difficulty = DifficultyManager.Difficulty;

public class LevelEditorUI : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] Toggle easyToggle;
    [SerializeField] Toggle mediumToggle;
    [SerializeField] Toggle hardToggle;

    [Header("Fields")]
    [SerializeField] GameObject heightField;
    [SerializeField] GameObject widthField;
    [SerializeField] GameObject levelNameField;
    [SerializeField] GameObject saveErrorField;

    [Header("Buttons")]
    [SerializeField] Button saveButton;
    [SerializeField] Button mouseButton;

    [Header("Images")]
    [SerializeField] Sprite paintMouseImage;
    [SerializeField] Sprite dragMouseImage;

    private CameraMovement _cameraMovement;
    private LevelEditorManager _levelEditorManager;

    private Difficulty _levelDifficulty = Difficulty.Easy;

    private const int MIN_SIZE = 10;
    private const int MAX_SIZE = 50;

    private void Start()
    {
        _levelEditorManager = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
        _cameraMovement = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraMovement>();
        if (!LevelEditorManager.Editing)
        {
            heightField.GetComponent<TMP_InputField>().text = MIN_SIZE.ToString();
            widthField.GetComponent<TMP_InputField>().text = MIN_SIZE.ToString();
            easyToggle.isOn = true;
        }
        else
        {
            LevelData level = SaveManager.LoadLevel(LevelEditorManager.LevelName);
            levelNameField.GetComponent<TMP_InputField>().text = LevelEditorManager.LevelName;
            heightField.GetComponent<TMP_InputField>().text = level.Height.ToString();
            widthField.GetComponent<TMP_InputField>().text = level.Width.ToString();
            _levelDifficulty = (Difficulty)level.Difficulty;
            SetDifficultyToggle(_levelDifficulty);
        }
    }

    public void ChangeMouseState()
    {
        _cameraMovement.SwitchDragOption();
        _levelEditorManager.SwitchPaintOption();
        if (_cameraMovement.IsMouseCanDrug) mouseButton.image.sprite = dragMouseImage;
        else mouseButton.image.sprite = paintMouseImage;
    }

    public void ChangeMapSize()
    {
        int height = ValueToInt(heightField.GetComponent<TMP_InputField>().text);
        int width = ValueToInt(widthField.GetComponent<TMP_InputField>().text);
        _levelEditorManager.ResizeMap(height, width);
        heightField.GetComponent<TMP_InputField>().text = height.ToString();
        widthField.GetComponent<TMP_InputField>().text = width.ToString();
    }

    private int ValueToInt(string value)
    {
        if (value.Length > 3) return MAX_SIZE; 
        int number = int.Parse(value);
        if (number <= MIN_SIZE) return MIN_SIZE;
        else if (number >= MAX_SIZE) return MAX_SIZE;
        else return number;
    }

    public void ChooseTile(int tileIndex)
    {
        if (!Enum.IsDefined(typeof(Tile), tileIndex)) return;
        if (_cameraMovement.IsMouseCanDrug) ChangeMouseState();
        _levelEditorManager.ChooseTile((Tile)tileIndex);
    }

    public void ExitFromLevelEditor() => SceneManager.LoadScene("LevelEditorMenuScene");

    public void SetDifficultyEasy() => SetDifficulty(easyToggle.isOn, Difficulty.Easy);

    public void SetDifficultyMedium() => SetDifficulty(mediumToggle.isOn, Difficulty.Medium);

    public void SetDifficultyHard() => SetDifficulty(hardToggle.isOn, Difficulty.Hard);

    private void SetDifficulty(bool isOn, Difficulty dif)
    {
        if (!isOn) return;
        _levelDifficulty = dif;
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

    public void SaveLevel()
    {
        string name = levelNameField.GetComponent<TMP_InputField>().text;
        saveErrorField.GetComponent<TextMeshProUGUI>().text = "";
        if (name.Length == 0) PrintSaveResult("Некоректна назва рівня!", false);
        else if (!_levelEditorManager.PossibleLevel()) PrintSaveResult("Неможилво зберегти! Некоректний рівень!", false);
        else if (LevelEditorManager.Editing)
        {
            SaveManager.DeleteLevel(LevelEditorManager.LevelName);
            _levelEditorManager.SaveLevel(name, (int)_levelDifficulty);
            LevelEditorManager.LevelName = name;
            PrintSaveResult("Рівень збережено!", true);
        }
        else if (!SaveManager.IsLevelExists(name))
        {
            _levelEditorManager.SaveLevel(name, (int)_levelDifficulty);
            PrintSaveResult("Рівень збережено!", true);
            LevelEditorManager.Editing = true;
        }
        else PrintSaveResult("Неможилво зберегти! Рівень з такою назвою вже існує!", false);
    }

    private void PrintSaveResult(string result, bool correct)
    {
        saveErrorField.GetComponent<TextMeshProUGUI>().text = result;
        saveErrorField.GetComponent<TextMeshProUGUI>().color =  correct? Color.green : Color.red;
    }
}

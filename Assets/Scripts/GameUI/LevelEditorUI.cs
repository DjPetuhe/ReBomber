using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Tile = TilemapManager.Tile;

public class LevelEditorUI : MonoBehaviour
{
    //TODO: Serializefields of levelname, level difficulty
    //TODO: const of max name size, min name size
    [Header("Fields")]
    [SerializeField] GameObject heightField;
    [SerializeField] GameObject widthField;

    [Header("Buttons")]
    [SerializeField] Button mouseButton;

    [Header("Images")]
    [SerializeField] Sprite paintMouseImage;
    [SerializeField] Sprite dragMouseImage;

    private CameraMovement _cameraMovement;
    private LevelEditorManager _levelEditorManager;

    private const int MIN_SIZE = 10;
    private const int MAX_SIZE = 50;

    private void Start()
    {
        //TODO: set default values in level name, level difficult
        _levelEditorManager = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
        _cameraMovement = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraMovement>();
        heightField.GetComponent<TMP_InputField >().text = MIN_SIZE.ToString();
        widthField.GetComponent<TMP_InputField>().text = MIN_SIZE.ToString();
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

    public void SaveLevel()
    {
        //TODO: verify level name. If ok - save level and write saved. if not ok - write wrong level name and not save
        //TODO: verify is level possible to finish by level editor manager that return string as result.
        //TODO: Save maanger function for save level (if already exists other level with same name - print error)
    }

    private string LevelNameValidation()
    {
        //TODO: validation
        return "";
    }
}

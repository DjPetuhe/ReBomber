using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LoadLevelMenu() => SceneManager.LoadScene("LevelsMenuScene");

    public void LoadLevelEditor() => SceneManager.LoadScene("LevelEditorMenuScene");

    public void LoadLastSession() { } //TODO: load last session from json

    public void ExitGame() => Application.Quit();
}

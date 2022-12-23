using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LoadLevelMenu()
    {
        SceneManager.LoadScene("LevelsMenuScene");
    }
    public void LoadLevelEditor()
    {
        SceneManager.LoadScene("LevelEditorScene");
    }

    public void LoadLastSession()
    {
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject gameManagerPrefab;

    [Header("Button")]
    [SerializeField] Button LoadButton;

    private void Start()
    {
        if (SaveManager.IsGameStateExists()) LoadButton.interactable = true; 
    }

    public void LoadLevelMenu() => SceneManager.LoadScene("LevelsMenuScene");

    public void LoadLevelEditor() => SceneManager.LoadScene("LevelEditorMenuScene");

    public void LoadLastSession()
    {
        if (!SaveManager.IsGameStateExists()) return;
        GameStateData gameState = SaveManager.LoadGameState();
        if (!LevelMenuButton.PossibleIndex(gameState.SceneID))
        {
            Debug.Log($"Can't load level scene with ID {gameState.SceneID}!");
            return;
        }
        GameObject gameManager = Instantiate(gameManagerPrefab);
        GameManager gameManagerScript = gameManager.GetComponent<GameManager>();
        if (gameState.SceneID == LevelsMenu.CUSTOM_LEVEL_ID) gameManagerScript.CustomName = gameState.Name;
        gameManagerScript.LoadedGameState = true;
        SceneManager.LoadScene(gameState.SceneID);
    }

    public void ExitGame() => Application.Quit();
}

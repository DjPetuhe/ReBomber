using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameState = GameManager.GameState;

public class LevelUI : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] GameObject timeField;
    [SerializeField] GameObject scoreField;
    [SerializeField] GameObject hpField;
    [SerializeField] GameObject totalScoreField;

    [Header("HP sprites")]
    [SerializeField] Sprite hpFull;
    [SerializeField] Sprite hpEmpty;

    [Header("Buttns")]
    [SerializeField] Button pauseButton;
    [SerializeField] Button resumeButton;

    [Header("Panel")]
    [SerializeField] GameObject panel;

    [Header("Game Over UI")]
    [SerializeField] GameObject gameOverUI;

    [Header("Keys")]
    [SerializeField] KeyCode pauseKey;

    private GameManager _gameManager;

    private void Awake() => _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (_gameManager.State == GameState.Play) pauseButton.onClick.Invoke();
            else if (_gameManager.State == GameState.Pause) resumeButton.onClick.Invoke();
        }
    }

    public void SetTime(float time) 
    {
        int minutes = Mathf.CeilToInt(time) / 60;
        int seconds = Mathf.CeilToInt(time) % 60;
        string secondsString = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
        timeField.GetComponent<TextMeshProUGUI>().text = $"{minutes}:" + secondsString;
    }

    public void SetScore(int score) => scoreField.GetComponent<TextMeshProUGUI>().text = score.ToString();

    public void SetHealth(int health)
    {
        foreach (Transform child in hpField.transform)
        {
            if (health > 0)
            {
                child.GetComponent<Image>().sprite = hpFull;
                health--;
            }
            else child.GetComponent<Image>().sprite = hpEmpty;
        }
    }

    public void QuitToMenu()
    {
        if (_gameManager.State != GameState.GameOver)
        {
            //TODO : Save current state (players corrds, breakable tilemap, enemies + enemies coords)
        }
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().DestroyThyself();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void GameOverPopUp(int score)
    {
        panel.SetActive(true);
        gameOverUI.SetActive(true);
        totalScoreField.GetComponent<TextMeshProUGUI>().text = score.ToString();
    }
}

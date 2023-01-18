using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("End Game UI")]
    [SerializeField] GameObject endGameText;
    [SerializeField] GameObject gameOverUI;

    [Header("Keys")]
    [SerializeField] KeyCode pauseKey;

    private GameManager _gameManager;
    private LevelManager _levelManager;

    private bool _pauseEnabled = true;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    private void Update()
    {
        if (!_pauseEnabled) return;
        if (Input.GetKeyDown(pauseKey))
        {
            if (_gameManager.State == GameState.Play) pauseButton.onClick.Invoke();
            else if (_gameManager.State == GameState.Pause) resumeButton.onClick.Invoke();
        }
    }

    public void Pause() => _gameManager.PauseGame();

    public void Resume() => _gameManager.ResumeGame();

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
        if (_gameManager.State != GameState.GameEnd) _gameManager.SaveGameState();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().DestroyThyself();
        _levelManager.QuitToMenu();
    }

    public void EndGamePopUp(int score, bool gameOver)
    {
        panel.SetActive(true);
        gameOverUI.SetActive(true);
        totalScoreField.GetComponent<TextMeshProUGUI>().text = score.ToString();
        endGameText.GetComponent<TextMeshProUGUI>().text = gameOver ? "цпс гюбепьемн" : "бх оепелнцкх";
    }

    public void SwitchPauseStatus(bool enable)
    {
        _pauseEnabled = enable;
        pauseButton.interactable = _pauseEnabled;
    }
}

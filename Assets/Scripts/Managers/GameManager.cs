using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Play = 0,
        Pause = 1,
        GameEnd = 2
    }

    private static GameManager s_instance;

    private LevelUI _levelUI;

    private float _currentTime = START_TIME_SECONDS;
    public float CurrentTime 
    {
        get { return _currentTime; }
        set
        {
            if (value <= 0)
            {
                _currentTime = 0;
                MakeAllEnemiesAngry();
            }
            else
            {
                _currentTime = value;
                _levelUI.SetTime(_currentTime);
            }
        }
    }

    private int _score = START_SCORE;
    public int Score
    {
        get { return _score; }
        set 
        { 
            _score = value < 0 ? 0 : value;
            _levelUI.SetScore(_score);
        }
    }

    private GameState _state = GameState.Play;
    public GameState State 
    { 
        get { return _state; }
        set
        {
            Time.timeScale = value switch
            {
                GameState.Play => 1,
                _ => 0
            };
            _state = value;
        }
    }
    public int StateInt { get { return (int)_state; } }

    private int _health = START_HEALTH;
    public int Health
    {
        get { return _health; }
        set
        {
            if (value <= 0) _health = 0;
            else if (value >= MAX_HEALTH) _health = MAX_HEALTH;
            else _health = value;
            _levelUI.SetHealth(_health);
            if (_health == 0) StartCoroutine(EndGame());
        }
    }
    private float speed = START_SPEED;
    public float Speed
    {
        get { return speed; }
        set 
        {
            if (value <= START_SPEED) speed = START_SPEED;
            else if (value >= MAX_SPEED) speed = MAX_SPEED;
            else speed = value;
        }
    }

    private int _bombCount = START_BOMBS;
    public int BombsCount 
    { 
        get { return _bombCount; }
        set
        {
            if (value <= 0) _bombCount = 0;
            else if (value >= MAX_BOMB) _bombCount = MAX_BOMB;
            else _bombCount = value;
        }
    }

    private const float TIME_BEFORE_GAME_OVER = 2f;

    private const float START_TIME_SECONDS = 300;
    private const int START_SCORE = 0;
    private const int START_HEALTH = 4;
    private const int START_BOMBS = 1;
    private const float START_SPEED = 2.5f;

    public const int MAX_HEALTH = 4;
    public const int MAX_BOMB = 5;
    public const float MAX_SPEED = 10f;

    private void Awake()
    {
        if (s_instance is null) s_instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnLevelLoading;

    private void OnDisable() => SceneManager.sceneLoaded -= OnLevelLoading;

    private void OnLevelLoading(Scene scene, LoadSceneMode mode) => AdaptGameManager();

    private void AdaptGameManager()
    {
        _levelUI = GameObject.Find("LevelUI").GetComponent<LevelUI>();
        CurrentTime = START_TIME_SECONDS;
        _levelUI.SetScore(_score);
        _levelUI.SetHealth(_health);
    }

    private void Update()
    {
        if (State == GameState.Pause) return;
        CurrentTime -= 1 * Time.deltaTime;
    }

    private void MakeAllEnemiesAngry()
    {
        //TODO: make enemies vision: max, speed : max
    }

    private IEnumerator EndGame()
    {
        yield return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealthControl>().Death();
        yield return new WaitForSeconds(TIME_BEFORE_GAME_OVER);
        EndGameUI(true);
    }

    public void EndGameUI(bool gameOver)
    {
        State = GameState.GameEnd;
        _levelUI.EndGamePopUp(Score, gameOver);
        //TODO: delete old gamestate save
    }

    public void ResumeGame() => State = GameState.Play;

    public void PauseGame() => State = GameState.Pause;

    public void DestroyThyself()
    {
        Destroy(gameObject);
        s_instance = null;
        Time.timeScale = 1;
    }
}

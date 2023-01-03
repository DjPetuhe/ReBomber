using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [field: Header("Originality")]
    [field: SerializeField]
    public bool OriginalLevel { get; private set; }

    [Header("Transition Animation")]
    [SerializeField] Animator transition;

    private static LevelManager s_instance;

    private static GameManager _gameManager;

    private int _sceneID;

    private const float TRANSITION_TIME_SECONDS = 1f;

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

    private void Start()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _sceneID = SceneManager.GetActiveScene().buildIndex;
        if (_sceneID == LevelsMenu.CUSTOM_LEVEL_ID) OriginalLevel = false;
    }

    private int FindNextSceneID()
    {
        return _sceneID switch
        {
            LevelsMenu.CUSTOM_LEVEL_ID => 0,
            LevelsMenu.LAST_LEVEL_ID => 0,
            _ => _sceneID + 1
        };
    }

    public IEnumerator LoadToNextLevel()
    {
        int nextSceneID = FindNextSceneID();
        if (nextSceneID == 0) _gameManager.EndGameUI(false);
        else
        {
            //Add next level to prefs as reached;
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(TRANSITION_TIME_SECONDS);
            SceneManager.LoadScene(nextSceneID);
            _sceneID = nextSceneID;
            transition.SetTrigger("End");
        }
    }
}

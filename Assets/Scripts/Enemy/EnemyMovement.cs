using Assets.Scripts.Auxilliary;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Difficulty = DifficultyManager.Difficulty;
using GameState = GameManager.GameState;

public class EnemyMovement : MonoBehaviour
{
    public enum EnemyState
    {
       Calm = 0,
       Chase = 1
    }

    [Header("Animator")]
    [SerializeField] Animator slimeAnimator;

    [Header("Slime characteristics")]
    [Range(3f, 10f)]
    [SerializeField] float visionRadius = MIN_VISION_RADIUS;
    [Range(0.5f, 10f)]
    [SerializeField] float speed = MIN_SPEED;

    public bool Recheck { get; set; }

    private bool _angry = false;
    public bool Angry
    {
        get { return _angry; }
        private set
        {
            _angry = value;
            slimeAnimator.SetBool("Angry", _angry);
        }
    }

    private EnemyState _state = EnemyState.Calm;

    public Vector2Int Position { get; private set; }

    private Vector2Int _moveDirection;
    private Vector2Int _moveWalkDirection;
    private Vector2Int goalPosition;

    private GameManager _gameManager;
    private DifficultyManager _difficultyManager;
    private TilemapManager _tilemapManager;
    private GameObject _player;

    private const float MIN_VISION_RADIUS = 1;
    private const float MIN_SPEED = 0.5f;
    private const float ANGRY_VISION_RADIUS = 100;
    private const float ANGRY_SPEED = 3;

    private void Awake()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _tilemapManager = GameObject.FindGameObjectWithTag("TilemapManager").GetComponent<TilemapManager>();
        _difficultyManager = GameObject.FindGameObjectWithTag("DifficultyManager").GetComponent<DifficultyManager>();
        visionRadius += AddVisionByDifficulty();
        speed += AddSpeedByDifficulty();
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        Position = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        goalPosition = Position;
    }

    private void Update()
    {
        if (_gameManager.State == GameState.Pause || _gameManager.State == GameState.GameEnd) return;
        if (!Recheck && Vector3.Distance(_player.transform.position, transform.position) > visionRadius)
        {
            Recheck = true;
            if (_state == EnemyState.Chase) _state = EnemyState.Calm;
        }
        else if (Recheck)
        {
            Recheck = false;
            _moveDirection = Astar.MoveDirection(Position);
            if (_moveDirection != Vector2Int.zero)
            {
                _state = EnemyState.Chase;
                goalPosition = Position + _moveDirection;
            }
            if (Mathf.Abs(_moveDirection.y) + Mathf.Abs(_moveDirection.y) == 1)
            {
                if (Vector3.Distance(transform.position, (Vector3Int)(Position + _moveDirection)) >= 1)
                {
                    goalPosition = Position;
                    _moveDirection = new(0, 0);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (_gameManager.State == GameState.Pause || _gameManager.State == GameState.GameEnd) return;
        Vector3 towards = _state switch
        {
            EnemyState.Chase => ((Vector3Int)_moveDirection),
            _ => (Vector3Int)_moveWalkDirection
        };
        Vector3 nextStep = Vector3.MoveTowards(transform.position, transform.position + towards, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, (Vector3Int)goalPosition) <= Vector3.Distance(nextStep, (Vector3Int)goalPosition))
        {
            transform.position = (Vector3Int)goalPosition;
            if (_state == EnemyState.Calm)
            {
                _moveWalkDirection = RandomWalkDirection();
                goalPosition = Position + _moveWalkDirection;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3Int)_moveWalkDirection, speed * Time.deltaTime);
            }
            else Recheck = true;
        }
        else transform.position = nextStep;
        Position = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    public void MakeAngry()
    {
        Angry = true;
        visionRadius = ANGRY_VISION_RADIUS;
        speed = ANGRY_SPEED;
    }

    private float AddVisionByDifficulty()
    {
        return _difficultyManager.GameDifficulty switch
        {
            Difficulty.Medium => 1,
            Difficulty.Hard => 2,
            _ => 0
        };
    }

    private float AddSpeedByDifficulty()
    {
        return _difficultyManager.GameDifficulty switch
        {
            Difficulty.Medium => 0.25f,
            Difficulty.Hard => 0.5f,
            _ => 0,
        };
    }
    
    private Vector2Int RandomWalkDirection()
    {
        Vector2Int indexes = _tilemapManager.PositionToMapIndexes(Position);
        List<(int, int)> directions = new();
        if (_tilemapManager.IsFineCoords(indexes.y - 1, indexes.x)) directions.Add((-1, 0));
        if (_tilemapManager.IsFineCoords(indexes.y, indexes.x - 1)) directions.Add((0, -1));
        if (_tilemapManager.IsFineCoords(indexes.y + 1, indexes.x)) directions.Add((1, 0));
        if (_tilemapManager.IsFineCoords(indexes.y, indexes.x + 1)) directions.Add((0, 1));
        if (directions.Count == 0) return new(0, 0);
        else
        {
            int randIndex = Random.Range(0, directions.Count);
            return new(directions[randIndex].Item2, directions[randIndex].Item1);
        }
    }
}

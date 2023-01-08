using UnityEngine;
using System.Collections.Generic;
using GameState = GameManager.GameState;
using Difficulty = DifficultyManager.Difficulty;

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
    private PlayerMovement _playerMovement;
    private PlayerHealthControl _playerHealth;

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
        _playerMovement = _player.GetComponent<PlayerMovement>();
        _playerHealth = _player.GetComponent<PlayerHealthControl>();
        Position = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        goalPosition = Position;
    }

    private void Update()
    {
        if (_gameManager.State == GameState.Pause || _gameManager.State == GameState.GameEnd) return;
        float distance = float.MaxValue;
        if (_player != null) distance = Vector3.Distance(_player.transform.position, transform.position);
        CheckPosition();
        if (distance > visionRadius && _state == EnemyState.Chase)
        {
            Recheck = true;
            _state = EnemyState.Calm;
            goalPosition = Position;
        }
        else if (distance < visionRadius && Recheck)
        {
            Recheck = false;
            _moveDirection = Astar.MoveDirection(Position);
            Vector2Int playerPos = new(_playerMovement.CellPosition.Item2, _playerMovement.CellPosition.Item1);
            if (_moveDirection == Vector2Int.zero && playerPos != Position)
            {
                _state = EnemyState.Calm;
                return;
            }
            _state = EnemyState.Chase;
            _moveWalkDirection = _moveDirection;
            if (Vector3.Distance(transform.position, (Vector3Int)(Position + _moveDirection)) > Vector3.Distance((Vector3Int)Position, (Vector3Int)(Position + _moveDirection)))
            {
                goalPosition = Position;
            }
            else goalPosition = Position + _moveDirection;
        }
    }

    private void FixedUpdate()
    {
        if (_gameManager.State == GameState.Pause || _gameManager.State == GameState.GameEnd) return;
        Vector3 nextStep = Vector3.MoveTowards(transform.position, (Vector3Int)goalPosition, speed * Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, (Vector3Int)goalPosition) <= Vector3.Distance(nextStep, (Vector3Int)goalPosition))
        {
            transform.position = (Vector3Int)goalPosition;
            Astar.AddPathToRoute(Position, new(_playerMovement.CellPosition.Item2, _playerMovement.CellPosition.Item1));
            if (_state == EnemyState.Calm)
            {
                _moveWalkDirection = RandomWalkDirection();
                goalPosition = Position + _moveWalkDirection;
            }
            else
            {
                _moveDirection = Astar.MoveDirection(Position);
                goalPosition = Position + _moveDirection;
            }
            transform.position = Vector3.MoveTowards(transform.position, (Vector3Int)goalPosition, speed * Time.fixedDeltaTime);
        }
        else transform.position = nextStep;
        if (transform.position != (Vector3Int)goalPosition) AdjustAnimator(goalPosition.y - transform.position.y, goalPosition.x - transform.position.x);
    }

    private void AdjustAnimator(float y, float x)
    {
        slimeAnimator.SetFloat("Direction_y", y);
        slimeAnimator.SetFloat("Direction_x", x);
    }

    private void CheckPosition()
    {
        Vector2Int actualPos = new(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (actualPos != Position)
        {
            Position = actualPos;
            Recheck = true;
        }
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

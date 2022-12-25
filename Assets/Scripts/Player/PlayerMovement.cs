using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player animator")]
    [SerializeField] Animator animator;

    [Header("Player rigidbody")]
    [SerializeField] Rigidbody2D rb2d;

    [Header("Player params")]
    [Range(0f, 10f)]
    [SerializeField] float speed = 2.5f;
    public float Speed
    {
        get { return speed; }
        set { if (value > 0 && value < 10) speed = value; }
    }

    private Vector2 _direction;
    private int _dir = 1;
    public int Dir
    {
        get { return _dir; }
        private set
        {
            _dir = value;
            transform.localScale = _dir == 4 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
        }
    }
    
    private const float EPSILON = 0.001f;

    private void OnEnable()
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;
    }

    private int FindDirection()
    {
        if (_direction.sqrMagnitude > EPSILON)
        {
            if (_direction.x > _direction.y) Dir = (_direction.x > -_direction.y) ? 2 : 1;
            else Dir = (_direction.x < -_direction.y) ? 4 : 3;
        }
        return Dir;
    }

    void Update()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");
        _direction.Normalize();

        animator.SetFloat("Horizontal", _direction.x);
        animator.SetFloat("Vertical", _direction.y);
        animator.SetFloat("Speed", _direction.magnitude);
        animator.SetInteger("Direction", FindDirection());
    }

    private void FixedUpdate() => rb2d.MovePosition(rb2d.position + speed * Time.fixedDeltaTime * _direction);
}

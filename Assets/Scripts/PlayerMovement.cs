using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private const float epsilon = 0.001f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D rb2d;

    [Range(0f, 10f)]
    [SerializeField]
    private float speed = 2.5f;
    public float Speed
    {
        get { return speed; }
        set
        {
            if (value > 0 && value < 10) speed = value;
        }
    }
    private Vector2 direction;
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

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;
    }

    private int FindDirection()
    {
        if (direction.sqrMagnitude > epsilon)
        {
            if (direction.x > direction.y) Dir = (direction.x > -direction.y) ? 2 : 1;
            else Dir = (direction.x < -direction.y) ? 4 : 3;
        }
        return Dir;
    }

    void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        direction.Normalize();

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.magnitude);
        animator.SetInteger("Direction", FindDirection());
    }

    private void FixedUpdate()
    {
        rb2d.MovePosition(rb2d.position + speed * Time.fixedDeltaTime * direction);
    }
}

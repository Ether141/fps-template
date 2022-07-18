using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public bool canMove = true;
    public bool canRun = true;
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 3f;
    [SerializeField] private LayerMask groundLayer;

    public bool IsMoving { get; private set; } = false;
    public bool IsRunning => Input.GetButton("Run") && IsMoving && VerticalMovement > 0f && canRun;
    public bool IsGrounded { get; private set; } = true;

    public float HorizontalMovement { get; private set; } = 0f;
    public float VerticalMovement { get; private set; } = 0f;

    public float Speed => canMove ? (IsRunning ? runSpeed : walkSpeed) : 0f;

    private Rigidbody rb;
    private CameraController cam;
    private AnimatorController animatorController;

    public CameraController Camera => cam;
    public AnimatorController AnimatorController => animatorController;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        rb = GetComponent<Rigidbody>();
        animatorController = GetComponent<AnimatorController>();
    }

    private void Update()
    {
        CollectInput();
        PlayerRotation();
        GroundedStatus();
        Jumping();
    }

    private void FixedUpdate()
    {
        IsMoving = VerticalMovement != 0f || HorizontalMovement != 0f;

        if (!canMove)
            return;

        Vector3 v = ((transform.right * HorizontalMovement) + (transform.forward * VerticalMovement)).normalized * Speed;
        rb.velocity = new Vector3(v.x, rb.velocity.y, v.z);
    }

    private void Jumping()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded)
        {
            rb.AddForce(10f * jumpForce * transform.up);
        }
    }

    private void PlayerRotation()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void CollectInput()
    {
        HorizontalMovement = Input.GetAxisRaw("Horizontal");
        VerticalMovement = Input.GetAxisRaw("Vertical");
    }

    private void GroundedStatus()
    {
        Debug.DrawLine(transform.position - new Vector3(0f, 0.9f, 0f), transform.position - new Vector3(0f, 0.9f, 0f) + (-transform.up * 0.5f), Color.red);
        IsGrounded = Physics.Raycast(transform.position - new Vector3(0f, 0.9f, 0f), -transform.up, 0.5f, groundLayer) && Mathf.Abs(rb.velocity.y) <= 1f;
    }
}

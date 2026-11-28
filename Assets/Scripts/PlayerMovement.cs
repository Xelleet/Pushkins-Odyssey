using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float changeSpeed = 1f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundDistanceCheck = 1f;

    [SerializeField] private float currentSpeed;

    [SerializeField] private Camera playerCamera;

    private InputHandler inputHandler;

    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
    }

    private void Start()
    {
        currentSpeed = speed;
    }

    private void Update()
    {
        Move();
    }

    public Vector3 GetMoveDir()
    {
        return rb.linearVelocity;
    }

    private void RotateCharacter(Vector3 direction)
{
    if (direction.sqrMagnitude < 0.01f)
        return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);
    
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            150f * Time.deltaTime
        );
}

    private void Move()
    {  
        float targetSpeed = inputHandler.SprintPressed ? sprintSpeed : speed;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, changeSpeed * Time.deltaTime);

        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * inputHandler.MoveInput.y + camRight * inputHandler.MoveInput.x).normalized;
        rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);

        RotateCharacter(new Vector3(rb.linearVelocity.normalized.x, 0, rb.linearVelocity.normalized.z));


        if (inputHandler.JumpPressed && OnGround())
        {
            Jump();
            inputHandler.ConsumeJump();
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    public bool OnGround()
    {
        bool ray = Physics.Raycast(transform.position, Vector3.down, groundDistanceCheck);
        return ray;
    }
}

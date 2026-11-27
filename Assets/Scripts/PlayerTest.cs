using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 6f;

    [SerializeField] private Camera playerCamera;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {   
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 camRight = playerCamera.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        
        //ToDO: Заменить эту херобору на нормальный Input Actions
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = camForward * z + camRight * x;

        rb.linearVelocity = new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed);

        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
        {
            Jump();
        }
    }

    private void Jump()
    {
        Debug.Log(123);
        rb.AddForce(new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z), ForceMode.VelocityChange);
    }

    private bool OnGround()
    {
        bool ray = Physics.Raycast(transform.position, Vector3.down, 1.2f);
        return ray;
    }
}

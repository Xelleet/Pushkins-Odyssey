using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        SetDir();
        SetParam("IsInAir", playerMovement.OnGround());
        Debug.Log(playerMovement.OnGround());
    }

    private void SetParam(string param, bool status)
    {
        animator.SetBool(param ,status);
    }

    private void SetDir()
    {
        Vector3 moveDir = playerMovement.GetMoveDir();

        float x = Vector3.Dot(transform.right, moveDir);
        float y = Vector3.Dot(transform.forward, moveDir);

        animator.SetFloat("X", x, 0.03f, Time.deltaTime);
        animator.SetFloat("Y", y, 0.03f, Time.deltaTime);
        animator.SetFloat("AirY", moveDir.y, 0.03f, Time.deltaTime);

    }
}

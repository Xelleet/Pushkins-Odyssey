
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputSystem controls;

    public Vector2 MoveInput { get; private set; }

    public Vector2 CameraRotation {get; private set;}

    public bool JumpPressed { get; private set; }
    public bool SprintPressed {get; private set;}

    private void Awake()
    {
        controls = new InputSystem();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Movement.Jump.performed += OnJump;
        controls.Movement.Sprint.performed += OnSprint;
    }

    private void OnDisable()
    {
        controls.Movement.Jump.performed -= OnJump;
        controls.Movement.Sprint.performed -= OnSprint;

        controls.Disable();
    }

    private void Update()
    {
        MoveInput = controls.Movement.Move
            .ReadValue<Vector2>();
        CameraRotation = controls.Camera.Look.ReadValue<Vector2>();
        //SprintPressed = true ? controls.Movement.Sprint.ReadValue<float>() == 1 : false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        JumpPressed = true;
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        SprintPressed = !SprintPressed;
    }

    public void ConsumeJump()
    {
        JumpPressed = false;
    }
}

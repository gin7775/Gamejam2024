using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public CharacterController controller;

    private Vector2 moveInput;
    private Vector2 lookInput; // Input del ratón
    private Vector2 lookInputController; // Input del joystick
    private bool isUsingController = false; // Booleano que indica si se está usando el mando o el ratón
    private Camera mainCamera;
    public float gravity = 9.81f;
    private Vector3 velocity;
    private Animator animator;

    private GameManager gameManager;
    private void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
        ApplyGravity();
        Animations();
        // Decidir si usar el input del mando o del ratón basado en el valor de `isUsingController`
        if (isUsingController)
        {
            RotateController(); // Usa el input del mando
        }
        else
        {
            RotateMouse(); // Usa el input del ratón
        }
    }


    public void OnMenu(InputValue value)
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.pause();

    }
    // Método llamado por el Player Input para el movimiento
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Método llamado por el Player Input para el ratón
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>(); // Capturamos el input del ratón
        if (lookInput.sqrMagnitude > 0.01f)
        {
            isUsingController = false; // Si hay input del ratón, se desactiva el control del mando
        }
    }
    public void Animations()
    {
        float characterSpeed = controller.velocity.magnitude;
        if (animator != null)
        {
            animator.SetFloat("SpeedBlendTree", characterSpeed);

        }
    }
    // Método llamado por el Player Input para el joystick derecho
    public void OnLookController(InputValue value)
    {
        lookInputController = value.Get<Vector2>(); // Capturamos el input del joystick
        if (lookInputController.sqrMagnitude > 0.01f)
        {
            isUsingController = true; // Si hay input del mando, se activa el control del mando
        }
    }

    private void Movement()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * moveInput.y + right * moveInput.x;
        controller.Move(desiredMoveDirection * speed * Time.deltaTime);
    }

    // Rotación basada en el input del joystick
    private void RotateController()
    {
        if (lookInputController.sqrMagnitude > 0.01f) // Si el joystick está en movimiento
        {
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calcular la dirección basada en el input del joystick y la cámara
            Vector3 desiredDirection = forward * lookInputController.y + right * lookInputController.x;

            if (desiredDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Método para rotación con el ratón utilizando Raycasting
    private void RotateMouse()
    {
        if (Mouse.current != null && Mouse.current.position.ReadValue().sqrMagnitude > 0.01f) // Verifica si el ratón se está moviendo
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 targetPosition = hitInfo.point;
                targetPosition.y = transform.position.y; // Mantener la altura constante
                Vector3 direction = targetPosition - transform.position;

                if (direction.sqrMagnitude > 0.01f) // Verificar si hay un cambio significativo en la posición
                {
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}

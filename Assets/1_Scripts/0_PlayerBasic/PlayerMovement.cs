using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UIElements;
public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movimiento y Dash Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private ParticleSystem dashParticle;

   
    [Header("Control de Input")]
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 lookInputController;
    private bool isUsingController = false;

    
    [Header("Referencias a Componentes")]
    [SerializeField] private CharacterController controller;
    private Collider playerCollider;
    private Camera mainCamera;
    private Animator animator;
    private GameManager gameManager;
    public GameObject dashIcon;

   
    [Header("Estados del Jugador")]
    private bool isDashing = false;
    private bool canDash = true;

   
    [Header("Física")]
    [SerializeField] public float gravity = 9.81f;
    private Vector3 velocity;

    private Vector3 waterCurrentDirection = Vector3.zero;
    private float waterCurrentForce = 0f;
    private bool inWaterCurrent = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        playerCollider = this.GetComponent<Collider>();
        dashIcon.gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isDashing) Movement();
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
    public void OnDash(InputValue value)
    {
        if (canDash)
        {
            StartCoroutine(PerformDash());
        }
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

        if (inWaterCurrent)
        {
            float dotProduct = Vector3.Dot(desiredMoveDirection.normalized, waterCurrentDirection.normalized);

            
            Vector3 currentEffect = (dotProduct < 0)
                ? waterCurrentDirection * (waterCurrentForce * 0.5f)
                : waterCurrentDirection * waterCurrentForce;

            desiredMoveDirection += currentEffect;
        }
        controller.Move(desiredMoveDirection * speed * Time.deltaTime);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;  // Desactivar el dash hasta que se complete el cooldown
        isDashing = true;
        dashParticle.Play();
        dashIcon.gameObject.SetActive(false);

        SetCollisionWithEnemies(false);                              // Hacer al jugador inmune y no detectable
        gameObject.layer = LayerMask.NameToLayer("Invisible");      
       

        float dashStartTime = Time.time;
        Vector3 dashDirection = mainCamera.transform.forward * moveInput.y + mainCamera.transform.right * moveInput.x;
        dashDirection.y = 0; // No aplicar fuerza en el eje Y

        // Movimiento del dash
        while (Time.time < dashStartTime + dashDuration)
        {
            controller.Move(dashDirection.normalized * dashSpeed * Time.deltaTime);
            yield return null;
        }

        // Fin del dash
        isDashing = false;
        SetCollisionWithEnemies(true);
        gameObject.layer = LayerMask.NameToLayer("Default"); // Cambiar de nuevo la capa para que sea detectable

        // Cooldown del dash
        yield return new WaitForSeconds(dashCooldown);
        dashIcon.gameObject.SetActive(true);
        canDash = true;  // Ahora el dash se puede usar nuevamente
    }

    // Método para activar o desactivar las colisiones con los enemigos
    private void SetCollisionWithEnemies(bool enableCollision)
    {
        // Buscar todos los enemigos activos cada vez que se hace el dash
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, enemyCollider, !enableCollision);
                
            }
        }
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

    public void ApplyWaterCurrent(Vector3 direction, float force)
    {
        waterCurrentDirection = direction;
        waterCurrentForce = force;
        inWaterCurrent = true;
    }

    
    public void RemoveWaterCurrent()
    {
        inWaterCurrent = false;
        waterCurrentDirection = Vector3.zero;
        waterCurrentForce = 0f;
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

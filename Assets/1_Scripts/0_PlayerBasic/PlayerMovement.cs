using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.VFX;
public class PlayerMovement : MonoBehaviour
{
    
    [Header("Movimiento y Dash Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private ParticleSystem dashParticle;
    [SerializeField] private VisualEffect walkingEffect;
    private Coroutine walkingEffectCoroutine;
    [SerializeField] private float particleTimeToSpawn;
    [SerializeField] private ParticleSystem jumpEffect; 
    [SerializeField] private float heightThreshold = 0.5f; 
    private float lastGroundHeight = 0f;  
    private bool hasActivatedHeightEffect = false;
    private bool wasGrounded = true;
    private float groundContactDelay = 0.1f; // Tiempo de espera tras aterrizar
    private float lastGroundContactTime = 0f; // �ltimo momento en que toc� el suelo

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
    private bool isWalking = false;

    [Header("F�sica")]
    [SerializeField] public float gravity = 9.81f;
    private Vector3 velocity;

    [Header("Cruceta Settings")]
    [SerializeField] private GameObject crosshair; // Referencia al sprite de la cruceta
    [SerializeField] private float crosshairDistance = 2f;
    [SerializeField] private LayerMask terrainLayer;

    [Header("Water")]
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
        // Decidir si usar el input del mando o del rat�n basado en el valor de `isUsingController`
        if (isUsingController)
        {
            RotateController(); // Usa el input del mando
        }
        else
        {
            RotateMouse(); // Usa el input del rat�n
        }
        IsGround();
        UpdateCrosshairPositionWithRotation();
        
        CheckLanding();
    }


    public void OnMenu(InputValue value)
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.Pause();

    }
    // M�todo llamado por el Player Input para el movimiento
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

    // M�todo llamado por el Player Input para el rat�n
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>(); // Capturamos el input del rat�n
        if (lookInput.sqrMagnitude > 0.01f)
        {
            isUsingController = false; // Si hay input del rat�n, se desactiva el control del mando
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
    // M�todo llamado por el Player Input para el joystick derecho
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

    // M�todo para activar o desactivar las colisiones con los enemigos
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

    // Rotaci�n basada en el input del joystick
    private void RotateController()
    {
        if (lookInputController.sqrMagnitude > 0.01f) // Si el joystick est� en movimiento
        {
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // Calcular la direcci�n basada en el input del joystick y la c�mara
            Vector3 desiredDirection = forward * lookInputController.y + right * lookInputController.x;

            if (desiredDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // M�todo para rotaci�n con el rat�n utilizando Raycasting
    private void RotateMouse()
    {
        if (Mouse.current != null && Mouse.current.position.ReadValue().sqrMagnitude > 0.01f) // Verifica si el rat�n se est� moviendo
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 targetPosition = hitInfo.point;
                targetPosition.y = transform.position.y; // Mantener la altura constante
                Vector3 direction = targetPosition - transform.position;

                if (direction.sqrMagnitude > 0.01f) // Verificar si hay un cambio significativo en la posici�n
                {
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    private void UpdateCrosshairPositionWithRotation()
    {
        if (crosshair != null)
        {
            // Obtener la direcci�n hacia la que est� mirando el jugador
            Vector3 playerForward = transform.forward;
            playerForward.y = 0;  // Ignoramos la componente Y para rotar solo alrededor del eje Y
            playerForward.Normalize();

            // Calculamos la posici�n de la cruceta en un c�rculo alrededor del jugador
            Vector3 crosshairPosition = transform.position + playerForward * crosshairDistance;

            // Raycast para obtener la altura del terreno en la posici�n de la cruceta (usando el LayerMask)
            RaycastHit hitInfo;
            if (Physics.Raycast(crosshairPosition + Vector3.up * 2f, Vector3.down, out hitInfo, Mathf.Infinity, terrainLayer))
            {
                // Ajustamos la posici�n de la cruceta para que coincida con la altura del terreno
                crosshairPosition.y = hitInfo.point.y + 0.25f; // Ajustar el +1f para elevar la cruceta un poco por encima del terreno
            }

            // Asignar la nueva posici�n de la cruceta
            crosshair.transform.position = crosshairPosition;

            // Rotar la cruceta para que mire en la misma direcci�n que el jugador (alrededor del eje Y)
            Quaternion lookRotation = Quaternion.LookRotation(playerForward);
            crosshair.transform.rotation = Quaternion.Euler(90f, lookRotation.eulerAngles.y, 0f); // Asegurando que solo rote alrededor del eje Y
        }

    }
    private void CheckLanding()
    {
        // Si el jugador est� en el aire y ahora est� en el suelo
        if (!wasGrounded && controller.isGrounded)
        {
            // Verificar si el tiempo desde el �ltimo contacto es mayor al delay
            if (Time.time - lastGroundContactTime > groundContactDelay)
            {
                // Calcular la diferencia de altura al aterrizar
                float fallHeight = lastGroundHeight - transform.position.y;

                if (fallHeight >= heightThreshold)
                {
                    PlayJumpEffect(); // Reproducir el efecto de part�cula
                }

                // Actualizar la �ltima altura del suelo
                lastGroundHeight = transform.position.y;

                // Actualizar el tiempo del �ltimo contacto
                lastGroundContactTime = Time.time;
            }
        }

        // Si est� tocando el suelo, actualizar la altura
        if (controller.isGrounded)
        {
            lastGroundHeight = transform.position.y;
        }

        // Actualizar el estado anterior
        wasGrounded = controller.isGrounded;
    }
    private void PlayJumpEffect()
    {
        if (jumpEffect != null)
        {
            jumpEffect.Play();
        }
    }

    private void IsGround()
    {
        if (controller.isGrounded && moveInput.magnitude > 0.1f) // Usamos moveInput para verificar movimiento
        {
            if (!isWalking)
            {
                // Si el jugador comienza a caminar, empezar el efecto visual
                isWalking = true;
                if (walkingEffectCoroutine == null)
                {
                    walkingEffectCoroutine = StartCoroutine(PlayWalkingEffect());
                }
            }
        }
        else
        {
            if (isWalking)
            {
                // Si el jugador deja de caminar o no est� tocando el suelo, detener el efecto visual
                isWalking = false;
                if (walkingEffectCoroutine != null)
                {
                    StopCoroutine(walkingEffectCoroutine);
                    walkingEffectCoroutine = null;
                }

                // Detener las part�culas si deja de caminar
                walkingEffect.Stop();
            }
        }


    }



    private IEnumerator PlayWalkingEffect()
    {
        while (isWalking)
        {
            // Activar la part�cula
            walkingEffect.Play();

            // Esperar 1 segundo
            yield return new WaitForSeconds(particleTimeToSpawn);

            // Desactivar la part�cula despu�s de 1 segundo
            walkingEffect.Stop();

            // Esperar 1 segundo antes de volver a activarla
            yield return new WaitForSeconds(particleTimeToSpawn);
        }
    }

    //private void JumpParticle()
    //{
    //    bool isGrounded = controller.isGrounded; // Verificar si el jugador est� tocando el suelo
    //    Debug.Log($"Is player grounded: {isGrounded}");

    //    if (!isGrounded)
    //    {
    //        // Si el jugador est� en el aire, medir la distancia desde la �ltima altura registrada
    //        float heightDifference = Mathf.Abs(transform.position.y - lastGroundHeight); // Asegurarnos de que la diferencia sea positiva
    //        Debug.Log($"Height difference: {heightDifference}");

    //        if (!hasActivatedHeightEffect && heightDifference >= heightThreshold)
    //        {
    //            // Si ha superado la distancia m�nima, activar la part�cula
    //            if (jumpEffect != null)
    //            {
    //                Debug.Log("Activating jump effect!");
    //                jumpEffect.Play();
    //            }
    //            hasActivatedHeightEffect = true; // Asegurarse de que la part�cula solo se active una vez
    //        }
    //    }
    //    else
    //    {
    //        // Si el jugador toca el suelo, detener la part�cula
    //        if (jumpEffect != null && jumpEffect.isPlaying)
    //        {
    //            Debug.Log("Deactivating jump effect!");
    //            jumpEffect.Stop();
    //        }

    //        hasActivatedHeightEffect = false;  // Reiniciar para la pr�xima vez que el jugador se eleve

    //        // Actualizar la altura de la �ltima vez que toc� el suelo
    //        lastGroundHeight = transform.position.y;
    //        Debug.Log($"Updating last ground height: {lastGroundHeight}");
    //    }
    //}

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

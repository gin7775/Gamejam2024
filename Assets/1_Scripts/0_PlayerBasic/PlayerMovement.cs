using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.VFX;
using MoreMountains.Feedbacks;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento y Dash Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private float dashCooldown = 2f;
    public Material dashMaterialInstance;
    private Color dashReadyColor = new Color32(191, 191, 191, 255);
    private Color dashCooldownColor = new Color32(191, 103, 0, 255);

    [SerializeField] private VisualEffect walkingEffect;
    private Coroutine walkingEffectCoroutine;
    [SerializeField] private float particleTimeToSpawn;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private float heightThreshold = 0.5f;
    private float lastGroundHeight = 0f;
    private bool hasActivatedHeightEffect = false;
    private bool wasGrounded = true;
    private float groundContactDelay = 0.1f;
    private float lastGroundContactTime = 0f;

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
    

    [Header("Estados del Jugador")]
    private bool isDashing = false;
    private bool canDash = true;
    private bool isWalking = false;

    [Header("Física")]
    [SerializeField] public float gravity = 9.81f;
    private Vector3 velocity;

    [Header("Cruceta Settings")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private float crosshairDistance = 2f;
    [SerializeField] private LayerMask terrainLayer;

    [Header("Water")]
    private Vector3 waterCurrentDirection = Vector3.zero;
    private float waterCurrentForce = 0f;
    private bool inWaterCurrent = false;
    public MMFeedbacks dashFeedback;

    private void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();
        
    }

    private void Update()
    {
        if (!isDashing) Movement();
        ApplyGravity();
        Animations();

        if (isUsingController)
            RotateController();
        else
            RotateMouse();

        IsGround();
        UpdateCrosshairPositionWithRotation();
        CheckLanding();
    }

    public void OnMenu(InputValue value)
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.Pause();
    }

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

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        if (lookInput.sqrMagnitude > 0.01f)
            isUsingController = false;
    }

    public void OnLookController(InputValue value)
    {
        lookInputController = value.Get<Vector2>();
        if (lookInputController.sqrMagnitude > 0.01f)
            isUsingController = true;
    }

    public void Animations()
    {
        float characterSpeed = controller.velocity.magnitude;
        if (animator != null)
        {
            animator.SetFloat("SpeedBlendTree", characterSpeed, 0.1f, Time.deltaTime);
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
            Vector3 currentEffect = (dotProduct < 0) ?
                waterCurrentDirection * (waterCurrentForce * 0.5f) :
                waterCurrentDirection * waterCurrentForce;

            desiredMoveDirection += currentEffect;
        }

        controller.Move(desiredMoveDirection * speed * Time.deltaTime);
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        VFXManager.Instance.PlayEffect("DashParticle", transform, new Vector3(0f, 0.7f, 0f), Quaternion.identity);
        dashFeedback?.PlayFeedbacks();
        

        StartCoroutine(LerpDashMaterialColor(dashReadyColor, dashCooldownColor, 0.3f));

        SetCollisionWithEnemies(false);
        gameObject.layer = LayerMask.NameToLayer("Invisible");

        float dashStartTime = Time.time;
        Vector3 dashDirection = moveInput.sqrMagnitude > 0.01f ?
            mainCamera.transform.forward * moveInput.y + mainCamera.transform.right * moveInput.x :
            transform.forward;
        dashDirection.y = 0;

        while (Time.time < dashStartTime + dashDuration)
        {
            controller.Move(dashDirection.normalized * dashSpeed * Time.deltaTime);
            yield return null;
        }

        isDashing = false;
        SetCollisionWithEnemies(true);
        gameObject.layer = LayerMask.NameToLayer("Default");

        yield return new WaitForSeconds(dashCooldown);
       
        StartCoroutine(LerpDashMaterialColor(dashCooldownColor, dashReadyColor, 0.3f));
        canDash = true;
    }

    private IEnumerator LerpDashMaterialColor(Color fromColor, Color toColor, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color currentColor = Color.Lerp(fromColor, toColor, elapsed / duration);
            dashMaterialInstance.SetColor("_Color", currentColor * 2.2f); // Multiplicamos por la intensidad fija
            yield return null;
        }
        dashMaterialInstance.SetColor("_Color", toColor * 2.2f); // Aseguramos que quede fijo al final
    }

    private void SetCollisionWithEnemies(bool enableCollision)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Collider enemyCollider = enemy.GetComponent<Collider>();
            if (enemyCollider != null)
                Physics.IgnoreCollision(playerCollider, enemyCollider, !enableCollision);
        }
    }

    private void RotateController()
    {
        if (lookInputController.sqrMagnitude > 0.01f)
        {
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredDirection = forward * lookInputController.y + right * lookInputController.x;
            if (desiredDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void RotateMouse()
    {
        if (Mouse.current != null && Mouse.current.position.ReadValue().sqrMagnitude > 0.01f)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 targetPosition = hitInfo.point;
                targetPosition.y = transform.position.y;
                Vector3 direction = targetPosition - transform.position;

                if (direction.sqrMagnitude > 0.01f)
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
            Vector3 playerForward = transform.forward;
            playerForward.y = 0;
            playerForward.Normalize();

            Vector3 crosshairPosition = transform.position + playerForward * crosshairDistance;

            if (Physics.Raycast(crosshairPosition + Vector3.up * 2f, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, terrainLayer))
                crosshairPosition.y = hitInfo.point.y + 0.25f;

            crosshair.transform.position = crosshairPosition;
            Quaternion lookRotation = Quaternion.LookRotation(playerForward);
            crosshair.transform.rotation = Quaternion.Euler(90f, lookRotation.eulerAngles.y, 0f);
        }
    }

    private void CheckLanding()
    {
        if (!wasGrounded && controller.isGrounded)
        {
            if (Time.time - lastGroundContactTime > groundContactDelay)
            {
                float fallHeight = lastGroundHeight - transform.position.y;
                if (fallHeight >= heightThreshold)
                    PlayJumpEffect();

                lastGroundHeight = transform.position.y;
                lastGroundContactTime = Time.time;
            }
        }

        if (controller.isGrounded)
            lastGroundHeight = transform.position.y;

        wasGrounded = controller.isGrounded;
    }

    private void PlayJumpEffect()
    {
        if (jumpEffect != null)
            VFXManager.Instance.PlayEffect("BigJumpParticle", transform, Vector3.zero, Quaternion.Euler(-90f, 0f, 0f));
    }

    private void IsGround()
    {
        if (controller.isGrounded && moveInput.magnitude > 0.1f)
        {
            if (!isWalking)
            {
                isWalking = true;
                if (walkingEffectCoroutine == null)
                    walkingEffectCoroutine = StartCoroutine(PlayWalkingEffect());
            }
        }
        else
        {
            if (isWalking)
            {
                isWalking = false;
                if (walkingEffectCoroutine != null)
                {
                    StopCoroutine(walkingEffectCoroutine);
                    walkingEffectCoroutine = null;
                }
                walkingEffect.Stop();
            }
        }
    }

    private IEnumerator PlayWalkingEffect()
    {
        while (isWalking)
        {
            walkingEffect.Play();
            yield return new WaitForSeconds(particleTimeToSpawn);
            walkingEffect.Stop();
            yield return new WaitForSeconds(particleTimeToSpawn);
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
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = -2f;

        controller.Move(velocity * Time.deltaTime);
    }
}

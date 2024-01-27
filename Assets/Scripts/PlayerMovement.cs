using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public CharacterController controller;

    private Vector2 moveInput;
    private Vector2 rotateInput;
    private Animator animator;
    private Camera mainCamera;

    public float gravity = 9.81f; // Valor típico de gravedad, ajusta según necesites.
    private Vector3 velocity;
    private void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
        Rotate();
        Animations();
        ApplyGravity();
       
    }

    public void Animations()
    {
        float characterSpeed = controller.velocity.magnitude; 
        if(animator != null)
        {
            animator.SetFloat("SpeedBlendTree", characterSpeed);

        }
    }

    // Método público llamado por el Player Input Component para movimiento
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Método público llamado por el Player Input Component para rotación
    public void OnLook(InputValue value)
    {
        rotateInput = value.Get<Vector2>();
        
    }

    private void Movement()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * moveInput.y + right * moveInput.x);
        controller.Move(desiredMoveDirection * speed * Time.deltaTime);
    }

    private void Rotate()
    {
        if (Mouse.current != null ) // Verificar si hay un mouse conectado
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
        else if (rotateInput.sqrMagnitude > 0.01f) // Rotación con el joystick derecho
        {
            Vector3 direction = new Vector3(rotateInput.x, 0f, rotateInput.y);
            RotateTowardsDirection(direction);
            Debug.Log("Mueve");
        }

    }
    private void RotateTowardsDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize(); // Normalizar la dirección
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        // Aplicar gravedad
        if (!controller.isGrounded) // Comprobar si el controlador de personaje está en el suelo
        {
            velocity.y -= gravity * Time.deltaTime; // Aplicar la gravedad a la velocidad vertical
        }
        else
        {
            // Si está en el suelo, resetea la velocidad vertical
            velocity.y = -2f; // Puedes ajustar esto según sea necesario
        }

        // Aplicar la velocidad a la posición
        controller.Move(velocity * Time.deltaTime);
    }
}

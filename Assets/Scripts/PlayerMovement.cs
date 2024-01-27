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

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Movement();
        Rotate();
    }

    // Método público llamado por el Player Input Component para movimiento
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Método público llamado por el Player Input Component para rotación
    public void OnRotate(InputValue value)
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
        if (Mouse.current != null) // Verificar si hay un mouse conectado
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
        else if (rotateInput.sqrMagnitude > 0.01f) // Umbral para evitar movimientos pequeños involuntarios
        {
            Vector3 direction = new Vector3(rotateInput.x, 0f, rotateInput.y);
            RotateTowardsDirection(direction);
        }

    }
    private void RotateTowardsDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

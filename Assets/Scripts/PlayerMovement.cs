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
        // Aplicar rotación basada en el joystick derecho
        if (rotateInput.sqrMagnitude > 0.01f) // Umbral para evitar movimientos pequeños involuntarios
        {
            float rotation = rotateInput.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 100f;
    public CharacterController controller;

    private Vector2 moveInput;
    private Vector2 mouseInput;

    private Camera mainCamera;
    private float xRotation = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Movement();
        Rotate();
    }

    // Método público llamado por el Player Input Component
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("Move Input: " + moveInput);
    }
    public void OnPrueba(InputValue value)
    {
        Debug.Log("Prueba");
    }

    // Método público llamado por el Player Input Component
    //public void OnLook(InputValue value)
    //{
    //    mouseInput = value.Get<Vector2>();
    //}

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
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

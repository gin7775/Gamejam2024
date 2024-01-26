using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.WSA;

public class riki_PlayerMovement : MonoBehaviour
{
    public float speed = 0.05f;
    public float rotationSpeed = 2f;
    public float minY = -90f;
    public float maxY = 90f;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        //UnityEngine.Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotate();
    }

    private void Movement()
    {
        Vector3 movement = transform.position;

        if (Input.GetKey(KeyCode.A))
        {
            float horizontal = Input.GetAxis("Horizontal");
            //float vertical = Input.GetAxis("Vertical");
            float vertical = transform.position.y;
            movement = new Vector3(transform.position.x + speed, 0.5f, vertical);
        }
        if (Input.GetKey(KeyCode.W))
        {
            //float horizontal = Input.GetAxis("Horizontal");
            float horizontal = transform.position.x;
            float vertical = Input.GetAxis("Vertical");
            movement = new Vector3(horizontal, 0.5f, transform.position.z + speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            //float horizontal = Input.GetAxis("Horizontal");
            float horizontal = transform.position.x;
            float vertical = Input.GetAxis("Vertical");
            movement = new Vector3(horizontal, 0.5f, transform.position.z - speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            float horizontal = Input.GetAxis("Horizontal");
            //float vertical = Input.GetAxis("Vertical");
            float vertical = transform.position.y;
            movement = new Vector3(transform.position.x - speed, 0.5f, vertical);
        }

        transform.position = movement;
    }

    private void Rotate()
    {
        float distance = 10f; // Distancia desde la cámara al plano imaginario

        //float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        //transform.Rotate(Vector3.up * rotation);
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Obtener la posición deseada en el plano imaginario
        Vector3 targetPosition = ray.GetPoint(distance);

        // Mantener la posición Y constante
        targetPosition.y = transform.position.y;

        // Hacer que el objeto mire hacia la posición calculada
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = targetRotation;
    }

}

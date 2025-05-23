using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class VFX_Score : MonoBehaviour
{
    private Transform mainCam;

    public float lifetime = 2f;


    [Header("Configuración")]
    public float upwardSpeed = 2f;           // velocidad inicial hacia arriba
    public float deceleration = 3f;          // desaceleración progresiva
    public float maxHeight = 1f;             // altura máxima desde el punto inicial
    private float currentSpeed;
    private Vector3 startPosition;


    private void Start()
    {
        Destroy(gameObject, lifetime);
        mainCam = Camera.main.transform;

        currentSpeed = upwardSpeed;
        startPosition = transform.position;


    }

    private void LateUpdate()
    {
        // Gira el objeto para que mire hacia la cámara (sin invertir)
        transform.LookAt(transform.position + mainCam.forward);

        // Mover hacia arriba con desaceleración
        if (Vector3.Distance(transform.position, startPosition) < maxHeight)
        {
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;
            currentSpeed = Mathf.Max(0f, currentSpeed - deceleration * Time.deltaTime);
        }

    }

    public void SetText(string text)
    {
        GetComponent<TextMeshPro>().text = text;
    }
}

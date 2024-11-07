using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health_Movement : MonoBehaviour
{
    public Transform player; // Asigna aquí la referencia al jugador
    public float detectionDistance = 5f; // Distancia a la que el NPC detecta al jugador
    public float escapeDistance = 10f; // Distancia a la que el NPC se detiene
    public float restTime = 3f; // Tiempo de descanso en segundos
    public float staminaTime = 5f; // Tiempo máximo de resistencia
    public float fleeSpeed = 5f; // Velocidad de huida

    public NavMeshAgent agent;
    private bool isExhausted = false;
    private bool isGettingExhausted = false;
    private float timeRunning;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionDistance && !isExhausted)
        {
            FleeFromPlayer();
        }
        else if (distanceToPlayer > detectionDistance && !isExhausted)
        {
            isGettingExhausted = false; // Reinicia el agotamiento si el jugador se aleja
        }

        // Manejo de resistencia
        if (isGettingExhausted)
        {
            timeRunning += Time.deltaTime;
            if (timeRunning >= staminaTime) // Usamos >= para activar agotamiento
            {
                isExhausted = true;
                StartCoroutine(Rest()); // Inicia el descanso
            }
        }

        // Recuperación de stamina si está en reposo
        if (agent.remainingDistance <= 1f && !isExhausted)
        {
            timeRunning = Mathf.Max(0, timeRunning - Time.deltaTime / 2); // Reduce stamina
        }
    }

    private void FleeFromPlayer()
    {
        agent.speed = fleeSpeed;
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleePosition = transform.position + fleeDirection * escapeDistance;

        agent.SetDestination(fleePosition);
        isGettingExhausted = true; // Empieza a contar tiempo de huida
    }

    private IEnumerator Rest()
    {
        agent.isStopped = true; // Detiene movimiento durante el descanso
        yield return new WaitForSeconds(restTime);

        isExhausted = false; // Recupera la stamina después de descansar
        timeRunning = 0; // Reinicia el contador de tiempo corriendo
        agent.isStopped = false; // Reanuda movimiento si es necesario
    }
}

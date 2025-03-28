using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Prefab del proyectil
    [SerializeField] private Transform shootingPoint; // Punto de origen del disparo
    [SerializeField] private float projectileSpeed = 10f; // Velocidad del proyectil
    [SerializeField] private float fireRate = 2f; // Frecuencia de disparo (Disparos por segundo)
    [SerializeField] private float stopDistance = 10f; // Distancia m�nima para detenerse y disparar
    [SerializeField] private float moveDistance = 15f; // Distancia m�xima para moverse hacia el jugador

    private Transform playerTransform; // Referencia al jugador
    private float fireCooldown; // Controla el tiempo entre disparos
    private NavMeshAgent agent; // Componente NavMeshAgent para el movimiento

    private void Start()
    {
        // Encontrar al jugador por etiqueta
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        // Configurar la distancia m�nima a la que el agente se detiene
        agent.stoppingDistance = stopDistance;
        fireCooldown = 0f;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Actualizar el cooldown para disparar
        fireCooldown -= Time.deltaTime;

        // Calcular la distancia al jugador
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Si el enemigo est� fuera de la distancia de movimiento, se dirige al jugador
        if (distanceToPlayer > moveDistance)
        {
            agent.isStopped = false; // Activar el movimiento
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            // Si est� dentro de cualquier distancia de disparo, se detiene
            agent.isStopped = true;

            // Mirar hacia el jugador
            Vector3 lookPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookPosition);

            // Disparar siempre que est� quieto y el cooldown ha terminado
            if (fireCooldown <= 0f)
            {
                ShootAtPlayer();
                fireCooldown = 1f / fireRate; // Reiniciar el cooldown basado en la tasa de disparo
            }
        }
    }

    private void ShootAtPlayer()
    {
        if (playerTransform != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            // Obtener la direcci�n hacia el jugador en el momento del disparo
            Vector3 directionToPlayer = playerTransform.position - shootingPoint.position;

            // Eliminar la componente Y para asegurar un disparo plano
            directionToPlayer.y = 0f;

            // Normalizar la direcci�n para que tenga longitud 1
            directionToPlayer = directionToPlayer.normalized;

            // Aplicar movimiento al proyectil
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = directionToPlayer * projectileSpeed;
            }
        }
    }
}
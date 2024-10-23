using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    // Parámetros configurables
    [SerializeField] private GameObject projectilePrefab; // Prefab del proyectil
    [SerializeField] private Transform shootingPoint; // Punto de origen del disparo
    [SerializeField] private float projectileSpeed = 10f; // Velocidad del proyectil
    [SerializeField] private float fireRate = 2f; // Frecuencia de disparo (Disparos por segundo)

    private Transform playerTransform; // Referencia al jugador
    private float fireCooldown; // Controla el tiempo entre disparos

    private void Start()
    {
        // Encontrar al jugador por etiqueta, puedes ajustarlo a tu caso
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        fireCooldown = 0f;
    }

    private void Update()
    {
        Vector3 lookPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        transform.LookAt(lookPosition);

        // Actualizar el cooldown para disparar
        fireCooldown -= Time.deltaTime;

        // Disparar si el cooldown ha terminado
        if (fireCooldown <= 0f)
        {
            ShootAtPlayer();
            fireCooldown = 1f / fireRate; // Reiniciar el cooldown basado en la tasa de disparo
        }
    }

    private void ShootAtPlayer()
    {
        // Verificar que el jugador esté presente
        if (playerTransform != null)
        {
            // Crear el proyectil en el punto de disparo (shootingPoint)
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

            // Obtener la dirección hacia el jugador en el momento del disparo
            Vector3 directionToPlayer = playerTransform.position - shootingPoint.position;

            // Asegurar que la componente Y (vertical) sea 0 para evitar disparar hacia arriba o abajo
            directionToPlayer.y = shootingPoint.position.y;

            // Normalizar la dirección para que tenga longitud 1
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

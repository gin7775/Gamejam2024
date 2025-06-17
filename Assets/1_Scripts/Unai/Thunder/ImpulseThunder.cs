using System.Collections;
using UnityEngine;

public class ImpulseThunder : MonoBehaviour
{
    [Header("Configuración de impactos")]
    [Tooltip("Número máximo de enemigos a golpear")]
    [SerializeField] private int maxHits = 7;
    [Tooltip("Distancia máxima para detectar un enemigo; si supera esto, el proyectil se destruye tras agotar impactos o sin objetivos")]
    [SerializeField] private float maxRange = 20f;
    [Tooltip("Tag usado para identificar enemigos")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Parámetros de movimiento")]
    [Tooltip("Velocidad de desplazamiento del proyectil")]
    [SerializeField] private float speed = 20f;
    [Tooltip("Distancia mínima para considerar que ha alcanzado al objetivo")]
    [SerializeField] private float arriveThreshold = 0.1f;
    [Tooltip("Altura relativa al suelo a la que vuela el proyectil")]
    [SerializeField] private float heightOffset = 1f;

    private int remainingHits;
    private Vector3 spawnPosition;

    private void Start()
    {
        remainingHits = maxHits;
        // Ajustar altura inicial
        spawnPosition = transform.position + Vector3.up * heightOffset;
        transform.position = spawnPosition;

        // Solo iniciar persecución si hay al menos un enemigo en rango
        if (FindNearestEnemy() != null)
        {
            StartCoroutine(ChaseRoutine());
        }
        // Si no hay enemigos, no hace nada (el proyectil permanece inactivo)
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f); // Proyectil

        Transform target = FindNearestEnemy();
        if (target != null)
        {
            Vector3 desired = target.position + Vector3.up * heightOffset;
            Vector3 dir = (desired - transform.position).normalized;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, desired); // Línea al objetivo

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + dir * 2f); // Dirección actual

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(desired, 0.1f); // Punto objetivo
        }
    }


    /// <summary>
    /// Busca y devuelve el Transform del enemigo más cercano dentro de maxRange, o null si ninguno
    /// </summary>
    private Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closest = null;
        float bestDist = maxRange;
        Vector3 currentPos = transform.position;

        foreach (var go in enemies)
        {
            float d = Vector3.Distance(currentPos, go.transform.position);
            if (d <= bestDist)
            {
                bestDist = d;
                closest = go.transform;
            }
        }
        return closest;
    }

    /// <summary>
    /// Rutina que busca un objetivo, se mueve hacia él, aplica efecto, decrementa contador, y repite
    /// </summary>
    private IEnumerator ChaseRoutine()
    {
        while (remainingHits > 0)
        {
            Transform target = FindNearestEnemy();
            if (target == null)
                break;

            Vector3 desired;
            // Mover en línea recta hasta llegar al objetivo
            while (target != null && Vector3.Distance(transform.position, (desired = target.position + Vector3.up * heightOffset)) > arriveThreshold)
            {
                Vector3 dir = (desired - transform.position).normalized;
                // Rotar para que el forward apunte al movimiento, manteniendo tumbado en X
                transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f);
                transform.position += dir * speed * Time.deltaTime;
                yield return null;
            }

            remainingHits--;
        }

        Destroy(gameObject);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseThunder : MonoBehaviour
{
    [Header("Configuración de objetivo múltiple")]
    [Tooltip("Número máximo de enemigos a golpear")]
    [SerializeField] private int maxTargets = 7;
    [Tooltip("Distancia máxima para perseguir al siguiente objetivo; si es mayor, el proyectil desaparecerá")]
    [SerializeField] private float maxRange = 20f;
    [Tooltip("Tag usado para identificar enemigos")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Parámetros de movimiento")]
    [Tooltip("Velocidad de desplazamiento del proyectil")]
    [SerializeField] private float speed = 20f;
    [Tooltip("Distancia mínima para considerar que ha alcanzado al objetivo")]
    [SerializeField] private float arriveThreshold = 0.1f;
    [Tooltip("Altura inicial sobre el suelo")]
    [SerializeField] private float heightOffset = 1f;

    private List<Transform> targets = new List<Transform>();
    private Vector3 spawnPosition;

    private void Start()
    {
        // Ajustar altura inicial y posición
        spawnPosition = transform.position + Vector3.up * heightOffset;
        transform.position = spawnPosition;

        // Detectar objetivos más cercanos
        GatherClosestTargets();

        // Si no hay enemigos, destruir
        if (targets.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        // Iniciar persecución
        StartCoroutine(ChaseTargets());
    }

    private void GatherClosestTargets()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Vector3 origin = spawnPosition;
        var sorted = new List<GameObject>(enemies);
        sorted.Sort((a, b) =>
        {
            float da = Vector3.SqrMagnitude(a.transform.position - origin);
            float db = Vector3.SqrMagnitude(b.transform.position - origin);
            return da.CompareTo(db);
        });

        int count = Mathf.Min(maxTargets, sorted.Count);
        for (int i = 0; i < count; i++)
            targets.Add(sorted[i].transform);
    }

    private IEnumerator ChaseTargets()
    {
        foreach (var target in targets)
        {
            if (target == null)
                continue;

            float distToSpawn = Vector3.Distance(spawnPosition, target.position);
            if (distToSpawn > maxRange)
            {
                Destroy(gameObject);
                yield break;
            }

            Vector3 desired;
            while (target != null && Vector3.Distance(transform.position, (desired = target.position + Vector3.up * heightOffset)) > arriveThreshold)
            {
                Vector3 dir = (desired - transform.position).normalized;
                // Rotar hacia la dirección de movimiento
                transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f); // Mantener tumbado 90° en X
                // Mover hacia el objetivo
                transform.position += dir * speed * Time.deltaTime;
                yield return null;
            }

            // Aquí puedes añadir lógica de impacto si lo deseas
        }

        Destroy(gameObject);
    }
}

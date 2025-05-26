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
        // Ajustar altura inicial
        spawnPosition = transform.position + Vector3.up * heightOffset;
        transform.position = spawnPosition;

        // Detectar objetivos
        GatherClosestTargets();

        // Si no hay enemigos cercanos, destruye el proyectil
        if (targets.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        // Comenzar persecución
        StartCoroutine(ChaseTargets());
    }

    private void GatherClosestTargets()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Vector3 origin = spawnPosition;
        var sorted = new List<GameObject>(enemies);
        sorted.Sort((a, b) =>
        {
            float da = Vector3.SqrMagnitude(a.transform.position - origin);
            float db = Vector3.SqrMagnitude(b.transform.position - origin);
            return da.CompareTo(db);
        });
        for (int i = 0; i < Mathf.Min(maxTargets, sorted.Count); i++)
        {
            targets.Add(sorted[i].transform);
        }
    }

    private IEnumerator ChaseTargets()
    {
        foreach (var target in targets)
        {
            if (target == null)
                continue;

            // Comprobar rango desde posición de spawn
            float dist = Vector3.Distance(spawnPosition, target.position);
            if (dist > maxRange)
            {
                Destroy(gameObject);
                yield break;
            }

            // Mover al objetivo manteniendo altura
            Vector3 desired;
            while (target != null && Vector3.Distance(transform.position, (desired = target.position + Vector3.up * heightOffset)) > arriveThreshold)
            {
                Vector3 dir = (desired - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(dir);

                yield return null;
            }

            //// Impactar al objetivo
            //if (target != null)
            //{
            //    var health = target.GetComponent<ContenedorEnemigo1>();
            //    if (health != null)
            //    {
            //        health.lifes = 0;
            //        health.PolloMansy();
            //    }
            //}
        }

        Destroy(gameObject);
    }

}

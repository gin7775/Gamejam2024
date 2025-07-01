using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyDetector : MonoBehaviour
{
    [Header("Modo AutoShoot")]
    [Tooltip("Marca esta casilla para activar el autoshoot")]
    public bool autoshoot = false;

    [Header("Detección")]
    [Tooltip("Radio en el que detectar enemigos")]
    [SerializeField] private float detectionRadius = 15f;

    [Header("Disparo")]
    [Tooltip("Intervalo en segundos entre cada ráfaga de disparos")]
    [SerializeField] private float shootInterval = 1f;

    private Collider[] detectionResults = new Collider[50];
    private float shootTimer;
    private ChickenLouncher chickenLauncher;


    private void Awake()
    {
        chickenLauncher = GetComponent<ChickenLouncher>();
        if (chickenLauncher == null)
            Debug.LogError("ChickenLauncher no encontrado en el GameObject.");
    }

    private void Update()
    {
        if(autoshoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                DetectAndAutoShootNearest();
                //DetectAndAutoShoot();
                shootTimer = 0f;
            }
        }
        
    }

    //private void DetectAndAutoShoot()
    //{
    //    if (chickenLauncher == null) return;

    //    int hits = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, detectionResults);
    //    for (int i = 0; i < hits; i++)
    //    {
    //        var col = detectionResults[i];
    //        if (col.CompareTag("Enemy"))
    //        {
    //            Vector3 targetPos = col.transform.position;
    //            chickenLauncher.autoShoot(targetPos);
    //        }
    //    }
    //}

    private void DetectAndAutoShootNearest()
    {
        if (chickenLauncher == null)
            return;

        int hits = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, detectionResults);
        Transform closest = null;
        float minDistSqr = float.MaxValue;

        for (int i = 0; i < hits; i++)
        {
            Collider col = detectionResults[i];
            if (!col.CompareTag("Enemy"))
                continue;

            float distSqr = (col.transform.position - transform.position).sqrMagnitude;
            if (distSqr < minDistSqr)
            {
                minDistSqr = distSqr;
                closest = col.transform;
            }
        }

        if (closest != null)
        {
            chickenLauncher.autoShoot(closest.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el radio de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Dibuja un raycast en dirección forward hasta el mismo rango
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * detectionRadius);
    }
}


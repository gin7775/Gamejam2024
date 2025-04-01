using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPollos : MonoBehaviour
{
    [Header("Explosión")]
    public float explosionForce = 25f;
    public float upwardModifier = 1.5f;
    public float explosionRadius = 10f;

    [Header("Visual")]
    public SpawnParticles spawnParticles;

    public void ExplodeRagdolls()
    {
        if (spawnParticles != null)
            spawnParticles.SpawnBothParticles();

        StartCoroutine(ApplyExplosionToRagdolls());
    }

    IEnumerator ApplyExplosionToRagdolls()
    {
        yield return new WaitForSeconds(0.1f); // Pequeño delay inicial opcional

        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");

        foreach (GameObject corpse in corpses)
        {
            if (corpse == null) continue;

            float dist = Vector3.Distance(transform.position, corpse.transform.position);
            if (dist <= explosionRadius * 2)
            {
                Collider[] colliders = corpse.GetComponentsInChildren<Collider>();
                Rigidbody[] rigidbodies = corpse.GetComponentsInChildren<Rigidbody>();

                // Desactivar colisiones temporalmente
                foreach (Collider col in colliders)
                    col.enabled = false;

                // 💥 APLICAR FUERZA SIN DELAY
                foreach (Rigidbody rb in rigidbodies)
                {
                    if (rb != null)
                    {
                        rb.AddExplosionForce(
                            explosionForce,
                            transform.position,
                            explosionRadius,
                            upwardModifier,
                            ForceMode.Impulse
                        );
                    }
                }

                // Reactivar colliders inmediatamente
                foreach (Collider col in colliders)
                    col.enabled = true;
            }
        }
    }
}

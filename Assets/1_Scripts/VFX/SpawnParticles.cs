using UnityEngine;

public class SpawnParticles : MonoBehaviour
{
    [Header("Prefabs originales desde Assets")]
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private GameObject secondaryParticlePrefab;

    public void SpawnParticle()
    {
        if (particlePrefab == null)
        {
            Debug.LogError("❌ No hay prefab asignado en 'particlePrefab'");
            return;
        }

        Debug.Log("✅ Instanciando particlePrefab: " + particlePrefab.name);
        GameObject obj = Instantiate(particlePrefab, transform.position, Quaternion.identity);
        obj.name = particlePrefab.name; // Evita clone clone

        obj.SetActive(true);
        Debug.Log("✅ Instancia activada: " + obj.name + " | ActiveInHierarchy: " + obj.activeInHierarchy);

        ParticleSystem ps = obj.GetComponent<ParticleSystem>() ?? obj.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Debug.Log("✅ ParticleSystem encontrado y ejecutado.");
        }
        else
        {
            Debug.LogError("❌ No se encontró ParticleSystem en el prefab o sus hijos.");
        }
    }

    public void SpawnSecondaryParticle()
    {
        if (secondaryParticlePrefab != null)
        {
            Debug.Log("✅ Instanciando secondaryParticlePrefab: " + secondaryParticlePrefab.name);
            GameObject obj = Instantiate(secondaryParticlePrefab, transform.position, Quaternion.identity);
            obj.name = secondaryParticlePrefab.name;
            obj.SetActive(true);

            ParticleSystem ps = obj.GetComponent<ParticleSystem>() ?? obj.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Debug.Log("✅ Secondary ParticleSystem ejecutado.");
            }
            else
            {
                Debug.LogError("❌ No se encontró ParticleSystem en el secundario.");
            }
        }
    }

    public void SpawnBothParticles()
    {
        SpawnParticle();
        SpawnSecondaryParticle();
    }
}

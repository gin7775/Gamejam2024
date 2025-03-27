using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChickenSpawnData
{
    public GameObject prefab;
    [Range(0f, 100f)]
    public float probability = 25f;
}

public class RagdollSpawner : MonoBehaviour
{
    [Header("Pollos con probabilidad")]
    public List<ChickenSpawnData> chickenDataList;

    [Header("Parámetros de spawneo")]
    public float spawnInterval = 2f;                      // Tiempo entre oleadas (después del primer burst)
    public float initialBurstDelay = 0.5f;                // Tiempo entre cada pollo al principio
    public int initialBurstAmount = 20;                   // Cantidad de pollos al inicio
    public float chickenVelocity = 10f;                   // Velocidad de caída
    public float chickenLifetime = 10f;                   // Tiempo antes de destruir al pollo

    [Header("Zona de spawneo")]
    public Vector3 spawnAreaCenter = new Vector3(0, 10, 0);
    public Vector3 spawnAreaSize = new Vector3(10, 1, 10);

    [Header("Cantidad aleatoria por oleada")]
    public int[] waveSizeOptions = { 3, 5, 7, 9, 11 };

    [Header("Activar auto-spawn")]
    public bool autoSpawn = true;

    private void Start()
    {
        if (autoSpawn)
        {
            StartCoroutine(InitialBurst());
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator InitialBurst()
    {
        for (int i = 0; i < initialBurstAmount; i++)
        {
            SpawnChicken();
            yield return new WaitForSeconds(initialBurstDelay);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int waveCount = waveSizeOptions[Random.Range(0, waveSizeOptions.Length)];
            for (int i = 0; i < waveCount; i++)
            {
                SpawnChicken();
            }
        }
    }

    private void SpawnChicken()
    {
        GameObject prefab = GetRandomChickenPrefab();
        if (prefab == null) return;

        Vector3 randomPos = spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );

        Quaternion randomRot = Random.rotation;
        GameObject newChicken = Instantiate(prefab, randomPos, randomRot);

        Rigidbody rb = newChicken.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.down * chickenVelocity;
        }

        StartCoroutine(DestroyChickenAfterTime(newChicken, chickenLifetime));
    }

    private GameObject GetRandomChickenPrefab()
    {
        float total = 0f;
        foreach (var data in chickenDataList)
        {
            total += data.probability;
        }

        if (total <= 0f) return null;

        float random = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var data in chickenDataList)
        {
            cumulative += data.probability;
            if (random <= cumulative)
            {
                return data.prefab;
            }
        }

        return null;
    }

    private IEnumerator DestroyChickenAfterTime(GameObject chicken, float time)
    {
        yield return new WaitForSeconds(time);
        if (chicken != null)
        {
            Destroy(chicken);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}

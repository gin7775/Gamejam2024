using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{

    [Header("Wave Control")]
    public int level = 1;                                                               // Nivel/Mapa
    public int waveCurrent = 0;                                                         // Número de la ronda actual
    public int enemyNumber = 0;                                                         // Número total de enemigos en la ronda
    public int enemyCount = 0;                                                          // Número de enemigos que quedan
    public int totalWaveChicken = 0;                                                    // Total de pollos en la oleada
    public int capGenerator = 200;                                                      // Cap del Generador

    [Header("Enemy Spawning")]
    //[SerializeField] private float timeWave = 15f;                                      // Tiempo por oleada
    [SerializeField] private List<ChickenConfig> chickensToSpawn;                       // Lista de enemigos con sus probabilidades
    [SerializeField] private List<ChickenConfigWave> chickensToSpawnWave;               // Enemigos por oleada y nivel
    [SerializeField] private Vector3 spawnPosition;                                     // Posición de spawn
    public List<GameObject> listEnemies;                                                // Lista de enemigos instanciados
    [SerializeField] private List<ChickenCount> instantiatedChickenCount;               // Contador de enemigos creados
    [SerializeField] private List<ChickenCount> killedChickenCount;                     // Contador de enemigos muertos

    [Header("Spawn Area")]
    [SerializeField] private float limiteXNegativo, limiteXPositivo, limiteZNegativo, limiteZPositivo;
    public Transform spawnParent;
    [SerializeField] private List<GameObject> listSpawns;                               // Posiciones de spawn


    [Header("VFX")]
    [SerializeField] private GameObject vfxHitEffect;                                   // Efecto al recibir daño
    [SerializeField] private GameObject vfxHitWaveEffect;                               // Efecto de impacto de oleada

    private void Start()
    {
        PopulateSpawnList(spawnParent); // Rellena la lista con los hijos

    }

    /// <summary>
    /// Rellena el array listSpawns con los hijos de un objeto padre especificado.
    /// </summary>
    /// <param name="parentObject">El objeto padre cuyos hijos serán añadidos a listSpawns.</param>
    public void PopulateSpawnList(Transform parentObject)
    {
        listSpawns.Clear(); // Limpiamos la lista antes de rellenarla

        // Recorremos todos los hijos del objeto padre
        foreach (Transform child in parentObject)
        {
            listSpawns.Add(child.gameObject); // Añadimos el GameObject del hijo a la lista
        }
    }

    /// <summary>
    /// Actualiza la oleada actual y escala la dificultad en función de la ola.
    /// </summary>
    public void StartWave()
    {
        waveCurrent++;
        capGenerator = new ChickenSpawnService().GetCapGenerator(level, waveCurrent);
        StartCoroutine(GenerateEnemiesAsync());
    }

    public void ChickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        if (enemy != null)
        {
            Instantiate(vfxHitWaveEffect, enemy.transform.position + Vector3.up, Quaternion.identity);
            ContenedorEnemigo1 auxEnemy = enemy.GetComponent<ContenedorEnemigo1>();

            if (auxEnemy != null && auxEnemy.lifes >= 1)
            {
                auxEnemy.lifes -= damage;

                if (auxEnemy.lifes <= 0)
                {
                    Instantiate(vfxHitEffect, enemy.transform.position + Vector3.up, Quaternion.identity);
                    EnemyDeath(enemy);
                    Destroy(enemy);
                }
            }
        }
    }

    public void EnemyDeath(GameObject enemy)
    {
        ChickenSpawnService spawnService = new(chickensToSpawn, chickensToSpawnWave);
        ChickenConfig chickenHeal = spawnService.SelectChicken("Heal");

        KillCountChicken(enemy.name);
        enemyCount--;

        // Spawn heal chicken
        if (enemyCount <= 0)
            StartWave();
    }

    private IEnumerator GenerateEnemiesAsync()
    {
        int remainingCap = capGenerator;
        ChickenSpawnService spawnService = new(chickensToSpawn, chickensToSpawnWave);

        for (int i = 0; i < remainingCap; i++)
        {
            float randomChance = Random.Range(0, 100);
            if (randomChance >= 50)
                i += SpawnChickenSquad(spawnService.SelectChickenBasedOnProb(level, waveCurrent)); // Generar escuadrón
            else
                SpawnSingleChicken(spawnService);
        }
        yield break;
    }

    private void SpawnSingleChicken(ChickenSpawnService spawnService)
    {
        ChickenConfig selectedChicken = spawnService.SelectChickenBasedOnProb(level, waveCurrent);
        Instantiate(selectedChicken.chickenPrefab, GetRandomAreaSpawn(), Quaternion.identity);
        totalWaveChicken += selectedChicken.difficultyScore;
        enemyCount++;
        CountChiken(selectedChicken.chickenPrefab.name);
    }

    private int SpawnChickenSquad(ChickenConfig chicken)
    {
        int squadSize = 5; // Tamaño del escuadrón
        for (int i = 0; i < squadSize; i++)
        {
            Instantiate(chicken.chickenPrefab, GetRandomAreaSpawn(), Quaternion.identity);
            enemyCount++;
            CountChiken(chicken.chickenPrefab.name);
        }
        return squadSize;
    }

    private void CountChiken(string name)
    {
        AddOrUpdateChickenCount(instantiatedChickenCount, GetEnemyTypeFromName(name));
    }

    private void KillCountChicken(string name)
    {
        AddOrUpdateChickenCount(killedChickenCount, GetEnemyTypeFromName(name));
    }

    public void AddOrUpdateChickenCount(List<ChickenCount> array, string enemyType)
    {
        ChickenCount existingChicken = array.FirstOrDefault(c => c.typeName == enemyType);
        if (existingChicken != null)
            existingChicken.quantity++;
        else
            array.Add(new ChickenCount(enemyType));
    }

    private string GetEnemyTypeFromName(string name)
    {
        int startIndex = name.IndexOf("Pfb_Enemy") + "Pfb_Enemy".Length;
        int endIndex = name.IndexOf("(Clone)") == -1 ? name.Length : name.IndexOf("(Clone)");
        return name[startIndex..endIndex];
    }

    private Vector3 GetRandomAreaSpawn()
    {
        return new Vector3(Random.Range(limiteXNegativo, limiteXPositivo), 1.2f, Random.Range(limiteZNegativo, limiteZPositivo));
    }

}

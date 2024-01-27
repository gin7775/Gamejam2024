using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPollos_rik : MonoBehaviour
{
    public static SpawnPollos_rik Instance { get; private set; }

    [SerializeField] private GameObject[] pollosToSpawn;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private int randomIterastor;
    // Tiempo por el que espera para generar enemigos por segundo
    [SerializeField] private float timeWave;
    // Tiempo por el que espera para generar enemigos por segundo
    [SerializeField] private float timeGeneration;
    // Numero total de enemigos en la ronda
    [SerializeField] private int enemyNumber;
    // Numero de enemigos por oleada inicial
    [SerializeField] private int enemyInitial;
    // Numero de enemigos que quedan en la ronda
    [SerializeField] private int enemyCount;
    // Numero de ronda totales
    [SerializeField] private int waveCurrent;
    // Numero de ronda totales
    [SerializeField] private int waveNumber = 3;
    // Numero de ronda actual
    [SerializeField] private int currentWave;
    // Nivel de dificultad -- No se usa
    [SerializeField] private int dificultiLevel;
    // Objeto para la espera
    [SerializeField] private Coroutine myCorutine;

    private void Awake()
    {
        // If there is an instance and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timeWave = 10f;
        enemyInitial = 5;
        enemyNumber = 0;
        enemyCount = 0;
        GameManager.Instance.waveNumber = waveNumber;
        UpdateWave();
    }

    public void InstantiatePollos(int enemys)
    {
        StartCoroutine(startWave(timeWave, enemys));
    }

    public void UpdateWave()
    {
        Debug.Log("UpdateWave");

        GameManager.Instance.currentWave++;
        waveCurrent = GameManager.Instance.currentWave;
        Debug.Log("Wave: " + GameManager.Instance.currentWave);
        Debug.Log("WaveTotal: " + GameManager.Instance.waveNumber);

        if (waveCurrent <= GameManager.Instance.waveNumber)
        {
            if (waveCurrent <= 1)
            {
                GameManager.Instance.dificultiLevel = 1;
                InstantiatePollos(enemyInitial);
                //generateWithTime();
            }
            else if (waveCurrent == 2)
            {
                GameManager.Instance.dificultiLevel = 2;
                InstantiatePollos(enemyInitial * 2);
                //generateWithTime();
            }
            else
            {
                GameManager.Instance.dificultiLevel = 3;
                InstantiatePollos(enemyInitial * 3);
                //generateWithTime();
            }
        }
    }

    public void enemyDeath()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            UpdateWave();
        }
    }

    public void generateWithTime()
    {
        // Inicia la corutina después de 3 segundos
        myCorutine = StartCoroutine(EsperarYExecutar());
    }

    IEnumerator EsperarYExecutar()
    {
        int auxEnemyNumber = GameManager.Instance.currentWave <= 1 ? 25 : 0;
        Debug.Log("currentwave1: " + GameManager.Instance.currentWave);
        Debug.Log("currentwave1: " + currentWave);
        Debug.Log("num aux1: " + auxEnemyNumber);
        auxEnemyNumber = GameManager.Instance.currentWave == 2 ? 50 : auxEnemyNumber;
        Debug.Log("num aux2: " + auxEnemyNumber);
        auxEnemyNumber = GameManager.Instance.currentWave >= 3 ? 105 : auxEnemyNumber;
        Debug.Log("num aux3: " + auxEnemyNumber);

        for (int i = 0; i < auxEnemyNumber; i++)
        {
            // Espera 3 segundos
            yield return new WaitForSeconds(timeGeneration);

            // Tu lógica aquí
            Debug.Log("Se ejecutó después de 3 segundos");
            randomIterastor = Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(Random.Range(8.5f, -8.5f), 1.2f, Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
            enemyCount++;
        }
    }

    IEnumerator startWave(float seconds, int totalEnemies)
    {
        Debug.Log("Esperamos 10 segundos");
        // Espera 10 segundos
        yield return new WaitForSeconds(seconds);
        Debug.Log("Han pasado");

        for (int i = 0; i < totalEnemies; i++)
        {
            randomIterastor = Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(Random.Range(8.5f, -8.5f), 1.2f, Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
            enemyCount++;
        }

        generateWithTime();
    }

    public void CancelarCorutina()
    {
        // Cancela la corutina si es necesario
        if (myCorutine != null)
        {
            StopCoroutine(myCorutine);
        }
    }

}

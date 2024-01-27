using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPollos_rik : MonoBehaviour
{
    public static SpawnPollos_rik Instance { get; private set; }

    [SerializeField] private GameObject [] pollosToSpawn;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float timeGeneration;
    [SerializeField] private int randomIterastor;
    [SerializeField] private int enemyNumber, enemyCount;
    [SerializeField] private int waveNumber, currentWave, dificultiLevel;
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
        randomIterastor = Random.Range(0, pollosToSpawn.Length);
        UpdateWave();
    }

    public void InstantiatePollos(int enemys)
    {
        enemyCount = enemys;

        for (int i = 0; i < enemys; i++)
        {
            randomIterastor = Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(Random.Range(8.5f,-8.5f ), 1.2f, Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
        }

        generateWithTime();
    }

    public void UpdateWave()
    {
        if (GameManager.Instance.currentWave <= GameManager.Instance.waveNumber)
        {
            if (GameManager.Instance.currentWave <= 1)
            {
                GameManager.Instance.dificultiLevel = 1;
                //InstantiatePollos(enemyNumber);
                generateWithTime();
            }
            else if (GameManager.Instance.currentWave == 2)
            {
                GameManager.Instance.dificultiLevel = 2;
                //InstantiatePollos(enemyNumber * 2);
                generateWithTime();
            }
            else
            {
                GameManager.Instance.dificultiLevel = 3;
                //InstantiatePollos(enemyNumber * 4);
                generateWithTime();
            }
            GameManager.Instance.currentWave++;
        }
    }

    public void enemyDeath()
    {
        enemyCount--;

        if(enemyCount <= 0)
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
        enemyCount = 0;
        enemyNumber = GameManager.Instance.currentWave <= 1 ? 20 : 0;
        enemyNumber = GameManager.Instance.currentWave == 2 ? 50 : 20;
        enemyNumber = GameManager.Instance.currentWave >= 3 ? 120 : 60;

        for (int i = 0; i < enemyNumber; i++)
        {
            // Espera 3 segundos
            yield return new WaitForSeconds(timeGeneration);

            // Tu lógica aquí
            Debug.Log("Se ejecutó después de 3 segundos");
            randomIterastor = Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(Random.Range(8.5f, -8.5f), 1.2f, Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[1], spawnPosition, Quaternion.identity);
            enemyCount++;
        }
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

using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public int score = 0;
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
    [SerializeField] private int waveNumber;
    // Numero de ronda actual
    [SerializeField] private int currentWave;
    // Nivel de dificultad -- No se usa
    [SerializeField] private int dificultiLevel;
    // Nivel de dificultad -- No se usa
    [SerializeField] private Canvas canvasRound;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private GameObject textMesh;
    [SerializeField] private GameObject vfxHitEffect;
    private CinemachineImpulseSource cinemachineImpulseSource;
    public GameObject scoreText;


    [SerializeField] private MusicManager musicManager;


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
      
        timeGeneration = 3f;
        timeWave = 6f;
        enemyInitial = 5;
        enemyNumber = 0;
        enemyCount = 0;
        waveNumber = 3;
        UpdateWave();
    }

    /*public void chikenEnemyDeath(GameObject enemy)
    {
        score++;
        enemyDeath();
    }*/

    public void chickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        int auxLife = enemy.GetComponent<ContenedorEnemigo1>().lifes -= damage;

        if (auxLife <= 0)
        {
            cinemachineImpulseSource = enemy.gameObject.GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();
            Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0,1,0), Quaternion.identity);
            StartCoroutine(FrameFreeze(0.03f));
            score++;
            //scoreText.GetComponent<TextMeshProUGUI>().text = "score: " + score;
            Destroy(enemy);
            //enemyDeath();
        }
    }

    private IEnumerator FrameFreeze(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    public void InstantiatePollos(int enemys)
    {
        StartCoroutine(startWave(timeWave, enemys));
    }

    public void UpdateWave()
    {
        currentWave++;
        waveCurrent = currentWave;

        if (waveCurrent <= waveNumber)
        {
            textMesh.GetComponent<TextMeshProUGUI>().text = "Round " + waveCurrent;
            if (waveCurrent <= 1)
            {
                dificultiLevel = 1;
                timeGeneration = 3f;
              
                InstantiatePollos(enemyInitial);
                musicManager.ChangeRaidTheme(0, 0); //Audio 0
            }
            else if (waveCurrent == 2)
            {
            
                dificultiLevel = 2;
                timeGeneration = 1.5f;
                InstantiatePollos(enemyInitial * 2);
                musicManager.ChangeRaidTheme(currentWave - 1, currentWave); //Audio 1
            }
            else
            {
             
                dificultiLevel = 3;
                timeGeneration = 0.75f;
                InstantiatePollos(enemyInitial * 3);
                musicManager.ChangeRaidTheme(currentWave - 1, currentWave); // Audio 2
            }
        }
    }

    public void enemyDeath()
    {
        enemyCount--;

        //AUDIO: Ver si funciona en lso enemigos sino, se pone aquí

        if (enemyCount <= 0)
        {
            UpdateWave();
        }
    }

    IEnumerator EsperarYExecutar()
    {
        int auxEnemyNumber = currentWave <= 1 ? 25 : 0;
        auxEnemyNumber = currentWave == 2 ? 50 : auxEnemyNumber;
        auxEnemyNumber = currentWave >= 3 ? 105 : auxEnemyNumber;

        for (int i = 0; i < auxEnemyNumber; i++)
        {
            // Espera 3 segundos
            yield return new WaitForSeconds(timeGeneration);

            // Tu lógica aquí
            randomIterastor = UnityEngine.Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(UnityEngine.Random.Range(8.5f, -8.5f), 1.2f, UnityEngine.Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
            enemyCount++;
        }
    }

    IEnumerator startWave(float seconds, int totalEnemies)
    {
        // Iniciar el canvas
        canvasRound.gameObject.SetActive(true);
        // Espera 10 segundos
        yield return new WaitForSeconds(seconds);

        // lanzar el quitar el canvas
        if(canvasAnimator != null)
        {
            canvasAnimator.SetTrigger("Change");
        }

        yield return new WaitForSeconds(2f);
        canvasRound.gameObject.SetActive(false);

        for (int i = 0; i < totalEnemies; i++)
        {
            randomIterastor = UnityEngine.Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(UnityEngine.Random.Range(8.5f, -8.5f), 1.2f, UnityEngine.Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
            enemyCount++;
        }

        StartCoroutine(EsperarYExecutar());
    }

}

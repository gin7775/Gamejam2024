using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public int score = 0;
    [SerializeField] private GameObject[] pollosToSpawn;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private int randomIterastor, probabilityIterator;
    // Tiempo por el que espera para generar enemigos por segundo
    [SerializeField] private float timeWave;
    // Tiempo por el que espera para generar enemigos por segundo
    [SerializeField] private float timeGeneration;
    // Numero total de enemigos en la ronda
    [SerializeField] public int enemyNumber;
    // Numero de enemigos por oleada inicial
    [SerializeField] public int enemyInitial;
    // Numero de enemigos que quedan en la ronda
    [SerializeField] public int enemyCount;
    // Numero de ronda totales
    [SerializeField] public int waveNumber;
    // Numero de ronda actual
    [SerializeField] public int waveCurrent;
    // Nivel de dificultad -- No se usa
    [SerializeField] private int dificultiLevel;
    // Nivel de dificultad -- No se usa
    [SerializeField] private Canvas canvasRound;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private GameObject textMesh;
    [SerializeField] private GameObject vfxHitEffect;
    [SerializeField] private GameObject vfxHitWaveEffect;
    [SerializeField] private GameObject SmokeEffect;
    private CinemachineImpulseSource cinemachineImpulseSource;
    public GameObject scoreText;
    public float limiteXNegativo, LimiteXPositivo, limiteZNegativo, LimiteZPositivo;
    [SerializeField] private int numMaxWave1;
    [SerializeField] private int numMaxWave2;
    [SerializeField] private int numMaxWave3;

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

        numMaxWave1 = 30;
        numMaxWave2 = 60;
        numMaxWave3 = 120;
        UpdateWave();
    }

    public void InstantiatePollos(int enemys)
    {
        StartCoroutine(startWave(timeWave, enemys));
    }

    public void UpdateWave()
    {
        waveCurrent++;

        if (waveCurrent <= waveNumber)
        {
            textMesh.GetComponent<TextMeshProUGUI>().text = "Round " + waveCurrent;
            if (waveCurrent <= 1)
            {
                dificultiLevel = 1;
                timeGeneration = 3f;
                InstantiatePollos(enemyInitial);
            }
            else if (waveCurrent == 2)
            {
                dificultiLevel = 2;
                timeGeneration = 1.5f;
                InstantiatePollos(enemyInitial * 2);
            }
            else
            {
                dificultiLevel = 3;
                timeGeneration = 0.75f;
                InstantiatePollos(enemyInitial * 3);
            }
        }
        else
        {
            dificultiLevel = 4;
            timeGeneration = 0.67f;
            InstantiatePollos(6 * 3);
        }
    }

    public void enemyDeath()
    {
        enemyCount--;

        //AUDIO: Ver si funciona en lso enemigos sino, se pone aquí
        musicManager.Play_FX_ExplosionPollo();

        if (enemyCount <= 0 && score == 30 || enemyCount <= 0 && score >= 60)
        {
            UpdateWave();
        }
    }

    public void chickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        int auxLife = 0;
        // Spawn particula de hit
        Instantiate(vfxHitWaveEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        if (enemy != null)
        {
            if (enemy.GetComponent<ContenedorEnemigo1>() != null)
            {
                auxLife = enemy.GetComponent<ContenedorEnemigo1>().lifes -= damage;
            }
        }

        if (auxLife <= 0)
        {
            cinemachineImpulseSource = enemy.gameObject.GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();
            Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            StartCoroutine(FrameFreeze(0.03f));
            score++;
            enemy.GetComponent<ContenedorEnemigo1>().PolloMansy();
            Destroy(enemy);
            enemyDeath();
        }
    }

    public void chikenGenerator() {
        probabilityIterator = Random.Range(0, 100);

        if (probabilityIterator >= 0 && probabilityIterator <= 60)
        {
            randomIterastor = 0;
        }
        else if (probabilityIterator > 60 && probabilityIterator <= 75)
        {
            randomIterastor = 1;
        }
        else if (probabilityIterator > 75 && probabilityIterator <= 90)
        {
            randomIterastor = 2;
        }
        else if (probabilityIterator > 90 && probabilityIterator <= 100)
        {
            randomIterastor = 3;
        }

        spawnPosition = new Vector3(Random.Range(limiteXNegativo, LimiteXPositivo), 1.2f, Random.Range(limiteZNegativo, LimiteZPositivo));
        Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
        enemyCount++;
    }

    private IEnumerator FrameFreeze(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    IEnumerator chikenWaitSpawner()
    {
        Debug.Log("111-" + enemyNumber);
        enemyNumber = waveCurrent <= 1 ? numMaxWave1 - enemyInitial * 1 : 0;
        Debug.Log("222-" + enemyNumber);
        enemyNumber = waveCurrent == 2 ? numMaxWave2 - enemyInitial * 2 : enemyNumber;
        Debug.Log("333-" + enemyNumber);
        enemyNumber = waveCurrent == 3 ? numMaxWave3 - enemyInitial * 3 : enemyNumber;
        Debug.Log("444-" + enemyNumber);
        enemyNumber = waveCurrent >= 4 ? numMaxWave3 - enemyInitial * 4 : enemyNumber;
        Debug.Log("555-" + enemyNumber);

        for (int i = 0; i < enemyNumber; i++)
        {
            yield return new WaitForSeconds(timeGeneration);
            chikenGenerator();
        }
    }

    IEnumerator startWave(float seconds, int totalEnemies)
    {
        canvasRound.gameObject.SetActive(true);
       
        //Audio
        musicManager.FX_ActivarCorutina(waveCurrent - 1, waveCurrent);

        yield return new WaitForSeconds(seconds - 2f);

        if (canvasAnimator != null)
        {
            canvasAnimator.SetTrigger("Change");
        }

        yield return new WaitForSeconds(2f);
        canvasRound.gameObject.SetActive(false);

        for (int i = 0; i < totalEnemies; i++)
        {
            chikenGenerator();
        }

        StartCoroutine(chikenWaitSpawner());
    }

    public void ActivarFXMuerte()
    {

    }

}

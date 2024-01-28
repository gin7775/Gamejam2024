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
    [SerializeField] private int waveNumber;
    // Numero de ronda actual
    [SerializeField] private int waveCurrent;
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
            Debug.Log("Enemy " + enemy);
            Debug.Log("Enemy1 " + enemy.GetComponent<ContenedorEnemigo1>());

            cinemachineImpulseSource = enemy.gameObject.GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();
            Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            StartCoroutine(FrameFreeze(0.03f));
            score++;
            //scoreText.GetComponent<TextMeshProUGUI>().text = "score: " + score;
            enemy.GetComponent<ContenedorEnemigo1>().PolloMansy();
            Destroy(enemy);
            enemyDeath();
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
    }

    public void enemyDeath()
    {
        enemyCount--;

        //AUDIO: Ver si funciona en lso enemigos sino, se pone aquí
        musicManager.Play_FX_ExplosionPollo();

        if (enemyCount <= 0)
        {
            UpdateWave();
        }
    }

    IEnumerator EsperarYExecutar()
    {
        int auxEnemyNumber = waveCurrent <= 1 ? 25 : 0;
        auxEnemyNumber = waveCurrent == 2 ? 50 : auxEnemyNumber;
        auxEnemyNumber = waveCurrent >= 3 ? 105 : auxEnemyNumber;

        for (int i = 0; i < auxEnemyNumber; i++)
        {
            yield return new WaitForSeconds(timeGeneration);

            randomIterastor = UnityEngine.Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(UnityEngine.Random.Range(8.5f, -8.5f), 1.2f, UnityEngine.Random.Range(8.5f, -8.5f));
            Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
            enemyCount++;
        }
    }

    IEnumerator startWave(float seconds, int totalEnemies)
    {
        canvasRound.gameObject.SetActive(true);
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
            randomIterastor = UnityEngine.Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(UnityEngine.Random.Range(8.5f, -8.5f), 1.2f, UnityEngine.Random.Range(8.5f, -8.5f));
            Instantiate(SmokeEffect, spawnPosition, Quaternion.identity);
            Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
            enemyCount++;
        }

        StartCoroutine(EsperarYExecutar());
    }

}

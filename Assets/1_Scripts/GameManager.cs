using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // ---- Control de oleadas ----
    [SerializeField] public int level = 1;                                              // Nivel/Mapa
    [SerializeField] private float timeWave = 15;                                       // Tiempo por oleada
    [SerializeField] private float timeGeneration = 15;                                 // Tiempo entre generaci�n de enemigos
    [SerializeField] public int enemyNumber = 0;                                        // N�mero total de enemigos en la ronda
    [SerializeField] public int enemyInitial = 5;                                       // N�mero de enemigos por oleada inicial
    [SerializeField] public int enemyCount = 5;                                         // N�mero de enemigos que quedan en la ronda
    [SerializeField] public int waveNumber = 0;                                         // N�mero de rondas totales
    [SerializeField] public int waveCurrent = 0;                                        // N�mero de la ronda actual
    [SerializeField] public int dificultiLevel = 0;                                     // Nivel de dificultad
    //[SerializeField] private bool siguiente;                                          // Control para saber si contin�a a la siguiente oleada
    //[SerializeField] private int numMaxWave1;                                         // M�x. enemigos en la ola 1
    //[SerializeField] private int numMaxWave2;                                         // M�x. enemigos en la ola 2
    //[SerializeField] private int numMaxWave3;                                         // M�x. enemigos en la ola 3

    [SerializeField] private Canvas canvasRound;                                        // UI de la ronda
    [SerializeField] private Animator canvasAnimator;                                   // Animador para transiciones
    [SerializeField] private GameObject textMesh;                                       // Texto de la ronda
    private CinemachineImpulseSource cinemachineImpulseSource;                          // Fuente del efecto de impulso

    // ---- Control de enemigos ----
    [SerializeField] private List<GameObject> listSpawns;                               // Lista de spawns
    [SerializeField] private List<ChickenConfig> chikenToSpawn;                         // Lista de enemigos, probabilidades y puntuacion
    [SerializeField] private List<ChickenConfigWave> chikenToSpawnWave;                 // Lista de enemigos, probabilidades y puntuacion seg�n oleada y mapa/nivel
    [SerializeField] private List<ChickenDifficult> difficult;                          // Lista de enemigos, probabilidades y puntuacion seg�n oleada y mapa/nivel
    [SerializeField] public int totalChicken = 0;                                       // Total de pollos
    [SerializeField] private Vector3 spawnPosition;                                     // Posici�n de generaci�n
    [SerializeField] private GameObject vfxHitEffect;                                   // Efecto al recibir golpe
    [SerializeField] private GameObject vfxHitWaveEffect;                               // Efecto al recibir golpe en oleada
    [SerializeField] private GameObject SmokeEffect;                                    // Efecto de humo al generar enemigo
    public float limiteXNegativo, LimiteXPositivo, limiteZNegativo, LimiteZPositivo;    // L�mites de generaci�n

    // ---- Control del juego ----
    [SerializeField] public bool paused = false;                                        // Estado de pausa
    [SerializeField] public int score = 0;                                              // Puntuaci�n
    public GameObject scoreText;                                                        // Texto de la puntuaci�n
    public GameObject pausemenu;                                                        // Men� de pausa
    [SerializeField] private int currentWaveScore = 0;                                  // Puntaje de la oleada actual

    // ---- Control de m�sica ----
    [SerializeField] private MusicManager musicManager;                                 // Controlador de m�sica
    public AudioMixer myMixer;                                                          // Mezclador de audio
    public Slider musicSlider;                                                          // Control deslizante de m�sica
    public Slider SFXSlider;                                                            // Control deslizante de efectos

   
    
    
    // Singleton pattern

    private void Awake()
    {
        // Si hay una instancia y no es esta, destruyela.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("INI - GAMEMANAGER - Start");
        timeGeneration = 3f;
        timeWave = 6f;
        enemyInitial = 5;
        enemyNumber = 0;
        enemyCount = 0;
        score = 0;
        //waveNumber = 3;
        //numMaxWave1 = 20;
        //numMaxWave2 = 50;
        //numMaxWave3 = 100;
        //siguiente = false;
        UpdateWave();
        Debug.Log("FIN - GAMEMANAGER - Start");
    }

    public void InstantiatePollos(int initialEnemys, int enemys)
    {
        var aux = initialEnemys;
        Debug.Log("INI - GAMEMANAGER - InstantiatePollos");
        StartCoroutine(startWave(timeWave, aux, enemys));
        Debug.Log("FIN - GAMEMANAGER - InstantiatePollos");
    }

    public void chickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        Debug.Log("INI - GAMEMANAGER - chickenEnemyTakeDamage");

        if (enemy != null)
        {
            // Spawn particula de hit
            Instantiate(vfxHitWaveEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

            ContenedorEnemigo1 auxEnemy = enemy.GetComponent<ContenedorEnemigo1>();

            if (auxEnemy != null && auxEnemy.lifes >= 1)
            {
                auxEnemy.lifes -= damage;

                if (auxEnemy.lifes <= 0)
                {
                    // Generar efecto de c�mara (impulso)
                    cinemachineImpulseSource = enemy.gameObject.GetComponent<CinemachineImpulseSource>();
                    cinemachineImpulseSource.GenerateImpulse();

                    // Instanciar efectos visuales de impacto y muerte
                    Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

                    // Pausa de la animaci�n para dar efecto visual
                    StartCoroutine(FrameFreeze(0.03f));

                    auxEnemy.PolloMansy();
                    Debug.Log(enemy.name);
                    Destroy(enemy);
                    enemyDeath();
                }
            }
        }
        Debug.Log("FIN - GAMEMANAGER - chickenEnemyTakeDamage");
    }

    public void enemyDeath()
    {
        Debug.Log("INI - GAMEMANAGER - enemyDeath");
        //AUDIO: Ver si funciona en lso enemigos sino, se pone aqu�
        musicManager.Play_FX_ExplosionPollo();

        /*
        enemyCount--;

        if (enemyCount <= 0 && score == numMaxWave1 || enemyCount <= 0 && score == numMaxWave2 || enemyCount <= 0 && score == numMaxWave3 || enemyCount <= 0 && siguiente)
        {
            UpdateWave();
        }
        */
        score++;
        // Actualizar puntaje de la oleada actual
        currentWaveScore++;
        enemyCount--;

        // AUDIO: Ver si funciona en los enemigos sino, se pone aqu�
        musicManager.Play_FX_ExplosionPollo();
        Debug.Log(
            "\n\n\n" +
            "Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.1f, waveCurrent))=" + Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.1f, waveCurrent)) + "\n" +
            "enemyNumber=" + enemyNumber + "\n" +
            "enemyCount=" + enemyCount + "\n" +
            "numChicken=" + totalChicken + "\n" +
            "currentWaveScore=" + currentWaveScore + "\n" +
            "score=" + score + "\n\n\n"
        );

        // Si ya no quedan enemigos, actualizar la oleada
        if (enemyCount <= 0 && currentWaveScore >= totalChicken)
        {
            UpdateWave();
        }

        Debug.Log("FIN - GAMEMANAGER - enemyDeath");
    }

    /// <summary>
    /// Actualiza la oleada actual y escala la dificultad en funci�n de la ola.
    /// </summary>
    public void UpdateWave()
    {
        Debug.Log("INI - GAMEMANAGER - UpdateWave");
        waveCurrent++;

        // Restablecer el puntaje de la oleada actual
        currentWaveScore = 0;

        try
        {
            // Mostrar el texto de la ronda actual
            textMesh.GetComponent<TextMeshProUGUI>().text = "Round " + waveCurrent;

            // Obtenemos la dificultad en base a la oleada actual
            dificultiLevel = WaveManager.Instance.GetDifficultyPointsByWave(waveCurrent);

            // Aumentar progresivamente la cantidad de enemigos a generar
            int enemiesToSpawn = Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.2f, waveCurrent)); // Aumenta un 20% cada

            // Calcula el total de enemigos
            totalChicken = enemyInitial + enemiesToSpawn;

            // Reducir el tiempo entre generaci�n de enemigos para hacer el juego m�s desafiante
            timeGeneration = Mathf.Max(0.5f, 3f - (0.1f * waveCurrent)); // El tiempo se reduce gradualmente hasta un m�nimo de 0.5 segundos

            // Iniciar la oleada con la cantidad de enemigos escalada
            InstantiatePollos(enemyInitial, enemiesToSpawn);

            // Incrementar enemyInitial levemente cada 5 rondas, bas�ndose en el nivel de dificultad
            //enemyInitial = Mathf.FloorToInt(enemyInitial * (1 + (dificultiLevel * 0.05f))); // Incremento del 5% por nivel de dificultad
        }
        catch (System.Exception) { }

        Debug.Log("FIN - GAMEMANAGER - UpdateWave");
    }

    IEnumerator startWave(float seconds, int initialEnemies, int numEnemies)
    {
        Debug.Log("INI - GAMEMANAGER - startWave");
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

        WaveManager.Instance.GenerateChickenSquad();
        //for (int i = 0; i < initialEnemies; i++)
        //{
        //    chikenGenerator();
        //}

        // StartCoroutine(chikenWaitSpawner(numEnemies));
        Debug.Log("FIN - GAMEMANAGER - startWave");
    }

    IEnumerator chikenWaitSpawner(int numEnemies)
    {
        Debug.Log("INI - GAMEMANAGER - chikenWaitSpawner");
        //siguiente = false;

        for (int i = 0; i < numEnemies; i++)
        {
            yield return new WaitForSeconds(timeGeneration);
            chikenGenerator();
        }

        //siguiente = true;
        Debug.Log("FIN - GAMEMANAGER - chikenWaitSpawner");
    }

    public GameObject prepareChikenGenerator()
    {
        Debug.Log("INI - GAMEMANAGER - prepareChikenGenerator");
        // Generar un n�mero aleatorio para determinar el pollo a generar
        float randomNumber = Random.Range(0, 100);
        GameObject selectedPollo = null;
        AdjustProbabilities();

        // Iterar a trav�s de la lista de pollos y seleccionar uno basado en las probabilidades acumuladas
        foreach (ChickenConfig chikenConfig in chikenToSpawn)
        {
            if (randomNumber < chikenConfig.probability)
            {
                selectedPollo = chikenConfig.chickenPrefab;
                break; // Salir del loop una vez que el pollo es seleccionado
            }

            // Restar la probabilidad actual para la pr�xima comparaci�n
            randomNumber -= chikenConfig.probability;
        }

        Debug.Log("FIN - GAMEMANAGER - prepareChikenGenerator");
        return selectedPollo;
    }

    public void chikenGenerator()
    {
        Debug.Log("INI - GAMEMANAGER - chikenGenerator");
        // Generar un n�mero aleatorio para determinar el pollo a generar
        float randomNumber = Random.Range(0, 100);
        GameObject selectedPollo = null;
        AdjustProbabilities();

        // Iterar a trav�s de la lista de pollos y seleccionar uno basado en las probabilidades acumuladas
        foreach (ChickenConfig chikenConfig in chikenToSpawn)
        {
            if (randomNumber < chikenConfig.probability)
            {
                selectedPollo = chikenConfig.chickenPrefab;
                break; // Salir del loop una vez que el pollo es seleccionado
            }

            // Restar la probabilidad actual para la pr�xima comparaci�n
            randomNumber -= chikenConfig.probability;
        }

        // Si hemos seleccionado un pollo v�lido, generarlo
        if (selectedPollo != null)
        {
            Debug.Log("INI - GAMEMANAGER - chikenGenerator - selectedPollo != null");
            spawnPosition = new Vector3(Random.Range(limiteXNegativo, LimiteXPositivo), 1.2f, Random.Range(limiteZNegativo, LimiteZPositivo));
            Instantiate(selectedPollo, spawnPosition, Quaternion.identity);
            enemyCount++;
            Debug.Log("FIN - GAMEMANAGER - chikenGenerator - selectedPollo != null");
        }

        Debug.Log("FIN - GAMEMANAGER - chikenGenerator");
    }

    public Vector3 getRandomAreaSpawn()
    {
        return new Vector3(Random.Range(limiteXNegativo, LimiteXPositivo), 1.2f, Random.Range(limiteZNegativo, LimiteZPositivo));
    }

    // M�todo para ajustar las probabilidades de los pollos en el array chikenToSpawn
    public void AdjustProbabilities()
    {
        // Sumar todas las probabilidades actuales
        float totalProbability = 0;
        foreach (ChickenConfig chicken in chikenToSpawn)
        {
            totalProbability += chicken.probability;
        }

        // Si la suma de probabilidades es 0, evitar la divisi�n por 0
        if (totalProbability == 0)
        {
            Debug.LogError("Error: Las probabilidades suman 0, no se pueden ajustar.");
            return;
        }

        // Ajustar cada probabilidad para que sumen 100%
        foreach (ChickenConfig chicken in chikenToSpawn)
        {
            chicken.probability = ((chicken.probability / totalProbability) * 100);
        }

        Debug.Log("Probabilidades ajustadas correctamente para que sumen 100%.");
    }

    private IEnumerator FrameFreeze(float duration)
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
    }

    public void ActivarFXMuerte()
    {

    }

    //Menu pausa
    public void exit_pause_menu()
    {
        Time.timeScale = 1;
        paused = false;
        pausemenu.SetActive(false);
    }

    public void pause()
    {
        if (paused)
        {
            exit_pause_menu();
        }

        if (paused == false)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName != "MenuPrincipal")
            {
                paused = true;
                pausemenu.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            if (currentSceneName == "MenuPrincipal")
            {
                Debug.Log("Est�s en la Escena principal");

            }
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back_to_Main_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("0_MenuPrincipal");
    }

    //Music Manager
    public void SetMusicVolume()
    {
        Debug.Log("Music modified");
        float volume = musicSlider.value;
        Debug.Log(musicSlider.value);
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
    }
}

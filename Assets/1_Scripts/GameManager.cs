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

    public GameObject player;                                                           // Player
    // ---- Control de oleadas ----
    public int level = 1;                                                               // Nivel/Mapa
    [SerializeField] private float timeWave = 15;                                       // Tiempo por oleada
    [SerializeField] private float timeGeneration = 15;                                 // Tiempo entre generación de enemigos
    public int enemyNumber = 0;                                                         // Número total de enemigos en la ronda
    public int enemyInitial = 5;                                                        // Número de enemigos por oleada inicial
    public int enemyCount = 5;                                                          // Número de enemigos que quedan en la ronda
    public int waveNumber = 0;                                                          // Número de rondas totales
    public int waveCurrent = 0;                                                         // Número de la ronda actual
    public int dificultPoints = 0;                                                      // Nivel de dificultad
    public int capGenerator = 200;                                                      // Cap del Generador
    //[SerializeField] private bool siguiente;                                          // Control para saber si continúa a la siguiente oleada
    //[SerializeField] private int numMaxWave1;                                         // Máx. enemigos en la ola 1
    //[SerializeField] private int numMaxWave2;                                         // Máx. enemigos en la ola 2
    //[SerializeField] private int numMaxWave3;                                         // Máx. enemigos en la ola 3

    [SerializeField] private Canvas canvasRound;                                        // UI de la ronda
    [SerializeField] private Animator canvasAnimator;                                   // Animador para transiciones
    [SerializeField] private GameObject textMesh;                                       // Texto de la ronda
    private CinemachineImpulseSource cinemachineImpulseSource;                          // Fuente del efecto de impulso

    // ---- Control de enemigos ----
    public List<GameObject> listEnemies;                                                // Lista de Enemigos
    public List<GameObject> listCorpses;                                                // Lista de Corpses
    public List<GameObject> listSpawns;                                                 // Lista de spawns
    [SerializeField] private List<ChickenConfig> chikenToSpawn;                         // Lista de enemigos, probabilidades y puntuacion
    [SerializeField] private List<ChickenConfigWave> chikenToSpawnWave;                 // Lista de enemigos, probabilidades y puntuacion según oleada y mapa/nivel
    [SerializeField] private List<ChickenDifficult> difficult;                          // Lista de enemigos, probabilidades y puntuacion según oleada y mapa/nivel
    public int totalChicken = 0;                                                        // Total de pollos
    [SerializeField] private Vector3 spawnPosition;                                     // Posición de generación
    [SerializeField] private GameObject vfxHitEffect;                                   // Efecto al recibir golpe
    [SerializeField] private GameObject vfxHitWaveEffect;                               // Efecto al recibir golpe en oleada
    [SerializeField] private GameObject SmokeEffect;                                    // Efecto de humo al generar enemigo
    public float limiteXNegativo, LimiteXPositivo, limiteZNegativo, LimiteZPositivo;    // Límites de generación

    // ---- Control del juego ----
    [SerializeField] public bool paused = false;                                        // Estado de pausa
    [SerializeField] public int score = 0;                                              // Puntuación
    public GameObject scoreText;                                                        // Texto de la puntuación
    public GameObject pausemenu;                                                        // Menú de pausa
    [SerializeField] private int currentWaveScore = 0;                                  // Puntaje de la oleada actual

    // ---- Control de música ----
    [SerializeField] private MusicManager musicManager;                                 // Controlador de música
    public AudioMixer myMixer;                                                          // Mezclador de audio
    public Slider musicSlider;                                                          // Control deslizante de música
    public Slider SFXSlider;                                                            // Control deslizante de efectos

    // -------- TEMPORAL ------------
    [SerializeField] private int normalChickenCount = 0;
    [SerializeField] private int fastChickenCount = 0;
    [SerializeField] private int bombChickenCount = 0;
    [SerializeField] private int bigChickenCount = 0;
    [SerializeField] private int capSquadNormalChicken = 0;
    [SerializeField] private int capSquadFastChicken = 0;
    [SerializeField] private int capSquadBombChicken = 0;
    [SerializeField] private int capSquadBigChicken = 0;

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
        //Debug.Log("INI - GAMEMANAGER - Start");
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
        //Debug.Log("FIN - GAMEMANAGER - Start");
    }

    public void chickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        //Debug.Log("INI - GAMEMANAGER - chickenEnemyTakeDamage");

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
                    // Generar efecto de cámara (impulso)
                    cinemachineImpulseSource = enemy.gameObject.GetComponent<CinemachineImpulseSource>();
                    cinemachineImpulseSource.GenerateImpulse();

                    // Instanciar efectos visuales de impacto y muerte
                    Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

                    // Pausa de la animación para dar efecto visual
                    StartCoroutine(FrameFreeze(0.03f));

                    auxEnemy.PolloMansy();
                    //Debug.Log(enemy.name);
                    EnemyDeath(enemy);
                    Destroy(enemy);
                }
            }
        }
        //Debug.Log("FIN - GAMEMANAGER - chickenEnemyTakeDamage");
    }

    public void EnemyDeath(GameObject enemy)
    {
        //Debug.Log("INI - GAMEMANAGER - enemyDeath");
        //AUDIO: Ver si funciona en lso enemigos sino, se pone aquí
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
        KillCountChiken(enemy.name);
        enemyCount--;

        // AUDIO: Ver si funciona en los enemigos sino, se pone aquí
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

        //Debug.Log("FIN - GAMEMANAGER - enemyDeath");
    }

    /// <summary>
    /// Actualiza la oleada actual y escala la dificultad en función de la ola.
    /// </summary>
    public void UpdateWave()
    {
        //Debug.Log("INI - GAMEMANAGER - UpdateWave");
        // Incrementar la oleada actual
        waveCurrent++;

        if ((waveCurrent > WaveManager.Instance.waveForLevel) && level <= 2)
        {
            level++;
            waveCurrent = 1;
        }

        // Restablecer el puntaje de la oleada actual
        currentWaveScore = 0;

        try
        {
            // Mostrar el texto de la ronda actual
            textMesh.GetComponent<TextMeshProUGUI>().text = $"Level {level}\nRound {waveCurrent}";

            // Obtenemos la dificultad en base a la oleada actual
            dificultPoints = WaveManager.Instance.GetDifficultyPointsByWave(level, waveCurrent);

            // Aumentar progresivamente la cantidad de enemigos a generar
            // int enemiesToSpawn = Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.2f, waveCurrent)); // Aumenta un 20% cada

            // Calcula el total de enemigos
            totalChicken = dificultPoints;

            // Reducir el tiempo entre generación de enemigos para hacer el juego más desafiante
            // timeGeneration = Mathf.Max(0.5f, 3f - (0.1f * waveCurrent)); // El tiempo se reduce gradualmente hasta un mínimo de 0.5 segundos

            StartCoroutine(StartWave());
        }
        catch (System.Exception) { }

        //Debug.Log("FIN - GAMEMANAGER - UpdateWave");
    }

    IEnumerator StartWave()
    {
        //Debug.Log("INI - GAMEMANAGER - startWave");
        canvasRound.gameObject.SetActive(true);

        //Audio
        musicManager.FX_ActivarCorutina((waveCurrent - 1) < 0 ? 0 : waveCurrent - 1, waveCurrent);

        yield return new WaitForSeconds(timeWave - 2f);

        if (canvasAnimator != null)
        {
            canvasAnimator.SetTrigger("Change");
        }

        yield return new WaitForSeconds(2f);
        canvasRound.gameObject.SetActive(false);

        /*if (dificultPoints <= 50)
            
            InstanceChicken();
        else
        {
            // Iniciar la oleada con la cantidad de enemigos escalada
            float randomNumber = Random.Range(0, 100);

            if (randomNumber <= 50)
                // Instantaneo 75%, entre Squads y Alone
                InstanceChicken();
            else
                // Cada 3 segundos, entre Squads y Alone
                InstanceChicken();
        }*/

        InstanceChicken();
        //WaveManager.Instance.GenerateChickenSquad();
        //Debug.Log("FIN - GAMEMANAGER - startWave");
    }

    public void InstanceChicken()
    {
        StartCoroutine(GenerateEnemiesAsync());
    }

    private IEnumerator GenerateEnemiesAsync()
    {
        // Instantaneo si el total es menor de 50, entre Squads y Alone
        if (dificultPoints <= 50)
        {
            while (dificultPoints > 0)
            {
                ChikenAloneGenerator();
            }
        }
        else
        {
            do
            {
                // Calcular un nuevo randomNumber para decidir entre las dos últimas condiciones
                float randomNumber = Random.Range(0, 100);

                // Si ya hay el límite de enemigos en pantalla, esperar hasta que baje
                if (enemyCount >= capGenerator)
                    if (randomNumber <= 50)
                        yield return new WaitUntil(() => enemyCount <= capGenerator * 0.25f);
                    else
                        yield return new WaitUntil(() => enemyCount <= capGenerator * 0.75f);

                if (randomNumber <= 50)
                    // Instantáneo 75% cuando enemyCount <= 25% del capGenerator
                    yield return StartCoroutine(GenerateInstantEnemies());
                else
                    // Generar cada 3 segundos hasta alcanzar el capGenerator
                    yield return StartCoroutine(GenerateEnemiesOverTime());

                // Actualizar dificultad, reducimos el difficultyLevel según los enemigos generados
                //dificultPoints -= Mathf.Min(capGenerator, totalChicken - enemyCount);
            } while (dificultPoints > 0);
        }
    }

    private IEnumerator GenerateInstantEnemies()
    {
        //int remainingEnemies = Mathf.FloorToInt(capGenerator * 0.75f);
        yield return new WaitForSeconds(0f);

        // Generar 75% de los enemigos de manera instantánea
        while (enemyCount < capGenerator)
        {
            float randomNumber = Random.Range(0, 100);

            if (randomNumber >= 50)
            {
                SpawnChickenSquad(SelectChickenBasedOnProb(), ref dificultPoints);
            }
            else
            {
                ChikenAloneGenerator();
            }

            // Si dificultPoints es 0 o menor, detén el bucle
            if (dificultPoints <= 0)
                break;
        }
    }

    private IEnumerator GenerateEnemiesOverTime()
    {
        //int remainingEnemies = Mathf.Min(capGenerator - enemyCount, dificultPoints);

        do
        {
            // Generar un enemigo
            float randomNumber = Random.Range(0, 100);

            if (randomNumber >= 50)
            {
                SpawnChickenSquad(SelectChickenBasedOnProb(), ref dificultPoints);
            }
            else
            {
                ChikenAloneGenerator();
            }
            //enemyCount++;
            //dificultPoints--;
            //remainingEnemies--;

            yield return new WaitForSeconds(3f); // Esperar 3 segundos entre generaciones

            // Si dificultPoints es 0 o menor, detén el bucle
            if (dificultPoints <= 0)
                break;
        }
        while (enemyCount < capGenerator);
    }

/*    IEnumerator chikenWaitSpawner(int numEnemies, int timeGeneration)
    {
        //Debug.Log("INI - GAMEMANAGER - chikenWaitSpawner");
        //siguiente = false;

        for (int i = 0; i < numEnemies; i++)
        {
            yield return new WaitForSeconds(timeGeneration);
            ChikenGenerator();
        }

        //siguiente = true;
        //Debug.Log("FIN - GAMEMANAGER - chikenWaitSpawner");
    }*/

/*    public void ChikenGenerator()
    {
        //Debug.Log("INI - GAMEMANAGER - chikenGenerator");
        // Generar un número aleatorio para determinar el pollo a generar
        float randomNumber = Random.Range(0, 100);

        if (randomNumber >= 50)
        {

        }
        else
        {
            ChikenAloneGenerator();
        }
        //Debug.Log("FIN - GAMEMANAGER - chikenGenerator");
    }*/

    public void ChikenAloneGenerator()
    {
        //Debug.Log("INI - GAMEMANAGER - chikenGenerator");
        ChickenConfig auxChicken = SelectChickenBasedOnProb();
        GameObject auxNewChicken = Instantiate(auxChicken.chickenPrefab, getRandomAreaSpawn(), Quaternion.identity);
        listEnemies.Add(auxNewChicken);
        dificultPoints -= auxChicken.difficultyScore;
        CountChiken(auxChicken.chickenPrefab.name);
        //Debug.Log("FIN - GAMEMANAGER - chikenGenerator");
    }

    // Método para generar escuadrón de pollos blancos
    private void SpawnChickenSquad(ChickenConfig chicken, ref int difficultyPointsLeft)
    {
        //int chickensToSpawn = Mathf.Min(10, difficultyPointsLeft); // Si hay menos puntos, genera menos pollos
        Vector3 auxPosition = GetRandomSpawnPosition();

        for (int i = 0; i < CapSquadChicken(chicken.chickenPrefab.name); i++)
        {
            GameObject auxNewChicken = Instantiate(chicken.chickenPrefab, auxPosition, Quaternion.identity);
            listEnemies.Add(auxNewChicken);
            difficultyPointsLeft -= chicken.difficultyScore;
            CountChiken(chicken.chickenPrefab.name);
        }
    }

    // Método auxiliar para seleccionar el pollo basado en probabilidades
    public ChickenConfig SelectChickenBasedOnProb()
    {
        //Debug.Log("INI - GAMEMANAGER - selectChickenBasedOnProb");
        ChickenConfig selectedPollo = null;
        // Generar un número aleatorio para determinar el pollo a generar
        float randomNumber = Random.Range(0, 100);

        // Ajusta las probabilidades de los pollos existentes a que realmente sean sobre 100 y esté balanceado
        AdjustProbabilities();

        // Selecciona uno basado en las probabilidades acumuladas
        foreach (ChickenConfig chikenConfig in chikenToSpawn)
        {
            if (randomNumber < chikenConfig.probability)
            {
                selectedPollo = chikenConfig;

                break; // Salir del loop una vez que el pollo es seleccionado
            }

            // Restar la probabilidad actual para la próxima comparación
            randomNumber -= chikenConfig.probability;
        }

        //Debug.Log("FIN - GAMEMANAGER - selectChickenBasedOnProb");
        return selectedPollo; // Si no se selecciona ningún pollo, devolver null
    }

    /*    public GameObject prepareChikenGeneratorWithProb()
        {
            //Debug.Log("INI - GAMEMANAGER - prepareChikenGeneratorWithProb");
            // Generar un número aleatorio para determinar el pollo a generar
            float randomNumber = Random.Range(0, 100);
            GameObject selectedPollo = null;
            ChickenConfig auxChikenConfig = null;
            AdjustProbabilities();

            // Iterar a través de la lista de pollos y seleccionar uno basado en las probabilidades acumuladas
            foreach (ChickenConfig chikenConfig in chikenToSpawn)
            {
                if (randomNumber < chikenConfig.probability)
                {
                    selectedPollo = chikenConfig.chickenPrefab;
                    auxChikenConfig = chikenConfig;
                    countChiken(selectedPollo.name);
                    break; // Salir del loop una vez que el pollo es seleccionado
                }

                // Restar la probabilidad actual para la próxima comparación
                randomNumber -= chikenConfig.probability;
            }

            dificultPoints -= auxChikenConfig.difficultyScore;
            //Debug.Log("FIN - GAMEMANAGER - prepareChikenGeneratorWithProb");
            return selectedPollo;
        }*/

    private void CountChiken(string name)
    {
        if (name.Contains("Normal"))
        {
            normalChickenCount++;
            enemyCount++;
        }
        else if (name.Contains("Fast"))
        {
            fastChickenCount++;
            enemyCount++;
        }
        else if (name.Contains("Bomb"))
        {
            bombChickenCount++;
            enemyCount++;
        }
        else if (name.Contains("Big"))
        {
            bigChickenCount++;
            enemyCount++;
        }
    }

    private void KillCountChiken(string name)
    {
        if (name.Contains("Normal"))
        {
            currentWaveScore++;
        }
        else if (name.Contains("Fast"))
        {
            currentWaveScore += 3;
        }
        else if (name.Contains("Bomb"))
        {
            currentWaveScore += 10;
        }
        else if (name.Contains("Big"))
        {
            currentWaveScore += 4;
        }
    }

    private int CapSquadChicken(string name)
    {
        int auxCap = 10;

        if (name.Contains("Bomb"))
            auxCap = capSquadBombChicken;
        else if (name.Contains("Big"))
            auxCap = capSquadBigChicken;

        return auxCap;
    }

    // Método para ajustar las probabilidades de los pollos en el array chikenToSpawn
    public void AdjustProbabilities()
    {
        // Sumar todas las probabilidades actuales
        float totalProbability = 0;
        foreach (ChickenConfig chicken in chikenToSpawn)
        {
            totalProbability += chicken.probability;
        }

        // Si la suma de probabilidades es 0, evitar la división por 0
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

        //Debug.Log("Probabilidades ajustadas correctamente para que sumen 100%.");
    }

    /// <summary>
    /// Area para los limites de spawn del mapa, que calcula una posicion aleatoria dentro de esa area
    /// </summary>
    /// <returns>
    /// Un vector con una posicion aleatoria dentro de esa area
    /// </returns>
    public Vector3 getRandomAreaSpawn()
    {
        return new Vector3(Random.Range(limiteXNegativo, LimiteXPositivo), 1.2f, Random.Range(limiteZNegativo, LimiteZPositivo));
    }

    // Método para obtener una posición aleatoria del array
    private Vector3 GetRandomSpawnPosition()
    {
        // Posición del jugador
        Vector3 playerPosition = player.transform.position;

        // Crear una lista de posiciones de spawn y su distancia al jugador
        List<(Vector3 position, float distance)> spawnDistances = new List<(Vector3, float)>();

        // Rellenar la lista con las posiciones de spawn y calcular sus distancias al jugador
        foreach (var spawnPosition in listSpawns)
        {
            float distance = Vector3.Distance(playerPosition, spawnPosition.transform.position);
            spawnDistances.Add((spawnPosition.transform.position, distance));
        }

        // Ordenar la lista por distancia al jugador
        spawnDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

        // Quitar la posición más cercana y la más lejana, si hay más de dos elementos
        if (spawnDistances.Count > 2)
        {
            spawnDistances.RemoveAt(0); // Eliminar la posición más cercana
            spawnDistances.RemoveAt(spawnDistances.Count - 1); // Eliminar la posición más lejana
        }

        // Elegir una posición aleatoria entre las posiciones restantes
        int randomIndex = Random.Range(0, spawnDistances.Count);
        return spawnDistances[randomIndex].position;
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
                //Debug.Log("Estás en la Escena principal");

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
        //Debug.Log("Music modified");
        float volume = musicSlider.value;
        //Debug.Log(musicSlider.value);
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("FX", Mathf.Log10(volume) * 20);
    }

    public void changeLevel()
    {
        level++;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.transform.position = new Vector3(0, 5, 0);


    }

}

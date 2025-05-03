using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    // ---- Singleton ----
    public static GameManager Instance { get; private set; }

    // Player
    [Header("Player")]
    public GameObject player; 
    private ChickenLouncher launcher;

    // ---- Control de Oleadas ----
    [Header("Wave Control")]
    public int level = 1;                                                               // Nivel/Mapa
    [SerializeField] private float timeWave = 15;                                       // Tiempo por oleada
    [SerializeField] private float timeGeneration = 15;                                 // Tiempo entre generación de enemigos
    public int enemyNumber = 0;                                                         // Número total de enemigos en la ronda
    public int enemyInitial = 5;                                                        // Número de enemigos por oleada inicial
    public int enemyCount = 0;                                                          // Número de enemigos que quedan en la ronda
    public int waveNumber = 0;                                                          // Número de rondas totales
    public int waveCurrent = 0;                                                         // Número de la ronda actual
    public int dificultPoints = 0;                                                      // Nivel de dificultad
    public int capGenerator = 200;                                                      // Cap del Generador

    // ---- Control de Enemigos ----
    [Header("Enemy Control")]
    public List<GameObject> listEnemies;                                                // Lista de Enemigos
    public List<GameObject> listCorpses;                                                // Lista de Corpses
    public Transform spawnParent;
    public List<GameObject> listSpawns;                                                 // Lista de spawns
    [SerializeField] private List<ChickenConfig> chikenToSpawn;                         // Lista de enemigos, probabilidades y puntuación
    [SerializeField] private List<ChickenConfigWave> chikenToSpawnWave;                 // Lista de enemigos por oleada y nivel
    public int totalWaveChicken = 0;                                                    // Total de pollos
    [SerializeField] private Vector3 spawnPosition;                                     // Posición de generación
    [SerializeField] private GameObject vfxHitEffect;                                   // Efecto al recibir golpe
    [SerializeField] private GameObject vfxHitWaveEffect;                               // Efecto al recibir golpe en oleada
    [SerializeField] private GameObject SmokeEffect;                                    // Efecto de humo al generar enemigo
    public float limiteXNegativo, LimiteXPositivo, limiteZNegativo, LimiteZPositivo;    // Límites de generación

    // ---- Temporales ----
    [Header("Temporary Variables")]
    [SerializeField] private int capSquadNormalChicken = 0;
    [SerializeField] private int capSquadFastChicken = 0;
    [SerializeField] private int capSquadBombChicken = 0;
    [SerializeField] private int capSquadBigChicken = 0;

    // ---- Cantidad de Pollos ----
    [Header("Chicken Quantity")]
    [SerializeField] private List<ChickenCount> instanciateChickenCount;                // Lista de enemigos instanciados
    [SerializeField] private List<ChickenCount> killChickenCount;                       // Lista de enemigos muertos

    // ---- UI y Animación ----
    [Header("UI & Animation")]
    [SerializeField] private Canvas canvasRound;                                        // UI de la ronda
    [SerializeField] private Animator canvasAnimator;                                   // Animador para transiciones
    [SerializeField] private GameObject textMesh;                                       // Texto de la ronda
    private CinemachineImpulseSource cinemachineImpulseSource;                          // Fuente del efecto de impulso

    // ---- Control del Juego ----
    [Header("Game Control")]
    [SerializeField] private bool paused = false;                                       // Estado de pausa
    public int score = 0;                                                               // Puntuación
    public GameObject pausemenu;                                                        // Menú de pausa
    public GameObject highscore;
    public HighscoreTable highscoreTable;
    public GameObject nameInput;
    public GameObject restartMenu;
    [SerializeField] private int currentWaveScore = 0;                                  // Puntuación de la oleada actual
    private int isNeededHeal = 0;                                                       // Control para la aparición del pollo heal

    // ---- Control de Música ----
    [Header("Music Control")]
    [SerializeField] private MusicManager musicManager;                                 // Controlador de música
    public AudioMixer myMixer;                                                          // Mezclador de audio
    public Slider musicSlider;                                                          // Control deslizante de música
    public Slider SFXSlider;                                                            // Control deslizante de efectos

    // ---- Control de Input ----
    [Header("Input Control")]
    public GameObject firstGameObjectMenu;
    public GameObject firstGameObjectMusic;
    public GameObject firstGameObjectMusicToMenu;
    public GameObject firstGameObjectRanking;
    public GameObject restartButtonFirstObject;
    // Singleton pattern
    private void Awake()
    {
        // Si hay una instancia y no es esta, destruyela.
        if (Instance != null && Instance != this)
            Destroy(this);

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("INI - GAMEMANAGER - Start");
        launcher = player.GetComponent<ChickenLouncher>();
        PopulateSpawnList(spawnParent); // Rellena la lista con los hijos

       
        timeGeneration = 3f;
        timeWave = 6f;
        enemyInitial = 5;
        enemyNumber = 0;
        enemyCount = 0;
        score = 0;
        totalWaveChicken = 0;
        UpdateWave();
        //Debug.Log("FIN - GAMEMANAGER - Start");
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


    public void ChickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        //Debug.Log("INI - GAMEMANAGER - ChickenEnemyTakeDamage");

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


                    // Generar efecto de camara (impulso)
                   // cinemachineImpulseSource = enemy.GetComponent<CinemachineImpulseSource>();
                    //cinemachineImpulseSource.GenerateImpulse();

                    // Instanciar efectos visuales de impacto y muerte
                    Instantiate(vfxHitEffect, enemy.transform.position + new Vector3(0, 1, 0), Quaternion.identity);

                    // Pausa de la animacion para dar efecto visual
                    StartCoroutine(FrameFreeze(0.03f));

                    //SISTEMA DE MELEE EN PROGRESO
                    //if (launcher.currentChickenType > 0) //Si el jugador ya tiene un pollo en la mano
                    //{
                    //    auxEnemy.PolloMansy();
                    //}
                    //else
                    //{
                    //    launcher.RetrieveChicken(auxEnemy.corpse.GetComponent<ChickenCorpse>().chickenType); //DIOS MIO LOS MALABARES PARA SACAR EL CHICKEN TYPE
                    //}


                    auxEnemy.PolloMansy(); //QUITAR CUANDO EL SISTEMA DE MELEE ESTE BIEN
                    EnemyDeath(enemy);
                    Destroy(enemy);

                    //auxEnemy.PolloMansy();
                    ////Debug.Log(enemy.name);
                    //EnemyDeath(enemy);
                    //Destroy(enemy);
                }
            }
        }
        //Debug.Log("FIN - GAMEMANAGER - ChickenEnemyTakeDamage");
    }

    public void EnemyDeath(GameObject enemy)
    {
        //Debug.Log("INI - GAMEMANAGER - enemyDeath");
        ChickenSpawnService aux = new(chikenToSpawn, chikenToSpawnWave);
        ChickenConfig chickenHeal = aux.SelectChicken("Heal");
        int auxCount = 0;
        //AUDIO: Ver si funciona en lso enemigos sino, se pone aqui
        musicManager.Play_FX_ExplosionPollo();

        /*
        enemyCount--;

        if (enemyCount <= 0 && score == numMaxWave1 || enemyCount <= 0 && score == numMaxWave2 || enemyCount <= 0 && score == numMaxWave3 || enemyCount <= 0 && siguiente)
        {
            UpdateWave();
        }
        */
        // Actualizar puntaje de la oleada actual

        if (!enemy.name.Contains("Heal"))
        {
            KillCountChicken(enemy.name);
            enemyCount--;
        }

        // AUDIO: Ver si funciona en los enemigos sino, se pone aqui
        musicManager.Play_FX_ExplosionPollo();
        /*
        Debug.Log(
            "\n\n\n" +
            "Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.1f, waveCurrent))=" + Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.1f, waveCurrent)) + "\n" +
            "enemyNumber=" + enemyNumber + "\n" +
            "enemyCount=" + enemyCount + "\n" +
            "numChicken=" + totalWaveChicken + "\n" +
            "currentWaveScore=" + currentWaveScore + "\n" +
            "score=" + score + "\n\n\n"
        );
        */

        foreach (ChickenCount count in killChickenCount)
        {
            auxCount += count.quantity;
        }

        if (isNeededHeal + 30 <= auxCount && player.GetComponent<PlayerHealth>().GetHealth() < 3)
        {
            Instantiate(chickenHeal.chickenPrefab, GetRandomAreaSpawn(), Quaternion.identity);
            isNeededHeal = auxCount;
        }

        // Si ya no quedan enemigos, actualizar la oleada
        if (enemyCount <= 0 && currentWaveScore >= totalWaveChicken)
        {
            UpdateWave();
        }

        //Debug.Log("FIN - GAMEMANAGER - enemyDeath");
    }

    /// <summary>
    /// Actualiza la oleada actual y escala la dificultad en funcion de la ola.
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

        capGenerator = new ChickenSpawnService().GetCapGenerator(level, waveCurrent);

        // Restablecer el puntaje de la oleada actual
        totalWaveChicken = 0;
        currentWaveScore = 0;
        // Por el momento no se reiniciarï¿½n los contadores
        //instanciateChickenCount = new List<ChickenCount>();
        //killChickenCount = new List<ChickenCount>();

        try
        {
            // Mostrar el texto de la ronda actual
            textMesh.GetComponent<TextMeshProUGUI>().text = $"Level {level}\nRound {waveCurrent}";

            // Obtenemos la dificultad en base a la oleada actual
            dificultPoints = WaveManager.Instance.GetDifficultyPointsByWave(level, waveCurrent);

            // Aumentar progresivamente la cantidad de enemigos a generar
            // int enemiesToSpawn = Mathf.FloorToInt(enemyInitial * Mathf.Pow(1.2f, waveCurrent)); // Aumenta un 20% cada

            // Calcula el total de enemigos
            // totalWaveChicken = dificultPoints;

            // Reducir el tiempo entre generacion de enemigos para hacer el juego mas desafiante
            // timeGeneration = Mathf.Max(0.5f, 3f - (0.1f * waveCurrent)); // El tiempo se reduce gradualmente hasta un minimo de 0.5 segundos

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
        // musicManager.FX_ActivarCorutina((waveCurrent - 1) < 0 ? 0 : waveCurrent - 1, waveCurrent);
        musicManager.FX_ActivarCorutina();


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
        int auxCapGenerator = capGenerator;
        ChickenSpawnService aux = new(chikenToSpawn, chikenToSpawnWave);

        //if (waveCurrent > 7)
        //    auxCapGenerator = capGenerator / 2;

        for (int i = 0; i < auxCapGenerator; i++)
        {
            float randomNumber = Random.Range(0, 100);

            if (randomNumber >= 50)
                i += SpawnChickenSquad(aux.SelectChickenBasedOnProb(level, waveCurrent), ref dificultPoints) - 1;
            else
                ChikenAloneGenerator();
        }

        // Instantaneo si el total es menor de 50, entre Squads y Alone
        /*if (dificultPoints <= 50)
        {
            while (dificultPoints > 0)
            {
                ChikenAloneGenerator();
            }
        }
        else
        {*/
        //do
        //{
        for (int i = 0; i < 1; i++)
        {
            // Calcular un nuevo randomNumber para decidir entre las dos ultimas condiciones
            float randomNumber = Random.Range(0, 100);

            // Si ya hay el limite de enemigos en pantalla, esperar hasta que baje
            /*if (enemyCount >= capGenerator)*/
            if (randomNumber <= 50)
                yield return new WaitUntil(() => enemyCount <= (capGenerator * 0.25f));
            else
                yield return new WaitUntil(() => enemyCount <= (capGenerator * 0.75f));

            if (randomNumber <= 50)
                // Instantaneo 75% cuando enemyCount <= 25% del capGenerator
                yield return StartCoroutine(GenerateInstantEnemies());
            else
                // Generar cada 3 segundos hasta alcanzar el capGenerator
                yield return StartCoroutine(GenerateEnemiesOverTime());

            // Actualizar dificultad, reducimos el difficultyLevel segun los enemigos generados
            // dificultPoints -= Mathf.Min(capGenerator, totalWaveChicken - enemyCount);
        }
        //} while (dificultPoints > 0);
        /*}*/
    }

    private IEnumerator GenerateInstantEnemies()
    {
        ChickenSpawnService aux = new(chikenToSpawn, chikenToSpawnWave);
        //int remainingEnemies = Mathf.FloorToInt(capGenerator * 0.75f);
        yield return new WaitForSeconds(0f);

        // Generar 75% de los enemigos de manera instantanea
        //while (enemyCount < capGenerator)
        //{
        for (int i = 0; i < capGenerator; i++)
        {
            float randomNumber = Random.Range(0, 100);

            if (randomNumber >= 50)
                i += SpawnChickenSquad(aux.SelectChickenBasedOnProb(level, waveCurrent), ref dificultPoints) - 1;
            else
                ChikenAloneGenerator();

            // Si dificultPoints es 0 o menor, deten el bucle
            if (dificultPoints <= 0)
                break;
        }
        //}
    }

    private IEnumerator GenerateEnemiesOverTime()
    {
        ChickenSpawnService aux = new(chikenToSpawn, chikenToSpawnWave);
        //int remainingEnemies = Mathf.Min(capGenerator - enemyCount, dificultPoints);

        //do
        //{
        for (int i = 0; i < capGenerator; i++)
        {
            // Generar un enemigo
            float randomNumber = Random.Range(0, 100);

            if (randomNumber >= 50)
                i += SpawnChickenSquad(aux.SelectChickenBasedOnProb(level, waveCurrent), ref dificultPoints) - 1;
            else
                ChikenAloneGenerator();
            //enemyCount++;
            //dificultPoints--;
            //remainingEnemies--;

            yield return new WaitForSeconds(2f); // Esperar 3 segundos entre generaciones

            // Si dificultPoints es 0 o menor, deten el bucle
            if (dificultPoints <= 0)
                break;
        }
        //}
        //while (enemyCount < capGenerator);
    }

    public void ChikenAloneGenerator()
    {
        ChickenSpawnService aux = new(chikenToSpawn, chikenToSpawnWave);
        //Debug.Log("INI - GAMEMANAGER - chikenGenerator");
        ChickenConfig auxChicken = aux.SelectChickenBasedOnProb(level, waveCurrent);
        //Debug.Log(auxChicken.chickenPrefab.name);
        //Debug.Log(GetRandomAreaSpawn());
        GameObject auxNewChicken = Instantiate(auxChicken.chickenPrefab, GetRandomAreaSpawn(), Quaternion.identity);
        listEnemies.Add(auxNewChicken);

        enemyCount++;
        totalWaveChicken += auxChicken.difficultyScore;
        dificultPoints -= auxChicken.difficultyScore;

        CountChiken(auxChicken.chickenPrefab.name);
        //Debug.Log("FIN - GAMEMANAGER - chikenGenerator");
    }

    // Metodo para generar escuadron de pollos blancos
    private int SpawnChickenSquad(ChickenConfig chicken, ref int difficultyPointsLeft)
    {
        //int chickensToSpawn = Mathf.Min(10, difficultyPointsLeft); // Si hay menos puntos, genera menos pollos
        Vector3 auxPosition = GetRandomSpawnPosition();
        int auxTotal = CapSquadChicken(chicken.chickenPrefab.name);

        for (int i = 0; i < auxTotal; i++)
        {
            GameObject auxNewChicken = Instantiate(chicken.chickenPrefab, auxPosition, Quaternion.identity);
            listEnemies.Add(auxNewChicken);

            enemyCount++;
            totalWaveChicken += chicken.difficultyScore;
            difficultyPointsLeft -= chicken.difficultyScore;

            CountChiken(chicken.chickenPrefab.name);
        }

        return auxTotal;
    }

    // Cuenta dinamicamente a los pollos
    private void CountChiken(string name)
    {
        // Extraer el tipo de enemigo desde el nombre
        string enemyType = GetEnemyTypeFromName(name);
        AddOrUpdateChickenCount(instanciateChickenCount, new(enemyType));
    }

    // Metodo para contar el puntaje de un enemigo eliminado
    private void KillCountChicken(string name)
    {
        // Extraer el tipo de enemigo desde el nombre
        string enemyType = GetEnemyTypeFromName(name);
        AddOrUpdateChickenCount(killChickenCount, new(enemyType));

        // Buscar en la lista chikenToSpawn la configuracion que coincide con el tipo de enemigo
        int scoreIncrement = chikenToSpawn.FirstOrDefault(c => c.chickenPrefab.name.Contains(enemyType))?.difficultyScore ?? 1;

        // Sumar al puntaje
        currentWaveScore += scoreIncrement;
        score += scoreIncrement;
    }

    // Metodo para agregar o actualizar un ChickenCount en la lista
    public void AddOrUpdateChickenCount(List<ChickenCount> array, string enemyType)
    {
        // Busca si ya existe un ChickenCount con el mismo typeName
        ChickenCount existingChicken = array.FirstOrDefault(c => c.typeName == enemyType);

        if (existingChicken != null)
        {
            // Si ya existe, incrementa la cantidad
            existingChicken.quantity++;
        }
        else
        {
            // Si no existe, crea uno nuevo y lo agrega a la lista
            array.Add(new ChickenCount(enemyType));
        }
    }

    // Metodo para ajustar las probabilidades de los pollos en el array chikenToSpawn
    public void AdjustProbabilities(ref List<ChickenConfig> chikenToSpawns)
    {
        // Sumar todas las probabilidades actuales
        float totalProbability = 0;
        foreach (ChickenConfig chicken in chikenToSpawns)
        {
            totalProbability += chicken.probability;
        }

        // Si la suma de probabilidades es 0, evitar la division por 0
        if (totalProbability == 0)
        {
            Debug.LogError("Error: Las probabilidades suman 0, no se pueden ajustar.");
            return;
        }

        // Ajustar cada probabilidad para que sumen 100%
        foreach (ChickenConfig chicken in chikenToSpawns)
        {
            chicken.probability = ((chicken.probability / totalProbability) * 100);
        }

        //Debug.Log("Probabilidades ajustadas correctamente para que sumen 100%.");
    }

    // Mï¿½todo auxiliar para extraer el tipo de enemigo de su nombre
    private string GetEnemyTypeFromName(string name)
    {
        int startIndex = name.IndexOf("Pfb_Enemy") + "Pfb_Enemy".Length;
        int endIndex = name.IndexOf("(Clone)") == -1 ? name.Length : name.IndexOf("(Clone)");
        return name[startIndex..endIndex];
    }

    private int CapSquadChicken(string name)
    {
        int auxCap = 5;

        if (name.Contains("Bomb"))
            auxCap = 1;
        else if (name.Contains("Big"))
            auxCap = 3;
        else if (name.Contains("Shoot"))
            auxCap = 3;

        return auxCap;
    }

    /// <summary>
    /// Area para los limites de spawn del mapa, que calcula una posicion aleatoria dentro de esa area
    /// </summary>
    /// <returns>
    /// Un vector con una posicion aleatoria dentro de esa area
    /// </returns>
    public Vector3 GetRandomAreaSpawn()
    {
        return new Vector3(Random.Range(limiteXNegativo, LimiteXPositivo), 1.2f, Random.Range(limiteZNegativo, LimiteZPositivo));
    }

    // Metodo para obtener una posicion aleatoria del array
    private Vector3 GetRandomSpawnPosition()
    {
        // Posicion del jugador
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

        // Quitar la posicion mas cercana y la mas lejana, si hay mas de dos elementos
        if (spawnDistances.Count > 2)
        {
            spawnDistances.RemoveAt(0); // Eliminar la posicion mas cercana
            spawnDistances.RemoveAt(spawnDistances.Count - 1); // Eliminar la posicion mas lejana
        }

        // Elegir una posicion aleatoria entre las posiciones restantes
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
    public void Exit_pause_menu()
    {
        Time.timeScale = 1;
        paused = false;
        pausemenu.SetActive(false);
        highscore.SetActive(false);
        
    }

    public void Pause()
    {
        if (paused)
        {
            Exit_pause_menu();
            
        }

        if (paused == false)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            if (currentSceneName != "MenuPrincipal")
            {
                paused = true;
                pausemenu.SetActive(true);
                
                highscore.SetActive(false);
                Time.timeScale = 0;
                EventSystem.current.SetSelectedGameObject(firstGameObjectMenu);
            }
            /*if (currentSceneName == "MenuPrincipal")
            {
                //Debug.Log("Estas en la Escena principal");
            }*/
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back_to_Main_Menu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
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

    public void ChangeLevel()
    {
        level++;
        player.GetComponent<PlayerMovement>().enabled = false;
        player.transform.position = new Vector3(0, 5, 0);
    }

    //public void EndRound()
    //{
    //    nameInput.SetActive(true);
    //    //int finalScore = score;

    //    //string playerName = "PlayerDefault"; // Habrá que hacer que establezca su nombre aquí
    //    //highscoreTable.CheckAndAddHighscore(finalScore, playerName);
    //    //highscore.SetActive(true);

    //}

    public void FirstGameGameObjectMusic() 
    {
        EventSystem.current.SetSelectedGameObject(firstGameObjectMusic);

    }
    public void FirstGameGameObjectMusicToMenu()
    {
        EventSystem.current.SetSelectedGameObject(firstGameObjectMusicToMenu);

    }
    public void FirstGameGameObjectRanking()
    {
        EventSystem.current.SetSelectedGameObject(firstGameObjectRanking);

    }
    public void FirstGameGameObjectRestart()
    {
        EventSystem.current.SetSelectedGameObject(restartButtonFirstObject);

    }
    

    public void EndRound() // Se llama cuando el jugador muere
    {

        // Obtener la puntuación final
        int finalScore = score;

        // Verificar si la puntuación está en el Top 10
        bool isTop10 = highscoreTable.CheckIfTop10(finalScore);

        if (isTop10)
        {
            // Llamar al método PATATA si está en el Top 10
            nameInput.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstGameObjectRanking);
        }

        else
        {
            highscore.SetActive(true);
            restartMenu.gameObject.SetActive(true);

        }


    }

}

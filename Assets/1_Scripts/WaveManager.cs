using System.Collections.Generic;
using UnityEngine;

//public class WaveManager : ScriptableObject
/// <summary>
/// La clase `WaveManager` gestiona la creación y control de las oleadas de enemigos o eventos en un juego.
/// Utiliza el patrón Singleton para garantizar que solo haya una instancia activa de la clase durante la ejecución.
/// Las oleadas se dividen en niveles, y cada nivel tiene un número de rondas con una dificultad creciente.
/// </summary>
public class WaveManager : MonoBehaviour
{
    // ---- Control de oleadas ----
    public int level = 1;                                                               // Nivel  /  Mapa
    [SerializeField] int waveCurrent = 1;                                               // Oleada /  Ronda
    [SerializeField] int waveForLevel = 10;                                             // Total numero de rondas por nivel
    [SerializeField] int numTotalLevel = 3;                                             // Total numero de niveles
    [SerializeField] double incremento = 1.2;                                           // Multiplicador para el incremento entre rondas
    [SerializeField] double initialDifficult = 15;                                      // Puntuación inicial o dificultad inicial
    [SerializeField] Dictionary<int, int> waveDificulty = new Dictionary<int, int>();   // Dificultad segun oleadas
    [SerializeField] private List<GameObject> spawnPositions;                           // Lista de posiciones donde spawnearán los pollos
    [SerializeField] private ChickenConfig chickenConfig;                               // Configuración de los pollos (probabilidad y tipo)
    [SerializeField] private GameObject player;                                         // Configuración de los pollos (probabilidad y tipo)
    private int totalDifficultyPoints;                                                  // Puntuación de dificultad total por oleada

    public static WaveManager Instance { get; private set; }                            // Singleton

    private void Awake() // Singleton pattern
    {
        // Si hay una instancia y no es esta, destruyela.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start() // Cuando ya se haya inicializado todo
    {
        // Asegurarnos de que GameManager.Instance está inicializado antes de llamarlo
        getWaveDificulty();
    }

    private void LateUpdate()
    {
        level = GameManager.Instance.level;
        waveCurrent = GameManager.Instance.waveCurrent;
    }


    // Método para generar escuadrones de pollos
    public void GenerateChickenSquad()
    {
        int difficultyPointsLeft = waveDificulty[waveCurrent]; // Obtener dificultad de la oleada actual
        totalDifficultyPoints = difficultyPointsLeft; // Almacenar la puntuación total

        // Generar pollos hasta que quede el 25% de la dificultad total
        while (difficultyPointsLeft > totalDifficultyPoints * 0.25)
        {
            float randomNumber = Random.Range(0, 100);

            // Generar escuadrón o de pollos blancos o mixto / único
            if (randomNumber <= (120 - totalDifficultyPoints))
                SpawnWhiteChickenSquad(ref difficultyPointsLeft);
            else
                SpawnMixedSquad(ref difficultyPointsLeft);
        }

        // Resto de generación aleatoria
        while (difficultyPointsLeft > 0)
        {
            // Generación de pollos aleatoria con los puntos restantes
            SpawnRandomChicken(ref difficultyPointsLeft);
        }
    }

    // Método para generar pollos de manera aleatoria
    private void SpawnRandomChicken(ref int difficultyPointsLeft)
    {
        GameObject chickenPrefab = GameManager.Instance.prepareChikenGenerator();
        Instantiate(chickenPrefab, GameManager.Instance.getRandomAreaSpawn(), Quaternion.identity);
        GameManager.Instance.enemyCount++;
        difficultyPointsLeft--;
    }

    // Método para generar escuadrón de pollos blancos
    private void SpawnWhiteChickenSquad(ref int difficultyPointsLeft)
    {
        int chickensToSpawn = Mathf.Min(10, difficultyPointsLeft); // Si hay menos puntos, genera menos pollos
        for (int i = 0; i < chickensToSpawn; i++)
        {
            GameObject chickenPrefab = GameManager.Instance.prepareChikenGenerator();
            Instantiate(chickenPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            GameManager.Instance.enemyCount++;
        }
        difficultyPointsLeft -= chickensToSpawn;
    }

    // Método para generar un escuadrón mixto
    private void SpawnMixedSquad(ref int difficultyPointsLeft)
    {
        int chickensToSpawn = Mathf.Min(5, difficultyPointsLeft); // Si hay menos puntos, genera menos pollos
        for (int i = 0; i < chickensToSpawn; i++)
        {
            // Lógica para mezclar blancos y otros tipos de pollos
            GameObject chickenPrefab = GameManager.Instance.prepareChikenGenerator();
            Instantiate(chickenPrefab, GetRandomSpawnPosition(), Quaternion.identity);
            GameManager.Instance.enemyCount++;
        }
        difficultyPointsLeft -= chickensToSpawn; // Ajustar la puntuación según el escuadrón
    }

    // Método para obtener una posición aleatoria del array
    private Vector3 GetRandomSpawnPosition()
    {
        // Posición del jugador
        Vector3 playerPosition = player.transform.position;

        // Crear una lista de posiciones de spawn y su distancia al jugador
        List<(Vector3 position, float distance)> spawnDistances = new List<(Vector3, float)>();

        // Rellenar la lista con las posiciones de spawn y calcular sus distancias al jugador
        foreach (var spawnPosition in spawnPositions)
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

    /// <summary>
    /// Genera un diccionario con las dificultades para cada ronda del nivel actual, basado en un incremento multiplicativo.
    /// Este método toma el valor de dificultad inicial y lo aumenta según el multiplicador configurado en cada ronda.
    /// Al finalizar una parte (nivel), la dificultad de la siguiente parte comienza con la mitad del valor de la última ronda.
    /// Muestra las dificultades calculadas en la consola de Unity.
    /// </summary>
    /// <returns>
    /// Retorna un diccionario (`Dictionary<int, int>`) donde la clave es el número de ronda y el valor es la dificultad correspondiente.
    /// </returns>
    private Dictionary<int, int> getWaveDificulty()
    {
        level = GameManager.Instance.level;
        waveCurrent = GameManager.Instance.waveCurrent;
        double difficultBefore = initialDifficult;

        // Llenar el diccionario con rondas y puntuaciones
        for (int parte = 1; parte <= numTotalLevel; parte++)
        {
            for (int ronda = 1; ronda <= waveForLevel; ronda++)
            {
                waveDificulty[waveCurrent] = (int) difficultBefore;
                difficultBefore *= incremento; // Incremento multiplicativo entre rondas
                waveCurrent++;
            }

            // Al final de la parte, el inicio de la siguiente parte será la mitad
            difficultBefore /= 2;
        }

        // Mostrar el resultado en consola
        foreach (var wave in waveDificulty)
        {
            Debug.Log($"Ronda {wave.Key} -- {wave.Value} puntos");
        }

        return waveDificulty;
    }

    public int GetDifficultyPointsByWave(int waveCurrent)
    {
        return waveDificulty[waveCurrent];
    }

}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// La clase `WaveManager` gestiona la creación y control de las oleadas de enemigos o eventos en un juego.
/// Utiliza el patrón Singleton para garantizar que solo haya una instancia activa de la clase durante la ejecución.
/// Las oleadas se dividen en niveles, y cada nivel tiene un número de rondas con una dificultad creciente.
/// Al cambiar de ronda llama a MusicManager.Instance.FX_ActivarCorutina() una sola vez.
/// </summary>
public class WaveManager : MonoBehaviour
{
    // ---- Control de oleadas ----
    public int level = 1;                                                   // Nivel / Mapa
    [SerializeField] int waveCurrent = 1;                                   // Oleada / Ronda
    public int waveForLevel = 10;                                           // Total número de rondas por nivel
    [SerializeField] int numTotalLevel = 3;                                 // Total número de niveles
    [SerializeField] double incremento = 1.15;                              // Multiplicador para el incremento entre rondas
    [SerializeField] double initialDifficult = 0;                           // Puntuación inicial o dificultad inicial
    [SerializeField] Dictionary<int, int> waveDificulty = new Dictionary<int, int>();
    [SerializeField] private List<GameObject> spawnPositions;                // Posiciones de spawn
    [SerializeField] private ChickenConfig chickenConfig;                    // Configuración de pollos
    private int totalDifficultyPoints;                                       // Puntos totales de dificultad por oleada

    private int _lastWaveNotified;                                           // Para evitar dobles invocaciones

    public static WaveManager Instance { get; private set; }                 // Singleton

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Inicializamos dependencia de GameManager en Start, cuando ya existe
        initialDifficult = 20;
        SetWaveDificulty();

        spawnPositions = GameManager.Instance.listSpawns;
        _lastWaveNotified = GameManager.Instance.waveCurrent;
    }

    private void Update()
    {
        // Sincronizamos nivel y ola actual desde GameManager
        level = GameManager.Instance.level;
        waveCurrent = GameManager.Instance.waveCurrent;

        // Si ha cambiado la ola desde la última vez que lo notificamos...
        if (waveCurrent != _lastWaveNotified)
        {
            _lastWaveNotified = waveCurrent;
            MusicManager.Instance.FX_ActivarCorutina();
        }
    }

    // --------------------------------------------------
    // Resto de métodos (GetRandomSpawnPosition, SetWaveDificulty, GetDifficultyPointsByWave, etc.)
    // --------------------------------------------------

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 playerPosition = GameManager.Instance.player.transform.position;
        List<(Vector3 position, float distance)> spawnDistances = new List<(Vector3, float)>();

        foreach (var spawnPosition in spawnPositions)
        {
            float distance = Vector3.Distance(playerPosition, spawnPosition.transform.position);
            spawnDistances.Add((spawnPosition.transform.position, distance));
        }

        spawnDistances.Sort((a, b) => a.distance.CompareTo(b.distance));

        if (spawnDistances.Count > 2)
        {
            spawnDistances.RemoveAt(0);
            spawnDistances.RemoveAt(spawnDistances.Count - 1);
        }

        int randomIndex = Random.Range(0, spawnDistances.Count);
        return spawnDistances[randomIndex].position;
    }

    private Dictionary<int, int> SetWaveDificulty()
    {
        level = GameManager.Instance.level;
        waveCurrent = GameManager.Instance.waveCurrent;
        double difficultBefore = initialDifficult;
        int currentWaveIndex = waveCurrent;

        for (int parte = 1; parte <= numTotalLevel; parte++)
        {
            for (int ronda = 1; ronda <= waveForLevel; ronda++)
            {
                waveDificulty[currentWaveIndex] = (int)difficultBefore;
                difficultBefore *= incremento;
                currentWaveIndex++;

                if (currentWaveIndex > 10)
                    incremento = 1.05;
                if (currentWaveIndex > 20)
                    incremento = 1.025;
            }
            difficultBefore /= 2;
        }

        return waveDificulty;
    }

    public int GetDifficultyPointsByWave(int level, int waveCurrent)
    {
        int adjustedWave = waveCurrent;
        if (level == 2) adjustedWave += waveForLevel;
        else if (level == 3) adjustedWave += waveForLevel * 2;
        return waveDificulty.ContainsKey(adjustedWave) ? waveDificulty[adjustedWave] : 0;
    }
}

using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] public int level = 1;                                              // Nivel/Mapa
    [SerializeField] int waveForLevel = 30;                                             // Total numero de rondas por nivel
    [SerializeField] int numTotalLevel = 3;                                             // Total numero de niveles
    [SerializeField] double incremento = 1.2;                                           // Multiplicador para el incremento entre rondas
    [SerializeField] double initialDifficult = 15;                                      // Puntuación inicial o dificultad inicial
    [SerializeField] Dictionary<int, int> waveDificulty = new Dictionary<int, int>();   // Dificultad segun oleadas
    [SerializeField] int waveCurrent = 1;

    // Singleton
    public static WaveManager Instance { get; private set; }

    // Singleton pattern
    private void Awake()
    {
        // Si hay una instancia y no es esta, destruyela.
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void LateUpdate()
    {
        level = GameManager.Instance.level;
        waveCurrent = GameManager.Instance.waveCurrent;
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
    public Dictionary<int, int> getWaveDificulty()
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


}

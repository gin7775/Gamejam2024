using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChickenSpawnService
{
    private readonly List<ChickenConfig> chikenToSpawn;
    private readonly List<ChickenConfigWave> chikenToSpawnWave;

    public ChickenSpawnService(List<ChickenConfig> chikenToSpawn, List<ChickenConfigWave> chikenToSpawnWave)
    {
        this.chikenToSpawn = chikenToSpawn;
        this.chikenToSpawnWave = chikenToSpawnWave;
    }

    public ChickenSpawnService()
    {
        this.chikenToSpawn = new List<ChickenConfig>();
        this.chikenToSpawnWave = new List<ChickenConfigWave>();
    }

    public ChickenConfig SelectChickenBasedOnProb(int currentLevel, int currentWave)
    {
        ChickenConfig selectedPollo = null;

        // Generar un Número aleatorio para determinar el pollo a generar
        float randomNumber = Random.Range(0, 100);

        // Filtrar la lista de pollos que pueden aparecer según el nivel y la oleada
        List<ChickenConfig> validChickens = chikenToSpawn.Where( chickenConfig => IsChickenValidForLevelAndWave(chickenConfig.chickenPrefab.name, currentLevel, currentWave) ).ToList();

        // Ajusta las probabilidades de los pollos existentes a que sumen 100%
        GameManager.Instance.AdjustProbabilities(ref validChickens);

        // Selecciona uno basado en las probabilidades acumuladas
        foreach (ChickenConfig chickenConfig in validChickens)
        {
            Debug.Log(chickenConfig.probability);
            if (randomNumber < chickenConfig.probability)
            {
                selectedPollo = chickenConfig;
                break; // Salir del bucle una vez que el pollo es seleccionado
            }

            // Restar la probabilidad actual para la próxima comparación
            randomNumber -= chickenConfig.probability;
        }

        return selectedPollo; // Si no se selecciona ningún pollo, devolver null
    }

    public ChickenConfig SelectChicken(string enemyType)
    {
        ChickenConfig selectedPollo = null;

        foreach (ChickenConfig auxChicken in chikenToSpawn)
        {
            if (auxChicken.chickenPrefab.name.Contains(enemyType))
            {
                selectedPollo = auxChicken;
                break; // Salir del bucle una vez que el pollo es seleccionado
            }
        }

        return selectedPollo; // Si no se selecciona ningún pollo, devolver null
    }

    // Método auxiliar para verificar si un pollo es válido según el nivel y la oleada
    private bool IsChickenValidForLevelAndWave(string enemyType, int level, int wave)
    {
        foreach (ChickenConfigWave chickenConfigWave in chikenToSpawnWave)
        {
            if (chickenConfigWave.chickenPrefab.name.Contains(enemyType))
            {
                // Verificar las condiciones de nivel
                if (!IsLevelConditionMet(chickenConfigWave.level, level))
                    continue;

                // Verificar las condiciones de oleada
                if (!IsWaveConditionMet(chickenConfigWave.wave, wave))
                    continue;

                // Verificar la cantidad si es necesario (puedes añadir esta lógica)
                // if (chickenConfigWave.cantidad <= someLimit) // Añadir lógica si hay límite
                //    continue;

                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Método para verificar las condiciones de nivel:<br/>
    /// <br/>
    /// <code>
    /// {
    ///       - 0         : Aparece en todos los niveles<br/>
    ///       - 1/2/3     : Aparece solo en ese nivel<br/>
    ///       - 1/2/3 + 0 : Aparece apartir de ese nivel<br/>
    ///       - XX    + 0 : Aparece en los niveles indicados -> 120, 130, 230
    /// }
    /// </code>
    /// </summary>
    /// <param name="configLevel"></param>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    private bool IsLevelConditionMet(double configLevel, int currentLevel)
    {
        return configLevel switch
        {
            0 => true,                                                                                         // Aparece en todos los niveles
            double n when n == currentLevel => true,                                                           // Aparece solo en este nivel
            double n when n / 10 <= currentLevel && n % 10 == 0 => true,                                       // Aparece a partir de este nivel si el segundo dígito es 0
            double n when n > 100 && (n / 100 == currentLevel || (n / 10) % 10 == currentLevel) => true,       // Aparece si currentLevel es uno de los dos primeros dígitos
            _ => false,
        };
    }

    /// <summary>
    /// Método para verificar las condiciones de oleada:<br/>
    /// <br/>
    /// <code>
    /// {
    /// - 0           : Aparece en cualquier oleada
    /// - XX          : Solo aparece en esa oleada --> 01, 02, 10, 20 ... (Por encima de la 10 solo sería en las oleadas infinitas)
    /// - 1XX         : Aparece a partir de esa oleada --> 01, 02, 10, 20   
    /// - ...XXYYZZ.F : Cada agrupación de 2 determina las rondas que se controlan.
    /// .             : La F es una flag de 1 u 2, que controla que en esas rondas sale o no sale el enemigo.
    /// }
    /// </code>
    /// </summary>
    /// <param name="configWave"></param>
    /// <param name="currentWave"></param>
    /// <returns></returns>
    private bool IsWaveConditionMet(double configWave, int currentWave)
    {
        if (configWave == 0)
            return true; // Aparece en cualquier oleada

        if (configWave == currentWave)
            return true; // Aparece solo en esa oleada

        if (configWave / 100 == 1 && configWave % 100 <= currentWave)
            return true; // Aparece a partir de esta oleada

        // Verificación avanzada basada en el patrón XXYYZZ.F
        string wavePattern = configWave.ToString();
        Debug.Log(wavePattern);

        if (wavePattern != "0" || wavePattern != "")
            for (int i = 0; i < wavePattern.Length - 2; i += 2)
            {
                Debug.Log($"{i} -- {wavePattern} -- {wavePattern.Length}");
                int waveGroup = int.Parse(wavePattern.Substring(i, 2));
                char flag = wavePattern.Last();

                if (waveGroup == currentWave && flag == '1')
                    return true;
            }

        return false;
    }

    public int GetCapGenerator(int wave)
    {
        // Determina el valor de capGenerator basado en la wave
        return wave switch
        {
            <= 4 => 20,
            >= 5 and <= 7 => 50,
            >= 8 and <= 10 => 100,
            _ => 150
        };
    }

}

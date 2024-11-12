using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChickenConfig
{
    public GameObject chickenPrefab;    // El tipo de enemigo
    public float probability;           // La probabilidad de que aparezca este enemigo
    public int difficultyScore;         // Puntuación de dificultad del pollo (1, 5, 10, etc.)
}

[System.Serializable]
public class ChickenConfigWave
{
    public GameObject chickenPrefab;    // El tipo de enemigo
    public double wave;                 // Oleada a la que se aplica
    public double level;                // Mapa al que se aplica
    public int quantity;                // Se limita la cantidad total en ronda
    public int quantityAlive;           // Se limita la cantidad instanciados
}

[System.Serializable]
public class ChickenDifficult
{
    public GameObject chickenPrefab;    // El tipo de enemigo
    public int difficultyScore;         // Puntuación de dificultad del pollo (1, 5, 10, etc.)
}

[System.Serializable]
public class ChickenCorpseTimeLife
{
    public GameObject corpse;           // El tipo de enemigo
    public float secondsLife;           // Tiempo de vida en segundos

    // Constructor
    public ChickenCorpseTimeLife(GameObject corpse, float secondsLife)
    {
        this.corpse = corpse;
        this.secondsLife = secondsLife;
    }
}

[System.Serializable]
public class ChickenCount
{
    public string typeName;             // El tipo de enemigo
    public int quantity = 0;            // Cantidad de pollos

    // Constructor
    public ChickenCount(string typeName)
    {
        this.typeName = typeName;
        this.quantity++;
    }
}

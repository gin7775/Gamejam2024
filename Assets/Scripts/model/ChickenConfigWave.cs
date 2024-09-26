using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChickenConfigWave
{
    public int wave;                    // Oleada a la que se aplica
    public int level;                   // Mapa al que se aplica
    public GameObject chickenPrefab;    // El tipo de enemigo
    public float probability;           // La probabilidad de que aparezca este enemigo
    public int difficultyScore;         // Puntuaci�n de dificultad del pollo (1, 5, 10, etc.)
}

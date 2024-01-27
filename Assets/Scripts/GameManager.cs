using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public int score = 0;
    [SerializeField] public int currentWave = 0;
    [SerializeField] public int waveNumber = 3;
    [SerializeField] public int dificultiLevel = 1;

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
    }

    public void chikenEnemyDeath(GameObject enemy)
    {
        score++;
        SpawnPollos_rik.Instance.enemyDeath();
    }

    public void chickenEnemyTakeDamage(GameObject enemy, int damage)
    {
        int auxLife = enemy.GetComponent<ContenedorEnemigo1>().lifes -= damage;

        if (auxLife <= 0)
        {
            score++;
            Destroy(enemy);
            SpawnPollos_rik.Instance.enemyDeath();
        }
    }

    /*public int getCurrentWave()
    {
        return currentWave;
    }

    public void setCurrentWave(int currentWave)
    {
        this.currentWave = currentWave;
    }

    public int getScore()
    {
        return score;
    }

    public void setScore(int score)
    {
        this.score = score;
    }*/

}

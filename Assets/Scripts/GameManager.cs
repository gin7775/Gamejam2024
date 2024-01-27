using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public int score;
    [SerializeField] public int currentWave;
    [SerializeField] public int waveNumber;
    [SerializeField] public int dificultiLevel;

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
        score = 0;
        currentWave = 1;
        waveNumber = 3;
        dificultiLevel = 1;
    }

    public void chikenEnemyDeath(GameObject enemy)
    {
        score++;
        Destroy(enemy);
        SpawnPollos_rik.Instance.enemyDeath();
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

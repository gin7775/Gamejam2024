using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPollos : MonoBehaviour
{
    public GameObject [] pollosToSpawn;
    public Vector3 spawnPosition;
    public int randomIterastor;
    public int enemyNumber,enemyCount;
    public int waveNumber,currentWave,dificultiLevel;
    


    // Start is called before the first frame update
    void Start()
    {
        randomIterastor = Random.Range(0, pollosToSpawn.Length);
        UpdateWave();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InstantiatePollos(int enemys)
    {
        enemyCount = enemys;
        for (int i = 0;i< enemys;i++ )
        {
            randomIterastor = Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(Random.Range(8.5f,-8.5f ), 1.2f, Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
        }
       

    }
    public void UpdateWave()
    {
        if (currentWave <= waveNumber)
        {
            if (currentWave <= waveNumber/3)
            {
                dificultiLevel = 1;
                InstantiatePollos(enemyNumber);
            }
            else if (currentWave <= waveNumber / 2)
            {
                dificultiLevel=2;
                InstantiatePollos(enemyNumber*2);
            }
            else 
            {
                dificultiLevel = 2;
                InstantiatePollos(enemyNumber * 3);

            }
            currentWave++;
            


        }
    }
    public void enemyDeath()
    {
        enemyCount--;
        if(enemyCount <= 0)
        {
            UpdateWave();
        }
    }
}

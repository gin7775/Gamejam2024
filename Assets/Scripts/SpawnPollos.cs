using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPollos : MonoBehaviour
{
    public GameObject [] pollosToSpawn;
    public Vector3 spawnPosition;
    public int randomIterastor;
    public int enemyNumber;


    // Start is called before the first frame update
    void Start()
    {
        randomIterastor = Random.RandomRange(0, pollosToSpawn.Length);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void InstantiatePollos()
    {
        for (int i = 0;i< enemyNumber;i++ )
        {
            randomIterastor = Random.Range(0, pollosToSpawn.Length);
            spawnPosition = new Vector3(Random.Range(8.5f,-8.5f ), 0, Random.Range(8.5f, -8.5f));
            GameObject toInstantiate = Instantiate(pollosToSpawn[randomIterastor], spawnPosition, Quaternion.identity);
        }
       

    }
}

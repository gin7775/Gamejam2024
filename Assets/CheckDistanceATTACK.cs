using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckDistanceATTACK : MonoBehaviour
{
    public float distanceToAttack = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckDistance()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= distanceToAttack)
            {
                Debug.Log("He atacado");
                player.GetComponent<ChickenLouncher>().ReciveDamage(1);

            }
        }
    }
}

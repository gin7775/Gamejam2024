using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankLaser : MonoBehaviour
{

    public int damageToPlayer = 10;
    public int damageToEnemies = 5;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().ReceiveDamage(damageToPlayer);
        }

        else if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.ChickenEnemyTakeDamage(other.gameObject, damageToEnemies);
        }

    }
}

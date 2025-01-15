using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBullet : MonoBehaviour
{
    public float projectileLifetime = 5f;

    public int damageToPlayer = 1;
    public int damageToEnemies = 1;

    void Start()
    {
        // Destruir el proyectil después de X segundos
        Destroy(this, projectileLifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>().ReceiveDamage(damageToPlayer);            
        }

        else if(other.CompareTag("Enemy"))
        {
            GameManager.Instance.ChickenEnemyTakeDamage(other.gameObject, damageToEnemies);
        }

    }
}

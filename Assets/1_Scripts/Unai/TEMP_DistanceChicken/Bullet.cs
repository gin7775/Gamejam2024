using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damageAmount = 1;
    public float projectileLifetime = 5f;
    void Start()
    {
        // Destruir el proyectil después de X segundos
        Destroy(this.gameObject, projectileLifetime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.ReceiveDamage(damageAmount);
            Destroy(this.gameObject);
        }
        
    }
}

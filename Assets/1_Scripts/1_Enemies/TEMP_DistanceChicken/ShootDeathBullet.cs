using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootDeathBullet : MonoBehaviour
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
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Kill");
            GameManager.Instance.ChickenEnemyTakeDamage(other.gameObject, 1);
            Destroy(this.gameObject);
        }

    }
}

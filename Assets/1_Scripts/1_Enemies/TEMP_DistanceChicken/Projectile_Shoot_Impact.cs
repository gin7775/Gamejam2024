using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Shoot_Impact : MonoBehaviour
{
    public GameObject bulletPrefab;  // Prefab de la bala
    public int bulletCount = 8;      // Número de balas generadas en el impacto
    public float bulletSpeed = 5f;   // Velocidad de las balas

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto impactado tiene una etiqueta específica (opcional)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpawnBulletsInCircle();
            Destroy(this.gameObject);
        }
    }

    void SpawnBulletsInCircle()
    {
        // Ángulo inicial y diferencia de ángulo entre cada bala
        float angleStep = 360f / bulletCount; // Divide 360 grados entre el número de balas

        for (int i = 0; i < bulletCount; i++)
        {
            // Calcular el ángulo para la bala actual
            float angle = i * angleStep;

            // Convertir el ángulo a radianes para calcular la posición y dirección de la bala
            float radian = angle * Mathf.Deg2Rad;

            // Calcular la dirección de la bala en el plano X-Z
            Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));

            // Instanciar la bala en la posición actual
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Obtener el Rigidbody de la bala y aplicar la fuerza en la dirección calculada
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar la velocidad a la bala en la dirección calculada
                rb.velocity = direction * bulletSpeed;
            }
        }
    }

}

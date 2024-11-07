using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Shoot_Impact : MonoBehaviour
{
    public GameObject bulletPrefab;     // Prefab de la bala
    public int bulletCount = 8;         // N�mero de balas generadas en el abanico
    public float bulletSpeed = 5f;      // Velocidad de las balas
    public float fanAngle = 90f;       // �ngulo total del abanico

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto impactado tiene una etiqueta espec�fica (opcional)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpawnBulletsInFan();
            Destroy(this.gameObject);
        }
    }

   
    void SpawnBulletsInFan()
    {
        // �ngulo inicial y diferencia de �ngulo entre cada bala
        float startAngle = -fanAngle / 2;         // Empezar el abanico desde el lado izquierdo (-105 grados)
        float angleStep = fanAngle / (bulletCount - 1); // Espacio entre cada bala en el abanico

        for (int i = 0; i < bulletCount; i++)
        {
            // Calcular el �ngulo para la bala actual dentro del abanico
            float angle = startAngle + (i * angleStep);

            // Convertir el �ngulo a radianes y calcular la direcci�n en el plano X-Z
            float radian = angle * Mathf.Deg2Rad;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Instanciar la bala en la posici�n actual
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Obtener el Rigidbody de la bala y aplicar la velocidad en la direcci�n calculada
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar la velocidad a la bala en la direcci�n calculada
                rb.velocity = direction * bulletSpeed;
            }
        }
    }


}

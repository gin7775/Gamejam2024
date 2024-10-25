using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Shoot_Impact : MonoBehaviour
{
    public GameObject bulletPrefab;  // Prefab de la bala
    public int bulletCount = 8;      // N�mero de balas generadas en el impacto
    public float bulletSpeed = 5f;   // Velocidad de las balas

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto impactado tiene una etiqueta espec�fica (opcional)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpawnBulletsInCircle();
            Destroy(this.gameObject);
        }
    }

    void SpawnBulletsInCircle()
    {
        // �ngulo inicial y diferencia de �ngulo entre cada bala
        float angleStep = 360f / bulletCount; // Divide 360 grados entre el n�mero de balas

        for (int i = 0; i < bulletCount; i++)
        {
            // Calcular el �ngulo para la bala actual
            float angle = i * angleStep;

            // Convertir el �ngulo a radianes para calcular la posici�n y direcci�n de la bala
            float radian = angle * Mathf.Deg2Rad;

            // Calcular la direcci�n de la bala en el plano X-Z
            Vector3 direction = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian));

            // Instanciar la bala en la posici�n actual
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Obtener el Rigidbody de la bala y aplicar la fuerza en la direcci�n calculada
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Aplicar la velocidad a la bala en la direcci�n calculada
                rb.velocity = direction * bulletSpeed;
            }
        }
    }

}

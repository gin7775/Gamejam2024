using UnityEngine;

public class WaterCurrent : MonoBehaviour
{
    [Header("Ajustes de la Corriente")]
    public Vector3 currentDirection = Vector3.forward; // Dirección de la corriente
    public float currentForce = 2f; // Intensidad de la corriente

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyWaterCurrent(currentDirection, currentForce);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            ContenedorEnemigo1 enemyContainer = other.GetComponent<ContenedorEnemigo1>();
            if (enemyContainer != null)
            {
                enemyContainer.ApplyWaterCurrent(currentDirection, currentForce);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ApplyWaterCurrent(currentDirection, currentForce);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            ContenedorEnemigo1 enemyContainer = other.GetComponent<ContenedorEnemigo1>();
            if (enemyContainer != null)
            {
                enemyContainer.ApplyWaterCurrent(currentDirection, currentForce);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.RemoveWaterCurrent();
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            ContenedorEnemigo1 enemyContainer = other.GetComponent<ContenedorEnemigo1>();
            if (enemyContainer != null)
            {
                enemyContainer.RemoveWaterCurrent();
            }
        }
    }
}

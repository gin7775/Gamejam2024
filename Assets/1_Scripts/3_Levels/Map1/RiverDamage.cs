using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverDamage : MonoBehaviour
{
    private Coroutine damageCoroutine;
    public float damageInterval = 3f; // Intervalo de tiempo entre daños (en segundos)
    public int damageAmount = 1; // Daño que se inflige inicialmente


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (damageCoroutine == null) // Evitar múltiples corrutinas activas
                {
                    damageCoroutine = StartCoroutine(ApplyDamageOverTime(playerHealth));
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null; // Resetear referencia
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(PlayerHealth playerHealth)
    {
        while (true) // Bucle infinito controlado por OnCollisionExit
        {
            playerHealth.ReciveDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("En contacto");
        if (hit.gameObject.CompareTag("Player"))
        {

        }
    }



}

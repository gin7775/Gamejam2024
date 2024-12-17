using System.Collections;
using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    [SerializeField]
    public int damageAmount = 10; // Daño que se inflige inicialmente

    [SerializeField]
    public float damageInterval = 1f; // Intervalo de tiempo entre daños (en segundos)

    private Coroutine damageCoroutine;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("En contacto");
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
            playerHealth.ReceiveDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}

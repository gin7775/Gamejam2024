using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollo_Bomba : MonoBehaviour
{
    public List<GameObject> chickensToDie;
    private GameObject player;
    public float timer = 2f; // Tiempo de cuenta atrás
    public float radius = 5f; // Radio para iniciar la explosión
    [SerializeField] private float activationDistance = 10f; // Distancia mínima para activar la cuenta atrás
    private bool isCodeExecuting = false;
    private bool countdownStarted = false;
    [SerializeField] Animator tickingLightAnimator;

    [SerializeField] private EnemyAudio enemyAudio;

    void OnDrawGizmos()
    {
        // Establece el color del círculo de depuración
        Gizmos.color = Color.blue;

        // Dibuja un círculo en la posición del objeto, representando la activationDistance
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }

    void Start()
    {
        timer = 2f;
        activationDistance = 10f;
        // Asignamos la referencia al jugador
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (player != null && !countdownStarted)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Si el jugador está dentro de la distancia de activación y la cuenta atrás no ha comenzado, comenzamos el Countdown
            if (distanceToPlayer <= activationDistance)
            {
                
                StartCoroutine(CountdownAndExplode());
                countdownStarted = true;
            }
        }
    }

    // Corrutina para contar hacia atrás y realizar la explosión
    IEnumerator CountdownAndExplode()
    {
        enemyAudio.Play_Enemy_FX_TickingPollo();
        tickingLightAnimator.SetTrigger("PlayerDetected");
        
        //while (timer > 0f)
        //{
        //    timer -= Time.deltaTime;
        //    yield return null; // Esperar hasta el siguiente frame
        //}

       // yield return new WaitForSeconds(timer);
        yield return new WaitForSeconds(enemyAudio.tickingPollo.clip.length);

        Explosion();
    }

    public void Explosion()
    {
        // Instancia partículas
        GetComponent<SpawnParticles>().SpawnBothParticles();

        if (!isCodeExecuting)
        {
            isCodeExecuting = true;

            chickensToDie = GameManager.Instance.listEnemies;

            foreach (GameObject chicken in chickensToDie)
            {
                if (chicken == null) continue; // Verifica si el objeto aún existe
                if (Vector3.Distance(transform.position, chicken.transform.position) <= radius)
                {
                    GameManager.Instance.ChickenEnemyTakeDamage(chicken, 99);
                }
            }

            isCodeExecuting = false;
        }

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= radius)
        {
            player.GetComponent<PlayerHealth>().ReceiveDamage(1);
        }

        StartCoroutine(ForceToChickens());
    }

    IEnumerator ForceToChickens()
    {
        yield return new WaitForSeconds(0.5f);

        List<GameObject> listaChickens = GameManager.Instance.listCorpses;

        foreach (GameObject chicken in listaChickens)
        {
            if (chicken == null) continue; // Verifica si el objeto aún existe

            float dist = Vector3.Distance(transform.position, chicken.transform.position);
            if (dist <= 100)
            {
                Collider[] colliders = chicken.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    if (col != null) col.enabled = false; // Deshabilitar colliders temporalmente
                }

                Rigidbody[] rigidbodies = chicken.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in rigidbodies)
                {
                    if (rb != null)
                    {
                        rb.AddExplosionForce(25.0f, transform.position, 10, 25.0f);
                    }
                }

                yield return new WaitForSeconds(1f); // Espera un segundo antes de volver a activar los colliders

                foreach (Collider col in colliders)
                {
                    if (col != null) col.enabled = true; // Habilitar colliders nuevamente
                }
            }
        }
    }
}

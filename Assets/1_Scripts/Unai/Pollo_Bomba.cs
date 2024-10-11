using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollo_Bomba : MonoBehaviour
{
    public float timer = 5f;
    public List<GameObject> chickensToDie;
    public float radius = 5f;
    private GameObject player;
    private bool isCodeExecuting = false;

    void Start()
    {
        StartCoroutine(CountdownAndExplode());
        player = GameManager.Instance.player;
    }

    // Corrutina para contar hacia atrás y realizar la explosión
    IEnumerator CountdownAndExplode()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null; // Esperar hasta el siguiente frame
        }

        Explosion();
        //Destroy(gameObject);
        //GameManager.Instance.enemyDeath();
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
                    GameManager.Instance.chickenEnemyTakeDamage(chicken, 99);
                }
            }

            isCodeExecuting = false;
        }

        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= radius)
        {
            player.GetComponent<PlayerHealth>().ReciveDamage(1);
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
                Rigidbody[] rigidbodies = chicken.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody rb in rigidbodies)
                {
                    if (rb != null)
                    {
                        rb.AddExplosionForce(25.0f, transform.position, 10, 25.0f);
                    }
                }

                Collider[] colliders = chicken.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    if (col != null) col.enabled = false; // Deshabilitar colliders temporalmente
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

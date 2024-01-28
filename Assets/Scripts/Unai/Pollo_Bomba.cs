using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollo_Bomba : MonoBehaviour
{

    public float timer = 5f;
    public GameObject[] chickensToDie;
    public float radius = 5f;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountdownAndExplode());
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Corrutina para contar hacia atrás y realizar la explosión
    IEnumerator CountdownAndExplode()
    {
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            // Puedes imprimir mensajes de depuración en intervalos específicos, por ejemplo:
            if (timer <= 3f && timer > 2f)
            {
                //Debug.Log("Menos de 3 segundos");
            }
            else if (timer <= 2f && timer > 1f)
            {
                //Debug.Log("Menos de 2 segundos");
            }
            else if (timer <= 1f)
            {
                //.Log("Menos de 1 segundo");
            }

            // Esperar hasta el siguiente frame
            yield return null;
        }

        // Cuando el temporizador llega a cero
        Explosion();
        Destroy(this.gameObject);
    }

    public void Explosion()
    {
        //Instancia particulas
        GetComponent<SpawnParticles>().SpawnBothParticles();
        chickensToDie = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject chicken in chickensToDie)
        {
            if (Vector3.Distance(transform.position, chicken.transform.position) <= radius)
            {
                /*HowToDie howToDie = chicken.GetComponent<HowToDie>();
                if (howToDie != null)
                {*/
                    //   howToDie.he_explotado(); // matar pollos, INCLUIDO JUGADOR SI COINCIDE QUE ESTÁ EN EL RADIO

                    Debug.Log("He muerto en una explosión jo qué pena, no sabía que acabaría tan inesperadamente. La vida es tan corta, tan rápida... quiero seguir viviendo, no quiero morir...");
                    GameManager.Instance.chickenEnemyTakeDamage(chicken, 99);
                //}
            }
        }

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= radius)
            {
                player.GetComponent<ChickenLouncher>().ReciveDamage(1);
                Debug.Log("Explosion ha herido al jugador");
            }
        }
        

    }
}



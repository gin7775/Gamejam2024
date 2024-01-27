using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pollo_Bomba : MonoBehaviour
{

    public float timer = 5f;
    public GameObject[] chickensToDie;
    public float radius = 5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountdownAndExplode());
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

        chickensToDie = GameObject.FindGameObjectsWithTag("CHICKEN");

            foreach (GameObject chicken in chickensToDie)
            {
                if (Vector3.Distance(transform.position, chicken.transform.position) <= radius)
                {
                    Prueba_explosion hemuerto_script = chicken.GetComponent<Prueba_explosion>();
                    if (hemuerto_script != null)
                    {
                        hemuerto_script.he_explotado(); // matar pollos, INCLUIDO JUGADOR SI COINCIDE QUE ESTÁ EN EL RADIO
                    }

                }

            }
    }
}



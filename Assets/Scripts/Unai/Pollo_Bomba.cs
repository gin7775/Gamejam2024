using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pollo_Bomba : MonoBehaviour
{

    public float timer = 5f;
    public GameObject[] chickensToDie;
    public float radius = 5f;
    public GameObject player;
    private bool isCodeExecuting = false;


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
                //Debug.Log("Menos de 1 segundo");
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
        if (!isCodeExecuting)
        {
            isCodeExecuting = true;

            chickensToDie = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject chicken in chickensToDie)
            {
                if (Vector3.Distance(transform.position, chicken.transform.position) <= radius)
                {
                    GameManager.Instance.chickenEnemyTakeDamage(chicken, 99);

                    
                }
            }

            isCodeExecuting = false;
        }

        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= radius)
            {
                player.GetComponent<ChickenLouncher>().ReciveDamage(1);
            }
        }
        Debug.Log("cickens a volar 1");
        // array para mandar a volar a los pollos
        StartCoroutine(ForceToChickens());
        Debug.Log("cickens a volar 2");
    }

    IEnumerator ForceToChickens()
    {
        Debug.Log("cickens corrutina");
        new WaitForSeconds(0.5f);
         List<GameObject> listaChickens = new List<GameObject>(GameObject.FindGameObjectsWithTag("Corpse"));
        Debug.Log("chicken lista es: " + listaChickens.Count);

        for (int i = 0; i < listaChickens.Count; i++)
        {

            float dist = Vector3.Distance(this.gameObject.transform.position, listaChickens[i].transform.position);
            if (dist <= 100)
            {
               GameObject bodyOne = listaChickens[i].gameObject.transform.GetChild(0).transform.GetChild(0).gameObject;
               GameObject bodyTwo = bodyOne.gameObject.transform.GetChild(0).gameObject;
               GameObject bodyNeck = bodyTwo.gameObject.transform.GetChild(2).gameObject;
               GameObject bodyHead = bodyNeck.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;



                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 100);

                foreach(Collider collider in colliders)
                {
                    bodyOne.GetComponent<CapsuleCollider>().enabled = false;
                    bodyTwo.GetComponent<CapsuleCollider>().enabled = false;
                    bodyNeck.GetComponent<CapsuleCollider>().enabled = false;
                    bodyHead.GetComponent<CapsuleCollider>().enabled = false;

                    bodyOne.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);
                    bodyTwo.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);
                    bodyNeck.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);
                    bodyHead.GetComponent<Rigidbody>().AddExplosionForce(25.0f, this.gameObject.transform.position, 10, 25.0f);

                    bodyOne.GetComponent<CapsuleCollider>().enabled = true;
                    bodyTwo.GetComponent<CapsuleCollider>().enabled = true;
                    bodyNeck.GetComponent<CapsuleCollider>().enabled = true;
                    bodyHead.GetComponent<CapsuleCollider>().enabled = true;
                }

            }
        }


        yield return null;
    }
}



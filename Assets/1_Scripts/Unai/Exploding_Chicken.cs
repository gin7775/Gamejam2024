using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exploding_Chicken : MonoBehaviour
{
    public bool dead;
    public bool thrown;
    public bool exploding_chicken = true;

    public float radius = 5f;

    public GameObject[] chickens_to_die;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (exploding_chicken && dead && thrown)
        {
            Debug.Log("CONTACTO");
            //this.gameObject.transform.localScale *= 3; //el radio...

            chickens_to_die = GameObject.FindGameObjectsWithTag("CHICKEN");

            foreach (GameObject chicken in chickens_to_die)
            {
                if (Vector3.Distance(transform.position, chicken.transform.position) <= radius)
                {
                    Prueba_explosion hemuerto_script = chicken.GetComponent<Prueba_explosion>();
                    if (hemuerto_script != null)
                    {
                        hemuerto_script.he_explotado(); // 
                    }

                }

            }
        }


    }
}
